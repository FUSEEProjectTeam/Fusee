using System;
using Fusee.Math;

namespace Fusee.Engine
{
    public delegate void GUIButtonHandler(GUIButton sender, MouseEventArgs mea);

    public sealed class GUIButton : GUIElement
    {
        public float4 ButtonColor { get; set; }

        public int BorderWidth { get; set; }
        public float4 BorderColor { get; set; }

        public event GUIButtonHandler OnGUIButtonDown;
        public event GUIButtonHandler OnGUIButtonUp;
        public event GUIButtonHandler OnGUIButtonEnter;
        public event GUIButtonHandler OnGUIButtonLeave;

        private bool _mouseOnButton;

        public GUIButton(RenderContext rc, string text, IFont font, float x, float y, float width, float height)
        {
            RContext = rc;

            // x, y, width, height
            PosX = x;
            PosY = y;

            Width = width;
            Height = height;

            // settings
            Text = text;
            Font = font;

            ButtonColor = new float4(1, 1, 1, 1);
            TextColor = new float4(0, 0, 0, 1);

            BorderWidth = 1;
            BorderColor = new float4(0, 0, 0, 1);

            // event listener
            Input.Instance.OnMouseButtonDown += OnButtonDown;
            Input.Instance.OnMouseButtonUp += OnButtonUp;
            Input.Instance.OnMouseMove += OnMouseMove;

            _mouseOnButton = false;

            // create Mesh
            CreateMesh();
        }

        protected override void CreateMesh()
        {
            var x = PosX;
            var y = PosY;

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
            var colorInt = MathHelper.Float4ToABGR(ButtonColor);

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

            // center text on button
            var maxW = GUIText.GetTextWidth(Text, Font);
            x = (float)System.Math.Round(x + (Width - maxW) / 2);

            var maxH = GUIText.GetTextHeight(Text, Font);
            y = (float)System.Math.Round(y + maxH + (Height - maxH) / 2);

            // get text mesh
            var guiText = new GUIText(RContext, Text, Font, x, y)
            {
                TextColor = TextColor
            };

            guiText.Refresh();
            var textMesh = guiText.GUIMesh;

            // combine button and text
            Array.Copy(textMesh.Vertices, vertices, textMesh.Vertices.Length);
            Array.Copy(textMesh.UVs, uvs, textMesh.UVs.Length);
            Array.Copy(textMesh.Triangles, 0, indices, (BorderWidth > 0) ? 12 : 6, textMesh.Triangles.Length);
            Array.Copy(textMesh.Colors, colors, textMesh.Colors.Length);

            // create final mesh
            GUIMesh = new Mesh { Vertices = vertices, UVs = uvs, Triangles = indices, Colors = colors };
        }

        private bool MouseOnButton(MouseEventArgs mea)
        {
            var x = mea.Position.x;
            var y = mea.Position.y;

            return x >= PosX && x <= PosX + Width && y >= PosY && y <= PosY + Height;
        }

        private void OnButtonDown(object sender, MouseEventArgs mea)
        {
            if (OnGUIButtonDown == null)
                return;

            if (MouseOnButton(mea))
                OnGUIButtonDown(this, mea);
        }

        private void OnButtonUp(object sender, MouseEventArgs mea)
        {
            if (OnGUIButtonUp == null)
                return;

            if (MouseOnButton(mea))
                OnGUIButtonUp(this, mea);
        }

        private void OnMouseMove(object sender, MouseEventArgs mea)
        {
            if (MouseOnButton(mea))
            {
                if ((OnGUIButtonEnter == null) || (_mouseOnButton)) return;

                OnGUIButtonEnter(this, mea);
                _mouseOnButton = true;
            }
            else
            {
                if ((OnGUIButtonLeave == null) || (!_mouseOnButton)) return;

                OnGUIButtonLeave(this, mea);
                _mouseOnButton = false;
            }
        }
    }
}