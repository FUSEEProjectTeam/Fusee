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
        /// The type of the light source.
        /// </summary>
        [ProtoMember(1)] 
        public LightType Type;

        /// <summary>
        /// The color emitted by the light source.
        /// </summary>
        [ProtoMember(2)]
        public float3 Color;

        /// <summary>
        /// The light's intensity.
        /// </summary>
        [ProtoMember(3)]
        public float3 Intensity;
    }
}
