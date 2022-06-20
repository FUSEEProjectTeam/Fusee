using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    /// <summary>
    /// Texture Implementation for OpenTK, an integer value is used as a handle
    /// </summary>
    public class TextureHandle : ITextureHandle
    {
        public OpenTK.Graphics.TextureHandle TexHandle = new(-1);
        public OpenTK.Graphics.FramebufferHandle FrameBufferHandle = new(-1);
        public OpenTK.Graphics.RenderbufferHandle DepthRenderBufferHandle = new(-1);
    }
}