using Fusee.Math.Core;
using Fusee.Structures;
using System;
using System.Collections.Generic;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Node/Bucket data structure that is used in a <see cref="PtOctree{TPoint}"/>. Needed to save a point cloud into a file format that can be used for out of core rendering.
    /// </summary>
    /// <typeparam name="TPoint">The type pf the point cloud points.</typeparam>
    public class PtOctantWrite<TPoint> : OctantD<TPoint>
    {
        /// <summary>
        /// Grid data structure used to distribute points in the octree.
        /// </summary>
        public PtGrid<TPoint> Grid;

        private readonly int _capacity;

        /// <summary>
        /// Creates a new instance of type <see cref="PtOctantWrite{TPoint}"/>.
        /// </summary>
        /// <param name="center">The center of this octant.</param>
        /// <param name="size">The size (in all three dimensions) of this octant.</param>
        /// <param name="capacity">The maximum number of payload elements.</param>
        /// <param name="children">The octants child octants.</param>
        public PtOctantWrite(double3 center, double size, int capacity, IOctant<double3, double, TPoint>[] children = null)
        {
            Guid = Guid.NewGuid();

            Center = center;
            Size = size;

            if (children == null)
                Children = new IOctant<double3, double, TPoint>[8];
            else
                Children = children;

            _capacity = capacity;
            Payload = new List<TPoint>(_capacity);
        }

        /// <summary>
        /// Creates a child at the given position. 
        /// </summary>
        /// <param name="posInParent">The new octants position in its parent.</param>
        /// <returns></returns>
        public override IOctant<double3, double, TPoint> CreateChild(int posInParent)
        {
            var childCenter = CalcChildCenterAtPos(posInParent);

            var childRes = Size / 2d;
            var child = new PtOctantWrite<TPoint>(childCenter, childRes, _capacity)
            {
                Resolution = Resolution / 2d,
                Level = Level + 1
            };

            child.Grid = new PtGrid<TPoint>(child.Center, new double3(child.Size, child.Size, child.Size));

            return child;
        }
    }
}