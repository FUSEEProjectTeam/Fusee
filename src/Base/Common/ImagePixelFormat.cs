using System.Collections.Generic;

namespace Fusee.Base.Common
{
    /// <summary>
    /// Offers additional Information for ColorFormat.
    /// </summary>
    public struct ImagePixelFormat
    {
        /// <summary>
        /// Constructor requires <see cref="ColorFormat"/> to extract additional information.
        /// </summary>
        /// <param name="colorFormat">The input <see cref="ColorFormat"/>.</param>
        public ImagePixelFormat(ColorFormat colorFormat)
        {
            ColorFormat = colorFormat;
        }

        /// <summary>
        /// Pixel encoding enum.
        /// </summary>
        public ColorFormat ColorFormat { get; private set; }

        /// <summary>
        /// Returns how many bytes make up one pixel. Example RGB = 3 Bytes, each channel consists of 1 byte.
        /// </summary>
        public int BytesPerPixel
        {
            get
            {
                return ColorFormat == ColorFormat.RGBA || ColorFormat == ColorFormat.iRGBA
                    ? 4
                    : ColorFormat == ColorFormat.RGB 
                        ? 3
                        : ColorFormat == ColorFormat.Depth
                            ? 2
                        : ColorFormat == ColorFormat.Intensity
                            ? 1
                                : ColorFormat == ColorFormat.fRGB
                                    ? 12
                                        : 0;
            }
        }

        /// <summary>
        /// Returns a <see cref="PixelChannel"/> for each channel of <see cref="ColorFormat"/> providing additional information about channel offset and bits per channel.
        /// </summary>
        public IEnumerator<ImagePixelChannel> PixelChannel
        {
            get
            {
                // Currently supports only formats where each channel is 8 Bits = 1 Byte big
                int bytesPerPixel = BytesPerPixel;
                int bitsPerByte = sizeof(byte) * 8;
                for (int i = 0; i < bytesPerPixel; i++) // begin at 0 -> firstBit=0
                {
                    yield return new ImagePixelChannel(bitsPerByte * i, bitsPerByte, PixelEncoding.Uint); // (dd) currently only uint is relevant, might change
                }
            }
        }
    }
}