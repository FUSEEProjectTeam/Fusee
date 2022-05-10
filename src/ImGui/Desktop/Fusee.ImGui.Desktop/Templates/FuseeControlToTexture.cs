using OpenTK.Graphics.OpenGL;
using System;

namespace Fusee.ImGuiDesktop.Templates
{
    public abstract class FuseeControlToTexture
    {
        private readonly int _viewportFramebuffer;
        private readonly int _depthRenderbuffer;
        private readonly int _returnTexture;

        private int _originalWidth;
        private int _originalHeight;

        #region UserCode

        /// <summary>
        /// Call this method when <see cref="RenderCanvas.Init()"/> is called
        /// </summary>
        public virtual void Init()
        {

        }

        /// <summary>
        /// Call this method when <see cref="RenderCanvas.Update()"/> is called
        /// Insert your usual update methods (e. g. input, etc.)
        /// </summary>
        public virtual void Update(bool allowInput)
        {

        }

        /// <summary>
        /// This method is called from within the base class, do not change anything inside base class
        /// Insert your usual render loop
        /// </summary>
        protected virtual void RenderAFrame()
        {

        }



        /// <summary>
        /// This method is called from within the base class, do not change anything inside base class
        /// Insert your usual changes during resize, if necessary
        /// </summary>
        protected virtual void Resize(int width, int height)
        {

        }

        #endregion

        /// <summary>
        /// Generate a Fusee view which is rendered to a MSAA texture and can then be used with <see cref="ImGuiNET.ImGui.Image(IntPtr, System.Numerics.Vector2)"/>
        /// Attention: Call <see cref="UpdateOriginalGameWindowDimensions(int, int)"/> after every window resize, as this class
        /// alters the window dimensions via <see cref="GL.Viewport(int, int, int, int)"/> and restores the original window size afterwards.
        /// When the original size information is missing or invalid, the result are terrible - as expected
        /// </summary>
        /// <param name="samplingFactor">MSAA sampling factor, default = 4</param>
        /// <exception cref="Exception"></exception>
        public FuseeControlToTexture()
        {
            GL.CreateFramebuffers(1, out int fb);
            _viewportFramebuffer = fb;
            GL.GenRenderbuffers(1, out _depthRenderbuffer);

            _returnTexture = GL.GenTexture();

            if (GL.GetError() != 0)
            {
                throw new Exception($"OpenGL Error {GL.GetError()}");
            }

        }

        /// <summary>
        /// Update original game window dims so we can restore the viewport after rendering Fusee content
        /// Needs to be called after each window resize!
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public void UpdateOriginalGameWindowDimensions(int width, int height)
        {
            _originalWidth = width;
            _originalHeight = height;
        }

        /// <summary>
        /// Generate Fusee output and render into texture
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>IntPtr to texture to use with <see cref="ImGuiNET.ImGui.Image(IntPtr, System.Numerics.Vector2)"/></returns>
        public IntPtr RenderToTexture(int width, int height)
        {
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            // Enable FB
            UpdateRenderTexture(width, height);
            Resize(width, height);
            // FIXME (later) (mr)
            // This should be RC.Viewport() which also sets _rc.DefaultState, howeever this is not possible right now
            // therefore we rely upon setting _rc.DefaultState later on (needed for Camera which needs the original size after rendering, too)
            GL.Viewport(0, 0, width, height);


            // Do the actual rendering
            // this can be set from the user code
            RenderAFrame();

            // Disable FB, reset size etc. to previous size
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Viewport(0, 0, _originalWidth, _originalHeight);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            // bind the render result and return ptr to texture
            GL.BindTexture(TextureTarget.Texture2D, _returnTexture);
            return new IntPtr(_returnTexture);
        }


        private void UpdateRenderTexture(int width, int height)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _viewportFramebuffer);
            GL.BindTexture(TextureTarget.Texture2D, _returnTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, new IntPtr());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthRenderbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, _depthRenderbuffer);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _returnTexture, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == 0)
            {
                throw new Exception("Error Framebuffer!");
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }
    }
}
