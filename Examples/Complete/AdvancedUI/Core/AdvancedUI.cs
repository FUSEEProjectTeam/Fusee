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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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

        private const float ZNear = 1f;
        private const float ZFar = 1000;

        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private float _initWidth;
        private float _initHeight;
        private float2 _resizeScaleFactor;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;

        private float _canvasWidth;
        private float _canvasHeight;

        private readonly float _fovy = M.PiOver4;

        private List<UIInput> _uiInput;

        private ScenePicker _scenePicker;

        //rnd is public so unit tests can inject a seeded random.
        public Random rnd;

        private SceneContainer BuildScene()
        {
            Sphere sphere = new Sphere(32, 24);

            List<float3> lineControlPoints = new List<float3>
            {
                new float3(-3f,0,0) , new float3(-1.5f,-1.5f,0), new float3(1f,1.5f,0)
            };
            Line line = new Line(lineControlPoints, 0.2f);

            return new SceneContainer()
            {
                Children = new List<SceneNode>()
                {
                    new SceneNode()
                    {
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Name = "SphereTransform",
                                Rotation = new float3(0,0,0),
                                Translation = new float3(0,0,0),
                                Scale = new float3(1, 1, 1)
                            },
                            MakeEffect.FromDiffuseSpecularTexture(new float4(0.90980f, 0.35686f, 0.35686f,1), 20,"crumpled-paper-free.jpg",0.5f)                            
                            //sphere
                        }
                    },
                    new SceneNode()
                    {
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Name = "LineTransform",
                                Rotation = new float3(0,0,0),
                                Translation = new float3(0,0,0),
                                Scale = new float3(1, 1, 1)
                            },
                            MakeEffect.FromDiffuseSpecular(new float4(0, 0, 1,1), 20),
                            line
                        }
                    }
                }
            };
        }

        // Init is called on startup.
        public override async Task<bool> Init()
        {
            if (_canvasRenderMode == CanvasRenderMode.SCREEN)
            {
                UIHelper.CanvasWidthInit = Width / 100f;
                UIHelper.CanvasHeightInit = Height / 100f;
            }
            else
            {
                UIHelper.CanvasHeightInit = 16;
                UIHelper.CanvasWidthInit = 9;
            }

            _canvasHeight = UIHelper.CanvasHeightInit;
            _canvasWidth = UIHelper.CanvasWidthInit;

            _uiInput = new List<UIInput>();

            _initWidth = Width;
            _initHeight = Height;

            //_scene = BuildScene();
            _scene = AssetStorage.Get<SceneContainer>("Monkey.fus");

            Mesh monkey = _scene.Children[0].GetComponent<Mesh>();
            rnd = new Random();
            int numberOfTriangles = monkey.Triangles.Length / 3;

            //Create dummy positions on model
            for (int i = 0; i < NumberOfAnnotations; i++)
            {
                int triangleNumber = rnd.Next(1, numberOfTriangles);
                int triIndex = (triangleNumber - 1) * 3;

                float3 triVert0 = monkey.Vertices[monkey.Triangles[triIndex]];
                float3 triVert1 = monkey.Vertices[monkey.Triangles[triIndex + 1]];
                float3 triVert2 = monkey.Vertices[monkey.Triangles[triIndex + 2]];

                float3 middle = (triVert0 + triVert1 + triVert2) / 3;

                float2 circleCanvasPos = new float2(middle.x, middle.y);
                float2 circleCanvasPosCache = new float2(0, 0);

                float prob = (float)rnd.NextDouble();
                prob = (float)System.Math.Round(prob, 3);
                string dummyClass = UIHelper.DummySegmentationClasses[rnd.Next(0, UIHelper.DummySegmentationClasses.Count - 1)];

                UIHelper.AnnotationKind annotationKind = (UIHelper.AnnotationKind)rnd.Next(0, Enum.GetNames(typeof(UIHelper.AnnotationKind)).Length);

                UIInput input = new UIInput(annotationKind, middle, new float2(0.65f, 0.65f), dummyClass, prob)
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

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.1f, 0.1f, 0.1f, 1);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _guiRenderer = new SceneRendererForward(_gui);

            return true;
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

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
                float2 touchVel = Input.Touch.GetVelocity(TouchPoints.Touchpoint_0);
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
                    float curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // Create the camera matrix and set it as the current ModelView transformation
            float4x4 mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            float4x4 mtxCam = float4x4.LookAt(0, 0, -5, 0, 0, 0, 0, 1, 0);

            float4x4 view = mtxCam * mtxRot;
            float4x4 perspective = float4x4.CreatePerspectiveFieldOfView(_fovy, (float)Width / Height, ZNear, ZFar);
            float4x4 orthographic = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);


            RC.View = view;
            RC.Projection = _canvasRenderMode == CanvasRenderMode.SCREEN ? orthographic : perspective;
            // Constantly check for interactive objects.
            if (!Input.Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Input.Mouse.Position, Width, Height);

            if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Input.Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Input.Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            #endregion Controls

            //Annotations will be updated according to circle positions.
            //Lines will be updated according to circle and annotation positions.

            RC.View = view;
            RC.Projection = perspective;
            SceneNode canvas = _gui.Children[0];

            foreach (SceneNode child in canvas.Children)
            {
                if (!child.Name.Contains("MarkModelContainer")) continue;

                //1. Calculate the circles canvas position.
                for (int k = 0; k < child.Children.Count; k++)
                {
                    SceneNode container = child.Children[k];

                    SceneNode circle = container.Children[0];
                    UIInput uiInput = _uiInput[k];

                    //the monkey's matrices
                    SceneNode monkey = _scene.Children[0];
                    float4x4 model = monkey.GetGlobalTransformation();
                    float4x4 projection = perspective;

                    float4x4 mvpMonkey = projection * view * model;

                    float3 clipPos = float4x4.TransformPerspective(mvpMonkey, uiInput.Position); //divides by 2
                    float2 canvasPosCircle = new float2(clipPos.x, clipPos.y) * 0.5f + 0.5f;

                    canvasPosCircle.x *= _canvasWidth;
                    canvasPosCircle.y *= _canvasHeight;
                    uiInput.CircleCanvasPos = canvasPosCircle;

                    float2 pos = new float2(uiInput.CircleCanvasPos.x - (uiInput.Size.x / 2), uiInput.CircleCanvasPos.y - (uiInput.Size.y / 2)); //we want the lower left point of the rect that encloses the
                    circle.GetComponent<RectTransform>().Offsets = UIElementPosition.CalcOffsets(AnchorPos.MIDDLE, pos, _canvasHeight, _canvasWidth, uiInput.Size);

                    //1.1   Check if circle is visible

                    PickResult newPick = _scenePicker.Pick(RC, new float2(clipPos.x, clipPos.y)).ToList().OrderBy(pr => pr.ClipPos.z).FirstOrDefault();

                    if (newPick != null && uiInput.AffectedTriangles[0] == newPick.Triangle) //VISIBLE
                    {
                        uiInput.IsVisible = true;

                        ShaderEffect shaderEffect = circle.GetComponent<ShaderEffect>();
                        shaderEffect.SetDiffuseAlphaInShaderEffect(UIHelper.alphaVis);
                    }
                    else
                    {
                        uiInput.IsVisible = false;
                        ShaderEffect shaderEffect = circle.GetComponent<ShaderEffect>();
                        shaderEffect.SetDiffuseAlphaInShaderEffect(UIHelper.alphaInv);

                    }

                    //1.2   Calculate annotation positions without intersections.
                    if (!uiInput.CircleCanvasPos.Equals(uiInput.CircleCanvasPosCache))
                    {
                        float yPosScale = uiInput.CircleCanvasPos.y / _canvasHeight;
                        yPosScale = (yPosScale - 0.5f) * 2f;
                        uiInput.AnnotationCanvasPos.y = uiInput.CircleCanvasPos.y - (UIHelper.AnnotationDim.y / 2) + (2 * UIHelper.AnnotationDim.y * yPosScale);

                        if (uiInput.CircleCanvasPos.x > _canvasWidth / 2) //RIGHT
                            uiInput.AnnotationCanvasPos.x = UIHelper.CanvasWidthInit - UIHelper.AnnotationDim.x - UIHelper.AnnotationDistToLeftOrRightEdge;
                        else
                            uiInput.AnnotationCanvasPos.x = UIHelper.AnnotationDistToLeftOrRightEdge;
                    }
                    _uiInput[k] = uiInput;
                }

                // 2.   Find intersecting annotations and correct their position in _uiInput.
                //      Disable rendering of annotation if its corresponding circle is not visible.
                for (int k = 0; k < child.Children.Count; k++)
                {
                    SceneNode container = child.Children[k];
                    SceneNode annotation = container.Children[1];
                    UIInput uiInput = _uiInput[k];

                    if (uiInput.IsVisible)
                    {
                        if (!uiInput.CircleCanvasPos.Equals(uiInput.CircleCanvasPosCache))
                        {
                            Dictionary<int, float2> intersectedAnnotations = new Dictionary<int, float2>();
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
                    UIInput uiInput = _uiInput[k];

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

            _sceneRenderer.Render(RC);

            RC.Projection = _canvasRenderMode == CanvasRenderMode.SCREEN ? orthographic : perspective;
            _guiRenderer.Render(RC);

            Present();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
            _resizeScaleFactor = new float2((100 / _initWidth * Width) / 100, (100 / _initHeight * Height) / 100);

            _canvasHeight = UIHelper.CanvasHeightInit * _resizeScaleFactor.y;
            _canvasWidth = UIHelper.CanvasWidthInit * _resizeScaleFactor.x;

        }

        private SceneContainer CreateGui()
        {
            float canvasScaleFactor = _initWidth / _canvasWidth;
            float borderScaleFactor = 1;
            if (_canvasRenderMode == CanvasRenderMode.SCREEN)
            {
                borderScaleFactor = canvasScaleFactor;
            }

            GUIButton btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            Texture guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            TextureNode fuseeLogo = new TextureNode(
                "fuseeLogo",
                UIHelper.VsTex,
                UIHelper.PsTex,
                guiFuseeLogo,
                UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_LEFT),
                UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(0, _canvasHeight - 0.5f), _canvasHeight, _canvasWidth, new float2(1.75f, 0.5f)),
                float2.One);
            fuseeLogo.AddComponent(btnFuseeLogo);

            SceneNode markModelContainer = new SceneNode
            {
                Name = "MarkModelContainer",
            };

            CanvasNode canvas = new CanvasNode(
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
                UIInput item = _uiInput[i];
                if (item.AnnotationKind != UIHelper.AnnotationKind.CONFIRMED)
                {
                    UIHelper.CreateAndAddCircleAnnotationAndLine(markModelContainer, item.AnnotationKind, item.Size, _uiInput[i].AnnotationCanvasPos, borderScaleFactor,
                    "#" + i + " " + item.SegmentationClass + ", " + item.Probability.ToString(CultureInfo.GetCultureInfo("en-gb")));
                }
                else
                {
                    UIHelper.CreateAndAddCircleAnnotationAndLine(markModelContainer, item.AnnotationKind, item.Size, _uiInput[i].AnnotationCanvasPos, borderScaleFactor,
                   "#" + i + " " + item.SegmentationClass);
                }
            }

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
            ShaderEffect effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, new float4(0.0f, 0.0f, 0.0f, 1f));
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 0.8f);
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            ShaderEffect effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<ShaderEffect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, float4.One);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 1f);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }

        private void UpdateAnnotationOffsets(SceneNode sncAnnotation, UIInput input)
        {
            if (input.CircleCanvasPos.x <= _canvasWidth / 2)
            {
                //LEFT
                sncAnnotation.GetComponent<RectTransform>().Anchors = UIElementPosition.GetAnchors(AnchorPos.DOWN_DOWN_LEFT);

                sncAnnotation.GetComponent<RectTransform>().Offsets = UIElementPosition.CalcOffsets(
                    AnchorPos.DOWN_DOWN_LEFT, input.AnnotationCanvasPos,
                    UIHelper.CanvasHeightInit, UIHelper.CanvasWidthInit, UIHelper.AnnotationDim);
            }
            else
            {
                //RIGHT
                sncAnnotation.GetComponent<RectTransform>().Anchors = UIElementPosition.GetAnchors(AnchorPos.DOWN_DOWN_RIGHT);

                sncAnnotation.GetComponent<RectTransform>().Offsets = UIElementPosition.CalcOffsets(
                    AnchorPos.DOWN_DOWN_RIGHT, input.AnnotationCanvasPos,
                    UIHelper.CanvasHeightInit, UIHelper.CanvasWidthInit, UIHelper.AnnotationDim);
            }
        }

        private void DrawLine(SceneNode sncLine, UIInput uiInput)
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
                        new float3(uiInput.AnnotationCanvasPos.x + UIHelper.AnnotationDim.x, uiInput.AnnotationCanvasPos.y + UIHelper.AnnotationDim.y/2,0),
                        new float3(uiInput.CircleCanvasPos.x - (uiInput.Size.x/2), uiInput.CircleCanvasPos.y,0)
                    };
                }
                else
                {
                    //RIGHT
                    float posX = _canvasWidth - UIHelper.AnnotationDim.x - UIHelper.AnnotationDistToLeftOrRightEdge;

                    linePoints = new List<float3>
                    {
                        new float3(posX, uiInput.AnnotationCanvasPos.y + UIHelper.AnnotationDim.y/2,0),
                        new float3(uiInput.CircleCanvasPos.x + (uiInput.Size.x/2), uiInput.CircleCanvasPos.y,0)
                    };
                }

                sncLine.GetComponent<RectTransform>().Offsets = UIElementPosition.CalcOffsets(AnchorPos.MIDDLE, new float2(0, 0), _canvasHeight, _canvasWidth, new float2(_canvasWidth, _canvasHeight));

                Line mesh = sncLine.GetComponent<Line>();

                if (mesh != null)
                {
                    Line newLine = new Line(linePoints, 0.0025f / _resizeScaleFactor.y, _canvasWidth, _canvasHeight);
                    mesh.Vertices = newLine.Vertices;
                    mesh.Normals = newLine.Normals;
                    mesh.Triangles = newLine.Triangles;
                    mesh.UVs = newLine.UVs;
                }
                else
                {
                    Line newLine = new Line(linePoints, 0.0025f / _resizeScaleFactor.y, _canvasWidth, _canvasHeight);
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

        private void CalculateNonIntersectingAnnotationPositions(ref UIInput input, ref Dictionary<int, float2> intersectedAnnotations, ref int iterations)
        {
            if (!input.IsVisible) return;

            int intersectionCount = 0;
            for (int i = 0; i < _uiInput.Count; i++)
            {
                UIInput counterpart = _uiInput[i];

                if (counterpart.Identifier == input.Identifier || !counterpart.IsVisible || intersectedAnnotations.ContainsKey(counterpart.Identifier))
                    continue;

                float halfAnnotationHeight = (UIHelper.AnnotationDim.y / 2f);
                float buffer = halfAnnotationHeight - (halfAnnotationHeight / 100f * 10f);
                //If we do not multiply by the resize scale factor the intersction test will return wrong results because AnnotationCanvasPos is in the range of the size of the initial canvas.
                bool intersect = UIHelper.DoesAnnotationIntersectWithAnnotation(input.AnnotationCanvasPos, _uiInput[i].AnnotationCanvasPos, new float2(0, buffer));

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
                float2 averagePos = new float2();

                for (int i = 0; i < intersectedAnnotations.Count; i++)
                    averagePos += intersectedAnnotations.ElementAt(i).Value;

                averagePos /= intersectedAnnotations.Count;

                for (int i = 0; i < intersectedAnnotations.Count; i++)
                {
                    int identifier = intersectedAnnotations.ElementAt(i).Key;
                    UIInput thisInput = _uiInput[identifier];
                    thisInput.AnnotationCanvasPos = averagePos;

                    int multiplier = System.Math.Abs(i - middleIndex);

                    //Distance between annotations is 0.5* AnnotationDim.y
                    if (intersectedAnnotations.Count % 2 == 0) //even
                    {
                        if (i == middleIndex - 1)
                            thisInput.AnnotationCanvasPos.y -= 0.75f * UIHelper.AnnotationDim.y;
                        else if (i == middleIndex)
                            thisInput.AnnotationCanvasPos.y += 0.75f * UIHelper.AnnotationDim.y;
                        else if (i > middleIndex)
                            thisInput.AnnotationCanvasPos.y += (0.75f * UIHelper.AnnotationDim.y) + (multiplier * (UIHelper.AnnotationDim.y + UIHelper.AnnotationDim.y / 2));
                        else if (i < middleIndex)
                            thisInput.AnnotationCanvasPos.y -= (0.75f * UIHelper.AnnotationDim.y) + ((multiplier - 1) * (UIHelper.AnnotationDim.y + UIHelper.AnnotationDim.y / 2));
                    }
                    else //odd
                    {
                        if (i > middleIndex)
                            thisInput.AnnotationCanvasPos.y += 0.5f * multiplier * UIHelper.AnnotationDim.y + (UIHelper.AnnotationDim.y * multiplier);
                        else if (i < middleIndex)
                            thisInput.AnnotationCanvasPos.y -= 0.5f * multiplier * UIHelper.AnnotationDim.y + (UIHelper.AnnotationDim.y * multiplier);
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
                UIInput uiInput = _uiInput[identifier];

                CalculateNonIntersectingAnnotationPositions(ref uiInput, ref intersectedAnnotations, ref iterations);
            }
        }
    }
}