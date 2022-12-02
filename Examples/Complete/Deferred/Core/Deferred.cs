using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.Deferred.Core
{
    [FuseeApplication(Name = "FUSEE Deferred Rendering Example", Description = "")]
    public class Deferred : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;

        private SceneContainer _sponzaScene;
        private SceneRendererDeferred _sceneRendererDeferred;
        private SceneRendererForward _sceneRendererForward;

        private bool _keys;

        private Transform _sunTransform;

        private float4 _backgroundColorDay;
        private float4 _backgroundColorNight;
        private float4 _backgroundColor;

        private Light _sun;

        private Transform _camTransform;
        private readonly Camera _campComp = new(ProjectionMethod.Perspective, 1, 1000, M.PiOver4);

        private bool _renderDeferred = true;

        private async Task Load()
        {
            VSync = false;

            _camTransform = new Transform()
            {
                Scale = float3.One,
                Translation = float3.Zero
            };

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            _campComp.BackgroundColor = _backgroundColorDay = _backgroundColor = new float4(0.8f, 0.9f, 1, 1);
            _backgroundColorNight = new float4(0, 0, 0.05f, 1);

            // Load the sponza model
            _sponzaScene = await AssetStorage.GetAsync<SceneContainer>("sponza.fus");

            //Add lights to the scene
            _sun = new Light() { Type = LightType.Parallel, Color = new float4(0.99f, 0.9f, 0.8f, 1), Active = true, Strength = 1f, IsCastingShadows = true, Bias = 0.0f };
            var redLight = new Light() { Type = LightType.Point, Color = new float4(1, 0.56f, 0.1f, 1), MaxDistance = 15, Active = true, IsCastingShadows = false, Bias = 0.015f };
            var blueLight = new Light() { Type = LightType.Spot, Color = new float4(0.6f, 0.8f, 1, 1), MaxDistance = 180, Active = true, OuterConeAngle = 25, InnerConeAngle = 5, IsCastingShadows = true, Bias = 0.0000001f };
            var greenLight = new Light() { Type = LightType.Point, Color = new float4(0.8f, 1.0f, 0.6f, 1), Strength = 1f, MaxDistance = 50, Active = true, IsCastingShadows = true, Bias = 0.1f };

            _sunTransform = new Transform() { Translation = new float3(0, 100, 0), Rotation = new float3(M.DegreesToRadians(90), 0, 0), Scale = new float3(100, 100, 100) };

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
                },
                new SceneNode()
                {
                    Name = "blueLight",
                    Components = new List<SceneComponent>()
                    {
                        new Transform(){ Translation = new float3(-100, 25, 36), Rotation = new float3(M.DegreesToRadians(180), 0, 0)},
                        blueLight,
                    }
                },
                new SceneNode()
                {
                    Name = "redLight1",
                    Components = new List<SceneComponent>()
                    {
                        new Transform(){ Translation = new float3(-49, 15, 15)},
                        redLight,
                    }
                },
                new SceneNode()
                {
                    Name = "redLight2",
                    Components = new List<SceneComponent>()
                    {
                        new Transform(){ Translation = new float3(-49, 15, -15)},
                        redLight,
                    }
                },
                new SceneNode()
                {
                    Name = "redLight3",
                    Components = new List<SceneComponent>()
                    {
                        new Transform(){ Translation = new float3(54, 15, 15)},
                        redLight,
                    }
                },
                new SceneNode()
                {
                    Name = "redLight4",
                    Components = new List<SceneComponent>()
                    {
                        new Transform(){ Translation = new float3(54, 15, -15)},
                        redLight,
                    }
                },
                new SceneNode()
                {
                    Name = "greenLight",
                    Components = new List<SceneComponent>()
                    {
                        new Transform(){ Translation = new float3(0, 25, 0)},
                        greenLight,
                    }
                },
            };

            _sponzaScene.Children.Add(new SceneNode()
            {
                Name = "Light",
                Children = aLotOfLights
            });

            _sponzaScene.Children.Add(
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
            _sceneRendererDeferred = new SceneRendererDeferred(_sponzaScene);
            _sceneRendererForward = new SceneRendererForward(_sponzaScene);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                System.Console.WriteLine("Not running on windows, disabling SSAO. Not supported for WebGL 2.0 / OpenGL 2.0 ES");
                _sceneRendererDeferred.SsaoOn = false;
            }
        }

        // InitAsync is called on startup.
        public override async Task InitAsync()
        {
            await Load();
            await base.InitAsync();
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
                    _angleVelHorz = RotationSpeed * Keyboard.LeftRightAxis * DeltaTimeUpdate;
                    _angleVelVert = RotationSpeed * Keyboard.UpDownAxis * DeltaTimeUpdate;
                }
            }

            _angleHorz -= _angleVelHorz;
            _angleVert -= _angleVelVert;
            _angleVelHorz = 0;
            _angleVelVert = 0;

            _camTransform.FpsView(_angleHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTimeUpdate * 200);

            if (Keyboard.IsKeyDown(KeyCodes.F))
                _sceneRendererDeferred.FxaaOn = !_sceneRendererDeferred.FxaaOn;

            if (Keyboard.IsKeyDown(KeyCodes.G) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _sceneRendererDeferred.SsaoOn = !_sceneRendererDeferred.SsaoOn;

            if (Keyboard.IsKeyDown(KeyCodes.F1) && _renderDeferred)
                _renderDeferred = false;
            else if (Keyboard.IsKeyDown(KeyCodes.F1) && !_renderDeferred)
                _renderDeferred = true;
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            //Diagnostics.Warn(FramesPerSecond);
            //_sunTransform.RotateAround(new float3(0, 0, 0), new float3(M.DegreesToRadians(0.5f) * DeltaTime * 50, 0 ,0));

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

            if (_renderDeferred)
                _sceneRendererDeferred.Render(RC);
            else
                _sceneRendererForward.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}