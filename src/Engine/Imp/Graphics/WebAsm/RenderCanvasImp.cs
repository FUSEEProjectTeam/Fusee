using System;
using System.Drawing;
using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.WebAsm;
using WebAssembly;


namespace Fusee.Engine.Imp.Graphics.WebAsm
{    public class RenderCanvasImp : IRenderCanvasImp
    {
        internal WebGL2RenderingContextBase _gl;
        internal JSObject _canvas;

        public RenderCanvasImp(JSObject canvas, /* TODO: remove rest of parameters */ WebGL2RenderingContextBase gl, int width, int height)
        {
            _canvas = canvas;

            // TODO: Extract a convenient Gl Context (version 2) ourselves from the given canvas. Then retrieve width and height
            _gl = gl;
            Width = width;
            Height = height;
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public string Caption { get => ""; set { } }

        public float DeltaTime { get; set; }

        public bool VerticalSync { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Fullscreen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler<InitEventArgs> Init;
        public event EventHandler<InitEventArgs> UnLoad;
        public event EventHandler<RenderEventArgs> Render;
        public event EventHandler<ResizeEventArgs> Resize;

        public void CloseGameWindow()
        {
            // throw new NotImplementedException();
        }

        public void OpenLink(string link)
        {
            // throw new NotImplementedException();
        }

        public void Present()
        {
            // Nohting to do in WebGL
        }

        public void Run()        
        {
            DoInit();
            DoResize(Width, Height);
        }

        public void SetCursor(CursorType cursorType)
        {
            // throw new NotImplementedException();
        }

        public void SetWindowSize(int width, int height, int posx = -1, int posy = -1, bool borderHidden = false)
        {
            _canvas.SetObjectProperty("width", Width);
            _canvas.SetObjectProperty("height", Height);
            Width = width;
            Height = height;
            Resize?.Invoke(this, new ResizeEventArgs(width, height));
            
        }

        public void DoRender()
        {
            Render?.Invoke(this, new RenderEventArgs());
        }

        public void DoInit()
        {
            Init?.Invoke(this, new InitEventArgs());
        }

        public void DoResize(int w, int h)
        {
            Width = w;
            Height = h;

            Resize?.Invoke(this, new ResizeEventArgs(w, h));
        }
    }
}