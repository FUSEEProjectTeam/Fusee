using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.WebAsm;
using Fusee.Engine.Core;
using Fusee.Engine.Imp.Graphics.WebAsm;
using Fusee.Serialization;
using Fusee.Xene;
using SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using Path = Fusee.Base.Common.Path;


namespace Fusee.Engine.Player.Main
{
    internal static class Program
    {
        public static void Main()
        {
            // This method takes care of everything
            WebAsmProgram.Start(new Main());
        }
    }

    public class Main : WebAsmBase
    {
        private RenderCanvasImp _canvasImp;
        private Core.Player _app;

        public override void Run()
        {
            base.Run();

            // Inject Fusee.Engine.Base InjectMe dependencies
            IO.IOImp = new Fusee.Base.Imp.WebAsm.IOImp();


            var fap = new Fusee.Base.Imp.WebAsm.AssetProvider();
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Font),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (Path.GetExtension(id).IndexOf("ttf", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var font = new Font
                            {
                                _fontImp = await Task.Factory.StartNew(() => new FontImp((Stream)storage)).ConfigureAwait(false)
                            };
                            
                            return font;
                        }

                        return null;
                    },
                    Checker = (string id) =>
                    {
                        return Path.GetExtension(id).IndexOf("ttf", System.StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                });
            
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(SceneContainer),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (Path.GetExtension(id).IndexOf("fus", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var storageStream = (Stream)storage;
                            return null;
                            //return await Task.Factory.StartNew(() => new ConvertSceneGraph().Convert(ser.Deserialize(storageStream, null, typeof(SceneContainer)) as SceneContainer)).ConfigureAwait(false);
                        }
                        return null;
                    },
                    Checker = (string id) =>
                    {
                        return Path.GetExtension(id).IndexOf("fus", System.StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                });

            // Image handler
            fap.RegisterTypeHandler(new AssetHandler
            {
                ReturnedType = typeof(Base.Core.ImageData),
                DecoderAsync = async (string id, object storage) =>
                {
                    var ext = Path.GetExtension(id).ToLower();
                    switch (ext)
                    {
                        //case ".jpg": // not possible YET!
                       // case ".jpeg":
                        case ".png":
                        case ".bmp":
                            // handle file
                            Console.WriteLine("Found image, processing");

                            using (var bitmap = await Task<SKBitmap>.Factory.StartNew(() => SKBitmap.Decode((Stream)storage)).ConfigureAwait(false))
                            {
                               var rotated = new SKBitmap(bitmap.Width, bitmap.Height, true);
                              
                                using (var surface = new SKCanvas(rotated))
                                {
                                    surface.Clear();
                                    surface.Scale(1, -1, 0, bitmap.Height / 2.0f); // this mirrors the image within its' x-axis
                                    surface.DrawBitmap(bitmap, 0, 0);
                                }
                                Console.WriteLine($"Found image, {rotated.Width}, {rotated.Height}");

                                return new Base.Core.ImageData(rotated.Width, rotated.Height)
                                {
                                    PixelData = rotated.Bytes
                                };
                            }
                    }
                    return null;
                },
                Checker = (string id) =>
                {
                    var ext = Path.GetExtension(id).ToLower();
                    switch (ext)
                    {
                       // case ".jpg":
                       // case ".jpeg":
                        case ".png":
                        case ".bmp":
                            return true;
                    }
                    return false;
                }
            });

            AssetStorage.RegisterProvider(fap);

            _app = new Core.Player();

            // Inject Fusee.Engine InjectMe dependencies (hard coded)
            _canvasImp = new RenderCanvasImp(canvas, gl, canvasWidth, canvasHeight);
            _app.CanvasImplementor = _canvasImp;
            _app.ContextImplementor = new RenderContextImp(_app.CanvasImplementor);
            Input.AddDriverImp(new RenderCanvasInputDriverImp(_app.CanvasImplementor));

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
