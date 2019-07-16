using Fusee.Base.Core;
using System;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;

namespace Fusee.Base.Imp.WebAsm
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
        public static async Task<ImageData> LoadImage(Stream file)
        {
            //var ms = new MemoryStream();
            //await file.CopyToAsync(ms);
            //var byteImage = ms.ToArray();

            ////var bmp = new Bitmap(file);

            //////Flip y-axis, otherwise texture would be upside down
            ////bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            ////BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
            ////    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            ////int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            ////int bytes = (strideAbs) * bmp.Height;            

            //var ret = new ImageData(byteImage, 174, 50,
            //  new Common.ImagePixelFormat(Common.ColorFormat.RGB));


            //Marshal.Copy(bmpData.Scan0, ret.PixelData, 0, bytes);

            //bmp.UnlockBits(bmpData);
            return new ImageData();

        }
    }
}
