using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Serialization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Fusee.Tests.Render.Desktop
{

    public class Program
    {
        private const int height = 512;
        private const int width = 512;

        private static RenderCanvas example;

        public static RenderCanvas Example { get => example; set => example = value; }

        public async static void Init(string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                // Inject Fusee.Engine.Base InjectMe dependencies
                IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();
                AssetStorage.UnRegisterAllAssetProviders();
                var fap = new Fusee.Base.Imp.Desktop.FileAssetProvider("Assets");
                fap.RegisterTypeHandler(
                    new AssetHandler
                    {
                        ReturnedType = typeof(Font),
                        DecoderAsync = async (string id, object storage) =>
                        {
                            if (!Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)) return null;
                            return await Task.Run(() => new Font { _fontImp = new FontImp((Stream)storage) });
                        },
                        Decoder = (string id, object storage) =>
                        {
                            if (!Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)) return null;
                            return new Font { _fontImp = new FontImp((Stream)storage) };
                        },

                        Checker = id => Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)
                    });
                fap.RegisterTypeHandler(
                    new AssetHandler
                    {
                        ReturnedType = typeof(SceneContainer),
                        DecoderAsync = async (string id, object storage) =>
                        {
                            if (!Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)) return null;
                            return await FusSceneConverter.ConvertFromAsync(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                        },
                        Decoder = (string id, object storage) =>
                        {
                            if (!Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)) return null;
                            return FusSceneConverter.ConvertFrom(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                        },
                        Checker = id => Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)
                    });
                AssetStorage.RegisterProvider(fap);

                var app = Example;

                // Inject Fusee.Engine InjectMe dependencies (hard coded)
                var cimp = new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasImp()
                {
                    EnableBlending = true
                };
                app.CanvasImplementor = cimp;
                app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp(cimp);
                Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(cimp));
                Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(cimp));

                // Initialize canvas/app and canvas implementor
                app.InitApp();
                cimp.Height = height;
                cimp.Width = width;

                app.RC.SetRenderStateSet(RenderStateSet.Default);

                // Render a single frame and save it
                using var img = cimp.ShootCurrentFrame(width, height) as Image<Rgba32>;
                img.SaveAsPng(arg);

                // Done
                Console.Error.WriteLine($"SUCCESS: Image {arg} generated.");

                app.CloseGameWindow();
            }
        }
    }
}