using System;
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

namespace Fusee.Examples.ABCompare.Core
{
    [FuseeApplication(Name = "FUSEE ABCompare Example", Description = "A example for comparing two Models.")]
    public class ABCompare : RenderCanvas
    {
        public string ModelFile = "FUSEERocket.fus";
        public string ModelFile2 = "test.fus";

        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit, _zoomVel, _zoom, _viewtranslate;
        private static float2 _offset;
        private static float2 _offsetInit;
        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;
        private SceneContainer _scene1;
        private SceneContainer _scene2;
        private SceneRendererForward _sceneRenderer1;
        private SceneRendererForward _sceneRenderer2;
        private float4x4 _sceneCenter;
        private float4x4 _sceneScale;
        private bool _twoTouchRepeated;

        private bool _keys;
        private const float ZNear = 1f;
        private const float ZFar = 3000;
        private float _aspectRatio;
        private float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;
        private float _initWindowWidth;
        private float _initWindowHeight;
        private float _initCanvasWidth;
        private float _initCanvasHeight;
        private float _canvasWidth = 16;
        private float _canvasHeight = 9;

        private float _maxPinchSpeed;

        private GamePadDevice _gamePad;


        private bool _abbtn1;
        private bool _abbtn2;
        private bool _viewports = false;
        private bool _multipass = false;
        private Texture _bltDestinationTex;
        private WritableTexture _renderTex1;
        private WritableTexture _renderTex2;
        private float2 _point1;
        private float2 _point2;
        private float _radius;
        private float2 _middlepoint;
        private ShaderEffect _blurPassEffect;
        private SceneContainer _quadScene;
        private SceneRendererForward _sceneRendererBlur;
        private readonly int _texRes = (int)TexRes.HIGH_RES;
        

        
        // Init is called on startup. 
        public override async Task<bool> Init()
        {
            //Initialize objects we need for the multipass blur effect
            _renderTex1 = WritableTexture.CreateAlbedoTex(_texRes, _texRes);
            _renderTex2 = WritableTexture.CreateAlbedoTex(_texRes, _texRes);

            //=> Punkte zur Geradenberechnung dürfen nicht aufeinander liegen!!

            _point1 = new float2(0.5f, 0.5f);
            _point2 = new float2(0.5f, 0.6f);
            _radius = 0.1f;
            _middlepoint = new float2(0.5f, 0.5f);

            _blurPassEffect = new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = AssetStorage.Get<string>("screenFilledQuad.vert"),
                    PS = AssetStorage.Get<string>("multitex.frag"),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    },
                }
            },
            new[]
            {
                new EffectParameterDeclaration { Name = "InputTex1", Value = _renderTex1},
                new EffectParameterDeclaration { Name = "InputTex2", Value = _renderTex2},
                new EffectParameterDeclaration { Name = "Point1", Value = _point1},
                new EffectParameterDeclaration { Name = "Point2", Value = _point2},
                new EffectParameterDeclaration { Name = "Middlepoint", Value = _middlepoint},
                new EffectParameterDeclaration { Name = "Radius", Value = _radius}

            });

            _quadScene = new SceneContainer()
            {
                Children = new List<SceneNodeContainer>()
                {
                    new SceneNodeContainer()
                    {
                        Components = new List<SceneComponentContainer>()
                        {
                            new ProjectionComponent(ProjectionMethod.PERSPECTIVE, 0.1f, 1, M.DegreesToRadians(45f)),

                            new ShaderEffectComponent()
                            {
                                Effect = _blurPassEffect
                            },
                            new Plane()
                        }
                    }
                }
            };

            _initWindowWidth = Width;
            _initWindowHeight = Height;

            _initCanvasWidth = Width / 100f;
            _initCanvasHeight = Height / 100f;

            _canvasHeight = _initCanvasHeight;
            _canvasWidth = _initCanvasWidth;

            _aspectRatio = Width / (float)Height;

            // Initial "Zoom" value (it's rather the distance in view direction, not the camera's focal distance/opening angle)
            _zoom = 400;

            _angleRoll = 0;
            _angleRollInit = 0;
            _twoTouchRepeated = false;
            _offset = float2.Zero;
            _offsetInit = float2.Zero;

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Load the standard model
            //==> Models for multiple Viewports
            
            _scene1 = AssetStorage.Get<SceneContainer>(ModelFile);


            _scene2 = AssetStorage.Get<SceneContainer>(ModelFile2);

            // _scene1.Children.RemoveAt(0);
             //_scene2.Children.RemoveAt(0);

            // New Shader for used Models ===> not usable right now
            //_scene1.Children[0].GetComponent<ShaderEffectComponent>().Effect = ShaderCodeBuilder.MakeShaderEffect(new float4(1, 0, 0, 1), new float4(1, 0, 1, 1), 0.5f, 0.5f);
            //_scene1.Children[2].GetComponent<ShaderEffectComponent>().Effect = ShaderCodeBuilder.MakeShaderEffect(new float4(1, 0, 1, 1), new float4(0, 0, 1, 1), 0.5f, 0.5f);
           // _scene2.Children[1].GetComponent<ShaderEffectComponent>().Effect = ShaderCodeBuilder.MakeShaderEffect(new float4(0, 0, 1, 1), new float4(1, 0, 0, 1), 0.5f, 0.5f);
            //_scene2.Children[2].GetComponent<ShaderEffectComponent>().Effect = ShaderCodeBuilder.MakeShaderEffect(new float4(0, 0, 1, 1), new float4(0, 0, 0, 1), 0.5f, 0.5f);

            _gui = CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);


            // Register the input devices that are not already given.
            _gamePad = GetDevice<GamePadDevice>(0);



            // ======> needs to be done for both scenes, so model is always at center of scene

            AABBCalculator aabbc = new AABBCalculator(_scene1);
            var bbox = aabbc.GetBox();
            if (bbox != null)
            {
                // If the model origin is more than one third away from its bounding box, 
                // recenter it to the bounding box. Do this check individually per dimension.
                // This way, small deviations will keep the model's original center, while big deviations 
                // will make the model rotate around its geometric center.
                float3 bbCenter = bbox.Value.Center;
                float3 bbSize = bbox.Value.Size;
                float3 center = float3.Zero;
                if (System.Math.Abs(bbCenter.x) > bbSize.x * 0.3)
                    center.x = bbCenter.x;
                if (System.Math.Abs(bbCenter.y) > bbSize.y * 0.3)
                    center.y = bbCenter.y;
                if (System.Math.Abs(bbCenter.z) > bbSize.z * 0.3)
                    center.z = bbCenter.z;
                _sceneCenter = float4x4.CreateTranslation(-center);

                // Adjust the model size
                float maxScale = System.Math.Max(bbSize.x, System.Math.Max(bbSize.y, bbSize.z));
                if (maxScale != 0)
                    _sceneScale = float4x4.CreateScale(200.0f / maxScale);
                else
                    _sceneScale = float4x4.Identity;
            }

            // Wrap a SceneRenderer around the model.
            _sceneRendererBlur = new SceneRendererForward(_quadScene);


            //==> rendering with multiple viewports
             _sceneRenderer1 = new SceneRendererForward(_scene1);
           
            _sceneRenderer2 = new SceneRendererForward(_scene2);
            
            _guiRenderer = new SceneRendererForward(_gui);

            return true;
        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            //if (_gamePad != null)
            //    Diagnostics.Log(_gamePad.LSX);

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);


            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;

            }
            if (Keyboard.ADAxis != 0)
            {
                _keys = true;
            }

            var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
            // Zoom & Roll
            if (Touch.TwoPoint)
            {
                if (!_twoTouchRepeated)
                {
                    _twoTouchRepeated = true;
                    _angleRollInit = Touch.TwoPointAngle - _angleRoll;
                    _offsetInit = Touch.TwoPointMidPoint - _offset;
                    _maxPinchSpeed = 0;
                }
                _zoomVel = Touch.TwoPointDistanceVel * -0.01f;
                _angleRoll = Touch.TwoPointAngle - _angleRollInit;
                _offset = Touch.TwoPointMidPoint - _offsetInit;
                float pinchSpeed = Touch.TwoPointDistanceVel;
                if (pinchSpeed > _maxPinchSpeed) _maxPinchSpeed = pinchSpeed; // _maxPinchSpeed is used for debugging only.
            }
            else
            {
                _twoTouchRepeated = false;
                _zoomVel = Mouse.WheelVel * -0.5f;
                _angleRoll *= curDamp * 0.8f;
                _offset *= curDamp * 0.8f;
            }


            // UpDown / LeftRight rotation
            if (Mouse.LeftButton)
            {
              
                 float mousex = (Mouse.Position.x / Width);
                 float mousey = 1.0f - (Mouse.Position.y / Height);

                if (_abbtn1)
                {
                  
                    _point1 = new float2(mousex,mousey);
                    
                    Diagnostics.Log(Mouse.Position);
                    Diagnostics.Log(_point1);

                    _gui.Children.FindNodes(node => node.Name == "ab1").First().GetComponent<RectTransformComponent>().Offsets = UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(_initCanvasWidth * _point1.x, _initCanvasHeight *_point1.y), _initCanvasHeight, _initCanvasWidth, new float2(0.5f, 0.5f));
                }

                else if(_abbtn2)
                {
                   
                    _point2 = new float2(mousex, mousey);

                    Diagnostics.Log(Mouse.Position);

                    _gui.Children.FindNodes(node => node.Name == "ab2").First().GetComponent<RectTransformComponent>().Offsets = UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(_initCanvasWidth * _point2.x, _initCanvasHeight * _point2.y), _initCanvasHeight, _initCanvasWidth, new float2(0.5f, 0.5f));
                }

                else
                {
                    _keys = false;
                    _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                    _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
                }

            }
                     
           /* else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _keys = false;
                float2 touchVel;
                touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
            }*/
            else
            {

                _abbtn1 = false;
                _abbtn2 = false;

                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * DeltaTime;
                    _viewtranslate = Keyboard.ADAxis * 0.5f;
                    _radius = Keyboard.ADAxis;
                }
                else
                {
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }


            //=> Zugriff auf Uniform zur Laufzeit!!!

            _blurPassEffect.SetEffectParam("Radius", _radius);
            _blurPassEffect.SetEffectParam("Point1", _point1);
            _blurPassEffect.SetEffectParam("Point2", _point2);

            _zoom += _zoomVel;
            // Limit zoom
            if (_zoom < 80)
                _zoom = 80;
            if (_zoom > 2000)
                _zoom = 2000;

            _angleHorz += _angleVelHorz;
            // Wrap-around to keep _angleHorz between -PI and + PI
            _angleHorz = M.MinAngle(_angleHorz);

            _angleVert += _angleVelVert;
            // Limit pitch to the range between [-PI/2, + PI/2]
            _angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

            // Wrap-around to keep _angleRoll between -PI and + PI
            _angleRoll = M.MinAngle(_angleRoll);

            // Create the camera matrix and set it as the current View transformation
            var mtxRot = /*float4x4.CreateRotationZ(_angleRoll) **/ float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 20, -_zoom, 0, 0, 0, 0, 1, 0);
            RC.View = mtxCam * mtxRot * _sceneScale * _sceneCenter;
            var mtxOffset = float4x4.CreateTranslation(2f * _offset.x / Width, -2f * _offset.y / Height, 0);
            RC.Projection *= mtxOffset;

            // Constantly check for interactive objects.
            _sih.CheckForInteractiveObjects(Input.Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }
            // Tick any animations and Render the scene loaded in Init()
            _sceneRenderer1.Animate();
            
            _sceneRenderer2.Animate();


            // Create two Viewports
            //==> sceneRenderer for Multipass Example
            if (_multipass)
            {
                var width = Width;
                var height = Height;
                RC.Viewport(0, 0, _texRes, _texRes, false);
                _sceneRenderer1.Render(RC, _renderTex1);   //Pass 1: render the rocket to "_renderTex", using the standard material. 

                RC.Viewport(0, 0, _texRes, _texRes, false);
                _sceneRenderer2.Render(RC, _renderTex2);

                RC.Viewport(0, 0, width, height);
                _sceneRendererBlur.Render(RC);           //Pass 2: render a screen filled quad, using the "_blurPassEffect" material we defined above.
                
                _sih.View = RC.View;

                _guiRenderer.Render(RC);
            }
            else if (_viewports)
            {
                //==> sceneRenderer for multiple Viewports
                
                var aspect = Width / (float)Height;
                RC.Projection = CreatePerspectiveFieldOfViewOwn(45.0f * M.Pi / 180.0f, aspect, ZNear, ZFar);
                RC.Viewport(0, 0, Width / 2, Height);
                _sceneRenderer1.Render(RC);
                RC.Projection = CreatePerspectiveFieldOfViewOwn2(45.0f * M.Pi / 180.0f, aspect, ZNear, ZFar);
                RC.Viewport(Width / 2, 0, Width / 2, Height);
                _sceneRenderer2.Render(RC);
                
            }
            else
            {
                var aspect = Width / (float)Height;
                RC.Projection = float4x4.CreatePerspectiveFieldOfView(45.0f * M.Pi / 180.0f, aspect, ZNear, ZFar);
                RC.Viewport(0, 0, Width, Height);
                _sceneRenderer1.Render(RC);
            }

            _sih.View = RC.View;

            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }


        //////////////////////////// Viewport Functions ===> need to be reduced to one function!! 
        
        public static float4x4 CreatePerspectiveFieldOfViewOwn(float fovy, float aspect, float zNear, float zFar)
        {
            float4x4 result;

            if (fovy <= 0 || fovy > System.Math.PI)
                throw new ArgumentOutOfRangeException("fovy");
            if (aspect <= 0)
                throw new ArgumentOutOfRangeException("aspect");
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            float yMax = zNear * (float)System.Math.Tan(0.5f * fovy);
            float yMin = -yMax;
            float xMin = yMin * aspect - _viewtranslate;
            float xMax = yMax * aspect * -_viewtranslate;

            result = Math.Core.float4x4.CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar);

            return result;
        }

        public static float4x4 CreatePerspectiveFieldOfViewOwn2(float fovy, float aspect, float zNear, float zFar)
        {
            float4x4 result;

            if (fovy <= 0 || fovy > System.Math.PI)
                throw new ArgumentOutOfRangeException("fovy");
            if (aspect <= 0)
                throw new ArgumentOutOfRangeException("aspect");
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            float yMax = zNear * (float)System.Math.Tan(0.5f * fovy);
            float yMin = -yMax;
            float xMin = yMin * aspect * _viewtranslate;
            float xMax = yMax * aspect - _viewtranslate;

            result = Math.Core.float4x4.CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar);

            return result;
        }
        

        ///////////////////////////////
        private InputDevice Creator(IInputDeviceImp device)
        {
            throw new NotImplementedException();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {

        }

        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");

            var btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };

            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;
           

            var btnAB1 = new GUIButton
            {
                Name = "AB_Compare1"
            };

            btnAB1.OnMouseDown += BtnAB1Down;

            var btnAB2 = new GUIButton
            {
                Name = "AB_Compare2"
            };

            btnAB2.OnMouseDown += BtnAB2Down;
       
            var btnmultiview = new GUIButton
            {
                Name = "Multiple Viewports"
            };

            btnmultiview.OnMouseDown += BtnMultiviewDown;
            
            var btnmultipass = new GUIButton
            {
                Name = "Multipass"
            };

            btnmultipass.OnMouseDown += BtnMultipassDown;

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
                UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(0, _initCanvasHeight - 0.5f), _initCanvasHeight, _initCanvasWidth, new float2(1.75f, 0.5f))
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var ab1 = new TextureNodeContainer(
                "ab1",
                vsTex,
                psTex,
                //Set the diffuse texture you want to use.
                _bltDestinationTex,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_LEFT),
                //Define Offset and therefor the size of the element.                
                UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(_initCanvasWidth * _point1.x, _initCanvasHeight * _point1.y), _initCanvasHeight, _initCanvasWidth, new float2(0.5f, 0.5f))
                );
            ab1.AddComponent(btnAB1);
            ab1.GetComponent<ShaderEffectComponent>().Effect =  ShaderCodeBuilder.MakeShaderEffect(new float4(1, 0, 0, 1), new float4(1, 0, 1, 1), 0.5f, 0.5f);
            
            var ab2 = new TextureNodeContainer(
                "ab2",
                vsTex,
                psTex,
                //Set the diffuse texture you want to use.
                _bltDestinationTex,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_LEFT),
                //Define Offset and therefor the size of the element.                
                UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(_initCanvasWidth * _point2.x, _initCanvasHeight * _point2.y), _initCanvasHeight, _initCanvasWidth, new float2(0.5f, 0.5f))
                );
            ab2.AddComponent(btnAB2);
            ab2.GetComponent<ShaderEffectComponent>().Effect = ShaderCodeBuilder.MakeShaderEffect(new float4(0, 1, 0, 1), new float4(1, 0, 1, 1), 0.5f, 0.5f);


            // Initialize the information text line.
            var textToDisplay = "FUSEE 3D Scene";
            if (_scene1.Header.CreatedBy != null || _scene1.Header.CreationDate != null)
            {
                textToDisplay += " created";
                if (_scene1.Header.CreatedBy != null)
                    textToDisplay += " by " + _scene1.Header.CreatedBy;

                if (_scene1.Header.CreationDate != null)
                    textToDisplay += " on " + _scene1.Header.CreationDate;
            }

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 18);

            var text = new TextNodeContainer(
                textToDisplay,
                "SceneDescriptionText",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_HORIZONTAL),
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_HORIZONTAL, new float2(_initCanvasWidth / 2 - 4, 0), _initCanvasHeight, _initCanvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery), 200f);

            var textmultipass = new TextNodeContainer(
                "Multipass Example",
                "MultipassExample",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_ALL),
                new MinMaxRect
                {
                    Min = new float2(0.0f, 0.0f),
                    Max = new float2(0.0f, 0.0f)
                }, 
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery), 200f);

            var multipasscontainer = new TextureNodeContainer(
               "multipasscontainer",
               vsTex,
               psTex,
               //Set the diffuse texture you want to use.
               _bltDestinationTex,
               //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
               //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
               UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_LEFT),
               //Define Offset and therefor the size of the element.                
               UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(_initCanvasWidth * 0.01f, _initCanvasHeight * 0.7f), _initCanvasHeight, _initCanvasWidth, new float2(2.0f, 0.5f))
               )
            { Children = new ChildList() { textmultipass } };

            multipasscontainer.AddComponent(btnmultipass);
            multipasscontainer.GetComponent<ShaderEffectComponent>().Effect = ShaderCodeBuilder.MakeShaderEffect(new float4(1, 1, 1, 1), new float4(1, 1, 1, 1), 0.5f, 0.5f);

            var textmultiview = new TextNodeContainer(
                "Viewport Example",
                "ViewportExample",
                vsTex,
                psTex,
                 UIElementPosition.GetAnchors(AnchorPos.STRETCH_ALL),
                new MinMaxRect
                {
                    Min = new float2(0.0f, 0.0f),
                    Max = new float2(0.0f, 0.0f)
                }, 
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery), 200f);

            var multiviewcontainer = new TextureNodeContainer(
               "multiviewcontainer",
               vsTex,
               psTex,
               //Set the diffuse texture you want to use.
               _bltDestinationTex,
               //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
               //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
               UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_LEFT),
               //Define Offset and therefor the size of the element.                
               UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(_initCanvasWidth * 0.01f, _initCanvasHeight * 0.6f), _initCanvasHeight, _initCanvasWidth, new float2(2.0f, 0.5f))
               )
            { Children = new ChildList() { textmultiview } };

            multiviewcontainer.AddComponent(btnmultiview);
            multiviewcontainer.GetComponent<ShaderEffectComponent>().Effect = ShaderCodeBuilder.MakeShaderEffect(new float4(1, 1, 1, 1), new float4(1, 1, 1, 1), 0.5f, 0.5f);

            var canvas = new CanvasNodeContainer(
                "Canvas",
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-_canvasWidth / 2, -_canvasHeight / 2f),
                    Max = new float2(_canvasWidth / 2, _canvasHeight / 2f)
                });
            canvas.Children.Add(fuseeLogo);
            canvas.Children.Add(text);
            canvas.Children.Add(multipasscontainer);
            canvas.Children.Add(multiviewcontainer);

            if (_multipass)
            {
                canvas.Children.Add(ab1);
                canvas.Children.Add(ab2);
            }
            //Create canvas projection component and add resize delegate
            var canvasProjComp = new ProjectionComponent(ProjectionMethod.ORTHOGRAPHIC, ZNear, ZFar, _fovy);
            canvas.Components.Insert(0, canvasProjComp);
            // AddResizeDelegate(delegate { canvasProjComp.Resize(Width, Height); });

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


        public void BtnMultiviewDown(CodeComponent sender)
        {
            _viewports = true;
        }

        public void BtnMultipassDown(CodeComponent sender)
        {
            _multipass = true;

            _gui = CreateGui();
            _sih = new SceneInteractionHandler(_gui);
            _guiRenderer = new SceneRendererForward(_gui);
        }

        public void BtnAB1Down(CodeComponent sender)
        {
            _abbtn1 = true;
            _abbtn2 = false;
        }
        
        public void BtnAB2Down(CodeComponent sender)
        {
            _abbtn1 = false;
            _abbtn2 = true;
        }

    }
}