using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Serialization;

namespace Fusee.Tutorial.Web
{
    public class Tutorial
    {
        public static void Main()
        {
            // Inject Fusee.Engine.Base InjectMe dependencies
            IO.IOImp = new Fusee.Base.Imp.Web.IOImp();

            var fap = new Fusee.Base.Imp.Web.WebAssetProvider();
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Font),
                    Decoder = delegate (string id, object storage)
                    {
                        if (Path.GetExtension(id).ToLower().Contains("ttf"))
                            return new Font
                            {
                                _fontImp = new Fusee.Base.Imp.Web.FontImp(storage)
                            };
                        return null;
                    },
                    Checker = delegate (string id)
                    {
                        return Path.GetExtension(id).ToLower().Contains("ttf");
                    }
                });
            fap.RegisterTypeHandler( 
                new AssetHandler
                {
                    ReturnedType = typeof(SceneContainer),
                    Decoder = delegate(string id, object storage)
                    {
                        if (Path.GetExtension(id).ToLower().Contains("fus"))
                        {
                            var ser = new Serializer();
                            return ser.Deserialize(IO.StreamFromFile("Assets/" + id, FileMode.Open), null, typeof(SceneContainer)) as SceneContainer;
                        }
                        return null;
                    },
                    Checker = delegate (string id)
                    {
                        return Path.GetExtension(id).ToLower().Contains("fus");
                    }
                });
            AssetStorage.RegisterProvider(fap);

            var app = new Fusee.Tutorial.Core.Tutorial();

            // Inject Fusee.Engine InjectMe dependencies (hard coded)
            app.CanvasImplementor = new Fusee.Engine.Imp.Graphics.Web.RenderCanvasImp();
            app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Web.RenderContextImp(app.CanvasImplementor);
            Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Web.RenderCanvasInputDriverImp(app.CanvasImplementor));
            // app.AudioImplementor = new Fusee.Engine.Imp.Sound.Web.AudioImp();
            // app.NetworkImplementor = new Fusee.Engine.Imp.Network.Web.NetworkImp();
            // app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Web.InputDriverImp();
            // app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

            // Start the app
            app.Run();
        }
    }
}
