using Fusee.Engine.Common;
using Silk.NET.OpenGL;

namespace Fusee.Engine.Imp.Graphics.SilkDesktop
{
    /// Type that sums up infos about the pixels OpenGl needs to create textures on the gpu.
    internal struct TexturePixelInfo : ITexturePixelInfo
    {
        public GLEnum InternalFormat;
        public PixelFormat Format;
        public PixelType PxType;
        public int RowAlignment;
    }
}