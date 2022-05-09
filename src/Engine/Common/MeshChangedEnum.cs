namespace Fusee.Engine.Common
{
    /// <summary>
    /// Propagates Mesh properties changed status to the MeshManager 
    /// </summary>
    public enum MeshChangedEnum
    {
        /// <summary>
        /// Mesh data will be removed from the GPU
        /// </summary>
        Disposed,

        /// <summary>
        /// The Vertices have changed.
        /// </summary>
        Vertices,

        /// <summary>
        /// The Triangles have changed.
        /// </summary>
        Triangles,

        /// <summary>
        /// The field Mesh changed.
        /// </summary>
        Colors,

        /// <summary>
        /// The field Colors1 changed.
        /// </summary>
        Colors1,

        /// <summary>
        /// The field Colors2 changed.
        /// </summary>
        Colors2,

        /// <summary>
        /// The field Normals changed.
        /// </summary>
        Normals,

        /// <summary>
        /// The field UVs changed.
        /// </summary>
        Uvs,

        /// <summary>
        /// The field BoneWeights changed.
        /// </summary>
        BoneWeights,

        /// <summary>
        /// The field BoneIndices changed.
        /// </summary>
        BoneIndices,

        /// <summary>
        /// The field Tangents changed.
        /// </summary>
        Tangents,

        /// <summary>
        /// The field BiTangents changed.
        /// </summary>
        BiTangents

    }
}