using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Fusee.Engine.GUI;

namespace Fusee.Examples.Camera.Core
{
    [FuseeApplication(Name = "FUSEE Camera Example", Description = " ")]
    public class Camera : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = 0, _angleVert = 0, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneRendererForward _sceneRenderer;

        //private const float ZNear = 1f;
        //private const float ZFar = 1000;
        //private float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;

        private bool _keys;

        private TransformComponent _mainCamTransform;
        private TransformComponent _guiCamTransform;
        private CameraComponent _mainCam = new CameraComponent(ProjectionMethod.PERSPECTIVE, 1, 1000, M.PiOver4);
        private CameraComponent _guiCam = new CameraComponent(ProjectionMethod.ORTHOGRAPHIC, 1, 1000, M.PiOver4);

        // Init is called on startup. 
        public override void Init()
        {
            _mainCam.Viewport = new float4(0, 0, 100, 100);
            _mainCam.BackgroundColor = new float4(1, 1, 1, 1);            
            _guiCam.BackgroundColor = new float4(1, 0, 0, 0);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            // RC.ClearColor = new float4(1, 1, 1, 1);

            _mainCamTransform = _guiCamTransform = new TransformComponent()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 2, -10),
                Scale = float3.One
            };

            _gui = CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            var cam = new SceneNodeContainer()
            {
                Name = "MainCam",
                Components = new List<SceneComponentContainer>()
                {
                    _mainCamTransform,
                    _mainCam,
                    new Cube()
                },
                Children = new ChildList()
                {
                    new SceneNodeContainer()
                    {
                        Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent()
                            {
                                Scale = new float3(0.33f, 0.33f, 0.5f),
                                Translation = new float3(0,0,0.75f)
                            },
                            new Cube()
                        }
                    }
                }
            };

            var cam1 = new SceneNodeContainer()
            {
                Name = "Cam1",
                Components = new List<SceneComponentContainer>()
                {
                    new TransformComponent()
                    {
                        Rotation = new float3(0, 0, 0),//float3.Zero,
                        Translation = new float3(0, 2, -20),
                        Scale = float3.One
                    },
                    new CameraComponent(ProjectionMethod.PERSPECTIVE, 1, 1000, M.PiOver4)
                    {
                        Viewport = new float4(60, 60, 40, 40),
                        BackgroundColor = new float4(0.5f,0.5f, 0.5f, 1),                        
                    },                   
                }
            };            

            // Load the rocket model            
            _rocketScene = AssetStorage.Get<SceneContainer>("FUSEERocket.fus");

            _rocketScene.Children.Add(cam);
            _rocketScene.Children.Add(cam1);
           

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_rocketScene);
            _guiRenderer = new SceneRendererForward(_gui);
        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            //RC.Clear(ClearFlags.Color | ClearFlags.Depth);

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

            //_mainCamTransform.Rotation = new float3(_angleVert, _angleHorz, 0);
            _mainCamTransform.RotateAround(new float3(0,0,0), new float3(M.PiOver4 * 0.01f, M.PiOver4 * 0.01f, 0));            

            // Create the camera matrix and set it as the current ModelView transformation
            //var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            //var mtxCam = float4x4.LookAt(0, +2, -10, 0, +2, 0, 0, 1, 0);

            //var view = mtxCam * mtxRot;
            //var perspective = float4x4.CreatePerspectiveFieldOfView(_fovy, (float)Width / Height, ZNear, ZFar);
            //var orthographic = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);

            //RC.View = view;
            //RC.Projection = perspective;

            _sceneRenderer.Render(RC);

            //Constantly check for interactive objects.            
            //RC.Projection = orthographic;

            //_guiRenderer.Render(RC);

            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);
            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
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
                "FUSEE Simple Example",
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

             

            var cam = new SceneNodeContainer()
            {
                Name = "GUICam",
                Components = new List<SceneComponentContainer>()
                {
                    _guiCamTransform,
                    _guiCam
                }
            };

            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    cam,
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