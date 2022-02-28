using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Common.Accessors;
using Fusee.PointCloud.Core.Accessors;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Delegate for a method that tries to get the mesh(es) of an octant. If they are not cached yet, they should be created an added to the MeshCache.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public delegate IEnumerable<GpuMesh> GetMeshes(string guid);

    /// <summary>
    /// Called in "determine visibility for node" - if the PointCache does not contain the points load them.
    /// </summary>
    /// <param name="guid">Unique ID of an octant.</param>
    public delegate void TriggerPointLoading(string guid);

    /// <summary>
    /// Delegate that allows to inject the loading method of the PointReader - loads the points from file.
    /// </summary>
    /// <param name="guid">Unique ID of an octant.</param>
    public delegate TPoint[] LoadPointsHandler<TPoint>(string guid);

    /// <summary>
    /// Generic delegate to inject a method that nows how to actually create a GpuMesh for the given point type.
    /// The injected methods decide which point values are assigned to which mesh properties (primarily important for the various color values).
    /// </summary>
    /// <typeparam name="TPoint">Generic that describes the point type.</typeparam>
    /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/> that can be used to access the point data without casting the points.</param>
    /// <param name="points">The point cloud points as generic array.</param>
    /// <param name="createMesh">Delegate that injects a method that is able to create the <see cref="GpuMesh"/>.</param>
    /// <returns></returns>
    public delegate GpuMesh CreateMesh<TPoint>(PointAccessor<TPoint> ptAccessor, TPoint[] points, CreateGpuMesh createMesh);

    /// <summary>
    /// Manages the caching and loading of point and mesh data.
    /// </summary>
    /// <typeparam name="TPoint"></typeparam>
    public class PointCloudDataHandler<TPoint> : PointCloudDataHandlerBase where TPoint : new()
    {
        /// <summary>
        /// Caches loaded points.
        /// </summary>
        public MemoryCache<string, TPoint[]> PointCache { get; private set; }

        /// <summary>
        /// Caches loaded points.
        /// </summary>
        public MemoryCache<string, IEnumerable<GpuMesh>> MeshCache { get; private set; }

        private readonly PointAccessor<TPoint> _pointAccessor;
        private readonly CreateMesh<TPoint> _createMeshHandler;
        private readonly LoadPointsHandler<TPoint> _loadPointsHandler;
        private const int _maxNumberOfDisposals = 1;
        private float _deltaTimeSinceLastDisposal;
        private bool _disposed;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="pointAccessor">The point accessor that allows to access the point data.</param>
        /// <param name="createMeshHandler">Method that knows how to create a mesh for the explicit point type (see <see cref="MeshMaker"/>).</param>
        /// <param name="loadPointsHandler">The method that is able to load the points from the hard drive/file.</param>
        public PointCloudDataHandler(PointAccessor<TPoint> pointAccessor, CreateMesh<TPoint> createMeshHandler, LoadPointsHandler<TPoint> loadPointsHandler)
        {
            PointCache = new();
            MeshCache = new();
            MeshCache.SlidingExpiration = 30;
            MeshCache.ExpirationScanFrequency = 31;

            _createMeshHandler = createMeshHandler;
            _loadPointsHandler = loadPointsHandler;
            _pointAccessor = pointAccessor;

            LoadingQueue = new((8 ^ 8) / 8);
            DisposeQueue = new Dictionary<string, IEnumerable<GpuMesh>>((8 ^ 8) / 8);

            MeshCache.HandleEvictedItem = OnItemEvictedFromCache;
        }

        /// <summary>
        /// First looks in the mesh cache, if there are meshes return, 
        /// else look in the DisposeQueue, if there are meshes return,
        /// else look in the point cache, if there are points create a mesh and add to the MeshCache.
        /// </summary>
        /// <param name="guid">The unique id of an octant.</param>
        /// <returns></returns>
        public override IEnumerable<GpuMesh> GetMeshes(string guid)
        {
            if (MeshCache.TryGetValue(guid, out var meshes))
                return meshes;
            else if (DisposeQueue.TryGetValue(guid, out meshes))
            {
                lock (LockDisposeQueue)
                {
                    DisposeQueue.Remove(guid);
                    MeshCache.Add(guid, meshes);
                    return meshes;
                }
            }
            else if (PointCache.TryGetValue(guid, out var points))
            {
                meshes = MeshMaker.CreateMeshes(_pointAccessor, points, _createMeshHandler);
                MeshCache.Add(guid, meshes);
            }
            //no points yet, probably in loading queue
            return null;
        }

        /// <summary>
        /// Disposes of unused meshes, if needed. Depends on the dispose rate and the expiration frequency of the MeshCache.
        /// </summary>
        public override void ProcessDisposeQueue()
        {
            if (_deltaTimeSinceLastDisposal < DisposeRate)
                _deltaTimeSinceLastDisposal += Time.DeltaTime;
            else
            {
                _deltaTimeSinceLastDisposal = 0;

                lock (LockDisposeQueue)
                {
                    if (DisposeQueue.Count > 0)
                    {
                        var nodesInQueue = DisposeQueue.Count;
                        var count = nodesInQueue < _maxNumberOfDisposals ? nodesInQueue : _maxNumberOfDisposals;

                        for (int i = 0; i < count; i++)
                        {
                            var meshes = DisposeQueue.Last();
                            var removed = DisposeQueue.Remove(meshes.Key);
                            foreach (var mesh in meshes.Value)
                                mesh.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads points from the hard drive if they are neither in the loading queue nor in the PointCahce.
        /// </summary>
        /// <param name="guid">The octant for which the points should be loaded.</param>
        public override void TriggerPointLoading(string guid)
        {
            if (!LoadingQueue.Contains(guid) && LoadingQueue.Count <= MaxNumberOfNodesToLoad)
            {
                lock (LockLoadingQueue)
                {
                    LoadingQueue.Add(guid);
                }

                _ = Task.Run(() =>
                {
                    if (!PointCache.TryGetValue(guid, out var points))
                    {
                        points = _loadPointsHandler.Invoke(guid);
                        PointCache.Add(guid, points);
                    }

                    lock (LockLoadingQueue)
                    {
                        LoadingQueue.Remove(guid);
                    }
                });
            }
        }

        private void OnItemEvictedFromCache(object guid, object meshes, EvictionReason reason, object state)
        {
            lock (LockDisposeQueue)
            {
                DisposeQueue.Add((string)guid, (IEnumerable<GpuMesh>)meshes);
            }
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
                    PointCache.Dispose();
                    MeshCache.Dispose();

                    LockDisposeQueue = null;
                    LockLoadingQueue = null;
                    foreach (var meshes in DisposeQueue)
                    {
                        foreach (var mesh in meshes.Value)
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
        ~PointCloudDataHandler()
        {
            Dispose(disposing: false);
        }
    }
}
