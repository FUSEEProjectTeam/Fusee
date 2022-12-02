using Fusee.Base.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Picker for octants.
    /// </summary>
    public class OctantPicker
    {
        private readonly PointCloudOctree _octree;
        private readonly RenderContext _rc;
        public Camera Cam;
        public float3 CamPosWorld;

        /// <summary>
        /// Constructor for the octant picker.
        /// </summary>
        /// <param name="octree">The octree to pick from.</param>
        /// <param name="rc">The render context.</param>
        /// <param name="cam">The camera the calculation is based on.</param>
        /// <param name="camTransform">The transform of previously given camera.</param>
        public OctantPicker(PointCloudOctree octree, RenderContext rc, Camera cam, float3 camPosWorld)
        {
            _octree = octree;
            _rc = rc;
            Cam = cam;
            CamPosWorld = camPosWorld;
        }

        /// <summary>
        /// Helper method to traverse octree and return octant. Assumes that the node is visible and intersected by given ray.
        /// </summary>
        /// <returns></returns>
        private List<PointCloudOctant> PickOctantRecursively(PointCloudOctant node, RayD ray, List<PointCloudOctant> list)
        {
            list.Add(node);
            if (node.Children[0] != null)
            {
                foreach (var child in node.Children.Cast<PointCloudOctant>())
                {
                    if (child?.IsVisible == true && child.IntersectRay(ray))
                    {
                        PickOctantRecursively(child, ray, list);
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// Pick the densest octant under a given mouse position.
        /// </summary>
        /// <param name="pickPosClip">The mouse position in clip space.</param>
        /// <param name="viewportSize">Width and height of the viewport.</param>
        private List<PointCloudOctant>? PickOctantWrapper(float2 pickPosClip, int2 viewportSize)
        {
            // Create ray to intersect aabb's with.
            var ray = new RayD(new double2(pickPosClip.x, pickPosClip.y), (double4x4)_rc.View, (double4x4)Cam.GetProjectionMat(viewportSize.x, viewportSize.y, out _));
            var rootnode = (PointCloudOctant)_octree.Root;
            List<PointCloudOctant> picked = new();

            // Check if ray is hitting the octree at all.
            if (!rootnode.IsVisible || !rootnode.IntersectRay(ray))
            {
                return null;
            }

            // Go over each child and check whether they are visible nad intersecting the ray. If thats the case, a smaller octant can be picked.
            if (rootnode.Children[0] != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    var child = (PointCloudOctant)rootnode.Children[i];
                    if (child?.IsVisible == true && child.IntersectRay(ray))
                    {
                        PickOctantRecursively(child, ray, picked);
                    }
                }
            }
            else
            {
                picked.Add(rootnode);
                return picked;
            }
            if (picked.Count > 0)
            {
                return picked;

            }
            return null;
        }

        /// <summary>
        /// Returns all <see cref="PointCloudOctant"/> under given mouse position, without filtering or sorting
        /// </summary>
        /// <param name="pickPosClip"></param>
        /// <param name="viewportSize"></param>
        /// <returns></returns>
        public List<PointCloudOctant?> PickAllOctants(float2 pickPosClip, int2 viewportSize)
        {
            return PickOctantWrapper(pickPosClip, viewportSize);
        }

        /// <summary>
        /// Pick the <see cref="PointCloudOctant"/> that is densest to the camera under given mouse position.
        /// Octants are ignored if the camera is inside or the distance from the camera to the octant is smaller than the octant size.
        /// </summary>
        /// <param name="pickPosClip">The mouse position in clip space.</param>
        /// <param name="viewportSize">Width and height of the window.</param>
        public PointCloudOctant? PickDensestOctant(float2 pickPosClip, int2 viewportSize)
        {
            var pickResult = PickOctantWrapper(pickPosClip, viewportSize);

            // No octants picked.
            if (pickResult == null)
            {
                return null;
            }
            else
            {
                pickResult = pickResult.OrderBy(pickResult => DistanceToCamera((float3)pickResult.Center)).ToList();
                for (int i = 0; i < pickResult.Count(); i++)
                {
                    // Only pick octant that has a certain distance to the camera.
                    var dist = DistanceToCamera((float3)pickResult[i].Center);
                    if (pickResult[i].Intersects((double3)CamPosWorld) || dist < pickResult[i].Size)
                    {
                        pickResult.Remove(pickResult[i]);
                    }
                }
                // Sort by ratio of number of points and size of octant.
                pickResult = pickResult.OrderByDescending(pickResult => pickResult.NumberOfPointsInNode / pickResult.Size).ToList();
                return pickResult[0];
            }
        }

        /// <summary>
        /// Pick the <see cref="PointCloudOctant"/> that is closest to the camera under given mouse position.
        /// Octants are ignored if the camera is inside or the distance from the camera to the octant is smaller than the octant size.
        /// </summary>
        /// <param name="pickPosClip">The mouse position in clip space.</param>
        /// <param name="viewportSize">Width and height of the window.</param>
        public PointCloudOctant? PickClosestOctant(float2 pickPosClip, int2 viewportSize)
        {
            var pickResult = PickOctantWrapper(pickPosClip, viewportSize);

            // No octants picked.
            if (pickResult == null)
            {
                return null;
            }
            else
            {
                pickResult = pickResult.OrderBy(pickResult => DistanceToCamera((float3)pickResult.Center)).ToList();
                for (int i = 0; i < pickResult.Count(); i++)
                {
                    // Only pick octant that has a certain distance to the camera.
                    var dist = DistanceToCamera((float3)pickResult[i].Center);
                    if (pickResult[i].Intersects((double3)CamPosWorld) || dist < pickResult[i].Size)
                    {
                        pickResult.Remove(pickResult[i]);
                    }
                }
                return pickResult[0];
            }
        }

        /// <summary>
        /// Helper method to calculate distance between given vector (usually center of an octant) and the cameras position.
        /// </summary>
        /// <param name="point">Point to calculate distance to.</param>
        private float DistanceToCamera(float3 point)
        {
            return (point - CamPosWorld).Length;
        }

        /// <summary>
        /// Generate a cube with the same size and position as given octant.
        /// </summary>
        /// <param name="node">The octant to extract information on size and position from.</param>
        public static SceneNode CreateCubeFromNode(PointCloudOctant node)
        {
            return new SceneNode
            {
                Components =
                {
                    new Transform
                    {
                        Translation = (float3)node.Center,
                        Scale = new float3((float)node.Size)
                    },
                    MakeEffect.FromUnlit((float4)ColorUint.Blue),
                    new Cube(),
                }
            };
        }
    }
}