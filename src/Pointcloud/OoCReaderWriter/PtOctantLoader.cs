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

        /// <summary>
        /// Used for breaking the loading loop when the application is shutting down.
        /// </summary>
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
                _visibleNodesOrderedByProjectionSize.Clear();
                _determinedAsVisible.Clear();
                _rootNode = value;
                SetMinScreenProjectedSize(InitCamPos, (float)RC.ViewportWidth / RC.ViewportHeight);
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
                    _minScreenProjectedSize = RootNode.GetComponent<OctantD>().ProjectedScreenSize * _minProjSizeModifier;
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

        private float _minProjSizeModifier = 0.1f;

        private Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> _getMeshsForNode;

        // Minimal screen projected size of a node. Depends on spacing of the octree.
        private double _minScreenProjectedSize;

        private float _deltaTimeSinceLastUpdate;

        private Dictionary<Guid, SceneNode> _nodesToRender;                                     // Visible AND loaded nodes - updated per traversal. Created from _determinedAsVisible.Except(_determinedAsVisibleAndUnloaded);

        private Dictionary<Guid, IEnumerable<Mesh>> _loadedMeshs;                              // Visible AND loaded meshes.

        private readonly SortedDictionary<double, SceneNode> _visibleNodesOrderedByProjectionSize;      // For traversal purposes only.
        private Dictionary<Guid, SceneNode> _determinedAsVisible;                              // All visible nodes in screen projected size order - cleared in every traversal.

        private ConcurrentDictionary<Guid, SceneNode> _determinedAsVisibleAndUnloaded;         // Visible but unloaded nodes - cleared in every traversal.
        private ConcurrentDictionary<Guid, SceneNode> _globalLoadingCache;                     // nodes that shall be loaded eventually. Loaded nodes are removed from cache and their PtOCtantComp.WasLoaded bool is set to true.

        private readonly WireframeCube wfc = new();
        private DefaultSurfaceEffect _wfcEffect;
        private readonly int _sceneUpdateTime = 33; // in ms

        //Number of nodes that will be loaded, starting with the one with the biggest screen projected size to ensure no octant is loaded that will be invisible in a few frames.
        //Load the five biggest nodes (screen projected size) as proposed in Schütz' thesis.
        private readonly int _maxNumberOfNodesToLoad = 5;

        /// <summary>
        /// Initializes the <see cref="RC"/> dependent properties and starts the loading task.
        /// </summary>
        /// <param name="rc">The RenderContext for this loader.</param>
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

                    LoadNode(PtAcc);
                }
            });
            loadingTask.Start();
        }

        /// <summary>
        /// Creates a new instance of type <see cref="PtOctantLoader{TPoint}"/>.
        /// </summary>
        /// <param name="fileFolderPath">Path to the folder that holds the file.</param>
        /// <param name="getMeshsForNode">Encapsulates a method that has a <see cref="PointAccessor{TPoint}"/>, and a list of point cloud points and as parameters. Returns a collection of <see cref="Mesh"/>es for a Octant.</param>
        public PtOctantLoader(string fileFolderPath, Func<PointAccessor<TPoint>, List<TPoint>, IEnumerable<Mesh>> getMeshsForNode)
        {
            _visibleNodesOrderedByProjectionSize = new SortedDictionary<double, SceneNode>(); // visible nodes ordered by screen-projected-size;
            _getMeshsForNode = getMeshsForNode;
            FileFolderPath = fileFolderPath;
            InitCollections(Directory.GetFiles($"{FileFolderPath}\\Octants").Length);
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

                TraverseByProjectedSizeOrder(); //determine visible nodes for this traversal step.

                if (_determinedAsVisible.Count == 0)
                    return;

                UnloadedNodesToLoadingCache(); //shove nodes, that shall be loaded, into the global "to load" cache.

                _nodesToRender = _determinedAsVisible.Except(_determinedAsVisibleAndUnloaded).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                if (_nodesToRender.Count == 0)
                    return;

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

        private void InitCollections(int octantCnt)
        {
            _globalLoadingCache = new(2, _maxNumberOfNodesToLoad);
            _determinedAsVisibleAndUnloaded = new(2, octantCnt);
            _determinedAsVisible = new(octantCnt);
            _loadedMeshs = new(octantCnt);
        }

        /// <summary>
        /// Only add the first n elements to the loading cache. Those will be loaded in one loop iteration int the loading thread.
        /// </summary>
        private void UnloadedNodesToLoadingCache()
        {
            int cnt = 0;

            foreach (var item in _determinedAsVisibleAndUnloaded)
            {
                if (cnt == _maxNumberOfNodesToLoad)
                    break;

                var added = _globalLoadingCache.TryAdd(item.Key, item.Value);
                if (!added)
                {
                    _globalLoadingCache.TryUpdate(item.Key, item.Value, _globalLoadingCache[item.Key]);
                }
                cnt++;
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
            _visibleNodesOrderedByProjectionSize.Clear();
            _determinedAsVisibleAndUnloaded.Clear();

            var fov = (float)RC.ViewportWidth / RC.ViewportHeight;

            ProcessNode(_rootNode, fov);

            while (_visibleNodesOrderedByProjectionSize.Count > 0 && NumberOfVisiblePoints <= PointThreshold)
            {
                // choose the nodes with the biggest screen size overall to process next
                var kvp = _visibleNodesOrderedByProjectionSize.Last();
                var biggestNode = kvp.Value;

                var ptOctantComp = kvp.Value.GetComponent<OctantD>();
                _determinedAsVisible.Add(kvp.Value.GetComponent<OctantD>().Guid, kvp.Value);

                if (!ptOctantComp.WasLoaded)
                {
                    if (ptOctantComp.NumberOfPointsInNode == 0)
                        NumberOfVisiblePoints += GetPtCountFromFile(ptOctantComp);
                    else
                        NumberOfVisiblePoints += ptOctantComp.NumberOfPointsInNode;

                    _determinedAsVisibleAndUnloaded.TryAdd(ptOctantComp.Guid, kvp.Value);
                }
                else
                    NumberOfVisiblePoints += ptOctantComp.NumberOfPointsInNode;

                _visibleNodesOrderedByProjectionSize.Remove(kvp.Key);
                ProcessChildren(biggestNode, fov);
            }
        }

        private void ProcessChildren(SceneNode node, float fov)
        {
            if (node.GetComponent<OctantD>().IsLeaf) return;

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
                TryRemoveMeshes(node, ptOctantChildComp);
                return;
            }

            var camPos = RC.View.Invert().Column4;
            var camPosD = new double3(camPos.x, camPos.y, camPos.z);

            SetMinScreenProjectedSize(camPosD, fov);

            // gets pixel radius of the node
            ptOctantChildComp.ComputeScreenProjectedSize(camPosD, RC.ViewportHeight, fov);

            //If the nodes screen projected size is too small, remove it from loaded meshes and return.
            if (ptOctantChildComp.ProjectedScreenSize < _minScreenProjectedSize)
            {
                TryRemoveMeshes(node, ptOctantChildComp);
                return;
            }

            //Else if the node is visible and big enough, load if necessary and add to visible nodes.
            // If by chance two same nodes have the same screen-projected-size can't add it to the dictionary....
            if (!_visibleNodesOrderedByProjectionSize.ContainsKey(ptOctantChildComp.ProjectedScreenSize))
                _visibleNodesOrderedByProjectionSize.Add(ptOctantChildComp.ProjectedScreenSize, node);
        }

        private void LoadNode(PointAccessor<TPoint> ptAccessor)
        {
            var kvp = _globalLoadingCache.First();
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
                }
            }
            _ = _globalLoadingCache.TryRemove(kvp.Key, out _);
            _ = _determinedAsVisibleAndUnloaded.TryRemove(kvp.Key, out _);
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
                TryRemoveMeshes(node, ptOctantComp);
            }
            else //is visible and was loaded
            {
                node.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));
                node.Components.AddRange(_loadedMeshs[ptOctantComp.Guid]);
            }

            foreach (var child in node.Children)
            {
                TraverseToUpdateScene(child);
            }
        }

        /// <summary>
        /// Removes the meshes from the given SceneNode.
        /// </summary>
        /// <param name="node">The SceneNode.</param>
        /// <param name="ptOctantComponent">The GUID od this octant is used to check if the meshes where loaded.</param>
        /// <returns>Only returns false if the meshes where never loaded.</returns>
        private bool TryRemoveMeshes(SceneNode node, OctantD ptOctantComponent)
        {
            bool contains = _loadedMeshs.ContainsKey(ptOctantComponent.Guid);
            if (!contains)
                return false;
            else
            {
                _ = node.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));
                var meshes = _loadedMeshs[ptOctantComponent.Guid];
                _ = _loadedMeshs.Remove(ptOctantComponent.Guid);

                ptOctantComponent.WasLoaded = false;
                ptOctantComponent.VisibleChildIndices = 0;

                foreach (Mesh mesh in meshes)
                {
                    mesh.Dispose();
                }
                return true;
            }
        }

        private List<TPoint> LoadPointsForNode(PointAccessor<TPoint> ptAccessor, OctantD ptOctantComponent)
        {
            var pathToFile = $"{FileFolderPath}/Octants/{ptOctantComponent.Guid.ToString("N")}.node";

            if (!File.Exists(pathToFile))
                throw new ArgumentException($"File: { ptOctantComponent.Guid }.node does not exist!");

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
            var pathToFile = $"{FileFolderPath}/Octants/{ptOctantComponent.Guid.ToString("N")}.node";

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

        private void SetMinScreenProjectedSize(double3 camPos, float fov)
        {
            RootNode.GetComponent<OctantD>().ComputeScreenProjectedSize(camPos, RC.ViewportHeight, fov);
            _minScreenProjectedSize = RootNode.GetComponent<OctantD>().ProjectedScreenSize * _minProjSizeModifier;
        }
    }
}