using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// This container holds all informations which are required for shader light calculation.
    /// </summary>
    public struct Light
    {
        /// <summary>
        /// Represents the light status.
        /// </summary>
        public float Active;
        /// <summary>
        /// Represents the color.
        /// </summary>
        public float3 Color;
        /// <summary>
        /// Represents the position of the light.
        /// </summary>
        public float4 Position;
        /// <summary>
        /// Represents the attenuation of the light.
        /// </summary>
        public float Attenuation;
        /// <summary>
        /// Represents the ambient coefficient of the light.
        /// </summary>
        public float AmbientCoefficient;
        /// <summary>
        /// Represents the type of the light.
        /// </summary>
        public LightType Type;
        /// <summary>
        /// Represents the spot angle of the light.
        /// </summary>
        public float ConeAngle;
        /// <summary>
        /// Represents the cone direction of the light.
        /// </summary>
        public float3 ConeDirection;
    }
}
