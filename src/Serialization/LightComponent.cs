using System.ComponentModel;
using Fusee.Math;
using ProtoBuf;

namespace Fusee.Serialization
{
    public enum LightType
    {
        Point,
        Parallel,
        Spot,
    }

    [ProtoContract]
    public class LightComponent : SceneComponentContainer
    {
        [ProtoMember(1)] 
        public LightType Type;

        [ProtoMember(2)]
        public float3 Color;

        [ProtoMember(3)]
        public float3 Intensity;
    }
}
