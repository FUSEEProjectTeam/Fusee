using System;
using System.Windows.Forms;

namespace Examples.WinFormsFusee
{
    public partial class RenderControl : UserControl
    {
        // ReSharper disable InconsistentNaming
        protected const int WM_KEYDOWN = 0x0100;
        protected const int WM_KEYUP = 0x0101;
        protected const int WM_SYSKEYDOWN = 0x0104;
        protected const int WM_SYSKEYUP = 0x0105;
        protected const int WM_SETFOCUS = 0x0007;
        // ReSharper restore InconsistentNaming

        public RenderControl()
        {
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();

            ResizeRedraw = true;
        }

        // Hard stuff to get access to keyboard events. Winforms seems quite over-engineered, found no other way.
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_SETFOCUS:
                    break;
                case WM_KEYDOWN:
                    // case WM_SYSKEYDOWN:
                    OnKeyDown(new KeyEventArgs((Keys) m.WParam));
                    break;
                case WM_KEYUP:
                    // case WM_SYSKEYUP:
                    OnKeyUp(new KeyEventArgs((Keys) m.WParam));
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        public override bool PreProcessMessage(ref Message msg)
        {
            switch (msg.Msg)
            {
                case 0x100: //WM_KEYDOWN
                    return false;
            }
            return base.PreProcessMessage(ref msg);
        }

        protected override void OnResize(EventArgs e)
        {
            // not implemented
        }
    }
}