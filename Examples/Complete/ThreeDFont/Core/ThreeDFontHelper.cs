﻿using Fusee.Base.Core;
using Fusee.Jometri;
using Fusee.Math.Core;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Examples.ThreeDFont.Core
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

        //Reads all curves from a given text and retruns them as a List of Curves.
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

        //Returns all control points of a given text.
        public IEnumerable<float3> GetTextControlPoints()
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

        //Returns all control points of a given text calculated with a uniform value of t.
        public IList<float3> GetTextVerticesUniformly(int seg)
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

        //Returns all control points of a given text calculated adaptively by testing the angle.
        //"angle" determines how far the angle between the two vectors(start point random point and random point end point) may vary from 180°.
        public IList<float3> GetTextVerticesWAngle(int angle)
        {
            var advance = 0f;
            var advanceComp = 0f;
            var kerning = 0f;
            var kerningComp = 0f;

            var textCurves = GetTextCurves();
            var combinedOutline = new List<float3>();

            for (var i = 0; i < textCurves.Count; i++)
            {
                var outline = textCurves[i].CalcAdaptivePolyline(angle).ToList();

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

        //Returns all control points of a given text by testing the acreage of a triangle.
        public IList<float3> GetTextVerticesWArcreage(float arcreage)
        {
            var advance = 0f;
            var advanceComp = 0f;
            var kerning = 0f;
            var kerningComp = 0f;

            var textCurves = GetTextCurves();
            var combinedOutline = new List<float3>();

            for (var i = 0; i < textCurves.Count; i++)
            {
                var outline = textCurves[i].CalcAdaptivePolyline(arcreage).ToList();

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

        //Returns all control points of a given text by testing the angle.
        public IList<PolyBoundary> GetTextOutlinesWAngle(int angle)
        {
            var advance = 0f;
            var advanceComp = 0f;
            var kerning = 0f;
            var kerningComp = 0f;

            var textCurves = GetTextCurves();
            var textOutlines = new List<PolyBoundary>();

            for (var i = 0; i < textCurves.Count; i++)
            {
                advanceComp = advanceComp + advance + 0.1f; // 0.1f offset to make sure letters do not have common vertices (example XY Lato).
                kerningComp = kerningComp + kerning;

                foreach (var part in textCurves[i].CurveParts)
                {
                    var outlinePoints = part.CalcAdaptivePolyline(angle).ToList();
                    for (var j = 0; j < outlinePoints.Count; j++)
                    {
                        outlinePoints[j] = new float3(outlinePoints[j].x + advanceComp + kerningComp,
                            outlinePoints[j].y, outlinePoints[j].z);
                    }
                    outlinePoints.Reverse();

                    if (outlinePoints.IsCounterClockwise())
                    {
                        var outline = new PolyBoundary
                        {
                            IsOuter = true,
                            Points = outlinePoints
                        };
                        textOutlines.Add(outline);
                    }
                    else
                    {
                        var outline = new PolyBoundary
                        {
                            IsOuter = false,
                            Points = outlinePoints
                        };
                        textOutlines.Add(outline);
                    }
                }

                advance = _font.GetUnscaledAdvance(_text[i]);

                if (i + 1 < _text.Length)
                    kerning = _font.GetUnscaledKerning(_text[i], _text[i + 1]);
            }
            return textOutlines;
        }
    }
}