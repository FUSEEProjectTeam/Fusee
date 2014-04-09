
using Fusee.Math;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class MeshContainer
    {
        [ProtoMember(1)]
        public float3[] Vertices;

        [ProtoMember(2)]
        public float3[] Normals;

        [ProtoMember(3)]
        public float2[] UVs;

        [ProtoMember(4)]
        public ushort[] Triangles;

        [ProtoMember(5)] 
        public AABBf BoundingBox;
    }
}
