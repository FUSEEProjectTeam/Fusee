using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Structures
{
    /// <summary>
    /// Double-precision Octant implementation.
    /// </summary>
    /// <typeparam name="P">The type of the octants payload.</typeparam>
    public class OctantD<P> : IOctant<double3, double, P>
    {
        /// <summary>
        ///The Resolution of an Octant is defined by the minimum distance (spacing) between points.
        ///If the minimum distance between a point and its nearest neighbor is smaller then this distance, it will fall into a child octant.
        /// </summary>
        public double Resolution;

        /// <summary>
        /// The globally unique identifier for this octant.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public double3 Center { get; set; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// Children of this Octant. Must contain eight or null (leaf node) children.
        /// </summary>
        public IOctant<double3, double, P>[] Children { get; set; }

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
        public int PosInParent { get; }

        /// <summary>
        /// The level of the octree this octant belongs to.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Creates a new instance of type PtOctant.
        /// </summary>
        /// <param name="center">The center point of this octant, <see cref="IBucket{T, K}.Center"/>.</param>
        /// <param name="size">The size of this octant, <see cref="IBucket{T, K}.Size"/>. </param>
        /// <param name="guid"></param>
        /// <param name="children">The children of this octant - can be null.</param>
        public OctantD(double3 center, double size, string guid, IOctant<double3, double, P>[] children = null)
        {
            Center = center;
            Size = size;

            Guid = guid;

            Level = Guid.Length;

            int posInParent;
            int.TryParse(Guid[Level - 1].ToString(), out posInParent);
            PosInParent = posInParent;

            if (children == null)
                Children = new IOctant<double3, double, P>[8];
            else
                Children = children;

            Payload = new List<P>();
        }

        /// <summary>
        /// Creates a new instance of type PtOctant.
        /// </summary>
        protected OctantD() { }

        /// <summary>
        /// Creates a child octant at the given position in its parent octant.
        /// </summary>
        /// <param name="posInParent">The position in the parent octant.</param>
        /// <returns></returns>
        public virtual IOctant<double3, double, P> CreateChild(int posInParent)
        {
            var childCenter = CalcChildCenterAtPos(posInParent);

            var childRes = Size / 2d;
            var child = new OctantD<P>(childCenter, childRes, Guid + posInParent)
            {
                Resolution = Resolution / 2d
            };
            return child;
        }

        /// <summary>
        /// Returns the center of the child octant at the given position of the parent octant.
        /// </summary>
        /// <param name="posInParent">The position in the parent octant.</param>
        /// <returns></returns>
        protected double3 CalcChildCenterAtPos(int posInParent)
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
        public static double3 CalcChildCenterAtPos(int posInParent, double parentSize, double3 parentCenter)
        {
            var childsHalfSize = parentSize / 4d;
            var childCenter = posInParent switch
            {
                1 => new double3(parentCenter.x + childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z - childsHalfSize),
                2 => new double3(parentCenter.x - childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z + childsHalfSize),
                3 => new double3(parentCenter.x + childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z + childsHalfSize),
                4 => new double3(parentCenter.x - childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z - childsHalfSize),
                5 => new double3(parentCenter.x + childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z - childsHalfSize),
                6 => new double3(parentCenter.x - childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z + childsHalfSize),
                7 => new double3(parentCenter.x + childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z + childsHalfSize),
                _ => new double3(parentCenter.x - childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z - childsHalfSize),
            };
            return childCenter;
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this Octant.
        /// </summary>
        /// <param name="plane">The plane to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingPlane(PlaneD plane)
        {
            return plane.InsideOrIntersecting(Center, Size);
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
        /// Checks if this Octant lies within or intersects a Frustum.
        /// Returns true if one of the Frustum planes is intersecting this octant.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumF frustum)
        {
            return frustum.Near.InsideOrIntersecting(new float3(Center), (float)Size) &&
                frustum.Far.InsideOrIntersecting(new float3(Center), (float)Size) &&
                frustum.Left.InsideOrIntersecting(new float3(Center), (float)Size) &&
                frustum.Right.InsideOrIntersecting(new float3(Center), (float)Size) &&
                frustum.Top.InsideOrIntersecting(new float3(Center), (float)Size) &&
                frustum.Bottom.InsideOrIntersecting(new float3(Center), (float)Size);
        }

        /// <summary>
        /// Checks if this Octant lies within or intersects a Frustum.
        /// Assumes that we do not need to process this octant if the near plane is completely inside it.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustumFast(FrustumF frustum)
        {
            if (!frustum.Near.InsideOrIntersecting(new float3(Center), (float)Size))
                return false;
            if (!frustum.Far.InsideOrIntersecting(new float3(Center), (float)Size))
                return false;
            if (!frustum.Left.InsideOrIntersecting(new float3(Center), (float)Size))
                return false;
            if (!frustum.Right.InsideOrIntersecting(new float3(Center), (float)Size))
                return false;
            if (!frustum.Top.InsideOrIntersecting(new float3(Center), (float)Size))
                return false;
            if (!frustum.Bottom.InsideOrIntersecting(new float3(Center), (float)Size))
                return false;

            return true;
        }
    }
}