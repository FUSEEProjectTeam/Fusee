using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System.Collections.Generic;
using System.Linq;
using BitmapCS;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.Simple.Core
{
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class Simple : RenderCanvas
    {


        //my var

        // angle variables
        private static float _angleVert, _angleVelVert, angle;

        //mouse rotation speed
        private const float RotationSpeed = 7;

        //speed from character
        private float speed = 7;

        //var for collision-detection
        private float2 cornerbox;
        private float2 wallZbox;
        private float2 wallXbox;
        private float2 groundbox;
        private float4[,] translation;
        private int[] ballbmp;
        private float length;
        private float height;
        private float ballradius;
        private float oldX;
        private float oldY;
        private int[,] bmp = Program.Bmp();

        //cam pos for changing
        private float3 cam = new float3(10, 5, 10);
        private int cases = 0;

        //movment
        private float velocityAD = 0;
        private float velocityWS = 0;
        private float newtime = 0;
        private float oldtime = 0;
        private float _moveX, _moveZ;

        //timer display
        private GUIText timertext;
        private FontMap _timeMap;

        //winning display
        private GUIText winText;
        private FontMap _winMap;

        //font
        private Font fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");


        //call winningdisplay method only once
        private bool readonce = true;


        //stops ball when winning
        private bool movement = true;
        private bool win = false;


        //camera
        private float4x4 mtxCam;
        private float camAngle = 0;

        //TransformComponent and SceneContainer
        private TransformComponent _head;
        private TransformComponent _body;
        private TransformComponent _bodytrans;
        private SceneContainer _scene;
        private TransformComponent mazeTransform = new TransformComponent();
        private TransformComponent[,] wallsPositions;

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
            //load the Nodes in 
            SceneContainer mazeScene = AssetStorage.Get<SceneContainer>("mazeAsset.fus");
            SceneNodeContainer cornerstone = mazeScene.Children.FindNodes(n => n.Name == "Cornerstone").First();
            SceneNodeContainer wallX = mazeScene.Children.FindNodes(n => n.Name == "WallX").First();
            SceneNodeContainer wallZ = mazeScene.Children.FindNodes(n => n.Name == "WallZ").First();
            SceneNodeContainer ball = mazeScene.Children.FindNodes(n => n.Name == "Body").First();
            SceneNodeContainer head = mazeScene.Children.FindNodes(n => n.Name == "Head").First();
            Cube _ground = new Cube();

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

            for (int countY = 0; countY < bmp.GetLength(1); countY++)
            {
                for (int countX = 0; countX < bmp.GetLength(0); countX++)
                {
                    if (countX % 2 == 0 && countY % 2 == 0 && bmp[countY, countX] == 1)
                    {
                        maze.Children.Add(new SceneNodeContainer
                        {
                            Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Translation = new float3(countX * (wallXbox.x + cornerbox.x)/2, 2.2f, countY * (wallZbox.y + cornerbox.y)/2)
                                    },
                                    cornerstone.GetComponent<ShaderEffectComponent>(),
                                    cornerstone.GetComponent<Mesh>()
                                },
                            Name = "Wall" + countY.ToString().PadLeft(2, '0') + countX.ToString().PadLeft(2, '0')
                        }
                        );
                    }

                    if (countX % 2 == 1 && countY % 2 == 0 && bmp[countY, countX] == 1)
                    {
                        maze.Children.Add(new SceneNodeContainer
                        {
                            Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Translation = new float3(countX * (wallXbox.x + cornerbox.x)/2, 2.2f, countY * (wallZbox.y + cornerbox.y)/2)
                                    },
                                    wallX.GetComponent<ShaderEffectComponent>(),
                                    wallX.GetComponent<Mesh>()
                                },
                            Name = "Wall" + countY.ToString().PadLeft(2, '0') + countX.ToString().PadLeft(2, '0')
                        }
                        );
                    }

                    if (countX % 2 == 0 && countY % 2 == 1 && bmp[countY, countX] == 1)
                    {
                        maze.Children.Add(new SceneNodeContainer
                        {
                            Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Translation = new float3(countX * (wallXbox.x + cornerbox.x)/2, 2.2f, countY * (wallZbox.y + cornerbox.y)/2)
                                    },
                                    wallZ.GetComponent<ShaderEffectComponent>(),
                                    wallZ.GetComponent<Mesh>()
                                },
                            Name = "Wall" + countY.ToString().PadLeft(2, '0') + countX.ToString().PadLeft(2, '0')
                        }
                        );
                    }

                    if (countX % 2 == 1 && countY % 2 == 1 && bmp[countY, countX] == -1)
                    {
                        maze.Children.Add(new SceneNodeContainer
                        {
                            Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Translation = new float3(countX * (wallXbox.x + cornerbox.x)/2, ballradius, countY * (wallZbox.y + cornerbox.y)/2),
                                    },
                                    head.GetComponent<ShaderEffectComponent>(),
                                    head.GetComponent<Mesh>()
                                },
                            Name = "Head",

                            Children = new ChildList
                            {
                                new SceneNodeContainer
                                {
                                    Components = new List<SceneComponentContainer>
                                    {
                                    new TransformComponent
                                        {
                                            Translation = new float3(0,0,0)
                                        },
                                    },
                                Name = "Bodytrans",

                                Children = new ChildList
                                {
                                    new SceneNodeContainer
                                    {
                                        Components = new List<SceneComponentContainer>
                                        {
                                        new TransformComponent
                                            {
                                                Translation = new float3(0,0,0)
                                            },
                                            ball.GetComponent<ShaderEffectComponent>(),
                                            ball.GetComponent<Mesh>()
                                        },
                                    Name = "Body",
                                    }
                                },
                                }
                            },
                        }
                        );
                    }
                }
            }

            maze.Children.Add(new SceneNodeContainer
            {
                Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Scale = new float3(length, 1, height),
                                        Translation = new float3(length/2 - cornerbox.x/2, -0.5f, height/2 - cornerbox.y/2)
                                    },
                                    new ShaderEffectComponent
                                    {
                                        Effect = ShaderCodeBuilder.MakeShaderEffectProto(new float4(0.545f, 0.270f, 0.074f, 1), new float4(0, 0, 0, 1), 136.75444f, 0.483772248f)
                                    },
                                    _ground
                                },
                Name = "Ground"
            }
            );

            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    maze
                }
            };
        }

        // Init is called on startup. 
        async public override Task<bool> Init()
        {
            _timeMap = new FontMap(fontLato, 24);
            _gui = CreateGui();
            Resize(new ResizeEventArgs(Width, Height));
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            //find the ball and create AABB
            findball();
            makebox();

            //get length and height for the ground
            translation = new float4[bmp.GetLength(0), bmp.GetLength(1)];

            CreatePositions();

            //create length and height for the ground
            length = translation[translation.GetLength(0) - 1, 0].w + cornerbox.y / 2;
            height = translation[0, translation.GetLength(1) - 1].z + cornerbox.x / 2;



            //create 2D-Array for walls positions
            wallsPositions = new TransformComponent[bmp.GetLength(0), bmp.GetLength(1)];

            // Load the rocket model
            _scene = CreateScene();
            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _guiRenderer = new SceneRendererForward(_gui);

            //get Nodes from _scene
            _head = _scene.Children.FindNodes(node => node.Name == "Head")?.FirstOrDefault()?.GetTransform();
            _body = _scene.Children.FindNodes(node => node.Name == "Body")?.FirstOrDefault()?.GetTransform();
            _bodytrans = _scene.Children.FindNodes(node => node.Name == "Bodytrans")?.FirstOrDefault()?.GetTransform();

            return true;
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Mouse movement
            if (Mouse.LeftButton)
            {
                _angleVelVert = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
            }

            else
            {
                _angleVelVert = 0;
            }

            _angleVert = (_angleVert + _angleVelVert) % (2 * M.Pi);

            if (Mouse.IsButtonDown(2))
            {
                switch (cases)
                {
                    case 0:
                        cam = new float3(1, 0, 1);
                        cases++;
                        break;
                    case 1:
                        cam = new float3(10, 5, 10);
                        cases %= 1;
                        break;
                };
            }

            ballmovement();
            collision();
            winner();

            var perspective = float4x4.CreatePerspectiveFieldOfView(_fovy, (float)Width / Height, ZNear, ZFar);
            var orthographic = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);

            // Render the scene loaded in Init()
            RC.Projection = perspective;
            _sceneRenderer.Render(RC);

            //Constantly check for interactive objects.

            RC.Projection = orthographic;
            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

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

            var guiLatoBlack = new FontMap(fontLato, 18);

            var text = new TextNodeContainer(
                "FUSEE Simple Example",
                "ButtonText",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_HORIZONTAL),
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_HORIZONTAL, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery),
                HorizontalTextAlignment.CENTER,
                VerticalTextAlignment.CENTER);

            //create stopwatch
            var timer = new TextNodeContainer(
                "00:00.00",
                "Timer",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_RIGHT),
                new MinMaxRect
                {
                    Min = new float2(-2, 0),
                    Max = new float2(-0.3f, -1)
                },
                 _timeMap,
                ColorUint.Tofloat4(ColorUint.Greenery),
                HorizontalTextAlignment.RIGHT,
                VerticalTextAlignment.CENTER
            );

            timertext = timer.GetComponentsInChildren<GUIText>().FirstOrDefault();

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
                    text,
                    timer
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

        //my methods
        public void ballmovement()
        {
            if (movement || !win)
            {
                //set time for stopwatch 
                int minutes = (int)Time.TimeSinceStart / 60;
                var seconds = Time.TimeSinceStart % 59.5f;
                var miliseconds = Time.TimeSinceStart % 0.99f;
                timertext.Text = minutes.ToString("00") + ":" + seconds.ToString("00") + miliseconds.ToString(".00", new System.Globalization.CultureInfo("en-US"));

                //camera
                angle = _angleVert;
                if (Keyboard.GetKey(KeyCodes.E))
                {
                    mtxCam = float4x4.LookAt(length / 2, 100, height / 2, length / 2, 0, height / 2, 1, 0, 0);
                    movement = false;
                }

                else
                {
                    mtxCam = float4x4.LookAt(_head.Translation.x - cam.x * M.Cos(_angleVert), _head.Translation.y + cam.y, _head.Translation.z - cam.z * M.Sin(_angleVert), _head.Translation.x, _head.Translation.y, _head.Translation.z, 0, 1, 0);
                    _head.Rotation = new float3(_head.Rotation.x, -angle - 90 * M.Pi / 180, _head.Rotation.z);
                    _bodytrans.Rotation = new float3(0, angle, 0);
                    movement = true;

                }
                RC.View = mtxCam;

                //get old positions for the head
                oldX = _head.Translation.x;
                oldY = _head.Translation.z;

                //move the ball
                _moveX = Keyboard.ADAxis * speed * DeltaTime;
                if (_moveX < 0)
                {
                    _head.Translation.x += _moveX * M.Sin(angle);
                    _head.Translation.z -= _moveX * M.Cos(angle);

                    _body.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(angle), 0, M.Cos(angle)), _moveX)), 0);

                }

                else if (_moveX > 0)
                {
                    _head.Translation.x += _moveX * M.Sin(angle);
                    _head.Translation.z -= _moveX * M.Cos(angle);
                    _body.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(angle), 0, M.Cos(angle)), _moveX)), 0);
                }



                _moveZ = Keyboard.WSAxis * speed * DeltaTime;
                if (_moveZ < 0)
                {
                    _head.Translation.x += _moveZ * M.Cos(angle);
                    _head.Translation.z += _moveZ * M.Sin(angle);
                    _body.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(angle), 0, M.Sin(angle)), -_moveZ)), 0);
                }

                else if (_moveZ > 0)
                {
                    _head.Translation.x += _moveZ * M.Cos(angle);
                    _head.Translation.z += _moveZ * M.Sin(angle);
                    _body.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(angle), 0, M.Sin(angle)), -_moveZ)), 0);
                }

                if (Keyboard.GetKey(KeyCodes.A) || Keyboard.GetKey(KeyCodes.D))
                {
                    if (_head.Rotation.z > -30 * M.Pi / 180 && _moveX < 0)
                    {
                        _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(angle), 0, M.Sin(angle)), -_moveX)), 0);
                    }
                    if (_head.Rotation.z < 30 * M.Pi / 180 && _moveX > 0)
                    {
                        _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(angle), 0, M.Sin(angle)), -_moveX)), 0);
                    }
                }
                else if (Keyboard.GetKey(KeyCodes.W) || Keyboard.GetKey(KeyCodes.S))
                {

                    if (_head.Rotation.x > -30 * M.Pi / 180 && _moveZ > 0)
                    {
                        _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(angle), 0, M.Cos(angle)), -_moveZ)), 0);
                    }
                    if (_head.Rotation.x < 30 * M.Pi / 180 && _moveZ < 0)
                    {
                        _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(angle), 0, M.Cos(angle)), -_moveZ)), 0);
                    }
                }
                if (_head.Translation.x == oldX)
                {
                    if (_head.Rotation.x < -0.00001f)
                    {
                        if (_head.Rotation.x + 0.1f < -0.1f)
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(angle), 0, M.Cos(angle)), 0.1f)), 0);
                        }
                        else
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(angle), 0, M.Cos(angle)), -_head.Rotation.x)), 0);
                        }
                    }

                    if (_head.Rotation.x > 0.00001f)
                    {
                        if (_head.Rotation.x - 0.1f > 0.1f)
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(angle), 0, M.Cos(angle)), -0.1f)), 0);
                        }
                        else
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(angle), 0, M.Cos(angle)), -_head.Rotation.x)), 0);
                        }
                    }
                }
                if (_head.Translation.z == oldY)
                {
                    if (_head.Rotation.z < -0.00001f)
                    {
                        if (_head.Rotation.z + 0.1f < -0.1f)
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(angle), 0, M.Sin(angle)), -0.1f)), 0);
                        }
                        else
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(angle), 0, M.Sin(angle)), _head.Rotation.z)), 0);
                        }
                    }

                    if (_head.Rotation.z > 0.00001f)
                    {
                        if (_head.Rotation.z - 0.1f > 0.1f)
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(angle), 0, M.Sin(angle)), 0.1f)), 0);
                        }
                        else
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(angle), 0, M.Sin(angle)), _head.Rotation.z)), 0);
                        }
                    }
                }
            }
        }


        public void collision()
        {
            //change the ballbmp when the body moves around
            if (translation[ballbmp[0], ballbmp[1]].x <= _head.Translation.x)
            {
                if (translation[ballbmp[0], ballbmp[1]].z >= _head.Translation.x)
                {
                    if (translation[ballbmp[0], ballbmp[1]].y <= _head.Translation.z)
                    {
                        if (translation[ballbmp[0], ballbmp[1]].w >= _head.Translation.z)
                        { }

                        else
                        {
                            ballbmp[0] = ballbmp[0] + 1;
                        }
                    }

                    else
                    {
                        ballbmp[0] = ballbmp[0] - 1;
                    }
                }
                else
                {
                    ballbmp[1] = ballbmp[1] + 1;
                }
            }
            else
            {
                ballbmp[1] = ballbmp[1] - 1;
            }

            //Walls
            if (bmp[ballbmp[0] - 1, ballbmp[1]] == 1 || bmp[ballbmp[0] - 1, ballbmp[1]] == 2)
            {
                if (_head.Translation.z - translation[ballbmp[0] - 1, ballbmp[1]].w < ballradius)
                {
                    _head.Translation.z = translation[ballbmp[0] - 1, ballbmp[1]].w + ballradius + 0.0001f;
                    velocityAD = 0;
                    velocityWS = 0;
                }
            }

            if (bmp[ballbmp[0] + 1, ballbmp[1]] == 1 || bmp[ballbmp[0] + 1, ballbmp[1]] == 2)
            {
                if (translation[ballbmp[0] + 1, ballbmp[1]].y - _head.Translation.z < ballradius)
                {
                    _head.Translation.z = translation[ballbmp[0] + 1, ballbmp[1]].y - ballradius - 0.0001f;
                    velocityAD = 0;
                    velocityWS = 0;
                }
            }

            if (bmp[ballbmp[0], ballbmp[1] - 1] == 1 || bmp[ballbmp[0], ballbmp[1] - 1] == 2)
            {
                if (_head.Translation.x - translation[ballbmp[0], ballbmp[1] - 1].z < ballradius)
                {
                    _head.Translation.x = translation[ballbmp[0], ballbmp[1] - 1].z + ballradius + 0.0001f;
                    velocityAD = 0;
                    velocityWS = 0;
                }
            }

            if (bmp[ballbmp[0], ballbmp[1] + 1] == 1 || bmp[ballbmp[0], ballbmp[1] + 1] == 2)
            {

                if (translation[ballbmp[0], ballbmp[1] + 1].x - _head.Translation.x < ballradius)
                {
                    _head.Translation.x = translation[ballbmp[0], ballbmp[1] + 1].x - ballradius - 0.0001f;
                    velocityAD = 0;
                    velocityWS = 0;
                }
            }
            //Corners

            if (bmp[ballbmp[0] - 1, ballbmp[1] - 1] == 1)
            {
                if (M.Sqrt((_head.Translation.z - translation[ballbmp[0] - 1, ballbmp[1] - 1].w) * (_head.Translation.z - translation[ballbmp[0] - 1, ballbmp[1] - 1].w) + (_head.Translation.x - translation[ballbmp[0] - 1, ballbmp[1] - 1].z) * (_head.Translation.x - translation[ballbmp[0] - 1, ballbmp[1] - 1].z)) < ballradius)
                {
                    if (_head.Translation.x < oldX)
                    {
                        _head.Translation.z = translation[ballbmp[0] - 1, ballbmp[1] - 1].w + M.Sqrt((((ballradius + 0.01f) * (ballradius + 0.01f)) - (_head.Translation.x - translation[ballbmp[0] - 1, ballbmp[1] - 1].z) * (_head.Translation.x - translation[ballbmp[0] - 1, ballbmp[1] - 1].z)));
                    }

                    if (_head.Translation.z < oldY)
                    {
                        _head.Translation.x = translation[ballbmp[0] - 1, ballbmp[1] - 1].z + M.Sqrt((((ballradius + 0.01f) * (ballradius + 0.01f)) - (_head.Translation.z - translation[ballbmp[0] - 1, ballbmp[1] - 1].w) * (_head.Translation.z - translation[ballbmp[0] - 1, ballbmp[1] - 1].w)));
                    }
                }
            }

            if (bmp[ballbmp[0] - 1, ballbmp[1] + 1] == 1)
            {

                if (M.Sqrt((_head.Translation.z - translation[ballbmp[0] - 1, ballbmp[1] + 1].w) * (_head.Translation.z - translation[ballbmp[0] - 1, ballbmp[1] + 1].w) + (translation[ballbmp[0] - 1, ballbmp[1] + 1].x - _head.Translation.x) * (translation[ballbmp[0] - 1, ballbmp[1] + 1].x - _head.Translation.x)) < ballradius)
                {

                    if (_head.Translation.x > oldX)
                    {
                        _head.Translation.z = translation[ballbmp[0] - 1, ballbmp[1] + 1].w + M.Sqrt((((ballradius + 0.001f) * (ballradius + 0.001f)) - (translation[ballbmp[0] - 1, ballbmp[1] + 1].x - _head.Translation.x) * (translation[ballbmp[0] - 1, ballbmp[1] + 1].x - _head.Translation.x)));
                    }

                    if (_head.Translation.z < oldY)
                    {
                        _head.Translation.x = translation[ballbmp[0] - 1, ballbmp[1] + 1].x - M.Sqrt((((ballradius + 0.001f) * (ballradius + 0.001f)) - (_head.Translation.z - translation[ballbmp[0] - 1, ballbmp[1] + 1].w) * (_head.Translation.z - translation[ballbmp[0] - 1, ballbmp[1] + 1].w)));
                    }
                }
            }

            if (bmp[ballbmp[0] + 1, ballbmp[1] - 1] == 1)
            {

                if (M.Sqrt((translation[ballbmp[0] + 1, ballbmp[1] - 1].y - _head.Translation.z) * (translation[ballbmp[0] + 1, ballbmp[1] - 1].y - _head.Translation.z) + (_head.Translation.x - translation[ballbmp[0] + 1, ballbmp[1] - 1].z) * (_head.Translation.x - translation[ballbmp[0] + 1, ballbmp[1] - 1].z)) < ballradius)
                {

                    if (_head.Translation.x < oldX)
                    {
                        _head.Translation.z = translation[ballbmp[0] + 1, ballbmp[1] - 1].y - M.Sqrt((((ballradius + 0.01f) * (ballradius + 0.01f)) - (_head.Translation.x - translation[ballbmp[0] + 1, ballbmp[1] - 1].z) * (_head.Translation.x - translation[ballbmp[0] + 1, ballbmp[1] - 1].z)));
                    }

                    if (_head.Translation.z > oldY)
                    {
                        _head.Translation.x = translation[ballbmp[0] + 1, ballbmp[1] - 1].z + M.Sqrt((((ballradius + 0.01f) * (ballradius + 0.01f)) - (translation[ballbmp[0] + 1, ballbmp[1] - 1].y - _head.Translation.z) * (translation[ballbmp[0] + 1, ballbmp[1] - 1].y - _head.Translation.z)));
                    }
                }
            }

            if (bmp[ballbmp[0] + 1, ballbmp[1] + 1] == 1)
            {

                if (M.Sqrt((translation[ballbmp[0] + 1, ballbmp[1] + 1].y - _head.Translation.z) * (translation[ballbmp[0] + 1, ballbmp[1] + 1].y - _head.Translation.z) + (translation[ballbmp[0] + 1, ballbmp[1] + 1].x - _head.Translation.x) * (translation[ballbmp[0] + 1, ballbmp[1] + 1].x - _head.Translation.x)) < ballradius)
                {
                    if (_head.Translation.x > oldX)
                    {
                        _head.Translation.z = translation[ballbmp[0] + 1, ballbmp[1] + 1].y - M.Sqrt((((ballradius + 0.01f) * (ballradius + 0.01f)) - (translation[ballbmp[0] + 1, ballbmp[1] + 1].x - _head.Translation.x) * (translation[ballbmp[0] + 1, ballbmp[1] + 1].x - _head.Translation.x)));
                    }

                    if (_head.Translation.z > oldY)
                    {
                        _head.Translation.x = translation[ballbmp[0] + 1, ballbmp[1] + 1].x - M.Sqrt((((ballradius + 0.01f) * (ballradius + 0.01f)) - (translation[ballbmp[0] + 1, ballbmp[1] + 1].y - _head.Translation.z) * (translation[ballbmp[0] + 1, ballbmp[1] + 1].y - _head.Translation.z)));
                    }
                }
            }

            //goal collision
            if (bmp[ballbmp[0] - 1, ballbmp[1]] == 3)
            {
                if (_head.Translation.z - translation[ballbmp[0] - 1, ballbmp[1]].w < ballradius)
                {
                    movement = false;
                    win = true;
                }
            }

            if (bmp[ballbmp[0] + 1, ballbmp[1]] == 3)
            {
                if (translation[ballbmp[0] + 1, ballbmp[1]].y - _head.Translation.z < ballradius)
                {
                    movement = false;
                    win = true;
                }
            }

            if (bmp[ballbmp[0], ballbmp[1] - 1] == 3)
            {
                if (_head.Translation.x - translation[ballbmp[0], ballbmp[1] - 1].z < ballradius)
                {
                    movement = false;
                    win = true;
                }
            }

            if (bmp[ballbmp[0], ballbmp[1] + 1] == 3)
            {

                if (translation[ballbmp[0], ballbmp[1] + 1].x - _head.Translation.x < ballradius)
                {
                    movement = false;
                    win = true;
                }
            }
        }

        //check for win 
        public void winner()
        {
            if (win)
            {
                //sets the camera in the sky and rotate it
                camAngle += 0.2f * M.Pi / 180;
                var mtxRot = float4x4.CreateRotationZ(camAngle);
                var mtxPos = float4x4.LookAt(length / 2, 100, height / 2, length / 2, 0, height / 2, 1, 0, 0);
                var view = mtxRot * mtxPos;
                RC.View = view;

                //create winningdisplay once
                if (readonce)
                {
                    _winMap = new FontMap(fontLato, 55);

                    _gui = winningdisplay();

                    Resize(new ResizeEventArgs(Width, Height));
                    // Create the interaction handler
                    _sih = new SceneInteractionHandler(_gui);

                    _guiRenderer = new SceneRendererForward(_gui);

                    readonce = false;
                }
            }
        }

        //create winning display 
        public SceneContainer winningdisplay()
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
                ColorUint.Tofloat4(ColorUint.Greenery),
                HorizontalTextAlignment.CENTER,
                VerticalTextAlignment.CENTER);

            var endtime = new TextNodeContainer(
                "SOLVED\n" +
                timertext.Text,
                "Timer",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.MIDDLE),
                new MinMaxRect
                {
                    Min = new float2(0.01f, 0),
                    Max = new float2(0, 0.01f)
                },
                 _winMap,
                ColorUint.Tofloat4(ColorUint.Greenery),
                HorizontalTextAlignment.CENTER,
                VerticalTextAlignment.CENTER
            );


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
                    text,
                    endtime
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













        //creates the AABB
        public void makebox()
        {
            SceneContainer mazeScene = AssetStorage.Get<SceneContainer>("mazeAsset.fus");

            var cornerstone = mazeScene.Children.FindNodes(node => node.Name == "Cornerstone")?.FirstOrDefault()?.GetMesh();
            cornerbox = cornerstone.BoundingBox.Size.xz;

            var wallZ = mazeScene.Children.FindNodes(node => node.Name == "WallZ")?.FirstOrDefault()?.GetMesh();
            wallZbox = wallZ.BoundingBox.Size.xz;

            var wallX = mazeScene.Children.FindNodes(node => node.Name == "WallX")?.FirstOrDefault()?.GetMesh();
            wallXbox = wallX.BoundingBox.Size.xz;

            var ball = mazeScene.Children.FindNodes(node => node.Name == "Body")?.FirstOrDefault()?.GetMesh();
            ballradius = ball.BoundingBox.Size.x / 2;

            groundbox = new float2(wallXbox.x, wallZbox.y);
        }


        //create an Array with the positions
        public void CreatePositions()
        {
            for (int countY = 0; countY < bmp.GetLength(0); countY++)
            {
                for (int countX = 0; countX < bmp.GetLength(1); countX++)
                {
                    if (countX % 2 == 0 && countY % 2 == 0)
                    {
                        translation[countY, countX] = new float4((countX * (wallXbox.x + cornerbox.x) / 2) - cornerbox.x / 2, (countY * (wallZbox.y + cornerbox.y) / 2) - cornerbox.y / 2, (countX * (wallXbox.x + cornerbox.x) / 2) + cornerbox.x / 2, (countY * (wallZbox.y + cornerbox.y) / 2) + cornerbox.y / 2);
                    }

                    if (countX % 2 == 0 && countY % 2 == 1)
                    {
                        translation[countY, countX] = new float4((countX * (wallXbox.x + cornerbox.x) / 2) - wallZbox.x / 2, (countY * (wallZbox.y + cornerbox.y) / 2) - wallZbox.y / 2, (countX * (wallXbox.x + cornerbox.x) / 2) + wallZbox.x / 2, (countY * (wallZbox.y + cornerbox.y) / 2) + wallZbox.y / 2);
                    }

                    if (countX % 2 == 1 && countY % 2 == 0)
                    {
                        translation[countY, countX] = new float4((countX * (wallXbox.x + cornerbox.x) / 2) - wallXbox.x / 2, (countY * (wallZbox.y + cornerbox.y) / 2) - wallXbox.y / 2, (countX * (wallXbox.x + cornerbox.x) / 2) + wallXbox.x / 2, (countY * (wallZbox.y + cornerbox.y) / 2) + wallXbox.y / 2);
                    }

                    if (countX % 2 == 1 && countY % 2 == 1)
                    {
                        translation[countY, countX] = new float4((countX * (wallXbox.x + cornerbox.x) / 2) - groundbox.x / 2, (countY * (wallZbox.y + cornerbox.y) / 2) - groundbox.y / 2, (countX * (wallXbox.x + cornerbox.x) / 2) + groundbox.x / 2, (countY * (wallZbox.y + cornerbox.y) / 2) + groundbox.y / 2);
                    }
                }
            }
        }


        //finds the startpoint of the character 
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