using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Image = OpenTK.Windowing.Common.Input.Image;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

// friend: Fusee.Tests.Render.Desktop
[assembly: InternalsVisibleTo("Fusee.Tests.Render.Desktop")]

namespace Fusee.Engine.Imp.Graphics.Desktop
{

    /// <summary>
    /// This is a default render canvas implementation creating its own rendering window.
    /// </summary>
    public class RenderCanvasImp : IRenderCanvasImp
    {
        #region Fields

        //Some tryptichon related variables.
        private bool _videoWallMode = false;
        private int _videoWallMonitorsHor;
        private int _videoWallMonitorsVert;
        private bool _windowBorderHidden = false;

        /// <summary>
        /// Window handle for the window the engine renders to.
        /// </summary>
        public IWindowHandle WindowHandle { get; protected set; }

        /// <summary>
        /// Implementation Tasks: Gets and sets the width(pixel units) of the Canvas.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
            }
        }
        private int _width;

        /// <summary>
        /// Gets and sets the height in pixel units.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height
        {
            get => _height;
            set
            {
                _height = value;
            }
        }
        private int _height;

        /// <summary>
        /// The Y-position (px) of the top left corner of the game window.
        /// </summary>
        protected internal int Top { get => _gameWindow.Bounds.Min.Y; }

        /// <summary>
        /// The X-position (px) of the top left corner of the game window.
        /// </summary>
        protected internal int Left { get => _gameWindow.Bounds.Min.X; }

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
        /// Gets a value indicating whether [focused].
        /// This property is used to identify if this application is the active window of the user.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [focused]; otherwise, <c>false</c>.
        /// </value>
        public bool Focused => _gameWindow.IsFocused;

        #region Tryptichon related Fields.

        /// <summary>
        /// Activates (true) or deactivates (false) the video wall feature.
        /// </summary>
        public bool VideoWallMode
        {
            get => _videoWallMode;
            set => _videoWallMode = value;
        }

        /// <summary>
        /// This represents the number of the monitors in a vertical column.
        /// </summary>
        public int TryptMonitorSetupVertical
        {
            get => _videoWallMonitorsVert;
            set => _videoWallMonitorsVert = value;
        }

        /// <summary>
        /// This represents the number of the monitors in a horizontal row.
        /// </summary>
        public int TryptMonitorSetupHorizontal
        {
            get => _videoWallMonitorsHor;
            set => _videoWallMonitorsHor = value;
        }

        #endregion

        internal RenderCanvasGameWindow _gameWindow;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCanvasImp"/> class.
        /// </summary>
        /// <param name="icon">The window icon to use</param>
        /// <param name="startVisible">Define if window is visible from the start, default: true.</param>
        /// <param name="width">Width of the game window.</param>
        /// <param name="height">Height of the game window.</param>
        /// <param name="minWidth"></param>
        /// <param name="minHeight"></param>
        public RenderCanvasImp(ImageData icon = null, bool startVisible = true, int width = 1280, int height = 720, int minWidth = 360, int minHeight = 640)
        {
            //TODO: Select correct monitor
            MonitorInfo mon = Monitors.GetMonitors()[0];

            if (mon != null)
            {
                minWidth = System.Math.Min(mon.HorizontalResolution - 100, minWidth);
                minHeight = System.Math.Min(mon.VerticalResolution - 100, minHeight);
                width = System.Math.Min(mon.HorizontalResolution - 100, width);
                height = System.Math.Min(mon.VerticalResolution - 100, height);
            }

            try
            {
                _gameWindow = new RenderCanvasGameWindow(this, width, height, false, startVisible, minWidth, minHeight);

            }
            catch
            {
                _gameWindow = new RenderCanvasGameWindow(this, width, height, false, startVisible, minWidth, minHeight);
            }

            WindowHandle = new WindowHandle()
            {
                Handle = _gameWindow.Context.WindowPtr
            };

            _gameWindow.CenterWindow();

            // convert icon to OpenTKImage
            if (icon != null)
            {
                var res = new Span<Rgba32>(new Rgba32[width * height]);
                var pxData = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(icon.PixelData, icon.Width, icon.Height);
                pxData.Mutate(x => x.AutoOrient());
                pxData.Mutate(x => x.RotateFlip(RotateMode.None, FlipMode.Vertical));

                pxData.CopyPixelDataTo(res);

                var resBytes = MemoryMarshal.AsBytes<Rgba32>(res.ToArray());
                _gameWindow.Icon = new WindowIcon(new Image[] { new Image(icon.Width, icon.Height, resBytes.ToArray()) });
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCanvasImp"/> class.
        /// </summary>
        /// <param name="width">The width of the render window.</param>
        /// <param name="height">The height of the render window.</param>
        /// <remarks>The window created by this constructor is not visible. Should only be used for internal testing.</remarks>
        public RenderCanvasImp(int width, int height)
        {
            try
            {
                _gameWindow = new RenderCanvasGameWindow(this, width, height, true);
            }
            catch
            {
                _gameWindow = new RenderCanvasGameWindow(this, width, height, false);
            }

            WindowHandle = new WindowHandle()
            {
                Handle = _gameWindow.Context.WindowPtr
            };

            _gameWindow.IsVisible = false;
        }

        #endregion

        #region Events
        /// <summary>
        /// Occurs when [initialize].
        /// </summary>
        public event EventHandler<InitEventArgs> Init;
        /// <summary>
        /// Occurs when [unload].
        /// </summary>
        public event EventHandler<InitEventArgs> UnLoad;
        /// <summary>
        /// Occurs when [update].
        /// </summary>
        public event EventHandler<RenderEventArgs> Update;
        /// <summary>
        /// Occurs when [render].
        /// </summary>
        public event EventHandler<RenderEventArgs> Render;
        /// <summary>
        /// Occurs when [resize].
        /// </summary>
        public event EventHandler<ResizeEventArgs> Resize;

        /// <summary>
        /// Occurs when [close] is called.
        /// </summary>
        public event EventHandler<CancelEventArgs> Closing;

        #endregion

        #region Members

        /// <summary>
        /// Makes no GraphicsContext current on the calling thread.
        /// Needed in multi-threaded applications if one thread creates the app but it is run on another: call this after <see cref="RenderCanvas.InitApp"/>.
        /// </summary>
        public void MakeNonCurrent()
        {
            _gameWindow.Context.MakeNoneCurrent();
        }

        /// <summary>
        /// Makes the GraphicsContext current on the calling thread.
        /// </summary>
        public void MakeCurrent()
        {
            _gameWindow.Context.MakeCurrent();
        }

        /// <summary>
        /// Does the initialize of this instance.
        /// </summary>
        public virtual void DoInit()
        {
            Init?.Invoke(this, new InitEventArgs());
        }

        /// <summary>
        /// Does the unload of this instance.
        /// </summary>
        public virtual void DoUnLoad()
        {
            UnLoad?.Invoke(this, new InitEventArgs());
        }
        /// <summary>
        /// Does the update of this instance.
        /// </summary>
        public virtual void DoUpdate()
        {
            Update?.Invoke(this, new RenderEventArgs());
        }

        /// <summary>
        /// Does the render of this instance.
        /// </summary>
        public virtual void DoRender()
        {
            Render?.Invoke(this, new RenderEventArgs());
        }

        /// <summary>
        /// Does close the window
        /// </summary>
        public virtual void DoClose(CancelEventArgs e)
        {
            Closing?.Invoke(this, e);
        }

        /// <summary>
        /// Does the resize on this instance.
        /// </summary>
        public virtual void DoResize(int width, int height)
        {
            Resize?.Invoke(this, new ResizeEventArgs(width, height));
        }

        private void ResizeWindow(int width, int height, int left, int top)
        {
            _gameWindow.WindowBorder = _windowBorderHidden ? OpenTK.Windowing.Common.WindowBorder.Hidden : OpenTK.Windowing.Common.WindowBorder.Resizable;
            _gameWindow.Bounds = new OpenTK.Mathematics.Box2i(left, top, width, height);
        }

        /// <summary>
        /// Changes the window of the application to video wall mode.
        /// </summary>
        /// <param name="monitorsHor">Number of monitors on horizontal axis.</param>
        /// <param name="monitorsVert">Number of monitors on vertical axis.</param>
        /// <param name="activate">Start the window in activated state-</param>
        /// <param name="borderHidden">Start the window with a hidden windows border.</param>
        public void VideoWall(int monitorsHor = 1, int monitorsVert = 1, bool activate = true, bool borderHidden = false)
        {
            VideoWallMode = activate;
            _videoWallMonitorsHor = monitorsHor > 0 ? monitorsHor : 1;
            _videoWallMonitorsVert = monitorsVert > 0 ? monitorsVert : 1;
            _windowBorderHidden = borderHidden;

            //TODO: Select correct monitor
            MonitorInfo mon = Monitors.GetMonitors()[0];

            var oneScreenWidth = mon.HorizontalResolution;
            var oneScreenHeight = mon.VerticalResolution;

            var width = oneScreenWidth * _videoWallMonitorsHor;
            var height = oneScreenHeight * _videoWallMonitorsVert;

            ResizeWindow(width, height, 0, 0);
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
            MonitorInfo mon = Monitors.GetMonitors()[0];

            Width = width;
            Height = height;

            var left = (posx == -1) ? mon.HorizontalResolution / 2 - width / 2 : posx;
            var top = (posy == -1) ? mon.VerticalResolution / 2 - height / 2 : posy;

            _windowBorderHidden = borderHidden;

            // Disable video wall mode for this because it would not make sense.
            _videoWallMode = false;

            ResizeWindow(width, height, left, top);
        }

        /// <summary>
        /// Closes the GameWindow with a call to OpenTk.
        /// </summary>
        public virtual void CloseGameWindow()
        {
            if (_gameWindow != null)
            {
                NativeWindow.ProcessWindowEvents(true);
                _gameWindow.Close();
                _gameWindow.Dispose();
            }
        }

        /// <summary>
        /// Presents this application instance. Call this function after rendering to show the final image.
        /// After Present is called the render buffers get flushed.
        /// </summary>
        public virtual void Present()
        {
            if (!_gameWindow.IsExiting)
                _gameWindow.SwapBuffers();
        }

        /// <summary>
        /// Set the cursor (the mouse pointer image) to one of the predefined types
        /// </summary>
        /// <param name="cursorType">The type of the cursor to set.</param>
        public void SetCursor(CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.Standard:
                    _gameWindow.Cursor = MouseCursor.Default;
                    break;
                case CursorType.Hand:
                    _gameWindow.Cursor = MouseCursor.Hand;
                    break;
                case CursorType.HResize:
                    _gameWindow.Cursor = MouseCursor.HResize;
                    break;
                case CursorType.VResize:
                    _gameWindow.Cursor = MouseCursor.VResize;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Opens the given URL in the user's standard web browser. The link MUST start with "http://".
        /// </summary>
        /// <param name="link">The URL to open</param>
        public void OpenLink(string link)
        {
            if (link.StartsWith("http://"))
            {
                //UseShellExecute needs to be set to true in .net 3.0. See:https://github.com/dotnet/corefx/issues/33714
                ProcessStartInfo psi = new()
                {
                    FileName = link,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        /// <summary>
        /// Implementation Tasks: Runs this application instance. This function should not be called more than once as its only for initialization purposes.
        /// </summary>
        public virtual void Run()
        {
            if (_gameWindow != null)
            {
                _gameWindow.UpdateFrequency = 60;
                _gameWindow.Run();
            }
        }

        /// <summary>
        /// Creates a bitmap image from the current frame of the application.
        /// </summary>
        /// <param name="width">The width of the window, and therefore image to render.</param>
        /// <param name="height">The height of the window, and therefore image to render.</param>
        /// <returns></returns>
        public SixLabors.ImageSharp.Image ShootCurrentFrame(int width, int height)
        {
            var mem = new byte[width * height * 4];

            GL.Flush();
            //GL.PixelStore(PixelStoreParameter.PackRowLength, 1);
            GL.ReadPixels(0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, mem);
            //GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Bgra, PixelType.UnsignedByte, mem);

            var img = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(mem, width, height);

            img.Mutate(x => x.AutoOrient());
            img.Mutate(x => x.RotateFlip(RotateMode.None, FlipMode.Vertical));
            return img;
        }

        #endregion
    }
}