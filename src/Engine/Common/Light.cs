using Fusee.Math;

namespace Fusee.Engine
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
        /// Represents the ambient color.
        /// </summary>
        public float4 AmbientColor;
        /// <summary>
        /// Represents the diffuse color.
        /// </summary>
        public float4 DiffuseColor;
        /// <summary>
        /// Represents the specular color.
        /// </summary>
        public float4 SpecularColor;
        /// <summary>
        /// Represents the position of the light.
        /// </summary>
        public float3 Position;
        /// <summary>
        /// Represents the direction of the light.
        /// </summary>
        public float3 Direction;
        /// <summary>
        /// Represents the type of the light.
        /// </summary>
        public LightType Type;
        /// <summary>
        /// Represents the spot angle of the light.
        /// </summary>
        public float Angle;
    }
}
