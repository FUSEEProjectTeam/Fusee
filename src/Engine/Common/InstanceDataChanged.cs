using System;

namespace Fusee.Engine.Common
{
    public class InstanceDataChangedEventArgs : EventArgs
    {
        public IManagedInstanceData InstanceData { get; }

        public InstanceDataChangedEnum ChangedEnum { get; protected set; }


        public InstanceDataChangedEventArgs(IManagedInstanceData instanceData, InstanceDataChangedEnum instanceDataChangedEnum)
        {
            InstanceData = instanceData;
            ChangedEnum = instanceDataChangedEnum;
        }
    }
}
