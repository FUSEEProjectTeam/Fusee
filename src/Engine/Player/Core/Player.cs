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
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert, _zoomVel;
        private static float2 _offset;
        private static float2 _offsetInit;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private bool _twoTouchRepeated;

        private bool _keys;

        private readonly Camera _mainCam = new(ProjectionMethod.Perspective, 1, 1000, M.PiOver4)
        {
            BackgroundColor = float4.One
        };
        private Transform _mainCamTransform;
        private Transform _mainCamPivot;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.Screen;

        private float _maxPinchSpeed;

        private float _maxSceneScale;

        public async Task LoadAssets()
        {
            // Load the standard model
            _scene = await AssetStorage.GetAsync<SceneContainer>(ModelFile);
            _gui = await FuseeGuiHelper.CreateDefaultGuiAsync(this, _canvasRenderMode, "FUSEE Player");


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
                float3 sceneCenter = float3.Zero;
                if (System.Math.Abs(bbCenter.x) > bbSize.x * 0.3)
                    sceneCenter.x = bbCenter.x;
                if (System.Math.Abs(bbCenter.y) > bbSize.y * 0.3)
                    sceneCenter.y = bbCenter.y;
                if (System.Math.Abs(bbCenter.z) > bbSize.z * 0.3)
                    sceneCenter.z = bbCenter.z;

                // Adjust the model size
                _maxSceneScale = System.Math.Max(bbSize.x, System.Math.Max(bbSize.y, bbSize.z));

                _mainCamTransform = new Transform()
                {
                    Translation = new float3(0, 0, -_maxSceneScale * 2)
                };
                _mainCamPivot = new Transform()
                {
                    Translation = sceneCenter
                };
                var camNode = new SceneNode()
                {
                    Name = "CamPivoteNode",
                    Children = new ChildList()
                    {
                        new SceneNode()
                        {
                            Name = "MainCam",
                            Components = new List<SceneComponent>()
                            {
                                _mainCamTransform,
                                _mainCam
                            }
                        }
                    },
                    Components = new List<SceneComponent>()
                    {
                        _mainCamPivot
                    }
                };

                _scene.Children.Add(camNode);
            }

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _guiRenderer = new SceneRendererForward(_gui);

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui, _guiRenderer.PrePassVisitor.CameraPrepassResults);
        }

        public override async Task InitAsync()
        {
            await LoadAssets();
            await base.InitAsync();
        }

        // Init is called on startup.
        public override void Init()
        {
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
                    _offsetInit = Touch.TwoPointMidPoint - _offset;
                    _maxPinchSpeed = 0;
                }
                _zoomVel = Touch.TwoPointDistanceVel * -0.01f;
                _offset = Touch.TwoPointMidPoint - _offsetInit;
                float pinchSpeed = Touch.TwoPointDistanceVel;
                if (pinchSpeed > _maxPinchSpeed) _maxPinchSpeed = pinchSpeed; // _maxPinchSpeed is used for debugging only.
            }
            else
            {
                _twoTouchRepeated = false;
                _zoomVel = Mouse.WheelVel * -0.01f;
                _offset *= curDamp * 0.8f;
            }

            // UpDown / LeftRight rotation
            if (Mouse.LeftButton)
            {

                _keys = false;
                _angleVelHorz = RotationSpeed * Mouse.XVel * DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * Mouse.YVel * DeltaTimeUpdate * 0.0005f;
            }

            else if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _keys = false;
                float2 touchVel;
                touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = RotationSpeed * touchVel.x * DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * touchVel.y * DeltaTimeUpdate * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = RotationSpeed * Keyboard.LeftRightAxis * DeltaTimeUpdate;
                    _angleVelVert = RotationSpeed * Keyboard.UpDownAxis * DeltaTimeUpdate;
                }
                else
                {
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            // Wrap-around to keep _angleHorz between -PI and + PI
            _angleHorz = M.MinAngle(_angleHorz);

            _angleVert += _angleVelVert;
            // Limit pitch to the range between [-PI/2, + PI/2]
            _angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            var currentTransl = _mainCamTransform.Translation + new float3(0, 0, _zoomVel);
            //Limit zoom
            if (currentTransl.z < -_maxSceneScale * 10)
                currentTransl.z = -_maxSceneScale * 10;
            if (currentTransl.z > -_maxSceneScale)
                currentTransl.z = -_maxSceneScale;
            _mainCamTransform.Translation = currentTransl;

            _mainCamPivot.RotationQuaternion = QuaternionF.FromEuler(_angleVert, _angleHorz, 0);

            _sceneRenderer.Animate();
            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

            // Constantly check for interactive objects.
            _sih.CheckForInteractiveObjects(Mouse.Position, Width, Height);

            if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}