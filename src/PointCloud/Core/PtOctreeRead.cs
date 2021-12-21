using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.Structures;
using System.Collections;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Tree data structure in which each internal node has up to eight children. 
    /// Octrees are most often used to partition a three-dimensional space by recursively subdividing it into eight octants.
    /// See <see cref="OctreeD{P}"/>
    /// This subclass is used for point cloud rendering. It has the additional fields <see cref="MaxNoOfPointsInBucket"/> and <see cref="PtAccessor"/>.
    /// </summary>
    /// <typeparam name="TPoint">The type of an octants payload.</typeparam>
    public class PtOctreeRead<TPoint> : OctreeD<TPoint> where TPoint : new()
    {
        /// <summary>
        /// The maximum number a octant can hold. If the number of points exceeds this value the octant is subdivided.
        /// </summary>
        public int MaxNoOfPointsInBucket { get; private set; }

        /// <summary>
        /// Provides access to properties of different point types.
        /// </summary>
        public IPointAccessor PtAccessor { get; private set; }

        private static readonly BitArray _getChildIdxBitArray = new(3);

        /// <summary>
        /// Constructor for creating an Octree that is suitable for creating files from it. 
        /// </summary>
        public PtOctreeRead(PtOctantRead<TPoint> root, IPointAccessor pa, int maxNoOfPointsInBucket)
        {
            MaxNoOfPointsInBucket = maxNoOfPointsInBucket;
            PtAccessor = pa;
            Root = root;
        }

        /// <summary>
        /// Needed for subdivision - Method that returns a condition that terminates the subdivision.
        /// </summary>
        /// <param name="child">The octant to subdivide.</param>
        /// <returns></returns>
        protected override bool SubdivTerminationCondition(OctantD<TPoint> child)
        {
            return child.Payload.Count <= MaxNoOfPointsInBucket;
        }

        /// <summary>
        /// Needed for subdivision - method that determines what happens to a payload item after the creation of an octants children.
        /// </summary>
        /// <param name="parent">The parent octant.</param>
        /// <param name="child">The child octant a payload item falls into.</param>
        /// <param name="payload">The payload item.</param>
        protected override void HandlePayload(OctantD<TPoint> parent, OctantD<TPoint> child, TPoint payload)
        {
            if (MaxLevel < child.Level)
                MaxLevel = child.Level;
        }

        /// <summary>
        /// Needed for subdivision - method that provides functionality to determine and return the index (position) of the child a payload item will fall into.
        /// </summary>
        /// <param name="octant">The octant to subdivide.</param>
        /// <param name="pt">The point for which the child index is determined.</param>
        /// <returns></returns>
        protected override int GetChildPosition(OctantD<TPoint> octant, TPoint pt)
        {
            var point = ((PointAccessor<TPoint>)PtAccessor).GetPositionFloat3_64(ref pt);

            var halfSize = octant.Size / 2d;
            var translationVec = new double3(octant.Center.x - halfSize, octant.Center.y - halfSize, octant.Center.z - halfSize); //translate to zero

            var x = point.x - translationVec.x;
            var y = point.y - translationVec.y;
            var z = point.z - translationVec.z;

            var indexX = (int)((x * 2.0) / octant.Size);
            var indexY = (int)((y * 2.0) / octant.Size);
            var indexZ = (int)((z * 2.0) / octant.Size);

            _getChildIdxBitArray[0] = indexX == 1;
            _getChildIdxBitArray[1] = indexZ == 1;
            _getChildIdxBitArray[2] = indexY == 1;

            return (int)BitArrayToU64(_getChildIdxBitArray);
        }

        private static ulong BitArrayToU64(BitArray ba)
        {
            var len = System.Math.Min(64, ba.Count);
            ulong n = 0;
            for (int i = 0; i < len; i++)
            {
                if (ba.Get(i))
                    n |= 1UL << i;
            }
            return n;
        }
    }
}