using Fusee.Math;

namespace Fusee.Engine
{
    public sealed class GUIText : GUIElement
    {
        public GUIText(RenderContext rc, string text, IFont font, float x, float y)
            :base(rc, text, font, x, y, 0, 0)
        {
            // settings
            TextColor = new float4(0, 0, 0, 1);

            // create Mesh
            CreateMesh();
        }

        protected override void CreateMesh()
        {
            // relative coordinates from -1 to +1
            var scaleX = (float)2 / RContext.ViewportWidth;
            var scaleY = (float)2 / RContext.ViewportHeight;

            var x = -1 + PosX * scaleX;
            var y = +1 - PosY * scaleY;

            // build complete structure
            var vertices = new float3[4 * Text.Length];
            var uvs = new float2[4 * Text.Length];
            var indices = new ushort[6 * Text.Length];
            var colors = new uint[4 * Text.Length];

            var charInfo = Font.CharInfo;
            var atlasWidth = Font.Width;
            var atlasHeight = Font.Height;

            var index = 0;
            ushort vertex = 0;

            // now build the mesh
            foreach (var letter in Text)
            {
                var x2 = x + charInfo[letter].BitmapL * scaleX;
                var y2 = -y - charInfo[letter].BitmapT * scaleY;
                var w = charInfo[letter].BitmapW * scaleX;
                var h = charInfo[letter].BitmapH * scaleY;

                x += charInfo[letter].AdvanceX * scaleX;
                y += charInfo[letter].AdvanceY * scaleY;

                // skip glyphs that have no pixels
                if ((w <= MathHelper.EpsilonFloat) || (h <= MathHelper.EpsilonFloat))
                    continue;

                var bitmapW = charInfo[letter].BitmapW;
                var bitmapH = charInfo[letter].BitmapH;
                var texOffsetX = charInfo[letter].TexOffX;
                var texOffsetY = charInfo[letter].TexOffY;

                // vertices
                vertices[vertex] = new float3(x2, -y2 - h, 0);
                vertices[vertex + 1] = new float3(x2, -y2, 0);
                vertices[vertex + 2] = new float3(x2 + w, -y2 - h, 0);
                vertices[vertex + 3] = new float3(x2 + w, -y2, 0);

                // uvs
                uvs[vertex] = new float2(texOffsetX, texOffsetY + bitmapH / atlasHeight);
                uvs[vertex + 1] = new float2(texOffsetX, texOffsetY);
                uvs[vertex + 2] = new float2(texOffsetX + bitmapW / atlasWidth, texOffsetY + bitmapH / atlasHeight);
                uvs[vertex + 3] = new float2(texOffsetX + bitmapW / atlasWidth, texOffsetY);

                // colors
                var colorInt = MathHelper.Float4ToABGR(TextColor);

                colors[vertex] = colorInt;
                colors[vertex + 1] = colorInt;
                colors[vertex + 2] = colorInt;
                colors[vertex + 3] = colorInt;

                // indices
                indices[index++] = (ushort)(vertex + 1);
                indices[index++] = vertex;
                indices[index++] = (ushort)(vertex + 2);

                indices[index++] = (ushort)(vertex + 1);
                indices[index++] = (ushort)(vertex + 2);
                indices[index++] = (ushort)(vertex + 3);

                vertex += 4;
            }

            vertices = RContext.FixTextKerning(Font, vertices, Text, scaleX);
            GUIMesh = new Mesh { Vertices = vertices, UVs = uvs, Triangles = indices, Colors = colors };
        }

        /// <summary>
        /// Gets the height of a text in a specific font.
        /// </summary>
        /// <param name="text">The text's string.</param>
        /// <param name="font">The text's font.</param>
        /// <returns>The height of the text.</returns>
        internal static float GetTextHeight(string text, IFont font)
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
        internal static float GetTextWidth(string text, IFont font)
        {
            var maxW = 0.0f;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var letter in text)
                maxW += font.CharInfo[letter].AdvanceX;

            return maxW;
        }
    }
}
