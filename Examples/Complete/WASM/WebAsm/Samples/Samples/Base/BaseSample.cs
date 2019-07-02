using Fusee.Math.Core;
using System;
using WebAssembly;
using WebGLDotNET;

namespace Samples
{
    public abstract class BaseSample : ISample
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
            gl = new WebGL2RenderingContext(canvas, webglContextAttrib);
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
}
