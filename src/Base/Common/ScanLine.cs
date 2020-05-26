using System;

namespace Fusee.Base.Common
{
    // This class is a bit awkward. It tries to solve the following things:
    // - IImageData instances must expose line-wise contiguous portions of internal memory where
    //   requesting objects can copy pixel data from.
    // - C# does not allow to simply wrap a byte[] object around existing memory without first
    //   creating such an object and copy the memory. (In C# 7.3 there is a feature called AsSpan, for now we must use ScanLine.)
    // - We could consider returning a pointer (byte*) but this would involve other headache, including
    //   unsafe code and extra-coding for JSIL.
    /// <summary>
    /// Provides view into a portion (= one horizontal line) of a byte[] dataSource of an <see cref="IImageData"/> instance.
    /// </summary>
    public class ScanLine
    {
        /// <summary>
        /// Constructor to initialize one horizontal ScanLine inside a byte[] dataSource, beginning at offset in bytes.
        /// </summary>
        /// <param name="dataSource">The dataSource array, usually the byte array of an IImageData.</param>
        /// <param name="offset">Offset in bytes (= the ScanLine begins at offset bytes inside dataSource).</param>
        /// <param name="width">Width of the ScanLine in pixels.</param>
        /// <param name="pixelFormat">ImagePixelFormat of the dataSource.</param>
        public ScanLine(byte[] dataSource, int offset, int width, ImagePixelFormat pixelFormat)
        {
            DataSource = dataSource;
            Offset = offset;
            Width = width;
            PixelFormat = pixelFormat;
        }

        /// <summary>
        /// The Data source byte array of this ScanLine.
        /// </summary>
        public byte[] DataSource { get; }  // The start of some internal array where the data comes from

        /// <summary>
        /// An Offset (in bytes) to add to the index to the first pixel of the requested line
        /// </summary>
        public int Offset { get; }


        /// <summary>
        /// Width of the ScanLine in pixels.
        /// </summary>
        public int Width { get; }

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
                    case ColorFormat.uiRgb8:
                        return 3;
                    case ColorFormat.Intensity:
                        return 1;
                    case ColorFormat.fRGB32:
                    case ColorFormat.fRGB16:
                        return 12;
                    case ColorFormat.Depth16:
                    case ColorFormat.Depth24:
                        return 2;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        /// <summary>
        /// Describes the PixelFormat of the ScanLine's data source.
        /// </summary>
        public ImagePixelFormat PixelFormat { get; }


        /// <summary>
        /// Copies the bytes that make up this ScanLine from the dataSource.
        /// </summary>
        /// <returns>Returns a byte array that makes up this ScanLine.</returns>
        public byte[] GetScanLineBytes()
        {
            int bytesPerLine = Width * BytesPerPixel;
            byte[] lineByteBuffer = new byte[bytesPerLine];
            Buffer.BlockCopy(DataSource, Offset, lineByteBuffer, 0, bytesPerLine);

            return lineByteBuffer;
        }
    }
}