using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Examples.PickingRayCast.Core
{
    [FuseeApplication(Name = "FUSEE Picking Example RayCast", Description = "How to use the Scene RayCaster.")]
    public class PickingRayCast : RenderCanvas
    {
        // angle variables
        private static float _angleVelHorz;

        private const float RotationSpeed = 7;
        private const float Damping = 0.0005f;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private SceneRayCaster _sceneRayCaster;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;

        private bool _pick;
        private float2 _pickPos;

        private readonly Camera _cam = new(ProjectionMethod.Perspective, 1, 1000, M.PiOver4);
        private readonly Camera _cam2 = new(ProjectionMethod.Perspective, 1, 1000, M.PiOver4);
        private readonly Camera _guiCam = new(ProjectionMethod.Orthographic, 1, 1000, M.PiOver4);
        private Transform _camTransform;
        private Transform _cam2Transform;
        private Transform _guiCamTransform;

        private bool _loaded = false;

        private async void Load()
        {
            // Create gui
            _gui = await FuseeGuiHelper.CreateDefaultGuiAsync(this, CanvasRenderMode.Screen, "FUSEE Picking Example with RayCast");
            SceneNode guiCam = new()
            {
                Name = "GuiCam",
                Components = new List<SceneComponent>()
                {
                    _guiCamTransform,
                    _guiCam,
                }
            };
            _gui.Children.Insert(0, guiCam);


            // Create Scene
            _scene = CreateScene();

            SceneNode cam = new()
            {
                Name = "Cam",
                Components = new List<SceneComponent>()
                {
                    _camTransform,
                    _cam,
                }
            };
            _scene.Children.Add(cam);

            SceneNode cam2 = new()
            {
                Name = "Cam2",
                Components = new List<SceneComponent>()
                {
                    _cam2Transform,
                    _cam2,
                }
            };
            _scene.Children.Add(cam2);


            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _sceneRayCaster = new SceneRayCaster(_scene, _sceneRenderer.PrePassVisitor.CameraPrepassResults, Cull.Clockwise);

            // Create the interaction handler
            _guiRenderer = new SceneRendererForward(_gui);

            _loaded = true;

        }


        // Init is called on startup.
        public override void Init()
        {
            _cam.Viewport = new float4(0, 0, 50, 100);
            _cam.BackgroundColor = new float4(1f, 1f, 1f, 1);
            _cam.Layer = -1;

            _cam2.Viewport = new float4(50, 0, 50, 100);
            _cam2.BackgroundColor = new float4(.5f, 0f, 1f, 1);
            _cam2.Layer = -1;

            _guiCam.ClearColor = false;
            _guiCam.ClearDepth = false;
            _guiCam.FrustumCullingOn = false;

            _camTransform = _guiCamTransform = new Transform()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 0, -40),
                Scale = new float3(1, 1, 1)
            };

            _cam2Transform = new Transform()
            {
                Rotation = new float3(0, M.DegreesToRadians(180), 0),
                Translation = new float3(0, 0, 40),
                Scale = new float3(1, 1, 1)
            };

            Load();
        }

        public override void Update()
        {
            if (!_loaded) return;

            // Mouse Controls
            if (Input.Mouse.LeftButton)
            {
                _pick = true;
                _pickPos = Input.Mouse.Position;
            }
            if (Input.Mouse.RightButton)
            {
                _angleVelHorz = RotationSpeed * Input.Mouse.XVel * Time.DeltaTimeUpdate * Damping;
            }

            _camTransform.RotateAround(float3.Zero, new float3(0, _angleVelHorz, 0));
            _cam2Transform.RotateAround(float3.Zero, new float3(0, _angleVelHorz, 0));


            // Check for hits
            if (_pick)
            {
                // prepare mouse coordinates; for each camera calculate clip space mouse position and create a new ray which travels through the scene and collects meshes
                CameraResult pickCam = default;
                Rectangle pickCamRect = new();

                // check in which camera our mouse is currently positioned (left or right)
                foreach (var camRes in _sceneRenderer.PrePassVisitor.CameraPrepassResults)
                {
                    Rectangle camRect = new()
                    {
                        Left = (int)(camRes.Camera.Viewport.x * RC.ViewportWidth / 100),
                        Top = (int)(camRes.Camera.Viewport.y * RC.ViewportHeight / 100)
                    };
                    camRect.Right = ((int)(camRes.Camera.Viewport.z * RC.ViewportWidth) / 100) + camRect.Left;
                    camRect.Bottom = ((int)(camRes.Camera.Viewport.w * RC.ViewportHeight) / 100) + camRect.Top;

                    if (!float2.PointInRectangle(new float2(camRect.Left, camRect.Top), new float2(camRect.Right, camRect.Bottom), Input.Mouse.Position))
                        continue;

                    if (pickCam == default || camRes.Camera.Layer > pickCam.Camera.Layer)
                    {
                        pickCam = camRes;
                        pickCamRect = camRect;
                    }
                }

                // generate fitting clip position with the currently used camera rectangle
                var pickPosClip = ((Input.Mouse.Position - new float2(pickCamRect.Left, pickCamRect.Top)) * new float2(2.0f / pickCamRect.Width, -2.0f / pickCamRect.Height)) + new float2(-1, 1);

                // generate ray at mouse position and...
                var ray = new RayF(pickPosClip, pickCam.View, pickCam.Camera.GetProjectionMat(RC.ViewportWidth, RC.ViewportHeight, out var _));
                // ... send it through the scene
                var castHit = _sceneRayCaster.Traverse(ray).ToList().OrderBy(rr => rr.DistanceFromOrigin).FirstOrDefault();

                if (castHit != null)
                    castHit.Node.GetComponent<SurfaceEffect>().SurfaceInput.Albedo = (float4)ColorUint.LawnGreen;

                _pick = false;
            }
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            if (!_loaded) return;

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private static SceneContainer CreateScene()
        {
            var scene = new SceneContainer
            {
                Header = new SceneHeader
                {
                    CreationDate = "July 2021",
                    CreatedBy = "Jonas Haller",
                    Generator = "Handcoded with pride :)",
                },
                Children = new List<SceneNode> { }
            };

            var rand = new Random();

            for (int i = 0; i < 20; i++)
            {
                var x = rand.Next(-10, 10);
                var y = rand.Next(-10, 10);
                var z = rand.Next(-10, 10);

                var mesh = new Engine.Core.Primitives.Cube();

                var cube = new SceneNode()
                {
                    Name = "Cube" + i,
                    Components = new List<SceneComponent>()
                    {
                        new Transform()
                        {
                            Rotation = float3.Zero,
                            Translation = new float3(x, y, z),
                            Scale = new float3(1f, 1f, 1f)
                        },
                        MakeEffect.FromDiffuseSpecular((float4)ColorUint.Gray),
                        mesh
                    }
                };

                scene.Children.Add(cube);
            }

            return scene;
        }
    }
}