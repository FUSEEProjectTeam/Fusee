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
    [ProtoInclude(101, typeof(AnimationKeyContainerFloat3))]
    public class AnimationKeyContainerBase
    {
        [ProtoMember(1)] 
        public int Time;
    }

    [ProtoContract]
    public class AnimationKeyContainerDouble : AnimationKeyContainerBase
    {
        [ProtoMember(1)]
        public Double Value;
    }

    [ProtoContract]
    public class AnimationKeyContainerFloat3 : AnimationKeyContainerBase
    {
        [ProtoMember(1)] 
        public float3 Value;
    }
    //...


    [ProtoContract]
    public class AnimationTrackContainer
    {

        [ProtoMember(1, AsReference = true)]
        public SceneObjectContainer SceneObject;

        [ProtoMember(2)] 
        public string Property;
        // e.g. Transform Translation

        [ProtoMemberAttribute(3, AsReference = true)] 
        public Type ValueType;

        [ProtoMemberAttribute(4, AsReference = true)]
        public List<AnimationKeyContainerBase> KeyFrames;
    }
}
