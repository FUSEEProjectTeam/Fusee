using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using System;
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
        public Dictionary<int3, GridCell<TPoint>> GridCells;

        private List<int3> _neighbouCellIdxOffsets;

        public Grid(GridPtAccessor<TPoint> ptAccessor, PtOctant<TPoint> parentOctant, TPoint point)
        {
            _neighbouCellIdxOffsets = GetGridNeighbourIndices(1);
            GridCells = new Dictionary<int3, GridCell<TPoint>>(128 * 128 * 128);

            var lowerLeft = parentOctant.Center - new double3(parentOctant.Size / 2d, parentOctant.Size / 2d, parentOctant.Size / 2d);
            var firstCenter = new double3(lowerLeft.x + (parentOctant.Resolution / 2d), lowerLeft.y + (parentOctant.Resolution / 2d), lowerLeft.z + (parentOctant.Resolution / 2d));
            //create GridCell Array
            for (var x = 0; x < 128; x++)
            {
                for (var y = 0; y < 128; y++)
                {
                    for (var z = 0; z < 128; z++)
                    {
                        var center = new double3(firstCenter.x + parentOctant.Resolution * x, firstCenter.y + parentOctant.Resolution * y, firstCenter.z + parentOctant.Resolution * z);
                        GridCells.Add(new int3(x, y, z), new GridCell<TPoint>(center, parentOctant.Resolution));
                    }
                }
            }

            ReadPointToGrid(ptAccessor, parentOctant, point);
        }

        public Grid(GridPtAccessor<TPoint> ptAccessor, PtOctant<TPoint> parentOctant, List<TPoint> points)
        {
            _neighbouCellIdxOffsets = GetGridNeighbourIndices(1);
            GridCells = new Dictionary<int3, GridCell<TPoint>>(128 * 128 * 128);

            var lowerLeft = parentOctant.Center - new double3(parentOctant.Size / 2d, parentOctant.Size / 2d, parentOctant.Size / 2d);
            var firstCenter = new double3(lowerLeft.x + (parentOctant.Resolution / 2d), lowerLeft.y + (parentOctant.Resolution / 2d), lowerLeft.z + (parentOctant.Resolution / 2d));
            //create GridCell Array
            for (var x = 0; x < 128; x++)
            {
                for (var y = 0; y < 128; y++)
                {
                    for (var z = 0; z < 128; z++)
                    {
                        var center = new double3(firstCenter.x + parentOctant.Resolution * x, firstCenter.y + parentOctant.Resolution * y, firstCenter.z + parentOctant.Resolution * z);
                        GridCells.Add(new int3(x, y, z), new GridCell<TPoint>(center, parentOctant.Resolution));
                    }
                }
            }

            for (int i = 0; i < points.Count; i++)
            {
                TPoint pt = points[i];
                ReadPointToGrid(ptAccessor, parentOctant, pt);
                points[i] = pt;
            }
        }

        //see https://math.stackexchange.com/questions/528501/how-to-determine-which-cell-in-a-grid-a-point-belongs-to
        public void ReadPointToGrid(GridPtAccessor<TPoint> ptAccessor, PtOctant<TPoint> parentOctant, TPoint point)
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

            GridCells.TryGetValue(new int3(indexX, indexY, indexZ), out var cell);

            //check if NN is too close            
            foreach (var idxOffset in _neighbouCellIdxOffsets)
            { 
                var neighbourCellIdx = new int3(indexX, indexY, indexZ) + idxOffset;

                if (neighbourCellIdx.x < 0 || neighbourCellIdx.x > 127)
                    continue;
                if (neighbourCellIdx.y < 0 || neighbourCellIdx.y > 127)
                    continue;
                if (neighbourCellIdx.z < 0 || neighbourCellIdx.z > 127)
                    continue;

                GridCells.TryGetValue(neighbourCellIdx, out var neighbourCell);

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
                child = (PtOctant<TPoint>)octant.Children[posInParent];
                child.Grid.ReadPointToGrid(PtAccessor, child, point);
            } 
        }

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

            return Convert.ToInt32(indexY.ToString() + indexZ + indexX, 2);
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

        public static IEnumerable<TPoint> GetPointsFromGrid<TPoint>(PtOctant<TPoint> octant)
        {
            foreach (var cell in octant.Grid.GridCells.Values)
            {
                if(cell.Occupant != null)
                    yield return cell.Occupant;
            }

        }

    }
}
