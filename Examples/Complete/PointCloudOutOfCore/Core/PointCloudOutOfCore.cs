using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using System;
using System.Collections.Generic;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.PointCloudOutOfCore.Core
{
    [FuseeApplication(Name = "FUSEE Point Cloud Viewer")]
    public class PointCloudOutOfCore<TPoint> : RenderCanvas, IPcRendering where TPoint : new()
    {
        public PointCloudOutOfCore(IPtOctantLoader oocLoader, IPtOctreeFileReader oocFileReader)
        {
            OocLoader = oocLoader;
            OocFileReader = oocFileReader;
        }
        public IPtOctantLoader OocLoader { get; }
        public IPtOctreeFileReader OocFileReader { get; }
        public bool UseWPF { get; set; }
        public bool DoShowOctants { get; set; }
        public bool IsSceneLoaded { get; private set; }
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

        private float3 _initCamPos;
        public float3 InitCameraPos
        {
            get => _initCamPos;
            private set
            {
                _initCamPos = value;
                OocLoader.InitCamPos = new double3(_initCamPos.x, _initCamPos.y, _initCamPos.z);
            }
        }

        public bool ClosingRequested
        {
            get
            {
                return _closingRequested;
            }
            set
            {
                _closingRequested = value;
            }
        }
        private bool _closingRequested;

        private bool _isTexInitialized;

        private Transform _camTransform;
        private Camera _cam;

        private SixDOFDevice _spaceMouse;

        public override void Init()
        {
            VSync = false;
            _spaceMouse = GetDevice<SixDOFDevice>();

            PtRenderingParams.Instance.DepthPassEf = PtRenderingParams.Instance.CreateDepthPassEffect();
            PtRenderingParams.Instance.ColorPassEf = PtRenderingParams.Instance.CreateColorPassEffect();

            OocLoader.Init(RC);

            IsAlive = true;

            ApplicationIsShuttingDown += (object sender, EventArgs e) =>
            {
                OocLoader.IsShuttingDown = true;
            };

            _scene = new SceneContainer
            {
                Children = new List<SceneNode>()
            };

            _camTransform = new Transform()
            {
                Name = "MainCamTransform",
                Scale = float3.One,
                Translation = InitCameraPos,
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

            _scene.Children.Insert(0, mainCam);

            _angleRoll = 0;
            _angleRollInit = 0;
            _twoTouchRepeated = false;
            _offset = float2.Zero;
            _offsetInit = float2.Zero;

            if (!UseWPF)
                LoadPointCloudFromFile();

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

            if (IsSceneLoaded)
            {
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

                    _angleVelHorz = RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                    _angleVelVert = RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
                }
                else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
                {
                    _keys = false;
                    float2 touchVel;
                    touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                    _angleVelHorz = RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                    _angleVelVert = RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
                }
                else
                {
                    if (_keys)
                    {
                        _angleVelHorz = RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;
                        _angleVelVert = RotationSpeed * Keyboard.UpDownAxis * DeltaTime;
                    }
                }

                if (isSpaceMouseMoving)
                {
                    _angleHorz -= velRot.y;
                    _angleVert -= velRot.x;

                    float speed = DeltaTime * 12;

                    _camTransform.FpsView(_angleHorz, _angleVert, velPos.z, velPos.x, speed);
                    _camTransform.Translation += new float3(0, velPos.y * speed, 0);
                }
                else
                {
                    _angleHorz += _angleVelHorz;
                    _angleVert += _angleVelVert;
                    _angleVelHorz = 0;
                    _angleVelVert = 0;

                    if (HasUserMoved() || _camTransform.Translation == InitCameraPos)
                        _camTransform.FpsView(_angleHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTime * 20);
                }

                //----------------------------  

                if (PtRenderingParams.Instance.Lighting != Lighting.Unlit)
                {
                    //Render Depth-only pass

                    _scene.Children[1].RemoveComponent<PointCloudSurfaceEffect>();
                    _scene.Children[1].Components.Insert(0, PtRenderingParams.Instance.DepthPassEf);
                    _cam.RenderTexture = PtRenderingParams.Instance.ColorPassEf.DepthTex;

                    _sceneRenderer.Render(RC);
                    _cam.RenderTexture = null;

                    _scene.Children[1].RemoveComponent<ShaderEffect>();
                    _scene.Children[1].Components.Insert(0, PtRenderingParams.Instance.ColorPassEf);
                }

                _sceneRenderer.Render(RC);

                //UpdateScene after Render / Traverse because there we calculate the view matrix (when using a camera) we need for the update.
                OocLoader.UpdateScene();

                if (DoShowOctants)
                    OocLoader.ShowOctants(_scene);
            }

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
            if (PtRenderingParams.Instance.Lighting == Lighting.Unlit) return;
            PtRenderingParams.Instance.ColorPassEf.DepthTex = WritableTexture.CreateDepthTex(Width, Height, new ImagePixelFormat(ColorFormat.Depth24));

        }

        public override void DeInit()
        {
            base.DeInit();
            IsAlive = false;
        }

        private bool HasUserMoved()
        {
            return RC.View == float4x4.Identity
                || Mouse.LeftButton
                || Keyboard.WSAxis != 0 || Keyboard.ADAxis != 0
                || (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint);
        }

        public RenderContext GetRc()
        {
            return RC;
        }

        public SceneNode GetOocLoaderRootNode()
        {
            return OocLoader.RootNode;
        }

        public bool GetOocLoaderWasSceneUpdated()
        {
            return OocLoader.WasSceneUpdated;
        }

        public int GetOocLoaderPointThreshold()
        {
            return OocLoader.PointThreshold;
        }

        public void SetOocLoaderPointThreshold(int value)
        {
            OocLoader.PointThreshold = value;
        }

        public void SetOocLoaderMinProjSizeMod(float value)
        {
            OocLoader.MinProjSizeModifier = value;
        }

        public float GetOocLoaderMinProjSizeMod()
        {
            return OocLoader.MinProjSizeModifier;
        }

        public void LoadPointCloudFromFile()
        {
            //create Scene from octree structure
            var root = OocFileReader.GetScene();

            var ptOctantComp = root.GetComponent<OctantD>();
            InitCameraPos = _camTransform.Translation = new float3((float)ptOctantComp.Center.x, (float)ptOctantComp.Center.y, (float)(ptOctantComp.Center.z - (ptOctantComp.Size * 2f)));

            _scene.Children.Add(root);

            OocLoader.RootNode = root;
            OocLoader.FileFolderPath = PtRenderingParams.Instance.PathToOocFile;

            IsSceneLoaded = true;
        }

        public void DeletePointCloud()
        {
            IsSceneLoaded = false;

            while (!OocLoader.WasSceneUpdated || !ReadyToLoadNewFile)
            {
                continue;
            }

            if (OocLoader.RootNode != null)
                _scene.Children.Remove(OocLoader.RootNode);
        }

        public void ResetCamera()
        {
            _camTransform.Translation = InitCameraPos;
            _angleHorz = _angleVert = 0;
        }

        public void DeleteOctants()
        {
            IsSceneLoaded = false;

            while (!OocLoader.WasSceneUpdated || !ReadyToLoadNewFile)
            {
                continue;
            }

            DoShowOctants = false;
            OocLoader.DeleteWireframeOctants(_scene);
            IsSceneLoaded = true;
        }
    }
}