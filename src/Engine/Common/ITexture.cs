using Fusee.Base.Common;

namespace Fusee.Engine.Common
{
    public interface ITexture : IImageData
    {
        byte[] PixelData { get; }
    }
}