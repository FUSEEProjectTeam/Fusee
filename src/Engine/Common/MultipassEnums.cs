using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Enum, that defines all available render texture types
    /// </summary>
    public enum RenderTargetTextures
    {
        /// <summary>
        /// Position texture.
        /// </summary>
        G_POSITION,
        /// <summary>
        /// Albedo texture (specular reflection in alpha channel).
        /// </summary>
        G_ALBEDO_SPECULAR,
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
        /// Stencil texture.
        /// </summary>
        //G_STENCIL
    }

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
        HIGH_RES = 4069,
    }
}
