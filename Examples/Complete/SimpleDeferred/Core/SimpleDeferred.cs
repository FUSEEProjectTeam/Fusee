using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
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

        private SceneContainer _sponzaScene;
        private SceneRendererDeferred _sceneRendererDeferred;
        private SceneRendererForward _sceneRendererForward;

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

        // Init is called on startup.
        public override void Init()
        {
            _camTransform = new Transform()
            {
                Scale = float3.One,
                Translation = float3.Zero
            };

            _gui = CreateGui();

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            _campComp.BackgroundColor = _backgroundColorDay = _backgroundColor = new float4(0.8f, 0.9f, 1, 1);
            _backgroundColorNight = new float4(0, 0, 0.05f, 1);

            // Load the rocket model
            _sponzaScene = AssetStorage.Get<SceneContainer>("sponza.fus");

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

            _camTransform.FpsView(_angleHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, Time.DeltaTime * 200);

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

        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");
            var psText = AssetStorage.Get<string>("text.frag");

            var canvasWidth = Width / 100f;
            var canvasHeight = Height / 100f;

            var btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            var fuseeLogo = new TextureNode(
                "fuseeLogo",
                vsTex,
                psTex,
                //Set the diffuse texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                //Define Offset and therefor the size of the element.
                UIElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f)),
                float2.One
                );

            // TODO (mr): How to add this?
            //fuseeLogo.Add(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 24);

            var text = new TextNode(
                "FUSEE Deferred Example",
                "ButtonText",
                vsTex,
                psText,
                UIElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                UIElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                (float4)ColorUint.Greenery,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            var guiCamComp = new Camera(ProjectionMethod.Orthographic, 1, 3000, M.PiOver4)
            {
                ClearColor = false
            };

            var cam = new SceneNode()
            {
                Name = "GUICam",
                Components = new List<SceneComponent>()
                {
                    guiCamComp
                }
            };

            var canvas = new CanvasNode(
                "Canvas",
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-canvasWidth / 2, -canvasHeight / 2f),
                    Max = new float2(canvasWidth / 2, canvasHeight / 2f)
                })
            {
                Children = new ChildList()
                {
                    //Simple Texture Node, contains the fusee logo.
                    fuseeLogo,
                    text
                }
            };

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    cam,
                    //Add canvas.
                    canvas
                }
            };
        }

        public void BtnLogoEnter(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, (float4)ColorUint.Black);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 0.8f);
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, float4.One);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 1f);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }
    }
}