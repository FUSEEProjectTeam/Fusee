﻿using System;
using System.Collections.Generic;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Texture implements <see cref="IImageData"/> and is used inside <see cref="RenderContext"/> to render bitmaps in fusee.
    /// </summary>
    public class Texture : ITexture
    {
        #region RenderContext Asset Management
        
        /// <summary>
        /// TextureChanged event notifies observing TextureManager about property changes and the Texture's disposal.
        /// </summary>
        public event EventHandler<TextureEventArgs> TextureChanged;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Textures's uniqueness in the current session.
        /// </summary>
        public Suid SessionUniqueIdentifier { get; private set; }
        #endregion

        private readonly ImageData _imageData;

        #region Properties

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int Width
        {
            get { return _imageData.Width; }
        }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        public int Height
        {
            get { return _imageData.Height; }
        }

        /// <summary>
        /// The raw Pixeldata byte buffer. This byte buffer will be uploaded to the GPU inside <see cref="RenderContext"/>
        /// </summary>
        public byte[] PixelData
        {
            get { return _imageData.PixelData; }
        }

        /// <summary>
        /// Provides additional information abut this Texture's pixel encoding.
        /// </summary>
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

        #endregion

        /// <summary>
        /// Creates a new instance of type Texture.
        /// </summary>
        protected Texture() { }

        /// <summary>
        /// Constructor initializes a Texture from a pixelData byte buffer, width and height in pixels and <see cref="ImagePixelFormat"/>.
        /// </summary>
        /// <param name="pixelData">The raw pixelData byte buffer that makes up the texture.</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="colorFormat">Provides additional information about pixel encoding.</param>
        /// <param name="generateMipMaps">Defines if mipmaps are created.</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        public Texture(byte[] pixelData, int width, int height, ImagePixelFormat colorFormat, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.LINEAR, TextureWrapMode wrapMode = TextureWrapMode.REPEAT)
        {
            SessionUniqueIdentifier = Suid.GenerateSuid();
            _imageData = new ImageData(pixelData, width, height, colorFormat);
            DoGenerateMipMaps = generateMipMaps;
            FilterMode = filterMode;
            WrapMode = wrapMode;            
        }

        /// <summary>
        /// Initialize a Texture from an existing IImageData. The input IImageData will be copied into this Texture via <seealso cref="Blt"/> command.
        /// </summary>
        /// <param name="imageData">The existing <see cref="IImageData"/> that will be copied to initialize a Texture instance.</param>
        /// <param name="generateMipMaps">Defines if mipmaps are created.</param>
        /// <param name="filterMode">Defines the filter mode <see cref="TextureFilterMode"/>.</param>
        /// <param name="wrapMode">Defines the wrapping mode <see cref="TextureWrapMode"/>.</param>
        public Texture(IImageData imageData, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.LINEAR, TextureWrapMode wrapMode = TextureWrapMode.REPEAT)
        {
            SessionUniqueIdentifier = Suid.GenerateSuid();
            _imageData = new ImageData(
                new byte[imageData.Width * imageData.Height * imageData.PixelFormat.BytesPerPixel],
                imageData.Width, imageData.Height, imageData.PixelFormat);
            _imageData.Blt(0,0, imageData);
            DoGenerateMipMaps = generateMipMaps;
            FilterMode = filterMode;
            WrapMode = wrapMode;
        }

        /// <summary>
        /// Copies a rectangular block of pixel data from a source image to a this image (Blt = BlockTransfer).
        /// </summary>
        /// <param name="xDst">The x destination coordinate (where to place the block within dst).</param>
        /// <param name="yDst">The y destination coordinate (where to place the block within dst).</param>
        /// <param name="src">The source image.</param>
        /// <param name="xSrc">The x source coordinate (where to start copying from within src).</param>
        /// <param name="ySrc">The y source coordinate (where to start copying from within src).</param>
        /// <param name="width">The width of the block to copy. (default is src.Width).</param>
        /// <param name="height">The height of the block to copy (default is src.Height).</param>
        /// <remarks>
        ///     All specified parameters are clipped to avoid out-of-bounds indices. No warnings or exceptions are issued
        ///     in case clipping results in a smaller or an empty block.
        /// </remarks>
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
            this.TextureChanged?.Invoke(this, new TextureEventArgs(this, TextureChangedEnum.RegionChanged, xDst, yDst, width, height));
        }

        /// <summary>
        /// Retrieve a rectangular block from this image that is represented by horizontal ScanLines from top to bottom along width and height, beginning at offsets xSrc and ySrc.
        /// </summary>
        /// <param name="xSrc">x offset in pixels.</param>
        /// <param name="ySrc">y offset in pixels.</param>
        /// <param name="width">width of ScanLines in pixels.</param>
        /// <param name="height">Height describes how many ScanLines will be returned.</param>
        /// <returns>Returns a rectangular block of horizontal ScanLine instances.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IEnumerator<ScanLine> ScanLines(int xSrc = 0, int ySrc = 0, int width = 0, int height = 0)
        {
            return _imageData.ScanLines(xSrc, ySrc, width, height);
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
            int dstRb = iDst + sizeBlk - sizeDst;
            int srcRb = iSrc + sizeBlk - sizeSrc;
            int iDeltaR = (dstRb > srcRb) ? dstRb : srcRb;
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