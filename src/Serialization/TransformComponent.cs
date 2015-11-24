using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class TransformComponent : SceneComponentContainer
    {
        [ProtoMember(1)]
        public float3 Translation;
        [ProtoMember(2)]
        public float3 Rotation;
        [ProtoMember(3)]
        public float3 Scale;
    }
}
