using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.WebAsm
{
    /// <summary>
    /// Opaque Texture Implementation for WebAsm.
    /// </summary>
    internal class TextureHandle : ITextureHandle
    {
        internal WebGLTexture TexHandle = null;
        internal WebGLFramebuffer FrameBufferHandle = null;
        internal WebGLRenderbuffer DepthRenderBufferHandle = null;
    }
}
