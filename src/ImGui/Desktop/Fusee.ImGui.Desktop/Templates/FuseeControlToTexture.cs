using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Imp.Graphics.Desktop;
using OpenTK.Graphics.OpenGL;
using System;

namespace Fusee.ImGuiImp.Desktop.Templates
{
    public abstract class FuseeControlToTexture
    {
        private int _originalWidth;
        private int _originalHeight;

        private int _lastWidth;
        private int _lastHeight;

        private IShaderHandle prgmHndl;

        protected RenderContext _rc;

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
        protected virtual ITextureHandle RenderAFrame()
        {
            return new Engine.Imp.Graphics.Desktop.TextureHandle();
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
        public FuseeControlToTexture(RenderContext rc) => _rc = rc;


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

            if (_rc.DefaultState != null)
            {
                _rc.DefaultState.CanvasWidth = width;
                _rc.DefaultState.CanvasHeight = height;
            }
        }

        /// <summary>
        /// Generate Fusee output and render into texture
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>IntPtr to texture to use with <see cref="ImGuiNET.ImGui.Image(IntPtr, System.Numerics.Vector2)"/></returns>
        public IntPtr RenderToTexture(int width, int height)
        {
            // prevent calls to resize every frame
            if (_lastHeight != height || _lastWidth != width)
            {
                _lastWidth = width;
                _lastHeight = height;
                Resize(width, height);
            }

            // Do the actual rendering
            var hndl = RenderAFrame();
            if (hndl == null) return IntPtr.Zero;
            var tex = ((TextureHandle)hndl).TexId;
            _rc.SetRenderTarget();
            _rc.Viewport(0, 0, _originalWidth, _originalHeight);

            // Warning: wolves ahead
            if (prgmHndl == null)
                prgmHndl = new ShaderHandle() { Handle = ImGuiController.ShaderProgram };
            _rc.CurrentShaderProgram = prgmHndl;

            return new IntPtr(tex);
        }
    }
}