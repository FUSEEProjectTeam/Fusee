using Fusee.Engine.Core;
using System.Collections.Generic;

namespace Fusee.PointCloud.Common
{
    public abstract class PointCloudDataHandlerBase
    {
        public abstract IEnumerable<GpuMesh> GetMeshes(string guid);
        public abstract void TriggerPointLoading(string guid);

        //Nodes that are queued for loading in the background
        protected List<string> LoadingQueue;

        protected static object LockLoadingQueue = new();

        //Number of nodes that will be loaded, starting with the one with the biggest screen projected size to ensure no octant is loaded that will be invisible in a few frames.
        //Load the five biggest nodes (screen projected size) as proposed in Schütz' thesis.
        protected int MaxNumberOfNodesToLoad = 5;

        protected Dictionary<string, IEnumerable<GpuMesh>> DisposeQueue;

        public float DisposeRate { get; set; } = 1 / 3f;

        public abstract void ProcessDisposeQueue();

        protected static object LockDisposeQueue = new();
    }
}
