using Fusee.Engine.Common;
using OpenTK.Graphics.OpenGL;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    /// Type that sums up infos about the pixels OpenGl needs to create textures on the gpu.
    internal struct TexturePixelInfo : ITexturePixelInfo
    {
        public PixelInternalFormat InternalFormat;
        public PixelFormat Format;
        public PixelType PxType;
        public SizedInternalFormat SizedInternalFormat;
    }
}
