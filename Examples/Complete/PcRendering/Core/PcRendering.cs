using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.PcRendering.Core
{
    [FuseeApplication(Name = "FUSEE Point Cloud Viewer")]
    public class PcRendering<TPoint> : RenderCanvas, IPcRendering where TPoint : new()
    {
        public PcRendering(IPtOctantLoader oocLoader, IPtOctreeFileReader oocFileReader)
        {
            OocLoader = oocLoader;
            OocFileReader = oocFileReader;
        }

        public IPtOctantLoader OocLoader { get; }
        public IPtOctreeFileReader OocFileReader { get; }
        public bool UseWPF { get; set; }
        public bool DoShowOctants { get; set; }
        public bool IsSceneLoaded { get; private set; }
        public bool ReadyToLoadNewFile { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool IsAlive { get; private set; }

        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit;
        private static float2 _offset;
        private static float2 _offsetInit;

        private const float RotationSpeed = 7;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;

        private bool _twoTouchRepeated;
        private bool _keys;

        private const float ZNear = 1f;
        private const float ZFar = 1000;

        private readonly float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.Screen;

        private float _maxPinchSpeed;

        private float3 _initCamPos;
        public float3 InitCameraPos
        {
            get => _initCamPos;
            private set
            {
                _initCamPos = value;
                OocLoader.InitCamPos = new double3(_initCamPos.x, _initCamPos.y, _initCamPos.z);
            }
        }

        public bool ClosingRequested
        {
            get
            {
                return _closingRequested;
            }
            set
            {
                _closingRequested = value;
            }
        }

        private bool _closingRequested;

        private bool _isTexInitialized = false;

        private Texture _octreeTex;
        private double3 _octreeRootCenter;
        private double _octreeRootLength;

        private WritableTexture _depthTex;

        private Transform _camTransform;
        private Camera _cam;

        private SixDOFDevice _spaceMouse;

        public override void Init()
        {
            VSync = false;
            _spaceMouse = GetDevice<SixDOFDevice>();

            _depthTex = WritableTexture.CreateDepthTex(Width, Height, new ImagePixelFormat(ColorFormat.Depth24));

            OocLoader.Init(RC);

            IsAlive = true;

            ApplicationIsShuttingDown += (object sender, EventArgs e) =>
            {
                OocLoader.IsShuttingDown = true;
            };

            _scene = new SceneContainer
            {
                Children = new List<SceneNode>()
            };

            _camTransform = new Transform()
            {
                Name = "MainCamTransform",
                Scale = float3.One,
                Translation = InitCameraPos,
                Rotation = float3.Zero
            };

            _cam = new Camera(ProjectionMethod.Perspective, ZNear, ZFar, _fovy)
            {
                BackgroundColor = float4.One
            };

            var mainCam = new SceneNode()
            {
                Name = "MainCam",
                Components = new List<SceneComponent>()
                {
                    _camTransform,
                    _cam
                }
            };

            _scene.Children.Insert(0, mainCam);

            _angleRoll = 0;
            _angleRollInit = 0;
            _twoTouchRepeated = false;
            _offset = float2.Zero;
            _offsetInit = float2.Zero;

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).            

            if (!UseWPF)
                LoadPointCloudFromFile();

            _gui = CreateGui();
            //Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _guiRenderer = new SceneRendererForward(_gui);

            IsInitialized = true;
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            Diagnostics.Warn(FramesPerSecond);
            ReadyToLoadNewFile = false;

            if (_closingRequested)
            {
                ReadyToLoadNewFile = true;
                return;
            }

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            if (IsSceneLoaded)
            {
                var isSpaceMouseMoving = SpaceMouseMoving(out float3 velPos, out float3 velRot);

                // ------------ Enable to update the Scene only when the user isn't moving ------------------
                /*if (Keyboard.WSAxis != 0 || Keyboard.ADAxis != 0 || (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint) || isSpaceMouseMoving)
                    OocLoader.IsUserMoving = true;
                else
                    OocLoader.IsUserMoving = false;*/
                //--------------------------------------------------------------------------------------------

                // Mouse and keyboard movement
                if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
                    _keys = true;

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

                    _angleRoll = Touch.TwoPointAngle - _angleRollInit;
                    _offset = Touch.TwoPointMidPoint - _offsetInit;
                    float pinchSpeed = Touch.TwoPointDistanceVel;
                    if (pinchSpeed > _maxPinchSpeed) _maxPinchSpeed = pinchSpeed;
                }
                else
                {
                    _twoTouchRepeated = false;
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
                }

                if (isSpaceMouseMoving)
                {
                    _angleHorz -= velRot.y;
                    _angleVert -= velRot.x;

                    float speed = DeltaTime * 12;

                    _camTransform.FpsView(_angleHorz, _angleVert, velPos.z, velPos.x, speed);
                    _camTransform.Translation += new float3(0, velPos.y * speed, 0);
                }
                else
                {
                    _angleHorz += _angleVelHorz;
                    _angleVert += _angleVelVert;
                    _angleVelHorz = 0;
                    _angleVelVert = 0;

                    if (HasUserMoved() || _camTransform.Translation == InitCameraPos)
                    {
                        _camTransform.FpsView(_angleHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTime * 20);
                    }
                }

                //----------------------------  

                if (PtRenderingParams.Instance.CalcSSAO || PtRenderingParams.Instance.Lighting != Lighting.Unlit)
                {
                    //Render Depth-only pass
                    _scene.Children[1].RemoveComponent<ShaderEffect>();
                    _scene.Children[1].Components.Insert(1, PtRenderingParams.Instance.DepthPassEf);

                    _cam.RenderTexture = _depthTex;
                    _sceneRenderer.Render(RC);
                    _cam.RenderTexture = null;
                }

                //Render color pass
                //Change shader effect in complete scene
                _scene.Children[1].RemoveComponent<ShaderEffect>();
                _scene.Children[1].Components.Insert(1, PtRenderingParams.Instance.ColorPassEf);
                _sceneRenderer.Render(RC);

                //UpdateScene after Render / Traverse because there we calculate the view matrix (when using a camera) we need for the update.
                OocLoader.UpdateScene(PtRenderingParams.Instance.PtMode, PtRenderingParams.Instance.DepthPassEf, PtRenderingParams.Instance.ColorPassEf);

                if (UseWPF)
                {
                    if (!PtRenderingParams.Instance.ShaderParamsToUpdate.IsEmpty)
                    {
                        UpdateShaderParams();
                        PtRenderingParams.Instance.ShaderParamsToUpdate.Clear();
                    }
                }

                if (DoShowOctants)
                    OocLoader.ShowOctants(_scene);
            }

            //Render GUI
            RC.Projection = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);

            // Constantly check for interactive objects.
            if (Mouse != null) //Mouse is null when the pointer is outside the GameWindow?
            {
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);

                if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
                {
                    _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
                }
            }

            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();

            ReadyToLoadNewFile = true;
        }

        private bool SpaceMouseMoving(out float3 velPos, out float3 velRot)
        {
            if (_spaceMouse != null && _spaceMouse.IsConnected)
            {
                bool spaceMouseMovement = false;
                velPos = 0.001f * _spaceMouse.Translation;
                if (velPos.LengthSquared < 0.01f)
                    velPos = float3.Zero;
                else
                    spaceMouseMovement = true;
                velRot = 0.0001f * _spaceMouse.Rotation;
                velRot.z = 0;
                if (velRot.LengthSquared < 0.000005f)
                    velRot = float3.Zero;
                else
                    spaceMouseMovement = true;

                return spaceMouseMovement;
            }
            velPos = float3.Zero;
            velRot = float3.Zero;
            return false;
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
            if (!PtRenderingParams.Instance.CalcSSAO && PtRenderingParams.Instance.Lighting == Lighting.Unlit) return;

            //(re)create depth tex and fbo
            if (_isTexInitialized)
            {
                _depthTex = WritableTexture.CreateDepthTex(Width, Height, new ImagePixelFormat(ColorFormat.Depth24));

                PtRenderingParams.Instance.DepthPassEf.SetFxParam("ScreenParams", new float2(Width, Height));
                PtRenderingParams.Instance.ColorPassEf.SetFxParam("ScreenParams", new float2(Width, Height));
                PtRenderingParams.Instance.ColorPassEf.SetFxParam("DepthTex", _depthTex);
            }

            _isTexInitialized = true;
        }

        public override void DeInit()
        {
            base.DeInit();
            IsAlive = false;
        }

        private bool HasUserMoved()
        {
            return RC.View == float4x4.Identity
                || Mouse.LeftButton
                || Keyboard.WSAxis != 0 || Keyboard.ADAxis != 0
                || (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint);
        }

        public RenderContext GetRc()
        {
            return RC;
        }

        public SceneNode GetOocLoaderRootNode()
        {
            return OocLoader.RootNode;
        }

        public bool GetOocLoaderWasSceneUpdated()
        {
            return OocLoader.WasSceneUpdated;
        }

        public int GetOocLoaderPointThreshold()
        {
            return OocLoader.PointThreshold;
        }

        public void SetOocLoaderPointThreshold(int value)
        {
            OocLoader.PointThreshold = value;
        }

        public void SetOocLoaderMinProjSizeMod(float value)
        {
            OocLoader.MinProjSizeModifier = value;
        }

        public float GetOocLoaderMinProjSizeMod()
        {
            return OocLoader.MinProjSizeModifier;
        }

        public void LoadPointCloudFromFile()
        {
            //create Scene from octree structure
            var root = OocFileReader.GetScene();

            var ptOctantComp = root.GetComponent<OctantD>();
            InitCameraPos = _camTransform.Translation = new float3((float)ptOctantComp.Center.x, (float)ptOctantComp.Center.y, (float)(ptOctantComp.Center.z - (ptOctantComp.Size * 2f)));

            _scene.Children.Add(root);

            OocLoader.RootNode = root;
            OocLoader.FileFolderPath = PtRenderingParams.Instance.PathToOocFile;

            var octreeTexImgData = new ImageData(ColorFormat.uiRgb8, OocFileReader.NumberOfOctants, 1);
            _octreeTex = new Texture(octreeTexImgData);
            OocLoader.VisibleOctreeHierarchyTex = _octreeTex;

            var byteSize = OocFileReader.NumberOfOctants * octreeTexImgData.PixelFormat.BytesPerPixel;
            octreeTexImgData.PixelData = new byte[byteSize];

            var ptRootComponent = root.GetComponent<OctantD>();
            _octreeRootCenter = ptRootComponent.Center;
            _octreeRootLength = ptRootComponent.Size;

            PtRenderingParams.Instance.DepthPassEf = PtRenderingParams.Instance.CreateDepthPassEffect(new float2(Width, Height), InitCameraPos.z, _octreeTex, _octreeRootCenter, _octreeRootLength);
            PtRenderingParams.Instance.ColorPassEf = PtRenderingParams.Instance.CreateColorPassEffect(new float2(Width, Height), InitCameraPos.z, new float2(ZNear, ZFar), _depthTex, _octreeTex, _octreeRootCenter, _octreeRootLength);

            _scene.Children[1].RemoveComponent<ShaderEffect>();
            if (PtRenderingParams.Instance.CalcSSAO || PtRenderingParams.Instance.Lighting != Lighting.Unlit)
                _scene.Children[1].AddComponent(PtRenderingParams.Instance.DepthPassEf);
            else
                _scene.Children[1].AddComponent(PtRenderingParams.Instance.ColorPassEf);

            IsSceneLoaded = true;
        }

        public void DeletePointCloud()
        {
            IsSceneLoaded = false;

            while (!OocLoader.WasSceneUpdated || !ReadyToLoadNewFile)
            {
                continue;
            }

            if (OocLoader.RootNode != null)
                _scene.Children.Remove(OocLoader.RootNode);
        }

        public void ResetCamera()
        {
            _camTransform.Translation = InitCameraPos;
            _angleHorz = _angleVert = 0;
        }

        public void DeleteOctants()
        {
            IsSceneLoaded = false;

            while (!OocLoader.WasSceneUpdated || !ReadyToLoadNewFile)
            {
                continue;
            }

            DoShowOctants = false;
            OocLoader.DeleteOctants(_scene);
            IsSceneLoaded = true;
        }

        private static void UpdateShaderParams()
        {
            foreach (var param in PtRenderingParams.Instance.ShaderParamsToUpdate)
            {
                if (PtRenderingParams.Instance.DepthPassEf.ParamDecl.ContainsKey(param.Key))
                    PtRenderingParams.Instance.DepthPassEf.SetFxParam(param.Key, param.Value);
                if (PtRenderingParams.Instance.ColorPassEf.ParamDecl.ContainsKey(param.Key))
                    PtRenderingParams.Instance.ColorPassEf.SetFxParam(param.Key, param.Value);
            }

            PtRenderingParams.Instance.ShaderParamsToUpdate.Clear();
        }

        #region UI

        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");
            var psText = AssetStorage.Get<string>("text.frag");

            var canvasWidth = Width / 100f;
            var canvasHeight = Height / 100f;

            var btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            var fuseeLogo = new TextureNode(
                "fuseeLogo",
                vsTex,
                psTex,
                //Set the albedo texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                //Define Offset and therefor the size of the element.
                UIElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f)),
                float2.One
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 24);

            var text = new TextNode(
                "FUSEE Simple Example",
                "ButtonText",
                vsTex,
                psText,
                UIElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                UIElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery),
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            var canvas = new CanvasNode(
                "Canvas",
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-canvasWidth / 2, -canvasHeight / 2f),
                    Max = new float2(canvasWidth / 2, canvasHeight / 2f)
                })
            {
                Children = new ChildList()
                {
                    //Simple Texture Node, contains the fusee logo.
                    fuseeLogo,
                    text
                }
            };

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    //Add canvas.
                    canvas
                }
            };
        }

        public void BtnLogoEnter(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Instance.Albedo, new float4(0.0f, 0.0f, 0.0f, 1f));
            effect.SetFxParam(UniformNameDeclarations.Instance.AlbedoMix, 0.8f);
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Instance.Albedo, float4.One);
            effect.SetFxParam(UniformNameDeclarations.Instance.AlbedoMix, 1f);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }

        #endregion
    }
}