using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Math.Core;

namespace Fusee.Engine.Examples.ThreeDFont.Core
{
    public class ThreeDFontHelper
    {
        private readonly string _text;
        private readonly Font _font;

        public ThreeDFontHelper(string text, Font font)
        {
            _text = text;
            _font = font;
        }

        //Reads all curves from a given text and retruns them as a List of Curves
        private IList<Curve> GetTextCurves()
        {
            var curves = new List<Curve>();
            foreach (var t in _text)
            {
                var curve = _font.GetGlyphCurve(t);
                curves.Add(curve);
            }
            return curves;
        }

        //Returns all control points of a given text
        public IEnumerable<float3> GetTextVertices()
        {
            var advance = 0f;
            var advanceComp = 0f;
            var kerning = 0f;
            var kerningComp = 0f;

            var textCurves = GetTextCurves();
            var controlPoints = new List<float3>();

            for (var i = 0; i < textCurves.Count; i++)
            {
                var outline = new List<float3>();

                foreach (var p in textCurves[i].CurveParts)
                {
                    foreach (var s in p.CurveSegments)
                    {
                        foreach (var v in s.Vertices)
                        {
                            outline.Add(v);
                        }
                    }
                }

                advanceComp = advanceComp + advance;
                kerningComp = kerningComp + kerning;

                for (var j = 0; j < outline.Count; j++)
                {
                    outline[j] = new float3(outline[j].x + advanceComp + kerningComp,
                        outline[j].y, outline[j].z);
                }
                advance = _font.GetUnscaledAdvance(_text[i]);

                if (i + 1 < _text.Length)
                    kerning = _font.GetUnscaledKerning(_text[i], _text[i + 1]);

                controlPoints.AddRange(outline);
            }
            return controlPoints;
        }

        public IList<float3> GetTextVertices(int seg)
        {
            var advance = 0f;
            var advanceComp = 0f;
            var kerning = 0f;
            var kerningComp = 0f;

            var textCurves = GetTextCurves();
            var combinedOutline = new List<float3>();
            
            for (var i = 0; i < textCurves.Count; i++)
            {
                var outline = textCurves[i].CalcUniformPolyline(seg).ToList();

                advanceComp = advanceComp + advance;
                kerningComp = kerningComp + kerning;

                for (var j = 0; j < outline.Count; j++)
                {
                    outline[j] = new float3(outline[j].x + advanceComp + kerningComp,
                        outline[j].y, outline[j].z);
                }
                advance = _font.GetUnscaledAdvance(_text[i]);

                if (i + 1 < _text.Length)
                    kerning = _font.GetUnscaledKerning(_text[i], _text[i + 1]);

                combinedOutline.AddRange(outline);
            }
            return combinedOutline;
        }
    }
}

