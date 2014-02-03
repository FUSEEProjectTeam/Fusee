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

        public GUIButton(int x, int y, int width, int height)
            : base("", null, x, y, 0, width, height)
        {
            SetupButton();
        }

        public GUIButton(int x, int y, int z, int width, int height)
            : base("", null, x, y, z, width, height)
        {
            SetupButton();
        }

        public GUIButton(string text, IFont font, int x, int y, int width, int height)
            : base(text, font, x, y, 0, width, height)
        {
            SetupButton();
        }

        public GUIButton(string text, IFont font, int x, int y, int z, int width, int height)
            :base(text, font, x, y, z, width, height)
        {
            SetupButton();
        }

        private void SetupButton()
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

            // shader
            CreateGUIShader();
        }

        protected override void CreateMesh()
        {
            // GUIMesh
            SetRectangleMesh(BorderWidth, ButtonColor, BorderColor);

            // TextMesh
            var x = PosX + OffsetX;
            var y = PosY + OffsetY;
            
            var maxW = GUIText.GetTextWidth(Text, Font);
            x = (int) System.Math.Round(x + (Width - maxW)/2);

            var maxH = GUIText.GetTextHeight(Text, Font);
            y = (int) System.Math.Round(y + maxH + (Height - maxH)/2);

            SetTextMesh(x, y);
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
                if (_mouseOnButton) return;
                _mouseOnButton = true;

                if (OnGUIButtonEnter == null) return;
                OnGUIButtonEnter(this, mea);
            }
            else
            {
                if (!_mouseOnButton) return;
                _mouseOnButton = false;

                if (OnGUIButtonLeave == null) return;
                OnGUIButtonLeave(this, mea);
            }
        }
    }
}