using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Pointcloud.OoCFileReaderWriter;
using Fusee.Pointcloud.PointAccessorCollections;
using Fusee.Serialization;
using Fusee.Xene;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.PcRendering.Core
{
    [FuseeApplication(Name = "FUSEE Point Cloud Viewer")]
    public class PcRendering<TPoint> : RenderCanvas, IPcRendering where TPoint : new()
    {
        public AppSetupHelper.AppSetupDelegate AppSetup;

        public PtOctantLoader<TPoint> OocLoader { get; set; }

        public PtOctreeFileReader<TPoint> OocFileReader { get; set; }

        public bool UseWPF { get; set; }
        public bool DoShowOctants { get; set; }
        public bool IsSceneLoaded { get; private set; }
        public bool ReadyToLoadNewFile { get; private set; }
        public bool IsInitialized { get; private set; } = false;
        public bool IsAlive { get; private set; }

        // angle variables
        private static float _angleHorz = 0, _angleVert = 0, _angleVelHorz, _angleVelVert, _angleRoll, _angleRollInit;
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
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;

        private float _maxPinchSpeed;

        private float3 _initCamPos;
        public float3 InitCameraPos { get { return _initCamPos; } private set { _initCamPos = value; OocLoader.InitCamPos = _initCamPos; } }

        internal static ShaderEffect _depthPassEf;
        internal static ShaderEffect _colorPassEf;

        private bool _isTexInitialized = false;

        private Texture _octreeTex;
        private double3 _octreeRootCenter;
        private double _octreeRootLength;

        private WritableTexture _depthTex;

        private TransformComponent _camTransform;
        private CameraComponent _cam;

        // Init is called on startup. 
        public override async Task<bool> Init()
        {
            _depthTex = new WritableTexture(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), Width, Height, false);

            IsAlive = true;

            AppSetup();

            _scene = new SceneContainer
            {
                Children = new List<SceneNodeContainer>()
            };

            _camTransform = new TransformComponent()
            {
                Name = "MainCamTransform",
                Scale = float3.One,
                Translation = InitCameraPos,
                Rotation = float3.Zero
            };

            _cam = new CameraComponent(ProjectionMethod.PERSPECTIVE, ZNear, ZFar, _fovy)
            {
                BackgroundColor = float4.One
            };

            var mainCam = new SceneNodeContainer()
            {
                Name = "MainCam",
                Components = new List<SceneComponentContainer>()
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
            {
                LoadPointCloudFromFile();
            }                        

            _gui = CreateGui();
            //Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene); 
            _guiRenderer = new SceneRendererForward(_gui);

            IsInitialized = true;

            return true;
        }       

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            ReadyToLoadNewFile = false;

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            if (IsSceneLoaded)
            {
                if (Keyboard.WSAxis != 0 || Keyboard.ADAxis != 0 || (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint))
                    OocLoader.IsUserMoving = true;
                else
                    OocLoader.IsUserMoving = false;

                // Mouse and keyboard movement
                if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
                {
                    _keys = true;
                }

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
                    if (pinchSpeed > _maxPinchSpeed) _maxPinchSpeed = pinchSpeed; // _maxPinchSpeed is used for debugging only.
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

                _angleHorz += _angleVelHorz;
                _angleVert += _angleVelVert;
                _angleVelHorz = 0;
                _angleVelVert = 0;

                if (HasUserMoved() || _camTransform.Translation == InitCameraPos)
                {
                    _camTransform.FpsView(_angleHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTime * 20);                    
                }
                
                //----------------------------  

                if (PtRenderingParams.CalcSSAO || PtRenderingParams.Lighting != Lighting.UNLIT)
                {
                    //Render Depth-only pass
                    _scene.Children[1].GetComponent<ShaderEffectComponent>().Effect = _depthPassEf;
                    _cam.RenderTexture = _depthTex;
                    _sceneRenderer.Render(RC);
                    _cam.RenderTexture = null;
                }

                //Render color pass
                //Change shader effect in complete scene
                _scene.Children[1].GetComponent<ShaderEffectComponent>().Effect = _colorPassEf;               

                _sceneRenderer.Render(RC);

                //UpdateScene after Render / Traverse because there we calculate the view matrix (when using a camera) we need for the update.
                OocLoader.RC = RC;
                OocLoader.UpdateScene(PtRenderingParams.PtMode, _depthPassEf, _colorPassEf);

                if (UseWPF)
                {
                    if (PtRenderingParams.ShaderParamsToUpdate.Count != 0)
                        UpdateShaderParams();
                }

                if (DoShowOctants)
                    OocLoader.ShowOctants(_scene);
            }

            //Render GUI
            RC.Projection = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);

            // Constantly check for interactive objects.
            _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();

            ReadyToLoadNewFile = true;

            Diagnostics.Debug(FramePerSecond.ToString());
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
            if (!PtRenderingParams.CalcSSAO && PtRenderingParams.Lighting == Lighting.UNLIT) return;

            //(re)create depth tex and fbo
            if (_isTexInitialized)
            {
                _depthTex = new WritableTexture(RenderTargetTextureTypes.G_DEPTH, new ImagePixelFormat(ColorFormat.Depth), Width, Height, false);

                _depthPassEf = PtRenderingParams.DepthPassEffect(new float2(Width, Height), InitCameraPos.z, _octreeTex, _octreeRootCenter, _octreeRootLength);
                _colorPassEf = PtRenderingParams.ColorPassEffect(new float2(Width, Height), InitCameraPos.z, new float2(ZNear, ZFar), _depthTex, _octreeTex, _octreeRootCenter, _octreeRootLength);
            }

            _isTexInitialized = true;
        }

        public override void DeInit()
        {
            IsAlive = false;
            base.DeInit();

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

        public SceneNodeContainer GetOocLoaderRootNode()
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
            var root = OocFileReader.GetScene(_depthPassEf);

            var ptOctantComp = root.GetComponent<PtOctantComponent>();
            InitCameraPos = _camTransform.Translation = new float3((float)ptOctantComp.Center.x, (float)ptOctantComp.Center.y, (float)(ptOctantComp.Center.z - (ptOctantComp.Size * 2f)));

            _scene.Children.Add(root);

            OocLoader.RootNode = root;
            OocLoader.FileFolderPath = PtRenderingParams.PathToOocFile;

            var octreeTexImgData = new ImageData(ColorFormat.uiRgb8, OocFileReader.NumberOfOctants, 1);
            _octreeTex = new Texture(octreeTexImgData);
            OocLoader.VisibleOctreeHierarchyTex = _octreeTex;

            var byteSize = OocFileReader.NumberOfOctants * octreeTexImgData.PixelFormat.BytesPerPixel;
            octreeTexImgData.PixelData = new byte[byteSize];

            var ptRootComponent = root.GetComponent<PtOctantComponent>();
            _octreeRootCenter = ptRootComponent.Center;
            _octreeRootLength = ptRootComponent.Size;

            _depthPassEf = PtRenderingParams.DepthPassEffect(new float2(Width, Height), InitCameraPos.z, _octreeTex, _octreeRootCenter, _octreeRootLength);
            _colorPassEf = PtRenderingParams.ColorPassEffect(new float2(Width, Height), InitCameraPos.z, new float2(ZNear, ZFar), _depthTex, _octreeTex, _octreeRootCenter, _octreeRootLength);

            if (PtRenderingParams.CalcSSAO || PtRenderingParams.Lighting != Lighting.UNLIT)
                _scene.Children[1].GetComponent<ShaderEffectComponent>().Effect = _depthPassEf;
            else
                _scene.Children[1].GetComponent<ShaderEffectComponent>().Effect = _colorPassEf;

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
            _camTransform .Translation = InitCameraPos;
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

        private void UpdateShaderParams()
        {
            foreach (var param in PtRenderingParams.ShaderParamsToUpdate)
            {
                RC.SetFXParam(param.Key, param.Value);
            }
            PtRenderingParams.ShaderParamsToUpdate.Clear();
        }        

        #region UI

        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");

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
                UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f))
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 18);

            var text = new TextNodeContainer(
                "FUSEE Simple Example",
                "ButtonText",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_HORIZONTAL),
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_HORIZONTAL, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery), 250f);

            var canvas = new CanvasNodeContainer(
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

        #endregion       
    }
}