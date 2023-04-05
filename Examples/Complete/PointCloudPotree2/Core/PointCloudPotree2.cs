using Fusee.Engine.Common;
using Fusee.Engine.Core;

namespace Fusee.Examples.PointCloudPotree2.Core
{
    [FuseeApplication(Name = "FUSEE Point Cloud Viewer")]
    public class PointCloudPotree2 : RenderCanvas
    {
        public bool UseWPF { get; set; }
        public bool ReadyToLoadNewFile { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool IsAlive { get; private set; }

        public bool ClosingRequested
        {
            get => _pointRenderingCore.ClosingRequested;
            set => _pointRenderingCore.ClosingRequested = value;
        }

        private PointCloudPotree2Core _pointRenderingCore;

        public override void Init()
        {
            VSync = false;
            _pointRenderingCore = new PointCloudPotree2Core(RC)
            {
                RenderToTexture = false
            };

            _pointRenderingCore.Init();
            IsInitialized = true;
            IsAlive = true;
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            ReadyToLoadNewFile = false;

            if (ClosingRequested)
            {
                ReadyToLoadNewFile = true;
                return;
            }

            _pointRenderingCore.RenderAFrame();
            Present();

            ReadyToLoadNewFile = true;
        }

        public override void Update()
        {
            if (ClosingRequested)
            {
                ReadyToLoadNewFile = true;
                return;
            }

            _pointRenderingCore?.Update(true);
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
            _pointRenderingCore?.Resize(e.Width, e.Height);
        }

        public override void DeInit()
        {
            base.DeInit();
            IsAlive = false;
        }

        public void ResetCamera()
        {
            _pointRenderingCore?.ResetCamera();
        }
    }
}