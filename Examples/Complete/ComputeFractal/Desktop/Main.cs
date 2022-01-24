using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Serialization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Fusee.Examples.ComputeFractal.Desktop
{
    public class ComputeFractal
    {
        public static void Main()
        {
            // Inject Fusee.Engine.Base InjectMe dependencies
            IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();

            var fap = new Fusee.Base.Imp.Desktop.FileAssetProvider("Assets");
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

            var app = new Core.ComputeFractal();

            // Inject Fusee.Engine InjectMe dependencies (hard coded)
            app.CanvasImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasImp();
            app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp(app.CanvasImplementor);
            Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(app.CanvasImplementor));
            Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(app.CanvasImplementor));
            // app.InputImplementor = new Fusee.Engine.Imp.Graphics.Desktop.InputImp(app.CanvasImplementor);
            // app.AudioImplementor = new Fusee.Engine.Imp.Sound.Desktop.AudioImp();
            // app.NetworkImplementor = new Fusee.Engine.Imp.Network.Desktop.NetworkImp();
            // app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Desktop.InputDriverImp();
            // app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

            app.InitApp();

            // Start the app
            app.Run();
        }
    }
}