using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Android
{
    /// <summary>
    /// Texture Implementation for OpenGL, an integer value is used as a handle
    /// </summary>
    class TextureHandle : ITextureHandle
    {
        internal int TexHandle = -1;
        internal int FrameBufferHandle = -1;
        internal int DepthRenderBufferHandle = -1;
    }
}