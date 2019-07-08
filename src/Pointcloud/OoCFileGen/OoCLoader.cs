using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fusee.Xene;
using Fusee.Engine.Common;
using Fusee.Base.Core;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    public class OoCOctantLoader<TPoint> where TPoint : new()
    {
        public RenderContext RC;

        //public PtOctree<TPoint> Octree { get; private set; }
        public SceneNodeContainer RootNode { get; private set; }
        public List<SceneNodeContainer> VisibleNodes = new List<SceneNodeContainer>();
        private Dictionary<Guid, IEnumerable<Mesh>> LoadedMeshs = new Dictionary<Guid, IEnumerable<Mesh>>();

        private SortedDictionary<double, SceneNodeContainer> _nodesOrderedByProjectionSize = new SortedDictionary<double, SceneNodeContainer>(); // visible nodes ordered by screen-projected-size        
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

        public OoCOctantLoader(SceneNodeContainer rootNode, string fileFolderPath, RenderContext rc)
        {
            //Octree = octree;
            RC = rc;
            RootNode = rootNode;
            _nodesOrderedByProjectionSize = new SortedDictionary<double, SceneNodeContainer>();            
            _minScreenProjectedSize = RootNode.GetComponent<PtOctantComponent>().Resolution; // = number of points -> number of pixels
            _fileFolderPath = fileFolderPath;
        }

        /// <summary>
        /// Traverses the octree and searches for nodes in screen-projected-size order.
        /// </summary>
        public void TraverseByProjectedSizeOrder(PointAccessor<TPoint> ptAccessor, Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode)
        {
            if (RC.Projection == float4x4.Identity || RC.View == float4x4.Identity) return;

            VisibleNodes.Clear();
            _nodesOrderedByProjectionSize.Clear();

            var fov = (float)RC.ViewportWidth / RC.ViewportHeight;
            var rootPtOctantComp = RootNode.GetComponent<PtOctantComponent>();

            if (!rootPtOctantComp.WasLoaded)
            {
                var pts = LoadPointsForNode(ptAccessor, rootPtOctantComp);
                rootPtOctantComp.NumberOfPointsInNode = pts.Count;
                var meshes = GetMeshsForNode(ptAccessor, pts);
                LoadedMeshs.Add(rootPtOctantComp.Guid, meshes);
                rootPtOctantComp.WasLoaded = true;
            }

            if (rootPtOctantComp.Intersects(RC.ModelViewProjection))
                VisibleNodes.Add(RootNode);
            else
            {
                RootNode.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));
                RootNode.RemoveComponentsInChildren<Mesh>();
                _numberOfVisiblePoints -= rootPtOctantComp.NumberOfPointsInNode;
                if (_numberOfVisiblePoints < 0)
                    _numberOfVisiblePoints = 0;
            }

            ProcessNode(ptAccessor, RootNode, fov, GetMeshsForNode);          

            while (_nodesOrderedByProjectionSize.Count > 0 && _numberOfVisiblePoints <= PointThreshold)
            {
                // choose the nodes with the biggest screen size overall to process next
                var kvp = _nodesOrderedByProjectionSize.Last();
                var biggestNode = kvp.Value;                
                _nodesOrderedByProjectionSize.Remove(kvp.Key);                
                ProcessNode(ptAccessor, biggestNode, fov, GetMeshsForNode);
            }
        }

        /// <summary>
        /// Sub function which calculates the screen-projected-size and adds it to the heap of nodesOrdered by sps.        
        /// </summary>
        /// <param name="node">The node to compute the pss for.</param>
        private void ProcessNode(PointAccessor<TPoint> ptAccessor, SceneNodeContainer node, float fov, Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode)
        {
            var ptOctantComp = node.GetComponent<PtOctantComponent>();

            if (ptOctantComp.IsLeaf) return;

            // add child nodes to the heap of ordered nodes
            foreach (var child in node.Children)
            {
                if (child == null)
                    continue;

                var ptOctantChildComp = child.GetComponent<PtOctantComponent>();

                if (!ptOctantChildComp.Intersects(RC.ModelViewProjection))
                {
                    child.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));
                    child.RemoveComponentsInChildren<Mesh>();
                    _numberOfVisiblePoints -= ptOctantChildComp.NumberOfPointsInNode;
                    if (_numberOfVisiblePoints < 0)
                        _numberOfVisiblePoints = 0;
                    continue;
                }
                               
                var camPos = RC.View.Invert().Column3;
                var camPosD = new double3(camPos.x, camPos.y, camPos.z);

                // gets pixel radius of the node
                var projectedSize = ptOctantChildComp.ComputeScreenProjectedSize(camPosD, RC.ViewportHeight, fov);

                if (projectedSize < _minScreenProjectedSize)                
                    continue;

                if (!ptOctantChildComp.WasLoaded)
                {
                    var pts = LoadPointsForNode(ptAccessor, ptOctantChildComp);
                    ptOctantChildComp.NumberOfPointsInNode = pts.Count;
                    var meshes = GetMeshsForNode(ptAccessor, pts);
                    LoadedMeshs.Add(ptOctantChildComp.Guid, meshes);
                    ptOctantChildComp.WasLoaded = true;
                }

                // by chance two same nodes have the same screen-projected-size; it's such a pitty we can't add it (because it's not allowed to have the same key twice)
                if (!_nodesOrderedByProjectionSize.ContainsKey(projectedSize))
                {
                    _nodesOrderedByProjectionSize.Add(projectedSize, child);
                    _numberOfVisiblePoints += ptOctantChildComp.NumberOfPointsInNode;
                    VisibleNodes.Add(child);
                }
            }
        }

        long loadedMesh = 0;

        public void SetMeshes(SceneContainer scene, WireframeCube wfc, ShaderEffect effect)
        {
            scene.Children.RemoveAll(node => node.Name == "WireframeCube");

            foreach (var node in VisibleNodes)
            {
                var ptOctantComp = node.GetComponent<PtOctantComponent>();

                if (LoadedMeshs.TryGetValue(ptOctantComp.Guid, out var loadedMeshes))
                {
                    if (node.GetComponents<Mesh>().ToList().Count == 0)
                    {
                        //Diagnostics.Log($"Mesh loaded {loadedMesh += loadedMeshes.Count()}");
                        //Diagnostics.Log($"Current components cnt {node.Components.Count}");

                        node.Components.AddRange(loadedMeshes);
                    }

                    scene.Children.Add(new SceneNodeContainer()
                    {
                        Name = "WireframeCube",
                        Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent()
                            {
                                Translation = (float3)ptOctantComp.Center,
                                Scale = float3.One * (float)ptOctantComp.Size
                            },
                            new ShaderEffectComponent()
                            {
                                Effect = effect
                            },
                            wfc
                        }
                    });
                }
                else
                {
                    throw new ArgumentException("Trying to set meshes that are not loaded yet!");
                }
            }
        }        

        private List<TPoint> LoadPointsForNode(PointAccessor<TPoint> ptAccessor, PtOctantComponent ptOctantComponent)
        {
            var pathToFile = _fileFolderPath + "/Octants/" + ptOctantComponent.Guid.ToString("N") + ".node";

            if (!File.Exists(pathToFile))
                throw new ArgumentException("File: " + ptOctantComponent.Guid + ".node does not exist!");

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

                ptOctantComponent.WasLoaded = true;

                return points;
            }

        }

        public void TraverseAndRemoveMeshes(SceneNodeContainer node)
        {
            _numberOfVisiblePoints = 0;
            loadedMesh = 0;
            node.RemoveComponent<Mesh>();
            node.RemoveComponentsInChildren<Mesh>();

            //foreach (var child in node.Children)            
            //    TraverseAndRemoveMeshes(child);
        }
    }
}
