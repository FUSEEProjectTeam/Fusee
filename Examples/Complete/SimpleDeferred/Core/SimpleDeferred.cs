using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;
using System.Linq;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.SimpleDeferred.Core
{
    [FuseeApplication(Name = "FUSEE Deferred Rendering Example", Description = "")]
    public class SimpleDeferred : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;

        private SceneContainer _rocketScene;
        private SceneRendererDeferred _sceneRenderer;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.Screen;

        private bool _keys;

        private Transform _sunTransform;

        private float4 _backgroundColorDay;
        private float4 _backgroundColorNight;
        private float4 _backgroundColor;

        private Light _sun;

        private Transform _camTransform;
        private readonly Camera _campComp = new Camera(ProjectionMethod.Perspective, 1, 1000, M.PiOver4);

        private bool _isLoaded;

        private async void LoadAssets()
        {

            _gui = await GUIHelper.CreateDefaultGui(Width, Height, "FUSEE Deferred Example", CanvasRenderMode.Screen,
                BtnLogoEnter, BtnLogoExit, BtnLogoDown);

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Load the rocket model
            _rocketScene = await AssetStorage.GetAsync<SceneContainer>("sponza.fus").ConfigureAwait(false);
            //_rocketScene = AssetStorage.Get<SceneContainer>("sponza_wo_textures.fus");
            //_rocketScene = AssetStorage.Get<Scene>("shadowTest.fus");

            //Add lights to the scene
            _sun = new Light() { Type = LightType.Parallel, Color = new float4(0.99f, 0.9f, 0.8f, 1), Active = true, Strength = 1f, IsCastingShadows = true, Bias = 0.0f };
            var redLight = new Light() { Type = LightType.Point, Color = new float4(1, 0, 0, 1), MaxDistance = 150, Active = true, IsCastingShadows = false, Bias = 0.015f };
            var blueLight = new Light() { Type = LightType.Spot, Color = new float4(0, 0, 1, 1), MaxDistance = 900, Active = true, OuterConeAngle = 25, InnerConeAngle = 5, IsCastingShadows = true, Bias = 0.000040f };
            var greenLight = new Light() { Type = LightType.Point, Color = new float4(0, 1, 0, 1), MaxDistance = 600, Active = true, IsCastingShadows = true, Bias = 0f };

            _sunTransform = new Transform() { Translation = new float3(0, 2000, 0), Rotation = new float3(M.DegreesToRadians(90), 0, 0), Scale = new float3(500, 500, 500) };

            var aLotOfLights = new ChildList
            {
                new SceneNode()
                {
                    Name = "sun",
                    Components = new List<SceneComponent>()
                    {
                        _sunTransform,
                        _sun,
                    },
                    //Children = new ChildList()
                    //{
                    //    new SceneNode()
                    //    {
                    //        s = new List<Scene>()
                    //        {
                    //            new Transform
                    //            {
                    //                Scale = float3.One/2f
                    //            },
                    //            new Cube()
                    //        }
                    //    }
                    //}
                },
                new SceneNode()
                {
                    Name = "blueLight",
                    Components = new List<SceneComponent>()
                {
                    new Transform(){ Translation = new float3(-600, 180, 180), Rotation = new float3(M.DegreesToRadians(180), 0, 0)},
                    blueLight,
                }
                },
                new SceneNode()
                {
                    Name = "redLight1",
                     Components = new List<SceneComponent>()
                {
                    new Transform(){ Translation = new float3(-600, 180, 180)},
                    redLight,
                }
                },
                new SceneNode()
                {
                    Name = "redLight2",
                     Components = new List<SceneComponent>()
                {
                    new Transform(){ Translation = new float3(-600, 180, -140)},
                    redLight,
                }
                },
                new SceneNode()
                {
                    Name = "redLight3",
                     Components = new List<SceneComponent>()
                {
                    new Transform(){ Translation = new float3(500, 180, 180)},
                    redLight,
                }
                },
                new SceneNode()
                {
                    Name = "redLight4",
                     Components = new List<SceneComponent>()
                {
                    new Transform(){ Translation = new float3(500, 180, -140)},
                    redLight,
                }
                },
                new SceneNode()
                {
                    Name = "greenLight",
                     Components = new List<SceneComponent>()
                {
                    new Transform(){ Translation = new float3(0, 100, 150)},
                    greenLight,
                }
                },
            };

            _rocketScene.Children.Add(new SceneNode()
            {
                Name = "Light",
                Children = aLotOfLights
            });

            _rocketScene.Children.Add(
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

            // Wrap a SceneRenderer around the scene.
            _sceneRenderer = new SceneRendererDeferred(_rocketScene);

            // Wrap a SceneRenderer around the GUI.
            _guiRenderer = new SceneRendererForward(_gui);

            _isLoaded = true;
        }

        // Init is called on startup.
        public override void Init()
        {
            _camTransform = new Transform()
            {
                Scale = float3.One,
                Translation = float3.Zero
            };

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            _campComp.BackgroundColor = _backgroundColorDay = _backgroundColor = new float4(0.8f, 0.9f, 1, 1);
            _backgroundColorNight = new float4(0, 0, 0.05f, 1);

            LoadAssets();
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            if (!_isLoaded) return;

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            //if (!rotate)
            //{
            //    _sunTransform.RotateAround(new float3(0, 0, 0), float3.UnitX, M.DegreesToRadians(20));
            //    rotate = true;
            //}

            //_sunTransform.RotateAround(new float3(0, 0, 0), float3.UnitX, M.DegreesToRadians(0.5f) * Time.DeltaTime * 50);

            var deg = (M.RadiansToDegrees(_sunTransform.Rotation.x)) - 90;
            if (deg < 0)
                deg = (360 + deg);

            var normalizedDeg = (deg) / 360;
            float localLerp;

            if (normalizedDeg <= 0.5)
            {
                _backgroundColor = _backgroundColorDay;
                localLerp = normalizedDeg / 0.5f;
                _backgroundColor.xyz = float3.Lerp(_backgroundColorDay.xyz, _backgroundColorNight.xyz, localLerp);
            }
            else
            {
                _backgroundColor = _backgroundColorNight;
                localLerp = (normalizedDeg - 0.5f) / (0.5f);
                _backgroundColor.xyz = float3.Lerp(_backgroundColorNight.xyz, _backgroundColorDay.xyz, localLerp);
            }

            _campComp.BackgroundColor = _backgroundColor;

            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Keyboard.IsKeyDown(KeyCodes.F))
                _sceneRenderer.FxaaOn = !_sceneRenderer.FxaaOn;

            if (Keyboard.IsKeyDown(KeyCodes.G))
                _sceneRenderer.SsaoOn = !_sceneRenderer.SsaoOn;

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

            _camTransform.FpsView(_angleHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, Time.DeltaTime * 1000);

            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        public void BtnLogoEnter(CodeComponent sender)
        {
            _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>().SetEffectParam("DiffuseColor", new float4(0.8f, 0.8f, 0.8f, 1f));
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>().SetEffectParam("DiffuseColor", float4.One);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }
    }
}