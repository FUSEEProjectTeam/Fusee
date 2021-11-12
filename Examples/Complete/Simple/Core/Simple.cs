using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.Simple.Core
{
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class Simple : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneRendererForward _sceneRenderer;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private readonly float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;

        private bool _keys;
        private bool _loaded;

        private async void Load()
        {
            _gui = FuseeGuiHelper.CreateDefaultGui(this, CanvasRenderMode.Screen, "FUSEE Simple Example");

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            Console.WriteLine("Loading scene");


            // Load the rocket model
            _rocketScene = await AssetStorage.GetAsync<SceneContainer>("RocketFus.fus");
            //var tex = await AssetStorage.GetAsync<ImageData>("background.png");
            //var dif = await AssetStorage.GetAsync<ImageData>("background_ddn.png");
            //var tex2 = await AssetStorage.GetAsync<ImageData>("sponza_curtain_green_diff.png");

            //_rocketScene = new SceneContainer
            //{
            //    Children = new List<SceneNode>
            //    {
            //        new SceneNode
            //        {
            //            Components = new List<SceneComponent>
            //            {
            //                new Transform(),
            //                MakeEffect.FromBRDFTexture(
            //                    float4.One, float4.One, 1.0f, 1.0f, 1.0f, 0.25f, 0.25f, new Texture(tex), new Texture(dif), 0.5f,  float2.One),
            //                new Cube()
            //            }
            //        },
            //        new SceneNode
            //        {
            //            Components = new List<SceneComponent>
            //            {
            //                new Transform
            //                {
            //                    Translation = float3.UnitX * 2
            //                },
            //                MakeEffect.FromDiffuseAlbedoTexture(
            //                    float4.One, new Texture(tex2), float2.One, 0.25f),
            //                new Cube()
            //            }
            //        }
            //    }
            //};

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_rocketScene);
            _guiRenderer = new SceneRendererForward(_gui);

            _loaded = true;
        }

        // Init is called on startup.
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            Load();
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            if (!_loaded) return;

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            //Console.WriteLine(FramesPerSecondAverage);

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
            else if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0))
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

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, +2, -10, 0, +2, 0, 0, 1, 0);

            var view = mtxCam * mtxRot;
            var perspective = float4x4.CreatePerspectiveFieldOfView(_fovy, (float)Width / Height, ZNear, ZFar);
            var orthographic = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);

            // Render the scene loaded in Init()
            RC.View = view;
            RC.Projection = perspective;
            _sceneRenderer.Render(RC);

            //Constantly check for interactive objects.

            RC.Projection = orthographic;
            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);
            if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}