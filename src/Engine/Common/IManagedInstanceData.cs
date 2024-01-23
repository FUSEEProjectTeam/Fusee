using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Interface for instance data implementations, that are observed by the MeshManager. 
    /// The MeshManger is handling the memory allocation and deallocation of meshes and their instance datas on the GPU.
    /// </summary>
    public interface IManagedInstanceData : IDisposable
    {
        /// <summary>
        /// This event notifies observing MeshManager about property changes and the InstanceData's disposal.
        /// </summary>
        public event EventHandler<InstanceDataChangedEventArgs> DataChanged;

        /// <summary>
        /// This event notifies observing MeshManager about property changes and the InstanceData's disposal.
        /// </summary>
        public event EventHandler<InstanceDataChangedEventArgs> DisposeData;

        /// <summary>
        /// The unique id of the object.
        /// </summary>
        public Guid UniqueId
        {
            get;
        }
    }
}