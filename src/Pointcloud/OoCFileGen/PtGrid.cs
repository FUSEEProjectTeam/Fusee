using Fusee.Engine.Core;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    public class GridCell<T> : Bucket<T>
    {
        public T Occupant;

        public GridCell(double3 center, double size)
        {
            Center = center;
            Size = size;
        }
    }

    public class PtGrid<TPoint>
    {
        public GridCell<TPoint>[,,] GridCells;        

        private List<int3> _neighbouCellIdxOffsets;

        public PtGrid(GridPtAccessor<TPoint> ptAccessor, PtOctantWrite<TPoint> parentOctant, TPoint point)
        {
            _neighbouCellIdxOffsets = GetGridNeighbourIndices(1);
            GridCells = new GridCell<TPoint>[128, 128, 128];

            var firstCenter = CalcCenterOfUpperLeftCell(parentOctant);

            ReadPointToGrid(ptAccessor, parentOctant, point, firstCenter);
        }

        public PtGrid(GridPtAccessor<TPoint> ptAccessor, PtOctantWrite<TPoint> parentOctant, List<TPoint> points)
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
        public void ReadPointToGrid(GridPtAccessor<TPoint> ptAccessor, PtOctantWrite<TPoint> parentOctant, TPoint point, double3 firstCenter)
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

            //check if NN is too close            
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
                    ptAccessor.SetGridIdx(ref point, new int3(-1, -1, -1));

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
                ptAccessor.SetGridIdx(ref point, new int3(indexX, indexY, indexZ));

                GridCells[indexX, indexY, indexZ] = cell;
            }            
            else if (cell.Occupant == null) //set or change cell occupant if neccessary
            {
                cell.Occupant = point;
                ptAccessor.SetGridIdx(ref point, new int3(indexX, indexY, indexZ));
            }
            else
            {
                var occupantDistToCenter = (ptAccessor.GetPositionFloat3_64(ref cell.Occupant) - cell.Center).Length;
                var pointDistToCenter = (tPointPos - cell.Center).Length;

                if (pointDistToCenter < occupantDistToCenter)
                {
                    var occ = cell.Occupant;
                    ptAccessor.SetGridIdx(ref occ, new int3(-1, -1, -1));
                    parentOctant.Payload.Add(cell.Occupant);

                    ptAccessor.SetGridIdx(ref point, new int3(indexX, indexY, indexZ));
                    cell.Occupant = point; 
                }
                else
                {
                    ptAccessor.SetGridIdx(ref point, new int3(-1, -1, -1));
                    parentOctant.Payload.Add(point);                    
                }
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
