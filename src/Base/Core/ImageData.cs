using System;
using System.Collections.Generic;
using Fusee.Base.Common;
using Fusee.Math.Core;

namespace Fusee.Base.Core
{
    public class ImageData : IImageData
    {
        public byte[] PixelData { get; set; }
        public ImageData(byte[] pixelData, int width, int height, ImagePixelFormat colorFormat)
        {
            PixelData = pixelData;

            Width = width;
            Height = height;
            PixelFormat = colorFormat;
        }

        public int Width { get; }
        public int Height { get; }
        public ImagePixelFormat PixelFormat { get; }

        // scanline verwenden um blt durchzuführen
        public void Blt(int xDst, int yDst, IImageData src, int xSrc = 0, int ySrc = 0, int width = 0, int height = 0)
        {
            if (width == 0)
                width = src.Width;
            if (height == 0)
                height = src.Height;

            ClipBlt(ref xDst, Width, ref xSrc, src.Width, ref width);
            ClipBlt(ref yDst, Height, ref ySrc, src.Height, ref height);

            if (width <= 0 || height <= 0)
                return;

            CopyFunc CopyLine = null;
            if (PixelFormat.ColorFormat.Equals(src.PixelFormat.ColorFormat))
            {
                // Case: same colorspace, just loop over scanlines from src and copy linewise into this ImageData
                CopyLine = delegate(byte[] srcScanLineBytes, int destinationIndex)
                {
                    Array.Copy(srcScanLineBytes, 0, PixelData, destinationIndex, srcScanLineBytes.Length);
                };
            }
            else
            {
                // Wee need to perform pixel-conversion while copying. -> still GetLineBytes and then perform pixel conversion

                switch (PixelFormat.ColorFormat)
                {
                    case Common.ColorFormat.RGBA:
                        switch (src.PixelFormat.ColorFormat)
                        {
                            case Common.ColorFormat.RGB:
                                CopyLine = delegate(byte[] srcLineBytes, int destinationIndex)
                                {
                                    for (int i = 0; i < srcLineBytes.Length; i += 3) // jump 3 units per loop because we want to copy src RGB to dst RGBA
                                    {
                                        PixelData[destinationIndex + i + 0] = srcLineBytes[i + 0];
                                        PixelData[destinationIndex + i + 1] = srcLineBytes[i + 1];
                                        PixelData[destinationIndex + i + 2] = srcLineBytes[i + 2];
                                        PixelData[destinationIndex + i + 3] = byte.MaxValue;
                                    }
                                };
                                break;
                            case Common.ColorFormat.Intensity:
                                CopyLine = delegate (byte[] srcLineBytes, int destinationIndex)
                                {
                                    for (int i = 0; i < srcLineBytes.Length; i++) // jump 1 unit per loop because we want to copy src Intensity to dst RGBA
                                    {
                                        PixelData[destinationIndex + i + 0] = srcLineBytes[i];
                                        PixelData[destinationIndex + i + 1] = srcLineBytes[i];
                                        PixelData[destinationIndex + i + 2] = srcLineBytes[i];
                                        PixelData[destinationIndex + i + 3] = byte.MaxValue;
                                    }
                                };
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(src), "Unknown source pixel format to copy to RGBA");
                        }
                        break;
                    case Common.ColorFormat.RGB:
                        switch (src.PixelFormat.ColorFormat)
                        {
                            case Common.ColorFormat.RGBA:

                                CopyLine = delegate (byte[] srcLineBytes, int destinationIndex)
                                {
                                    for (int i = 0; i < srcLineBytes.Length; i += 4) // jump 4 units per loop because we want to copy src RGBA to dst RGB
                                    {
                                        PixelData[destinationIndex + i + 0] = srcLineBytes[i + 0];
                                        PixelData[destinationIndex + i + 1] = srcLineBytes[i + 1];
                                        PixelData[destinationIndex + i + 2] = srcLineBytes[i + 2];
                                        // skip source alpha
                                    }
                                };

                                break;
                            case Common.ColorFormat.Intensity:
                                CopyLine = delegate (byte[] srcLineBytes, int destinationIndex)
                                {
                                    for (int i = 0; i < srcLineBytes.Length; i++) // jump 1 unit per loop because we want to copy src Intensity to dst RGB
                                    {
                                        PixelData[destinationIndex + i + 0] = srcLineBytes[i];
                                        PixelData[destinationIndex + i + 1] = srcLineBytes[i];
                                        PixelData[destinationIndex + i + 2] = srcLineBytes[i];
                                    }
                                };
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(src), "Unknown source pixel format to copy to RGB");
                        }
                        break;
                    case Common.ColorFormat.Intensity:
                        switch (src.PixelFormat.ColorFormat)
                        {
                            case Common.ColorFormat.RGB:
                                CopyLine = delegate (byte[] srcLineBytes, int destinationIndex)
                                {
                                    for (int i = 0; i < srcLineBytes.Length; i += 3) // jump 3 units per loop because we want to copy src RGB to dst Intensity
                                    {
                                        // Quick integer Luma conversion (not accurate)
                                        // See http://stackoverflow.com/questions/596216/formula-to-determine-brightness-of-rgb-color
                                        int R = srcLineBytes[destinationIndex + i + 0];
                                        int G = srcLineBytes[destinationIndex + i + 1];
                                        int B = srcLineBytes[destinationIndex + i + 2];
                                        PixelData[destinationIndex + i] = (byte)((R + R + B + G + G + G) / 6);
                                    }
                                };
                                break;
                            case Common.ColorFormat.RGBA:
                                CopyLine = delegate (byte[] srcLineBytes, int destinationIndex)
                                {
                                    for (int i = 0; i < srcLineBytes.Length; i += 4) // jump 4 units per loop because we want to copy src RGBA to dst Intensity
                                    {
                                        // Quick integer Luma conversion (not accurate)
                                        // See http://stackoverflow.com/questions/596216/formula-to-determine-brightness-of-rgb-color
                                        int R = srcLineBytes[destinationIndex + i + 0];
                                        int G = srcLineBytes[destinationIndex + i + 1];
                                        int B = srcLineBytes[destinationIndex + i + 2];
                                        PixelData[destinationIndex + i] = (byte)((R + R + B + G + G + G) / 6);
                                    }
                                };
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(src), "Unknown source pixel format to copy to RGB");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(this.ToString(), "Unknown destination pixel format");
                } // end switch

            } // end else block

            // loop over the ScanLineEnumerator and call CopyLine delegate
            var srcEnumerator = src.ScanLines(xSrc, ySrc, width, height);

            while (srcEnumerator.MoveNext())
            {
                var srcScanLine = srcEnumerator.Current;
                if (srcScanLine != null)
                {
                    byte[] srcScanLineBytes = srcScanLine.GetScanLineBytes();
                    int destinationIndex = yDst * PixelFormat.BytesPerPixel*Width + xDst * PixelFormat.BytesPerPixel; // move down by yDst and add (move right) xDst
                    CopyLine(srcScanLineBytes, destinationIndex);
                    yDst++; // increment yDst == move to the next line
                }
            }
        }

        public IEnumerator<ScanLine> ScanLines(int xSrc = 0, int ySrc = 0, int width = 0, int height = 0)
        {
            if ((xSrc + width) > Width)
            {
                throw new ArgumentOutOfRangeException("Cannot get ScanLineEnumerator due to exceeding ImageData Width="+Width+". Choose xSrc+width to be smaller than Width of ImageData");
            }
            if ((ySrc + height) > Height)
            {
                throw new ArgumentOutOfRangeException("Cannot get ScanLineEnumerator due to exceeding ImageData Height=" + Height + ". Choose ySrc+height to be smaller than Height of ImageData");
            }

            for (int i = ySrc; i < height; i++)
            {
                // 1D offsetCoordinate that represents location in PixelData byte array (sizeof PixelData is Width*Height*BytesPerPixel)
                int srcOffset = ((PixelFormat.BytesPerPixel * Width) * i) + xSrc * PixelFormat.BytesPerPixel; // go down vertical along i by width times BytesPerPixel and then add horizontal width offset times BytesPerPixel
                yield return new ScanLine(PixelData,srcOffset,width, PixelFormat);
            }
            //IEnumerator<ScanLine> result = new ScanLineEnumerator(PixelData, ColorFormat, xSrc, ySrc, width, height, Width);

            //return result;
        }

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
        private static void ClipBlt(ref int iDst, int sizeDst, ref int iSrc, int sizeSrc, ref int sizeBlk)
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


        private delegate void CopyFunc(byte[] srcScanLineBytes, int dstIndex);

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
        //private static void Blt(ImageData dst, int xDst, int yDst, ImageData src, int xSrc = 0, int ySrc = 0, int width = 0, int height = 0)
        //{
        //    if (width == 0)
        //        width = src.Width;
        //    if (height == 0)
        //        height = src.Height;

        //    ClipBlt(ref xDst, dst.Width, ref xSrc, src.Width, ref width);
        //    ClipBlt(ref yDst, dst.Height, ref ySrc, src.Height, ref height);

        //    if (width <= 0 || height <= 0)
        //        return;

        //    CopyFunc CopyLine = null;
        //    if (dst.ColorFormat.Equals(src.ColorFormat))
        //    {
        //        // We can copy an entire line en-bloc
        //        CopyLine = delegate (int idl, int isl)
        //        {
        //            // hier scanline rein
        //            Array.Copy(src.PixelData, isl + xSrc * src.ColorFormat.BytesPerPixel,
        //                       dst.PixelData, idl + xDst * dst.ColorFormat.BytesPerPixel,
        //                       width * dst.ColorFormat.BytesPerPixel);
        //        };
        //    }
        //    else
        //    {
        //        // Wee need to perform pixel-conversion while copying.
        //        CopyFunc CopyPixel = null;

        //        switch (dst.ColorFormat.ColorFormat)
        //        {
        //            case ColorFormat.RGBA:
        //                switch (src.ColorFormat.ColorFormat)
        //                {
        //                    case ColorFormat.RGB:
        //                        CopyPixel = delegate (int idp, int isp)
        //                        {
        //                            dst.PixelData[idp + 0] = src.PixelData[isp + 0];
        //                            dst.PixelData[idp + 1] = src.PixelData[isp + 1];
        //                            dst.PixelData[idp + 2] = src.PixelData[isp + 2];
        //                            dst.PixelData[idp + 3] = byte.MaxValue;
        //                        };
        //                        break;
        //                    case ColorFormat.Intensity:
        //                        CopyPixel = delegate (int idp, int isp)
        //                        {
        //                            dst.PixelData[idp + 0] = src.PixelData[isp];
        //                            dst.PixelData[idp + 1] = src.PixelData[isp];
        //                            dst.PixelData[idp + 2] = src.PixelData[isp];
        //                            dst.PixelData[idp + 3] = byte.MaxValue;
        //                        };
        //                        break;
        //                    default:
        //                        throw new ArgumentOutOfRangeException(nameof(src), "Unknown source pixel format to copy to RGBA");
        //                }
        //                break;
        //            case ColorFormat.RGB:
        //                switch (src.ColorFormat.ColorFormat)
        //                {
        //                    case ColorFormat.RGBA:
        //                        CopyPixel = delegate (int idp, int isp)
        //                        {
        //                            dst.PixelData[idp + 0] = src.PixelData[isp + 0];
        //                            dst.PixelData[idp + 1] = src.PixelData[isp + 1];
        //                            dst.PixelData[idp + 2] = src.PixelData[isp + 2];
        //                            // skip source alpha src.PixelData[isp + 3];
        //                        };
        //                        break;
        //                    case ColorFormat.Intensity:
        //                        CopyPixel = delegate (int idp, int isp)
        //                        {
        //                            dst.PixelData[idp + 0] = src.PixelData[isp];
        //                            dst.PixelData[idp + 1] = src.PixelData[isp];
        //                            dst.PixelData[idp + 2] = src.PixelData[isp];
        //                        };
        //                        break;
        //                    default:
        //                        throw new ArgumentOutOfRangeException(nameof(src), "Unknown source pixel format to copy to RGB");
        //                }
        //                break;
        //            case ColorFormat.Intensity:
        //                switch (src.ColorFormat.ColorFormat)
        //                {
        //                    case ColorFormat.RGB:
        //                    case ColorFormat.RGBA:
        //                        CopyPixel = delegate (int idp, int isp)
        //                        {
        //                            // Quick integer Luma conversion (not accurate)
        //                            // See http://stackoverflow.com/questions/596216/formula-to-determine-brightness-of-rgb-color
        //                            int R = src.PixelData[isp + 0];
        //                            int G = src.PixelData[isp + 1];
        //                            int B = src.PixelData[isp + 2];
        //                            dst.PixelData[idp] = (byte)((R + R + B + G + G + G) / 6);
        //                        };
        //                        break;
        //                    default:
        //                        throw new ArgumentOutOfRangeException(nameof(src), "Unknown source pixel format to copy to RGB");
        //                }
        //                break;
        //            default:
        //                throw new ArgumentOutOfRangeException(nameof(dst), "Unknown destination pixel format");
        //        }
        //        CopyLine = delegate (int idl, int isl)
        //        {
        //            // hier scanline holen
        //            int iDstPxl = idl + xDst * dst.ColorFormat.BytesPerPixel;
        //            int iSrcPxl = isl + xSrc * src.ColorFormat.BytesPerPixel;
        //            for (int x = 0; x < width; x++)
        //            {
        //                CopyPixel(iDstPxl, iSrcPxl);
        //                iDstPxl += dst.ColorFormat.BytesPerPixel;
        //                iSrcPxl += src.ColorFormat.BytesPerPixel;
        //            }
        //        };
        //    }

        //    int iDstLine = yDst * dst.ColorFormat.BytesPerPixel;
        //    int iSrcLine = ySrc * src.ColorFormat.BytesPerPixel;
        //    for (int y = 0; y < height; y++)
        //    {
        //        CopyLine(iDstLine, iSrcLine);
        //        iDstLine += dst.ColorFormat.BytesPerPixel;
        //        iSrcLine += src.ColorFormat.BytesPerPixel;
        //    }
        //}

        /// <summary>
        /// Checks if an image contains no data by checking if <see cref="Width"/> or <see cref="Height"/> is 0.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => (Width <= 0 || Height <= 0);


        /// <summary>
        /// Creates an image and fills it with the given color.
        /// </summary>
        /// <param name="width">The width (in pixels) of the image to create.</param>
        /// <param name="height">The height (in pixels) of the image to create.</param>
        /// <param name="color">The color to fill the image with.</param>
        /// <returns>The newly created image.</returns>
        public static ImageData CreateImage(int width, int height, ColorUint color)
        {
            int colorVal = color.ToBgra();
            int nPixels = width * height;
            int nBytes = nPixels * 4;
            int[] pxls = new int[nPixels];

            for (int i = 0; i < pxls.Length; i++)
                pxls[i] = colorVal;

            var ret = new ImageData(new byte[nBytes], width, height,
                new ImagePixelFormat(ColorFormat.RGBA));

            Buffer.BlockCopy(pxls, 0, ret.PixelData, 0, nBytes);
            return ret;


            /*
            var ret = new ImageData(new byte[width * height*4], width, height, new ImagePixelFormat(ColorFormat.RGBA));

            MemSet(ret.PixelData, color.ToArray());

            return ret;*/
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
        private static void MemSet(byte[] array, byte[] value)
        {
            if (array == null || array.Length == 0) throw new ArgumentNullException(nameof(array), "Must not be null and not an empty array.");
            if (value == null || value.Length == 0) throw new ArgumentNullException(nameof(value), "Must not be null and not an empty array.");

            // should work without this check as well
            // if (array.Length < value.Length || array.Length % value.Length != 0) 
            //    throw new InvalidOperationException($"Length of {nameof(value)} ({value.Length}) does not fit into {nameof(array)} (Length: {array.Length}).");

            int valLength = value.Length;
            int block = 32 * valLength;
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
    }
}