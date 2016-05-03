using System.IO;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Core;
using Fusee.Serialization;
using Path = Fusee.Base.Common.Path;

namespace Fusee.Tutorial.Desktop
{
    public class Simple
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
                    Decoder = delegate (string id, object storage)
                    {
                        if (!Path.GetExtension(id).ToLower().Contains("ttf")) return null;
                        return new Font{ _fontImp = new FontImp((Stream)storage) };
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
                        var ser = new Serializer();
                        return ser.Deserialize((Stream)storage, null, typeof(SceneContainer)) as SceneContainer;
                    },
                    Checker = id => Path.GetExtension(id).ToLower().Contains("fus")
                });

            AssetStorage.RegisterProvider(fap);

            var app = new Core.Tutorial();

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

            // Start the app
            app.Run();
        }
    }
}
