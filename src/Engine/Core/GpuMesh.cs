using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Delegate that points to a method that is able to create a <see cref="GpuMesh"/>
    /// </summary>
    /// <param name="primitiveType">See <see cref="PrimitiveType"/>.</param>
    /// <param name="vertices">The vertices of the mesh.</param>
    /// <param name="triangles">The triangles of the mesh.</param>
    /// <param name="normals">The normals of the mesh.</param>
    /// <param name="colors">The colors of the mesh.</param>
    /// <param name="colors1">The second color field of the mesh.</param>
    /// <param name="colors2">The third color field of the mesh.</param>
    /// <param name="uvs">The UV coordinates of the mesh.</param>
    /// <param name="tangents">The tangents of the mesh.</param>
    /// <param name="bitangents">The bitangents of the mesh.</param>
    /// <param name="boneIndices">The boneIndices of the mesh.</param>
    /// <param name="boneWeights">The vertiboneWeightsces of the mesh.</param>
    /// <returns></returns>
    public delegate GpuMesh CreateGpuMesh(PrimitiveType primitiveType, float3[] vertices, ushort[] triangles = null,
            float3[] normals = null, uint[] colors = null, uint[] colors1 = null, uint[] colors2 = null, float2[] uvs = null,
            float4[] tangents = null, float3[] bitangents = null, float4[] boneIndices = null, float4[] boneWeights = null);

    /// <summary>
    /// This type of mesh doesn't create a copy of the mesh data in the RAM.
    /// </summary>
    public class GpuMesh : SceneComponent, IManagedMesh
    {
        /// <summary>
        /// The bounding box of this geometry.
        /// </summary>
        public AABBf BoundingBox;

        /// <summary>
        /// MeshChanged event notifies observing MeshManager the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs> DisposeData;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public Suid SessionUniqueIdentifier { get; } = Suid.GenerateSuid();

        /// <summary>
        /// Type of data of this mesh (e.g. Triangles, Points, Lines, etc.)
        /// </summary>
        public PrimitiveType MeshType { get; set; }

        private bool _disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DisposeData?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Disposed));
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizers (historically referred to as destructors) are used to perform any necessary final clean-up when a class instance is being collected by the garbage collector.
        /// </summary>
        ~GpuMesh()
        {
            Dispose(false);
        }

    }
}