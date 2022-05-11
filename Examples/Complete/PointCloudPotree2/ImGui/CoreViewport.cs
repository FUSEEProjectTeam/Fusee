using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core.Scene;
using Fusee.PointCloud.Potree.V2;
using System.Collections.Generic;

namespace Fusee.Examples.PointCloudPotree2.PotreeImGui
{
    internal class CoreViewport : ImGuiDesktop.Templates.FuseeControlToTexture
    {
        public bool UseWPF { get; set; }
        public bool ReadyToLoadNewFile { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool IsAlive { get; private set; }

        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit;
        private static float2 _offset;
        private static float2 _offsetInit;

        private int Width;
        private int Height;

        private const float RotationSpeed = 7;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;

        private bool _twoTouchRepeated;
        private bool _keys;

        private const float ZNear = 1f;
        private const float ZFar = 1000;

        private readonly float _fovy = M.PiOver4;


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

        //private SixDOFDevice _spaceMouse;
        private PointCloudComponent _pointCloud;

        private RenderContext _rc;

        public CoreViewport(RenderContext rc) : base()
        {
            _rc = rc;
        }

        public override void Init()
        {
            //_spaceMouse = GetDevice<SixDOFDevice>();

            PtRenderingParams.Instance.DepthPassEf = MakePointCloudEffect.ForDepthPass(PtRenderingParams.Instance.Size, PtRenderingParams.Instance.PtMode, PtRenderingParams.Instance.Shape);
            PtRenderingParams.Instance.ColorPassEf = MakePointCloudEffect.ForColorPass(PtRenderingParams.Instance.Size, PtRenderingParams.Instance.ColorMode, PtRenderingParams.Instance.PtMode, PtRenderingParams.Instance.Shape, PtRenderingParams.Instance.EdlStrength, PtRenderingParams.Instance.EdlNoOfNeighbourPx);
            PtRenderingParams.Instance.PointThresholdHandler = OnThresholdChanged;
            PtRenderingParams.Instance.ProjectedSizeModifierHandler = OnProjectedSizeModifierChanged;

            IsAlive = true;

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

            var potreeReader = new Potree2Reader();
            _pointCloud = (PointCloudComponent)potreeReader.GetPointCloudComponent(PtRenderingParams.Instance.PathToOocFile);
            _pointCloud.PointCloudImp.MinProjSizeModifier = PtRenderingParams.Instance.ProjectedSizeModifier;
            _pointCloud.PointCloudImp.PointThreshold = PtRenderingParams.Instance.PointThreshold;

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

            _camTransform.Translation = _initCameraPos = _pointCloud.Center - new float3(0, 0, _pointCloud.Size.z * 2);

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


            _sceneRenderer = new SceneRendererForward(_scene);
            _sceneRenderer.VisitorModules.Add(new PointCloudRenderModule());

            IsInitialized = true;
        }

        // RenderAFrame is called once a frame
        protected override void RenderAFrame()
        {
            ReadyToLoadNewFile = false;

            if (_closingRequested)
            {
                ReadyToLoadNewFile = true;
                return;
            }


            // TOOD(mr): Idea: insert a new template class where one can specify invidividual passes
            // and set them inside RenderAFrame_Pass1, and so on.

            if (PtRenderingParams.Instance.EdlStrength != 0f)
            {
                //Render Depth-only pass
                PtRenderingParams.Instance.DepthPassEf.Active = true;
                PtRenderingParams.Instance.ColorPassEf.Active = false;

                _cam.RenderTexture = PtRenderingParams.Instance.ColorPassEf.DepthTex;
                _sceneRenderer.Render(_rc);
                _cam.RenderTexture = null;

                PtRenderingParams.Instance.DepthPassEf.Active = false;
                PtRenderingParams.Instance.ColorPassEf.Active = true;
            }

            _sceneRenderer.Render(_rc);

            ReadyToLoadNewFile = true;
        }

        public override void Update(bool allowInput)
        {
            if (!allowInput) return;

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
            if (Input.Keyboard.LeftRightAxis != 0 || Input.Keyboard.UpDownAxis != 0)
                _keys = true;


                _twoTouchRepeated = false;


            // UpDown / LeftRight rotation
            if (Input.Mouse.LeftButton)
            {
                _keys = false;

                _angleVelHorz = RotationSpeed * Input.Mouse.XVel * Time.DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * Input.Mouse.YVel * Time.DeltaTimeUpdate * 0.0005f;
            }

            else
            {
                if (_keys)
                {
                    _angleVelHorz = RotationSpeed * Input.Keyboard.LeftRightAxis;
                    _angleVelVert = RotationSpeed * Input.Keyboard.UpDownAxis;
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

                _camTransform.FpsView(_angleHorz, _angleVert, Input.Keyboard.WSAxis, Input.Keyboard.ADAxis, Time.DeltaTimeUpdate * 20);
            }
        }

        private void OnThresholdChanged(int newValue)
        {
            _pointCloud.PointCloudImp.PointThreshold = newValue;
        }

        private void OnProjectedSizeModifierChanged(float newValue)
        {
            _pointCloud.PointCloudImp.MinProjSizeModifier = newValue;
        }

        private bool SpaceMouseMoving(out float3 velPos, out float3 velRot)
        {
            //if (_spaceMouse != null && _spaceMouse.IsConnected)
            //{
            //    bool spaceMouseMovement = false;
            //    velPos = 0.001f * _spaceMouse.Translation;
            //    if (velPos.LengthSquared < 0.01f)
            //        velPos = float3.Zero;
            //    else
            //        spaceMouseMovement = true;
            //    velRot = 0.0001f * _spaceMouse.Rotation;
            //    velRot.z = 0;
            //    if (velRot.LengthSquared < 0.000005f)
            //        velRot = float3.Zero;
            //    else
            //        spaceMouseMovement = true;
            //
            //    return spaceMouseMovement;
            //}
            velPos = float3.Zero;
            velRot = float3.Zero;
            return false;
        }

        // Is called when the window was resized
        protected override void Resize(int width, int height)
        {
            Width = width;
            Height = height;

            // FIXME (later) (mr)
            // This is necessary as the camera uses the defaultState, for reseting original size, too
            // see: SceneRendererForward:297
            if (_rc.DefaultState != null)
            {
                _rc.DefaultState.CanvasWidth = Width;
                _rc.DefaultState.CanvasHeight = Height;
            }

            if (PtRenderingParams.Instance.EdlStrength == 0f) return;
            PtRenderingParams.Instance.ColorPassEf.DepthTex = WritableTexture.CreateDepthTex(width, height, new ImagePixelFormat(ColorFormat.Depth24));
        }

        //public override void DeInit()
        //{
        //    base.DeInit();
        //    IsAlive = false;
        //}

        public void ResetCamera()
        {
            _camTransform.Translation = _initCameraPos;
            _angleHorz = _angleVert = 0;
            _camTransform.FpsView(_angleHorz, _angleVert, Input.Keyboard.WSAxis, Input.Keyboard.ADAxis, Time.DeltaTimeUpdate * 20);
        }
    }

}

