
using System;
using System.ServiceModel;
using Fusee.Math.Core;

namespace Fusee.Base.Common
{
    /// <summary>
    /// The Enumerator specifying the PixelFormat of an Image.
    /// </summary>
    public enum ImagePixelFormat
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

        /// <summary>
        /// Used for Images containing a depth value per-pixel. Each pixel consists of one byte.
        /// </summary>
        Depth,

        /// <summary>
        /// Used for a CubeMap Image with 6 faces, containing a depth value per-pixel. Each pixel consists of one byte.
        /// </summary>
        DepthCubeMap


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
                switch (PixelFormat)
                {
                    case ImagePixelFormat.RGBA:
                        return 4;
                    case ImagePixelFormat.RGB:
                        return 3;
                    case ImagePixelFormat.Intensity:
                        return 1;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has alpha with respect to the <see cref="PixelFormat"/>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has alpha; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">For unknown pixel formats.</exception>
        public bool HasAlpha
        {
            get
            {
                switch (PixelFormat)
                {
                    case ImagePixelFormat.RGBA:
                        return true;
                    case ImagePixelFormat.RGB:
                        return false;
                    case ImagePixelFormat.Intensity:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Gets the pixel color at the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate (in pixels).</param>
        /// <param name="y">The y-coordinate (in pixels).</param>
        /// <returns>The color at the given position.</returns>
        public ColorUint GetPixel(int x, int y)
        {
            if (!(0 <= x && x < Width)) throw new ArgumentOutOfRangeException(nameof(x), $"{x} is not in the range [0..{Width-1}]");
            if (!(0 <= y && y < Height)) throw new ArgumentOutOfRangeException(nameof(y), $"{y} is not in the range [0..{Height-1}]");

            int bpp = BytesPerPixel;
            bool alpha = bpp > 3;
            switch (PixelFormat)
            {
                case ImagePixelFormat.RGBA:
                case ImagePixelFormat.RGB:
                    int iPix = y*Stride + BytesPerPixel*x;
                    return new ColorUint(PixelData, iPix, HasAlpha);
                case ImagePixelFormat.Intensity:
                    byte b = PixelData[y * Stride + BytesPerPixel * x];
                    return new ColorUint(b, b, b, 0xFF);
                default:
                    throw new InvalidOperationException("Unkown pixel format.");
            }
        }

        /// <summary>
        /// Checks if an image contains no data by checking if <see cref="Width"/> or <see cref="Height"/> is 0.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => (Width <= 0 || Height <= 0);


        /// <summary>
        /// Performs clipping along one dimension of a blt operation.
        /// </summary>
        /// <param name="iDst">The destination coordinate.</param>
        /// <param name="sizeDst">The size of the destination buffer.</param>
        /// <param name="iSrc">The source coordinate.</param>
        /// <param name="sizeSrc">The size of the source coordinate.</param>
        /// <param name="sizeBlk">The size of the block to copy.</param>
        /// <remarks>
        ///    All parameters decorated with "ref" might be altered to avoid out-of-bounds indices.
        ///    If the resulting number of items to copy is 0, only sizeBlk will be set to 0. No other
        ///    ref-parameter will be altered then.
        /// </remarks>
        public static void ClipBlt(ref int iDst, int sizeDst, ref int iSrc, int sizeSrc, ref int sizeBlk)
        {
            // Adjust left border
            // The negative number with the biggest magnitude of negative start indices (or 0, if both are 0 or bigger).
            // int iDeltaL = M.Min(0, M.Min(iDst, iSrc));
            int iDeltaL = (iDst < iSrc) ? iDst : iSrc;
            if (iDeltaL > 0)
                iDeltaL = 0;

            // Adjust right border
            // The biggest overlap over the right border (or 0 if no overlap).
            // int iDeltaR = M.Max(0, M.Max(iDst + sizeBlk - sizeDst, iSrc + sizeBlk - sizeSrc));
            int dstRB = iDst + sizeBlk - sizeDst;
            int srcRB = iSrc + sizeBlk - sizeSrc;
            int iDeltaR = (dstRB > srcRB) ? dstRB : srcRB;
            if (iDeltaR < 0)
                iDeltaR = 0;

            iDst -= iDeltaL;
            iSrc -= iDeltaL;
            sizeBlk += iDeltaL;
            sizeBlk -= iDeltaR;
            if (sizeBlk < 0)
                sizeBlk = 0;
        }


        private delegate void CopyFunc(int iDst, int iSrc);

        /// <summary>
        /// Copys a rectangular block of pixel data from a source image to a destiation image (Blt = BlockTransfer).
        /// </summary>
        /// <param name="dst">The destination image.</param>
        /// <param name="xDst">The x destination coordinate (where to place the block within dst).</param>
        /// <param name="yDst">The y destination coordinate (where to place the block within dst).</param>
        /// <param name="src">The source image.</param>
        /// <param name="xSrc">The x source coordinate (where to start copying from within src).</param>
        /// <param name="ySrc">The y source coordinate (where to start copying from within src).</param>
        /// <param name="width">The width of the block to copy. (default is src.Width).</param>
        /// <param name="height">The height of the block to copy (default is src.Height).</param>
        /// <remarks>
        ///     All specified parameters are clipped to avoid out-of-bounds indices. No warnings or exceptions are issued
        ///     in case clipping results in a smaller or an empty block.
        /// </remarks>
        public static void Blt(ImageData dst, int xDst, int yDst, ImageData src, int xSrc=0, int ySrc=0, int width=0, int height=0)
        {
            if (width == 0)
                width = src.Width;
            if (height == 0)
                height = src.Height;

            ClipBlt(ref xDst, dst.Width, ref xSrc, src.Width, ref width);
            ClipBlt(ref yDst, dst.Height, ref ySrc, src.Height, ref height);

            if (width <= 0 || height <= 0)
                return;

            CopyFunc CopyLine = null;
            if (dst.PixelFormat == src.PixelFormat)
            {
                // We can copy an entire line en-bloc
                CopyLine = delegate(int idl, int isl)
                {
                    Array.Copy(src.PixelData, isl + xSrc * src.BytesPerPixel, 
                               dst.PixelData, idl + xDst * dst.BytesPerPixel,
                               width*dst.BytesPerPixel);
                };
            }
            else
            {
                // Wee need to perform pixel-conversion while copying.
                CopyFunc CopyPixel = null;

                switch (dst.PixelFormat)
                {
                    case ImagePixelFormat.RGBA:
                        switch (src.PixelFormat)
                        {
                            case ImagePixelFormat.RGB:
                                CopyPixel = delegate(int idp, int isp)
                                {
                                    dst.PixelData[idp + 0] = src.PixelData[isp + 0];
                                    dst.PixelData[idp + 1] = src.PixelData[isp + 1];
                                    dst.PixelData[idp + 2] = src.PixelData[isp + 2];
                                    dst.PixelData[idp + 3] = byte.MaxValue;
                                };
                                break;
                            case ImagePixelFormat.Intensity:
                                CopyPixel = delegate (int idp, int isp)
                                {
                                    dst.PixelData[idp + 0] = src.PixelData[isp];
                                    dst.PixelData[idp + 1] = src.PixelData[isp];
                                    dst.PixelData[idp + 2] = src.PixelData[isp];
                                    dst.PixelData[idp + 3] = byte.MaxValue;
                                };
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(src), "Unknown source pixel format to copy to RGBA");
                        }
                        break;
                    case ImagePixelFormat.RGB:
                        switch (src.PixelFormat)
                        {
                            case ImagePixelFormat.RGBA:
                                CopyPixel = delegate (int idp, int isp)
                                {
                                    dst.PixelData[idp + 0] = src.PixelData[isp + 0];
                                    dst.PixelData[idp + 1] = src.PixelData[isp + 1];
                                    dst.PixelData[idp + 2] = src.PixelData[isp + 2];
                                    // skip source alpha src.PixelData[isp + 3];
                                };
                                break;
                            case ImagePixelFormat.Intensity:
                                CopyPixel = delegate (int idp, int isp)
                                {
                                    dst.PixelData[idp + 0] = src.PixelData[isp];
                                    dst.PixelData[idp + 1] = src.PixelData[isp];
                                    dst.PixelData[idp + 2] = src.PixelData[isp];
                                };
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(src), "Unknown source pixel format to copy to RGB");
                        }
                        break;
                    case ImagePixelFormat.Intensity:
                        switch (src.PixelFormat)
                        {
                            case ImagePixelFormat.RGB:
                            case ImagePixelFormat.RGBA:
                                CopyPixel = delegate (int idp, int isp)
                                {
                                    // Quick integer Luma conversion (not accurate)
                                    // See http://stackoverflow.com/questions/596216/formula-to-determine-brightness-of-rgb-color
                                    int R = src.PixelData[isp + 0];
                                    int G = src.PixelData[isp + 1];
                                    int B = src.PixelData[isp + 2];
                                    dst.PixelData[idp] = (byte)((R + R + B + G + G + G) / 6);
                                };
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(src), "Unknown source pixel format to copy to RGB");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dst), "Unknown destination pixel format");
                }
                CopyLine = delegate(int idl, int isl)
                {
                    int iDstPxl = idl + xDst*dst.BytesPerPixel;
                    int iSrcPxl = isl + xSrc*src.BytesPerPixel;
                    for (int x = 0; x < width; x++)
                    {
                        CopyPixel(iDstPxl, iSrcPxl);
                        iDstPxl += dst.BytesPerPixel;
                        iSrcPxl += src.BytesPerPixel;
                    }
                };
            }

            int iDstLine = yDst*dst.Stride;
            int iSrcLine = ySrc*src.Stride;
            for (int y = 0; y < height; y++)
            {
                CopyLine(iDstLine, iSrcLine);
                iDstLine += dst.Stride;
                iSrcLine += src.Stride;
            }
        }

        /// <summary>
        /// Sets each byte in an array to the given value.
        /// </summary>
        /// <param name="array">The array to fill.</param>
        /// <param name="value">The contents.</param>
        /// <exception cref="System.ArgumentNullException">The array must not be null.</exception>
        /// <remarks>
        ///    Implementation note: There is no "canonical" way in .NET to initialize an array like "memset" in C.
        ///    This implementation is a tradeof between performance and NOT relying on things like "unsafe" or "IL emit".
        ///    See the discussion on <a href="http://stackoverflow.com/questions/1897555/what-is-the-equivalent-of-memset-in-c">
        ///    StackOverflow (1897555)</a>
        ///    where this implementation is taken from but where you can also find faster but less portable implementations.
        /// </remarks>
        private static void MemSet(byte[] array, byte value)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            int block = 32, index = 0;
            int length = M.Min(block, array.Length);

            //Fill the initial array
            while (index < length)
            {
                array[index++] = value;
            }

            length = array.Length;
            while (index < length)
            {
                Buffer.BlockCopy(array, 0, array, index, M.Min(block, length - index));
                index += block;
                block *= 2;
            }
        }

        /// <summary>
        /// Sets the contents of a byte array to a given pattern.
        /// </summary>
        /// <param name="array">The array to fill.</param>
        /// <param name="value">The (multi-byte) pattern value.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Array and value must not be null and not empty.
        /// </exception>
        /// <remarks>
        ///     See remarks on <see cref="MemSet(byte[],byte)"/> for implementation details.
        /// </remarks>
        public static void MemSet(byte[] array, byte[] value)
        {
            if (array == null || array.Length == 0) throw new ArgumentNullException(nameof(array), "Must not be null and not an empty array.");
            if (value == null || value.Length == 0) throw new ArgumentNullException(nameof(value), "Must not be null and not an empty array.");

            // should work without this check as well
            // if (array.Length < value.Length || array.Length % value.Length != 0) 
            //    throw new InvalidOperationException($"Length of {nameof(value)} ({value.Length}) does not fit into {nameof(array)} (Length: {array.Length}).");

            int valLength = value.Length;
            int block = 32*valLength;
            int length = M.Min(block, array.Length);

            //Fill the initial array
            int index = 0;
            while (index < length)
            {
                Buffer.BlockCopy(value, 0, array, index, M.Min(valLength, length - index));
                index += valLength;
            }

            length = array.Length;
            while (index < length)
            {
                Buffer.BlockCopy(array, 0, array, index, M.Min(block, length - index));
                index += block;
                block *= 2;
            }
        }


        /// <summary>
        /// Creates an image and fills it with the given color.
        /// </summary>
        /// <param name="width">The width (in pixels) of the image to create.</param>
        /// <param name="height">The height (in pixels) of the image to create.</param>
        /// <param name="color">The color to fill the image with.</param>
        /// <returns>The newly created image.</returns>
        public static ImageData CreateImage(int width, int height, ColorUint color)
        {
            var ret = new ImageData
            {
                PixelData = new byte[width * height],
                Height = height,
                Width = width,
                PixelFormat = ImagePixelFormat.RGBA,
                Stride = width
            };

            MemSet(ret.PixelData, color.ToArray());

            return ret;
        }

    }
}