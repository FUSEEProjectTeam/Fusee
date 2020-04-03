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
        G_POSITION,

        /// <summary>
        /// Albedo texture (specular reflection in alpha channel).
        /// </summary>
        G_ALBEDO,

        /// <summary>
        /// Normal texture.
        /// </summary>
        G_NORMAL,

        /// <summary>
        /// Depth texture.
        /// </summary>
        G_DEPTH,

        /// <summary>
        /// SSAO texture (stores occlusion).
        /// </summary>
        G_SSAO,

        /// <summary>
        /// Specular texture.
        /// </summary>
        G_SPECULAR
    }

    /// <summary>
    /// Common texture resolutions for render textures. The value is given in px.
    /// </summary>
    public enum TexRes
    {
        /// <summary>
        /// Create textures in low resolution.
        /// </summary>
        LOW_RES = 1024,
        /// <summary>
        /// Create textures in middle resolution.
        /// </summary>
        MID_RES = 2048,
        /// <summary>
        /// Create textures in high resolution.
        /// </summary>
        HIGH_RES = 4096,
    }
}
