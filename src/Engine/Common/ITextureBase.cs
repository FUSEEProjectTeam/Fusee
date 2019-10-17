using Fusee.Base.Common;
using Fusee.Serialization;
using System;

namespace Fusee.Engine.Common
{

    /// <summary>
    /// Defines how the texture should be sampled when a uv coordinate outside the range of 0 to 1 is given.
    /// </summary>
    public enum TextureWrapMode
    {
        /// <summary>
        /// The integer part of the uv coordinate will be ignored and a repeating pattern is formed.
        /// </summary>
        REPEAT,
        /// <summary>
        /// The texture will be repeated, but it will be mirrored when the integer part of the uv coordinate is odd.
        /// </summary>
        MIRRORED_REPEAT,
        /// <summary>
        /// The coordinate will simply be clamped between 0 and 1.
        /// </summary>
        CLAMP_TO_EDGE,
        /// <summary>
        /// The coordinates that fall outside the range will be given a specified border color.
        /// </summary>
        CLAMP_TO_BORDER
    }

    /// <summary>
    /// Defines how to map texels to uv coordinates.
    /// </summary>
    public enum TextureFilterMode
    {
        /// <summary>
        /// Default texture filtering method. When set, the pixel which center is closest to the texture coordinate is selected. 
        /// </summary>
        NEAREST,
        /// <summary>
        /// Bilinear filtering.An interpolated value from the texture coordinate's neighboring texels, approximating a color between the texels, is taken.
        /// </summary>
        LINEAR,
        /// <summary>
        /// Takes the nearest mipmap to match the pixel size and uses nearest neighbor interpolation for texture sampling.
        /// </summary>
        NEAREST_MIPMAP_NEAREST,
        /// <summary>
        /// Takes the nearest mipmap level and samples using linear interpolation. 
        /// </summary>
        LINEAR_MIPMAP_NEAREST,
        /// <summary>
        /// Linearly interpolates between the two mipmaps that most closely match the size of a pixel and samples via nearest neighbor interpolation.
        /// </summary>
        NEAREST_MIPMAP_LINEAR,
        /// <summary>
        /// Linearly interpolates between the two closest mipmaps and samples the texture via linear interpolation.
        /// </summary>
        LINEAR_MIPMAP_LINEAR
    }

    /// <summary>
    /// Collection of members all texture types need to implement.
    /// All textures must be IDisposable.
    /// </summary>
    public interface ITextureBase : IImageBase, IDisposable
    {
        /// <summary>
        /// TextureChanged event notifies observing TextureManager about property changes and the Texture's disposal.
        /// </summary>
        event EventHandler<TextureEventArgs> TextureChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Textures's uniqueness in the current session.
        /// </summary>
        Suid SessionUniqueIdentifier { get; }

        /// <summary>
        /// Defines if Mipmaps are generated for this texture.
        /// </summary>
        bool DoGenerateMipMaps { get; }

        /// <summary>
        /// Defines how the texture should be sampled when a uv coordinate outside the range of 0 to 1 is given.
        /// </summary>
        TextureWrapMode WrapMode { get; }

        /// <summary>
        /// Defines how to map texels to uv coordinates.
        /// </summary>
        TextureFilterMode FilterMode { get; }
    }
}
