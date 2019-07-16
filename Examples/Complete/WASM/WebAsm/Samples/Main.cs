using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.WebAsm;
using Fusee.Engine.Core;
using Fusee.Engine.Imp.Graphics.WebAsm;
using Fusee.Examples.RocketOnly.Main.Helpers;
using Fusee.Math.Core;
using Fusee.Serialization;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using WebAssembly;
using FileMode = Fusee.Base.Common.FileMode;
using Path = Fusee.Base.Common.Path;

namespace Fusee.Examples.RocketOnly.Main
{
    internal class Program
    {
        private static readonly float4 CanvasColor = new float4(255, 0, 255, 255);
        private static readonly Action<double> loop = new Action<double>(Loop);
        private static double previousMilliseconds;
        private static JSObject window;

        private static Main mainExecutable;

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
            mainExecutable = new Main();
            var sampleName = mainExecutable.GetType().Name;

            divCanvasName = $"div_canvas_{sampleName}";
            canvasName = $"canvas_{sampleName}";

            using (var window = (JSObject)Runtime.GetGlobalObject("window"))
            {
                var windowWidth = (int)window.GetObjectProperty("innerWidth");
                var windowHeight = (int)window.GetObjectProperty("innerHeight");

                using (var canvas = HtmlHelper.AddCanvas(divCanvasName, canvasName, windowWidth, windowHeight))
                {
                    mainExecutable.Init(canvas, CanvasColor);
                    mainExecutable.Run();
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
                        mainExecutable.Resize(windowWidth, windowHeight);
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
                        mainExecutable.Resize(width, height);
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

            mainExecutable.Update(elapsedMilliseconds);
            mainExecutable.Draw();

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
    }
    public abstract class WebAsmMainBase
    {
        protected WebGLRenderingContextBase gl;
        protected float4 clearColor;
        protected JSObject canvas;
        protected int canvasWidth;
        protected int canvasHeight;

        public virtual string Description => string.Empty;

        public virtual bool EnableFullScreen => true;

        public virtual void Init(JSObject canvas, float4 clearColor)
        {
            this.clearColor = clearColor;
            this.canvas = canvas;

            canvasWidth = (int)canvas.GetObjectProperty("width");
            canvasHeight = (int)canvas.GetObjectProperty("height");

            var webglContextAttrib = new JSObject();
            webglContextAttrib.SetObjectProperty("alpha", false);
            gl = new Fusee.Engine.Imp.Graphics.WebAsm.WebGL2RenderingContext(canvas, webglContextAttrib);
        }

        public virtual void Run()
        {
        }

        public virtual void Update(double elapsedMilliseconds)
        {
        }

        public virtual void Draw()
        {
            gl.Enable(WebGLRenderingContextBase.DEPTH_TEST);

            gl.Viewport(0, 0, canvasWidth, canvasHeight);

            gl.ClearColor(clearColor.x, clearColor.y, clearColor.z, clearColor.w);
            gl.Clear(WebGLRenderingContextBase.COLOR_BUFFER_BIT);
        }

        public virtual void Resize(int width, int height)
        {
            canvasWidth = width;
            canvasHeight = height;
        }
    }

    public class Main : WebAsmMainBase
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

    public static class GLExtensions
    {
        public static WebGLBuffer CreateArrayBufferWithUsage(this WebGLRenderingContextBase gl, Array items, uint usage)
        {
            var arrayBuffer = gl.CreateBuffer();
            gl.BindBuffer(WebGLRenderingContextBase.ARRAY_BUFFER, arrayBuffer);
            gl.BufferData(WebGLRenderingContextBase.ARRAY_BUFFER, items, usage);

            return arrayBuffer;
        }

        public static WebGLBuffer CreateArrayBuffer(this WebGLRenderingContextBase gl, Array items)
        {
            var arrayBuffer = gl.CreateBuffer();
            gl.BindBuffer(WebGLRenderingContextBase.ARRAY_BUFFER, arrayBuffer);
            gl.BufferData(WebGLRenderingContextBase.ARRAY_BUFFER, items, WebGLRenderingContextBase.STATIC_DRAW);
            gl.BindBuffer(WebGLRenderingContextBase.ARRAY_BUFFER, null);

            return arrayBuffer;
        }

        public static WebGLBuffer CreateElementArrayBuffer(this WebGLRenderingContextBase gl, Array items)
        {
            var elementArrayBuffer = gl.CreateBuffer();
            gl.BindBuffer(WebGLRenderingContextBase.ELEMENT_ARRAY_BUFFER, elementArrayBuffer);
            gl.BufferData(WebGLRenderingContextBase.ELEMENT_ARRAY_BUFFER, items, WebGLRenderingContextBase.STATIC_DRAW);
            gl.BindBuffer(WebGLRenderingContextBase.ELEMENT_ARRAY_BUFFER, null);

            return elementArrayBuffer;
        }

        public static WebGLProgram InitializeShaders(this WebGLRenderingContextBase gl, string vertexShaderCode, string fragmentShaderCode)
        {
            var shaderProgram = gl.CreateProgram();

            var vertexShader = GetShader(gl, vertexShaderCode, WebGLRenderingContextBase.VERTEX_SHADER);
            var fragmentShader = GetShader(gl, fragmentShaderCode, WebGLRenderingContextBase.FRAGMENT_SHADER);

            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);

            gl.LinkProgram(shaderProgram);

            gl.UseProgram(shaderProgram);

            return shaderProgram;
        }

        public static WebGLShader GetShader(this WebGLRenderingContextBase gl, string shaderSource, uint type)
        {
            var shader = gl.CreateShader(type);
            gl.ShaderSource(shader, shaderSource);
            gl.CompileShader(shader);

            var message = gl.GetShaderInfoLog(shader);
            if (message.Length > 0)
            {
                var msg = $"Shader Error: {message}";
                throw new Exception(msg);
            }

            return shader;
        }
    }

    public static class EmbeddedResourceHelper
    {
        public static Stream Load(string resourceName, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetExecutingAssembly();
            }

            var fullResourceName = assembly
                .GetManifestResourceNames()
                .First(resource => resource.EndsWith(resourceName));

            return assembly.GetManifestResourceStream(fullResourceName);
        }
    }
}


namespace Fusee.Examples.RocketOnly.Main.Helpers
{
    public static class HtmlHelper
    {
        public static void GetBrowserWindowSize()
        {
            using (var document = (JSObject)Runtime.GetGlobalObject("document"))
            using (var body = (JSObject)document.GetObjectProperty("body"))
            {
                Console.WriteLine($"Body width [{body.GetObjectProperty("width")}]");
            }

        }

        public static JSObject AddCanvas(string divId, string canvasId, int width = 800, int height = 600)
        {
            using (var document = (JSObject)Runtime.GetGlobalObject("document"))
            using (var body = (JSObject)document.GetObjectProperty("body"))
            {

                var canvas = (JSObject)document.Invoke("createElement", "canvas");
                canvas.SetObjectProperty("width", width);
                canvas.SetObjectProperty("height", height);
                canvas.SetObjectProperty("id", canvasId);

                using (var canvasDiv = (JSObject)document.Invoke("createElement", "div"))
                {
                    canvasDiv.SetObjectProperty("id", divId);
                    canvasDiv.Invoke("appendChild", canvas);

                    body.Invoke("appendChild", canvasDiv);
                }

                return canvas;
            }
        }

        public static void AddHeader(int headerIndex, string text)
        {
            using (var document = (JSObject)Runtime.GetGlobalObject("document"))
            using (var body = (JSObject)document.GetObjectProperty("body"))
            using (var header = (JSObject)document.Invoke("createElement", $"h{headerIndex}"))
            using (var headerText = (JSObject)document.Invoke("createTextNode", text))
            {
                header.Invoke("appendChild", headerText);
                body.Invoke("appendChild", header);
            }
        }

        public static void AddHeader1(string text) => AddHeader(1, text);

        public static void AddHeader2(string text) => AddHeader(2, text);

        public static void AddParagraph(string text)
        {
            using (var document = (JSObject)Runtime.GetGlobalObject("document"))
            using (var body = (JSObject)document.GetObjectProperty("body"))
            using (var paragraph = (JSObject)document.Invoke("createElement", "p"))
            {
                paragraph.SetObjectProperty("innerHTML", text);
                body.Invoke("appendChild", paragraph);
            }
        }

        public static void AddButton(string id, string text)
        {
            using (var document = (JSObject)Runtime.GetGlobalObject("document"))
            using (var body = (JSObject)document.GetObjectProperty("body"))
            using (var button = (JSObject)document.Invoke("createElement", "button"))
            {
                button.SetObjectProperty("innerHTML", text);
                button.SetObjectProperty("id", id);
                body.Invoke("appendChild", button);
            }
        }

        public static void AttachButtonOnClickEvent(string id, Action<JSObject> onClickAction)
        {
            using (var document = (JSObject)Runtime.GetGlobalObject("document"))
            using (var button = (JSObject)document.Invoke("getElementById", id))
            {
                button.SetObjectProperty("onclick", onClickAction);
            }
        }
    }

    public static class StampHelper
    {
        // https://www.meziantou.net/2018/09/24/getting-the-date-of-build-of-a-net-assembly-at-runtime
        public static DateTime GetBuildDate(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(x => x.Key == "BuildDate");
            if (attribute != null)
            {
                if (DateTime.TryParseExact(
                    attribute.Value,
                    "yyyyMMddHHmmss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var result))
                {
                    return result;
                }
            }

            return default(DateTime);
        }

        public static string GetCommitHash(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(x => x.Key == "CommitHash");
            if (attribute != null)
            {
                return attribute.Value;
            }

            return string.Empty;
        }
    }
}