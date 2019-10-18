using Fusee.Base.Common;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Cross platform abstraction for Textures.
    /// Implements <see cref="IImageData"/> and <see cref="ITextureBase"/>and offers access to internal <seealso cref="PixelData"/> for GPU upload.
    /// </summary>
    public interface ITexture : IImageData, ITextureBase
    {
        /// <summary>
        /// The byte buffer that makes up the <see cref="ITexture"/> instance.
        /// </summary>
        byte[] PixelData { get; }
        
    }
}