using Fusee.Math.Core;

namespace Fusee.Base.Common
{
    /// <summary>
    /// A struct for saving character information needed for processing a font.
    /// </summary>
    public struct GlyphInfo
    {
        /// <summary>
        /// The unicode character code this information is for.
        /// </summary>
        public uint CharCode;

        /// <summary>
        /// The amount to advance the pen horizontally when drawing this glyph.
        /// </summary>
        public float AdvanceX;

        /// <summary>
        /// The amount to advance the pen vertically when drawing this glyph. 
        /// Typically 0 for western fonts/script systems.
        /// </summary>
        public float AdvanceY;

        /// <summary>
        /// The width of this glyph. 
        /// </summary>
        public float Width;

        /// <summary>
        /// The height of this glyph.
        /// </summary>
        public float Height;


        /* These values have gone to FontMap in Engine.Core
        /// <summary>
        ///     The width of this char on the texture atlas.
        /// </summary>
        public float BitmapW;

        /// <summary>
        ///     The height of this char on the texture atlas.
        /// </summary>
        public float BitmapH;

        /// <summary>
        ///     The left border of this char on the texture atlas.
        /// </summary>
        public float BitmapL;

        /// <summary>
        ///     The top border of this char on the texture atlas.
        /// </summary>
        public float BitmapT;

        /// <summary>
        ///     The x-offset of this char on the texture atlas.
        /// </summary>
        public float TexOffX;

        /// <summary>
        ///     The y-offset of this char on the texture atlas.
        /// </summary>
        public float TexOffY;
        */
    }

    /// <summary>
    /// Common functionality that needs to be provided by a Font implementor.
    /// </summary>
    public interface IFontImp
    {
        /// <summary>
        ///     Gets and sets a value indicating whether the kerning definition of a font should be used.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the kerning definition of a font should be used; otherwise, <c>false</c>.
        /// </value>
        bool UseKerning { get; set; }

        /// <summary>
        /// Gets and sets the size in pixels.
        /// </summary>
        /// <value>
        /// The vertical size of the font in pixels.
        /// </value>
        uint PixelHeight { get; set; }

        /// <summary>
        /// Gets the character information.
        /// </summary>
        /// <param name="c">The character to retrieve information.</param>
        /// <returns>An information record about the character.</returns>
        GlyphInfo GetGlyphInfo(uint c);

        /// <summary>
        /// Translates the character's control points into a curve.
        /// </summary>
        /// <param name="c">The character from which the information is to be read.</param>
        /// <returns></returns>
        Curve GetGlyphCurve(uint c);

        /// <summary>
        /// Get the unscaled advance from a character.
        /// </summary>
        /// <param name="c">The character from which the information is to be read.</param>
        /// <returns></returns>
        float GetUnscaledAdvance(uint c);

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
        IImageData RenderGlyph(uint c, out int bitmapLeft, out int bitmapTop);

        /// <summary>
        /// Gets the kerning offset between a pair of two consecutive characters in a text string.
        /// </summary>
        /// <param name="leftC">The left character.</param>
        /// <param name="rightC">The right character.</param>
        /// <returns>An offset to add to the normal advance. Typically negative since kerning rather compacts text lines.</returns>
        float GetKerning(uint leftC, uint rightC);

        /// <summary>
        /// Gets the unscaled kerning offset between a pair of two consecutive characters in a text string.
        /// </summary>
        /// <param name="leftC">The left character.</param>
        /// <param name="rightC">The right character.</param>
        /// <returns></returns>
        float GetUnscaledKerning(uint leftC, uint rightC);
    }




}
