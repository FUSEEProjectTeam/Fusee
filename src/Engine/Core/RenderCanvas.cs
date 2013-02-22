using Fusee.Engine;
using Fusee.Math;

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
        private RenderContext _rc;
        internal IRenderCanvasImp _canvasImp;


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



        /// <summary>
        /// The RenderCanvas constructor. Depending on the implementation this constructor instantiates a 3D viewing window or connects a 3D 
        /// render context to an existing part of the application window.
        /// </summary>

        private Audio _audio;
        protected Audio Aud
        {
            get { return _audio; }
        }

        public RenderCanvas()
        {
            _canvasImp = ImpFactory.CreateIRenderCanvasImp();
            _rc = new RenderContext(ImpFactory.CreateIRenderContextImp(_canvasImp));
            Input.Instance.InputImp = ImpFactory.CreateIInputImp(_canvasImp);
            _canvasImp.Init += delegate(object sender, InitEventArgs args)
                                    {
                                        Init();
                                    };

            _canvasImp.UnLoad += delegate(object sender, InitEventArgs args)
                                    {
                                        UnLoad();
                                    };

            _canvasImp.Render += delegate(object sender, RenderEventArgs args)
                                     {
                                         Input.Instance.OnUpdateFrame(_canvasImp.DeltaTime);
                                         Time.Instance.DeltaTimeIncrement = _canvasImp.DeltaTime;
                                         RenderAFrame();
                                     };

            _canvasImp.Resize += delegate(object sender, ResizeEventArgs args)
                                     {
                                         Resize();
                                     };

            _audio = new Audio(ImpFactory.CreateIAudioImp());
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

        
        public virtual void UnLoad()
        {
            _audio.CloseDevice();
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
        /// Runs this instance.
        /// </summary>
        /// <remarks>
        /// Users should call this method of their RenderCanvas instance to start the application. The RenderCanvas will then do all
        /// necessary initialization, call the Init method and enter the application main loop.
        /// </remarks>
        public void Run()
        {
            _canvasImp.Run();
        }

        /// <summary>
        /// Retrieves the width of the canvas.
        /// </summary>
        /// <value>
        /// The width in pixels.
        /// </value>
        public int Width { get { return _canvasImp.Width; } }

        /// <summary>
        /// Retrieves the height of the canvas.
        /// </summary>
        /// <value>
        /// The height in pixels.
        /// </value>
        public int Height { get { return _canvasImp.Height; } }


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
     }
}
