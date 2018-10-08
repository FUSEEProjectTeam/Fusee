using Fusee.Engine.Core;
using Fusee.Math.Core;

namespace Fusee.Engine.GUI
{
    /// <summary>
    ///     The <see cref="GUIText_Legacy" /> class provides all text writing functionality.
    /// </summary>
    public sealed class GUIText_Legacy : GUIElement_Legacy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GUIText_Legacy" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fontMap">The font map.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="zRot">The rotation about Z axis as radian.</param>
        /// <param name="xPivot">The x-coordinate of the pivot point</param>
        /// <param name="yPivot">The y-coordinate of the pivot point</param>
        public GUIText_Legacy(string text, FontMap fontMap, int x, int y, float zRot = 0, int xPivot = 0, int yPivot = 0)
            : base(text, fontMap, x, y, 0, 0, 0, zRot, xPivot, yPivot)
        {
            TextColor = new float4(0, 0, 0, 1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIText_Legacy" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fontMap">The font map.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="z">The z-index.</param>
        /// <param name="zRot">The rotation about Z axis as radian.</param>
        /// <param name="xPivot">The x-coordinate of the pivot point</param>
        /// <param name="yPivot">The y-coordinate of the pivot point</param>
        /// <remarks>
        /// The z-index: lower values means further away. If two elements have the same z-index
        /// then they are rendered according to their order in the <see cref="GUIHandler_Legacy" />.
        /// </remarks>
        public GUIText_Legacy(string text, FontMap fontMap, int x, int y, int z, float zRot = 0, int xPivot = 0, int yPivot = 0)
            : base(text, fontMap, x, y, z, 0, 0, zRot, xPivot, yPivot)
        {
            TextColor = new float4(0, 0, 0, 1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIText_Legacy" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fontMap">The font map.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="z">The z-index.</param>
        /// <param name="color">The color.</param>    
        /// <param name="zRot">The rotation about Z axis as radian.</param>
        /// <param name="xPivot">The x-coordinate of the pivot point</param>
        /// <param name="yPivot">The y-coordinate of the pivot point</param>
        /// <remarks>
        /// The z-index: lower values means further away. If two elements have the same z-index
        /// then they are rendered according to their order in the <see cref="GUIHandler_Legacy" />.
        /// </remarks>
        public GUIText_Legacy(string text, FontMap fontMap, int x, int y, int z, float4 color, float zRot = 0, int xPivot = 0, int yPivot = 0)
            : base(text, fontMap, x, y, z, 0, 0, zRot, xPivot, yPivot)
        {
            TextColor = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIText_Legacy" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fontMap">The font map.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="color">The color.</param>
        public GUIText_Legacy(string text, FontMap fontMap, int x, int y, float4 color)
            : base(text, fontMap, x, y, 0, 0, 0)
        {
            TextColor = color;
        }

        protected override void CreateMesh()
        {
            SetTextMesh(PosX + OffsetX, PosY + OffsetY);
        }

        /// <summary>
        /// Gets the height of a text written in a specific font.
        /// </summary>
        /// <param name="text">The text's string.</param>
        /// <param name="fontMap">The text's font map.</param>
        /// <returns>
        /// The height of the text.
        /// </returns>
        public static float GetTextHeight(string text, FontMap fontMap)
        {
            var maxH = 0.0f;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var letter in text)
                maxH = System.Math.Max(maxH, fontMap.GetGlyphOnMap(letter).BitmapH);

            return maxH;
        }

        /// <summary>
        /// Gets the width of a text written in a specific font.
        /// </summary>
        /// <param name="text">The text's string.</param>
        /// <param name="fontMap">The text's font map.</param>
        /// <returns>
        /// The width of the text.
        /// </returns>
        public static float GetTextWidth(string text, FontMap fontMap)
        {
            var maxW = 0.0f;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var letter in text)
                maxW += fontMap.Font.GetGlyphInfo(letter).AdvanceX;

            return maxW;
        }
    }
}