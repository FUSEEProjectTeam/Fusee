using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.ImGuiImp.Desktop.Templates;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core.Scene;
using Fusee.PointCloud.Potree.V2;
using System;
using System.Collections.Generic;

namespace Fusee.Examples.PointCloudPotree2.PotreeImGui
{
    internal class PointCloudControlCore : FuseeControlToTexture, IDisposable
    {
        public bool UseWPF { get; set; }
        public bool ReadyToLoadNewFile { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool IsAlive { get; private set; }

        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;
        private const float RotationSpeed = 7;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;

        private bool _keys;

        private const float ZNear = 1f;
        private const float ZFar = 1000;

        private readonly float _fovy = M.PiOver4;

        public bool ClosingRequested
        {
            get => _closingRequested;
            set => _closingRequested = value;
        }
        private bool _closingRequested;

        private Transform _camTransform;
        private Camera _cam;
        private float3 _initCameraPos;

        private PointCloudComponent _pointCloud;

        public PointCloudControlCore(RenderContext rc) : base(rc)
        {

        }

        public override void Init()
        {
            try
            {
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

                _cam.RenderTexture = RenderTexture;

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
                _pointCloud = (PointCloudComponent)potreeReader.GetPointCloudComponent(PtRenderingParams.Instance.PathToOocFile, RenderMode.PointSize);
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

                _sceneRenderer = new SceneRendererForward(_scene);
                _sceneRenderer.VisitorModules.Add(new PointCloudRenderModule(_sceneRenderer.GetType() == typeof(SceneRendererForward)));

                _pointCloud.Camera = _cam;

                IsInitialized = true;
            }
            catch (Exception ex)
            {
                Diagnostics.Error("Error loading potree2 file", ex);
                _sceneRenderer = new SceneRendererForward(new SceneContainer());
            }
        }

        private WritableTexture RenderTexture;
        private bool disposedValue;

        // RenderAFrame is called once a frame
        protected override ITextureHandle RenderAFrame()
        {
            ReadyToLoadNewFile = false;

            if (_closingRequested)
            {
                ReadyToLoadNewFile = true;

                return new Engine.Imp.Graphics.Desktop.TextureHandle
                {
                    DepthRenderBufferHandle = -1,
                    FrameBufferHandle = -1,
                    TexId = -1
                };
            }

            //Render Depth-only pass
            PtRenderingParams.Instance.DepthPassEf.Active = true;
            PtRenderingParams.Instance.ColorPassEf.Active = false;

            _cam.RenderTexture = PtRenderingParams.Instance.ColorPassEf.DepthTex;

            _sceneRenderer.Render(_rc);
            _cam.RenderTexture = RenderTexture;

            PtRenderingParams.Instance.DepthPassEf.Active = false;
            PtRenderingParams.Instance.ColorPassEf.Active = true;

            _sceneRenderer.Render(_rc);

            ReadyToLoadNewFile = true;

            return RenderTexture?.TextureHandle;
        }

        public override void Update(bool allowInput)
        {
            if (!allowInput) return;

            if (_closingRequested)
            {
                ReadyToLoadNewFile = true;
                return;
            }

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
        protected override void Resize(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;

            // delete old texture, generate new
            RenderTexture?.Dispose();
            // RenderTexture = WritableMultisampleTexture.CreateAlbedoTex(_rc, width, height, 8);
            RenderTexture = WritableTexture.CreateAlbedoTex(width, height, new ImagePixelFormat(ColorFormat.RGBA));

            if (PtRenderingParams.Instance.EdlStrength == 0f) return;
            PtRenderingParams.Instance.ColorPassEf.DepthTex?.Dispose();
            PtRenderingParams.Instance.ColorPassEf.DepthTex = WritableTexture.CreateDepthTex(width, height, new ImagePixelFormat(ColorFormat.Depth24));
        }

        public void ResetCamera()
        {
            _camTransform.Translation = _initCameraPos;
            _angleHorz = _angleVert = 0;
            _camTransform.FpsView(_angleHorz, _angleVert, Input.Keyboard.WSAxis, Input.Keyboard.ADAxis, Time.DeltaTimeUpdate * 20);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    RenderTexture?.Dispose();
                    PtRenderingParams.Instance.ColorPassEf.DepthTex?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}