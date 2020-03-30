using Fusee.Engine.Imp.Graphics.WebAsm;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Text;
using WebAssembly;

namespace Fusee.Base.Imp.WebAsm
{
    public abstract class WebAsmBase
    {
        protected WebGL2RenderingContextBase gl;
        protected float4 clearColor;
        protected JSObject canvas;
        protected int canvasWidth;
        protected int canvasHeight;

        public virtual bool EnableFullScreen => true;

        public virtual void Init(JSObject canvas, float4 clearColor)
        {
            this.clearColor = clearColor;
            this.canvas = canvas;

            canvasWidth = (int)canvas.GetObjectProperty("width");
            canvasHeight = (int)canvas.GetObjectProperty("height");

            gl = new WebGL2RenderingContext(canvas, new WebGLContextAttributes
            {
                Alpha = true,
                Antialias = true,
                PreferLowPowerToHighPerformance = false,
                Depth = true,
                PowerPreference = "high-performance",
                Desynchronized = true
            });
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
            Resize(canvasWidth, canvasHeight);

            gl.ClearColor(clearColor.x, clearColor.y, clearColor.z, clearColor.w);
            gl.Clear(WebGLRenderingContextBase.COLOR_BUFFER_BIT);
        }

        public virtual void Resize(int width, int height)
        {
            gl.Viewport(0, 0, width, height);
            canvasWidth = width;
            canvasHeight = height;
        }
    }
}
