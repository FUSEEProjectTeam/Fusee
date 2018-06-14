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
        internal int Handle = -1;
        internal int FboHandle = -1;

        // GBUFFER
        internal int GBufferHandle = -1;
        internal int GBufferPositionTextureHandle = -1;
        internal int GBufferNormalTextureHandle = -1;
        internal int GBufferAlbedoSpecTextureHandle = -1;
        internal int GBufferDepthTextureHandle = -1;
        internal int GDepthRenderbufferHandle = -1;

        // RenderTexture
        internal int RenderToTextureBufferHandle = -1;
        internal int IntermediateToTextureBufferHandle = -1;
        internal bool Toggle = false;
        internal int DepthHandle = -1;

        internal int TextureWidth;
        internal int TextureHeight;
    }


}
