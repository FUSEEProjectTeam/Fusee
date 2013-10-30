using SharpFont;

namespace Fusee.Engine
{
    internal class Font : IFont
    {
        internal Face Face;

        // texture atlas
        internal ITexture TexAtlas;

        public int Width { get; internal set; }
        public int Height { get; internal set; }
        
        // font settings
        public uint FontSize {get; internal set; }
        public bool UseKerning { get; set; }

        // char info
        public CharInfoStruct[] CharInfo { get; internal set; }
    }
}