using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Engine.SimpleScene;
using Fusee.Math;
using Fusee.Serialization;
using Microsoft.Kinect;

namespace Examples.HandOnCanvasKinect2
{
    public class HandOnCanvasKinect2 : RenderCanvas
    {
        // angle variables
        private float _angleHorz, _angleVert;

        private const float RotationSpeed = 30f;
        private const float MaxRotChange = 40f;
        private const float Damping = 1f;

        // model variables
        private Mesh _meshHandOpen;
        private Mesh _meshHandClosed;
        private Mesh _meshHandIndex;
        private SceneRenderer _sr;
        private float4x4 _modelScaleOffset;

        // variables for shader
        private ShaderEffect _shaderEffect;

        // <Kinect>
        private float _normWidth;
        private float _normHeight;
        private KinectSensor _kinectSensor;
        private CoordinateMapper _coordinateMapper;
        private int _jointSpaceWidth;
        private int _jointSpaceHeight;
        private BodyFrameReader _bodyFrameReader;
        private Body[] _bodies;
        private float3 _handPos;
        private float3 _handVel;
        private float3 _handPosLast;
        private HandState _rightHandState;
        private float _handPitch;
        private float _handYaw;
        private const float InferredZPositionClamp = 0.1f;
        private float3 _pHand;
        private float3 _pTip;
        private float _filterAlpha = 0.5f;
        // </Kinect>

        // GUI
        private GUIHandler _guiHandler;
        private GUIImage _guiFuseeLogo;
        private GUIButton _guiFuseeLink;
        private GUIText _guiSubText;
        private IFont _guiLatoBlack;
        private float _subtextWidth;
        private float _subtextHeight;
        private float _angleVelHorz;
        private float _angleVelVert;


        private const float LineWidth = 1.7f;
        private const float CamDist = 500.0f;
        private const float SquareScreenPxls = 2048.0f;
        private const float HandScale = 0.5f;

        private void _guiFuseeLink_OnGUIButtonLeave(GUIButton sender, MouseEventArgs mea)
        {
            _guiFuseeLink.ButtonColor = new float4(0, 0, 0, 0);
            _guiFuseeLink.BorderWidth = 0;
            SetCursor(CursorType.Standard);
        }


        private void _guiFuseeLink_OnGUIButtonEnter(GUIButton sender, MouseEventArgs mea)
        {
            _guiFuseeLink.ButtonColor = new float4(0, 0.6f, 0.2f, 0.4f);
            _guiFuseeLink.BorderWidth = 1;
            SetCursor(CursorType.Hand);
        }

        void _guiFuseeLink_OnGUIButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            OpenLink("http://fusee3d.org");
        }

        public void AdjustModelScaleOffset()
        {
            AABBf? box = null;
            if (_sr == null || (box = _sr.GetAABB()) == null)
            {
                _modelScaleOffset = float4x4.Identity;
            }
            var bbox = ((AABBf)box);
            float scale = Math.Max(Math.Max(bbox.Size.x, bbox.Size.y), bbox.Size.z);
            _modelScaleOffset = float4x4.CreateScale(500.0f / scale) * float4x4.CreateTranslation(-bbox.Center);
        }


        private void InitShader()
        {
            var imgData = RC.LoadImage("Assets/art_billard.jpg");
            var iTex = RC.CreateTexture(imgData);

            _shaderEffect = new ShaderEffect(
                new[]
                {
                    new EffectPassDeclaration
                    {
                        VS = @"
                                attribute vec4 fuColor;
                                attribute vec3 fuVertex;
                                attribute vec3 fuNormal;
                                attribute vec2 fuUV;
                    
                                varying vec4 vColor;
                                varying vec3 vNormal;
                                varying vec2 vUV;
        
                                uniform mat4 FUSEE_MVP;
                                uniform mat4 FUSEE_ITMV;

                                uniform vec2 uLineWidth;

                                void main()
                                {
                                    vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                                    vNormal = normalize(vNormal);
                                    gl_Position = (FUSEE_MVP * vec4(fuVertex, 1.0) ) + vec4(uLineWidth * vNormal.xy, 0, 0) + vec4(0, 0, 0.06, 0);
                                    vUV = fuUV;
                                }",
                        PS = @"
                                #ifdef GL_ES
                                    precision highp float;
                                #endif
        
                                uniform vec4 uLineColor;
                                varying vec3 vNormal;

                                void main()
                                {
                                    gl_FragColor = uLineColor;
                                }",
                        StateSet = new RenderStateSet
                        {
                            AlphaBlendEnable = false,
                            ZEnable = true
                        }
                    },
                    new EffectPassDeclaration
                    {
                        VS = @"
                                attribute vec4 fuColor;
                                attribute vec3 fuVertex;
                                attribute vec3 fuNormal;
                                attribute vec2 fuUV;
                    
                                varying vec3 vNormal;
                                varying vec2 vUV;
        
                                uniform mat4 FUSEE_MVP;
                                uniform mat4 FUSEE_ITMV;

                                void main()
                                {
                                    gl_Position = (FUSEE_MVP * vec4(fuVertex, 1.0) );
                                    vNormal = normalize(mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal);
                                    vUV = fuUV;
                                }",
                        PS = @"
                                #ifdef GL_ES
                                    precision highp float;
                                #endif
        
                                uniform sampler2D texture1;

                                varying vec3 vNormal;
                                varying vec2 vUV;

                                void main()
                                {
                                    gl_FragColor = vec4(texture2D(texture1, vNormal.xy * 0.5 + vec2(0.5, 0.5)).rgb, 0.85);
                                }",
                        StateSet = new RenderStateSet
                        {
                            AlphaBlendEnable = false,
                            ZEnable = true,

                            //AlphaBlendEnable = true,
                            //ZEnable = true,
                            //BlendFactor = new float4(0.5f, 0.5f, 0.5f, 0.5f),
                            //BlendOperation = BlendOperation.Add,
                            //SourceBlend = Blend.BlendFactor,
                            //DestinationBlend = Blend.InverseBlendFactor

                        }
                    }
                },
                new[]
                {
                    new EffectParameterDeclaration {Name = "uLineColor", Value = new float4(0, 0, 0, 1)},
                    new EffectParameterDeclaration {Name = "texture1", Value = iTex},
                    new EffectParameterDeclaration {Name = "uLineWidth", Value = new float2(5, 5)}
                });
            _shaderEffect.AttachToContext(RC);
        }

        // is called on startup
        public override void Init()
        {
            InitShader();
            RC.ClearColor = new float4(1f, 1f, 1f, 0.0f);
            // _meshHand = MeshReader.LoadMesh(@"Assets/handhipolynorm.obj.model");

            SceneContainer scene;
            var ser = new Serializer();
            using (var file = File.OpenRead(@"Assets/HandOpen.fus"))
            {
                scene = ser.Deserialize(file, null, typeof(SceneContainer)) as SceneContainer;
                _meshHandOpen = SceneRenderer.MakeMesh(scene.Children[0]);
            }
            using (var file = File.OpenRead(@"Assets/HandClosed.fus"))
            {
                scene = ser.Deserialize(file, null, typeof(SceneContainer)) as SceneContainer;
                _meshHandClosed = SceneRenderer.MakeMesh(scene.Children[0]);
            }
            using (var file = File.OpenRead(@"Assets/HandIndex.fus"))
            {
                scene = ser.Deserialize(file, null, typeof(SceneContainer)) as SceneContainer;
                _meshHandIndex = SceneRenderer.MakeMesh(scene.Children[0]);
            }


            // Pyramidlogo
            // Scene loading
            using (var file = File.OpenRead(@"Assets/Pyramid.fus"))
            {
                scene = ser.Deserialize(file, null, typeof(SceneContainer)) as SceneContainer;
            }
            _sr = new SceneRenderer(scene, "Assets");
            AdjustModelScaleOffset();


            // GUI initialization
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);

            _guiFuseeLink = new GUIButton(6, 6, 157, 87);
            _guiFuseeLink.ButtonColor = new float4(0, 0, 0, 0);
            _guiFuseeLink.BorderColor = new float4(0, 0.6f, 0.2f, 1);
            _guiFuseeLink.BorderWidth = 0;
            _guiFuseeLink.OnGUIButtonDown += _guiFuseeLink_OnGUIButtonDown;
            _guiFuseeLink.OnGUIButtonEnter += _guiFuseeLink_OnGUIButtonEnter;
            _guiFuseeLink.OnGUIButtonLeave += _guiFuseeLink_OnGUIButtonLeave;
            _guiHandler.Add(_guiFuseeLink);

            _guiFuseeLogo = new GUIImage("Assets/FuseeLogo150.png", 10, 10, -5, 150, 80);
            _guiHandler.Add(_guiFuseeLogo);

            _guiLatoBlack = RC.LoadFont("Assets/Lato-Black.ttf", 14);
            _guiSubText = new GUIText("FUSEE 3D Scene Viewer", _guiLatoBlack, 100, 100);
            _guiSubText.TextColor = new float4(0.05f, 0.25f, 0.15f, 0.8f);
            _guiHandler.Add(_guiSubText);

            // <Kinect>
            // one sensor is currently supported
            _kinectSensor = KinectSensor.GetDefault();

            // get the coordinate mapper
            _coordinateMapper = _kinectSensor.CoordinateMapper;

            // get the depth (display) extents
            FrameDescription frameDescription = _kinectSensor.DepthFrameSource.FrameDescription;

            // get size of joint space
            _jointSpaceWidth = frameDescription.Width;
            _jointSpaceHeight = frameDescription.Height;

            _kinectSensor.IsAvailableChanged += Sensor_IsAvailableChanged;

            // open the sensor
            _kinectSensor.Open();

            bool kinectAvailable = _kinectSensor.IsAvailable;



            // open the reader for the body frames
            _bodyFrameReader = _kinectSensor.BodyFrameSource.OpenReader();

            _bodyFrameReader.FrameArrived += Reader_FrameArrived;
            
            // </Kinect>


        }

        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            _guiSubText.Text = _kinectSensor.IsAvailable ? "Kinect up and running"
                                                            : "Kinect not available";

            _subtextWidth = GUIText.GetTextWidth(_guiSubText.Text, _guiLatoBlack);
            _subtextHeight = GUIText.GetTextHeight(_guiSubText.Text, _guiLatoBlack);
        }

        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (_bodies == null)
                    {
                        _bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(_bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                foreach (Body body in _bodies)
                {
                    if (body.IsTracked)
                    {
                        _rightHandState = body.HandRightState;
                        IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                        CameraSpacePoint positionHand = joints[JointType.HandRight].Position;
                        if (positionHand.Z < 0)
                        {
                            positionHand.Z = InferredZPositionClamp;
                        }
                        float3 pHandRaw = new float3(positionHand.X, positionHand.Y, positionHand.Z);
                        _pHand = _pHand*_filterAlpha + (1.0f - _filterAlpha)*pHandRaw;


                        CameraSpacePoint positionTip = joints[JointType.HandTipRight].Position;
                        if (positionTip.Z < 0)
                        {
                            positionTip.Z = InferredZPositionClamp;
                        }
                        float3 pTipRaw = new float3(positionTip.X, positionTip.Y, positionTip.Z);
                        _pTip = _pTip * _filterAlpha + (1.0f - _filterAlpha) * pTipRaw;


                        float3 handDir = _pTip - _pHand;
                        handDir.Normalize();
                        _handYaw = (float)(Math.Atan2(handDir.z, handDir.x) + 0.5 * Math.PI);
                        float xzLen = (float)Math.Sqrt(handDir.x * handDir.x + handDir.z * handDir.z);
                        _handPitch = (float)Math.Atan2(xzLen, handDir.y);

                        DepthSpacePoint depthSpacePoint = _coordinateMapper.MapCameraPointToDepthSpace(positionHand);
                        _handPosLast = _handPos;
                        _handPos = new float3(depthSpacePoint.X * Width / _jointSpaceWidth, depthSpacePoint.Y * Height / _jointSpaceHeight, positionHand.Z);
                        _handVel = 0.0001f * (_handPos - _handPosLast);

                        //    HandTipRight,
                        //    ThumbRight,
                        break;
                    }
                }
            }
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            var lineWidthFactor = 1.0f;
            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                lineWidthFactor = 3.0f;
                _shaderEffect.SetEffectParam("uLineColor", new float4(1, 0.2f, 0.2f, 0.9f));
            }
            else
            {
                _shaderEffect.SetEffectParam("uLineColor", new float4(0, 0, 0, 1));
            }
            _shaderEffect.SetEffectParam("uLineWidth",
                new float2(lineWidthFactor*LineWidth/_normWidth, lineWidthFactor*LineWidth/_normHeight));

            // Handcontrol
            if (_rightHandState == HandState.Closed)
            {
                _angleVelHorz = RotationSpeed * _handVel.x; //(float)Time.Instance.DeltaTime *
                _angleVelVert = RotationSpeed * _handVel.y; //(float)Time.Instance.DeltaTime *
            }
            else
            {
                var curDamp = (float)Math.Exp(-Damping * Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz -= _angleVelHorz;
            _angleVert -= _angleVelVert;

            // Model
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 200, -500, 0, 0, 0, 0, 1, 0);

            RC.ModelView = mtxCam * mtxRot * _modelScaleOffset;
            _sr.Render(RC);

            // Hand
            mtxRot = float4x4.CreateRotationY(_handYaw) * float4x4.CreateRotationX(_handPitch);
            mtxCam = float4x4.LookAt(0, 0, -CamDist, 0, 0, 0, 0, 1, 0);

            float3 hndPoint = _handPos;
            var handWorldPos = new float3(hndPoint.x - 0.5f * Width, 0.5f * Height - hndPoint.y, 0);
            handWorldPos = 2 * CamDist / SquareScreenPxls * handWorldPos;
            handWorldPos.z = 600.0f * (0.8f - hndPoint.z);

            RC.ModelView = mtxCam * float4x4.CreateTranslation(handWorldPos) * mtxRot * new float4x4(HandScale, 0, 0, 0, 0, HandScale, 0, 0, 0, 0, HandScale, 0, 0, 0, 0, 1);

            switch (_rightHandState)
            {
                case HandState.Unknown:
                case HandState.NotTracked:
                case HandState.Open:
                    _shaderEffect.RenderMesh(_meshHandOpen);
                    break;
                case HandState.Closed:
                    _shaderEffect.RenderMesh(_meshHandClosed);
                    break;
                case HandState.Lasso:
                    _shaderEffect.RenderMesh(_meshHandIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _guiHandler.RenderGUI();

            // swap buffers
            Present();
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            // var aspectRatio = Width / (float)Height;
            _normWidth = Width/SquareScreenPxls;
            _normHeight = Height/SquareScreenPxls;

            RC.Projection = float4x4.CreatePerspectiveOffCenter(-_normWidth, _normWidth, -_normHeight, _normHeight, 1,
                10000);

            _shaderEffect.SetEffectParam("uLineWidth", new float2(LineWidth/_normWidth, LineWidth/_normHeight));

            _guiSubText.PosX = (int)((Width - _subtextWidth) / 2);
            _guiSubText.PosY = (int)(Height - _subtextHeight - 3);
            _guiHandler.Refresh();        

        }

        public static void Main()
        {
            var app = new HandOnCanvasKinect2();
            app.Run();
        }
    }
}