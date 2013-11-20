using System;
using Fusee.Math;

namespace Fusee.Engine
{
    partial class RenderContext
    {
        public Mesh GetButton(string text, IFont font, float x, float y, float width, float height, float4 buttonColor, float4 textColor)
        {
            // relative coordinates from -1 to +1
            var scaleX = (float) 2/_viewportWidth;
            var scaleY = (float) 2/_viewportHeight;

            var xS = -1 + x*scaleX;
            var yS = +1 - y*scaleY;

            // build complete structure
            var vtCount = 4*text.Length;
            var indCount = 6*text.Length;

            var vertices = new float3[vtCount + 4];
            var uvs = new float2[vtCount + 4];
            var indices = new ushort[indCount + 6];
            var colors = new uint[vtCount + 4];

            var buttonW = width*scaleX;
            var buttonH = height*scaleY;

            // vertices
            vertices[vtCount + 0] = new float3(xS, yS - buttonH, 0);
            vertices[vtCount + 1] = new float3(xS, yS, 0);
            vertices[vtCount + 2] = new float3(xS + buttonW, yS - buttonH, 0);
            vertices[vtCount + 3] = new float3(xS + buttonW, yS, 0);

            // colors
            var colorInt = MathHelper.Float4ToABGR(buttonColor);

            colors[vtCount + 0] = colorInt;
            colors[vtCount + 1] = colorInt;
            colors[vtCount + 2] = colorInt;
            colors[vtCount + 3] = colorInt;

            // uvs
            uvs[vtCount + 0] = new float2(-1, -1);
            uvs[vtCount + 1] = new float2(-1, -1);
            uvs[vtCount + 2] = new float2(-1, -1);
            uvs[vtCount + 3] = new float2(-1, -1);

            // indices
            indices[0] = (ushort)(vtCount + 1);
            indices[1] = (ushort)(vtCount + 0);
            indices[2] = (ushort)(vtCount + 2);

            indices[3] = (ushort)(vtCount + 1);
            indices[4] = (ushort)(vtCount + 2);
            indices[5] = (ushort)(vtCount + 3);

            // center text on button
            var maxW = GetTextWidth(text, font);
            x = (float) System.Math.Round(x + (width - maxW)/2);

            var maxH = GetTextHeight(text, font);
            y = (float) System.Math.Round(y + maxH + (height - maxH)/2);

            // get text mesh
            var textMesh = GetTextMesh(text, font, x, y, new float4(0, 0, 0, 1));

            // combine button and text
            Array.Copy(textMesh.Vertices, vertices, textMesh.Vertices.Length);
            Array.Copy(textMesh.UVs, uvs, textMesh.UVs.Length);
            Array.Copy(textMesh.Triangles, 0, indices, 6, textMesh.Triangles.Length);
            Array.Copy(textMesh.Colors, colors, textMesh.Colors.Length);

            return new Mesh {Vertices = vertices, UVs = uvs, Triangles = indices, Colors = colors};
        }
    }
}
