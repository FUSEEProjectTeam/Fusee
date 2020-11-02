using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Structures;
using System.Collections.Generic;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    /// <summary>
    /// The cell of a <see cref="PtGrid{TPoint}"/>.
    /// </summary>
    /// <typeparam name="O">The type of the point that occupies this cell.</typeparam>
    public class GridCell<O> : IBucket<double3, double>
    {
        /// <summary>
        /// The point that occupies this cell.
        /// </summary>
        public O Occupant;

        /// <summary>
        /// Creates a new instance of type GridCell.
        /// </summary>
        /// <param name="center">The center of the cell.</param>
        /// <param name="size">The size of the cell.</param>
        public GridCell(double3 center, double size)
        {
            Center = center;
            Size = size;
        }

        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public double3 Center { get; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public double Size { get; }
    }

    /// <summary>
    /// Data structure that filters points and determines which fall into the next octree level.
    /// </summary>
    /// <typeparam name="TPoint">Point type (<seealso cref="PointAccessor{TPoint}"/>)</typeparam>
    public class PtGrid<TPoint>
    {
        /// <summary>
        /// All grid cells as three dimensional array.
        /// </summary>
        public GridCell<TPoint>[,,] GridCells;

        private readonly List<int3> _neighbouCellIdxOffsets;

        /// <summary>
        /// Creates a new instance of type PtGrid.
        /// </summary>
        public PtGrid()
        {
            _neighbouCellIdxOffsets = GetGridNeighbourIndices(1);
            GridCells = new GridCell<TPoint>[128, 128, 128];
        }

        public PtGrid(PointAccessor<TPoint> ptAccessor, PtOctantWrite<TPoint> parentOctant, TPoint point)
        {
            _neighbouCellIdxOffsets = GetGridNeighbourIndices(1);
            GridCells = new GridCell<TPoint>[128, 128, 128];

            var firstCenter = CalcCenterOfUpperLeftCell(parentOctant);
            ReadPointToGrid(ptAccessor, parentOctant, point, firstCenter);
        }

        public PtGrid(PointAccessor<TPoint> ptAccessor, PtOctantWrite<TPoint> parentOctant, List<TPoint> points)
        {
            _neighbouCellIdxOffsets = GetGridNeighbourIndices(1);
            GridCells = new GridCell<TPoint>[128, 128, 128];

            var firstCenter = CalcCenterOfUpperLeftCell(parentOctant);

            for (int i = 0; i < points.Count; i++)
            {
                TPoint pt = points[i];
                ReadPointToGrid(ptAccessor, parentOctant, pt, firstCenter);
                points[i] = pt;
            }
        }

        public static double3 CalcCenterOfUpperLeftCell(PtOctantWrite<TPoint> parentOctant)
        {
            var parentHalfSize = parentOctant.Size / 2d;
            var parentHalfRes = parentOctant.Resolution / 2d;
            var lowerLeft = parentOctant.Center - new double3(parentHalfSize, parentHalfSize, parentHalfSize);
            return new double3(lowerLeft.x + parentHalfRes, lowerLeft.y + parentHalfRes, lowerLeft.z + parentHalfRes);
        }

        //see https://math.stackexchange.com/questions/528501/how-to-determine-which-cell-in-a-grid-a-point-belongs-to
        public void ReadPointToGrid(PointAccessor<TPoint> ptAccessor, PtOctantWrite<TPoint> parentOctant, TPoint point, double3 firstCenter)
        {
            var halfSize = parentOctant.Size / 2d;
            var translationVec = new double3(parentOctant.Center.x - halfSize, parentOctant.Center.y - halfSize, parentOctant.Center.z - halfSize); //translate to zero

            var tPointPos = ptAccessor.GetPositionFloat3_64(ref point);

            var x = tPointPos.x - translationVec.x;
            var y = tPointPos.y - translationVec.y;
            var z = tPointPos.z - translationVec.z;

            var indexX = (int)((x * 128) / parentOctant.Size);
            var indexY = (int)((y * 128) / parentOctant.Size);
            var indexZ = (int)((z * 128) / parentOctant.Size);

            var cell = GridCells[indexX, indexY, indexZ];

            //Check if NN is too close - a point remains in the parent octant if the distance to the occupant of a neighbor cell is smaller than the neighbor cells' size.         
            foreach (var idxOffset in _neighbouCellIdxOffsets)
            {
                //var neighbourCellIdx = new int3(indexX, indexY, indexZ) + idxOffset;

                var nIndexX = indexX + idxOffset.x;
                var nIndexY = indexY + idxOffset.y;
                var nIndexZ = indexZ + idxOffset.z;

                if (nIndexX < 0 || nIndexX > 127)
                    continue;
                if (nIndexY < 0 || nIndexY > 127)
                    continue;
                if (nIndexZ < 0 || nIndexZ > 127)
                    continue;

                var neighbourCell = GridCells[nIndexX, nIndexY, nIndexZ];

                if (neighbourCell == null)
                    continue;

                if ((tPointPos - ptAccessor.GetPositionFloat3_64(ref neighbourCell.Occupant)).Length < neighbourCell.Size) //neighbourCell.Size equals spacing/ resolution of the octant
                {
                    parentOctant.Payload.Add(point);
                    return;
                }
            }

            //create CridCell on demand
            if (cell == null)
            {
                var center = new double3(firstCenter.x + parentOctant.Resolution * x, firstCenter.y + parentOctant.Resolution * y, firstCenter.z + parentOctant.Resolution * z);
                cell = new GridCell<TPoint>(center, parentOctant.Resolution)
                {
                    Occupant = point
                };

                GridCells[indexX, indexY, indexZ] = cell;
            }
            else if (cell.Occupant == null) //set or change cell occupant if necessary            
            {
                cell.Occupant = point;
            }
            else
            {
                var occupantDistToCenter = (ptAccessor.GetPositionFloat3_64(ref cell.Occupant) - cell.Center).Length;
                var pointDistToCenter = (tPointPos - cell.Center).Length;

                if (pointDistToCenter < occupantDistToCenter)
                {
                    parentOctant.Payload.Add(cell.Occupant);
                    cell.Occupant = point;
                }
                else
                    parentOctant.Payload.Add(point);

            }

        }

        private static List<int3> GetGridNeighbourIndices(int dist)
        {
            var searchkernel = new List<int3>();
            var startIdx = new int3(-dist, -dist, -dist);
            var loopL = dist * 2;

            for (var x = 0; x <= loopL; x++)
            {
                var xIndex = startIdx.x + x;

                for (var y = 0; y <= loopL; y++)
                {
                    var yIndex = startIdx.y + y;

                    for (var z = 0; z <= loopL; z++)
                    {
                        var zIndex = startIdx.z + z;

                        //skip "inner" vertices
                        if (System.Math.Abs(xIndex) == dist ||
                            System.Math.Abs(yIndex) == dist ||
                            System.Math.Abs(zIndex) == dist)
                        {
                            searchkernel.Add(new int3(xIndex, yIndex, zIndex));
                        }
                    }
                }
            }

            return searchkernel;
        }
    }
}