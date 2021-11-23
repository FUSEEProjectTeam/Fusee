using Fusee.Engine.Core.Scene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Propagates Mesh properties changed status inside <see cref="MeshDataEventArgs"/> 
    /// </summary>
    public enum MeshChangedEnum
    {
        /// <summary>
        /// The <see cref="Mesh"/> has been disposed.
        /// </summary>
        Disposed = 0,

        /// <summary>
        /// The field <see cref="Mesh.Vertices"/> changed.
        /// </summary>
        Vertices,

        /// <summary>
        /// The field <see cref="Mesh.Triangles"/> changed.
        /// </summary>
        Triangles,

        /// <summary>
        /// The field <see cref="Mesh.Colors"/> changed.
        /// </summary>
        Colors,

        /// <summary>
        /// The field <see cref="Mesh.Colors1"/> changed.
        /// </summary>
        Colors1,

        /// <summary>
        /// The field <see cref="Mesh.Colors2"/> changed.
        /// </summary>
        Colors2,

        /// <summary>
        /// The field <see cref="Mesh.Normals"/> changed.
        /// </summary>
        Normals,

        /// <summary>
        /// The field <see cref="Mesh.UVs"/> changed.
        /// </summary>
        Uvs,

        /// <summary>
        /// The field <see cref="Mesh.BoneWeights"/> changed.
        /// </summary>
        BoneWeights,

        /// <summary>
        /// The field <see cref="Mesh.BoneIndices"/> changed.
        /// </summary>
        BoneIndices,

        /// <summary>
        /// The field <see cref="Mesh.Tangents"/> changed.
        /// </summary>
        Tangents,

        /// <summary>
        /// The field <see cref="Mesh.BiTangents"/> changed.
        /// </summary>
        BiTangents

    }
}