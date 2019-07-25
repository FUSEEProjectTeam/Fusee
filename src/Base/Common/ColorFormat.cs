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
        /// Used for Images containing a single grey-scale value per-pixel. Each pixel consists of one byte.
        /// </summary>
        Intensity,

        iRGBA,

        fRGB
    }
}