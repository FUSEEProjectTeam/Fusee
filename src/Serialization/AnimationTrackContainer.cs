using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
using ProtoBuf;

namespace Fusee.Serialization
{
    [ProtoContract]
    [ProtoInclude(100, typeof(AnimationKeyContainerDouble))]
    [ProtoInclude(101, typeof(AnimationKeyContainerInt))]
    [ProtoInclude(102, typeof(AnimationKeyContainerFloat))]
    [ProtoInclude(103, typeof(AnimationKeyContainerFloat2))]
    [ProtoInclude(104, typeof(AnimationKeyContainerFloat3))]
    [ProtoInclude(105, typeof(AnimationKeyContainerFloat4))]
    public class AnimationKeyContainerBase
    {
        [ProtoMember(1)] 
        public float Time;
    }

    [ProtoContract]
    public class AnimationKeyContainerDouble : AnimationKeyContainerBase
    {
        [ProtoMember(1)]
        public Double Value;
    }

    [ProtoContract]
    public class AnimationKeyContainerInt: AnimationKeyContainerBase
    {
        [ProtoMember(1)]
        public int Value;
    }

    [ProtoContract]
    public class AnimationKeyContainerFloat : AnimationKeyContainerBase
    {
        [ProtoMember(1)]
        public float Value;
    }

    [ProtoContract]
    public class AnimationKeyContainerFloat2 : AnimationKeyContainerBase
    {
        [ProtoMember(1)]
        public float2 Value;
    }

    [ProtoContract]
    public class AnimationKeyContainerFloat3 : AnimationKeyContainerBase
    {
        [ProtoMember(1)] 
        public float3 Value;
    }

    [ProtoContract]
    public class AnimationKeyContainerFloat4 : AnimationKeyContainerBase
    {
        [ProtoMember(1)]
        public float4 Value;
    }


    [ProtoContract]
    public class AnimationTrackContainer
    {

        [ProtoMember(1, AsReference = true)]
        public SceneNodeContainer SceneObject;

        [ProtoMember(2)] 
        public string Property;

        [ProtoMemberAttribute(3, AsReference = true)] 
        public Type KeyType;

        [ProtoMemberAttribute(4, AsReference = true)]
        public List<AnimationKeyContainerBase> KeyFrames;
    }
}
