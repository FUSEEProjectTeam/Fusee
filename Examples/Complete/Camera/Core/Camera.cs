using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.Camera.Core
{
    [FuseeApplication(Name = "FUSEE Camera Example", Description = " ")]
    public class CameraExample : RenderCanvas
    {
        private SceneContainer _rocketScene;
        private SceneRendererForward _sceneRenderer;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;

        private Transform _mainCamTransform;
        private readonly Engine.Core.Scene.Camera _mainCam = new(ProjectionMethod.Perspective, 5, 100, M.PiOver4);
        private readonly Engine.Core.Scene.Camera _sndCam = new(ProjectionMethod.Perspective, 1, 1000, M.PiOver4);

        private Transform _sndCamTransform;

        private WireframeCube _frustum;
        private float _anlgeHorznd;
        private float _angleVertSnd;
        private float _valHorzSnd;
        private float _valVertSnd;

        private float _anlgeHorzMain;
        private float _angleVertMain;
        private float _valHorzMain;
        private float _valVertMain;

        private async Task Load()
        {
            _gui = await FuseeGuiHelper.CreateDefaultGuiAsync(this, CanvasRenderMode.Screen, "FUSEE Camera Example");


            _frustum = new WireframeCube();
            SceneNode frustumNode = new()
            {
                Name = "Frustum",
                Components = new List<SceneComponent>()
                {
                    MakeEffect.FromDiffuseSpecular(new float4(1,1,0,1)),
                    _frustum
                }
            };

            SceneNode cam = new()
            {
                Name = "MainCam",
                Components = new List<SceneComponent>()
                {
                    _mainCamTransform,
                    _mainCam,
                    MakeEffect.FromDiffuseSpecular(new float4(1,0,0,1)),
                    new Cube(),

                },
                Children = new ChildList()
                {
                    new SceneNode()
                    {
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Scale = new float3(0.5f, 0.5f, 1f),
                                Translation = new float3(0,0, 1f)
                            },
                            new Cube()
                        }
                    }
                }
            };

            _sndCamTransform = new Transform()
            {
                Rotation = new float3(M.PiOver6, 0, 0),//float3.Zero,
                Translation = new float3(10, 40, -60),
                Scale = float3.One
            };

            SceneNode cam1 = new()
            {
                Name = "SecondCam",
                Components = new List<SceneComponent>()
                {
                    _sndCamTransform,
                    _sndCam,
                }
            };

            _anlgeHorznd = _sndCamTransform.Rotation.y;
            _angleVertSnd = _sndCamTransform.Rotation.x;
            _anlgeHorzMain = _mainCamTransform.Rotation.y;
            _angleVertMain = _mainCamTransform.Rotation.x;

            // Load the rocket model
            _rocketScene = await AssetStorage.GetAsync<SceneContainer>("rnd.fus");

            _rocketScene.Children.Add(cam);
            _rocketScene.Children.Add(cam1);
            _rocketScene.Children.Add(frustumNode);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_rocketScene);
            _guiRenderer = new SceneRendererForward(_gui);

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui, _guiRenderer.PrePassVisitor.CameraPrepassResults);
        }

        public override async Task InitAsync()
        {
            await Load();
            await base.InitAsync();
        }

        // Init is called on startup.
        public override void Init()
        {
            VSync = false;

            _mainCam.Viewport = new float4(0, 0, 100, 100);
            _mainCam.BackgroundColor = new float4(0f, 0f, 0f, 1);
            _mainCam.Layer = -1;

            _sndCam.Viewport = new float4(60, 60, 40, 40);
            _sndCam.BackgroundColor = new float4(0.5f, 0.5f, 0.5f, 1);
            _sndCam.Layer = 10;

            _mainCamTransform = new Transform()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 1, -30),
                Scale = new float3(1, 1, 1)
            };
        }

        public override void Update()
        {
            if (Mouse.RightButton)
            {
                _valHorzSnd = Mouse.XVel * 0.003f * DeltaTimeUpdate;
                _valVertSnd = Mouse.YVel * 0.003f * DeltaTimeUpdate;

                _anlgeHorznd += _valHorzSnd;
                _angleVertSnd += _valVertSnd;

                _valHorzSnd = _valVertSnd = 0;

                _sndCamTransform.FpsView(_anlgeHorznd, _angleVertSnd, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTimeUpdate * 10);
            }
            else if (Mouse.LeftButton)
            {
                _valHorzMain = Mouse.XVel * 0.003f * DeltaTimeUpdate;
                _valVertMain = Mouse.YVel * 0.003f * DeltaTimeUpdate;

                _anlgeHorzMain += _valHorzMain;
                _angleVertMain += _valVertMain;

                _valHorzMain = _valVertMain = 0;

                _mainCamTransform.FpsView(_anlgeHorzMain, _angleVertMain, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTimeUpdate * 10);
            }
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            float4x4 viewProjection = _mainCam.GetProjectionMat(Width, Height, out _) * float4x4.Invert(_mainCamTransform.Matrix);
            _frustum.Vertices.SetAttributeData(FrustumF.CalculateFrustumCorners(viewProjection).ToArray());

            FrustumF frustum = new();
            frustum.CalculateFrustumPlanes(viewProjection);

            // Sets a mesh inactive if it does not pass the culling test and active if it does.
            // The reason for this is to achieve that the cubes don't get rendered in the viewport in the upper right.
            // Because SceneRenderer.RenderMesh has an early-out if a Mesh is inactive we do not perform the culling test twice.
            UserSideFrustumCulling(_rocketScene.Children, frustum);

            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

            if (!Mouse.Desc.Contains("Android"))
            {
                _sih.CheckForInteractiveObjects(Mouse.Position, Width, Height);
            }
            if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private void UserSideFrustumCulling(IList<SceneNode> nodeChildren, FrustumF frustum)
        {
            foreach (SceneNode node in nodeChildren)
            {
                if (node.Name == "Frustum" || node.Name.Contains("Cam"))
                    continue;

                Mesh mesh = node.GetComponent<Mesh>();
                if (mesh != null)
                {
                    //We only perform the test for meshes that do have a calculated - non-zero sized - bounding box.
                    if (mesh.BoundingBox.Size != float3.Zero)
                    {
                        AABBf worldSpaceBoundingBox = node.GetComponent<Transform>().Matrix * mesh.BoundingBox;
                        if (!worldSpaceBoundingBox.InsideOrIntersectingFrustum(frustum))
                        {
                            mesh.Active = false;
                        }
                        else
                        {
                            mesh.Active = true;
                        }
                    }
                }

                if (node.Children.Count != 0)
                {
                    UserSideFrustumCulling(node.Children, frustum);
                }
            }
        }
    }
}