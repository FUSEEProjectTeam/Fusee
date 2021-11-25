using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Blazor;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Imp.Graphics.Blazor;
using Fusee.Serialization;
using Microsoft.JSInterop;
using ProtoBuf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Path = System.IO.Path;
using Stream = System.IO.Stream;

namespace Fusee.Engine.Player.Blazor
{
    public class Main : BlazorBase
    {
        private RenderCanvasImp _canvasImp;
        private RenderCanvas app2Inject;

        public async override void Run()
        {
            Console.WriteLine("Starting Blazor program");

            // Disable colored console ouput, not supported
            Diagnostics.UseConsoleColor(false);
            Diagnostics.SetMinDebugOutputLoggingSeverityLevel(Diagnostics.SeverityLevel.Verbose);

            base.Run();

            // Inject Fusee.Engine.Base InjectMe dependencies
            Base.Core.IO.IOImp = new Fusee.Base.Imp.Blazor.IO();

            #region FAP

            var fap = new AssetProvider(Runtime);
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Base.Core.Font),
                    Decoder = (_, __) => throw new System.NotImplementedException("Non-async decoder isn't supported in Blazor builds"),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase))
                        {
                            var font = new Base.Core.Font
                            {
                                _fontImp = new FontImp((Stream)storage)
                            };

                            return await Task.FromResult(font);
                        }

                        return null;
                    },
                    Checker = (string id) =>
                    {
                        return Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase);
                    }
                });

            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(SceneContainer),
                    Decoder = (_, __) => throw new System.NotImplementedException("Non-async decoder isn't supported in Blazor builds"),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (Path.GetExtension(id).IndexOf("fus", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return await FusSceneConverter.ConvertFromAsync(Serializer.Deserialize<FusFile>((System.IO.Stream)storage));
                        }
                        return null;
                    },
                    Checker = (string id) =>
                    {
                        return Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase);
                    }
                });

            // Image handler
            fap.RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(Base.Core.ImageData),
                Decoder = (_, __) => throw new NotImplementedException("Non-async decoder isn't supported in Blazor builds"),
                DecoderAsync = async (string id, object storage) =>
                {
                    var ext = Path.GetExtension(id).ToLower();

                    try
                    {
                        //using var ms = new MemoryStream();
                        //((Stream)storage).CopyTo(ms);
                        using var image = await Image.LoadAsync((Stream)storage);

                        image.Mutate(x => x.AutoOrient());
                        image.Mutate(x => x.RotateFlip(RotateMode.None, FlipMode.Vertical));
                        var ret = ReadPixels(image);

                        return ret;

                        // inner method to prevent Span<T> inside async method error
                        static ImageData ReadPixels(Image image)
                        {
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

                                        rgb.TryGetSinglePixelSpan(out var res);
                                        var resBytes = MemoryMarshal.AsBytes<Rgb24>(res.ToArray());
                                        return new ImageData(resBytes.ToArray(), image.Width, image.Height,
                                            new ImagePixelFormat(ColorFormat.RGB));
                                    }
                                case 32:
                                    {
                                        var rgba = image as Image<Rgba32>;

                                        rgba.TryGetSinglePixelSpan(out var res);
                                        var resBytes = MemoryMarshal.AsBytes<Rgba32>(res.ToArray());
                                        return new ImageData(resBytes.ToArray(), image.Width, image.Height,
                                            new ImagePixelFormat(ColorFormat.RGBA));
                                    }
                                case 48:
                                    {
                                        var rgba = image as Image<Rgba32>;

                                        rgba.TryGetSinglePixelSpan(out var res);
                                        var resBytes = MemoryMarshal.AsBytes<Rgba32>(res.ToArray());
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
                                    {
                                        Console.WriteLine($"Error converting! {bpp}");

                                        throw new ArgumentException($"{bpp} Bits per pixel not supported!");

                                    }
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading/converting image {id} {ex}");
                        Diagnostics.Error($"Error loading/converting image {id}", ex);

                        // return empty 1x1 image
                        return new ImageData(new byte[] { 0, 0, 0, 1, 0, 0, 0, 1 }, 1, 1,
                                    new ImagePixelFormat(ColorFormat.RGBA));

                    }

                },
                Checker = (string id) =>
                {
                    var ext = Path.GetExtension(id).ToLower();
                    return true;
                }
            });

            AssetStorage.RegisterProvider(fap);

            #endregion

            try
            {
                var baseAddress = await BlazorAssetProvider.GetLocalAddress(Runtime);
                using var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
                var response = await httpClient.GetAsync("Fusee.App.dll");

                if (!response.IsSuccessStatusCode)
                    response = await httpClient.GetAsync("Fusee.Engine.Player.Core.dll");

                if (!response.IsSuccessStatusCode)
                {
                    throw new DllNotFoundException("Error loading. Neither Fusee.App.dll nor Fusee.Engine.Player.Core.dll found! Abort.");
                }

                var res = await response.Content.ReadAsByteArrayAsync();

                var assembly = Assembly.Load(res);

                var tApp = assembly.GetTypes().FirstOrDefault(t => typeof(RenderCanvas).IsAssignableFrom(t));
                app2Inject = (RenderCanvas)Activator.CreateInstance(tApp);

                // Inject Fusee.Engine InjectMe dependencies (hard coded)
                _canvasImp = new RenderCanvasImp(canvas, Runtime, gl, canvasWidth, canvasHeight);
                app2Inject.CanvasImplementor = _canvasImp;
                app2Inject.ContextImplementor = new RenderContextImp(app2Inject.CanvasImplementor);
                Input.AddDriverImp(new RenderCanvasInputDriverImp(app2Inject.CanvasImplementor, Runtime));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading DLL: {ex}");
            }

            app2Inject.LoadingCompleted += (s, e) =>
            {
                Console.WriteLine("Loading finished");
                ((IJSInProcessRuntime)Runtime).InvokeVoidAsync("LoadingFinished");
            };

            app2Inject.InitApp();

            // Start the app
            app2Inject.Run();
        }

        public override void Update(double elapsedMilliseconds)
        {
            if (_canvasImp != null)
                _canvasImp.DeltaTimeUpdate = (float)(elapsedMilliseconds / 1000.0);

            _canvasImp?.DoUpdate();
        }

        public override void Draw(double elapsedMilliseconds)
        {
            if (_canvasImp != null)
                _canvasImp.DeltaTime = (float)(elapsedMilliseconds / 1000.0);

            _canvasImp?.DoRender();
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);
            _canvasImp?.DoResize(width, height);
        }
    }
}