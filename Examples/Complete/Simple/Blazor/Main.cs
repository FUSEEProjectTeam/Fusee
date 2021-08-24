﻿using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.WebAsm;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Imp.Graphics.WebAsm;
using Fusee.Serialization;
using ProtoBuf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime.InteropServices;
using Path = System.IO.Path;

namespace Fusee.Examples.Simple.WebAsm
{
    public class Main : WebAsmBase
    {
        private RenderCanvasImp _canvasImp;
        private Core.Simple _app;

        public override void Run()
        {
            // Disable colored console ouput, not supported
            Diagnostics.UseConsoleColor(false);
            Diagnostics.SetMinDebugOutputLoggingSeverityLevel(Diagnostics.SeverityLevel.Verbose);

            // Disable text logging as this is not supported for platform: web
            //Diagnostics.SetMinTextFileLoggingSeverityLevel(Diagnostics.SeverityLevel.None);


            base.Run();

            // Inject Fusee.Engine.Base InjectMe dependencies
            IO.IOImp = new Fusee.Base.Imp.WebAsm.IOImp();

            #region FAP

            var fap = new Fusee.Base.Imp.WebAsm.AssetProvider(Runtime);
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Base.Core.Font),
                    Decoder = (_, __) => throw new NotImplementedException("Non-async decoder isn't supported in WebAsmBuilds"),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase))
                        {
                            var font = new Base.Core.Font
                            {
                                _fontImp = new FontImp((Stream)storage)
                            };

                            return font;
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
                    Decoder = (_, __) => throw new NotImplementedException("Non-async decoder isn't supported in WebAsmBuilds"),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (Path.GetExtension(id).IndexOf("fus", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return await FusSceneConverter.ConvertFrom(Serializer.Deserialize<FusFile>((Stream)storage));
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
                Decoder = (_, __) => throw new NotImplementedException("Non-async decoder isn't supported in WebAsmBuilds"),
                DecoderAsync = async (string id, object storage) =>
                {
                    var ext = Path.GetExtension(id).ToLower();

                    try
                    {
                        using var image = await Image.LoadAsync<Rgba32>((Stream)storage);

                        image.Mutate(x => x.AutoOrient());
                        image.Mutate(x => x.RotateFlip(RotateMode.None, FlipMode.Vertical));
                        var ret = new ImageData(ReadPixels(image), image.Width, image.Height,
                                new ImagePixelFormat(ColorFormat.RGBA));

                        return ret;

                        // inner method to prevent Span<T> inside async method error
                        static byte[] ReadPixels(Image<Rgba32> image)
                        {
                            image.TryGetSinglePixelSpan(out var res);
                            var resBytes = MemoryMarshal.AsBytes<Rgba32>(res.ToArray());
                            return resBytes.ToArray();
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading/converting image {id}", ex);
                        Diagnostics.Error($"Error loading/converting image {id}", ex);
                    }

                    // return empty 1x1 image
                    return new ImageData(new byte[] { 0, 0, 0, 1, 0, 0, 0, 1 }, 1, 1,
                                new ImagePixelFormat(ColorFormat.RGBA));
                },
                Checker = (string id) =>
                {
                    var ext = Path.GetExtension(id).ToLower();
                    return true;
                }
            });

            AssetStorage.RegisterProvider(fap);

            #endregion

            _app = new Core.Simple();

            // Inject Fusee.Engine InjectMe dependencies (hard coded)
            _canvasImp = new RenderCanvasImp(canvas, Runtime, gl, canvasWidth, canvasHeight);
            _app.CanvasImplementor = _canvasImp;
            _app.ContextImplementor = new RenderContextImp(_app.CanvasImplementor);
            Input.AddDriverImp(new RenderCanvasInputDriverImp(_app.CanvasImplementor, Runtime));

            // Start the app
            _app.Run();
        }

        public override void Update(double elapsedMilliseconds)
        {
            if (_canvasImp != null)
                _canvasImp.DeltaTime = (float)(elapsedMilliseconds / 1000.0);
        }

        public override void Draw()
        {
            _canvasImp?.DoRender();
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);
            _canvasImp.DoResize(width, height);
        }
    }
}