using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Serialization;
using System;

namespace Fusee.Engine.Core
{
    public class WritableCubeMap : IWritableCubeMap
    {
        /// <summary>
        /// TextureChanged event notifies observing TextureManager about property changes and the Texture's disposal.
        /// </summary>
        public event EventHandler<TextureEventArgs> TextureChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Textures's uniqueness in the current session.
        /// </summary>
        public Suid SessionUniqueIdentifier { get; private set; }

        public bool DoGenerateMipMaps { get; set; }

        public TextureWrapMode WrapMode { get; set; }

        public TextureFilterMode FilterMode { get; set; }

        public bool IsEmpty { get; set; }

        public RenderTargetTextureTypes TextureType { get; private set; }
       
        public ITextureHandle TextureHandle { get; set; }


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
        /// Creates a new instance of type "WritableTexture".
        /// </summary>
        /// <param name="colorFormat">The color format of the texture, <see cref="ImagePixelFormat"/></param>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="generateMipMaps">Defines if mipmaps are created.</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        /// <param name="textureType">The type of the texture.</param>        
        public WritableCubeMap(RenderTargetTextureTypes textureType, ImagePixelFormat colorFormat, int width, int height, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.LINEAR, TextureWrapMode wrapMode = TextureWrapMode.REPEAT)
        {
            SessionUniqueIdentifier = Suid.GenerateSuid();
            PixelFormat = colorFormat;
            Width = width;
            Height = height;
            DoGenerateMipMaps = generateMipMaps;
            FilterMode = filterMode;
            WrapMode = wrapMode;
            TextureType = textureType;
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
        ~WritableCubeMap()
        {
            Dispose();
        }
    }
}
