using Fusee.Base.Common;
using Fusee.Base.Core;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.Base.Imp.Blazor
{
    /// <summary>
    /// Loading resources, helper class
    /// </summary>
    public static class BlazorAssetProvider
    {
        /// <summary>
        /// returns the local HTTP address
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetLocalAddress(IJSRuntime runtime)
        {
            //using IJSInProcessObjectReference window = runtime.GetGlobalObject<IJSInProcessObjectReference>("window");
            //using IJSInProcessObjectReference location = window.GetObjectProperty<IJSInProcessObjectReference>("location");

            string address = await runtime.InvokeAsync<string>("getBaseAdress");

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
        private readonly IJSRuntime _runtime;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetProvider"/> class.
        /// </summary>
        /// <param name="baseDir">The base directory where assets should be looked for.</param>
        /// <param name="runtime">The JSRuntime</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public AssetProvider(IJSRuntime runtime, string baseDir = null)
        {
            _baseDir = (string.IsNullOrEmpty(baseDir)) ? "Assets" : baseDir;
            _runtime = runtime;

            if (_baseDir[^1] != '/')
                _baseDir += '/';

            // Image handler
            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(ImageData),
                Decoder = (string id, object storage) =>
                {
                    throw new NotSupportedException("Synchronous Decoder is not supported - use DecoderAsync instead!");
                },
                DecoderAsync = async (string id, object storage) =>
                {
                    var ext = Path.GetExtension(id).ToLower();
                    return ext switch
                    {
                        ".jpg" or ".jpeg" or ".png" or ".bmp" or ".tga" => await FileDecoder.LoadImageAsync((Stream)storage).ConfigureAwait(false),
                        _ => null,
                    };
                },
                Checker = (string id) =>
                {
                    var ext = Path.GetExtension(id).ToLower();
                    return ext switch
                    {
                        ".jpg" or ".jpeg" or ".png" or ".bmp" or ".tga" => true,
                        _ => false,
                    };
                }
            });

            // Text file -> String handler. Keep this one the last entry as it doesn't check the extension
            RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(string),
                Decoder = (string id, object storage) =>
                {
                    throw new NotSupportedException("Synchronous Decoder is not supported - use DecoderAsync instead!");
                },
                DecoderAsync = async (string _, object storage) =>
                {
                    Stream storageStream = (Stream)storage;
                    using StreamReader streamReader = new(storageStream, Encoding.ASCII);
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
            string baseAddress = BlazorAssetProvider.GetLocalAddress(_runtime).Result + "Assets/";
            using HttpClient httpClient = new() { BaseAddress = new Uri(baseAddress) };
            Task<HttpResponseMessage> response = httpClient.GetAsync(id);
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
            string baseAddress = await BlazorAssetProvider.GetLocalAddress(_runtime) + "Assets/";
            HttpClient httpClient = new() { BaseAddress = new Uri(baseAddress) };

            Diagnostics.Debug($"Requesting '{id}' at '{baseAddress}' ...");

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(id);
                response.EnsureSuccessStatusCode();
                byte[] stream = await response.Content.ReadAsByteArrayAsync();
                MemoryStream ms = new(stream); // copy to memory stream to prevent any loading issues
                httpClient.Dispose();
                return ms;
            }
            catch (HttpRequestException exception)
            {
                Diagnostics.Error($"Error while loading asset {id}", exception);
                httpClient.Dispose();
                return null;
            }
            catch (ArgumentNullException exception)
            {
                Diagnostics.Error($"Error while loading asset, not found {id}", exception);
                httpClient.Dispose();
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

            string baseAddress = BlazorAssetProvider.GetLocalAddress(_runtime).Result + "Assets/";
            using HttpClient httpClient = new() { BaseAddress = new Uri(baseAddress) };
            Task<HttpResponseMessage> response = httpClient.GetAsync(id);
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

            string baseAddress = await BlazorAssetProvider.GetLocalAddress(_runtime) + "Assets/";
            using HttpClient httpClient = new() { BaseAddress = new Uri(baseAddress) };
            HttpResponseMessage response = await httpClient.GetAsync(id).ConfigureAwait(false);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}