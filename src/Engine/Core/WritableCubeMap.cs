using Fusee.Base.Common;
using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core
{   

    public class WritableCubeMap : IWritableCubeMap
    {
        private IWritableTexture[] _textures { get; }

        public RenderTargetTextures TextureType { get ; private set; }

        public IWritableTexture PositiveX => _textures[0];

        public IWritableTexture NegativeX => _textures[1];

        public IWritableTexture PositiveY => _textures[2];

        public IWritableTexture NegativeY => _textures[3];

        public IWritableTexture PositiveZ => _textures[4];

        public IWritableTexture NegativeZ => _textures[5];

        /// <summary>
        ///  Creates a new instance of type WritableCubeMap.
        /// </summary>
        /// <param name="textureType">The type of the texture.</param>
        /// <param name="TextureResolution">The resolution of the faces.</param>
        public WritableCubeMap(RenderTargetTextures textureType, TexRes TextureResolution)
        {
            TextureType = textureType;
            _textures = new IWritableTexture[6];

            switch (textureType)
            {
                case RenderTargetTextures.G_POSITION:
                    for (int i = 0; i < 6; i++)
                        _textures[i] = WritableTexture.CreatePosTex((int)TextureResolution, (int)TextureResolution);
                    break;
                case RenderTargetTextures.G_ALBEDO_SPECULAR:
                    for (int i = 0; i < 6; i++)
                        _textures[i] = WritableTexture.CreateAlbedoSpecularTex((int)TextureResolution, (int)TextureResolution);
                    break;
                case RenderTargetTextures.G_NORMAL:
                    for (int i = 0; i < 6; i++)
                        _textures[i] = WritableTexture.CreateNormalTex((int)TextureResolution, (int)TextureResolution);
                    break;
                case RenderTargetTextures.G_DEPTH:
                    for (int i = 0; i < 6; i++)
                        _textures[i] = WritableTexture.CreateDepthTex((int)TextureResolution, (int)TextureResolution);
                    break;
                case RenderTargetTextures.G_SSAO:
                    for (int i = 0; i < 6; i++)
                        _textures[i] = WritableTexture.CreateSSAOTex((int)TextureResolution, (int)TextureResolution);
                    break;
                default:
                    throw new ArgumentException("Texture type not supported!");
            }
        }

        public ITexture GetTextureByFace(CubeMapFaces face)
        {
            switch (face)
            {
                case CubeMapFaces.POSITIVE_X:
                    return PositiveX;
                case CubeMapFaces.NEGATIVE_X:
                    return NegativeX;
                case CubeMapFaces.POSITIVE_Y:
                    return PositiveY;
                case CubeMapFaces.NEGATIVE_Y:
                    return NegativeY;
                case CubeMapFaces.POSITIVE_Z:
                    return PositiveZ;
                case CubeMapFaces.NEGATIVE_Z:
                    return NegativeZ;
                default:
                    throw new ArgumentException("Unsupported face type!");                    
            }
        }
        
    }


}
