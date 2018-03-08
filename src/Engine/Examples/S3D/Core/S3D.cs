using System;
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

        private static float _fov = M.PiOver4;
        private static float _aspectRatio;


        #region Mouse control fields
        // Horizontal and vertical rotation Angles for the displayed object 
        public static float AngleHorz, AngleVert = M.DegreesToRadians(-10);

        // Horizontal and vertical angular speed
        public static float AngleVelHorz, AngleVelVert;

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

        private TransformComponent _cubeTransform;
        private TransformComponent _sphereTransform;
        private float4x4 _sphereModelMatrix;

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
            _sceneA = AssetStorage.Get<SceneContainer>("baymax_scene.fus");

            #region Initialize members BC

            _sceneBc = UtilityBc.CreateScene(RC);
            _cubeTransform = (TransformComponent)_sceneBc.Children[0].Children[0].Components[0];
            _sphereTransform = (TransformComponent)_sceneBc.Children[0].Children[0].Children[0].Components[0];
            _sphereModelMatrix = _sphereTransform.Matrix();
            UtilityBc.CamPosBc = new float3(0, 0, -UtilityBc.CamOffset);

            #endregion


            _sceneD = AssetStorage.Get<SceneContainer>("baymax_scene.fus");

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
                AngleVelHorz = -RotationSpeed * Input.Mouse.XVel * Time.DeltaTime * 0.0005f;
                AngleVelVert = -RotationSpeed * Input.Mouse.YVel * Time.DeltaTime * 0.0005f;
            }
            else if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Input.Touch.GetVelocity(TouchPoints.Touchpoint_0);
                AngleVelHorz = -RotationSpeed * touchVel.x * Time.DeltaTime * 0.0005f;
                AngleVelVert = -RotationSpeed * touchVel.y * Time.DeltaTime * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    AngleVelHorz = -RotationSpeed * Input.Keyboard.LeftRightAxis * Time.DeltaTime;
                    AngleVelVert = -RotationSpeed * Input.Keyboard.UpDownAxis * Time.DeltaTime;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTime);
                    AngleVelHorz *= curDamp;
                    AngleVelVert *= curDamp;
                }
            }


            AngleHorz += AngleVelHorz;
            AngleVert += AngleVelVert;


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
            var mtxRot = float4x4.CreateRotationX(AngleVert) * float4x4.CreateRotationY(AngleHorz); // Create rotation around X and Y asix based upon mouse angle input
            var mtxCam = float4x4.LookAt(0, 0, -10, 0, 0, 0, 0, 1, 0); // Create Camera Matrix. Position of Camera = 0,0,-10, camera aims/looks at 0,0,0; y-axis up
            RC.ModelView = mtxCam * mtxRot; // Rotation * Cameramatrix = new RenderContext ModelView Matrix (== Cameramatrix)

            RC.Viewport(0, 0, Width, Height / 2); // Adjust the viewport and hence the visible render ouput. Start at 0,0 and fill the complete window width but only half of the window height

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC); // Render the scene to the viewport with current RC.ModelView-Matrix and current RC.Projection-Matrix!

            // Create the second camera matrix and set it as the current ModelView transformation
            mtxRot = float4x4.CreateRotationX(AngleVert) * float4x4.CreateRotationY(AngleHorz); // Create rotation around X and Y asix based upon mouse angle input
            mtxCam = float4x4.LookAt(2, 0, -10, 2, 0, 0, 0, 1, 0); // Create Camera Matrix. Position of Camera = 2,0,-10, camera aims/looks at 2,0,0; y-axis up
            RC.ModelView = mtxCam * mtxRot; // Rotation * Cameramatrix = new RenderContext ModelView Matrix (== Cameramatrix)

            RC.Viewport(0, Height / 2, Width, Height / 2); // Adjust the viewport and hence the visible render ouput. Starte bei 0 und der Hälfte der Höhe des Fensters und rendere damit den unteren Teil des Viewports.

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC); // Render the scene to the viewport with current RC.ModelView-Matrix and current RC.Projection-Matrix!

        }

        // B) Are there perceptible shape ratio changes (high, low fov values)?
        //Assumption: 1 Fusee unit equals 1 decimeter
        private void GroupBc()
        {
            SetWindowSize(UtilityBc.ResolutionW, UtilityBc.ResolutonH, 0, 0, true);

            #region LEFT Camera setup

            var mtxCam = float4x4.LookAt(-UtilityBc.Interaxial / 2f, 0, -UtilityBc.CamOffset, -UtilityBc.Interaxial / 2f, 0, 0, 0, 1, 0);

            RC.ModelView = mtxCam;//* mtxRot;

            const int n = 1;
            const int f = 20000;
            var tanFov = (float)System.Math.Tan(_fov / 2);

            var top = n * tanFov;
            var bottom = -top;

            var a = _aspectRatio * tanFov * UtilityBc.ConvergenceDist;

            var b = a - UtilityBc.Interaxial / 2;
            var c = a + UtilityBc.Interaxial / 2;

            var left = -b * n / UtilityBc.ConvergenceDist;
            var right = c * n / UtilityBc.ConvergenceDist;


            var offCenterPorjection = float4x4.CreatePerspectiveOffCenter(left, right, bottom, top, n, f);
            RC.Projection = offCenterPorjection;

            RC.Viewport(-UtilityBc.HitInPx, 0 - UtilityBc.HitInPx, Width / 2 + UtilityBc.HitInPx, Height + UtilityBc.HitInPx);
            _guiHandler.RenderGUI();

            #region Debug

            /*var screenCoord1 = UtilityBc.WorldToScreenCoord(new float3(-0.5f, 0.5f, -0.5f), RC, UtilityBc.ResolutonH, UtilityBc.ResolutionW / 2);
            var screenCoord2 = UtilityBc.WorldToScreenCoord(new float3(0.5f, 0.5f, -0.5f), RC, UtilityBc.ResolutonH, UtilityBc.ResolutionW / 2);
            var screenCoord3 = UtilityBc.WorldToScreenCoord(new float3(0.5f, 0.5f, 0.5f), RC, UtilityBc.ResolutonH, UtilityBc.ResolutionW / 2);

            var debugL = new[]
            {
                screenCoord1, screenCoord2, screenCoord3,
            };

            foreach (var item in debugL)
            {
                Debug.WriteLine(item);
            }

            // Calc prallax from ModelCoords
            var mvpL = RC.ModelViewProjection;*/

            #endregion

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

            #endregion

            #region RIGHT Camera setup

            mtxCam = float4x4.LookAt(UtilityBc.Interaxial / 2f, 0, -UtilityBc.CamOffset, UtilityBc.Interaxial / 2f, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam;

            top = n * tanFov;
            bottom = -top;
            left = -c * n / UtilityBc.ConvergenceDist;
            right = b * n / UtilityBc.ConvergenceDist;

            offCenterPorjection = float4x4.CreatePerspectiveOffCenter(left, right, bottom, top, n, f);
            RC.Projection = offCenterPorjection;

            RC.Viewport(Width / 2, 0 - UtilityBc.HitInPx, Width / 2 + UtilityBc.HitInPx, Height + UtilityBc.HitInPx);
            _guiHandler.RenderGUI();

            #region Debug

            /*
            screenCoord1 = UtilityBc.WorldToScreenCoord(new float3(-0.5f, 0.5f, -0.5f), RC, UtilityBc.ResolutonH, UtilityBc.ResolutionW / 2);
            screenCoord2 = UtilityBc.WorldToScreenCoord(new float3(0.5f, 0.5f, -0.5f), RC, UtilityBc.ResolutonH, UtilityBc.ResolutionW / 2);
            screenCoord3 = UtilityBc.WorldToScreenCoord(new float3(0.5f, 0.5f, 0.5f), RC, UtilityBc.ResolutonH, UtilityBc.ResolutionW / 2);
            

            // Calc prallax from ModelCoords in mm
            var mvpR = RC.ModelViewProjection;
            var parallaxInMm = UtilityBc.CalcParallaxFromModelCoord(new float3(-0.5f, 0.5f, -0.5f), mvpR, mvpL, UtilityBc.ResolutionW / 2, 0.4843f);

            //Calc Xi in mm
            var xiP1 = UtilityBc.CalcXi(new float3(-0.5f, 0.5f, -0.5f), UtilityBc.EyeSeparation, mvpR, mvpL, UtilityBc.ResolutionW, 0.4843f);
            var xiP2 = UtilityBc.CalcXi(new float3(0.5f, 0.5f, -0.5f), UtilityBc.EyeSeparation, mvpR, mvpL, UtilityBc.ResolutionW, 0.4843f);
            var ziP1 = UtilityBc.CalcZi(new float3(-0.5f, 0.5f, -0.5f), UtilityBc.EyeSeparation, mvpR, mvpL, UtilityBc.ResolutionW, 0.4843f, 2500);

            // Calc DepthMag3D
            var withMag3D = UtilityBc.CalcWidthMag3D(xiP1, xiP2, 100);

            var debugR = new[]
            {
                screenCoord1, screenCoord2, screenCoord3,
            };

            foreach (var item in debugR)
            {
                Debug.WriteLine(item);
            }

            Debug.WriteLine(" ");
            */

            #endregion
            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

            #endregion

            #region Controls

            //control fov
            var fovDelta = _fov + Input.Mouse.WheelVel * 0.001f;
            _fov += fovDelta > 0.01f && fovDelta < M.Pi ? Input.Mouse.WheelVel * 0.001f : 0;

            //Rotate Cube and its children via mouse
            _cubeTransform.Rotation = new float3(AngleVert, AngleHorz, 0);
            #endregion

            #region Shape ratio calculation

            var distCamToObjOne = UtilityBc.CamOffset + UtilityBc.ObjOneDistToRoot;
            var sphereModelMat = _cubeTransform.Matrix() * _sphereModelMatrix;
            var sphereWorldPos = new float3(sphereModelMat.M14, sphereModelMat.M24, sphereModelMat.M34);
            var distCamToObjTwo = (sphereWorldPos - UtilityBc.CamPosBc).z;

            const float interaxialInMm = UtilityBc.Interaxial * 100;
            var convPlaneWInMm = a * 2 * 100; //"a" is half of the convergence plane width in the viewing frustum calculation
            var convDistInMm = UtilityBc.ConvergenceDist * 100;

            var shapeRatioObjOne = UtilityBc.CalcRoundnessFactor(interaxialInMm, UtilityBc.ViewingDistance, convPlaneWInMm, distCamToObjOne * 100, UtilityBc.EyeSeparation, UtilityBc.PhysicalDisplayWidth, convDistInMm);
            var shapeRatioObjTwo = UtilityBc.CalcRoundnessFactor(interaxialInMm, UtilityBc.ViewingDistance, convPlaneWInMm, distCamToObjTwo * 100, UtilityBc.EyeSeparation, UtilityBc.PhysicalDisplayWidth, convDistInMm);

            #endregion

            #region GUI
            _distToCamTextOne.Text = "Distance to camera Cube (Fusee units): " + distCamToObjOne;
            _shapeRatioTextOne.Text = "Calculated shape ratio Sphere: " + shapeRatioObjOne;
            _distToCamTextTwo.Text = "Distance to camera green (Fusee units): " + distCamToObjTwo;
            _shapeRatioTextTwo.Text = "Calculated shape ratio green: " + shapeRatioObjTwo;
            _fovText.Text = "Field of View (degree): " + M.RadiansToDegrees(_fov);
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
            // Load the scene
            _sceneA = AssetStorage.Get<SceneContainer>("baymax_scene.fus");

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_sceneA);
        }

        private void HotLoadSceneGroupD()
        {
            // Load the scene
            _sceneD = AssetStorage.Get<SceneContainer>("baymax_scene.fus");

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