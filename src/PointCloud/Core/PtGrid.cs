using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.Structures;
using System;
using System.Collections.Generic;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Data structure that filters points and determines which fall into the next octree level.
    /// </summary>
    /// <typeparam name="TPoint">Point type (<seealso cref="PointAccessor{TPoint}"/>)</typeparam>
    public class PtGrid<TPoint> : GridD<TPoint>
    {
        /// <summary>
        /// Allows access to point properties.
        /// </summary>
        public PointAccessor<TPoint> PtAccessor { get; internal set; }

        /// <summary>
        /// A PtGrid is a property of an PtOctant - its parent octant.
        /// </summary>
        public PtOctantWrite<TPoint> ParentOctant { get; internal set; }

        private readonly List<int3> _neighbourKernel = GetGridNeighbourIndices(new(-1, -1, -1));

        /// <summary>
        /// Creates a new instance of type PtGrid. Will not create any GridCells.
        /// </summary>
        internal PtGrid(double3 center, double3 size) : base(center, size, 128, 128, 128) { }

        /// <summary>
        /// Creates a new instance of type PtGrid.
        /// </summary>
        public PtGrid(PointAccessor<TPoint> ptAccessor, PtOctantWrite<TPoint> parentOctant, IEnumerable<TPoint> points)
            : base(parentOctant.Center, new double3(parentOctant.Size, parentOctant.Size, parentOctant.Size), 128, 128, 128)
        {
            PtAccessor = ptAccessor;
            ParentOctant = parentOctant;
            CreateCells(points);
        }

        /// <summary>
        /// Creates a new <see cref="GridCellD{O}"/> for the given item.
        /// </summary>
        /// <param name="GetPositionOfPayloadItem">Method to retrieve the points coordinate.</param>
        /// <param name="point">The generic point cloud point.</param>
        public override void CreateCellForItem(Func<TPoint, double3> GetPositionOfPayloadItem, TPoint point)
        {
            var tPointPos = GetPositionOfPayloadItem(point);
            var cell = TryGetCellForPos(Size, Center, tPointPos, out var cellIdx);

            //Check if NN is too close - a point remains in the parent octant if the distance to the occupant of a neighbor cell is smaller than the neighbor cells' size.         
            foreach (var idxOffset in _neighbourKernel)
            {
                var neighbourCellIdx = cellIdx + idxOffset;

                if (neighbourCellIdx.x < 0 || neighbourCellIdx.x >= NumberOfGridCells.x
                    || neighbourCellIdx.y < 0 || neighbourCellIdx.y >= NumberOfGridCells.y
                    || neighbourCellIdx.z < 0 || neighbourCellIdx.z >= NumberOfGridCells.z)
                    continue;

                if (!GridCellsDict.TryGetValue(neighbourCellIdx, out var neighbourCell))
                    continue;

                if ((tPointPos - GetPositionOfPayloadItem(neighbourCell.Payload)).Length < neighbourCell.Size.x) //neighbourCell.Size equals spacing/ resolution of the octant
                {
                    ParentOctant.Payload.Add(point);
                    return;
                }
            }

            //Create CridCell on demand
            if (cell == null)
            {
                CreateCell(cellIdx);
                GridCellsDict[cellIdx].Payload = point;
            }
            else
            {
                //Check if the occupant must be changed
                var occupantDistToCenter = (GetPositionOfPayloadItem(cell.Payload) - cell.Center).Length;
                var pointDistToCenter = (tPointPos - cell.Center).Length;

                if (pointDistToCenter < occupantDistToCenter)
                {
                    ParentOctant.Payload.Add(cell.Payload);
                    cell.Payload = point;
                }
                else
                    ParentOctant.Payload.Add(point);
            }
        }

        /// <summary>
        /// Returns the (x,y,z) coordinates of a payload item.
        /// </summary>
        /// <param name="pt">The point (payload item)</param>
        /// <returns></returns>
        public override double3 GetPositionOfPayloadItem(TPoint pt)
        {
            return PtAccessor.GetPositionFloat3_64(ref pt);
        }

        public override void CreateCellForItem(Func<TPoint, Fusee.Math.Core.double3> GetPositionOfPayloadItem, TPoint payloadItem)
        {
            throw new NotImplementedException();
        }

        public override void CreateCellForItem(Func<TPoint, Fusee.Math.Core.double3> GetPositionOfPayloadItem, TPoint payloadItem)
        {
            throw new NotImplementedException();
        }
    }
}