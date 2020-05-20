using Fusee.Engine.Core.Scene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Propagates Mesh properties changed status inside <see cref="MeshDataEventArgs"/> 
    /// </summary>
    public enum MeshChangedEnum
    {
        /// <summary>
        /// The <see cref="Scene.Mesh"/> has been disposed.
        /// </summary>
        Disposed = 0,

        /// <summary>
        /// The field <see cref="Scene.Mesh.Vertices"/> changed.
        /// </summary>
        Vertices,

        /// <summary>
        /// The field <see cref="Scene.Mesh.Triangles"/> changed.
        /// </summary>
        Triangles,

        /// <summary>
        /// The field <see cref="Scene.Mesh.Colors"/> changed.
        /// </summary>
        Colors,

        /// <summary>
        /// The field <see cref="Scene.Mesh.Normals"/> changed.
        /// </summary>
        Normals,

        /// <summary>
        /// The field <see cref="Scene.Mesh.UVs"/> changed.
        /// </summary>
        Uvs,

        /// <summary>
        /// The field <see cref="Scene.Mesh.BoneWeights"/> changed.
        /// </summary>
        BoneWeights,

        /// <summary>
        /// The field <see cref="Scene.Mesh.BoneIndices"/> changed.
        /// </summary>
        BoneIndices,

        /// <summary>
        /// The field <see cref="Scene.Mesh.Tangents"/> changed.
        /// </summary>
        Tangents,

        /// <summary>
        /// The field <see cref="Scene.Mesh.BiTangents"/> changed.
        /// </summary>
        BiTangents

    }
}