using System.Collections.Generic;
using Fusee.Math;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class SceneObjectContainer
    {
        [ProtoMember(1)]
        public float4x4 Transform;

        [ProtoMember(2, AsReference = true)]
        public MeshContainer Mesh;

        [ProtoMember(3, AsReference = true)]
        public List<SceneObjectContainer> Children;
    }
}
