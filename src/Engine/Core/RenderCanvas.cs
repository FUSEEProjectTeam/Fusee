using System;
using Fusee.Base.Common;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    ///     A render canvas object references the physical output screen space real estate (e.g. the rendering window).
    ///     A typical Game application will inherit from this class and overwrite methods to implement your user code to
    ///     to be performed on events like initialization, resize, and display refresh.
    ///     In the future, it will be likely that this class' functionality will be divided at two different places with
    ///     one containing the more view oriented aspects and the other containing the more application oriented aspects.
    /// </summary>
    public class RenderCanvas
    {
        #region Implementor Fields

        /// <summary>
        ///     Gets and sets the canvas implementor.
        /// </summary>
        /// <value>
        ///     The canvas implementor.
        /// </value>
        [InjectMe]
        public IRenderCanvasImp CanvasImplementor { set; get; }

        /// <summary>
        ///     Gets and sets the RenderContext implementor.
        /// </summary>
        /// <value>
        ///     The context implementor.
        /// </value>
        [InjectMe]
        public IRenderContextImp ContextImplementor { set; get; }

        /// <summary>
        ///     Gets and sets the audio implementor.
        /// </summary>
        /// <value>
        ///     The audio implementor.
        /// </value>
        [InjectMe]
        public IAudioImp AudioImplementor { set; get; }

        /// <summary>
        ///     Gets and sets the input driver implementor.
        /// </summary>
        /// <value>
        ///     The input driver implementor.
        /// </value>
        [InjectMe]
        public IInputDriverImp InputDriverImplementor { set; get; }

        /// <summary>
        ///     Gets and sets the video manager implementor.
        /// </summary>
        /// <value>
        ///     The video manager implementor.
        /// </value>
        [InjectMe]
        public IVideoManagerImp VideoManagerImplementor { set; get; }

        /// <summary>
        ///     Gets and sets the network implementor.
        /// </summary>
        /// <value>
        ///     The network implementor.
        /// </value>
        [InjectMe]
        public INetworkImp NetworkImplementor { set; get; }

        /// <summary>
        ///     Returns the render context object.
        /// </summary>
        /// <value>
        ///     Use the render context (<see cref="RenderContext" />) to fill the render canvas with 3d contents.
        /// </value>
        protected RenderContext RC { get; private set; }

        #endregion

        #region Constructors

        #endregion

        #region Members       

        /// <summary>
        ///     Gets the name of the app.
        /// </summary>
        /// <returns>Name of the app as string.</returns>
        protected string GetAppName()
        {
            Object[] attributes = GetType().GetCustomAttributes(
                typeof (FuseeApplicationAttribute), true);

            if (attributes.Length > 0)
            {
                var fae = (FuseeApplicationAttribute) attributes[0];
                return fae.Name;
            }
            return GetType().Name;
        }

        /// <summary>
        ///     Gets the width of the application's window.
        /// </summary>
        /// <returns>Width of the application's window as int.</returns>
        protected int GetWindowWidth()
        {
            Object[] attributes = GetType().GetCustomAttributes(
                typeof (FuseeApplicationAttribute), true);

            if (attributes.Length > 0)
            {
                var fae = (FuseeApplicationAttribute) attributes[0];
                return fae.Width;
            }

            return -1;
        }

        /// <summary>
        ///     Gets the height of the application's window.
        /// </summary>
        /// <returns>Height of the application's window as int.</returns>
        protected int GetWindowHeight()
        {
            Object[] attributes = GetType().GetCustomAttributes(
                typeof(FuseeApplicationAttribute), true);

            if (attributes.Length > 0)
            {
                var fae = (FuseeApplicationAttribute)attributes[0];
                return fae.Height;
            }

            return -1;
        }

        /// <summary>
        /// Initializes the canvas for the rendering loop.
        /// </summary>
        protected void InitCanvas()
        {
            // InitImplementors();
            CanvasImplementor.Caption = GetAppName();

            var windowWidth = GetWindowWidth();
            var windowHeight = GetWindowHeight();

            if (windowWidth != -1 && windowHeight != -1)
                SetWindowSize(windowWidth, windowHeight);

            RC = new RenderContext(ContextImplementor);
            RC.Viewport(0, 0, Width, Height);            

            Audio.Instance.AudioImp = AudioImplementor;
            Network.Instance.NetworkImp = NetworkImplementor;
            VideoManager.Instance.VideoManagerImp = VideoManagerImplementor;

            CanvasImplementor.Init += delegate { Init(); };
            CanvasImplementor.UnLoad += delegate { DeInit(); };

            CanvasImplementor.Render += delegate
            {
                // pre-rendering
                Network.Instance.OnUpdateFrame();
                Input.Instance.PreRender();
                Time.Instance.DeltaTimeIncrement = CanvasImplementor.DeltaTime;

                // rendering
                RenderAFrame();

                //Resets Viewport and View and Projection Matrix to their default state.
                RC.ResetToDefaultState();

                // post-rendering
                Input.Instance.PostRender();
            };

            CanvasImplementor.Resize += delegate 
            {
                RC.Viewport(0, 0, Width, Height);
                RC.DefaultState.CanvasWidth = Width;
                RC.DefaultState.CanvasHeight = Height;
                Resize(new ResizeEventArgs(Width, Height)); 
            };
        }

        /// <summary>
        ///     Callback method to invoke user code for rendering a frame.
        /// </summary>
        /// <remarks>
        ///     Override this method in inherited classes of RenderCanvas to render 3D contents. Typically, an application will
        ///     use the render context (<see cref="RC" />) to achieve this. Consider the code you implement here as the body of the
        ///     application's rendering loop.
        /// </remarks>
        public virtual void RenderAFrame()
        {
        }

        /// <summary>
        ///     Callback method to invoke user code after initialization of the render canvas.
        /// </summary>
        /// <remarks>
        ///     Override this method in inherited classes of RenderCanvas to apply initialization code. Typically, an application
        ///     will call one-time initialization code on the render context (<see cref="RC" />) to set render states.
        /// </remarks>
        public virtual void Init()
        {
        }

        /// <summary>
        ///     Used to release the resources of all audio and network instances.
        ///     All audio and network resources get reset.
        /// </summary>
        public virtual void DeInit()
        {            
            Audio.Instance.CloseDevice();
            Network.Instance.CloseDevice();

            Audio.Instance.Dispose();
            Time.Instance.Dispose();
            Input.Instance.Dispose();
            Network.Instance.Dispose();
        }

        /// <summary>
        ///     Callback method to invoke user code when the render canvas size changes.
        /// </summary>
        /// <remarks>
        ///     Override this method in inherited classes of RenderCanvas to apply window resize code. Typically, an application
        ///     will change the projection matrix of the render context (<see cref="RC" />) to match the new aspect ratio.
        /// </remarks>
        public virtual void Resize(ResizeEventArgs e)
        {
        }

        /// <summary>
        ///     Set the cursor (the mouse pointer image) to one of the pre-defined types
        /// </summary>
        /// <param name="cursorType">The type of the cursor to set.</param>
        public void SetCursor(CursorType cursorType)
        {
            CanvasImplementor.SetCursor(cursorType);
        }

        /// <summary>
        ///     Opens the given URL in the user's standard web browser. The link MUST start with "http://".
        /// </summary>
        /// <param name="link">The URL to open</param>
        public void OpenLink(string link)
        {
            CanvasImplementor.OpenLink(link);
        }

        /// <summary>
        ///     Runs this instance.
        /// </summary>
        /// <remarks>
        ///     Users should call this method of their RenderCanvas instance to start the application. The RenderCanvas will then
        ///     do all
        ///     necessary initialization, call the Init method and enter the application main loop.
        /// </remarks>
        public void Run()
        {
            InitCanvas();
            CanvasImplementor.Run();
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        /// <remarks>
        ///     This does not run the application, merely initialize the RenderCanvas in preparation.
        /// </remarks>
        public void DoInit()
        {
            InitCanvas();
        }

        /// <summary>
        ///     Presents the contents of the back-buffer on the visible part of this render canvas.
        /// </summary>
        /// <remarks>
        ///     Call this method from your rendering code implementation <see cref="RenderAFrame" /> after rendering geometry on
        ///     the rendering context.
        /// </remarks>
        public void Present()
        {
            CanvasImplementor.Present();
        }

        /*
        /// <summary>
        ///     Sets the data for a video wall.
        /// </summary>
        /// <param name="monitorsHor">Monitor count on horizontal axis.</param>
        /// <param name="monitorsVert">Monitor count on vertical axis.</param>
        /// <param name="activate">Activate or deactivate videoWall mode.</param>
        /// <param name="borderHidden">Activate or deactivate videoWall mode.</param>
        public void VideoWall(int monitorsHor, int monitorsVert, bool activate = true, bool borderHidden = true)
        {
            CanvasImplementor.VideoWall(monitorsHor, monitorsVert, activate, borderHidden);
        }
        */

        /// <summary>
        ///     Sets the size of the output window for desktop development.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="posx">The x position of the window.</param>
        /// <param name="posy">The y position of the window.</param>
        /// <param name="borderHidden">Show the window border or not.</param>
        public void SetWindowSize(int width, int height, int posx = -1, int posy = -1, bool borderHidden = false)
        {
            CanvasImplementor.SetWindowSize(width, height, posx, posy, borderHidden);
        }

        /// <summary>
        ///     Closes the GameWindow with a call to OpenTK.
        /// </summary>
        public void CloseGameWindow()
        {
            CanvasImplementor.CloseGameWindow();
        }

        #endregion

        #region Screen related Fields

        /// <summary>
        ///     Retrieves the width of the canvas.
        /// </summary>
        /// <value>
        ///     The width in pixels.
        /// </value>
        public int Width
        {
            get { return CanvasImplementor.Width; }
            set { CanvasImplementor.Width = value; }
        }

        /// <summary>
        ///     Retrieves the height of the canvas.
        /// </summary>
        /// <value>
        ///     The height in pixels.
        /// </value>
        public int Height
        {
            get { return CanvasImplementor.Height; }
            set { CanvasImplementor.Height = value; }
        }

        /// <summary>
        ///     Gets and sets a value indicating whether VSync is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if VSync is active; otherwise, <c>false</c>.
        /// </value>
        public bool VSync
        {
            set { CanvasImplementor.VerticalSync = value; }
            get { return CanvasImplementor.VerticalSync; }
        }

        /// <summary>
        ///     Gets and sets a value indicating whether this <see cref="RenderCanvas" /> is fullscreen.
        /// </summary>
        /// <value>
        ///     <c>true</c> if fullscreen; otherwise, <c>false</c>.
        /// </value>
        public bool Fullscreen
        {
            get { return CanvasImplementor.Fullscreen; }
            set { CanvasImplementor.Fullscreen = value; }
        }

        #endregion
    }
}