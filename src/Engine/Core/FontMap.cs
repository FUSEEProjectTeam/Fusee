using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Used in conjunction with a <see cref="FontMap"/> containting information about
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
        private Font _font;
        private ImageData _image;
        private uint _pixelHeight;
        private string _alphabet;
        private bool _uptodate;
        private readonly Dictionary<uint, GlyphOnMap> _glyphOnMapCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontMap"/> class.
        /// </summary>
        /// <param name="font">The font to be used. See <see cref="Font"/>.</param>
        /// <param name="pixelHeight">Height in pixesl of a character. See <see cref="PixelHeight"/>.</param>
        /// <param name="alphabet">The alphabet. See <see cref="Alphabet"/>.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public FontMap(Font font, uint pixelHeight, string alphabet = null)
        {
            if (font == null) throw new ArgumentNullException(nameof(font));
            if (pixelHeight <= 0) throw new ArgumentOutOfRangeException(nameof(pixelHeight));

            _font = font;
            _pixelHeight = pixelHeight;
            _glyphOnMapCache = new Dictionary<uint, GlyphOnMap>();
            Alphabet = alphabet; // will invalidate _uptodate
        }

        private void Invalidate()
        {
            _glyphOnMapCache.Clear();
            _uptodate = false;
        }

        /// <summary>
        /// Gets the image containing all characters specified in the <see cref="Alphabet"/>. Use this image
        /// as a texture used by individual rectangles each displaying a single character. Use the information
        /// retrieved from <see cref="GetGlyphOnMap"/> to position the character rectangles and to align 
        /// their texure coordinates.
        /// </summary>
        /// <value>
        /// The font image.
        /// </value>
        public ImageData Image
        {
            get
            {
                if (_uptodate)
                    return _image;

                _font.PixelHeight = _pixelHeight;

                var rowW = 0;
                var rowH = 0;
                var h = 0;

                const int maxWidth = 512;
                
                foreach (char c in _alphabet)
                {
                    uint i = (uint) c;
                    GlyphInfo gi = _font.GetGlyphInfo(i);

                    if (rowW + ((int)gi.AdvanceX) + 1 >= maxWidth)
                    {
                        h += rowH;
                        rowW = 0;
                        rowH = 0;
                    }

                    rowW += ((int)gi.AdvanceX) + 1;
                    rowH = System.Math.Max((int)gi.Height, rowH);
                }

                // for resizing to non-power-of-two
                var potH = (h + rowH) - 1;

                potH |= potH >> 1;
                potH |= potH >> 2;
                potH |= potH >> 4;
                potH |= potH >> 8;
                potH |= potH >> 16;

                int texMapWidth = maxWidth;
                int texMapHeight = ++potH;

                // Create the font atlas (the texture containting ALL glyphs)
                _image = new ImageData(new byte[texMapWidth * texMapHeight], texMapWidth, texMapHeight, new ImagePixelFormat(ColorFormat.Intensity));
                //_image = new ImageData
                //{
                //    PixelData = new byte[texMapWidth * texMapHeight],
                //    Width = texMapWidth,
                //    Height = texMapHeight,
                //    ColorFormat = ImagePixelFormat.Intensity,
                //    
                //};

                var offX = 0;
                var offY = 0;
                rowH = 0;

                // Copy each character in the alphabet to the font atlas
                foreach (char c in _alphabet)
                {
                    int bitmapLeft, bitmapTop;
                    IImageData glyphImg = _font.RenderGlyph((uint) c, out bitmapLeft, out bitmapTop);
                    if (offX + glyphImg.Width + 1 >= maxWidth)
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
                    GlyphOnMap glyphOnMap = new GlyphOnMap
                    {
                        BitmapW = glyphImg.Width,
                        BitmapH = glyphImg.Height,
                        BitmapL = bitmapLeft,
                        BitmapT = bitmapTop,
                        TexOffX = offX/(float) maxWidth,
                        TexOffY = offY/(float) potH,
                    };

                    _glyphOnMapCache[c] = glyphOnMap;

                    rowH = System.Math.Max(rowH, glyphImg.Height);
                    offX += glyphImg.Width + 1;
                }


                _uptodate = true;
                return _image;

                /*
                // atlas texture
                var tex = GL.GenTexture();

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, tex);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Alpha, maxWidth, potH, 0,
                    OpenTK.Graphics.OpenGL.ColorFormat.Alpha, PixelType.UnsignedByte, IntPtr.Zero);

                // texture settings
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                    (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                    (int)TextureWrapMode.ClampToEdge);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);

                texAtlas.TexAtlas = new Texture { handle = tex };

                // paste the glyph images into the texture atlas
                texAtlas.CharInfo = new CharInfoStruct[256];

                var offX = 0;
                var offY = 0;
                rowH = 0;

                for (uint i = 32; i < 256; i++)
                {
                    face.LoadChar(i, LoadFlags.Default, LoadTarget.Normal);
                    face.Glyph.RenderGlyph(RenderMode.Normal);

                    if (offX + face.Glyph.Bitmap.Width + 1 >= maxWidth)
                    {
                        offY += rowH;
                        rowH = 0;
                        offX = 0;
                    }

                    GL.TexSubImage2D(TextureTarget.Texture2D, 0, offX, offY, face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows,
                        OpenTK.Graphics.OpenGL.ColorFormat.Alpha, PixelType.UnsignedByte, face.Glyph.Bitmap.Buffer);

                    // char informations
                    texAtlas.CharInfo[i].AdvanceX = (int)face.Glyph.Advance.X;
                    texAtlas.CharInfo[i].AdvanceY = (int)face.Glyph.Advance.Y;

                    texAtlas.CharInfo[i].BitmapW = face.Glyph.Bitmap.Width;
                    texAtlas.CharInfo[i].BitmapH = face.Glyph.Bitmap.Rows;

                    texAtlas.CharInfo[i].BitmapL = face.Glyph.BitmapLeft;
                    texAtlas.CharInfo[i].BitmapT = face.Glyph.BitmapTop;

                    texAtlas.CharInfo[i].TexOffX = offX / (float)maxWidth;
                    texAtlas.CharInfo[i].TexOffY = offY / (float)potH;

                    rowH = System.Math.Max(rowH, face.Glyph.Bitmap.Rows);
                    offX += face.Glyph.Bitmap.Width + 1;
                }

                return texAtlas;
                */

            }
        }

        /// <summary>
        /// Fixes the kerning of a text (if possible). Todo: Instead of fixing existing geometry provide methods to create kerned geometry.
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
        /// Gets or sets the font displayed on this font map.
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
        /// Gets or sets the pixel height of each character on this font. This setting will affect the 
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
        /// Gets or sets the list of characters that should be present on the font <see cref="Image"/>.
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
                    StringBuilder sb = new StringBuilder(256 - 32);
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
        /// Gets a value indicating whether this <see cref="FontMap"/> is uptodate.
        /// </summary>
        /// <value>
        ///   <c>true</c> if uptodate; otherwise, <c>false</c>.
        /// </value>
        public bool Uptodate => _uptodate;

        /// <summary>
        /// Gets the glyph on map information for the given character/glyph. This information can be used to create geometry textured with on single 
        /// character from the font <see cref="Image"/>.
        /// </summary>
        /// <param name="c">The character to obtain information for.</param>
        /// <returns>The <see cref="GlyphOnMap"/> record for the given character containting information where on the texture the glyph resides.</returns>
        public GlyphOnMap GetGlyphOnMap(uint c)
        {
            return _glyphOnMapCache[c];
        }
    }
}
