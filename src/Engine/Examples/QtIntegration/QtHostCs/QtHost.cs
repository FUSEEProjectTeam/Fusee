using System;
using System.Runtime.InteropServices;
using System.Security;
using KeyEventArgs = Fusee.Engine.KeyEventArgs;
using MouseEventArgs = Fusee.Engine.MouseEventArgs;

namespace Fusee.Engine
{
    /// <summary>
    /// Instances of this class act as the glue between a Windows Forms form and FUSEE when FUSEE is intended to display its contents
    /// on the Windows form and to wire interactions to user input performed on the Windows form.
    /// </summary>
    public class QtHost  : RenderCanvasWindowImp, IInputImp
    {
        public delegate int MousePosDelegate();

        protected MousePosDelegate _GetMousePos;


        protected FuseeApp _theApp;


        public QtHost(IntPtr windowHandle)
            : base(windowHandle)
        {
            if (windowHandle == IntPtr.Zero) 
                throw new ArgumentNullException("windowHandle");

            _mouseWheelPos = 0;



            // Start the application
            _theApp = new FuseeApp();

            _theApp.CanvasImplementor = this;
            _theApp.InputImplementor = this;
            _theApp.Run();
            // 
        }


        public void SetMousePosQuery(MousePosDelegate mpd)
        {
            _GetMousePos = mpd;
        }

        public void TriggerMouseDown(int button, int xx, int yy)
        {
            if (MouseButtonDown != null)
                MouseButtonDown(this,
                                new MouseEventArgs
                                    {
                                        Button = (MouseButtons)button,
                                        Position = new Point{x=xx,y=yy}
                                    });
        }

        public void TriggerMouseUp(int button, int xx, int yy)
        {
            if (MouseButtonUp != null)
                MouseButtonUp(this,
                                new MouseEventArgs
                                    {
                                        Button = (MouseButtons)button,
                                        Position = new Point{x=xx,y=yy}
                                    });        
        }

        public void TriggerKeyDown(int vKey)
        {
            if (KeyDown != null)
                KeyDown(this,
                        new KeyEventArgs
                            {
                                Alt = false,
                                Control = false,
                                KeyCode = (KeyCodes) vKey,
                            });
        }

        public void TriggerKeyUp(int vKey)
        {
            if (KeyDown != null)
                KeyUp(this,
                        new KeyEventArgs
                        {
                                Alt = false,
                                Control = false,
                                KeyCode = (KeyCodes) vKey,
                        });
        }

        public void TriggerSizeChanged(int w, int h)
        {
            // _form.Invalidate();
            _width = w;
            _height = h;
            DoResize();
            DoRender();        
        }

        public void TriggerIdle()
        {
            OnIdle(this, new EventArgs());
        }

        private bool _disposed = false;
        private int _mouseWheelPos;
        private bool _initialized;


        private void OnIdle(object idleSender, EventArgs ea)
        {
            // Message message;
            // while (!PeekMessage(out message, IntPtr.Zero, 0, 0, 0))
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
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        public override void Run()
        {
            if (!_initialized)
            {
                DoInit();
                // _width = _form.Width;
                // _height = _form.Height;
                DoResize();
                _initialized = true;
            }
        }


        public void FrameTick(double time)
        {
            // ignore - we create our own timer.
        }

        public Point GetMousePos()
        {
            if (_GetMousePos != null)
            {
                int i = _GetMousePos();
                return new Point { x = (i >> 16) & 0xFFFF, y = (i & 0xFFFF) };
            }
            return new Point {x = 0, y = 0};
        }

        public int GetMouseWheelPos()
        {
            return _mouseWheelPos;
        }

        public event EventHandler<MouseEventArgs> MouseButtonDown;
        public event EventHandler<MouseEventArgs> MouseButtonUp;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;

        public void SetTeapotColor(int color)
        {
            this._theApp.SetTeapotColor(color);
        }



        /*
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
        */
    }
  
}
