using System;

namespace Fusee.Engine.Common
{
    public interface IManagedInstanceData
    {
        /// <summary>
        /// This event notifies observing MeshManager about property changes and the InstanceData's disposal.
        /// </summary>
        public event EventHandler<InstanceDataChangedEventArgs> DataChanged;

        /// <summary>
        /// This event notifies observing MeshManager about property changes and the InstanceData's disposal.
        /// </summary>
        public event EventHandler<InstanceDataChangedEventArgs> DisposeData;

        public Suid SessionUniqueId
        {
            get;
        }
    }
}
