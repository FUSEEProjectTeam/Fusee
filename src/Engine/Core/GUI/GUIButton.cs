using System;
using Fusee.Math;

namespace Fusee.Engine
{
    public delegate void GUIButtonHandler(GUIButton sender, MouseEventArgs mea);

    public sealed class GUIButton : GUIElement
    {
        #region Private Fields

        private float4 _buttonColor;

        private int _borderWidth;
        private float4 _borderColor;

        private bool _mouseOnButton;

        #endregion

        #region Public Fields

        public float4 ButtonColor
        {
            get { return _buttonColor; }
            set
            {
                _buttonColor = value;
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

        public event GUIButtonHandler OnGUIButtonDown;
        public event GUIButtonHandler OnGUIButtonUp;
        public event GUIButtonHandler OnGUIButtonEnter;
        public event GUIButtonHandler OnGUIButtonLeave;

        #endregion

        public GUIButton(RenderContext rc, string text, IFont font, int x, int y, int width, int height)
            :base(rc, text, font, x, y, width, height)
        {
            // settings
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
            var x = PosX + OffsetX;
            var y = PosY + OffsetY;

            // GUIMesh
            SetRectangleMesh(BorderWidth, ButtonColor, BorderColor);

            // TextMesh
            var maxW = GUIText.GetTextWidth(Text, Font);
            x = (int) System.Math.Round(x + (Width - maxW)/2);

            var maxH = GUIText.GetTextHeight(Text, Font);
            y = (int) System.Math.Round(y + maxH + (Height - maxH)/2);

            TextMesh = new GUIText(RContext, Text, Font, x, y, TextColor).TextMesh;
        }

        private bool MouseOnButton(MouseEventArgs mea)
        {
            var x = mea.Position.x;
            var y = mea.Position.y;

            return x >= PosX + OffsetX &&
                   x <= PosX + OffsetX + Width &&
                   y >= PosY + OffsetY &&
                   y <= PosY + OffsetY + Height;
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