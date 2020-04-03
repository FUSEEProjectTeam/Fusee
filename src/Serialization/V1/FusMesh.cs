using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Contains 3D geometry information (Vertices, Triangles, Normals, UVs, ...).
    /// </summary>
    [ProtoContract]
    public class FusMesh : FusComponent
    {
        #region Payload
        /// <summary>
        /// Gets and sets the vertices.
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        [ProtoMember(1)]
        public float3[] Vertices;

        /// <summary>
        /// Gets and sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        [ProtoMember(2)]
        public uint[] Colors;

        /// <summary>
        /// Gets and sets the normals.
        /// </summary>
        /// <value>
        /// The normals..
        /// </value>
        [ProtoMember(3)]
        public float3[] Normals;

        /// <summary>
        /// Gets and sets the UV-coordinates.
        /// </summary>
        /// <value>
        /// The UV-coordinates.
        /// </value>
        [ProtoMember(4)]
        public float2[] UVs;

        /// <summary>
        /// Gets and sets the boneweights.
        /// </summary>
        /// <value>
        /// The boneweights.
        /// </value>
        [ProtoMember(5)]
        public float4[] BoneWeights;

        /// <summary>
        /// Gets and sets the boneindices.
        /// </summary>
        /// <value>
        /// The boneindices.
        /// </value>
        [ProtoMember(6)]
        public float4[] BoneIndices;

        /// <summary>
        /// Gets and sets the triangles.
        /// </summary>
        /// <value>
        /// The triangles.
        /// </value>
        [ProtoMember(7)]
        public ushort[] Triangles;

        /// <summary>
        /// The bounding box of this geometry chunk.
        /// </summary>
        [ProtoMember(8)]
        public AABBf BoundingBox;

        /// <summary>
        /// The tangent of each triangle for bump mapping.
        /// w-component is handedness
        /// </summary>
        [ProtoMember(9)]
        public float4[] Tangents;

        /// <summary>
        /// The bitangent of each triangle for bump mapping.
        /// </summary>
        [ProtoMember(10)]
        public float3[] BiTangents;

        /// <summary>
        /// The type of the mesh ???
        /// </summary>
        [ProtoMember(11)]
        public int MeshType = 0;
        #endregion
    }

}