﻿using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FontMap = Fusee.Engine.Core.FontMap;

namespace Fusee.Examples.UI.Core
{
    [FuseeApplication(Name = "FUSEE UI Example")]
    public class UI : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRendererForward _guiRenderer;

        private bool _keys;

        private Texture _bltDestinationTex;

        private SceneInteractionHandler _sih;
        private GuiButton _btnCanvas;
        private GuiButton _btnCat;

        private FontMap _fontMap;

        private readonly Camera _uiCam = new(ProjectionMethod.Perspective, 0.1f, 1000, M.PiOver4)
        {
            BackgroundColor = float4.One
        };
        public CanvasRenderMode CanvasRenderMode
        {
            get
            {
                return _canvasRenderMode;
            }
            set
            {
                _canvasRenderMode = value;
                if (_canvasRenderMode == CanvasRenderMode.World)
                {
                    _uiCam.ProjectionMethod = ProjectionMethod.Perspective;
                }
                else
                {
                    _uiCam.ProjectionMethod = ProjectionMethod.Orthographic;
                }
            }
        }
        private CanvasRenderMode _canvasRenderMode;

        private float _initWindowWidth;
        private float _initCanvasWidth;
        private float _initCanvasHeight;
        private float _canvasWidth = 16;
        private float _canvasHeight = 9;

        private Transform _camPivot;

        private readonly float4 _canvasDefaultColor = (float4)ColorUint.Red;
        private readonly float4 _canvasHoverColor = (float4)ColorUint.OrangeRed;

        private GuiText _fpsText;

        //Build a scene graph consisting out of a canvas and other UI elements.
        private SceneContainer CreateNineSliceScene()
        {
            var canvasScaleFactor = _initWindowWidth / _canvasWidth;

            float borderScaleFactor = 1;
            if (_canvasRenderMode == CanvasRenderMode.Screen)
            {
                borderScaleFactor = canvasScaleFactor;
            }

            var fps = TextNode.Create(
                "FPS: 0.00",
                "FPSText",
                GuiElementPosition.GetAnchors(AnchorPos.DownDownRight),
                new MinMaxRect
                {
                    Min = new float2(-2, 0),
                    Max = new float2(0, 1)
                },
                 _fontMap,
                (float4)ColorUint.White,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center
            );

            _fpsText = fps.GetComponentsInChildren<GuiText>().FirstOrDefault();

            var text = TextNode.Create(
                "The five\n" +
                "boxing wizards\n" +
                "jump\n" +
                "quickly.",
                "ButtonText",
                GuiElementPosition.GetAnchors(AnchorPos.StretchAll),
                new MinMaxRect
                {
                    Min = new float2(1f, 0.5f),
                    Max = new float2(-1f, -0.5f)
                },
                _fontMap,
                (float4)ColorUint.Greenery,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            var catTextureNode = TextureNode.Create(
                "Cat",
                //Set the albedo texture you want to use.
                new Texture(AssetStorage.Get<ImageData>("Kitti.jpg"), false, TextureFilterMode.Linear),

                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                GuiElementPosition.GetAnchors(AnchorPos.StretchHorizontal),//Anchor is in the lower left corner of the parent. Anchor is in the lower right corner of the parent.

                //Define Offset and therefor the size of the element.
                //Min: distance to this elements Min anchor.
                //Max: distance to this elements Max anchor.
                GuiElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(_initCanvasWidth / 2 - 2.5f, 0), _initCanvasHeight, _initCanvasWidth, new float2(5, 4)),
                //Choose in how many tiles you want to split the inner part of the texture. Use float2.one if you want it stretched.
                new float2(5, 5),
                //Tell how many percent of the texture, seen from the edges, belongs to the border. Order: left, right, top, bottom.
                new float4(0.11f, 0.11f, 0.06f, 0.17f),
                4, 4, 4, 4,
                borderScaleFactor

            );
            catTextureNode.Children = new ChildList() { text };
            catTextureNode.Components.Add(_btnCat);

            var bltTextureNode = TextureNode.Create(
                "Blt",
                //Set the albedo texture you want to use.
                _bltDestinationTex,
                //_fontMap.Image,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                GuiElementPosition.GetAnchors(AnchorPos.DownDownLeft),//Anchor is in the lower left corner of the parent. Anchor is in the lower right corner of the parent.

                //Define Offset and therefor the size of the element.
                //Min: distance to this elements Min anchor.
                //Max: distance to this elements Max anchor.
                GuiElementPosition.CalcOffsets(AnchorPos.DownDownLeft, new float2(0, 0), _initCanvasHeight, _initCanvasWidth, new float2(4, 4)),
                float2.One);

            var quagganTextureNode1 = TextureNode.Create(
                "Quaggan1",
                new Texture(AssetStorage.Get<ImageData>("testTex.jpg"), false, TextureFilterMode.Linear),
                //In this setup the element will stay in the upper left corner of the parent and will not be stretched at all.
                GuiElementPosition.GetAnchors(AnchorPos.TopTopLeft), //Anchor is in the lower right corner.Anchor is in the lower left corner.
                GuiElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(2.5f, 0), 3, 6, new float2(1, 1)),

                new float2(1, 1),
                new float4(0.1f, 0.1f, 0.1f, 0.09f),
                1, 1, 1, 1,
                borderScaleFactor
            );

            var nineSliceTextureNode = TextureNode.Create(
                "testImage",
                new Texture(AssetStorage.Get<ImageData>("9SliceSprites-4.png")),
                //In this setup the element will stay in the upper right corner of the parent and will not be stretched at all.
                GuiElementPosition.GetAnchors(AnchorPos.TopTopRight),//Anchor is in the upper right corner.//Anchor is in the upper right corner.

                GuiElementPosition.CalcOffsets(AnchorPos.TopTopRight, new float2(_initCanvasWidth - 6, _initCanvasHeight - 3), _initCanvasHeight, _initCanvasWidth, new float2(6, 3)),

                new float2(2, 3),
                new float4(0.1f, 0.1f, 0.1f, 0.1f),
                2.5f, 2.5f, 2.5f, 2.5f,
                borderScaleFactor
            );
            nineSliceTextureNode.Children = new ChildList() { quagganTextureNode1, text };

            var quagganTextureNode = TextureNode.Create(
                "Quaggan",
                new Texture(AssetStorage.Get<ImageData>("testTex.jpg"), false, TextureFilterMode.Linear),
                //In this setup the element will stay in the upper left corner of the parent and will not be stretched at all.
                GuiElementPosition.GetAnchors(AnchorPos.TopTopLeft), //Anchor is in the lower right corner.Anchor is in the lower left corner.
                GuiElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, _initCanvasHeight - 1), _initCanvasHeight, _initCanvasWidth, new float2(6, 1)),
                new float2(5, 1),
                new float4(0.1f, 0.1f, 0.1f, 0.09f),
                1, 1, 1, 1,
                borderScaleFactor
            );

            var quagganTextureNode2 = TextureNode.Create(
                "Quaggan",
                new Texture(AssetStorage.Get<ImageData>("testTex.jpg"), false, TextureFilterMode.Linear),
                //In this setup the element will stay in the upper left corner of the parent and will not be stretched at all.
                GuiElementPosition.GetAnchors(AnchorPos.TopTopLeft), //Anchor is in the lower right corner.Anchor is in the lower left corner.
                GuiElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, _initCanvasHeight - 3), _initCanvasHeight, _initCanvasWidth, new float2(6, 1)),
                new float2(5, 1),
                new float4(0.1f, 0.1f, 0.1f, 0.09f),
                1, 1, 1, 1,
                borderScaleFactor
            );

            var quagganTextureNode3 = TextureNode.Create(
                "Quaggan",
                new Texture(AssetStorage.Get<ImageData>("testTex.jpg"), false, TextureFilterMode.Linear),
                //In this setup the element will stay in the upper left corner of the parent and will not be stretched at all.
                GuiElementPosition.GetAnchors(AnchorPos.StretchVertical), //Anchor is in the lower right corner. Anchor is in the lower left corner.
                GuiElementPosition.CalcOffsets(AnchorPos.StretchVertical, new float2(0, _initCanvasHeight - 5), _initCanvasHeight, _initCanvasWidth, new float2(6, 1)),
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

            canvas.AddComponent(MakeEffect.FromDiffuseSpecular((float4)ColorUint.Red));
            canvas.AddComponent(new Plane());
            canvas.AddComponent(_btnCanvas);

            _camPivot = new Transform()
            {
                Translation = new float3(0, 0, 0),
                Rotation = float3.Zero
            };

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    new SceneNode()
                    {
                        Name = "CamPivot",
                        Components = new List<SceneComponent>()
                        {
                            _camPivot
                        },
                        Children = new ChildList()
                        {
                            new SceneNode()
                            {
                                Name = "MainCam",
                                Components = new List<SceneComponent>()
                                {
                                    new Transform()
                                    {
                                        Translation = new float3(0, 0, -15),
                                        Rotation = float3.Zero
                                    },
                                    _uiCam
                                }
                            },
                        }
                    },
                    //Add canvas.
                    new SceneNode()
                    {
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Translation = new float3(0, 0, 0),
                                Rotation = new float3(0, M.PiOver4, 0)
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
            _scene.Children.FindNodes(node => node.Name == "Canvas").First().GetComponent<SurfaceEffect>().SurfaceInput.Albedo = _canvasHoverColor;
        }

        public void OnBtnCanvasExit(CodeComponent sender)
        {
            Debug.WriteLine("Canvas: Exit Btn!");
            _scene.Children.FindNodes(node => node.Name == "Canvas").First().GetComponent<SurfaceEffect>().SurfaceInput.Albedo = _canvasDefaultColor;
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
            CanvasRenderMode = CanvasRenderMode.World;

            _initWindowWidth = Width;
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

            _fontMap = new FontMap(fontLato, 24);

            _bltDestinationTex = new Texture(AssetStorage.Get<ImageData>("townmusicians_297x500.jpg"));
            var bltScrImgData = AssetStorage.Get<ImageData>("censored_rgba.png");
            _bltDestinationTex.Blt(210, 225, bltScrImgData);

            _btnCanvas = new GuiButton
            {
                Name = "Canvas_Button"
            };
            _btnCanvas.OnMouseUp += OnBtnCanvasUp;
            _btnCanvas.OnMouseDown += OnBtnCanvasDown;
            _btnCanvas.OnMouseEnter += OnBtnCanvasEnter;
            _btnCanvas.OnMouseExit += OnBtnCanvasExit;
            _btnCanvas.OnMouseOver += OnMouseOverBtnCanvas;

            _btnCat = new GuiButton
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

            // Wrap a SceneRenderer around the model.
            _guiRenderer = new SceneRendererForward(_scene);

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_scene, _guiRenderer.PrePassVisitor.CameraPrepassResults);
        }

        public override void Update()
        {
            // Mouse and keyboard movement
            if (Input.Keyboard.LeftRightAxis != 0 || Input.Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Input.Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = RotationSpeed * Input.Mouse.XVel * Time.DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * Input.Mouse.YVel * Time.DeltaTimeUpdate * 0.0005f;
            }
            else if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Input.Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = RotationSpeed * touchVel.x * Time.DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * touchVel.y * Time.DeltaTimeUpdate * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = RotationSpeed * Input.Keyboard.LeftRightAxis * Time.DeltaTimeUpdate;
                    _angleVelVert = RotationSpeed * Input.Keyboard.UpDownAxis * Time.DeltaTimeUpdate;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTimeUpdate);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            _camPivot.RotationQuaternion = QuaternionF.FromEuler(_angleVert, _angleHorz, 0);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            _fpsText.Text = "FPS: " + Time.FramesPerSecond.ToString("0.00");

            _guiRenderer.Render(RC);

            // Constantly check for interactive objects.
            if (!Input.Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(Input.Mouse.Position, Width, Height);

            if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Input.Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Input.Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            // Swap buffers: Show the contents of the back buffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}