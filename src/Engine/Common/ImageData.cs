
namespace Fusee.Engine.Common
{
    /// <summary>
    /// The Enumerator specifying the PixelFormat of an Image.
    /// </summary>
    public enum ImagePixelFormat
    {
        /// <summary>
        /// Used for images containing an alpha-channel.
        /// </summary>
        RGBA,

        /// <summary>
        /// Used for images without an alpha-channel.
        /// </summary>
        RGB
    }

    /// <summary>
    /// Struct containing Image Data for further processing (e.g. texturing)
    /// </summary>
    public struct ImageData
    {
        /// <summary>
        /// The width in pixel units. 
        /// </summary>
        public int Width;
        /// <summary>
        /// The height in pixel units.
        /// </summary>
        public int Height;
        /// <summary>
        /// The PixelFormat of the Image.
        /// </summary>
        public ImagePixelFormat PixelFormat;
        /// <summary>
        /// Number of bytes in one row. 
        /// </summary>
        public int Stride;
        /// <summary>
        /// The pixel data array.
        /// </summary>
        public byte[] PixelData;

        /// <summary>
        /// Gets the pixel color at the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate (in pixels).</param>
        /// <param name="y">The y-coordinate (in pixels).</param>
        /// <returns>The color at the given position.</returns>
        public ColorUint GetPixel(int x, int y)
        {
            if (PixelFormat == ImagePixelFormat.RGB)
            {
                int iPix = y*Stride + 3*x;
                return new ColorUint(PixelData, iPix, true);
            }
            else
            {
                int iPix = y * Stride + 4 * x;
                return new ColorUint(PixelData, iPix, false);
            }
        }
    }
}