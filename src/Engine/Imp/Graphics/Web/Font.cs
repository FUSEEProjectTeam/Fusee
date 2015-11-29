// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Graphics.Web
{
    internal class Font : IFont
    {
        [JSExternal]
        public ITexture TexAtlas { get; }
        [JSExternal]
        public int Width { get; }
        [JSExternal]
        public int Height { get; }
        [JSExternal]
        public uint FontSize { get; }
        [JSExternal]
        public bool UseKerning { get; set; }
        [JSExternal]
        public CharInfoStruct[] CharInfo { get; }
    }
}