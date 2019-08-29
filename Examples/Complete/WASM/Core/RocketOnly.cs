using System;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.RocketOnly.Core
{
    [FuseeApplication(Name = "FUSEE Rocket Only Example", Description = "An even simpler example.")]
    public class RocketOnly : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;

        private TransformComponent _cubeTransform;
        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private float _fovy = M.PiOver4;

        private SceneRenderer _guiRenderer;

        private bool _keys;

        public string VertexShader = "";
        public string PixelShader = "";
        public ImageData CurrentTex;

        // Init is called on startup. 
        public async override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            Console.WriteLine("trying to get asset");
            var readVar = await AssetStorage.GetAsync<string>("Assets/LoremIpsum.txt");
            Console.WriteLine(readVar);

            RocketScene = await AssetStorage.GetAsync<SceneContainer>("Assets/FUSEERocket.fus");
            Console.WriteLine(_rocketScene.Children[0].Components[0].Name);
            Console.WriteLine(_rocketScene.Children.Count());

            var x = await AssetStorage.GetAsync<ImageData>("Assets/FuseeText.png");
            Console.WriteLine($"ImageData {x.Height}:{x.Width}");
        }

        public SceneContainer RocketScene
        {
            get => _rocketScene;
            set
            {
                _rocketScene = value;

                //Add resize delegate
                var projComp = _rocketScene.Children[0].GetComponent<ProjectionComponent>();
                AddResizeDelegate(delegate
                {
                    projComp.Resize(Width, Height);
                    RC.Viewport(0, 0, Width, Height);
                });
                _sceneRenderer = new SceneRenderer(_rocketScene);
                Diagnostics.Log("Rocket Set");
                projComp.Resize(Width, Height);
            }
        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Mouse and keyboard movement
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

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, +2, -10, 0, +2, 0, 0, 1, 0);
            RC.View = mtxCam * mtxRot;

            // Render the scene   
            if(_sceneRenderer != null)
                _sceneRenderer.Render(RC);           

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs rea)
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 0.01 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 200 (Anything further away from the camera than 200 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 0.01f, 200.0f);
            RC.Projection = projection;
        }
    }
}