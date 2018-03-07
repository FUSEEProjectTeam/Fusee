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
        internal int gBufferDepthTextureHandle;
        internal int gDepthRenderbufferHandle;

        // RenderTexture
        internal int renderToTextureBufferHandle = -1;
        internal int intermediateToTextureBufferHandle = -1;
        internal bool toggle = false;
        internal int depthHandle;

        internal int textureWidth;
        internal int textureHeight;
    }


}
