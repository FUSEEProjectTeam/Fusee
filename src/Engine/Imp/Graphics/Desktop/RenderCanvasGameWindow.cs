using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.ComponentModel;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    internal class RenderCanvasGameWindow : GameWindow
    {
        #region Fields

        private readonly RenderCanvasImp _renderCanvasImp;

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
        /// <param name="minimumWidth">The minimum width of the game window.</param>
        /// <param name="minimumHeight">The minimum height of the game window</param>
        /// <param name="startVisible">Should the window be visible from the start, default: true.</param>
        public RenderCanvasGameWindow(RenderCanvasImp renderCanvasImp, int width, int height, bool antiAliasing, bool startVisible = true, int minimumWidth = 1280, int minimumHeight = 720)
            : base(new GameWindowSettings(), new NativeWindowSettings
            {
                Size = new OpenTK.Mathematics.Vector2i(width, height),
                Profile = OpenTK.Windowing.Common.ContextProfile.Core,
                Flags = OpenTK.Windowing.Common.ContextFlags.ForwardCompatible,
                StartVisible = startVisible,
                MinimumSize = new OpenTK.Mathematics.Vector2i(minimumWidth, minimumHeight)
            })
        {
            _renderCanvasImp = renderCanvasImp;
            _renderCanvasImp.Width = width;
            _renderCanvasImp.Height = height;
        }

        #endregion

        #region Overrides

        protected override void OnLoad()
        {
            // Check for necessary capabilities
            string version = GL.GetString(StringName.Version);

            int major = version[0];
            // int minor = (int)version[2];

            if (major < 3)
            {
                throw new InvalidOperationException("You need at least OpenGL 3.0 to run this application. GLSL not supported.");
            }

            // Use VSync!
            VSync = OpenTK.Windowing.Common.VSyncMode.On;

            _renderCanvasImp.DoInit();
        }

        protected override void OnUnload()
        {
            _renderCanvasImp.DoUnLoad();
        }

        protected override void OnResize(OpenTK.Windowing.Common.ResizeEventArgs e)
        {
            base.OnResize(e);

            if (_renderCanvasImp != null)
            {
                _renderCanvasImp.Width = e.Width;
                _renderCanvasImp.Height = e.Height;
                _renderCanvasImp.DoResize(e.Width, e.Height);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _renderCanvasImp.DoClose(e);
            base.OnClosing(e);
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