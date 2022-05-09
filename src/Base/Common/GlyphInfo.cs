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
}