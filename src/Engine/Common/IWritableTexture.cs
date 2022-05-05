using Fusee.Base.Common;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Cross platform abstraction for a WritableTexture.
    /// Use writable textures if you want to render into a texture.
    /// Does NOT offer access to the pixel data.
    /// </summary>
    public interface IWritableTexture : ITextureBase
    {
        /// <summary>
        /// Width in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Offers additional Information about the color format of the texture.
        ///</summary>
        ImagePixelFormat PixelFormat { get; }

        /// <summary>
        /// Type of the render texture, <see cref="RenderTargetTextureTypes"/>.
        /// </summary>
        RenderTargetTextureTypes TextureType { get; }

        /// <summary>
        /// Specifies the texture's comparison mode, see <see cref="TextureCompareMode"/>.
        /// </summary>
        TextureCompareMode CompareMode { get; }


        /// <summary>
        /// Specifies the texture's comparison function.
        /// </summary>
        Compare CompareFunc { get; }
    }
}