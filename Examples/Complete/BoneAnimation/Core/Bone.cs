using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using System;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.BoneAnimation.Core
{
    [FuseeApplication(Name = "FUSEE Bone Animation Example", Description = "Quick bone animation example")]
    public class Bone : RenderCanvas
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
        private SceneContainer _scene;
        private bool _keys;

        private ComputeShader _computeShader;
        private ShaderEffect _renderEffect;
        private async Task Load()
        {
            Console.WriteLine("Loading scene ...");
            _scene = AssetStorage.Get<SceneContainer>("Snake zum testen.fus");
            
            _gui = await FuseeGuiHelper.CreateDefaultGuiAsync(this, CanvasRenderMode.Screen, "FUSEE Simple Example");

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Load the rocket model

            // Wrap a SceneRenderer around the model.
            _computeShader = new ComputeShader(
                        shaderCode: AssetStorage.Get<string>("MandelbrotFractal.comp"),
                        effectParameters: new IFxParamDeclaration[]
                        {
                new FxParamDeclaration<WritableTexture> { Name = "destTex", Value = RWTexture},
                new FxParamDeclaration<StorageBuffer<float4>>{ Name = "colorStorageBuffer", Value = _colors},
                new FxParamDeclaration<StorageBuffer<double>>{ Name = "rectStorageBuffer", Value = _rect},
                new FxParamDeclaration<double>{ Name = "k", Value = _k},
                        });

            _renderEffect = new ShaderEffect(
            new FxPassDeclaration
            {
                VS = AssetStorage.Get<string>("RenderTexToScreen.vert"),
                PS = AssetStorage.Get<string>("RenderTexToScreen.frag"),
                StateSet = new RenderStateSet
                {
                    AlphaBlendEnable = false,
                    ZEnable = true,
                }
            },
        }

        public override async Task InitAsync()
        {
            await Load();
            await base.InitAsync();
        }

        // Init is called on startup.
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);
        }

        public override void Update()
        {
            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTimeUpdate * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTimeUpdate * 0.0005f;
            }
            else if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * DeltaTimeUpdate * 0.0005f;
                _angleVelVert = -RotationSpeed * touchVel.y * DeltaTimeUpdate * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * DeltaTimeUpdate;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * DeltaTimeUpdate;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTimeUpdate);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;
        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, -2, -4, 0, 2, 0, 0, 1, 0);

            var view = mtxCam * mtxRot;
            var perspective = float4x4.CreatePerspectiveFieldOfView(_fovy, (float)Width / Height, ZNear, ZFar);
            var orthographic = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);

            // Render the scene loaded in Init()
            RC.View = view;
            RC.Projection = perspective;
            _sceneRenderer.Render(RC);

            //Constantly check for interactive objects.
            RC.View = float4x4.LookAt(0, 0, 1, 0, 0, 0, 0, 1, 0);
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