using Fusee.Engine;
using Fusee.Math;

namespace Fusee.Engine
{
    public class RenderCanvas
    {
        internal IRenderCanvasImp _canvasImp;

        private RenderContext _rc;
        protected RenderContext RC
        {
            get { return _rc; }
        }

        private Input _in;
        protected Input In
        {
            get { return _in; }
        }

        private Audio _audio;
        protected Audio Aud
        {
            get { return _audio; }
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

            _canvasImp.UnLoad += delegate(object sender, InitEventArgs args)
                                    {
                                        UnLoad();
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

            _audio = new Audio(ImpFactory.CreateIAudioImp());
        }

        public virtual void Init()
        {
        }

        public virtual void UnLoad()
        {
            _audio.CloseDevice();
        }

        public virtual void RenderAFrame()
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
