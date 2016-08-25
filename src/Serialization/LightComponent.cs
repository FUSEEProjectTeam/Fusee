using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Specifies the type of the light.
    /// </summary>
    public enum LightType
    {
        /// <summary>
        /// Point light. Emits rays from a single point radially into all directions
        /// </summary>
        Point,
        /// <summary>
        /// Parallel light. Emits parallel rays into a specified direction.
        /// </summary>
        Parallel,
        /// <summary>
        /// Spot light. Like a point light but with rules describing the intensities of the
        /// rays depending on their direction.
        /// </summary>
        Spot,
    }

    /// <summary>
    /// Contains light information. If contained in a node, the node serves as a light object.
    /// </summary>
    [ProtoContract]
    public class LightComponent : SceneComponentContainer
    {
        /// <summary>
        /// Represents the light status.
        /// </summary>
        [ProtoMember(1)]
        public float Active;
        /// <summary>
        /// Represents the color.
        /// </summary>
        [ProtoMember(2)]
        public float3 Color;
        /// <summary>
        /// Represents the position of the light.
        /// </summary>
        [ProtoMember(3)]
        public float4 Position;
        /// <summary>
        /// Represents the attenuation of the light.
        /// </summary>
        [ProtoMember(4)]
        public float Attenuation;
        /// <summary>
        /// Represents the ambient coefficient of the light.
        /// </summary>
        [ProtoMember(5)]
        public float AmbientCoefficient;
        /// <summary>
        /// Represents the type of the light.
        /// </summary>
        [ProtoMember(6)]
        public LightType Type;
        /// <summary>
        /// Represents the spot angle of the light.
        /// </summary>
        [ProtoMember(6)]
        public float ConeAngle;
        /// <summary>
        /// Represents the cone direction of the light.
        /// </summary>
        [ProtoMember(7)]
        public float3 ConeDirection;
    }
}
