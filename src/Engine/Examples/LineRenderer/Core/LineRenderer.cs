using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Examples.LineRenderer.Core
{
    [FuseeApplication(Name = "FUSEE UI Example", Description = " ")]
    public class LineRenderer : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;
        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;
        private bool _keys;

        public int NumberOfAnnotations = 4;

        private const float ZNear = 1f;
        private const float ZFar = 1000;

        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;

        private SceneRenderer _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private float _initWidth;
        private float _initHeight;
        private float2 _resizeScaleFactor;
        private CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;


        private float _canvasWidth;
        private float _canvasHeight;
        private float _canvasScaleFactor;

        private float _aspectRatio;
        private float _fovy = M.PiOver4;

        private float3[] _dummyPositions;
        private int[] _dummyPosTriangleIndex;
        private bool[] _isCircleVisible;
        private float2[] _circleCanvasPositions;
        private float2[] _circleCanvasPositionsCache;

        private float2[] _annotationCanvasPositions;

        private ScenePicker _scenePicker;

        private SceneContainer BuildScene()
        {
            var sphere = new Sphere(32, 24);

            var lineControlPoints = new float3[]
            {
                new float3(-3f,0,0) , new float3(-1.5f,-1.5f,0), new float3(1f,1.5f,0)
            };
            var line = new Line(lineControlPoints, 0.2f);

            var rnd = new Random();

            for (var i = 0; i < NumberOfAnnotations; i++)
            {
                var vertIndex = rnd.Next(sphere.Vertices.Length);
                _dummyPositions[i] = (sphere.Vertices[vertIndex]);
                _circleCanvasPositions[i] = (new float2(sphere.Vertices[vertIndex].x, sphere.Vertices[vertIndex].y));
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
            _canvasHeight = GuiHelper.CanvasHeightInit;
            _canvasWidth = GuiHelper.CanvasWidthInit;

            _isCircleVisible = new bool[NumberOfAnnotations];
            _dummyPositions = new float3[NumberOfAnnotations];
            _dummyPosTriangleIndex = new int[NumberOfAnnotations];
            _circleCanvasPositions = new float2[NumberOfAnnotations];
            _circleCanvasPositionsCache = new float2[NumberOfAnnotations];

            _annotationCanvasPositions = new[]
            {
                // Input position for GUIHelper.CreateAnnotation = min offset = lower left corner of the rect transform. 
                // Range: [GUIHelper.CanvasWidthInit, GUIHelper.CanvasHeightInit]
                // Will be updated in RenderAFrame
                new float2(1, 2),
                new float2(1, 3),
                new float2(1, 4),
                new float2(1, 5)
            };

            _initWidth = Width;
            _initHeight = Height;
            _aspectRatio = Width / (float)Height;

            //_scene = BuildScene();
            _scene = AssetStorage.Get<SceneContainer>("Monkey.fus");
            var monkey = _scene.Children[0].GetComponent<Mesh>();
            var rnd = new Random();

            //Create dummy positions on model
            for (var i = 0; i < NumberOfAnnotations; i++)
            {
                var numberOfTriangles = monkey.Triangles.Length / 3;

                var triangleNumber = rnd.Next(1, numberOfTriangles);
                var triIndex = (triangleNumber - 1) * 3;

                _dummyPosTriangleIndex[i] = triIndex;
                var triVert0 = monkey.Vertices[triIndex];
                var triVert1 = monkey.Vertices[triIndex + 1];
                var triVert2 = monkey.Vertices[triIndex + 2];

                var middle = (triVert0 + triVert1 + triVert2) / 3;

                _dummyPositions[i] = middle;
                _circleCanvasPositions[i] = new float2(middle.x, middle.y);
                _circleCanvasPositionsCache[i] = new float2(middle.x, middle.y);
            }

            _gui = CreateGui();

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            //Create a scene picker for performing visibility tests
            _scenePicker = new ScenePicker(_scene);

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

            //TODO: set screenspace UI projection to orthographic in SceneRenderer
            var projection = float4x4.CreatePerspectiveFieldOfView(_fovy, _aspectRatio, ZNear, ZFar);
            RC.Projection = projection;

            #region Controls
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
            _scenePicker.View = RC.ModelView;

            // Constantly check for interactive objects.
            _sih.CheckForInteractiveObjects(Input.Mouse.Position, Width, Height);

            if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Input.Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Input.Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }
            #endregion

            #region Update UI elements
            //Annotations will be unpdated according to circle positions.
            //Lines will be updated according to circle and annotation positions.

            var circleDim = new float2(0.65f, 0.65f);
            var canvas = _gui.Children[0];
            var circleCount = 0;
            var lineCount = 0;
            var annotationCount = 0;

            foreach (var child in canvas.Children)
            {
                if (child.Name.Contains("Circle"))
                {
                    //1. Update Circle pos
                    var clipPos = _dummyPositions[circleCount].TransformPerspective(RC.ModelViewProjection); //divides by w
                    var circleCanvasPos = new float2(clipPos.x, clipPos.y) * 0.5f + 0.5f;

                    circleCanvasPos.x *= _canvasWidth;
                    circleCanvasPos.y *= _canvasHeight;
                    _circleCanvasPositions[circleCount] = circleCanvasPos;

                    var pos = new float2(_circleCanvasPositions[circleCount].x - (circleDim.x / 2), _circleCanvasPositions[circleCount].y - (circleDim.y / 2)); //we want the lower left point of the rect that encloses the
                    child.GetComponent<RectTransformComponent>().Offsets = GuiHelper.CalcOffsets(GuiHelper.AnchorPos.MIDDLE, pos, _canvasHeight, _canvasWidth, circleDim);

                    //2. Check if circle is visible
                    var newPick = _scenePicker.Pick(new float2(clipPos.x, clipPos.y)).ToList().OrderBy(pr => pr.ClipPos.z).FirstOrDefault();

                    var col = GuiHelper.MatColor.WHITE;

                    if (_dummyPosTriangleIndex[circleCount] == newPick.Triangle) //VISIBLE
                    {
                        _isCircleVisible[circleCount] = true;

                        if (child.Name.Contains("green"))
                            col = GuiHelper.MatColor.GREEN;

                        else if (child.Name.Contains("yellow"))
                            col = GuiHelper.MatColor.YELLOW;

                        if (child.Name.Contains("gray"))
                            col = GuiHelper.MatColor.GRAY;

                        child.GetComponent<ShaderEffectComponent>().Effect = GuiHelper.GetShaderEffectFromMatColor(col);
                    }
                    else
                    {
                        _isCircleVisible[circleCount] = false;
                        col = GuiHelper.MatColor.WHITE;
                        child.GetComponent<ShaderEffectComponent>().Effect = GuiHelper.GetShaderEffectFromMatColor(col);
                    }
                    circleCount++;
                }
                else if (child.Name.Contains("Annotation"))
                {
                    if (_isCircleVisible[annotationCount])
                    {
                        child.GetComponent<NineSlicePlane>().Active = true;
                        foreach (var comp in child.GetComponentsInChildren<Mesh>())
                            comp.Active = true;

                        _annotationCanvasPositions[annotationCount].y = _circleCanvasPositions[annotationCount].y;
                        if (_circleCanvasPositions[annotationCount].x <= _canvasWidth / 2)
                        {
                            //LEFT
                            _annotationCanvasPositions[annotationCount].x = GuiHelper.AnnotationDistToLeftOrRightEdge;

                            child.GetComponent<RectTransformComponent>().Anchors = new MinMaxRect
                            {
                                Min = new float2(0, 0),
                                Max = new float2(0, 0)
                            };
                            child.GetComponent<RectTransformComponent>().Offsets = GuiHelper.CalcOffsets(
                                GuiHelper.AnchorPos.DOWN_DOWN_LEFT, _annotationCanvasPositions[annotationCount],
                                GuiHelper.CanvasHeightInit, GuiHelper.CanvasWidthInit, GuiHelper.AnnotationDim);
                        }
                        else
                        {
                            //RIGHT
                            _annotationCanvasPositions[annotationCount].x =
                                GuiHelper.CanvasWidthInit - GuiHelper.AnnotationDim.x -
                                GuiHelper.AnnotationDistToLeftOrRightEdge;

                            child.GetComponent<RectTransformComponent>().Anchors = new MinMaxRect
                            {
                                Min = new float2(1, 0),
                                Max = new float2(1, 0)
                            };
                            child.GetComponent<RectTransformComponent>().Offsets = GuiHelper.CalcOffsets(
                                GuiHelper.AnchorPos.DOWN_DOWN_RIGHT, _annotationCanvasPositions[annotationCount],
                                GuiHelper.CanvasHeightInit, GuiHelper.CanvasWidthInit, GuiHelper.AnnotationDim);
                        }
                    }
                    else
                    {
                        child.GetComponent<NineSlicePlane>().Active = false;
                        foreach (var comp in child.GetComponentsInChildren<Mesh>())
                            comp.Active = false;
                    }

                    annotationCount++;
                }
                else if (child.Name.Contains("line"))
                {
                    if (_isCircleVisible[lineCount])
                    {
                        List<float3> linePoints;
                        var annotationPos = _annotationCanvasPositions[lineCount];
                        var circlePos = _circleCanvasPositions[lineCount];

                        if (_circleCanvasPositions[lineCount].x <= _canvasWidth / 2)
                        {
                            //LEFT
                            linePoints = new List<float3>
                            {
                                new float3(annotationPos.x + GuiHelper.AnnotationDim.x, annotationPos.y + GuiHelper.AnnotationDim.y/2,0),
                                new float3(circlePos.x - (circleDim.x/2), circlePos.y,0)
                            };
                        }
                        else
                        {
                            //RIGHT
                            var posX = _canvasWidth - GuiHelper.AnnotationDim.x - GuiHelper.AnnotationDistToLeftOrRightEdge;

                            linePoints = new List<float3>
                            {
                                new float3(posX, annotationPos.y + GuiHelper.AnnotationDim.y/2,0),
                                new float3(circlePos.x + (circleDim.x/2), circlePos.y,0)
                            };
                        }

                        child.GetComponent<RectTransformComponent>().Offsets = GuiHelper.CalcOffsets(GuiHelper.AnchorPos.MIDDLE, new float2(0, 0), _canvasHeight, _canvasWidth, new float2(_canvasWidth, _canvasHeight));

                        //TODO: Does not work in web build - memoize value error
                        var mesh = child.GetComponent<Line>();

                        if (mesh != null && _circleCanvasPositions[lineCount] != _circleCanvasPositionsCache[lineCount])
                        {
                            var line = new Line(linePoints, 0.0025f / _resizeScaleFactor.y, _canvasWidth, _canvasHeight);
                            mesh.Vertices = line.Vertices;
                            mesh.Normals = line.Normals;
                            mesh.Triangles = line.Triangles;
                            //mesh.UVs = line.UVs;
                        }

                        if (mesh == null)
                        {
                            var line = new Line(linePoints, 0.0025f / _resizeScaleFactor.y, _canvasWidth, _canvasHeight);
                            child.AddComponent(line);
                        }
                    }
                    else
                    {
                        var line = child.GetComponent<Line>();
                        if (line != null)
                            child.Components.Remove(line);
                    }
                    lineCount++;
                }
            }

            Array.Copy(_circleCanvasPositions, _circleCanvasPositionsCache, NumberOfAnnotations);

            #endregion

            //TODO: set screenspace UI projection to orthographic in SceneRenderer
            if (_canvasRenderMode == CanvasRenderMode.SCREEN)
            {
                // Render the scene loaded in Init()
                _sceneRenderer.Render(RC);

                projection = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);
                RC.Projection = projection;
                _sih.Projection = projection;

                _guiRenderer.Render(RC);
            }
            else
            {
                _sceneRenderer.Render(RC);
                _guiRenderer.Render(RC);
            }

            Present();
        }

        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            _resizeScaleFactor = new float2((100 / _initWidth * Width) / 100, (100 / _initHeight * Height) / 100);
            _canvasHeight = GuiHelper.CanvasHeightInit * _resizeScaleFactor.y;
            _canvasWidth = GuiHelper.CanvasWidthInit * _resizeScaleFactor.x;

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            _aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(_fovy, _aspectRatio, ZNear, ZFar);
            RC.Projection = projection;
            _sih.Projection = projection;
            _scenePicker.Projection = projection;
        }

        private SceneContainer CreateGui()
        {
            var canvasScaleFactor = _initWidth / _canvasWidth;
            float textSize = 2;
            float borderScaleFactor = 1;
            if (_canvasRenderMode == CanvasRenderMode.SCREEN)
            {
                textSize *= canvasScaleFactor;
                borderScaleFactor = canvasScaleFactor;
            }

            #region annotations

            var annotationGreen = GuiHelper.CreateAnnotation(_annotationCanvasPositions[0], textSize, borderScaleFactor,
                "#1 Abcdefgh, 1.234", "check-circle.png", "frame_green.png");

            var annotationGreenFilled = GuiHelper.CreateAnnotation(_annotationCanvasPositions[2], textSize, borderScaleFactor,
                "#4 Abcdefgh", "check-circle_filled.png", "frame_green.png", 0.7f);

            var annotationYellow = GuiHelper.CreateAnnotation(_annotationCanvasPositions[1], textSize, borderScaleFactor,
                "#2 Abcde, 1.234", "lightbulb.png", "frame_yellow.png", 0.85f);

            var annotationGray = GuiHelper.CreateAnnotation(_annotationCanvasPositions[3], textSize, borderScaleFactor,
                "#3 Abcdefgh, 1.234", "minus-oktagon.png", "frame_gray.png");

            #endregion

            #region circles

            //Should possibly come from external data
            var circleDim = new float2(0.65f, 0.65f);

            var circleGreen = GuiHelper.CreateCircle(circleDim, GuiHelper.MatColor.GREEN);
            var circleGreenFilled = GuiHelper.CreateCircle(circleDim, GuiHelper.MatColor.GREEN);
            var circleYellow = GuiHelper.CreateCircle(circleDim, GuiHelper.MatColor.YELLOW);
            var circleGray = GuiHelper.CreateCircle(circleDim, GuiHelper.MatColor.GRAY);

            #endregion

            #region lines without actual meshes

            var lineGreen = GuiHelper.CreateLine(GuiHelper.MatColor.GREEN);
            var lineGreenFilled = GuiHelper.CreateLine(GuiHelper.MatColor.GREEN);
            var lineYellow = GuiHelper.CreateLine(GuiHelper.MatColor.YELLOW);
            var lineGray = GuiHelper.CreateLine(GuiHelper.MatColor.GRAY);

            #endregion

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
                GuiHelper.VsTex,
                GuiHelper.PsTex,
                guiFuseeLogo,
                new MinMaxRect
                {
                    Min = new float2(0, 1),
                    Max = new float2(0, 1)
                },
                GuiHelper.CalcOffsets(GuiHelper.AnchorPos.TOP_TOP_LEFT, new float2(0, _canvasHeight - 0.5f), _canvasHeight, _canvasWidth, new float2(1.75f, 0.5f)));
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

                    circleGreen,
                    circleYellow,
                    circleGray,
                    circleGreenFilled,

                    annotationGreen,
                    annotationYellow,
                    annotationGray,
                    annotationGreenFilled,

                    lineGreen,
                    lineYellow,
                    lineGray,
                    lineGreenFilled,
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