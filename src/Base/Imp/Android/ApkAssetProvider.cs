using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Fusee.Base.Common;
using Fusee.Base.Core;
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
                            return FileDecoder.LoadImage((Stream)storage);
                    }
                    return null;
                },
                Checker = delegate (string id)
                {
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
            });

            // Text file -> String handler. Keep this one the last entry as it doesn't check the extension
            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(string),
                Decoder = delegate (string id, object storage)
                {
                    var sr = new StreamReader((Stream)storage, System.Text.Encoding.Default, true);
                    return sr.ReadToEnd();
                },
                Checker = id => true // If it's there, we can handle it...
            });
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
        /// Checks the existence of the identified asset using <see cref="File.Exists"/>
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

        protected override Task<Stream> GetStreamAsync(string id)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> CheckExistsAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}