using System.Collections.Generic;
using Fusee.Math;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class SceneObjectContainer
    {
        [ProtoMember(1)]
        public string Name;

        [ProtoMember(2)]
        public float4x4 Transform;

        [ProtoMember(5)] 
        public float3 Color;

        [ProtoMember(3, AsReference = true)]
        public MeshContainer Mesh;

        [ProtoMember(4, AsReference = true)]
        public List<SceneObjectContainer> Children;
    }
}
