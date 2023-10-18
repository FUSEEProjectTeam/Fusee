using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Interface for meshes that are observed by the MeshManager. The MeshManger is handling the memory allocation and deallocation of meshes on the GPU.
    /// </summary>
    public interface IManagedMesh : IDisposable
    {
        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs> DisposeData;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public Guid UniqueIdentifier { get; }

        /// <summary>
        /// The primitive type this mesh is composed of.
        /// </summary>
        PrimitiveType MeshType { get; set; }
    }
}