using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using Fusee.Engine;
using KeyEventArgs = Fusee.Engine.KeyEventArgs;
using MouseEventArgs = Fusee.Engine.MouseEventArgs;

namespace Examples.WinFormsFusee
{
    /// <summary>
    ///     Instances of this class act as the glue between a Windows Forms form and FUSEE when FUSEE is intended to display
    ///     its contents
    ///     on the Windows form and to wire interactions to user input performed on the Windows form.
    /// </summary>
    internal class WinformsHost : RenderCanvasWindowImp, IInputImp
    {
        private bool _disposed;

        private Control _form;
        private readonly MainWindow _parent;

        private int _mouseWheelPos;
        private bool _initialized;

        public WinformsHost(Control form, MainWindow parent)
            : base(form.Handle, form.Width, form.Height)
        {
            if (form == null) throw new ArgumentNullException("form");

            _form = form;
            _parent = parent;

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

            form.MouseMove += delegate(object sender, System.Windows.Forms.MouseEventArgs args)
            {
                if (MouseMove != null)
                    MouseMove(this,
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
                            KeyCode = (KeyCodes) (args.KeyCode)
                        });
            };

            form.MouseWheel +=
                delegate(object sender, System.Windows.Forms.MouseEventArgs args) { _mouseWheelPos += args.Delta; };

            form.SizeChanged += delegate
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

        public override void SetCursor(CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.Standard:
                    _form.Cursor = Cursors.Default;
                    break;
                case CursorType.Hand:
                    _form.Cursor = Cursors.WaitCursor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("cursorType");
            }
            
        }

        public override void OpenLink(string link)
        {
            if (link.StartsWith("http://"))
                Process.Start(link);
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


        public override int Width
        {
            get { return _width; }
            set
            {
                _width = value + 200;
                _parent.SetSize(_width, _height);
            }
        }

        public override int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                _parent.SetSize(_width, _height);
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
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;

        private static Fusee.Engine.MouseButtons XLateButtons(System.Windows.Forms.MouseButtons button)
        {
            var result = Fusee.Engine.MouseButtons.Unknown;
            if ((button & System.Windows.Forms.MouseButtons.Left) != 0)
                result |= Fusee.Engine.MouseButtons.Left;
            if ((button & System.Windows.Forms.MouseButtons.Right) != 0)
                result |= Fusee.Engine.MouseButtons.Right;
            if ((button & System.Windows.Forms.MouseButtons.Middle) != 0)
                result |= Fusee.Engine.MouseButtons.Middle;
            return result;
        }

        private static Point XLatePoint(System.Drawing.Point point)
        {
            return new Point {x = point.X, y = point.Y};
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Message
        {
            private readonly IntPtr hWnd;
            private readonly int msg;
            private readonly IntPtr wParam;
            private readonly IntPtr lParam;
            private readonly uint time;
            private readonly Point p;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint
            messageFilterMin, uint messageFilterMax, uint flags);
    }
}