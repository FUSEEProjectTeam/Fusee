using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Math.Core;
using SharpFont;

namespace Fusee.Base.Imp.Desktop
{
    /// <summary>
    /// Font implementation using freetype (262) / SharpFont.
    /// </summary>
    public class FontImp : IFontImp
    {
        internal static Library _sharpFont;
        internal Face _face;
        internal uint _pixelHeight;
        private bool _useKerning;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontImp"/> class. 
        /// </summary>
        /// <param name="stream">The stream.</param>
        public FontImp(Stream stream)
        {
            if (_sharpFont == null)
                _sharpFont = new Library();

            byte[] fileArray;
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                fileArray = ms.ToArray();
            }

            _face = _sharpFont.NewMemoryFace(fileArray, 0);
            _useKerning = false;
        }


        /// <summary>
        /// Gets or sets a value indicating whether the kerning definition of a font should be used.
        /// </summary>
        /// <value>
        /// <c>true</c> if the kerning definition of a font should be used; otherwise, <c>false</c>.
        /// </value>
        public bool UseKerning
        {
            get { return _useKerning; }
            set
            {
                if (_face.HasKerning)
                    _useKerning = value;
            }
        }

        /// <summary>
        /// Gets or sets the size in pixels.
        /// </summary>
        /// <value>
        /// The vertical size of the font in pixels.
        /// </value>
        public uint PixelHeight
        {
            get
            {
                return _pixelHeight;
            }
            set
            {
                _face.SetPixelSizes(0, value);
                _pixelHeight = value;
            }
        }

        /// <summary>
        /// Gets the character information.
        /// </summary>
        /// <param name="c">The character to retrieve information.</param>
        /// <returns>
        /// An information record about the character.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public GlyphInfo GetGlyphInfo(uint c)
        {
            GlyphInfo ret;
            _face.LoadChar(c, LoadFlags.Default, LoadTarget.Normal);

            ret.CharCode = c;
            ret.AdvanceX = (float)_face.Glyph.Advance.X;
            ret.AdvanceY = (float)_face.Glyph.Advance.Y;

            ret.Width = (float)_face.Glyph.Metrics.Width;
            ret.Height = (float)_face.Glyph.Metrics.Height;

            return ret;
        }

        /// <summary>
        /// Gets the characters points, contours and tags and translates them into a curve
        /// </summary>
        /// <param name="c">The character to retrive information</param>
        /// <returns></returns>
        public Curve GetGlyphCurve(uint c)
        {
            var curve = new Curve();

            _face.LoadChar(c, LoadFlags.NoScale, LoadTarget.Normal);

            curve.CurveParts = new List<CurvePart>();
            var orgPointCoords = _face.Glyph.Outline.Points;
            var pointTags = _face.Glyph.Outline.Tags;
            //Freetype contours are defined by their end points
            var curvePartEndPoints = _face.Glyph.Outline.Contours;

            //Write points of a freetyp contour into a CurvePart
            for (var i = 0; i <= orgPointCoords.Length; i++)
            {
                //If a certain index of outline points is in array of contour end points - create new CurvePart and add it to Curve.CurveParts
                if (curvePartEndPoints.Contains((short)i))
                {
                    curve.CurveParts.Add(FontImpHelper.CreateCurvePart(orgPointCoords, pointTags, curvePartEndPoints, i));
                }
            }

            //Create CurveSegments for every CurvePart
            foreach (var part in curve.CurveParts)
            {
                var segments = FontImpHelper.SplitPartIntoSegments(part);
                FontImpHelper.CombineCurveSegmentsAndAddThemToCurvePart(segments, part);
            }
            return curve;
        }

        /// <summary>
        ///     Renders the given glyph.
        /// </summary>
        /// <param name="c">The character code (Unicode) of the character to render.</param>
        /// <param name="bitmapLeft">
        ///     The x-Bearing of the glyph on the bitmap (in pixels). The number of pixels from the left border of the image 
        ///     to the leftmost pixel of the glyph within the rendered image.
        /// </param>
        /// <param name="bitmapTop">
        ///     The y-Bearing of the glyph on the bitmap (in pixels). The number of pixels from the character's origin 
        ///     (base line) of the image to the topmost pixel of the glyph within the rendered image.
        /// </param>
        /// <returns>
        ///     An image data structure containing an image of the given character.
        /// </returns>
        public ImageData RenderGlyph(uint c, out int bitmapLeft, out int bitmapTop)
        {
            _face.LoadChar(c, LoadFlags.Default, LoadTarget.Normal);
            _face.Glyph.RenderGlyph(RenderMode.Normal);

            FTBitmap bmp = _face.Glyph.Bitmap;

            ImageData ret = new ImageData
            {
                Height = bmp.Rows,
                Width = bmp.Width,
                Stride = bmp.Width,
                PixelFormat = ImagePixelFormat.Intensity,
            };

            if (!ret.IsEmpty)
            {
                ret.PixelData = new byte[bmp.BufferData.Length];
                Array.Copy(bmp.BufferData, ret.PixelData, bmp.BufferData.Length);
            }
            bitmapLeft = _face.Glyph.BitmapLeft;
            bitmapTop = _face.Glyph.BitmapTop;
            return ret;
        }

        /// <summary>
        /// Gets the kerning offset between a pair of two consecutive characters in a text string.
        /// </summary>
        /// <param name="leftC">The left character.</param>
        /// <param name="rightC">The right character.</param>
        /// <returns>
        /// An offset to add to the normal advance. Typically negative since kerning rather compacts text lines.
        /// </returns>
        public float GetKerning(uint leftC, uint rightC)
        {
            if (!_useKerning)
                return 0.0f;

            var leftInx = _face.GetCharIndex(leftC);
            var rightInx = _face.GetCharIndex(rightC);

            return (float)_face.GetKerning(leftInx, rightInx, KerningMode.Default).X;
        }
    }

    internal class FontImpHelper
    {
        public static void CurvePartVertice(CurvePart cp, int j, FTVector[] orgPointCoords)
        {
            var vert = new float3(orgPointCoords[j].X.Value, orgPointCoords[j].Y.Value, 0);
            cp.Vertices.Add(vert);
        }

        public static CurvePart CreateCurvePart(FTVector[] orgPointCoords, byte[] pointTags, short[] curvePartEndPoints, int i)
        {
            var index = Array.IndexOf(curvePartEndPoints, (short)i);
            var cp = new CurvePart
            {
                Vertices = new List<float3>(),
                VertTags = new List<byte>(),
                Closed = true,
                CurveSegments = new List<CurveSegment>()
            };


            //Marginal case - first contour ( 0 to contours[0] ) 
            if (index == 0)
            {
                for (var j = 0; j <= i; j++)
                {
                    CurvePartVertice(cp, j, orgPointCoords);
                    cp.VertTags.Add(pointTags[j]);
                }
                //The start point is the first point in the outline.Points array
                cp.StartPoint = new float3(orgPointCoords[0].X.Value, orgPointCoords[0].Y.Value, 0);
            }
            //contours[0]+1 to contours[1]
            else
            {
                for (var j = curvePartEndPoints[index - 1] + 1; j <= curvePartEndPoints[index]; j++)
                {
                    CurvePartVertice(cp, j, orgPointCoords);
                    cp.VertTags.Add(pointTags[j]);
                }

                //The index in outline.Points which describes the start point is given by the index of the foregone outline.contours index +1
                cp.StartPoint = new float3(orgPointCoords[curvePartEndPoints[index - 1] + 1].X.Value, orgPointCoords[curvePartEndPoints[index - 1] + 1].Y.Value, 0);
            }
            return cp;
        }

        public static List<CurveSegment> SplitPartIntoSegments(CurvePart part)
        {
            byte[] linearPattern = { 1, 1 };
            byte[] conicPattern = { 1, 0, 1 };
            byte[] cubicPattern = { 1, 0, 0, 1 };
            byte[] conicVirtualPattern = { 1, 0, 0, 0 };
            //TODO: Deal with possibility that a contour/part starts with an "off" curve point
            var segments = new List<CurveSegment>();

            for (var i = 0; i < part.VertTags.Count; i++)
            {
                if (part.VertTags.Skip(i).Take(linearPattern.Length).SequenceEqual(linearPattern))
                {
                    segments.Add(CreateCurveSegment(part, i, linearPattern, InterpolationMethod.LINEAR));
                }
                else if (part.VertTags.Skip(i).Take(conicPattern.Length).SequenceEqual(conicPattern))
                {
                    segments.Add(CreateCurveSegment(part, i, conicPattern, InterpolationMethod.BEZIER_CONIC));
                    i = i + 1;
                }
                else if (part.VertTags.Skip(i).Take(cubicPattern.Length).SequenceEqual(cubicPattern))
                {
                    segments.Add(CreateCurveSegment(part, i, cubicPattern, InterpolationMethod.BEZIER_CUBIC));
                    i = i + 2;
                }
                else if (part.VertTags.Skip(i).Take(conicVirtualPattern.Length).SequenceEqual(conicVirtualPattern))
                {
                    var count = 0;
                    var cs = CreateCurveSegment(part, i, conicVirtualPattern, InterpolationMethod.BEZIER_CONIC);

                    i = i + 3;

                    for (var j = i + 1; j < part.VertTags.Count; j++)
                    {
                        cs.Vertices.Add(part.Vertices[j]);
                        if (part.VertTags[j].Equals(0)) continue;
                        count = j;
                        break;
                    }
                    segments.Add(cs);
                    i = count - 1;
                }
                else
                {
                    //Only needed for "closed" CurveParts (like letters always are)
                    var lastSegment = new List<byte>();
                    lastSegment.AddRange(part.VertTags.Skip(i).Take(part.VertTags.Count - i));
                    lastSegment.Add(part.VertTags[0]);
                    if (lastSegment.SequenceEqual(conicPattern))
                    {
                        segments.Add(CreateCurveSegment(part, i, conicPattern, InterpolationMethod.BEZIER_CONIC, part.Vertices[0]));
                    }
                    else if (lastSegment.SequenceEqual(cubicPattern))
                    {
                        segments.Add(CreateCurveSegment(part, i, cubicPattern, InterpolationMethod.BEZIER_CUBIC, part.Vertices[0]));
                    }
                    else if (lastSegment.SequenceEqual(conicVirtualPattern))
                    {
                        segments.Add(CreateCurveSegment(part, i, cubicPattern, InterpolationMethod.BEZIER_CUBIC, part.Vertices[0]));
                    }
                }
            }
            return segments;
        }

        public static CurveSegment CreateCurveSegment(CurvePart cp, int i, byte[] pattern, InterpolationMethod methode)
        {
            var segmentVerts = new List<float3>();
            segmentVerts.AddRange(cp.Vertices.Skip(i).Take(pattern.Length));
            var cs = new CurveSegment
            {
                Interpolation = methode,
                Vertices = new List<float3>()
            };
            cs.Vertices = segmentVerts;
            return cs;
        }

        public static CurveSegment CreateCurveSegment(CurvePart cp, int i, byte[] pattern, InterpolationMethod methode, float3 startPoint)
        {
            var segmentVerts = new List<float3>();
            segmentVerts.AddRange(cp.Vertices.Skip(i).Take(pattern.Length));
            segmentVerts.Add(startPoint);
            var cs = new CurveSegment
            {
                Interpolation = methode,
                Vertices = new List<float3>()
            };
            cs.Vertices = segmentVerts;
            return cs;
        }

        public static void CombineCurveSegmentsAndAddThemToCurvePart(List<CurveSegment> segments, CurvePart part)
        {
            //Combine segments that follow each other and have the same interpolation methode.
            for (var i = 0; i < segments.Count; i++)
            {
                //Constraint
                if (i + 1 >= segments.Count) break;

                //Check whether two successive segments have the same interpolation Methode, if so combine them.
                if (segments[i].Interpolation.Equals(segments[i + 1].Interpolation))
                {
                    foreach (var vertex in segments[i + 1].Vertices)
                    {
                        if (vertex.Equals(segments[i + 1].Vertices[0])) continue;
                        segments[i].Vertices.Add(vertex);
                    }
                    segments.RemoveAt(i + 1);
                    //Set the for loop one step back, to check the "new" CurvePart with its follower
                    if (i >= 0)
                        i = i - 1;
                }
            }
            part.CurveSegments = segments; //TODO: decide whether to delete duplicate points or just do not draw them
        }
    }
}
