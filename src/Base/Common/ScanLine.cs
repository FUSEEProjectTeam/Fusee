using System;

namespace Fusee.Base.Common
{
    // This class is a bit awkward. It tries to solve the following things:
    // - IImageData instances must expose line-wise contiguous portions of internal memory where
    //   requesting objects can copy pixel data from.
    // - C# does not allow to simply wrap a byte[] object around existing memory without first
    //   creating such an object and copy the memory.
    // - We could consider returning a pointer (byte*) but this would involve other headache, including
    //   unsafe code and extra-coding for JSIL.
    public class ScanLine
    {
        public ScanLine(byte[] dataSource, int offset, int width, ImagePixelFormat pixelFormat)
        {
            DataSource = dataSource;
            Offset = offset;
            Width = width;
            PixelFormat = pixelFormat;
        }

        public byte[] DataSource { get; }  // The start of some internal array where the data comes from
        public int Offset { get; }     // An Offset (in bytes) to add to the index to the first pixel of the requested line
        public int Width { get; }      // The
        /// <summary>
        /// Returns the byes per pixel with respect to the <see cref="PixelFormat"/>.
        /// </summary>
        /// <value>
        /// The number of bytes each pixel consists of.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">For unknown pixel formats.</exception>
        public int BytesPerPixel
        {
            get
            {
                switch (PixelFormat.ColorFormat)
                {
                    case ColorFormat.RGBA:
                        return 4;
                    case ColorFormat.RGB:
                        return 3;
                    case ColorFormat.Intensity:
                        return 1;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public ImagePixelFormat PixelFormat { get; }


        public byte[] GetScanLineBytes()
        {
            int bytesPerLine = Width * BytesPerPixel;
            byte[] lineByteBuffer = new byte[bytesPerLine];
            Buffer.BlockCopy(DataSource, Offset, lineByteBuffer, 0, bytesPerLine);

            return lineByteBuffer;
        }
    }
}