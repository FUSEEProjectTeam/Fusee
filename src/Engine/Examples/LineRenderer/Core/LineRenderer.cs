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

namespace Fusee.Engine.Examples.LineRenderer.Core
{
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class LineRenderer : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = 0, _angleVert = 0, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;

        private SceneRenderer _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private float _initWidth;
        private CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;

        private const int _canvasWidth = 16;
        private const int _canvasHeight = 9;

        private float _aspectRatio;
        private float _fovy = M.PiOver4;

        private bool _keys;

        private List<float3> _dummyPositions;
        private List<float2> _canvasPositions;

        private SceneContainer BuildScene()
        {
            var sphere = new Sphere(32, 24);

            var lineControlPoints = new float3[]
            {
                new float3(-0.5f,0,0),new float3(0,0,0),new float3(.5f,.5f,0)
            };
            var line = new Line(1, 1, lineControlPoints, 0.2f);

            var rnd = new Random();

            for (var i = 0; i < 4; i++)
            {
                var vertIndex = rnd.Next(sphere.Vertices.Length);
                _dummyPositions.Add(sphere.Vertices[vertIndex]);
                _canvasPositions.Add(new float2(sphere.Vertices[vertIndex].x, sphere.Vertices[vertIndex].y));
            }
            
            return new SceneContainer()
            {
                Children = new List<SceneNodeContainer>()
                {
                    new SceneNodeContainer()
                    {
                        Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent()
                            {
                                Name = "SphereTransform",
                                Rotation = new float3(0,0,0),
                                Translation = new float3(0,0,0),
                                Scale = new float3(1, 1, 1)

                            },
                            new ShaderEffectComponent()
                            {
                                Effect = ShaderCodeBuilder.MakeShaderEffect(new float3(0.90980f, 0.35686f, 0.35686f), new float3(1,1,1), 20,"crumpled-paper-free.jpg",0.5f)
                            },
                            //sphere
                        }
                    },
                    new SceneNodeContainer()
                    {
                        Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent()
                            {
                                Name = "LineTransform",
                                Rotation = new float3(0,0,0),
                                Translation = new float3(0,0,0),
                                Scale = new float3(1, 1, 1)

                            },
                            new ShaderEffectComponent()
                            {
                                Effect = ShaderCodeBuilder.MakeShaderEffect(new float3(0, 0, 1), new float3(1,1,1), 20)
                            },
                            line
                        }
                    }
                }
            };
        }

        // Init is called on startup. 
        public override void Init()
        {
            _dummyPositions = new List<float3>();
            _canvasPositions = new List<float2>();

            _initWidth = Width;
            _aspectRatio = Width / (float)Height;

            _scene = BuildScene();
            //_scene = AssetStorage.Get<SceneContainer>("Monkey.fus");
            //var monkey = _scene.Children[0].GetComponent<Mesh>();
            //var rnd = new Random();

            //for (var i = 0; i < 4; i++)
            //{
            //    var vertIndex = rnd.Next(monkey.Vertices.Length);
            //    _dummyPositions.Add(monkey.Vertices[vertIndex]);
            //    _canvasPositions.Add(new float2(monkey.Vertices[vertIndex].x, monkey.Vertices[vertIndex].y));
            //}

            _gui = CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.1f, 0.1f, 0.1f, 1);            
            
            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_scene);
            _guiRenderer = new SceneRenderer(_gui);
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

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 0, -5, 0, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRot;

            //Set the view matrix for the interaction handler.
            _sih.View = RC.ModelView;

            // Constantly check for interactive objects.
            _sih.CheckForInteractiveObjects(Input.Mouse.Position, Width, Height);

            if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Input.Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Input.Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            var projection = float4x4.CreatePerspectiveFieldOfView(_fovy, _aspectRatio, 1, 20000);
            RC.Projection = projection;

            //UPDATE POS
            //-----------------------------------------
            var circleDim = new float2(0.65f, 0.65f);

            for (int i = 0; i < _dummyPositions.Count; i++)
            {
                var clipPos = (_dummyPositions[i] * RC.ModelViewProjection);

                var pos = new float2(clipPos.x, clipPos.y)*0.5f + 0.5f;
                pos.x *= _canvasWidth;
                pos.y *= _canvasHeight;
                _canvasPositions[i] = pos;
            }

            var canvas = _gui.Children[0];
            var circleCount = 0;
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                SceneNodeContainer child = canvas.Children[i];
                if (child.Name.Contains("Circle"))
                {
                    var pos = new float2(_canvasPositions[circleCount].x - (circleDim.x / 2), _canvasPositions[circleCount].y - (circleDim.y / 2)); //we want the lower left point of the rect that encloses the
                    child.GetComponent<RectTransformComponent>().Offsets = CalcOffsets(AnchorPos.MIDDLE, pos, _canvasHeight, _canvasWidth, circleDim);
                    circleCount++;
                }
            }                      

            if (_canvasRenderMode == CanvasRenderMode.SCREEN)
            {           
                // Render the scene loaded in Init()
                _sceneRenderer.Render(RC);

                projection = float4x4.CreateOrthographic(Width, Height, 1, 20000);
                RC.Projection = projection;
                _sih.Projection = projection;

                _guiRenderer.Render(RC);
            }
            else
            {
                _sceneRenderer.Render(RC);
                _guiRenderer.Render(RC);
            }

            // Swap buffers: Show the contents of the back buffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            _aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(_fovy, _aspectRatio, 1, 20000);
            RC.Projection = projection;
            _sih.Projection = projection;
        }

        private enum AnchorPos
        {
            DOWN_DOWN_LEFT,     //Min = Max = 0,0
            DOWN_DOWN_RIGHT,    //Min = Max = 0,1
            TOP_TOP_LEFT,       //Min = Max = 0,1
            TOP_TOP_RIGHT,      //Min = Max = 1,1
            STRETCH_ALL,        //Min 0, 0 and Max 1, 1
            MIDDLE              //Min = Max = 0.5, 0.5
        }

        private MinMaxRect CalcOffsets(AnchorPos anchorPos, float2 posOnParent, float parentHeight, float parentWidth, float2 guiElementDim)
        {
            switch (anchorPos)
            {
                default:
                case AnchorPos.MIDDLE:
                    var middle = new float2(parentWidth / 2f, parentHeight / 2f);
                    return new MinMaxRect
                    {
                        //only for the anchors Min 0.5,0.5 and Max 0.5,0.5!!!
                        Min = posOnParent - middle,
                        Max = posOnParent - middle + guiElementDim
                    };

                case AnchorPos.STRETCH_ALL:
                    return new MinMaxRect
                    {
                        //only for the anchors Min 0,0 and Max 1,1!!!
                        Min = new float2(posOnParent.x, posOnParent.y),
                        Max = new float2(-(parentWidth - posOnParent.x - guiElementDim.x), -(parentHeight - posOnParent.y - guiElementDim.y))
                    };
                case AnchorPos.DOWN_DOWN_LEFT:
                    return new MinMaxRect
                    {
                        //only for the anchors Min 0,0 and Max 0,0!!!
                        Min = new float2(posOnParent.x, posOnParent.y),
                        Max = new float2(posOnParent.x + guiElementDim.x, posOnParent.y + guiElementDim.y)
                    };
                case AnchorPos.DOWN_DOWN_RIGHT:
                    return new MinMaxRect
                    {
                        //only for the anchors Min 1,0 and Max 1,0!!!
                        Min = new float2(-(parentWidth - posOnParent.x), posOnParent.y),
                        Max = new float2(-(parentWidth - posOnParent.x - guiElementDim.x), posOnParent.y + guiElementDim.y)
                    };
                case AnchorPos.TOP_TOP_LEFT:
                    return new MinMaxRect
                    {
                        //only for the anchors Min 0,1 and Max 0,1!!!
                        Min = new float2(posOnParent.x, -(parentHeight - posOnParent.y)),
                        Max = new float2(posOnParent.x + guiElementDim.x, -(parentHeight - guiElementDim.y - posOnParent.y))
                    };
                case AnchorPos.TOP_TOP_RIGHT:
                    return new MinMaxRect
                    {
                        //only for the anchors Min 1,1 and Max 1,1!!!
                        Min = new float2(-(parentWidth - posOnParent.x), -(parentHeight - posOnParent.y)),
                        Max = new float2(-(parentWidth - guiElementDim.x - posOnParent.x), -(parentHeight - guiElementDim.y - posOnParent.y))
                    };
            }
        }

        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");
            var vsNineSlice = AssetStorage.Get<string>("nineSlice.vert");
            var psNineSlice = AssetStorage.Get<string>("nineSliceTile.frag");
            
            var fontRaleway = AssetStorage.Get<Font>("Raleway-Regular.ttf");
            var ralewayFontMap = new FontMap(fontRaleway, 12);

            var canvasScaleFactor = _canvasWidth / _initWidth;
            float textSize = 2;
            float borderScaleFactor = 1;
            if (_canvasRenderMode == CanvasRenderMode.SCREEN)
            {
                textSize /= canvasScaleFactor;
                borderScaleFactor = canvasScaleFactor;
            }

            var btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var annotationDim = new float2(3f, 0.5f);
            var annotationBorderScale = new float4(6, 0.8f, 0.8f, 0.8f);

            //posOnParent = min offset = lower left corner of the rect transform
            var posGreen = new float2(1, 2);
            var posYellow = new float2(1, 3);
            var posGray = new float2(1, 4);
            var posFilled = new float2(1, 5);

            #region green annotation

            var iconCheckCircle = new TextureNodeContainer(
                "iconCheck",
                vsTex,
                psTex,
                new Texture(AssetStorage.Get<ImageData>("check-circle.png")),
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.07f, 0.07f), annotationDim.y, annotationDim.x, new float2(0.35f, 0.35f))
            );

            var textAnnotationGreen = new TextNodeContainer(
                "#1 Abcdefgh, 1.234",
                "annotation text",
                vsTex,
                psTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.5f, 0.07f), annotationDim.y, annotationDim.x, new float2(2.5f, 0.35f)),
                ralewayFontMap,
                ColorUint.Tofloat4(ColorUint.Black), textSize);


            var annotationGreen = new TextureNodeContainer(
                "AnnotationGreen",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("frame_green.png")),
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(0, 0)
                },
                CalcOffsets(AnchorPos.DOWN_DOWN_LEFT, posGreen, _canvasHeight, _canvasWidth, annotationDim),
                new float2(1, 1),
                new float4(0.09f, 0.09f, 0.09f, 0.09f),
                annotationBorderScale.x, annotationBorderScale.y, annotationBorderScale.z, annotationBorderScale.w,
                borderScaleFactor

            )
            {
                Children = new List<SceneNodeContainer>
                {
                    textAnnotationGreen,
                    iconCheckCircle
                }
            };
            #endregion

            #region green filled annotation

            var iconCheckCircleFilled = new TextureNodeContainer(
                "iconCheck",
                vsTex,
                psTex,
                new Texture(AssetStorage.Get<ImageData>("check-circle_filled.png")),
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.07f, 0.07f), annotationDim.y, annotationDim.x, new float2(0.35f, 0.35f))
            );

            var textAnnotationGreenFilled = new TextNodeContainer(
                "#4 Abcdefgh",
                "annotation text",
                vsTex,
                psTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.5f, 0.07f), annotationDim.y, annotationDim.x, new float2(2.5f, 0.35f)),
                ralewayFontMap,
                ColorUint.Tofloat4(ColorUint.Black), textSize * 0.65f);


            var annotationGreenFilled = new TextureNodeContainer(
                "AnnotationGreen",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("frame_green.png")),
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(0, 0)
                },
                CalcOffsets(AnchorPos.DOWN_DOWN_LEFT, posFilled, _canvasHeight, _canvasWidth, annotationDim),
                new float2(1, 1),
                new float4(0.09f, 0.09f, 0.09f, 0.09f),
                annotationBorderScale.x, annotationBorderScale.y, annotationBorderScale.z, annotationBorderScale.w,
                borderScaleFactor

            )
            {
                Children = new List<SceneNodeContainer>
                {
                    textAnnotationGreenFilled,
                    iconCheckCircleFilled
                }
            };
            #endregion

            #region yellow annotation

            var iconBulb = new TextureNodeContainer(
                "iconBulb",
                vsTex,
                psTex,
                new Texture(AssetStorage.Get<ImageData>("lightbulb.png")),
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.07f, 0.07f), annotationDim.y, annotationDim.x, new float2(0.35f, 0.35f))
            );

            var textAnnotationYellow = new TextNodeContainer(
                "#2 Abcde, 1.234",
                "annotation text",
                vsTex,
                psTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.5f, 0.07f), annotationDim.y, annotationDim.x, new float2(2.5f, 0.35f)),
                ralewayFontMap,
                ColorUint.Tofloat4(ColorUint.Black), textSize * 0.85f);


            var annotationYellow = new TextureNodeContainer(
                "AnnotationYellow",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("frame_yellow.png")),
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(0, 0)
                },
                CalcOffsets(AnchorPos.DOWN_DOWN_LEFT, posYellow, _canvasHeight, _canvasWidth, annotationDim),
                new float2(1, 1),
                new float4(0.09f, 0.09f, 0.09f, 0.09f),
                annotationBorderScale.x, annotationBorderScale.y, annotationBorderScale.z, annotationBorderScale.w,
                borderScaleFactor

            )
            {
                Children = new List<SceneNodeContainer>
                {
                    textAnnotationYellow,
                    iconBulb
                }
            };
            #endregion

            #region gray annotation

            var iconOctaMin = new TextureNodeContainer(
                "iconBulb",
                vsTex,
                psTex,
                new Texture(AssetStorage.Get<ImageData>("minus-oktagon.png")),
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.07f, 0.07f), annotationDim.y, annotationDim.x, new float2(0.35f, 0.35f))
            );

            var textAnnotationGray = new TextNodeContainer(
                "#3 Abcdefgh, 1.234",
                "annotation text",
                vsTex,
                psTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.5f, 0.07f), annotationDim.y, annotationDim.x, new float2(2.5f, 0.35f)),
                ralewayFontMap,
                ColorUint.Tofloat4(ColorUint.Black), textSize);


            var annotationGray = new TextureNodeContainer(
                "AnnotationGray",
                vsNineSlice,
                psNineSlice,
                new Texture(AssetStorage.Get<ImageData>("frame_gray.png")),
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(0, 0)
                },
                CalcOffsets(AnchorPos.DOWN_DOWN_LEFT, posGray, _canvasHeight, _canvasWidth, annotationDim),
                new float2(1, 1),
                new float4(0.09f, 0.09f, 0.09f, 0.09f),
                annotationBorderScale.x, annotationBorderScale.y, annotationBorderScale.z, annotationBorderScale.w,
                borderScaleFactor

            )
            {
                Children = new List<SceneNodeContainer>
                {
                    textAnnotationGray,
                    iconOctaMin
                }
            };
            #endregion

            #region circles

            var circleGreen = new SceneNodeContainer
            {
                Name = "CircleGreen",
                Components = new List<SceneComponentContainer>
                {
                    new RectTransformComponent
                    {
                        Name = "circle" + "_RectTransform",
                        Anchors = new MinMaxRect
                        {
                            Min = new float2(0.5f, 0.5f),
                            Max = new float2(0.5f, 0.5f)
                        },
                        Offsets = CalcOffsets(AnchorPos.MIDDLE, new float2(0,0), _canvasHeight, _canvasWidth, new float2(0.65f,0.65f)),
                    },
                    new XFormComponent
                    {
                        Name = "circle" + "_XForm",
                    },
                    new ShaderEffectComponent()
                    {
                        Effect = ShaderCodeBuilder.MakeShaderEffect(new float3(0.14117f, 0.76078f, 0.48627f), new float3(1,1,1), 20, 0)
                    },
                    new Circle(false, 30,100,0.04f)
                }
            };

            var circleGreenFilled = new SceneNodeContainer
            {
                Name = "CircleGreenFilled",
                Components = new List<SceneComponentContainer>
                {
                    new RectTransformComponent
                    {
                        Name = "circle" + "_RectTransform",
                        Anchors = new MinMaxRect
                        {
                            Min = new float2(0.5f, 0.5f),
                            Max = new float2(0.5f, 0.5f)
                        },
                        Offsets = CalcOffsets(AnchorPos.MIDDLE, new float2(0,0), _canvasHeight, _canvasWidth, new float2(0.65f,0.65f)),
                    },
                    new XFormComponent
                    {
                        Name = "circle" + "_XForm",
                    },
                    new ShaderEffectComponent()
                    {
                        Effect = ShaderCodeBuilder.MakeShaderEffect(new float3(0.14117f, 0.76078f, 0.48627f), new float3(1,1,1), 20, 0)
                    },
                    new Circle(false, 30,100,0.04f)
                }
            };

            var circleYellow = new SceneNodeContainer
            {
                Name = "CircleYellow",
                Components = new List<SceneComponentContainer>
                {
                    new RectTransformComponent
                    {
                        Name = "circle" + "_RectTransform",
                        Anchors = new MinMaxRect
                        {
                            Min = new float2(0.5f, 0.5f),
                            Max = new float2(0.5f, 0.5f)
                        },
                        Offsets = CalcOffsets(AnchorPos.MIDDLE, new float2(0,0), _canvasHeight, _canvasWidth, new float2(0.65f,0.65f)),
                    },
                    new XFormComponent
                    {
                        Name = "circle" + "_XForm",
                    },
                    new ShaderEffectComponent()
                    {
                        Effect = ShaderCodeBuilder.MakeShaderEffect(new float3(0.89411f, 0.63137f, 0.31372f), new float3(1,1,1), 20, 0)
                    },
                    new Circle(false, 30,100,0.04f)
                }
            };

            var circleGray = new SceneNodeContainer
            {
                Name = "CircleGray",
                Components = new List<SceneComponentContainer>
                {
                    new RectTransformComponent
                    {
                        Name = "circle" + "_RectTransform",
                        Anchors = new MinMaxRect
                        {
                            Min = new float2(0.5f, 0.5f),
                            Max = new float2(0.5f, 0.5f)
                        },
                        Offsets = CalcOffsets(AnchorPos.MIDDLE, new float2(0,0), _canvasHeight, _canvasWidth, new float2(0.65f,0.65f)),
                    },
                    new XFormComponent
                    {
                        Name = "circle" + "_XForm",
                    },
                    new ShaderEffectComponent()
                    {
                        Effect = ShaderCodeBuilder.MakeShaderEffect(new float3(0.47843f, 0.52549f, 0.54901f), new float3(1,1,1), 20, 0)
                    },
                    new Circle(false, 30,100,0.04f)
                }
            };

            #endregion


            var guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            var fuseeLogo = new TextureNodeContainer(
                "fuseeLogo",
                vsTex,
                psTex,
                guiFuseeLogo,
                new MinMaxRect
                {
                    Min = new float2(0, 1),
                    Max = new float2(0, 1)
                },
                CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(0, _canvasHeight-0.5f), _canvasHeight, _canvasWidth, new float2(1.75f, 0.5f)));
            fuseeLogo.AddComponent(btnFuseeLogo);

            var canvas = new CanvasNodeContainer(
                "Canvas",
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-_canvasWidth / 2f, -_canvasHeight / 2f),
                    Max = new float2(_canvasWidth / 2f, _canvasHeight / 2f)
                })
            {
                Children = new List<SceneNodeContainer>()
                {
                    fuseeLogo,
                    annotationGreen,
                    annotationGreenFilled,
                    annotationYellow,
                    annotationGray,
                    circleGreen,
                    circleYellow,
                    circleGreenFilled,
                    circleGray
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

    }
}