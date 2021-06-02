using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.Primitives;
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

        private SceneNode _sphere;
        private Texture _texture;
        private DefaultSurfaceEffect _shader;

        private const float ZNear = 0.1f;
        private const float ZFar = 1000;
        private float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.Screen;
        private float _initCanvasWidth;
        private float _initCanvasHeight;
        private float _canvasWidth = 16;
        private float _canvasHeight = 9;

        private Transform _modelCamTransform;
        private Transform _fromCamTransform;
        private Transform _backCamTransform;
        private Transform _rotCamTransform;
        private Transform _guiCamTransform;
        private readonly Fusee.Engine.Core.Scene.Camera _modelCam = new Fusee.Engine.Core.Scene.Camera(Fusee.Engine.Core.Scene.ProjectionMethod.Perspective, 1, 100, M.PiOver4);
        private readonly Fusee.Engine.Core.Scene.Camera _frontCam = new Fusee.Engine.Core.Scene.Camera(Fusee.Engine.Core.Scene.ProjectionMethod.Perspective, 1, 100, M.PiOver4);
        private readonly Fusee.Engine.Core.Scene.Camera _backCam = new Fusee.Engine.Core.Scene.Camera(Fusee.Engine.Core.Scene.ProjectionMethod.Perspective, 1, 100, M.PiOver4);
        private readonly Fusee.Engine.Core.Scene.Camera _rotCam = new Fusee.Engine.Core.Scene.Camera(Fusee.Engine.Core.Scene.ProjectionMethod.Perspective, 1, 100, M.PiOver4);
        private readonly Fusee.Engine.Core.Scene.Camera _guiCam = new Fusee.Engine.Core.Scene.Camera(Fusee.Engine.Core.Scene.ProjectionMethod.Orthographic, 1, 1000, M.PiOver4);

        private float _anlgeHorzMain;
        private float _angleVertMain;
        private float _valHorzMain;
        private float _valVertMain;

        private float _angleHorzCam;

        private readonly float _rotAngle = M.PiOver4;
        private float3 _rotAxis;
        private float3 _rotPivot;

        private bool _pick;
        private SceneRayCaster _sceneRayCaster;

        private float _dist = 0.2f;
        private int _rings = 3;
        private int _size = 10;

        // Init is called on startup.
        public override void Init()
        {
            _frontCam.Viewport = new float4(0, 50, 50, 50);
            _frontCam.BackgroundColor = new float4(0.5f, 0.5f, 0.5f, 1);
            _frontCam.Layer = 10;

            _backCam.Viewport = new float4(50, 50, 50, 50);
            _backCam.BackgroundColor = new float4(0.5f, 0.5f, 0.5f, 1);
            _backCam.Layer = 10;

            _modelCam.Viewport = new float4(0, 0, 50, 50);
            _modelCam.BackgroundColor = new float4(0.5f, 0.5f, 0.5f, 1);
            _modelCam.Layer = 10;

            _rotCam.Viewport = new float4(50, 0, 50, 50);
            _rotCam.BackgroundColor = new float4(0.5f, 0.5f, 0.5f, 1);
            _rotCam.Layer = 10;

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
            _scene = AssetStorage.Get<SceneContainer>("sphere.fus");
            //_scene = CreateScene();

            _sphere = _scene.Children[0];
            _shader = _sphere.GetComponent<DefaultSurfaceEffect>();

            ImageData image = AssetStorage.Get<ImageData>("red.png");
            _texture = new Texture(image);

            _shader.SetFxParam("SurfaceInput.AlbedoTex", _texture);

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
                    MakeEffect.FromDiffuseSpecular((float4)ColorUint.Blue, float4.Zero, 4.0f, 1f),
                    _modelCam,
                    new Cube(),

                },
                Children = new ChildList()
                {
                    new SceneNode()
                    {
                        Name = "ModelCamChild",
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Scale = new float3(0.5f, 0.5f, 1f),
                                Translation = new float3(0,0, 1f)
                            },
                            new Cube()
                        }
                    },
                    new SceneNode()
                    {
                        Name = "Line",
                        Components = new List<SceneComponent>()
                        {
                            new Line(new List<float3>()
                            {
                                new float3(0, 0, 100),
                                new float3(0, 0, -100)
                            }, 0.1f),
                            MakeEffect.FromDiffuseSpecular((float4)ColorUint.Black, float4.Zero, 4.0f, 1f),
                        }
                    }
                }
            };

            _fromCamTransform = _guiCamTransform = new Transform()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 0, -30),
                Scale = float3.One
            };

            _backCamTransform = new Transform()
            {
                Rotation = new float3(0, M.Pi, 0),
                Translation = new float3(0, 0, 30),
                Scale = float3.One
            };

            _rotCamTransform = new Transform()
            {
                Rotation = new float3(0, 0, 0),
                Translation = new float3(0, 20, 30),
                Scale = float3.One
            };
            var initialPos = float4x4.LookAt(_rotCamTransform.Translation, float3.Zero, float3.UnitY);
            _rotCamTransform.Rotate(initialPos.RotationComponent());

            var cam1 = new SceneNode()
            {
                Name = "FrontCam",
                Components = new List<SceneComponent>()
                {
                    _fromCamTransform,
                    _frontCam,
                }
            };

            var cam2 = new SceneNode()
            {
                Name = "BackCam",
                Components = new List<SceneComponent>()
                {
                    _backCamTransform,
                    _backCam,
                }
            };

            var cam3 = new SceneNode()
            {
                Name = "RotationCam",
                Components = new List<SceneComponent>()
                {
                    _rotCamTransform,
                    _rotCam,
                }
            };

            _anlgeHorzMain = _modelCamTransform.Rotation.y;
            _angleVertMain = _modelCamTransform.Rotation.x;
            _angleHorzCam = _rotCamTransform.Rotation.y;

            _gui = CreateGui();

            _scene.Children.Add(modelCam);
            _scene.Children.Add(cam1);
            _scene.Children.Add(cam2);
            _scene.Children.Add(cam3);

            _sceneRenderer = new SceneRendererForward(_scene);
            _sceneRayCaster = new SceneRayCaster(_scene, Cull.Counterclockwise);

            _sih = new SceneInteractionHandler(_gui);
            _guiRenderer = new SceneRendererForward(_gui);

            _rotAxis = float3.UnitY * float4x4.CreateRotationYZ(new float2(M.PiOver4, M.PiOver4));
            _rotPivot = _scene.Children[1].GetComponent<Transform>().Translation;
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
                _valHorzMain = Mouse.XVel * 0.003f * DeltaTime;
                _valVertMain = Mouse.YVel * 0.003f * DeltaTime;

                _anlgeHorzMain += _valHorzMain;
                _angleVertMain += _valVertMain;

                _valHorzMain = _valVertMain = 0;

                _modelCamTransform.FpsView(_anlgeHorzMain, _angleVertMain, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTime * 10);
            }

            if (Keyboard.IsKeyUp(KeyCodes.Space))
                _pick = true;
            else
                _pick = false;


            _rotCamTransform.RotateAround(float3.Zero, new float3(0, DeltaTime * 0.1f, 0));


            // Check
            if (_pick)
            {
                float3 origin = _modelCamTransform.Translation;
                float3 direction = float4x4.Transform(_modelCamTransform.Matrix(), new float3(0, 0, 1));

                CastArea(origin, direction);

                _pick = false;
            }

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();
        }

        private void CastArea(float3 origin, float3 direction)
        {
            Rayf centerRay = new Rayf(origin, direction);
            var newPick = _sceneRayCaster.RayCast(centerRay).ToList();

            for (int i = 1; i <= _rings; i++)
            {
                float dist = _dist * i;
                double rays = System.Math.Pow(2, i + 1);
                double angle = 360 / rays;

                float3 top = float3.Add(origin, new float3(0, dist, 0));
                Rayf ray = new Rayf(top, direction);

                for (int j = 1; j < rays; j++)
                {
                    float3 point = float3.Rotate(new float3(0, 0, (float)(j * angle)), top, true);
                    point = float4x4.Transform(_modelCamTransform.Matrix(), point);

                    ray.Origin = point;
                    newPick.AddRange(_sceneRayCaster.RayCast(ray));
                }

                top = float4x4.Transform(_modelCamTransform.Matrix(), float3.Add(origin, new float3(0, dist, 0)));
                ray.Origin = top;

                newPick.AddRange(_sceneRayCaster.RayCast(ray));
            }

            var size = _size;

            foreach (var hit in newPick)
            {
                if (hit.DistanceFromOrigin > 5)
                {
                    var cube = new SceneNode()
                    {
                        Name = "Cube",
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Rotation = float3.Zero,
                                Translation = hit.WorldPos,
                                Scale = new float3(0.2f, 0.2f, 0.2f)
                            },
                            MakeEffect.FromDiffuseSpecular((float4)ColorUint.Blue, float4.Zero, 4.0f, 1f),
                            new Cube()
                        }
                    };
                    _scene.Children.Add(cube);

                    int x = (int)(_texture.Width * hit.UV.x);
                    int y = (int)(_texture.Height * hit.UV.y);
                    _texture.Blt(x - size / 2, y - size / 2, AssetStorage.Get<ImageData>("green.png"), 0, 0, size, size);
                }
            }
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
                UIElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                //Define Offset and therefor the size of the element.
                UIElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, _initCanvasHeight - 0.5f), _initCanvasHeight, _initCanvasWidth, new float2(1.75f, 0.5f)),
                float2.One
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 24);

            var text = new TextNode(
                "Completion Scan",
                "ButtonText",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                UIElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(_initCanvasWidth / 2 - 4, 0), _initCanvasHeight, _initCanvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery),
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

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
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, (float4)ColorUint.Black);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 0.8f);
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, float4.One);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 1f);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }
    }
}