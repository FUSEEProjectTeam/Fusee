using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Structures.Octree;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    public class PtOctree<TPoint>
    {
        public int MaxNoOfPointsInBucket { get; private set; }

        public PointAccessor<TPoint> PtAccessor { get; private set; }

        public PtOctant<TPoint> Root;

        public int MaxLevel;

        private static readonly BitArray _getChildIdxBitArray = new BitArray(3);
        private static readonly int[] _getChildIdxResultArray = new int[1];

        //Constructor for creating an Octree that is suitable for creating files from it. 
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
                    Subdivide(child);                
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

        private static int GetChildIndexToWritePoint(PayloadOctantD<TPoint> octant, double3 point)
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
                for (int i = parent.Children.Length-1; i >= 0; i--)
                {
                    PayloadOctantD<TPoint> child = parent.Children[i];
                    if (child != null)
                        iterateAction?.Invoke((PtOctantWrite<TPoint>)child);
                }
                
            }
        }

    }
}
