namespace Fusee.Engine.Core
{
    /// <summary>
    /// Enum describes if a Texture has been disposed or if the texture's region changed. Used inside <see cref="TextureManager"/>.
    /// </summary>
    public enum TextureChangedEnum
    {
        /// <summary>
        /// The texture has been disposed.
        /// </summary>
        Disposed = 0,
        /// <summary>
        /// The texture's region has changed
        /// </summary>
        RegionChanged,
    }
}