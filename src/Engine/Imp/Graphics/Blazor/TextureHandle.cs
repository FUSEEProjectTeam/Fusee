using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Blazor
{
    /// <summary>
    /// Opaque Texture Implementation for Blazor.
    /// </summary>
    internal class TextureHandle : ITextureHandle
    {
        internal WebGLTexture TexId = null;
        internal WebGLFramebuffer FrameBufferHandle = null;
        internal WebGLRenderbuffer DepthRenderBufferHandle = null;
    }
}