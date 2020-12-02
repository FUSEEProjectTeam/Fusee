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
        public Guid Guid { get; set; }

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
        public OctantD(double3 center, double size, IOctant<double3, double, P>[] children = null)
        {
            Center = center;
            Size = size;

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
            var childCenter = CalcCildCenterAtPos(posInParent);

            var childRes = Size / 2d;
            var child = new OctantD<P>(childCenter, childRes)
            {
                Resolution = Resolution / 2d,
                Level = Level + 1
            };
            return child;
        }

        /// <summary>
        /// Returns the center of the child octant at the given position of the parent octant.
        /// </summary>
        /// <param name="posInParent">The position in the parent octant.</param>
        /// <returns></returns>
        protected double3 CalcCildCenterAtPos(int posInParent)
        {
            return CalcCildCenterAtPos(posInParent, Size, Center);
        }

        /// <summary>
        /// Returns the center of the child octant at the given position of the parent octant from the parents center and size.
        /// </summary>
        /// <param name="posInParent">The position in the parent octant.</param>
        /// <param name="parentSize">The size of the parent octant.</param>
        /// <param name="parentCenter">The center of the parent octant.</param>
        /// <returns></returns>
        public static double3 CalcCildCenterAtPos(int posInParent, double parentSize, double3 parentCenter)
        {
            double3 childCenter;
            var childsHalfSize = parentSize / 4d;
            switch (posInParent)
            {
                default:
                case 0:
                    childCenter = new double3(parentCenter.x - childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z - childsHalfSize);
                    break;
                case 1:
                    childCenter = new double3(parentCenter.x + childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z - childsHalfSize);
                    break;
                case 2:
                    childCenter = new double3(parentCenter.x - childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z + childsHalfSize);
                    break;
                case 3:
                    childCenter = new double3(parentCenter.x + childsHalfSize, parentCenter.y - childsHalfSize, parentCenter.z + childsHalfSize);
                    break;
                case 4:
                    childCenter = new double3(parentCenter.x - childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z - childsHalfSize);
                    break;
                case 5:
                    childCenter = new double3(parentCenter.x + childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z - childsHalfSize);
                    break;
                case 6:
                    childCenter = new double3(parentCenter.x - childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z + childsHalfSize);
                    break;
                case 7:
                    childCenter = new double3(parentCenter.x + childsHalfSize, parentCenter.y + childsHalfSize, parentCenter.z + childsHalfSize);
                    break;
            }

            return childCenter;
        }
    }
}