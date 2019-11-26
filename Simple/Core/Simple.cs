using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Fusee.Engine.GUI;

namespace Fusee.Examples.Simple.Core
{
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class Simple : RenderCanvas
    {
        // angle variables
        private static float _angleVert = 0.5f * M.Pi, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        //my var
        private float2 cornerbox;
        private float2 wallZbox;
        private float2 wallXbox;
        private float2 groundbox;
        private float2[,] translation = new float2[31, 31];
        private int[] ballbmp;

        private float4x4 mtxCam;
        private float deg;
        private TransformComponent _ball;
        private SceneContainer _scene;
        private float _moveX, _moveZ;
        private bool keykeymoveX = false, keymoveZ = false;
        private TransformComponent mazeTransform = new TransformComponent();
        private const float _speed = 7;
        private TransformComponent[,] wallsTransform;
        private int[,] bmp = new int[,] {{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1},
                                        {1,0,1,1,1,1,1,1,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1},
                                        {1,0,1,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,0,0,1,0,0,0,0,0,1,0,1,0,1},
                                        {1,0,1,1,1,1,1,0,1,1,1,0,1,0,1,0,1,1,1,0,1,0,1,1,1,0,1,0,1,1,1},
                                        {1,0,0,0,0,0,1,0,1,0,0,0,1,0,1,0,0,0,0,0,1,0,1,0,1,0,0,0,0,0,2},
                                        {1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1},
                                        {1,0,0,0,1,0,1,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,1,0,1,0,0,0,1,0,1},
                                        {1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1},
                                        {1,0,1,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1},
                                        {1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1},
                                        {1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,1,0,0,0,1,0,1},
                                        {1,0,1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1},
                                        {1,0,1,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,1,0,1},
                                        {1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,1,1},
                                        {1,0,0,0,1,0,1,0,1,0,1,0,0,0,1,0,0,0,1,0,1,0,1,0,0,0,0,0,0,0,1},
                                        {1,1,1,1,1,0,1,1,1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,0,1,1,1,1,1,0,1},
                                        {1,0,0,0,1,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,1},
                                        {1,0,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1},
                                        {1,0,1,0,1,0,1,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,1,0,1,0,1},
                                        {1,0,1,0,1,1,1,0,1,0,1,1,1,0,1,1,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1},
                                        {1,0,1,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1},
                                        {1,0,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1},
                                        {2,-1,0,0,0,0,0,0,1,0,1,0,1,0,1,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,1},
                                        {1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,0,1,1,1,0,1,1,1},
                                        {1,0,1,0,0,0,1,0,1,0,1,0,1,0,1,0,1,0,0,0,0,0,1,0,1,0,1,0,1,0,1},
                                        {1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1},
                                        {1,0,1,0,1,0,1,0,0,0,0,0,0,0,1,0,1,0,1,0,1,0,1,0,0,0,1,0,1,0,1},
                                        {1,0,1,1,1,0,1,1,1,1,1,1,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1},
                                        {1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1},
                                        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}};



        //other var
        private SceneRendererForward _sceneRenderer;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;

        SceneContainer CreateScene()
        {
            SceneContainer mazeScene = AssetStorage.Get<SceneContainer>("mazeAsset.fus");
            SceneNodeContainer cornerstone = mazeScene.Children.FindNodes(n => n.Name == "Cornerstone").First();
            SceneNodeContainer wallX = mazeScene.Children.FindNodes(n => n.Name == "WallX").First();
            SceneNodeContainer wallZ = mazeScene.Children.FindNodes(n => n.Name == "WallZ").First();
            SceneNodeContainer ball = mazeScene.Children.FindNodes(n => n.Name == "Ball").First();
            SceneNodeContainer ground = mazeScene.Children.FindNodes(n => n.Name == "Ground").First();
            SceneNodeContainer maze = new SceneNodeContainer
            {
                Components = new List<SceneComponentContainer>
                {
                    // TRANSFROM COMPONENT
                    mazeTransform,

                    // SHADER EFFECT COMPONENT
                    cornerstone.GetComponent<ShaderEffectComponent>(),
                },
                Children = new ChildList()
            };
            maze.Children.Add(new SceneNodeContainer
            {
                Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Translation = new float3(0 , -0.5f, 0)
                                    },
                                    ground.GetComponent<ShaderEffectComponent>(),
                                    ground.GetComponent<Mesh>()
                                },
                Name = "Ground"
            }
);
            for (int countY = 0; countY < bmp.GetLength(1); countY++)
            {
                for (int countX = 0; countX < bmp.GetLength(0); countX++)
                {
                    if (countX % 2 == 0 && countY % 2 == 0 && bmp[countX, countY] == 1)
                    {
                        maze.Children.Add(new SceneNodeContainer
                        {
                            Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Translation = new float3(countX * (wallXbox.x + cornerbox.x)/2, 2, countY * (wallZbox.y + cornerbox.y)/2)
                                    },
                                    cornerstone.GetComponent<ShaderEffectComponent>(),
                                    cornerstone.GetComponent<Mesh>()
                                },
                            Name = "Cornerstone"
                        }
                        );
                    }
                    if (countX % 2 == 0 && countY % 2 == 1 && bmp[countX, countY] == 1)
                    {
                        maze.Children.Add(new SceneNodeContainer
                        {
                            Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Translation = new float3(countX * (wallXbox.x + cornerbox.x)/2, 2, countY * (wallZbox.y + cornerbox.y)/2)
                                    },
                                    wallZ.GetComponent<ShaderEffectComponent>(),
                                    wallZ.GetComponent<Mesh>()
                                },
                            Name = "WallZ"

                        }
                        );
                    }
                    if (countX % 2 == 1 && countY % 2 == 0 && bmp[countX, countY] == 1)
                    {
                        maze.Children.Add(new SceneNodeContainer
                        {
                            Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Translation = new float3(countX * (wallXbox.x + cornerbox.x)/2, 2, countY * (wallZbox.y + cornerbox.y)/2)
                                    },
                                    wallX.GetComponent<ShaderEffectComponent>(),
                                    wallX.GetComponent<Mesh>()
                                },
                            Name = "WallX"
                        }
                        );
                    }
                    if (countX % 2 == 1 && countY % 2 == 1 && bmp[countX, countY] == -1)
                    {
                        maze.Children.Add(new SceneNodeContainer
                        {
                            Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Translation = new float3(countX * (wallXbox.x + cornerbox.x)/2, 2, countY * (wallZbox.y + cornerbox.y)/2)
                                    },
                                    ball.GetComponent<ShaderEffectComponent>(),
                                    ball.GetComponent<Mesh>()
                                },
                            Name = "Ball"
                        }
                        );
                    }
                }
            }
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    mazeScene.Children.First(),
                    maze
                }
            };
        }



        // Init is called on startup. 
        public override void Init()
        {
            findball();
            //create BoundingBox
            makebox();
            translation = new float2[bmp.GetLength(0), bmp.GetLength(1)];


            _gui = CreateGui();
            Resize(new ResizeEventArgs(Width, Height));
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            wallsTransform = new TransformComponent[bmp.GetLength(0), bmp.GetLength(1)];
            // Load the rocket model
            _scene = CreateScene();
            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _guiRenderer = new SceneRendererForward(_gui);

            //my Init
            _ball = _scene.Children.FindNodes(node => node.Name == "Ball")?.FirstOrDefault()?.GetTransform();
            CreateTranslation();

        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Mouse and keyboard movement

            if (Mouse.LeftButton)
            {
                _angleVelVert = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelVert = -RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
            }
            else
            {
                _angleVelVert = 0;
            }

            _angleVert = (_angleVert + _angleVelVert) % (2 * M.Pi);

            collision();
            ballmovement();


            //Set the view matrix for the interaction handler.
            _sih.View = RC.View;

            // Constantly check for interactive objects.
            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

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

            var canvasProjComp = new ProjectionComponent(ProjectionMethod.ORTHOGRAPHIC, ZNear, ZFar, _fovy);
            canvas.Components.Insert(0, canvasProjComp);

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

        //my methods
        public void ballmovement()
        {
            mtxCam = float4x4.LookAt(_ball.Translation.x - 10 * M.Cos(_angleVert), _ball.Translation.y + 10, _ball.Translation.z - 10 * M.Sin(_angleVert), _ball.Translation.x, _ball.Translation.y, _ball.Translation.z, 0, 1, 0);
            RC.View = mtxCam;

            //move the ball
            deg = 0.0f;
            if (_angleVert % (0.5f * M.Pi) <= 0.25f * M.Pi)
            {
                deg = -(_angleVert % (0.5f * M.Pi));
            }
            else
            {
                deg = (0.5f * M.Pi) - (_angleVert % (0.5f * M.Pi));
            }

            if(!(Keyboard.ADAxis != 0 && Keyboard.WSAxis != 0))
            {
                if (Keyboard.ADAxis != 0 && collision())
                {
                    _moveX = _speed * Keyboard.ADAxis * DeltaTime;
                    _ball.Translation.x += _moveX * M.Sin(_angleVert + deg);
                    _ball.Translation.z -= _moveX * M.Cos(_angleVert + deg);

                    _ball.RotateAround(new float3(_ball.Translation.x, _ball.Translation.y, _ball.Translation.z), new float3(-_moveX * M.Cos(_angleVert + deg), 0, -_moveX * M.Sin(_angleVert + deg)));


                }
                if (Keyboard.WSAxis != 0)
                {
                    _moveZ = _speed * Keyboard.WSAxis * DeltaTime;
                    _ball.Translation.x += _moveZ * M.Cos(_angleVert + deg);
                    _ball.Translation.z += _moveZ * M.Sin(_angleVert + deg);

                    _ball.RotateAround(new float3(_ball.Translation.x, _ball.Translation.y, _ball.Translation.z), new float3(_moveZ * M.Sin(_angleVert + deg), 0, -_moveZ * M.Cos(_angleVert + deg)));
                }
            }
            

        }
        public bool collision()
        {
            if (!(_ball.Translation.x <= translation[ballbmp[0], ballbmp[1]].x + wallZbox.x / 2))
            {
                if (bmp[ballbmp[0], ballbmp[1]] != 1)
                {
                    bmp[ballbmp[0], ballbmp[1]] = 0;
                    bmp[ballbmp[0] -1, ballbmp[1]] = -1;
                    ballbmp[0] = ballbmp[0] - 1;
                    return false;
                }
                return false;
            }
                return true;
        }
        public void makebox()
        {
            SceneContainer mazeScene = AssetStorage.Get<SceneContainer>("mazeAsset.fus");
            var cornerstone = mazeScene.Children.FindNodes(node => node.Name == "Cornerstone")?.FirstOrDefault()?.GetMesh();
            cornerbox = cornerstone.BoundingBox.Size.xz;
            var wallZ = mazeScene.Children.FindNodes(node => node.Name == "WallZ")?.FirstOrDefault()?.GetMesh();
            wallZbox = wallZ.BoundingBox.Size.xz;
            var wallX = mazeScene.Children.FindNodes(node => node.Name == "WallX")?.FirstOrDefault()?.GetMesh();
            wallXbox = wallX.BoundingBox.Size.xz;

            groundbox = new float2(wallXbox.x, wallZbox.y);
        }

        public void CreateTranslation()
        {
            for (int countY = 0; countY < bmp.GetLength(1); countY++)
            {
                for (int countX = 0; countX < bmp.GetLength(0); countX++)
                {
                    if (countX % 2 == 0 && countY % 2 == 0)
                    {
                        translation[countX, countY] = new float2(countX * (wallXbox.x + cornerbox.x) / 2, countY * (wallZbox.y + cornerbox.y) / 2);
                    }
                    if (countX % 2 == 0 && countY % 2 == 1)
                    {
                        translation[countX, countY] = new float2(countX * (wallXbox.x + cornerbox.x) / 2, countY * (wallZbox.y + cornerbox.y) / 2);
                    }
                    if (countX % 2 == 1 && countY % 2 == 0)
                    {
                        translation[countX, countY] = new float2(countX * (wallXbox.x + cornerbox.x) / 2, countY * (wallZbox.y + cornerbox.y) / 2);
                    }
                    if (countX % 2 == 1 && countY % 2 == 1)
                    {
                        translation[countX, countY] = new float2(countX * (wallXbox.x + cornerbox.x) / 2, countY * (wallZbox.y + cornerbox.y) / 2);
                    }
                }
            }
        }
        public void findball()
        {
            for (int countY = 0; countY < bmp.GetLength(1); countY++)
            {
                for (int countX = 0; countX < bmp.GetLength(0); countX++)
                {
                    if (bmp[countX, countY] == -1)
                    {
                        ballbmp = new int[] { countX, countY };
                    }
                }
            }
        }
    }
}