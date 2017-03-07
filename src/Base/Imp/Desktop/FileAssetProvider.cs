using System;
using System.IO;
using System.Runtime.InteropServices;
using Fusee.Base.Common;
using System.Drawing;
using System.Drawing.Imaging;
using FileMode = System.IO.FileMode;
using Path = Fusee.Base.Common.Path;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Fusee.Base.Imp.Desktop
{
    /// <summary>
    /// Asset provider for direct file access. Typically used in desktop builds where assets are simply contained within
    /// a subdirectory of the application.
    /// </summary>
    public class FileAssetProvider : StreamAssetProvider
    {
        private readonly string _baseDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAssetProvider"/> class.
        /// </summary>
        /// <param name="baseDir">The base directory where assets should be looked for.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public FileAssetProvider(string baseDir = null) : base()
        {
            if (string.IsNullOrEmpty(baseDir))
                baseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(baseDir))
                throw new ArgumentException($"Asset base directory \"{baseDir}\"does not exist.", nameof(baseDir));

            _baseDir = baseDir;

            // Image handler
            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(ImageData),
                Decoder = delegate (string id, object storage)
                {
                    string ext = Path.GetExtension(id).ToLower();
                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                            return LoadImage((Stream)storage);
                    }
                    return null;
                },
                Checker = delegate (string id) {
                    string ext = Path.GetExtension(id).ToLower();
                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                            return true;
                    }
                    return false;
                }
            }
            );

            // Text file -> String handler. Keep this one the last entry as it doesn't check the extension
            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(string),
                Decoder = delegate (string id, object storage)
                {
                    string ret;
                    using (var sr = new StreamReader((Stream) storage, System.Text.Encoding.Default, true))
                    {
                        ret = sr.ReadToEnd();
                    }
                    return ret;
                },
                Checker = id => true // If it's there, we can handle it...
            }
            );
        }


        /// <summary>
        /// Creates a stream for the asset identified by id using <see cref="FileStream"/>
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>
        /// A valid stream for reading if the asset ca be retrieved. null otherwise.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected override Stream GetStream(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            // If it is an absolute path (e.g. C:\SomeDir\AnAssetFile.ext) open it directly
            if (Path.IsPathRooted(id))
                return new FileStream(id, FileMode.Open);
            
            // Path seems relative. First see if the file exists at the current working directory
            if (File.Exists(id))
                return new FileStream(id, FileMode.Open);

            // At last, look at the specified asst path
            string path = Path.Combine(_baseDir, id);
            return new FileStream(path, FileMode.Open);
        }

        /// <summary>
        /// Checks the existance of the identified asset using <see cref="File.Exists"/>
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>
        /// true if a stream can be created.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected override bool CheckExists(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            
            // If it is an absolute path (e.g. C:\SomeDir\AnAssetFile.ext) directly check its presence
            if (Path.IsPathRooted(id))
                return File.Exists(id);

            // Path seems relative. First see if the file exists at the current working directory
            if (File.Exists(id))
                return true;

            string path = Path.Combine(_baseDir, id);
            return File.Exists(path);
        }

        /// <summary>
        /// Loads a new Bitmap-Object from the given stream.
        /// </summary>
        /// <param name="file">Stream containing the image in a supported format (png, jpg).</param>
        /// <returns>An ImageData object with all necessary information.</returns>
        public static ImageData LoadImage(Stream file)
        {
            var bmp = new Bitmap(file);

            //Flip y-axis, otherwise texture would be upside down
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            int bytes = (strideAbs) * bmp.Height;

            var ret = new ImageData
            {
                PixelData = new byte[bytes],
                Height = bmpData.Height,
                Width = bmpData.Width,
                PixelFormat = ImagePixelFormat.RGBA,
                Stride = bmpData.Stride
            };

            Marshal.Copy(bmpData.Scan0, ret.PixelData, 0, bytes);

            bmp.UnlockBits(bmpData);
            return ret;
        }

    }
}
