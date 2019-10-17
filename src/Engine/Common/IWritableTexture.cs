namespace Fusee.Engine.Common
{
   /// <summary>
   /// Cross platform abstraction for a WritableTexture.
   /// </summary>
    public interface IWritableTexture : ITextureBase
    {
        RenderTargetTextureTypes TextureType { get; }
    }
}
