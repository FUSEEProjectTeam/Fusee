using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fusee.Pointcloud.OoCFileGen
{
    public class GridPtAccessor<TPoint> : PointAccessor<TPoint>
    {
        public bool HasGridIdx = true;

        public virtual ref int3 GetGridIdx(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetPositionFloat32");
        }

        public virtual void SetGridIdx(ref TPoint point, int3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetPositionFloat32");
        }
    }

    public class Grid<TPoint>
    {
        public GridCell<TPoint>[,,] GridCells;        

        private List<int3> _neighbouCellIdxOffsets;

        public Grid(GridPtAccessor<TPoint> ptAccessor, PtOctant<TPoint> parentOctant, TPoint point)
        {
            _neighbouCellIdxOffsets = GetGridNeighbourIndices(1);
            GridCells = new GridCell<TPoint>[128, 128, 128];

            var firstCenter = CalcCenterOfUpperLeftCell(parentOctant);

            ReadPointToGrid(ptAccessor, parentOctant, point, firstCenter);
        }

        public Grid(GridPtAccessor<TPoint> ptAccessor, PtOctant<TPoint> parentOctant, List<TPoint> points)
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

        public static double3 CalcCenterOfUpperLeftCell(PtOctant<TPoint> parentOctant)
        {
            var parentHalfSize = parentOctant.Size / 2d;
            var parentHalfRes = parentOctant.Resolution / 2d;
            var lowerLeft = parentOctant.Center - new double3(parentHalfSize, parentHalfSize, parentHalfSize);
            return new double3(lowerLeft.x + parentHalfRes, lowerLeft.y + parentHalfRes, lowerLeft.z + parentHalfRes);
        }

        //see https://math.stackexchange.com/questions/528501/how-to-determine-which-cell-in-a-grid-a-point-belongs-to
        public void ReadPointToGrid(GridPtAccessor<TPoint> ptAccessor, PtOctant<TPoint> parentOctant, TPoint point, double3 firstCenter)
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

            //create CridCell on demand
            if(cell == null)
            {
                var center = new double3(firstCenter.x + parentOctant.Resolution * x, firstCenter.y + parentOctant.Resolution * y, firstCenter.z + parentOctant.Resolution * z);
                cell = new GridCell<TPoint>(center, parentOctant.Resolution);
                GridCells[indexX, indexY, indexZ] = cell;               
            }

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
                if (neighbourCell.Occupant == null)
                    continue;

                if ((tPointPos - ptAccessor.GetPositionFloat3_64(ref neighbourCell.Occupant)).Length < neighbourCell.Size) //neighbourCell.Size equals spacing/ resolution of the octant
                { 
                    parentOctant.Payload.Add(point);
                    ptAccessor.SetGridIdx(ref point, new int3(-1, -1, -1));

                    return;
                }
            }

            //set or change cell occupant if neccessary
            if (cell.Occupant == null)
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

    public class PtOctant<TPoint> : Octant<TPoint>
    {
        //The Resolution of an Octant is defined by the minimum distance (spacing) between points.
        //If the minimum distance between a point and its nearest neighbour is smaller then this distance, it will fall into a child octant.
        public double Resolution;

        public Grid<TPoint> Grid;

        public bool IsLeaf { get; internal set; }

        public PtOctant(double3 center, double size, Octant<TPoint>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new PtOctant<TPoint>[8];
            else
                Children = children;

            Payload = new List<TPoint>();
        }
    }

    public class PtOctree<TPoint>
    {
        public int MaxNoOfPointsInBucket { get; private set; }

        public GridPtAccessor<TPoint> PtAccessor { get; private set; }

        public PtOctant<TPoint> Root { get; set; }
        
        public int MaxLevel { get; private set; }

        public PtOctree(AABBd aabb, GridPtAccessor<TPoint> pa, List<TPoint> points, int maxNoOfPointsInBucket)
        {
            MaxNoOfPointsInBucket = maxNoOfPointsInBucket;

            PtAccessor = pa;

            //aabb must not be a cube, therefor get the max length
            var aabbMaxLength = aabb.Size.x;
            if (aabb.Size.y > aabbMaxLength)
                aabbMaxLength = aabb.Size.y;
            if (aabb.Size.z > aabbMaxLength)
                aabbMaxLength = aabb.Size.z;

            aabbMaxLength += (aabbMaxLength / 100 * 0.01d); //add 1% of the size to it to ensure no point lies on the border of the aabb or the octant.

            var res = aabbMaxLength / 128d; //spacing            
            var root = new PtOctant<TPoint>(aabb.Center, aabbMaxLength)
            {
                Resolution = res
            };
            var rootGrid = new Grid<TPoint>(PtAccessor, root, points);
            root.Grid = rootGrid;
            root.Level = 0;

            Root = root;

            if (Root.Payload.Count >= MaxNoOfPointsInBucket)
            {
                Subdivide(Root);//Initial subdiv
            }
        }

        public void Subdivide(PtOctant<TPoint> octant)
        {            
            for (int i = 0; i < octant.Payload.Count; i++) {            
                var pt = octant.Payload[i];
                var ptPos = PtAccessor.GetPositionFloat3_64(ref pt);
                var posInParent = GetChildIndexToWritePoint(octant, ptPos);

                CreateChildAndReadPtToGrid(posInParent, octant, pt);                
            }            
            octant.Payload.Clear();

            for (int i = 0; i < octant.Children.Length; i++)
            {
                var child = (PtOctant<TPoint>)octant.Children[i];
                if (child == null) continue;

                if (child.Payload.Count >= MaxNoOfPointsInBucket)
                {
                    Subdivide(child);
                }
                else                
                    child.IsLeaf = true;                
            }
        }
               
        private void CreateChildAndReadPtToGrid(int posInParent, PtOctant<TPoint> octant, TPoint point)
        {
            PtOctant<TPoint> child;

            if (octant.Children[posInParent] == null)
            {
                child = CreateChild(octant, posInParent);
                var childGrid = new Grid<TPoint>(PtAccessor, child, point);
                child.Grid = childGrid;
                octant.Children[posInParent] = child;                
            }
            else
            {
                var firstCenter = Grid<TPoint>.CalcCenterOfUpperLeftCell(octant);
                child = (PtOctant<TPoint>)octant.Children[posInParent];
                child.Grid.ReadPointToGrid(PtAccessor, child, point, firstCenter);
            } 
        }

        private static BitArray bitArray = new BitArray(3);
        private static int[] resultArray = new int[1];

        private static int GetChildIndexToWritePoint(Octant<TPoint> octant, double3 point)
        {
            var halfSize = octant.Size / 2d;
            var translationVec = new double3(octant.Center.x - halfSize, octant.Center.y - halfSize, octant.Center.z - halfSize); //translate to zero           

            var x = point.x - translationVec.x;
            var y = point.y - translationVec.y;
            var z = point.z - translationVec.z;

            var indexX = (int)((x * 2.0) / octant.Size);
            var indexY = (int)((y * 2.0) / octant.Size);
            var indexZ = (int)((z * 2.0) / octant.Size);

            bitArray[0] = indexX == 1;
            bitArray[1] = indexZ == 1;
            bitArray[2] = indexY == 1;            

            bitArray.CopyTo(resultArray, 0);

            return resultArray[0];           
        }

        private PtOctant<TPoint> CreateChild(PtOctant<TPoint> parent, int posInParent)
        {
            double3 childCenter;
            var childsHalfSize = parent.Size / 4d;
            switch (posInParent)
            {
                default:
                case 0:
                    childCenter = new double3(parent.Center.x - childsHalfSize, parent.Center.y - childsHalfSize, parent.Center.z - childsHalfSize);
                    break;
                case 1:
                    childCenter = new double3(parent.Center.x + childsHalfSize, parent.Center.y - childsHalfSize, parent.Center.z - childsHalfSize);
                    break;
                case 2:
                    childCenter = new double3(parent.Center.x - childsHalfSize, parent.Center.y - childsHalfSize, parent.Center.z + childsHalfSize);
                    break;
                case 3:
                    childCenter = new double3(parent.Center.x + childsHalfSize, parent.Center.y - childsHalfSize, parent.Center.z + childsHalfSize);
                    break;
                case 4:
                    childCenter = new double3(parent.Center.x - childsHalfSize, parent.Center.y + childsHalfSize, parent.Center.z - childsHalfSize);
                    break;
                case 5:
                    childCenter = new double3(parent.Center.x + childsHalfSize, parent.Center.y + childsHalfSize, parent.Center.z - childsHalfSize);
                    break;
                case 6:
                    childCenter = new double3(parent.Center.x - childsHalfSize, parent.Center.y + childsHalfSize, parent.Center.z + childsHalfSize);
                    break;
                case 7:
                    childCenter = new double3(parent.Center.x + childsHalfSize, parent.Center.y + childsHalfSize, parent.Center.z + childsHalfSize);
                    break;
            }

            var childRes = parent.Size / 2d;
            var child = new PtOctant<TPoint>(childCenter, childRes)
            {
                Resolution = parent.Resolution / 2d,
                Level = parent.Level + 1
            };

            if (MaxLevel < child.Level)
                MaxLevel = child.Level;

            return child;
        }

        public static IEnumerable<TPoint> GetPointsFromGrid(PtOctant<TPoint> octant)
        {
            foreach (var cell in octant.Grid.GridCells)
            { 
                if (cell == null) continue;
                if (cell.Occupant != null)
                    yield return cell.Occupant;
            }
        }        

    }
}
