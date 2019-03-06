using System;
using System.Collections.Generic;
using System.Globalization;
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
        private const float Damping = 0.0f;
        private bool _keys;

        public int NumberOfAnnotations = 20;

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

        private float2[] _circleCanvasPositions;
        private float2[] _circleCanvasPositionsCache;

        private float2[] _annotationCanvasPositions;

        private List<UIInput> _uiInput;

        private ScenePicker _scenePicker;

        private SceneContainer BuildScene()
        {
            var sphere = new Sphere(32, 24);

            var lineControlPoints = new List<float3>
            {
                new float3(-3f,0,0) , new float3(-1.5f,-1.5f,0), new float3(1f,1.5f,0)
            };
            var line = new Line(lineControlPoints, 0.2f);

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
            _canvasHeight = UIHelper.CanvasHeightInit;
            _canvasWidth = UIHelper.CanvasWidthInit;

            _circleCanvasPositions = new float2[NumberOfAnnotations];
            _circleCanvasPositionsCache = new float2[NumberOfAnnotations];
            _annotationCanvasPositions = new float2[NumberOfAnnotations];
            _uiInput = new List<UIInput>();

            _initWidth = Width;
            _initHeight = Height;
            _aspectRatio = Width / (float)Height;

            //_scene = BuildScene();
            _scene = AssetStorage.Get<SceneContainer>("Monkey.fus");
            var monkey = _scene.Children[0].GetComponent<Mesh>();
            var rnd = new Random();
            var numberOfTriangles = monkey.Triangles.Length / 3;

            //Create dummy positions on model
            for (var i = 0; i < NumberOfAnnotations; i++)
            {
                var triangleNumber = rnd.Next(1, numberOfTriangles);
                var triIndex = (triangleNumber - 1) * 3;

                var triVert0 = monkey.Vertices[triIndex];
                var triVert1 = monkey.Vertices[triIndex + 1];
                var triVert2 = monkey.Vertices[triIndex + 2];

                var middle = (triVert0 + triVert1 + triVert2) / 3;

                _circleCanvasPositions[i] = new float2(middle.x, middle.y);
                _circleCanvasPositionsCache[i] = new float2(0, 0);

                var prob = (float)rnd.NextDouble();
                prob = (float)System.Math.Round(prob, 3);
                var dummyClass = UIHelper.DummySegmentationClasses[rnd.Next(0, UIHelper.DummySegmentationClasses.Count - 1)];

                var annotationKind = (UIHelper.AnnotationKind)rnd.Next(0, Enum.GetNames(typeof(UIHelper.AnnotationKind)).Length);

                var input = new UIInput(annotationKind, middle, new float2(0.65f, 0.65f), dummyClass, prob);
                input.AffectedTriangles.Add(triIndex);
                _uiInput.Add(input);
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

            //TODO: set screen space UI projection to orthographic in SceneRenderer
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
                    //var curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTime);
                    //_angleVelHorz *= curDamp;
                    //_angleVelVert *= curDamp;
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

            for (int i = 0; i < canvas.Children.Count; i++)
            {
                SceneNodeContainer child = canvas.Children[i];
                if (child.Name.Contains("Circle"))
                {
                    //1. Update Circle pos
                    var clipPos = _uiInput[circleCount].Position.TransformPerspective(RC.ModelViewProjection); //divides by w
                    var circleCanvasPos = new float2(clipPos.x, clipPos.y) * 0.5f + 0.5f;

                    circleCanvasPos.x *= _canvasWidth;
                    circleCanvasPos.y *= _canvasHeight;
                    _circleCanvasPositions[circleCount] = circleCanvasPos;

                    var pos = new float2(_circleCanvasPositions[circleCount].x - (circleDim.x / 2), _circleCanvasPositions[circleCount].y - (circleDim.y / 2)); //we want the lower left point of the rect that encloses the
                    child.GetComponent<RectTransformComponent>().Offsets = UIHelper.CalcOffsets(UIHelper.AnchorPos.MIDDLE, pos, _canvasHeight, _canvasWidth, circleDim);

                    //2. Check if circle is visible
                    var newPick = _scenePicker.Pick(new float2(clipPos.x, clipPos.y)).ToList().OrderBy(pr => pr.ClipPos.z).FirstOrDefault();

                    var col = UIHelper.MatColor.WHITE;

                    if (_uiInput[circleCount].AffectedTriangles[0] == newPick.Triangle) //VISIBLE
                    {
                        var input = _uiInput[circleCount];
                        input.IsVisible = true;
                        _uiInput[circleCount] = input;

                        if (child.Name.Contains("green"))
                            col = UIHelper.MatColor.GREEN;

                        else if (child.Name.Contains("yellow"))
                            col = UIHelper.MatColor.YELLOW;

                        if (child.Name.Contains("gray"))
                            col = UIHelper.MatColor.GRAY;

                        child.GetComponent<ShaderEffectComponent>().Effect = UIHelper.GetShaderEffectFromMatColor(col);
                    }
                    else
                    {
                        var input = _uiInput[circleCount];
                        input.IsVisible = false;
                        _uiInput[circleCount] = input;

                        col = UIHelper.MatColor.WHITE;
                        child.GetComponent<ShaderEffectComponent>().Effect = UIHelper.GetShaderEffectFromMatColor(col);
                    }

                    //Annotation Positions without intersections
                    if (_circleCanvasPositions[circleCount] != _circleCanvasPositionsCache[circleCount])
                    {
                        var yPosScale = _circleCanvasPositions[circleCount].y / _canvasHeight;
                        yPosScale = (yPosScale - 0.5f) * 2f;
                        _annotationCanvasPositions[circleCount].y = _circleCanvasPositions[circleCount].y - (UIHelper.AnnotationDim.y / 2) + (2 * UIHelper.AnnotationDim.y * yPosScale);

                        if (_circleCanvasPositions[circleCount].x > _canvasWidth / 2) //RIGHT                        
                            _annotationCanvasPositions[circleCount].x = UIHelper.CanvasWidthInit - UIHelper.AnnotationDim.x - UIHelper.AnnotationDistToLeftOrRightEdge;                        
                        else                        
                            _annotationCanvasPositions[circleCount].x = UIHelper.AnnotationDistToLeftOrRightEdge;

                        UpdateAnnotationOffsets(canvas.Children[i+1], annotationCount); //TODO: Think of somethimg clever to get annoation that belongs to this circle
                    }

                    circleCount++;
                }
            }

            foreach (var child in canvas.Children)
            {
                if (child.Name.Contains("Annotation"))
                {
                    if (_uiInput[annotationCount].IsVisible)
                    {
                        if (_circleCanvasPositions[annotationCount] != _circleCanvasPositionsCache[annotationCount])
                        {
                            var intersectedAnnotations = new Dictionary<int, float2>();
                            var iterations = 0;
                            CalculateAnnotationPositions(annotationCount, ref intersectedAnnotations, ref iterations);
                            //var text = child.Children[0].Children[0].GetComponent<GUIText>();
                            //child.Children[0].Children[0].Components.Remove(text);
                            //var newText = new GUIText(UIHelper.RalewayFontMap, _annotationCanvasPositions[annotationCount].ToString());
                            //child.Children[0].Children[0].AddComponent(newText);

                            UpdateAnnotationOffsets(child, annotationCount);
                        }

                        child.GetComponent<NineSlicePlane>().Active = true;
                        foreach (var comp in child.GetComponentsInChildren<Mesh>())
                            comp.Active = true;                        
                    }
                    else
                    {
                        child.GetComponent<NineSlicePlane>().Active = false;
                        foreach (var comp in child.GetComponentsInChildren<Mesh>())
                            comp.Active = false;
                    }

                    annotationCount++;
                }
            }

            foreach (var child in canvas.Children)
            {
                if (child.Name.Contains("line"))
                {
                    if (_uiInput[lineCount].IsVisible)
                    {
                        if (_circleCanvasPositions[lineCount] != _circleCanvasPositionsCache[lineCount])
                        {
                            List<float3> linePoints;
                            var annotationPos = _annotationCanvasPositions[lineCount];
                            var circlePos = _circleCanvasPositions[lineCount];

                            if (_circleCanvasPositions[lineCount].x <= _canvasWidth / 2)
                            {
                                //LEFT
                                linePoints = new List<float3>
                            {
                                new float3(annotationPos.x + UIHelper.AnnotationDim.x, annotationPos.y + UIHelper.AnnotationDim.y/2,0),
                                new float3(circlePos.x - (circleDim.x/2), circlePos.y,0)
                            };
                            }
                            else
                            {
                                //RIGHT
                                var posX = _canvasWidth - UIHelper.AnnotationDim.x - UIHelper.AnnotationDistToLeftOrRightEdge;

                                linePoints = new List<float3>
                            {
                                new float3(posX, annotationPos.y + UIHelper.AnnotationDim.y/2,0),
                                new float3(circlePos.x + (circleDim.x/2), circlePos.y,0)
                            };
                            }

                            child.GetComponent<RectTransformComponent>().Offsets = UIHelper.CalcOffsets(UIHelper.AnchorPos.MIDDLE, new float2(0, 0), _canvasHeight, _canvasWidth, new float2(_canvasWidth, _canvasHeight));

                            var mesh = child.GetComponent<Line>();

                            if (mesh != null)
                            {
                                var line = new Line(linePoints, 0.0025f / _resizeScaleFactor.y, _canvasWidth, _canvasHeight);
                                mesh.Vertices = line.Vertices;
                                mesh.Normals = line.Normals;
                                mesh.Triangles = line.Triangles;
                                mesh.UVs = line.UVs;
                            }

                            else if (mesh == null)
                            {
                                var line = new Line(linePoints, 0.0025f / _resizeScaleFactor.y, _canvasWidth, _canvasHeight);
                                child.AddComponent(line);
                            }
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

            //TODO: set screen space UI projection to orthographic in SceneRenderer
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

        //Calculate y position, taking intersection into account
        private void CalculateAnnotationPositions(int annotationIndex, ref Dictionary<int, float2> intersectedAnnotations, ref int iterations)
        {
            if (_uiInput[annotationIndex].IsVisible)
            {
                var intersectionCount = 0;
                for (var i = 0; i < _annotationCanvasPositions.Length; i++)
                {
                    if (i == annotationIndex || !_uiInput[i].IsVisible || intersectedAnnotations.ContainsKey(i))
                        continue;

                    var intersect = UIHelper.DoesAnnotationIntersectWithAnnotation(
                        _annotationCanvasPositions[annotationIndex], _annotationCanvasPositions[i]);

                    if (!intersect) continue;
                    if (!intersectedAnnotations.ContainsKey(i))
                    {
                        intersectedAnnotations.Add(i, _annotationCanvasPositions[i]);
                        intersectionCount++;
                    }
                    
                }

                if (intersectionCount == 0)
                    return;

                if (intersectedAnnotations.Count >= 1)
                {
                    if (!intersectedAnnotations.ContainsKey(annotationIndex))
                        intersectedAnnotations.Add(annotationIndex, _annotationCanvasPositions[annotationIndex]); //add pos that is just being checked

                    var orderdBy = intersectedAnnotations.OrderBy(item => item.Value.y).ToList(); //JSIL not implemented exception: OrderdByDecending and Reverse
                    var orderdByDescending = new List<KeyValuePair<int, float2>>();
                    for (int i = 0; i < orderdBy.Count; i++) //Reverse
                    {
                        orderdByDescending.Add(orderdBy[orderdBy.Count - 1 - i]);
                    }

                    intersectedAnnotations = new Dictionary<int, float2>();
                    foreach (var keyValue in orderdByDescending) //JSIL not implemented exception: ToDictionary
                    {
                        intersectedAnnotations.Add(keyValue.Key, keyValue.Value);
                    }

                    var middleIndex = (intersectedAnnotations.Count) / 2;
                    var averagePos = new float2();

                    for (int i = 0; i < intersectedAnnotations.Count; i++)
                        averagePos += intersectedAnnotations.ElementAt(middleIndex).Value;

                    averagePos /= intersectedAnnotations.Count;

                    for (int i = 0; i < intersectedAnnotations.Count; i++)
                    {
                        var annotCount = intersectedAnnotations.ElementAt(i).Key;
                        _annotationCanvasPositions[annotCount] = averagePos;

                        var multiplier = System.Math.Abs(i - middleIndex);
                        var pos = _annotationCanvasPositions[annotCount];
                        //Distance between annotations is 0.5* AnnotationDim.y
                        if (intersectedAnnotations.Count % 2 == 0) //even
                        {
                            if (i == middleIndex - 1)
                                pos.y += 0.75f * UIHelper.AnnotationDim.y;

                            else if (i == middleIndex)
                                pos.y -= 0.75f * UIHelper.AnnotationDim.y;

                            else if (i > middleIndex)
                                pos.y -= (0.75f * UIHelper.AnnotationDim.y) + (multiplier * (UIHelper.AnnotationDim.y + UIHelper.AnnotationDim.y / 2));

                            else if (i < middleIndex)
                                pos.y += (0.75f * UIHelper.AnnotationDim.y) + ((multiplier - 1) * (UIHelper.AnnotationDim.y + UIHelper.AnnotationDim.y / 2));

                        }
                        else //odd
                        {
                            if (i > middleIndex)
                                pos.y -= 0.5f * multiplier * UIHelper.AnnotationDim.y + (UIHelper.AnnotationDim.y * multiplier);

                            else if (i < middleIndex)
                                pos.y += 0.5f * multiplier * UIHelper.AnnotationDim.y + (UIHelper.AnnotationDim.y * multiplier);
                        }
                        _annotationCanvasPositions[annotCount] = pos;
                    }
                }


                //Recursively check all annotations that where involved in this intersection
                for (var i = 0; i < intersectedAnnotations.Count; i++)
                {
                    if (i == 0 || i == intersectedAnnotations.Count - 1)
                    {
                        iterations++;
                        CalculateAnnotationPositions(intersectedAnnotations.ElementAt(i).Key, ref intersectedAnnotations, ref iterations);
                    }
                }
            }
        }

        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            _resizeScaleFactor = new float2((100 / _initWidth * Width) / 100, (100 / _initHeight * Height) / 100);
            _canvasHeight = UIHelper.CanvasHeightInit * _resizeScaleFactor.y;
            _canvasWidth = UIHelper.CanvasWidthInit * _resizeScaleFactor.x;

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
                UIHelper.VsTex,
                UIHelper.PsTex,
                guiFuseeLogo,
                new MinMaxRect
                {
                    Min = new float2(0, 1),
                    Max = new float2(0, 1)
                },
                UIHelper.CalcOffsets(UIHelper.AnchorPos.TOP_TOP_LEFT, new float2(0, _canvasHeight - 0.5f), _canvasHeight, _canvasWidth, new float2(1.75f, 0.5f)));
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
                }
            };


            //Should possibly come from external data
            var circleDim = new float2(0.65f, 0.65f);

            for (int i = 0; i < _uiInput.Count; i++)
            {
                UIInput item = _uiInput[i];
                if (item.AnnotationKind != UIHelper.AnnotationKind.CONFIRMED)
                {
                    UIHelper.CreateAndAddCircleAnnotationAndLine(canvas, item.AnnotationKind, item.Size, _annotationCanvasPositions[i], textSize, borderScaleFactor,
                    "#" + i + " " + item.SegmentationClass + ", " + item.Probability.ToString(CultureInfo.GetCultureInfo("en-gb")));
                }
                else
                {
                    UIHelper.CreateAndAddCircleAnnotationAndLine(canvas, item.AnnotationKind, item.Size, _annotationCanvasPositions[i], textSize, borderScaleFactor,
                   "#" + i + " " + item.SegmentationClass);
                }
            }

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

        private void UpdateAnnotationOffsets(SceneNodeContainer sncAnnotation, int annotationCount)
        {
            if (_circleCanvasPositions[annotationCount].x <= _canvasWidth / 2)
            {
                //LEFT
                sncAnnotation.GetComponent<RectTransformComponent>().Anchors = new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(0, 0)
                };

                sncAnnotation.GetComponent<RectTransformComponent>().Offsets = UIHelper.CalcOffsets(
                    UIHelper.AnchorPos.DOWN_DOWN_LEFT, _annotationCanvasPositions[annotationCount],
                    UIHelper.CanvasHeightInit, UIHelper.CanvasWidthInit, UIHelper.AnnotationDim);
            }
            else
            {
                //RIGHT
                sncAnnotation.GetComponent<RectTransformComponent>().Anchors = new MinMaxRect
                {
                    Min = new float2(1, 0),
                    Max = new float2(1, 0)
                };

                sncAnnotation.GetComponent<RectTransformComponent>().Offsets = UIHelper.CalcOffsets(
                    UIHelper.AnchorPos.DOWN_DOWN_RIGHT, _annotationCanvasPositions[annotationCount],
                    UIHelper.CanvasHeightInit, UIHelper.CanvasWidthInit, UIHelper.AnnotationDim);
            }
        }
    }
}