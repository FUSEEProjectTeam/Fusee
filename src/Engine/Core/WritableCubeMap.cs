using Fusee.Base.Common;
using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Special type of <see cref="WritableTexture"/>. If this type is used a cube map with six faces is created on the gpu.
    /// </summary>
    public class WritableCubeMap : IWritableCubeMap
    {
        /// <summary>
        /// TextureChanged event notifies observing TextureManager about property changes and the Texture's disposal.
        /// </summary>
        public event EventHandler<TextureEventArgs>? TextureChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Textures's uniqueness in the current session.
        /// </summary>
        public Guid UniqueIdentifier { get; private set; }

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
        /// Type of the render texture, <see cref="RenderTargetTextureTypes"/>.
        /// </summary>
        public RenderTargetTextureTypes TextureType { get; private set; }


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
        /// Specifies the texture's comparison mode, see <see cref="TextureCompareMode"/>.
        /// </summary>
        public TextureCompareMode CompareMode
        {
            get;
            private set;
        }


        /// <summary>
        /// Specifies the texture's comparison function. See <see cref="Compare"/>.
        /// </summary>
        public Compare CompareFunc
        {
            get;
            private set;
        }

        /// <summary>
        /// Opaque handle to the texture object on the gpu.
        /// </summary>
        public ITextureHandle TextureHandle { get; internal set; }

        /// <summary>
        /// Creates a new instance of type "WritableTexture".
        /// </summary>
        /// <param name="colorFormat">The color format of the texture, <see cref="ImagePixelFormat"/></param>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="generateMipMaps">Defines if mipmaps are created.</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        /// <param name="compareMode">Defines the compare mode <see cref="TextureCompareMode"/>.</param>
        /// <param name="compareFunc">Specifies the texture's comparison function.</param>
        /// <param name="textureType">The type of the texture.</param>
        public WritableCubeMap(RenderTargetTextureTypes textureType, ImagePixelFormat colorFormat, int width, int height, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.LinearMipmapLinear, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureCompareMode compareMode = TextureCompareMode.None, Compare compareFunc = Compare.Less)
        {
            UniqueIdentifier = Guid.NewGuid();
            PixelFormat = colorFormat;
            Width = width;
            Height = height;
            DoGenerateMipMaps = generateMipMaps;
            FilterMode = filterMode;
            WrapMode = wrapMode;
            TextureType = textureType;
            CompareMode = compareMode;
            CompareFunc = compareFunc;
        }

        private bool _disposed;

        /// <summary>
        /// Fire dispose mesh event
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                TextureChanged?.Invoke(this, new TextureEventArgs(this, TextureChangedEnum.Disposed));

                _disposed = true;
            }
        }

        /// <summary>
        /// Fire dispose mesh event
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose(bool)"/> in order to fire TextureChanged event.
        /// </summary>
        ~WritableCubeMap()
        {
            Dispose(false);
        }
    }
}