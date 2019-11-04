using System.Collections.Generic;

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
        /// The gpu handle that represents the gbuffer object.
        /// </summary>
        IBufferHandle GBufferHandle { get; set; }

        /// <summary>
        /// The gpu handle that represents the depth render buffer object.
        /// </summary>
        IBufferHandle DepthBufferHandle { get; set; }

        /// <summary>
        /// The array that holds all textures of the gbuffer.
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
    }
}
