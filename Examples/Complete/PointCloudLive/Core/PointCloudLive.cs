using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.FileReader.LasReader;
using System.Collections.Generic;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.PointCloudLive.Core
{
    [FuseeApplication(Name = "FUSEE Point Cloud Example", Description = "")]
    public class PointCloudLive : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _pointCloud;
        private SceneRendererForward _sceneRenderer;

        private bool _keys;
        private SceneNode _node;
        private Transform _mainCamTransform;

        private WritableTexture _depthTex;
        private PointCloudSurfaceEffect _pcFx;
        private ShaderEffect _depthFx;
        private Camera _mainCam;
        private bool _renderForward;

        // Init is called on startup.
        public override void Init()
        {
            VSync = false;
            RC.ClearColor = new float4(1, 1, 1, 1);

            _node = new SceneNode();

            _node.Components.AddRange(LasToMesh.GetMeshsFromLasFile(new Pos64Col32_Accessor(), PointType.Pos64Col32, "D:\\LAS\\HolbeinPferd.las", out var aabbRes));

            _mainCamTransform = new Transform()
            {
                Translation = aabbRes.Center - new float3(0, 0, aabbRes.Size.z * 2)
            };

            _mainCam = new Camera(ProjectionMethod.Perspective, 1f, 5000, M.PiOver4)
            {
                FrustumCullingOn = false,
                BackgroundColor = float4.One
            };

            SceneNode camNode = new()
            {
                Name = "MainCam",
                Components = new List<SceneComponent>()
                {
                    _mainCamTransform,
                    _mainCam
                }
            };

            _pcFx = new PointCloudSurfaceEffect
            {
                PointSize = 5,
                ColorMode = (int)ColorMode.Point,
                DoEyeDomeLighting = true,
                DepthTex = _depthTex,
                EDLStrength = 0.7f,
                EDLNeighbourPixels = 2,
                ScreenParams = new float2(Width, Height),
                ClippingPlanes = _mainCam.ClippingPlanes
            };
            _node.Components.Insert(0, _pcFx);

            _pointCloud = new SceneContainer()
            {
                Children = new List<SceneNode>()
                {
                    _node,
                    camNode
                }
            };



            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_pointCloud);
            _renderForward = _sceneRenderer.GetType() == typeof(SceneRendererForward);

            if (_renderForward)
            {
                _depthTex = WritableTexture.CreateDepthTex(Width, Height, new ImagePixelFormat(ColorFormat.Depth24));
                _depthFx = CreateDepthPassEffect(_pcFx.PointSize, (int)PointShape.Rect, (int)PointSizeMode.FixedPixelSize, new float2(Width, Height), _mainCamTransform.Translation.z);
            }
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            Diagnostics.Warn(FramesPerSecond);
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            RC.Viewport(0, 0, Width, Height);

            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
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
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            _mainCamTransform.Rotation = new float3(_angleVert, _angleHorz, 0);

            if (_pcFx.DoEyeDomeLighting && _renderForward)
            {
                //Render Depth-only pass
                _node.RemoveComponent<PointCloudSurfaceEffect>();
                _node.Components.Insert(0, _depthFx);

                _mainCam.RenderTexture = _depthTex;
                _sceneRenderer.Render(RC);
                _mainCam.RenderTexture = null;

                _node.RemoveComponent<ShaderEffect>();
                _node.Components.Insert(0, _pcFx);
            }

            _sceneRenderer.Render(RC);

            Present();
        }

        public override void Resize(ResizeEventArgs e)
        {
            _pcFx.ScreenParams = new float2(Width, Height);
            base.Resize(e);
        }

        private static ShaderEffect CreateDepthPassEffect(int ptSize, int ptShape, int ptMode, float2 screenParams, float initCamPosZ)
        {
            return new ShaderEffect(
            new FxPassDeclaration
            {
                VS = AssetStorage.Get<string>("PointCloud.vert"),
                PS = AssetStorage.Get<string>("PointDepth.frag"),
                StateSet = new RenderStateSet
                {
                    AlphaBlendEnable = true,
                    ZEnable = true,
                }
            },
            new List<IFxParamDeclaration>
            {
                new FxParamDeclaration<float4x4> {Name = "FUSEE_MVP", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_MV", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_P", Value = float4x4.Identity},

                new FxParamDeclaration<float2> {Name = "ScreenParams", Value = screenParams},
                new FxParamDeclaration<float> {Name = "InitCamPosZ", Value = System.Math.Abs(initCamPosZ)},

                new FxParamDeclaration<int> {Name = "PointSize", Value = ptSize},
                new FxParamDeclaration<int> {Name = "PointShape", Value = ptShape},
                new FxParamDeclaration<int> {Name = "PointSizeMode", Value = ptMode},
            });
        }
    }
}