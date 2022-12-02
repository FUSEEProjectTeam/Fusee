using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Examples.PointCloudPotree2.Core;
using Fusee.ImGuiImp.Desktop.Templates;
using Fusee.PointCloud.Common;
using System;

namespace Fusee.Examples.PointCloudPotree2.Gui
{
    internal class PointCloudRenderingControl : FuseeSceneToTexture
    {
        public bool ClosingRequested
        {
            get { return _pointRenderingCore.ClosingRequested; }
            set { _pointRenderingCore.ClosingRequested = value; }
        }

        public bool RequestedNewFile = false;
        public RenderMode PointRenderMode = RenderMode.StaticMesh;

        private readonly PointCloudPotree2Core _pointRenderingCore;

        public PointCloudRenderingControl(RenderContext rc) : base(rc)
        {
            _pointRenderingCore = new PointCloudPotree2Core(rc)
            {
                RenderToTexture = true
            };
        }

        public void OnLoadNewFile(object sender, EventArgs e)
        {
            if (!RequestedNewFile) return;

            _pointRenderingCore.OnLoadNewFile(sender, e);

            RequestedNewFile = false;
        }

        public override void Init()
        {
            _pointRenderingCore.Init();
        }

        // RenderAFrame is called once a frame
        protected override ITextureHandle RenderAFrame()
        {
            _pointRenderingCore.RenderAFrame();
            return _pointRenderingCore.RenderTexture?.TextureHandle;
        }

        public override void Update(bool allowInput)
        {
            if (!allowInput) return;

            if (ClosingRequested)
                return;

            _pointRenderingCore.Update(allowInput);
        }

        // Is called when the window was resized
        protected override void Resize(int width, int height)
        {
            _pointRenderingCore.Resize(width, height);
        }

        public void ResetCamera()
        {
            _pointRenderingCore.ResetCamera();
        }
    }
}