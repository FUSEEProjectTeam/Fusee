using System;
using System.Drawing;
using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.WebAsm;
using WebAssembly;


namespace Fusee.Engine.Imp.Graphics.WebAsm
{
    /// <summary>
    /// This is a default render canvas implementation creating its own rendering window.
    /// </summary>
    public class RenderCanvasImp : IRenderCanvasImp
    {
        internal WebGL2RenderingContextBase _gl;
        internal JSObject _canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCanvasImp"/> class.
        /// </summary>
        public RenderCanvasImp(JSObject canvas, /* TODO: remove rest of parameters */ WebGL2RenderingContextBase gl, int width, int height)
        {
            _canvas = canvas;

            // TODO: Extract a convenient Gl Context (version 2) ourselves from the given canvas. Then retrieve width and height
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
        /// Gets and sets a value indicating whether vertical snychronization is used.
        /// </summary>
        /// <remarks> Currently not implemented.</remarks>
        public bool VerticalSync { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Gets and sets a value indicating whether fullscreen is enabled.
        /// </summary>
        /// <remarks> Currently not implemented.</remarks>
        public bool Fullscreen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

        /// <summary>
        /// Closes the game window.
        /// </summary>
        /// <remarks>Not needed in WebGL.</remarks>
        public void CloseGameWindow()
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// Opens the given url in the user's standard browser. The link MUST start with "http://".
        /// </summary>
        /// <param name="link">The URL to open.</param>
        /// <remarks>Not needed in WebGL.</remarks>
        public void OpenLink(string link)
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// Not needed due to WebGL.
        /// </summary>
        public void Present()
        {
            // Nohting to do in WebGL
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
        /// Set the cursor to one of the pre-defined types.
        /// </summary>
        /// <param name="cursorType">THe type of cursor to set.</param>
        /// <remarks>Not needed in WebGL.</remarks>
        public void SetCursor(CursorType cursorType)
        {
            // throw new NotImplementedException();
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
        /// Dpes the initialize of this instance.
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