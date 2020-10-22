﻿using Fusee.Math.Core;

namespace Fusee.Structures.Octree
{
    /// <summary>
    /// 1/8 of an cube - used in an Octree.
    /// </summary>
    public class OctantF
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
        /// Center in world space.
        /// </summary>
        public float3 Center;

        /// <summary>
        /// Length of on side of the cubical node.
        /// </summary>
        public float Size;

        /// <summary>
        /// Defines if the node is a leaf node.
        /// </summary>
        public bool IsLeaf;

        /// <summary>
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(float3, int, float)"/>.
        /// </summary>
        public float ProjectedScreenSize { get; private set; }

        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        public void ComputeScreenProjectedSize(float3 camPos, int screenHeight, float fov)
        {
            var distance = (Center - camPos).Length;
            var slope = (float)System.Math.Tan(fov / 2f);
            ProjectedScreenSize = screenHeight / 2f * Size / (slope * distance);
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this AABB.      
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumF frustum)
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
    }
}