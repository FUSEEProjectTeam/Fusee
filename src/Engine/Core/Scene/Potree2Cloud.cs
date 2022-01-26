using Fusee.Base.Core;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.PotreeReader.V2;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Point type specific implementation of Potree2 clouds.
    /// </summary>
    public class Potree2Cloud : IPointCloudImp, IDisposable
    {
        public List<GpuMesh> MeshesToRender { get; set; }

        public PointCloudFileType FileType { get; } = PointCloudFileType.Potree2;

        internal PointCloudLoader PointCloudLoader;

        /// <summary>
        /// The number of points that are currently visible.
        /// </summary>
        public int NumberOfVisiblePoints
        {
            get => PointCloudLoader.NumberOfVisiblePoints;
        }

        /// <summary>
        /// Changes the minimum size of a octant. If an octant is smaller it won't be rendered.
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
        /// Maximal number of points that are visible in one frame - trade-off between performance and quality.
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

        /// <summary>
        /// Caches already created meshes.
        /// </summary>
        private MemoryCache<IEnumerable<GpuMesh>> _meshCache { get; set; }

        public float3 Center => (float3)PointCloudLoader.Octree.Root.Center;

        public float3 Size => new((float)PointCloudLoader.Octree.Root.Size);

        private readonly List<IEnumerable<GpuMesh>> _disposeQueue;
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloud"/>
        /// </summary>
        /// <param name="fileFolderPath"></param>
        public Potree2Cloud(string fileFolderPath)
        {
            var noOfOctants = (8 ^ 8) / 4;// Directory.GetFiles($"{fileFolderPath}\\Octants").Length;

            PointCloudLoader = new PointCloudLoader(fileFolderPath, noOfOctants, new ReadPotree2Data());

            _meshCache = new();
            _meshCache.AddItem += OnCreateMesh;
            _meshCache.HandleEvictedItem = OnItemEvictedFromCache;
            _disposeQueue = new List<IEnumerable<GpuMesh>>(noOfOctants);
            MeshesToRender = new List<GpuMesh>();
        }

        private const int _maxNumberOfDisposals = 3;

        private static object _lockDisposeQueue = new();

        public void Update(CreateGpuMesh createGpuMesh, float fov, int viewportHeight, FrustumF renderFrustum, float3 camPos)
        {
            MeshesToRender.Clear();
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

            PointCloudLoader.RenderFrustum = renderFrustum;
            PointCloudLoader.ViewportHeight = viewportHeight;
            PointCloudLoader.Fov = fov;
            PointCloudLoader.CamPos = camPos;

            PointCloudLoader.Update();

            foreach (var guid in PointCloudLoader.VisibleNodes)
            {
                IEnumerable<GpuMesh> meshes;

                if (guid != null)
                {
                    switch (PointCloudLoader.PtAccessor.PointType)
                    {
                        default:
                        case PointType.Undefined:
                            throw new ArgumentException();
                        case PointType.PosD3:
                            meshes = TryGetMeshesFromCache<PosD3>(createGpuMesh, guid);
                            break;
                        case PointType.PosD3ColF3InUs:
                            meshes = TryGetMeshesFromCache<PosD3ColF3InUs>(createGpuMesh, guid);
                            break;
                        case PointType.PosD3InUs:
                            meshes = TryGetMeshesFromCache<PosD3InUs>(createGpuMesh, guid);
                            break;
                        case PointType.PosD3ColF3:
                            meshes = TryGetMeshesFromCache<PosD3ColF3>(createGpuMesh, guid);
                            break;
                        case PointType.PosD3LblB:
                            meshes = TryGetMeshesFromCache<PosD3LblB>(createGpuMesh, guid);
                            break;
                        case PointType.PosD3NorF3ColF3InUs:
                            meshes = TryGetMeshesFromCache<PosD3NorF3ColF3InUs>(createGpuMesh, guid);
                            break;
                        case PointType.PosD3NorF3InUs:
                            meshes = TryGetMeshesFromCache<PosD3NorF3InUs>(createGpuMesh, guid);
                            break;
                        case PointType.PosD3NorF3ColF3:
                            meshes = TryGetMeshesFromCache<PosD3NorF3ColF3>(createGpuMesh, guid);
                            break;
                        case PointType.PosD3ColF3LblB:
                            meshes = TryGetMeshesFromCache<PosD3ColF3LblB>(createGpuMesh, guid);
                            break;
                    }

                    if (meshes == null) continue;
                    foreach (var mesh in meshes)
                    {
                        MeshesToRender.Add(mesh);
                    }
                }
            }
        }

        private IEnumerable<GpuMesh> TryGetMeshesFromCache<TPoint>(CreateGpuMesh createGpuMesh, string guid)
        {
            if (!PointCloudLoader.PointCache.TryGetValue(guid, out var points)) return null;
            _meshCache.AddOrUpdate(guid, new GpuMeshFromPointsEventArgs(points, createGpuMesh, points.Length), out var meshes);
            return meshes;
        }

        private void OnItemEvictedFromCache(object guid, object meshes, EvictionReason reason, object state)
        {
            lock (_lockDisposeQueue)
            {
                _disposeQueue.Add((IEnumerable<GpuMesh>)meshes);
            }
        }

        private IEnumerable<GpuMesh> OnCreateMesh(object sender, EventArgs e)
        {
            var args = (GpuMeshFromPointsEventArgs)e;
            var ptCnt = args.NumberOfPoints;
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)ptCnt / maxVertCount);
            List<GpuMesh> meshes = new(noOfMeshes);

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

                IPointCloudPoint[] points;
                var typedArgs = (GpuMeshFromPointsEventArgs)e;
                if (ptCnt > maxVertCount)
                {
                    points = new IPointCloudPoint[numberOfPointsInMesh];
                    Array.Copy(typedArgs.Points, i, points, 0, numberOfPointsInMesh);
                }
                else
                {
                    points = typedArgs.Points;
                }
                var mesh = MeshFromPointCloudPoints.GetMesh(PointCloudLoader.PtAccessor, points, true, float3.Zero, typedArgs.CreateGpuMesh);

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
                    _meshCache.Dispose();

                    _lockDisposeQueue = null;
                    foreach (var meshes in _disposeQueue)
                    {
                        foreach (var mesh in meshes)
                        {
                            mesh.Dispose();
                        }
                        foreach (var mesh in MeshesToRender)
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
        ~Potree2Cloud()
        {
            Dispose(disposing: false);
        }
    }
}
