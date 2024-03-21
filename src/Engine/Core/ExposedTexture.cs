using Fusee.Base.Common;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// A texture which reveals an opaque <see cref="ITextureHandle"/> which holds the pointer to the pixel data one the GPU
    /// Use this texture only for special cases.
    /// To make this texture known to the <see cref="RenderContext"/> one has to call <see cref="RenderContext.RegisterTexture(ExposedTexture)"/>
    /// Current use case: Fusse.ImGui.
    /// </summary>
    public class ExposedTexture : Texture, IExposedTexture
    {
        /// <summary>
        /// The opaque <see cref="ITextureHandle"/> to the pixel data on the GPU
        /// </summary>
        public ITextureHandle? TextureHandle { get; internal set; }

        /// <summary>
        /// Constructor initializes a Texture from a pixelData byte buffer, width and height in pixels and <see cref="ImagePixelFormat"/>.
        /// </summary>
        /// <param name="pixelData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="colorFormat"></param>
        /// <param name="generateMipMaps"></param>
        /// <param name="filterMode"></param>
        /// <param name="wrapMode"></param>
        public ExposedTexture(byte[] pixelData, int width, int height, ImagePixelFormat colorFormat, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.LinearMipmapLinear, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
            : base(pixelData, width, height, colorFormat, generateMipMaps, filterMode, wrapMode)
        {

        }

        /// <summary>
        /// Initialize a Texture from an existing IImageData. The input IImageData will be copied into this Texture via <seealso cref="RenderContext.BlitMultisample2DTextureToTexture(WritableMultisampleTexture, WritableTexture)"/> command.
        /// </summary>
        /// <param name="imageData">The existing <see cref="IImageData"/> that will be copied to initialize a Texture instance.</param>
        /// <param name="generateMipMaps">Defines if mipmaps are created.</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        public ExposedTexture(IImageData imageData, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.NearestMipmapLinear, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
            : base(imageData, generateMipMaps, filterMode, wrapMode)
        {

        }
    }
}