using Fusee.Math.Core;
using System;
using WebAssembly;

namespace Fusee.Base.Imp.WebAsm
{
    public class WebAsmProgram
    {
        protected static readonly float4 CanvasColor = new float4(255, 0, 255, 255);
        protected static readonly Action<double> loop = new Action<double>(Loop);
        protected static double previousMilliseconds;
        protected static JSObject window;

        protected static string divCanvasName;
        protected static string canvasName;

        protected static WebAsmBase mainExecutable;

        public static void Start(WebAsmBase webAsm)
        {
            // Let's first check if we can continue with WebGL2 instead of crashing.
            if (!isBrowserSupportsWebGL2())
            {
                HtmlHelper.AddParagraph("We are sorry, but your browser does not seem to support WebGL2.");
                return;
            }

            // Create our sample
            mainExecutable = webAsm;
            var sampleName = mainExecutable.GetType().Name;

            HtmlHelper.GetBrowserWindowSize();

            divCanvasName = $"div_canvas";
            canvasName = $"canvas";

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
            using var window = (JSObject)Runtime.GetGlobalObject("window");
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

    public static class HtmlHelper
    {
        public static void GetBrowserWindowSize()
        {
            using var document = (JSObject)Runtime.GetGlobalObject("document");
            using var body = (JSObject)document.GetObjectProperty("body");
            Console.WriteLine($"Body width [{body.GetObjectProperty("width")}]");

        }

        public static JSObject AddCanvas(string divId, string canvasId, int width = 800, int height = 600)
        {
            using var document = (JSObject)Runtime.GetGlobalObject("document");
            using var body = (JSObject)document.GetObjectProperty("body");
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

        public static void AddHeader(int headerIndex, string text)
        {
            using var document = (JSObject)Runtime.GetGlobalObject("document");
            using var body = (JSObject)document.GetObjectProperty("body");
            using var header = (JSObject)document.Invoke("createElement", $"h{headerIndex}");
            using var headerText = (JSObject)document.Invoke("createTextNode", text);
            header.Invoke("appendChild", headerText);
            body.Invoke("appendChild", header);
        }

        public static void AddHeader1(string text)
        {
            AddHeader(1, text);
        }

        public static void AddHeader2(string text)
        {
            AddHeader(2, text);
        }

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
            using var document = (JSObject)Runtime.GetGlobalObject("document");
            using var button = (JSObject)document.Invoke("getElementById", id);
            button.SetObjectProperty("onclick", onClickAction);
        }
    }
}
