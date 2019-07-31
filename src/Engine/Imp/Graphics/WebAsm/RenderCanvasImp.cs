using System;
using System.Drawing;
using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.WebAsm;
using WebAssembly;


namespace Fusee.Engine.Imp.Graphics.WebAsm
{    public class RenderCanvasImp : IRenderCanvasImp
    {
        internal WebGLRenderingContextBase _gl;
        internal JSObject _canvas;
        private int _width;
        private int _height;


        public RenderCanvasImp(JSObject canvas, /* TODO: remove rest of parameters */ WebGLRenderingContextBase gl, int width, int height)
        {
            _canvas = canvas;

            // TODO: Extract a convenient Gl Context (version 2) ourselves from the given canvas. Then retrieve width and height
            _gl = gl;
            _width = width;
            _height = height;
        }

        public int Width { get => _width; set => throw new NotImplementedException(); }
        public int Height { get => _height; set => throw new NotImplementedException(); }
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
            DoResize(_width, _height);
        }

        public void SetCursor(CursorType cursorType)
        {
            // throw new NotImplementedException();
        }

        public void SetWindowSize(int width, int height, int posx = -1, int posy = -1, bool borderHidden = false)
        {
            throw new NotImplementedException();
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
            _width = w;
            _height = h;
            Resize?.Invoke(this, new ResizeEventArgs(w, h));
        }
    }
}