using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    public class TransformContainer
    {
        [ProtoMember(1)]
        public float3 Translation;
        [ProtoMember(2)]
        public float3 Rotation;
        [ProtoMember(3)]
        public float3 Scale;
    }
}
