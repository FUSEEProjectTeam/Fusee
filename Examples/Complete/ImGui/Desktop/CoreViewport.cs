using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Examples.FuseeImGui.Desktop
{
    internal class CoreViewport : ImGuiDesktop.Templates.FuseeControlToTexture
    {
        private SceneContainer _rocketScene;
        private SceneRendererForward _renderer;
        private readonly RenderContext _rc;

        private Transform _camPivotTransform;


        public int Width;
        public int Height;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private readonly float _fovy = M.PiOver4;

        public CoreViewport(RenderContext ctx) : base()
        {
            _rc = ctx;
        }

        public override void Init()
        {
            _rocketScene = AssetStorage.Get<SceneContainer>("RocketFus.fus");
            _camPivotTransform = new Transform();
            var camNode = new SceneNode()
            {
                Name = "CamPivoteNode",
                Children = new ChildList()
                {
                    new SceneNode()
                    {
                        Name = "MainCam",
                        Components = new System.Collections.Generic.List<SceneComponent>()
                        {
                            new Transform() { Translation = new float3(0, 2, -10) },
                            new Camera(ProjectionMethod.Perspective, ZNear, ZFar, _fovy) { BackgroundColor = new float4(0,0,0,0) }
                        }
                    }
                },
                Components = new System.Collections.Generic.List<SceneComponent>()
                {
                    _camPivotTransform
                }
            };
            _rocketScene.Children.Add(camNode);


            _renderer = new SceneRendererForward(_rocketScene);
        }

        protected override void Update()
        {
            if (Input.Mouse.LeftButton)
            {
                _angleVelHorz = RotationSpeed *  Input.Mouse.XVel * Time.DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed *  Input.Mouse.YVel * Time.DeltaTimeUpdate * 0.0005f;
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
            _camPivotTransform.RotationQuaternion = QuaternionF.FromEuler(_angleVert, _angleHorz, 0);

            _renderer.Render(_rc);
        }

        protected override void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            // FIXME (later) (mr)
            // This is necessary as the camera uses the defaultState, for reseting original size, too
            // see: SceneRendererForward:297
            if (_rc.DefaultState != null)
            {
                _rc.DefaultState.CanvasWidth = Width;
                _rc.DefaultState.CanvasHeight = Height;
            }
        }

    }
}
