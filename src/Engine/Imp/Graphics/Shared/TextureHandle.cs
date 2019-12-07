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
    class TextureHandle : ITextureHandle
    {
        internal int TexHandle = -1;
        internal int FrameBufferHandle = -1;
        internal int DepthRenderBufferHandle = -1;
    }
}