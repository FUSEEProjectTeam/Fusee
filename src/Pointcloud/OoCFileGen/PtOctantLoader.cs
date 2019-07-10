using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fusee.Xene;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    public class PtOctantLoader<TPoint> where TPoint : new()
    {
        public RenderContext RC;

        public SceneNodeContainer RootNode { get; private set; }
        public List<SceneNodeContainer> VisibleNodes = new List<SceneNodeContainer>();
        private readonly Dictionary<Guid, IEnumerable<Mesh>> LoadedMeshs;

        private readonly SortedDictionary<double, SceneNodeContainer> _nodesOrderedByProjectionSize;        
        private List<PtOctant<TPoint>> _determinedAsVisible; // per traversal
        private int _numberOfVisiblePoints;
        private readonly string _fileFolderPath;

        #region Traversal Properties

        private readonly int TraversalInterval = 200; // in ms

        // Maximal number of points that are visible in one frame - tradeoff between performance and quality
        public int PointThreshold = 100000;

        // Minimal screen projected size of a node. Depends on spacing of the octree.
        private readonly double _minScreenProjectedSize = 128;

        #endregion

        public PtOctantLoader(SceneNodeContainer rootNode, string fileFolderPath, RenderContext rc)
        {
            LoadedMeshs = new Dictionary<Guid, IEnumerable<Mesh>>();
            _nodesOrderedByProjectionSize = new SortedDictionary<double, SceneNodeContainer>(); // visible nodes ordered by screen-projected-size;
            RC = rc;
            RootNode = rootNode;
            _nodesOrderedByProjectionSize = new SortedDictionary<double, SceneNodeContainer>();            
            _fileFolderPath = fileFolderPath;
        }

        /// <summary>
        /// Traverses the scene nodes the point cloud is stored in and searches for nodes in screen-projected-size order.
        /// </summary>
        /// <param name="ptAccessor">PointAccessor, needed to load the actual points.</param>
        /// <param name="GetMeshsForNode">User-given Function that defines how to create the mesh for a scene node.</param>
        public void TraverseByProjectedSizeOrder(PointAccessor<TPoint> ptAccessor, Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode)
        {
            if (RC.Projection == float4x4.Identity || RC.View == float4x4.Identity) return;

            VisibleNodes.Clear();
            _nodesOrderedByProjectionSize.Clear();

            var fov = (float)RC.ViewportWidth / RC.ViewportHeight;
            var rootPtOctantComp = RootNode.GetComponent<PtOctantComponent>();

            ProcessNode(ptAccessor, RootNode, fov, GetMeshsForNode);          

            while (_nodesOrderedByProjectionSize.Count > 0 && _numberOfVisiblePoints <= PointThreshold)
            {
                // choose the nodes with the biggest screen size overall to process next
                var kvp = _nodesOrderedByProjectionSize.Last();
                var biggestNode = kvp.Value;                
                _nodesOrderedByProjectionSize.Remove(kvp.Key);                
                ProcessChildren(ptAccessor, biggestNode, fov, GetMeshsForNode);
            }
        }

        /// <summary>
        /// Iterates the VisibleNodes list and sets the mesh from LoadedMeshes
        /// </summary>
        /// <param name="scene">The scene that contains the point cloud and the wireframe cubes. Only needed to visualize the octants.</param>
        /// <param name="wfc">A wireframe cube. Only needed to visualize the octants.</param>
        /// <param name="effect">Shader effect for rendering the wireframe cubes.</param>
        public void SetMeshes(SceneContainer scene, WireframeCube wfc, ShaderEffect effect)
        {
            scene.Children.RemoveAll(node => node.Name == "WireframeCube");

            foreach (var node in VisibleNodes)
            {
                var ptOctantComp = node.GetComponent<PtOctantComponent>();

                if (LoadedMeshs.TryGetValue(ptOctantComp.Guid, out var loadedMeshes))
                {
                    if (node.GetComponents<Mesh>().ToList().Count != 0) continue;
                                         
                    node.Components.AddRange(loadedMeshes);                    

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

        private void ProcessChildren(PointAccessor<TPoint> ptAccessor, SceneNodeContainer node, float fov, Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode)
        {
            var ptOctantComp = node.GetComponent<PtOctantComponent>();

            if (ptOctantComp.IsLeaf) return;

            // add child nodes to the heap of ordered nodes
            foreach (var child in node.Children)
            {
                if (child == null)
                    continue;

                ProcessNode(ptAccessor, child, fov, GetMeshsForNode);
            }
        }

        private void ProcessNode(PointAccessor<TPoint> ptAccessor, SceneNodeContainer node, float fov, Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode)
        {
            var ptOctantChildComp = node.GetComponent<PtOctantComponent>();

            if (!ptOctantChildComp.Intersects(RC.ModelViewProjection))
            {
                node.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));
                node.RemoveComponentsInChildren<Mesh>();
                _numberOfVisiblePoints -= ptOctantChildComp.NumberOfPointsInNode;
                if (_numberOfVisiblePoints < 0)
                    _numberOfVisiblePoints = 0;
                return;
            }

            var camPos = RC.View.Invert().Column3;
            var camPosD = new double3(camPos.x, camPos.y, camPos.z);

            // gets pixel radius of the node
            ptOctantChildComp.ComputeScreenProjectedSize(camPosD, RC.ViewportHeight, fov);

            //_minScreenProjectedSize = ptOctantChildComp.Size;

            if (ptOctantChildComp.ProjectedScreenSize < _minScreenProjectedSize)
                return;

            if (!ptOctantChildComp.WasLoaded)
            {
                var pts = LoadPointsForNode(ptAccessor, ptOctantChildComp);
                ptOctantChildComp.NumberOfPointsInNode = pts.Count;
                var meshes = GetMeshsForNode(ptAccessor, pts);
                LoadedMeshs.Add(ptOctantChildComp.Guid, meshes);
                ptOctantChildComp.WasLoaded = true;
            }

            // by chance two same nodes have the same screen-projected-size; it's a pity we can't add it (because it's not allowed to have the same key twice)
            if (!_nodesOrderedByProjectionSize.ContainsKey(ptOctantChildComp.ProjectedScreenSize))
            {
                _nodesOrderedByProjectionSize.Add(ptOctantChildComp.ProjectedScreenSize, node);
                _numberOfVisiblePoints += ptOctantChildComp.NumberOfPointsInNode;
                VisibleNodes.Add(node);
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
            node.RemoveComponent<Mesh>();
            node.RemoveComponentsInChildren<Mesh>();
        }
        
    }
}
