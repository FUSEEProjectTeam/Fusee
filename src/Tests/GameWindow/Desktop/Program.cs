using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Serialization;
using System.IO;
using System.Threading.Tasks;

namespace Fusee.Tests.GameWindow.Desktop
{
    public class Program
    {
        private const int height = 512;
        private const int width = 512;

        private static RenderCanvas example;

        public static RenderCanvas Example { get => example; set => example = value; }

        public static void Init()
        {
            // Inject Fusee.Engine.Base InjectMe dependencies
            IO.IOImp = new IOImp();

            var fap = new FileAssetProvider("Assets");
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Font),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)) return null;
                        return await Task.FromResult(new Font { _fontImp = new FontImp((Stream)storage) });
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
            var cimp = new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasImp(width, height)
            {
                EnableBlending = true
            };
            app.CanvasImplementor = cimp;
            app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp(cimp);

            // Initialize canvas/app and canvas implementor
            app.InitApp();
            app.Init();
        }
    }
}