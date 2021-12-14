using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    /// <summary>
    /// Texture Implementation for OpenTK, an integer value is used as a handle
    /// </summary>
    class TextureHandle : ITextureHandle
    {
        internal OpenTK.Graphics.TextureHandle TexHandle = new(-1);
        internal OpenTK.Graphics.FramebufferHandle FrameBufferHandle = new(-1);
        internal OpenTK.Graphics.RenderbufferHandle DepthRenderBufferHandle = new(-1);
    }
}