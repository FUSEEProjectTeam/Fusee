using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Math.Core;
using SharpFont;

namespace Fusee.Base.Imp.Android
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
            ret.AdvanceX = (float) _face.Glyph.Advance.X;
            ret.AdvanceY = (float) _face.Glyph.Advance.Y;

            ret.Width = (float) _face.Glyph.Metrics.Width;
            ret.Height = (float)_face.Glyph.Metrics.Height;

            return ret;
        }

        /// <summary>
        /// Gets the character's points, contours and tags and translates them into a curve.
        /// </summary>
        /// <param name="c">The character from which the information is to be read.</param>
        /// <returns></returns>
        public Curve GetGlyphCurve(uint c)
        {
            var curve = new Curve();

            _face.LoadChar(c, LoadFlags.NoScale, LoadTarget.Normal);

            curve.CurveParts = new List<CurvePart>();
            var orgPointCoords = _face.Glyph.Outline.Points;
            var pointTags = _face.Glyph.Outline.Tags;
            if (orgPointCoords == null) return curve;

            //Freetype contours are defined by their end points.
            var curvePartEndPoints = _face.Glyph.Outline.Contours;

            var partTags = new List<byte>();
            var partVerts = new List<float3>();

            //Writes points of a freetyp contour into a CurvePart,
            for (var i = 0; i <= orgPointCoords.Length; i++)
            {
                //If a certain index of outline points is in array of contour end points - create new CurvePart and add it to Curve.CurveParts
                if (!curvePartEndPoints.Contains((short)i)) continue;

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
        /// Gets the unscaled advance from a character.
        /// </summary>
        /// <param name="c">The character from which the information is to be read.</param>
        /// <returns></returns>
        public float GetUnscaledAdvance(uint c)
        {
            _face.LoadChar(c, LoadFlags.NoScale, LoadTarget.Normal);
            var advance = _face.Glyph.Metrics.HorizontalAdvance.Value;
            return advance;
        }

        /// <summary>
        /// Renders the given glyph.
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
        public IImageData RenderGlyph(uint c, out int bitmapLeft, out int bitmapTop)
        {
            _face.LoadChar(c, LoadFlags.Default, LoadTarget.Normal);
            _face.Glyph.RenderGlyph(RenderMode.Normal);

            FTBitmap bmp = _face.Glyph.Bitmap;

            ImageData ret = new ImageData(bmp.BufferData, bmp.Width, bmp.Rows,
                new ImagePixelFormat(ColorFormat.Intensity));

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

            return (float) _face.GetKerning(leftInx, rightInx, KerningMode.Default).X;
        }

        /// <summary>
        /// Gets the unscaled kerning offset between a pair of two consecutive characters in a text string.
        /// </summary>
        /// <param name="leftC">The left character.</param>
        /// <param name="rightC">The right character.</param>
        /// <returns></returns>
        public float GetUnscaledKerning(uint leftC, uint rightC)
        {
            var leftInx = _face.GetCharIndex(leftC);
            var rightInx = _face.GetCharIndex(rightC);

            return _face.GetKerning(leftInx, rightInx, KerningMode.Unscaled).X.Value;
        }
    }

    internal class SplitToCurvePartHelper
    {
        #region Methodes
        public static void CurvePartVertice(CurvePart cp, int j, FTVector[] orgPointCoords, List<float3> partVerts)
        {
            var vert = new float3(orgPointCoords[j].X.Value, orgPointCoords[j].Y.Value, 0);
            partVerts.Add(vert);
        }

        public static CurvePart CreateCurvePart(FTVector[] orgPointCoords, byte[] pointTags, short[] curvePartEndPoints, int i, List<float3> partVerts, List<byte> partTags)
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
                cp.StartPoint = new float3(orgPointCoords[0].X.Value, orgPointCoords[0].Y.Value, 0);
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
                cp.StartPoint = new float3(orgPointCoords[curvePartEndPoints[index - 1] + 1].X.Value, orgPointCoords[curvePartEndPoints[index - 1] + 1].Y.Value, 0);
            }
            return cp;
        }
        #endregion
    }
}
