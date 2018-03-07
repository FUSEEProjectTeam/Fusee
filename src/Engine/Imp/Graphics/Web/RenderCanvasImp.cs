// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using System;
using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Graphics.Web
{
    public class RenderCanvasImp : IRenderCanvasImp
    {
        [JSExternal]
        // The webgl canvas. Will be set in the c# constructor
        internal object _canvas;

        [JSExternal]
        public RenderCanvasImp()
        {
            throw new NotImplementedException();
        }

        event EventHandler IRenderCanvasImp.Resize
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        [JSExternal]
        public int Width { get; set; }
        [JSExternal]
        public int Height { get; set; }
        [JSExternal]
        public string Caption { get; set; }
        [JSExternal]
        public float DeltaTime { get; }
        [JSExternal]
        public bool VerticalSync { get; set; }
        [JSExternal]
        public bool EnableBlending { get; set; }
        [JSExternal]
        public bool Fullscreen { get; set; }
        [JSExternal]
        public void Present()
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public void Run()
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public void SetCursor(CursorType cursorType)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public void OpenLink(string link)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public void SetWindowSize(int width, int height, int posx = -1, int posy = -1, bool borderHidden = false)
        {
            throw new NotImplementedException();
        }

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