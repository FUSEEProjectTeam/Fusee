using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Structures;
using System.Collections;
using System.Collections.Generic;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    public class PtOctree<TPoint> : OctreeD<TPoint>
    {
        /// <summary>
        /// The maximum number a octant can hold. If the number of points exceeds this value the octant is subdivided.
        /// </summary>
        public int MaxNoOfPointsInBucket { get; private set; }

        /// <summary>
        /// Provides access to properties of different point types.
        /// </summary>
        public PointAccessor<TPoint> PtAccessor { get; private set; }

        private static readonly BitArray _getChildIdxBitArray = new BitArray(3);
        private static readonly int[] _getChildIdxResultArray = new int[1];

        /// <summary>
        /// Constructor for creating an Octree that is suitable for creating files from it. 
        /// </summary>
        /// <param name="aabb">A <see cref="AABBd"/> that is used to create the Octree's <see cref="OctreeD{P}.Root"/></param>
        /// <param name="pa">The <see cref="PtAccessor"/> that provides access to the point properties.</param>
        /// <param name="points">The point cloud as unordered list of generic type.</param>
        /// <param name="maxNoOfPointsInBucket">This value sets the <see cref="MaxNoOfPointsInBucket"/>.</param>
        public PtOctree(AABBd aabb, PointAccessor<TPoint> pa, List<TPoint> points, int maxNoOfPointsInBucket)
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
            var root = new PtOctantWrite<TPoint>(aabb.Center, aabbMaxLength)
            {
                Resolution = res
            };
            var rootGrid = new PtGrid<TPoint>(PtAccessor, root, points);
            root.Grid = rootGrid;
            root.Level = 0;

            Root = root;

            if (Root.Payload.Count >= MaxNoOfPointsInBucket)
                Subdivide((PtOctantWrite<TPoint>)Root); //Initial subdivision
        }

        /// <summary>
        /// Constructor for creating an Octree that is suitable for creating files from it. 
        /// </summary>
        public PtOctree(PtOctant<TPoint> root, PointAccessor<TPoint> pa, int maxNoOfPointsInBucket)
        {
            MaxNoOfPointsInBucket = maxNoOfPointsInBucket;
            PtAccessor = pa;
            Root = root;
        }

        protected override bool SubdivTerminationCondition(IOctant<double3, double, TPoint> child)
        {
            return child.Payload.Count >= MaxNoOfPointsInBucket;
        }

        protected override void HandlePayload(IOctant<double3, double, TPoint> parent, IOctant<double3, double, TPoint> child, TPoint payload)
        {
            if (MaxLevel < child.Level)
                MaxLevel = child.Level;

            var parentWrite = (PtOctantWrite<TPoint>)parent;
            var firstCenter = PtGrid<TPoint>.CalcCenterOfUpperLeftCell(parentWrite);

            ((PtOctantWrite<TPoint>)child).Grid.ReadPointToGrid(PtAccessor, parentWrite, payload, firstCenter);
        }

        protected override int GetChildPosition(IOctant<double3, double, TPoint> octant, TPoint pt)
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

            _getChildIdxBitArray.CopyTo(_getChildIdxResultArray, 0);

            return _getChildIdxResultArray[0];
        }

        private static IEnumerable<TPoint> GetPointsFromGrid(PtOctantWrite<TPoint> octant)
        {
            foreach (var cell in octant.Grid.GridCells)
            {
                if (cell == null) continue;
                yield return cell.Occupant;
            }
        }
    }
}