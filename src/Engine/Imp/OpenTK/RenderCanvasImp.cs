using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace Fusee.Engine
{
    /// <summary>
    /// Use this class as a base class for implementing connectivity to whatever windows system you intend to support.
    /// Inherit from this class, make sure to call the constructor with the window handle to render on, implement the
    /// Run method and call the DoInit, DoUnload, DoRender and DoResize methods at appropriate incidences. Make sure
    /// that _width and _height are set to the new window size before calling DoResize.
    /// In addition you might have your connectivity class as well implement the <see cref="IInputImp"/> interface because
    /// often mouse and keyboard input are tied to the windows output.
    /// </summary>
    public abstract class RenderCanvasWindowImp : RenderCanvasImpBase, IRenderCanvasImp, IDisposable
    {
        internal IWindowInfo _wi;
        internal IGraphicsContext _context;
        internal GraphicsMode _mode;
        internal int _major, _minor;
        internal GraphicsContextFlags _flags;
        public int Width
        {
            get { return _width; }
            set { throw new NotImplementedException("Cannot (yet) set width on RenderContextWindowImp");}
        }
        public int Height        
        {
            get { return _height; }
            set { throw new NotImplementedException("Cannot (yet) set height on RenderContextWindowImp");}
        }

        public string Caption { get; set; }
        private double _lastTimeTick;
        private double _deltaFrameTime;
        private static Stopwatch _daWatch;

        public RenderCanvasWindowImp(IntPtr windowHandle)
        {
            // _mode = GraphicsMode.Default;
            bool antiAliasing = true;
            _mode = new GraphicsMode(32, 24, 0, (antiAliasing) ? 8 : 0);
            _major = 1;
            _minor = 0;
            _flags = GraphicsContextFlags.Default;
            _wi = Utilities.CreateWindowsWindowInfo(windowHandle);

            try
            {
                _context = new GraphicsContext(_mode, _wi, _major, _minor, _flags);
            }
            catch
            {
                antiAliasing = false;
                _mode = new GraphicsMode(32, 24, 0, (antiAliasing) ? 8 : 0);
                _context = new GraphicsContext(_mode, _wi, _major, _minor, _flags);
            }

            _context.MakeCurrent(_wi);
            ((IGraphicsContextInternal)_context).LoadAll();

            GL.ClearColor(Color.MidnightBlue);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            // Use VSync!
            _context.SwapInterval = 1;
            _lastTimeTick = Timer;
        }

        public double DeltaTime
        {
            get
            {
                return _deltaFrameTime;
            }
        }


        public bool VerticalSync
        {
            get { return _context.SwapInterval == 1; }
            set { _context.SwapInterval = (value) ? 1 : 0; }
        }

        public bool EnableBlending { get; set; }
        public bool Fullscreen { get; set; }

        public static double Timer
        {
            get
            {
                if (_daWatch == null)
                {
                    _daWatch = new Stopwatch();
                    _daWatch.Start();
                }
                return ((double)_daWatch.ElapsedTicks) / ((double)Stopwatch.Frequency);
            }
        }

        public void Present()
        {
            // Recalculate time tick.
            double newTick = Timer;
            _deltaFrameTime = newTick - _lastTimeTick;
            _lastTimeTick = newTick;

            // _context.MakeCurrent(_wi);
            _context.SwapBuffers();
        }

        public abstract void Run();

        private bool _disposed = false;

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                _context.Dispose();
                _context = null;
                _wi.Dispose();
                _wi = null;
                _disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~RenderCanvasWindowImp()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

    }

    /// <summary>
    /// This is a default render canvas implementation creating its own rendering window.
    /// </summary>
    public class RenderCanvasImp : RenderCanvasImpBase, IRenderCanvasImp
    {
        public int Width
        {
            get { return _width; }
            set
            {
                _gameWindow.Width = value;
                _width = value;
                ResizeWindow();
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                _gameWindow.Height = value;
                _height = value;
                ResizeWindow();
            }
        }

        private void ResizeWindow()
        {
            var width2 = _width / 2;
            var height2 = _height / 2;

            var scHeight2 = Screen.PrimaryScreen.Bounds.Height / 2;
            var scWidth2 = Screen.PrimaryScreen.Bounds.Width / 2;

            _gameWindow.Bounds = new Rectangle(scWidth2 - width2, scHeight2 - height2, _width, _height);
        }

        public string Caption
        {
            get { return (_gameWindow == null) ? "" : _gameWindow.Title; }
            set { if (_gameWindow != null) _gameWindow.Title = value; }
        }

        public double DeltaTime
        {
            get
            {
                if (_gameWindow != null)
                    return _gameWindow.DeltaTime;
                return 0.01f;
            }
        }

        public bool VerticalSync
        {
            get { return (_gameWindow != null) && _gameWindow.Context.SwapInterval == 1; }
            set { if (_gameWindow != null) _gameWindow.Context.SwapInterval = (value) ? 1 : 0; }
        }

        public bool EnableBlending
        {
            get { return _gameWindow.Blending; }
            set { _gameWindow.Blending = value; }
        }

        public bool Fullscreen
        {
            get { return (_gameWindow.WindowState == WindowState.Fullscreen); }
            set { _gameWindow.WindowState = (value) ? WindowState.Fullscreen : WindowState.Normal; }
        }

        public bool Focused
        {
            get { return _gameWindow.Focused; }
        }

        internal RenderCanvasGameWindow _gameWindow;

        public RenderCanvasImp()
        {
            const int width = 1280;
            var height = System.Math.Min(Screen.PrimaryScreen.Bounds.Height - 100, 720);

            try
            {
                _gameWindow = new RenderCanvasGameWindow(this, width, height, true);
            }
            catch
            {
                _gameWindow = new RenderCanvasGameWindow(this, width, height, false);
            }
        }

        public void Present()
        {
            if (_gameWindow != null)
                _gameWindow.SwapBuffers();
        }

        public void Run()
        {
            if (_gameWindow != null)
                _gameWindow.Run(30.0, 0.0);
        }
    }

    public class RenderCanvasImpBase
    {
        protected internal int _width;
        protected internal int _height;

        public event EventHandler<InitEventArgs> Init;
        public event EventHandler<InitEventArgs> UnLoad;
        public event EventHandler<RenderEventArgs> Render;
        public event EventHandler<ResizeEventArgs> Resize;

        internal protected void DoInit()
        {
            if (Init != null)
                Init(this, new InitEventArgs());
        }

        internal protected void DoUnLoad()
        {
            if (UnLoad != null)
                UnLoad(this, new InitEventArgs());
        }

        internal protected void DoRender()
        {
            if (Render != null)
                Render(this, new RenderEventArgs());
        }

        internal protected void DoResize()
        {
            if (Resize != null)
                Resize(this, new ResizeEventArgs());
        }
    }

    class RenderCanvasGameWindow : GameWindow
    {
        private RenderCanvasImp _renderCanvasImp;
        private double _deltaTime;

        public double DeltaTime
        {
            get { return _deltaTime; }
        }

        public bool Blending
        {
            get { return GL.IsEnabled(EnableCap.Blend); }
            set
            {
                if (value)
                {
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                }
                else
                {
                    GL.Disable(EnableCap.Blend);
                }
            }
        }

        public RenderCanvasGameWindow(RenderCanvasImp renderCanvasImp, int width, int height, bool antiAliasing)
            : base(width, height, new GraphicsMode(32, 24, 0, (antiAliasing) ? 8 : 0) /*GraphicsMode.Default*/, "Fusee Engine")
        {
            _renderCanvasImp = renderCanvasImp;
        }

        protected override void OnLoad(EventArgs e)
        {
            // Check for necessary capabilities:
            string version = GL.GetString(StringName.Version);

            int major = (int)version[0];
            // int minor = (int)version[2];

            if (major < 2)
            {
                MessageBox.Show("You need at least OpenGL 2.0 to run this example. Aborting.", "GLSL not supported",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Exit();
            }

            GL.ClearColor(Color.MidnightBlue);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            // Use VSync!
            Context.SwapInterval = 1;

            _renderCanvasImp.DoInit();
        }

        protected override void OnUnload(EventArgs e)
        {
            _renderCanvasImp.DoUnLoad();
            // if (_renderCanvasImp != null)
            //     _renderCanvasImp.Dispose();      
        }


        protected override void OnResize(EventArgs e)
        {
            if (_renderCanvasImp != null)
            {
                _renderCanvasImp._width = Width;
                _renderCanvasImp._height = Height;
                _renderCanvasImp.DoResize();
            }

            /*
            GL.Viewport(0, 0, Width, Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
             * */
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[OpenTK.Input.Key.Escape])
                this.Exit();

            if (Keyboard[OpenTK.Input.Key.F11])
                if (WindowState != WindowState.Fullscreen)
                    WindowState = WindowState.Fullscreen;
                else
                    WindowState = WindowState.Normal;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _deltaTime = e.Time;
            if (_renderCanvasImp != null)
            {
                _renderCanvasImp.DoRender();
            }
        }
    }
}