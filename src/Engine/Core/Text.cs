using System;
using System.Diagnostics;
using System.IO;
using Fusee.Math;
using JSIL.Meta;

namespace Fusee.Engine
{
    partial class RenderContext
    {
        private readonly ShaderProgram _textShader;
        private readonly IShaderParam _textTextureParam;

        public IFont LoadFont(string filename, uint size)
        {
            if (!File.Exists(filename))
                throw new Exception("Font not found: " + filename);

            return _rci.LoadFont(filename, size);
        }

        [JSExternal]
        public IFont LoadSystemFont(string fontname, uint size)
        {
            var fontsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            var pathToFont = Path.Combine(fontsFolder, fontname + ".ttf");

            return LoadFont(pathToFont, size);
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

        public Mesh GetTextMesh(string text, IFont font, float x, float y, float4 color)
        {
            // relative coordinates from -1 to +1
            var scaleX = (float)2 / ViewportWidth;
            var scaleY = (float)2 / ViewportHeight;

            x = -1 + x * scaleX;
            y = +1 - y * scaleY;

            // build complete structure
            var vertices = new float3[4 * text.Length];
            var uvs = new float2[4 * text.Length];
            var indices = new ushort[6 * text.Length];
            var colors = new uint[4 * text.Length];

            var charInfo = font.CharInfo;
            var atlasWidth = font.Width;
            var atlasHeight = font.Height;

            var index = 0;
            ushort vertex = 0;

            // now build the mesh
            foreach (var letter in text)
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
                var colorInt = MathHelper.Float4ToABGR(color);

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

            vertices = _rci.FixTextKerning(font, vertices, text, scaleX);
            return new Mesh { Vertices = vertices, UVs = uvs, Triangles = indices, Colors = colors };
        }

        public void TextOut(Mesh textMesh, IFont font)
        {
            var curShader = _currentShader;

            if (_currentShader != _textShader)
                SetShader(_textShader);

            SetShaderParamTexture(_textTextureParam, font.TexAtlas);

            _rci.PrepareTextRendering(true);
            Render(textMesh);
            _rci.PrepareTextRendering(false);

            if (curShader != null && curShader != _textShader)
                SetShader(curShader);
        }

        public void TextOut(string text, IFont font, float4 color, float x, float y)
        {
            TextOut(GetTextMesh(text, font, x, y, color), font);
        }
    }
}
