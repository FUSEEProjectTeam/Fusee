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
        private TransformComponent _sndCamTransform;
        private readonly CameraComponent _mainCam = new CameraComponent(ProjectionMethod.PERSPECTIVE, 5, 250, M.PiOver4);
        private readonly CameraComponent _guiCam = new CameraComponent(ProjectionMethod.ORTHOGRAPHIC, 1, 1000, M.PiOver4);
        private readonly CameraComponent _sndCam = new CameraComponent(ProjectionMethod.PERSPECTIVE, 1, 1000, M.PiOver4);

        private TransformComponent _cubeOneTransform;
        private WireframeCube _frustum;
        private float _anlgeHorz;
        private float _angleVert;
        private float _valHorz;
        private float _valVert;

        // Init is called on startup. 
        public override async Task<bool> Init()
        {
            VSync = false;

            _mainCam.Viewport = new float4(0, 0, 100, 100);
            _sndCam.BackgroundColor = new float4(0f, 0f, 0f, 1);
            _mainCam.Layer = -1;
            _mainCam.FrustumCullingOn = true;

            _sndCam.Viewport = new float4(60, 60, 40, 40);
            _sndCam.BackgroundColor = new float4(0.5f, 0.5f, 0.5f, 1);
            _sndCam.Layer = 10;
            _sndCam.FrustumCullingOn = false;

            _guiCam.ClearColor = false;
            _guiCam.ClearDepth = false;
            _guiCam.FrustumCullingOn = false;

            _mainCamTransform = _guiCamTransform = new TransformComponent()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 1, -30),
                Scale = new float3(1, 1, 1)
            };

            _gui = CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            _frustum = new WireframeCube();
            var frustumNode = new SceneNodeContainer()
            {
                Name = "Frustum",
                Components = new List<SceneComponentContainer>()
                {
                    new ShaderEffectComponent(){Effect = ShaderCodeBuilder.MakeShaderEffect(new float4(1,1,0,1), float4.One, 0) },
                    _frustum
                }
            };

            var cam = new SceneNodeContainer()
            {
                Name = "MainCam",
                Components = new List<SceneComponentContainer>()
                {
                    _mainCamTransform,
                    _mainCam,
                    new ShaderEffectComponent(){Effect = ShaderCodeBuilder.MakeShaderEffect(new float4(1,0,0,1), float4.One, 10) },
                    new Cube(),
                    
                },
                Children = new ChildList()
                {
                    new SceneNodeContainer()
                    {
                        Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent()
                            {
                                Scale = new float3(0.5f, 0.5f, 1f),
                                Translation = new float3(0,0, 1f)
                            },
                            new Cube()
                        }
                    }
                }
            };

            _sndCamTransform = new TransformComponent()
            {
                Rotation = new float3(M.PiOver6, 0, 0),//float3.Zero,
                Translation = new float3(10, 40, -60),
                Scale = float3.One
            };

            var cam1 = new SceneNodeContainer()
            {
                Name = "SecondCam",
                Components = new List<SceneComponentContainer>()
                {
                    _sndCamTransform,
                    _sndCam,
                }
            };           
            

            // Load the rocket model            
            _rocketScene = AssetStorage.Get<SceneContainer>("rnd.fus");

            _cubeOneTransform = _rocketScene.Children[0].GetComponent<TransformComponent>();
            //_cubeOneTransform.Rotate(new float3(0, M.PiOver4, 0));
                        
            _rocketScene.Children.Add(cam);
            _rocketScene.Children.Add(cam1);
            _rocketScene.Children.Add(frustumNode);
           
            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_rocketScene);
            _guiRenderer = new SceneRendererForward(_gui);

            _rotAxis = float3.UnitY * float4x4.CreateRotationYZ(new float2(M.PiOver4, M.PiOver4));
            _rotPivot = _rocketScene.Children[1].GetComponent<TransformComponent>().Translation;

            return true;
        }

        //TODO: borrowed from ShadowMapping - find a place for this method where it can be accessed for shadow mapping, frustum debugging and all the other cases.
        private static IEnumerable<float3> GetWorldSpaceFrustumCorners(float4x4 projectionMatrix, float4x4 viewMatrix)
        {
            //1. Calculate the 8 corners of the view frustum in world space. This can be done by using the inverse view-projection matrix to transform the 8 corners of the NDC cube (which in OpenGL is [‒1, 1] along each axis).
            //2. Transform the frustum corners to a space aligned with the shadow map axes.This would commonly be the directional light object's local space. 
            //In fact, steps 1 and 2 can be done in one step by combining the inverse view-projection matrix of the camera with the inverse world matrix of the light.
            var invViewProjection = float4x4.Invert(projectionMatrix * viewMatrix);
            
            var frustumCorners = new float4[8];

            //TODO: make the NDC corners static, likely at the same place where this method lives in the future. "Frustum" Object?
            frustumCorners[0] = invViewProjection * new float4(-1, -1, -1, 1); //nbl
            frustumCorners[1] = invViewProjection * new float4(1, -1, -1, 1); //nbr 
            frustumCorners[2] = invViewProjection * new float4(-1, 1, -1, 1); //ntl  
            frustumCorners[3] = invViewProjection * new float4(1, 1, -1, 1); //ntr  
            frustumCorners[4] = invViewProjection * new float4(-1, -1, 1, 1); //fbl 
            frustumCorners[5] = invViewProjection * new float4(1, -1, 1, 1); //fbr 
            frustumCorners[6] = invViewProjection * new float4(-1, 1, 1, 1); //ftl  
            frustumCorners[7] = invViewProjection * new float4(1, 1, 1, 1); //ftr     

            for (int i = 0; i < frustumCorners.Length; i++)
            {
                var corner = frustumCorners[i];
                corner /= corner.w; //world space frustum corners               
                frustumCorners[i] = corner;
                yield return frustumCorners[i].xyz;
            }            
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            if (Mouse.RightButton)
            {
                _valHorz = Mouse.XVel * 0.000005f;
                _valVert = Mouse.YVel * 0.000005f;

                _anlgeHorz -= _valHorz;
                _angleVert -= _valVert;
                _valHorz = _valVert = 0;

                _sndCamTransform.FpsView(_anlgeHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, Time.DeltaTime * 10);
            }
            else if (Mouse.LeftButton)
            {
                _valHorz = Mouse.XVel * 0.00005f;
                _valVert = Mouse.YVel * 0.00005f;

                _anlgeHorz -= _valHorz;
                _angleVert -= _valVert;
                _valHorz = _valVert = 0;

                _mainCamTransform.FpsView(_anlgeHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, Time.DeltaTime * 10);
            }

            _frustum.Vertices = GetWorldSpaceFrustumCorners(_mainCam.GetProjectionMat(Width, Height, out var viewport), float4x4.Invert(_mainCamTransform.Matrix())).ToArray();

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