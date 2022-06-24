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
        /// One Vertex has changed.
        /// </summary>
        Vertex,

        /// <summary>
        /// The Triangles have changed.
        /// </summary>
        Triangles,

        /// <summary>
        /// One Triangle has changed.
        /// </summary>
        Triangle,

        /// <summary>
        /// The field Mesh changed.
        /// </summary>
        Colors,

        /// <summary>
        /// One color has changed.
        /// </summary>
        Color,

        /// <summary>
        /// The field Colors1 changed.
        /// </summary>
        Colors1,

        /// <summary>
        /// One Color1 has changed.
        /// </summary>
        Color1,

        /// <summary>
        /// The field Colors2 changed.
        /// </summary>
        Colors2,

        /// <summary>
        /// One Color2 has changed.
        /// </summary>
        Color2,

        /// <summary>
        /// The field Normals changed.
        /// </summary>
        Normals,

        /// <summary>
        /// One Normals has changed.
        /// </summary>
        Normal,

        /// <summary>
        /// The field UVs changed.
        /// </summary>
        Uvs,

        /// <summary>
        /// One UV has changed.
        /// </summary>
        Uv,

        /// <summary>
        /// The field BoneWeights changed.
        /// </summary>
        BoneWeights,

        /// <summary>
        /// One BoneWeight has changed.
        /// </summary>
        BoneWeight,

        /// <summary>
        /// The field BoneIndices changed.
        /// </summary>
        BoneIndices,

        /// <summary>
        /// One Bone Index has changed.
        /// </summary>
        BondeIndex,

        /// <summary>
        /// The field Tangents changed.
        /// </summary>
        Tangents,

        /// <summary>
        /// One Tangent has changed.
        /// </summary>
        Tangent,

        /// <summary>
        /// The field BiTangents changed.
        /// </summary>
        BiTangents,

        /// <summary>
        /// One BiTangent has changed.
        /// </summary>
        BiTangent
    }
}