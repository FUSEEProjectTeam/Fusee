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

namespace Fusee.Tests.SLIRP.SLIRPTest
{
    public static class Program
    {
        private const int height = 512;
        private const int width = 512;

        public static RenderCanvas Example { get; set; }

        public static string FilePath;

        public static void Init(string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                // Inject Fusee.Engine.Base InjectMe dependencies
                IO.IOImp = new IOImp();
                AssetStorage.Instance.Dispose();

                var baseDirOfExample = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));
                FilePath = baseDirOfExample.LocalPath;

                var fap = new FileAssetProvider(Path.Combine(baseDirOfExample.LocalPath, "Assets"));
                fap.RegisterTypeHandler(
                    new AssetHandler
                    {
                        ReturnedType = typeof(Font),
                        DecoderAsync = async (id, storage) =>
                        {
                            if (!Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)) return null;
                            return await Task.Run(() => new Font { _fontImp = new FontImp((Stream)storage) });
                        },
                        Decoder = (id, storage) =>
                        {
                            if (!Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)) return null;
                            return new Font { _fontImp = new FontImp((Stream)storage) };
                        },

                        Checker = id => Path.GetExtension(id).Contains("ttf", StringComparison.OrdinalIgnoreCase)
                    });
                fap.RegisterTypeHandler(
                    new AssetHandler
                    {
                        ReturnedType = typeof(SceneContainer),
                        DecoderAsync = async (id, storage) =>
                        {
                            if (!Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)) return null;
                            return await FusSceneConverter.ConvertFromAsync(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                        },
                        Decoder = (id, storage) =>
                        {
                            if (!Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)) return null;
                            return FusSceneConverter.ConvertFrom(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                        },
                        Checker = id => Path.GetExtension(id).Contains("fus", StringComparison.OrdinalIgnoreCase)
                    });
                AssetStorage.RegisterProvider(fap);

                var app = Example;

                // Inject Fusee.Engine InjectMe dependencies (hard coded)
                var cimp = new Engine.Imp.Graphics.Desktop.RenderCanvasImp(null, false, width, height, width, height)
                {
                    EnableBlending = true
                };
                app.CanvasImplementor = cimp;
                app.ContextImplementor = new Engine.Imp.Graphics.Desktop.RenderContextImp(cimp, Engine.Common.FuseePlatformId.Mesa);
                Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(cimp));
                Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(cimp));

                // Initialize canvas/app and canvas implementor
                app.InitApp();

                ((Engine.Imp.Graphics.Desktop.RenderCanvasImp)app.CanvasImplementor).DoInit();
                ((Engine.Imp.Graphics.Desktop.RenderCanvasImp)app.CanvasImplementor).DoResize(width, height);
                ((Engine.Imp.Graphics.Desktop.RenderCanvasImp)app.CanvasImplementor).DoUpdate();

                SpinWait.SpinUntil(() => app.IsLoaded);

                // skip the first frame, empty, skip the second as deferred needs three, second pass has an empty frame, too
                for (var i = 0; i < 3; i++)
                {
                    ((Engine.Imp.Graphics.Desktop.RenderCanvasImp)app.CanvasImplementor).DoRender();
                }

                // Render a single frame and save it
                using var img = cimp.ShootCurrentFrame(width, height) as Image<Bgra32>;
                img.SaveAsPng(Path.Combine(FilePath, arg));

                // Done
                Console.Error.WriteLine($"SUCCESS: Image {Path.Combine(FilePath, arg)} generated.");

                app.CloseGameWindow();
            }
        }
    }
}
