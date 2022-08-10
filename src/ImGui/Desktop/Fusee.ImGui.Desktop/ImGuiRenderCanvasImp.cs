using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using System;
using System.Threading;

namespace Fusee.ImGuiImp.Desktop
{
    struct UniformFieldInfo
    {
        public int Location;
        public string Name;
        public int Size;
        public ActiveUniformType Type;
    }

    /// <summary>
    /// Implementation of the cross-platform abstraction of the window handle.
    /// </summary>
    public class WindowHandle : IWindowHandle
    {
        /// <summary>
        /// The Window Handle as IntPtr
        /// </summary>
        public IntPtr Handle { get; internal set; }
    }

    public class ImGuiRenderCanvasImp : IRenderCanvasImp
    {
        private static bool _initialized = false;

        public IWindowHandle WindowHandle { get; private set; }

        /// <summary>
        /// The Width
        /// </summary>
        protected internal int BaseWidth;

        /// <summary>
        /// The Height
        /// </summary>
        protected internal int BaseHeight;

        /// <summary>
        /// The Top Position
        /// </summary>
        protected internal int BaseTop;

        /// <summary>
        /// The Left Position
        /// </summary>
        protected internal int BaseLeft;

        private ImGuiController _controller;
        private bool _isShuttingDown;

        public ImGuiRenderCanvasImp(ImageData? icon = null, bool isMultithreaded = false)
        {

            int width = 1280;
            int height = 720;

            try
            {
                _gameWindow = new RenderCanvasGameWindow(this, width, height, false, isMultithreaded);
            }
            catch
            {
                _gameWindow = new RenderCanvasGameWindow(this, width, height, false, isMultithreaded);
            }

            WindowHandle = new WindowHandle()
            {
                Handle = _gameWindow.Context.WindowPtr
            };

            _gameWindow.CenterWindow();

            _controller = new ImGuiController(_gameWindow);
        }

        public void Run()
        {
            if (_gameWindow != null)
            {
                _gameWindow.UpdateFrequency = 0;
                _gameWindow.RenderFrequency = 0;

                _gameWindow.Run();
            }
        }

        /// <summary>
        /// Implementation Tasks: Presents the final rendered image. The FrameBuffer needs to be cleared afterwards.
        /// The delta time needs to be recalculated when this function is called.
        /// </summary>
        public void Present()
        {
            // do nothing, pipe all Present() calls inside OnRenderFrame() to /dev/null
            // we call present ourselves
        }

        protected internal void DoInit()
        {
            _controller.InitImGUI(14, "Assets/Lato-Black.ttf");
            Init?.Invoke(this, new InitEventArgs());
            _initialized = true;
        }

        protected internal void DoUpdate()
        {
            if (!_initialized) return;
            if (_isShuttingDown) return;

            // HACK(mr): Fixme, don't know why
            //Input.Instance.PreUpdate();

            Update?.Invoke(this, new RenderEventArgs());
            _controller.UpdateImGui(DeltaTimeUpdate);

            //Input.Instance.PostUpdate();
        }

        protected internal void DoRender()
        {
            if (!_initialized) return;
            if (_controller.GameWindowWidth <= 0) return;
            if (_isShuttingDown) return;

            Input.Instance.PreUpdate();

            Render?.Invoke(this, new RenderEventArgs());

            if (_isShuttingDown) return;

            _controller.RenderImGui();
            Input.Instance.PostUpdate();
        }

        protected internal void DoUnLoad()
        {
            UnLoad?.Invoke(this, new InitEventArgs());
        }

        protected internal void DoResize(int width, int height)
        {
            Resize?.Invoke(this, new ResizeEventArgs(width, height));
            _controller?.WindowResized(width, height);
        }

        public void SetCursor(CursorType cursorType)
        {
            throw new NotImplementedException();
        }

        public void OpenLink(string link)
        {
            //throw new NotImplementedException();
        }

        public void SetWindowSize(int width, int height, int posx = -1, int posy = -1, bool borderHidden = false)
        {
            throw new NotImplementedException();
        }

        public void CloseGameWindow()
        {
            _isShuttingDown = true;
            NativeWindow.ProcessWindowEvents(true);
            _controller.Dispose();
            _gameWindow.Context.MakeCurrent();
            _gameWindow.Close();
            _gameWindow.Dispose();
        }

        private void ResizeWindow()
        {
            _controller.WindowResized(Width, Height);
        }

        /// <summary>
        /// Implementation Tasks: Gets and sets the width(pixel units) of the Canvas.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width
        {
            get => BaseWidth;
            set
            {
                //_gameWindow.Size = new OpenTK.Mathematics.Vector2i(value, _gameWindow.Size.Y);
                BaseWidth = value;
                ResizeWindow();
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
            get => BaseHeight;
            set
            {
                //_gameWindow.Size = new OpenTK.Mathematics.Vector2i(_gameWindow.Size.X, value);
                BaseHeight = value;
                ResizeWindow();
            }
        }

        /// <summary>
        /// Gets and sets the caption(title of the window).
        /// </summary>
        /// <value>
        /// The caption.
        /// </value>
        public string Caption
        {
            get => (_gameWindow == null) ? "" : _gameWindow.Title;
            set { if (_gameWindow != null) _gameWindow.Title = value; }
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
                if (_gameWindow != null)
                    return _gameWindow.DeltaTime;
                return 0.01f;
            }
        }

        /// <summary>
        /// Gets the delta time.
        /// The delta time is the time that was required to update the last frame in milliseconds.
        /// </summary>
        /// <value>
        /// The delta time in milliseconds.
        /// </value>
        public float DeltaTimeUpdate
        {
            get
            {
                if (_gameWindow != null)
                    return _gameWindow.DeltaTimeUpdate;
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
            get => (_gameWindow != null) && _gameWindow.VSync == OpenTK.Windowing.Common.VSyncMode.On;
            set { if (_gameWindow != null) _gameWindow.VSync = value ? OpenTK.Windowing.Common.VSyncMode.On : OpenTK.Windowing.Common.VSyncMode.Off; }
        }

        /// <summary>
        /// Gets and sets a value indicating whether [enable blending].
        /// Blending is used to render transparent objects.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable blending]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableBlending
        {
            get => _gameWindow.Blending;
            set => _gameWindow.Blending = value;
        }

        /// <summary>
        /// Gets and sets a value indicating whether [fullscreen] is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fullscreen]; otherwise, <c>false</c>.
        /// </value>
        public bool Fullscreen
        {
            get => (_gameWindow.WindowState == OpenTK.Windowing.Common.WindowState.Fullscreen);
            set => _gameWindow.WindowState = (value) ? OpenTK.Windowing.Common.WindowState.Fullscreen : OpenTK.Windowing.Common.WindowState.Normal;
        }

        /// <summary>
        /// Occurs when [initialize].
        /// </summary>
        public event EventHandler<InitEventArgs>? Init;
        /// <summary>
        /// Occurs when [unload].
        /// </summary>
        public event EventHandler<InitEventArgs>? UnLoad;
        /// <summary>
        /// Occurs when [update].
        /// </summary>
        public event EventHandler<RenderEventArgs>? Update;
        /// <summary>
        /// Occurs when [render].
        /// </summary>
        public event EventHandler<RenderEventArgs>? Render;
        /// <summary>
        /// Occurs when [resize].
        /// </summary>
        public event EventHandler<ResizeEventArgs>? Resize;

        internal readonly RenderCanvasGameWindow _gameWindow;

    }

    public class RenderCanvasGameWindow : GameWindow
    {
        #region Fields

        private readonly ImGuiRenderCanvasImp _renderCanvasImp;

        /// <summary>
        /// Gets the delta time.
        /// The delta time is the time that was required to render the last frame in milliseconds.
        /// This value can be used to determine the frames per second of the application.
        /// </summary>
        /// <value>
        /// The delta time in milliseconds.
        /// </value>
        public float DeltaTime { get; private set; }

        /// <summary>
        /// Gets the delta time.
        /// The delta time is the time that was required to update the last frame in milliseconds.
        /// </summary>
        /// <value>
        /// The delta time in milliseconds.
        /// </value>
        public float DeltaTimeUpdate { get; private set; }

        /// <summary>
        /// Gets and sets a value indicating whether [blending].
        /// Blending is used to render transparent objects.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [blending]; otherwise, <c>false</c>.
        /// </value>
        public bool Blending
        {
            get => GL.IsEnabled(EnableCap.Blend);
            set
            {
                if (value)
                {
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                }
                else
                {
                    GL.Disable(EnableCap.Blend);
                }
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCanvasGameWindow"/> class.
        /// </summary>
        /// <param name="renderCanvasImp">The render canvas implementation.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="antiAliasing">if set to <c>true</c> [anti aliasing] is on.</param>
        /// <param name="isMultithreaded">If true OpenTk will call run() in a new Thread. The default value is false.</param>
        public RenderCanvasGameWindow(ImGuiRenderCanvasImp renderCanvasImp, int width, int height, bool antiAliasing, bool isMultithreaded = false)
            : base(new GameWindowSettings { IsMultiThreaded = isMultithreaded }, new NativeWindowSettings { Size = new OpenTK.Mathematics.Vector2i(width, height), Profile = OpenTK.Windowing.Common.ContextProfile.Core, Flags = OpenTK.Windowing.Common.ContextFlags.ForwardCompatible })
        {
            _renderCanvasImp = renderCanvasImp;
            _renderCanvasImp.BaseWidth = width;
            _renderCanvasImp.BaseHeight = height;
        }

        #endregion

        #region Overrides

        protected override void OnLoad()
        {
            // Check for necessary capabilities
            string version = GL.GetString(StringName.Version);

            int major = version[0];
            // int minor = (int)version[2];

            if (major < 2)
            {
                throw new InvalidOperationException("You need at least OpenGL 2.0 to run this example. GLSL not supported.");
            }

            GL.ClearColor(25, 25, 112, byte.MaxValue);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            // Use VSync!
            //VSync = OpenTK.Windowing.Common.VSyncMode.On;

            _renderCanvasImp.DoInit();
        }

        protected override void OnUnload()
        {
            _renderCanvasImp.DoUnLoad();
            // _renderCanvasImp.Dispose(); // TODO(mr): Implement
        }

        protected override void OnResize(OpenTK.Windowing.Common.ResizeEventArgs e)
        {
            base.OnResize(e);

            if (_renderCanvasImp != null)
            {
                _renderCanvasImp.BaseWidth = e.Width;
                _renderCanvasImp.BaseHeight = e.Height;
                _renderCanvasImp.DoResize(e.Width, e.Height);
            }
        }

        protected override void OnUpdateFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            DeltaTimeUpdate = (float)args.Time;

            if (KeyboardState.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.F11))
                WindowState = (WindowState != OpenTK.Windowing.Common.WindowState.Fullscreen) ? OpenTK.Windowing.Common.WindowState.Fullscreen : OpenTK.Windowing.Common.WindowState.Normal;

            _renderCanvasImp?.DoUpdate();
        }

        protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            DeltaTime = (float)args.Time;

            _renderCanvasImp?.DoRender();
        }

        #endregion
    }
}