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

        /// <summary>
        /// Position of the component in the hierarchy.
        /// </summary>
        public int PosInHierarchyTex;

        /// <summary>
        /// The visible child indices in byte.
        /// </summary>
        public byte VisibleChildIndices; 

        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        public void ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov)
        {
            var distance = (Center - camPos).Length;
            var slope = System.Math.Tan(fov / 2d);
            ProjectedScreenSize = screenHeight / 2d * Size / (slope * distance);
        }

        /// <summary>
        /// Returns whether the given float4x4 intersects with the planes
        /// </summary>
        /// <param name="vf"></param>
        /// <returns></returns>
        public bool Intersects(float4x4 vf)
        {            
            var planes = new float4[6];

            planes[0] = vf.Row3 + vf.Row2;
            planes[1] = vf.Row3 - vf.Row2;            
            planes[2] = vf.Row3 + vf.Row0;           
            planes[3] = vf.Row3 - vf.Row0;           
            planes[4] = vf.Row3 + vf.Row1;           
            planes[5] = vf.Row3 - vf.Row1;           

            foreach (var plane in planes)
            {
                var side = Classify(plane);
                if (side < 0) return false;
            }
            return true;
            //return PlaneIntersects(planes);
        }

        private float Classify(float4 plane)
        {
            plane.Normalize();

            // maximum extent in direction of plane normal (plane.xyz)
            var r = System.Math.Abs(Size * plane.x)
                + System.Math.Abs(Size * plane.y)
                + System.Math.Abs(Size * plane.z);

            // signed distance between box center and plane
            //float d = plane.Test(mCenter);
            var d = float3.Dot(plane.xyz, (float3)Center) + plane.w;

            // return signed distance
            if (System.Math.Abs(d) < r)
                return 0.0f;
            else if (d < 0.0)
                return (float)(d + r);
            return (float)(d - r);
        }
    }
}
