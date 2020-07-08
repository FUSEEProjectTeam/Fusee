namespace Fusee.Engine.Common
{
    /// <summary>
    /// Enum, that defines all available render texture types. 
    /// </summary>
    public enum RenderTargetTextureTypes
    {
        /// <summary>
        /// Position texture.
        /// </summary>
        Position,

        /// <summary>
        /// Albedo texture (specular reflection in alpha channel).
        /// </summary>
        Albedo,

        /// <summary>
        /// Normal texture.
        /// </summary>
        Normal,

        /// <summary>
        /// Depth texture.
        /// </summary>
        Depth,

        /// <summary>
        /// SSAO texture (stores occlusion).
        /// </summary>
        Ssao,

        /// <summary>
        /// Specular texture.
        /// </summary>
        Specular,

        /// <summary>
        /// Contains the emissive color.
        /// </summary>
        Emission
    }

    /// <summary>
    /// Common texture resolutions for render textures. The value is given in px.
    /// </summary>
    public enum TexRes
    {
        /// <summary>
        /// Create textures in low resolution.
        /// </summary>
        Low = 1024,
        /// <summary>
        /// Create textures in middle resolution.
        /// </summary>
        Middle = 2048,
        /// <summary>
        /// Create textures in high resolution.
        /// </summary>
        High = 4096,
    }
}