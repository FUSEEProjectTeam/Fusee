using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fusee.Math;

namespace Fusee.Engine
{
    public sealed class GUIPanel : GUIElement
    {
        #region Private Fields

        private float4 _panelColor;

        private int _borderWidth;
        private float4 _borderColor;

        #endregion

        #region Public Fields

        public float4 PanelColor
        {
            get { return _panelColor; }
            set
            {
                _panelColor = value;
                Dirty = true;
            }
        }

        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                _borderWidth = value;
                Dirty = true;
            }
        }

        public float4 BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                Dirty = true;
            }
        }

        public List<GUIElement> ChildElements;

        #endregion

        public GUIPanel(RenderContext rc, string text, IFont font, int x, int y, int width, int height)
            :base(rc, text, font, x, y, width, height)
        {
            ChildElements = new List<GUIElement>();

            // settings
            PanelColor = new float4(0.1f, 0.1f, 0.1f, 0.5f);
            TextColor = new float4(0.9f, 0.9f, 0.9f, 1);

            BorderWidth = 1;
            BorderColor = new float4(0.2f, 0.2f, 0.2f, 0.5f);

            // create Mesh
            CreateMesh();
        }

        protected override void CreateMesh()
        {
            var x = PosX + OffsetX;
            var y = PosY + OffsetY;

            // relative coordinates from -1 to +1
            var scaleX = (float)2 / RContext.ViewportWidth;
            var scaleY = (float)2 / RContext.ViewportHeight;

            var xS = -1 + x * scaleX;
            var yS = +1 - y * scaleY;

            var width = Width * scaleX;
            var height = Height * scaleY;

            var borderX = System.Math.Max(0, BorderWidth*scaleX);
            var borderY = System.Math.Max(0, BorderWidth*scaleY);

            // build complete structure
            var vtCount = 4 * Text.Length;
            var indCount = 6 * Text.Length;

            var vertices = new float3[(BorderWidth > 0) ? vtCount + 8 : vtCount + 4];
            var uvs = new float2[(BorderWidth > 0) ? vtCount + 8 : vtCount + 4];
            var indices = new ushort[(BorderWidth > 0) ? indCount + 12 : indCount + 6];
            var colors = new uint[(BorderWidth > 0) ? vtCount + 8 : vtCount + 4];

            indCount = 0;

            // border
            if (BorderWidth > 0)
            {
                // vertices
                vertices[vtCount + 0] = new float3(xS, yS - height, 0);
                vertices[vtCount + 1] = new float3(xS, yS, 0);
                vertices[vtCount + 2] = new float3(xS + width, yS - height, 0);
                vertices[vtCount + 3] = new float3(xS + width, yS, 0);

                // colors
                var bColorInt = MathHelper.Float4ToABGR(BorderColor);

                colors[vtCount + 0] = bColorInt;
                colors[vtCount + 1] = bColorInt;
                colors[vtCount + 2] = bColorInt;
                colors[vtCount + 3] = bColorInt;

                // uvs
                uvs[vtCount + 0] = new float2(-1, -1);
                uvs[vtCount + 1] = new float2(-1, -1);
                uvs[vtCount + 2] = new float2(-1, -1);
                uvs[vtCount + 3] = new float2(-1, -1);

                // indices
                indices[indCount+0] = (ushort)(vtCount + 1);
                indices[indCount+1] = (ushort)(vtCount + 0);
                indices[indCount+2] = (ushort)(vtCount + 2);

                indices[indCount+3] = (ushort)(vtCount + 1);
                indices[indCount+4] = (ushort)(vtCount + 2);
                indices[indCount+5] = (ushort)(vtCount + 3);

                vtCount += 4;
                indCount += 6;
            }

            // vertices
            vertices[vtCount + 0] = new float3(xS+borderX, yS - height+borderY, 0);
            vertices[vtCount + 1] = new float3(xS + borderX, yS-borderY, 0);
            vertices[vtCount + 2] = new float3(xS-borderX + width, yS - height+borderY, 0);
            vertices[vtCount + 3] = new float3(xS-borderX + width, yS-borderY, 0);

            // colors
            var colorInt = MathHelper.Float4ToABGR(PanelColor);

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
            indices[indCount + 0] = (ushort)(vtCount + 1);
            indices[indCount + 1] = (ushort)(vtCount + 0);
            indices[indCount + 2] = (ushort)(vtCount + 2);

            indices[indCount + 3] = (ushort)(vtCount + 1);
            indices[indCount + 4] = (ushort)(vtCount + 2);
            indices[indCount + 5] = (ushort)(vtCount + 3);

            // position text on panel
            var maxW = GUIText.GetTextWidth(Text, Font);
            x = (int)System.Math.Round(x + (Width - maxW) / 2);

            // get text mesh
            var guiText = new GUIText(RContext, Text, Font, x, y + 20)
            {
                TextColor = TextColor
            };

            guiText.Refresh();
            var textMesh = guiText.GUIMesh;

            // combine panel and text
            Array.Copy(textMesh.Vertices, vertices, textMesh.Vertices.Length);
            Array.Copy(textMesh.UVs, uvs, textMesh.UVs.Length);
            Array.Copy(textMesh.Triangles, 0, indices, (BorderWidth > 0) ? 12 : 6, textMesh.Triangles.Length);
            Array.Copy(textMesh.Colors, colors, textMesh.Colors.Length);

            // create final mesh
            GUIMesh = new Mesh { Vertices = vertices, UVs = uvs, Triangles = indices, Colors = colors };
        }

        protected override void PostRender()
        {
            foreach (var childElement in ChildElements)
            {
                childElement.OffsetX = PosX;
                childElement.OffsetY = PosY;
                childElement.Render();
            }
        }
    }
}