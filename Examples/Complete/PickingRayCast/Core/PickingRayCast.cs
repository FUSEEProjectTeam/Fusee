using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fusee.Examples.PickingRayCast.Core
{
    [FuseeApplication(Name = "FUSEE Picking Example RayCast", Description = "How to use the Scene RayCaster.")]
    public class PickingRayCast : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver4, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private SceneRayCaster _sceneRayCaster;

        private bool _keys;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private readonly float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.Screen;
        private float _initCanvasWidth;
        private float _initCanvasHeight;
        private float _canvasWidth = 16;
        private float _canvasHeight = 9;

        private RayCastResult _currentPick;
        private float4 _oldColor;
        private bool _pick;
        private float2 _pickPos;

        private Camera _cam = new Engine.Core.Scene.Camera(ProjectionMethod.Perspective, 1, 1000, M.PiOver4);
        private readonly Camera _guiCam = new Fusee.Engine.Core.Scene.Camera(ProjectionMethod.Orthographic, 1, 1000, M.PiOver4);
        private Transform _camTransform;
        private Transform _guiCamTransform;

        // Init is called on startup.
        public override void Init()
        {
            _initCanvasWidth = Width / 100f;
            _initCanvasHeight = Height / 100f;

            _canvasHeight = _initCanvasHeight;
            _canvasWidth = _initCanvasWidth;

            _cam.Viewport = new float4(0, 0, 100, 100);
            _cam.BackgroundColor = new float4(1f, 1f, 1f, 1);
            _cam.Layer = -1;

            _guiCam.ClearColor = false;
            _guiCam.ClearDepth = false;
            _guiCam.FrustumCullingOn = false;

            _camTransform = _guiCamTransform = new Transform()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 20, 20),
                Scale = new float3(1, 1, 1)
            };

            var rotation = float4x4.LookAt(_camTransform.Translation, new float3(0, 0, 0), float3.UnitY);
            _camTransform.Rotate(rotation);

            SceneNode cam = new SceneNode()
            {
                Name = "Cam",
                Components = new List<SceneComponent>()
                {
                    _camTransform,
                    _cam,
                }
            };

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Create the robot model
            _scene = CreateScene();

            _scene.Children.Add(cam);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _sceneRayCaster = new SceneRayCaster(_scene);

            _gui = CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);
            _guiRenderer = new SceneRendererForward(_gui);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            if (Input.Mouse.LeftButton)
            {
                _pick = true;
                _pickPos = Input.Mouse.Position;
            }

            if (Input.Mouse.RightButton)
            {
                _angleVelHorz = RotationSpeed * Input.Mouse.XVel * Time.DeltaTime * 0.0005f;
                _angleVelVert = RotationSpeed * Input.Mouse.YVel * Time.DeltaTime * 0.0005f;
            }

            if (Input.Keyboard.IsKeyUp(KeyCodes.Space))
            {
                _pick = true;
                _pickPos = new float2(Width / 2, Height / 2);
            }

            _camTransform.RotateAround(float3.Zero, new float3(0, _angleVelHorz, 0));


            if (_pick)
            {
                float2 pickPosClip = (_pickPos * new float2(2.0f / Width, -2.0f / Height)) + new float2(-1, 1);
                var ray_eye = float4x4.Transform(RC.InvProjection, new float4(pickPosClip.x, pickPosClip.y, 0, 1));
                ray_eye.w = 0;
                var ray_cam = float4x4.Transform(RC.InvView, ray_eye).xyz;
                ray_cam.Normalize();

                float3 origin = _camTransform.Translation;
                float3 direction = float4x4.Transform(_camTransform.Matrix().RotationComponent(), ray_cam);

                Rayf ray = new Rayf(origin, direction);

                var hits = _sceneRayCaster.RayCast(ray);
                foreach (var hit in hits)
                {
                    Console.WriteLine("Hit: " + hit.Node.Name);
                }


                var cube = new SceneNode()
                {
                    Name = "Cube",
                    Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Rotation = float3.Zero,
                                Translation = origin + (direction * 10),
                                Scale = new float3(.5f, .5f, .5f)
                            },
                            MakeEffect.FromDiffuseSpecular((float4)ColorUint.Blue, float4.Zero, 4.0f, 1f),
                            new Engine.Core.Primitives.Cube()
                        }
                };
                _scene.Children.Add(cube);

                var cube2 = new SceneNode()
                {
                    Name = "Cube",
                    Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Rotation = float3.Zero,
                                Translation = origin + (direction * 20),
                                Scale = new float3(.5f, .5f, .5f)
                            },
                            MakeEffect.FromDiffuseSpecular((float4)ColorUint.Red, float4.Zero, 4.0f, 1f),
                            new Engine.Core.Primitives.Cube()
                        }
                };
                _scene.Children.Add(cube2);

                var cube3 = new SceneNode()
                {
                    Name = "Cube",
                    Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Rotation = float3.Zero,
                                Translation = origin + (direction * 30),
                                Scale = new float3(.5f, .5f, .5f)
                            },
                            MakeEffect.FromDiffuseSpecular((float4)ColorUint.Yellow, float4.Zero, 4.0f, 1f),
                            new Engine.Core.Primitives.Cube()
                        }
                };
                _scene.Children.Add(cube3);


                _pick = false;
            }

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
            //_guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private InputDevice Creator(IInputDeviceImp device)
        {
            throw new NotImplementedException();
        }

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
                "FUSEE Picking Example using Raycasting",
                "ButtonText",
                vsTex,
                psText,
                UIElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                UIElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                (float4)ColorUint.Greenery,
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

            SceneNode cam = new SceneNode()
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
                    //Add canvas.
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

        private SceneContainer CreateScene()
        {
            var scene = new SceneContainer
            {
                Header = new SceneHeader
                {
                    CreationDate = "July 2021",
                    CreatedBy = "Jonas Haller",
                    Generator = "Handcoded with pride :)",
                },
                Children = new List<SceneNode> { }
            };

            var rand = new Random();

            for (int i = 0; i < 20; i++)
            {
                var x = rand.Next(-10, 10);
                var y = rand.Next(-10, 10);
                var z = rand.Next(-10, 10);

                var cube = new SceneNode()
                {
                    Name = "Cube" + i,
                    Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Rotation = float3.Zero,
                                Translation = new float3(x, y, z),
                                Scale = new float3(1f, 1f, 1f)
                            },
                            MakeEffect.FromDiffuseSpecular((float4)ColorUint.Gray, float4.Zero, 4.0f, 1f),
                            new Engine.Core.Primitives.Cube()
                        }
                };
                scene.Children.Add(cube);
            }

            return scene;
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