using System.Collections.Generic;
using System.ComponentModel;
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

        private bool _keys;

        //private SceneContainer BuildScene()
        //{
        //    var sphere = new Sphere(24,16);

        //    //var line = new NineSlicePlane();
        //    //var start = new float3(0, 0, 0);
        //    //var end = new float3(2, 2, 2);
        //    //var middle = (start + end) / 2; //translate to
        //    //var scaleX = (start - end).Length;
        //    //var width = 0.2f;

        //    //var dirVec = (end - start);
        //    //dirVec.Normalize();
        //    //var rotAxis = float3.Cross(float3.UnitX, dirVec);
        //    //var dot = float3.Dot(float3.UnitX, dirVec);

        //    //var rot = Quaternion.FromToRotation(float3.UnitX, dirVec);
        //    //var eulerRot = Quaternion.QuaternionToEuler(rot,true);

        //    //var quat = Quaternion.EulerToQuaternion(new float3(45,0,45),true);
        //    //var e = Quaternion.QuaternionToEuler(quat);

        //    //float3 a = float3.Cross(dirVec, float3.UnitX);
        //    //a.Normalize();
        //    //var w = 1 + float3.Dot(dirVec,float3.UnitX);
        //    //Quaternion q = new Quaternion()
        //    //{
        //    //    xyz = a,
        //    //    w = w
        //    //};
        //    //q.Normalize();
        //    //var eulerRot = Quaternion.QuaternionToEuler(q);

        //    return new SceneContainer()
        //    {
        //        Children = new List<SceneNodeContainer>()
        //        {
        //            new SceneNodeContainer()
        //            {
        //                Components = new List<SceneComponentContainer>()
        //                {
        //                    new TransformComponent()
        //                    {
        //                        Name = "SphereTransform",
        //                        Rotation = new float3(0,0,0),
        //                        Translation = start,
        //                        Scale = new float3(0.25f,0.25f,0.25f)
                                
        //                    },
        //                    new ShaderEffectComponent()
        //                    {
        //                        Effect = ShaderCodeBuilder.MakeShaderEffect(new float3(0,0,1), new float3(1,1,1), 20)
        //                    },
        //                    new Cube()
        //                }
        //            },
        //            new SceneNodeContainer()
        //            {
        //                Components = new List<SceneComponentContainer>()
        //                {
        //                    new TransformComponent()
        //                    {
        //                        Name = "SphereTransform",
        //                        Rotation = new float3(0,0,0),
        //                        Translation = end,
        //                        Scale = new float3(0.25f,0.25f,0.25f)

        //                    },
        //                    new ShaderEffectComponent()
        //                    {
        //                        Effect = ShaderCodeBuilder.MakeShaderEffect(new float3(0,0,1), new float3(1,1,1), 20)
        //                    },
        //                    new Cube()
        //                }
        //            },
        //            new SceneNodeContainer()
        //            {
        //                Components = new List<SceneComponentContainer>()
        //                {
        //                    new TransformComponent()
        //                    {
        //                        Name = "SphereTransform",
        //                        Rotation = eulerRot,
        //                        Translation = middle,
        //                        Scale = new float3(scaleX, width, 1)

        //                    },
        //                    new ShaderEffectComponent()
        //                    {
        //                        Effect = ShaderCodeBuilder.MakeShaderEffect(new float3(0,1,1), new float3(1,1,1), 20)
        //                    },
        //                    line
        //                }
        //            }
        //        }
        //    };
        //}

        

        // Init is called on startup. 
        public override void Init()
        {

            _gui = CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the back buffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.2f, 0.2f, 0.2f, 1);

            // Load the rocket model
            _scene = AssetStorage.Get<SceneContainer>("Blase_Final2_mod_inv3.fus");
            

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
                    var curDamp = (float) System.Math.Exp(-Damping * Time.DeltaTime);
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

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the back buffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float) Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;

            _sih.Projection = projection;
        }

        private SceneContainer CreateGui()
        {
            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var latoFontMap = new FontMap(fontLato, 12);
            var canvasRenderMode = CanvasRenderMode.SCREEN;
            var canvasWidth = 16;
            var canvasHeight = 9;
            
            var canvasScaleFactor = 0.1f;
            float textSize = 2;
            float borderScaleFactor = 1;
            if (canvasRenderMode == CanvasRenderMode.SCREEN)
            {
                textSize *= canvasScaleFactor;
                borderScaleFactor = 0.01f;
            }

            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");

            var btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var textAnnotationGreen = new TextNodeContainer(
                "#1 Karzinom, 0.978",
                "annotation text",
                vsTex,
                psTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                new MinMaxRect
                {
                    Min = new float2(1f, 0.5f),
                    Max = new float2(-0.2f, -0.5f)
                },
                latoFontMap,
                ColorUint.Tofloat4(ColorUint.Black), textSize);            

            var annotationGreen = new TextureNodeContainer(
                "AnnotationGreen",
                AssetStorage.Get<string>("nineSlice.vert"),
                AssetStorage.Get<string>("nineSliceTile.frag"),
                //Set the diffuse texture you want to use.
                new Texture(AssetStorage.Get<ImageData>("frame_green.png")),
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                new MinMaxRect
                {
                    Min = new float2(0, 0), //Anchor is in the lower left corner of the parent.
                    Max = new float2(1, 0) //Anchor is in the lower right corner of the parent
                },
                //Define Offset and therefor the size of the element.
                //Min: distance to this elements Min anchor.
                //Max: distance to this elements Max anchor.
                new MinMaxRect
                {
                    Min = new float2(5.5f, 0),
                    Max = new float2(-5.5f, 0.55f)
                },
                //Choose in how many tiles you want to split the inner part of the texture. Use float2.one if you want it stretched.
                new float2(1, 1),
                //Tell how many percent of the texture, seen from the edges, belongs to the border. Order: left, right, top, bottom.
                new float4(0.09f, 0.09f, 0.09f, 0.09f),
                100,8,8,8,
                borderScaleFactor

            ){ Children = new List<SceneNodeContainer> { textAnnotationGreen } };
                        

            var circleNodeContainer = new SceneNodeContainer
            {
                Components = new List<SceneComponentContainer>
                {
                    new RectTransformComponent
                    {
                        Name = "circle" + "_RectTransform",
                        Anchors = new MinMaxRect
                        {
                            Min = new float2(0, 1), //Anchor is in the lower left corner of the parent.
                            Max = new float2(0, 1) //Anchor is in the lower right corner of the parent
                        },
                        Offsets = new MinMaxRect
                        {
                            Min = new float2(2, -1.5f),
                            Max = new float2(2.5f, -1)
                        }
                    },
                    new XFormComponent
                    {
                        Name = "circle" + "_XForm",
                    },
                    new ShaderEffectComponent()
                    {
                        Effect = ShaderCodeBuilder.MakeShaderEffect(new float3(0,0,1), new float3(1,1,1), 20)
                    },
                    new Circle(false, 30,100,0.04f)
                }
            };

            var guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            var fuseeLogo = new TextureNodeContainer(
                "fuseeLogo",
                vsTex,
                psTex,
                //Set the diffuse texture you want to use.
                guiFuseeLogo,
                //_fontMap.Image,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                new MinMaxRect
                {
                    Min = new float2(0, 1), //Anchor is in the lower left corner of the parent.
                    Max = new float2(0, 1) //Anchor is in the lower right corner of the parent
                },
                //Define Offset and therefor the size of the element.
                //Min: distance to this elements Min anchor.
                //Max: distance to this elements Max anchor.
                new MinMaxRect
                {
                    Min = new float2(0, -0.5f),
                    Max = new float2(1.75f, 0)
                });
            fuseeLogo.AddComponent(btnFuseeLogo); 

            var canvas = new CanvasNodeContainer(
                "Canvas",
                CanvasRenderMode.SCREEN,
                new MinMaxRect
                {
                    Min = new float2(-canvasWidth/2, -canvasHeight/2),
                    Max = new float2(canvasWidth / 2, canvasHeight / 2)
                }, canvasScaleFactor
            )
            {
                Children = new List<SceneNodeContainer>()
                {
                    //Simple Texture Node, contains the fusee logo.
                    fuseeLogo,                    
                    circleNodeContainer,
                    annotationGreen
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