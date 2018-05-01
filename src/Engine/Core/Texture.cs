using System;
using System.Collections.Generic;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    public class Texture : ITexture, IDisposable
    {
        #region RenderContext Asset Management
        // Event of mesh Data changes
        /// <summary>
        /// TextureChanged event notifies observing TextureManager about property changes and the Texture's disposal.
        /// </summary>
        public event EventHandler<TextureDataEventArgs> TextureChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Textures's uniqueness in the current session.
        /// </summary>
        public readonly Suid SessionUniqueIdentifier = Suid.GenerateSuid();
        #endregion

        private ImageData _imageData;

        #region Properties
        public int Width
        {
            get { return _imageData.Width; }
        }
        public int Height
        {
            get { return _imageData.Height; }
        }
        public byte[] PixelData
        {
            get { return _imageData.PixelData; }
        }

        public ImagePixelFormat PixelFormat
        {
            get { return _imageData.PixelFormat; }
        }

        /// <summary>
        /// Checks if an image contains no data by checking if <see cref="Width"/> or <see cref="Height"/> is 0.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => (Width <= 0 || Height <= 0);

        #endregion

        public Texture(byte[] pixelData, int width, int height, ImagePixelFormat colorFormat)
        {
            _imageData = new ImageData(pixelData, width, height, colorFormat);
        }

        public Texture(ImageData imageData)
        {
            _imageData = imageData;
        }

        public void Blt(int xDst, int yDst, IImageData src, int xSrc = 0, int ySrc = 0, int width = 0, int height = 0)
        {
            // Blit into private _imageData
            _imageData.Blt(xDst,yDst, src, xSrc, ySrc, width, height);

            // Fire Texture Changed Event -> Update TextureRegion on GPU
            var del = this.TextureChanged;
            if (del != null)
            {
                del(this, new TextureDataEventArgs(this, TextureChangedEnum.RegionChanged, xDst, yDst, width, height));
            }
        }

        public IEnumerator<ScanLine> ScanLines(int xSrc = 0, int ySrc = 0, int width = 0, int height = 0)
        {
            return _imageData.ScanLines(xSrc, ySrc, width, height);
        }

        /// <summary>
        /// Implementation of the <see cref="IDisposable"/> interface.
        /// </summary>
        public void Dispose()
        {
            var del = TextureChanged;
            if (del != null)
            {
                del(this, new TextureDataEventArgs(this, TextureChangedEnum.Disposed));
            }
        }

        /// <summary>
        /// Destructor calls <see cref="Dispose"/> in order to fire TextureChanged event.
        /// </summary>
        ~Texture()
        {
            Dispose();
        }

    }
}