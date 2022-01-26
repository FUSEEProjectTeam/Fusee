using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using System;
using System.Collections.Generic;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.PointCloudPotree2.Core
{
    [FuseeApplication(Name = "FUSEE Point Cloud Viewer")]
    public class PointCloudOutOfCore : RenderCanvas, IPointCloudOutOfCore
    {
        public bool UseWPF { get; set; }
        public bool DoShowOctants { get; set; }
        public bool ReadyToLoadNewFile { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool IsAlive { get; private set; }

        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit;
        private static float2 _offset;
        private static float2 _offsetInit;

        private const float RotationSpeed = 7;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;

        private bool _twoTouchRepeated;
        private bool _keys;

        private const float ZNear = 1f;
        private const float ZFar = 1000;

        private readonly float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;

        private float _maxPinchSpeed;

        public bool ClosingRequested
        {
            get => _closingRequested;
            set => _closingRequested = value;
        }
        private bool _closingRequested;

        private Transform _camTransform;
        private Camera _cam;
        private float3 _initCameraPos;

        private SixDOFDevice _spaceMouse;
        private Engine.Core.Scene.PointCloud _pointCloud;

        public override void Init()
        {
            VSync = false;
            _spaceMouse = GetDevice<SixDOFDevice>();

            PtRenderingParams.Instance.DepthPassEf = PtRenderingParams.Instance.CreateDepthPassEffect();
            PtRenderingParams.Instance.ColorPassEf = PtRenderingParams.Instance.CreateColorPassEffect();
            PtRenderingParams.Instance.PointThresholdHandler = OnThresholdChanged;
            PtRenderingParams.Instance.ProjectedSizeModifierHandler = OnProjectedSizeModifierChanged;

            IsAlive = true;

            ApplicationIsShuttingDown += (object sender, EventArgs e) =>
            {

            };

            _camTransform = new Transform()
            {
                Name = "MainCamTransform",
                Scale = float3.One,
                Translation = float3.Zero,
                Rotation = float3.Zero
            };

            _cam = new Camera(ProjectionMethod.Perspective, ZNear, ZFar, _fovy)
            {
                BackgroundColor = float4.One
            };

            var mainCam = new SceneNode()
            {
                Name = "MainCam",
                Components = new List<SceneComponent>()
                {
                    _camTransform,
                    _cam
                }
            };

            _pointCloud = new Engine.Core.Scene.PointCloud(PtRenderingParams.Instance.PathToOocFile, PointCloudFileType.Potree2);

            ((Potree2Cloud)_pointCloud.PointCloudImp).MinProjSizeModifier = PtRenderingParams.Instance.ProjectedSizeModifier;
            ((Potree2Cloud)_pointCloud.PointCloudImp).PointThreshold = PtRenderingParams.Instance.PointThreshold;

            var pointCloudNode = new SceneNode()
            {
                Name = "PointCloud",
                Components = new List<SceneComponent>()
                {
                    new Transform()
                    {
                        Scale = float3.One,
                        Translation = float3.Zero,
                        Rotation = float3.Zero
                    },
                    PtRenderingParams.Instance.DepthPassEf,
                    PtRenderingParams.Instance.ColorPassEf,
                    _pointCloud
                }
            };

            _camTransform.Translation = _initCameraPos = _pointCloud.Center - new float3(_pointCloud.Size.x, _pointCloud.Size.y, _pointCloud.Size.z * 2);

            _scene = new SceneContainer
            {
                Children = new List<SceneNode>()
                {
                    mainCam,
                    pointCloudNode
                }
            };

            _angleRoll = 0;
            _angleRollInit = 0;
            _twoTouchRepeated = false;
            _offset = float2.Zero;
            _offsetInit = float2.Zero;

            _gui = FuseeGuiHelper.CreateDefaultGui(this, CanvasRenderMode.Screen, "FUSEE Out-Of-Core Point Cloud Rendering");
            _sih = new SceneInteractionHandler(_gui);

            _sceneRenderer = new SceneRendererForward(_scene);
            _guiRenderer = new SceneRendererForward(_gui);

            IsInitialized = true;
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            Diagnostics.Warn(FramesPerSecond);
            ReadyToLoadNewFile = false;

            if (_closingRequested)
            {
                ReadyToLoadNewFile = true;
                return;
            }

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            if (PtRenderingParams.Instance.EdlStrength != 0f)
            {
                //Render Depth-only pass
                PtRenderingParams.Instance.DepthPassEf.Active = true;
                PtRenderingParams.Instance.ColorPassEf.Active = false;

                _cam.RenderTexture = PtRenderingParams.Instance.ColorPassEf.DepthTex;
                _sceneRenderer.Render(RC);
                _cam.RenderTexture = null;

                PtRenderingParams.Instance.DepthPassEf.Active = false;
                PtRenderingParams.Instance.ColorPassEf.Active = true;
            }

            _sceneRenderer.Render(RC);

            //Render GUI
            RC.Projection = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);
            // Constantly check for interactive objects.
            if (Mouse != null) //Mouse is null when the pointer is outside the GameWindow?
            {
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);

                if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
                {
                    _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
                }
            }
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();

            ReadyToLoadNewFile = true;
        }

        public override void Update()
        {
            if (_closingRequested)
            {
                ReadyToLoadNewFile = true;
                return;
            }

            var isSpaceMouseMoving = SpaceMouseMoving(out float3 velPos, out float3 velRot);

            // ------------ Enable to update the Scene only when the user isn't moving ------------------
            /*if (Keyboard.WSAxis != 0 || Keyboard.ADAxis != 0 || (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint) || isSpaceMouseMoving)
                OocLoader.IsUserMoving = true;
            else
                OocLoader.IsUserMoving = false;*/
            //--------------------------------------------------------------------------------------------

            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
                _keys = true;

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

                _angleRoll = Touch.TwoPointAngle - _angleRollInit;
                _offset = Touch.TwoPointMidPoint - _offsetInit;
                float pinchSpeed = Touch.TwoPointDistanceVel;
                if (pinchSpeed > _maxPinchSpeed) _maxPinchSpeed = pinchSpeed;
            }
            else
            {
                _twoTouchRepeated = false;
            }

            // UpDown / LeftRight rotation
            if (Mouse.LeftButton)
            {
                _keys = false;

                _angleVelHorz = RotationSpeed * Mouse.XVel * DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * Mouse.YVel * DeltaTimeUpdate * 0.0005f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
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
                    _angleVelHorz = RotationSpeed * Keyboard.LeftRightAxis;
                    _angleVelVert = RotationSpeed * Keyboard.UpDownAxis;
                }
            }

            if (isSpaceMouseMoving)
            {
                _angleHorz -= velRot.y;
                _angleVert -= velRot.x;

                float speed = 12;

                _camTransform.FpsView(_angleHorz, _angleVert, velPos.z, velPos.x, speed);
                _camTransform.Translation += new float3(0, velPos.y * speed, 0);
            }
            else
            {
                _angleHorz += _angleVelHorz;
                _angleVert += _angleVelVert;
                _angleVelHorz = 0;
                _angleVelVert = 0;

                _camTransform.FpsView(_angleHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTimeUpdate * 20);
            }
        }

        private void OnThresholdChanged(int newValue)
        {
            ((Potree2Cloud)_pointCloud.PointCloudImp).PointThreshold = newValue;
        }

        private void OnProjectedSizeModifierChanged(float newValue)
        {
            ((Potree2Cloud)_pointCloud.PointCloudImp).MinProjSizeModifier = newValue;
        }

        private bool SpaceMouseMoving(out float3 velPos, out float3 velRot)
        {
            if (_spaceMouse != null && _spaceMouse.IsConnected)
            {
                bool spaceMouseMovement = false;
                velPos = 0.001f * _spaceMouse.Translation;
                if (velPos.LengthSquared < 0.01f)
                    velPos = float3.Zero;
                else
                    spaceMouseMovement = true;
                velRot = 0.0001f * _spaceMouse.Rotation;
                velRot.z = 0;
                if (velRot.LengthSquared < 0.000005f)
                    velRot = float3.Zero;
                else
                    spaceMouseMovement = true;

                return spaceMouseMovement;
            }
            velPos = float3.Zero;
            velRot = float3.Zero;
            return false;
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
            if (PtRenderingParams.Instance.EdlStrength == 0f) return;
            PtRenderingParams.Instance.ColorPassEf.DepthTex = WritableTexture.CreateDepthTex(Width, Height, new ImagePixelFormat(ColorFormat.Depth24));
        }

        public override void DeInit()
        {
            base.DeInit();
            IsAlive = false;
        }

        public void ResetCamera()
        {
            _camTransform.Translation = _initCameraPos;
            _angleHorz = _angleVert = 0;
            _camTransform.FpsView(_angleHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTimeUpdate * 20);
        }
    }
}