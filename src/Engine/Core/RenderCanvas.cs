using Fusee.Engine;
using Fusee.Math;

namespace Fusee.Engine
{
    public class RenderCanvas
    {
        private RenderContext _rc;
        internal IRenderCanvasImp _canvasImp;
        protected RenderContext RC
        {
            get { return _rc; }
        }

        private Input _in;
        protected Input In
        {
            get { return _in; }
        }

        public RenderCanvas()
        {
            _canvasImp = ImpFactory.CreateIRenderCanvasImp();
            _rc = new RenderContext(ImpFactory.CreateIRenderContextImp(_canvasImp));
            _in = new Input(ImpFactory.CreateIInputImp(_canvasImp));
            _canvasImp.Init += delegate(object sender, InitEventArgs args)
                                   {
                                       Init();
                                   };

            _canvasImp.Render += delegate(object sender, RenderEventArgs args)
                                     {
                                         _in.OnUpdateFrame(_canvasImp.DeltaTime);
                                         RenderAFrame();
                                     };

            _canvasImp.Resize += delegate(object sender, ResizeEventArgs args)
                                     {
                                         Resize();
                                     };

        }

        public virtual void RenderAFrame()
        {
        }

        public virtual void Init()
        {
        }

        public virtual void Resize()
        {
        }

        public void Run()
        {
            _canvasImp.Run();
        }

        public int Width { get { return _canvasImp.Width; } }

        public int Height { get { return _canvasImp.Height; } }

        public double DeltaTime
        {
            get
            {
                return _canvasImp.DeltaTime;
            }
        }

        public void Present()
        {
            _canvasImp.Present();
        }
     }
}
