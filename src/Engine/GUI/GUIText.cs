using System;
using Fusee.Base.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.GUI
{
    public class GUIText: Mesh
    {
        private readonly FontMap _fontMap;
        private readonly string _text;

        public GUIText(FontMap fontMap, string text)
        {
            _fontMap = fontMap;
            _text = text;

            CreateTextMesh();
        }

        private void CreateTextMesh()
        {
            
            if (_fontMap == null)
                throw new ArgumentException("Can not create Text Mesh - FontMap not found!");

            Vertices = new float3[4 * _text.Length];
            UVs = new float2[4 * _text.Length];
            Triangles = new ushort[6 * _text.Length];
            Normals = new float3[4 * _text.Length];
            

            // build complete structure

            // var charInfo = Font.CharInfo;
            var atlasWidth = _fontMap.Image.Width;
            var atlasHeight = _fontMap.Image.Height;

            var index = 0;
            ushort vertex = 0;
            var posX = 0f;
            var posY = 0f;

            // now build the mesh
            foreach (var letter in _text)
            {
                GlyphOnMap glyphOnMap = _fontMap.GetGlyphOnMap(letter);
                GlyphInfo glyphInfo = _fontMap.Font.GetGlyphInfo(letter);

                var x2 = posX + glyphOnMap.BitmapL;
                var y2 = posY - glyphOnMap.BitmapT;
                var w = glyphOnMap.BitmapW;
                var h = glyphOnMap.BitmapH ;

                posX += glyphInfo.AdvanceX;
                posY += glyphInfo.AdvanceY;

                // skip glyphs that have no pixels
                if ((w <= M.EpsilonFloat) || (h <= M.EpsilonFloat))
                    continue;

                var bitmapW = glyphOnMap.BitmapW;
                var bitmapH = glyphOnMap.BitmapH;
                var texOffsetX = glyphOnMap.TexOffX;
                var texOffsetY = glyphOnMap.TexOffY;

                // vertices
                Vertices[vertex] = new float3(x2, -y2 - h, 0);
                Vertices[vertex + 1] = new float3(x2, -y2, 0);
                Vertices[vertex + 2] = new float3(x2 + w, -y2 - h, 0);
                Vertices[vertex + 3] = new float3(x2 + w, -y2, 0);

                Normals[vertex] = -float3.UnitZ;
                Normals[vertex + 1] = -float3.UnitZ;
                Normals[vertex + 2] = -float3.UnitZ;
                Normals[vertex + 3] = -float3.UnitZ;

                // uvs
                UVs[vertex] = new float2(texOffsetX, texOffsetY + bitmapH / atlasHeight);
                UVs[vertex + 1] = new float2(texOffsetX, texOffsetY);
                UVs[vertex + 2] = new float2(texOffsetX + bitmapW / atlasWidth, texOffsetY + bitmapH / atlasHeight);
                UVs[vertex + 3] = new float2(texOffsetX + bitmapW / atlasWidth, texOffsetY);

                // indices
                Triangles[index++] = (ushort)(vertex + 1);
                Triangles[index++] = vertex;
                Triangles[index++] = (ushort)(vertex + 2);

                Triangles[index++] = (ushort)(vertex + 1);
                Triangles[index++] = (ushort)(vertex + 2);
                Triangles[index++] = (ushort)(vertex + 3);

                vertex += 4;
            }

            Vertices = _fontMap.FixTextKerning(Vertices, _text, 1); //ToDo: scale??
        }

    }

}
