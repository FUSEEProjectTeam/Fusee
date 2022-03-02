using Fusee.Engine.Core;
using System.Collections.Generic;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Manages the caching and loading of point and mesh data.
    /// </summary>
    public abstract class PointCloudDataHandlerBase
    {
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
        /// Contains nodes that are queued for loading in the background.
        /// </summary>
        protected List<string> LoadingQueue;

        /// <summary>
        /// Contains meshes that are marked for disposal.
        /// </summary>
        protected Dictionary<string, IEnumerable<GpuMesh>> DisposeQueue;

        /// <summary>
        /// Locking object for the loading queue.
        /// </summary>
        protected static object LockLoadingQueue = new();
        /// <summary>
        /// Locking object for the dispose queue.
        /// </summary>
        protected static object LockDisposeQueue = new();

        /// <summary>
        /// First looks in the mesh cache, if there are meshes return, 
        /// else look in the DisposeQueue, if there are meshes return,
        /// else look in the point cache, if there are points create a mesh and add to the MeshCache.
        /// </summary>
        /// <param name="guid">The unique id of an octant.</param>
        public abstract IEnumerable<GpuMesh> GetMeshes(string guid);

        /// <summary>
        /// Loads points from the hard drive if they are neither in the loading queue nor in the PointCahce.
        /// </summary>
        /// <param name="guid">The octant for which the points should be loaded.</param>
        public abstract void TriggerPointLoading(string guid);

        /// <summary>
        /// Disposes of unused meshes, if needed. Depends on the dispose rate and the expiration frequency of the MeshCache.
        /// </summary>
        public abstract void ProcessDisposeQueue();
    }
}