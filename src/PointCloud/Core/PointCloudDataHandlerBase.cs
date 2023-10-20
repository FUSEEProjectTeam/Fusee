// Ignore Spelling: kvp

using CommunityToolkit.HighPerformance.Buffers;
using Fusee.PointCloud.Common;
using System;
using System.Collections.Generic;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Manages the caching and loading of point and mesh data.
    /// </summary>
    public abstract class PointCloudDataHandlerBase<TGpuData> : IDisposable where TGpuData : IDisposable
    {
        /// <summary>
        /// Allows to inject a method that knows how to create a specific type of mesh.
        /// </summary>
        public CreateGpuData<TGpuData>? CreateGpuDataHandler;

        /// <summary>
        /// Token, that allows to invalidate the complete GpuData cache.
        /// </summary>
        public InvalidateGpuDataCache InvalidateCacheToken { get; } = new();

        /// <summary>
        /// Used to manage gpu pressure when disposing of a large quantity of meshes.
        /// </summary>
        public float DisposeRate = 1 / 3f;

        /// <summary>
        /// Number of nodes that will be loaded, starting with the one with the biggest screen projected size to ensure no octant is loaded that will be invisible in a few frames.
        /// Load the five biggest nodes (screen projected size) as proposed in Schütz' thesis.
        /// </summary>
        protected int MaxNumberOfNodesToLoad = 5;

        /// <summary>
        /// First looks in the mesh cache, if there are meshes return, 
        /// else look in the DisposeQueue, if there are meshes return,
        /// else look in the point cache, if there are points create a mesh and add to the MeshCache.
        /// </summary>
        /// <param name="guid">The unique id of an octant.</param>
        /// <param name="doUpdateIf">Allows inserting a condition, if true the mesh will be updated.</param>
        public abstract IEnumerable<TGpuData>? GetGpuData(OctantId guid, Func<bool>? doUpdateIf);

        /// <summary>
        /// Loads points from the hard drive if they are neither in the loading queue nor in the PointCahce.
        /// </summary>
        /// <param name="guid">The octant for which the points should be loaded.</param>
        public abstract void TriggerPointLoading(OctantId guid);

        /// <summary>
        /// Disposes of unused meshes, if needed. Depends on the dispose rate and the expiration frequency of the gpu data cache.
        /// Make sure to call this on the main thread.
        /// </summary>
        public abstract void ProcessDisposeQueue();

        /// <summary>
        /// Allows to update meshes with data from the points.
        /// </summary>
        public UpdateGpuData<IEnumerable<TGpuData>, MemoryOwner<VisualizationPoint>>? UpdateGpuDataCache;

        /// <summary>
        /// Used to inject a application dependent method that processes newly created gpu data.
        /// </summary>
        public Action<TGpuData>? NewMeshAction;

        /// <summary>
        /// Used to inject a application dependent method that processes gpu data after some update.
        /// </summary>
        public Action<TGpuData>? UpdatedMeshAction;

        /// <summary>
        /// Returns all bytes of one node for a specific attribute.
        /// </summary>
        /// <param name="attribName"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public abstract byte[] GetAllBytesForAttribute(string attribName, OctantId guid);

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        ///</summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
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
        protected virtual void Dispose(bool disposing)
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

                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# finalizer syntax for finalization code.
        /// This finalizer will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide finalizer in types derived from this class.
        /// </summary>
        ~PointCloudDataHandlerBase()
        {
            Dispose(disposing: false);
        }
    }
}