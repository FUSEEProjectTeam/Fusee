using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Xene;
using FontMap = Fusee.Engine.Core.FontMap;

namespace Fusee.Engine.Examples.UI.Core
{
    [FuseeApplication(Name = "FUSEE UI Example", Description = "A very ui example.")]
    public class UI : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRenderer _guiRenderer;

        private bool _keys;

        private Texture _bltDestinationTex;

        private SceneInteractionHandler _sih;
        private GUIButton _btnCanvas;
        private GUIButton _btnCat;

        private FontMap _fontMap;
        private FontMap _fontMap1;

        private CanvasRenderMode _canvasRenderMode;

        //Build a scene graph consisting out of a canvas and other UI elements.
        private SceneContainer CreateNineSliceScene()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");
            var vsNineSlice = AssetStorage.Get<string>("nineSlice.vert");
            var psNineSlice = AssetStorage.Get<string>("nineSliceTile.frag");

            _canvasRenderMode = CanvasRenderMode.SCREEN;
                       
            float textSize = 2;
            float borderScaleFactor = 1;
            if (_canvasRenderMode == CanvasRenderMode.SCREEN)
            {
                textSize *= 100f;
                borderScaleFactor = 100f;
            }

            var text = new TextNodeContainer(
                "Hallo !",
                "ButtonText",
                vsTex,
                psTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                new MinMaxRect
                {
                    Min = new float2(1f, 0.5f),
                    Max = new float2(-1f, -0.5f)
                },
                _fontMap,
                ColorUint.Tofloat4(ColorUint.Greenery), textSize);

            var catTextureNode = new TextureNodeContainer(
                "Cat",
                AssetStorage.Get<string>("nineSlice.vert"),
                AssetStorage.Get<string>("nineSliceTile.frag"),
                //Set the diffuse texture you want to use.
                new Texture(AssetStorage.Get<ImageData>("Kitti.jpg")),
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                new MinMaxRect
                {
                    Min = new float2(0, 0), //Anchor is in the lower left corner of the parent.
                    Max = new float2(1, 0) //Anchor is in the lower right corner of the parent
                },
                //Define Offset and therefor the size of the element.
                //Min: distance to this elements Min anchor.
                //Max: distance to this elements Max anchor.
                new MinMaxRect
                {
                    Min = new float2(5.5f, 0),
                    Max = new float2(-5.5f, 4f)
                },
                //Choose in how many tiles you want to split the inner part of the texture. Use float2.one if you want it stretched.
                new float2(5, 5),
                //Tell how many percent of the texture, seen from the edges, belongs to the border. Order: left, right, top, bottom.
                new float4(0.11f, 0.11f, 0.06f, 0.17f),
                4,4,4,4,
                borderScaleFactor
                
            ){ Children = new List<SceneNodeContainer> { text} };
            catTextureNode.Components.Add(_btnCat);

            var bltTextureNode = new TextureNodeContainer(
                "Blt",
                vsTex,
                psTex,
                //Set the diffuse texture you want to use.
                _bltDestinationTex,
                //_fontMap.Image,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                new MinMaxRect
                {
                    Min = new float2(0, 0), //Anchor is in the lower left corner of the parent.
                    Max = new float2(0, 0) //Anchor is in the lower right corner of the parent
                },
                //Define Offset and therefor the size of the element.
                //Min: distance to this elements Min anchor.
                //Max: distance to this elements Max anchor.
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(4, 4)
                });

            var quagganTextureNode1 = new TextureNodeContainer(
                "Quaggan1",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("testTex.jpg")),
                //In this setup the element will stay in the upper left corner of the parent and will not be stretched at all.
                new MinMaxRect
                {
                    Min = new float2(0, 0), //Anchor is in the lower right corner.
                    Max = new float2(1, 0) //Anchor is in the lower left corner.
                },
                new MinMaxRect
                {
                    Min = new float2(2.5f, 0),
                    Max = new float2(-2.5f, 1)
                },
                new float2(1, 1),
                new float4(0.1f, 0.1f, 0.1f, 0.09f),
                1,1,1,1,
                borderScaleFactor
            );

            var nineSliceTextureNode = new TextureNodeContainer(
                "testImage",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("9SliceSprites-4.png")),
                //In this setup the element will stay in the upper right corner of the parent and will not be stretched at all.
                new MinMaxRect
                {
                    Min = new float2(1, 1), //Anchor is in the upper right corner.
                    Max = new float2(1, 1) //Anchor is in the upper right corner.
                },
                new MinMaxRect {Min = new float2(-6, -3f), Max = new float2(0, 0)},
                new float2(2, 3),
                new float4(0.1f, 0.1f, 0.1f, 0.1f),
                2.5f, 2.5f, 2.5f, 2.5f,
                borderScaleFactor
            ) {Children = new List<SceneNodeContainer> {text, quagganTextureNode1}};

            var quagganTextureNode = new TextureNodeContainer(
                "Quaggan",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("testTex.jpg")),
                //In this setup the element will stay in the upper left corner of the parent and will not be stretched at all.
                new MinMaxRect
                {
                    Min = new float2(0, 1), //Anchor is in the upper left corner.
                    Max = new float2(0, 1) //Anchor is in the upper left corner.
                },
                new MinMaxRect
                {
                    Min = new float2(0, -1),
                    Max = new float2(6, 0)
                },
                new float2(5, 1),
                new float4(0.1f, 0.1f, 0.1f, 0.09f),
                1, 1, 1, 1,
                borderScaleFactor
            );

            var canvas = new CanvasNodeContainer(
                "Canvas",
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-8, -4.5f),
                    Max = new float2(8, 4.5f)
                })
            {
                Children = new List<SceneNodeContainer>()
                {
                    //Simple Texture Node, contains a Blt"ed" texture.
                    bltTextureNode,
                    //Add nine sliced textures to canvas
                    catTextureNode,
                    quagganTextureNode,
                    nineSliceTextureNode
                }
            };            
            
            var canvasMat = new ShaderEffectComponent
            {
                Effect = ShaderCodeBuilder.MakeShaderEffectFromMatComp(new MaterialComponent
                {
                    Diffuse = new MatChannelContainer {Color = new float3(1, 0, 0)},
                })
            };
            canvas.AddComponent(canvasMat);
            canvas.AddComponent(new Plane());
            canvas.AddComponent(_btnCanvas);

            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    //Add canvas.
                    canvas
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
            var color = ShaderCodeBuilder.MakeShaderEffectFromMatComp(new MaterialComponent
            {
                Diffuse = new MatChannelContainer {Color = new float3(1, 0.4f, 0.1f)},
            });
            _scene.Children.FindNodes(node => node.Name == "Canvas").First().GetComponent<ShaderEffectComponent>()
                .Effect = color;
        }

        public void OnBtnCanvasExit(CodeComponent sender)
        {
            Debug.WriteLine("Canvas: Exit Btn!");
            var color = ShaderCodeBuilder.MakeShaderEffectFromMatComp(new MaterialComponent
            {
                Diffuse = new MatChannelContainer {Color = new float3(1, 0, 0)},
            });
            _scene.Children.FindNodes(node => node.Name == "Canvas").First().GetComponent<ShaderEffectComponent>()
                .Effect = color;
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

        #endregion

        // Init is called on startup. 
        public override void Init()
        {
            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");

            _fontMap1 = new FontMap(fontLato, 8);
            _fontMap = new FontMap(fontLato, 72);

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);
            RCGui.ClearColor = new float4(0, 0, 0, 0);

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
            _guiRenderer = new SceneRenderer(_scene);
        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            //RCGui.Clear(ClearFlags.Color | ClearFlags.Depth);

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
                    var curDamp = (float) System.Math.Exp(-Damping * Time.DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            var mtxRot = float4x4.Identity;
            if (_canvasRenderMode == CanvasRenderMode.WORLD)
                mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);

            var mtxCam = float4x4.LookAt(0, 0, -15, 0, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRot;

            // Render the scene loaded in Init()
            _guiRenderer.Render(RCGui);

            //Set the view matrix for the interaction handler.
            _sih.View = RC.ModelView;

            // Constantly check for interactive objects.
            _sih.CheckForInteractiveObjects(Input.Mouse.Position, Width, Height);

            // Swap buffers: Show the contents of the back buffer (containing the currently rendered frame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);
            RCGui.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float) Height;

            var distToNearClippingPlane = 1;
            var distToFarClippingPlane = 20000;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            float4x4 projection;
            if (_canvasRenderMode == CanvasRenderMode.SCREEN)
            {
                projection = float4x4.CreateOrthographic(Width, Height, distToNearClippingPlane, distToFarClippingPlane);
            }
            else
            {
                projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, distToNearClippingPlane,
                    distToFarClippingPlane);
            }
            RCGui.Projection = projection;
            _sih.Projection = projection;
        }
    }
}