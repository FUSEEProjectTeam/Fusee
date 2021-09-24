using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Gui
{
    /// <summary>
    /// Creates a text mesh for the GUI.
    /// </summary>
    public class GuiText : Mesh
    {
        private readonly FontMap _fontMap;

        /// <summary>
        /// The text to display.
        /// The Vertices, Triangles, Normals and UV coordinates will be recalculated if this value is set.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                CreateTextMesh();
                Merge();
            }
        }
        private string _text;

        /// <summary>
        /// The Vertices per line of text.
        /// </summary>
        public List<List<float3>> LineVertices { get; private set; }

        /// <summary>
        /// The Triangles per line of text.
        /// </summary>
        public List<List<ushort>> LineTriangles { get; private set; }

        /// <summary>
        /// The Normals per line of text.
        /// </summary>
        public List<List<float3>> LineNormals { get; private set; }

        /// <summary>
        /// The UV coordinates per line of text.
        /// </summary>
        public List<List<float2>> LineUVs { get; private set; }

        /// <summary>
        /// Defines the <see cref="HorizontalAlignment"/> of the text. 
        /// This is needed here because it changes the shape of the mesh.
        /// The mesh's vertices will be recalculated the value is set.
        /// </summary>
        public HorizontalTextAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                _horizontalAlignment = value;
                Merge();
            }
        }
        private HorizontalTextAlignment _horizontalAlignment;

        /// <summary>
        /// The width of the text mesh's bounding box.
        /// </summary>
        public float Width { get; private set; }

        /// <summary>
        /// The height of the text mesh's bounding box.
        /// </summary>
        public float Height { get; private set; }

        //The min and max coordinates of the bounding box, as received from the font imp.      
        private float2 _min;
        private float2 _max;

        /// <summary>
        /// Returns the text mesh.
        /// </summary>
        /// <param name="fontMap"></param>
        /// <param name="text"></param>
        /// <param name="horizontalAlignment">Defines the <see cref="HorizontalAlignment"/> of the text. 
        /// This is needed here because it changes the shape of the mesh.</param>
        public GuiText(FontMap fontMap, string text, HorizontalTextAlignment horizontalAlignment)
        {
            _fontMap = fontMap;
            _text = text;

            CreateTextMesh();
            HorizontalAlignment = horizontalAlignment;
        }

        private void Merge()
        {
            var allVerts = new List<float3>();
            var allTriangles = new List<ushort>();
            var allNormals = new List<float3>();
            var allUvs = new List<float2>();

            for (var i = 0; i < LineVertices.Count; i++)
            {
                var lineWidth = LineVertices[i].Last().x - LineVertices[i][0].x;
                switch (HorizontalAlignment)
                {
                    case HorizontalTextAlignment.Left:
                        for (var j = 0; j < LineVertices[i].Count; j++)
                        {
                            var vert = LineVertices[i][j];

                            //translate to zero
                            vert.x += _min.x;
                            vert.y -= _max.y;

                            allVerts.Add(vert);
                        }
                        break;
                    case HorizontalTextAlignment.Center:

                        for (var j = 0; j < LineVertices[i].Count; j++)
                        {
                            var vert = LineVertices[i][j];

                            //translate to zero
                            vert.x += _min.x;
                            vert.y -= _max.y;

                            vert.x = vert.x + (Width / 2) - (lineWidth / 2);
                            allVerts.Add(vert);
                        }
                        break;
                    case HorizontalTextAlignment.Right:

                        for (var j = 0; j < LineVertices[i].Count; j++)
                        {
                            var vert = LineVertices[i][j];

                            //translate to zero
                            vert.x += _min.x;
                            vert.y -= _max.y;

                            vert.x += (Width - lineWidth);
                            allVerts.Add(vert);
                        }
                        break;
                    default:
                        break;
                }

                allTriangles.AddRange(LineTriangles[i]);
                allNormals.AddRange(LineNormals[i]);
                allUvs.AddRange(LineUVs[i]);
            }

            Vertices = allVerts.ToArray();
            Triangles = allTriangles.ToArray();
            Normals = allNormals.ToArray();
            UVs = allUvs.ToArray();

        }

        private void CreateTextMesh()
        {
            if (_fontMap == null)
                throw new ArgumentException("Can not create Text Mesh - FontMap not found!");

            LineVertices = new List<List<float3>>();
            LineTriangles = new List<List<ushort>>();
            LineNormals = new List<List<float3>>();
            LineUVs = new List<List<float2>>();

            var verts = new List<float3>();
            var uvs = new List<float2>();
            var tris = new List<ushort>();
            var normals = new List<float3>();

            // build complete structure
            var atlasWidth = _fontMap.Image.Width;
            var atlasHeight = _fontMap.Image.Height;

            ushort vertex = 0;
            var advanceX = 0f;
            var advanceY = 0f;

            var lineBreakOffset = _fontMap.PixelHeight * 1.5f; //TODO: add user-field. At the moment we have a fixed line height of 150% of pixel height, 
            var lineCnt = 0;

            _min = float2.One * float.MaxValue;
            _max = float2.One * float.MinValue;

            // now build the mesh
            for (var j = 0; j <= _text.Length; j++)
            {
                if (j != _text.Length && _text[j] != '\n')
                {
                    var glyphOnMap = _fontMap.GetGlyphOnMap(_text[j]);
                    var glyphInfo = _fontMap.Font.GetGlyphInfo(_text[j]);

                    var x = advanceX + glyphOnMap.BitmapL;
                    var y = advanceY - glyphOnMap.BitmapT;
                    var w = glyphOnMap.BitmapW;
                    var h = glyphOnMap.BitmapH;

                    advanceX += glyphInfo.AdvanceX;
                    advanceY += glyphInfo.AdvanceY;

                    // skip glyphs that have no pixels
                    if ((w <= M.EpsilonFloat) || (h <= M.EpsilonFloat))
                        continue;

                    var bitmapW = glyphOnMap.BitmapW;
                    var bitmapH = glyphOnMap.BitmapH;
                    var texOffsetX = glyphOnMap.TexOffX;
                    var texOffsetY = glyphOnMap.TexOffY;

                    //account for line height
                    y += lineCnt * lineBreakOffset;

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
                else //new line or last char in the text
                {
                    lineCnt++;
                    advanceX = 0;

                    var vertsArray = _fontMap.FixTextKerning(verts.ToArray(), _text, 1);

                    for (var i = 0; i < vertsArray.Length; i++)
                    {
                        vertsArray[i].y -= _fontMap.PixelHeight;

                        if (vertsArray[i].x > _max.x)
                            _max.x = vertsArray[i].x;
                        if (vertsArray[i].y > _max.y)
                            _max.y = vertsArray[i].y;

                        if (vertsArray[i].x < _min.x)
                            _min.x = vertsArray[i].x;
                        if (vertsArray[i].y < _min.y)
                            _min.y = vertsArray[i].y;
                    }

                    Width = _max.x - _min.x;
                    Height = _max.y - _min.y;

                    LineVertices.Add(vertsArray.ToList());
                    LineTriangles.Add(tris);
                    LineNormals.Add(normals);
                    LineUVs.Add(uvs);

                    verts = new List<float3>();
                    uvs = new List<float2>();
                    tris = new List<ushort>();
                    normals = new List<float3>();

                }
            }
        }
    }
}