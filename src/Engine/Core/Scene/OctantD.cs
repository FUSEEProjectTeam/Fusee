using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Component that allows a SceneNode to save octant information."/>.
    /// </summary>
    public class OctantD : SceneComponent
    {
        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public double3 Center { get; set; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// Is this octant a leaf node in the octree?
        /// </summary>
        public bool IsLeaf { get; set; }

        /// <summary>
        /// Integer that defines this octants position (1 to 8) in its parent.
        /// </summary>
        public int PosInParent { get; set; }

        /// <summary>
        /// The level of the octree this octant belongs to.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Unique identifier of the node.
        /// </summary>
        public Guid Guid;

        /// <summary>
        /// Defines if the node was loaded into memory.
        /// </summary>
        public bool WasLoaded = false;

        /// <summary>
        /// Number of point cloud points, this node holds.
        /// </summary>
        public int NumberOfPointsInNode;       

        /// <summary>
        /// Creates a new instance of type Octant.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        public OctantD(double3 center, double size)
        {
            Center = center;
            Size = size;
        }

        /// <summary>
        /// Create a new instance of type Octant.
        /// </summary>
        public OctantD() { }

        /// <summary>
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(double3, int, float)"/>.
        /// </summary>
        public double ProjectedScreenSize { get; private set; }

        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        public void ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov)
        {
            var distance = (Center - camPos).Length;
            var slope = (float)System.Math.Tan(fov / 2d);
            ProjectedScreenSize = screenHeight / 2d * Size / (slope * distance);
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this Octant.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumD frustum)
        {
            if (!frustum.Near.InsideOrIntersecting(Center, Size))
                return false;
            if (!frustum.Far.InsideOrIntersecting(Center, Size))
                return false;
            if (!frustum.Left.InsideOrIntersecting(Center, Size))
                return false;
            if (!frustum.Right.InsideOrIntersecting(Center, Size))
                return false;
            if (!frustum.Top.InsideOrIntersecting(Center, Size))
                return false;
            if (!frustum.Bottom.InsideOrIntersecting(Center, Size))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this Octant.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumF frustum)
        {
            if (!frustum.Near.InsideOrIntersecting((float3)Center, (float)Size))
                return false;
            if (!frustum.Far.InsideOrIntersecting((float3)Center, (float)Size))
                return false;
            if (!frustum.Left.InsideOrIntersecting((float3)Center, (float)Size))
                return false;
            if (!frustum.Right.InsideOrIntersecting((float3)Center, (float)Size))
                return false;
            if (!frustum.Top.InsideOrIntersecting((float3)Center, (float)Size))
                return false;
            if (!frustum.Bottom.InsideOrIntersecting((float3)Center, (float)Size))
                return false;

            return true;
        }
    }
}