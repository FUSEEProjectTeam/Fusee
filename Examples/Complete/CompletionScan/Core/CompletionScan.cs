using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.CompletionScan.Core
{
    [FuseeApplication(Name = "FUSEE Picking Example", Description = "How to use the Scene Picker.")]
    public class CompletionScan : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver4, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private ScenePicker _scenePicker;

        private SceneNode _sphere;
        private Texture _texture;
        private ShaderEffect _shader;

        private bool _keys;

        private const float ZNear = 0.1f;
        private const float ZFar = 1000;
        private float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;
        private float _initCanvasWidth;
        private float _initCanvasHeight;
        private float _canvasWidth = 16;
        private float _canvasHeight = 9;

        private PickResult _currentPick;
        private bool _pick;
        private float2 _pickPos;


        private Transform _modelCamTransform;
        private Transform _mainCamTransform;
        private Transform _sndCamTransform;
        private Transform _guiCamTransform;
        private readonly Fusee.Engine.Core.Scene.Camera _modelCam = new Fusee.Engine.Core.Scene.Camera(Fusee.Engine.Core.Scene.ProjectionMethod.Perspective, 5, 100, M.PiOver4);
        private readonly Fusee.Engine.Core.Scene.Camera _mainCam = new Fusee.Engine.Core.Scene.Camera(Fusee.Engine.Core.Scene.ProjectionMethod.Perspective, 1, 100, M.PiOver4);
        private readonly Fusee.Engine.Core.Scene.Camera _sndCam = new Fusee.Engine.Core.Scene.Camera(Fusee.Engine.Core.Scene.ProjectionMethod.Perspective, 1, 100, M.PiOver4);
        private readonly Fusee.Engine.Core.Scene.Camera _guiCam = new Fusee.Engine.Core.Scene.Camera(Fusee.Engine.Core.Scene.ProjectionMethod.Orthographic, 1, 1000, M.PiOver4);

        private float _anlgeHorzMain;
        private float _angleVertMain;
        private float _valHorzMain;
        private float _valVertMain;

        private readonly float _rotAngle = M.PiOver4;
        private float3 _rotAxis;
        private float3 _rotPivot;



        // Init is called on startup.
        public override async Task<bool> Init()
        {
            _modelCam.Viewport = new float4(0, 0, 100, 100);
            _modelCam.BackgroundColor = new float4(0f, 0f, 0f, 1);
            _modelCam.Layer = -1;

            _mainCam.Viewport = new float4(0, 0, 50, 100);
            _mainCam.BackgroundColor = new float4(0.5f, 0.5f, 0.5f, 1);
            _mainCam.Layer = 10;

            _sndCam.Viewport = new float4(50, 0, 50, 100);
            _sndCam.BackgroundColor = new float4(0.5f, 0.5f, 0.5f, 1);
            _sndCam.Layer = 10;

            _guiCam.ClearColor = false;
            _guiCam.ClearDepth = false;
            _guiCam.FrustumCullingOn = false;

            _initCanvasWidth = Width / 100f;
            _initCanvasHeight = Height / 100f;

            _canvasHeight = _initCanvasHeight;
            _canvasWidth = _initCanvasWidth;

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Create the sphere model
            _scene = AssetStorage.Get<SceneContainer>("sphere_highpoly.fus");

            _sphere = _scene.Children[0];
            _shader = _sphere.GetComponent<ShaderEffect>();

            ImageData image = AssetStorage.Get<ImageData>("green.png");
            _texture = new Texture(image);

            _shader.SetEffectParam("AlbedoTexture", _texture);

            // Set up cameras
            _modelCamTransform = new Transform()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 0, 0),
                Scale = float3.One
            };

            var modelCam = new SceneNode()
            {
                Name = "ModelCam",
                Components = new List<SceneComponent>()
                {
                    _modelCamTransform,
                    _modelCam,
                    ShaderCodeBuilder.MakeShaderEffect(new float4(1,0,0,1), float4.One, 10),
                    new Cube(),

                },
                Children = new ChildList()
                {
                    new SceneNode()
                    {
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Scale = new float3(0.5f, 0.5f, 1f),
                                Translation = new float3(0,0, 1f)
                            },
                            new Cube()
                        }
                    }
                }
            };

            _mainCamTransform = _guiCamTransform = new Transform()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 1, -30),
                Scale = float3.One
            };

            _sndCamTransform = new Transform()
            {
                Rotation = new float3(0, M.Pi, 0),
                Translation = new float3(0, 1, 30),
                Scale = float3.One
            };

            var cam1 = new SceneNode()
            {
                Name = "MainCam",
                Components = new List<SceneComponent>()
                {
                    _mainCamTransform,
                    _mainCam,
                }
            };

            var cam2 = new SceneNode()
            {
                Name = "SndCam",
                Components = new List<SceneComponent>()
                {
                    _sndCamTransform,
                    _sndCam,
                }
            };

            _anlgeHorzMain = _modelCamTransform.Rotation.y;
            _angleVertMain = _modelCamTransform.Rotation.x;

            _gui = CreateGui();

            _scene.Children.Add(modelCam);
            _scene.Children.Add(cam1);
            _scene.Children.Add(cam2);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _scenePicker = new ScenePicker(_scene);

            
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);
            _guiRenderer = new SceneRendererForward(_gui);

            _rotAxis = float3.UnitY * float4x4.CreateRotationYZ(new float2(M.PiOver4, M.PiOver4));
            _rotPivot = _scene.Children[1].GetComponent<Transform>().Translation;

            return true;
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            // Mouse and keyboard movement
            if (Mouse.LeftButton)
            {
                _keys = false;

                _valHorzMain = Mouse.XVel * 0.003f * DeltaTime;
                _valVertMain = Mouse.YVel * 0.003f * DeltaTime;

                _anlgeHorzMain += _valHorzMain;
                _angleVertMain += _valVertMain;

                _valHorzMain = _valVertMain = 0;

                _modelCamTransform.FpsView(_anlgeHorzMain, _angleVertMain, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTime * 10);
            }
            else if (Keyboard.GetKey(KeyCodes.Space))
            {
                _pick = true;
                _pickPos = new float2(Width / 2, Height / 2);
            }
            else if (Mouse.MiddleButton)
            {
                _pick = true;
                _pickPos = Mouse.Position;
            }
            else
            {
                _pick = false;
            }

            // Check
            if (_pick)
            {
                float2 pickPosClip = (_pickPos * new float2(2.0f / Width, -2.0f / Height)) + new float2(-1, 1);

                RC.View = _modelCam.GetProjectionMat(Width, Height, out var viewport) * float4x4.Invert(_modelCamTransform.Matrix());

                PickResult newPick = _scenePicker.Pick(RC, pickPosClip).ToList().OrderBy(pr => pr.ClipPos.z).FirstOrDefault();

                if (newPick != null)
                {
                    int x = (int)(_texture.Width * newPick.UV.x);
                    int y = (int)(_texture.Height * newPick.UV.y);

                    _texture.Blt(x - 50, y - 50, AssetStorage.Get<ImageData>("red.png"), 0, 0, 100, 100);

                    var cube = new SceneNode()
                    {
                        Name = "Cube",
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Rotation = float3.Zero,
                                Translation = newPick.WorldPos,
                                Scale = new float3(0.1f, 0.1f, 0.1f)
                            },
                            ShaderCodeBuilder.MakeShaderEffect(new float4(0,0,1,1), float4.One, 10),
                            new Cube(),

                        }
                    };

                    _scene.Children.Add(cube);
                }

                _pick = false;
            }
            
            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();
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
            var fuseeLogo = new TextureNode(
                "fuseeLogo",
                vsTex,
                psTex,
                //Set the albedo texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_LEFT),
                //Define Offset and therefor the size of the element.
                UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(0, _initCanvasHeight - 0.5f), _initCanvasHeight, _initCanvasWidth, new float2(1.75f, 0.5f))
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 24);

            var text = new TextNode(
                "Completion Scan",
                "ButtonText",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_HORIZONTAL),
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_HORIZONTAL, new float2(_initCanvasWidth / 2 - 4, 0), _initCanvasHeight, _initCanvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery),
                HorizontalTextAlignment.CENTER,
                VerticalTextAlignment.CENTER);

            var canvas = new CanvasNode(
                "Canvas",
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-_canvasWidth / 2, -_canvasHeight / 2f),
                    Max = new float2(_canvasWidth / 2, _canvasHeight / 2f)
                }
            );
            canvas.Children.Add(fuseeLogo);
            canvas.Children.Add(text);

            var cam = new SceneNode()
            {
                Name = "GUICam",
                Components = new List<SceneComponent>()
                {
                    _guiCamTransform,
                    _guiCam
                }
            };

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    cam,
                    canvas
                }
            };
        }

        public void BtnLogoEnter(CodeComponent sender)
        {
            _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>().SetEffectParam(UniformNameDeclarations.AlbedoColor, new float4(0.8f, 0.8f, 0.8f, 1f));
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>().SetEffectParam(UniformNameDeclarations.AlbedoColor, float4.One);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }
    }
}