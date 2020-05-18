using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FontMap = Fusee.Engine.Core.FontMap;

namespace Fusee.Examples.UI.Core
{
    [FuseeApplication(Name = "FUSEE UI Example")]
    public class UI : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver4, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;

        private bool _keys;

        private Texture _bltDestinationTex;

        private SceneInteractionHandler _sih;
        private GUIButton _btnCanvas;
        private GUIButton _btnCat;

        private FontMap _fontMap;
        private FontMap _fontMap1;

        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;
        private float _initWindowWidth;
        private float _initWindowHeight;
        private float _initCanvasWidth;
        private float _initCanvasHeight;
        private float _canvasWidth = 16;
        private float _canvasHeight = 9;

        private readonly float zNear = 1f;
        private readonly float zFar = 1000;
        private readonly float fov = M.PiOver4;

        private readonly float4 _canvasDefaultColor = new float4(1, 0f, 0f, 1);
        private readonly float4 _canvasHoverColor = new float4(1, 0.4f, 0.1f, 1);

        private GUIText _fpsText;

        //Build a scene graph consisting out of a canvas and other UI elements.
        private SceneContainer CreateNineSliceScene()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");
            var psText = AssetStorage.Get<string>("text.frag");
            var vsNineSlice = AssetStorage.Get<string>("nineSlice.vert");
            var psNineSlice = AssetStorage.Get<string>("nineSliceTile.frag");

            var canvasScaleFactor = _initWindowWidth / _canvasWidth;

            float borderScaleFactor = 1;
            if (_canvasRenderMode == CanvasRenderMode.Screen)
            {
                borderScaleFactor = canvasScaleFactor;
            }

            var fps = new TextNode(
                "FPS: 0.00",
                "FPSText",
                vsTex,
                psText,
                UIElementPosition.GetAnchors(AnchorPos.DOWN_DOWN_RIGHT),
                new MinMaxRect
                {
                    Min = new float2(-2, 0),
                    Max = new float2(0, 1)
                },
                 _fontMap,
                ColorUint.Tofloat4(ColorUint.White),
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center
            );

            _fpsText = fps.GetComponentsInChildren<GUIText>().FirstOrDefault();

            var text = new TextNode(
                "The five\n" +
                "boxing wizards\n" +
                "jump\n" +
                "quickly.",
                "ButtonText",
                vsTex,
                psText,
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_ALL),
                new MinMaxRect
                {
                    Min = new float2(1f, 0.5f),
                    Max = new float2(-1f, -0.5f)
                },
                _fontMap,
                ColorUint.Tofloat4(ColorUint.Greenery),
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            var catTextureNode = new TextureNode(
                "Cat",
                AssetStorage.Get<string>("nineSlice.vert"),
                AssetStorage.Get<string>("nineSliceTile.frag"),
                //Set the albedo texture you want to use.
                new Texture(AssetStorage.Get<ImageData>("Kitti.jpg"), false, TextureFilterMode.LINEAR),

                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.StretchHorizontal),//Anchor is in the lower left corner of the parent. Anchor is in the lower right corner of the parent.

                //Define Offset and therefor the size of the element.
                //Min: distance to this elements Min anchor.
                //Max: distance to this elements Max anchor.
                UIElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(_initCanvasWidth / 2 - 2.5f, 0), _initCanvasHeight, _initCanvasWidth, new float2(5, 4)),
                //Choose in how many tiles you want to split the inner part of the texture. Use float2.one if you want it stretched.
                new float2(5, 5),
                //Tell how many percent of the texture, seen from the edges, belongs to the border. Order: left, right, top, bottom.
                new float4(0.11f, 0.11f, 0.06f, 0.17f),
                4, 4, 4, 4,
                borderScaleFactor

            )
            { Children = new ChildList() { text } };
            catTextureNode.Components.Add(_btnCat);

            var bltTextureNode = new TextureNode(
                "Blt",
                vsTex,
                psTex,
                //Set the albedo texture you want to use.
                _bltDestinationTex,
                //_fontMap.Image,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.DownDownLeft),//Anchor is in the lower left corner of the parent. Anchor is in the lower right corner of the parent.

                //Define Offset and therefor the size of the element.
                //Min: distance to this elements Min anchor.
                //Max: distance to this elements Max anchor.
                UIElementPosition.CalcOffsets(AnchorPos.DOWN_DOWN_LEFT, new float2(0, 0), _initCanvasHeight, _initCanvasWidth, new float2(4, 4)),
                float2.One);

            var quagganTextureNode1 = new TextureNode(
                "Quaggan1",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("testTex.jpg"), false, TextureFilterMode.LINEAR),
                //In this setup the element will stay in the upper left corner of the parent and will not be stretched at all.
                UIElementPosition.GetAnchors(AnchorPos.TopTopLeft), //Anchor is in the lower right corner.Anchor is in the lower left corner.
                UIElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(2.5f, 0), 3, 6, new float2(1, 1)),

                new float2(1, 1),
                new float4(0.1f, 0.1f, 0.1f, 0.09f),
                1, 1, 1, 1,
                borderScaleFactor
            );

            var nineSliceTextureNode = new TextureNode(
                "testImage",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("9SliceSprites-4.png")),
                //In this setup the element will stay in the upper right corner of the parent and will not be stretched at all.
                UIElementPosition.GetAnchors(AnchorPos.TopTopRight),//Anchor is in the upper right corner.//Anchor is in the upper right corner.

                UIElementPosition.CalcOffsets(AnchorPos.TopTopRight, new float2(_initCanvasWidth - 6, _initCanvasHeight - 3), _initCanvasHeight, _initCanvasWidth, new float2(6, 3)),

                new float2(2, 3),
                new float4(0.1f, 0.1f, 0.1f, 0.1f),
                2.5f, 2.5f, 2.5f, 2.5f,
                borderScaleFactor
            )
            { Children = new ChildList() { quagganTextureNode1, text } };

            var quagganTextureNode = new TextureNode(
                "Quaggan",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("testTex.jpg"), false, TextureFilterMode.LINEAR),
                //In this setup the element will stay in the upper left corner of the parent and will not be stretched at all.
                UIElementPosition.GetAnchors(AnchorPos.TopTopLeft), //Anchor is in the lower right corner.Anchor is in the lower left corner.
                UIElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, _initCanvasHeight - 1), _initCanvasHeight, _initCanvasWidth, new float2(6, 1)),
                new float2(5, 1),
                new float4(0.1f, 0.1f, 0.1f, 0.09f),
                1, 1, 1, 1,
                borderScaleFactor
            );

            var quagganTextureNode2 = new TextureNode(
                "Quaggan",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("testTex.jpg"), false, TextureFilterMode.LINEAR),
                //In this setup the element will stay in the upper left corner of the parent and will not be stretched at all.
                UIElementPosition.GetAnchors(AnchorPos.TopTopLeft), //Anchor is in the lower right corner.Anchor is in the lower left corner.
                UIElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, _initCanvasHeight - 3), _initCanvasHeight, _initCanvasWidth, new float2(6, 1)),
                new float2(5, 1),
                new float4(0.1f, 0.1f, 0.1f, 0.09f),
                1, 1, 1, 1,
                borderScaleFactor
            );

            var quagganTextureNode3 = new TextureNode(
                "Quaggan",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("testTex.jpg"), false, TextureFilterMode.LINEAR),
                //In this setup the element will stay in the upper left corner of the parent and will not be stretched at all.
                UIElementPosition.GetAnchors(AnchorPos.StretchVertical), //Anchor is in the lower right corner. Anchor is in the lower left corner.
                UIElementPosition.CalcOffsets(AnchorPos.StretchVertical, new float2(0, _initCanvasHeight - 5), _initCanvasHeight, _initCanvasWidth, new float2(6, 1)),
                new float2(5, 1),
                new float4(0.1f, 0.1f, 0.1f, 0.09f),
                1, 1, 1, 1,
                borderScaleFactor
            );

            var canvas = new CanvasNode(
                "Canvas",
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-_canvasWidth / 2, -_canvasHeight / 2f),
                    Max = new float2(_canvasWidth / 2, _canvasHeight / 2f)
                })
            {
                Children = new ChildList()
                {
                    //Simple Texture Node, contains a Blt"ed" texture.
                    bltTextureNode,
                    //Add nine sliced textures to canvas
                    catTextureNode,
                    quagganTextureNode,
                    nineSliceTextureNode,
                    quagganTextureNode2,
                    quagganTextureNode3,
                    fps
                }
            };

            canvas.AddComponent(MakeEffect.FromDiffuseSpecular(new float4(1, 0, 0, 1), 0, 0));
            canvas.AddComponent(new Plane());
            canvas.AddComponent(_btnCanvas);

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    //Add canvas.

                    new SceneNode()
                    {
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Translation = new float3(0,0,0)
                            }
                        },
                        Children = new ChildList()
                        {
                            canvas
                        }
                    },

                }
            };
        }

        #region Interactions

        public void OnBtnCanvasDown(CodeComponent sender)
        {
            Debug.WriteLine("Canvas: Btn down!");
        }

        public void OnBtnCanvasUp(CodeComponent sender)
        {
            Debug.WriteLine("Canvas: Btn up!");
        }

        public void OnBtnCanvasEnter(CodeComponent sender)
        {
            Debug.WriteLine("Canvas: Btn entered!" + Time.Frames);
            _scene.Children.FindNodes(node => node.Name == "Canvas").First().GetComponent<DefaultSurfaceEffect>().SurfaceInput.Albedo = _canvasHoverColor;
        }

        public void OnBtnCanvasExit(CodeComponent sender)
        {
            Debug.WriteLine("Canvas: Exit Btn!");
            _scene.Children.FindNodes(node => node.Name == "Canvas").First().GetComponent<DefaultSurfaceEffect>().SurfaceInput.Albedo = _canvasDefaultColor;
        }

        public void OnBtnCatDown(CodeComponent sender)
        {
            Debug.WriteLine("Cat: Btn down!");
        }

        public void OnBtnCatUp(CodeComponent sender)
        {
            Debug.WriteLine("Cat: Btn up!");
        }

        public void OnBtnCatEnter(CodeComponent sender)
        {
            Debug.WriteLine("Cat: Btn entered!" + Time.Frames);
        }

        public void OnBtnCatExit(CodeComponent sender)
        {
            Debug.WriteLine("Cat: Exit Btn!");
        }

        public void OnMouseOverBtnCat(CodeComponent sender)
        {
            Debug.WriteLine("Cat: Mouse over!");
        }

        public void OnMouseOverBtnCanvas(CodeComponent sender)
        {
            //Debug.WriteLine("Canvas: Mouse over!");
        }

        #endregion Interactions

        // Init is called on startup.
        public override void Init()
        {
            _initWindowWidth = Width;
            _initWindowHeight = Height;
            if (_canvasRenderMode == CanvasRenderMode.Screen)
            {
                _initCanvasWidth = Width / 100f;
                _initCanvasHeight = Height / 100f;
            }
            else
            {
                _initCanvasWidth = 16;
                _initCanvasHeight = 9;
            }
            _canvasHeight = _initCanvasHeight;
            _canvasWidth = _initCanvasWidth;

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");

            _fontMap1 = new FontMap(fontLato, 8);
            _fontMap = new FontMap(fontLato, 24);

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            _bltDestinationTex = new Texture(AssetStorage.Get<ImageData>("townmusicians.jpg"));
            var bltScrTex = new Texture(AssetStorage.Get<ImageData>("censored_79_16.png"));
            _bltDestinationTex.Blt(180, 225, bltScrTex);

            _btnCanvas = new GUIButton
            {
                Name = "Canvas_Button"
            };
            _btnCanvas.OnMouseUp += OnBtnCanvasUp;
            _btnCanvas.OnMouseDown += OnBtnCanvasDown;
            _btnCanvas.OnMouseEnter += OnBtnCanvasEnter;
            _btnCanvas.OnMouseExit += OnBtnCanvasExit;
            _btnCanvas.OnMouseOver += OnMouseOverBtnCanvas;

            _btnCat = new GUIButton
            {
                Name = "Cat_Button"
            };
            _btnCat.OnMouseUp += OnBtnCatUp;
            _btnCat.OnMouseDown += OnBtnCatDown;
            _btnCat.OnMouseEnter += OnBtnCatEnter;
            _btnCat.OnMouseExit += OnBtnCatExit;
            _btnCat.OnMouseOver += OnMouseOverBtnCat;

            // Set the scene by creating a scene graph
            _scene = CreateNineSliceScene();

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_scene);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            _fpsText.Text = "FPS: " + Time.FramePerSecond.ToString("0.00");

            // Mouse and keyboard movement
            if (Input.Keyboard.LeftRightAxis != 0 || Input.Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Input.Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Input.Mouse.XVel * Time.DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Input.Mouse.YVel * Time.DeltaTime * 0.0005f;
            }
            else if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Input.Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * Time.DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * touchVel.y * Time.DeltaTime * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Input.Keyboard.LeftRightAxis * Time.DeltaTime;
                    _angleVelVert = -RotationSpeed * Input.Keyboard.UpDownAxis * Time.DeltaTime;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = float4x4.LookAt(0, 0, -15, 0, 0, 0, 0, 1, 0);
            var view = mtxCam * mtxRot;
            var projection = _canvasRenderMode == CanvasRenderMode.SCREEN ? float4x4.CreateOrthographic(Width, Height, zNear, zFar) : float4x4.CreatePerspectiveFieldOfView(fov, (float)Width / Height, zNear, zFar);

            RC.Projection = projection;
            RC.View = view;
            _sceneRenderer.Render(RC);

            // Constantly check for interactive objects.
            if (!Input.Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Input.Mouse.Position, Width, Height);

            if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Input.Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Input.Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            // Swap buffers: Show the contents of the back buffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}