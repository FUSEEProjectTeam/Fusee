using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Font = Fusee.Base.Core.Font;

namespace Fusee.Examples.Labyrinth.Core
{
    [FuseeApplication(Name = "FUSEE Labyrinth Example", Description = "A very sqiggly example.")]
    public class Labyrinth : RenderCanvas
    {
        //my var

        // angle variables
        private static float _angleVert = 1.5707f, _angleVelVert, _angle;

        //mouse rotation speed
        private const float _rotationSpeed = 7;

        //speed from character
        private float _speed = 7;

        //var for collision-detection
        private float2 _cornerbox;

        private float2 _wallZbox;
        private float2 _wallXbox;
        private float2 _groundbox;
        private float4[,] _translation;
        private int[] _ballbmp;
        private float _length;
        private float _height;
        private float _ballradius;
        private float _oldX;
        private float _oldY;
        private int[,] _bmp = Bmp();

        //cam pos for changing
        private float3 _cam = new float3(10, 5, 10);

        private int _cases = 0;

        //movment
        private float _moveX, _moveZ;

        //timer display
        private GUIText _timertext;

        private FontMap _timeMap;

        //winning display

        private FontMap _winMap;

        //font
        private Font _fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");

        //call winningdisplay method only once
        private bool _readonce = true;

        //stops ball when winning
        private bool _movement = true;

        private bool _win = false;

        //camera
        private float4x4 _mtxCam;

        private float _camAngle = 0;

        //Transform and SceneContainer
        private Transform _head;

        private Transform _body;
        private Transform _bodytrans;
        private SceneContainer _scene;
        private Transform mazeTransform = new Transform();

        //other var
        private SceneRendererForward _sceneRenderer;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;

        private SceneContainer CreateScene()
        {
            //load the Nodes in
            SceneContainer mazeScene = AssetStorage.Get<SceneContainer>("mazeAsset.fus");
            SceneNode cornerstone = mazeScene.Children.FindNodes(n => n.Name == "Cornerstone").First();
            SceneNode wallX = mazeScene.Children.FindNodes(n => n.Name == "WallX").First();
            SceneNode wallZ = mazeScene.Children.FindNodes(n => n.Name == "WallZ").First();
            SceneNode ball = mazeScene.Children.FindNodes(n => n.Name == "Body").First();
            SceneNode head = mazeScene.Children.FindNodes(n => n.Name == "Head").First();
            Cube _ground = new Cube();

            SceneNode maze = new SceneNode
            {
                Components = new List<SceneComponent>
                {
                    // TRANSFROM COMPONENT
                    mazeTransform,
                    // SHADER EFFECT COMPONENT
                    cornerstone.GetComponent<ShaderEffect>(),
                },
                Children = new ChildList()
            };

            for (int countY = 0; countY < _bmp.GetLength(1); countY++)
            {
                for (int countX = 0; countX < _bmp.GetLength(0); countX++)
                {
                    if (countX % 2 == 0 && countY % 2 == 0 && _bmp[countY, countX] == 1)
                    {
                        maze.Children.Add(new SceneNode
                        {
                            Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Translation = new float3(countX * (_wallXbox.x + _cornerbox.x)/2, 2.2f, countY * (_wallZbox.y + _cornerbox.y)/2)
                                    },
                                    cornerstone.GetComponent<ShaderEffect>(),
                                    cornerstone.GetComponent<Mesh>()
                                },
                            Name = "Wall" + countY.ToString().PadLeft(2, '0') + countX.ToString().PadLeft(2, '0')
                        }
                        );
                    }

                    if (countX % 2 == 1 && countY % 2 == 0 && _bmp[countY, countX] == 1)
                    {
                        maze.Children.Add(new SceneNode
                        {
                            Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Translation = new float3(countX * (_wallXbox.x + _cornerbox.x)/2, 2.2f, countY * (_wallZbox.y + _cornerbox.y)/2)
                                    },
                                    wallX.GetComponent<ShaderEffect>(),
                                    wallX.GetComponent<Mesh>()
                                },
                            Name = "Wall" + countY.ToString().PadLeft(2, '0') + countX.ToString().PadLeft(2, '0')
                        }
                        );
                    }

                    if (countX % 2 == 0 && countY % 2 == 1 && _bmp[countY, countX] == 1)
                    {
                        maze.Children.Add(new SceneNode
                        {
                            Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Translation = new float3(countX * (_wallXbox.x + _cornerbox.x)/2, 2.2f, countY * (_wallZbox.y + _cornerbox.y)/2)
                                    },
                                    wallZ.GetComponent<ShaderEffect>(),
                                    wallZ.GetComponent<Mesh>()
                                },
                            Name = "Wall" + countY.ToString().PadLeft(2, '0') + countX.ToString().PadLeft(2, '0')
                        }
                        );
                    }

                    if (countX % 2 == 1 && countY % 2 == 1 && _bmp[countY, countX] == -1)
                    {
                        maze.Children.Add(new SceneNode
                        {
                            Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Translation = new float3(countX * (_wallXbox.x + _cornerbox.x)/2, _ballradius, countY * (_wallZbox.y + _cornerbox.y)/2),
                                    },
                                    head.GetComponent<ShaderEffect>(),
                                    head.GetComponent<Mesh>()
                                },
                            Name = "Head",

                            Children = new ChildList
                            {
                                new SceneNode
                                {
                                    Components = new List<SceneComponent>
                                    {
                                    new Transform
                                        {
                                            Translation = new float3(0,0,0)
                                        },
                                    },
                                Name = "Bodytrans",

                                Children = new ChildList
                                {
                                    new SceneNode
                                    {
                                        Components = new List<SceneComponent>
                                        {
                                        new Transform
                                            {
                                                Translation = new float3(0,0,0)
                                            },
                                            ball.GetComponent<ShaderEffect>(),
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

            maze.Children.Add(new SceneNode
            {
                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Scale = new float3(_length, 1, _height),
                                        Translation = new float3(_length/2 - _cornerbox.x/2, -0.5f, _height/2 - _cornerbox.y/2)
                                    },
                                    ShaderCodeBuilder.MakeShaderEffectProto(new float4(0.545f, 0.270f, 0.074f, 1), new float4(0, 0, 0, 1), 136.75444f, 0.483772248f),
                                    _ground
                                },
                Name = "Ground"
            }
            );

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    maze
                }
            };
        }

        // Init is called on startup.
        async public override Task<bool> Init()
        {
            _timeMap = new FontMap(_fontLato, 24);
            _gui = CreateGui();
            Resize(new ResizeEventArgs(Width, Height));
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            //find the ball and create AABB
            FindBall();
            MakeBox();

            //get length and height for the ground
            _translation = new float4[_bmp.GetLength(0), _bmp.GetLength(1)];

            CreatePositions();

            //create length and height for the ground
            _length = _translation[_translation.GetLength(0) - 1, 0].w + _cornerbox.y / 2;
            _height = _translation[0, _translation.GetLength(1) - 1].z + _cornerbox.x / 2;

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
                _angleVelVert = -_rotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
            }
            else
            {
                _angleVelVert = 0;
            }

            _angleVert = (_angleVert + _angleVelVert) % (2 * M.Pi);

            if (Mouse.IsButtonDown(2))
            {
                switch (_cases)
                {
                    case 0:
                        _cam = new float3(1, 0, 1);
                        _cases++;
                        break;

                    case 1:
                        _cam = new float3(10, 5, 10);
                        _cases %= 1;
                        break;
                };
            }

            Ballmovement();
            Collision();
            Winner();

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
            var fuseeLogo = new TextureNode(
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

            var guiLatoBlack = new FontMap(_fontLato, 18);

            var text = new TextNode(
                "FUSEE Labyrinth Example",
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
            var timer = new TextNode(
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

            _timertext = timer.GetComponentsInChildren<GUIText>().FirstOrDefault();

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
                    text,
                    timer
                }
            };

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    //Add canvas.
                    canvas
                }
            };
        }

        public void BtnLogoEnter(CodeComponent sender)
        {
            _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>().SetEffectParam("DiffuseColor", new float4(0.8f, 0.8f, 0.8f, 1f));
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>().SetEffectParam("DiffuseColor", float4.One);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }

        //my methods
        public void Ballmovement()
        {
            if (_movement || !_win)
            {
                //set time for stopwatch
                int minutes = (int)Time.TimeSinceStart / 60;
                var seconds = Time.TimeSinceStart % 59.5f;
                var miliseconds = Time.TimeSinceStart % 0.99f;
                _timertext.Text = minutes.ToString("00") + ":" + seconds.ToString("00") + miliseconds.ToString(".00", new System.Globalization.CultureInfo("en-US"));

                //camera
                _angle = _angleVert;
                if (Keyboard.GetKey(KeyCodes.E))
                {
                    _mtxCam = float4x4.LookAt(_length / 2, 100, _height / 2, _length / 2, 0, _height / 2, -1, 0, 0);
                    _movement = false;
                }
                else
                {
                    _mtxCam = float4x4.LookAt(_head.Translation.x - _cam.x * M.Cos(_angle), _head.Translation.y + _cam.y, _head.Translation.z - _cam.z * M.Sin(_angle), _head.Translation.x, _head.Translation.y, _head.Translation.z, 0, 1, 0);
                    _head.Rotation = new float3(_head.Rotation.x, -_angle - 90 * M.Pi / 180, _head.Rotation.z);
                    _bodytrans.Rotation = new float3(0, _angle, 0);
                    _movement = true;
                }
                RC.View = _mtxCam;

                if (_movement)
                {
                    Diagnostics.Debug(_head.Rotation);
                    //get old positions for the head
                    _oldX = _head.Translation.x;
                    _oldY = _head.Translation.z;

                    //move the ball
                    _moveX = Keyboard.ADAxis * _speed * DeltaTime;
                    if (_moveX < 0)
                    {
                        _head.Translation.x += _moveX * M.Sin(_angle);
                        _head.Translation.z -= _moveX * M.Cos(_angle);

                        _body.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(_angle), 0, M.Cos(_angle)), _moveX)), 0);
                    }
                    else if (_moveX > 0)
                    {
                        _head.Translation.x += _moveX * M.Sin(_angle);
                        _head.Translation.z -= _moveX * M.Cos(_angle);
                        _body.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(_angle), 0, M.Cos(_angle)), _moveX)), 0);
                    }

                    _moveZ = Keyboard.WSAxis * _speed * DeltaTime;
                    if (_moveZ < 0)
                    {
                        _head.Translation.x += _moveZ * M.Cos(_angle);
                        _head.Translation.z += _moveZ * M.Sin(_angle);
                        _body.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(_angle), 0, M.Sin(_angle)), -_moveZ)), 0);
                    }
                    else if (_moveZ > 0)
                    {
                        _head.Translation.x += _moveZ * M.Cos(_angle);
                        _head.Translation.z += _moveZ * M.Sin(_angle);
                        _body.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(_angle), 0, M.Sin(_angle)), -_moveZ)), 0);
                    }

                    if (Keyboard.GetKey(KeyCodes.A) || Keyboard.GetKey(KeyCodes.D))
                    {
                        if (_head.Rotation.z > -30 * M.Pi / 180 && _moveX < 0)
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(_angle), 0, M.Sin(_angle)), -_moveX)), 0);
                        }
                        if (_head.Rotation.z < 30 * M.Pi / 180 && _moveX > 0)
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(_angle), 0, M.Sin(_angle)), -_moveX)), 0);
                        }
                    }
                    else if (Keyboard.GetKey(KeyCodes.W) || Keyboard.GetKey(KeyCodes.S))
                    {
                        if (_head.Rotation.x > -30 * M.Pi / 180 && _moveZ > 0)
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(_angle), 0, M.Cos(_angle)), -_moveZ)), 0);
                        }
                        if (_head.Rotation.x < 30 * M.Pi / 180 && _moveZ < 0)
                        {
                            _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(_angle), 0, M.Cos(_angle)), -_moveZ)), 0);
                        }
                    }
                    if (_head.Translation.x == _oldX)
                    {
                        if (_head.Rotation.x < -0.00001f)
                        {
                            if (_head.Rotation.x + 0.1f < -0.1f)
                            {
                                _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(_angle), 0, M.Cos(_angle)), 0.1f)), 0);
                            }
                            else
                            {
                                _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(_angle), 0, M.Cos(_angle)), -_head.Rotation.x)), 0);
                            }
                        }

                        if (_head.Rotation.x > 0.00001f)
                        {
                            if (_head.Rotation.x - 0.1f > 0.1f)
                            {
                                _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(_angle), 0, M.Cos(_angle)), -0.1f)), 0);
                            }
                            else
                            {
                                _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(-M.Sin(_angle), 0, M.Cos(_angle)), -_head.Rotation.x)), 0);
                            }
                        }
                    }
                    if (_head.Translation.z == _oldY)
                    {
                        if (_head.Rotation.z < -0.00001f)
                        {
                            if (_head.Rotation.z + 0.1f < -0.1f)
                            {
                                _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(_angle), 0, M.Sin(_angle)), -0.1f)), 0);
                            }
                            else
                            {
                                _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(_angle), 0, M.Sin(_angle)), _head.Rotation.z)), 0);
                            }
                        }

                        if (_head.Rotation.z > 0.00001f)
                        {
                            if (_head.Rotation.z - 0.1f > 0.1f)
                            {
                                _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(_angle), 0, M.Sin(_angle)), 0.1f)), 0);
                            }
                            else
                            {
                                _head.Rotate(Quaternion.QuaternionToEuler(Quaternion.FromAxisAngle(new float3(M.Cos(_angle), 0, M.Sin(_angle)), _head.Rotation.z)), 0);
                            }
                        }
                    }
                }
            }
        }

        public void Collision()
        {
            //change the ballbmp when the body moves around
            if (_translation[_ballbmp[0], _ballbmp[1]].x <= _head.Translation.x)
            {
                if (_translation[_ballbmp[0], _ballbmp[1]].z >= _head.Translation.x)
                {
                    if (_translation[_ballbmp[0], _ballbmp[1]].y <= _head.Translation.z)
                    {
                        if (_translation[_ballbmp[0], _ballbmp[1]].w >= _head.Translation.z)
                        { }
                        else
                        {
                            _ballbmp[0] = _ballbmp[0] + 1;
                        }
                    }
                    else
                    {
                        _ballbmp[0] = _ballbmp[0] - 1;
                    }
                }
                else
                {
                    _ballbmp[1] = _ballbmp[1] + 1;
                }
            }
            else
            {
                _ballbmp[1] = _ballbmp[1] - 1;
            }

            //Walls
            if (_bmp[_ballbmp[0] - 1, _ballbmp[1]] == 1 || _bmp[_ballbmp[0] - 1, _ballbmp[1]] == 2)
            {
                if (_head.Translation.z - _translation[_ballbmp[0] - 1, _ballbmp[1]].w < _ballradius)
                {
                    _head.Translation.z = _translation[_ballbmp[0] - 1, _ballbmp[1]].w + _ballradius + 0.0001f;
                }
            }

            if (_bmp[_ballbmp[0] + 1, _ballbmp[1]] == 1 || _bmp[_ballbmp[0] + 1, _ballbmp[1]] == 2)
            {
                if (_translation[_ballbmp[0] + 1, _ballbmp[1]].y - _head.Translation.z < _ballradius)
                {
                    _head.Translation.z = _translation[_ballbmp[0] + 1, _ballbmp[1]].y - _ballradius - 0.0001f;
                }
            }

            if (_bmp[_ballbmp[0], _ballbmp[1] - 1] == 1 || _bmp[_ballbmp[0], _ballbmp[1] - 1] == 2)
            {
                if (_head.Translation.x - _translation[_ballbmp[0], _ballbmp[1] - 1].z < _ballradius)
                {
                    _head.Translation.x = _translation[_ballbmp[0], _ballbmp[1] - 1].z + _ballradius + 0.0001f;
                }
            }

            if (_bmp[_ballbmp[0], _ballbmp[1] + 1] == 1 || _bmp[_ballbmp[0], _ballbmp[1] + 1] == 2)
            {
                if (_translation[_ballbmp[0], _ballbmp[1] + 1].x - _head.Translation.x < _ballradius)
                {
                    _head.Translation.x = _translation[_ballbmp[0], _ballbmp[1] + 1].x - _ballradius - 0.0001f;
                }
            }
            //Corners

            if (_bmp[_ballbmp[0] - 1, _ballbmp[1] - 1] == 1)
            {
                if (System.MathF.Sqrt((_head.Translation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].w) * (_head.Translation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].w) + (_head.Translation.x - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].z) * (_head.Translation.x - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].z)) < _ballradius)
                {
                    if (_head.Translation.x < _oldX)
                    {
                        _head.Translation.z = _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].w + System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (_head.Translation.x - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].z) * (_head.Translation.x - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].z)));
                    }

                    if (_head.Translation.z < _oldY)
                    {
                        _head.Translation.x = _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].z + System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (_head.Translation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].w) * (_head.Translation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].w)));
                    }
                }
            }

            if (_bmp[_ballbmp[0] - 1, _ballbmp[1] + 1] == 1)
            {
                if (System.MathF.Sqrt((_head.Translation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].w) * (_head.Translation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].w) + (_translation[_ballbmp[0] - 1, _ballbmp[1] + 1].x - _head.Translation.x) * (_translation[_ballbmp[0] - 1, _ballbmp[1] + 1].x - _head.Translation.x)) < _ballradius)
                {
                    if (_head.Translation.x > _oldX)
                    {
                        _head.Translation.z = _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].w + System.MathF.Sqrt((((_ballradius + 0.001f) * (_ballradius + 0.001f)) - (_translation[_ballbmp[0] - 1, _ballbmp[1] + 1].x - _head.Translation.x) * (_translation[_ballbmp[0] - 1, _ballbmp[1] + 1].x - _head.Translation.x)));
                    }

                    if (_head.Translation.z < _oldY)
                    {
                        _head.Translation.x = _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].x - System.MathF.Sqrt((((_ballradius + 0.001f) * (_ballradius + 0.001f)) - (_head.Translation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].w) * (_head.Translation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].w)));
                    }
                }
            }

            if (_bmp[_ballbmp[0] + 1, _ballbmp[1] - 1] == 1)
            {
                if (System.MathF.Sqrt((_translation[_ballbmp[0] + 1, _ballbmp[1] - 1].y - _head.Translation.z) * (_translation[_ballbmp[0] + 1, _ballbmp[1] - 1].y - _head.Translation.z) + (_head.Translation.x - _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].z) * (_head.Translation.x - _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].z)) < _ballradius)
                {
                    if (_head.Translation.x < _oldX)
                    {
                        _head.Translation.z = _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].y - System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (_head.Translation.x - _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].z) * (_head.Translation.x - _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].z)));
                    }

                    if (_head.Translation.z > _oldY)
                    {
                        _head.Translation.x = _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].z + System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (_translation[_ballbmp[0] + 1, _ballbmp[1] - 1].y - _head.Translation.z) * (_translation[_ballbmp[0] + 1, _ballbmp[1] - 1].y - _head.Translation.z)));
                    }
                }
            }

            if (_bmp[_ballbmp[0] + 1, _ballbmp[1] + 1] == 1)
            {
                if (System.MathF.Sqrt((_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].y - _head.Translation.z) * (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].y - _head.Translation.z) + (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].x - _head.Translation.x) * (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].x - _head.Translation.x)) < _ballradius)
                {
                    if (_head.Translation.x > _oldX)
                    {
                        _head.Translation.z = _translation[_ballbmp[0] + 1, _ballbmp[1] + 1].y - System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].x - _head.Translation.x) * (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].x - _head.Translation.x)));
                    }

                    if (_head.Translation.z > _oldY)
                    {
                        _head.Translation.x = _translation[_ballbmp[0] + 1, _ballbmp[1] + 1].x - System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].y - _head.Translation.z) * (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].y - _head.Translation.z)));
                    }
                }
            }

            //goal collision
            if (_bmp[_ballbmp[0] - 1, _ballbmp[1]] == 3)
            {
                if (_head.Translation.z - _translation[_ballbmp[0] - 1, _ballbmp[1]].w < _ballradius)
                {
                    _movement = false;
                    _win = true;
                }
            }

            if (_bmp[_ballbmp[0] + 1, _ballbmp[1]] == 3)
            {
                if (_translation[_ballbmp[0] + 1, _ballbmp[1]].y - _head.Translation.z < _ballradius)
                {
                    _movement = false;
                    _win = true;
                }
            }

            if (_bmp[_ballbmp[0], _ballbmp[1] - 1] == 3)
            {
                if (_head.Translation.x - _translation[_ballbmp[0], _ballbmp[1] - 1].z < _ballradius)
                {
                    _movement = false;
                    _win = true;
                }
            }

            if (_bmp[_ballbmp[0], _ballbmp[1] + 1] == 3)
            {
                if (_translation[_ballbmp[0], _ballbmp[1] + 1].x - _head.Translation.x < _ballradius)
                {
                    _movement = false;
                    _win = true;
                }
            }
        }

        //check for win
        public void Winner()
        {
            if (_win)
            {
                //sets the camera in the sky and rotate it
                _camAngle += 0.2f * M.Pi / 180;
                var mtxRot = float4x4.CreateRotationZ(_camAngle);
                var mtxPos = float4x4.LookAt(_length / 2, 100, _height / 2, _length / 2, 0, _height / 2, 1, 0, 0);
                var view = mtxRot * mtxPos;
                RC.View = view;

                //create winningdisplay once
                if (_readonce)
                {
                    _winMap = new FontMap(_fontLato, 55);

                    _gui = WinningDisplay();

                    Resize(new ResizeEventArgs(Width, Height));
                    // Create the interaction handler
                    _sih = new SceneInteractionHandler(_gui);

                    _guiRenderer = new SceneRendererForward(_gui);

                    _readonce = false;
                }
            }
        }

        //create winning display
        public SceneContainer WinningDisplay()
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
            var fuseeLogo = new TextureNode(
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

            var text = new TextNode(
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

            var endtime = new TextNode(
                "SOLVED\n" +
                _timertext.Text,
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
                    text,
                    endtime
                }
            };

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    //Add canvas.
                    canvas
                }
            };
        }

        //creates the AABB
        public void MakeBox()
        {
            SceneContainer mazeScene = AssetStorage.Get<SceneContainer>("mazeAsset.fus");

            var cornerstone = mazeScene.Children.FindNodes(node => node.Name == "Cornerstone")?.FirstOrDefault()?.GetMesh();
            _cornerbox = cornerstone.BoundingBox.Size.xz;

            var wallZ = mazeScene.Children.FindNodes(node => node.Name == "WallZ")?.FirstOrDefault()?.GetMesh();
            _wallZbox = wallZ.BoundingBox.Size.xz;

            var wallX = mazeScene.Children.FindNodes(node => node.Name == "WallX")?.FirstOrDefault()?.GetMesh();
            _wallXbox = wallX.BoundingBox.Size.xz;

            var ball = mazeScene.Children.FindNodes(node => node.Name == "Body")?.FirstOrDefault()?.GetMesh();
            _ballradius = ball.BoundingBox.Size.x / 2;

            _groundbox = new float2(_wallXbox.x, _wallZbox.y);
        }

        //create an Array with the positions
        public void CreatePositions()
        {
            for (int countY = 0; countY < _bmp.GetLength(0); countY++)
            {
                for (int countX = 0; countX < _bmp.GetLength(1); countX++)
                {
                    if (countX % 2 == 0 && countY % 2 == 0)
                    {
                        _translation[countY, countX] = new float4((countX * (_wallXbox.x + _cornerbox.x) / 2) - _cornerbox.x / 2, (countY * (_wallZbox.y + _cornerbox.y) / 2) - _cornerbox.y / 2, (countX * (_wallXbox.x + _cornerbox.x) / 2) + _cornerbox.x / 2, (countY * (_wallZbox.y + _cornerbox.y) / 2) + _cornerbox.y / 2);
                    }

                    if (countX % 2 == 0 && countY % 2 == 1)
                    {
                        _translation[countY, countX] = new float4((countX * (_wallXbox.x + _cornerbox.x) / 2) - _wallZbox.x / 2, (countY * (_wallZbox.y + _cornerbox.y) / 2) - _wallZbox.y / 2, (countX * (_wallXbox.x + _cornerbox.x) / 2) + _wallZbox.x / 2, (countY * (_wallZbox.y + _cornerbox.y) / 2) + _wallZbox.y / 2);
                    }

                    if (countX % 2 == 1 && countY % 2 == 0)
                    {
                        _translation[countY, countX] = new float4((countX * (_wallXbox.x + _cornerbox.x) / 2) - _wallXbox.x / 2, (countY * (_wallZbox.y + _cornerbox.y) / 2) - _wallXbox.y / 2, (countX * (_wallXbox.x + _cornerbox.x) / 2) + _wallXbox.x / 2, (countY * (_wallZbox.y + _cornerbox.y) / 2) + _wallXbox.y / 2);
                    }

                    if (countX % 2 == 1 && countY % 2 == 1)
                    {
                        _translation[countY, countX] = new float4((countX * (_wallXbox.x + _cornerbox.x) / 2) - _groundbox.x / 2, (countY * (_wallZbox.y + _cornerbox.y) / 2) - _groundbox.y / 2, (countX * (_wallXbox.x + _cornerbox.x) / 2) + _groundbox.x / 2, (countY * (_wallZbox.y + _cornerbox.y) / 2) + _groundbox.y / 2);
                    }
                }
            }
        }

        //finds the startpoint of the character
        public void FindBall()
        {
            for (int countY = 0; countY < _bmp.GetLength(1); countY++)
            {
                for (int countX = 0; countX < _bmp.GetLength(0); countX++)
                {
                    if (_bmp[countX, countY] == -1)
                    {
                        _ballbmp = new int[] { countX, countY };
                    }
                }
            }
        }

        public static int[,] Bmp()
        {
            int x = 25;
            int y = 25;

            int posX = 0;
            int posY = 0;

            ImageData img = AssetStorage.Get<ImageData>("Maze.png");
            int[,] image = new int[img.Width / x, img.Height / y];
            for (int j = 5; j < img.Height; j += y)
            {
                for (int i = 5; i < img.Width; i += x)
                {
                    if (GetPixel(img, j, i).Equals(new ColorUint(0,0,0,255)))
                    {
                        image[posY, posX] = 1;
                    }
                    else if (GetPixel(img, j, i).Equals(new ColorUint(255, 0, 0, 255)))
                    {
                        image[posY, posX] = 2;
                    }
                    else if (GetPixel(img, j, i).Equals(new ColorUint(0, 255, 0, 255)))
                    {
                        image[posY, posX] = -1;
                    }
                    else if (GetPixel(img, j, i).Equals(new ColorUint(0, 0, 255, 255)))
                    {
                        image[posY, posX] = 3;
                    }
                    else
                    {
                        image[posY, posX] = 0;
                    }
                    if (x == 25) { x++; } else { x--; }
                    posX += 1;
                    posX %= 31;
                }
                if (y == 25) { y++; } else { y--; }
                posY += 1;
                posY %= 31;
            }
            return image;
        }

        public static Color[,] imageorder(byte[] arr, int width, int height)
        {
            Color[,] image = new Color[width, height];

            for (int i = height - 1; i >= 0; i--)
            {
                for (int j = 0; j < width; j++)
                {
                    image[j, height - 1 - i] = Color.FromArgb(arr[4 * (width * i + j) + 3], arr[4 * (width * i + j) + 2], arr[4 * (width * i + j) + 1], arr[4 * (width * i + j)]);
                }
            }

            return image;
        }

        public static ColorUint GetPixel(ImageData img, int x, int y)
        {
            int bpp = img.PixelFormat.BytesPerPixel;
            switch (bpp)
            {
                case 4:
                    return new ColorUint(img.PixelData[bpp * (img.Width * y + x) + 2], img.PixelData[bpp * (img.Width * y + x) + 1], img.PixelData[bpp * (img.Width * y + x)], img.PixelData[bpp * (img.Width * y + x) + 3]);

                case 3:
                    return new ColorUint(img.PixelData[bpp * (img.Width * y + x) + 2], img.PixelData[bpp * (img.Width * y + x) + 1], img.PixelData[bpp * (img.Width * y + x)], 255);

                case 1:
                    return new ColorUint(0, 0, 0, img.PixelData[bpp * (img.Width * y + x) + 3]);

                default:
                    return new ColorUint(0, 0, 0, 0);

            }

        }   
    }
}