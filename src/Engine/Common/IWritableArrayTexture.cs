namespace Fusee.Engine.Common
{
    /// <summary>
    /// Cross platform abstraction for a WritableTexture.
    /// Use writable textures if you want to render into a texture.
    /// Does NOT offer access to the pixel data.
    /// </summary>
    public interface IWritableArrayTexture : IWritableTexture
    {
        /// <summary>
        /// The number of layers the array texture has.
        /// </summary>
        int Layers { get; }
    }
}