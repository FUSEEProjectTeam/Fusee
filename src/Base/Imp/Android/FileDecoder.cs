using Android.Graphics;
using Fusee.Base.Common;
using Fusee.Base.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Fusee.Base.Imp.Android
{
    /// <summary>
    /// Provides methods for platform specific conversions of file content.
    /// </summary>
    public static class FileDecoder
    {
        /// <summary>
        /// Creates a new Bitmap-Object from an image file,
        /// locks the bits in the memory and makes them available
        /// for further action (e.g. creating a texture).
        /// Method must be called before creating a texture to get the necessary
        /// ImageData struct.
        /// </summary>
        /// <param name="file">Stream containing the image in a supported format (png, jpg).</param>
        /// <returns>An ImageData object with all necessary information for the texture-binding process.</returns>
        public static Task<ImageData> LoadImageAsync(Stream file)
        {
            return Task.Factory.StartNew(() => LoadImage(file));
        }

        /// <summary>
        /// Creates a new Bitmap-Object from an image file,
        /// locks the bits in the memory and makes them available
        /// for further action (e.g. creating a texture).
        /// Method must be called before creating a texture to get the necessary
        /// ImageData struct.
        /// </summary>
        /// <param name="file">Stream containing the image in a supported format (png, jpg).</param>
        /// <returns>An ImageData object with all necessary information for the texture-binding process.</returns>
        public static ImageData LoadImage(Stream file)
        {
            var bmp = BitmapFactory.DecodeStream(file, null, new BitmapFactory.Options { InPremultiplied = false, InPreferredConfig = Bitmap.Config.Argb8888 });

            int nPixels = bmp.Width * bmp.Height;
            int nBytes = nPixels * 4;
            int[] pxls = new int[nPixels];
            bmp.GetPixels(pxls, 0, bmp.Width, 0, 0, bmp.Width, bmp.Height);
            // Convert ABGR to ARGB!
            // On Android Images are loaded in ABGR format by default and need to be converted to ARGB for usage inside RenderContextImp
            for (int i = 0; i < pxls.Length; i++)
            {
                uint pixel = (uint)pxls[i];
                uint alpha = (pixel & 0xFF000000) >> 24;
                uint red = (pixel & 0x00FF0000) >> 16;
                uint green = (pixel & 0x0000FF00) >> 8;
                uint blue = (pixel & 0x000000FF) >> 0;
                pixel = (alpha << 24) | (blue << 16) | (green << 8) | red;
                pxls[i] = (int)pixel;
            }
            var ret = new ImageData(new byte[nBytes], bmp.Width, bmp.Height,
                new ImagePixelFormat(ColorFormat.RGBA));

            // Flip upside down
            for (int iLine = 0; iLine < ret.Height; iLine++)
            {
                Buffer.BlockCopy(pxls, (bmp.Height - 1 - iLine) * bmp.Width * 4, ret.PixelData, iLine * bmp.Width * 4, bmp.Width * 4);
            }

            // As a whole... Buffer.BlockCopy(pixels, 0, ret.PixelData, 0, nBytes);
            return ret;
        }
    }
}