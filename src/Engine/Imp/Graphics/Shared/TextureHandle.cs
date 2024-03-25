using Fusee.Engine.Common;

#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID
namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    /// <summary>
    /// Texture Implementation for OpenTK, an integer value is used as a handle
    /// </summary>
    public class TextureHandle : ITextureHandle
    {
        /// <summary>
        /// The texture id
        /// </summary>
        public int TexId = -1;
        /// <summary>
        /// The framebuffer handle
        /// </summary>
        public int FrameBufferHandle = -1;
        /// <summary>
        /// The depth renderbuffer handle
        /// </summary>
        public int DepthRenderBufferHandle = -1;
    }
}