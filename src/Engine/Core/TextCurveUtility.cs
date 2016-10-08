using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class TextCurveUtility
    {
        private readonly string _text;
        private readonly Font _font;

        public TextCurveUtility(string text, Font font)
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

        private void CalcCurvesAdvanceAndKerning(IList<Curve> textCurves)
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
                            segment.Vertices[j] = new float3(segment.Vertices[j].x + advanceComp + kerningComp,
                                segment.Vertices[j].y, segment.Vertices[j].z);
                        }
                    }
                }
                advance = _font.GetUnscaledAdvance(_text[i]);

                if (i + 1 < _text.Length)
                    kerning = _font.GetUnscaledKerning(_text[i], _text[i + 1]);
            }
            
        }

        public List<float3> GetTextOutline(int seg)
        {
            var advance = 0f;
            var advanceComp = 0f;
            var kerning = 0f;
            var kerningComp = 0f;

            var textCurves = GetTextCurves();
            List<float3> combOutline = new List<float3>();

            for (var i = 0; i < textCurves.Count; i++)
            {
                advanceComp = advanceComp + advance;
                kerningComp = kerningComp + kerning;

                var outline =  textCurves[i].CalcUniformPolyline(seg).ToList();

                for (var j = 0; j < outline.Count; j++)
                {
                    outline[j] = new float3(outline[j].x + advanceComp + kerningComp,
                        outline[j].y, outline[j].z);
                }
                combOutline.AddRange(outline);

                advance = _font.GetUnscaledAdvance(_text[i]);

                if (i + 1 < _text.Length)
                    kerning = _font.GetUnscaledKerning(_text[i], _text[i + 1]);
            }
            return combOutline;
        }

        public Curve GetCombinedTextCurve()
        {
            var curves = GetTextCurves();
            CalcCurvesAdvanceAndKerning(curves);

            return Curve.CombineCurve(curves);
        }

        public IEnumerable<float3> GetTextControlPoints()
        {
            var curves = GetTextCurves();
            CalcCurvesAdvanceAndKerning(curves);

            foreach (var c in curves)
            {
                foreach (var p in c.CurveParts)
                {
                    foreach (var s in p.CurveSegments)
                    {
                        foreach (var v in s.Vertices)
                        {
                            yield return v;
                        }
                    }
                }
            }
        }
    }
}

