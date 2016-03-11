using System;
using Fusee.Base.Common;
using JSIL.Meta;

namespace Fusee.Base.Imp.Web
{
    /// <summary>
    /// Font implementation for web builds using opentype.js.
    /// </summary>
    public class FontImp : IFontImp
    {
        [JSExternal]
        public FontImp(object storage)
        {
            throw new NotImplementedException();
        }

        public bool UseKerning { get; set; }
        public uint PixelHeight { get; set; }

        [JSExternal]
        public GlyphInfo GetGlyphInfo(uint c)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public ImageData RenderGlyph(uint c, out int bitmapLeft, out int bitmapTop)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public float GetKerning(uint leftC, uint rightC)
        {
            throw new NotImplementedException();
        }
    }
}
