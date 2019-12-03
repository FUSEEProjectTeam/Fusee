using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System;
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

        private const float ZNear = 0.1f;

        //For shadow calculation the distance to the far clipping plane should be as small as possible to ensure the best shadow map resolution when using parallel lights.
        //Can be a custom value when cascaded shadow maps are implemented.
        private const float ZFar = 3000;
        private readonly float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;
        
        private bool _keys;

        private const float twoPi = M.Pi * 2.0f;       

        private TransformComponent _sunTransform;

        private float4 _backgroundColorDay;
        private float4 _backgroundColorNight;
        private float4 _backgroundColor;

        private LightComponent _sun;

        private TransformComponent _camTransform;

        // Init is called on startup. 
        public override void Init()
        {           
            _gui = CreateGui();
            
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = _backgroundColorDay = _backgroundColor = new float4(0.8f, 0.9f, 1, 1);
            _backgroundColorNight = new float4(0, 0, 0.05f, 1);

            // Load the rocket model
            //_rocketScene = AssetStorage.Get<SceneContainer>("sponza.fus");
            _rocketScene = AssetStorage.Get<SceneContainer>("sponza_wo_textures.fus");
            //_rocketScene = AssetStorage.Get<SceneContainer>("shadowTest.fus");            

            //_camTransform = new TransformComponent()
            //{
            //    Rotation = new float3(0, M.DegreesToRadians(0), 0),
            //    Scale = float3.One,
            //    Translation = new float3(0, 20, -10)

            //};

            //var camera = new SceneNodeContainer()
            //{
            //    Name = "Camera",
            //    Components = new List<SceneComponentContainer>()
            //    {
            //       _camTransform,
            //        new CameraComponent(ProjectionMethod.PERSPECTIVE, ZNear, ZFar, _fovy)
            //    }
            //};

            //_rocketScene.Children.Insert(0, camera);

            //Add lights to the scene
            _sun = new LightComponent() { Type = LightType.Parallel, Color = new float4(0.99f, 0.9f, 0.8f, 1), Active = true, Strength = 1f, IsCastingShadows = true, Bias = 0.025f };
            var redLight = new LightComponent() { Type = LightType.Point, Color = new float4(1, 0, 0, 1), MaxDistance = 150, Active = true, IsCastingShadows = false, Bias = 0.015f };
            var blueLight = new LightComponent() { Type = LightType.Spot, Color = new float4(0, 0, 1, 1), MaxDistance = 1000, Active = true, OuterConeAngle = 25, InnerConeAngle = 5, IsCastingShadows = true, Bias = 0.000008f };
            var greenLight = new LightComponent() { Type = LightType.Point, Color = new float4(0, 1, 0, 1), MaxDistance = 600, Active = true, IsCastingShadows = true, Bias = 0f };

            _sunTransform = new TransformComponent() { Translation = new float3(0, 2000, 0), Rotation = new float3(M.DegreesToRadians(90), 0, 0), Scale = new float3(500, 500, 500) };

            var aLotOfLights = new ChildList
            {
                new SceneNodeContainer()
                {
                    Name = "sun",
                    Components = new List<SceneComponentContainer>()
                {
                    _sunTransform,
                    _sun,
                },
                    //Children = new ChildList()
                    //{
                    //    new SceneNodeContainer()
                    //    {
                    //        Components = new List<SceneComponentContainer>()
                    //        {
                    //            new TransformComponent
                    //            {
                    //                Scale = float3.One/2f
                    //            },
                    //            new Cube()
                    //        }
                    //    }
                    //}

                },
                new SceneNodeContainer()
                {
                    Name = "blueLight",
                    Components = new List<SceneComponentContainer>()
                {
                    new TransformComponent(){ Translation = new float3(-600, 180, 180), Rotation = new float3(M.DegreesToRadians(180), 0, 0)},
                    blueLight,
                }
                },
                new SceneNodeContainer()
                {
                    Name = "redLight1",
                    Components = new List<SceneComponentContainer>()
                {
                    new TransformComponent(){ Translation = new float3(-600, 180, 180)},
                    redLight,
                }
                },
                new SceneNodeContainer()
                {
                    Name = "redLight2",
                    Components = new List<SceneComponentContainer>()
                {
                    new TransformComponent(){ Translation = new float3(-600, 180, -140)},
                    redLight,
                }
                },
                new SceneNodeContainer()
                {
                    Name = "redLight3",
                    Components = new List<SceneComponentContainer>()
                {
                    new TransformComponent(){ Translation = new float3(500, 180, 180)},
                    redLight,
                }
                },
                new SceneNodeContainer()
                {
                    Name = "redLight4",
                    Components = new List<SceneComponentContainer>()
                {
                    new TransformComponent(){ Translation = new float3(500, 180, -140)},
                    redLight,
                }
                },
                new SceneNodeContainer()
                {
                    Name = "greenLight",
                    Components = new List<SceneComponentContainer>()
                {
                    new TransformComponent(){ Translation = new float3(0, 100, 150)},
                    greenLight,

                }
                },
            };


            _rocketScene.Children.Add(new SceneNodeContainer()
            {
                Name = "LightContainer",
                Children = aLotOfLights
            });

            // Wrap a SceneRenderer around the scene.
            _sceneRenderer = new SceneRendererDeferred(_rocketScene);
           
            // Wrap a SceneRenderer around the GUI.
            _guiRenderer = new SceneRendererForward(_gui);
        }

        bool rotate = false;
        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            //if (!rotate)
            //{
            //    _sunTransform.RotateAround(new float3(0, 0, 0), new float3(M.DegreesToRadians(20), 0, 0));
            //    rotate = true;
            //}

            //_sunTransform.RotateAround(new float3(0, 0, 0), new float3(M.DegreesToRadians(0.5f), 0, 0));

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
           
            RC.ClearColor = _backgroundColor;

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

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, +2, -10, 0, +2, 0, 0, 1, 0);

            var view = mtxCam * mtxRot;
            var perspective = float4x4.CreatePerspectiveFieldOfView(_fovy, (float)Width / Height, ZNear, ZFar);
            var orthographic = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);

            //FpsView();            

            RC.View = view;
            RC.Projection = perspective;
            _sceneRenderer.Render(RC);

            // Constantly check for interactive objects.           
            RC.View = view;
            RC.Projection = orthographic;
            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            RC.View = view;
            RC.Projection = orthographic;
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        /// <summary>
        /// Translates and rotates the camera to achieve a fps cam.
        /// </summary>
        private void FpsView()
        {
            if ((_angleHorz >= twoPi && _angleHorz > 0f) || _angleHorz <= -twoPi)
                _angleHorz %= twoPi;
            if ((_angleVert >= twoPi && _angleVert > 0f) || _angleVert <= -twoPi)
                _angleVert %= twoPi;

            var camForward = float4x4.CreateRotationYX(new float2(_angleVert, _angleHorz)) * float3.UnitZ;
            var camRight = float4x4.CreateRotationYX(new float2(_angleVert, _angleHorz)) * float3.UnitX;

            _camTransform.Translation += camForward * Keyboard.WSAxis * DeltaTime * 1000;
            _camTransform.Translation += camRight * Keyboard.ADAxis * DeltaTime * 1000;           

            _camTransform.Rotation.y = _angleHorz;
            _camTransform.Rotation.x = _angleVert;            
        }
        

        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");

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
            var fuseeLogo = new TextureNodeContainer(
                "fuseeLogo",
                vsTex,
                psTex,
                //Set the diffuse texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_LEFT),
                //Define Offset and therefor the size of the element.                
                UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f))
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 18);

            var text = new TextNodeContainer(
                "FUSEE Deferred Example",
                "ButtonText",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_HORIZONTAL),
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_HORIZONTAL, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery), 250f);


            var canvas = new CanvasNodeContainer(
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
                Children = new List<SceneNodeContainer>
                {
                    //Add canvas.
                    canvas
                }
            };
        }

        public void BtnLogoEnter(CodeComponent sender)
        {
            _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffectComponent>().Effect.SetEffectParam("DiffuseColor", new float4(0.8f, 0.8f, 0.8f, 1f));
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffectComponent>().Effect.SetEffectParam("DiffuseColor", float4.One);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }
    }
}