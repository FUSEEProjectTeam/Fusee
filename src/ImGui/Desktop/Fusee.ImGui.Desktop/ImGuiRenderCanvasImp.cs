using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Imp.Graphics.Desktop;
using Fusee.Math.Core;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.DImGui.Desktop
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


        public ImGuiRenderCanvasImp(ImageData? icon = null, bool isMultithreaded = false)
        {
            int width = 1280;
            int height = 720;

            try
            {
                _gameWindow = new DImGui.Desktop.RenderCanvasGameWindow(this, width, height, false, isMultithreaded);
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
            if (_gameWindow.IsMultiThreaded)
                _gameWindow.Context.MakeNoneCurrent();

            // convert icon to OpenTKImage
            if (icon != null)
            {
                // convert Bgra to Rgba for OpenTK.WindowIcon
                var pxData = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(icon.PixelData, icon.Width, icon.Height);
                var bgra = pxData.CloneAs<Rgba32>();
                bgra.Mutate(x => x.AutoOrient());
                bgra.Mutate(x => x.RotateFlip(RotateMode.None, FlipMode.Vertical));

                if (!bgra.TryGetSinglePixelSpan(out var res))
                {
                    Diagnostics.Warn("Couldn't convert icon image to Rgba32!");
                    return;
                }
                var resBytes = MemoryMarshal.AsBytes<Rgba32>(res.ToArray());
                _gameWindow.Icon = new WindowIcon(new Image[] { new Image(icon.Width, icon.Height, resBytes.ToArray()) });
            }

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
            Init?.Invoke(this, new InitEventArgs());
            _controller.InitImGUI();
            _initialized = true;
        }

        protected internal void DoUpdate()
        {
            Update?.Invoke(this, new RenderEventArgs());

            if (!_initialized) return;

            _controller.UpdateImGui(DeltaTimeUpdate);
        }

        protected internal void DoRender()
        {
            if (!_initialized) return;

            Input.Instance.PreRender();
            Time.Instance.DeltaTimeIncrement = DeltaTime;

            #region FuseeRender

            if (_controller.FuseeViewportSize.X != 0)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

                Width = (int)_controller.FuseeViewportSize.X;
                Height = (int)_controller.FuseeViewportSize.Y;

                // Enable FB
                _controller.UpdateRenderTexture(Width, Height);
                DoResize(Width, Height);
                Render?.Invoke(this, new RenderEventArgs());

                // after rendering, blt result into ViewportRenderTexture
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _controller.ViewportFramebuffer);
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _controller.IntermediateFrameBuffer);
                GL.BlitFramebuffer(0, 0, Width, Height, 0, 0, Width, Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

            }

            // Disable FB
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Viewport(0, 0, _gameWindow.Size.X, _gameWindow.Size.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            #endregion

            _controller.RenderImGui();
            Input.Instance.PostRender();
            _gameWindow.SwapBuffers();
        }

        protected internal void DoUnLoad()
        {
            UnLoad?.Invoke(this, new InitEventArgs());
        }

        protected internal void DoResize(int width, int height)
        {
            Resize?.Invoke(this, new ResizeEventArgs(width, height));
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
            throw new NotImplementedException();
        }

        private void ResizeWindow()
        {
            //_gameWindow.WindowBorder = _windowBorderHidden ? OpenTK.Windowing.Common.WindowBorder.Hidden : OpenTK.Windowing.Common.WindowBorder.Resizable;
            //_gameWindow.Bounds = new OpenTK.Mathematics.Box2i(BaseLeft, BaseTop, BaseWidth, BaseHeight);
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

        internal readonly DImGui.Desktop.RenderCanvasGameWindow _gameWindow;

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
