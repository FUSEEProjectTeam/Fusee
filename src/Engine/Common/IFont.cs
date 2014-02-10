namespace Fusee.Engine
{
    /// <summary>
    ///     A struct for saving character information needed for proccesing a font.
    /// </summary>
    public struct CharInfoStruct
    {
        /// <summary>
        ///     The width of this char.
        /// </summary>
        public float AdvanceX;

        /// <summary>
        ///     The height of this char.
        /// </summary>
        public float AdvanceY;

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
    }

    /// <summary>
    ///     Interface for a font object to save informationen needed for proccesing a font.
    /// </summary>
    public interface IFont
    {
        /// <summary>
        ///     Gets the texture atlas.
        /// </summary>
        /// <value>
        ///     The texture atlas.
        /// </value>
        /// <remarks>
        ///     This texture contains all pre-rendered chars of a font in a specific font size.
        /// </remarks>
        ITexture TexAtlas { get; }

        /// <summary>
        ///     Gets the width of the texture atlas.
        /// </summary>
        /// <value>
        ///     The width of the texture atlas.
        /// </value>
        int Width { get; }

        /// <summary>
        ///     Gets the height of the texture atlas.
        /// </summary>
        /// <value>
        ///     The height of the texture atlas.
        /// </value>
        int Height { get; }

        /// <summary>
        ///     Gets the font size.
        /// </summary>
        /// <value>
        ///     The font size.
        /// </value>
        /// <remarks>
        ///     The font size cannot be changed after loading a font.
        /// </remarks>
        uint FontSize { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the kerning definition of a font should be used.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the kerning definition of a font should be used; otherwise, <c>false</c>.
        /// </value>
        bool UseKerning { get; set; }

        /// <summary>
        ///     Gets the character information.
        /// </summary>
        /// <value>
        ///     The character information.
        /// </value>
        CharInfoStruct[] CharInfo { get; }
    }
}