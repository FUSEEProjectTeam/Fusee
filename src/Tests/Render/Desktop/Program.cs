using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Serialization;
using System;
using System.IO;
using Path = Fusee.Base.Common.Path;

namespace Fusee.Test.Render.Desktop
{
    public class Program
    {
        private const int height = 512;
        private const int width = 512;

        private static RenderCanvas example;

        public static RenderCanvas Example { get => example; set => example = value; }

        public static void Init(string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                // Inject Fusee.Engine.Base InjectMe dependencies
                IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();

                var fap = new Fusee.Base.Imp.Desktop.FileAssetProvider("Assets");
                fap.RegisterTypeHandler(
                    new AssetHandler
                    {
                        ReturnedType = typeof(Font),
                        Decoder = delegate (string id, object storage)
                        {
                            if (!Path.GetExtension(id).ToLower().Contains("ttf")) return null;
                            return new Font { _fontImp = new FontImp((Stream)storage) };
                        },
                        Checker = id => Path.GetExtension(id).ToLower().Contains("ttf")
                    });
                fap.RegisterTypeHandler(
                    new AssetHandler
                    {
                        ReturnedType = typeof(SceneContainer),
                        Decoder = delegate (string id, object storage)
                        {
                            if (!Path.GetExtension(id).ToLower().Contains("fus")) return null;
                            return Serializer.DeserializeSceneContainer((Stream)storage);
                        },
                        Checker = id => Path.GetExtension(id).ToLower().Contains("fus")
                    });

                AssetStorage.RegisterProvider(fap);

                var app = Example;

                // Inject Fusee.Engine InjectMe dependencies (hard coded)
                var cimp = new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasImp(width, height);
                cimp.EnableBlending = true;
                app.CanvasImplementor = cimp;
                app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp(cimp);
                Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(cimp));
                Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(cimp));

                // Initialize canvas/app and canvas implementor
                app.DoInit();

                // Render a single frame and save it
                var bmp = cimp.ShootCurrentFrame(width, height);
                bmp.Save(arg, System.Drawing.Imaging.ImageFormat.Png);

                // Done
                Console.Error.WriteLine($"SUCCESS: Image {arg} generated.");
            }
        }
    }
}