using System;

using Fusee.Base.Common;

using Fusee.Base.Core;

using Fusee.Engine.Common;

using Fusee.Engine.Core;

using Fusee.Math.Core;

using Fusee.Serialization;

using static Fusee.Engine.Core.Input;

using static Fusee.Engine.Core.Time;

using Fusee.Engine.GUI;

using Fusee.Xene;

using System.Linq;

using System.Collections.Generic;

namespace FuseeApp

{




    public enum CameraType
    {
        // Free world cam
        FREE = 0,

        // Attached to drone, mouse move rotates around drone, arrow keys moves drone

        FOLLOW,

        // Free cam follows drone, mouselook & wasd steers drone (e.g. Jetfighter)
        DRONE,
        //Resets Camera
        Reset


    }
    #region Drone
    internal class Drone
    {
        // fields 

        private float3 _position;
        private float3 _rotation;
        private float3 _yaw;
        private float ldle;
        private float _RotationSpeed;
        private float height;
        private Quaternion Orientation;
        private float3 _scale;
        private float speedx;
        private float speedz;
        private float d = 5;
        private float Yaw;
        private float Pitch;
        public float4x4 view;
        public SceneNodeContainer DroneRoot

        {
            get
            {
                return _cnt;
            }
        }
        public float RotationSpeed
        {
            get
            {
                return _RotationSpeed;
            }
            set
            {
                _RotationSpeed = value;
            }
        }
        public static float i = 0;

        private SceneNodeContainer _cnt;
        public Drone(SceneNodeContainer cnt)
        {
            _cnt = cnt;
        }

        public float3 Position
        {

            get
            {
                return DroneRoot.GetTransform().Translation;
            }
            set
            {
                _position = new float3(value);
            }

        }
        public float3 Rotation
        {
            get
            {
                return _yaw;
            }
            set
            {
                _yaw = value;
            }
        }
        public float3 Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = new float3(value);
            }
        }
        public float idle
        {
            get
            {
                return ldle;
            }
            set
            {
                ldle = value;
            }

        }
        public void Idle()
        {

            Random random = new Random();
            int rN = random.Next(0, 3);

            if (idle <= 0.5f)
            {
                _position.y += 0.0015f;

                idle += 0.004f;
            }
            if (idle > 0.5 && idle <= 1)
            {
                _position.y -= 0.0015f;
                idle += 0.004f;
            }
            if (idle >= 0.99f)
                idle = 0.01f;
        }
        public void Tilt()
        {
            if (Keyboard.WSAxis == 0)
            {
                if (_rotation.x > 0)
                    _rotation.x -= 0.01f;
                if (_rotation.x < 0)
                    _rotation.x += 0.01f;
            }

            if (Keyboard.ADAxis == 0)
            {
                if (_rotation.z > 0)
                    _rotation.z -= 0.005f;
                if (_rotation.z < 0)
                    _rotation.z += 0.005f;
            }

            if (-Keyboard.WSAxis < 0)
                if (_rotation.x > -0.2)
                    _rotation.x -= 0.005f;

            if (-Keyboard.WSAxis > 0)
                if (_rotation.x < 0.2)
                    _rotation.x += 0.005f;
            // Drone Tilt while Moving
            if (-Keyboard.ADAxis < 0)
                if (_rotation.z < 0.2)
                    _rotation.z += 0.01f;

            if (-Keyboard.ADAxis > 0)
                if (_rotation.z > -0.2)
                    _rotation.z -= 0.01f;


        }
        public Quaternion orientation(float Yaw, float Pitch)
        {
            
            Orientation = Quaternion.FromAxisAngle(float3.UnitY, Yaw) *
                            Quaternion.FromAxisAngle(float3.UnitX, Pitch);
            return Orientation;
        }
        public void MoveRotor()
        {
            var rbl = DroneRoot.Children.FindNodes(node => node.Name == "Rotor back left")?.FirstOrDefault()?.GetTransform();
            var rfl = DroneRoot.Children.FindNodes(node => node.Name == "Rotor front left")?.FirstOrDefault()?.GetTransform();
            var rfr = DroneRoot.Children.FindNodes(node => node.Name == "Rotor front right")?.FirstOrDefault()?.GetTransform();
            var rbr = DroneRoot.Children.FindNodes(node => node.Name == "Rotor back right")?.FirstOrDefault()?.GetTransform();

            if (i <= 23)
                i += 0.05f;

            rbl.Rotation.y = i * TimeSinceStart;
            rfl.Rotation.y = i * TimeSinceStart;
            rfr.Rotation.y = i * TimeSinceStart;
            rbr.Rotation.y = i * TimeSinceStart;

        }
        public float4x4 Update(CameraType _cameraType)
        {
            Rotation = DroneRoot.GetTransform().Rotation;
            _scale = DroneRoot.GetTransform().Scale;
            Idle();
            Tilt();
            MoveRotor();
            var camPosOld = new float3(Position.x, Position.y + 1, Position.z - d);
            var DroneposOld = DroneRoot.GetTransform().Translation;
            float mouse = 0;
            var rot = _rotation.y;
            if (Mouse.LeftButton)
            {
                mouse = (Mouse.XVel * 0.0005f);
            }

            _rotation.y = _rotation.y + mouse;

            if (Keyboard.WSAxis == 0)
                speedx = 0.02f;

            if (Keyboard.ADAxis == 0)
                speedz = 0.02f;

            // if (Keyboard.WSAxis != 0)
            if (speedx <= 0.5f)
                speedx += 0.005f;

            if (Keyboard.ADAxis != 0)
                if (speedz <= 0.5f)
                    speedz += 0.005f;


            float posVelX = -Keyboard.WSAxis * speedx * (DeltaTime * 15);
            float posVelZ = -Keyboard.ADAxis * speedz * (DeltaTime * 15);
            float3 newPos = DroneposOld;

            newPos += float3.Rotate(orientation(_rotation.y, 0), float3.UnitX * posVelZ);
            newPos += float3.Rotate(orientation(_rotation.y, 0), float3.UnitZ * posVelX);

            // Height
            if (Keyboard.GetKey(KeyCodes.R))
                newPos.y += 0.1f;
            if (Keyboard.GetKey(KeyCodes.F))
            {
                height = 0.1f;
                if (newPos.y <= 0.5f)
                    height = 0;
                newPos.y -= height;
            }
            Position = newPos;

            var posVec = float3.Normalize(camPosOld - Position);
            var camposnew = Position + posVec * d;
            if (Mouse.RightButton)
            {
                Yaw += Mouse.XVel * 0.0005f;
                Pitch += Mouse.YVel * 0.0005f;
            }
            if (Keyboard.GetKey(KeyCodes.Z))
            {
                Scale = new float3(Scale.x + 0.01f, Scale.y + 0.01f, Scale.z + 0.01f);
                d += 0.1f;
            }
            if (Keyboard.GetKey(KeyCodes.X))
                if (Scale.y + Scale.x + Scale.z >= 0.03)
                {
                    Scale = new float3(Scale.x - 0.01f, Scale.y - 0.01f, Scale.z - 0.01f);
                    d -= 0.1f;
                }

            if (_cameraType == CameraType.DRONE)
            {
                view = float4x4.LookAt(
                                                      new float3(DroneposOld) + d * float3.Rotate(orientation(rot, -0.3f),float3.UnitZ),
                                                      new float3(Position),
                                                      float3.UnitY
                                                      );
            }
            if (_cameraType == CameraType.FOLLOW)
            {
                view = float4x4.LookAt(
                                                     new float3(DroneposOld) + d * float3.Rotate(orientation(Yaw, Pitch),float3.UnitZ),
                                                     new float3(Position),
                                                     float3.UnitY
                                                     );
            }

            var Drone = DroneRoot;
            Drone.GetTransform().Translation = _position;
            Drone.GetTransform().Rotation = _rotation;
            Drone.GetTransform().Scale = _scale;
            return view;
        }
    }
    #endregion

    #region Camera
    internal class Camera
    {
        public float3 _Position;
        public float4x4 view;
        private CameraType _cameraType;
        public float _Yaw;
        public float _Pitch;
        private float _MouseSensitivity;
        public Camera()
        {

        }
        public float MovementSpeed
        {
            get
            {
                return MovementSpeed;
            }
            set
            {
                MovementSpeed = value;
            }
        }
        public float MouseSensitivity
        {
            get
            {
                return _MouseSensitivity;
            }
            set
            {
                _MouseSensitivity = value;
            }
        }
        public float Yaw
        {
            get
            {
                float yaw = 0;
                if (Mouse.RightButton)
                    yaw = Mouse.XVel * MouseSensitivity;
                return _Yaw += yaw;
            }
        }
        public float Pitch
        {
            get
            {
                float pitch = 0;
                if (Mouse.RightButton)
                    pitch = Mouse.YVel * MouseSensitivity;
                return _Pitch += pitch;
            }
        }
        public float3 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = new float3(value);
            }
        }
        public Quaternion Rotation
        {
            get
            {
                return Rotation;
            }
            set
            {
                Rotation = value;
            }
        }
        public CameraType cameraType
        {
            get
            {
                return _cameraType;
            }

            set
            {
                _cameraType = ((int)_cameraType + 1) <= 2 ? value : 0;

            }

        }
        public float3 ForwardVector
        {
            get
            {
                var Orientation = Quaternion.FromAxisAngle(float3.UnitY, Yaw) * Quaternion.FromAxisAngle(float3.UnitX, Pitch);
                return float3.Rotate(Orientation, float3.UnitZ);
            }
        }
        public float4x4 ViewMatrix
        {
            get
            {
                return float4x4.LookAt(Position, Position + ForwardVector, float3.UnitY);
            }
        }
        public void SetCameraType()
        {
            // cameraType++;
            // Diagnostics.Log("Der Camera Typ ist " + _cameraType);
        }
        public void SetPositionLocally(float3 pos)
        {
            view = ViewMatrix;
        }
        public float4x4 Update(CameraType _cameraType)
        {
            MouseSensitivity = 0.00005f;
            if (cameraType == CameraType.FREE)
            {

                Position += float3.Rotate(Quaternion.FromAxisAngle(float3.UnitY, Yaw) * Quaternion.FromAxisAngle(float3.UnitX, Pitch), float3.UnitX * (Keyboard.ADAxis * DeltaTime * 8));
                Position += float3.Rotate(Quaternion.FromAxisAngle(float3.UnitY, Yaw) * Quaternion.FromAxisAngle(float3.UnitX, Pitch),float3.UnitZ * (Keyboard.WSAxis * DeltaTime * 8));
                Position += float3.Rotate(Quaternion.FromAxisAngle(float3.UnitY, Yaw) * Quaternion.FromAxisAngle(float3.UnitX, Pitch), float3.UnitY * (Keyboard.UpDownAxis * DeltaTime * 8));
                if (_cameraType == CameraType.FREE)
                    SetPositionLocally(Position);
            }
            return view;

        }

    }
    #endregion

    [FuseeApplication(Name = "Droneflight", Description = "Droneflight Demo")]
    public class DroneDemo : RenderCanvas

    {
        private Camera _camera;
        private Drone _drone;
        private float4x4 view;

        // Variables init
        private const float RotationSpeed = 7;
        public SceneContainer _droneScene;
        private SceneRenderer _sceneRenderer;
        private SceneRenderer _guiRenderer;
        private SceneNodeContainer DroneRoot;
        private CameraType _cameraType;
        private SceneContainer _gui;
        public String _text;
        private float _initWindowWidth;
        private float _initWindowHeight;
        private float _initCanvasWidth;
        private float _initCanvasHeight;
        private const float ZNear = 1f;
        private const float ZFar = 3000;
        private float _aspectRatio;
        private float _fovy = M.PiOver4;
        private float wait;
        private float _canvasWidth = 16;
        private float _canvasHeight = 9;


        // Init is called on startup. 
        public override void Init()

        {

            _initWindowWidth = Width;
            _initWindowHeight = Height;

            _initCanvasWidth = Width / 100f;
            _initCanvasHeight = Height / 100f;

            _canvasHeight = _initCanvasHeight;
            _canvasWidth = _initCanvasWidth;

            _aspectRatio = Width / (float)Height;
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).

            RC.ClearColor = new float4(0.7f, 0.9f, 0.5f, 1);




            // Load the drone model
            _droneScene = AssetStorage.Get<SceneContainer>("GroundNoMat.fus");
            var droneBody = _droneScene.Children.FindNodes(node => node.Name == "Body")?.FirstOrDefault();
            _gui = CreateGui();
            _drone = new Drone(droneBody);

            _camera = new Camera();

            //Add resize delegate
            var projComp = _droneScene.Children[0].GetComponent<ProjectionComponent>();
            AddResizeDelegate(delegate { projComp.Resize(Width, Height); });

            // Wrap a SceneRenderer around the model.

            _sceneRenderer = new SceneRenderer(_droneScene);
            _guiRenderer = new SceneRenderer(_gui);

            DroneRoot = _droneScene.Children.FindNodes(node => node.Name == "Body")?.FirstOrDefault();
            
            
            } 


        // RenderAFrame is called once a frame
        public override void RenderAFrame()

        {
            // Clear the backbuffer

            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            
            

            // Switch between Drone and Freefly            
            if (_cameraType == CameraType.Reset)
                _cameraType = CameraType.FREE;

            wait++;

            if (wait >= 25)
                if (Keyboard.IsKeyUp(KeyCodes.Q))
                {
                    _cameraType++;
                    wait = 0;

                    Diagnostics.Log(_cameraType);
                }

            if (_cameraType == CameraType.FREE)
                view = _camera.Update(_cameraType);
            if (_cameraType == CameraType.FOLLOW || _cameraType == CameraType.DRONE)
                view = _drone.Update(_cameraType);

            RC.View = view;



            // Render the scene loaded in Init()

            _sceneRenderer.Render(RC);

            var projection = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);
            RC.Projection = projection;

          //  _guiRenderer.Render(RC);

            projection = float4x4.CreatePerspectiveFieldOfView(_fovy, _aspectRatio, ZNear, ZFar);
            RC.Projection = projection;

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.

            Present();

        }

        private InputDevice Creator(IInputDeviceImp device)

        {

            throw new NotImplementedException();

        }
        public override void Resize(ResizeEventArgs e)
        {
            
        }
        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");


            // Initialize the information text line.
            var textToDisplay = "Drone Demo";
            if (_droneScene.Header.CreatedBy != null || _droneScene.Header.CreationDate != null)
            {
                textToDisplay += " created";
                if (_droneScene.Header.CreatedBy != null)
                    textToDisplay += " by " + _droneScene.Header.CreatedBy;

                if (_droneScene.Header.CreationDate != null)
                    textToDisplay += " on " + _droneScene.Header.CreationDate;
            }

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 18);

            var text = new TextNodeContainer(
                textToDisplay,
                "SceneDescriptionText",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_HORIZONTAL),
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_HORIZONTAL, new float2(_initCanvasWidth / 2 - 4, 0), _initCanvasHeight, _initCanvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery), 200f);


            var canvas = new CanvasNodeContainer(
                "Canvas",
                CanvasRenderMode.SCREEN,
                new MinMaxRect
                {
                    Min = new float2(-_canvasWidth / 2, -_canvasHeight / 2f),
                    Max = new float2(_canvasWidth / 2, _canvasHeight / 2f)
                });
            canvas.Children.Add(text);

            //Create canvas projection component and add resize delegate
            var canvasProjComp = new ProjectionComponent(ProjectionMethod.ORTHOGRAPHIC, ZNear, ZFar, _fovy);
            canvas.Components.Insert(0, canvasProjComp);
            AddResizeDelegate(delegate { canvasProjComp.Resize(Width, Height); });

            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    //Add canvas.
                    canvas
                }
            };
        }
    }

}