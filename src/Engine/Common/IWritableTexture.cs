namespace Fusee.Engine.Common
{
   /// <summary>
   /// Cross platform abstraction for a WritableTexture.
   /// </summary>
    public interface IWritableTexture : ITexture
    {
        RenderTargetTextureTypes TextureType { get; }
    }
}
