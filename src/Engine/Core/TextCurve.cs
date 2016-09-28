using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Base.Core;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class TextCurve
    {
        private readonly string _text;
        private readonly Font _font;
        public TextCurve(string text, Font font)
        {
            _text = text;
            _font = font;
        }
        private List<Curve> GetTextCurves()
        {
            var curves = new List<Curve>();
            foreach (var t in _text)
            {
                var curve = _font.GetGlyphCurve(t);
                curves.Add(curve);
            }
            return curves;
        }

        private void CalcAdvanceAndKerning(IList<Curve> textCurves)
        {
            var advance = 0f;
            var advanceComp = 0f;
            var kerning = 0f;
            var kerningComp = 0f;

            for (int i = 0; i < textCurves.Count; i++)
            {
                advanceComp = advanceComp + advance;
                kerningComp = kerningComp + kerning;

                foreach (var part in textCurves[i].CurveParts)
                {
                    foreach (var segment in part.CurveSegments)
                    {
                        for (var j = 0; j < segment.Vertices.Count; j++)
                        {
                            segment.Vertices[j] = new float3(segment.Vertices[j].x + advanceComp + kerningComp, segment.Vertices[j].y, segment.Vertices[j].z);
                        }
                    }
                }
                advance = _font.GetUnscaledAdvance(_text[i]);

                if (i + 1 < _text.Length)
                    kerning = _font.GetUnscaledKerning(_text[i], _text[i + 1]);
            }
        }

        public Curve GetTextCurve()
        {
            var curves = GetTextCurves();
            CalcAdvanceAndKerning(curves);

            return Curve.CombineCurve(curves);
        }

    }


}

