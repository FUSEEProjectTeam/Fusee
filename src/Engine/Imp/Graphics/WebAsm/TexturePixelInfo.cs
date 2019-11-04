using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.WebAsm
{
    /// Type that sums up infos about the pixels OpenGl needs to create textures on the gpu.
    internal struct TexturePixelInfo : ITexturePixelInfo
    {
        public uint InternalFormat;
        public uint Format;
        public uint PxType;
    }
}
