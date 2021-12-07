using Fusee.Base.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.PotreeReader.V1;
using Fusee.Structures;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Class that manages the out of core (on demand) loading of point clouds.
    /// </summary>
    /// <typeparam name="TPoint">The type of the point cloud points.</typeparam>
    public class PointCloudLoader<TPoint> : IDisposable where TPoint : new()
    {
        /// <summary>
        /// Caches already created meshes.
        /// </summary>
        public MemoryCache<IEnumerable<Mesh>> MeshCache { get; private set; }

        /// <summary>
        /// Cahces loaded points.
        /// </summary>
        public MemoryCache<TPoint[]> PointCache { get; private set; }

        /// <summary>
        /// Flat list of meshes that will be rendered.
        /// </summary>
        public List<Mesh> MeshesToRender;

        /// <summary>
        /// If true, the visible octants will be rendered as WireframeCubes.
        /// </summary>
        public bool ShowOctants { get; set; }

        /// <summary>
        /// Is set to true internally when all visible nodes are loaded.
        /// </summary>
        public bool WasSceneUpdated { get; private set; } = true;

        /// <summary>
        /// Current Field of View - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public float Fov;

        /// <summary>
        /// Current camera position - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public float3 CamPos;

        /// <summary>
        /// Current height of the viewport - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public int ViewportHeight;

        /// <summary>
        /// Current camera frustum - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public FrustumF RenderFrustum;

        /// <summary>
        /// Provides access to properties of different point types.
        /// </summary>
        public PointAccessor<TPoint> PtAccessor { get; set; }

        /// <summary>
        /// The octree structure of the point cloud.
        /// </summary>
        public OctreeD<TPoint> Octree
        {
            get => _octree;
            set
            {
                _loadingQueue.Clear();
                MeshesToRender.Clear();
                _visibleNodesOrderedByProjectionSize.Clear();
                _octree = value;
            }
        }
        private OctreeD<TPoint> _octree;

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
                if (_octree.Root != null)
                    _minScreenProjectedSize = ((PtOctantRead<TPoint>)Octree.Root).ProjectedScreenSize * _minProjSizeModifier;
            }
        }

        /// <summary>
        /// The path to the folder that holds the file.
        /// </summary>
        public string FileFolderPath { get; set; }

        /// <summary>
        /// Maximal number of points that are visible in one frame - tradeoff between performance and quality.
        /// </summary>
        public int PointThreshold { get; set; } = 2000000;

        /// <summary>
        /// The amount of milliseconds needed to pass before rendering next frame
        /// </summary>
        public double UpdateRate { get; set; } = 1000 / 30d;

        private float _deltaTimeSinceLastUpdate;

        private float _minProjSizeModifier = 0.1f;
        private readonly PointType _ptType;

        // Minimal screen projected size of a node. Depends on spacing of the octree.
        private double _minScreenProjectedSize;

        // Allowes traversal in order of screen projected size.
        private readonly SortedDictionary<double, PtOctantRead<TPoint>> _visibleNodesOrderedByProjectionSize;

        //All visible nodes
        private List<Guid> _visibleNodes;

        //Nodes that are queued for loading in the background
        private List<Guid> _loadingQueue;

        private bool _disposed;

        private List<IEnumerable<Mesh>> _disposeQueue;

        //Number of nodes that will be loaded, starting with the one with the biggest screen projected size to ensure no octant is loaded that will be invisible in a few frames.
        //Load the five biggest nodes (screen projected size) as proposed in Schütz' thesis.
        private readonly int _maxNumberOfNodesToLoad = 5;

        private readonly int _maxNumberOfDisposals = 3;

        private double3 _camPosD;
        private float _fov;

        private static readonly object _lockLoadingQueue = new();
        private static readonly object _lockDisposeQueue = new();
        public static readonly object LockMeshesToRender = new();

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloudLoader{TPoint}"/>.
        /// </summary>
        /// <param name="fileFolderPath">Path to the folder that holds the file.</param>
        /// <param name="ptType">The <see cref="PointType"/> of the point cloud that is loaded.</param>
        public PointCloudLoader(string fileFolderPath, PointType ptType)
        {
            _ptType = ptType;
            _visibleNodesOrderedByProjectionSize = new SortedDictionary<double, PtOctantRead<TPoint>>(); // visible nodes ordered by screen-projected-size;            
            FileFolderPath = fileFolderPath;
            InitCollections(Directory.GetFiles($"{FileFolderPath}\\Octants").Length);
        }

        /// <summary>
        /// Updates the visible octree hierarchy in the scene and updates the VisibleOctreeHierarchyTex in the shaders.
        /// </summary>
        public void Update(float fov, float3 camPos)
        {
            _camPosD = new double3(camPos.x, camPos.y, camPos.z);
            _fov = fov;

            WasSceneUpdated = false;
            SetMinScreenProjectedSize(_camPosD, _fov);

            if (_deltaTimeSinceLastUpdate < UpdateRate)
                _deltaTimeSinceLastUpdate += Time.RealDeltaTime * 1000;
            else
            {
                _deltaTimeSinceLastUpdate = 0;
                //Traverses ordered by projected size.
                DetermineVisibility();

                if (_disposeQueue.Count > 0)
                {
                    lock (_lockDisposeQueue)
                    {
                        var nodesInQueue = _disposeQueue.Count;
                        var count = nodesInQueue < _maxNumberOfDisposals ? nodesInQueue : _maxNumberOfDisposals;
                        for (int i = 0; i < count; i++)
                        {
                            var meshes = _disposeQueue.Last();
                            _disposeQueue.RemoveAt(_disposeQueue.Count - 1);
                            foreach (var mesh in meshes)
                                mesh.Dispose();
                        }
                    }
                }

                lock (LockMeshesToRender)
                {
                    MeshesToRender.Clear();
                    foreach (var guid in _visibleNodes)
                    {
                        if (MeshCache.TryGetValue(guid, out var meshes))
                            MeshesToRender.AddRange(meshes);
                    }
                }
            }

            WasSceneUpdated = true;
        }

        private void InitCollections(int octantCnt)
        {
            _visibleNodes = new(octantCnt);
            MeshCache = new();
            MeshCache.AddItem += OnCreateMesh;
            MeshCache.HandleEvictedItem = OnItemEvictedFromCache;
            PointCache = new();
            PointCache.AddItem += OnLoadPoints;
            _loadingQueue = new(octantCnt);
            MeshesToRender = new(octantCnt);
            _disposeQueue = new List<IEnumerable<Mesh>>(octantCnt);
        }

        /// <summary>
        /// Traverses the scene nodes the point cloud is stored in and searches for visible nodes in screen-projected-size order.
        /// Recursive traversal stopps if: the screen-projected size is too small, a certain "global" point threshold is reached.
        /// </summary>
        private void DetermineVisibility()
        {
            NumberOfVisiblePoints = 0;
            _visibleNodesOrderedByProjectionSize.Clear();
            _visibleNodes.Clear();

            DetermineVisibilityForNode((PtOctantRead<TPoint>)_octree.Root);

            while (_visibleNodesOrderedByProjectionSize.Count > 0 && NumberOfVisiblePoints <= PointThreshold)
            {
                // choose the nodes with the biggest screen size overall to process next
                var kvp = _visibleNodesOrderedByProjectionSize.Last();

                var octant = kvp.Value;

                if (!_loadingQueue.Contains(octant.Guid) && _loadingQueue.Count <= _maxNumberOfNodesToLoad)
                {
                    lock (_lockLoadingQueue)
                    {
                        _loadingQueue.Add(octant.Guid);
                    }

                    Task.Run(async () =>
                    {
                        await PointCache.AddOrUpdate(octant.Guid, new LoadPointsEventArgs<TPoint>(octant, FileFolderPath, PtAccessor));
                        PointCache.TryGetValue(octant.Guid, out var points);

                        await MeshCache.AddOrUpdate(octant.Guid, new CreateMeshEventArgs<TPoint>(octant, points));

                        lock (_lockLoadingQueue)
                        {
                            _loadingQueue.Remove(octant.Guid);
                        }
                        if (octant.NumberOfPointsInNode == 0)
                            NumberOfVisiblePoints += ReadPotreeMetadata.GetPtCountFromFile(FileFolderPath, octant);
                        else
                            NumberOfVisiblePoints += octant.NumberOfPointsInNode;
                    });

                    _visibleNodes.Add(octant.Guid);
                    _visibleNodesOrderedByProjectionSize.Remove(kvp.Key);
                    DetermineVisibilityForChildren(kvp.Value);
                }
                else
                    return;
            }

        }

        private void DetermineVisibilityForChildren(PtOctantRead<TPoint> node)
        {
            // add child nodes to the heap of ordered nodes
            foreach (var child in node.Children)
            {
                if (child == null)
                    continue;

                DetermineVisibilityForNode((PtOctantRead<TPoint>)child);
            }
        }

        //Use Fusee.Struictures Octree - remove from Scene
        private void DetermineVisibilityForNode(PtOctantRead<TPoint> node)
        {
            // gets pixel radius of the node
            node.ComputeScreenProjectedSize(_camPosD, ViewportHeight, _fov);

            //If node does not intersect the viewing frustum or is smaller than the minimal projected size:
            //Return -> will not be added to _visibleNodesOrderedByProjectionSize -> traversal of this branch stops.
            if (!node.InsideOrIntersectingFrustum(RenderFrustum) || node.ProjectedScreenSize < _minScreenProjectedSize)
            {
                return;
            }

            // Else if the node is visible and big enough, load if necessary and add to visible nodes.
            // If by chance two same nodes have the same screen-projected-size can't add it to the dictionary....
            _visibleNodesOrderedByProjectionSize.TryAdd(node.ProjectedScreenSize, node);
        }

        private void OnItemEvictedFromCache(object guid, object meshes, EvictionReason reason, object state)
        {
            _disposeQueue.Add((IEnumerable<Mesh>)meshes);
        }

        private async Task<IEnumerable<Mesh>> OnCreateMesh(object sender, EventArgs e)
        {
            var meshArgs = (CreateMeshEventArgs<TPoint>)e;
            return await LoadMeshAsync(meshArgs.Octant, meshArgs.Points);
        }

        private async Task<TPoint[]> OnLoadPoints(object sender, EventArgs e)
        {
            var meshArgs = (LoadPointsEventArgs<TPoint>)e;
            return await ReadPotreeData<TPoint>.LoadPointsForNodeAsync(meshArgs.PathToFile, meshArgs.PtAccessor, meshArgs.Octant);
        }

        //Loading 
        private async Task<IEnumerable<Mesh>> LoadMeshAsync(PtOctantRead<TPoint> octant, TPoint[] points)
        {
            octant.NumberOfPointsInNode = points.Length;
            var meshes = await GetMeshsForOctantAsync(PtAccessor, _ptType, points);
            return meshes;
        }

        /// <summary>
        /// Returns meshes for point clouds that only have position information in double precision.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="ptType">The <see cref="PointType"/> for the cloud that is to be loaded.</param>
        /// <param name="pointsInNode">The lists of "raw" points.</param>
        private async Task<List<Mesh>> GetMeshsForOctantAsync(PointAccessor<TPoint> ptAccessor, PointType ptType, TPoint[] pointsInNode)
        {
            return await Task.Run(() => { return GetMeshsForOctant(ptAccessor, ptType, pointsInNode); });
        }

        /// <summary>
        /// Returns meshes for point clouds that only have position information in double precision.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="ptType">The <see cref="PointType"/> for the cloud that is to be loaded.</param>
        /// <param name="pointsInNode">The lists of "raw" points.</param>
        private List<Mesh> GetMeshsForOctant(PointAccessor<TPoint> ptAccessor, PointType ptType, TPoint[] pointsInNode)
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
                    numberOfPointsInMesh = (ptCnt - maxVertCount * meshCnt);
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
                    PointType.Pos64 => MeshFromPointCloudPoints.GetMeshPos64(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Col32IShort => MeshFromPointCloudPoints.GetMeshPos64Col32IShort(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64IShort => MeshFromPointCloudPoints.GetMeshPos64IShort(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Col32 => MeshFromPointCloudPoints.GetMeshPos64Col32(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Label8 => MeshFromPointCloudPoints.GetMeshPos64Label8(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Nor32Col32IShort => MeshFromPointCloudPoints.GetMeshPos64Nor32Col32IShort(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Nor32IShort => MeshFromPointCloudPoints.GetMeshPos64Nor32IShort(ptAccessor, points, false, float3.Zero),
                    PointType.Pos64Nor32Col32 => MeshFromPointCloudPoints.GetMeshPos64Nor32Col32(ptAccessor, points, false, float3.Zero),
                    _ => throw new ArgumentOutOfRangeException($"Invalid PointType {ptType}"),
                };
                meshes.Add(mesh);
                meshCnt++;
            }

            return meshes;
        }

        private void SetMinScreenProjectedSize(double3 camPos, float fov)
        {
            var root = (PtOctantRead<TPoint>)_octree.Root;
            root.ComputeScreenProjectedSize(camPos, ViewportHeight, fov);
            _minScreenProjectedSize = root.ProjectedScreenSize * _minProjSizeModifier;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var meshes in _disposeQueue)
                    {
                        foreach (var mesh in meshes)
                        {
                            mesh.Dispose();
                        }
                    }
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizers (historically referred to as destructors) are used to perform any necessary final clean-up when a class instance is being collected by the garbage collector.
        /// </summary>
        ~PointCloudLoader()
        {
            Dispose(disposing: false);
        }
    }
}