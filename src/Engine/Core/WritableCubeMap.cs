using Fusee.Base.Common;
using Fusee.Engine.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{

    public class WritableCubeMap : Texture, IWritableCubeMap
    {

        public bool DoGenerateMipMaps { get; set; }

        public TextureWrapMode WrapMode { get; set; }

        public TextureFilterMode FilterMode { get; set; }

        public bool IsEmpty { get; set; }

        public RenderTargetTextureTypes TextureType { get; private set; }
       
        public ITextureHandle TextureHandle { get; set; }

        /// <summary>
        /// Should be containing zeros by default. If you want to use the PixelData directly it gets blted from the graphics card (not implemented yet).
        /// </summary>
        public new byte[] PixelData { get; private set; } //TODO: (?) get px data (and _imageData) from graphics card on PixelData get()

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public new int Width
        {
            get;
            private set;
        }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        public new int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// PixelFormat provides additional information about pixel encoding.
        /// </summary>
        public new ImagePixelFormat PixelFormat
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
        /// <param name="sizePerFace">The resolution of the faces.</param>
        public WritableCubeMap(RenderTargetTextureTypes textureType, ImagePixelFormat colorFormat, int width, int height, bool generateMipMaps = true, TextureFilterMode filterMode = TextureFilterMode.LINEAR, TextureWrapMode wrapMode = TextureWrapMode.REPEAT)
        {           
            PixelFormat = colorFormat;
            Width = width;
            Height = height;
            DoGenerateMipMaps = generateMipMaps;
            FilterMode = filterMode;
            WrapMode = wrapMode;
            TextureType = textureType;
        }

        //private IWritableTexture[] _textures { get; set; }

        //public IWritableTexture PositiveX => _textures[0];

        //public IWritableTexture NegativeX => _textures[1];

        //public IWritableTexture PositiveY => _textures[2];

        //public IWritableTexture NegativeY => _textures[3];

        //public IWritableTexture PositiveZ => _textures[4];

        //public IWritableTexture NegativeZ => _textures[5];

        //public ITexture GetTextureByFace(CubeMapFaces face)
        //{            
        //    switch (face)
        //    {
        //        case CubeMapFaces.POSITIVE_X:
        //            return PositiveX;
        //        case CubeMapFaces.NEGATIVE_X:
        //            return NegativeX;
        //        case CubeMapFaces.POSITIVE_Y:
        //            return PositiveY;
        //        case CubeMapFaces.NEGATIVE_Y:
        //            return NegativeY;
        //        case CubeMapFaces.POSITIVE_Z:
        //            return PositiveZ;
        //        case CubeMapFaces.NEGATIVE_Z:
        //            return NegativeZ;
        //        default:
        //            throw new ArgumentException("Unsupported face type!");
        //    }
        //}

        public void Blt(int xDst, int yDst, IImageData src, int xSrc = 0, int ySrc = 0, int width = 0, int height = 0)
        {
            //possibly fill _texture array here.....
            throw new NotImplementedException();
        }

        public IEnumerator<ScanLine> ScanLines(int xSrc = 0, int ySrc = 0, int width = 0, int height = 0)
        {
            throw new NotImplementedException();
        }
    }
}
