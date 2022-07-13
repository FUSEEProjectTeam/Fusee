using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Used by <see cref="IManagedInstanceData"/> to notify a MeshManager about a change of one of its properties.
    /// </summary>
    public class InstanceDataChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="IManagedInstanceData"/> that was changed.
        /// </summary>
        public IManagedInstanceData InstanceData { get; }

        /// <summary>
        /// The type of change that occurred.
        /// </summary>
        public InstanceDataChangedEnum ChangedEnum { get; protected set; }


        /// <summary>
        /// Creates a new instance of type <see cref="InstanceDataChangedEventArgs"/>.
        /// </summary>
        /// <param name="instanceData">The <see cref="IManagedInstanceData"/> that was changed.</param>
        /// <param name="instanceDataChangedEnum">The type of change that occurred.</param>
        public InstanceDataChangedEventArgs(IManagedInstanceData instanceData, InstanceDataChangedEnum instanceDataChangedEnum)
        {
            InstanceData = instanceData;
            ChangedEnum = instanceDataChangedEnum;
        }
    }
}