using System;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    ///     A delegation for the event listeners of a <see cref="GUIButton" />.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="mea">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
    public delegate void GUIButtonHandler(GUIButton sender, MouseEventArgs mea);

    /// <summary>
    ///     The <see cref="GUIButton" /> class provides functionality for creating 2D/GUI buttons.
    /// </summary>
    /// <remarks>
    ///     A <see cref="GUIButton" /> doesn't need to have a text on it. It can be modified to be a rectangle with
    ///     an event listener by making its background color transparent and setting a border width of 1 or greater.
    /// </remarks>
    public sealed class GUIButton : GUIElement
    {
        #region Private Fields

        private float4 _buttonColor;

        private int _borderWidth;
        private float4 _borderColor;

        private bool _mouseOnButton;

        #endregion

        #region Public Fields

        /// <summary>
        ///     Gets or sets the color of the button.
        /// </summary>
        /// <value>
        ///     The color of the button.
        /// </value>
        public float4 ButtonColor
        {
            get { return _buttonColor; }
            set
            {
                _buttonColor = value;
                Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the width of the border.
        /// </summary>
        /// <value>
        ///     The width of the border.
        /// </value>
        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                _borderWidth = value;
                Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the color of the border.
        /// </summary>
        /// <value>
        ///     The color of the border.
        /// </value>
        public float4 BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                Dirty = true;
            }
        }

        /// <summary>
        ///     Occurs when mouse button is pressed on this button.
        /// </summary>
        public event GUIButtonHandler OnGUIButtonDown;

        /// <summary>
        ///     Occurs when mouse button is released on this button.
        /// </summary>
        public event GUIButtonHandler OnGUIButtonUp;

        /// <summary>
        ///     Occurs when the mouse cursor enters this button.
        /// </summary>
        public event GUIButtonHandler OnGUIButtonEnter;

        /// <summary>
        ///     Occurs when the mouse cursor leaves this button.
        /// </summary>
        public event GUIButtonHandler OnGUIButtonLeave;

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="GUIButton" /> class.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public GUIButton(int x, int y, int width, int height)
            : base(String.Empty, null, x, y, 0, width, height)
        {
            SetupButton();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GUIButton" /> class.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="z">The z-index.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <remarks>
        ///     The z-index: lower values means further away. If two elements have the same z-index
        ///     then they are rendered according to their order in the <see cref="GUIHandler" />.
        /// </remarks>
        public GUIButton(int x, int y, int z, int width, int height)
            : base(String.Empty, null, x, y, z, width, height)
        {
            SetupButton();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GUIButton" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public GUIButton(string text, IFont font, int x, int y, int width, int height)
            : base(text, font, x, y, 0, width, height)
        {
            SetupButton();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GUIButton" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="z">The z-index.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <remarks>
        ///     The z-index: lower values means further away. If two elements have the same z-index
        ///     then they are rendered according to their order in the <see cref="GUIHandler" />.
        /// </remarks>
        public GUIButton(string text, IFont font, int x, int y, int z, int width, int height)
            : base(text, font, x, y, z, width, height)
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