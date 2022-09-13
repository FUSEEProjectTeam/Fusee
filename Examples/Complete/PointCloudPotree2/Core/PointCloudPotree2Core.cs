using Fusee.Base.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core.Scene;
using Fusee.PointCloud.Potree.V2;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fusee.Examples.PointCloudPotree2.Core
{
    public class PointCloudPotree2Core
    {
        public bool RenderToTexture { get; set; }

        public WritableTexture RenderTexture { get; private set; }

        public bool ClosingRequested
        {
            get { return _closingRequested; }
            set { _closingRequested = value; }
        }
        private bool _closingRequested;

        public RenderMode PointRenderMode = RenderMode.StaticMesh;
        public string AssetsPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public bool RequestedNewFile = false;

        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;
        private const float RotationSpeed = 7;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;

        private bool _keys;

        private const float ZNear = 1f;
        private const float ZFar = 1000;

        private readonly float _fovy = M.PiOver4;

        private Transform _camTransform;
        private Camera _cam;
        private float3 _initCameraPos;

        private PointCloudComponent _pointCloud;
        private SceneNode _pointCloudNode;
        private SceneNode _camNode;
        private readonly Potree2Reader _potreeReader;

        private readonly RenderContext _rc;

        public void OnLoadNewFile(object sender, EventArgs e)
        {
            if (!RequestedNewFile) return;

            _pointCloud = (PointCloudComponent)_potreeReader.GetPointCloudComponent(Path.Combine(AssetsPath, PointRenderingParams.Instance.PathToOocFile), PointRenderMode);
            _pointCloud.PointCloudImp.MinProjSizeModifier = PointRenderingParams.Instance.ProjectedSizeModifier;
            _pointCloud.PointCloudImp.PointThreshold = PointRenderingParams.Instance.PointThreshold;
            _pointCloud.Camera = _cam;

            _pointCloudNode.Components[3] = _pointCloud;

            RequestedNewFile = false;
        }

        public PointCloudPotree2Core(RenderContext rc)
        {
            _potreeReader = new Potree2Reader();
            _rc = rc;
        }

        public void Init()
        {
            switch (PointRenderMode)
            {
                default:
                case RenderMode.DynamicMesh:
                case RenderMode.StaticMesh:
                    PointRenderingParams.Instance.DepthPassEf = MakePointCloudEffect.ForDepthPass(PointRenderingParams.Instance.Size, PointRenderingParams.Instance.PtMode, PointRenderingParams.Instance.Shape);
                    PointRenderingParams.Instance.ColorPassEf = MakePointCloudEffect.ForColorPass(PointRenderingParams.Instance.Size, PointRenderingParams.Instance.ColorMode, PointRenderingParams.Instance.PtMode, PointRenderingParams.Instance.Shape, PointRenderingParams.Instance.EdlStrength, PointRenderingParams.Instance.EdlNoOfNeighbourPx);

                    break;
                case RenderMode.Instanced:
                    PointRenderingParams.Instance.DepthPassEf = MakePointCloudEffect.ForDepthPassInstanced(PointRenderingParams.Instance.Size, PointRenderingParams.Instance.PtMode, PointRenderingParams.Instance.Shape);
                    PointRenderingParams.Instance.ColorPassEf = MakePointCloudEffect.ForColorPassInstanced(PointRenderingParams.Instance.Size, PointRenderingParams.Instance.ColorMode, PointRenderingParams.Instance.PtMode, PointRenderingParams.Instance.Shape, PointRenderingParams.Instance.EdlStrength, PointRenderingParams.Instance.EdlNoOfNeighbourPx);
                    break;
            }
            PointRenderingParams.Instance.PointThresholdHandler = OnThresholdChanged;
            PointRenderingParams.Instance.ProjectedSizeModifierHandler = OnProjectedSizeModifierChanged;

            _pointCloud = (PointCloudComponent)_potreeReader.GetPointCloudComponent(Path.Combine(AssetsPath, PointRenderingParams.Instance.PathToOocFile), PointRenderMode);
            _pointCloud.PointCloudImp.MinProjSizeModifier = PointRenderingParams.Instance.ProjectedSizeModifier;
            _pointCloud.PointCloudImp.PointThreshold = PointRenderingParams.Instance.PointThreshold;

            _camTransform = new Transform()
            {
                Name = "MainCamTransform",
                Scale = float3.One,
                Translation = float3.Zero,
                Rotation = float3.Zero
            };

            _cam = new(ProjectionMethod.Perspective, ZNear, ZFar, _fovy)
            {
                BackgroundColor = float4.One,
                RenderTexture = RenderTexture
            };

            _camNode = new SceneNode()
            {
                Name = "MainCam",
                Components = new List<SceneComponent>()
                    {
                        _camTransform,
                        _cam
                    }
            };

            _pointCloudNode = new SceneNode()
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
                    PointRenderingParams.Instance.DepthPassEf,
                    PointRenderingParams.Instance.ColorPassEf,
                    _pointCloud
                }
            };

            _camTransform.Translation = _initCameraPos = _pointCloud.Center - new float3(0, 0, _pointCloud.Size.z * 2);

            _scene = new SceneContainer
            {
                Children = new List<SceneNode>()
                {
                    _camNode,
                    _pointCloudNode
                }
            };

            _sceneRenderer = new SceneRendererForward(_scene);
            _sceneRenderer.VisitorModules.Add(new PointCloudRenderModule(_sceneRenderer.GetType() == typeof(SceneRendererForward)));

            _pointCloud.Camera = _cam;
        }

        // RenderAFrame is called once a frame
        public void RenderAFrame()
        {
            if (_closingRequested)
            {
                return;
            }

            //Render Depth-only pass
            PointRenderingParams.Instance.DepthPassEf.Active = true;
            PointRenderingParams.Instance.ColorPassEf.Active = false;

            _cam.RenderTexture = PointRenderingParams.Instance.ColorPassEf.DepthTex;

            _sceneRenderer.Render(_rc);
            if (RenderToTexture)
                _cam.RenderTexture = RenderTexture;
            else
                _cam.RenderTexture = null;

            PointRenderingParams.Instance.DepthPassEf.Active = false;
            PointRenderingParams.Instance.ColorPassEf.Active = true;

            _sceneRenderer.Render(_rc);
        }

        public void Update(bool allowInput)
        {
            if (!allowInput) return;

            if (_closingRequested)
                return;

            // ------------ Enable to update the Scene only when the user isn't moving ------------------
            /*if (Keyboard.WSAxis != 0 || Keyboard.ADAxis != 0 || (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint) || isSpaceMouseMoving)
                OocLoader.IsUserMoving = true;
            else
                OocLoader.IsUserMoving = false;*/
            //--------------------------------------------------------------------------------------------

            // Mouse and keyboard movement
            if (Input.Keyboard.LeftRightAxis != 0 || Input.Keyboard.UpDownAxis != 0)
                _keys = true;

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

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;
            _angleVelHorz = 0;
            _angleVelVert = 0;

            _camTransform.FpsView(_angleHorz, _angleVert, Input.Keyboard.WSAxis, Input.Keyboard.ADAxis, Time.DeltaTimeUpdate * 20);
        }

        private void OnThresholdChanged(int newValue)
        {
            if (_pointCloud != null)
                _pointCloud.PointCloudImp.PointThreshold = newValue;
        }

        private void OnProjectedSizeModifierChanged(float newValue)
        {
            if (_pointCloud != null)
                _pointCloud.PointCloudImp.MinProjSizeModifier = newValue;
        }

        // Is called when the window was resized
        public void Resize(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;

            // delete old texture, generate new
            RenderTexture?.Dispose();
            // RenderTexture = WritableMultisampleTexture.CreateAlbedoTex(_rc, width, height, 8);
            RenderTexture = WritableTexture.CreateAlbedoTex(width, height, new ImagePixelFormat(ColorFormat.RGBA));

            if (PointRenderingParams.Instance.EdlStrength == 0f) return;
            PointRenderingParams.Instance.ColorPassEf.DepthTex?.Dispose();
            PointRenderingParams.Instance.ColorPassEf.DepthTex = WritableTexture.CreateDepthTex(width, height, new ImagePixelFormat(ColorFormat.Depth24));
        }

        public void ResetCamera()
        {
            _camTransform.Translation = _initCameraPos;
            _angleHorz = _angleVert = 0;
            _camTransform.FpsView(_angleHorz, _angleVert, Input.Keyboard.WSAxis, Input.Keyboard.ADAxis, Time.DeltaTimeUpdate * 20);
        }
    }
}