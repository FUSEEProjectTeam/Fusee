using Fusee.Math;

namespace Fusee.Engine
{
    public sealed class GUIText : GUIElement
    {
        public GUIText(string text, IFont font, int x, int y)
            : base(text, font, x, y, 0, 0, 0)
        {
            TextColor = new float4(0, 0, 0, 1);
        }
        
        public GUIText(string text, IFont font, int x, int y, int z)
            :base(text, font, x, y, z, 0, 0)
        {
            TextColor = new float4(0, 0, 0, 1);
        }

        public GUIText(string text, IFont font, int x, int y, int z, float4 color)
            : base(text, font, x, y, z, 0, 0)
        {
            TextColor = color;
        }

        public GUIText(string text, IFont font, int x, int y, float4 color)
            : base(text, font, x, y, 0, 0, 0)
        {
            TextColor = color;
        }

        internal GUIText(RenderContext rc, string text, IFont font, int x, int y, int z, float4 color)
            : base(text, font, x, y, z, 0, 0)
        {
            TextColor = color;
            AttachToContext(rc);
        }

        protected override void CreateMesh()
        {
            SetTextMesh(PosX + OffsetX, PosY + OffsetY);
        }

        /// <summary>
        /// Gets the height of a text in a specific font.
        /// </summary>
        /// <param name="text">The text's string.</param>
        /// <param name="font">The text's font.</param>
        /// <returns>The height of the text.</returns>
        public static float GetTextHeight(string text, IFont font)
        {
            var maxH = 0.0f;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var letter in text)
                maxH = System.Math.Max(maxH, font.CharInfo[letter].BitmapH);

            return maxH;
        }

        /// <summary>
        /// Gets the width of a text in a specific font.
        /// </summary>
        /// <param name="text">The text's string.</param>
        /// <param name="font">The text's font.</param>
        /// <returns>The width of the text.</returns>
        public static float GetTextWidth(string text, IFont font)
        {
            var maxW = 0.0f;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var letter in text)
                maxW += font.CharInfo[letter].AdvanceX;

            return maxW;
        }
    }
}
