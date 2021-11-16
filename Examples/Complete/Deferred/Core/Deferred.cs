using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.FileReader.LasReader;
using System.Collections.Generic;
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

        private SceneContainer _gui;
        private SceneInteractionHandler _sih;

        private bool _keys;

        private Transform _sunTransform;

        private float4 _backgroundColorDay;
        private float4 _backgroundColorNight;
        private float4 _backgroundColor;

        private Light _sun;

        private Transform _camTransform;
        private readonly Camera _campComp = new(ProjectionMethod.Perspective, 1, 1000, M.PiOver4);

        // Init is called on startup.
        public override void Init()
        {
            VSync = false;

            _camTransform = new Transform()
            {
                Scale = float3.One,
                Translation = float3.Zero
            };

            _gui = FuseeGuiHelper.CreateDefaultGui(this, CanvasRenderMode.Screen, "FUSEE Deferred Rendering Example");

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            _campComp.BackgroundColor = _backgroundColorDay = _backgroundColor = new float4(0.8f, 0.9f, 1, 1);
            _backgroundColorNight = new float4(0, 0, 0.05f, 1);

            // Load the rocket model
            _sponzaScene = AssetStorage.Get<SceneContainer>("sponza.fus");

            var holbeinNode = new SceneNode() 
            { 
                Components = new List<SceneComponent>()
                {
                    new Transform()
                    {
                        Translation = float3.Zero,
                        Rotation = float3.Zero,
                        Scale = float3.One
                    },
                    new PointCloudSurfaceEffect
                    {
                        PointSize = 5,
                        ColorMode = (int)ColorMode.Point,
                        PointShape = (int)PointShape.Paraboloid,
                        DepthTex = null,
                        EDLStrength = 1f,
                        EDLNeighbourPixels = 2
                    }
                }
            };
            holbeinNode.Components.AddRange(LasToMesh.GetMeshsFromLasFile(new Pos64Col32_Accessor(), PointType.Pos64Col32, "D:\\LAS\\HolbeinPferd.las", out var aabbRes, false));

            _sponzaScene.Children[0].Children.Add(holbeinNode);

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

            // Wrap a SceneRenderer around the GUI.
        }

        private bool _renderDeferred = true;

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            Diagnostics.Warn(FramesPerSecond);
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

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

            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Keyboard.IsKeyDown(KeyCodes.F))
                _sceneRendererDeferred.FxaaOn = !_sceneRendererDeferred.FxaaOn;

            if (Keyboard.IsKeyDown(KeyCodes.G))
                _sceneRendererDeferred.SsaoOn = !_sceneRendererDeferred.SsaoOn;

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

            _camTransform.FpsView(_angleHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTime * 200);

            if (Keyboard.IsKeyDown(KeyCodes.F1) && _renderDeferred)
                _renderDeferred = false;
            else if (Keyboard.IsKeyDown(KeyCodes.F1) && !_renderDeferred)
                _renderDeferred = true;

            if (_renderDeferred)
                _sceneRendererDeferred.Render(RC);
            else
                _sceneRendererForward.Render(RC);

            //_guiRenderer.Render(RC);

            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}