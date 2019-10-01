
namespace Fusee.Engine.Common
{
    /// <summary>
    /// Interface for creating a WritableCubeMap with six single WritableTextures.
    /// </summary>
    public interface IWritableCubeMap
    {
        /// <summary>
        /// Textures are evaluated in the following order: 
        /// Positive X
        /// Negative X
        /// Positive Y
        /// Negative Y
        /// Positive Z
        /// Negative Z
        /// </summary>
        IWritableTexture[] Textures { get; set; }
    }
}
