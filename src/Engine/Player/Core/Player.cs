using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

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
        private float3 _sceneCenter;
        private float _sceneScale;
        private bool _twoTouchRepeated;

        private bool _keys;
        
        private Camera _mainCam = new(ProjectionMethod.Perspective, 1, 1000, M.PiOver4)
        {
            BackgroundColor = float4.One
        };
        private Transform _mainCamTransform;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.Screen;

        private float _maxPinchSpeed;

        public async Task LoadAssets()
        {
            // Load the standard model
            _scene = await AssetStorage.GetAsync<SceneContainer>(ModelFile);
            _gui = await FuseeGuiHelper.CreateDefaultGuiAsync(this, _canvasRenderMode, "FUSEE Player");

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            AABBCalculator aabbc = new(_scene);
            var bbox = aabbc.GetBox();
            if (bbox != null)
            {
                // If the model origin is more than one third away from its bounding box,
                // recenter it to the bounding box. Do this check individually per dimension.
                // This way, small deviations will keep the model's original center, while big deviations
                // will make the model rotate around its geometric center.
                float3 bbCenter = bbox.Value.Center;
                float3 bbSize = bbox.Value.Size;
                float3 _sceneCenter = float3.Zero;
                if (System.Math.Abs(bbCenter.x) > bbSize.x * 0.3)
                    _sceneCenter.x = bbCenter.x;
                if (System.Math.Abs(bbCenter.y) > bbSize.y * 0.3)
                    _sceneCenter.y = bbCenter.y;
                if (System.Math.Abs(bbCenter.z) > bbSize.z * 0.3)
                    _sceneCenter.z = bbCenter.z;
                _sceneCenter *= -1;

                // Adjust the model size
                float maxScale = System.Math.Max(bbSize.x, System.Math.Max(bbSize.y, bbSize.z));
                if (maxScale != 0)
                    _sceneScale = 200.0f / maxScale;
                else
                    _sceneScale = 1;

                _mainCamTransform = new Transform()
                {
                    Translation = new float3(0, 0, -_zoom) - _sceneCenter,
                    Rotation = new float3(0, 0, 0),
                    Scale = new float3(1)
                };
                var camNode = new SceneNode()
                {
                    Name = "MainCam",
                    Components = new List<SceneComponent>()
                    {
                        //determines the View Matrix
                        _mainCamTransform,
                        _mainCam
                    }
                };

                _scene.Children.Add(camNode);
            }

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _guiRenderer = new SceneRendererForward(_gui);
        }

        public override async Task InitAsync()
        {
            await LoadAssets();
            await base.InitAsync();
        }

        // Init is called on startup.
        public override void Init()
        {
            // Initial "Zoom" value (it's rather the distance in view direction, not the camera's focal distance/opening angle)
            _zoom = 10;

            _angleRoll = 0;
            _angleRollInit = 0;
            _twoTouchRepeated = false;
            _offset = float2.Zero;
            _offsetInit = float2.Zero;
        }

        public override void Update()
        {
            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            var curDamp = (float)System.Math.Exp(-Damping * DeltaTimeUpdate);
            // Zoom & Roll
            if (Touch != null && Touch.TwoPoint)
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
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }

            else if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
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
            if (_zoom < 1)
                _zoom = 1;
            if (_zoom > 100)
                _zoom = 100;

            _angleHorz += _angleVelHorz;
            // Wrap-around to keep _angleHorz between -PI and + PI
            _angleHorz = M.MinAngle(_angleHorz);

            _angleVert += _angleVelVert;
            // Limit pitch to the range between [-PI/2, + PI/2]
            _angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

            // Wrap-around to keep _angleRoll between -PI and + PI
            _angleRoll = M.MinAngle(_angleRoll);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            Diagnostics.Debug(FramesPerSecond);
            VSync = false;
            //_mainCamTransform.Translation = new float3(0, 0, -_zoom) + _sceneCenter + new float3(2f * _offset.x / Width, -2f * _offset.y / Height, 0);
            //_mainCamTransform.Scale = new float3(_sceneScale);
            //_mainCamTransform.Rotation = new float3(_angleVert, _angleHorz, 0);

            
            _mainCamTransform.RotateAround(_sceneCenter, new float3(_angleVelVert, _angleVelHorz, 0));

            _sceneRenderer.Animate();
            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

            // Constantly check for interactive objects.
            _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);

            if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}