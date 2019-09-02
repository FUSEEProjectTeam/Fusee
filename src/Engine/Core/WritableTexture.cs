using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Special Texture, e.g. for usage in multipass rendering.
    /// </summary>
    public class WritableTexture : Texture, IWritableTexture
    {
        /// <summary>
        /// Gets set, if the texture is created on the graphics card.
        /// </summary>
        public int TextureHandle { get; set; } = -1; // only int (texHandle.Handle) to allow a shader code builder

        /// <summary>
        /// Should be containing zeros by default. If you want to use the PixelData directly it gets blted from the graphics card (not implemented yet).
        /// </summary>
        public new byte[] PixelData { get; private set; } //TODO: get px data from graphics card on PixelData get()

        /// <summary>
        /// Creates a new instance of type "WritableTexture".
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="generateMipMaps">Defines if mipmaps are created.</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        public WritableTexture(IImageData imageData, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.LINEAR, TextureWrapMode wrapMode = TextureWrapMode.REPEAT)
        {
            _imageData = (ImageData)imageData;

            DoGenerateMipMaps = generateMipMaps;
            FilterMode = filterMode;
            WrapMode = wrapMode;
        }


        /// <summary>
        /// Creates a new instance of type "WritableTexture".
        /// </summary>
        /// <param name="colorFormat">The color format of the texture, <see cref="ImagePixelFormat"/></param>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="generateMipMaps">Defines if mipmaps are created.</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        public WritableTexture(ImagePixelFormat colorFormat, int width, int height, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.LINEAR, TextureWrapMode wrapMode = TextureWrapMode.REPEAT)
        {
            _imageData = new ImageData(
               new byte[width * height * colorFormat.BytesPerPixel],
               width, height, colorFormat);
            //_imageData.Blt(0, 0, imageData);

            DoGenerateMipMaps = generateMipMaps;
            FilterMode = filterMode;
            WrapMode = wrapMode;
        }
    }
}
