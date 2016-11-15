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
    class Texture : ITexture
    {
        internal int handle;
        internal int fboHandle = -1;

        // GBUFFER
        internal int gBufferHandle = -1;
        internal int gBufferPositionTextureHandle;
        internal int gBufferNormalTextureHandle;
        internal int gBufferAlbedoSpecTextureHandle;
        internal int gDepthRenderbufferHandle;

        internal int textureWidth;
        internal int textureHeight;
    }


}
