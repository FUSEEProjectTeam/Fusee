namespace Fusee.Engine.Common
{
    public interface IWritableTexture : ITexture
    {
        int TextureHandle { get; set; }
        byte[] PixelData { get; }
    }
}
