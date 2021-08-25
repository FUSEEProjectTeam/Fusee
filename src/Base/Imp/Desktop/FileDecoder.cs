using Fusee.Base.Common;
using Fusee.Base.Core;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
                using var image = Image.Load<Rgba32>(file);

                image.Mutate(x => x.AutoOrient());
                image.Mutate(x => x.RotateFlip(RotateMode.None, FlipMode.Vertical));
                image.TryGetSinglePixelSpan(out var res);
                var resBytes = MemoryMarshal.AsBytes<Rgba32>(res.ToArray());
                var ret = new ImageData(resBytes.ToArray(), image.Width, image.Height,
                        new ImagePixelFormat(ColorFormat.RGBA));

                return ret;
            }
            catch (Exception ex)
            {
                Diagnostics.Error($"Error loading/converting image", ex);
                return null;
            }
        }
    }
}