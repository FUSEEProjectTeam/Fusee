using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Specifies the type of the light.
    /// A light is specified by one of three types represented by integers between 0 and 2.
    /// </summary>
    public enum LightType
    {
        /// <summary>
        /// Directional light is represented by 0.
        /// </summary>
        Directional,
        /// <summary>
        /// Point light is represented by 1.
        /// </summary>
        Point,
        /// <summary>
        /// Spot light is represented by 2.
        /// </summary>
        Spot
    }

    /// <summary>
    /// The container that holds all informations that are required for shader light calculation.
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
