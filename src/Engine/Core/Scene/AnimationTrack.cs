using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Base class to hold a single animation key. Derived types specify the type of the value
    /// controlled by the keys.
    /// </summary>
    public class AnimationKeyBase
    {
        /// <summary>
        /// The position of the key on the time-line.
        /// </summary>
        public float Time;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing double values.
    /// </summary>
    public class AnimationKeyDouble : AnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyBase.Time"/>.
        /// </summary>
        public double Value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing integer values.
    /// </summary>
    public class AnimationKeyInt : AnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyBase.Time"/>.
        /// </summary>
        public int Value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing float values.
    /// </summary>
    public class AnimationKeyFloat : AnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyBase.Time"/>.
        /// </summary>
        public float Value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing float2 values.
    /// </summary>
    public class AnimationKeyFloat2 : AnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyBase.Time"/>.
        /// </summary>
        public float2 Value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing float3 values.
    /// </summary>
    public class AnimationKeyFloat3 : AnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyBase.Time"/>.
        /// </summary>
        public float3 Value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Animation key storing float4 values.
    /// </summary>
    public class AnimationKeyFloat4 : AnimationKeyBase
    {
        /// <summary>
        /// The key value effective at the <see cref="AnimationKeyBase.Time"/>.
        /// </summary>
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
    /// Stores data about a single animation track (mainly a list of keyframes).
    /// </summary>
    public class AnimationTrack
    {
        /// <summary>
        /// The scene component to be controlled by this animation track.
        /// </summary>
        public SceneComponent SceneComponent;

        /// <summary>
        /// The name to the property/field to control. May be a dot-separated path to a sub-item (e.g. "Transform.Position.x").
        /// </summary>
        public string Property;

        /// <summary>
        /// The type of the key-values stored in this animation track.
        /// </summary>
        public TypeId TypeId;

        /// <summary>
        /// The lerp type to use for interpolation. 
        /// </summary>
        public LerpType LerpType = LerpType.Lerp;

        /// <summary>
        /// The list of key frames ordered by time.
        /// </summary>
        public List<AnimationKeyBase> KeyFrames = new();
    }
}