using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Used in conjunction with a <see cref="FontMap"/> containing information about
    /// a rendered character (glyph) on a font map texture.
    /// </summary>
    public struct GlyphOnMap
    {
        /// <summary>
        ///     The width of this char on the font map texture.
        /// </summary>
        public float BitmapW;

        /// <summary>
        ///     The height of this char on the font map texture.
        /// </summary>
        public float BitmapH;

        /// <summary>
        ///     The left border of this char on the font map texture.
        /// </summary>
        public float BitmapL;

        /// <summary>
        ///     The top border of this char on the font map texture.
        /// </summary>
        public float BitmapT;

        /// <summary>
        ///     The x-offset of this char on the font map texture.
        /// </summary>
        public float TexOffX;

        /// <summary>
        ///     The y-offset of this char on the font map texture.
        /// </summary>
        public float TexOffY;
    };

    /// <summary>
    /// A FontMap creates an <see cref="Image"/> containing a subset of rendered glyphs of a given Font.
    /// In addition a FontMap provides information about each glyph on the image such as its pixel position
    /// on the image. Such an image can be used together with the provided information to create geometry
    /// consisting of individual quads with each quad displaying a single character of a text string.
    /// </summary>
    public class FontMap
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="FontMap"/> is up-to-date.
        /// </summary>
        /// <value>
        ///   <c>true</c> if up-to-date; otherwise, <c>false</c>.
        /// </value>
        public bool Uptodate { get; private set; }

        private Font _font;
        private Texture _image;
        private uint _pixelHeight;
        private string _alphabet;
        private readonly Dictionary<uint, GlyphOnMap> _glyphOnMapCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontMap"/> class.
        /// </summary>
        /// <param name="font">The font to be used. See <see cref="Font"/>.</param>
        /// <param name="pixelHeight">Height in pixels of a character. See <see cref="PixelHeight"/>.</param>
        /// <param name="alphabet">The alphabet. See <see cref="Alphabet"/>.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public FontMap(Font font, uint pixelHeight, string alphabet = null)
        {
            if (pixelHeight <= 0) throw new ArgumentOutOfRangeException(nameof(pixelHeight));

            _font = font ?? throw new ArgumentNullException(nameof(font));
            _pixelHeight = pixelHeight;
            _glyphOnMapCache = new Dictionary<uint, GlyphOnMap>();
            Alphabet = alphabet; // will invalidate _uptodate
        }

        private void Invalidate()
        {
            _glyphOnMapCache.Clear();
            Uptodate = false;
        }

        /// <summary>
        /// Gets the image containing all characters specified in the <see cref="Alphabet"/>. Use this image
        /// as a texture used by individual rectangles each displaying a single character. Use the information
        /// retrieved from <see cref="GetGlyphOnMap"/> to position the character rectangles and to align 
        /// their texture coordinates.
        /// </summary>
        /// <value>
        /// The font image.
        /// </value>
        public Texture Image
        {
            get
            {
                if (Uptodate)
                    return _image;

                _font.PixelHeight = _pixelHeight;

                const int maxWidth = 4096;

                var averageAdvance = 0f;
                var charCount = 0f;

                //Calculate averageAdvance in Font? Ratio FontSize/ averageAdvance does not change with fontSize
                foreach (char c in _alphabet)
                {
                    GlyphInfo gi = _font.GetGlyphInfo(c);
                    averageAdvance += gi.AdvanceX + 1;
                    charCount++;
                }

                averageAdvance /= charCount;

                //_alphabet.ToCharArray().Length * averageAdvance * _pixelHeight is the area of ​​the rectangle with the width equal to the number of letters * averageAdvance and the height equals pixelHeight.
                // Since this rectangle has the same area as the desired square (texture atlas), the square root of the rectangle is the width of that square.
                var widthOld = System.Math.Sqrt(_alphabet.ToCharArray().Length * averageAdvance * _pixelHeight);
                var width = (int)System.Math.Pow(2, (int)System.Math.Ceiling(System.Math.Log(widthOld, 2)));

                if (width > maxWidth)
                {
                    width = maxWidth;
                    Debug.WriteLine("Font texture resolution automatically set to 4096 - consider to choose a lower font size");
                }

                // Create the font atlas (the texture containing ALL glyphs)
                _image = new Texture(new byte[width * width], width, width, new ImagePixelFormat(ColorFormat.Intensity), false);

                var offX = 0;
                var offY = 0;
                var rowH = 0;

                // Copy each character in the alphabet to the font atlas
                foreach (char c in _alphabet)
                {
                    IImageData glyphImg = _font.RenderGlyph((uint)c, out int bitmapLeft, out int bitmapTop);
                    if (offX + glyphImg.Width + 1 >= width)
                    {
                        offY += rowH;
                        rowH = 0;
                        offX = 0;
                    }

                    if (!glyphImg.IsEmpty)
                    {
                        _image.Blt(offX, offY, glyphImg); // blit glyph into _image
                    }

                    // char information
                    GlyphOnMap glyphOnMap = new()
                    {
                        BitmapW = glyphImg.Width,
                        BitmapH = glyphImg.Height,
                        BitmapL = bitmapLeft,
                        BitmapT = bitmapTop,
                        TexOffX = offX / (float)width,
                        TexOffY = offY / (float)width,
                    };

                    _glyphOnMapCache[c] = glyphOnMap;

                    rowH = System.Math.Max(rowH, glyphImg.Height);
                    offX += glyphImg.Width + 1;
                }

                Uptodate = true;
                return _image;

            }
        }

        /// <summary>
        /// Fixes the kerning of a text (if possible). ToDo: Instead of fixing existing geometry provide methods to create kerned geometry.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="text">The text.</param>
        /// <param name="scaleX">The scale x (OpenGL scaling factor).</param>
        /// <returns>The fixed vertices as an array of <see cref="float3"/>.</returns>
        public float3[] FixTextKerning(float3[] vertices, string text, float scaleX)
        {
            if (!Font.UseKerning)
                return vertices;

            // use kerning -> fix values
            var fixX = 0f;
            var fixVert = 4;

            for (var c = 0; c < text.Length - 1; c++)
            {
                fixX += (int)Font.GetKerning(text[c], text[c + 1]) * scaleX;

                vertices[fixVert++].x += fixX;
                vertices[fixVert++].x += fixX;
                vertices[fixVert++].x += fixX;
                vertices[fixVert++].x += fixX;
            }

            return vertices;
        }


        /// <summary>
        /// Gets and sets the font displayed on this font map.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets and sets the pixel height of each character on this font. This setting will affect the 
        /// size of the font <see cref="Image"/> , so be careful with this setting.
        /// </summary>
        /// <value>
        /// The height in pixels of an individual character.
        /// </value>
        public uint PixelHeight
        {
            get { return _pixelHeight; }
            set
            {
                _pixelHeight = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets and sets the list of characters that should be present on the font <see cref="Image"/>.
        /// </summary>
        /// <value>
        /// The alphabet.
        /// </value>
        public string Alphabet
        {
            get { return _alphabet; }
            set
            {
                if (value == null)
                {
                    StringBuilder sb = new(256 - 32);
                    for (int i = 32; i < 256; i++)
                        sb.Append((char)i);
                    _alphabet = sb.ToString();
                }
                else
                    _alphabet = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the glyph on map information for the given character/glyph. This information can be used to create geometry textured with on single 
        /// character from the font <see cref="Image"/>.
        /// </summary>
        /// <param name="c">The character to obtain information for.</param>
        /// <returns>The <see cref="GlyphOnMap"/> record for the given character containing information where on the texture the glyph resides.</returns>
        public GlyphOnMap GetGlyphOnMap(uint c)
        {
            try
            {
                return _glyphOnMapCache[c];
            }
            catch
            {
                return _glyphOnMapCache[63];
            }
        }
    }
}