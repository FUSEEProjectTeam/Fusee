
using Fusee.Base.Common;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Interface for creating a WritableCubeMap with six single WritableTextures.
    /// </summary>
    public interface IWritableCubeMap
    {
        RenderTargetTextures TextureType { get;}       

        IWritableTexture PositiveX { get; }
        IWritableTexture NegativeX { get; }
        IWritableTexture PositiveY { get; }
        IWritableTexture NegativeY { get; }
        IWritableTexture PositiveZ { get; }
        IWritableTexture NegativeZ { get; }

        ITexture GetTextureByFace(CubeMapFaces face);

    }
}
