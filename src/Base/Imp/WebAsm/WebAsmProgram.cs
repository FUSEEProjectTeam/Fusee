using Fusee.Base.Core;
using Fusee.Math.Core;
using Microsoft.JSInterop;
using System;


namespace Fusee.Base.Imp.WebAsm
{
    /// <summary>
    /// A WebAsmProgram contains some runtime variables, like canvasName, the canvas clear color as well as the render loop action
    /// </summary>
    public class WebAsmProgram
    {
        private readonly float4 CanvasColor = new float4(0.5f, 0.5f, 0.5f, 1.0f);
        private Action<double> loop;
        private double previousMilliseconds;
        private IJSObjectReference window;

        /// <summary>
        /// The JS Runtime
        /// </summary>
        public static IJSRuntime Runtime;

        private static string divCanvasName;
        private static string canvasName;

        private Fusee.Base.Imp.WebAsm.WebAsmBase mainExecutable;

        /// <summary>
        /// Starts the WASM program
        /// </summary>
        /// <param name="runtime"></param>
        /// <param name="wasm"></param>
        public void Start(WebAsmBase wasm, IJSRuntime runtime)
        {
            // TODO: Add Fus-Startup-Loading Screen

            Runtime = runtime;
            loop = Loop;

            WebAsmExtensions.Runtime = runtime; // set runtime for extension methods

            // Let's first check if we can continue with WebGL2 instead of crashing.
            //if (!IsBrowserSupportsWebGL2())
            //{
            //HtmlHelper.AddParagraph(wasm.Runtime, "We are sorry, but your browser does not seem to support WebGL2.");
            //return;
            //}

            // Create our sample
            mainExecutable = wasm;

            divCanvasName = "div_canvas";
            canvasName = "canvas";

            using var window = Runtime.GetGlobalObject<IJSInProcessObjectReference>("window");

            var windowWidth = window.GetObjectProperty<int>(runtime, "innerWidth");
            var windowHeight = window.GetObjectProperty<int>(runtime, "innerHeight");
           
            var canvas = HtmlHelper.AddCanvas(runtime, divCanvasName, canvasName, windowWidth, windowHeight);
            mainExecutable.Init(canvas, runtime, CanvasColor);
            mainExecutable.Run();

            // TODO: Implement in javascript
            //AddEnterFullScreenHandler();
            //AddResizeHandler();

            RequestAnimationFrame();
        }

        private void AddResizeHandler()
        {
            using var window = Runtime.GetGlobalObject<IJSInProcessObjectReference>("window");
            window.InvokeVoid("addEventListener", "resize", new Action<IJSInProcessObjectReference>((o) =>
            {
                using (var d = Runtime.GetGlobalObject<IJSInProcessObjectReference>("document"))
                using (var w = Runtime.GetGlobalObject<IJSInProcessObjectReference>("window"))
                {
                    using var canvasObject = d.Invoke<IJSInProcessObjectReference>("getElementById", canvasName);

                    var windowWidth = w.GetObjectProperty<int>("innerWidth");
                    var windowHeight = w.GetObjectProperty<int>("innerHeight");

                    var cobj = canvasObject.GetObjectProperty<string>("id");

                    canvasObject.SetObjectProperty("width", windowWidth);
                    canvasObject.SetObjectProperty("height", windowHeight);
                    // call fusee resize
                    mainExecutable.Resize(windowWidth, windowHeight);
                }

                o.Dispose();
            }), false);
        }

        private void RequestFullscreen(IJSInProcessObjectReference canvas)
        {
            if (canvas.GetObjectProperty<IJSInProcessObjectReference>("requestFullscreen") != null)
                canvas.InvokeVoid("requestFullscreen");
            if (canvas.GetObjectProperty<IJSInProcessObjectReference>("webkitRequestFullscreen") != null)
                canvas.InvokeVoid("webkitRequestFullscreen");
            if (canvas.GetObjectProperty<IJSInProcessObjectReference>("msRequestFullscreen") != null)
                canvas.InvokeVoid("msRequestFullscreen");

        }

        private void AddEnterFullScreenHandler()
        {
            using var canvas = Runtime.GetGlobalObject<IJSInProcessObjectReference>(canvasName);
            canvas.InvokeVoid("addEventListener", "dblclick", new Action<IJSInProcessObjectReference>((o) =>
           {
               using var d = Runtime.GetGlobalObject<IJSInProcessObjectReference>("document");
               
                   var canvasObject = d.Invoke<IJSInProcessObjectReference>("getElementById", canvasName);

                   RequestFullscreen(canvasObject);

                   var width = canvasObject.GetObjectProperty<int>("clientWidth");
                   var height = canvasObject.GetObjectProperty<int>("clientHeight");

                   SetNewCanvasSize(canvasObject, width, height);

                   // call fusee resize
                   mainExecutable.Resize(width, height);
               

               o.Dispose();
           }), false);
        }

        private void SetNewCanvasSize(IJSInProcessObjectReference canvasObject, int newWidth, int newHeight)
        {
            canvasObject.SetObjectProperty("width", newWidth);
            canvasObject.SetObjectProperty("height", newHeight);
        }

        [JSInvokable]
        public void Loop(double milliseconds = 0.0)
        {
            var elapsedMilliseconds = milliseconds - previousMilliseconds;
            previousMilliseconds = milliseconds;

            mainExecutable.Update(elapsedMilliseconds);
            mainExecutable.Draw();
        }

        
        private void RequestAnimationFrame()
        {
            if (window == null)
            {
                window = Runtime.GetGlobalObject<IJSObjectReference>("window");
            }

            // disable
            Diagnostics.SetMinConsoleLoggingSeverityLevel(Diagnostics.SeverityLevel.None);
            Diagnostics.SetMinDebugOutputLoggingSeverityLevel(Diagnostics.SeverityLevel.None);

            ((IJSInProcessObjectReference)window).InvokeVoid("init", DotNetObjectReference.Create(this));
        }
    }

    /// <summary>
    /// Helper class for often used functions
    /// </summary>
    public static class HtmlHelper
    {
        /// <summary>
        /// Creates an attaches a canvas to the HTML page
        /// </summary>
        /// <param name="runtime"></param>
        /// <param name="divId"></param>
        /// <param name="canvasId"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static IJSInProcessObjectReference AddCanvas(IJSRuntime runtime, string divId, string canvasId, int width = 800, int height = 600)
        {
            // we need to add runtime everywhere as we do not have RenderCanvasImp._runtime, yet

            using var document = runtime.GetGlobalObject<IJSInProcessObjectReference>("document");
            using var body = document.GetObjectProperty<IJSInProcessObjectReference>("body");
            var canvas = document.Invoke<IJSInProcessObjectReference>("createElement", "canvas");
            canvas.SetObjectProperty("width", width);
            canvas.SetObjectProperty("height", height);
            canvas.SetObjectProperty("id", canvasId);

            using var canvasDiv = document.Invoke<IJSInProcessObjectReference>("createElement", "div");

            canvasDiv.SetObjectProperty("id", divId);
            canvasDiv.InvokeVoid("appendChild", canvas);

            body.InvokeVoid("appendChild", canvasDiv);

            return canvas;
        }

        /// <summary>
        /// Adds a paragraph to the current HTML page
        /// </summary>
        /// <param name="runtime"></param>
        /// <param name="text"></param>
        public static void AddParagraph(IJSRuntime runtime, string text)
        {
            using var document = runtime.GetGlobalObject<IJSInProcessObjectReference>("document");
            using var body = document.GetObjectProperty<IJSInProcessObjectReference>("body");
            using var paragraph = document.Invoke<IJSInProcessObjectReference>("createElement", "p");
            paragraph.SetObjectProperty("innerHTML", text);
            body.InvokeVoid("appendChild", paragraph);
        }
    }
}
