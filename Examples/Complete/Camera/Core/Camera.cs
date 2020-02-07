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
using System.Threading.Tasks;

namespace Fusee.Examples.Camera.Core
{
    [FuseeApplication(Name = "FUSEE Camera Example", Description = " ")]
    public class Camera : RenderCanvas
    {
        // angle variables
        private readonly float _rotAngle = M.PiOver4;
        private float3 _rotAxis;
        private float3 _rotPivot;

        private SceneContainer _rocketScene;
        private SceneRendererForward _sceneRenderer;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;

        private TransformComponent _mainCamTransform;
        private TransformComponent _guiCamTransform;
        private readonly CameraComponent _mainCam = new CameraComponent(ProjectionMethod.PERSPECTIVE, 1, 1000, M.PiOver4);
        private readonly CameraComponent _guiCam = new CameraComponent(ProjectionMethod.ORTHOGRAPHIC, 1, 1000, M.PiOver4);
        private readonly CameraComponent _sndCam = new CameraComponent(ProjectionMethod.PERSPECTIVE, 1, 1000, M.PiOver4);



        // Init is called on startup. 
        public override async Task<bool> Init()
        {
            VSync = false;

            _mainCam.Viewport = new float4(0, 0, 100, 100);
            _mainCam.BackgroundColor = new float4(1, 1, 1, 1);
            _mainCam.Layer = -1;

            _sndCam.Viewport = new float4(60, 60, 40, 40);
            _sndCam.BackgroundColor = new float4(0.5f, 0.5f, 0.5f, 1);
            _sndCam.Layer = 10;

            _guiCam.ClearColor = false;
            _guiCam.ClearDepth = false;

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            // RC.ClearColor = new float4(1, 1, 1, 1);

            _mainCamTransform = _guiCamTransform = new TransformComponent()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 2, -10),
                Scale = new float3(0.33f, 0.33f, 0.5f)
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
                                Scale = new float3(3.03f, 3.03f, 2f),
                                Translation = new float3(0,0,-1.4f)
                            },
                            new Cube()
                        }
                    }
                }
            };

            var cam1 = new SceneNodeContainer()
            {
                Name = "SecondCam",
                Components = new List<SceneComponentContainer>()
                {
                    new TransformComponent()
                    {
                        Rotation = new float3(0, 0, 0),//float3.Zero,
                        Translation = new float3(0, 2, -20),
                        Scale = float3.One
                    },
                    _sndCam,
                }
            };

            // Load the rocket model            
            _rocketScene = AssetStorage.Get<SceneContainer>("FUSEERocket.fus");

            _rocketScene.Children.Add(cam);
            _rocketScene.Children.Add(cam1);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_rocketScene);
            _guiRenderer = new SceneRendererForward(_gui);

            _rotAxis = float3.UnitY * float4x4.CreateRotationYZ(new float2(M.PiOver4, M.PiOver4));
            _rotPivot = _rocketScene.Children[1].GetComponent<TransformComponent>().Translation;

            return true;
        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            _mainCamTransform.RotateAround(_rotPivot, _rotAxis, _rotAngle * DeltaTime * 5);

            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

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