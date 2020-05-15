using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.WebAsm;

namespace Fusee.Engine.Imp.Graphics.WebAsm
{
    /// <summary>
    /// Opaque Texture Implementation for WebAsm.
    /// </summary>
    class TextureHandle : ITextureHandle
    {
        internal WebGLTexture TexHandle = null;
        internal WebGLFramebuffer FrameBufferHandle = null;
        internal WebGLRenderbuffer DepthRenderBufferHandle = null;
    }
}
