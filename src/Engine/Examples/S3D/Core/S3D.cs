using System;
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

        #region GUI ShapeRatio
        private GUIHandler _guiHandler;
        private GUIText _distToCamTextOne;
        private GUIText _shapeRatioTextOne;
        private GUIText _distToCamTextTwo;
        private GUIText _shapeRatioTextTwo;
        private GUIText _fovText;
        private FontMap _guiLatoBlackMap;
        private GUIText _descriptionShapeRatio;
        #endregion

        #region GUI Hyper- Hypostereo
        private GUIText _guiSubText3DWidthMag;
        private GUIText _guiSubTextInteraxials;
        private GUIText _guiSubTextConvergencePlane;
        private GUIText _descriptionHyperHypo;
        #endregion

        private TransformComponent _cubeTransform;
        private TransformComponent _sphereTransform;
        private float4x4 _sphereModelMatrix;

        private SceneContainer _sceneShapeRatio;
        private SceneContainer _sceneHyperHypoStereo;
        private SceneRenderer _sceneRenderer;

        private bool _keys;

        private enum Assignment
        {
            SHAPERATIO,
            HYPERHYPOSTEREO
        }

        private static Assignment _assignment = Assignment.SHAPERATIO;

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Fullscreen
            SetWindowSize(Utility.ResolutionW, Utility.ResolutonH, 0, 0, true);

            #region Initialize members shape ratio scene
            _sceneShapeRatio = Utility.CreateScene(RC);
            _cubeTransform = (TransformComponent)_sceneShapeRatio.Children[0].Children[0].Components[0];
            _sphereTransform = (TransformComponent)_sceneShapeRatio.Children[0].Children[0].Children[0].Components[0];
            _sphereModelMatrix = _sphereTransform.Matrix();
            Utility.CamPosBc = new float3(0, 0, -Utility.CamOffset);
            #endregion

            _sceneHyperHypoStereo = AssetStorage.Get<SceneContainer>("Sandbox.fus");

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_sceneShapeRatio);

            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);
            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            fontLato.UseKerning = true;

            #region GUI Shape Ratio
            var textColor = ColorUint.Tofloat4(ColorUint.Greenery);
            _guiLatoBlackMap = new FontMap(fontLato, 18);

            _descriptionShapeRatio = new GUIText(" ", _guiLatoBlackMap, 50, 50) { TextColor = textColor };
            _distToCamTextOne = new GUIText("Distance Camera to Cube", _guiLatoBlackMap, 50, 100) { TextColor = textColor };
            _shapeRatioTextOne = new GUIText("Shape ratio Cube", _guiLatoBlackMap, 50, 130) { TextColor = textColor };

            _distToCamTextTwo = new GUIText("Distance to Sphere", _guiLatoBlackMap, 50, 180) { TextColor = textColor };
            _shapeRatioTextTwo = new GUIText("Shape ratio Sphere", _guiLatoBlackMap, 50, 210) { TextColor = textColor };
            _fovText = new GUIText("Fiel of View (degree)", _guiLatoBlackMap, 50, 260) { TextColor = textColor };
            #endregion

            #region GUI Hyper- Hypostereo
            textColor = new float4(1f, 1f, 1f, 1f);
            _guiSubText3DWidthMag = new GUIText("3D Width Magnification (based on the red house):", _guiLatoBlackMap, 50, 100) { TextColor = textColor };
            _guiSubTextInteraxials = new GUIText("AD-Axis - Interaxial distance in mm:", _guiLatoBlackMap, 50, 130) { TextColor = textColor };
            _guiSubTextConvergencePlane = new GUIText("WS-Axis - Convergence plane in m:", _guiLatoBlackMap, 50, 160) { TextColor = textColor };
            _descriptionHyperHypo = new GUIText(" ", _guiLatoBlackMap, 50, 200) { TextColor = textColor };
            #endregion

            AddInitialGuiElements();
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

                Utility.Interaxial = 0.2f;
                Utility.ConvergenceDist = 10;
                
                _guiHandler.Add(_distToCamTextOne);
                _guiHandler.Add(_shapeRatioTextOne);
                _guiHandler.Add(_distToCamTextTwo);
                _guiHandler.Add(_shapeRatioTextTwo);
                _guiHandler.Add(_fovText);

                _sceneRenderer = new SceneRenderer(_sceneShapeRatio);
                _assignment = Assignment.SHAPERATIO;
            }
            if (Input.Keyboard.GetKey(KeyCodes.F2))
            {
                ResetAllParams();

                Utility.Interaxial = 0.065f;
                Utility.ConvergenceDist = -Utility.Cam2ObjDistanceHyperHypo;
                
                _guiHandler.Add(_guiSubText3DWidthMag);
                _guiHandler.Add(_guiSubTextInteraxials);
                _guiHandler.Add(_guiSubTextConvergencePlane);
                _guiHandler.Add(_descriptionHyperHypo);

                _sceneRenderer = new SceneRenderer(_sceneHyperHypoStereo);
                _assignment = Assignment.HYPERHYPOSTEREO;
            }

            // Call group Methods
            switch (_assignment)
            {
                case Assignment.SHAPERATIO:
                    ShapeRatio();
                    break;
                case Assignment.HYPERHYPOSTEREO:
                    HyposteroHyperstereo();
                    break;
                default:
                    new NotSupportedException($"Assignment {_assignment} not supported.");
                    break;
            }

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        // B) Are there perceptible shape ratio changes (high, low fov values)?
        //Assumption: 1 Fusee unit equals 1 decimeter
        private void ShapeRatio()
        {
            #region LEFT Camera setup

            var mtxCam = float4x4.LookAt(-Utility.Interaxial / 2f, 0, -Utility.CamOffset, -Utility.Interaxial / 2f, 0, 0, 0, 1, 0);

            RC.ModelView = mtxCam;

            const int n = 1;
            const int f = 20000;
            var tanFov = (float)System.Math.Tan(_fov / 2);
            var a = _aspectRatio * tanFov * Utility.ConvergenceDist;

            RC.Projection = CreateLeftAssymCamMatrix(n, f, tanFov, a);

            RC.Viewport(-Utility.HitInPx, 0 - Utility.HitInPx, Width / 2 + Utility.HitInPx, Height + Utility.HitInPx);
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

            mtxCam = float4x4.LookAt(Utility.Interaxial / 2f, 0, -Utility.CamOffset, Utility.Interaxial / 2f, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam;

            RC.Projection = CreateRightAssymCamMatrix(n, f, tanFov, a);

            RC.Viewport(Width / 2, 0 - Utility.HitInPx, Width / 2 + Utility.HitInPx, Height + Utility.HitInPx);
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

            //Control fov
            var fovDelta = _fov + Input.Mouse.WheelVel * 0.001f;
            _fov += fovDelta > 0.01f && fovDelta < M.Pi ? Input.Mouse.WheelVel * 0.001f : 0;

            //Control convergence plane
            Utility.ConvergenceDist += Input.Keyboard.WSAxis * 0.01f;

            // Update Scene
            var convPlane = (TransformComponent)_sceneShapeRatio.Children.FindNodes(x => x.Name == "ConvergencePlane").First().Components[0];
            if (convPlane != null)
                convPlane.Translation = new float3(0, 0, -Utility.CamOffset + Utility.ConvergenceDist);

            //Rotate Cube and its children via mouse
            _cubeTransform.Rotation = new float3(AngleVert, AngleHorz, 0);
            #endregion

            #region Shape ratio calculation

            var distCamToObjOne = Utility.CamOffset + Utility.ObjOneDistToRootShapeRatio;
            var sphereModelMat = _cubeTransform.Matrix() * _sphereModelMatrix;
            var sphereWorldPos = new float3(sphereModelMat.M14, sphereModelMat.M24, sphereModelMat.M34);
            var distCamToObjTwo = (sphereWorldPos - Utility.CamPosBc).z;

            var interaxialInMm = Utility.Interaxial * 100;
            var convPlaneWInMm = a * 2 * 100; //"a" is half of the convergence plane width in the viewing frustum calculation
            var convDistInMm = Utility.ConvergenceDist * 100;

            var shapeRatioObjOne = Utility.CalcRoundnessFactor(interaxialInMm, Utility.ViewingDistance, convPlaneWInMm, distCamToObjOne * 100, Utility.EyeSeparationMm, Utility.PhysicalDisplayWidth, convDistInMm);
            var shapeRatioObjTwo = Utility.CalcRoundnessFactor(interaxialInMm, Utility.ViewingDistance, convPlaneWInMm, distCamToObjTwo * 100, Utility.EyeSeparationMm, Utility.PhysicalDisplayWidth, convDistInMm);
            #endregion

            #region GUI
            _distToCamTextOne.Text = "Distance to camera Cube (Fusee units): " + distCamToObjOne;
            _shapeRatioTextOne.Text = "Calculated shape ratio Cube: " + shapeRatioObjOne;
            _distToCamTextTwo.Text = "Distance to camera Sphere (Fusee units): " + distCamToObjTwo;
            _shapeRatioTextTwo.Text = "Calculated shape ratio Sphere: " + shapeRatioObjTwo;
            _fovText.Text = "Field of View (degree): " + M.RadiansToDegrees(_fov);
            _descriptionShapeRatio.Text = "Move the convergence plane for- and backwards by pressing W or S, change FOV by moving the mouse wheel.";

            #endregion
        }

        //  D) Are there Hyper- or Hypostereo effects when changeing the interaxial distance?   
        private void HyposteroHyperstereo()
        {
            // Get interaxials delta from the keyboard controls     
            var interaxialsDelta = Input.Keyboard.ADAxis * 0.0006f;//0.0001f;            
            Utility.Interaxial += interaxialsDelta;

            // Stop interaxials from getting below zero
            if (Utility.Interaxial < 0)
                Utility.Interaxial = 0;

            // Set ineraxial distance for both cameras calculate from the midpoint
            var interaxialRight = Utility.Interaxial / 2;
            var interaxialLeft = -interaxialRight;

            // Set distance for near clipping plane and far clipping plane and adjust convergence/zpp plane
            const int n = 1;
            const int f = 20000;
            Utility.ConvergenceDist += Input.Keyboard.WSAxis * 0.01f;

            // Set rotation matrices. RotCam defines correct cam location, RotMouse adjusts the cam location based on mouse angle inpout.
            var mtxRotCam = float4x4.CreateRotationX(0.25f)* float4x4.CreateRotationY(1.5f);
            var mtxRotMouse = float4x4.CreateRotationX(AngleVert) * float4x4.CreateRotationY(AngleHorz);
            var mtxCam = float4x4.LookAt(interaxialLeft, 1.5f, Utility.Cam2ObjDistanceHyperHypo, interaxialLeft, 0, 0, 0, 1, 0);

            #region LEFT EYE CAMERA 
            RC.ModelView = mtxCam * mtxRotMouse * mtxRotCam;

            // Calculate parameters for asymetric viewing frustums/off-axis alignment.
            var tanFov = (float)System.Math.Tan(_fov / 2);

            var a = _aspectRatio * tanFov * Utility.ConvergenceDist;
            
            RC.Projection = CreateLeftAssymCamMatrix(n, f, tanFov, a);

            // Store mvp for later calculation of 3d width magnification
            var mvpL = RC.ModelViewProjection;

            // Set current viewport for the left eye camera according to side by side spec for s3d rendering
            RC.Viewport(0, 0, Width / 2, Height);

            // Render the scene loaded in Init() with specs for the left eye camera
            _sceneRenderer.Render(RC);
            _guiHandler.RenderGUI();
            #endregion

            #region RIGHT EYE CAMERA           
            mtxCam = float4x4.LookAt(interaxialRight, 1.5f, Utility.Cam2ObjDistanceHyperHypo, interaxialRight, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRotMouse* mtxRotCam;

            RC.Projection = RC.Projection = CreateRightAssymCamMatrix(n, f, tanFov, a); ;

            // Store mvp for later calculation of 3d width magnification
            var mvpR = RC.ModelViewProjection;

            // Set current viewport for the right eye camera according to side by side spec for s3d rendering
            RC.Viewport(Width / 2, 0, Width / 2, Height);

            // Render the scene loaded in Init() with specs for the right eye camera          
            _sceneRenderer.Render(RC);
            _guiHandler.RenderGUI();
            #endregion

            // Calculate the physical pixel width on the display and convert it into fusee metrics (relative to 1 m)
            const float pixelWidth = Utility.PhysicalDisplayWidth / Utility.ResolutionW / 1000;

            // Calculate the 3d width magnification specified by Smith & Collar
            var widthMagnification = Utility.CalcWidthMag3D(Utility.LeftUpperCorner, Utility.RightUpperCorner, Utility.EyeSeparationM,
                mvpR, mvpL, Utility.ResolutionW, pixelWidth, (Utility.RightUpperCorner - Utility.LeftUpperCorner).x);

            // Update dynamic parameters showed in the GUI
            _guiSubText3DWidthMag.Text = "3D Width Magnification (based on the red house): " + widthMagnification;
            _guiSubTextInteraxials.Text = "AD-Axis - Interaxial distance in mm: " + Utility.Interaxial * 1000;
            _guiSubTextConvergencePlane.Text = "WS-Axis - Convergence plane in m: " + -Utility.ConvergenceDist;
            _descriptionHyperHypo.Text = "Press mousebutton for movement, A&D-buttons for interaxial, W&S-buttons for convergence plane";
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

        // Reset ModelView and ProjectionMatrix between group switch
        private void ResetAllParams()
        {
            _fov = M.PiOver4;
            RC.ModelView = float4x4.LookAt(0, 0, -10, 0, 0, 0, 0, 1, 0);
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (Height / 2f);
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;

            _guiHandler.Clear();

            // Fullscreen
            SetWindowSize(1920, 1080, 0, 0, true);
        }

        private void AddInitialGuiElements()
        {
            _guiHandler.Add(_distToCamTextOne);
            _guiHandler.Add(_shapeRatioTextOne);
            _guiHandler.Add(_distToCamTextTwo);
            _guiHandler.Add(_shapeRatioTextTwo);
            _guiHandler.Add(_fovText);
            _guiHandler.Add(_descriptionShapeRatio);
        }

        private float4x4 CreateLeftAssymCamMatrix(float n, float f, float tanFov, float a)
        {
            var top = n * tanFov;
            var bottom = -top;

            var b = a - Utility.Interaxial / 2;
            var c = a + Utility.Interaxial / 2;

            var left = -b * n / Utility.ConvergenceDist;
            var right = c * n / Utility.ConvergenceDist;


            return float4x4.CreatePerspectiveOffCenter(left, right, bottom, top, n, f);
        }

        private float4x4 CreateRightAssymCamMatrix(float n, float f, float tanFov, float a)
        {
            var top = n * tanFov;
            var bottom = -top;

            var b = a - Utility.Interaxial / 2;
            var c = a + Utility.Interaxial / 2;

            var left = -c * n / Utility.ConvergenceDist;
            var right = b * n / Utility.ConvergenceDist;

            return float4x4.CreatePerspectiveOffCenter(left, right, bottom, top, n, f);
        }
    }
}