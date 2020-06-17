using Fusee.Base.Common;
using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Abstraction for the different texture target used in the graphics API.
    /// </summary>
    public enum TextureType
    {
        /// <summary>
        /// 1D Texture, width only.
        /// </summary>
        Texture1D,

        /// <summary>
        /// 2D Texture, width and height.
        /// </summary>
        Texture2D,

        /// <summary>
        /// 3D Texture, width, height, depth.
        /// </summary>
        Texture3D,

        /// <summary>
        /// Exactly 6 distinct sets of 2D images, all of the same size. They act as 6 faces of a cube.
        /// </summary>
        TextureCubeMap
    }

    /// <summary>
    /// Defines how the texture should be sampled when a uv coordinate outside the range of 0 to 1 is given.
    /// </summary>
    public enum TextureWrapMode
    {
        /// <summary>
        /// The integer part of the uv coordinate will be ignored and a repeating pattern is formed.
        /// </summary>
        Repeat,
        /// <summary>
        /// The texture will be repeated, but it will be mirrored when the integer part of the uv coordinate is odd.
        /// </summary>
        MirroredRepeat,
        /// <summary>
        /// The coordinate will simply be clamped between 0 and 1.
        /// </summary>
        ClampToEdge,
        /// <summary>
        /// The coordinates that fall outside the range will be given a specified border color.
        /// </summary>
        ClampToBorder
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
        None,
        /// <summary>
        /// Enables the compare mode.
        /// </summary>
        CompareRefToTexture
    }

    /// <summary>
    /// Defines how to map texels to uv coordinates.
    /// </summary>
    public enum TextureFilterMode
    {
        /// <summary>
        /// Default texture filtering method. When set, the pixel which center is closest to the texture coordinate is selected. 
        /// </summary>
        Nearest,
        /// <summary>
        /// Bilinear filtering.An interpolated value from the texture coordinate's neighboring texels, approximating a color between the texels, is taken.
        /// </summary>
        Linear,
        /// <summary>
        /// Takes the nearest mipmap to match the pixel size and uses nearest neighbor interpolation for texture sampling.
        /// </summary>
        NearestMipmapNearest,
        /// <summary>
        /// Takes the nearest mipmap level and samples using linear interpolation. 
        /// </summary>
        LinearMipmapNearest,
        /// <summary>
        /// Linearly interpolates between the two mipmaps that most closely match the size of a pixel and samples via nearest neighbor interpolation.
        /// </summary>
        NearestMipmapLinear,
        /// <summary>
        /// Linearly interpolates between the two closest mipmaps and samples the texture via linear interpolation.
        /// </summary>
        LinearMipmapLinear
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