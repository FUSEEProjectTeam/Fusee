using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;


namespace Fusee.Examples.MuVista.Core
{
    [FuseeApplication(Name = "FUSEE MuVista Viewer", Description = "Viewer for 360 degree pictures.")]
    public class MuVista : RenderCanvas
    {

        private static float _angleHorz = M.Pi, _angleVert = 0, _angleVelHorz, _angleVelVert, _zoom;

        private const float RotationSpeed = 2;
        private const float Damping = 0.8f;

        private SceneRendererForward _sceneRenderer;
        private SceneRendererForward _guiRenderer;

        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.Screen;

        private SceneContainer _sphereScene;
        private Transform _sphereTransform;

        private Transform _mainCamTransform;
        private readonly Camera _mainCam = new Camera(ProjectionMethod.Perspective, 3, 100, M.PiOver4)
        {
            BackgroundColor = float4.One
        };
        private Camera _guiCam;

        private GuiButton _zoomIn;
        private GuiButton _zoomOut;

        private float _maxFov = 1.2f;
        private float _minFov = 0.3f;

        private SurfaceEffect _sphere_Fx;

        //Inactivity Checker
        private float _inActiveTimer = 0f;


        // Init is called on startup.
        public override async Task InitAsync()
        {
            _guiCam = new Camera(ProjectionMethod.Orthographic, 0.01f, 500, M.PiOver4)
            {
                Active = true,
                ClearColor = false
            };

            var sphereTex = new Texture(AssetStorage.Get<ImageData>("LadyBug_C1P2.jpg"));

            Sphere sphere = new Sphere(10, 20, 50);

            //Creating CameraComponent and TransformComponent
            _mainCam.Viewport = new float4(0, 0, 100, 100);
            _mainCam.BackgroundColor = new float4(0f, 0f, 0f, 1);
            _mainCam.Layer = -1;
            _mainCam.Active = true;


            _mainCamTransform = new Transform()
            {
                Rotation = new float3(_angleVert, _angleHorz, 0),
                Translation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1)
            };

            _sphereTransform = new Transform()
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };

            _sphere_Fx = MakeEffect.FromBRDF
                (
                    albedoColor: float4.One,
                    albedoMix: 1f,
                    albedoTex: sphereTex,
                    roughness: 0.3f,
                    texTiles: float2.One,
                    metallic: 0.0f,
                    specular: 0.0f,
                    ior: 1.519f,
                    subsurfaceColor: float3.Zero,
                    emissionColor: float3.Zero
                );


            _gui = new GUI(Width, Height, _canvasRenderMode, _mainCamTransform, _guiCam, out _zoomOut, out _zoomIn);

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);


            //Scene with Main Camera and Mesh
            _sphereScene = new SceneContainer
            {
                Children = new List<SceneNode>
                {
                     new SceneNode
                     {
                        Name = "MainCam",
                        Components = new List<SceneComponent>()
                        {
                            _mainCamTransform,
                            _mainCam
                        }
                     },
                    new SceneNode
                    {
                        Components = new List<SceneComponent>
                        {
                            _sphereTransform,
                            _sphere_Fx,
                            sphere
                        }
                    }
                }
            };

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_sphereScene);
            _guiRenderer = new SceneRendererForward(_gui);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            MouseWheelZoom();

            CalculateRotationAngle();

            HndGuiButtonInput();

            UpdateCameraTransform();

            if (Mouse.LeftButton || (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0))
            {
                _inActiveTimer = 0f;
            }

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);


            if (!Input.Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Input.Mouse.Position, Width, Height);
            if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Input.Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Input.Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        public void MouseWheelZoom()
        {
            _zoom = Mouse.WheelVel * DeltaTime * -0.05f;
            if (!(_mainCam.Fov + _zoom >= _maxFov) && !(_mainCam.Fov + _zoom <= _minFov))
            {
                _mainCam.Fov += _zoom;
            }
        }

        public void UpdateCameraTransform()
        {
            if (_inActiveTimer < 12f)
            {
                _inActiveTimer += Time.DeltaTime;

                _mainCamTransform.Rotation = new float3(_angleVert, _angleHorz, 0);
            }
            if (_inActiveTimer > 12f)
            {
                RotationAfterInactivity();
            }
        }

        public void CalculateRotationAngle()
        {
            if (Mouse.LeftButton)
            {
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
            }
            else
            {
                if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
                {
                    _angleVelHorz = RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;

                    if (_angleVert < 0)
                    {
                        _angleVelVert = -(RotationSpeed / (((_angleVert * -1) + 1) * (1.5f))) * Keyboard.UpDownAxis * DeltaTime;
                    }
                    else
                    {
                        _angleVelVert = -(RotationSpeed / ((_angleVert + 1) * 1.5f)) * Keyboard.UpDownAxis * DeltaTime;
                    }
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }
            //Calculations to test the camera has max vertical angle
            if (!(_angleVert + _angleVelVert >= 1.5f) && !(_angleVert + _angleVelVert <= -1.5f))
            {
                _angleVert += _angleVelVert;
            }
            _angleHorz += _angleVelHorz;
        }

        public void RotationAfterInactivity()
        {
            _angleHorz += 0.5f * Time.DeltaTime;
            _mainCamTransform.Rotation = new float3(_angleVert, _angleHorz, 0);
        }

        public void HndGuiButtonInput()
        {
            if (_zoomOut.IsMouseOver)
            {
                _zoomOut.OnMouseDown += BtnZoomOutDown;
            }

            if (_zoomIn.IsMouseOver)
            {
                _zoomIn.OnMouseDown += BtnZoomInDown;
            }
        }

        public void BtnZoomInDown(CodeComponent sender)
        {
            if (!(_mainCam.Fov - 0.001 <= 0.3))
            {
                _mainCam.Fov -= 0.001f;
            }
        }

        public void BtnZoomOutDown(CodeComponent sender)
        {
            if (_mainCam.Fov + 0.001 <= 1.2f)
            {
                _mainCam.Fov += 0.001f;
            }
        }

    }
}