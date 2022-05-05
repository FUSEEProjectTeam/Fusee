using OpenTK.Graphics.OpenGL;
using System;

namespace Fusee.ImGuiDesktop.Templates
{
    public abstract class FuseeViewportToMSAATexture
    {
        private readonly int _viewportFramebuffer;
        private readonly int _intermediateFrameBuffer;
        private readonly int _viewportRenderTexture;
        private readonly int _depthRenderbuffer;
        private readonly int _texture4ImGui;
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
        /// This method is called from within the base class, do not interfere here
        /// Insert your usual render loop
        /// </summary>
        protected virtual void RenderAFrame()
        {

        }

        /// <summary>
        /// This method is called from within the base class, do not interfere here
        /// Insert your usual update methods (e. g. input, etc.)
        /// </summary>
        protected virtual void Update()
        {

        }

        /// <summary>
        /// This method is called from within the base class, do not interfere here
        /// Insert your usual changes during resize, if necessary
        /// </summary>
        protected virtual void Resize(int width, int height)
        {

        }

        #endregion

        /// <summary>
        /// Expects the original game window dims for reseting view to orignal size after rendering
        /// </summary>
        /// <param name="originalWidth"></param>
        /// <param name="originalHeight"></param>
        /// <exception cref="Exception"></exception>
        public FuseeViewportToMSAATexture(int gameWindowWidth, int gameWindowHeight)
        {
            // FRAMEBUFFER STUFF
            GL.CreateFramebuffers(1, out int fb);
            _viewportFramebuffer = fb;
            GL.GenTextures(1, out int renderTex);
            _viewportRenderTexture = renderTex;
            GL.GenRenderbuffers(1, out _depthRenderbuffer);

            // Set up texture to blt to
            _texture4ImGui = GL.GenTexture();
            GL.CreateFramebuffers(1, out int iFb);
            _intermediateFrameBuffer = iFb;

            if (GL.GetError() != 0)
            {
                throw new Exception($"OpenGL Error {GL.GetError()}");
            }

            (_originalWidth, _originalHeight) = (gameWindowWidth, gameWindowHeight);
        }

        /// <summary>
        /// Update original game window dims so we can restore the viewport after rendering Fusee content
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public void UpdateOriginalGameWindowDimensions(int width, int height)
        {
            _originalWidth = width;
            _originalHeight = height;
        }

        public IntPtr RenderToTexture(int width, int height)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            // Enable FB
            UpdateRenderTexture(width, height);
            Resize(width, height);
            GL.Viewport(0, 0, width, height);

            // Do the actual rendering
            // this can be set from the user code
            Update();
            RenderAFrame();

            // after rendering, blt result into ViewportRenderTexture
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _viewportFramebuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _intermediateFrameBuffer);
            GL.BlitFramebuffer(0, 0, width, height, 0, 0, width, height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

            // Disable FB, reset size etc. to previous size
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Viewport(0, 0, _originalWidth, _originalHeight);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            // bind the render result and return ptr to texture
            GL.BindTexture(TextureTarget.Texture2D, _texture4ImGui);
            return new IntPtr(_texture4ImGui);
        }


        private void UpdateRenderTexture(int width, int height)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _intermediateFrameBuffer);
            GL.BindTexture(TextureTarget.Texture2D, _texture4ImGui);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, new IntPtr());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _texture4ImGui, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.Enable(EnableCap.Multisample);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _viewportFramebuffer);
            GL.BindTexture(TextureTarget.Texture2DMultisample, _viewportRenderTexture);

            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, 4, PixelInternalFormat.Rgba, width, height, true);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthRenderbuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, 4, RenderbufferStorage.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, _depthRenderbuffer);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, _viewportRenderTexture, 0);

            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == 0)
            {
                throw new Exception("Error Framebuffer!");
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }
    }
}
