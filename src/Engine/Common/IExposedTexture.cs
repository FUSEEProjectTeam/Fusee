namespace Fusee.Engine.Common
{
    /// <summary>
    /// Cross platform abstraction for textures, that are used on the gpu and expose their <see cref="ITextureHandle"/>.
    /// </summary>
    public interface IExposedTexture : ITextureBase
    {
        /// <summary>
        /// Raw TextureHandle after GPU texture creation
        /// </summary>
        ITextureHandle? TextureHandle { get; }
    }
}