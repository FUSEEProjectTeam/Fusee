using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
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
        /// The <see cref="RenderContext"/> the app uses.2
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
                _visibleLoadedNodes.Clear();
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

        ///The amount of milliseconds needed to pass before rendering next frame
        public const double UpdateRate = 1000 / 30d;
        private float _deltaTimeSinceLastUpdate;
        private float _deltaTimeSinceLastLoading;

        private float _minProjSizeModifier = 0.1f;
        private readonly PointType _ptType;

        // Minimal screen projected size of a node. Depends on spacing of the octree.
        private double _minScreenProjectedSize;

        // Visible AND loaded meshes.
        private Dictionary<Guid, IEnumerable<Mesh>> _loadedMeshs;

        // Allowes traversal in order of screen projected size.
        private readonly SortedDictionary<double, SceneNode> _visibleNodesOrderedByProjectionSize;

        //All visible and loaded nodes
        private Dictionary<Guid, SceneNode> _visibleLoadedNodes;

        // Visible but unloaded nodes.
        private Dictionary<Guid, SceneNode> _visibleUnloadedNodes;

        // Nodes that shall be loaded eventually. Loaded nodes are removed from cache and their PtOCtantComp.WasLoaded bool is set to true.
        // size is limited by _maxNumberOfNodesToLoad
        private ConcurrentDictionary<Guid, OctantD> _loadingQueue;

        private readonly WireframeCube wfc = new();
        private SurfaceEffect _wfcEffect;

        //Number of nodes that will be loaded, starting with the one with the biggest screen projected size to ensure no octant is loaded that will be invisible in a few frames.
        //Load the five biggest nodes (screen projected size) as proposed in Schütz' thesis.
        private readonly int _maxNumberOfNodesToLoad = 5;

        private const string _octreeTexName = "OctreeTex";

        private double3 _camPosD;
        private float _fov;

        /// <summary>
        /// Creates a new instance of type <see cref="PtOctantLoader{TPoint}"/>.
        /// </summary>
        /// <param name="fileFolderPath">Path to the folder that holds the file.</param>
        /// <param name="ptType">The <see cref="PointType"/> of the point cloud that is loaded.</param>
        public PtOctantLoader(string fileFolderPath, PointType ptType)
        {
            _ptType = ptType;
            _visibleNodesOrderedByProjectionSize = new SortedDictionary<double, SceneNode>(); // visible nodes ordered by screen-projected-size;            
            FileFolderPath = fileFolderPath;
            InitCollections(Directory.GetFiles($"{FileFolderPath}\\Octants").Length);
        }

        /// <summary>
        /// Initializes the <see cref="RC"/> dependent properties and starts the loading task.
        /// </summary>
        /// <param name="rc">The RenderContext for this loader.</param>
        public void Init(RenderContext rc)
        {
            RC = rc;
            _wfcEffect = (SurfaceEffect)RC.DefaultEffect;
            Task loadingTask = new(() =>
            {
                Thread.CurrentThread.Name = "OocLoader";
                while (!IsShuttingDown)
                {
                    if (_deltaTimeSinceLastLoading < UpdateRate)
                        _deltaTimeSinceLastLoading += Time.RealDeltaTime * 1000;
                    else
                    {
                        _deltaTimeSinceLastLoading = 0;
                        if (!_loadingQueue.IsEmpty)
                        {
                            LoadNode(_loadingQueue.OrderByDescending(kvp => kvp.Value.ProjectedScreenSize).First());
                        }
                    }
                }
            });
            loadingTask.Start();
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

            var camPos = RC.InvView.Column4;
            _camPosD = new double3(camPos.x, camPos.y, camPos.z);
            _fov = (float)RC.ViewportWidth / RC.ViewportHeight;
            SetMinScreenProjectedSize(_camPosD, _fov);

            if (_deltaTimeSinceLastUpdate < UpdateRate)
                _deltaTimeSinceLastUpdate += Time.RealDeltaTime * 1000;
            else
            {
                _deltaTimeSinceLastUpdate = 0;

                TraverseByProjectedSizeOrder(); //determine visible nodes for this traversal step.
                UnloadedNodesToLoadingQueue(); //shove nodes, that shall be loaded, into the global "to load" queue.

                if (_visibleLoadedNodes.Count == 0)
                    return;

                if (ptSizeMode == PointSizeMode.AdaptiveSize)
                {
                    TraverseBreadthFirstToCreate1DTex(_rootNode, VisibleOctreeHierarchyTex);
                    depthPassEf.SetFxParam(_octreeTexName, VisibleOctreeHierarchyTex);
                    colorPassEf.SetFxParam(_octreeTexName, VisibleOctreeHierarchyTex);
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
            DeleteWireframeOctants(scene);
            foreach (var node in _visibleLoadedNodes.Values)
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
        /// Octants can be visualized as wireframe cubes. This method deletes all wireframe cubes from the scene.
        /// </summary>
        /// <param name="scene">The <see cref="SceneContainer"/> the wireframe cubes will be deleted from.</param>
        public void DeleteWireframeOctants(SceneContainer scene)
        {
            _ = scene.Children.RemoveAll(node => node.Name == "WireframeCube");
        }

        private void InitCollections(int octantCnt)
        {
            _loadingQueue = new(16, _maxNumberOfNodesToLoad);
            _visibleUnloadedNodes = new(octantCnt);
            _visibleLoadedNodes = new(octantCnt);
            _loadedMeshs = new(octantCnt);
        }

        /// <summary>
        /// Returns meshes for point clouds that only have position information in double precision.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="ptType">The <see cref="PointType"/> for the cloud that is to be loaded.</param>
        /// <param name="pointsInNode">The lists of "raw" points.</param>
        private static List<Mesh> GetMeshsForOctant(PointAccessor<TPoint> ptAccessor, PointType ptType, TPoint[] pointsInNode)
        {
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)pointsInNode.Length / maxVertCount);
            List<Mesh> meshes = new(noOfMeshes);
            var ptCnt = pointsInNode.Length;

            int meshCnt = 0;

            for (int i = 0; i < ptCnt; i += maxVertCount)
            {
                int numberOfPointsInMesh;
                if (noOfMeshes == 1)
                    numberOfPointsInMesh = ptCnt;
                else if (noOfMeshes == meshCnt + 1)
                    numberOfPointsInMesh = (int)(ptCnt - maxVertCount * meshCnt);
                else
                    numberOfPointsInMesh = maxVertCount;

                TPoint[] points;
                if (ptCnt > maxVertCount)
                {
                    points = new TPoint[numberOfPointsInMesh];
                    Array.Copy(pointsInNode, i, points, 0, numberOfPointsInMesh);
                }
                else
                    points = pointsInNode;
                Mesh mesh = ptType switch
                {
                    PointType.Pos64 => MeshFromPoints.GetMeshPos64(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Col32IShort => MeshFromPoints.GetMeshPos64Col32IShort(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64IShort => MeshFromPoints.GetMeshPos64IShort(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Col32 => MeshFromPoints.GetMeshPos64Col32(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Label8 => MeshFromPoints.GetMeshPos64Label8(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Nor32Col32IShort => MeshFromPoints.GetMeshPos64Nor32Col32IShort(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Nor32IShort => MeshFromPoints.GetMeshPos64Nor32IShort(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Nor32Col32 => MeshFromPoints.GetMeshPos64Nor32Col32(ptAccessor, points, false, float3.Zero),
                    _ => throw new ArgumentOutOfRangeException($"Invalid PointType {ptType}"),
                };
                meshes.Add(mesh);
                meshCnt++;
            }

            return meshes;
        }

        /// <summary>
        /// Only add the first n elements to the loading cache. Those will be loaded in one loop iteration int the loading thread.
        /// </summary>
        private void UnloadedNodesToLoadingQueue()
        {
            int cnt = 0;

            foreach (var item in _visibleUnloadedNodes)
            {
                if (cnt == _maxNumberOfNodesToLoad)
                    break;

                var octantComp = item.Value.GetComponent<OctantD>();
                var added = _loadingQueue.TryAdd(item.Key, octantComp);
                if (!added)
                {
                    _loadingQueue.TryUpdate(item.Key, octantComp, _loadingQueue[item.Key]);
                }

                cnt++;
            }

            foreach (var item in _loadingQueue)
            {
                _visibleUnloadedNodes.Remove(item.Key);
            }
        }

        /// <summary>
        /// Traverses the scene nodes the point cloud is stored in and searches for visible nodes in screen-projected-size order.
        /// Recursive traversal stopps if: the screen-projected size is too small, a certain "global" point threshold is reached.
        /// </summary>
        private void TraverseByProjectedSizeOrder()
        {
            NumberOfVisiblePoints = 0;

            if (RC.Projection == float4x4.Identity || RC.View == float4x4.Identity) return;

            //_visibleLoadedNodes.Clear();
            _visibleNodesOrderedByProjectionSize.Clear();
            //_visibleUnloadedNodes.Clear();

            ProcessNode(_rootNode, _rootNode.GetComponent<OctantD>());

            while (_visibleNodesOrderedByProjectionSize.Count > 0 && NumberOfVisiblePoints <= PointThreshold)
            {
                // choose the nodes with the biggest screen size overall to process next
                var kvp = _visibleNodesOrderedByProjectionSize.Last();

                var ptOctantComp = kvp.Value.GetComponent<OctantD>();

                if (!ptOctantComp.WasLoaded)
                {
                    if (ptOctantComp.NumberOfPointsInNode == 0)
                        NumberOfVisiblePoints += PtOctreeFileReader<TPoint>.GetPtCountFromFile(FileFolderPath, ptOctantComp);
                    else
                        NumberOfVisiblePoints += ptOctantComp.NumberOfPointsInNode;

                    _visibleUnloadedNodes.TryAdd(ptOctantComp.Guid, kvp.Value);
                }
                else
                {
                    _visibleLoadedNodes.TryAdd(ptOctantComp.Guid, kvp.Value);
                    NumberOfVisiblePoints += ptOctantComp.NumberOfPointsInNode;
                }

                _visibleNodesOrderedByProjectionSize.Remove(kvp.Key);
                ProcessChildren(kvp.Value);
            }
        }

        private void ProcessChildren(SceneNode node)
        {
            // add child nodes to the heap of ordered nodes
            foreach (var child in node.Children)
            {
                if (child == null)
                    continue;

                ProcessNode(child, child.GetComponent<OctantD>());
            }
        }

        private void UpdateLists(OctantD ptOctantChildComp)
        {
            if (_visibleLoadedNodes.ContainsKey(ptOctantChildComp.Guid))
                _visibleLoadedNodes.Remove(ptOctantChildComp.Guid);
            else if (_visibleUnloadedNodes.ContainsKey(ptOctantChildComp.Guid))
                _visibleUnloadedNodes.Remove(ptOctantChildComp.Guid);

            if (_loadingQueue.ContainsKey(ptOctantChildComp.Guid))
                _loadingQueue.TryRemove(ptOctantChildComp.Guid, out _);
        }

        private void ProcessNode(SceneNode node, OctantD ptOctantChildComp)
        {
            //If node does not intersect the viewing frustum, remove it from loaded meshes and return.
            if (!ptOctantChildComp.InsideOrIntersectingFrustum(RC.RenderFrustum))
            {
                UpdateLists(ptOctantChildComp);
                return;
            }

            // gets pixel radius of the node
            ptOctantChildComp.ComputeScreenProjectedSize(_camPosD, RC.ViewportHeight, _fov);

            //If the nodes screen projected size is too small, remove it from loaded meshes and return.
            if (ptOctantChildComp.ProjectedScreenSize < _minScreenProjectedSize)
            {
                UpdateLists(ptOctantChildComp);
                return;
            }

            // Else if the node is visible and big enough, load if necessary and add to visible nodes.
            // If by chance two same nodes have the same screen-projected-size can't add it to the dictionary....            
            _visibleNodesOrderedByProjectionSize.TryAdd(ptOctantChildComp.ProjectedScreenSize, node);
        }

        //Loading 
        private void LoadNode(KeyValuePair<Guid, OctantD> kvp)
        {
            var ptOctantComp = kvp.Value;

            if (!ptOctantComp.WasLoaded)
            {
                var pts = PtOctreeFileReader<TPoint>.LoadPointsForNode(FileFolderPath, PtAcc, ptOctantComp);
                ptOctantComp.NumberOfPointsInNode = pts.Length;
                var meshes = GetMeshsForOctant(PtAcc, _ptType, pts);

                var added = _loadedMeshs.TryAdd(ptOctantComp.Guid, meshes);
                if (!added)
                {
                    _loadedMeshs[ptOctantComp.Guid] = meshes;
                }
            }
            _ = _loadingQueue.TryRemove(kvp.Key, out _);
        }

        /// <summary>
        /// Traverse and updates the scene (octree) according to the _nodesToRender list.
        /// </summary>
        /// <param name="node">Node that is processed in this step of the traversal.</param>
        private void TraverseToUpdateScene(SceneNode node)
        {
            var ptOctantComp = node.GetComponent<OctantD>();
            ptOctantComp.VisibleChildIndices = 0;

            if (!_visibleLoadedNodes.ContainsKey(ptOctantComp.Guid)) //Node isn't visible
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

        /// <summary>
        /// Traverse breadth first to create 1D texture that contains the visible octree hierarchy.
        /// </summary>
        private void TraverseBreadthFirstToCreate1DTex(SceneNode node, Texture tex)
        {
            if (_visibleLoadedNodes.Count == 0) return;

            //clear texture
            tex.Blt(0, 0, new ImageData(new byte[tex.PixelData.Length], tex.Width, tex.Height, tex.PixelFormat));

            var visibleOctantsImgData = new ImageData(new byte[_visibleLoadedNodes.Count * tex.PixelFormat.BytesPerPixel], _visibleLoadedNodes.Count, 1, tex.PixelFormat);
            var candidates = new Queue<SceneNode>();

            var rootPtOctantComp = node.GetComponent<OctantD>();
            rootPtOctantComp.PosInHierarchyTex = 0;
            if (!_visibleLoadedNodes.ContainsKey(rootPtOctantComp.Guid))
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
                if (_visibleLoadedNodes.ContainsKey(ptOctantComp.Guid))
                {
                    ptOctantComp.PosInHierarchyTex = nodePixelPos;

                    if (node.Parent != null)
                    {
                        var parentPtOctantComp = node.Parent.GetComponent<OctantD>();

                        //If parentPtOctantComp.VisibleChildIndices == 0 this child is the first visible one.
                        if (_visibleLoadedNodes.ContainsKey(parentPtOctantComp.Guid))
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