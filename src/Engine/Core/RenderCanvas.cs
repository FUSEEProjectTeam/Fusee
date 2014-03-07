using System;

namespace Fusee.Engine
{
    /// <summary>
    /// A render canvas object references the physical output screen space real estate (e.g. the rendering window).
    /// A typical Game application will inherit from this class and overrite methods to implement your 
    /// user code to be performed on events like initialization, resize,
    /// and display refresh.
    /// In the future, it will be likely that this class' functionality will be divided at two different places with
    /// one containing the more view oriented aspects and the other containing the more application oriented aspects.
    /// </summary>
    public class RenderCanvas
    {
        #region Private Fields

        private RenderContext _rc;
        private IRenderContextImp _renderContextImp;

        private IInputImp _inputImp;
        private IAudioImp _audioImp;
        private INetworkImp _networkImp;

        internal IRenderCanvasImp _canvasImp;
        private IInputDriverImp _inputDriverImp;

        #endregion

        #region Implementor Fields

        /// <summary>
        /// Gets or sets the canvas implementor.
        /// </summary>
        /// <value>
        /// The canvas implementor.
        /// </value>
        [InjectMe]
        public IRenderCanvasImp CanvasImplementor
        {
            set { _canvasImp = value; }
            get { return _canvasImp; }
        }

        /// <summary>
        /// Gets or sets the RenderContext implementor.
        /// </summary>
        /// <value>
        /// The context implementor.
        /// </value>
        [InjectMe]
        public IRenderContextImp ContextImplementor
        {
            set { _renderContextImp = value; }
            get { return _renderContextImp; }
        }

        /// <summary>
        /// Gets or sets the input implementor.
        /// </summary>
        /// <value>
        /// The input implementor.
        /// </value>
        [InjectMe]
        public IInputImp InputImplementor
        {
            set { _inputImp = value; }
            get { return _inputImp; }
        }

        /// <summary>
        /// Gets or sets the audio implementor.
        /// </summary>
        /// <value>
        /// The audio implementor.
        /// </value>
        [InjectMe]
        public IAudioImp AudioImplementor
        {
            set { _audioImp = value; }
            get { return _audioImp; }
        }

        /// <summary>
        /// Gets or sets the inputdevice implementor.
        /// </summary>
        /// <value>
        /// The inputdevice implementor.
        /// </value>
        [InjectMe]
        public IInputDriverImp InputDriverImplementor
        {
            set { _inputDriverImp = value; }
            get { return _inputDriverImp; }
        }

        /// <summary>
        /// Returns the render context object.
        /// </summary>
        /// <value>
        /// Use the render context (<see cref="RenderContext"/>) to fill the render canvas with 3d contents.
        /// </value>
        protected RenderContext RC
        {
            get { return _rc; }
        }

        #endregion

        #region Constructors

        #endregion

        #region Members

        /// <summary>
        /// The RenderCanvas constructor. Depending on the implementation this constructor instantiates a 3D viewing window or connects a 3D 
        /// render context to an existing part of the application window.
        /// </summary>
        public void InitImplementors()
        {
            if (_canvasImp == null)
                _canvasImp = ImpFactory.CreateIRenderCanvasImp();

            if (_renderContextImp == null)
                _renderContextImp = ImpFactory.CreateIRenderContextImp(_canvasImp);

            if (_inputImp == null)
                _inputImp = ImpFactory.CreateIInputImp(_canvasImp);

            if (_audioImp == null)
                _audioImp = ImpFactory.CreateIAudioImp();

            if (_inputDriverImp == null)
                _inputDriverImp = ImpFactory.CreateIInputDriverImp();

            if (_networkImp == null)
                _networkImp = ImpFactory.CreateINetworkImp();
        }

        /// <summary>
        /// Gets the name of the app.
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
        /// Inits the canvas for the rendering loop.
        /// </summary>
        protected void InitCanvas()
        {
            InitImplementors();

            _canvasImp.Caption = GetAppName();

            _rc = new RenderContext(_renderContextImp);
            _rc.Viewport(0, 0, Width, Height);

            Input.Instance.InputImp = _inputImp;
            Audio.Instance.AudioImp = _audioImp;
            Input.Instance.InputDriverImp = _inputDriverImp;
            Network.Instance.NetworkImp = _networkImp;

            _canvasImp.Init += delegate { Init(); };
            _canvasImp.UnLoad += delegate { UnLoad(); };

            _canvasImp.Render += delegate
            {
                // pre-rendering
                Network.Instance.OnUpdateFrame();
                Input.Instance.OnUpdateFrame();
                Time.Instance.DeltaTimeIncrement = _canvasImp.DeltaTime;

                // rendering
                RenderAFrame();

                // post-rendering
                Input.Instance.OnLateUpdate();
            };

            _canvasImp.Resize += delegate { Resize(); };
        }

        /// <summary>
        /// Callback method to invoke user code for rendering a frame.
        /// </summary>
        /// <remarks>
        /// Override this method in inherited classes of RenderCanvas to render 3D contents. Typically, an application will
        /// use the render context (<see cref="RC"/>) to achieve this. Consider the code you implement here as the body of the
        /// application's rendering loop.
        /// </remarks>
        public virtual void RenderAFrame()
        {
        }

        /// <summary>
        /// Callback method to invoke user code after initialization of the render canvas.
        /// </summary>
        /// <remarks>
        /// Override this method in inherited classes of RenderCanvas to apply initialization code. Typically, an application
        /// will call one-time initialization code on the render context (<see cref="RC"/>) to set render states.
        /// </remarks>
        public virtual void Init()
        {
        }


        /// <summary>
        /// Used to release the ressources of all audio and network instances.
        /// All audio and network ressources get reset.
        /// </summary>
        public virtual void UnLoad()
        {
            Audio.Instance.CloseDevice();
            Network.Instance.CloseDevice();
        }

        /// <summary>
        /// Callback method to invoke user code when the render canvas size changes.
        /// </summary>
        /// <remarks>
        /// Override this method in inherited classes of RenderCanvas to apply window resize code. Typically, an application
        /// will change the projection matrix of the render context (<see cref="RC"/>) to match the new aspect ratio.
        /// </remarks>
        public virtual void Resize()
        {
        }

        /// <summary>
        /// Set the cursor (the mouse pointer image) to one of the pre-defined types
        /// </summary>
        /// <param name="cursorType">The type of the cursor to set.</param>
        public void SetCursor(CursorType cursorType)
        {
            _canvasImp.SetCursor(cursorType);
        }

        /// <summary>
        /// Opens the given URL in the user's standard web browser. The link MUST start with "http://".
        /// </summary>
        /// <param name="link">The URL to open</param>
        public void OpenLink(string link)
        {
            _canvasImp.OpenLink(link);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <remarks>
        /// Users should call this method of their RenderCanvas instance to start the application. The RenderCanvas will then do all
        /// necessary initialization, call the Init method and enter the application main loop.
        /// </remarks>
        public void Run()
        {
            InitCanvas();
            _canvasImp.Run();
        }

        /// <summary>
        /// Presents the contents of the backbuffer on the visible part of this render canvas.
        /// </summary>
        /// <remarks>
        /// Call this method from your rendering code implementation <see cref="RenderAFrame"/> after rendering geometry on 
        /// the rendering context.
        /// </remarks>
        public void Present()
        {
            _canvasImp.Present();
        }

        #endregion

        #region Screen related Fields

        /// <summary>
        /// Retrieves the width of the canvas.
        /// </summary>
        /// <value>
        /// The width in pixels.
        /// </value>
        public int Width
        {
            get { return _canvasImp.Width; }
            set { _canvasImp.Width = value; }
        }

        /// <summary>
        /// Retrieves the height of the canvas.
        /// </summary>
        /// <value>
        /// The height in pixels.
        /// </value>
        public int Height
        {
            get { return _canvasImp.Height; }
            set { _canvasImp.Height = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether VSync is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if VSync is active; otherwise, <c>false</c>.
        /// </value>
        public bool VSync
        {
            set { _canvasImp.VerticalSync = value; }
            get { return _canvasImp.VerticalSync; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Blending is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if Blending is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Blending
        {
            get { return _canvasImp.EnableBlending; }
            set { _canvasImp.EnableBlending = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RenderCanvas"/> is fullscreen.
        /// </summary>
        /// <value>
        ///   <c>true</c> if fullscreen; otherwise, <c>false</c>.
        /// </value>
        public bool Fullscreen
        {
            get { return _canvasImp.Fullscreen; }
            set { _canvasImp.Fullscreen = value; }
        }

        #endregion
    }
}
