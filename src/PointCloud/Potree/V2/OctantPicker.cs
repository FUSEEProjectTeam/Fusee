using Fusee.Base.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Potree.V2.Data;
using System.Diagnostics;
using System.Linq;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Picker for octants.
    /// </summary>
    public class OctantPicker
    {
        private readonly Potree2Reader _reader;
        private readonly RenderContext _rc;
        private readonly Camera _cam;
        private readonly Transform _camTransform;

        /// <summary>
        /// Constructor for the octant picker.
        /// </summary>
        /// <param name="reader">The potree reader.</param>
        /// <param name="rc">The render context.</param>
        /// <param name="cam">The camera the calculation is based on.</param>
        /// <param name="camTransform">The transform of previously given camera.</param>
        public OctantPicker(Potree2Reader reader, RenderContext rc, Camera cam, Transform camTransform)
        {
            _reader = reader;
            _rc = rc;
            _cam = cam;
            _camTransform = camTransform;
        }

        /// <summary>
        /// Pick the densest octant under a given mouse position. 
        /// </summary>
        /// <param name="pickPosClip">The mouse position in clip space.</param>
        /// <param name="viewportSize">Width and height of the viewport.</param>
        public PotreeNode? PickDensenstOctant(float2 pickPosClip, int2 viewportSize)
        {
            // Create ray to intersect aabb's with.
            var ray = new RayD(new double2(pickPosClip.x, pickPosClip.y), (double4x4)_rc.View, (double4x4)_cam.GetProjectionMat(viewportSize.x, viewportSize.y, out _));

            // Get octree hierarchy.
            var nodeList = _reader.FileDataInstance.Hierarchy.Nodes;

            // Index based on octant density.
            var highestNumberOfPoints = 0L;
            var densestIndex = -1;

            // Traverse hierarchy to get raycast hits.
            for (var i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].NodeType == NodeType.LEAF && nodeList[i].Aabb.IntersectRay(ray))
                {
                    var numpoints = nodeList[i].NumPoints;

                    // Save index of densest octant.
                    if (highestNumberOfPoints < numpoints)
                    {
                        highestNumberOfPoints = numpoints;
                        densestIndex = i;
                    }
                }
            }
            // Return densest octant.
            if (densestIndex != -1)
            {
                return nodeList[densestIndex];
            }
            return null;
        }

        /// <summary>
        /// Pick the octant that is closest to the camera under given mouse position.
        /// </summary>
        /// <param name="pickPosClip">The mouse position in clip space.</param>
        /// <param name="viewportSize">Width and height of the window.</param>
        public PotreeNode? PickClosestOctant(float2 pickPosClip, int2 viewportSize)
        {
            // Create ray to intersect aabb's with.
            var ray = new RayD(new double2(pickPosClip.x, pickPosClip.y), (double4x4)_rc.View, (double4x4)_cam.GetProjectionMat(viewportSize.x, viewportSize.y, out _));

            // Get octree hierarchy.
            var nodeList = _reader.FileDataInstance.Hierarchy.Nodes;

            // Index based on nearest octant.
            var lowestDistance = double.MaxValue;
            var nearestIndex = -1;

            // Traverse hierarchy to get raycast hits.
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].NodeType == NodeType.LEAF && nodeList[i].Aabb.IntersectRay(ray))
                {
                    Debug.Assert(nodeList[i].Children.Any(x => x == null));

                    var aabb = nodeList[i].Aabb.Center;
                    var cameraTranslation = (double3)_camTransform.Translation;

                    // Calculate distance and save index of nearest octant.
                    var distance = System.Math.Sqrt
                        (
                            System.Math.Pow(aabb.x - cameraTranslation.x, 2) +
                            System.Math.Pow(aabb.y - cameraTranslation.y, 2) +
                            System.Math.Pow(aabb.z - cameraTranslation.z, 2)
                        );

                    if (distance < lowestDistance)
                    {
                        lowestDistance = distance;
                        nearestIndex = i;
                    }
                }
            }

            // Return octant closest to camera.
            if (nearestIndex != -1)
            {
                return nodeList[nearestIndex];
            }
            return null;
        }

        /// <summary>
        /// Generate a cube with the same size and position as given octant.
        /// </summary>
        /// <param name="node">The PotreeNode to extract information on size and position from.</param>
        public SceneNode CreateCubeFromNode(PotreeNode node)
        {
            return new SceneNode
            {
                Components =
                {
                    new Transform
                    {
                        Translation = (float3)node.Aabb.Center,
                        Scale = (float3)node.Aabb.Size
                    },
                    MakeEffect.FromUnlit((float4)ColorUint.Blue),
                    new Cube(),
                }
            };
        }
    }
}