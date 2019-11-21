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
    /// Depth textures can be sampled in one of two ways. 
    /// They can be sampled as a normal texture, which simply retrieves the depth value, this will return a vec4 containing a single floating-point value,
    /// or they can be fetched in comparison mode. The result of the comparison depends on the comparison function set in the texture. If the function succeeds, the resulting value is 1.0f; if it fails, it is 0.0f.
    /// see: https://www.khronos.org/opengl/wiki/Sampler_Object
    /// </summary>
    public enum TextureCompareMode
    {
        /// <summary>
        /// Disables the compare mode.
        /// </summary>
        NONE,
        /// <summary>
        /// Enables the compare mode.
        /// </summary>
        GL_COMPARE_REF_TO_TEXTURE 
    }

    /// <summary>
    /// Comparison functions (operators) for use with <see cref="TextureCompareMode"/>.
    /// The comparison works as follows: ref OPERATOR texture. Where ref is the reference value given to the texture lookup function by GLSL, and texture is the value fetched from the texture.
    /// see: https://www.khronos.org/opengl/wiki/Sampler_Object
    /// </summary>
    public enum TextureCompareFunc
    {
        /// <summary>
        /// Comparison always fails. 
        /// </summary>
        GL_NEVER,
        /// <summary>
        /// Comparison always succeeds. 
        /// </summary>
        GL_ALWAYS,
        /// <summary>
        /// Comparison will be true if the reference value is strictly less than the value pulled from the texture.
        /// </summary>
        GL_LESS,
        /// <summary>
        /// Comparison will be true if the reference value is less or equal than the value pulled from the texture.
        /// </summary>
        GL_LEQUAL,
        /// <summary>
        /// Comparison will be true if the reference value is strictly equal than the value pulled from the texture.
        /// </summary>
        GL_EQUAL,
        /// <summary>
        /// Comparison will be true if the reference value is not equal than the value pulled from the texture.
        /// </summary>
        GL_NOT_EQUAL,
        /// <summary>
        /// Comparison will be true if the reference value is greater or equal than the value pulled from the texture.
        /// </summary>
        GL_GEQUAL,
        /// <summary>
        /// Comparison will be true if the reference value is strictly greater than the value pulled from the texture.
        /// </summary>
        GL_GREATER
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
