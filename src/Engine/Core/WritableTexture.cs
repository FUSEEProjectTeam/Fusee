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

        /// <summary>
        /// Create a texture that is intended to save position information.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <returns></returns>
        public static WritableTexture CreatePosTex(int width, int height)
        {
            return new WritableTexture(new ImagePixelFormat(ColorFormat.fRGB32), width, height, false, TextureFilterMode.NEAREST);
        }

        /// <summary>
        /// Create a texture that is intended to save albedo (rgb channels) and specular (alpha channel) information.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <returns></returns>
        public static WritableTexture CreateAlbedoSpecularTex(int width, int height)
        {
            return new WritableTexture(new ImagePixelFormat(ColorFormat.RGBA), width, height, false);
        }

        /// <summary>
        /// Create a texture that is intended to save normal information.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <returns></returns>
        public static WritableTexture CreateNormalTex(int width, int height)
        {
            return new WritableTexture(new ImagePixelFormat(ColorFormat.fRGB16), width, height, false, TextureFilterMode.NEAREST);
        }

        /// <summary>
        /// Create a texture that is intended to save depth information.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <returns></returns>
        public static WritableTexture CreateDepthTex(int width, int height)
        {
            return new WritableTexture(new ImagePixelFormat(ColorFormat.Depth), width, height, false, TextureFilterMode.NEAREST, TextureWrapMode.CLAMP_TO_BORDER);
        }

        /// <summary>
        /// Create a texture that is intended to save ssao information.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <returns></returns>
        public static WritableTexture CreateSSAOTex(int width, int height)
        {
            return new WritableTexture(new ImagePixelFormat(ColorFormat.fRGB32), width, height, false, TextureFilterMode.NEAREST);
        }
    }
}
