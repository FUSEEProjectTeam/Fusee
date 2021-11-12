using Fusee.Base.Common;
using Fusee.Base.Core;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
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
        public static string GetLocalAddress(IJSRuntime runtime)
        {
            using IJSInProcessObjectReference window = runtime.GetGlobalObject<IJSInProcessObjectReference>("window");
            using IJSInProcessObjectReference location = window.GetObjectProperty<IJSInProcessObjectReference>("location");

            string address = location.GetObjectProperty<string>("href");

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
                    Stream storageStream = (Stream)storage;
                    using StreamReader streamReader = new StreamReader(storageStream, Encoding.ASCII);
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
            string baseAddress = BlazorAssetProvider.GetLocalAddress(_runtime) + "Assets/";
            using HttpClient httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
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
            string baseAddress = BlazorAssetProvider.GetLocalAddress(_runtime) + "Assets/";
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };

            Diagnostics.Debug($"Requesting '{id}' at '{baseAddress}' ...");

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(id);
                response.EnsureSuccessStatusCode();
                byte[] stream = await response.Content.ReadAsByteArrayAsync();
                MemoryStream ms = new MemoryStream(stream); // copy to memory stream to prevent any loading issues
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

            string baseAddress = BlazorAssetProvider.GetLocalAddress(_runtime) + "Assets/";
            using HttpClient httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
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

            string baseAddress = BlazorAssetProvider.GetLocalAddress(_runtime) + "Assets/";
            using HttpClient httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
            HttpResponseMessage response = await httpClient.GetAsync(id).ConfigureAwait(false);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Loads an image from a given stream
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static ImageData LoadImage(Stream file)
        {
            try
            {
                using Image image = Image.Load(file, out SixLabors.ImageSharp.Formats.IImageFormat imgFormat);

                image.Mutate(x => x.AutoOrient());
                image.Mutate(x => x.RotateFlip(RotateMode.None, FlipMode.Vertical));


                int bpp = image.PixelType.BitsPerPixel;

                switch (image.PixelType.BitsPerPixel)
                {
                    case 16:
                        {
                            (image as Image<Rg32>).TryGetSinglePixelSpan(out Span<Rg32> res);
                            Span<byte> resBytes = MemoryMarshal.AsBytes<Rg32>(res.ToArray());
                            return new ImageData(resBytes.ToArray(), image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.Depth16));
                        }
                    case 24:
                        {
                            Image<Rgb24> rgb = image as Image<Rgb24>;
                            Image<Bgr24> bgr = rgb.CloneAs<Bgr24>();

                            bgr.TryGetSinglePixelSpan(out Span<Bgr24> res);
                            Span<byte> resBytes = MemoryMarshal.AsBytes<Bgr24>(res.ToArray());
                            return new ImageData(resBytes.ToArray(), image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.RGB));
                        }
                    case 32:
                        {
                            Image<Rgba32> rgba = image as Image<Rgba32>;
                            Image<Bgra32> bgra = rgba.CloneAs<Bgra32>();

                            bgra.TryGetSinglePixelSpan(out Span<Bgra32> res);
                            Span<byte> resBytes = MemoryMarshal.AsBytes<Bgra32>(res.ToArray());
                            return new ImageData(resBytes.ToArray(), image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.RGBA));
                        }
                    case 48:
                        {
                            Image<Rgba32> rgba = image as Image<Rgba32>;
                            Image<Bgra32> bgra = rgba.CloneAs<Bgra32>();

                            (image as Image<Rgb48>).TryGetSinglePixelSpan(out Span<Rgb48> res);
                            Span<byte> resBytes = MemoryMarshal.AsBytes<Rgb48>(res.ToArray());
                            return new ImageData(resBytes.ToArray(), image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.fRGB32));
                        }
                    case 64:
                        {
                            (image as Image<Rgba64>).TryGetSinglePixelSpan(out Span<Rgba64> res);
                            Span<byte> resBytes = MemoryMarshal.AsBytes<Rgba64>(res.ToArray());
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