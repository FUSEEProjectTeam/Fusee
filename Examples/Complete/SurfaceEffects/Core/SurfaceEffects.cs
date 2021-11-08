using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;


namespace Fusee.Examples.SurfaceEffects.Core
{
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class SurfaceEffects : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneRendererDeferred _sceneRenderer;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private readonly float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;

        private bool _keys;

        private SurfaceEffect _gold_brdfFx;
        private SurfaceEffect _paint_brdfFx;
        private SurfaceEffect _rubber_brdfFx;
        private SurfaceEffect _subsurf_brdfFx;

        private SurfaceEffect _testFx;
        private SurfaceEffect _testEdlFx;

        // Init is called on startup.
        public override void Init()
        {
            _gui = FuseeGuiHelper.CreateDefaultGui(this, CanvasRenderMode.Screen, "FUSEE SurfaceEffects Example");

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.1f, 0.1f, 0.1f, 1).LinearColorFromSRgb();

            // Load the rocket model
            _rocketScene = AssetStorage.Get<SceneContainer>("monkeys.fus");

            var albedoTex = new Texture(AssetStorage.Get<ImageData>("Bricks_1K_Color.png"), true, TextureFilterMode.LinearMipmapLinear);
            var normalTex = new Texture(AssetStorage.Get<ImageData>("Bricks_1K_Normal.png"), true, TextureFilterMode.LinearMipmapLinear);

            var surfInput = new TextureInputSpecular();
            _testFx = new SurfaceEffect(surfInput,
                Engine.Core.ShaderShards.Fragment.FragShards.SurfOutBody_Textures(surfInput.ShadingModel, surfInput.TextureSetup),
                Engine.Core.ShaderShards.Vertex.VertShards.SufOutBody_PosNorm);

            _testFx.SurfaceInput.Albedo = new float4(1.0f, 0, 0, 1.0f);

            ((TextureInputSpecular)_testFx.SurfaceInput).AlbedoTex = albedoTex;
            ((TextureInputSpecular)_testFx.SurfaceInput).NormalTex = normalTex;
            ((TextureInputSpecular)_testFx.SurfaceInput).AlbedoMix = 1.0f;
            ((TextureInputSpecular)_testFx.SurfaceInput).Shininess = 5f;
            ((TextureInputSpecular)_testFx.SurfaceInput).SpecularStrength = 1f;
            ((TextureInputSpecular)_testFx.SurfaceInput).TexTiles = new float2(3, 3);

            _gold_brdfFx = MakeEffect.FromBRDF
            (
                albedoColor: new float4(1.0f, 227f / 256f, 157f / 256, 1.0f).LinearColorFromSRgb(),
                emissionColor: new float4(0, 0, 0, 0),
                subsurfaceColor: float3.Zero,
                roughness: 0.2f,
                metallic: 1,
                specular: 0,
                ior: 0.47f,
                subsurface: 0
            );

            _paint_brdfFx = MakeEffect.FromBRDF
            (
                //ColorUint.Greenery, 
                new float4(float4.LinearColorFromSRgb(0x708828FF)),
                emissionColor: new float4(),
                subsurfaceColor: float3.Zero,
                roughness: 0.05f,
                metallic: 0,
                specular: 1f,
                ior: 1.46f,
                subsurface: 0
            );

            _rubber_brdfFx = MakeEffect.FromBRDF
            (
                albedoColor: new float4(214f / 256f, 84f / 256f, 68f / 256f, 1.0f).LinearColorFromSRgb(),
                emissionColor: new float4(),
                subsurfaceColor: float3.Zero,
                roughness: 1.0f,
                metallic: 0,
                specular: 0.1f,
                ior: 1.519f,
                subsurface: 0
            );

            _subsurf_brdfFx = MakeEffect.FromBRDF
            (
                albedoColor: new float4(float4.LinearColorFromSRgb(0xDEB887FF)),
                emissionColor: new float4(),
                subsurfaceColor: new float3(1, 0, 0),
                roughness: 0.508f,
                metallic: 0,
                specular: 0.079f,
                ior: 1.4f,
                subsurface: 0.1f
            );

            _rocketScene.Children[0].Components[1] = _testEdlFx;//_subsurf_brdfFx;
            _rocketScene.Children[1].Components[1] = _rubber_brdfFx;
            _rocketScene.Children[2].Components[1] = _paint_brdfFx;
            _rocketScene.Children[3].Components[1] = _gold_brdfFx;

            var monkeyOne = (Mesh)_rocketScene.Children[0].Components[2];
            monkeyOne.Tangents = monkeyOne.CalculateTangents();
            monkeyOne.BiTangents = monkeyOne.CalculateBiTangents();

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererDeferred(_rocketScene);
            _guiRenderer = new SceneRendererForward(_gui);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            if (Keyboard.IsKeyDown(KeyCodes.W))
            {
                if (_paint_brdfFx.SurfaceInput.Albedo.g <= 1.0f)
                    _paint_brdfFx.SurfaceInput.Albedo += new float4(0, 0.2f, 0, 0);
            }
            if (Keyboard.IsKeyDown(KeyCodes.S))
            {
                if (_paint_brdfFx.SurfaceInput.Albedo.g >= 0.0f)
                    _paint_brdfFx.SurfaceInput.Albedo -= new float4(0, 0.2f, 0, 0);
            }

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

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 0, -7, 0, 0, 0, 0, 1, 0);

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
            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}