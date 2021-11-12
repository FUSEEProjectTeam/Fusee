using Android.Content;
using Fusee.Base.Common;
using Fusee.Base.Core;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fusee.Base.Imp.Android
{
    /// <summary>
    /// <see cref="StreamAssetProvider"/> implementation for the Android platform
    /// </summary>
    public class ApkAssetProvider : StreamAssetProvider
    {
        readonly Context _androidContext;

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
                Decoder = (string id, object storage) =>
                {
                    var ext = Path.GetExtension(id).ToLower();
                    return ext switch
                    {
                        ".jpg" or ".jpeg" or ".png" or ".bmp" => FileDecoder.LoadImage((Stream)storage),
                        _ => null,
                    };
                },
                DecoderAsync = async (string id, object storage) =>
                {
                    var ext = Path.GetExtension(id).ToLower();
                    return ext switch
                    {
                        ".jpg" or ".jpeg" or ".png" or ".bmp" => await FileDecoder.LoadImageAsync((Stream)storage).ConfigureAwait(false),
                        _ => null,
                    };
                },
                Checker = (string id) =>
                {
                    string ext = Path.GetExtension(id).ToLower();
                    return ext switch
                    {
                        ".jpg" or ".jpeg" or ".png" or ".bmp" => true,
                        _ => false,
                    };
                }
            });

            // Text file -> String handler. Keep this one the last entry as it doesn't check the extension
            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(string),
                Decoder = (string _, object storage) =>
                {
                    var sr = new StreamReader((Stream)storage, System.Text.Encoding.Default, true);
                    return sr.ReadToEnd();
                },
                DecoderAsync = async (string _, object storage) =>
                {
                    var sr = new StreamReader((Stream)storage, System.Text.Encoding.Default, true);
                    return await sr.ReadToEndAsync().ConfigureAwait(false);
                },
                Checker = _ => true // If it's there, we can handle it...
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
        /// Checks the existence of the identified asset.
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>true if the asset exists.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected override bool CheckExists(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            string dir = Path.GetDirectoryName(id);
            string file = Path.GetFileName(id);

            return _androidContext.Assets.List(dir).Contains(file);
        }

        /// <summary>
        /// Creates a stream for the asset identified by id using <see cref="FileStream"/>
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>
        /// A valid stream for reading if the asset ca be retrieved. null otherwise.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected override async Task<Stream> GetStreamAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await Task.FromResult(_androidContext.Assets.Open(id));
        }

        /// <summary>
        /// Checks the existence of the identified asset
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>true if the asset exists.</returns>
        protected override Task<bool> CheckExistsAsync(string id)
        {
            return Task.Factory.StartNew(() =>
            {
                if (id == null) throw new ArgumentNullException(nameof(id));

                string dir = Path.GetDirectoryName(id);
                string file = Path.GetFileName(id);

                return _androidContext.Assets.List(dir).Contains(file);
            });
        }
    }
}