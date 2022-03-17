﻿using Fusee.Base.Common;
using Fusee.Base.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Fusee.Base.Imp.Desktop
{
    /// <summary>
    /// Provides methods for platform specific conversions of file content.
    /// </summary>
    public static class FileDecoder
    {
        /// <summary>
        /// Loads a new Bitmap-Object from the given stream.
        /// </summary>
        /// <param name="file">Stream containing the image in a supported format (png, jpg).</param>
        /// <returns>An ImageData object with all necessary information.</returns>
        public static Task<ImageData> LoadImageAsync(Stream file)
        {
            return Task.FromResult(LoadImage(file));
        }

        /// <summary>
        /// Loads a new Bitmap-Object from the given stream.
        /// </summary>
        /// <param name="file">Stream containing the image in a supported format (png, jpg).</param>
        /// <returns>An ImageData object with all necessary information.</returns>
        public static ImageData LoadImage(Stream file)
        {
            try
            {
                using var ms = new MemoryStream();
                file.CopyTo(ms);
                ms.Position = 0;
                //Load the image
                var image = Image.Load(ms, out var imgFormat);

                //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
                //This will correct that, making the texture display properly.
                image.Mutate(x => x.Flip(FlipMode.Vertical));

                //Convert ImageSharp's format into a byte array, so we can use it with OpenGL.
                var pixels = new byte[4 * image.Width * image.Height];

                var bpp = image.PixelType.BitsPerPixel;

                switch (image.PixelType.BitsPerPixel)
                {
                    case 16:
                        {
                            (image as Image<L16>).CopyPixelDataTo(pixels);
                            return new ImageData(pixels, image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.Depth16));
                        }
                    case 24:
                        {
                            var rgb = image as Image<Rgb24>;
                            var bgr = rgb.CloneAs<Bgr24>();

                            bgr.CopyPixelDataTo(pixels);
                            return new ImageData(pixels, image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.RGB));
                        }
                    case 32:
                        {
                            var rgba = image as Image<Rgba32>;
                            var bgra = rgba.CloneAs<Bgra32>();

                            bgra.CopyPixelDataTo(pixels);
                            return new ImageData(pixels, image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.RGBA));
                        }
                    case 48:
                        {
                            (image as Image<Rgb48>).CopyPixelDataTo(pixels);
                            return new ImageData(pixels, image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.fRGB32));
                        }
                    case 64:
                        {
                            (image as Image<Rgba64>).CopyPixelDataTo(pixels);
                            return new ImageData(pixels, image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.fRGBA32));
                        }
                    default:
                        throw new ArgumentException($"{bpp} Bits per pixel not supported!");
                }
            }
            catch (Exception ex)
            {
                Diagnostics.Error($"Error loading/converting image", ex);
                return new ImageData();
            }
        }
    }
}