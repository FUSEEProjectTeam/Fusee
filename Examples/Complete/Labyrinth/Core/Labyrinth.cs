using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Font = Fusee.Base.Core.Font;

namespace Fusee.Examples.Labyrinth.Core
{
    [FuseeApplication(Name = "FUSEE Labyrinth Example", Description = "A very squiggly example.")]
    public class Labyrinth : RenderCanvas
    {
        // Angle variables
        private static float _angle, _angleVelVert;

        // Mouse rotation speed
        private const float _rotationSpeed = 7;

        // Speed from character
        private readonly float _speed = 7;

        // Var for collision-detection
        private float3 _cornerbox;
        private float3 _wallZbox;
        private float3 _wallXbox;
        private float2 _groundbox;
        private float4[,] _translation;
        private int[] _ballbmp;
        private float _length;
        private float _height;
        private float _ballradius;
        private float _oldX;
        private float _oldY;
        private readonly int[,] _bmp = Bmp();

        private int _camViewCase = 1;
        private float _moveX, _moveZ;

        private GuiText _timertext;
        private readonly Font _fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");

        // Call winning display method only once
        private bool _isGuiCreated = false;

        // Bool for Character movement
        private bool _isMoving = true;

        private Transform _mainCamTransform;
        private Camera _mainCam;

        private SceneNode _overViewCamNode;
        private Transform _overviewCamTransform;
        private Camera _overviewCam;

        private bool _isGameWon = false;

        // Transform and SceneContainer
        private Transform _headTransform;
        private Transform _bodyTransform;
        private SceneNode _bodyNode;
        private Transform _bodyPivotTransform;
        private SceneContainer _scene;
        private readonly Transform mazeTransform = new();

        private SceneRendererForward _sceneRenderer;
        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.Screen;

        private SceneContainer CreateScene()
        {
            // Load the Nodes
            SceneContainer mazeScene = AssetStorage.Get<SceneContainer>("mazeAsset_tga.fus");
            SceneNode cornerstone = mazeScene.Children.FindNodes(n => n.Name == "Cornerstone").First();
            SceneNode wallX = mazeScene.Children.FindNodes(n => n.Name == "WallX").First();
            SceneNode wallZ = mazeScene.Children.FindNodes(n => n.Name == "WallZ").First();
            SceneNode ball = mazeScene.Children.FindNodes(n => n.Name == "Body").First();
            SceneNode head = mazeScene.Children.FindNodes(n => n.Name == "Head").First();
            Cube _ground = new();

            _mainCamTransform = new Transform()
            {
                Rotation = new float3(M.PiOver6, 0, 0),
                Translation = new float3(0, 7, -10),
                Scale = float3.One
            };
            _mainCam = new Camera(ProjectionMethod.Perspective, 1, 1000, M.PiOver4)
            {
                BackgroundColor = float4.UnitW
            };

            _overviewCamTransform = new Transform()
            {
                Translation = new float3(_length / 2, 100, _length / 2),
                Rotation = new float3(M.PiOver2, 0, 0),
                Scale = float3.One
            };

            _overviewCam = new Camera(ProjectionMethod.Perspective, 1, 1000, M.PiOver4)
            {
                Active = false,
                BackgroundColor = float4.UnitW
            };

            SceneNode maze = new()
            {
                Components = new List<SceneComponent>
                {
                    // TRANSFROM COMPONENT
                    mazeTransform
                },
                Name = "maze",
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
                                        Translation = new float3(countX * (_wallXbox.x + _cornerbox.x)/2, _cornerbox.y / 2, countY * (_wallZbox.z + _cornerbox.z)/2)
                                    },
                                    cornerstone.GetComponent<SurfaceEffect>(),
                                    cornerstone.GetComponent<Mesh>()
                                },
                            Name = "Cornerstone" + countY.ToString().PadLeft(2, '0') + countX.ToString().PadLeft(2, '0')
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
                                        Translation = new float3(countX * (_wallXbox.x + _cornerbox.x)/2, _wallXbox.y / 2, countY * (_wallZbox.z + _cornerbox.z)/2)
                                    },
                                    wallX.GetComponent<SurfaceEffect>(),
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
                                        Translation = new float3(countX * (_wallXbox.x + _cornerbox.x)/2, _wallZbox.y / 2, countY * (_wallZbox.z + _cornerbox.z)/2)
                                    },
                                    wallZ.GetComponent<SurfaceEffect>(),
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
                                    Translation = new float3(countX * (_wallXbox.x + _cornerbox.x)/2, _ballradius, countY * (_wallZbox.z + _cornerbox.z)/2),
                                },
                                head.GetComponent<SurfaceEffect>(),
                                head.GetComponent<Mesh>()
                            },
                            Name = "Head",

                            Children = new ChildList
                            {
                                new SceneNode
                                {
                                    Name = "MainCam",
                                    Components = new List<SceneComponent>
                                    {
                                        _mainCamTransform,
                                        _mainCam
                                    }
                                },

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
                                                ball.GetComponent<SurfaceEffect>(),
                                                ball.GetComponent<Mesh>()
                                            },
                                        Name = "Body",
                                        }
                                    },
                                }
                            },
                        });
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
                        Translation = new float3(_length/2 - _cornerbox.x/2, -0.5f, _height/2 - _cornerbox.z/2)
                    },
                    //ShaderCodeBuilder.MakeShaderEffectProto(new float4(0.8f, 0.8f, 0.8f, 1), new float4(0, 0, 0, 1), 136.75444f, 0.483772248f),
                    MakeEffect.FromDiffuseSpecular(new float4(0.5f, 0.5f, 0.5f, 1)),
                    _ground
                },
                Name = "Ground"
            }
            );

            _overViewCamNode = new SceneNode()
            {
                Components = new List<SceneComponent>
                {
                    _overviewCamTransform,
                    _overviewCam
                }
            };

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    _overViewCamNode,
                    maze
                }
            };
        }

        // Init is called on startup.
        public override void Init()
        {
            _gui = CreateGui();
            _sih = new SceneInteractionHandler(_gui);

            // Find the ball and create AABB
            FindBall();
            MakeBox();

            // Creates Position variable for all Walls with xy and x + length and y + length (xywz)
            _translation = new float4[_bmp.GetLength(0), _bmp.GetLength(1)];

            CreatePositions();

            // Creates length and height for the ground
            _length = _translation[_translation.GetLength(0) - 1, 0].w + _cornerbox.z / 2;
            _height = _translation[0, _translation.GetLength(1) - 1].z + _cornerbox.x / 2;

            // Load the rocket model
            _scene = CreateScene();
            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _guiRenderer = new SceneRendererForward(_gui);

            // Get Nodes from _scene
            _headTransform = _scene.Children.FindNodes(node => node.Name == "Head")?.FirstOrDefault()?.GetTransform();

            _bodyPivotTransform = _scene.Children.FindNodes(node => node.Name == "Bodytrans").FirstOrDefault()?.GetTransform();
            _bodyNode = _scene.Children.FindNodes(node => node.Name == "Body")?.FirstOrDefault();
            _bodyTransform = _bodyNode?.GetTransform();
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            if (!_isGameWon && Keyboard.IsKeyUp(KeyCodes.C))
                _isGameWon = true;

            // Mouse movement
            if (Mouse.LeftButton)
                _angleVelVert = _rotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
            else
                _angleVelVert = 0;


            _angle = (_angle + _angleVelVert) % (2 * M.Pi);

            if (Mouse.IsButtonDown(2) && _isMoving)
            {
                switch (_camViewCase)
                {
                    case 0:
                        _mainCamTransform.Rotation = new float3(M.PiOver6, 0, 0);
                        _mainCamTransform.Translation = new float3(0, 7, -10);
                        _camViewCase = 1;
                        break;

                    case 1:
                        _mainCamTransform.Rotation = new float3(0, 0, 0);
                        _mainCamTransform.Translation = new float3(0, 0, 0);
                        _camViewCase = 0;
                        break;
                };
            }

            if (!_isMoving)
            {
                _overviewCam.Active = true;
                _mainCam.Active = false;
            }
            else
            {
                _overviewCam.Active = false;
                _mainCam.Active = true;
            }

            Collision();
            Ballmovement();
            OnWonGame();

            _sceneRenderer.Render(RC);

            _guiRenderer.Render(RC);
            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private SceneContainer CreateGui()
        {
            var canvasWidth = Width / 100f;
            var canvasHeight = Height / 100f;

            var btnFuseeLogo = new GuiButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            var fuseeLogo = TextureNode.Create(
                "fuseeLogo",
                //Set the albedo texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                GuiElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                //Define Offset and therefor the size of the element.
                GuiElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f)),
                float2.One
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 24);
            var guiLatoBlackSmall = new FontMap(fontLato, 16);

            var text = TextNode.Create(
                "FUSEE Labyrinth Example",
                "ButtonText",
                GuiElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                GuiElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                (float4)ColorUint.Greenery,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            var instructions = TextNode.Create(
                "Controls:\n" +
                "W, S, A, D: Move Robot\n" +
                "Left Click & move mouse: rotate camera\n" +
                "Right Click: Toggle between first and third person\n" +
                "Hold E: labyrinth overview ",
                "ButtonText",
                GuiElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                GuiElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0.125f, canvasHeight - 2), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlackSmall,
                (float4)ColorUint.White,
                HorizontalTextAlignment.Left,
                VerticalTextAlignment.Top);

            // Create stopwatch
            var timer = TextNode.Create(
                "00:00.00",
                "Timer",
                GuiElementPosition.GetAnchors(AnchorPos.TopTopRight),
                new MinMaxRect
                {
                    Min = new float2(-2, 0),
                    Max = new float2(-0.3f, -1)
                },
                 new FontMap(_fontLato, 24),
                (float4)ColorUint.Greenery,
                HorizontalTextAlignment.Right,
                VerticalTextAlignment.Center
            );

            _timertext = timer.GetComponentsInChildren<GuiText>().FirstOrDefault();

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
                    // Simple Texture Node, contains the fusee logo.
                    fuseeLogo,
                    text,
                    timer,
                    instructions
                }
            };

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    new SceneNode()
                    {
                        Name = "GuiCam",
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Translation = new float3(0, 0, 0),
                                Rotation = float3.Zero,
                                Scale = float3.One
                            },
                            new Camera(ProjectionMethod.Orthographic, 0.01f, 500, M.PiOver4)
                            {
                                ClearColor = false
                            }
                        }
                    },
                    // Add canvas.
                    canvas
                }
            };
        }

        // Creates winning display
        private SceneContainer CreateWinningGui()
        {
            var winMap = new FontMap(_fontLato, 55);
            var canvasWidth = Width / 100f;
            var canvasHeight = Height / 100f;

            var btnFuseeLogo = new GuiButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            var fuseeLogo = TextureNode.Create(
                "fuseeLogo",
                //Set the albedo texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                GuiElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                //Define Offset and therefor the size of the element.
                GuiElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f)),
                float2.One
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 24);

            var text = TextNode.Create(
                "FUSEE Labyrinth Example",
                "ButtonText",
                GuiElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                GuiElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                (float4)ColorUint.Greenery,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            var endtime = TextNode.Create(
                "SOLVED\n" +
                _timertext.Text,
                "Timer",
                GuiElementPosition.GetAnchors(AnchorPos.Middle),
                new MinMaxRect
                {
                    Min = new float2(0.01f, 0),
                    Max = new float2(0, 0.01f)
                },
                 winMap,
                (float4)ColorUint.Greenery,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center
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
                    // Simple Texture Node, contains the fusee logo.
                    fuseeLogo,
                    text,
                    endtime
                }
            };

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    new SceneNode()
                    {
                        Name = "GuiCam",
                        Components = new List<SceneComponent>()
                        {
                            new Transform
                            {
                                Translation = float3.Zero,
                                Rotation = float3.Zero,
                                Scale = float3.One
                            },
                            new Camera(ProjectionMethod.Orthographic, 0.01f, 500, M.PiOver4)
                            {
                                ClearColor = false
                            }
                        }
                    },
                    // Add canvas.
                    canvas
                }
            };
        }

        private void BtnLogoEnter(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, (float4)ColorUint.Black);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 0.8f);
        }

        private void BtnLogoExit(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, float4.One);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 1f);
        }

        private void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }

        private void Ballmovement()
        {
            if (_isGameWon) return;

            // Create camera angle
            if (Keyboard.GetKey(KeyCodes.E))
            {
                _isMoving = false;
            }
            else
            {
                _headTransform.Rotate(new float3(0, _angleVelVert, 0));
                _isMoving = true;
            }

            if (_isMoving)
            {
                // Set time for stopwatch
                int minutes = (int)TimeSinceStart / 60;
                var seconds = TimeSinceStart % 59.5f;
                var miliseconds = TimeSinceStart % 0.99f;
                _timertext.Text = minutes.ToString("00") + ":" + seconds.ToString("00") + miliseconds.ToString(".00", new System.Globalization.CultureInfo("en-US"));

                //Get old positions of the head
                _oldX = _headTransform.Translation.x;
                _oldY = _headTransform.Translation.z;

                //Move the ball
                _moveX = Keyboard.WSAxis * _speed * DeltaTime;
                _moveZ = Keyboard.ADAxis * _speed * DeltaTime;

                var headTranslation = _headTransform.Translation;
                headTranslation.x += _moveX * M.Sin(_angle);
                headTranslation.z += _moveX * M.Cos(_angle);
                headTranslation.x += _moveZ * M.Cos(_angle);
                headTranslation.z -= _moveZ * M.Sin(_angle);
                _headTransform.Translation = headTranslation;

                _bodyPivotTransform.RotationQuaternion = Quaternion.Invert(_headTransform.RotationQuaternion);
                _bodyTransform.RotationQuaternion *= _bodyPivotTransform.RotationQuaternion * Quaternion.EulerToQuaternion(new float3(_moveX, 0, -_moveZ)) * Quaternion.Invert(_bodyPivotTransform.RotationQuaternion);
            }
        }

        private void Collision()
        {
            var headTranslation = _headTransform.Translation;

            // Changes the ballbmp when the character moves around
            if (_translation[_ballbmp[0], _ballbmp[1]].x <= headTranslation.x)
            {
                if (_translation[_ballbmp[0], _ballbmp[1]].z >= headTranslation.x)
                {
                    if (_translation[_ballbmp[0], _ballbmp[1]].y <= headTranslation.z)
                    {
                        if (_translation[_ballbmp[0], _ballbmp[1]].w >= headTranslation.z)
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

            // Walls collision
            if (_bmp[_ballbmp[0] - 1, _ballbmp[1]] == 1 || _bmp[_ballbmp[0] - 1, _ballbmp[1]] == 2)
            {
                if (headTranslation.z - _translation[_ballbmp[0] - 1, _ballbmp[1]].w < _ballradius)
                {
                    headTranslation.z = _translation[_ballbmp[0] - 1, _ballbmp[1]].w + _ballradius + 0.0001f;
                }
            }

            if (_bmp[_ballbmp[0] + 1, _ballbmp[1]] == 1 || _bmp[_ballbmp[0] + 1, _ballbmp[1]] == 2)
            {
                if (_translation[_ballbmp[0] + 1, _ballbmp[1]].y - headTranslation.z < _ballradius)
                {
                    headTranslation.z = _translation[_ballbmp[0] + 1, _ballbmp[1]].y - _ballradius - 0.0001f;
                }
            }

            if (_bmp[_ballbmp[0], _ballbmp[1] - 1] == 1 || _bmp[_ballbmp[0], _ballbmp[1] - 1] == 2)
            {
                if (headTranslation.x - _translation[_ballbmp[0], _ballbmp[1] - 1].z < _ballradius)
                {
                    headTranslation.x = _translation[_ballbmp[0], _ballbmp[1] - 1].z + _ballradius + 0.0001f;
                }
            }

            if (_bmp[_ballbmp[0], _ballbmp[1] + 1] == 1 || _bmp[_ballbmp[0], _ballbmp[1] + 1] == 2)
            {
                if (_translation[_ballbmp[0], _ballbmp[1] + 1].x - headTranslation.x < _ballradius)
                {
                    headTranslation.x = _translation[_ballbmp[0], _ballbmp[1] + 1].x - _ballradius - 0.0001f;
                }
            }

            // Corners collision
            if (_bmp[_ballbmp[0] - 1, _ballbmp[1] - 1] == 1)
            {
                if (System.MathF.Sqrt((headTranslation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].w) * (headTranslation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].w) + (headTranslation.x - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].z) * (headTranslation.x - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].z)) < _ballradius)
                {
                    if (headTranslation.x < _oldX)
                    {
                        headTranslation.z = _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].w + System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (headTranslation.x - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].z) * (headTranslation.x - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].z)));
                    }

                    if (headTranslation.z < _oldY)
                    {
                        headTranslation.x = _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].z + System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (headTranslation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].w) * (headTranslation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] - 1].w)));
                    }
                }
            }

            if (_bmp[_ballbmp[0] - 1, _ballbmp[1] + 1] == 1)
            {
                if (System.MathF.Sqrt((headTranslation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].w) * (headTranslation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].w) + (_translation[_ballbmp[0] - 1, _ballbmp[1] + 1].x - headTranslation.x) * (_translation[_ballbmp[0] - 1, _ballbmp[1] + 1].x - headTranslation.x)) < _ballradius)
                {
                    if (headTranslation.x > _oldX)
                    {
                        headTranslation.z = _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].w + System.MathF.Sqrt((((_ballradius + 0.001f) * (_ballradius + 0.001f)) - (_translation[_ballbmp[0] - 1, _ballbmp[1] + 1].x - headTranslation.x) * (_translation[_ballbmp[0] - 1, _ballbmp[1] + 1].x - headTranslation.x)));
                    }

                    if (headTranslation.z < _oldY)
                    {
                        headTranslation.x = _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].x - System.MathF.Sqrt((((_ballradius + 0.001f) * (_ballradius + 0.001f)) - (headTranslation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].w) * (headTranslation.z - _translation[_ballbmp[0] - 1, _ballbmp[1] + 1].w)));
                    }
                }
            }

            if (_bmp[_ballbmp[0] + 1, _ballbmp[1] - 1] == 1)
            {
                if (System.MathF.Sqrt((_translation[_ballbmp[0] + 1, _ballbmp[1] - 1].y - headTranslation.z) * (_translation[_ballbmp[0] + 1, _ballbmp[1] - 1].y - headTranslation.z) + (headTranslation.x - _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].z) * (headTranslation.x - _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].z)) < _ballradius)
                {
                    if (headTranslation.x < _oldX)
                    {
                        headTranslation.z = _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].y - System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (headTranslation.x - _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].z) * (headTranslation.x - _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].z)));
                    }

                    if (headTranslation.z > _oldY)
                    {
                        headTranslation.x = _translation[_ballbmp[0] + 1, _ballbmp[1] - 1].z + System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (_translation[_ballbmp[0] + 1, _ballbmp[1] - 1].y - headTranslation.z) * (_translation[_ballbmp[0] + 1, _ballbmp[1] - 1].y - headTranslation.z)));
                    }
                }
            }

            if (_bmp[_ballbmp[0] + 1, _ballbmp[1] + 1] == 1)
            {
                if (System.MathF.Sqrt((_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].y - headTranslation.z) * (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].y - headTranslation.z) + (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].x - headTranslation.x) * (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].x - headTranslation.x)) < _ballradius)
                {
                    if (headTranslation.x > _oldX)
                    {
                        headTranslation.z = _translation[_ballbmp[0] + 1, _ballbmp[1] + 1].y - System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].x - headTranslation.x) * (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].x - headTranslation.x)));
                    }

                    if (headTranslation.z > _oldY)
                    {
                        headTranslation.x = _translation[_ballbmp[0] + 1, _ballbmp[1] + 1].x - System.MathF.Sqrt((((_ballradius + 0.01f) * (_ballradius + 0.01f)) - (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].y - headTranslation.z) * (_translation[_ballbmp[0] + 1, _ballbmp[1] + 1].y - headTranslation.z)));
                    }
                }
            }

            // Goal collision
            if (_bmp[_ballbmp[0] - 1, _ballbmp[1]] == 3)
            {
                if (headTranslation.z - _translation[_ballbmp[0] - 1, _ballbmp[1]].w < _ballradius)
                {
                    _isMoving = false;
                    _isGameWon = true;
                }
            }

            if (_bmp[_ballbmp[0] + 1, _ballbmp[1]] == 3)
            {
                if (_translation[_ballbmp[0] + 1, _ballbmp[1]].y - headTranslation.z < _ballradius)
                {
                    _isMoving = false;
                    _isGameWon = true;
                }
            }

            if (_bmp[_ballbmp[0], _ballbmp[1] - 1] == 3)
            {
                if (headTranslation.x - _translation[_ballbmp[0], _ballbmp[1] - 1].z < _ballradius)
                {
                    _isMoving = false;
                    _isGameWon = true;
                }
            }

            if (_bmp[_ballbmp[0], _ballbmp[1] + 1] == 3)
            {
                if (_translation[_ballbmp[0], _ballbmp[1] + 1].x - headTranslation.x < _ballradius)
                {
                    _isMoving = false;
                    _isGameWon = true;
                }
            }

            _headTransform.Translation = headTranslation;
        }

        private void OnWonGame()
        {
            if (_isGameWon)
            {
                // Create the winning display once
                if (!_isGuiCreated)
                {
                    _gui = CreateWinningGui();
                    _sih = new SceneInteractionHandler(_gui);
                    _guiRenderer = new SceneRendererForward(_gui);
                    _isGuiCreated = true;
                }

                _overviewCam.Active = true;
                _mainCam.Active = false;
                _overViewCamNode.Rotate(new float3(0, 0.2f * M.Pi / 180, 0), SceneExtensions.Space.World);
            }
        }

        private void MakeBox()
        {
            SceneContainer mazeScene = AssetStorage.Get<SceneContainer>("mazeAsset.fus");

            var cornerstone = mazeScene.Children.FindNodes(node => node.Name == "Cornerstone")?.FirstOrDefault()?.GetMesh();
            _cornerbox = cornerstone.BoundingBox.Size.xyz;

            var wallZ = mazeScene.Children.FindNodes(node => node.Name == "WallZ")?.FirstOrDefault()?.GetMesh();
            _wallZbox = wallZ.BoundingBox.Size.xyz;

            var wallX = mazeScene.Children.FindNodes(node => node.Name == "WallX")?.FirstOrDefault()?.GetMesh();
            _wallXbox = wallX.BoundingBox.Size.xyz;

            var ball = mazeScene.Children.FindNodes(node => node.Name == "Body")?.FirstOrDefault()?.GetMesh();
            _ballradius = ball.BoundingBox.Size.x / 2;

            _groundbox = new float2(_wallXbox.x, _wallZbox.z);
        }

        // Create an Array with the positions
        private void CreatePositions()
        {
            for (int countY = 0; countY < _bmp.GetLength(0); countY++)
            {
                for (int countX = 0; countX < _bmp.GetLength(1); countX++)
                {
                    if (countX % 2 == 0 && countY % 2 == 0)
                    {
                        _translation[countY, countX] = new float4((countX * (_wallXbox.x + _cornerbox.x) / 2) - _cornerbox.x / 2, (countY * (_wallZbox.z + _cornerbox.z) / 2) - _cornerbox.z / 2, (countX * (_wallXbox.x + _cornerbox.x) / 2) + _cornerbox.x / 2, (countY * (_wallZbox.z + _cornerbox.z) / 2) + _cornerbox.z / 2);
                    }

                    if (countX % 2 == 0 && countY % 2 == 1)
                    {
                        _translation[countY, countX] = new float4((countX * (_wallXbox.x + _cornerbox.x) / 2) - _wallZbox.x / 2, (countY * (_wallZbox.z + _cornerbox.z) / 2) - _wallZbox.z / 2, (countX * (_wallXbox.x + _cornerbox.x) / 2) + _wallZbox.x / 2, (countY * (_wallZbox.z + _cornerbox.z) / 2) + _wallZbox.z / 2);
                    }

                    if (countX % 2 == 1 && countY % 2 == 0)
                    {
                        _translation[countY, countX] = new float4((countX * (_wallXbox.x + _cornerbox.x) / 2) - _wallXbox.x / 2, (countY * (_wallZbox.z + _cornerbox.z) / 2) - _wallXbox.z / 2, (countX * (_wallXbox.x + _cornerbox.x) / 2) + _wallXbox.x / 2, (countY * (_wallZbox.z + _cornerbox.z) / 2) + _wallXbox.z / 2);
                    }

                    if (countX % 2 == 1 && countY % 2 == 1)
                    {
                        _translation[countY, countX] = new float4((countX * (_wallXbox.x + _cornerbox.x) / 2) - _groundbox.x / 2, (countY * (_wallZbox.z + _cornerbox.z) / 2) - _groundbox.y / 2, (countX * (_wallXbox.x + _cornerbox.x) / 2) + _groundbox.x / 2, (countY * (_wallZbox.z + _cornerbox.z) / 2) + _groundbox.y / 2);
                    }
                }
            }
        }

        // Finds the start point of the character
        private void FindBall()
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

        // Creates the Maze out of a Image
        private static int[,] Bmp()
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
                    if (GetPixel(img, j, i).Equals(new ColorUint(0, 0, 0, 255)))
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

        // Checks the all Pixel from the Image and orders them in ARB 2D-Array
        private static Color[,] Imageorder(byte[] arr, int width, int height)
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

        // Gets one defined Pixel ARGB/RGB/Intensity color
        private static ColorUint GetPixel(ImageData img, int x, int y)
        {
            return img.PixelFormat switch
            {
                { BytesPerPixel: 4, ColorFormat: ColorFormat.RGBA } => new ColorUint(img.PixelData[4 * (img.Width * y + x) + 2], img.PixelData[4 * (img.Width * y + x) + 1], img.PixelData[4 * (img.Width * y + x)], img.PixelData[4 * (img.Width * y + x) + 3]),
                { BytesPerPixel: 3, ColorFormat: ColorFormat.RGB } => new ColorUint(img.PixelData[3 * (img.Width * y + x) + 2], img.PixelData[3 * (img.Width * y + x) + 1], img.PixelData[3 * (img.Width * y + x)], 255),
                { BytesPerPixel: 1, ColorFormat: ColorFormat.Intensity } => new ColorUint(0, 0, 0, img.PixelData[1 * (img.Width * y + x) + 3]),
                _ => new ColorUint(0, 0, 0, 0)
            };
        }
    }
}