using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    public class WritableTexture : Texture, IWritableTexture
    {
        public int TextureHandle { get; set; } = -1; // only int (texHandle.Handle) to allow a shader code builder

        public byte[] PixelData { get; private set; } //TODO: get px data from graphics card on PixelData get()

        
        public WritableTexture(IImageData imageData)
        {
            _imageData = new ImageData(
                new byte[imageData.Width * imageData.Height * imageData.PixelFormat.BytesPerPixel],
                imageData.Width, imageData.Height, imageData.PixelFormat);
            _imageData.Blt(0, 0, imageData);
        }

        public void Resize(int width, int height)
        {
            var imageData = _imageData;
            _imageData = new ImageData(
                new byte[width * height * imageData.PixelFormat.BytesPerPixel],
                width, height, imageData.PixelFormat);
            _imageData.Blt(0, 0, imageData);
        }
    }
}
