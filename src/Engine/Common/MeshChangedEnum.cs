namespace Fusee.Engine.Common
{
    /// <summary>
    /// Propagates a <see cref="IManagedInstanceData"/> property's changed status to the MeshManager 
    /// </summary>
    public enum InstanceDataChangedEnum
    {
        /// <summary>
        /// The <see cref="IInstanceDataImp"/> belonging to a <see cref="IManagedInstanceData"/> need to be disposed of.
        /// </summary>
        Disposed,

        /// <summary>
        /// The positions / transformations of a <see cref="IManagedInstanceData"/> changed an need to be updated on the gpu.
        /// </summary>
        Transform,

        /// <summary>
        /// The colors of a <see cref="IManagedInstanceData"/> changed an need to be updated on the gpu.
        /// </summary>
        Colors
    }

    /// <summary>
    /// Propagates a <see cref="IManagedMesh"/> property's changed status to the MeshManager 
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