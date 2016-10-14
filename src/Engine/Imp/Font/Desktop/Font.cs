using SharpFont;
using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Font.Desktop
{
    internal class FontImp : IFontImp
    {
        internal Face Face;

        // texture atlas
        public ITexture TexAtlas { get; internal set; }

        public int Width { get; internal set; }
        public int Height { get; internal set; }
        
        // font settings
        public uint FontSize {get; internal set; }
        public bool UseKerning { get; set; }

        // char info
        public CharInfoStruct[] CharInfo { get; internal set; }
    }
}