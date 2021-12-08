using Fusee.Math.Core;
using ProtoBuf;
using System.Collections.Generic;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Base class to hold a single animation key. Derived types specify the type of the value
    /// controlled by the keys.
    /// </summary>
    [ProtoContract]
    [ProtoInclude(100, typeof(FusAnimationKeyDouble))]
    [ProtoInclude(101, typeof(FusAnimationKeyInt))]
    [ProtoInclude(102, typeof(FusAnimationKeyFloat))]
    [ProtoInclude(103, typeof(FusAnimationKeyFloat2))]
    [ProtoInclude(104, typeof(FusAnimationKeyFloat3))]
    [ProtoInclude(105, typeof(FusAnimationKeyFloat4))]
    public class FusAnimationKeyBase
    {
        /// <summary>
        /// The position of the key on the timeline.
        /// </summary>
        [ProtoMember(1)]
        public float Time;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing double values.
    /// </summary>
    [ProtoContract]
    public class FusAnimationKeyDouble : FusAnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="FusAnimationKeyBase.Time"/>.
        /// </summary>
        [ProtoMember(1)]
        public double Value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing integer values.
    /// </summary>
    [ProtoContract]
    public class FusAnimationKeyInt : FusAnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="FusAnimationKeyBase.Time"/>.
        /// </summary>
        [ProtoMember(1)]
        public int Value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing float values.
    /// </summary>
    [ProtoContract]
    public class FusAnimationKeyFloat : FusAnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="FusAnimationKeyBase.Time"/>.
        /// </summary>
        [ProtoMember(1)]
        public float Value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing float2 values.
    /// </summary>
    [ProtoContract]
    public class FusAnimationKeyFloat2 : FusAnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="FusAnimationKeyBase.Time"/>.
        /// </summary>
        [ProtoMember(1)]
        public float2 Value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing float3 values.
    /// </summary>
    [ProtoContract]
    public class FusAnimationKeyFloat3 : FusAnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="FusAnimationKeyBase.Time"/>.
        /// </summary>
        [ProtoMember(1)]
        public float3 Value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing float4 values.
    /// </summary>
    [ProtoContract]
    public class FusAnimationKeyFloat4 : FusAnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="FusAnimationKeyBase.Time"/>.
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
    /// Symbolic value describing the data type of stored values. 
    /// </summary>
    public enum TypeId
    {
        /// <summary>Contains double values. </summary>
        Double,
        /// <summary>Contains int values. </summary>
        Int,
        /// <summary>Contains float values. </summary>
        Float,
        /// <summary>Contains float2 values. </summary>
        Float2,
        /// <summary>Contains float3 values. </summary>
        Float3,
        /// <summary>Contains float4 values. </summary>
        Float4,
        /// <summary>Contains boolean values. </summary>
        Bool,
    }

    /// <summary>
    /// Stores data about a single animation track (mainly a list of keyframes)
    /// </summary>
    [ProtoContract]
    public class FusAnimationTrack
    {
        /// <summary>
        /// The index to the scene component to be controlled by this animation track.
        /// </summary>
        [ProtoMember(1)]
        public int SceneComponent;

        /// <summary>
        /// The name to the property/field to control. May be a dot-separated path to a sub-item (e.g. "Transform.Position.x").
        /// </summary>
        [ProtoMember(2)]
        public string Property;

        /// <summary>
        /// The type of the key-values stored in this animation track.
        /// </summary>
        [ProtoMember(3)]
        public TypeId TypeId;

        /// <summary>
        /// The lerp type to use for interpolation. 
        /// </summary>
        [ProtoMember(5)]
        public LerpType LerpType = LerpType.Lerp;

        /// <summary>
        /// The list of key frames ordered by time.
        /// </summary>
        [ProtoMember(4)]
        public List<FusAnimationKeyBase> KeyFrames = new();
    }
}