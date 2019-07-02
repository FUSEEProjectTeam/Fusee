using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    public class PtOctantWrite<TPoint> : PtOctant<TPoint>
    {
        public PtGrid<TPoint> Grid;

        public PtOctantWrite(double3 center, double size, Octant<TPoint>[] children = null)
        {
            Guid = Guid.NewGuid();

            Center = center;
            Size = size;

            if (children == null)
                Children = new Octant<TPoint>[8];
            else
                Children = children;

            Payload = new List<TPoint>();
        }

        public new PtOctantWrite<TPoint> CreateChild(int posInParent)
        {
            var childCenter = CalcCildCenterAtPos(posInParent);

            var childRes = Size / 2d;
            var child = new PtOctantWrite<TPoint>(childCenter, childRes)
            {
                Resolution = Resolution / 2d,
                Level = Level + 1
            };
            return child;
        }
    }

    public class PtOctant<TPoint> : Octant<TPoint>
    {
        //The Resolution of an Octant is defined by the minimum distance (spacing) between points.
        //If the minimum distance between a point and its nearest neighbour is smaller then this distance, it will fall into a child octant.
        public double Resolution;

        public Guid Guid { get; set; }

        public bool WasLoaded = false; //TODO: consider making a new Octant type? A Octant that gets written to the file does not need to contain this field.

        public PtOctant(double3 center, double size, Octant<TPoint>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new Octant<TPoint>[8];
            else
                Children = children;

            Payload = new List<TPoint>();
        }
        protected PtOctant() {}

        public PtOctant<TPoint> CreateChild(int posInParent)
        {
            var childCenter = CalcCildCenterAtPos(posInParent);

            var childRes = Size / 2d;
            var child = new PtOctant<TPoint>(childCenter, childRes)
            {
                Resolution = Resolution / 2d,
                Level = Level + 1
            };
            return child;
        }

        protected double3 CalcCildCenterAtPos(int posInParent)
        {
            double3 childCenter;
            var childsHalfSize = Size / 4d;
            switch (posInParent)
            {
                default:
                case 0:
                    childCenter = new double3(Center.x - childsHalfSize, Center.y - childsHalfSize, Center.z - childsHalfSize);
                    break;
                case 1:
                    childCenter = new double3(Center.x + childsHalfSize, Center.y - childsHalfSize, Center.z - childsHalfSize);
                    break;
                case 2:
                    childCenter = new double3(Center.x - childsHalfSize, Center.y - childsHalfSize, Center.z + childsHalfSize);
                    break;
                case 3:
                    childCenter = new double3(Center.x + childsHalfSize, Center.y - childsHalfSize, Center.z + childsHalfSize);
                    break;
                case 4:
                    childCenter = new double3(Center.x - childsHalfSize, Center.y + childsHalfSize, Center.z - childsHalfSize);
                    break;
                case 5:
                    childCenter = new double3(Center.x + childsHalfSize, Center.y + childsHalfSize, Center.z - childsHalfSize);
                    break;
                case 6:
                    childCenter = new double3(Center.x - childsHalfSize, Center.y + childsHalfSize, Center.z + childsHalfSize);
                    break;
                case 7:
                    childCenter = new double3(Center.x + childsHalfSize, Center.y + childsHalfSize, Center.z + childsHalfSize);
                    break;
            }

            return childCenter;
        }

        /// <summary>
        /// Computes the current screen projected size of a Octant.
        /// </summary>
        public double ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov)
        {
            var distance = (Center - camPos).Length;            
            var slope = (float)System.Math.Tan(fov / 2f);
            var projectedSize = screenHeight / 2d * Size / (slope * distance);

            return projectedSize;
        }
    }

    public class PtOctree<TPoint>
    {
        public int MaxNoOfPointsInBucket { get; private set; }

        public PointAccessor<TPoint> PtAccessor { get; private set; }

        public PtOctant<TPoint> Root;

        public int MaxLevel;

        private static BitArray _getChildIdxBitArray = new BitArray(3);
        private static int[] _getChildIdxResultArray = new int[1];

        //Contructor for creating an Octree that is suitable for creating files from it. 
        public PtOctree(AABBd aabb, PointAccessor<TPoint> pa, List<TPoint> points, int maxNoOfPointsInBucket)
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
            var root = new PtOctantWrite<TPoint>(aabb.Center, aabbMaxLength)
            {
                Resolution = res
            };
            var rootGrid = new PtGrid<TPoint>(PtAccessor, root, points);
            root.Grid = rootGrid;
            root.Level = 0;

            Root = root;

            if (Root.Payload.Count >= MaxNoOfPointsInBucket)
            {
                Subdivide((PtOctantWrite<TPoint>)Root);//Initial subdiv
            }
        }

        public PtOctree(PtOctant<TPoint> root, PointAccessor<TPoint> pa, int maxNoOfPointsInBucket)
        {
            MaxNoOfPointsInBucket = maxNoOfPointsInBucket;
            PtAccessor = pa;
            Root = root;
        }

        public void Subdivide(PtOctantWrite<TPoint> octant)
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
                var child = (PtOctantWrite<TPoint>)octant.Children[i];
                if (child == null) continue;

                if (child.Payload.Count >= MaxNoOfPointsInBucket)
                {
                    Subdivide(child);
                }
                else                
                    child.IsLeaf = true;                
            }
        }
               
        private void CreateChildAndReadPtToGrid(int posInParent, PtOctantWrite<TPoint> octant, TPoint point)
        {
            PtOctantWrite<TPoint> child;

            if (octant.Children[posInParent] == null)
            {
                child = octant.CreateChild(posInParent) as PtOctantWrite<TPoint>;

                if (MaxLevel < child.Level)
                    MaxLevel = child.Level;

                var childGrid = new PtGrid<TPoint>(PtAccessor, child, point);
                child.Grid = childGrid;
                octant.Children[posInParent] = child;                
            }
            else
            {
                var firstCenter = PtGrid<TPoint>.CalcCenterOfUpperLeftCell(octant);
                child = (PtOctantWrite<TPoint>)octant.Children[posInParent];
                child.Grid.ReadPointToGrid(PtAccessor, child, point, firstCenter);
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

            _getChildIdxBitArray[0] = indexX == 1;
            _getChildIdxBitArray[1] = indexZ == 1;
            _getChildIdxBitArray[2] = indexY == 1;            

            _getChildIdxBitArray.CopyTo(_getChildIdxResultArray, 0);

            return _getChildIdxResultArray[0];           
        }        

        public static IEnumerable<TPoint> GetPointsFromGrid(PtOctantWrite<TPoint> octant)
        {
            foreach (var cell in octant.Grid.GridCells)
            { 
                if (cell == null) continue;
                yield return cell.Occupant;
            }
        }

        /// <summary>
        /// Starts traversing from root.>.
        /// </summary>
        public void Traverse(Action<PtOctantWrite<TPoint>> callback)
        {
            DoTraverse((PtOctantWrite<TPoint>)Root, callback);
        }

        /// <summary>
        /// Starts traversing from a given node.>.
        /// </summary>
        public void Traverse(PtOctantWrite<TPoint>node, Action<PtOctantWrite<TPoint>> callback)
        {
            DoTraverse(node, callback);
        }

        private static void DoTraverse(PtOctantWrite<TPoint> node, Action<PtOctantWrite<TPoint>> callback)
        {
            var candidates = new Stack<PtOctantWrite<TPoint>>();
            candidates.Push(node);

            while (candidates.Count > 0)
            {
                node = candidates.Pop();
                callback(node);

                // add children as candidates

                IterateChildren(node, (PtOctantWrite<TPoint> childNode) =>
                {
                    candidates.Push(childNode);
                });
            }
        }

        /// <summary>
        /// Iterates through the child node and calls for each child the given action.
        /// </summary>        
        private static void IterateChildren(PtOctantWrite<TPoint> parent, Action<PtOctantWrite<TPoint>> iterateAction)
        {
            if (parent.Children != null)
            {                
                foreach (var child in parent.Children)
                {
                    if (child != null)
                        iterateAction?.Invoke((PtOctantWrite<TPoint>)child);
                }
                
            }
        }

    }
}
