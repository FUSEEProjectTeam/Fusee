using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using System.Collections.Generic;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.Materials.Core
{
    [FuseeApplication(Name = "FUSEE Materials Example", Description = "Showcases different material options.")]
    public class Materials : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRendererDeferred _sceneRenderer;

        private Transform _camTransform;
        private readonly Camera _campComp = new(ProjectionMethod.Perspective, 1, 1000, M.PiOver4);

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;

        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;
        private const float RotationSpeed = 7;
        private bool _keys;

        private SurfaceEffect _gold_brdfFx;
        private SurfaceEffect _paint_brdfFx;
        private SurfaceEffect _rubber_brdfFx;
        private SurfaceEffect _subsurf_brdfFx;
        private SurfaceEffect _brick_brdfFx;

        private SurfaceEffect _glossy_Fx;
        private SurfaceEffect _emissive_Fx;


        // Init is called on startup.
        public override void Init()
        {
            _gui = FuseeGuiHelper.CreateDefaultGui(this, CanvasRenderMode.Screen, "FUSEE SurfaceEffects Example");

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            _campComp.BackgroundColor = new float4(0.8f, 0.9f, 1, 1).LinearColorFromSRgb();

            BuildScene();

            var albedoTex = new Texture(AssetStorage.Get<ImageData>("Bricks_1K_Color.png"), true, TextureFilterMode.LinearMipmapLinear);
            var normalTex = new Texture(AssetStorage.Get<ImageData>("Bricks_1K_Normal.png"), true, TextureFilterMode.LinearMipmapLinear);
            var thicknessTex = new Texture(AssetStorage.Get<ImageData>("monkey-thickness-1.png"), true, TextureFilterMode.LinearMipmapLinear);

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
                new float4(float4.LinearColorFromSRgb(0x39979cFF)),
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

            _brick_brdfFx = MakeEffect.FromBRDF
            (
                albedoColor: float4.One,
                albedoMix: 1.0f,
                albedoTex: albedoTex,
                normalMapStrength: 0.5f,
                normalTex: normalTex,
                texTiles: new float2(3, 3),
                emissionColor: new float4(),
                subsurfaceColor: float3.Zero,
                roughness: 0.3f,
                metallic: 0,
                specular: 0.8f,
                ior: 1.519f,
                subsurface: 0
            );

            _subsurf_brdfFx = MakeEffect.FromBRDF
            (
                albedoColor: new float4(178f / 256, 135f / 256, 100f / 256, 1.0f).LinearColorFromSRgb(),
                emissionColor: new float4(),
                subsurfaceColor: new float3(1, 0, 0).LinearColorFromSRgb(),
                roughness: 0.508f,
                metallic: 0,
                specular: 0.079f,
                ior: 1.4f,
                subsurface: 0.2f,
                albedoTex: null,
                albedoMix: 0,
                texTiles: float2.One,
                normalTex: null,
                normalMapStrength: 0,
                thicknessMap: thicknessTex
            );

            _glossy_Fx = MakeEffect.FromGlossy(new float4(1,0,0,1), 0.4f);
            _emissive_Fx = MakeEffect.FromDiffuseSpecular(float4.One, 0.5f, 255, 0.5f, float4.LinearColorFromSRgb(0x2A84FAFF));

            _scene.Children[3].Components.Insert(1, _emissive_Fx);
            _scene.Children[4].Components.Insert(1, _brick_brdfFx);
            _scene.Children[5].Components.Insert(1, _rubber_brdfFx);
            _scene.Children[6].Components.Insert(1, _gold_brdfFx);
            _scene.Children[7].Components.Insert(1, _subsurf_brdfFx);
            _scene.Children[8].Components.Insert(1, _paint_brdfFx);
            _scene.Children[9].Components.Insert(1, _glossy_Fx);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererDeferred(_scene);
            _guiRenderer = new SceneRendererForward(_gui);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

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
                    _angleVelHorz = RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;
                    _angleVelVert = RotationSpeed * Keyboard.UpDownAxis * DeltaTime;
                }
            }

            _angleHorz -= _angleVelHorz;
            _angleVert -= _angleVelVert;
            _angleVelHorz = 0;
            _angleVelVert = 0;

            _camTransform.FpsView(_angleHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTime * 5);
            
            _sceneRenderer.Render(RC);

            //Constantly check for interactive objects.
            //_campComp.ProjectionMethod = ProjectionMethod.Orthographic;
            //if (!Mouse.Desc.Contains("Android"))
            //    _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);
            //if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            //{
            //    _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            //}

            //_guiRenderer.Render(RC);
            //_campComp.ProjectionMethod = ProjectionMethod.Perspective;
            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    
        private void BuildScene()
        {
            // Load the rocket model
            _scene = AssetStorage.Get<SceneContainer>("monkey.fus");

            _camTransform = new Transform()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 5f, -15),
                Scale = float3.One
            };

            _scene.Children.Add(
                new SceneNode()
                {
                    Name = "Cam",
                    Components = new List<SceneComponent>()
                    {
                        _camTransform,
                        _campComp
                    }
                }
            );

            var lightNode = new SceneNode()
            {
                Name = "Light",
                Components = new List<SceneComponent>{
                    new Transform()
                    {
                        Rotation = new float3(new float3(M.DegreesToRadians(45), 0, 0)),
                        Translation = new float3(0, 15, -15),
                        Scale = float3.One
                    },
                    new Light()
                    {
                        Type = LightType.Spot,
                        Color = new float4(0.6f, 0.8f, 1, 1),
                        MaxDistance = 50,
                        Active = true,
                        OuterConeAngle = 30,
                        InnerConeAngle = 15,
                        IsCastingShadows = true,
                        Bias = 0.00000001f
                    }
                }
            };
            _scene.Children.Insert(0, lightNode);
            var monkeyMesh = _scene.Children[1].GetComponent<Mesh>();
            _scene.Children.RemoveAt(1);

            monkeyMesh.Tangents = monkeyMesh.CalculateTangents();
            monkeyMesh.BiTangents = monkeyMesh.CalculateBiTangents();

            var checkerboardTex = new Texture(AssetStorage.Get<ImageData>("checkerboard.png"), true, TextureFilterMode.LinearMipmapLinear);

            _scene.Children.Add(
            new SceneNode()
            {
                Name = $"Plane",
                Components = new System.Collections.Generic.List<SceneComponent>{
                    new Transform()
                    {
                        Rotation = new float3(M.DegreesToRadians(90), 0, 0),
                        Translation = new float3(0, -1, 0),
                        Scale = new float3(50, 50,0.1f)
                    },
                    MakeEffect.FromDiffuse(float4.One, 0, checkerboardTex, 1f, new float2(2,2)),
                    new Plane()
                }
            });

            int count = 0;
            for (int z = 0; z < 2; z++)
            {
                for (int x = -1; x < 2; x++)
                {
                    count++;
                    _scene.Children.Add(
                    new SceneNode()
                    {
                        Name = $"Monkey_{count}",
                        Components = new System.Collections.Generic.List<SceneComponent>{
                            new Transform()
                            {
                                Rotation = float3.Zero,
                                Translation = new float3(x * 4, 0 ,(-1 + z) * 4),
                                Scale = float3.One
                            },
                            monkeyMesh
                        }
                    });

                    if (x == 1 && z == 1)
                    {
                        _scene.Children.Add(
                        new SceneNode()
                        {
                            Name = $"Monkey_{count}",
                            Components = new System.Collections.Generic.List<SceneComponent>{
                                new Transform()
                                {
                                    Rotation = float3.Zero,
                                    Translation = new float3(0, 0 ,z * 4),
                                    Scale = float3.One
                                },
                                monkeyMesh
                            }
                        });
                    }
                }
            }
        }
    
    }
}