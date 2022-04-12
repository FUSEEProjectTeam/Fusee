namespace Fusee.Base.Common
{
    /// <summary>
    /// ColorFormat information in enum representation. Entries are hints for Color channel encoding.
    /// </summary>
    public enum ColorFormat
    {
        /// <summary>
        /// Used for images containing an alpha-channel. Each pixel consists of four bytes.
        /// </summary>
        RGBA,

        /// <summary>
        /// Used for images without an alpha-channel. Each pixel consists of three bytes.
        /// </summary>
        RGB,

        /// <summary>
        /// Used for Images containing a single grayscale value per-pixel. Each pixel consists of one byte.
        /// </summary>
        Intensity,

        /// <summary>
        /// RGB integer.
        /// </summary>
        uiRgb8,

        /// <summary>
        /// RGB float, 32bit.
        /// </summary>
        fRGB32,

        /// <summary>
        /// RGB float, 16bit.
        /// </summary>
        fRGB16,

        /// <summary>
        /// RGBA float, 16bit.
        /// </summary>
        fRGBA16,

        /// <summary>
        /// RGBA float, 32bit.
        /// </summary>
        fRGBA32,

        /// <summary>
        /// Used for creating depth maps.
        /// </summary>
        Depth16,

        /// <summary>
        /// Used for creating depth maps.
        /// </summary>
        Depth24,

        /// <summary>
        /// RGBA int, 32bit.
        /// </summary>
        iRGBA32,
    }
}