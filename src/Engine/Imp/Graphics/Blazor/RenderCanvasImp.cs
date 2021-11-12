using Fusee.Base.Imp.Blazor;
using Fusee.Engine.Common;
using Microsoft.JSInterop;
using System;


namespace Fusee.Engine.Imp.Graphics.Blazor
{
    /// <summary>
    /// This is a default render canvas implementation creating its own rendering window.
    /// </summary>
    public class RenderCanvasImp : IRenderCanvasImp
    {
        internal WebGL2RenderingContextBase _gl;
        internal IJSObjectReference _canvas;
        internal static IJSRuntime _runtime;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCanvasImp"/> class.
        /// </summary>
        public RenderCanvasImp(IJSObjectReference canvas, IJSRuntime runtime, WebGL2RenderingContextBase gl, int width, int height)
        {
            _canvas = canvas;
            _runtime = runtime;


            _gl = gl;
            Width = width;
            Height = height;
        }

        private int _width;
        private int _height;
        /// <summary>
        /// Gets he width of the rendering window.
        /// </summary>
        public int Width { get => _width; set => _width = value; }

        /// <summary>
        /// Gets the height of the rendering window.
        /// </summary>
        public int Height { get => _height; set => _height = value; }

        /// <summary>
        /// Gets and sets the caption (title of the rendering window).
        /// </summary>
        public string Caption { get => ""; set { } }

        /// <summary>
        /// Gets and sets the delta time.
        /// The delta time is the time that was required to render the last frame in milliseconds.
        /// This value can be used to determine the frames per second of the application.
        /// </summary>
        public float DeltaTime { get; set; }

        /// <summary>
        /// Gets and sets a value indicating whether vertical synchronization is used.
        /// </summary>
        /// <remarks> Currently not implemented.</remarks>
        public bool VerticalSync { get => true; set => _ = true; }

        /// <summary>
        /// Gets and sets a value indicating whether fullscreen is enabled.
        /// </summary>
        /// <remarks> Currently not implemented.</remarks>
        /// <exception cref="NotImplementedException"></exception>
        public bool Fullscreen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IWindowHandle WindowHandle => throw new NotImplementedException();

        public float DeltaTimeUpdate => throw new NotImplementedException();

        /// <summary>
        /// Occurs during initialization.
        /// </summary>
        public event EventHandler<InitEventArgs> Init;

        /// <summary>
        /// Occurs when unloading.
        /// </summary>
        public event EventHandler<InitEventArgs> UnLoad;

        /// <summary>
        /// Occurs when rendering.
        /// </summary>
        public event EventHandler<RenderEventArgs> Render;

        /// <summary>
        /// Occurs when resizing.
        /// </summary>
        public event EventHandler<ResizeEventArgs> Resize;
        public event EventHandler<RenderEventArgs> Update;

        /// <summary>
        /// Closes the game window.
        /// </summary>
        /// <remarks>Not needed in WebGL.</remarks>
        public void CloseGameWindow()
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// Opens the given URL in the user's standard browser. The link MUST start with "http://".
        /// </summary>
        /// <param name="link">The URL to open.</param>
        /// <remarks>Not needed in WebGL.</remarks>
        public void OpenLink(string link)
        {
            using IJSInProcessObjectReference window = _runtime.GetGlobalObject<IJSInProcessObjectReference>("window");
            window.InvokeVoid("open", link);
        }

        /// <summary>
        /// Not needed due to WebGL.
        /// </summary>
        public void Present()
        {
            // Nothing to do in WebGL
        }

        /// <summary>
        /// Runs this application instance.
        /// </summary>
        public void Run()
        {
            DoInit();
            DoResize(Width, Height);
        }

        /// <summary>
        /// Set the cursor to one of the predefined types.
        /// </summary>
        /// <param name="cursorType">THe type of cursor to set.</param>
        /// <remarks>Not needed in WebGL.</remarks>
        public void SetCursor(CursorType cursorType)
        {
            if (_canvas.Equals(-1)) return;

            switch (cursorType)
            {
                case CursorType.Standard:
                    _canvas.SetObjectProperty("style.cursor", "default");
                    break;
                case CursorType.Hand:
                    _canvas.SetObjectProperty("style.cursor", "pointer");
                    break;
            }
        }

        /// <summary>
        /// Sets the size of the output window for desktop development.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="posx">The x position of the window.</param>
        /// <param name="posy">The y position of the window.</param>
        /// <param name="borderHidden">Show the window border or not.</param>
        /// <remarks>Currently not implemented.</remarks>
        public void SetWindowSize(int width, int height, int posx = -1, int posy = -1, bool borderHidden = false)
        {
            _canvas.SetObjectProperty("width", Width);
            _canvas.SetObjectProperty("height", Height);
            Width = width;
            Height = height;
            Resize?.Invoke(this, new ResizeEventArgs(width, height));

        }

        /// <summary>
        /// Does the render of this instance.
        /// </summary>
        public void DoRender()
        {
            Render?.Invoke(this, new RenderEventArgs());
        }

        /// <summary>
        /// Does initialize this instance.
        /// </summary>
        public void DoInit()
        {
            Init?.Invoke(this, new InitEventArgs());
        }

        /// <summary>
        /// Does the resize on this instance.
        /// </summary>
        /// <param name="w">The width.</param>
        /// <param name="h">The height.</param>
        public void DoResize(int w, int h)
        {
            Width = w;
            Height = h;

            Resize?.Invoke(this, new ResizeEventArgs(w, h));
        }
    }
}