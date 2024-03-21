using Fusee.Base.Common;
using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use writable textures if you want to render into a texture.
    /// Does NOT offer access to the pixel data.
    /// </summary>
    public class WritableMultisampleTexture : IWritableTexture
    {

        /// <summary>
        /// TextureChanged event notifies observing TextureManager about property changes and the Texture's disposal.
        /// </summary>
        public event EventHandler<TextureEventArgs>? TextureChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Textures's uniqueness in the current session.
        /// </summary>
        public Guid UniqueIdentifier { get; }

        /// <summary>
        /// Type of the render texture, <see cref="RenderTargetTextureTypes"/>.
        /// </summary>
        public RenderTargetTextureTypes TextureType { get; private set; }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// PixelFormat provides additional information about pixel encoding.
        /// </summary>
        public ImagePixelFormat PixelFormat { get; private set; }

        /// <summary>
        /// How many samples are being used for multi-sampling
        /// </summary>
        public int MultisampleFactor { get; private set; }

        /// <summary>
        /// Specifies the texture's comparison mode, see <see cref="TextureCompareMode"/>.
        /// </summary>
        public TextureCompareMode CompareMode { get; private set; }

        /// <summary>
        /// Specifies the texture's comparison function, see <see cref="Compare"/>.
        /// </summary>
        public Compare CompareFunc { get; private set; }

        /// <summary>
        /// Renderable result texture.
        /// </summary>
        public WritableTexture InternalResultTexture { get; private set; }

        /// <summary>
        /// Opaque handle to texture, this is the internal handle, which can be used. However this is not yet sampled to one result texture
        /// This is done after rendering by blitting the result into a new <see cref="WritableTexture"/> object.
        /// To use the <see cref="ITextureHandle"/> with any OpenGL Texture2d method one needs to use the <see cref="TextureHandle"/>!
        /// </summary>
        public ITextureHandle InternalTextureHandle
        {
            get; internal set;
        }

        /// <summary>
        /// Resulting <see cref="ITextureHandle"/> after blitting and sampling is finished
        /// Can be used as any other <see cref="WritableTexture"/>, the content is sampled and ready to go.
        /// </summary>
        public ITextureHandle TextureHandle
        {
            get
            {
                return InternalResultTexture.TextureHandle;
            }
        }

        /// <summary>
        /// Specifies if mipmaps are created for this texture.
        /// </summary>
        public bool DoGenerateMipMaps => false;

        /// <summary>
        /// Specifies the texture's wrap mode, see <see cref="TextureWrapMode"/>.
        /// </summary>
        public TextureWrapMode WrapMode { get; private set; }

        /// <summary>
        /// Specifies the texture's filter mode, see <see cref="TextureWrapMode"/>.
        /// </summary>
        public TextureFilterMode FilterMode { get; private set; }

        private bool disposedValue;

        /// <summary>
        /// Creates a new instance of type "WritableTexture".
        /// </summary>
        /// <param name="texType">Defines the type of the render texture.</param>
        /// <param name="colorFormat">The color format of the texture, <see cref="ImagePixelFormat"/></param>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="multisampleFactor">Define how many samples are being used to sample this texture, default: 4</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        /// <param name="compareMode">The textures compare mode. If uncertain, leaf on NONE, this is only important for depth (shadow) textures (<see cref="TextureCompareMode"/>).</param>
        /// <param name="compareFunc">The textures compare function. If uncertain, leaf on LEESS, this is only important for depth (shadow) textures and if the CompareMode isn't NONE (<see cref="Compare"/>)</param>
        public WritableMultisampleTexture(RenderTargetTextureTypes texType, ImagePixelFormat colorFormat, int width, int height, int multisampleFactor = 4, TextureFilterMode filterMode = TextureFilterMode.NearestMipmapLinear, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureCompareMode compareMode = TextureCompareMode.None, Compare compareFunc = Compare.Less)
        {
            //var maxSamples = rc.GetHardwareCapabilities(HardwareCapability.MaxSamples);
            //if(maxSamples == 0)
            //{
            //    throw new NotSupportedException($"Multisample texture is not supported for this platform");
            //}
            //
            //if (multisampleFactor > maxSamples || multisampleFactor == 0)
            //{
            //    throw new ArgumentException($"Multisample texture factor {multisampleFactor} is either '0' or too big. GL_MAX_SAMPLES for this ImagePixelFormat is {maxSamples}");
            //}

            UniqueIdentifier = Guid.NewGuid();
            PixelFormat = colorFormat;
            Width = width;
            Height = height;
            FilterMode = filterMode;
            WrapMode = wrapMode;
            TextureType = texType;
            CompareMode = compareMode;
            CompareFunc = compareFunc;
            MultisampleFactor = multisampleFactor;

            InternalResultTexture = WritableTexture.CreateAlbedoTex(Width, Height, PixelFormat);
        }

        /// <summary>
        /// Convenience method, creates a new instance of type "WritableTexture".
        /// </summary>
        /// <param name="width">Width in px.</param>
        /// <param name="height">Height in px.</param>
        /// <param name="multisampleFactor">Define how many samples are being used to sample this texture, default: 4</param>
        public static WritableMultisampleTexture CreateAlbedoTex(int width, int height, int multisampleFactor = 4)
        {
            var pxFormat = new ImagePixelFormat(ColorFormat.RGBA);
            var resTex = new WritableMultisampleTexture(RenderTargetTextureTypes.Albedo, pxFormat, width, height, multisampleFactor);

            resTex.InternalResultTexture = WritableTexture.CreateAlbedoTex(resTex.Width, resTex.Height, pxFormat);

            return resTex;
        }

        /// <summary>
        /// Fire dispose texture event
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                TextureChanged?.Invoke(this, new TextureEventArgs(this, TextureChangedEnum.Disposed));

                disposedValue = true;
            }
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose()"/> in order to fire TextureChanged event.
        /// </summary>
        ~WritableMultisampleTexture()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Fire dispose texture event
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}