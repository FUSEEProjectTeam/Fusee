using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.PointCloud.Common;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
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
    public delegate MemoryMappedFile LoadPointsHandler(OctantId guid);

    /// <summary>
    /// Delegate that allows to inject method of the PointReader that knows how to return a byte array containing all bytes of one attribute of the point cloud.
    /// </summary>
    /// <param name="attribName"></param>
    /// <param name="pointsMmf"></param>
    /// <param name="guid"></param>
    /// <returns></returns>
    public delegate byte[] GetAllBytesForAttributeHandler(string attribName, MemoryMappedFile pointsMmf, OctantId guid);

    /// <summary>
    /// Delegate for a method that tries to get the mesh(es) of an octant. If they are not cached yet, they should be created an added to the _gpuDataCache.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public delegate IEnumerable<GpuMesh> GetMeshes(OctantId guid);

    /// <summary>
    /// Delegate for a method that tries to get the mesh(es) of an octant. If they are not cached yet, they should be created an added to the _gpuDataCache.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public delegate IEnumerable<Mesh> GetDynamicMeshes(OctantId guid);

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
    /// <param name="points">The points.</param>
    public delegate TGpuData CreateGpuData<TGpuData>(MemoryOwner<VisualizationPoint> points);

    /// <summary>
    /// Manages the caching and loading of point and mesh data.
    /// </summary>
    /// <typeparam name="TGpuData">Generic for the point/mesh type.</typeparam>
    public class PointCloudDataHandler<TGpuData> : PointCloudDataHandlerBase<TGpuData>, IDisposable where TGpuData : IDisposable
    {
        /// <summary>
        /// If we encounter an error during our loading process, this event is being triggered
        /// </summary>
        public EventHandler<ErrorEventArgs>? OnLoadingErrorEvent;

        /// <summary>
        /// Information about the point cloud, needed to create meshes.
        /// </summary>
        public CreateMeshMetaData MetaData { get; private set; }

        private readonly HandleReadExtraBytes? _handleExtraBytes;

        private HashSet<OctantId> _updateFromInvalidateCache = new();

        /// <summary>
        /// Caches loaded raw points.
        /// </summary>
        private readonly MemoryCache<OctantId, MemoryMappedFile> _rawPointCache;

        /// <summary>
        /// Caches loaded points that are ready for the visualization.
        /// </summary>
        private readonly MemoryCache<OctantId, MemoryOwner<VisualizationPoint>> _visPtCache;

        /// <summary>
        /// Caches loaded gpu data.
        /// </summary>
        private readonly MemoryCache<OctantId, IEnumerable<TGpuData>> _gpuDataCache;

        private readonly LoadPointsHandler _loadPointsHandler;
        private readonly Func<OctantId, long> _getNumberOfPointsInNode;
        private readonly GetAllBytesForAttributeHandler _getAllBytesForAttribute;

        private readonly bool _doRenderInstanced;

        private readonly ConcurrentDictionary<OctantId, OctantId> _loadingPointsTriggeredFor = new();
        private readonly ConcurrentDictionary<OctantId, OctantId> _creatingMeshesTriggeredFor = new();

        private readonly ConcurrentQueue<IEnumerable<TGpuData>> _disposeQueue = new();
        private const int _maxNumberOfDisposals = 10;
        private float _deltaTimeSinceLastDisposal = 0.0f;

        private bool _disposed;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="createMeshHandler">Method that knows how to create a mesh for the explicit point type (see <see cref="MeshMaker"/>).</param>
        /// <param name="handleExtraBytes">Method that know how to handle the extra bytes when creating the mesh for the points.</param>
        /// <param name="metaData">Information about the point cloud, needed to create meshes.</param>
        /// <param name="loadPointsHandler">The method that is able to load the points from the hard drive/file.</param>
        /// <param name="getNumberOfPointsInNode"></param>
        /// <param name="getAllBytesForAttribute"></param>
        /// <param name="doRenderInstanced"></param>
        public PointCloudDataHandler(CreateGpuData<TGpuData> createMeshHandler, HandleReadExtraBytes? handleExtraBytes, CreateMeshMetaData metaData, LoadPointsHandler loadPointsHandler, Func<OctantId, long> getNumberOfPointsInNode, GetAllBytesForAttributeHandler getAllBytesForAttribute, bool doRenderInstanced = false)
        {
            _rawPointCache = new()
            {
                SlidingExpiration = 15,
                ExpirationScanFrequency = 16
            };

            _visPtCache = new()
            {
                SlidingExpiration = 60,
                ExpirationScanFrequency = 61
            };

            _gpuDataCache = new()
            {
                SlidingExpiration = 30,
                ExpirationScanFrequency = 31
            };

            CreateGpuDataHandler = createMeshHandler;
            _loadPointsHandler = loadPointsHandler;
            _getNumberOfPointsInNode = getNumberOfPointsInNode;
            _doRenderInstanced = doRenderInstanced;
            _handleExtraBytes = handleExtraBytes;
            _getAllBytesForAttribute = getAllBytesForAttribute;
            MetaData = metaData;

            _gpuDataCache.HandleEvictedItem += OnItemEvictedFromGpuDataCache;
            _rawPointCache.HandleEvictedItem += OnItemEvictedFromRawPointCache;
            _visPtCache.HandleEvictedItem += OnItemEvictedFromVisPointCache;

            InvalidateCacheToken.IsDirtyPropertyChanged += (isDirty) =>
            {
                if (isDirty)
                {
                    _updateFromInvalidateCache = _visPtCache.GetKeys.ToHashSet();
                }
            };
        }

        private HashSet<OctantId> _queuedForUpdate = new HashSet<OctantId>();

        /// <summary>
        /// First looks in the mesh cache, if there isn't pending update for this mesh, return.
        /// Else try to trigger the mesh creation.
        /// </summary>
        /// <param name="octantId">The unique id of an octant.</param>
        /// <param name="doUpdateIf"> Allows inserting a condition, if true the mesh will be updated. This is an addition to <see cref="InvalidateGpuDataCache.IsDirty"/></param>
        /// <returns></returns>
        public override IEnumerable<TGpuData>? GetGpuData(OctantId octantId, Func<bool>? doUpdateIf)
        {
            Guard.IsNotNull(CreateGpuDataHandler);
            IEnumerable<TGpuData>? gpuData;

            var doUpdate = doUpdateIf != null && doUpdateIf.Invoke();

            //Queue meshes for update.
            if ((doUpdate || _updateFromInvalidateCache.Contains(octantId)) && !_queuedForUpdate.Contains(octantId))
            {
                _queuedForUpdate.Add(octantId);
                _updateFromInvalidateCache.Remove(octantId);
            }

            //Update or return meshes.
            if (_queuedForUpdate.Contains(octantId))
            {
                _gpuDataCache.TryGetValue(octantId, out gpuData);

                if (gpuData == null)
                {
                    //Octant contents need to be updated / rendered but we haven't a mesh to update.
                    TriggerMeshCreation(octantId);
                    return null;
                }

                if (UpdateGpuData(octantId, gpuData))
                    _queuedForUpdate.Remove(octantId);
            }
            else if (_gpuDataCache.TryGetValue(octantId, out gpuData))
            {
                return gpuData;
            }
            else
            {
                TriggerMeshCreation(octantId);
            }

            //no points yet, probably in loading queue
            return null;
        }

        /// <summary>
        /// Disposes of unused meshes, if needed. Depends on the dispose rate and the expiration frequency of the gpu data cache.
        /// Make sure to call this on the main thread.
        /// </summary>
        public override void ProcessDisposeQueue()
        {
            if (_deltaTimeSinceLastDisposal < DisposeRate)
                _deltaTimeSinceLastDisposal += Time.DeltaTime;
            else
            {
                _deltaTimeSinceLastDisposal = 0;

                if (_disposeQueue.Count > 0)
                {
                    var nodesInQueue = _disposeQueue.Count;
                    var count = nodesInQueue < _maxNumberOfDisposals ? nodesInQueue : _maxNumberOfDisposals;

                    for (int i = 0; i < count; i++)
                    {
                        _disposeQueue.TryDequeue(out var gpuData);
                        foreach (var data in gpuData)
                            data.Dispose();
                    }
                }

            }
        }

        /// <summary>
        /// Returns all bytes of one node for a specific attribute.
        /// </summary>
        /// <param name="attribName"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public override byte[] GetAllBytesForAttribute(string attribName, OctantId guid)
        {
            //NOTE: Don't add to the caches here! This defies the purpose of the caches, that is to save the "lastly-viewed" nodes.
            if (!_rawPointCache.TryGetValue(guid, out var pointsMmf))
                pointsMmf = _loadPointsHandler.Invoke(guid);

            return _getAllBytesForAttribute(attribName, pointsMmf, guid);
        }

        private void TriggerMeshCreation(OctantId octantId)
        {
            Guard.IsNotNull(CreateGpuDataHandler);

            if (!_visPtCache.TryGetValue(octantId, out var points)) return;
            if (_creatingMeshesTriggeredFor.ContainsKey(octantId)) return;

            _creatingMeshesTriggeredFor.TryAdd(octantId, octantId);

            IEnumerable<TGpuData> gpuData;

            int numberOfPointsInNode = (int)_getNumberOfPointsInNode(octantId);
            if (!_doRenderInstanced)
                gpuData = MeshMaker.CreateMeshes(points, CreateGpuDataHandler);
            else
                gpuData = MeshMaker.CreateInstanceData(points, CreateGpuDataHandler);

            foreach (var mesh in gpuData)
            {
                NewMeshAction?.Invoke(mesh);
            }

            _gpuDataCache.AddOrUpdate(octantId, gpuData);
            _creatingMeshesTriggeredFor.TryRemove(octantId, out var _);
        }

        /// <summary>
        /// Runs a task that loads points from the hard drive.
        /// </summary>
        /// <param name="guid">The octant for which the points should be loaded.</param>
        public override void TriggerPointLoading(OctantId guid)
        {
            if (_loadingPointsTriggeredFor.ContainsKey(guid))
                return;

            _loadingPointsTriggeredFor.TryAdd(guid, guid);
            _ = Task.Run(() =>
            {
                if (!_rawPointCache.TryGetValue(guid, out var pointsMmf))
                {
                    pointsMmf = _loadPointsHandler.Invoke(guid);
                    _rawPointCache.AddOrUpdate(guid, pointsMmf);
                }

                CreateVisPointCacheEntry(pointsMmf, guid);
                _loadingPointsTriggeredFor.TryRemove(guid, out var _);

            }).ContinueWith((finishedTask) =>
            {
                // if an exception happened during loading process call the error event for further handling of the situation
                if (finishedTask.Exception != null)
                {
                    OnLoadingErrorEvent?.Invoke(this, new ErrorEventArgs(finishedTask.Exception));
                }
            });
        }

        private void CreateVisPointCacheEntry(MemoryMappedFile pointsMmf, OctantId guid)
        {
            if (_visPtCache.TryGetValue(guid, out var _)) return;

            int numberOfPointsInNode = (int)_getNumberOfPointsInNode(guid);
            var visPts = MeshMaker.CreateVisualizationPoints(pointsMmf, numberOfPointsInNode, _handleExtraBytes, MetaData, OnLoadingErrorEvent);
            _visPtCache.AddOrUpdate(guid, visPts);
        }

        private void OnItemEvictedFromRawPointCache(object guid, object? obj, EvictionReason reason, object? state)
        {
            if (obj != null && obj is MemoryMappedFile mo)
            {
                mo.Dispose();
            }
        }

        private void OnItemEvictedFromVisPointCache(object guid, object? obj, EvictionReason reason, object? state)
        {
            if (obj != null && obj is MemoryOwner<VisualizationPoint> mo)
            {
                mo.Dispose();
            }
        }

        private void OnItemEvictedFromGpuDataCache(object guid, object? meshes, EvictionReason reason, object? state)
        {
            if (meshes == null) return;
            _disposeQueue.Enqueue((IEnumerable<TGpuData>)meshes);
        }

        private bool UpdateGpuData(OctantId octantId, IEnumerable<TGpuData> gpuData)
        {
            var updateSucceded = UpdateFromVisPoints(octantId, ref gpuData);
            if (updateSucceded)
            {
                foreach (var mesh in gpuData)
                {
                    UpdatedMeshAction?.Invoke(mesh);
                }
                _gpuDataCache.AddOrUpdate(octantId, gpuData);
            }

            return updateSucceded;
        }

        private bool UpdateFromVisPoints(OctantId octantId, ref IEnumerable<TGpuData> gpuData)
        {
            Guard.IsNotNull(CreateGpuDataHandler);

            if (_visPtCache.TryGetValue(octantId, out var points))
            {
                if (UpdateGpuDataCache != null)
                {
                    UpdateGpuDataCache.Invoke(ref gpuData, points);
                    return true;
                }
                else
                {
                    //Mesh has to be created anew.
                    TriggerMeshCreation(octantId);
                }
            }

            //No points in cache - cannot update (point loading is triggered in VisibilityTester)
            return false;
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        ///</summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.

                _gpuDataCache.Dispose();
                _visPtCache.Dispose();
                _rawPointCache.Dispose();

                foreach (var gpuDatas in _disposeQueue)
                {
                    foreach (var gpuData in gpuDatas)
                    {
                        gpuData.Dispose();
                    }
                }

                // Note disposing has been done.
                _disposed = true;
            }
        }
    }
}