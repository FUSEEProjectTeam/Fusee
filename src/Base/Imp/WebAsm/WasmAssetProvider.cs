using Fusee.Base.Common;
using Fusee.Base.Core;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime.InteropServices;

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
        public static string GetLocalAddress(IJSRuntime runtime)
        {
            using var window = runtime.GetGlobalObject<IJSInProcessObjectReference>("window");
            using var location = window.GetObjectProperty<IJSInProcessObjectReference>("location");

            var address = location.GetObjectProperty<string>("href");

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
            var baseAddress = WasmResourceLoader.GetLocalAddress(_runtime) + "Assets/";
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
            var baseAddress = WasmResourceLoader.GetLocalAddress(_runtime) + "Assets/";
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };

            Diagnostics.Debug($"Requesting '{id}' at '{baseAddress}' ...");

            try
            {
                var response = await httpClient.GetAsync(id);
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsByteArrayAsync();
                var ms = new MemoryStream(stream); // copy to memory stream to prevent any loading issues
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

            var baseAddress = WasmResourceLoader.GetLocalAddress(_runtime) + "Assets/";
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

            var baseAddress = WasmResourceLoader.GetLocalAddress(_runtime) + "Assets/";
            using var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
            var response = await httpClient.GetAsync(id).ConfigureAwait(false);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public static ImageData LoadImage(Stream file)
        {
            try
            {
                using var image = Image.Load(file, out var imgFormat);

                image.Mutate(x => x.AutoOrient());
                image.Mutate(x => x.RotateFlip(RotateMode.None, FlipMode.Vertical));


                var bpp = image.PixelType.BitsPerPixel;

                switch (image.PixelType.BitsPerPixel)
                {
                    case 16:
                        {
                            (image as Image<Rg32>).TryGetSinglePixelSpan(out var res);
                            var resBytes = MemoryMarshal.AsBytes<Rg32>(res.ToArray());
                            return new ImageData(resBytes.ToArray(), image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.Depth16));
                        }
                    case 24:
                        {
                            var rgb = image as Image<Rgb24>;
                            var bgr = rgb.CloneAs<Bgr24>();

                            bgr.TryGetSinglePixelSpan(out var res);
                            var resBytes = MemoryMarshal.AsBytes<Bgr24>(res.ToArray());
                            return new ImageData(resBytes.ToArray(), image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.RGB));
                        }
                    case 32:
                        {
                            var rgba = image as Image<Rgba32>;
                            var bgra = rgba.CloneAs<Bgra32>();

                            bgra.TryGetSinglePixelSpan(out var res);
                            var resBytes = MemoryMarshal.AsBytes<Bgra32>(res.ToArray());
                            return new ImageData(resBytes.ToArray(), image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.RGBA));
                        }
                    case 48:
                        {
                            var rgba = image as Image<Rgba32>;
                            var bgra = rgba.CloneAs<Bgra32>();

                            (image as Image<Rgb48>).TryGetSinglePixelSpan(out var res);
                            var resBytes = MemoryMarshal.AsBytes<Rgb48>(res.ToArray());
                            return new ImageData(resBytes.ToArray(), image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.fRGB32));
                        }
                    case 64:
                        {
                            (image as Image<Rgba64>).TryGetSinglePixelSpan(out var res);
                            var resBytes = MemoryMarshal.AsBytes<Rgba64>(res.ToArray());
                            return new ImageData(resBytes.ToArray(), image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.fRGBA32));
                        }
                    default:
                        throw new ArgumentException($"{bpp} Bits per pixel not supported!");
                }
            }
            catch (Exception ex)
            {
                Diagnostics.Error($"Error loading/converting image", ex);
                return null;
            }
        }
    }
}
