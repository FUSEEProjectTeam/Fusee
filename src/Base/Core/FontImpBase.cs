using Fusee.Base.Common;
using Fusee.Math.Core;
using SixLabors.Fonts;
using SixLabors.Fonts.Unicode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Base.Core
{
    /// <summary>
    /// Basic font implementation using Sixlabors.Fonts
    /// </summary>
    public abstract class FontImpBase
    {
        /// <summary>
        /// The font instance generated from a font collection with only one font
        /// </summary>
        protected SixLabors.Fonts.Font _font;

        /// <summary>
        /// A font collection can contain several fonts
        /// </summary>
        protected FontCollection _collection;

        private uint _pixelHeight = 18;

        /// <summary>
        /// Use kerning
        /// </summary>
        public bool UseKerning { get; set; } = false;

        /// <summary>
        /// The current monitor's dots per inch.
        /// </summary>
        public int Dpi = 72;

        /// <summary>
        /// Gets and sets the currently used pixel height
        /// </summary>
        public uint PixelHeight
        {
            get => _pixelHeight;

            set
            {
                _pixelHeight = value;
                _font = _collection.Families.AsEnumerable().First().CreateFont(_pixelHeight);
            }
        }

        /// <summary>
        /// Returns the glyph curve from a given char
        /// </summary>
        /// <param name="c"></param>
        public Curve GetGlyphCurve(uint c)
        {
            var curve = new Curve
            {
                CurveParts = new List<CurvePart>()
            };

            // don't print space
            if (c == 32)
            {
                return curve;
            }

            var glyph = _font.GetGlyphs(new CodePoint(c), ColorFontSupport.None).First();
            var outline = glyph.GlyphMetrics.GetOutline();

            var orgPointCoords = outline.ControlPoints.ToArray();
            var pointTags = outline.OnCurves.ToArray().Select(x => x ? (byte)1 : (byte)0).ToArray();
            if (orgPointCoords == null) return curve;

            // Freetype contours are defined by their end points.
            var curvePartEndPoints = outline.EndPoints.ToArray().Select(x => (short)x).ToArray();

            var partTags = new List<byte>();
            var partVerts = new List<float3>();

            //Writes points of a freetype contour into a CurvePart,
            for (var i = 0; i <= orgPointCoords.Length; i++)
            {
                //If a certain index of outline points is in array of contour end points - create new CurvePart and add it to Curve.CurveParts
                if (!curvePartEndPoints.ToList().Contains((short)i)) continue;

                partVerts.Clear();
                partTags.Clear();

                var part = SplitToCurvePartHelper.CreateCurvePart(orgPointCoords, pointTags, curvePartEndPoints, i,
                    partVerts, partTags);
                curve.CurveParts.Add(part);

                var segments = SplitToCurveSegmentHelper.SplitPartIntoSegments(part, partTags, partVerts);
                SplitToCurveSegmentHelper.CombineCurveSegmentsAndAddThemToCurvePart(segments, part);
            }

            return curve;

        }

        /// <summary>
        /// Get glyph info from letter
        /// </summary>
        /// <param name="c">letter char</param>
        /// <returns></returns>
        public GlyphInfo GetGlyphInfo(uint c)
        {
            var glyph = _font.GetGlyphs(new CodePoint(c), ColorFontSupport.None).First();

            var scaledPointSize = _font.Size * Dpi;
            var scaleFactor = scaledPointSize / glyph.GlyphMetrics.ScaleFactor;

            GlyphInfo ret;
            ret.CharCode = c;
            ret.AdvanceX = glyph.GlyphMetrics.AdvanceWidth * scaleFactor;
            ret.AdvanceY = glyph.GlyphMetrics.AdvanceHeight * scaleFactor;

            ret.Width = glyph.GlyphMetrics.Width * scaleFactor;
            ret.Height = glyph.GlyphMetrics.Height * scaleFactor;

            return ret;
        }


        /// <summary>
        /// Returns the kerning between two chars
        /// </summary>
        /// <param name="leftC"></param>
        /// <param name="rightC"></param>
        /// <returns></returns>
        public float GetKerning(uint leftC, uint rightC)
        {
            //var glyphLeft = _font.GetGlyphs(new CodePoint(leftC), ColorFontSupport.None).First();
            //var glyphRight = _font.GetGlyphs(new CodePoint(rightC), ColorFontSupport.None).First();
            return 0;
        }

        /// <summary>
        /// Returns the unscaled advance of one glyph
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public float GetUnscaledAdvance(uint c)
        {
            var glyphs = _font.GetGlyphs(new CodePoint(c), ColorFontSupport.None);
            return glyphs.First().GlyphMetrics.AdvanceWidth;
        }

        /// <summary>
        /// Returns the unscaled kerning, currently TODO
        /// </summary>
        /// <param name="leftC"></param>
        /// <param name="rightC"></param>
        /// <returns></returns>
        public float GetUnscaledKerning(uint leftC, uint rightC)
        {
            var glyphLeft = _font.GetGlyphs(new CodePoint(leftC), ColorFontSupport.None).First();
            var glyphRight = _font.GetGlyphs(new CodePoint(rightC), ColorFontSupport.None).First();
            return 0;
        }

        /// <summary>
        /// Renders a glyph to an IImageData for further use
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public IImageData GetImageDataForGlyph(uint c, in GlyphInfo info)
        {
            var width = (int)System.Math.Ceiling(info.AdvanceX);
            var height = (int)System.Math.Ceiling(info.AdvanceY);
            var arr = new Rgba32[width * height];
            var res = new Span<Rgba32>(arr);

            try
            {
                using var img = CreateImage(new DrawingOptions(), Convert.ToChar(c).ToString(),
                    _font, width, height,
                    new System.Numerics.Vector2(0, 0), Color.Black);
                img.CopyPixelDataTo(res);
            }
            // invalid (unknown) chars 
            catch (Exception e)
            {
                Diagnostics.Warn($"Generating glyph for char {c}:{Convert.ToChar(c)} failed, skipping");
                return new ImageData(0, 0);
            }

            var ret = new ImageData(res.ToArray().Select(x => x.A).ToArray(), width, height, new ImagePixelFormat(ColorFormat.Intensity));
            return ret;
        }

        private static Image<Rgba32> CreateImage(DrawingOptions options,
            string text,
            SixLabors.Fonts.Font font,
            int width,
            int height,
            System.Numerics.Vector2 origin,
            Color color)
        {
            var img = new Image<Rgba32>(width, height);
            img.Mutate(x => x.Fill(Color.FromRgba(0, 0, 0, 0)));
            img.Mutate(x => x.DrawText(options, text, font, color, origin));

            return img;
        }
    }

    internal static class SplitToCurvePartHelper
    {
        #region Methods

        public static void CurvePartVertice(int j, System.Numerics.Vector2[] orgPointCoords, List<float3> partVerts)
        {
            var vert = new float3(orgPointCoords[j].X, orgPointCoords[j].Y, 0);
            partVerts.Add(vert);
        }

        public static CurvePart CreateCurvePart(System.Numerics.Vector2[] orgPointCoords, byte[] pointTags, short[] curvePartEndPoints, int i, List<float3> partVerts, List<byte> partTags)
        {
            var index = Array.IndexOf(curvePartEndPoints, (short)i);
            var cp = new CurvePart
            {
                IsClosed = true,
                CurveSegments = new List<CurveSegment>()
            };

            //Marginal case - first contour ( 0 to contours[0] ).
            if (index == 0)
            {
                for (var j = 0; j <= i; j++)
                {
                    CurvePartVertice(j, orgPointCoords, partVerts);
                    partTags.Add(pointTags[j]);
                }
                //The start point is the first point in the outline.Points array.
                cp.StartPoint = new float3(orgPointCoords[0].X, orgPointCoords[0].Y, 0);
            }

            //contours[0]+1 to contours[1]
            else
            {
                for (var j = curvePartEndPoints[index - 1] + 1; j <= curvePartEndPoints[index]; j++)
                {
                    CurvePartVertice(j, orgPointCoords, partVerts);
                    partTags.Add(pointTags[j]);
                }

                //The index in outline.Points which describes the start point is given by the index of the foregone outline.contours index +1.
                cp.StartPoint = new float3(orgPointCoords[curvePartEndPoints[index - 1] + 1].X, orgPointCoords[curvePartEndPoints[index - 1] + 1].Y, 0);
            }
            return cp;
        }
        #endregion
    }
}