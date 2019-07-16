using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Math.Core;
// using SharpFont;

namespace Fusee.Base.Imp.WebAsm
{
    /// <summary>
    /// Font implementation using freetype (262) / SharpFont.
    /// </summary>
    public class FontImp : IFontImp
    {
        public bool UseKerning { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint PixelHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Curve GetGlyphCurve(uint c)
        {
            throw new NotImplementedException();
        }

        public GlyphInfo GetGlyphInfo(uint c)
        {
            throw new NotImplementedException();
        }

        public float GetKerning(uint leftC, uint rightC)
        {
            throw new NotImplementedException();
        }

        public float GetUnscaledAdvance(uint c)
        {
            throw new NotImplementedException();
        }

        public float GetUnscaledKerning(uint leftC, uint rightC)
        {
            throw new NotImplementedException();
        }

        public IImageData RenderGlyph(uint c, out int bitmapLeft, out int bitmapTop)
        {
            throw new NotImplementedException();
        }
    }
}
