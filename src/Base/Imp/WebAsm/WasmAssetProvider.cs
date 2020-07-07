using Fusee.Base.Common;
using Fusee.Base.Core;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAssembly;

namespace Fusee.Base.Imp.WebAsm
{
    /// <summary>
    /// Loading resources, helper class
    /// </summary>
    public static class WasmResourceLoader
    {
        /// <summary>
        /// returns the local HTTP address
        /// </summary>
        /// <returns></returns>
        public static string GetLocalAddress()
        {
            using var window = (JSObject)Runtime.GetGlobalObject("window");
            using var location = (JSObject)window.GetObjectProperty("location");

            var address = (string)location.GetObjectProperty("href");

            if (address.Contains("/"))
            {
                address = address.Substring(0, address.LastIndexOf('/') + 1);
            }

            return address;
        }
    }

    /// <summary>
    /// Asset provider for direct file access. Typically used in desktop builds where assets are simply contained within
    /// a subdirectory of the application.
    /// </summary>
    public class AssetProvider : StreamAssetProvider
    {
        private readonly string _baseDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetProvider"/> class.
        /// </summary>
        /// <param name="baseDir">The base directory where assets should be looked for.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public AssetProvider(string baseDir = null)
        {
            _baseDir = (string.IsNullOrEmpty(baseDir)) ? "Assets" : baseDir;

            if (_baseDir[_baseDir.Length - 1] != '/')
                _baseDir += '/';

            // Text file -> String handler. Keep this one the last entry as it doesn't check the extension
            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(string),
                DecoderAsync = async (string _, object storage) =>
                {
                    var storageStream = (Stream)storage;
                    using var streamReader = new StreamReader(storageStream, Encoding.ASCII);
                    return await streamReader.ReadToEndAsync().ConfigureAwait(false);
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
            var baseAddress = WasmResourceLoader.GetLocalAddress() + "Assets/";
            using var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
            var response = httpClient.GetAsync(id);
            return response.Result.Content.ReadAsStreamAsync().Result;
        }

        /// <summary>
        /// Creates an async stream for the asset identified by id using <see cref="FileStream"/>
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>
        /// A valid stream for reading if the asset ca be retrieved. null otherwise.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected override async Task<Stream> GetStreamAsync(string id)
        {
            var baseAddress = WasmResourceLoader.GetLocalAddress() + "Assets/";
            using var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };

            Diagnostics.Debug($"Requesting '{id}' at '{baseAddress}' ...");

            try
            {
                var response = await httpClient.GetAsync(id).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
            catch (HttpRequestException exception)
            {
                Diagnostics.Error($"Error while loading asset {id}", exception);
                return null;
            }
            catch (ArgumentNullException exception)
            {
                Diagnostics.Error($"Error while loading asset, not found {id}", exception);
                return null;
            }
        }

        /// <summary>
        /// Checks the existence of the identified asset
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>
        /// true if a stream can be created.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected override bool CheckExists(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var baseAddress = WasmResourceLoader.GetLocalAddress() + "Assets/";
            using var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
            var response = httpClient.GetAsync(id);
            return response.Result.StatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Checks the existence of the identified asset in an async manner
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <returns>
        /// true if a stream can be created.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected override async Task<bool> CheckExistsAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var baseAddress = WasmResourceLoader.GetLocalAddress() + "Assets/";
            using var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
            var response = await httpClient.GetAsync(id).ConfigureAwait(false);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
