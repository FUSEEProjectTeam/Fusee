using System;
using System.Collections.Generic;
using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Base class to hold a single animation key. Derived types specify the type of the value
    /// controlled by the keys.
    /// </summary>
    [ProtoContract]
    [ProtoInclude(100, typeof(AnimationKeyContainerDouble))]
    [ProtoInclude(101, typeof(AnimationKeyContainerInt))]
    [ProtoInclude(102, typeof(AnimationKeyContainerFloat))]
    [ProtoInclude(103, typeof(AnimationKeyContainerFloat2))]
    [ProtoInclude(104, typeof(AnimationKeyContainerFloat3))]
    [ProtoInclude(105, typeof(AnimationKeyContainerFloat4))]
    public class AnimationKeyContainerBase
    {
        /// <summary>
        /// The position of the key on the timeline.
        /// </summary>
        [ProtoMember(1)] 
        public float Time;
    }

    /// <summary>
    /// Animation key storing double values.
    /// </summary>
    [ProtoContract]
    public class AnimationKeyContainerDouble : AnimationKeyContainerBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyContainerBase.Time"/>.
        /// </summary>
        [ProtoMember(1)]
        public double Value;
    }

    /// <summary>
    /// Animation key storing integer values.
    /// </summary>
    [ProtoContract]
    public class AnimationKeyContainerInt: AnimationKeyContainerBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyContainerBase.Time"/>.
        /// </summary>
        [ProtoMember(1)]
        public int Value;
    }

    /// <summary>
    /// Animation key storing float values.
    /// </summary>
    [ProtoContract]
    public class AnimationKeyContainerFloat : AnimationKeyContainerBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyContainerBase.Time"/>.
        /// </summary>
        [ProtoMember(1)]
        public float Value;
    }

    /// <summary>
    /// Animation key storing float2 values.
    /// </summary>
    [ProtoContract]
    public class AnimationKeyContainerFloat2 : AnimationKeyContainerBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyContainerBase.Time"/>.
        /// </summary>
        [ProtoMember(1)]
        public float2 Value;
    }

    /// <summary>
    /// Animation key storing float3 values.
    /// </summary>
    [ProtoContract]
    public class AnimationKeyContainerFloat3 : AnimationKeyContainerBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyContainerBase.Time"/>.
        /// </summary>
        [ProtoMember(1)] 
        public float3 Value;
    }

    /// <summary>
    /// Animation key storing float4 values.
    /// </summary>
    [ProtoContract]
    public class AnimationKeyContainerFloat4 : AnimationKeyContainerBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyContainerBase.Time"/>.
        /// </summary>
        [ProtoMember(1)]
        public float4 Value;
    }

    /// <summary>
    /// Symbolic value describing the type of Lerp (linear interpolation) to perform.
    /// </summary>
    public enum LerpType
    {
        /// <summary>
        /// Standard interpolation. Multi-Value-Types are interpolated individually for each value (e.g., x, y, z).
        /// </summary>
        Lerp,
        /// <summary>
        /// Spherical interpolation. float3 are interpreted as Euler angles and interpolated in a shortest-path way using <see cref="Quaternion"/>s.
        /// </summary>
        Slerp,  
    }


    /// <summary>
    /// Stores data about a single animation track (mainly a list of keyframes)
    /// </summary>
    [ProtoContract]
    public class AnimationTrackContainer
    {
        /// <summary>
        /// The scene component to be controlled by this animation track.
        /// </summary>
        [ProtoMember(1, AsReference = true)]
        public SceneComponentContainer SceneComponent;

        /// <summary>
        /// The name to the property/field to control. May be a dot-separated path to a sub-item (e.g. "Transform.Position.x").
        /// </summary>
        [ProtoMember(2)] 
        public string Property;

        /// <summary>
        /// The type of the key-values stored in this animation track.
        /// </summary>
        [ProtoMember(3, AsReference = true)] 
        public Type KeyType;

        /// <summary>
        /// The lerp type to use for interpolation. 
        /// </summary>
        [ProtoMember(5)]
        public LerpType LerpType = LerpType.Lerp;

        /// <summary>
        /// The list of key frames ordered by time.
        /// </summary>
        [ProtoMember(4, AsReference = true)]
        public List<AnimationKeyContainerBase> KeyFrames;
    }
}
