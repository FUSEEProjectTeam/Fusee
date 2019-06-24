using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Pointcloud.OoCFileGen
{

    public struct int3
    {
        public int x;
        public int y;
        public int z;

        public int3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

        }
    }

    public class GridPtAccessor<TPoint> : PointAccessor<TPoint>
    {
        public bool HasGridIdx = true;

        public virtual ref int3 GetGridIdx(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetPositionFloat32");
        }

        public virtual void SetGridIndex(ref TPoint point, int3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetPositionFloat32");
        }
    }

    public class Grid<TPoint>
    {
        public Bucket<TPoint>[,,] GridCells;

        public Grid(GridPtAccessor<TPoint> ptAccessor, double3 parentOctantCenter, double resolution, List<TPoint> gridPoints)
        {
            //create GridCell Array
            for (var x = 0; x < 128; x++)
            {
                for (var y = 0; y < 128; y++)
                {
                    for (var z = 0; z < 128; z++)
                    {
                        var parentRes = resolution * 2;
                        var startPos = new double3((parentOctantCenter.x - parentRes) + resolution / 2, (parentOctantCenter.y - parentRes) + resolution / 2, (parentOctantCenter.z - parentRes) + resolution / 2);
                        var center = new double3(startPos.x * x, startPos.x * y, startPos.x * z);
                        GridCells[x, y, z] = new Bucket<TPoint>(center, resolution);
                    }
                }
            }

            //fill with gridPoints
            ReadPointsToGrid(ptAccessor, resolution, parentOctantCenter, gridPoints);
        }

        //see https://math.stackexchange.com/questions/528501/how-to-determine-which-cell-in-a-grid-a-point-belongs-to
        public void ReadPointsToGrid(GridPtAccessor<TPoint> ptAccessor, double resolution, double3 center, List<TPoint> points)
        {
            var halfSize = resolution / 2d;
            var translationVec = new double3(center.x - halfSize, center.y - halfSize, center.z - halfSize); //translate to zero

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var tPointPos = ptAccessor.GetPositionFloat3_64(ref point);

                var x = tPointPos.x - translationVec.x;
                var y = tPointPos.y - translationVec.y;
                var z = tPointPos.z - translationVec.z;

                var indexX = (int)((x * 128) / resolution);
                var indexY = (int)((y * 128) / resolution);
                var indexZ = (int)((z * 128) / resolution);

                var cell = GridCells[indexX, indexY, indexZ];
                cell.Payload.Add(point);
                ptAccessor.SetGridIndex(ref point, new int3(indexX, indexY, indexZ));
            }
        }

        public static List<int3> GetGridNeighbourIndices(int dist)
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
        //public PointAccessor<TPoint> PtAccessor { get; private set; }

        public TPoint Occupant;

        //The Resolution of an Octant is defined by the minimum distance (spacing) between points.
        //If the minimum distance between a point and its nearest neighbour is smaller then this distance, it will fall into a child octant.
        public double Resolution;

        public Grid<TPoint> Grid;

        public PtOctant(double3 center, double size, Grid<TPoint> grid, List<TPoint> payload = null, Octant<TPoint>[] children = null)
        {

            Center = center;
            Size = size;

            if (children == null)
                Children = new PtOctant<TPoint>[8];
            else
                Children = children;

            Payload = payload;
        }
    }


    public class PtOctree<TCollection, TPoint> : IOctree<TPoint>
    {
        public int MaxNoOfPointsInBucket { get; private set; }

        public GridPtAccessor<TPoint> PtAccessor { get; private set; }

        public Octant<TPoint> Root { get; set; }

        private List<int3> _gridCellNeighbourIndices;

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

            var res = aabbMaxLength / 128d; //spacing
            var rootGrid = new Grid<TPoint>(PtAccessor, aabb.Center, res, points);
            var root = new PtOctant<TPoint>(aabb.Center, aabbMaxLength, rootGrid)
            {
                Resolution = res
            };

            Root = root;

            _gridCellNeighbourIndices = Grid<TPoint>.GetGridNeighbourIndices(1);

            Subdivide(Root);//Initial subdiv
        }

        public void Subdivide(Octant<TPoint> octant)
        {
            //counts the number of points a child octant would have
            var child0Cnt = 0;
            var child1Cnt = 0;
            var child2Cnt = 0;
            var child3Cnt = 0;
            var child4Cnt = 0;
            var child5Cnt = 0;
            var child6Cnt = 0;
            var child7Cnt = 0;

            //saves the index of the child octant a point would fall into
            var childIndices = new int[octant.Payload.Count];

            for (int i = 0; i < octant.Payload.Count; i++)
            {
                var point = octant.Payload[i];
                var gridIdx = PtAccessor.GetGridIdx(ref point);

                //check if neighbour gridCell has point with dist < res
                foreach (var neighbourCellIdx in _gridCellNeighbourIndices)
                {
                    var ptOctant = (PtOctant<TPoint>)octant;
                    var neighbourCell = ptOctant.Grid.GridCells[gridIdx.x + neighbourCellIdx.x, gridIdx.y + neighbourCellIdx.y, gridIdx.z + neighbourCellIdx.z];

                    for (int k = 0; k < neighbourCell.Payload.Count; k++)
                    {
                        var pt = neighbourCell.Payload[i];
                        var neighbouPtPos = PtAccessor.GetPositionFloat3_64(ref pt);
                        var pointPos = PtAccessor.GetPositionFloat3_64(ref point);

                        var dist = (pointPos - neighbouPtPos).Length;

                        //if a point is closer as minimum distance it falls into a child
                        if (dist <= ptOctant.Resolution)
                        {
                            var childOctantIdx = GetChildIndexToWritePoint(octant, pointPos);
                            childIndices[i] = childOctantIdx;

                            //but only create child if it would have more then the max number of points --> a Octant does not nessesarily have 8 children!!
                            //if one of the lists reaches the max amount of points --> create child bucket
                            switch (childOctantIdx)
                            {
                                default:
                                case 0:
                                    child0Cnt++;
                                    if (child0Cnt > MaxNoOfPointsInBucket)
                                    {
                                        //get points from payload and childIndices
                                        var childPayload = GetPayload(0, octant.Payload, childIndices).ToList();
                                        var childOctant = CreateChild(ptOctant, 0, childPayload);
                                        octant.Children[0] = childOctant;
                                        Subdivide(childOctant);
                                    }

                                    break;
                                case 1:
                                    child1Cnt++;
                                    if (child1Cnt > MaxNoOfPointsInBucket)
                                    {
                                        //get points from payload and childIndices
                                        var childPayload = GetPayload(1, octant.Payload, childIndices).ToList();
                                        var childOctant = CreateChild(ptOctant, 1, childPayload);
                                        octant.Children[1] = childOctant;
                                        Subdivide(childOctant);
                                    }
                                    break;
                                case 2:
                                    child2Cnt++;
                                    if (child2Cnt > MaxNoOfPointsInBucket)
                                    {
                                        //get points from payload and childIndices
                                        var childPayload = GetPayload(2, octant.Payload, childIndices).ToList();
                                        var childOctant = CreateChild(ptOctant, 2, childPayload);
                                        octant.Children[2] = childOctant;
                                        Subdivide(childOctant);
                                    }
                                    break;
                                case 3:
                                    child3Cnt++;
                                    if (child3Cnt > MaxNoOfPointsInBucket)
                                    {
                                        //get points from payload and childIndices
                                        var childPayload = GetPayload(3, octant.Payload, childIndices).ToList();
                                        var childOctant = CreateChild(ptOctant, 3, childPayload);
                                        octant.Children[3] = childOctant;
                                        Subdivide(childOctant);
                                    }
                                    break;
                                case 4:
                                    child4Cnt++;
                                    if (child4Cnt > MaxNoOfPointsInBucket)
                                    {
                                        //get points from payload and childIndices
                                        var childPayload = GetPayload(4, octant.Payload, childIndices).ToList();
                                        var childOctant = CreateChild(ptOctant, 4, childPayload);
                                        octant.Children[4] = childOctant;
                                        Subdivide(childOctant);
                                    }
                                    break;
                                case 5:
                                    child5Cnt++;
                                    if (child5Cnt > MaxNoOfPointsInBucket)
                                    {
                                        //get points from payload and childIndices
                                        var childPayload = GetPayload(5, octant.Payload, childIndices).ToList();
                                        var childOctant = CreateChild(ptOctant, 5, childPayload);
                                        octant.Children[5] = childOctant;
                                        Subdivide(childOctant);
                                    }
                                    break;
                                case 6:
                                    child6Cnt++;
                                    if (child5Cnt > MaxNoOfPointsInBucket)
                                    {
                                        //get points from payload and childIndices
                                        var childPayload = GetPayload(6, octant.Payload, childIndices).ToList();
                                        var childOctant = CreateChild(ptOctant, 6, childPayload);
                                        octant.Children[6] = childOctant;
                                        Subdivide(childOctant);
                                    }
                                    break;
                                case 7:
                                    child7Cnt++;
                                    if (child5Cnt > MaxNoOfPointsInBucket)
                                    {
                                        //get points from payload and childIndices
                                        var childPayload = GetPayload(7, octant.Payload, childIndices).ToList();
                                        var childOctant = CreateChild(ptOctant, 7, childPayload);
                                        octant.Children[7] = childOctant;
                                        Subdivide(childOctant);
                                    }
                                    break;
                            }
                        }

                    }
                }
            }

        }



        private IEnumerable<TPoint> GetPayload(int childIdx, List<TPoint> parentPayload, int[] childIndices)
        {
            for (int i = 0; i < childIndices.Length; i++)
            {
                var thisPointsChildIdx = childIndices[i];
                if (thisPointsChildIdx == childIdx)
                    yield return parentPayload[i];
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

        private PtOctant<TPoint> CreateChild(PtOctant<TPoint> parent, int posInParent, List<TPoint> points)
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
            var childGrid = new Grid<TPoint>(PtAccessor, childCenter, childRes, points);
            var child = new PtOctant<TPoint>(childCenter, childRes, childGrid);
            child.Resolution = parent.Resolution / 2d;

            return child;
        }

        public void Traverse()
        {
            throw new NotImplementedException();
        }
    }
}
