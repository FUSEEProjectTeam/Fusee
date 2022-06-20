using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Android
{
    /// <summary>
    /// Texture Implementation for OpenGL, an integer value is used as a handle
    /// </summary>
    public class TextureHandle : ITextureHandle
    {
        public int TexHandle = -1;
        public int FrameBufferHandle = -1;
        public int DepthRenderBufferHandle = -1;
    }
}