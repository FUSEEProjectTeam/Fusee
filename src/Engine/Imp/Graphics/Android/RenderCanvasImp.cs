using System;
using System.Diagnostics;
using Android.Util;
using Android.Content;
using Android.Views;
using OpenTK;
using Fusee.Engine.Common;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Platform.Android;
using Uri = Android.Net.Uri;

namespace Fusee.Engine.Imp.Graphics.Android
{
    /// <summary>
    /// This is a default render canvas implementation creating its own rendering window.
    /// </summary>
    public class RenderCanvasImp : IRenderCanvasImp
    {
        #region Fields


        /// <summary>
        /// Implementation Tasks: Gets and sets the width(pixel units) of the Canvas.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width
        {
            get { return _gameView.Width; }
            set
            {
                _gameView.Width = value;
            }
        }

        /// <summary>
        /// Gets and sets the height in pixel units.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height
        {
            get { return _gameView.Height; }
            set
            {
                _gameView.Height = value;
            }
        }


        private string _title;
        /// <summary>
        /// Gets and sets the caption(title of the window).
        /// </summary>
        /// <value>
        /// The caption.
        /// </value>
        public string Caption
        {
            get { return _title; }
            set
            {
                 _title = value;
            }
        }

        /// <summary>
        /// Gets the delta time.
        /// The delta time is the time that was required to render the last frame in milliseconds.
        /// This value can be used to determine the frames per second of the application.
        /// </summary>
        /// <value>
        /// The delta time in milliseconds.
        /// </value>
        public float DeltaTime
        {
            get
            {
                if (_gameView != null)
                    return _gameView.DeltaTime;
                return 0.01f;
            }
        }

        /// <summary>
        /// Gets and sets a value indicating whether [vertical synchronize].
        /// This option is used to reduce "Glitches" during rendering.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [vertical synchronize]; otherwise, <c>false</c>.
        /// </value>
        public bool VerticalSync
        {
            get { return false; }
            set {  }
        }

        /// <summary>
        /// Gets and sets a value indicating whether [fullscreen] is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fullscreen]; otherwise, <c>false</c>.
        /// </value>
        public bool Fullscreen
        {
            get { return (_gameView.WindowState == WindowState.Fullscreen); }
            set { _gameView.WindowState = (value) ? WindowState.Fullscreen : WindowState.Normal; }
        }

        /// <summary>
        /// Gets a value indicating whether [focused].
        /// This property is used to identify if this application is the active window of the user.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [focused]; otherwise, <c>false</c>.
        /// </value>
        public bool Focused
        {
            get { return _gameView.Focused; }
        }

        // Some tryptichon related Fields.

        /// <summary>
        /// Activates (true) or deactivates (false) the video wall feature.
        /// </summary>
        public bool VideoWallMode
        {
            get { return false; }
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        /// <summary>
        /// This represents the number of the monitors in a vertical column.
        /// </summary>
        public int TryptMonitorSetupVertical
        {
            get { return 1; }
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        /// <summary>
        /// This represents the number of the monitors in a horizontal row.
        /// </summary>
        public int TryptMonitorSetupHorizontal
        {
            get { return 1; }
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        /// <summary>
        /// Gets and sets the android view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public View View => _gameView;

        internal RenderCanvasGameView _gameView;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCanvasImp"/> class.
        /// </summary>
        public RenderCanvasImp(Context context, IAttributeSet attrs, Action run)
        {
            _gameView = new RenderCanvasGameView(this, true, context, attrs, run);
        }

        /// <summary>
        /// Implementation of the Dispose pattern. Disposes of the OpenTK game window.
        /// </summary>
        public void Dispose()
        {
            _gameView.Dispose();
        }

        #endregion

        #region Members


        /// <summary>
        /// Changes the window of the application to video wall mode.
        /// </summary>
        /// <param name="monitorsHor">Number of monitors on horizontal axis.</param>
        /// <param name="monitorsVert">Number of monitors on vertical axis.</param>
        /// <param name="activate">Start the window in activated state-</param>
        /// <param name="borderHidden">Start the window with a hidden windows border.</param>
        public void VideoWall(int monitorsHor = 1, int monitorsVert = 1, bool activate = true, bool borderHidden = false)
        {
        }

        /// <summary>
        /// Sets the size of the output window for desktop development.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="posx">The x position of the window.</param>
        /// <param name="posy">The y position of the window.</param>
        /// <param name="borderHidden">Show the window border or not.</param>
        public void SetWindowSize(int width, int height, int posx = -1, int posy = -1, bool borderHidden = false)
        {
        }

        /// <summary>
        /// Closes the GameWindow with a call to opentk.
        /// </summary>
        public void CloseGameWindow()
        {
        }

        /// <summary>
        /// Presents this application instance. Call this function after rendering to show the final image. 
        /// After Present is called the render buffers get flushed.
        /// </summary>
        public void Present()
        {
            _gameView?.SwapBuffers();
        }

        /// <summary>
        /// Set the cursor (the mouse pointer image) to one of the pre-defined types
        /// </summary>
        /// <param name="cursorType">The type of the cursor to set.</param>
        public void SetCursor(CursorType cursorType)
        {
            // Currently not supported by OpenTK... Too bad.
        }

        /// <summary>
        /// Opens the given URL in the user's standard web browser. The link MUST start with "http://".
        /// </summary>
        /// <param name="link">The URL to open</param>
        public void OpenLink(string link)
        {
            if (link.StartsWith("http://"))
            {
                var intent = new Intent(Intent.ActionView);
                intent.SetData(Uri.Parse(link));
                _gameView.Context.StartActivity(intent);
            }
        }

        /// <summary>
        /// Implementation Tasks: Runs this application instance. This function should not be called more than once as it is only for initilization purposes.
        /// </summary>
        public void Run()
        {
            if (_gameView != null)
                _gameView.Run(30.0);

            Width = _gameView.Size.Width;
            Height = _gameView.Size.Height;
            
        }

        #endregion

        #region Events
        /// <summary>
        /// Occurs when [initialize].
        /// </summary>
        public event EventHandler<InitEventArgs> Init;
        /// <summary>
        /// Occurs when [un load].
        /// </summary>
        public event EventHandler<InitEventArgs> UnLoad;
        /// <summary>
        /// Occurs when [render].
        /// </summary>
        public event EventHandler<RenderEventArgs> Render;
        /// <summary>
        /// Occurs when [resize].
        /// </summary>
        public event EventHandler<ResizeEventArgs> Resize;

        #endregion

        #region Internal Members
        /// <summary>
        /// Does the initialize of this instance.
        /// </summary>
        protected internal void DoInit()
        {
            if (Init != null)
                Init(this, new InitEventArgs());
        }

        /// <summary>
        /// Does the unload of this instance. 
        /// </summary>
        protected internal void DoUnLoad()
        {
            if (UnLoad != null)
                UnLoad(this, new InitEventArgs());
        }

        /// <summary>
        /// Does the render of this instance.
        /// </summary>
        protected internal void DoRender()
        {
            if (Render != null)
                Render(this, new RenderEventArgs());
        }

        /// <summary>
        /// Does the resize on this instance.
        /// </summary>
        protected internal void DoResize(int width, int height)
        {
            if (Resize != null)
                Resize(this, new ResizeEventArgs(width, height));
        }
        #endregion

    }


    public class RenderCanvasGameView : AndroidGameView
    {
        #region Fields
        private RenderCanvasImp _renderCanvasImp;
        private float _deltaTime;
        private Action _run;
        internal Context AndroidContext;
        private Stopwatch _stopwatch = new Stopwatch();
        #endregion

        /// <summary>
        /// Gets the delta time.
        /// The delta time is the time that was required to render the last frame in milliseconds.
        /// This value can be used to determine the frames per second of the application.
        /// </summary>
        /// <value>
        /// The delta time in milliseconds.
        /// </value>
        public float DeltaTime
        {
            get { return _deltaTime; }
        }


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCanvasGameView"/> class.
        /// </summary>
        /// <param name="renderCanvasImp">The render canvas implementation.</param>
        /// <param name="antiAliasing">if set to <c>true</c> perform Antialiasing.</param>
        /// <param name="context">The Android context.</param>
        /// <param name="attrs">An Android View attribute set.</param>
        /// <param name="run"></param>
        public RenderCanvasGameView(RenderCanvasImp renderCanvasImp, bool antiAliasing, Context context, IAttributeSet attrs, Action run) :
            base(context, attrs)
        {
            _renderCanvasImp = renderCanvasImp;
            AndroidContext = context;
            _run = run;
        }

        #endregion

        #region Overrides
        protected override void OnLoad(EventArgs e)
        {
            // Check for necessary capabilities
            string version = GL.GetString(All.Version);

            int major = version[0];
            // int minor = (int)version[2];

            if (major < 2)
            {
                throw new InvalidOperationException("You need at least OpenGL 2.0 to run this example. GLSL not supported.");
            }

            GL.ClearColor(0, 0.3f, 0.1f, 1);

            GL.Enable(All.DepthTest);
            GL.Enable(All.CullFace);

            // Use VSync!
            // Context.SwapInterval = 1;
            _run?.Invoke();

            _renderCanvasImp.DoInit();
            _stopwatch.Start();
        }

        protected override void OnUnload(EventArgs e)
        {
            _renderCanvasImp.DoUnLoad();
            _renderCanvasImp.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            if (_renderCanvasImp != null)
            {
                _renderCanvasImp.DoResize(Width, Height);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _deltaTime = (float)_stopwatch.ElapsedMilliseconds / 1000.0f;
            _stopwatch.Restart();

            if (_renderCanvasImp != null)
                _renderCanvasImp.DoRender();
        }

        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        protected override void CreateFrameBuffer()
        {
            ContextRenderingApi = GLVersion.ES3;

            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try
            {
                Log.Verbose("FUSEE Android Game View", "Loading with default settings");

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("FUSEE Android Game View", "{0}", ex);
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try
            {
                Log.Verbose("FUSEE Android Game View", "Loading with custom Android settings (low mode)");
                GraphicsMode = new AndroidGraphicsMode();//0, 0, 0, 0, 0, false);

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("FUSEE Android Game View", "{0}", ex);
            }
            throw new Exception("Can't load egl, aborting");
        }

        #endregion
    }
}