using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.Integrations.Core
{
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class Main : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneRendererForward _sceneRenderer;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;

        private bool _keys;

        public event EventHandler<FusEvent> FusToWpfEvents;
        private Transform rocketTransform;

        private readonly Camera _mainCam = new(ProjectionMethod.Perspective, 0.1f, 1000, M.PiOver4)
        {
            BackgroundColor = float4.One
        };
        private Transform _camTransform;

        // Init is called on startup.
        public override void Init()
        {
            _gui = FuseeGuiHelper.CreateDefaultGui(this, CanvasRenderMode.Screen, "FUSEE Integrations Example");

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Load the rocket model
            _rocketScene = AssetStorage.Get<SceneContainer>("RocketFus.fus");
            _camTransform = new Transform()
            {
                Translation = new float3(0, 2, -10),
                Scale = float3.One
            };
            var camNode = new SceneNode()
            {
                Components = new List<SceneComponent>()
                {
                    _camTransform,
                    _mainCam
                }
            };
            _rocketScene.Children.Add(camNode);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_rocketScene);
            _guiRenderer = new SceneRendererForward(_gui);

            rocketTransform = _rocketScene.Children[0].GetTransform();

            FusToWpfEvents?.Invoke(this, new StartupInfoEvent(VSync));
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * DeltaTime;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            _camTransform.RotationMatrix = mtxRot;

            _sceneRenderer.Render(RC);
            FusToWpfEvents?.Invoke(this, new FpsEvent(FramesPerSecondAverage));

            _guiRenderer.Render(RC);

            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);
            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            Present();
        }

        public void ChangeRocketX(float x)
        {
            rocketTransform.Translation = new float3(x, rocketTransform.Translation.y, rocketTransform.Translation.z);
        }
        public void ChangeRocketY(float y)
        {
            rocketTransform.Translation = new float3(rocketTransform.Translation.x, y, rocketTransform.Translation.z);
        }
        public void ChangeRocketZ(float z)
        {
            rocketTransform.Translation = new float3(rocketTransform.Translation.x, rocketTransform.Translation.y, z);
        }
    }
}