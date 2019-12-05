
namespace Fusee.Base.Common
{
    /// <summary>
    /// Collection of members, all types of images, e. g. ImageData and Textures, need to implement.
    /// </summary>
    public interface IImageBase
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

    }
}
