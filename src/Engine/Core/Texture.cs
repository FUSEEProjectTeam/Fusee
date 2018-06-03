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

        public Texture(IImageData imageData)
        {
            _imageData = new ImageData(
                new byte[imageData.Width * imageData.Height * imageData.PixelFormat.BytesPerPixel],
                imageData.Width, imageData.Height, imageData.PixelFormat);
            _imageData.Blt(0,0, imageData);
        }

        public void Blt(int xDst, int yDst, IImageData src, int xSrc = 0, int ySrc = 0, int width = 0, int height = 0)
        {
            // Blit into private _imageData
            _imageData.Blt(xDst, yDst, src, xSrc, ySrc, width, height);
            if (width == 0)
                width = src.Width;
            if (height == 0)
                height = src.Height;

            ClipBlt(ref xDst, Width, ref xSrc, src.Width, ref width);
            ClipBlt(ref yDst, Height, ref ySrc, src.Height, ref height);

            if (width <= 0 || height <= 0)
                return;

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

        /// <summary>
        /// Performs clipping along one dimension of a blt operation.
        /// </summary>
        /// <param name="iDst">The destination coordinate.</param>
        /// <param name="sizeDst">The size of the destination buffer.</param>
        /// <param name="iSrc">The source coordinate.</param>
        /// <param name="sizeSrc">The size of the source coordinate.</param>
        /// <param name="sizeBlk">The size of the block to copy.</param>
        /// <remarks>
        ///    All parameters decorated with "ref" might be altered to avoid out-of-bounds indices.
        ///    If the resulting number of items to copy is 0, only sizeBlk will be set to 0. No other
        ///    ref-parameter will be altered then.
        /// </remarks>
        private static void ClipBlt(ref int iDst, int sizeDst, ref int iSrc, int sizeSrc, ref int sizeBlk)
        {
            // Adjust left border
            // The negative number with the biggest magnitude of negative start indices (or 0, if both are 0 or bigger).
            // int iDeltaL = M.Min(0, M.Min(iDst, iSrc));
            int iDeltaL = (iDst < iSrc) ? iDst : iSrc;
            if (iDeltaL > 0)
                iDeltaL = 0;

            // Adjust right border
            // The biggest overlap over the right border (or 0 if no overlap).
            // int iDeltaR = M.Max(0, M.Max(iDst + sizeBlk - sizeDst, iSrc + sizeBlk - sizeSrc));
            int dstRB = iDst + sizeBlk - sizeDst;
            int srcRB = iSrc + sizeBlk - sizeSrc;
            int iDeltaR = (dstRB > srcRB) ? dstRB : srcRB;
            if (iDeltaR < 0)
                iDeltaR = 0;

            iDst -= iDeltaL;
            iSrc -= iDeltaL;
            sizeBlk += iDeltaL;
            sizeBlk -= iDeltaR;
            if (sizeBlk < 0)
                sizeBlk = 0;
        }

    }
}