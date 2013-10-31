using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;
using Fusee.Engine;
using KeyEventArgs = Fusee.Engine.KeyEventArgs;
using MouseButtons = Fusee.Engine.MouseButtons;
using MouseEventArgs = Fusee.Engine.MouseEventArgs;

namespace Examples.WinFormsFusee
{
    /// <summary>
    /// Instances of this class act as the glue between a Windows Forms form and FUSEE when FUSEE is intended to display its contents
    /// on the Windows form and to wire interactions to user input performed on the Windows form.
    /// </summary>
    class WinformsHost : RenderCanvasWindowImp, IInputImp
    {
        private bool _disposed = false;
        private Control _form;
        private int _mouseWheelPos;
        private bool _initialized;

        public WinformsHost(Control form) : base(form.Handle, form.Width, form.Height)
        {
            if (form == null) throw new ArgumentNullException("form");

            _form = form;
            _mouseWheelPos = 0;
            form.MouseDown += delegate(object sender, System.Windows.Forms.MouseEventArgs args)
                {
                    if (MouseButtonDown != null)
                        MouseButtonDown(this,
                                        new MouseEventArgs
                                            {
                                                Button = XLateButtons(args.Button),
                                                Position = XLatePoint(args.Location)
                                            });
                };
           form.MouseUp += delegate(object sender, System.Windows.Forms.MouseEventArgs args)
                {
                    if (MouseButtonUp != null)
                        MouseButtonUp(this,
                                        new MouseEventArgs
                                            {
                                                Button = XLateButtons(args.Button),
                                                Position = XLatePoint(args.Location)
                                            });
                };
            form.KeyDown += delegate(object sender, System.Windows.Forms.KeyEventArgs args)
                {
                    if (KeyDown != null)
                        KeyDown(this,
                                new KeyEventArgs
                                    {
                                        Alt = args.Alt,
                                        Control = args.Control,
                                        KeyCode = (KeyCodes) (args.KeyCode)
                                    });
                };
            form.KeyUp += delegate(object sender, System.Windows.Forms.KeyEventArgs args)
                {
                    if (KeyDown != null)
                        KeyUp(this,
                                new KeyEventArgs
                                {
                                    Alt = args.Alt,
                                    Control = args.Control,
                                    KeyCode = (KeyCodes)(args.KeyCode)
                                });
                };
            form.MouseWheel += delegate(object sender, System.Windows.Forms.MouseEventArgs args)
                {
                    _mouseWheelPos += args.Delta;
                };
            form.SizeChanged += delegate(object sender, EventArgs args)
                {
                    _form.Invalidate();
                    _width = _form.Width;
                    _height = _form.Height;
                    DoResize();
                    DoRender();
                };
            Application.Idle += OnIdle;   
        }

        private void OnIdle(object idleSender, EventArgs ea)
        {
            Message message;
            while (!PeekMessage(out message, IntPtr.Zero, 0, 0, 0))
            {
                DoRender();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                Application.Idle -= OnIdle;
                _form = null;
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        public override void Run()
        {
            if (!_initialized)
            {
                DoInit();
                _width = _form.Width;
                _height = _form.Height;
                DoResize();
                _initialized = true;
            }
        }


        public void FrameTick(double time)
        {
            // ignore - we create our own timer.
        }

        public void SetMousePos(Point pos)
        {
            throw new NotImplementedException();
        }

        public Point SetMouseToCenter()
        {
            throw new NotImplementedException();
        }

        public bool CursorVisible { get; set; }

        public Point GetMousePos()
        {
            return new Point {x = Cursor.Position.X, y = Cursor.Position.Y};
        }

        public int GetMouseWheelPos()
        {
            return _mouseWheelPos;
        }

        public event EventHandler<MouseEventArgs> MouseButtonDown;
        public event EventHandler<MouseEventArgs> MouseButtonUp;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;

        private static Fusee.Engine.MouseButtons XLateButtons(System.Windows.Forms.MouseButtons button)
        {
            Fusee.Engine.MouseButtons result = Fusee.Engine.MouseButtons.Unknown;
            if ((button & System.Windows.Forms.MouseButtons.Left) != 0)
                result |= Fusee.Engine.MouseButtons.Left;
            if ((button & System.Windows.Forms.MouseButtons.Right) != 0)
                result |= Fusee.Engine.MouseButtons.Right;
            if ((button & System.Windows.Forms.MouseButtons.Middle) != 0)
                result |= Fusee.Engine.MouseButtons.Middle;
            return result;
        }

        private static Fusee.Engine.Point XLatePoint(System.Drawing.Point point)
        {
            return new Point { x = point.X, y = point.Y };
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Message
        {
            public IntPtr hWnd;
            public int msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint
            messageFilterMin, uint messageFilterMax, uint flags);
    }
}
