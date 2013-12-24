using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fusee.Math;

namespace Fusee.Engine
{
    public delegate void GUIPanelHandler(GUIPanel sender, MouseEventArgs mea);

    public sealed class GUIPanel : GUIElement
    {
        #region Private Fields

        private float4 _panelColor;

        private int _borderWidth;
        private float4 _borderColor;

        private bool _mouseOnPanel;

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

        public event GUIPanelHandler OnGUIPanelDown;
        public event GUIPanelHandler OnGUIPanelUp;
        public event GUIPanelHandler OnGUIPanelEnter;
        public event GUIPanelHandler OnGUIPanelLeave;

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

            // event listener
            Input.Instance.OnMouseButtonDown += OnButtonDown;
            Input.Instance.OnMouseButtonUp += OnButtonUp;
            Input.Instance.OnMouseMove += OnMouseMove;

            // create Mesh
            CreateMesh();
        }

        protected override void CreateMesh()
        {
            var x = PosX + OffsetX;
            var y = PosY + OffsetY;

            // GUIMesh
            SetRectangleMesh(BorderWidth, PanelColor, BorderColor);

            // TextMesh
            var maxW = GUIText.GetTextWidth(Text, Font);
            x = (int)System.Math.Round(x + (Width - maxW) / 2);

            TextMesh = new GUIText(RContext, Text, Font, x, y + 20, TextColor).TextMesh;
        }

        public override void Refresh()
        {
            base.Refresh();

            foreach (var childElement in ChildElements)
                childElement.Refresh();
        }

        protected override void PreRender()
        {
            base.PreRender();

            foreach (var childElement in ChildElements)
            {
                childElement.OffsetX = PosX;
                childElement.OffsetY = PosY;

                childElement.Render();
            }
        }

        private bool MouseOnPanel(MouseEventArgs mea)
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
            if (OnGUIPanelDown == null)
                return;

            if (MouseOnPanel(mea))
               OnGUIPanelDown(this, mea);
        }

        private void OnButtonUp(object sender, MouseEventArgs mea)
        {
            if (OnGUIPanelUp == null)
                return;

            if (MouseOnPanel(mea))
                OnGUIPanelUp(this, mea);
        }

        private void OnMouseMove(object sender, MouseEventArgs mea)
        {
            if (MouseOnPanel(mea))
            {
                if ((OnGUIPanelEnter == null) || (_mouseOnPanel)) return;

                OnGUIPanelEnter(this, mea);
                _mouseOnPanel = true;
            }
            else
            {
                if ((OnGUIPanelLeave == null) || (!_mouseOnPanel)) return;

                OnGUIPanelLeave(this, mea);
                _mouseOnPanel = false;
            }
        }
    }
}