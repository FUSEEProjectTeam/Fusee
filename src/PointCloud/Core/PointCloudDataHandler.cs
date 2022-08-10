using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core.Accessors;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Called in "determine visibility for node" - if the _pointCache does not contain the points load them.
    /// </summary>
    /// <param name="guid">Unique ID of an octant.</param>
    public delegate void TriggerPointLoading(OctantId guid);

    /// <summary>
    /// Delegate that allows to inject the loading method of the PointReader - loads the points from file.
    /// </summary>
    /// <param name="guid">Unique ID of an octant.</param>
    public delegate TPoint[] LoadPointsHandler<TPoint>(OctantId guid);

    /// <summary>
    /// Delegate for a method that tries to get the mesh(es) of an octant. If they are not cached yet, they should be created an added to the _gpuDataCache.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public delegate IEnumerable<GpuMesh> GetMeshes(OctantId guid);

    /// <summary>
    /// Delegate for a method that tries to get the <see cref="InstanceData"/> of an octant. If they are not cached yet, they should be created an added to the _gpuDataCache.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public delegate IEnumerable<InstanceData> GetInstanceData(OctantId guid);

    /// <summary>
    /// Generic delegate to inject a method that nows how to actually create a GpuMesh or InstanceData for the given point type.
    /// </summary>
    /// <typeparam name="TGpuData"></typeparam>
    /// <typeparam name="TPoint">Generic that describes the point type.</typeparam>
    /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/> that can be used to access the point data without casting the points.</param>
    /// <param name="points">The point cloud points as generic array.</param>
    /// <returns></returns>
    public delegate TGpuData CreateGpuData<TGpuData, TPoint>(PointAccessor<TPoint> ptAccessor, TPoint[] points);

    /// <summary>
    /// Manages the caching and loading of point and mesh data.
    /// </summary>
    /// <typeparam name="TGpuData"></typeparam>
    /// <typeparam name="TPoint"></typeparam>
    public class PointCloudDataHandler<TGpuData, TPoint> : PointCloudDataHandlerBase<TGpuData> where TPoint : new() where TGpuData : IDisposable
    {
        /// <summary>
        /// Caches loaded points.
        /// </summary>
        private readonly MemoryCache<OctantId, TPoint[]> _pointCache;

        /// <summary>
        /// Caches loaded points.
        /// </summary>
        private readonly MemoryCache<OctantId, IEnumerable<TGpuData>> _gpuDataCache;

        private readonly PointAccessor<TPoint> _pointAccessor;
        private readonly CreateGpuData<TGpuData, TPoint> _createGpuDataHandler;
        private readonly LoadPointsHandler<TPoint> _loadPointsHandler;
        private const int _maxNumberOfDisposals = 1;
        private float _deltaTimeSinceLastDisposal;
        private readonly bool _doRenderInstanced;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="pointAccessor">The point accessor that allows to access the point data.</param>
        /// <param name="createMeshHandler">Method that knows how to create a mesh for the explicit point type (see <see cref="MeshMaker"/>).</param>
        /// <param name="loadPointsHandler">The method that is able to load the points from the hard drive/file.</param>
        /// <param name="doRenderInstanced"></param>
        public PointCloudDataHandler(PointAccessor<TPoint> pointAccessor, CreateGpuData<TGpuData, TPoint> createMeshHandler, LoadPointsHandler<TPoint> loadPointsHandler, bool doRenderInstanced = false)
        {
            _pointCache = new();
            _gpuDataCache = new()
            {
                SlidingExpiration = 30,
                ExpirationScanFrequency = 31
            };

            _createGpuDataHandler = createMeshHandler;
            _loadPointsHandler = loadPointsHandler;
            _pointAccessor = pointAccessor;

            _doRenderInstanced = doRenderInstanced;

            LoadingQueue = new((8 ^ 8) / 8);
            DisposeQueue = new Dictionary<OctantId, IEnumerable<TGpuData>>((8 ^ 8) / 8);

            _gpuDataCache.HandleEvictedItem = OnItemEvictedFromCache;
        }

        /// <summary>
        /// First looks in the mesh cache, if there are meshes return, 
        /// else look in the DisposeQueue, if there are meshes return,
        /// else look in the point cache, if there are points create a mesh and add to the _meshCache.
        /// </summary>
        /// <param name="guid">The unique id of an octant.</param>
        /// <returns></returns>
        public override IEnumerable<TGpuData> GetGpuData(OctantId guid)
        {
            if (_gpuDataCache.TryGetValue(guid, out var gpuData))
                return gpuData;
            else if (DisposeQueue.TryGetValue(guid, out gpuData))
            {
                lock (LockDisposeQueue)
                {
                    DisposeQueue.Remove(guid);
                    _gpuDataCache.Add(guid, gpuData);
                    return gpuData;
                }
            }
            else if (_pointCache.TryGetValue(guid, out var points))
            {
                if (!_doRenderInstanced)
                    gpuData = MeshMaker.CreateMeshes(_pointAccessor, points, _createGpuDataHandler);
                else
                    gpuData = MeshMaker.CreateInstanceData(_pointAccessor, points, _createGpuDataHandler);
                _gpuDataCache.Add(guid, gpuData);
            }
            //no points yet, probably in loading queue
            return null;
        }

        /// <summary>
        /// Disposes of unused meshes, if needed. Depends on the dispose rate and the expiration frequency of the _meshCache.
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
        public override void TriggerPointLoading(OctantId guid)
        {
            if (!LoadingQueue.Contains(guid) && LoadingQueue.Count <= MaxNumberOfNodesToLoad)
            {
                lock (LockLoadingQueue)
                {
                    LoadingQueue.Add(guid);
                }

                _ = Task.Run(() =>
                {
                    if (!_pointCache.TryGetValue(guid, out var points))
                    {
                        points = _loadPointsHandler.Invoke(guid);
                        _pointCache.Add(guid, points);
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
                DisposeQueue.Add((OctantId)guid, (IEnumerable<TGpuData>)meshes);
            }
        }
    }
}