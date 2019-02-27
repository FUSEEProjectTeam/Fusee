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
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
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
        private float2[] _circleCanvasPositions;
        private float2[] _annotationCanvasPositions;

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

            _dummyPositions = new float3[NumberOfAnnotations];
            _circleCanvasPositions = new float2[NumberOfAnnotations];
            _annotationCanvasPositions = new []
            {
                //posOnParent = min offset = lower left corner of the rect transform
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

            for (var i = 0; i < NumberOfAnnotations; i++)
            {
                var vertIndex = rnd.Next(monkey.Vertices.Length);
                _dummyPositions[i] = (monkey.Vertices[vertIndex]);
                _circleCanvasPositions[i] = (new float2(monkey.Vertices[vertIndex].x, monkey.Vertices[vertIndex].y));
            }

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

            var projection = float4x4.CreatePerspectiveFieldOfView(_fovy, _aspectRatio, ZNear, ZFar);
            RC.Projection = projection;

            //UPDATE Circle pos
            var circleDim = new float2(0.65f, 0.65f);

            for (var i = 0; i < _dummyPositions.Length; i++)
            {
                var clipPos = _dummyPositions[i].TransformPerspective(RC.ModelViewProjection); //divides by w
                var pos = new float2(clipPos.x, clipPos.y)*0.5f + 0.5f;

                pos.x *= _canvasWidth;
                pos.y *= _canvasHeight;
                _circleCanvasPositions[i] = pos;
            }

            var canvas = _gui.Children[0];
            var circleCount = 0;
            foreach (var child in canvas.Children)
            {
                if (child.Name.Contains("Circle"))
                {
                    var pos = new float2(_circleCanvasPositions[circleCount].x - (circleDim.x / 2), _circleCanvasPositions[circleCount].y - (circleDim.y / 2)); //we want the lower left point of the rect that encloses the
                    child.GetComponent<RectTransformComponent>().Offsets = GuiHelper.CalcOffsets(GuiHelper.AnchorPos.MIDDLE, pos, _canvasHeight, _canvasWidth, circleDim);

                    circleCount++;
                }

                if (child.Name.Contains("line"))
                {
                    //TODO: insert new line (mesh component) to SceneNodeContainer

                    child.GetComponent<RectTransformComponent>().Offsets = GuiHelper.CalcOffsets(GuiHelper.AnchorPos.MIDDLE, new float2(0, 0), _canvasHeight, _canvasWidth, new float2(_canvasWidth, _canvasHeight));

                    var annotationPos = _annotationCanvasPositions[0];
                    var circlePos = _circleCanvasPositions[0];
                                       

                    var lineGreenPoints = new List<float3>()
                    {
                        new float3(annotationPos.x + GuiHelper.AnnotationDim.x, annotationPos.y + GuiHelper.AnnotationDim.y/2,0),
                        new float3(circlePos.x - (circleDim.x/2), circlePos.y,0)
                    };
                    //var line = new Line(lineGreenPoints, 0.0025f/_resizeScaleFactor.y,_canvasWidth,_canvasHeight);
                    //var mesh = child.GetComponent<Line>();
                    //if (mesh == null)
                    //    child.AddComponent(line);
                    //else
                    //{
                    //    mesh.Vertices = line.Vertices;
                    //    mesh.Normals = line.Normals;
                    //    mesh.Triangles = line.Triangles;
                    //    //mesh.UVs = line.UVs;
                    //}
                }
            }

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

            // Swap buffers: Show the contents of the back buffer (containing the currently rendered frame) on the front buffer.
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
        }

        //TODO: not working - depth value is always 0
        private bool IsVertVisible(float3 clipPos)
        {
            var pixelPosX = (int)System.Math.Floor((clipPos.x + 1) / (2.0f / Width));
            var pixelPosY = (int)System.Math.Floor((clipPos.y - 1) / (-2.0f / Height));
            var depthBufferValue = RC.GetPixelDepth(pixelPosX,pixelPosY);
            //var depthBufferValue1 = RC.GetPixelDepth((int)System.Math.Floor(Width/2f), (int)System.Math.Floor(Height / 2f));
            //var depthBufferValue2 = RC.GetPixelDepth((int)System.Math.Floor(Width / 2f)-1, (int)System.Math.Floor(Height / 2f)-1);
            //var colValue1 = RC.GetPixelColor((int)System.Math.Floor(Width / 2f), (int)System.Math.Floor(Height / 2f),5,5);

            var clipZValue = clipPos.z;

            return System.Math.Abs(depthBufferValue - clipZValue) < 0.001f;
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

            //TODO: write method for creating lines.
            var lineGreenPoints = new List<float3>()
            {
               new float3(-0.5f,0,0),
                new float3(0.5f,0,0)
            };

            var lineThickness = 0.02f;

            var lineGreen = new SceneNodeContainer()
            {
                Name = "lineGreen",
                Components = new List<SceneComponentContainer>
                {
                    new RectTransformComponent
                    {
                        Name = "lineGreen" + "_RectTransform",
                        Anchors = new MinMaxRect
                        {
                            Min = new float2(0.5f, 0.5f),
                            Max = new float2(0.5f, 0.5f)
                        },
                        Offsets = GuiHelper.CalcOffsets(GuiHelper.AnchorPos.MIDDLE, new float2(0,0), _canvasHeight, _canvasWidth, new float2(_canvasWidth,_canvasHeight)),
                    },
                    new XFormComponent
                    {
                        Name = "lineGreen" + "_XForm",
                    },
                    new ShaderEffectComponent()
                    {
                        Effect = ShaderCodeBuilder.MakeShaderEffect(new float3(0.14117f, 0.76078f, 0.48627f), new float3(1,1,1), 20, 0)
                    },
                    //new Line(lineGreenPoints,lineThickness),
                    //insert mesh...later!
                } //TODO: Line does not scale when resizing - add plane to observe behavoiur. Maybe use "stretch" anchors.
            };

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
                GuiHelper.CalcOffsets(GuiHelper.AnchorPos.TOP_TOP_LEFT, new float2(0, _canvasHeight-0.5f), _canvasHeight, _canvasWidth, new float2(1.75f, 0.5f)));
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
                    circleGray,
                    lineGreen
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