using Fusee.Math.Core;
using System;

namespace Fusee.Serialization
{
    /// <summary>
    /// Component that allows a SceneNode to save information usually associated with a "PtOctant".
    /// </summary>
    public class PtOctantComponent : SceneComponentContainer
    {
        /// <summary>
        /// Defines the position in the parent octant.
        /// </summary>
        public int PosInParent;

        /// <summary>
        /// The level of the octree the node lies in.
        /// </summary>
        public int Level;

        /// <summary>
        /// Unique identifier of the node.
        /// </summary>
        public Guid Guid;

        /// <summary>
        /// Center in world space.
        /// </summary>
        public double3 Center;

        /// <summary>
        /// Length of on side of the cubical node.
        /// </summary>
        public double Size;

        /// <summary>
        /// Defines if the node is a leaf node.
        /// </summary>
        public bool IsLeaf;

        /// <summary>
        /// Defines if the node was loaded into memory.
        /// </summary>
        public bool WasLoaded;

        /// <summary>
        /// Number of point cloud points, this node holds.
        /// </summary>
        public int NumberOfPointsInNode;

        /// <summary>
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(double3, int, float)"/>.
        /// </summary>
        public double ProjectedScreenSize { get; private set; }

        public byte PosInHierarchyTex;

        public byte VisibleChildIndices;


        //TODO: duplicate code, clean up! The original is found in AABB.
        /// <summary>
        ///     Checks if a viewing frustum lies within this AABB.
        ///     If feeded with a projection matrix, the result of the clipping planes is in view space
        ///     If feeded with a projection view matrix, the clipping planes are given in model space
        /// </summary>
        /// <param name="vf">Viewing frustum / projection matrix</param>
        /// <returns>false if fully outside, true if inside or intersects</returns>
        public bool Intersects(float4x4 vf)
        {
            // split the viewing frustum in 6 planes
            // plane equation = ax + by + cz + d = 0;
            // For the GL-style frustum we find, that the six frustum planes in view space are exactly the six planes p_4^T±p_i^T for i=1, 2, 3 
            var planes = new double4[6];
            // left
            planes[0] = new double4(vf.M41 + vf.M11,
                                    vf.M42 + vf.M12,
                                    vf.M43 + vf.M13,
                                    vf.M44 + vf.M14);
            // right
            planes[1] = new double4(vf.M41 - vf.M11,
                                    vf.M42 - vf.M12,
                                    vf.M43 - vf.M13,
                                    vf.M44 - vf.M14);

            // bottom
            planes[2] = new double4(vf.M41 + vf.M21,
                                    vf.M42 + vf.M22,
                                    vf.M43 + vf.M23,
                                    vf.M44 + vf.M24);

            // top
            planes[3] = new double4(vf.M41 - vf.M21,
                                    vf.M42 - vf.M22,
                                    vf.M43 - vf.M23,
                                    vf.M44 - vf.M24);

            // near
            planes[4] = new double4(vf.M41 + vf.M31,
                                     vf.M42 + vf.M32,
                                     vf.M43 + vf.M33,
                                     vf.M44 + vf.M34);

            // far
            planes[5] = new double4(vf.M41 - vf.M31,
                                    vf.M42 - vf.M32,
                                    vf.M43 - vf.M33,
                                    vf.M44 - vf.M34);

            foreach (var plane in planes)
            {
                var side = Classify(plane);
                if (side < 0) return false;
            }
            return true;

        }

        private double Classify(double4 plane)
        {
            // maximum extent in direction of plane normal (plane.xyz)
            var r = System.Math.Abs(Size * plane.x)
                + System.Math.Abs(Size * plane.y)
                + System.Math.Abs(Size * plane.z);

            // signed distance between box center and plane
            //float d = plane.Test(mCenter);
            var d = double3.Dot(plane.xyz, Center) + plane.w;

            // return signed distance
            if (System.Math.Abs(d) < r)
                return 0.0f;
            else if (d < 0.0)
                return d + r;
            return d - r;
        }


        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        public void ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov)
        {
            var distance = (Center - camPos).Length;
            var slope = (float)System.Math.Tan(fov / 2f);
            ProjectedScreenSize = screenHeight / 2d * Size / (slope * distance);
        }
    }
}
