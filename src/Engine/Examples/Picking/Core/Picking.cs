#define GUI_SIMPLE

// dynamic magic works @desktopbuild only! 
#define WEBBUILD

using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
#if GUI_SIMPLE

#endif

namespace Fusee.Engine.Examples.Picking.Core
{

    [FuseeApplication(Name = "FUSEE Picking Example", Description = "How to use the Scene Picker.")]
    public class Picking : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver4, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private ScenePicker _scenePicker;

        private bool _keys;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private float _aspectRatio;
        private float _fovy = M.PiOver4;

#if GUI_SIMPLE

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

#endif
        private PickResult _currentPick;
        private float3 _oldColor;
        private bool _pick;
        private float2 _pickPos;

        // Init is called on startup. 
        public override void Init()
        {
            _initWindowWidth = Width;
            _initWindowHeight = Height;

            _initCanvasWidth = Width / 100f;
            _initCanvasHeight = Height / 100f;

            _canvasHeight = _initCanvasHeight;
            _canvasWidth = _initCanvasWidth;

            _aspectRatio = Width / (float)Height;

            // Set the clear color for the backbuffer to white (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Create the robot model
            _scene = CreateScene();

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_scene);
            _scenePicker = new ScenePicker(_scene);

#if GUI_SIMPLE
            _gui = CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);
            _guiRenderer = new SceneRenderer(_gui);
#endif
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Mouse and keyboard movement
            if (Input.Keyboard.LeftRightAxis != 0 || Input.Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Input.Mouse.LeftButton)
            {
                _pick = true;
                _pickPos = Input.Mouse.Position;
                _keys = false;
                _angleVelHorz = -RotationSpeed * Input.Mouse.XVel * Time.DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Input.Mouse.YVel * Time.DeltaTime * 0.0005f;
            }
            else if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _pick = true;
                _pickPos = Input.Touch.GetPosition(TouchPoints.Touchpoint_0);
                var touchVel = Input.Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * Time.DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * touchVel.y * Time.DeltaTime * 0.0005f;
            }
            else
            {
                _pick = false;
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

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 20, -600, 0, 150, 0, 0, 1, 0);

            // Check 
            if (_pick)
            {
                Diagnostics.Log(_pickPos);
                float2 pickPosClip = _pickPos * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1);

                _scenePicker.View = mtxCam * mtxRot;

                PickResult newPick = _scenePicker.Pick(pickPosClip).ToList().OrderBy(pr => pr.ClipPos.z).FirstOrDefault();

#if WEBBUILD

                if (newPick?.Node != _currentPick?.Node)
                {

                    if (_currentPick != null)
                    {
                        var ef = _currentPick.Node.GetComponent<ShaderEffectComponent>().Effect;
                        ef.SetEffectParam("DiffuseColor", _oldColor);
                    }
                    if (newPick != null)
                    {
                        var ef = newPick.Node.GetComponent<ShaderEffectComponent>().Effect;
                        _oldColor = (float3)ef.GetEffectParam("DiffuseColor"); // cast needed 
                        ef.SetEffectParam("DiffuseColor", ColorUint.Tofloat3(ColorUint.LawnGreen));
                    }
                    _currentPick = newPick;
                }
#else
                if (newPick?.Node != _currentPick?.Node)
                {
                    dynamic shaderEffectComponent; // this needs to be dynamic! & reference Microsoft.CSharp.dll

                    if (_currentPick != null)
                    {
                        shaderEffectComponent = _currentPick.Node.GetComponent<ShaderEffectComponent>().Effect;
                        shaderEffectComponent.DiffuseColor = _oldColor;

                    }
                    if (newPick != null)
                    {
                        shaderEffectComponent = newPick.Node.GetComponent<ShaderEffectComponent>().Effect;
                        _oldColor = (float3) shaderEffectComponent.DiffuseColor;
                        shaderEffectComponent.DiffuseColor = ColorUint.Tofloat3(ColorUint.LawnGreen);
                    }
                    _currentPick = newPick;
#endif


                _pick = false;
            }

            RC.ModelView = mtxCam * mtxRot;
            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
#if GUI_SIMPLE

            //Set the view matrix for the interaction handler.
            _sih.View = RC.ModelView;

            // Constantly check for interactive objects.
            _sih.CheckForInteractiveObjects(Input.Mouse.Position, Width, Height);

            if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Input.Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Input.Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }           
            _guiRenderer.Render(RC);          
#endif           
            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();
        }

        private static T ParseToType<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }


        private InputDevice Creator(IInputDeviceImp device)
        {
            throw new NotImplementedException();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            var resizeScaleFactor = new float2((100 / _initWindowWidth * Width) / 100, (100 / _initWindowHeight * Height) / 100);
            _canvasHeight = _initCanvasHeight * resizeScaleFactor.y;
            _canvasWidth = _initCanvasWidth * resizeScaleFactor.x;

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            _aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(_fovy, _aspectRatio, ZNear, ZFar);
            RC.Projection = projection;
            _scenePicker.Projection = projection;
        }

#if GUI_SIMPLE
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

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 18);

            var text = new TextNodeContainer(
                "FUSEE Picking Example",
                "ButtonText",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_HORIZONTAL),
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_HORIZONTAL, new float2(_initCanvasWidth / 2 - 4, 0), _initCanvasHeight, _initCanvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery), 250f);


            var canvas = new CanvasNodeContainer(
                "Canvas",
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-_canvasWidth / 2, -_canvasHeight / 2f),
                    Max = new float2(_canvasWidth / 2, _canvasHeight / 2f)
                })
            {
                Children = new List<SceneNodeContainer>()
                {
                    //Simple Texture Node, contains the fusee logo.
                    fuseeLogo,
                    text
                }
            };

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
#endif

        private SceneContainer CreateScene()
        {
            return new ConvertSceneGraph().Convert(new SceneContainer
            {
                Header = new SceneHeader
                {
                    CreationDate = "April 2017",
                    CreatedBy = "mch@hs-furtwangen.de",
                    Generator = "Handcoded with pride",
                    Version = 42,
                },
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = "Base",
                        Components = new List<SceneComponentContainer>
                        {
                            new TransformComponent { Scale = float3.One },
                           new MaterialComponent
                           {
                                Diffuse = new MatChannelContainer { Color = ColorUint.Tofloat3(ColorUint.Red) },
                                Specular = new SpecularChannelContainer {Color = ColorUint.Tofloat3(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                            },
                            CreateCuboid(new float3(100, 20, 100))
                        },
                        Children = new List<SceneNodeContainer>
                        {
                            new SceneNodeContainer
                            {
                                Name = "Arm01",
                                Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent {Translation=new float3(0, 60, 0),  Scale = float3.One },
                                   new MaterialComponent
                                    {
                                        Diffuse = new MatChannelContainer { Color = ColorUint.Tofloat3(ColorUint.Green) },
                                        Specular = new SpecularChannelContainer {Color = ColorUint.Tofloat3(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                                    },
                                    CreateCuboid(new float3(20, 100, 20))
                                },
                                Children = new List<SceneNodeContainer>
                                {
                                    new SceneNodeContainer
                                    {
                                        Name = "Arm02Rot",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new TransformComponent {Translation=new float3(-20, 40, 0),  Rotation = new float3(0.35f, 0, 0), Scale = float3.One},
                                        },
                                        Children = new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Arm02",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new TransformComponent {Translation=new float3(0, 40, 0),  Scale = float3.One },
                                                    new MaterialComponent
                                                    {
                                                        Diffuse = new MatChannelContainer { Color = ColorUint.Tofloat3(ColorUint.Yellow) },
                                                        Specular = new SpecularChannelContainer {Color =ColorUint.Tofloat3(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                                                    },
                                                    CreateCuboid(new float3(20, 100, 20))
                                                },
                                                Children = new List<SceneNodeContainer>
                                                {
                                                    new SceneNodeContainer
                                                    {
                                                        Name = "Arm03Rot",
                                                        Components = new List<SceneComponentContainer>
                                                        {
                                                            new TransformComponent {Translation=new float3(20, 40, 0),  Rotation = new float3(0.25f, 0, 0), Scale = float3.One},
                                                        },
                                                        Children = new List<SceneNodeContainer>
                                                        {
                                                            new SceneNodeContainer
                                                            {
                                                                Name = "Arm03",
                                                                Components = new List<SceneComponentContainer>
                                                                {
                                                                    new TransformComponent {Translation=new float3(0, 40, 0),  Scale = float3.One },
                                                                    new MaterialComponent
                                                                    {
                                                                        Diffuse = new MatChannelContainer { Color = ColorUint.Tofloat3(ColorUint.Blue) },
                                                                        Specular = new SpecularChannelContainer {Color = ColorUint.Tofloat3(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                                                                    },
                                                                    CreateCuboid(new float3(20, 100, 20))
                                                                }
                                                            },
                                                        }
                                                    }
                                                }
                                            },
                                        }
                                    }
                                }
                            },
                        }
                    },
                }
            });
        }


        public static Mesh CreateCuboid(float3 size)
        {
            return new Mesh
            {
                Vertices = new[]
                {
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z},
                    new float3 {x = +0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z},
                    new float3 {x = -0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z}
                },

                Triangles = new ushort[]
                {
                    // front face
                    0, 2, 1, 0, 3, 2,

                    // right face
                    4, 6, 5, 4, 7, 6,

                    // back face
                    8, 10, 9, 8, 11, 10,

                    // left face
                    12, 14, 13, 12, 15, 14,

                    // top face
                    16, 18, 17, 16, 19, 18,

                    // bottom face
                    20, 22, 21, 20, 23, 22

                },

                Normals = new[]
                {
                    new float3(0, 0, 1),
                    new float3(0, 0, 1),
                    new float3(0, 0, 1),
                    new float3(0, 0, 1),
                    new float3(1, 0, 0),
                    new float3(1, 0, 0),
                    new float3(1, 0, 0),
                    new float3(1, 0, 0),
                    new float3(0, 0, -1),
                    new float3(0, 0, -1),
                    new float3(0, 0, -1),
                    new float3(0, 0, -1),
                    new float3(-1, 0, 0),
                    new float3(-1, 0, 0),
                    new float3(-1, 0, 0),
                    new float3(-1, 0, 0),
                    new float3(0, 1, 0),
                    new float3(0, 1, 0),
                    new float3(0, 1, 0),
                    new float3(0, 1, 0),
                    new float3(0, -1, 0),
                    new float3(0, -1, 0),
                    new float3(0, -1, 0),
                    new float3(0, -1, 0)
                },

                UVs = new[]
                {
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0),
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0),
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0),
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0),
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0),
                    new float2(1, 0),
                    new float2(1, 1),
                    new float2(0, 1),
                    new float2(0, 0)
                },
                BoundingBox = new AABBf(-0.5f * size, 0.5f * size)
            };
        }
    }
}