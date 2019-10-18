using Fusee.Engine.Common;
using OpenTK.Graphics.ES30;

namespace Fusee.Engine.Imp.Graphics.Android
{
    /// Type that sums up infos about the pixels OpenGl needs to create textures on the gpu.
    internal struct TexturePixelInfo : ITexturePixelInfo
    {
        public PixelInternalFormat InternalFormat;
        public PixelFormat Format;
        public PixelType PxType;
    }
}