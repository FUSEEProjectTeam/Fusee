using Fusee.Base.Common;
using Fusee.Math.Core;
using ProtoBuf;
using System;

namespace Fusee.Serialization
{
    /// <summary>
    /// Contains light information. If contained in a node, the node serves as a light object.
    /// If possible, avoid adding or removing these at runtime, instead create all you need and set active or inactive.
    /// The Position and Direction of a Light gets calculated internally, depending on the parent transform components, found in the scene graph.
    /// </summary>
    [ProtoContract]
    public class LightComponent: SceneComponentContainer
    {
        /// <summary>
        /// Represents the light status.
        /// </summary>
        [ProtoMember(1)]
        public bool Active;

        /// <summary>
        /// Represents the color.
        /// </summary>
        [ProtoMember(2)]
        public float4 Color;

        /// <summary>
        /// Represents the attenuation of the light.
        /// </summary>
        [ProtoMember(3)]
        public float MaxDistance;

        /// <summary>
        /// Represents the strength of the light (non-physically representation of the brightness).
        /// Should be a value between 0 and 1.
        /// </summary>
        [ProtoMember(4)]
        public float Strength;

        /// <summary>
        /// Represents the type of the light.
        /// </summary>
        [ProtoMember(5)]
        public LightType Type;

        /// <summary>
        /// Represents the outer spot angle of the light.
        /// </summary>
        [ProtoMember(6)]
        public float OuterConeAngle;


        /// <summary>
        /// Represents the spot inner angle of the light.
        /// </summary>
        [ProtoMember(7)]
        public float InnerConeAngle;

        /// <summary>
        /// Creates a new instance of type LightComponent.
        /// </summary>
        /// <param name="strength">Represents the strength of the light (non-physically representation of the brightness).</param>
        public LightComponent(float strength = 1)
        {
            Strength = strength;
        }

        /// <summary>
        /// Defines if a shadow map is created for this light.
        /// </summary>
        public bool IsCastingShadows;

        /// <summary>
        /// Bias for calculating shadows.
        /// </summary>
        public float Bias;
    }
}
