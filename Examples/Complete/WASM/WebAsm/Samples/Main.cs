using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.WebAsm;
using Fusee.Engine.Core;
using Fusee.Engine.Imp.Graphics.WebAsm;
using Fusee.Serialization;
using FileMode = Fusee.Base.Common.FileMode;
using Path = Fusee.Base.Common.Path;
using Fusee.Base.Imp.WebAsm;

namespace Fusee.Examples.RocketOnly.Main
{
    internal class Program
    {
        public static void Main()
        {   
            // This method takes care of everything
            WebAsmProgram.Start((new Main()));
        }
    } 

    public class Main : WebAsmBase
    {     
        private RenderCanvasImp _canvasImp;
        private Core.RocketOnly _app;


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
                    Decoder = delegate (string id, object storage)
                    {
                        if (Path.GetExtension(id).ToLower().Contains("ttf"))
                        {
                            return new Font
                            {
                                _fontImp = new Fusee.Base.Imp.WebAsm.FontImp(/* storage */)
                            };
                        }

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
                    Decoder = delegate (string id, object storage)
                    {
                        if (Path.GetExtension(id).ToLower().Contains("fus"))
                        {
                            var ser = new Serializer();
                            return new ConvertSceneGraph().Convert(ser.Deserialize(IO.StreamFromFile("Assets/" + id, FileMode.Open), null, typeof(SceneContainer)) as SceneContainer);
                        }
                        return null;
                    },
                    Checker = delegate (string id)
                    {
                        return Path.GetExtension(id).ToLower().Contains("fus");
                    }
                });
            AssetStorage.RegisterProvider(fap);

            _app = new Fusee.Examples.RocketOnly.Core.RocketOnly();

        
            // Inject Fusee.Engine InjectMe dependencies (hard coded)
            _canvasImp = new Fusee.Engine.Imp.Graphics.WebAsm.RenderCanvasImp(canvas, gl, canvasWidth, canvasHeight);
            _app.CanvasImplementor = _canvasImp;
            _app.ContextImplementor = new Fusee.Engine.Imp.Graphics.WebAsm.RenderContextImp(_app.CanvasImplementor);
            Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.WebAsm.RenderCanvasInputDriverImp(_app.CanvasImplementor));
            // app.AudioImplementor = new Fusee.Engine.Imp.Sound.Web.AudioImp();
            // app.NetworkImplementor = new Fusee.Engine.Imp.Network.Web.NetworkImp();
            // app.InputDriverImplementor = new Fusee.Engine.Imp.Input.Web.InputDriverImp();
            // app.VideoManagerImplementor = ImpFactory.CreateIVideoManagerImp();

            // Start the app
            _app.Run();

            LoadRocket();

        }
        private async void LoadRocket()
        {        
            var stream = await WasmResourceLoader.LoadAsync("Assets/FUSEERocket.fus", WasmResourceLoader.GetLocalAddress());
            var seri = new Serializer();
            var scene = new ConvertSceneGraph().Convert(seri.Deserialize(stream, null, typeof(SceneContainer)) as SceneContainer);
            _app.RocketScene = scene;
        }

        public override void Update(double elapsedMilliseconds)
        {
            if (_canvasImp != null)
                _canvasImp.DeltaTime = (float)(elapsedMilliseconds / 1000.0);
        }

        public override void Draw()
        {
            if (_canvasImp != null)
                _canvasImp.DoRender();
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);
            _canvasImp.DoResize(width, height);
        }
    }
}
