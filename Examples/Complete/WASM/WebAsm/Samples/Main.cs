using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.WebAsm;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Samples.Helpers;
using System;
using System.Reflection;
using WebAssembly;
using FileMode = Fusee.Base.Common.FileMode;
using Path = Fusee.Base.Common.Path;

namespace Samples
{
    internal class Program
    { 
        private static readonly float4 CanvasColor = new float4(255, 0, 255, 255);
        private static readonly Action<double> loop = new Action<double>(Loop);
        private static double previousMilliseconds;
        private static JSObject window;

        private static ISample sample;

        private static string divCanvasName;
        private static string canvasName;


        private static void Main()
        {
            // Let's first check if we can continue with WebGL2 instead of crashing.
            if (!isBrowserSupportsWebGL2())
            {
                HtmlHelper.AddParagraph("We are sorry, but your browser does not seem to support WebGL2.");
                return;
            }

            HtmlHelper.GetBrowserWindowSize();

            // Create our sample
            sample = new RocketOnlyWebAsm();
            var sampleName = sample.GetType().Name;

            divCanvasName = $"div_canvas_{sampleName}";
            canvasName = $"canvas_{sampleName}";

            using(var window = (JSObject)Runtime.GetGlobalObject("window"))
            {
                var windowWidth = (int)window.GetObjectProperty("innerWidth");
                var windowHeight = (int)window.GetObjectProperty("innerHeight");

                using (var canvas = HtmlHelper.AddCanvas(divCanvasName, canvasName, windowWidth, windowHeight))
                {
                    sample.Init(canvas, CanvasColor);
                    sample.Run();
                }
            }

            AddEnterFullScreenHandler();
            AddResizeHandler();

            RequestAnimationFrame();
        }

        private static void AddResizeHandler()
        {
            using (var window = (JSObject)Runtime.GetGlobalObject("window"))
            {
                window.Invoke("addEventListener", "resize", new Action<JSObject>((o) =>
                {
                    using (var d = (JSObject)Runtime.GetGlobalObject("document"))
                    using (var w = (JSObject)Runtime.GetGlobalObject("window"))
                    {                      
                        var canvasObject = (JSObject)d.Invoke("getElementById", canvasName);

                        var windowWidth = (int)w.GetObjectProperty("innerWidth");
                        var windowHeight = (int)w.GetObjectProperty("innerHeight");

                        var cobj = (string)canvasObject.GetObjectProperty("id");

                        canvasObject.SetObjectProperty("width", windowWidth);
                        canvasObject.SetObjectProperty("height", windowHeight);

                        Console.WriteLine($"{cobj}.Resize({windowWidth}, {windowHeight});");

                        // call fusee resize
                        sample.Resize(windowWidth, windowHeight);
                    }

                    o.Dispose();
                }), false);
            }
        }

        private static void RequestFullscreen(JSObject canvas)
        {
            if (canvas.GetObjectProperty("requestFullscreen") != null)
                canvas.Invoke("requestFullscreen");
            if (canvas.GetObjectProperty("mozRequestFullScreen") != null)
                canvas.Invoke("mozRequestFullScreen");
            if (canvas.GetObjectProperty("webkitRequestFullscreen") != null)
                canvas.Invoke("webkitRequestFullscreen");
            if (canvas.GetObjectProperty("msRequestFullscreen") != null)
                canvas.Invoke("msRequestFullscreen");
        }
        
        private static void AddEnterFullScreenHandler()
        {
            using (var canvas = (JSObject)Runtime.GetGlobalObject(canvasName))
            {
                canvas.Invoke("addEventListener", "dblclick", new Action<JSObject>((o) =>
                {
                    using (var d = (JSObject)Runtime.GetGlobalObject("document"))
                    {                      
                        var canvasObject = (JSObject)d.Invoke("getElementById", canvasName);

                        RequestFullscreen(canvasObject);                      

                        var width = (int)canvasObject.GetObjectProperty("clientWidth");
                        var height = (int)canvasObject.GetObjectProperty("clientHeight");

                        SetNewCanvasSize(canvasObject, width, height);

                        // call fusee resize
                        sample.Resize(width, height);
                    }

                    o.Dispose();
                }), false);
            }
        }

        private static void SetNewCanvasSize(JSObject canvasObject, int newWidth, int newHeight)
        {
            canvasObject.SetObjectProperty("width", newWidth);
            canvasObject.SetObjectProperty("height", newHeight);
        }


        private static void Loop(double milliseconds)
        {
            var elapsedMilliseconds = milliseconds - previousMilliseconds;
            previousMilliseconds = milliseconds;

            sample.Update(elapsedMilliseconds);
            sample.Draw();

            RequestAnimationFrame();
        }

        private static void RequestAnimationFrame()
        {
            if (window == null)
            {
                window = (JSObject)Runtime.GetGlobalObject();
            } 

            window.Invoke("requestAnimationFrame", loop);
        }

        private static bool isBrowserSupportsWebGL2()
        {
            if (window == null)
            {
                window = (JSObject)Runtime.GetGlobalObject();
            }

            // This is a very simple check for WebGL2 support.
            return window.GetObjectProperty("WebGL2RenderingContext") != null;
        }


        public class RocketOnlyWebAsm : BaseSample
        {
            public override string Description =>
                "Complete <a href=\"https://fusee3d.org\">FUSEE</a> Example with experimental RenderCanvas/RenderContext implementation.";

            private Fusee.Engine.Imp.Graphics.WebAsm.RenderCanvasImp _canvasImp;
            private Fusee.Examples.RocketOnly.Core.RocketOnly _app;


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

                Console.WriteLine("[TEST]");

                /*var task = WasmResourceLoader.LoadAsync("Assets/FUSEERocket.fus", WasmResourceLoader.GetLocalAddress());
                Console.WriteLine("[1] " + task);
                task.Wait();
                Console.WriteLine("[2] task finished");
                var seri = new Serializer();
                app._rocketScene = new ConvertSceneGraph().Convert(seri.Deserialize(task.Result, null, typeof(SceneContainer)) as SceneContainer);
                Console.WriteLine("[3] " + app._rocketScene);
                */
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

                var vert = await WasmResourceLoader.LoadAsync("Assets/VertexShader.vert", WasmResourceLoader.GetLocalAddress());
                var frag = await WasmResourceLoader.LoadAsync("Assets/PixelShader.frag", WasmResourceLoader.GetLocalAddress());

                var img = await WasmResourceLoader.LoadAsync("Assets/FuseeText.png", WasmResourceLoader.GetLocalAddress());

                var readerVert = new System.IO.StreamReader(vert);
                var readerFrag = new System.IO.StreamReader(frag);

                _app.VertexShader = await readerVert.ReadToEndAsync();
                _app.PixelShader = await readerFrag.ReadToEndAsync();

                _app.CurrentTex = FileDecoder.LoadImage(img).Result;

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
}
