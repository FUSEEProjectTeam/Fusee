namespace Fusee.Engine.Common
{
   
    public interface IWritableTexture : ITexture
    {       
        ITextureHandle TextureHandle { get; set; }        
    }
}
