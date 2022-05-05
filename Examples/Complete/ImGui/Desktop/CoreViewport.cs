using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Examples.FuseeImGui.Desktop
{
    internal class CoreViewport : ImGuiDesktop.Templates.FuseeViewportToMSAATexture
    {
        private SceneContainer _rocketScene;
        private SceneRendererForward _renderer;
        private readonly RenderContext _rc;

        public int Width;
        public int Height;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private readonly float _fovy = M.PiOver4;

        public CoreViewport(RenderContext ctx, int wndWidth, int wndHeight) : base(wndWidth, wndHeight)
        {
            _rc = ctx;
        }

        public override void Init()
        {
            _rocketScene = AssetStorage.Get<SceneContainer>("RocketFus.fus");
            _renderer = new SceneRendererForward(_rocketScene);
        }

        protected override void Update()
        {
            if (Input.Mouse.LeftButton)
            {
                _angleVelHorz = RotationSpeed * 2000f/** Input.Mouse.XVel*/ * Time.DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * 2000f/** Input.Mouse.YVel*/ * Time.DeltaTimeUpdate * 0.0005f;
            }

            else
            {
                var curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTimeUpdate);
                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;
        }

        protected override void RenderAFrame()
        {


            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 2, -10, 0, 2, 0, 0, 1, 0);

            var view = mtxCam * mtxRot;
            var perspective = float4x4.CreatePerspectiveFieldOfView(_fovy, (float)Width / Height, ZNear, ZFar);
            var orthographic = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);

            // Render the scene loaded in Init()
            _rc.View = view;
            _rc.Projection = perspective;
            _renderer.Render(_rc);
        }

        protected override void Resize(int width, int height)
        {
            Width = width;
            Height = height;
        }

    }
}
