using System.Collections.ObjectModel;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;

namespace Fusee.Engine.GUI
{
    /// <summary>
    ///     A delegation for the event listeners of a <see cref="GUIPanel_Legacy" />.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="mea">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
    public delegate void GUIPanelHandler(GUIPanel_Legacy sender, GUIPanelEventArgs mea);

    /// <summary>
    /// Better than nothing.
    /// </summary>
    public class GUIPanelEventArgs
    {
        public int mouseX;
        public int mouseY;
    }

    /// <summary>


    /// <summary>
    ///     A panel which groups other GUIElements together and give them a headline.
    /// </summary>
    /// <remarks>
    ///     This is a hierarchical structure which means that the position of the children
    ///     of a <see cref="GUIPanel_Legacy" /> depends on the position of the GUIPanel itself.
    ///     One could for example add some buttons to such a panel and then position the
    ///     panel in the middle of the screen. In this case, just the coordinates of the
    ///     panel have to be set, the buttons inside the panel will move accordingly!
    /// </remarks>
    public sealed class GUIPanel_Legacy : GUIElement_Legacy
    {
        #region Private Fields

        private float4 _panelColor;

        private int _borderWidth;
        private float4 _borderColor;

        private bool _mouseOnPanel;

        #endregion

        #region Public Fields

        /// <summary>
        ///     Gets or sets the color of the panel.
        /// </summary>
        /// <value>
        ///     The color of the panel.
        /// </value>
        public float4 PanelColor
        {
            get { return _panelColor; }
            set
            {
                _panelColor = value;
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
        ///     The children of this panel.
        /// </summary>
        /// <remarks>
        ///     This is a hierarchical structure which means that the position of the children depends on
        ///     the position of the panel. This can be used to group <see cref="GUIElement_Legacy" />s
        ///     together and move them (e.g. to the center of the screen) just by moving the panel.
        /// </remarks>
        public Collection<GUIElement_Legacy> ChildElements;

        /// <summary>
        ///     Occurs when mouse button is pressed on this panel.
        /// </summary>
        public event GUIPanelHandler OnGUIPanelDown;

        /// <summary>
        ///     Occurs when mouse button is released on this panel.
        /// </summary>
        public event GUIPanelHandler OnGUIPanelUp;

        /// <summary>
        ///     Occurs when the mouse cursor enters this panel.
        /// </summary>
        public event GUIPanelHandler OnGUIPanelEnter;

        /// <summary>
        ///     Occurs when the mouse cursor leaves this panel.
        /// </summary>
        public event GUIPanelHandler OnGUIPanelLeave;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIPanel_Legacy" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fontMap">The font map.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="z">The z-index.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <remarks>
        /// The z-index: lower values means further away. If two elements have the same z-index
        /// then they are rendered according to their order in the <see cref="GUIHandler_Legacy" />.
        /// </remarks>
        public GUIPanel_Legacy(string text, FontMap fontMap, int x, int y, int z, int width, int height)
            : base(text, fontMap, x, y, z, width, height)
        {
            SetupPanel();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIPanel_Legacy" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fontMap">The font map.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public GUIPanel_Legacy(string text, FontMap fontMap, int x, int y, int width, int height)
            : base(text, fontMap, x, y, 0, width, height)
        {
            SetupPanel();
        }

        private void SetupPanel()
        {
            ChildElements = new Collection<GUIElement_Legacy>();

            // settings
            PanelColor = new float4(0.1f, 0.1f, 0.1f, 0.5f);
            TextColor = new float4(0.9f, 0.9f, 0.9f, 1);

            BorderWidth = 1;
            BorderColor = new float4(0.2f, 0.2f, 0.2f, 0.5f);

            // event listener
            Input.Mouse.ButtonValueChanged += OnMouseButton;
            Input.Mouse.AxisValueChanged += OnMouseMove;

            // shader
            CreateGUIShader();
        }

        protected override void CreateMesh()
        {
            // GUIMesh
            SetRectangleMesh(BorderWidth, PanelColor, BorderColor);

            // TextMesh
            var x = PosX + OffsetX;
            var y = PosY + OffsetY;

            var maxW = GUIText_Legacy.GetTextWidth(Text, FontMap);
            x = (int) System.Math.Round(x + (Width - maxW)/2);

            SetTextMesh(x, y + 20);
        }

        /// <summary>
        ///     Refreshes this element and all children (is called when the properties of this element have been changed).
        /// </summary>
        /// <remarks>
        ///     This should be called after the viewport / the windows has been resized.
        ///     It's also possible to call the Refresh method of a <see cref="GUIHandler_Legacy" /> object."
        /// </remarks>
        public override void Refresh()
        {
            base.Refresh();

            foreach (var childElement in ChildElements)
                childElement.Refresh();
        }

        protected override void PreRender(RenderContext rc)
        {
            base.PreRender(rc);

            foreach (var childElement in ChildElements)
            {
                childElement.OffsetX = PosX;
                childElement.OffsetY = PosY;
                childElement.OffsetZ = PosZ;

                childElement.Render(rc);
            }
        }

        private bool MouseOnPanel()
        {
            int x = Input.Mouse.PositionInt.x;
            int y = Input.Mouse.PositionInt.y;
            return x >= PosX + OffsetX &&
                   x <= PosX + OffsetX + Width &&
                   y >= PosY + OffsetY &&
                   y <= PosY + OffsetY + Height;
        }

        private void OnMouseButton(object sender, ButtonValueChangedArgs bvca)
        {
            if (MouseOnPanel())
                if (bvca.Pressed)
                    OnGUIPanelDown?.Invoke(this, new GUIPanelEventArgs { mouseX = Input.Mouse.PositionInt.x, mouseY = Input.Mouse.PositionInt.y });
                else
                    OnGUIPanelUp?.Invoke(this, new GUIPanelEventArgs { mouseX = Input.Mouse.PositionInt.x, mouseY = Input.Mouse.PositionInt.y });
        }

        private void OnMouseMove(object sender, AxisValueChangedArgs avca)
        {
            if (MouseOnPanel())
            {
                if (_mouseOnPanel) return;
                _mouseOnPanel = true;

                if (OnGUIPanelEnter == null) return;
                OnGUIPanelEnter(this, new GUIPanelEventArgs { mouseX = Input.Mouse.PositionInt.x, mouseY = Input.Mouse.PositionInt.y });
            }
            else
            {
                if (!_mouseOnPanel) return;
                _mouseOnPanel = false;

                if (OnGUIPanelLeave == null) return;
                OnGUIPanelLeave(this, new GUIPanelEventArgs { mouseX = Input.Mouse.PositionInt.x, mouseY = Input.Mouse.PositionInt.y });
            }
        }
    }
}