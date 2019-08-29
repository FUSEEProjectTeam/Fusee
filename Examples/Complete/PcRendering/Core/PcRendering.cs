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
        private SceneRenderer _sceneRenderer;
        private ScenePicker _scenePicker;

        private bool _twoTouchRepeated;
        private bool _keys;

        private const float ZNear = 1f;
        private const float ZFar = 1000;

        private readonly float _fovy = M.PiOver4;

        private SceneRenderer _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;
        private float _initCanvasWidth;
        private float _initCanvasHeight;
        private float _canvasWidth = 16;
        private float _canvasHeight = 9;

        private float _maxPinchSpeed;

        private float3 _cameraPos;

        private float3 _initCamPos;
        public float3 InitCameraPos { get { return _initCamPos; } private set { _initCamPos = value; OocLoader.InitCamPos = _initCamPos; } } /*= new float3(10, 0, -30);*/

        private ITextureHandle _texHandle;
        
        internal static ShaderEffect _depthPassEf;
        internal static ShaderEffect _colorPassEf;       

        private bool _isTexInitialized = false;        
        
        private ProjectionComponent projectionComponent;

        private Texture _octreeTex;
        private double3 _octreeRootCenter;
        private double _octreeRootLength;

        private const float twoPi = M.Pi * 2f;

        private RenderTarget _renderTarget = new RenderTarget();
         
        // Init is called on startup. 
        public override void Init()
        {

            _renderTarget.CreateDepthTex(Width, Height);

            IsAlive = true;
                                    
            AppSetup();

            _scene = new SceneContainer
            {
                Children = new List<SceneNodeContainer>()
            };
            
            if (projectionComponent != null)
                RemoveResizeDelegate(delegate { projectionComponent.Resize(Width, Height); });

            projectionComponent = new ProjectionComponent(ProjectionMethod.PERSPECTIVE, ZNear, ZFar, _fovy);

            _scene.Children.Insert(0, new SceneNodeContainer() { Name = "ProjNode", Components = new List<SceneComponentContainer>() { projectionComponent } });
            _scene.Children[0].Components[0] = projectionComponent;

            AddResizeDelegate(delegate
            {
                projectionComponent.Resize(Width, Height);
                RC.Viewport(0, 0, Width, Height);
            });

            //create depth tex and fbo
            //_texHandle = RC.CreateWritableTexture(Width, Height, WritableTextureFormat.Depth);

            _initCanvasWidth = Width / 100f;
            _initCanvasHeight = Height / 100f;

            _canvasHeight = _initCanvasHeight;
            _canvasWidth = _initCanvasWidth;

            _angleRoll = 0;
            _angleRollInit = 0;
            _twoTouchRepeated = false;
            _offset = float2.Zero;
            _offsetInit = float2.Zero;

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            if (!UseWPF)
            {                
                LoadPointCloudFromFile();
            }
            

            _gui = CreateGui();
            //Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);           

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_scene);
            _scenePicker = new ScenePicker(_scene);
            _guiRenderer = new SceneRenderer(_gui);

            IsInitialized = true;
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

                if (HasUserMoved() || _cameraPos == InitCameraPos) //User has moved, RC.View must be recalculated.
                {
                    CalcAndSetViewMat();
                }               

                // Constantly check for interactive objects.
                _sih.CheckForInteractiveObjects(Input.Mouse.Position, Width, Height);

                if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
                {
                    _sih.CheckForInteractiveObjects(Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
                }
                //----------------------------                

                if (PtRenderingParams.CalcSSAO || PtRenderingParams.Lighting != Lighting.UNLIT)
                {
                    //Render Depth-only pass
                    _scene.Children[1].GetComponent<ShaderEffectComponent>().Effect = _depthPassEf;
                    _sceneRenderer.Render(RC, _renderTarget);
                    //_sceneRenderer.Render(RC, _texHandle);
                }

                //Render color pass
                //Change shader effect in complete scene
                _scene.Children[1].GetComponent<ShaderEffectComponent>().Effect = _colorPassEf;

                OocLoader.RC = RC;
                OocLoader.UpdateScene(PtRenderingParams.PtMode, _depthPassEf, _colorPassEf);

                if (UseWPF)
                {
                    if (PtRenderingParams.ShaderParamsToUpdate.Count != 0)
                        UpdateShaderParams();
                }

                if (DoShowOctants)
                    OocLoader.ShowOctants(_scene);
                
                _sceneRenderer.Render(RC);
            }

            //Render GUI
            _sih.View = RC.View;
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();

            ReadyToLoadNewFile = true;

            Diagnostics.Log(FramePerSecond);
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
            if (!PtRenderingParams.CalcSSAO && PtRenderingParams.Lighting == Lighting.UNLIT) return;

            //(re)create depth tex and fbo
            if (_isTexInitialized)
            {
                RC.RemoveTextureHandle(_texHandle);
                //_texHandle = RC.CreateWritableTexture(Width, Height, WritableTextureFormat.Depth);

                //TODO: resize writable textures?

                _depthPassEf = PtRenderingParams.DepthPassEffect(new float2(Width, Height), InitCameraPos.z, _octreeTex, _octreeRootCenter, _octreeRootLength);
                _colorPassEf = PtRenderingParams.ColorPassEffect(new float2(Width, Height), InitCameraPos.z, new float2(ZNear, ZFar), (WritableTexture)_renderTarget.RenderTextures[3], _octreeTex, _octreeRootCenter, _octreeRootLength);
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
            InitCameraPos = _cameraPos = new float3((float)ptOctantComp.Center.x, (float)ptOctantComp.Center.y, (float)(ptOctantComp.Center.z - (ptOctantComp.Size * 2f)));            

            _scene.Children.Add(root);

            OocLoader.RootNode = root;
            OocLoader.FileFolderPath = PtRenderingParams.PathToOocFile;

            var octreeTexImgData = new ImageData(ColorFormat.iRGBA, OocFileReader.NumberOfOctants, 1);
            _octreeTex = new Texture(octreeTexImgData);
            OocLoader.VisibleOctreeHierarchyTex = _octreeTex;

            var byteSize = OocFileReader.NumberOfOctants * octreeTexImgData.PixelFormat.BytesPerPixel;
            octreeTexImgData.PixelData = new byte[byteSize];

            var ptRootComponent = root.GetComponent<PtOctantComponent>();
            _octreeRootCenter = ptRootComponent.Center;
            _octreeRootLength = ptRootComponent.Size;

            _depthPassEf = PtRenderingParams.DepthPassEffect(new float2(Width, Height), InitCameraPos.z, _octreeTex, _octreeRootCenter, _octreeRootLength);
            _colorPassEf = PtRenderingParams.ColorPassEffect(new float2(Width, Height), InitCameraPos.z, new float2(ZNear, ZFar), (WritableTexture)_renderTarget.RenderTextures[3]/*_texHandle*/, _octreeTex, _octreeRootCenter, _octreeRootLength);

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
            _cameraPos = InitCameraPos;
            _angleHorz = _angleVert = 0;
        }

        private void CalcAndSetViewMat()
        {
            if ((_angleHorz >= twoPi && _angleHorz > 0f) || _angleHorz <= -twoPi)
                _angleHorz %= twoPi;
            if ((_angleVert >= twoPi && _angleVert > 0f) || _angleVert <= -twoPi)
                _angleVert %= twoPi;

            _cameraPos += RC.View.Row2.xyz * Keyboard.WSAxis * Time.DeltaTime * 20;
            _cameraPos += RC.View.Row0.xyz * Keyboard.ADAxis * Time.DeltaTime * 20;

            RC.View = FPSView(_cameraPos, _angleVert, _angleHorz);
            _scenePicker.View = RC.View;
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

        #region UI

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
            var textToDisplay = "FUSEE Point Cloud Viewer";
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
            AddResizeDelegate(delegate
            {
                canvasProjComp.Resize(Width, Height);
                RC.Viewport(0, 0, Width, Height);
            });

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