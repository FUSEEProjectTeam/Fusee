using Fusee.Base.Core;
using Fusee.Math.Core;
using Microsoft.JSInterop;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fusee.Base.Imp.Blazor
{
    /// <summary>
    /// A BlazorProgramm contains some runtime variables, like canvasName, the canvas clear color as well as the render loop action
    /// </summary>
    public class BlazorProgramm
    {
        private readonly float4 CanvasColor = new(0.5f, 0.5f, 0.5f, 1.0f);
        private Action<double> loop;
        private double previousMilliseconds;
        private IJSObjectReference window;

        /// <summary>
        /// The JS Runtime
        /// </summary>
        public static IJSRuntime Runtime;

        private static string divCanvasName;
        private static string canvasName;

        private static BlazorBase mainExecutable;

        /// <summary>
        /// Starts the WASM program
        /// </summary>
        /// <param name="runtime"></param>
        /// <param name="wasm"></param>
        public void Start(BlazorBase wasm, IJSRuntime runtime)
        {
            // TODO: Add Fus-Startup-Loading Screen
            Runtime = runtime;
            loop = Loop;

            BlazorExtensions.Runtime = runtime; // set runtime for extension methods

            // Create our sample
            mainExecutable = wasm;

            divCanvasName = "div_canvas";
            canvasName = "canvas";

            using IJSInProcessObjectReference window = Runtime.GetGlobalObject<IJSInProcessObjectReference>("window");

            int windowWidth = window.GetObjectProperty<int>(runtime, "innerWidth");
            int windowHeight = window.GetObjectProperty<int>(runtime, "innerHeight");

            IJSInProcessObjectReference canvas = HtmlHelper.AddCanvas(runtime, divCanvasName, canvasName, windowWidth, windowHeight);
            mainExecutable.Init(canvas, runtime, CanvasColor);
            mainExecutable.Run();

            RequestAnimationFrame();
        }

        /// <summary>
        /// Called on resize from Javascript
        /// </summary>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        [JSInvokable]
        public static void OnResize(int newX, int newY)
        {
            mainExecutable?.Resize(newX, newY);
        }

        private static readonly Stopwatch _sw;
        private static Stopwatch SW => _sw ?? new Stopwatch();

        /// <summary>
        /// Main application loop, triggered via Javascript RequestAnimationFrame
        /// Runs with 60 fps if possible
        /// </summary>
        /// <param name="milliseconds"></param>
        [JSInvokable]
        public void Loop(double milliseconds = 0.0)
        {
            double elapsedMilliseconds = milliseconds - previousMilliseconds;
            previousMilliseconds = milliseconds;

            // calculate time from last render call + time it took to update the method
            SW.Start();
            mainExecutable.Update(elapsedMilliseconds);
            var updateDelta = elapsedMilliseconds + SW.ElapsedMilliseconds;
            SW.Stop();

            mainExecutable.Draw(updateDelta);
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

            using IJSInProcessObjectReference document = runtime.GetGlobalObject<IJSInProcessObjectReference>("document");
            using IJSInProcessObjectReference body = document.GetObjectProperty<IJSInProcessObjectReference>("body");
            IJSInProcessObjectReference canvas = document.Invoke<IJSInProcessObjectReference>("createElement", "canvas");
            canvas.SetObjectProperty("width", width);
            canvas.SetObjectProperty("height", height);
            canvas.SetObjectProperty("id", canvasId);

            using IJSInProcessObjectReference canvasDiv = document.Invoke<IJSInProcessObjectReference>("createElement", "div");

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
            using IJSInProcessObjectReference document = runtime.GetGlobalObject<IJSInProcessObjectReference>("document");
            using IJSInProcessObjectReference body = document.GetObjectProperty<IJSInProcessObjectReference>("body");
            using IJSInProcessObjectReference paragraph = document.Invoke<IJSInProcessObjectReference>("createElement", "p");
            paragraph.SetObjectProperty("innerHTML", text);
            body.InvokeVoid("appendChild", paragraph);
        }
    }
}