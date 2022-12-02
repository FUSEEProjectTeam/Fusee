using Fusee.Base.Common;
using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// The engines abstraction for OpenGL's Array Texture.
    /// </summary>
    public class WritableArrayTexture : IWritableArrayTexture
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
        /// The number of layers the array texture has.
        /// </summary>
        public int Layers { get; private set; }

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
        /// Opaque handle to the texture object on the gpu.
        /// </summary>
        public ITextureHandle TextureHandle { get; internal set; }

        /// <summary>
        /// Creates a new instance of type "WritableArrayTexture".
        /// </summary>
        /// <param name="layers">The number of sub-textures in this array texture.</param>
        /// <param name="texType">Defines the type of the render texture.</param>
        /// <param name="colorFormat">The color format of the texture, <see cref="ImagePixelFormat"/></param>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="generateMipMaps">Defines if mipmaps are created.</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        /// <param name="compareMode">The textures compare mode. If uncertain, leaf on NONE, this is only important for depth (shadow) textures (<see cref="TextureCompareMode"/>).</param>
        /// <param name="compareFunc">The textures compare function. If uncertain, leaf on LEESS, this is only important for depth (shadow) textures and if the CompareMode isn't NONE (<see cref="Compare"/>)</param>
        public WritableArrayTexture(int layers, RenderTargetTextureTypes texType, ImagePixelFormat colorFormat, int width, int height, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.NearestMipmapLinear, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureCompareMode compareMode = TextureCompareMode.None, Compare compareFunc = Compare.Less)
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
            Layers = layers;
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
                if (disposing)
                {

                }

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
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose(bool)"/> in order to fire TextureChanged event.
        /// </summary>
        ~WritableArrayTexture()
        {
            Dispose(false);
        }
    }
}