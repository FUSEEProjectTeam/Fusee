using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Cross platform abstraction for the RenderTarget object.
    /// Use a RenderTarget if you want to render into buffer object, associated with one or more textures.
    /// If only a single texture is needed, the usage of a <see cref="IWritableTexture"/> as a render target is preferred.
    /// </summary>
    public interface IRenderTarget
    {
        /// <summary>
        /// The gpu handle that represents the G-buffer object.
        /// </summary>
        IBufferHandle GBufferHandle { get; set; }

        /// <summary>
        /// The gpu handle that represents the depth render buffer object.
        /// </summary>
        IBufferHandle DepthBufferHandle { get; set; }

        /// <summary>
        /// The array that holds all textures of the G-buffer.
        /// </summary>
        IWritableTexture[] RenderTextures { get; }

        /// <summary>
        /// The resolution of the render textures in px.
        /// </summary>
        TexRes TextureResolution { get; }

        /// <summary>
        /// Set this if the RenderTarget only contains a depth texture to prevent the creation of color buffers.
        /// Careful: it is better to use a <see cref="IWritableTexture"/> when rendering to a single texture!
        /// </summary>
        bool IsDepthOnly { get; set; }

        /// <summary>
        /// Generates a position texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        void SetPositionTex();

        /// <summary>
        /// Generates a albedo and specular (alpha channel) texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        void SetAlbedoTex();

        /// <summary>
        /// Generates a normal texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        void SetNormalTex();

        /// <summary>
        /// Generates a depth texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        void SetDepthTex(TextureCompareMode texCompareMode, Compare depthCompare);

        /// <summary>
        /// Generates a specular texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        void SetSpecularTex();

        /// <summary>
        /// Generates a emissive texture and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        void SetEmissiveTex();

        /// <summary>
        /// Generates a texture containing subsurface color and strength and sets it at the correct position in the RenderTextures Array.
        /// </summary>
        void SetSubsurfaceTex();

        /// <summary>
        /// Event that deletes unmanaged buffer objects.
        /// </summary>
        event EventHandler<EventArgs> DeleteBuffers;

        /// <summary>
        /// Sets a RenderTexture into the correct position in the RenderTexture array.
        /// </summary>
        /// <param name="src">The source RenderTexture.</param>
        /// <param name="tex">The type of the texture.</param>
        void SetTexture(IWritableTexture src, RenderTargetTextureTypes tex);
    }
}