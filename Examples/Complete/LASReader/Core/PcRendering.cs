using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Pointcloud.OoCFileGen;
using Fusee.Serialization;
using Fusee.Xene;
using System.Collections.Generic;
using System.Linq;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.PcRendering.Core
{
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class PcRendering : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = 0, _angleVert = 0, _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit, _zoomVel, _zoom;
        private static float2 _offset;
        private static float2 _offsetInit;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private ScenePicker _scenePicker;
        private bool _twoTouchRepeated;

        private bool _keys;

        private const float ZNear = 1f;
        private const float ZFar = 1000;

        private float _fovy = M.PiOver4;

        private SceneRenderer _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;
        private float _initCanvasWidth;
        private float _initCanvasHeight;
        private float _canvasWidth = 16;
        private float _canvasHeight = 9;

        private float _maxPinchSpeed;
        private SixDOFDevice _spaceMouse;
        private GamePadDevice _gamePad;

        private float3 _cameraPos;
        private ITextureHandle _texHandle;
        private SceneNodeContainer _pc;

        private ShaderEffect _depthPassEf;
        private ShaderEffect _colorPassEf;

        private ShaderEffect _wireFrameMat;

        private bool _isTexInitialized = false;

        private readonly WireframeCube wfc = new WireframeCube();
        private readonly ShaderEffect wfcEffect = ShaderCodeBuilder.MakeShaderEffect(new float4(1, 1, 0, 1), new float4(1, 1, 1, 1), 10);

        private void GetPointsFromOctreeLevel(int lvl, PtOctant<LAZPointType> octant)
        {   
            for (int i = 0; i < octant.Children.Length; i++)
            {
                var child = (PtOctant<LAZPointType>)octant.Children[i];

                if (child == null) continue;

                var wcSn = new SceneNodeContainer()
                {
                    Name = "WireframeCube",
                    Components = new List<SceneComponentContainer>()
                    {
                        new TransformComponent()
                        {
                            Scale = float3.One * (float)child.Size,
                            Translation = (float3) child.Center
                        },
                        new ShaderEffectComponent()
                        {
                            Effect = wfcEffect
                        },
                        wfc
                    }
                };

                _scene.Children.Add(wcSn);

                if (child.Level < lvl)
                {
                    
                    if (child.IsLeaf)
                    {                        
                        var childPoints = PtOctree<LAZPointType>.GetPointsFromGrid(child).ToList();
                        //childPoints.AddRange(child.Payload);
                        var sn = LAZtoSceneNode.FromPointList(childPoints, _depthPassEf);
                        _scene.Children.Add(sn);
                    }

                    GetPointsFromOctreeLevel(lvl, child);                    
                }
                else
                {
                    var childPoints = PtOctree<LAZPointType>.GetPointsFromGrid(child).ToList();
                    var sn = LAZtoSceneNode.FromPointList(childPoints, _depthPassEf);
                    _scene.Children.Add(sn);
                }
            }
        }        

        // Init is called on startup. 
        public override void Init()
        {
            _cameraPos = new float3(0.0f, 2, -10);

            _initCanvasWidth = Width / 100f;
            _initCanvasHeight = Height / 100f;

            _canvasHeight = _initCanvasHeight;
            _canvasWidth = _initCanvasWidth;

            // Initial "Zoom" value (it's rather the distance in view direction, not the camera's focal distance/opening angle)
            _zoom = 1;       

            _angleRoll = 0;
            _angleRollInit = 0;
            _twoTouchRepeated = false;
            _offset = float2.Zero;
            _offsetInit = float2.Zero;

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Load the rocket model
            _scene = new SceneContainer
            {
                Children = new List<SceneNodeContainer>()
            };           

            var points = LAZtoSceneNode.ListFromLAZ("E:/HolbeinPferd.las");

            var aabb = new AABBd(points[0].Position, points[0].Position);
            foreach (var pt in points)
            {
                aabb |= pt.Position;
            }
            var gridPtAccessor = new MyPointAcessor();
            var octree = new PtOctree<LAZPointType>(aabb, gridPtAccessor, points, 5000);

            GetPointsFromOctreeLevel(3, octree.Root); 

            var pc = new ProjectionComponent(ProjectionMethod.PERSPECTIVE, ZNear, ZFar, _fovy);
            _scene.Children[0].Components.Insert(0, pc);

            _gui = CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            //Add resize delegate
            var projComp = _scene.Children[0].GetComponent<ProjectionComponent>();
            AddResizeDelegate(delegate { projComp.Resize(Width, Height); });

            //create depth tex and fbo
            _texHandle = RC.CreateWritableTexture(Width, Height, WritableTextureFormat.Depth);

            _wireFrameMat = ShaderCodeBuilder.MakeShaderEffect(new float4(1, 1, 0, 1), new float4(1, 1, 1, 1), 10);
            _depthPassEf = LAZtoSceneNode.DepthPassEffect(new float2(Width, Height));
            _colorPassEf = LAZtoSceneNode.StandardEffect(new float2(Width, Height), new float2(ZNear, ZFar), _texHandle);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_scene);
            _scenePicker = new ScenePicker(_scene);
            _guiRenderer = new SceneRenderer(_gui);           
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
                float pinchSpeed = Touch.TwoPointDistanceVel;
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
                _angleVelHorz = RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angleVelVert = RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _keys = false;
                float2 touchVel;
                touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                _angleVelVert = RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;
                    _angleVelVert = RotationSpeed * Keyboard.UpDownAxis * DeltaTime;
                }
                else
                {
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _zoom += _zoomVel;
            // Limit zoom
            if (_zoom < 1)
                _zoom = 1;
            if (_zoom > 1000)
                _zoom = 1000;

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            var twoPi = M.Pi * 2;

            if ((_angleHorz > twoPi && _angleHorz > 0) || _angleHorz < -twoPi)
                _angleHorz = _angleHorz % twoPi;
            if ((_angleVert > twoPi && _angleVert > 0) || _angleVert < -twoPi)
                _angleVert = _angleVert % twoPi;

            _cameraPos += RC.View.Row2.xyz * Keyboard.WSAxis * Time.DeltaTime * 3;
            _cameraPos += RC.View.Row0.xyz * Keyboard.ADAxis * Time.DeltaTime * 3;

            RC.View = FPSView(_cameraPos, _angleVert, _angleHorz);

            _scenePicker.View = RC.View;

            // Constantly check for interactive objects.
            _sih.CheckForInteractiveObjects(Input.Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            //Render Depth-only pass
            foreach (var sn in _scene.Children)
            {
                sn.GetComponent<ShaderEffectComponent>().Effect = _depthPassEf;
            }            
            _sceneRenderer.Render(RC, _texHandle);

            //Render color pass
            foreach (var sn in _scene.Children)
            {           
                if(sn.Name.Contains("Cube"))
                    sn.GetComponent<ShaderEffectComponent>().Effect = _wireFrameMat;
                else if(sn.Name.Contains("PointCloud"))
                    sn.GetComponent<ShaderEffectComponent>().Effect = _colorPassEf;
            }

            _scene.Children[0].GetComponent<ShaderEffectComponent>().Effect = ShaderCodeBuilder.MakeShaderEffect(new float4(1, 1, 0, 1), new float4(1, 1, 1, 1), 10);

            //_pc.GetComponent<ShaderEffectComponent>().Effect = _colorPassEf;            
            _sceneRenderer.Render(RC);

            //Render GUI
            _sih.View = RC.View;
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
            //(re)create depth tex and fbo
            if (_isTexInitialized)
            {
                RC.RemoveTextureHandle(_texHandle);
                _texHandle = RC.CreateWritableTexture(Width, Height, WritableTextureFormat.Depth);

                _depthPassEf = LAZtoSceneNode.DepthPassEffect(new float2(Width, Height));
                _colorPassEf = LAZtoSceneNode.StandardEffect(new float2(Width, Height), new float2(ZNear, ZFar), _texHandle);
            }          

            _isTexInitialized = true;

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

        private float4x4 FPSView(float3 eye, float pitch, float yaw)
        {
            // I assume the values are already converted to radians.
            float cosPitch = M.Cos(pitch);
            float sinPitch = M.Sin(pitch);
            float cosYaw = M.Cos(yaw);
            float sinYaw = M.Sin(yaw);

            float3 xaxis = float3.Normalize(new float3(cosYaw, 0, -sinYaw));
            float3 yaxis = float3.Normalize(new float3(sinYaw * sinPitch, cosPitch, cosYaw * sinPitch));
            float3 zaxis = float3.Normalize(new float3(sinYaw * cosPitch, -sinPitch, cosPitch * cosYaw));

            // Create a 4x4 view matrix from the right, up, forward and eye position vectors
            float4x4 viewMatrix = new float4x4(
                new float4(xaxis.x, yaxis.x, zaxis.x, 0),
                new float4(xaxis.y, yaxis.y, zaxis.y, 0),
                new float4(xaxis.z, yaxis.z, zaxis.z, 0),
                new float4(-float3.Dot(xaxis, eye), -float3.Dot(yaxis, eye), -float3.Dot(zaxis, eye), 1)
            );

            viewMatrix = float4x4.Transpose(viewMatrix);

            return viewMatrix;

        }

    }
}