using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Serialization;
using Fusee.Xene;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    public class PtOctantLoader<TPoint> where TPoint : new()
    {
        public PointAccessor<TPoint> PtAcc;
        public Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode;

        public bool WasSceneUpdated { get; private set; } = true;

        //Scene is only updated if the user is moving.
        public bool IsUserMoving;

        public RenderContext RC;
        public Texture VisibleOctreeHierarchyTex;

        public SceneNode RootNode
        {
            get
            {
                return _rootNode;
            }
            set
            {
                _loadedMeshs = new ConcurrentDictionary<Guid, IEnumerable<Mesh>>();
                _nodesOrderedByProjectionSize = new SortedDictionary<double, SceneNode>();
                _determinedAsVisible = new Dictionary<Guid, SceneNode>();

                var fov = (float)RC.ViewportWidth / RC.ViewportHeight;
                var camPosD = new double3(InitCamPos.x, InitCamPos.y, InitCamPos.z);

                _rootNode = value;

                // gets pixel radius of the node
                RootNode.GetComponent<PtOctant>().ComputeScreenProjectedSize(camPosD, RC.ViewportHeight, fov);
                _initRootScreenProjSize = (float)RootNode.GetComponent<PtOctant>().ProjectedScreenSize;
                _minScreenProjectedSize = _initRootScreenProjSize * _minProjSizeModifier;
            }
        }

        public int NumberOfVisiblePoints { get; private set; }

        private float _minProjSizeModifier = 1 / 3f;
        public float MinProjSizeModifier
        {
            get
            {
                return _minProjSizeModifier;
            }
            set
            {
                _minProjSizeModifier = value;
                if (RootNode != null)
                    _minScreenProjectedSize = _initRootScreenProjSize * _minProjSizeModifier;
            }
        }

        private SceneNode _rootNode;

        public float3 InitCamPos;

        private Dictionary<Guid, SceneNode> _nodesToRender;                                                                // Visible AND loaded nodes - updated per traversal. Created from _determinedAsVisible.Except(_determinedAsVisibleAndUnloaded);

        private ConcurrentDictionary<Guid, IEnumerable<Mesh>> _loadedMeshs;                                                         // Visible AND loaded meshes.

        private SortedDictionary<double, SceneNode> _nodesOrderedByProjectionSize;                                         // For traversal purposes only.
        private Dictionary<Guid, SceneNode> _determinedAsVisible = new Dictionary<Guid, SceneNode>();             // All visible nodes in screen projected size order - cleared in every traversal.
        private readonly Dictionary<Guid, SceneNode> _determinedAsVisibleAndUnloaded = new Dictionary<Guid, SceneNode>(); // Visible but unloaded nodes - cleared in every traversal.
        private readonly ConcurrentDictionary<Guid, SceneNode> _globalLoadingCache = new ConcurrentDictionary<Guid, SceneNode>(); //nodes that shall be loaded eventually. Loaded nodes are removed from cache and their PtOCtantComp.WasLoaded bool is set to true.

        private readonly WireframeCube wfc = new WireframeCube();
        private readonly ShaderEffect _wfcEffect = ShaderCodeBuilder.MakeShaderEffect(new float4(0, 0, 0, 1), new float4(1, 1, 1, 1), 10);

        /// <summary>
        /// The path to the folder that holds the file.
        /// </summary>
        public string FileFolderPath;

        /// <summary>
        /// Maximal number of points that are visible in one frame - tradeoff between performance and quality.
        /// </summary>
        public int PointThreshold = 1000000;

        private float _initRootScreenProjSize;

        // Minimal screen projected size of a node. Depends on spacing of the octree.
        private float _minScreenProjectedSize;

        private readonly int SceneUpdateTime = 300; // in ms
        private float _deltaTimeSinceLastUpdate;

        //Number of nodes that will be loaded, starting with the one with the biggest screen projected size to ensure no octant is loaded that will be invisible in a few frames.
        //Load the five biggest nodes (screen projected size) as proposed in Schütz' thesis.
        private readonly int _noOfLoadedNodes = 5;

        /// <summary>
        /// Creates a new instance of type <see cref="PtOctantLoader{TPoint}"/>.
        /// </summary>
        /// <param name="fileFolderPath">Path to the folder that holds the file.</param>
        /// <param name="rc">The <see cref="RenderContext"/> that is used.</param>
        public PtOctantLoader(string fileFolderPath, RenderContext rc)
        {
            _nodesToRender = new Dictionary<Guid, SceneNode>();
            _loadedMeshs = new ConcurrentDictionary<Guid, IEnumerable<Mesh>>();            
            _nodesOrderedByProjectionSize = new SortedDictionary<double, SceneNode>(); // visible nodes ordered by screen-projected-size;
            RC = rc;

            FileFolderPath = fileFolderPath;

            var loadingThread = new Thread(() =>
            {
                while (true)
                {
                    if (_globalLoadingCache.Count == 0)
                        continue;

                    var loopLength = _globalLoadingCache.Count;
                    var orderdToLoad = _globalLoadingCache.OrderByDescending(kvp => kvp.Value.GetComponent<PtOctant>().ProjectedScreenSize).ToList();
                    Parallel.For(0, loopLength,
                    index =>
                    {
                        LoadNode(GetMeshsForNode, PtAcc, ref orderdToLoad);
                    });
                   
                    //while (orderdToLoad.Count > 0)
                    //{
                    //    LoadNode(GetMeshsForNode, PtAcc, ref orderdToLoad);
                    //}

                    Diagnostics.Debug("Finished loading.");
                }
            });
            loadingThread.Start();
        }

        /// <summary>
        /// Updates the visible octree hierarchy in the scene and updates the VisibleOctreeHierarchyTex in the shaders.
        /// </summary>
        /// <param name="ptSizeMode">The <see cref="PointSizeMode"./></param>
        /// <param name="depthPassEf">Shader effect used in the depth pass in eye dome lighting.</param>
        /// <param name="colorPassEf">Shader effect that is accountable for rendering the color pass.</param>       
        public void UpdateScene(PointSizeMode ptSizeMode, ShaderEffect depthPassEf, ShaderEffect colorPassEf)
        {
            WasSceneUpdated = false;

            if (_deltaTimeSinceLastUpdate < SceneUpdateTime)
                _deltaTimeSinceLastUpdate += Time.RealDeltaTimeMs * 1000;

            else
            {
                _deltaTimeSinceLastUpdate = 0;
                if (IsUserMoving) return;

                TraverseByProjectedSizeOrder(); //determine visible nodes for this traversal step.
                UnloadedNodesToLoadingCache(); //shove nodes, that shall be loaded eventually, into the global "to load" cache.

                _nodesToRender = _determinedAsVisible.Except(_determinedAsVisibleAndUnloaded).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);               

                if (ptSizeMode == PointSizeMode.ADAPTIVE_SIZE)
                {
                    TraverseBreadthFirstToCreate1DTex(_rootNode, VisibleOctreeHierarchyTex);
                    depthPassEf.SetEffectParam("OctreeTex", VisibleOctreeHierarchyTex);
                    colorPassEf.SetEffectParam("OctreeTex", VisibleOctreeHierarchyTex);
                }

                TraverseToUpdateScene(_rootNode);
            }

            WasSceneUpdated = true;
        }

        /// <summary>
        /// Iterates the VisibleNodes list and sets the octant mesh for visible nodes.
        /// </summary>
        /// <param name="scene">The scene that contains the point cloud and the wireframe cubes. Only needed to visualize the octants.</param>       
        public void ShowOctants(SceneContainer scene)
        {
            WasSceneUpdated = false;
            DeleteOctants(scene);
            foreach (var node in _nodesToRender.Values)
            {
                var ptOctantComp = node.GetComponent<PtOctant>();

                if (_loadedMeshs.ContainsKey(ptOctantComp.Guid))
                {
                    scene.Children.Add(new SceneNode()
                    {
                        Name = "WireframeCube",
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Translation = (float3)ptOctantComp.Center,
                                Scale = float3.One * (float)ptOctantComp.Size
                            },
                            new ShaderEffect()
                            {
                                Effect = _wfcEffect
                            },
                            wfc
                        }
                    });
                }
            }

            WasSceneUpdated = true;
        }

        /// <summary>
        /// Deletes all wireframe cubes from the scene.
        /// </summary>
        /// <param name="scene"></param>
        public void DeleteOctants(SceneContainer scene)
        {
            scene.Children.RemoveAll(node => node.Name == "WireframeCube");
        }

        /// <summary>
        /// Only add the first n elements to the loading cache. Those will be loaded in one loop iteration int the loading thread.
        /// </summary>
        private void UnloadedNodesToLoadingCache()
        {
            var loopLength = _determinedAsVisibleAndUnloaded.Count < _noOfLoadedNodes ? _determinedAsVisibleAndUnloaded.Count : _noOfLoadedNodes;
            var toCache = _determinedAsVisibleAndUnloaded.Take(loopLength).ToList();
            
            for (int i = 0; i < loopLength; i++)
            {
                var item = toCache[i];
                _globalLoadingCache.AddOrUpdate(item.Key, item.Value, (key, val) => val);
            }
        }

        /// <summary>
        /// Traverses the scene nodes the point cloud is stored in and searches for visible nodes in screen-projected-size order.
        /// </summary>        
        private void TraverseByProjectedSizeOrder()
        {
            NumberOfVisiblePoints = 0;

            if (RC.Projection == float4x4.Identity || RC.View == float4x4.Identity) return;

            _determinedAsVisible.Clear();
            _nodesOrderedByProjectionSize.Clear();
            _determinedAsVisibleAndUnloaded.Clear();

            var fov = (float)RC.ViewportWidth / RC.ViewportHeight;

            ProcessNode(_rootNode, fov);

            while (_nodesOrderedByProjectionSize.Count > 0 && NumberOfVisiblePoints <= PointThreshold)
            {
                // choose the nodes with the biggest screen size overall to process next
                var kvp = _nodesOrderedByProjectionSize.Last();
                var biggestNode = kvp.Value;

                var ptOctantComp = kvp.Value.GetComponent<PtOctant>();
                _determinedAsVisible.Add(kvp.Value.GetComponent<PtOctant>().Guid, kvp.Value);

                if (!ptOctantComp.WasLoaded)
                {
                    if (ptOctantComp.NumberOfPointsInNode == 0)
                        NumberOfVisiblePoints += GetPtCountFromFile(ptOctantComp);
                    else
                        NumberOfVisiblePoints += ptOctantComp.NumberOfPointsInNode;

                    _determinedAsVisibleAndUnloaded.Add(ptOctantComp.Guid, kvp.Value);
                }
                else
                    NumberOfVisiblePoints += ptOctantComp.NumberOfPointsInNode;

                _nodesOrderedByProjectionSize.Remove(kvp.Key);
                ProcessChildren(biggestNode, fov);
            }
        }

        private void ProcessChildren(SceneNode node, float fov)
        {
            var ptOctantComp = node.GetComponent<PtOctant>();

            if (ptOctantComp.IsLeaf) return;

            // add child nodes to the heap of ordered nodes
            foreach (var child in node.Children)
            {
                if (child == null)
                    continue;

                ProcessNode(child, fov);
            }
        }

        private void ProcessNode(SceneNode node, float fov)
        {
            var ptOctantChildComp = node.GetComponent<PtOctant>();

            //If node does not intersect the viewing frustum, remove it from loaded meshes and return.
            if (!ptOctantChildComp.Intersects(RC.Projection * RC.View))
            {
                _globalLoadingCache.TryRemove(ptOctantChildComp.Guid, out var val); //node that is in loading cache isn't visible anymore                
                return;
            }

            var camPos = RC.View.Invert().Column3;
            var camPosD = new double3(camPos.x, camPos.y, camPos.z);

            // gets pixel radius of the node
            ptOctantChildComp.ComputeScreenProjectedSize(camPosD, RC.ViewportHeight, fov);

            //If the nodes screen projected size is too small, remove it from loaded meshes and return.
            if (ptOctantChildComp.ProjectedScreenSize < _minScreenProjectedSize)
            {
                _globalLoadingCache.TryRemove(ptOctantChildComp.Guid, out var val); //node that is in loading cache isn't visible anymore                
                return;
            }
            //Else if the node is visible and big enough, load if necessary and add to visible nodes.
            // If by chance two same nodes have the same screen-projected-size can't add it to the dictionary....
            if (!_nodesOrderedByProjectionSize.ContainsKey(ptOctantChildComp.ProjectedScreenSize))
                _nodesOrderedByProjectionSize.Add(ptOctantChildComp.ProjectedScreenSize, node);

        }

        private void LoadNode(Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> GetMeshsForNode, PointAccessor<TPoint> ptAccessor, ref List<KeyValuePair<Guid, SceneNode>> orderdToLoad)
        {
            var kvp = orderdToLoad.First();
            var node = kvp.Value;

            var ptOctantComp = node.GetComponent<PtOctant>();
            if (!ptOctantComp.WasLoaded)
            {
                var pts = LoadPointsForNode(ptAccessor, ptOctantComp);
                ptOctantComp.NumberOfPointsInNode = pts.Count;
                var meshes = GetMeshsForNode(ptAccessor, pts);
                _loadedMeshs.AddOrUpdate(ptOctantComp.Guid, meshes, (key, val) => val);
            }

            orderdToLoad.RemoveAt(0);
            _globalLoadingCache.TryRemove(kvp.Key, out var removedNode);

        }

        /// <summary>
        /// Traverse and updates the scene (octree) according to the _nodesToRender list.
        /// </summary>
        /// <param name="nodesToRender">Nodes that are visible AND loaded - corresponds to the ones that can be rendered.</param>
        /// <param name="node">Node that is processed in this step of the traversal.</param>        
        private void TraverseToUpdateScene(SceneNode node)
        {
            var ptOctantComp = node.GetComponent<PtOctant>();
            ptOctantComp.VisibleChildIndices = 0;

            if (!_nodesToRender.ContainsKey(ptOctantComp.Guid)) //Node isn't visible
            {
                RemoveMeshes(node, ptOctantComp);
            }
            else
            {
                if (_nodesToRender.ContainsKey(ptOctantComp.Guid)) //is visible and was loaded
                {
                    if (_loadedMeshs.TryGetValue(ptOctantComp.Guid, out var loadedMeshes))
                    {
                        node.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));
                        node.Components.AddRange(loadedMeshes);
                    }
                    else
                    {
                        throw new ArgumentException("Trying to set meshes that are not loaded yet!");
                    }
                }
            }

            foreach (var child in node.Children)
            {
                TraverseToUpdateScene(child);
            }
        }

        private void RemoveMeshes(SceneNode node, PtOctant ptOctantComponent)
        {
            node.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));

            _loadedMeshs.TryGetValue(ptOctantComponent.Guid, out var meshs);
            if (meshs != null)
            {
                //_numberOfVisiblePoints -= ptOctantComponent.NumberOfPointsInNode;
                foreach (var mesh in meshs)
                {
                    mesh.Dispose();
                }
            }
            _loadedMeshs.TryRemove(ptOctantComponent.Guid, out var loadedMesh);

            ptOctantComponent.WasLoaded = false;
            ptOctantComponent.VisibleChildIndices = 0;
        }

        private List<TPoint> LoadPointsForNode(PointAccessor<TPoint> ptAccessor, PtOctantComponent ptOctantComponent)
        {
            var pathToFile = FileFolderPath + "/Octants/" + ptOctantComponent.Guid.ToString("N") + ".node";

            if (!File.Exists(pathToFile))
                throw new ArgumentException("File: " + ptOctantComponent.Guid + ".node does not exist!");

            using (BinaryReader br = new BinaryReader(File.Open(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
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

        private int GetPtCountFromFile(PtOctant ptOctantComponent)
        {
            var pathToFile = FileFolderPath + "/Octants/" + ptOctantComponent.Guid.ToString("N") + ".node";

            if (!File.Exists(pathToFile))
                throw new ArgumentException("File: " + ptOctantComponent.Guid + ".node does not exist!");

            using (BinaryReader br = new BinaryReader(File.Open(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                // step to stream position
                //br.BaseStream.Position = node.StreamPosition;

                // read number of points
                return br.ReadInt32();
            }

        }

        /// <summary>
        /// Traverse breadth first to create 1D texture that contains the visible octree hierarchy.
        /// </summary>
        private void TraverseBreadthFirstToCreate1DTex(SceneNode node, Texture tex)
        {
            if (_nodesToRender.Count == 0) return;

            //clear texture
            tex.Blt(0, 0, new ImageData(new byte[tex.PixelData.Length], tex.Width, tex.Height, tex.PixelFormat));

            var visibleOctantsImgData = new ImageData(new byte[_nodesToRender.Count * tex.PixelFormat.BytesPerPixel], _nodesToRender.Count, 1, tex.PixelFormat);

            Queue<SceneNode> candidates = new Queue<SceneNode>();

            var rootPtOctantComp = node.GetComponent<PtOctant>();
            rootPtOctantComp.PosInHierarchyTex = 0;
            if (!_nodesToRender.ContainsKey(rootPtOctantComp.Guid))
                return;

            candidates.Enqueue(node);

            //The nodes' position in the texture
            int nodePixelPos = 0;

            while (candidates.Count > 0)
            {
                node = candidates.Dequeue();
                var ptOctantComp = node.GetComponent<PtOctant>();

                //check if octantcomp.guid is in VisibleNode
                //yes --> write to 1D tex
                if (_nodesToRender.ContainsKey(ptOctantComp.Guid))
                {
                    ptOctantComp.PosInHierarchyTex = nodePixelPos;

                    if (node.Parent != null)
                    {
                        var parentPtOctantComp = node.Parent.GetComponent<PtOctant>();

                        //If parentPtOctantComp.VisibleChildIndices == 0 this child is the first visible one.
                        if (_nodesToRender.ContainsKey(parentPtOctantComp.Guid))
                        {
                            if (parentPtOctantComp.VisibleChildIndices == 0)
                            {
                                //Get the "green byte" (+1) and calculate the offset from the parent to this node (in px)
                                var parentBytePos = (parentPtOctantComp.PosInHierarchyTex * tex.PixelFormat.BytesPerPixel) + 1;
                                visibleOctantsImgData.PixelData[parentBytePos] = (byte)(ptOctantComp.PosInHierarchyTex - parentPtOctantComp.PosInHierarchyTex);
                            }

                            //add the index of this node to VisibleChildIndices
                            byte indexNumber = (byte)System.Math.Pow(2, ptOctantComp.PosInParent);
                            parentPtOctantComp.VisibleChildIndices += indexNumber;
                            visibleOctantsImgData.PixelData[parentPtOctantComp.PosInHierarchyTex * tex.PixelFormat.BytesPerPixel] = parentPtOctantComp.VisibleChildIndices;
                        }
                    }

                    nodePixelPos++;
                }

                //enqueue all children
                foreach (var child in node.Children)
                {
                    candidates.Enqueue(child);
                }
            }

            //replace PixelData with new contents
            tex.Blt(0, 0, visibleOctantsImgData);
        }

    }
}
