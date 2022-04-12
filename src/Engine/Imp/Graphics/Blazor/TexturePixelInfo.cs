using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Graphics.Blazor
{
    /// <summary>
    /// Type that sums up infos about the pixels OpenGl needs to create textures on the GPU.
    /// </summary>
    internal struct TexturePixelInfo : ITexturePixelInfo
    {
        public uint InternalFormat;
        public uint Format;
        public uint PxType;
    }
}