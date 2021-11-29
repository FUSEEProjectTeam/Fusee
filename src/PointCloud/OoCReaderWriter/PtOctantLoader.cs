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
        public bool ShowOctants { get; set; }

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

        // Nodes that shall be loaded eventually. Loaded nodes are removed from cache and their PtOCtantComp.WasLoaded bool is set to true.
        // size is limited by _maxNumberOfNodesToLoad
        private Dictionary<Guid, OctantD> _loadingQueue;

        // Allowes traversal in order of screen projected size.
        //TODO: SceneNode to Octant
        private readonly SortedDictionary<double, SceneNode> _visibleNodesOrderedByProjectionSize;

        //All visible nodes
        private List<Guid> _visibleNodes;

        //Number of nodes that will be loaded, starting with the one with the biggest screen projected size to ensure no octant is loaded that will be invisible in a few frames.
        //Load the five biggest nodes (screen projected size) as proposed in Schütz' thesis.
        private readonly int _maxNumberOfNodesToLoad = 5;

        private double3 _camPosD;
        private float _fov;

        private Mutex _mutex = new Mutex();

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
            Task loadingTask = new(() =>
            {
                Thread.CurrentThread.Name = "OocLoader";
                while (!IsShuttingDown)
                {
                    if (_deltaTimeSinceLastLoading < UpdateRate)
                        _deltaTimeSinceLastLoading += Time.RealDeltaTime * 1000;
                    else
                    {
                        _mutex.WaitOne();
                        _deltaTimeSinceLastLoading = 0;
                        if (_loadingQueue.Count != 0)
                            LoadNode(_loadingQueue.OrderByDescending(kvp => kvp.Value.ProjectedScreenSize).First());
                        
                        _mutex.ReleaseMutex();
                    }
                }
            });
            loadingTask.Start();
        }

        private void SetMinScreenProjectedSize(double3 camPos, float fov)
        {
            RootNode.GetComponent<OctantD>().ComputeScreenProjectedSize(camPos, RC.ViewportHeight, fov);
            _minScreenProjectedSize = RootNode.GetComponent<OctantD>().ProjectedScreenSize * _minProjSizeModifier;
        }

        /// <summary>
        /// Updates the visible octree hierarchy in the scene and updates the VisibleOctreeHierarchyTex in the shaders.
        /// </summary>
        public void UpdateScene()
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
                _mutex.WaitOne();

                _deltaTimeSinceLastUpdate = 0;
                //Traverses ordered by projected size.
                DetermineVisibility();
                //Complete FUSEE Scene Graph Traversal
                TraverseToUpdateScene(_rootNode);

                _mutex.ReleaseMutex();
            }

            WasSceneUpdated = true;
        }

        private void InitCollections(int octantCnt)
        {
            _loadingQueue = new(_maxNumberOfNodesToLoad);
            _visibleNodes = new(octantCnt);
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
        /// Traverses the scene nodes the point cloud is stored in and searches for visible nodes in screen-projected-size order.
        /// Recursive traversal stopps if: the screen-projected size is too small, a certain "global" point threshold is reached.
        /// </summary>
        private void DetermineVisibility()
        {
            NumberOfVisiblePoints = 0;

            if (RC.Projection == float4x4.Identity || RC.View == float4x4.Identity) return;

            _visibleNodesOrderedByProjectionSize.Clear();
            _visibleNodes.Clear();

            DetermineVisibilityForNode(_rootNode, _rootNode.GetComponent<OctantD>());

            while (_visibleNodesOrderedByProjectionSize.Count > 0 && NumberOfVisiblePoints <= PointThreshold)
            {
                // choose the nodes with the biggest screen size overall to process next
                var kvp = _visibleNodesOrderedByProjectionSize.Last();

                var ptOctantComp = kvp.Value.GetComponent<OctantD>();

                if (!_loadedMeshs.ContainsKey(ptOctantComp.Guid))
                {
                    if (ptOctantComp.NumberOfPointsInNode == 0)
                        NumberOfVisiblePoints += PtOctreeFileReader<TPoint>.GetPtCountFromFile(FileFolderPath, ptOctantComp);
                    else
                        NumberOfVisiblePoints += ptOctantComp.NumberOfPointsInNode;
                    
                    if(_loadingQueue.Count < _maxNumberOfNodesToLoad)
                        _loadingQueue.TryAdd(ptOctantComp.Guid, ptOctantComp);
                }
                else
                {
                    _visibleNodes.Add(ptOctantComp.Guid);
                    NumberOfVisiblePoints += ptOctantComp.NumberOfPointsInNode;
                }

                _visibleNodesOrderedByProjectionSize.Remove(kvp.Key);
                
                DetermineVisibilityForChildren(kvp.Value);
            }
        }

        private void DetermineVisibilityForChildren(SceneNode node)
        {
            // add child nodes to the heap of ordered nodes
            foreach (var child in node.Children)
            {
                if (child == null)
                    continue;

                DetermineVisibilityForNode(child, child.GetComponent<OctantD>());
            }
        }

        //Use Fusee.Struictures Octree - remove from Scene
        private void DetermineVisibilityForNode(SceneNode node, OctantD ptOctantChildComp)
        {
            // gets pixel radius of the node
            ptOctantChildComp.ComputeScreenProjectedSize(_camPosD, RC.ViewportHeight, _fov);

            //If node does not intersect the viewing frustum or is smaller than the minimal projected size:
            //Return -> will not be added to _visibleNodesOrderedByProjectionSize -> traversal of this branch stops.
            if (!ptOctantChildComp.InsideOrIntersectingFrustum(RC.RenderFrustum) || ptOctantChildComp.ProjectedScreenSize < _minScreenProjectedSize)
                return;

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

                ptOctantComp.WasLoaded = true;
            }
            _ = _loadingQueue.Remove(kvp.Key);
        }

        /// <summary>
        /// Traverse and updates the scene (octree) according to the _nodesToRender list.
        /// </summary>
        /// <param name="node">Node that is processed in this step of the traversal.</param>
        private void TraverseToUpdateScene(SceneNode node)
        {
            var ptOctantComp = node.GetComponent<OctantD>();

            if (!_visibleNodes.Contains(ptOctantComp.Guid)) //Node isn't visible
            {
                TryRemoveMeshes(node, ptOctantComp);
            }
            else //is visible
            {
                if (_loadedMeshs.ContainsKey(ptOctantComp.Guid))
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
            if (!ptOctantComponent.WasLoaded)
                return false;
            else
            {
                _ = node.Components.RemoveAll(cmp => cmp.GetType() == typeof(Mesh));
                var meshes = _loadedMeshs[ptOctantComponent.Guid];
                _ = _loadedMeshs.Remove(ptOctantComponent.Guid);

                ptOctantComponent.WasLoaded = false;

                foreach (Mesh mesh in meshes)
                {
                    mesh.Dispose();
                }
                return true;
            }
        }

        
    }
}