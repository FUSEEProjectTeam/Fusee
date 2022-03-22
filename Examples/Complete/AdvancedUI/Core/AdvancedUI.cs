using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Fusee.Examples.AdvancedUI.Core
{
    [FuseeApplication(Name = "FUSEE UI Example", Description = " ")]
    public class AdvancedUI : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;
        private bool _keys;

        public int NumberOfAnnotations = 8;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private SceneNode _camNode;
        private Transform _mainCamPivot;
        private Camera _mainCam = new(ProjectionMethod.Perspective, 0.1f, 1000, M.PiOver4)
        {
            BackgroundColor = new float4(0.1f, 0.1f, 0.1f, 1)
        };

        private float _initWidth;
        private float _initHeight;
        private float2 _resizeScaleFactor;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.Screen;

        private float _canvasWidth;
        private float _canvasHeight;

        private List<UserInterfaceInput> _uiInput;

        private ScenePicker _scenePicker;

        private CanvasNode _canvas;
        private Mesh _monkey;
        private SceneNode _monkeyNode;

        //rnd is public so unit tests can inject a seeded random.
        public Random rnd;

        // Init is called on startup.
        public override void Init()
        {
            if (_canvasRenderMode == CanvasRenderMode.Screen)
            {
                UserInterfaceHelper.CanvasWidthInit = Width / 100f;
                UserInterfaceHelper.CanvasHeightInit = Height / 100f;
            }
            else
            {
                UserInterfaceHelper.CanvasHeightInit = 9;
                UserInterfaceHelper.CanvasWidthInit = 16;
            }

            _canvasHeight = UserInterfaceHelper.CanvasHeightInit;
            _canvasWidth = UserInterfaceHelper.CanvasWidthInit;

            _uiInput = new List<UserInterfaceInput>();

            _initWidth = Width;
            _initHeight = Height;

            _mainCamPivot = new Transform()
            {
                Translation = new float3(0, 0, 0),
                Rotation = new float3()
            };
            _camNode = new SceneNode()
            {
                Components = new List<SceneComponent>()
                {
                    _mainCamPivot
                },
                Children = new ChildList()
                {
                    new SceneNode()
                    {
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Translation = new float3(0, 0, -5),
                                Rotation = new float3()
                            },
                            _mainCam
                        }
                    }
                }

            };
            _scene = AssetStorage.Get<SceneContainer>("Monkey.fus");
            _scene.Children.Add(_camNode);

            _monkeyNode = _scene.Children[0];
            _monkey = _monkeyNode.GetComponent<Mesh>();

            if (rnd == null)
                rnd = new Random();

            var numberOfTriangles = _monkey.Triangles.Length / 3;

            //Create dummy positions on model
            for (int i = 0; i < NumberOfAnnotations; i++)
            {
                int triangleNumber = rnd.Next(1, numberOfTriangles);
                int triIndex = (triangleNumber - 1) * 3;

                float3 triVert0 = _monkey.Vertices[_monkey.Triangles[triIndex]];
                float3 triVert1 = _monkey.Vertices[_monkey.Triangles[triIndex + 1]];
                float3 triVert2 = _monkey.Vertices[_monkey.Triangles[triIndex + 2]];

                float3 middle = (triVert0 + triVert1 + triVert2) / 3;

                float2 circleCanvasPos = new(middle.x, middle.y);
                float2 circleCanvasPosCache = new(0, 0);

                float prob = (float)rnd.NextDouble();
                prob = (float)System.Math.Round(prob, 3);
                string dummyClass = UserInterfaceHelper.DummySegmentationClasses[rnd.Next(0, UserInterfaceHelper.DummySegmentationClasses.Count - 1)];

                UserInterfaceHelper.AnnotationKind annotationKind = (UserInterfaceHelper.AnnotationKind)rnd.Next(0, Enum.GetNames(typeof(UserInterfaceHelper.AnnotationKind)).Length);

                UserInterfaceInput input = new(annotationKind, middle, new float2(0.65f, 0.65f), dummyClass, prob)
                {
                    Identifier = i,
                    CircleCanvasPos = circleCanvasPos,
                    CircleCanvasPosCache = circleCanvasPosCache
                };

                input.AffectedTriangles.Add(triIndex);
                _uiInput.Add(input);
            }

            _gui = CreateGui();


            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            //Create a scene picker for performing visibility tests
            _scenePicker = new ScenePicker(_scene);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _guiRenderer = new SceneRendererForward(_gui);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            #region Controls

            // Mouse and keyboard movement
            if (Input.Keyboard.LeftRightAxis != 0 || Input.Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Input.Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = RotationSpeed * Input.Mouse.XVel * Time.DeltaTime * 0.0005f;
                _angleVelVert = RotationSpeed * Input.Mouse.YVel * Time.DeltaTime * 0.0005f;
            }
            else if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                float2 touchVel = Input.Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = RotationSpeed * touchVel.x * Time.DeltaTime * 0.0005f;
                _angleVelVert = RotationSpeed * touchVel.y * Time.DeltaTime * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = RotationSpeed * Input.Keyboard.LeftRightAxis * Time.DeltaTime;
                    _angleVelVert = RotationSpeed * Input.Keyboard.UpDownAxis * Time.DeltaTime;
                }
                else
                {
                    float curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // Create the camera matrix and set it as the current ModelView transformation
            _mainCamPivot.RotationMatrix = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            #endregion Controls

            _sceneRenderer.Render(RC);

            //Annotations will be updated according to circle positions.
            //Lines will be updated according to circle and annotation positions.
            foreach (SceneNode child in _canvas.Children)
            {
                if (!child.Name.Contains("MarkModelContainer")) continue;

                //1. Calculate the circles canvas position.
                for (int k = 0; k < child.Children.Count; k++)
                {
                    SceneNode container = child.Children[k];

                    SceneNode circle = container.Children[0];
                    UserInterfaceInput uiInput = _uiInput[k];

                    //the monkey's matrices
                    float4x4 model = _monkeyNode.GetGlobalTransformation();
                    var view = _camNode.Children[0].GetGlobalTransformation().Invert();
                    var projection = _mainCam.GetProjectionMat(Width, Height, out _);
                    float4x4 mvpMonkey = projection * view * model;

                    float3 clipPos = float4x4.TransformPerspective(mvpMonkey, uiInput.Position);

                    float2 canvasPosCircle = new float2(clipPos.x, clipPos.y) * 0.5f + 0.5f;
                    canvasPosCircle.x *= _canvasWidth;
                    canvasPosCircle.y *= _canvasHeight;

                    uiInput.CircleCanvasPos = canvasPosCircle.xy;

                    var pos = new float2(uiInput.CircleCanvasPos.x - (uiInput.Size.x / 2), uiInput.CircleCanvasPos.y - (uiInput.Size.y / 2));

                    circle.GetComponent<RectTransform>().Offsets = GuiElementPosition.CalcOffsets(AnchorPos.Middle, pos, _canvasHeight, _canvasWidth, uiInput.Size);

                    //1.1   Check if circle is visible
                    PickResult newPick = _scenePicker.Pick(RC, new float2(clipPos.x, clipPos.y)).ToList().OrderBy(pr => pr.ClipPos.z).FirstOrDefault();

                    if (newPick != null && uiInput.AffectedTriangles[0] == newPick.Triangle) //VISIBLE
                    {
                        uiInput.IsVisible = true;

                        var effect = circle.GetComponent<SurfaceEffect>();
                        effect.SetDiffuseAlphaInShaderEffect(UserInterfaceHelper.alphaVis);
                    }
                    else
                    {
                        uiInput.IsVisible = false;
                        var effect = circle.GetComponent<SurfaceEffect>();
                        effect.SetDiffuseAlphaInShaderEffect(UserInterfaceHelper.alphaInv);

                    }

                    //1.2   Calculate annotation positions without intersections.
                    if (!uiInput.CircleCanvasPos.Equals(uiInput.CircleCanvasPosCache))
                    {
                        float yPosScale = uiInput.CircleCanvasPos.y / _canvasHeight;
                        yPosScale = (yPosScale - 0.5f) * 2f;
                        uiInput.AnnotationCanvasPos.y = uiInput.CircleCanvasPos.y - (UserInterfaceHelper.AnnotationDim.y / 2) + (2 * UserInterfaceHelper.AnnotationDim.y * yPosScale);

                        if (uiInput.CircleCanvasPos.x > _canvasWidth / 2) //RIGHT
                            uiInput.AnnotationCanvasPos.x = UserInterfaceHelper.CanvasWidthInit - UserInterfaceHelper.AnnotationDim.x - UserInterfaceHelper.AnnotationDistToLeftOrRightEdge;
                        else
                            uiInput.AnnotationCanvasPos.x = UserInterfaceHelper.AnnotationDistToLeftOrRightEdge;
                    }
                    _uiInput[k] = uiInput;
                }

                // 2.   Find intersecting annotations and correct their position in _uiInput.
                //      Disable rendering of annotation if its corresponding circle is not visible.
                for (int k = 0; k < child.Children.Count; k++)
                {
                    SceneNode container = child.Children[k];
                    SceneNode annotation = container.Children[1];
                    UserInterfaceInput uiInput = _uiInput[k];

                    if (uiInput.IsVisible)
                    {
                        if (!uiInput.CircleCanvasPos.Equals(uiInput.CircleCanvasPosCache))
                        {
                            Dictionary<int, float2> intersectedAnnotations = new();
                            int iterations = 0;
                            CalculateNonIntersectingAnnotationPositions(ref uiInput, ref intersectedAnnotations, ref iterations);
                        }

                        annotation.GetComponent<NineSlicePlane>().Active = true;
                        foreach (Mesh comp in annotation.GetComponentsInChildren<Mesh>())
                            comp.Active = true;
                    }
                    else
                    {
                        annotation.GetComponent<NineSlicePlane>().Active = false;
                        foreach (Mesh comp in annotation.GetComponentsInChildren<Mesh>())
                            comp.Active = false;
                    }
                }

                // 3.   Update annotation positions on canvas and draw line
                for (int k = 0; k < child.Children.Count; k++)
                {
                    SceneNode container = child.Children[k];

                    SceneNode line = container.Children[2];
                    UserInterfaceInput uiInput = _uiInput[k];

                    if (uiInput.IsVisible)
                    {
                        if (!uiInput.CircleCanvasPos.Equals(uiInput.CircleCanvasPosCache))
                        {
                            UpdateAnnotationOffsets(child.Children[uiInput.Identifier].Children[1], uiInput);
                            DrawLine(child.Children[uiInput.Identifier].Children[2], uiInput);
                        }
                    }

                    DrawLine(line, uiInput);

                    uiInput.CircleCanvasPosCache = uiInput.CircleCanvasPos;
                    _uiInput[k] = uiInput;
                }
            }

            // Constantly check for interactive objects.
            _guiRenderer.Render(RC);
            if (!Input.Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Input.Mouse.Position, Width, Height);

            if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Input.Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Input.Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }


            Present();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
            _resizeScaleFactor = new float2((100 / _initWidth * Width) / 100, (100 / _initHeight * Height) / 100);

            _canvasHeight = UserInterfaceHelper.CanvasHeightInit * _resizeScaleFactor.y;
            _canvasWidth = UserInterfaceHelper.CanvasWidthInit * _resizeScaleFactor.x;
        }

        private SceneContainer CreateGui()
        {
            float canvasScaleFactor = _initWidth / _canvasWidth;
            float borderScaleFactor = 1;
            if (_canvasRenderMode == CanvasRenderMode.Screen)
            {
                borderScaleFactor = canvasScaleFactor;
            }

            GuiButton btnFuseeLogo = new()
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            Texture guiFuseeLogo = new(AssetStorage.Get<ImageData>("FuseeText.png"));
            TextureNode fuseeLogo = TextureNode.Create(
                "fuseeLogo",
                guiFuseeLogo,
                GuiElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                GuiElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, _canvasHeight - 0.5f), _canvasHeight, _canvasWidth, new float2(1.75f, 0.5f)), float2.One);
            fuseeLogo.AddComponent(btnFuseeLogo);

            SceneNode markModelContainer = new()
            {
                Name = "MarkModelContainer",
            };

            _canvas = new(
                "Canvas",
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-_canvasWidth / 2f, -_canvasHeight / 2f),
                    Max = new float2(_canvasWidth / 2f, _canvasHeight / 2f)
                }
            )
            { Children = new ChildList() { fuseeLogo, markModelContainer } };

            for (int i = 0; i < _uiInput.Count; i++)
            {
                var item = _uiInput[i];
                if (item.AnnotationKind != UserInterfaceHelper.AnnotationKind.Confirmed)
                {
                    UserInterfaceHelper.CreateAndAddCircleAnnotationAndLine(markModelContainer, item.AnnotationKind, item.Size, _uiInput[i].AnnotationCanvasPos, borderScaleFactor,
                    "#" + i + " " + item.SegmentationClass + ", " + item.Probability.ToString(CultureInfo.GetCultureInfo("en-gb")));
                }
                else
                {
                    UserInterfaceHelper.CreateAndAddCircleAnnotationAndLine(markModelContainer, item.AnnotationKind, item.Size, _uiInput[i].AnnotationCanvasPos, borderScaleFactor,
                   "#" + i + " " + item.SegmentationClass);
                }
            }

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    new SceneNode()
                    {
                        Name = "GuiCamNode",
                        Components = new List<SceneComponent>()
                        {
                            _mainCamPivot
                        },
                        Children = new ChildList()
                        {
                            new SceneNode()
                            {
                                Components = new List<SceneComponent>()
                                {
                                    new Transform()
                                    {
                                        Translation = new float3(0, 0, -5),
                                        Rotation = new float3()
                                    },
                                    new Camera(_canvasRenderMode == CanvasRenderMode.Screen ? ProjectionMethod.Orthographic : ProjectionMethod.Perspective, 0.01f, 500, M.PiOver4)
                                    {
                                        ClearColor = false
                                    }
                                }
                            }
                        }
                    },
                    new SceneNode()
                    {
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Translation = float3.Zero,
                                Rotation = float3.Zero,
                                Scale = new float3(_canvasWidth, _canvasHeight, 0.1f)
                            },

                        },
                        Children = new ChildList()
                        {
                            _canvas
                        }
                    }
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
            effect.SetFxParam(UniformNameDeclarations.Albedo, (float4)ColorUint.White);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 1f);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }

        private void UpdateAnnotationOffsets(SceneNode sncAnnotation, UserInterfaceInput input)
        {
            if (input.CircleCanvasPos.x <= _canvasWidth / 2)
            {
                //LEFT
                sncAnnotation.GetComponent<RectTransform>().Anchors = GuiElementPosition.GetAnchors(AnchorPos.DownDownLeft);

                sncAnnotation.GetComponent<RectTransform>().Offsets = GuiElementPosition.CalcOffsets(
                    AnchorPos.DownDownLeft, input.AnnotationCanvasPos,
                    UserInterfaceHelper.CanvasHeightInit, UserInterfaceHelper.CanvasWidthInit, UserInterfaceHelper.AnnotationDim);
            }
            else
            {
                //RIGHT
                sncAnnotation.GetComponent<RectTransform>().Anchors = GuiElementPosition.GetAnchors(AnchorPos.DownDownRight);

                sncAnnotation.GetComponent<RectTransform>().Offsets = GuiElementPosition.CalcOffsets(
                    AnchorPos.DownDownRight, input.AnnotationCanvasPos,
                    UserInterfaceHelper.CanvasHeightInit, UserInterfaceHelper.CanvasWidthInit, UserInterfaceHelper.AnnotationDim);
            }
        }

        private void DrawLine(SceneNode sncLine, UserInterfaceInput uiInput)
        {
            if (uiInput.IsVisible)
            {
                if (uiInput.CircleCanvasPos.Equals(uiInput.CircleCanvasPosCache)) return;
                List<float3> linePoints;

                if (uiInput.CircleCanvasPos.x <= _canvasWidth / 2)
                {
                    //LEFT
                    linePoints = new List<float3>
                    {
                        new float3(uiInput.AnnotationCanvasPos.x + UserInterfaceHelper.AnnotationDim.x, uiInput.AnnotationCanvasPos.y + UserInterfaceHelper.AnnotationDim.y/2,0),
                        new float3(uiInput.CircleCanvasPos.x - (uiInput.Size.x/2), uiInput.CircleCanvasPos.y,0)
                    };
                }
                else
                {
                    //RIGHT
                    float posX = _canvasWidth - UserInterfaceHelper.AnnotationDim.x - UserInterfaceHelper.AnnotationDistToLeftOrRightEdge;

                    linePoints = new List<float3>
                    {
                        new float3(posX, uiInput.AnnotationCanvasPos.y + UserInterfaceHelper.AnnotationDim.y/2,0),
                        new float3(uiInput.CircleCanvasPos.x + (uiInput.Size.x/2), uiInput.CircleCanvasPos.y,0)
                    };
                }

                sncLine.GetComponent<RectTransform>().Offsets = GuiElementPosition.CalcOffsets(AnchorPos.Middle, new float2(0, 0), _canvasHeight, _canvasWidth, new float2(_canvasWidth, _canvasHeight));

                Line mesh = sncLine.GetComponent<Line>();

                if (mesh != null)
                {
                    Line newLine = new(linePoints, 0.0025f / _resizeScaleFactor.y, _canvasWidth, _canvasHeight);
                    mesh.Vertices = newLine.Vertices;
                    mesh.Normals = newLine.Normals;
                    mesh.Triangles = newLine.Triangles;
                    mesh.UVs = newLine.UVs;
                }
                else
                {
                    Line newLine = new(linePoints, 0.0025f / _resizeScaleFactor.y, _canvasWidth, _canvasHeight);
                    sncLine.AddComponent(newLine);
                }
            }
            else
            {
                Line newLine = sncLine.GetComponent<Line>();
                if (newLine != null)
                    sncLine.Components.Remove(newLine);
            }
        }

        private void CalculateNonIntersectingAnnotationPositions(ref UserInterfaceInput input, ref Dictionary<int, float2> intersectedAnnotations, ref int iterations)
        {
            if (!input.IsVisible) return;

            int intersectionCount = 0;
            for (int i = 0; i < _uiInput.Count; i++)
            {
                UserInterfaceInput counterpart = _uiInput[i];

                if (counterpart.Identifier == input.Identifier || !counterpart.IsVisible || intersectedAnnotations.ContainsKey(counterpart.Identifier))
                    continue;

                float halfAnnotationHeight = (UserInterfaceHelper.AnnotationDim.y / 2f);
                float buffer = halfAnnotationHeight - (halfAnnotationHeight / 100f * 10f);
                //If we do not multiply by the resize scale factor the intersction test will return wrong results because AnnotationCanvasPos is in the range of the size of the initial canvas.
                bool intersect = UserInterfaceHelper.DoesAnnotationIntersectWithAnnotation(input.AnnotationCanvasPos, _uiInput[i].AnnotationCanvasPos, new float2(0, buffer));

                if (!intersect || intersectedAnnotations.ContainsKey(counterpart.Identifier)) continue;

                intersectedAnnotations.Add(counterpart.Identifier, _uiInput[i].AnnotationCanvasPos);
                intersectionCount++;
            }

            if (intersectionCount == 0)
                return;

            if (intersectedAnnotations.Count >= 1)
            {
                if (!intersectedAnnotations.ContainsKey(input.Identifier))
                    intersectedAnnotations.Add(input.Identifier, input.AnnotationCanvasPos); //add pos that is just being checked

                List<KeyValuePair<int, float2>> orderedBy = intersectedAnnotations.OrderBy(item => item.Value.y).ToList();

                intersectedAnnotations = new Dictionary<int, float2>();
                foreach (KeyValuePair<int, float2> keyValue in orderedBy) //JSIL not implemented exception: ToDictionary
                {
                    intersectedAnnotations.Add(keyValue.Key, keyValue.Value);
                }

                int middleIndex = (intersectedAnnotations.Count) / 2;
                float2 averagePos = new();

                for (int i = 0; i < intersectedAnnotations.Count; i++)
                    averagePos += intersectedAnnotations.ElementAt(i).Value;

                averagePos /= intersectedAnnotations.Count;

                for (int i = 0; i < intersectedAnnotations.Count; i++)
                {
                    int identifier = intersectedAnnotations.ElementAt(i).Key;
                    UserInterfaceInput thisInput = _uiInput[identifier];
                    thisInput.AnnotationCanvasPos = averagePos;

                    int multiplier = System.Math.Abs(i - middleIndex);

                    //Distance between annotations is 0.5* AnnotationDim.y
                    if (intersectedAnnotations.Count % 2 == 0) //even
                    {
                        if (i == middleIndex - 1)
                            thisInput.AnnotationCanvasPos.y -= 0.75f * UserInterfaceHelper.AnnotationDim.y;
                        else if (i == middleIndex)
                            thisInput.AnnotationCanvasPos.y += 0.75f * UserInterfaceHelper.AnnotationDim.y;
                        else if (i > middleIndex)
                            thisInput.AnnotationCanvasPos.y += (0.75f * UserInterfaceHelper.AnnotationDim.y) + (multiplier * (UserInterfaceHelper.AnnotationDim.y + UserInterfaceHelper.AnnotationDim.y / 2));
                        else if (i < middleIndex)
                            thisInput.AnnotationCanvasPos.y -= (0.75f * UserInterfaceHelper.AnnotationDim.y) + ((multiplier - 1) * (UserInterfaceHelper.AnnotationDim.y + UserInterfaceHelper.AnnotationDim.y / 2));
                    }
                    else //odd
                    {
                        if (i > middleIndex)
                            thisInput.AnnotationCanvasPos.y += 0.5f * multiplier * UserInterfaceHelper.AnnotationDim.y + (UserInterfaceHelper.AnnotationDim.y * multiplier);
                        else if (i < middleIndex)
                            thisInput.AnnotationCanvasPos.y -= 0.5f * multiplier * UserInterfaceHelper.AnnotationDim.y + (UserInterfaceHelper.AnnotationDim.y * multiplier);
                    }

                    _uiInput[identifier] = thisInput;
                }
            }

            //Recursively check all annotations that where involved in this intersection
            for (int i = 0; i < intersectedAnnotations.Count; i++)
            {
                if (i != 0 && i != intersectedAnnotations.Count - 1) continue;
                iterations++;
                int identifier = intersectedAnnotations.ElementAt(i).Key;
                UserInterfaceInput uiInput = _uiInput[identifier];

                CalculateNonIntersectingAnnotationPositions(ref uiInput, ref intersectedAnnotations, ref iterations);
            }
        }
    }
}