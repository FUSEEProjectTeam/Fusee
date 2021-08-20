using Fusee.Base.Common;
using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use writable textures if you want to render into a texture.
    /// Does NOT offer access to the pixel data.
    /// </summary>
    public class WritableTexture : IWritableTexture
    {
        /// <summary>
        /// TextureChanged event notifies observing TextureManager about property changes and the Texture's disposal.
        /// </summary>
        public event EventHandler<TextureEventArgs> TextureChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Textures's uniqueness in the current session.
        /// </summary>
        public Suid SessionUniqueIdentifier { get; private set; }

        /// <summary>
        /// Type of the render texture, <see cref="RenderTargetTextureTypes"/>.
        /// </summary>
        public RenderTargetTextureTypes TextureType { get; private set; }

        /// <summary>
        /// Is treated as "image2D" in the shader.
        /// </summary>
        public bool AsImage = false;

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int Width
        {
            get;
            private set;
        }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        public int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// PixelFormat provides additional information about pixel encoding.
        /// </summary>
        public ImagePixelFormat PixelFormat
        {
            get;
            private set;
        }

        /// <summary>
        /// Specifies if mipmaps are created for this texture.
        /// </summary>
        public bool DoGenerateMipMaps
        {
            get;
            private set;
        }

        /// <summary>
        /// Specifies the texture's wrap mode, see <see cref="TextureWrapMode"/>.
        /// </summary>
        public TextureWrapMode WrapMode
        {
            get;
            private set;
        }

        /// <summary>
        /// Specifies the texture's filter mode, see <see cref="TextureWrapMode"/>.
        /// </summary>
        public TextureFilterMode FilterMode
        {
            get;
            private set;
        }

        /// <summary>
        /// Specifies the texture's comparison mode, see <see cref="TextureCompareMode"/>.
        /// </summary>
        public TextureCompareMode CompareMode
        {
            get;
            private set;
        }


        /// <summary>
        /// Specifies the texture's comparison function, see <see cref="Compare"/>.
        /// </summary>
        public Compare CompareFunc
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of type "WritableTexture".
        /// </summary>
        /// <param name="texType">Defines the type of the render texture.</param>
        /// <param name="colorFormat">The color format of the texture, <see cref="ImagePixelFormat"/></param>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="generateMipMaps">Defines if mipmaps are created.</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        /// <param name="compareMode">The textures compare mode. If uncertain, leaf on NONE, this is only important for depth (shadow) textures (<see cref="TextureCompareMode"/>).</param>
        /// <param name="compareFunc">The textures compare function. If uncertain, leaf on LEESS, this is only important for depth (shadow) textures and if the CompareMode isn't NONE (<see cref="Compare"/>)</param>
        public WritableTexture(RenderTargetTextureTypes texType, ImagePixelFormat colorFormat, int width, int height, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.NearestMipmapLinear, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureCompareMode compareMode = TextureCompareMode.None, Compare compareFunc = Compare.Less)
        {
            SessionUniqueIdentifier = Suid.GenerateSuid();
            PixelFormat = colorFormat;
            Width = width;
            Height = height;
            DoGenerateMipMaps = generateMipMaps;
            FilterMode = filterMode;
            WrapMode = wrapMode;
            TextureType = texType;
            CompareMode = compareMode;
            CompareFunc = compareFunc;
        }

        /// <summary>
        /// Create a texture that is intended to save position information.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="pxFormat">The color format of this texture.</param>
        public static WritableTexture CreatePosTex(int width, int height, ImagePixelFormat pxFormat)
        {
            return new WritableTexture(RenderTargetTextureTypes.Position, pxFormat, width, height, false, TextureFilterMode.Nearest);
        }

        /// <summary>
        /// Create a texture that is intended to save albedo (rgb channels) information.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="pxFormat">The color format of this texture.</param>
        public static WritableTexture CreateAlbedoTex(int width, int height, ImagePixelFormat pxFormat)
        {
            return new WritableTexture(RenderTargetTextureTypes.Albedo, pxFormat, width, height, false, TextureFilterMode.Linear);
        }

        /// <summary>
        /// Create a texture that is intended to save specular information, depending on whether standard or physically based calculation is used.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="pxFormat">The color format of this texture.</param>
        public static WritableTexture CreateSpecularTex(int width, int height, ImagePixelFormat pxFormat)
        {
            return new WritableTexture(RenderTargetTextureTypes.Specular, pxFormat, width, height, false, TextureFilterMode.Linear);
        }

        /// <summary>
        /// Create a texture that is intended to save emissive color.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="pxFormat">The color format of this texture.</param>
        public static WritableTexture CreateEmissionTex(int width, int height, ImagePixelFormat pxFormat)
        {
            return new WritableTexture(RenderTargetTextureTypes.Emission, pxFormat, width, height, false, TextureFilterMode.Linear);
        }

        /// <summary>
        /// Create a texture that is intended to save normal information.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="pxFormat">The color format of this texture.</param>
        public static WritableTexture CreateNormalTex(int width, int height, ImagePixelFormat pxFormat)
        {
            return new WritableTexture(RenderTargetTextureTypes.Normal, pxFormat, width, height, false, TextureFilterMode.Nearest);
        }

        /// <summary>
        /// Create a texture that is intended to save depth information.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="pxFormat">The color format of this texture.</param>
        /// <param name="compareMode">The textures compare mode. If uncertain, leaf on NONE, this is only important for depth (shadow) textures (<see cref="TextureCompareMode"/>).</param>
        /// <param name="compareFunc">The textures compare function. If uncertain, leaf on LEESS, this is only important for depth (shadow) textures and if the CompareMode isn't NONE (<see cref="Compare"/>)</param>
        /// <returns></returns>
        public static WritableTexture CreateDepthTex(int width, int height, ImagePixelFormat pxFormat, TextureCompareMode compareMode = TextureCompareMode.None, Compare compareFunc = Compare.Less)
        {
            return new WritableTexture(RenderTargetTextureTypes.Depth, pxFormat, width, height, false, TextureFilterMode.Nearest, TextureWrapMode.ClampToBorder, compareMode, compareFunc);
        }

        /// <summary>
        /// Create a texture that is intended to save SSAO information.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <returns></returns>
        public static WritableTexture CreateSSAOTex(int width, int height)
        {
            return new WritableTexture(RenderTargetTextureTypes.Ssao, new ImagePixelFormat(ColorFormat.fRGB16), width, height, false, TextureFilterMode.Nearest);
        }

        /// <summary>
        /// Create a texture that is intended to save SSAO information.
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <returns></returns>
        public static WritableTexture CreateForComputeShader(int width, int height)
        {
            return new WritableTexture(RenderTargetTextureTypes.Albedo, new ImagePixelFormat(ColorFormat.fRGBA32), width, height, false, TextureFilterMode.Linear);
        }

        /// <summary>
        /// Implementation of the <see cref="IDisposable"/> interface.
        /// </summary>
        public void Dispose()
        {
            TextureChanged?.Invoke(this, new TextureEventArgs(this, TextureChangedEnum.Disposed));
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose"/> in order to fire TextureChanged event.
        /// </summary>
        ~WritableTexture()
        {
            Dispose();
        }
    }
}