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
            ret.AdvanceX = (float) _face.Glyph.Advance.X;
            ret.AdvanceY = (float) _face.Glyph.Advance.Y;

            ret.Width = (float) _face.Glyph.Metrics.Width;
            ret.Height = (float)_face.Glyph.Metrics.Height;

            return ret;
        }

        /// <summary>
        /// Gets the start, end and control points of a character.
        /// </summary>
        /// <param name="c">The character to retrive information</param>
        /// <returns></returns>
        public GlyphPoints GetGlyphPoints(uint c)
        {
            GlyphPoints ret;
            _face.LoadChar(c, LoadFlags.Default, LoadTarget.Normal);

            ret.CharCode = c;
            ret.Pos = new float2();
            ret.PointFlags = new List<int[]>();
            ret.PointCoords = new List<float2>();
            ret.Points = new Dictionary<float2, int[]>();
            ret.OrgPointCoords = _face.Glyph.Outline.Points;


            //TODO: Split into 3 methods?
            //Get tags and add them to an array
            byte[] helper = _face.Glyph.Outline.Tags;

            if (helper == null) return ret; //Is null if c is space

            //Convert values of byte array into binary representation
            foreach (var flags in helper)
            {
                var s = Convert.ToString(flags, 2);

                var bits = s.PadLeft(8, '0')                        // Add 0's from left
                             .Select(x => int.Parse(x.ToString()))  // convert each char to int
                             .ToArray();                            // Convert IEnumerable from select to Array

                ret.PointFlags.Add(bits);                           //Bits are read from right to left, therfore the last bit in the array is bit 0 --> bits[7] is responsible to say wheather a point is on a curve or not
            }

            //Get point coordinates and add them to a list
            if (ret.OrgPointCoords == null) return ret; //Is null if c is space

            foreach (FTVector vec in ret.OrgPointCoords)
            {
                ret.Pos = new float2(vec.X.Value, vec.Y.Value);
                ret.PointCoords.Add(ret.Pos);
            }

            //Write tags and coordinates into a dictionary
            for (int i = 0; i < ret.PointCoords.Count; i++)
            {
                ret.Points.Add(ret.PointCoords[i], ret.PointFlags[i]);
            }
            return ret;
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

            return (float) _face.GetKerning(leftInx, rightInx, KerningMode.Default).X;
        }
    }
}
