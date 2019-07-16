using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.WebAsm;

namespace Fusee.Engine.Imp.Graphics.WebAsm
{
    /// <summary>
    /// Opaque Texture Implementation for WebAsm.
    /// </summary>
    class TextureHandle : ITextureHandle
    {
        internal WebGLTexture Handle = null;
        internal WebGLFramebuffer FboHandle = null;

        // GBUFFER
        internal WebGLFramebuffer GBufferHandle = null;
        internal WebGLTexture GBufferPositionTextureHandle = null;
        internal WebGLTexture GBufferNormalTextureHandle = null;
        internal WebGLTexture GBufferAlbedoSpecTextureHandle = null;
        internal WebGLTexture GBufferDepthTextureHandle = null;
        internal WebGLTexture GDepthRenderbufferHandle = null;

        // RenderTexture
        internal WebGLTexture RenderToTextureBufferHandle = null;
        internal WebGLTexture IntermediateToTextureBufferHandle = null;
        internal bool Toggle = false;
        internal WebGLTexture DepthHandle = null;
        /// <summary>
        /// The textures width.
        /// </summary>
        internal int TextureWidth;
        /// <summary>
        /// The textures height
        /// </summary>
        internal int TextureHeight;
    }


}
