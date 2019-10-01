using Fusee.Base.Common;
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
        public ITextureHandle TextureHandle { get; set; }

        /// <summary>
        /// Should be containing zeros by default. If you want to use the PixelData directly it gets blted from the graphics card (not implemented yet).
        /// </summary>
        public new byte[] PixelData { get; private set; } //TODO: (?) get px data (and _imageData) from graphics card on PixelData get()

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public new int Width
        {
            get;
            private set;
        }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        public new int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// PixelFormat provides additional information about pixel encoding.
        /// </summary>
        public new ImagePixelFormat PixelFormat
        {
            get;
            private set;
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
            PixelFormat = colorFormat;
            Width = width;
            Height = height;
            DoGenerateMipMaps = generateMipMaps;
            FilterMode = filterMode;
            WrapMode = wrapMode;            
        }
    }
}
