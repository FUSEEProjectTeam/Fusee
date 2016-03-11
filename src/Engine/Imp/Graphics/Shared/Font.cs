// using SharpFont;
using Fusee.Engine.Common;

#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID
namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    internal class Font : IFont
    {
        // internal Face Face;

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