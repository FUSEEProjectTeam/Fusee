using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Structures
{
    /// <summary>
    /// Single-precision Octant implementation.
    /// </summary>
    /// <typeparam name="P">The type of the octants payload.</typeparam>
    public class OctantF<P> : IOctant<float3, float, P>
    {
        /// <summary>
        ///The Resolution of an Octant is defined by the minimum distance (spacing) between points.
        ///If the minimum distance between a point and its nearest neighbor is smaller then this distance, it will fall into a child octant.
        /// </summary>
        public double Resolution;

        /// <summary>
        /// The globally unique identifier for this octant.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public float3 Center { get; set; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// Children of this Octant. Must contain eight or null (leaf node) children.
        /// </summary>
        public IOctant<float3, float, P>[] Children { get; set; }

        /// <summary>
        /// The payload of this octant.
        /// </summary>
        public List<P> Payload { get; set; }

        /// <summary>
        /// Is this octant a leaf node in the octree?
        /// </summary>
        public bool IsLeaf { get; set; }

        /// <summary>
        /// Integer that defines this octants position in its parent.
        /// </summary>
        public int PosInParent { get; set; }

        /// <summary>
        /// The level of the octree this octant belongs to.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Creates a new instance of type PtOctant.
        /// </summary>
        /// <param name="center">The center point of this octant, <see cref="IBucket{T, K}.Center"/>.</param>
        /// <param name="size">The size of this octant, <see cref="IBucket{T, K}.Size"/>. </param>
        /// <param name="children">The children of this octant - can be null.</param>
        public OctantF(float3 center, float size, IOctant<float3, float, P>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new IOctant<float3, float, P>[8];
            else
                Children = children;

            Payload = new List<P>();
        }

        /// <summary>
        /// Creates a new instance of type PtOctant.
        /// </summary>
        protected OctantF() { }

        /// <summary>
        /// Creates a child octant at the given position in its parent octant.
        /// </summary>
        /// <param name="posInParent">The position in the parent octant.</param>
        /// <returns></returns>
        public virtual IOctant<float3, float, P> CreateChild(int posInParent)
        {
            var childCenter = CalcChildCenterAtPos(posInParent);

            var childRes = Size / 2f;
            var child = new OctantF<P>(childCenter, childRes)
            {
                Resolution = Resolution / 2f,
                Level = Level + 1
            };
            return child;
        }

        /// <summary>
        /// Returns the center of the child octant at the given position of the parent octant.
        /// </summary>
        /// <param name="posInParent">The position in the parent octant.</param>
        /// <returns></returns>
        protected float3 CalcChildCenterAtPos(int posInParent)
        {
            return CalcChildCenterAtPos(posInParent, Size, Center);
        }

        /// <summary>
        /// Returns the center of the child octant at the given position of the parent octant from the parents center and size.
        /// </summary>
        /// <param name="posInParent">The position in the parent octant.</param>
        /// <param name="parentSize">The size of the parent octant.</param>
        /// <param name="parentCenter">The center of the parent octant.</param>
        /// <returns></returns>
        public static float3 CalcChildCenterAtPos(int posInParent, float parentSize, float3 parentCenter)
        {
            var childsHalfSize = parentSize / 4f;
            var childCenter = posInParent switch
            {
                1 => new float3(parentCenter.x + childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z - childsHalfSize),
                2 => new float3(parentCenter.x - childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z + childsHalfSize),
                3 => new float3(parentCenter.x + childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z + childsHalfSize),
                4 => new float3(parentCenter.x - childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z - childsHalfSize),
                5 => new float3(parentCenter.x + childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z - childsHalfSize),
                6 => new float3(parentCenter.x - childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z + childsHalfSize),
                7 => new float3(parentCenter.x + childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z + childsHalfSize),
                _ => new float3(parentCenter.x - childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z - childsHalfSize),
            };
            return childCenter;
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this Octant.
        /// </summary>
        /// <param name="plane">The plane to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingPlane(PlaneF plane)
        {
            return plane.InsideOrIntersecting(Center, Size);
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this Octant.
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