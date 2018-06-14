using Fusee.Base.Common;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Implements <see cref="IImageData"/> and offers access to internal <seealso cref="PixelData"/> for GPU upload.
    /// </summary>
    public interface ITexture : IImageData
    {
        /// <summary>
        /// The byte buffer that makes up the <see cref="ITexture"/> instance.
        /// </summary>
        byte[] PixelData { get; }
    }
}