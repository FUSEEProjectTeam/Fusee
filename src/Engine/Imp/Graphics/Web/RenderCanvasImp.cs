// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using System;
using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Graphics.Web
{
    /// <summary>
    /// Implements the render canvas.
    /// </summary>
    public class RenderCanvasImp : IRenderCanvasImp
    {
        [JSExternal]
        // The webgl canvas. Will be set in the c# constructor
        internal object _canvas;
        /// <summary>
        /// Not implemented!.
        /// </summary>
        [JSExternal]
        public RenderCanvasImp()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Gets and sets the Rendercanvas width.
        /// </summary>
        [JSExternal]
        public int Width { get; set; }
        /// <summary>
        /// Gets and sets the rendercanvas height.
        /// </summary>
        [JSExternal]
        public int Height { get; set; }
        /// <summary>
        /// Gets and sets the rendercanvas caption.
        /// </summary>
        [JSExternal]
        public string Caption { get; set; }
        /// <summary>
        /// Returns the time since the last frame.
        /// </summary>
        [JSExternal]
        public float DeltaTime { get; }
        /// <summary>
        /// Gets and sets the vsync value.
        /// </summary>
        [JSExternal]
        public bool VerticalSync { get; set; }
        /// <summary>
        /// Determines wether blending is enabled.
        /// </summary>
        [JSExternal]
        public bool EnableBlending { get; set; }
        /// <summary>
        /// Determines wether fullscreen is activated.
        /// </summary>
        [JSExternal]
        public bool Fullscreen { get; set; }
        /// <summary>
        /// shows the currently rendered image on the screen.
        /// </summary>
        [JSExternal]
        public void Present()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Starts the application
        /// </summary>
        [JSExternal]
        public void Run()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets the cursor type.
        /// </summary>
        /// <param name="cursorType"></param>
        [JSExternal]
        public void SetCursor(CursorType cursorType)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Opens a link.
        /// </summary>
        /// <param name="link"></param>
        [JSExternal]
        public void OpenLink(string link)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets the window size.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="posx"></param>
        /// <param name="posy"></param>
        /// <param name="borderHidden"></param>
        [JSExternal]
        public void SetWindowSize(int width, int height, int posx = -1, int posy = -1, bool borderHidden = false)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Closes the game window.
        /// </summary>
        [JSExternal]
        public void CloseGameWindow()
        {
            throw new NotImplementedException();
        }

#pragma warning disable 0067 // events are used in external javascript
        public event EventHandler<InitEventArgs> Init;
        public event EventHandler<InitEventArgs> UnLoad;
        public event EventHandler<RenderEventArgs> Render;
        public event EventHandler<ResizeEventArgs> Resize;
#pragma warning restore 0067
    }
}