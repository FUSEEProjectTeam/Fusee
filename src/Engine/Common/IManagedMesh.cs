using System;

namespace Fusee.Engine.Common
{
    public interface IManagedMesh : IDisposable
    {
        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs> DisposeData;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public Suid SessionUniqueIdentifier { get; }

        PrimitiveType MeshType { get; set; }
    }
}
