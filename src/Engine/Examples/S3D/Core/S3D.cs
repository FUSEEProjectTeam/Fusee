using System;
using System.Diagnostics;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Examples.S3D.Core
{

    [FuseeApplication(Name = "fuseeStereoApp", Description = "Yet another FUSEE App.")]
    public class S3D : RenderCanvas
    {

        #region S3D fields
        //Assumption: 1 fusee unit = 1 meter, all following varaiables are in meters
        public static float ViewingDistance;//Distance User to Display (V)
        public static float Interaxial;     //Stereo base (t)
        public static float Magnification;  //Magnification factor sensor to image (M)
        public static float FocalLength;    //Camera focal lenght - only fov for calculation... (f)
        public static float Hit;            //Image to Sensor offset (h)
        public static float EyeSeparation;  //Eye separation of the user (e)

        private static float _fov = M.PiOver4;
        private static float _aspectRatio;
        #endregion

        #region Mouse control fields
        // Horizontal and vertical rotation Angles for the displayed object 
        public static float _angleHorz, _angleVert = M.DegreesToRadians(-10);

        // Horizontal and vertical angular speed
        public static float _angleVelHorz, _angleVelVert;

        // Overall speed factor. Change this to adjust how fast the rotation reacts to input
        private const float RotationSpeed = 7;

        // Damping factor 
        private const float Damping = 0.8f;
        #endregion

        #region GUI BC

        private GUIHandler _guiHandler;
        private GUIText _distToCamTextOne;
        private GUIText _shapeRatioTextOne;
        private GUIText _distToCamTextTwo;
        private GUIText _shapeRatioTextTwo;
        private GUIText _fovText;
        private FontMap _guiLatoBlackMap;
        #endregion

        private SceneContainer _sceneA;
        private SceneContainer _sceneBc;
        private SceneContainer _sceneD;
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

            // TODO: Replace with scene from group A
            _sceneA = AssetStorage.Get<SceneContainer>("RocketModel.fus");
            _sceneBc = AssignmentShapeRatioHelper.CreateScene(RC);
            _sceneD = AssetStorage.Get<SceneContainer>("RocketModel.fus");

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_sceneA);

            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);
            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            fontLato.UseKerning = true;

            #region GUI BC
            _guiLatoBlackMap = new FontMap(fontLato, 18);
            _distToCamTextOne = new GUIText("Distance Camera to yellow", _guiLatoBlackMap, 50, 50) { TextColor = ColorUint.Tofloat4(ColorUint.Greenery) };
            _shapeRatioTextOne = new GUIText("Shape ratio yellow", _guiLatoBlackMap, 50, 70) { TextColor = ColorUint.Tofloat4(ColorUint.Greenery) };
            _distToCamTextTwo = new GUIText("Distance to green", _guiLatoBlackMap, 50, 150) { TextColor = ColorUint.Tofloat4(ColorUint.Greenery) };
            _shapeRatioTextTwo = new GUIText("Shape ratio green", _guiLatoBlackMap, 50, 180) { TextColor = ColorUint.Tofloat4(ColorUint.Greenery) };
            _fovText = new GUIText("Fiel of View (degree)", _guiLatoBlackMap, 50, 250) { TextColor = ColorUint.Tofloat4(ColorUint.Greenery) };

            _guiHandler.Add(_distToCamTextOne);
            _guiHandler.Add(_shapeRatioTextOne);
            _guiHandler.Add(_distToCamTextTwo);
            _guiHandler.Add(_shapeRatioTextTwo);
            _guiHandler.Add(_fovText);
            #endregion

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
            Debug.WriteLine(_angleVelVert);
            Debug.WriteLine(_angleVelHorz);
            // switch groups
            if (Input.Keyboard.GetKey(KeyCodes.F1))
            {
                ResetAllParams();
                // TODO: Replace with scene Group A
                _sceneRenderer = new SceneRenderer(_sceneA);
                _assignment = Assignment.A;
            }
            if (Input.Keyboard.GetKey(KeyCodes.F2))
            {
                ResetAllParams();
                _sceneRenderer = new SceneRenderer(_sceneBc);
                _assignment = Assignment.BC;
            }
            if (Input.Keyboard.GetKey(KeyCodes.F3))
            {
                ResetAllParams();
                // TODO: Replace with scene Group D
                _sceneRenderer = new SceneRenderer(_sceneD);
                _assignment = Assignment.D;
            }

            // Call group Methods
            switch (_assignment)
            {
                case Assignment.A:
                    GroupA();
                    break;
                case Assignment.BC:
                    GroupBc();
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

        // B) Are there perceptible shape ratio changes (high, low fov values)?
        //Assumption: 1 Fusee unit equals 1 meter
        private void GroupBc()
        {
            const float physicalDisplayWidth = 1.107f;
            const float interaxial = 0.2f;
            const int hitInPx = 0;
            const int resolutionW = 1920;
            const int resolutonH = 1080;
            float camOffset = AssignmentShapeRatioHelper.CamOffset;

            SetWindowSize(resolutionW, resolutonH, 0, 0, true);

            //in mm for shape ratio calculation
            var distCamToObjOne = camOffset + AssignmentShapeRatioHelper.ObjOneDistToRoot;
            var distCamToObjTwo = camOffset + AssignmentShapeRatioHelper.ObjTwoDistToRoot;

            #region Shape ratio calculation
            //All following parameters are given in millimeters
            Interaxial = interaxial;
            EyeSeparation = 65 / 1000f;
            FocalLength = (float)System.Math.Tan(0.5f * _fov);
            Hit = AssignmentShapeRatioHelper.PixelToMeter(hitInPx, resolutionW, physicalDisplayWidth);
            Magnification = 1; //factor is 1 because we only have perspective projection, the only factor that affects the objects size in the picture is fov.
            ViewingDistance = 2.5f;

            //Smith Collar
            var shapeRatioObjOne = AssignmentShapeRatioHelper.CalculateShapeRatio(distCamToObjOne);
            var shapeRatioObjTwo = AssignmentShapeRatioHelper.CalculateShapeRatio(distCamToObjTwo);
            #endregion

            #region GUI
            _distToCamTextOne.Text = "Distance to camera yellow: " + distCamToObjOne;
            _shapeRatioTextOne.Text = "Calculated shape ratio yellow: " + shapeRatioObjOne;
            _distToCamTextTwo.Text = "Distance to camera green: " + distCamToObjTwo;
            _shapeRatioTextTwo.Text = "Calculated shape ratio green: " + shapeRatioObjTwo;
            _fovText.Text = "Field of View (degree): " + M.RadiansToDegrees(_fov);
            #endregion

            #region LEFT Camera setup

            //var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(-interaxial / 2f, 0, -camOffset, -interaxial / 2f, 0, 0, 0, 1, 0);

            RC.ModelView = mtxCam;//* mtxRot;

            const int n = 1;
            const int f = 20000;
            var convergence = AssignmentShapeRatioHelper.Convergence;
            var tanFov = (float)System.Math.Tan(_fov / 2);

            var top = n * tanFov;
            var bottom = -top;

            var a = _aspectRatio * tanFov * convergence;

            var b = a - Interaxial / 2;
            var c = a + Interaxial / 2;

            var left = -b * n / convergence;
            var right = c * n / convergence;


            var offCenterPorjection = float4x4.CreatePerspectiveOffCenter(left, right, bottom, top, n, f);
            RC.Projection = offCenterPorjection;

            RC.Viewport(-hitInPx, 0 - hitInPx, Width / 2 + hitInPx, Height + hitInPx);
            _guiHandler.RenderGUI();


            var screenCoord1 = AssignmentShapeRatioHelper.WorldToScreenCoord(new float3(-0.5f, 0.5f, -0.5f), RC, resolutonH, resolutionW / 2);
            var screenCoord2 = AssignmentShapeRatioHelper.WorldToScreenCoord(new float3(0.5f, 0.5f, -0.5f), RC, resolutonH, resolutionW / 2);
            var screenCoord3 = AssignmentShapeRatioHelper.WorldToScreenCoord(new float3(0.5f, 0.5f, 0.5f), RC, resolutonH, resolutionW / 2);

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

            // Calc prallax from ModelCoords
            var mvpL = RC.ModelViewProjection;

            var debugL = new[]
            {
                screenCoord1, screenCoord2, screenCoord3,
            };

            foreach (var item in debugL)
            {
                Debug.WriteLine(item);
            }

            #endregion

            #region RIGHT Camera setup

            mtxCam = float4x4.LookAt(interaxial / 2f, 0, -camOffset, interaxial / 2f, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam; //* mtxRot;

            top = n * tanFov;
            bottom = -top;
            left = -c * n / convergence;
            right = b * n / convergence;

            offCenterPorjection = float4x4.CreatePerspectiveOffCenter(left, right, bottom, top, n, f);
            RC.Projection = offCenterPorjection;

            RC.Viewport(Width / 2, 0 - hitInPx, Width / 2 + hitInPx, Height + hitInPx);
            _guiHandler.RenderGUI();


            screenCoord1 = AssignmentShapeRatioHelper.WorldToScreenCoord(new float3(-0.5f, 0.5f, -0.5f), RC, resolutonH, resolutionW / 2);
            screenCoord2 = AssignmentShapeRatioHelper.WorldToScreenCoord(new float3(0.5f, 0.5f, -0.5f), RC, resolutonH, resolutionW / 2);
            screenCoord3 = AssignmentShapeRatioHelper.WorldToScreenCoord(new float3(0.5f, 0.5f, 0.5f), RC, resolutonH, resolutionW / 2);

            // Calc prallax from ModelCoords in mm
            var mvpR = RC.ModelViewProjection;
            var parallaxInMm = AssignmentShapeRatioHelper.CalcParallaxFromModelCoord(new float3(-0.5f, 0.5f, -0.5f), mvpR, mvpL, resolutionW / 2, 0.4843f);

            //Calc Xi in mm
            var xiP1 = AssignmentShapeRatioHelper.CalcXi(new float3(-0.5f, 0.5f, -0.5f), EyeSeparation, mvpR, mvpL, resolutionW, 0.4843f);
            var xiP2 = AssignmentShapeRatioHelper.CalcXi(new float3(0.5f, 0.5f, -0.5f), EyeSeparation, mvpR, mvpL, resolutionW, 0.4843f);
            var ziP1 = AssignmentShapeRatioHelper.CalcZi(new float3(-0.5f, 0.5f, -0.5f), EyeSeparation, mvpR, mvpL, resolutionW, 0.4843f, 2500);

            // Calc DepthMag3D
            var withMag3D = AssignmentShapeRatioHelper.CalcWidthMag3D(xiP1, xiP2, 100);

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

            var debugR = new[]
            {
                screenCoord1, screenCoord2, screenCoord3,
            };

            foreach (var item in debugR)
            {
                Debug.WriteLine(item);
            }

            var cubeTransform = (TransformComponent)_sceneBc.Children[0].Children[0].Components[0];
            cubeTransform.Rotation = new float3(_angleVert, _angleHorz, 0);

            #endregion

            #region Control FOV
            var fovDelta = _fov + Input.Mouse.WheelVel * 0.001f;
            _fov += fovDelta > 0.01f && fovDelta < M.Pi ? Input.Mouse.WheelVel * 0.001f : 0;
            #endregion
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
            _sceneA = AssetStorage.Get<SceneContainer>("RocketModel.fus");

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_sceneA);
        }

        private void HotLoadSceneGroupD()
        {
            // Load the rocket model
            _sceneD = AssetStorage.Get<SceneContainer>("RocketModel.fus");

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_sceneD);
        }

        // Reset ModelView and ProjectionMatrix between group switch
        private void ResetAllParams()
        {
            RC.ModelView = float4x4.LookAt(0, 0, -10, 0, 0, 0, 0, 1, 0);

            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (Height / 2f);
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;

            // Fullscreen
            SetWindowSize(1920, 1080, 0, 0, true);

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
            _aspectRatio = Width / (float)(Height);

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(_fov, _aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}