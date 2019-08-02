using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.GUI;
using Fusee.LASReader;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Serialization;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Engine.Player.Core
{
    internal struct LAZPointType
    {
        public double3 Position;
        public ushort Intensity;
    }

    internal class MyPointAcessor : PointAccessor<LAZPointType>
    {
        public override bool HasPositionFloat3_64 => true;
        public override bool HasIntensityUInt_16 => true;

        public override void SetPositionFloat3_64(ref LAZPointType point, double3 val)
        {
            point.Position = val;
        }

        public override ref double3 GetPositionFloat3_64(ref LAZPointType point)
        {
            return ref point.Position;
        }

        public override ref ushort GetIntensityUInt_16(ref LAZPointType point)
        {
            return ref point.Intensity;
        }

        public override void SetIntensityUInt_16(ref LAZPointType point, ushort val)
        {
            point.Intensity = val;
        }
    }

    internal class LAZPointcloud : IPointcloud<LAZPointType>
    {
        public IMeta MetaInfo { get; private set; }

        private readonly LAZPointType[] _points;

        public Span<LAZPointType> Points => new Span<LAZPointType>(_points);

        public PointAccessor<LAZPointType> Pa { get; private set; }

        public LAZPointcloud(int pntCnt)
        {
            var myPointAccessor = new MyPointAcessor();
            Pa = myPointAccessor;

            _points = new LAZPointType[pntCnt];
        }
    }
    public static class ListExtension
    {
        public static IEnumerable<List<T>> Split<T>(this List<T> locations, int nSize = 30)
        {
            for (var i = 0; i < locations.Count; i += nSize)
                yield return locations.GetRange(i, M.Min(nSize, locations.Count - i));
        }
    }

    [FuseeApplication(Name = "Pointcloud Viewer", Description = "Watch a LAZ pointcloud.")]
    public class Player : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit, _zoomVel, _zoom;
        private static float2 _offset;
        private static float2 _offsetInit;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private float4x4 _sceneCenter;
        private float4x4 _sceneScale;
        private bool _twoTouchRepeated;

        private bool _keys;

        private const float ZNear = 1f;
        private const float ZFar = 3000;
        private float _aspectRatio;
        private readonly float _fovy = M.PiOver4;

        private SceneRenderer _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;
        private float _initWindowWidth;
        private float _initWindowHeight;
        private float _initCanvasWidth;
        private float _initCanvasHeight;
        private float _canvasWidth = 16;
        private float _canvasHeight = 9;

        private float _maxPinchSpeed;
        private SixDOFDevice _spaceMouse;
        private GamePadDevice _gamePad;


        // Init is called on startup. 
        public override void Init()
        {
            var sw = new Stopwatch();
            sw.Start();

            var reader = new LASPointReader("E:\\\\LOAD_ME.laz");
            var pointCnt = (MetaInfo)reader.MetaInfo;
            var myPC = new LAZPointcloud((int)pointCnt.PointCnt);
            for (var i = 0; i < myPC.Points.Length; i++)
                if (!reader.ReadNextPoint(ref myPC.Points[i], myPC.Pa)) break;

            sw.Stop();
            Diagnostics.Log($"Read {pointCnt.PointCnt} points in {sw.ElapsedMilliseconds} ms.");

            var allPointsSplitted = myPC.Points.ToArray().ToList().Split(ushort.MaxValue - 1);
            var allMeshes = new List<Mesh>();

            foreach (var points in allPointsSplitted)
            {
                allMeshes.Add(new Mesh
                {
                    Vertices = points.Select(pt => new float3(new float3((float)pt.Position.x, (float)pt.Position.z, (float)pt.Position.y))).ToArray(),
                    Triangles = Enumerable.Range(0, points.Count).Select(tri => (ushort)tri).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT
                });
            }

            _scene = new SceneContainer
            {
                Header = new SceneHeader
                {
                    CreatedBy = "Moritz",
                    CreationDate = DateTime.Now.ToString(),
                    Generator = "LASPointReader",
                    Version = 1
                },
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = "Pointcloud",
                        Components = new List<SceneComponentContainer>
                        {
                            new ProjectionComponent(ProjectionMethod.PERSPECTIVE, 0.1f, 1000, M.PiOver4),
                            new TransformComponent
                            {
                                Rotation = float3.Zero,
                                Translation = -(float3)myPC.Points[0].Position,
                                Scale = float3.One
                            }                            
                        }
                    }
                }
            };

            _scene.Children[0].Components.AddRange(allMeshes);

            _initWindowWidth = Width;
            _initWindowHeight = Height;

            _initCanvasWidth = Width / 100f;
            _initCanvasHeight = Height / 100f;

            _canvasHeight = _initCanvasHeight;
            _canvasWidth = _initCanvasWidth;

            _aspectRatio = Width / (float)Height;

            // Initial "Zoom" value (it's rather the distance in view direction, not the camera's focal distance/opening angle)
            _zoom = 400;

            _angleRoll = 0;
            _angleRollInit = 0;
            _twoTouchRepeated = false;
            _offset = float2.Zero;
            _offsetInit = float2.Zero;

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            _gui = CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);


            // Register the input devices that are not already given.

            _spaceMouse = GetDevice<SixDOFDevice>();
            _gamePad = GetDevice<GamePadDevice>();



            var aabbc = new AABBCalculator(_scene);
            var bbox = aabbc.GetBox();
            if (bbox != null)
            {
                // If the model origin is more than one third away from its bounding box, 
                // recenter it to the bounding box. Do this check individually per dimension.
                // This way, small deviations will keep the model's original center, while big deviations 
                // will make the model rotate around its geometric center.
                var bbCenter = bbox.Value.Center;
                var bbSize = bbox.Value.Size;
                var center = float3.Zero;
                if (System.Math.Abs(bbCenter.x) > bbSize.x * 0.3)
                    center.x = bbCenter.x;
                if (System.Math.Abs(bbCenter.y) > bbSize.y * 0.3)
                    center.y = bbCenter.y;
                if (System.Math.Abs(bbCenter.z) > bbSize.z * 0.3)
                    center.z = bbCenter.z;
                _sceneCenter = float4x4.CreateTranslation(-center);

                // Adjust the model size
                var maxScale = System.Math.Max(bbSize.x, System.Math.Max(bbSize.y, bbSize.z));
                if (maxScale != 0)
                    _sceneScale = float4x4.CreateScale(200.0f / maxScale);
                else
                    _sceneScale = float4x4.Identity;
            }

            //Add resize delegate
            var projComp = _scene.Children[0].GetComponent<ProjectionComponent>();
            AddResizeDelegate(delegate { projComp.Resize(Width, Height); });

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_scene);
            _guiRenderer = new SceneRenderer(_gui);

        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            //if (_gamePad != null)
            //    Diagnostics.Log(_gamePad.LSX);

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
            // Zoom & Roll
            if (Touch.TwoPoint)
            {
                if (!_twoTouchRepeated)
                {
                    _twoTouchRepeated = true;
                    _angleRollInit = Touch.TwoPointAngle - _angleRoll;
                    _offsetInit = Touch.TwoPointMidPoint - _offset;
                    _maxPinchSpeed = 0;
                }
                _zoomVel = Touch.TwoPointDistanceVel * -0.01f;
                _angleRoll = Touch.TwoPointAngle - _angleRollInit;
                _offset = Touch.TwoPointMidPoint - _offsetInit;
                var pinchSpeed = Touch.TwoPointDistanceVel;
                if (pinchSpeed > _maxPinchSpeed) _maxPinchSpeed = pinchSpeed; // _maxPinchSpeed is used for debugging only.
            }
            else
            {
                _twoTouchRepeated = false;
                _zoomVel = Mouse.WheelVel * -0.5f;
                _angleRoll *= curDamp * 0.8f;
                _offset *= curDamp * 0.8f;
            }


            // UpDown / LeftRight rotation
            if (Mouse.LeftButton)
            {

                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }
            
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _keys = false;
                float2 touchVel;
                touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
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
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _zoom += _zoomVel;
            // Limit zoom
            if (_zoom < 80)
                _zoom = 80;
            if (_zoom > 2000)
                _zoom = 2000;

            _angleHorz += _angleVelHorz;
            // Wrap-around to keep _angleHorz between -PI and + PI
            _angleHorz = M.MinAngle(_angleHorz);

            _angleVert += _angleVelVert;
            // Limit pitch to the range between [-PI/2, + PI/2]
            _angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

            // Wrap-around to keep _angleRoll between -PI and + PI
            _angleRoll = M.MinAngle(_angleRoll);

            // Create the camera matrix and set it as the current View transformation
            var mtxRot = /*float4x4.CreateRotationZ(_angleRoll) **/ float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 20, -_zoom, 0, 0, 0, 0, 1, 0);
            RC.View = mtxCam * mtxRot * _sceneScale * _sceneCenter;
            var mtxOffset = float4x4.CreateTranslation(2f * _offset.x / Width, -2f * _offset.y / Height, 0);
            RC.Projection *= mtxOffset;

            // Constantly check for interactive objects.
            _sih.CheckForInteractiveObjects(Input.Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }
            // Tick any animations and Render the scene loaded in Init()
            _sceneRenderer.Animate();
            _sceneRenderer.Render(RC);

            _sih.View = RC.View;

            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private InputDevice Creator(IInputDeviceImp device)
        {
            throw new NotImplementedException();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {

        }

        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");

            var btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            var fuseeLogo = new TextureNodeContainer(
                "fuseeLogo",
                vsTex,
                psTex,
                //Set the diffuse texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_LEFT),
                //Define Offset and therefor the size of the element.                
                UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(0, _initCanvasHeight - 0.5f), _initCanvasHeight, _initCanvasWidth, new float2(1.75f, 0.5f))
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            // Initialize the information text line.
            var textToDisplay = "FUSEE 3D Scene";
            if (_scene.Header.CreatedBy != null || _scene.Header.CreationDate != null)
            {
                textToDisplay += " created";
                if (_scene.Header.CreatedBy != null)
                    textToDisplay += " by " + _scene.Header.CreatedBy;

                if (_scene.Header.CreationDate != null)
                    textToDisplay += " on " + _scene.Header.CreationDate;
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
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-_canvasWidth / 2, -_canvasHeight / 2f),
                    Max = new float2(_canvasWidth / 2, _canvasHeight / 2f)
                });
            canvas.Children.Add(fuseeLogo);
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

        public void BtnLogoEnter(CodeComponent sender)
        {
            _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffectComponent>().Effect.SetEffectParam("DiffuseColor", new float4(0.8f, 0.8f, 0.8f, 1f));
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffectComponent>().Effect.SetEffectParam("DiffuseColor", float4.One);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }
    }
}