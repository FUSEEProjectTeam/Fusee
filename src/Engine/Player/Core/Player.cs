using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Fusee.Engine.GUI;
using Fusee.Xene;
using System.Threading.Tasks;

namespace Fusee.Engine.Player.Core
{
    [FuseeApplication(Name = "FUSEE Player", Description = "Watch any FUSEE scene.")]
    public class Player : RenderCanvas
    {
        public string ModelFile = "Model.fus";

        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit, _zoomVel, _zoom;
        private static float2 _offset;
        private static float2 _offsetInit;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
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

        // Init is called on startup. 
        public override async Task<bool> Init()
        {
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
            _scene = await AssetStorage.GetAsync<SceneContainer>(ModelFile);
            
            _gui = await CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);
           
            // Register the input devices that are not already given.
            _gamePad = GetDevice<GamePadDevice>(0);

            AABBCalculator aabbc = new AABBCalculator(_scene);
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
            _sceneRenderer = new SceneRendererForward(_scene);
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
            if (Mouse.LeftButton) {

                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }

            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _keys = false;
                float2 touchVel;
                touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
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
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

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
            _sceneRenderer.Animate();
            _sceneRenderer.Render(RC);            
            
            _sih.View = RC.View;

            _guiRenderer.Render(RC);            

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private InputDevice Creator(IInputDeviceImp device)
        {
            throw new NotImplementedException();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
            
        }

        private async Task<SceneContainer> CreateGui()
        {
            var vsTex = await AssetStorage.GetAsync<string>("texture.vert");
            var psTex = await AssetStorage.GetAsync<string>("texture.frag");

            var btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var guiFuseeLogo = new Texture(await AssetStorage.GetAsync<ImageData>("FuseeText.png"));
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

            // Initialize the information text line.
            var textToDisplay = "FUSEE 3D Scene";
            if (_scene.Header.CreatedBy != null || _scene.Header.CreationDate != null)
            {
                textToDisplay += " created";
                if (_scene.Header.CreatedBy != null)
                    textToDisplay += " by " + _scene.Header.CreatedBy;

                if (_scene.Header.CreationDate != null)
                    textToDisplay += " on " + _scene.Header.CreationDate;
            }

            var fontLato = await AssetStorage.GetAsync<Font>("Lato-Black.ttf");
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

            //Create canvas projection component and add resize delegate
            var canvasProjComp = new ProjectionComponent(ProjectionMethod.ORTHOGRAPHIC, ZNear, ZFar, _fovy);
            canvas.Components.Insert(0, canvasProjComp);
            
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