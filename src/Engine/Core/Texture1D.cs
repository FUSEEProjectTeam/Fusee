using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// 1D Texture
    /// </summary>
    public class Texture1D : Texture
    {

        /// <summary>
        /// Constructor initializes a Texture from a pixelData byte buffer, width in pixels and <see cref="ImagePixelFormat"/>.
        /// </summary>
        /// <param name="pixelData">The raw pixelData byte buffer that makes up the texture.</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="colorFormat">Provides additional information about pixel encoding.</param>
        /// <param name="generateMipMaps">Defines if mipmaps are created.</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        public Texture1D(byte[] pixelData, int width, ImagePixelFormat colorFormat, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.LinearMipmapLinear, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
        {
            UniqueIdentifier = Guid.NewGuid();
            ImageData = new ImageData(pixelData, width, 1, colorFormat);
            DoGenerateMipMaps = generateMipMaps;
            FilterMode = filterMode;
            WrapMode = wrapMode;
        }

        /// <summary>
        /// Initialize a Texture from an existing IImageData. The input IImageData will be copied into this Texture via <seealso cref="RenderContext.BlitMultisample2DTextureToTexture(WritableMultisampleTexture, WritableTexture)"/> command.
        /// </summary>
        /// <param name="imageData">The existing <see cref="IImageData"/> that will be copied to initialize a Texture instance.</param>
        /// <param name="generateMipMaps">Defines if mipmaps are created.</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        public Texture1D(IImageData imageData, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.NearestMipmapLinear, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
        {
            if (imageData.Height != 1)
                throw new ArgumentException("Height of the image data is not 1, use a Texture instead.");

            UniqueIdentifier = Guid.NewGuid();
            ImageData = imageData;

            DoGenerateMipMaps = generateMipMaps;
            FilterMode = filterMode;
            WrapMode = wrapMode;
        }
    }
}