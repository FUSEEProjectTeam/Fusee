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
using System.Threading;
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
    /// <param name="numberOfPointsInGpuData">The number of points in one GPU Data.</param>
    /// <param name="handleExtraBytes">Method that knows how to deal with a "extra byte" array.</param>
    /// <param name="metaData">Meta information about the point stream.</param>
    /// <param name="onPointCloudReadError">Event handler that deals with occurring errors.</param>
    /// <returns></returns>
    public delegate TGpuData CreateGpuData<TGpuData>(byte[] points, int numberOfPointsInGpuData, HandleReadExtraBytes handleExtraBytes, CreateMeshMetaData metaData, EventHandler<ErrorEventArgs>? onPointCloudReadError);

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

        private readonly HandleReadExtraBytes _handleExtraBytes;

        private HashSet<OctantId> _meshesToUpdate = new();

        /// <summary>
        /// Caches loaded points.
        /// </summary>
        private MemoryCache<OctantId, MemoryMappedFile> _pointCache;

        /// <summary>
        /// Caches loaded points.
        /// </summary>
        private MemoryCache<OctantId, IEnumerable<TGpuData>> _gpuDataCache;

        private readonly LoadPointsHandler _loadPointsHandler;
        private readonly Func<OctantId, long> _getNumberOfPointsInNode;
        private readonly GetAllBytesForAttributeHandler _getAllBytesForAttribute;

        private readonly bool _doRenderInstanced;

        private readonly ConcurrentDictionary<OctantId, OctantId> _loadingTriggeredFor = new();

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
        public PointCloudDataHandler(CreateGpuData<TGpuData> createMeshHandler, HandleReadExtraBytes handleExtraBytes, CreateMeshMetaData metaData, LoadPointsHandler loadPointsHandler, Func<OctantId, long> getNumberOfPointsInNode, GetAllBytesForAttributeHandler getAllBytesForAttribute, bool doRenderInstanced = false)
        {
            _pointCache = new()
            {
                SlidingExpiration = 15,
                ExpirationScanFrequency = 16
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
            _pointCache.HandleEvictedItem += OnItemEvictedFromPointCache;

            InvalidateCacheToken.IsDirtyPropertyChanged += (isDirty) =>
            {
                if (isDirty)
                {
                    _meshesToUpdate = _pointCache.GetKeys.ToHashSet();
                }
            };
        }

        private GpuDataState DoUpdateGpuData(OctantId octantId, ref IEnumerable<TGpuData> gpuData)
        {
            Guard.IsNotNull(CreateGpuDataHandler);

            if (_pointCache.TryGetValue(octantId, out var pointMmf))
            {
                if (UpdateGpuDataCache != null)
                {
                    UpdateGpuDataCache.Invoke(ref gpuData, pointMmf);
                }
                else
                {
                    int numberOfPointsInNode = (int)_getNumberOfPointsInNode(octantId);
                    if (!_doRenderInstanced)
                        gpuData = MeshMaker.CreateMeshes(pointMmf, numberOfPointsInNode, CreateGpuDataHandler, _handleExtraBytes, MetaData, OnLoadingErrorEvent);
                    else
                        gpuData = MeshMaker.CreateInstanceData(pointMmf, numberOfPointsInNode, CreateGpuDataHandler, _handleExtraBytes, MetaData, OnLoadingErrorEvent);
                }
                return GpuDataState.Changed;
            }

            //No points in cache - cannot update (point loading is triggered in VisibilityTester)
            return GpuDataState.None;
        }

        /// <summary>
        /// First looks in the mesh cache, if there are meshes return,
        /// else look in the DisposeQueue, if there are meshes return,
        /// else look in the point cache, if there are points create a mesh and add to the _meshCache.
        /// </summary>
        /// <param name="octantId">The unique id of an octant.</param>
        /// <param name="doUpdateIf"> Allows inserting a condition, if true the mesh will be updated. This is an addition to <see cref="InvalidateGpuDataCache.IsDirty"/></param>
        /// <param name="gpuDataState">State of the gpu data in it's life cycle.</param>
        /// <returns></returns>
        public override IEnumerable<TGpuData>? GetGpuData(OctantId octantId, Func<bool>? doUpdateIf, out GpuDataState gpuDataState)
        {
            Guard.IsNotNull(CreateGpuDataHandler);

            if (_gpuDataCache.TryGetValue(octantId, out var gpuData))
            {
                var doUpdate = doUpdateIf != null && doUpdateIf.Invoke();
                if (_meshesToUpdate.Contains(octantId) || doUpdate)
                {
                    gpuDataState = DoUpdateGpuData(octantId, ref gpuData);

                    if (gpuDataState != GpuDataState.None)
                    {
                        _gpuDataCache.AddOrUpdate(octantId, gpuData);
                        _meshesToUpdate.Remove(octantId);
                        if (_meshesToUpdate.Count == 0)
                        {
                            InvalidateCacheToken.IsDirty = false;
                        }
                        return gpuData;
                    }

                    //Mesh remains in the _meshesToUpdate list but couldn't be updated because the points were missing.
                    _gpuDataCache.Remove(octantId);
                    return null;
                }
                else
                    gpuDataState = GpuDataState.Unchanged;

                return gpuData;
            }
            else
            {
                if (_pointCache.TryGetValue(octantId, out var points))
                {
                    int numberOfPointsInNode = (int)_getNumberOfPointsInNode(octantId);
                    if (!_doRenderInstanced)
                        gpuData = MeshMaker.CreateMeshes(points, numberOfPointsInNode, CreateGpuDataHandler, _handleExtraBytes, MetaData, OnLoadingErrorEvent);
                    else
                        gpuData = MeshMaker.CreateInstanceData(points, numberOfPointsInNode, CreateGpuDataHandler, _handleExtraBytes, MetaData, OnLoadingErrorEvent);

                    _gpuDataCache.AddOrUpdate(octantId, gpuData);
                    gpuDataState = GpuDataState.New;
                    return gpuData;
                }
            }

            gpuDataState = GpuDataState.None;

            //no points yet, probably in loading queue
            return null;
        }

        /// <summary>
        /// Runs a task that loads points from the hard drive.
        /// </summary>
        /// <param name="guid">The octant for which the points should be loaded.</param>
        public override void TriggerPointLoading(OctantId guid)
        {
            if (_pointCache.TryGetValue(guid, out var pointsMmf)) return;
            if (_loadingTriggeredFor.ContainsKey(guid))
                return;

            _loadingTriggeredFor.TryAdd(guid, guid);

            _ = Task.Run(() =>
            {
                pointsMmf = _loadPointsHandler.Invoke(guid);
                _loadingTriggeredFor.TryRemove(guid, out var _);
                _pointCache.AddOrUpdate(guid, pointsMmf);

            }).ContinueWith((finishedTask) =>
            {
                // if an exception happened during loading process call the error event for further handling of the situation
                if (finishedTask.Exception != null)
                {
                    OnLoadingErrorEvent?.Invoke(this, new ErrorEventArgs(finishedTask.Exception));
                }
            });
        }

        private void OnItemEvictedFromPointCache(object guid, object? obj, EvictionReason reason, object? state)
        {
            if (obj != null && obj is MemoryMappedFile mo)
            {
                mo.Dispose();
            }
        }

        private void OnItemEvictedFromGpuDataCache(object guid, object? meshes, EvictionReason reason, object? state)
        {
            if (meshes == null) return;

            foreach (var gpuData in (IEnumerable<TGpuData>)meshes)
            {
                gpuData.Dispose();
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
            if (!_pointCache.TryGetValue(guid, out var pointsMmf))
            {
                pointsMmf = _loadPointsHandler.Invoke(guid);
                _pointCache.AddOrUpdate(guid, pointsMmf);
            }

            return _getAllBytesForAttribute(attribName, pointsMmf, guid);

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
                _pointCache.Dispose();


                // Note disposing has been done.
                _disposed = true;
            }
        }
    }
}