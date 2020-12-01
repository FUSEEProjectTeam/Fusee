using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.Structures;
using System;
using System.Collections.Generic;

namespace Fusee.PointCloud.OoCReaderWriter
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
        public PointAccessor<TPoint> PtAccessor {get; internal set;}

        /// <summary>
        /// A PtGrid is a property of an PtOctant - its parent octant.
        /// </summary>
        public PtOctantWrite<TPoint> ParentOctant { get; internal set; }

        /// <summary>
        /// Creates a new instance of type PtGrid. Will not create any GridCells.
        /// </summary>
        internal PtGrid(double3 center, double3 size) : base(center, size, 128, 128, 128){ }

        /// <summary>
        /// Creates a new instance of type PtGrid.
        /// </summary>
        public PtGrid(PointAccessor<TPoint> ptAccessor, PtOctantWrite<TPoint> parentOctant, TPoint point)
            : base(parentOctant.Center, new double3(parentOctant.Size, parentOctant.Size, parentOctant.Size), 128, 128, 128)
        {
            PtAccessor = ptAccessor;
            ParentOctant = parentOctant;
            CreateCellForItem(GetPositionOfPayloadItem, point);
        }

        /// <summary>
        /// Creates a new instance of type PtGrid.
        /// </summary>
        public PtGrid(PointAccessor<TPoint> ptAccessor, PtOctantWrite<TPoint> parentOctant, List<TPoint> points)
            : base(parentOctant.Center, new double3(parentOctant.Size, parentOctant.Size, parentOctant.Size), 128, 128, 128)
        {
            PtAccessor = ptAccessor;
            ParentOctant = parentOctant;

            for (int i = 0; i < points.Count; i++)
            {
                TPoint pt = points[i];
                CreateCellForItem(GetPositionOfPayloadItem, pt);
                points[i] = pt;
            }
        }

        public override void CreateCellForItem(Func<TPoint, double3> GetPositionOfPayloadItem, TPoint point)
        {
            var tPointPos = GetPositionOfPayloadItem(point);
            var cell = GetCellForPos(Size, Center, tPointPos, out var cellIdx);

            //Check if NN is too close - a point remains in the parent octant if the distance to the occupant of a neighbor cell is smaller than the neighbor cells' size.         
            foreach (var idxOffset in GetGridNeighbourIndices(new int3(-1, -1, -1)))
            {
                var neighbourCellIdx = new int3(cellIdx.x, cellIdx.y, cellIdx.z) + idxOffset;

                if (neighbourCellIdx.x < 0 || neighbourCellIdx.x >= NumberOfGridCells.x)
                    continue;
                if (neighbourCellIdx.y < 0 || neighbourCellIdx.y >= NumberOfGridCells.y)
                    continue;
                if (neighbourCellIdx.z < 0 || neighbourCellIdx.z >= NumberOfGridCells.z)
                    continue;

                var neighbourCell = GridCells[neighbourCellIdx.x, neighbourCellIdx.y, neighbourCellIdx.z];

                if (neighbourCell == null)
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
                var lowerLeftCenter = (Center - Size / 2d) + CellSize;
                var center = lowerLeftCenter + (new double3(cellIdx.x * CellSize.x, cellIdx.y * CellSize.y, cellIdx.z * CellSize.z));
                cell = new GridCellD<TPoint>(center, CellSize)
                {
                    Payload = point
                };

                GridCells[cellIdx.x, cellIdx.y, cellIdx.z] = (GridCellD<TPoint>)cell;
            }
            else if (cell.Payload == null) //set or change cell occupant if necessary
            {
                cell.Payload = point;
            }
            else
            {
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
    }
}