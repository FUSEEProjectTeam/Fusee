using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Defines mesh data (geometry).
    /// </summary>
    [ProtoContract]
    public class MeshComponent : SceneComponentContainer
    {
        /// <summary>
        /// The list of vertices.
        /// </summary>
        [ProtoMember(1)]
        public float3[] Vertices;

        /// <summary>
        /// The list of normals at the vertices.
        /// </summary>
        [ProtoMember(2)]
        public float3[] Normals;

        /// <summary>
        /// The list of texture coordinates at the vertices.
        /// </summary>
        [ProtoMember(3)]
        public float2[] UVs;

        /// <summary>
        /// The list of triagles. Three contiguous indeces into the <see cref="Vertices"/> list
        /// define one triangle.
        /// </summary>
        [ProtoMember(4)]
        public ushort[] Triangles;

        /// <summary>
        /// The bounding box of this geometry chunk.
        /// </summary>
        [ProtoMember(5)] 
        public AABBf BoundingBox;
    }
}
