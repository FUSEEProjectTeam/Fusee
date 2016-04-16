using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Android.Content;
using Android.Graphics;
using Fusee.Base.Common;
using Path = Fusee.Base.Common.Path;

namespace Fusee.Base.Imp.Android
{
    public class ApkAssetProvider : StreamAssetProvider
    {
        Context _androidContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApkAssetProvider" /> class.
        /// </summary>
        /// <param name="androidContext">The android context.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ApkAssetProvider(Context androidContext) : base()
        {
            _androidContext = androidContext;

            RegisterTypeHandler( new AssetHandler { 
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
                }}
            );

            /*
            // Text file -> String handler. Keep this one the last entry as it doesn't check the extension
            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(string),
                Decoder = delegate (string id, object storage)
                {
                    var sr = new StreamReader((Stream)storage);
                    return sr.ReadToEnd();
                },
                Checker = id => true // If it's there, we can handle it...
            }
            );
            */
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

            return _androidContext.Assets.Open(id);
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

            string dir = Path.GetDirectoryName(id);
            string file = Path.GetFileName(id);

            return _androidContext.Assets.List(dir).Contains(file);
        }


        /// <summary>
        /// Creates a new Bitmap-Object from an image file,
        /// locks the bits in the memory and makes them available
        /// for furher action (e.g. creating a texture).
        /// Method must be called before creating a texture to get the necessary
        /// ImageData struct.
        /// </summary>
        /// <param name="file">Stream containing the image in a supported format (png, jpg).</param>
        /// <returns>An ImageData object with all necessary information for the texture-binding process.</returns>
        public static ImageData LoadImage(Stream file)
        {
            Bitmap bmp = BitmapFactory.DecodeStream(file, null, new BitmapFactory.Options {InPremultiplied = false});

            int nPixels = bmp.Width * bmp.Height;
            int nBytes = nPixels * 4;
            int[] pxls = new int[nPixels];
            bmp.GetPixels(pxls, 0, bmp.Width, 0, 0, bmp.Width, bmp.Height);

            var ret = new ImageData
            {
                PixelData = new byte[nBytes],
                Height = bmp.Height,
                Width = bmp.Width,
                PixelFormat = ImagePixelFormat.RGBA,
                Stride = bmp.Width
            };

            // Flip upside down
            for (int iLine = 0; iLine < ret.Height; iLine++)
            {
                Buffer.BlockCopy(pxls, (bmp.Height - 1 - iLine) * bmp.Width * 4, ret.PixelData, iLine * bmp.Width * 4, bmp.Width * 4);
            }

            // As a whole... Buffer.BlockCopy(pxls, 0, ret.PixelData, 0, nBytes);
            return ret;
        }

    }
}