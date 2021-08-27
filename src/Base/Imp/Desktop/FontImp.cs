using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Math.Core;
using System.Numerics;
using SixLabors.Fonts;
using Font = SixLabors.Fonts.Font;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace Fusee.Base.Imp.Desktop
{
    /// <summary>
    /// Font implementation using SixLabors.Fonts
    /// </summary>
    public class FontImp : IFontImp
    {
        internal Font _font;
        internal FontCollection _collection;

        /// <summary>
        /// Font implementation for WebAsm
        /// </summary>
        /// <param name="stream"></param>
        public FontImp(Stream stream)
        {
            _collection = new FontCollection();
            _collection.Install(stream);
        }

        /// <summary>
        /// Use kerning
        /// </summary>
        public bool UseKerning { get; set; } = false;

        private uint _pixelHeight = 24;

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
            if (c == 32) {
                return curve;
            }

            var glyph = _font.GetGlyph(Convert.ToChar(c));

            var orgPointCoords = glyph.Instance.ControlPoints.ToArray();
            var pointTags = glyph.Instance.OnCurves.Select(x => x ? (byte)1 : (byte)0).ToArray();
            if (orgPointCoords == null) return curve;

            // Freetype contours are defined by their end points.
            var curvePartEndPoints = glyph.Instance.EndPoints.Select(x => (short)x).ToArray();

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
            GlyphInfo ret;

            var options = new RendererOptions(_font);
            FontRectangle size = TextMeasurer.Measure(Convert.ToChar(c).ToString(), options);

            ret.CharCode = c;
            ret.AdvanceX = size.Width;
            ret.AdvanceY = 0;

            ret.Width = size.Width;
            ret.Height = size.Height;

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
            var offset = _font.Instance.GetOffset(_font.GetGlyph(Convert.ToChar(leftC)).Instance, _font.GetGlyph(Convert.ToChar(rightC)).Instance);
            return offset.X;
        }

        /// <summary>
        /// Returns the unscaled advance of one glyph
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public float GetUnscaledAdvance(uint c)
        {
            var glyph = _font.Instance.GetGlyph(Convert.ToChar(c));
            return glyph.AdvanceWidth;
        }

        /// <summary>
        /// Returns the unscaled kerning, currently TODO
        /// </summary>
        /// <param name="leftC"></param>
        /// <param name="rightC"></param>
        /// <returns></returns>
        public float GetUnscaledKerning(uint leftC, uint rightC)
        {
            var offset = _font.Instance.GetOffset(_font.GetGlyph(Convert.ToChar(leftC)).Instance, _font.GetGlyph(Convert.ToChar(rightC)).Instance);
            return offset.X;
        }

        /// <summary>
        /// Renders a glyph to an IImageData for further use
        /// </summary>
        /// <param name="c"></param>
        /// <param name="bitmapLeft">
        ///     The x-Bearing of the glyph on the bitmap (in pixels). The number of pixels from the left border of the image 
        ///     to the leftmost pixel of the glyph within the rendered image.
        /// </param>
        /// <param name="bitmapTop">
        ///     The y-Bearing of the glyph on the bitmap (in pixels). The number of pixels from the character's origin 
        ///     (base line) of the image to the topmost pixel of the glyph within the rendered image.
        /// </param>
        /// <returns></returns>
        public IImageData RenderGlyph(uint c, out int bitmapLeft, out int bitmapTop)
        {
            var options = new RendererOptions(_font);
            FontRectangle size = TextMeasurer.Measure(Convert.ToChar(c).ToString(), options);
            
            var drawingOptions = new DrawingOptions
            {
                TextOptions = new TextOptions()
                {
                    ApplyKerning = UseKerning,
                    DpiX = options.DpiX,
                    DpiY = options.DpiY,
                    TabWidth = options.TabWidth,
                    LineSpacing = options.LineSpacing,
                    HorizontalAlignment = options.HorizontalAlignment,
                    VerticalAlignment = options.VerticalAlignment,
                    WrapTextWidth = options.WrappingWidth,
                    RenderColorFonts = options.ColorFontSupport != ColorFontSupport.None
                }
            };

            var width = (int)System.Math.Max(1, System.Math.Round(size.Width));
            var height = (int)System.Math.Max(1, size.Height);

            using var img = CreateImage(drawingOptions, Convert.ToChar(c).ToString(),
                options.Font, width, height,
                options.Origin, Color.Black);

            bitmapLeft =(int)size.Left;
            bitmapTop = -(int)size.Top;

            img.TryGetSinglePixelSpan(out var res);

            var ret = new ImageData(res.ToArray().Select(x => x.A).ToArray(), width, height, new ImagePixelFormat(ColorFormat.Intensity));

            return ret;
        }


        private static Image<Rgba32> CreateImage(DrawingOptions options,
            string text,
            Font font,
            int width,
            int height,
            Vector2 origin,
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

        public static void CurvePartVertice(CurvePart cp, int j, Vector2[] orgPointCoords, List<float3> partVerts)
        {
            var vert = new float3(orgPointCoords[j].X, orgPointCoords[j].Y, 0);
            partVerts.Add(vert);
        }

        public static CurvePart CreateCurvePart(Vector2[] orgPointCoords, byte[] pointTags, short[] curvePartEndPoints, int i, List<float3> partVerts, List<byte> partTags)
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
                    CurvePartVertice(cp, j, orgPointCoords, partVerts);
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
                    CurvePartVertice(cp, j, orgPointCoords, partVerts);
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

