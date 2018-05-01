using System;
using Fusee.Base.Common;
using Fusee.Math.Core;
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
        public IImageData RenderGlyph(uint c, out int bitmapLeft, out int bitmapTop)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public float GetKerning(uint leftC, uint rightC)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public float GetUnscaledKerning(uint leftC, uint rightC)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public Curve GetGlyphCurve(uint c)
        {
            throw new NotImplementedException();
        }

        [JSExternal]
        public float GetUnscaledAdvance(uint c)
        {
            throw new NotImplementedException();
        }
    }
}
