using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    public class OoCOctantLoader<TPoint> where TPoint : new()
    {
        public RenderContext RC;

        public PtOctree<TPoint> Octree { get; private set; }
        public List<PtOctant<TPoint>> VisibleNodes = new List<PtOctant<TPoint>>();

        private SortedDictionary<double, PtOctant<TPoint>> _nodesOrderedByProjectionSize = new SortedDictionary<double, PtOctant<TPoint>>(); // visible nodes ordered by screen-projected-size        
        private List<PtOctant<TPoint>> _determinedAsVisible; // per traversal
        private int _numberOfVisiblePoints;
        private string _fileFolderPath;

        #region Traversal Properties

        private readonly int TraversalInterval = 200; // in ms

        // number of points that are visible at one frame, tradeoff between performance and quality
        public int PointThreshold = 100000;

        // ... of a node. Depends on spacing of the octree, see constructors.
        private double _minScreenProjectedSize;

        #endregion

        public OoCOctantLoader(PtOctree<TPoint> octree, string fileFolderPath)
        {
            Octree = octree;
            _nodesOrderedByProjectionSize = new SortedDictionary<double, PtOctant<TPoint>>();            
            _minScreenProjectedSize = 1d / (octree.Root.Resolution); // = number of points -> number of pixels
            _fileFolderPath = fileFolderPath;
        }

        /// <summary>
        /// Updates the rc. Call it in render a frame.
        /// </summary>
        /// <param name="rc">The RenderContext.</param>
        public void UpdateRC(RenderContext rc)
        {
            RC = rc;
        }

        /// <summary>
        /// Traverses the octree and searches for nodes in screen-projected-size order.
        /// </summary>
        public void TraverseByProjectedSizeOrder(PointAccessor<TPoint> ptAccessor)
        {
            _numberOfVisiblePoints = 0;
            VisibleNodes.Clear();
            _nodesOrderedByProjectionSize.Clear();

            var fov = RC.ViewportWidth / RC.ViewportHeight;

            if (Octree.Root.Intersects(RC.ModelViewProjection))
                VisibleNodes.Add(Octree.Root);

            ProcessNode(ptAccessor, Octree.Root, fov);          

            while (_nodesOrderedByProjectionSize.Count > 0 && _numberOfVisiblePoints <= PointThreshold)
            {
                // choose the nodes with the biggest screen size overall to process next
                var kvp = _nodesOrderedByProjectionSize.First();
                var biggestNode = kvp.Value;                
                _nodesOrderedByProjectionSize.Remove(kvp.Key);                
                ProcessNode(ptAccessor, biggestNode, fov);
            }
        }

        /// <summary>
        /// Sub function which calculates the screen-projected-size and adds it to the heap of nodesOrdered by sps.        
        /// </summary>
        /// <param name="node">The node to compute the pss for.</param>
        private void ProcessNode(PointAccessor<TPoint> ptAccessor, PtOctant<TPoint> node, float fov)
        {
            //check if node was loaded - if not, load
            if (!node.WasLoaded)
                node.Payload = LoadPointsForNode(ptAccessor, node);

            if (node.IsLeaf) return;

            // add child nodes to the heap of ordered nodes
            foreach (var child in node.Children)
            {
                if (child == null)
                    continue;

                var ptChild = (PtOctant<TPoint>)child;

                if (!ptChild.Intersects(RC.ModelViewProjection))
                {
                    continue;
                }
                               
                var camPos = RC.View.Invert().Column3;
                var camPosD = new double3(camPos.x, camPos.y, camPos.z);

                // gets pixel radius of the node
                var projectedSize = ptChild.ComputeScreenProjectedSize(camPosD, RC.ViewportHeight, fov);

                if (projectedSize < _minScreenProjectedSize)                
                    continue;

                // by chance two same nodes have the same screen-projected-size; it's such a pitty we can't add it (because it's not allowed to have the same key twice)
                if (!_nodesOrderedByProjectionSize.ContainsKey(projectedSize))
                {
                    _nodesOrderedByProjectionSize.Add(projectedSize, ptChild);
                    VisibleNodes.Add(ptChild);
                    _numberOfVisiblePoints += ptChild.Payload.Count;
                }
                
            }

        }

        private List<TPoint> LoadPointsForNode(PointAccessor<TPoint> ptAccessor, PtOctant<TPoint> node)
        {
            string pathToFile = _fileFolderPath + "/Octants/" + node.Guid.ToString("N") + ".node";

            if (!File.Exists(pathToFile))
                throw new ArgumentException("File: " + node.Guid + ".node does not exist!");

            using (BinaryReader br = new BinaryReader(File.Open(pathToFile, FileMode.Open, FileAccess.Read)))
            {
                // step to stream position
                //br.BaseStream.Position = node.StreamPosition;

                // read number of points
                var numberOfPoints = br.ReadInt32();
                var lengthOfPoint = br.ReadInt32();

                List<TPoint> points = new List<TPoint>(numberOfPoints);

                for (var i = 0; i < numberOfPoints; i++)
                {
                    var pt = new TPoint();
                    var ptBytes = br.ReadBytes(lengthOfPoint);

                    ptAccessor.SetRawPoint(ref pt, ptBytes);

                    points.Add(pt);
                }

                node.WasLoaded = true;

                return points;
            }

        }

    }
}
