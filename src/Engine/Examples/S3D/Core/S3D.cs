using System;
using System.Diagnostics;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Examples.S3D.Core
{

    [FuseeApplication(Name = "fuseeStereoApp", Description = "Yet another FUSEE App.")]
    public class S3D : RenderCanvas
    {
        // Horizontal and vertical rotation Angles for the displayed object 
        private static float _angleHorz = M.PiOver4, _angleVert;

        // Horizontal and vertical angular speed
        private static float _angleVelHorz, _angleVelVert;

        // Overall speed factor. Change this to adjust how fast the rotation reacts to input
        private const float RotationSpeed = 7;

        // Damping factor 
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;

        private bool _keys;

        private enum Assignment
        {
            A,
            BC,
            D
        }

        private static Assignment _assignment = Assignment.A;

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Fullscreen
            SetWindowSize(1920, 1080, 0, 0, true);

            // Load the rocket model
            // TODO: Replace with scene from group A
            _scene = AssetStorage.Get<SceneContainer>("RocketModel.fus");

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Close Window with ESC
            if (Input.Keyboard.GetKey(KeyCodes.Escape))
                CloseGameWindow();

            // Close Window with Alt+F4
            if (Input.Keyboard.GetKey(KeyCodes.LMenu) && Input.Keyboard.GetKey(KeyCodes.F4))
                CloseGameWindow();

            // Mouse and keyboard movement
            if (Input.Keyboard.LeftRightAxis != 0 || Input.Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Input.Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Input.Mouse.XVel * Time.DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Input.Mouse.YVel * Time.DeltaTime * 0.0005f;
            }
            else if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Input.Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * Time.DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * touchVel.y * Time.DeltaTime * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Input.Keyboard.LeftRightAxis * Time.DeltaTime;
                    _angleVelVert = -RotationSpeed * Input.Keyboard.UpDownAxis * Time.DeltaTime;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }


            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // switch groups
            if (Input.Keyboard.GetKey(KeyCodes.F1))
            {
                ResetAllParams();
                // TODO: Replace with scene Group A
                _scene = AssetStorage.Get<SceneContainer>("RocketModel.fus");
                _sceneRenderer = new SceneRenderer(_scene);
                _assignment = Assignment.A;
            }
            if (Input.Keyboard.GetKey(KeyCodes.F2))
            {
                ResetAllParams();
                // TODO: Replace with scene Group BC
                _scene = AssetStorage.Get<SceneContainer>("balls_sky.fus");
                _sceneRenderer = new SceneRenderer(_scene);
                _assignment = Assignment.BC;
            }
            if (Input.Keyboard.GetKey(KeyCodes.F3))
            {
                ResetAllParams();
                // TODO: Replace with scene Group D
                _scene = AssetStorage.Get<SceneContainer>("RocketModel.fus");
                _sceneRenderer = new SceneRenderer(_scene);
                _assignment = Assignment.D;
            }

            // Call group Methods
            switch (_assignment)
            {
                case Assignment.A:
                    GroupA();
                    break;
                case Assignment.BC:
                    GroupBC();
                    break;
                case Assignment.D:
                    GroupD();
                    break;
                default:
                    new NotSupportedException($"Assignment {_assignment} not supported.");
                    break;
            }

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        // Reset ModelView and ProjectionMatrix between group switch
        private void ResetAllParams()
        {

            RC.ModelView = float4x4.LookAt(0, 0, -10, 0, 0, 0, 0, 1, 0);

            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)(Height);
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;

            // Fullscreen
            SetWindowSize(1920, 1080, 0, 0, true);

        }

        private void GroupA()
        {

            var aspectRatio = Width / (Height / 2f); // Set aspect ratio ganze Weite halbe Höhe
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000); // Erzeuge Projektionsmatrix Öffnugnswinkel PiOver4, nearplane 1, farplane 2000
            RC.Projection = projection; // Setze Projektionsmatrix

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz); // Create rotation around X and Y asix based upon mouse angle input
            var mtxCam = float4x4.LookAt(0, 0, -10, 0, 0, 0, 0, 1, 0); // Create Camera Matrix. Position of Camera = 0,0,-10, camera aims/looks at 0,0,0; y-axis up
            RC.ModelView = mtxCam * mtxRot; // Rotation * Cameramatrix = new RenderContext ModelView Matrix (== Cameramatrix)

            RC.Viewport(0, 0, Width, Height / 2); // Adjust the viewport and hence the visible render ouput. Start at 0,0 and fill the complete window width but only half of the window height

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC); // Render the scene to the viewport with current RC.ModelView-Matrix and current RC.Projection-Matrix!

            // Create the second camera matrix and set it as the current ModelView transformation
            mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz); // Create rotation around X and Y asix based upon mouse angle input
            mtxCam = float4x4.LookAt(2, 0, -10, 2, 0, 0, 0, 1, 0); // Create Camera Matrix. Position of Camera = 2,0,-10, camera aims/looks at 2,0,0; y-axis up
            RC.ModelView = mtxCam * mtxRot; // Rotation * Cameramatrix = new RenderContext ModelView Matrix (== Cameramatrix)

            RC.Viewport(0, Height / 2, Width, Height / 2); // Adjust the viewport and hence the visible render ouput. Starte bei 0 und der Hälfte der Höhe des Fensters und rendere damit den unteren Teil des Viewports.



            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC); // Render the scene to the viewport with current RC.ModelView-Matrix and current RC.Projection-Matrix!

        }



        // B) gibt es Szenen-Stauchungseffekte bei Renderkameras mit sehr langen Brennweiten (schmales viewing frustum)?
        private static float _fov = M.PiOver2;
        private void GroupBC()
        {
            // DEBUG
            SetWindowSize(1920, 1080, 0, 0, true);
            Diagnostics.Log($"FOV: {_fov}.");
            
            var fovDelta = _fov + Input.Mouse.WheelVel * 0.001f;
            Debug.WriteLine(Input.Mouse.WheelVel);
            _fov += fovDelta > 0.01f && fovDelta < M.Pi ? Input.Mouse.WheelVel * 0.001f : 0;

            var aspectRatio = Width / (Height / 2f); // Set aspect ratio ganze Weite halbe Höhe
            var projection = float4x4.CreatePerspectiveFieldOfView(_fov, aspectRatio, 1, 200000); // Erzeuge Projektionsmatrix Öffnugnswinkel PiOver4, nearplane 1, farplane 2000
            RC.Projection = projection; // Setze Projektionsmatrix

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz); // Create rotation around X and Y asix based upon mouse angle input
            var mtxCam = float4x4.LookAt(0, 0, -7, 0, 0, 0, 0, 1, 0); // Create Camera Matrix. Position of Camera = 0,0,-10, camera aims/looks at 0,0,0; y-axis up
            RC.ModelView = mtxCam * mtxRot; // Rotation * Cameramatrix = new RenderContext ModelView Matrix (== Cameramatrix)

            RC.Viewport(20, 0, Width, Height / 2); // Adjust the viewport and hence the visible render ouput. Start at 0,0 and fill the complete window width but only half of the window height

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC); // Render the scene to the viewport with current RC.ModelView-Matrix and current RC.Projection-Matrix!

            // Create the second camera matrix and set it as the current ModelView transformation
            mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz); // Create rotation around X and Y asix based upon mouse angle input
            mtxCam = float4x4.LookAt(-0.5f, 0, -7, -0.5f, 0, 0, 0, 1, 0); // Create Camera Matrix. Position of Camera = 2,0,-10, camera aims/looks at 2,0,0; y-axis up
            RC.ModelView = mtxCam * mtxRot; // Rotation * Cameramatrix = new RenderContext ModelView Matrix (== Cameramatrix)

            RC.Viewport(-20, Height / 2, Width, Height / 2); // Adjust the viewport and hence the visible render ouput. Starte bei 0 und der Hälfte der Höhe des Fensters und rendere damit den unteren Teil des Viewports.

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC); // Render the scene to the viewport with current RC.ModelView-Matrix and current RC.Projection-Matrix!
        }

        private void GroupD()
        {
            // HotLoad Scene
            if (Input.Keyboard.GetKey(KeyCodes.L))
                HotLoadSceneGroupD();

            // Hier Code für Gruppe D hinterlegen
            Diagnostics.Log("Group D");
            _sceneRenderer.Render(RC); // Render the scene to the viewport with current RC.ModelView-Matrix and current RC.Projection-Matrix!
        }

        private void HotLoadSceneGroupA()
        {
            // Load the rocket model
            _scene = AssetStorage.Get<SceneContainer>("RocketModel.fus");

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_scene);
        }

        private void HotLoadSceneGroupD()
        {
            // Load the rocket model
            _scene = AssetStorage.Get<SceneContainer>("RocketModel.fus");

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_scene);
        }

        private InputDevice Creator(IInputDeviceImp device)
        {
            throw new NotImplementedException();
        }

        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            //RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)(Height);

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}