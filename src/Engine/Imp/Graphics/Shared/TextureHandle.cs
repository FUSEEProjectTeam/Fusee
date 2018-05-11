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
        internal int handle = -1;
        internal int fboHandle = -1;

        // GBUFFER
        internal int gBufferHandle = -1;
        internal int gBufferPositionTextureHandle = -1;
        internal int gBufferNormalTextureHandle = -1;
        internal int gBufferAlbedoSpecTextureHandle = -1;
        internal int gBufferDepthTextureHandle = -1;
        internal int gDepthRenderbufferHandle = -1;

        // RenderTexture
        internal int renderToTextureBufferHandle = -1;
        internal int intermediateToTextureBufferHandle = -1;
        internal bool toggle = false;
        internal int depthHandle = -1;

        internal int textureWidth;
        internal int textureHeight;
    }


}
