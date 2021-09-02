using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fusee.PointCloud.OoCReaderWriter
{
    /// <summary>
    /// Class that manages the out of core (on demand) loading of point clouds.
    /// </summary>
    /// <typeparam name="TPoint">The type of the point cloud points.</typeparam>
    public class PtOctantLoader<TPoint> : IPtOctantLoader where TPoint : new()
    {
        /// <summary>
        /// Provides access to properties of different point types.
        /// </summary>
        public PointAccessor<TPoint> PtAcc { get; set; }

        /// <summary>
        /// Is set to true internally when all visible nodes are loaded.
        /// </summary>
        public bool WasSceneUpdated { get; private set; } = true;

        /// <summary>
        /// Scene is only updated if the user is moving.
        /// </summary>
        public bool IsUserMoving { get; set; } = false;

        /// <summary>
        /// The <see cref="RenderContext"/> the app uses.
        /// Needed to determine the field of view and the camera position.
        /// </summary>
        public RenderContext RC { get; private set; }

        /// <summary>
        /// 1D Texture that stores info that is needed by the vertex shader when rendering with adaptive point size.
        /// </summary>
        public Texture VisibleOctreeHierarchyTex { get; set; }

        public bool IsShuttingDown
        {
            get; 
            set; 
        }

        /// <summary>
        /// The root node of the octree that is used to render the point cloud.
        /// </summary>
        public SceneNode RootNode
        {
            get => _rootNode;
            set
            {
                _loadedMeshs.Clear();
                _nodesOrderedByProjectionSize.Clear();
                _determinedAsVisible.Clear();

                var fov = (float)RC.ViewportWidth / RC.ViewportHeight;

                _rootNode = value;

                // gets pixel radius of the node
                RootNode.GetComponent<OctantD>().ComputeScreenProjectedSize(InitCamPos, RC.ViewportHeight, fov);
                _initRootScreenProjSize = (float)RootNode.GetComponent<OctantD>().ProjectedScreenSize;
                _minScreenProjectedSize = _initRootScreenProjSize * _minProjSizeModifier;
            }
        }
        private SceneNode _rootNode;

        /// <summary>
        /// The number of points that are currently visible.
        /// </summary>
        public int NumberOfVisiblePoints { get; private set; }

        /// <summary>
        /// Changes the minimum size of octants. If an octant is smaller it won't be rendered.
        /// </summary>
        public float MinProjSizeModifier
        {
            get => _minProjSizeModifier;
            set
            {
                _minProjSizeModifier = value;
                if (RootNode != null)
                    _minScreenProjectedSize = _initRootScreenProjSize * _minProjSizeModifier;
            }
        }

        /// <summary>
        /// The initial camera position.
        /// </summary>
        public double3 InitCamPos { get; set; }

        /// <summary>
        /// The path to the folder that holds the file.
        /// </summary>
        public string FileFolderPath { get; set; }

        /// <summary>
        /// Maximal number of points that are visible in one frame - tradeoff between performance and quality.
        /// </summary>
        public int PointThreshold { get; set; } = 1000000;

        private float _minProjSizeModifier = 1 / 3f;

        private Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> _getMeshsForNode;

        private float _initRootScreenProjSize;

        // Minimal screen projected size of a node. Depends on spacing of the octree.
        private float _minScreenProjectedSize;

        private float _deltaTimeSinceLastUpdate;

        private Dictionary<Guid, SceneNode> _nodesToRender;                                             // Visible AND loaded nodes - updated per traversal. Created from _determinedAsVisible.Except(_determinedAsVisibleAndUnloaded);

        private readonly Dictionary<Guid, IEnumerable<Mesh>> _loadedMeshs;                              // Visible AND loaded meshes.

        private SortedDictionary<double, SceneNode> _nodesOrderedByProjectionSize;                      // For traversal purposes only.
        private Dictionary<Guid, SceneNode> _determinedAsVisible = new();                               // All visible nodes in screen projected size order - cleared in every traversal.

        private readonly Dictionary<Guid, SceneNode> _determinedAsVisibleAndUnloaded = new();           // Visible but unloaded nodes - cleared in every traversal.
        private readonly ConcurrentDictionary<Guid, SceneNode> _globalLoadingCache = new();             // nodes that shall be loaded eventually. Loaded nodes are removed from cache and their PtOCtantComp.WasLoaded bool is set to true.

        private readonly WireframeCube wfc = new();
        private DefaultSurfaceEffect _wfcEffect;
        private readonly int _sceneUpdateTime = 300; // in ms

        //Number of nodes that will be loaded, starting with the one with the biggest screen projected size to ensure no octant is loaded that will be invisible in a few frames.
        //Load the five biggest nodes (screen projected size) as proposed in Schütz' thesis.
        private readonly int _noOfLoadedNodes = 5;

        public void Init(RenderContext rc)
        {
            RC = rc;
            _wfcEffect = (DefaultSurfaceEffect)RC.DefaultEffect;

            Task loadingTask = new(() =>
            {
                Thread.CurrentThread.Name = "OocLoader";
                while (!IsShuttingDown)
                {
                    if (_globalLoadingCache.IsEmpty)
                        continue;

                    _globalLoadingCache.OrderByDescending(kvp => kvp.Value.GetComponent<OctantD>().ProjectedScreenSize);

                    LoadNode(PtAcc, _globalLoadingCache);
                }
            });
            loadingTask.Start();
        }

        /// <summary>
        /// Creates a new instance of type <see cref="PtOctantLoader{TPoint}"/>.
        /// </summary>
        /// <param name="fileFolderPath">Path to the folder that holds the file.</param>
        /// <param name="rc">The <see cref="RenderContext"/> that is used.</param>
        /// <param name="getMeshsForNode">Encapsulates a method that has a <see cref="PointAccessor{TPoint}"/>, and a list of point cloud points and as parameters. Returns a collection of <see cref="Mesh"/>es for a Octant.</param>
        public PtOctantLoader(string fileFolderPath, Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> getMeshsForNode)
        {
            _nodesToRender = new Dictionary<Guid, SceneNode>();
            _loadedMeshs = new Dictionary<Guid, IEnumerable<Mesh>>();
            _nodesOrderedByProjectionSize = new SortedDictionary<double, SceneNode>(); // visible nodes ordered by screen-projected-size;
            _getMeshsForNode = getMeshsForNode;

            FileFolderPath = fileFolderPath;
        }

        /// <summary>
        /// Updates the visible octree hierarchy in the scene and updates the VisibleOctreeHierarchyTex in the shaders.
        /// </summary>
        /// <param name="ptSizeMode">The <see cref="PointSizeMode"/>.</param>
        /// <param name="depthPassEf">Shader effect used in the depth pass in eye dome lighting.</param>
        /// <param name="colorPassEf">Shader effect that is accountable for rendering the color pass.</param>       
        public void UpdateScene(PointSizeMode ptSizeMode, ShaderEffect depthPassEf, ShaderEffect colorPassEf)
        {
            WasSceneUpdated = false;

            if (_deltaTimeSinceLastUpdate < _sceneUpdateTime)
                _deltaTimeSinceLastUpdate += Time.RealDeltaTime * 1000;

            else
            {
                _deltaTimeSinceLastUpdate = 0;
                if (IsUserMoving) return;

                TraverseByProjectedSizeOrder(); //determine visible nodes for this traversal step.
                UnloadedNodesToLoadingCache(); //shove nodes, that shall be loaded eventually, into the global "to load" cache.

                _nodesToRender = _determinedAsVisible.Except(_determinedAsVisibleAndUnloaded).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                if (ptSizeMode == PointSizeMode.AdaptiveSize)
                {
                    TraverseBreadthFirstToCreate1DTex(_rootNode, VisibleOctreeHierarchyTex);
                    depthPassEf.SetFxParam("OctreeTex", VisibleOctreeHierarchyTex);
                    colorPassEf.SetFxParam("OctreeTex", VisibleOctreeHierarchyTex);
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
                var ptOctantComp = node.GetComponent<OctantD>();

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
                            _wfcEffect,
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
            _ = scene.Children.RemoveAll(node => node.Name == "WireframeCube");
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

                var added = _globalLoadingCache.TryAdd(item.Key, item.Value);
                if(!added)
                {
                    _globalLoadingCache.TryUpdate(item.Key, item.Value, _globalLoadingCache[item.Key]);
                }
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

                var ptOctantComp = kvp.Value.GetComponent<OctantD>();
                _determinedAsVisible.Add(kvp.Value.GetComponent<OctantD>().Guid, kvp.Value);

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
            var ptOctantComp = node.GetComponent<OctantD>();

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
            var ptOctantChildComp = node.GetComponent<OctantD>();

            //If node does not intersect the viewing frustum, remove it from loaded meshes and return.

            var frustum = new FrustumD();
            frustum.CalculateFrustumPlanes(RC.Projection * RC.View);

            if (!ptOctantChildComp.InsideOrIntersectingFrustum(frustum))
            {
                RemoveMeshes(node, ptOctantChildComp);
                return;
            }

            var camPos = RC.View.Invert().Column3;
            var camPosD = new double3(camPos.x, camPos.y, camPos.z);

            // gets pixel radius of the node
            ptOctantChildComp.ComputeScreenProjectedSize(camPosD, RC.ViewportHeight, fov);

            //If the nodes screen projected size is too small, remove it from loaded meshes and return.
            if (ptOctantChildComp.ProjectedScreenSize < _minScreenProjectedSize)
            {
                RemoveMeshes(node, ptOctantChildComp);
                return;
            }
            //Else if the node is visible and big enough, load if necessary and add to visible nodes.
            // If by chance two same nodes have the same screen-projected-size can't add it to the dictionary....
            if (!_nodesOrderedByProjectionSize.ContainsKey(ptOctantChildComp.ProjectedScreenSize))
                _nodesOrderedByProjectionSize.Add(ptOctantChildComp.ProjectedScreenSize, node);
        }

        private void LoadNode(PointAccessor<TPoint> ptAccessor, ConcurrentDictionary<Guid, SceneNode> orderdToLoad)
        {
            var kvp = orderdToLoad.First();
            var node = kvp.Value;

            var ptOctantComp = node.GetComponent<OctantD>();
            if (!ptOctantComp.WasLoaded)
            {
                var pts = LoadPointsForNode(ptAccessor, ptOctantComp);
                ptOctantComp.NumberOfPointsInNode = pts.Count;
                var meshes = _getMeshsForNode(ptAccessor, pts);

                var added = _loadedMeshs.TryAdd(ptOctantComp.Guid, meshes);
                if (!added)
                {
                    _loadedMeshs[ptOctantComp.Guid] = meshes;
                    //_loadedMeshs.TryUpdate(ptOctantComp.Guid, meshes, _loadedMeshs[ptOctantComp.Guid]);
                }
            }
            _ = _globalLoadingCache.TryRemove(kvp.Key, out _);

        }

        /// <summary>
        /// Traverse and updates the scene (octree) according to the _nodesToRender list.
        /// </summary>
        /// <param name="node">Node that is processed in this step of the traversal.</param>
        private void TraverseToUpdateScene(SceneNode node)
        {
            var ptOctantComp = node.GetComponent<OctantD>();
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

        private void RemoveMeshes(SceneNode node, OctantD ptOctantComponent)
        {
            _ = node.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));

            bool couldGetValue = _loadedMeshs.TryGetValue(ptOctantComponent.Guid, out var meshs);
            if (couldGetValue)
            {
                _ = _loadedMeshs.Remove(ptOctantComponent.Guid);

                ptOctantComponent.WasLoaded = false;
                ptOctantComponent.VisibleChildIndices = 0;

                foreach (Mesh mesh in meshs)
                {
                    mesh.Dispose();
                }
            }
        }

        private List<TPoint> LoadPointsForNode(PointAccessor<TPoint> ptAccessor, OctantD ptOctantComponent)
        {
            var pathToFile = FileFolderPath + "/Octants/" + ptOctantComponent.Guid.ToString("N") + ".node";

            if (!File.Exists(pathToFile))
                throw new ArgumentException("File: " + ptOctantComponent.Guid + ".node does not exist!");

            using BinaryReader br = new(File.Open(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read));
            // step to stream position
            //br.BaseStream.Position = node.StreamPosition;

            // read number of points
            var numberOfPoints = br.ReadInt32();
            var lengthOfPoint = br.ReadInt32();

            List<TPoint> points = new(numberOfPoints);

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

        private int GetPtCountFromFile(OctantD ptOctantComponent)
        {
            var pathToFile = FileFolderPath + "/Octants/" + ptOctantComponent.Guid.ToString("N") + ".node";

            if (!File.Exists(pathToFile))
                throw new ArgumentException("File: " + ptOctantComponent.Guid + ".node does not exist!");

            using BinaryReader br = new BinaryReader(File.Open(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read));
            // step to stream position
            //br.BaseStream.Position = node.StreamPosition;

            // read number of points
            return br.ReadInt32();

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
            var candidates = new Queue<SceneNode>();

            var rootPtOctantComp = node.GetComponent<OctantD>();
            rootPtOctantComp.PosInHierarchyTex = 0;
            if (!_nodesToRender.ContainsKey(rootPtOctantComp.Guid))
                return;

            candidates.Enqueue(node);

            //The nodes' position in the texture
            int nodePixelPos = 0;

            while (candidates.Count > 0)
            {
                node = candidates.Dequeue();
                var ptOctantComp = node.GetComponent<OctantD>();

                //check if octantcomp.guid is in VisibleNode
                //yes --> write to 1D tex
                if (_nodesToRender.ContainsKey(ptOctantComp.Guid))
                {
                    ptOctantComp.PosInHierarchyTex = nodePixelPos;

                    if (node.Parent != null)
                    {
                        var parentPtOctantComp = node.Parent.GetComponent<OctantD>();

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