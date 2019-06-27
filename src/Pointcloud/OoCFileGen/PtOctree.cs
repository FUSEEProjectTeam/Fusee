using Fusee.Engine.Core;
using Fusee.Math.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Pointcloud.OoCFileGen
{
    public class PtOctant<TPoint> : Octant<TPoint>
    {
        //The Resolution of an Octant is defined by the minimum distance (spacing) between points.
        //If the minimum distance between a point and its nearest neighbour is smaller then this distance, it will fall into a child octant.
        public double Resolution;

        public PtGrid<TPoint> Grid;

        public bool IsLeaf { get; internal set; }

        public Guid Guid { get; private set; }


        public PtOctant(double3 center, double size, Octant<TPoint>[] children = null)
        {
            Guid = Guid.NewGuid();

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

        private static BitArray _getChildIdxBitArray = new BitArray(3);
        private static int[] _getChildIdxResultArray = new int[1];

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
            var rootGrid = new PtGrid<TPoint>(PtAccessor, root, points);
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
                var childGrid = new PtGrid<TPoint>(PtAccessor, child, point);
                child.Grid = childGrid;
                octant.Children[posInParent] = child;                
            }
            else
            {
                var firstCenter = PtGrid<TPoint>.CalcCenterOfUpperLeftCell(octant);
                child = (PtOctant<TPoint>)octant.Children[posInParent];
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

        //public void Traverse(Octant<TPoint>[] children, Action<Octant<TPoint>> DoAction)
        //{
        //    if (children == null)
        //        return;

        //    foreach (var node in children)
        //    {
        //        DoAction(node);
        //        Traverse(node.Children, DoAction);
        //    }          

        //}

        /// <summary>
        /// Starts traversing from root. For starting from another node, use the static methods of <see cref="OctreeTraverser"/>.
        /// </summary>
        public void Traverse(Action<PtOctant<TPoint>> callback)
        {
            DoTraverse(Root, callback);
        }

        private static void DoTraverse(PtOctant<TPoint> node, Action<PtOctant<TPoint>> callback)
        {
            var candidates = new Stack<PtOctant<TPoint>>();
            candidates.Push(node);

            while (candidates.Count > 0)
            {
                node = candidates.Pop();
                callback(node);

                // add children as candidates

                IterateChildren(node, (PtOctant<TPoint> childNode) =>
                {
                    candidates.Push(childNode);
                });
            }
        }

        /// <summary>
        /// Iterates through the child node and calls for each child the given action.
        /// </summary>        
        private static void IterateChildren(PtOctant<TPoint> parent, Action<PtOctant<TPoint>> iterateAction)
        {
            if (parent.Children != null)
            {                
                foreach (var child in parent.Children)
                {
                    if (child != null)
                        iterateAction?.Invoke((PtOctant<TPoint>)child);
                }
                
            }
        }

    }
}
