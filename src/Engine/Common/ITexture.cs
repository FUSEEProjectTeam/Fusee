using Fusee.Base.Common;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Cross platform abstraction for textures, that are used on the gpu.
    /// </summary>
    public interface ITexture : ITextureBase
    {
        /// <summary>
        /// The <see cref="IImageData"/> that makes up the <see cref="ITexture"/> instance.
        /// </summary>
        IImageData ImageData { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public interface IExposedTexture : ITextureBase
    {
        /// <summary>
        /// Raw TextureHandle after GPU texture creation
        /// </summary>
        ITextureHandle TextureHandle { get; }
    }
}