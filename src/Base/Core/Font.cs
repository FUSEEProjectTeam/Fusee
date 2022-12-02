﻿using Fusee.Base.Common;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Base.Core
{
    /// <summary>
    /// A Font instance contains methods to access font and glyph data stored in a font
    /// description (TrueType or OpenType fonts).
    /// </summary>
    public class Font
    {
        /// <summary>
        /// For implementation purposes only. Do not use this.
        /// </summary>
        public FontImpBase _fontImp;

        private readonly Dictionary<uint, GlyphInfo> _glyphInfoCache = new();

        private readonly Dictionary<uint, Curve> _glyphCurveChache = new();

        private readonly Dictionary<uint, float> _glyphAdvanceCache = new();


        /// <summary>
        ///     Gets and sets a value indicating whether the kerning definition of a font should be used.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the kerning definition of a font should be used; otherwise, <c>false</c>.
        /// </value>
        public bool UseKerning
        {
            get { return _fontImp.UseKerning; }
            set { _fontImp.UseKerning = value; }
        }

        /// <summary>
        /// Gets and sets the size in pixels.
        /// </summary>
        /// <value>
        /// The vertical size of the font in pixels.
        /// </value>
        public uint PixelHeight
        {
            get { return _fontImp.PixelHeight; }
            set
            {
                _fontImp.PixelHeight = value;
                _glyphInfoCache.Clear();
            }
        }

        /// <summary>
        /// Gets the character information.
        /// </summary>
        /// <param name="c">The character to retrieve information.</param>
        /// <returns>An information record about the character.</returns>
        public GlyphInfo GetGlyphInfo(uint c)
        {
            if (_glyphInfoCache.TryGetValue(c, out GlyphInfo ret))
                return ret;

            // its not in the cache...
            ret = _fontImp.GetGlyphInfo(c);
            _glyphInfoCache[c] = ret;
            return ret;
        }


        /// <summary>
        /// Gets the character's points, contours and tags and translates them into a curve.
        /// </summary>
        /// <param name="c">The character from which the information is to be read.</param>
        /// <returns></returns>
        public Curve GetGlyphCurve(uint c)
        {
            if (_glyphCurveChache.TryGetValue(c, out Curve curve))
                return curve;

            // its not in the cache...
            curve = _fontImp.GetGlyphCurve(c);
            _glyphCurveChache[c] = curve;
            return curve;
        }


        /// <summary>
        /// Get the unscaled advance from a character.
        /// </summary>
        /// <param name="c">The character from which the information is to be read.</param>
        /// <returns></returns>
        public float GetUnscaledAdvance(uint c)
        {
            if (_glyphAdvanceCache.TryGetValue(c, out float ret))
                return ret;

            ret = _fontImp.GetUnscaledAdvance(c);
            _glyphAdvanceCache[c] = ret;
            return ret;
        }

        /// <summary>
        ///     Renders the given glyph.
        /// </summary>
        /// <param name="c">The character code (Unicode) of the character to render.</param>
        /// <param name="info">Provides info about a character.</param>
        /// <returns>
        ///     An image data structure containing an image of the given character.
        /// </returns>
        public IImageData GetImageDataForGlyph(uint c, in GlyphInfo info) => _fontImp.GetImageDataForGlyph(c, in info);

        /// <summary>
        /// Gets the kerning offset between a pair of two consecutive characters in a text string.
        /// </summary>
        /// <param name="leftC">The left character.</param>
        /// <param name="rightC">The right character.</param>
        /// <returns>An offset to add to the normal advance. Typically negative since kerning rather compacts text lines.</returns>
        public float GetKerning(uint leftC, uint rightC) => _fontImp.GetKerning(leftC, rightC);

        /// <summary>
        /// Gets the unscaled kerning offset between a pair of two consecutive characters in a text string.
        /// </summary>
        /// <param name="leftC">The left character.</param>
        /// <param name="rightC">The right character.</param>
        /// <returns></returns>
        public float GetUnscaledKerning(uint leftC, uint rightC) => _fontImp.GetUnscaledKerning(leftC, rightC);

    }
}