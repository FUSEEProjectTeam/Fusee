namespace Fusee.Engine.Common
{
   
    public interface IWritableTexture : ITexture
    {
        RenderTargetTextures TextureType { get; }
    }
}
