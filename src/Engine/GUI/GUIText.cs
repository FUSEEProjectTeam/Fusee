using System;
using System.Collections.Generic;
using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;

namespace Fusee.Engine.GUI
{
    /// <summary>
    /// Creates a text mesh for the GUI.
    /// </summary>
    public class GUIText : Mesh
    {
        private readonly FontMap _fontMap;
        private readonly string _text;

        /// <summary>
        /// Returns the text mesh.
        /// </summary>
        /// <param name="fontMap"></param>
        /// <param name="text"></param>
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


            var verts = new List<float3>();
            var uvs = new List<float2>();
            var tris = new List<ushort>();
            var normals = new List<float3>();

            // build complete structure

            // var charInfo = Font.CharInfo;
            var atlasWidth = _fontMap.Image.Width;
            var atlasHeight = _fontMap.Image.Height;

            ushort vertex = 0;
            var advanceX = 0f;
            var advanceY = 0f;

            var textMeshHeight = 0f;

            // now build the mesh
            foreach (var letter in _text)
            {
                GlyphOnMap glyphOnMap = _fontMap.GetGlyphOnMap(letter);
                GlyphInfo glyphInfo = _fontMap.Font.GetGlyphInfo(letter);

                var x = advanceX + glyphOnMap.BitmapL;
                var y = advanceY - glyphOnMap.BitmapT;
                var w = glyphOnMap.BitmapW;
                var h = glyphOnMap.BitmapH;

                if (-y > textMeshHeight)
                    textMeshHeight = -y;

                advanceX += glyphInfo.AdvanceX;
                advanceY += glyphInfo.AdvanceY;

                // skip glyphs that have no pixels
                if ((w <= M.EpsilonFloat) || (h <= M.EpsilonFloat))
                    continue;

                var bitmapW = glyphOnMap.BitmapW;
                var bitmapH = glyphOnMap.BitmapH;
                var texOffsetX = glyphOnMap.TexOffX;
                var texOffsetY = glyphOnMap.TexOffY;

                // vertices
                verts.Add(new float3(x, -y - h, 0));
                verts.Add(new float3(x, -y, 0));
                verts.Add(new float3(x + w, -y - h, 0));
                verts.Add(new float3(x + w, -y, 0));

                normals.Add(-float3.UnitZ);
                normals.Add(-float3.UnitZ);
                normals.Add(-float3.UnitZ);
                normals.Add(-float3.UnitZ);

                // uvs
                uvs.Add(new float2(texOffsetX, texOffsetY + bitmapH / atlasHeight));
                uvs.Add(new float2(texOffsetX, texOffsetY));
                uvs.Add(new float2(texOffsetX + bitmapW / atlasWidth, texOffsetY + bitmapH / atlasHeight));
                uvs.Add(new float2(texOffsetX + bitmapW / atlasWidth, texOffsetY));

                // indices
                tris.Add((ushort)(vertex + 1));
                tris.Add(vertex);
                tris.Add((ushort)(vertex + 2));

                tris.Add((ushort)(vertex + 1));
                tris.Add((ushort)(vertex + 2));
                tris.Add((ushort)(vertex + 3));

                vertex += 4;
            }

            Vertices = verts.ToArray();
            Triangles = tris.ToArray();
            UVs = uvs.ToArray();
            Normals = normals.ToArray();

            Vertices = _fontMap.FixTextKerning(Vertices, _text, 1);

            var meshWidth = Vertices[Vertices.Length - 1].x - Vertices[0].x;
            var translateToZero = Vertices[0].x;

            for (var i = 0; i < Vertices.Length; i++)
            {
                var translateVert = new float3(Vertices[i].x - translateToZero - meshWidth / 2, Vertices[i].y - textMeshHeight / 2, Vertices[i].z);
                var scaledVert = translateVert / meshWidth;
                Vertices[i] = scaledVert;
            }
        }

    }

}
