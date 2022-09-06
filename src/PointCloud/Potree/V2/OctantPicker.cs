using Fusee.Engine.Core;
using Fusee.Base.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Potree.V2.Data;
using Fusee.Engine.Core.Effects;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Picker for octants.
    /// </summary>
    public class OctantPicker
    {
        private Potree2Reader _reader;
        private RenderContext _rc;
        private Camera _cam;
        private Transform _camTransform;

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
        /// <param name="textureSize">Width and height of the window.</param>
        public PotreeNode? PickDensenstOctant(float2 pickPosClip, int2 textureSize)
        {
            // Create ray to intersect aabb's with.
            RayD ray = new RayD(new double2(pickPosClip.x, pickPosClip.y), (double4x4)_rc.View, (double4x4)_cam.GetProjectionMat(textureSize.x, textureSize.y, out _));

            // Get octree hierarchy.
            PotreeHierarchy hierarchy = _reader.LoadHierarchy(_reader.FileDataInstance.Metadata.FolderPath);
            PotreeNode[] nodelist = hierarchy.Nodes.ToArray();

            // Index based on octant density.
            long highestNumberOfPoints = 0;
            int densestIndex = -1;

            // Traverse hierarchy to get raycast hits.
            for (int i = 0; i < nodelist.Length; i++)
            {
                if (nodelist[i].Aabb.IntersectRay(ray) && nodelist[i].NodeType == NodeType.LEAF)
                {
                    long numpoints = nodelist[i].NumPoints;

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

                //Fusee.Base.Core.Diagnostics.Debug(nodelist[densestIndex].Name);
                return nodelist[densestIndex];
            }
            return null;
        }

        /// <summary>
        /// Pick the octant that is closest to the camera under given mouse position.
        /// </summary>
        /// <param name="pickPosClip">The mouse position in clip space.</param>
        /// <param name="textureSize">Width and height of the window.</param>
        public PotreeNode? PickClosestOctant(float2 pickPosClip, int2 textureSize)
        {
            // Create ray to intersect aabb's with.
            RayD ray = new RayD(new double2(pickPosClip.x, pickPosClip.y), (double4x4)_rc.View, (double4x4)_cam.GetProjectionMat(textureSize.x, textureSize.y, out _));

            // Get octree hierarchy.
            PotreeHierarchy hierarchy = _reader.LoadHierarchy(_reader.FileDataInstance.Metadata.FolderPath);
            PotreeNode[] nodelist = hierarchy.Nodes.ToArray();

            // Index based on nearest octant.
            double lowestDistance = 10000000;
            int nearestIndex = -1;

            // Traverse hierarchy to get raycast hits.
            for (int i = 0; i < nodelist.Length; i++)
            {
                if (nodelist[i].Aabb.IntersectRay(ray) && nodelist[i].NodeType == PointCloud.Potree.V2.Data.NodeType.LEAF)
                {
                    double3 aabb = nodelist[i].Aabb.Center;
                    double3 camt = (double3)_camTransform.Translation;

                    // Calculate distance and save index of nearest octant.
                    double distance = System.Math.Sqrt(System.Math.Pow(aabb.x - camt.x, 2) + System.Math.Pow(aabb.y - camt.y, 2) + System.Math.Pow(aabb.z - camt.z, 2));
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
                //Fusee.Base.Core.Diagnostics.Debug(nodelist[nearestIndex].Name);
                return nodelist[nearestIndex];
            }
            return null;
        }

        /// <summary>
        /// Generate a cube with the same size and position as given octant.
        /// </summary>
        /// <param name="node">The PotreeNode to extract information on size and position from.</param>
        public SceneNode CreateCubeFromNode(PotreeNode node)
        {
            SceneNode cube = new SceneNode
            {
                Components =
                {
                    new Transform
                    {
                        Translation = (float3)node.Aabb.Center,
                    },
                    MakeEffect.FromUnlit((float4)ColorUint.Blue),
                    Cube.CreateCuboid((float3)node.Aabb.Size),
                }
            };
            return cube;
        }
    }

    /// <summary>
    /// Helper class to help visualising picked octant.
    /// </summary>
    public static class Cube
    {
        public static Mesh CreateCuboid(float3 size)
        {
            return new Mesh
            {
                Vertices = new[]
                {
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z}
                },

                Triangles = new ushort[]
                {
                    // front face
                    0, 2, 1, 0, 3, 2,

                    // right face
                    4, 6, 5, 4, 7, 6,

                    // back face
                    8, 10, 9, 8, 11, 10,

                    // left face
                    12, 14, 13, 12, 15, 14,

                    // top face
                    16, 18, 17, 16, 19, 18,

                    // bottom face
                    20, 22, 21, 20, 23, 22

                },

                Normals = new[]
                {
                    new float3(0, 0, 1),
                    new float3(0, 0, 1),
                    new float3(0, 0, 1),
                    new float3(0, 0, 1),
                    new float3(1, 0, 0),
                    new float3(1, 0, 0),
                    new float3(1, 0, 0),
                    new float3(1, 0, 0),
                    new float3(0, 0, -1),
                    new float3(0, 0, -1),
                    new float3(0, 0, -1),
                    new float3(0, 0, -1),
                    new float3(-1, 0, 0),
                    new float3(-1, 0, 0),
                    new float3(-1, 0, 0),
                    new float3(-1, 0, 0),
                    new float3(0, 1, 0),
                    new float3(0, 1, 0),
                    new float3(0, 1, 0),
                    new float3(0, 1, 0),
                    new float3(0, -1, 0),
                    new float3(0, -1, 0),
                    new float3(0, -1, 0),
                    new float3(0, -1, 0)
                },

                UVs = new[]
                {
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0),
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0),
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0),
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0),
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0),
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0)
                },
                BoundingBox = new AABBf(-0.5f * size, 0.5f * size)
            };
        }

        public static SurfaceEffect MakeMaterial(float4 color)
        {
            return MakeEffect.FromDiffuseSpecular(
                albedoColor: color,
                emissionColor: float3.Zero,
                shininess: 25.0f,
                specularStrength: 1f);
        }
    }
}
