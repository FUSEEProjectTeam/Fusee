using Fusee.Base.Core;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.PotreeReader.V1;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Will render a Potree 1.0 Point Cloud if visited by the SceneRenderer.
    /// </summary>
    /// <typeparam name="TPoint">The type of the point cloud points.</typeparam>
    public class PointCloud<TPoint> : SceneComponent, IDisposable where TPoint : new()
    {
        /// <summary>
        /// Caches already created meshes.
        /// </summary>
        public MemoryCache<IEnumerable<GpuMesh>> MeshCache { get; private set; }

        /// <summary>
        /// The <see cref="PointType"/> of this PointCloud.
        /// </summary>
        public PointType Type { get; private set; }

        /// <summary>
        /// The <see cref="PointAccessor"/> used to get the point data for this PointCloud.
        /// </summary>
        public PointAccessor<TPoint> PointAccessor;

        private readonly List<IEnumerable<GpuMesh>> _disposeQueue;
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloud"/>
        /// </summary>
        /// <param name="pointAccessor"></param>
        /// <param name="fileFolderPath"></param>
        /// <param name="pointType"></param>
        public PointCloud(PointAccessor<TPoint> pointAccessor, string fileFolderPath, PointType pointType)
        {
            var noOfOctants = Directory.GetFiles($"{fileFolderPath}\\Octants").Length;
            PointCloudLoader = new PointCloudLoader<TPoint>(fileFolderPath, noOfOctants)
            {
                Octree = ReadPotreeData<TPoint>.GetOctree(pointAccessor, fileFolderPath),
                FileFolderPath = fileFolderPath,
                PtAccessor = pointAccessor,

            };
            Center = new float3(PointCloudLoader.Octree.Root.Center);
            Size = (float)PointCloudLoader.Octree.Root.Size;

            MeshCache = new();
            MeshCache.AddItem += OnCreateMesh;
            MeshCache.HandleEvictedItem = OnItemEvictedFromCache;
            Type = pointType;
            PointAccessor = pointAccessor;
            _disposeQueue = new List<IEnumerable<GpuMesh>>(noOfOctants);
        }

        /// <summary>
        /// Center of the point cloud.
        /// </summary>
        public float3 Center { get; private set; }

        /// <summary>
        /// Size of the point clouds (quadratic) bounding box.
        /// </summary>
        public float Size { get; private set; }

        /// <summary>
        /// The number of points that are currently visible.
        /// </summary>
        public int NumberOfVisiblePoints
        {
            get => PointCloudLoader.NumberOfVisiblePoints;
        }

        /// <summary>
        /// Changes the minimum size of octants. If an octant is smaller it won't be rendered.
        /// </summary>
        public float MinProjSizeModifier
        {
            get => PointCloudLoader.MinProjSizeModifier;
            set
            {
                PointCloudLoader.MinProjSizeModifier = value;
            }
        }

        /// <summary>
        /// Maximal number of points that are visible in one frame - tradeoff between performance and quality.
        /// </summary>
        public int PointThreshold
        {
            get => PointCloudLoader.PointThreshold;
            set
            {
                PointCloudLoader.PointThreshold = value;
            }
        }

        /// <summary>
        /// The amount of milliseconds needed to pass before rendering next frame
        /// </summary>
        public double UpdateRate
        {
            get => PointCloudLoader.UpdateRate;
            set
            {
                PointCloudLoader.UpdateRate = value;
            }
        }

        internal float Fov;
        internal float3 CamPos;
        internal FrustumF RenderFrustum;
        internal int ViewportHeight;
        internal PointCloudLoader<TPoint> PointCloudLoader;

        private int _maxNumberOfDisposals = 3;

        private static object _lockDisposeQueue = new object();

        internal void Update()
        {
            lock (_lockDisposeQueue)
            {
                if (_disposeQueue.Count > 0)
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

            PointCloudLoader.RenderFrustum = RenderFrustum;
            PointCloudLoader.ViewportHeight = ViewportHeight;
            PointCloudLoader.Update(Fov, CamPos);
        }

        private void OnItemEvictedFromCache(object guid, object meshes, EvictionReason reason, object state)
        {
            lock (_lockDisposeQueue)
            {
                _disposeQueue.Add((IEnumerable<GpuMesh>)meshes);
            }
        }

        private async Task<IEnumerable<GpuMesh>> OnCreateMeshAsync(object sender, EventArgs e)
        {
            var meshArgs = (GpuMeshFromPointsEventArgs<TPoint>)e;
            return await Task.Run(() => { return GetMeshsForOctant(PointAccessor, Type, meshArgs.Points, meshArgs.RenderContext); });
        }

        //TODO: Temporary "hack" because multi-threaded gpu buffer creation is not supported right now
        private async Task<IEnumerable<GpuMesh>> OnCreateMesh(object sender, EventArgs e)
        {
            var meshArgs = (GpuMeshFromPointsEventArgs<TPoint>)e;
            return GetMeshsForOctant(PointAccessor, Type, meshArgs.Points, meshArgs.RenderContext);
        }

        /// <summary>
        /// Returns meshes for point clouds that only have position information in double precision.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="ptType">The <see cref="PointType"/> for the cloud that is to be loaded.</param>
        /// <param name="pointsInNode">The lists of "raw" points.</param>
        /// <param name="rc">The <see cref="RenderContext"/>, used to create the mesh on the GPU.</param>
        private List<GpuMesh> GetMeshsForOctant(PointAccessor<TPoint> ptAccessor, PointType ptType, TPoint[] pointsInNode, RenderContext rc)
        {
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)pointsInNode.Length / maxVertCount);
            List<GpuMesh> meshes = new(noOfMeshes);
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
                GpuMesh mesh = ptType switch
                {
                    PointType.Pos64 => MeshFromPointCloudPoints.GetMeshPos64(ptAccessor, points, false, float3.Zero, rc.CreateGpuMesh),
                    PointType.Pos64Col32IShort => MeshFromPointCloudPoints.GetMeshPos64Col32IShort(ptAccessor, points, false, float3.Zero, rc.CreateGpuMesh),
                    PointType.Pos64IShort => MeshFromPointCloudPoints.GetMeshPos64IShort(ptAccessor, points, false, float3.Zero, rc.CreateGpuMesh),
                    PointType.Pos64Col32 => MeshFromPointCloudPoints.GetMeshPos64Col32(ptAccessor, points, false, float3.Zero, rc.CreateGpuMesh),
                    PointType.Pos64Label8 => MeshFromPointCloudPoints.GetMeshPos64Label8(ptAccessor, points, false, float3.Zero, rc.CreateGpuMesh),
                    PointType.Pos64Nor32Col32IShort => MeshFromPointCloudPoints.GetMeshPos64Nor32Col32IShort(ptAccessor, points, false, float3.Zero, rc.CreateGpuMesh),
                    PointType.Pos64Nor32IShort => MeshFromPointCloudPoints.GetMeshPos64Nor32IShort(ptAccessor, points, false, float3.Zero, rc.CreateGpuMesh),
                    PointType.Pos64Nor32Col32 => MeshFromPointCloudPoints.GetMeshPos64Nor32Col32(ptAccessor, points, false, float3.Zero, rc.CreateGpuMesh),
                    _ => throw new ArgumentOutOfRangeException($"Invalid PointType {ptType}"),
                };
                meshes.Add(mesh);
                meshCnt++;
            }

            return meshes;
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
                    _lockDisposeQueue = null;
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
        ~PointCloud()
        {
            Dispose(disposing: false);
        }
    }
}
