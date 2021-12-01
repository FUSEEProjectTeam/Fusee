using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.Structures;
using System.Collections;
using System.Collections.Generic;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Tree data structure in which each internal node has up to eight children. 
    /// Octrees are most often used to partition a three-dimensional space by recursively subdividing it into eight octants.
    /// See <see cref="OctreeD{P}"/>
    /// This subclass is used for point cloud rendering. It has the additional fields <see cref="MaxNoOfPointsInBucket"/> and <see cref="PtAccessor"/>.
    /// </summary>
    /// <typeparam name="TPoint">The type of an octants payload.</typeparam>
    public class PtOctreeWrite<TPoint> : OctreeD<TPoint> where TPoint : new()
    {
        /// <summary>
        /// The maximum number a octant can hold. If the number of points exceeds this value the octant is subdivided.
        /// </summary>
        public int MaxNoOfPointsInBucket { get; private set; }

        /// <summary>
        /// Provides access to properties of different point types.
        /// </summary>
        public PointAccessor<TPoint> PtAccessor { get; private set; }

        private static readonly BitArray _getChildIdxBitArray = new(3);
        private static readonly int[] _getChildIdxResultArray = new int[1];

        /// <summary>
        /// Constructor for creating an Octree that is suitable for creating files from it. 
        /// </summary>
        /// <param name="aabb">A <see cref="AABBd"/> that is used to create the Octree's <see cref="OctreeD{P}.Root"/></param>
        /// <param name="pa">The <see cref="PtAccessor"/> that provides access to the point properties.</param>
        /// <param name="points">The point cloud as unordered list of generic type.</param>
        /// <param name="maxNoOfPointsInBucket">This value sets the <see cref="MaxNoOfPointsInBucket"/>.</param>
        public PtOctreeWrite(AABBd aabb, PointAccessor<TPoint> pa, IEnumerable<TPoint> points, int maxNoOfPointsInBucket)
        {
            MaxNoOfPointsInBucket = maxNoOfPointsInBucket;

            PtAccessor = pa;

            //Aabb must be a cube, therefor get the max length.
            var aabbMaxLength = aabb.Size.x;
            if (aabb.Size.y > aabbMaxLength)
                aabbMaxLength = aabb.Size.y;
            if (aabb.Size.z > aabbMaxLength)
                aabbMaxLength = aabb.Size.z;

            aabbMaxLength += (aabbMaxLength / 100 * 0.01d); //add 1% of the size to it to ensure no point lies on the border of the aabb or the octant.

            var res = aabbMaxLength / 128d; //spacing
            var root = new PtOctantWrite<TPoint>(aabb.Center, aabbMaxLength, maxNoOfPointsInBucket)
            {
                Resolution = res,
                Level = 0
            };
            root.Grid = new PtGrid<TPoint>(PtAccessor, root, points);
            Root = root;

            Subdivide((PtOctantWrite<TPoint>)Root);
        }

        /// <summary>
        /// Constructor for creating an Octree that is suitable for creating files from it. 
        /// </summary>
        public PtOctreeWrite(OctantD<TPoint> root, PointAccessor<TPoint> pa, int maxNoOfPointsInBucket)
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

            var childWrite = (PtOctantWrite<TPoint>)child;
            childWrite.Grid.PtAccessor = PtAccessor;
            childWrite.Grid.ParentOctant = childWrite;
            childWrite.Grid.CreateCellForItem(childWrite.Grid.GetPositionOfPayloadItem, payload);
        }

        /// <summary>
        /// Needed for subdivision - method that provides functionality to determine and return the index (position) of the child a payload item will fall into.
        /// </summary>
        /// <param name="octant">The octant to subdivide.</param>
        /// <param name="pt">The point for which the child index is determined.</param>
        /// <returns></returns>
        protected override int GetChildPosition(OctantD<TPoint> octant, TPoint pt)
        {
            var point = PtAccessor.GetPositionFloat3_64(ref pt);

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

        private static IEnumerable<TPoint> GetPointsFromGrid(PtOctantWrite<TPoint> octant)
        {
            foreach (var cell in octant.Grid.GridCellsDict)
            {
                if (cell.Value == null) continue;
                yield return cell.Value.Payload;
            }
        }
    }
}