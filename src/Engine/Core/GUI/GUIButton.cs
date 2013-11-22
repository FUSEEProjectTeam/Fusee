using System;
using Fusee.Math;

namespace Fusee.Engine
{
    public delegate void GUIButtonHandler(object sender, EventArgs e);

    public sealed class GUIButton : GUIElement
    {
        private float4 _buttonColor;

        public event GUIButtonHandler OnGUIButtonDown;
        public event GUIButtonHandler OnGUIButtonUp;
        public event GUIButtonHandler OnGUIButtonEnter;
        public event GUIButtonHandler OnGUIButtonLeave;

        private bool _mouseOnButton;

        public float4 ButtonColor
        {
            get
            {
                return _buttonColor;
            }
            set
            {
                _buttonColor = value;
                CreateMesh();
            }
        }

        public GUIButton(RenderContext rc, string text, IFont font, float x, float y, float width, float height,
            float4 buttonColor, float4 textColor)
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
            
            _buttonColor = buttonColor;
            TextColor = textColor;

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

            var width = Width;
            var height = Height;

            // relative coordinates from -1 to +1
            var scaleX = (float)2 / RContext.ViewportWidth;
            var scaleY = (float)2 / RContext.ViewportHeight;

            var xS = -1 + x * scaleX;
            var yS = +1 - y * scaleY;

            // build complete structure
            var vtCount = 4 * Text.Length;
            var indCount = 6 * Text.Length;

            var vertices = new float3[vtCount + 4];
            var uvs = new float2[vtCount + 4];
            var indices = new ushort[indCount + 6];
            var colors = new uint[vtCount + 4];

            var widthS = width * scaleX;
            var heightS = height * scaleY;

            // vertices
            vertices[vtCount + 0] = new float3(xS, yS - heightS, 0);
            vertices[vtCount + 1] = new float3(xS, yS, 0);
            vertices[vtCount + 2] = new float3(xS + widthS, yS - heightS, 0);
            vertices[vtCount + 3] = new float3(xS + widthS, yS, 0);

            // colors
            var colorInt = MathHelper.Float4ToABGR(_buttonColor);

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
            var maxW = RenderContext.GetTextWidth(Text, Font);
            x = (float)System.Math.Round(x + (width - maxW) / 2);

            var maxH = RenderContext.GetTextHeight(Text, Font);
            y = (float)System.Math.Round(y + maxH + (height - maxH) / 2);

            // get text mesh
            var textMesh = RContext.GetTextMesh(Text, Font, x, y, TextColor);

            // combine button and text
            Array.Copy(textMesh.Vertices, vertices, textMesh.Vertices.Length);
            Array.Copy(textMesh.UVs, uvs, textMesh.UVs.Length);
            Array.Copy(textMesh.Triangles, 0, indices, 6, textMesh.Triangles.Length);
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
                OnGUIButtonDown(this, EventArgs.Empty);
        }

        private void OnButtonUp(object sender, MouseEventArgs mea)
        {
            if (OnGUIButtonUp == null)
                return;

            if (MouseOnButton(mea))
                OnGUIButtonUp(this, EventArgs.Empty);
        }

        private void OnMouseMove(object sender, MouseEventArgs mea)
        {
            if (MouseOnButton(mea))
            {
                if ((OnGUIButtonEnter == null) || (_mouseOnButton)) return;

                OnGUIButtonEnter(this, EventArgs.Empty);
                _mouseOnButton = true;
            }
            else
            {
                if ((OnGUIButtonLeave == null) || (!_mouseOnButton)) return;

                OnGUIButtonLeave(this, EventArgs.Empty);
                _mouseOnButton = false;
            }
        }
    }
}