using System.Collections.Generic;

namespace Fusee.Base.Common
{
    public struct ImagePixelFormat
    {

        public ImagePixelFormat(ColorFormat colorFormat)
        {
            ColorFormat = colorFormat;
        }
        public ColorFormat ColorFormat { get; private set; }

        public int BytesPerPixel
        {
            get
            {
                return ColorFormat == ColorFormat.RGBA
                    ? 4
                    : ColorFormat == ColorFormat.RGB
                        ? 3
                        : ColorFormat == ColorFormat.Intensity
                            ? 1
                            : 0;
            }
        }

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