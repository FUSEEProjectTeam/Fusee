using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using System.Collections.Generic;

namespace Fusee.Engine.Examples.UI.Core
{
    public class UI : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;

        private bool _keys;

        private Texture _bltDestinationTex;

       //Build a scene graph consisting out of a canvas and other UI elements.
        private SceneContainer CreateNineSliceScene()
        {
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        //Rotate canvas and all its children.
                        Name = "Null_Transform",
                        Components = new List<SceneComponentContainer>
                        {
                            new TransformComponent
                            {
                                Translation = new float3(0,0,0),
                                Rotation = new float3(0,45,0),
                                Scale = new float3(1,1,1)
                            }
                        },
                        Children = new List<SceneNodeContainer>
                        {
                            //Add canvas.
                            new SceneNodeContainer
                            {
                                Name = "Canvas",
                                Components = new List<SceneComponentContainer>
                                {
                                    new CanvasTransformComponent
                                    {
                                        Name = "Canvas_CanvasTransform",
                                        Size = new MinMaxRect
                                        {
                                            Min = new float2(-10,-5),
                                            Max = new float2(10,5)
                                        }
                                    }
                                },
                                Children = new List<SceneNodeContainer>
                                {
                                    new SceneNodeContainer
                                    {
                                        Name = "Canvas_XForm",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new XFormComponent
                                            {
                                                Name = "Canvas_XForm"
                                            },
                                            new ShaderEffectComponent
                                            {
                                                Effect = ShaderCodeBuilder.MakeShaderEffectFromMatComp(new MaterialComponent
                                                {
                                                    Diffuse = new MatChannelContainer {Color = new float3(1,0,0)},
                                                })
                                            },
                                            new Plane()
                                        }
                                    },
                                    //Simple Texture Node, contains a Blt"ed" texture.
                                    GUINodes.TextureNode(
                                        
                                        "Blt",
                                        //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                                        //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                                        new MinMaxRect
                                        {
                                            Min = new float2(0,0), //Anchor is in the lower left corner of the parent.
                                            Max = new float2(0,0) //Anchor is in the lower right corner of the parent
                                        },
                                        //Define Offset and therefor the size of the element.
                                        //Min: distance to this elements Min anchor.
                                        //Max: distance to this elements Max anchor.
                                        new MinMaxRect
                                        {
                                            Min = new float2(0,0),
                                            Max = new float2(5,5)
                                        },
                                        //Set the diffuse texture you want to use.
                                        _bltDestinationTex
                                    ),
                                    //Add nine sliced textures to canvas
                                    GUINodes.NineSliceNode
                                    (
                                        "Child1",
                                        //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                                        //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                                        new MinMaxRect
                                        {
                                            Min = new float2(0,0), //Anchor is in the lower left corner of the parent.
                                            Max = new float2(1,0) //Anchor is in the lower right corner of the parent
                                        },
                                        //Define Offset and therefor the size of the element.
                                        //Min: distance to this elements Min anchor.
                                        //Max: distance to this elements Max anchor.
                                        new MinMaxRect
                                        {
                                            Min = new float2(7.5f,0),
                                            Max = new float2(-7.5f,5)
                                        },
                                        //Set the diffuse texture you want to use.
                                        new Texture(AssetStorage.Get<ImageData>("Kitti.jpg")),
                                        //Choose in how many tiles you want to split the inner part of the texture. Use float2.one if you want it stretched.
                                        new float2(5,5),
                                        //Tell how many percent of the texture, seen from the edges, belongs to the border. Order: left, right, top, bottom.
                                        new float4(0.11f,0.11f,0.06f,0.17f),
                                        5
                                    ),
                                    GUINodes.NineSliceNode
                                    (
                                        "Child2",
                                        //In this setup the element will stay in the upper right corner of the parent and will not be stretched at all.
                                        new MinMaxRect
                                        {
                                            Min = new float2(1,1), //Anchor is in the upper right corner.
                                            Max = new float2(1,1) //Anchor is in the upper right corner.
                                        },
                                        new MinMaxRect
                                        {
                                            Min = new float2(-8,-4),
                                            Max = new float2(0,0)
                                        },
                                        new Texture(AssetStorage.Get<ImageData>("9SliceSprites-4.png")),
                                        new float2(2,3),
                                        new float4(0.1f,0.1f,0.1f,0.1f),
                                        2
                                    ),
                                    GUINodes.NineSliceNode
                                    (
                                        "Child3",
                                        //In this setup the element will stay in the upper left corner of the parent and will not be stretched at all.
                                        new MinMaxRect
                                        {
                                            Min = new float2(0,1), //Anchor is in the upper left corner.
                                            Max = new float2(0,1) //Anchor is in the upper left corner.
                                        },
                                        new MinMaxRect
                                        {
                                            Min = new float2(0,-1),
                                            Max = new float2(6,0)
                                        },
                                        new Texture(AssetStorage.Get<ImageData>("testTex.jpg")),
                                        new float2(5,1),
                                        new float4(0.1f,0.1f,0.1f,0.09f)
                                    )
                                }
                            }
                        }
                    }
                }
            };
        }

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            _bltDestinationTex = new Texture(AssetStorage.Get<ImageData>("holz.jpg"));
            var bltScrTex = new Texture(AssetStorage.Get<ImageData>("SuperMario.png"));
            _bltDestinationTex.Blt(50, 0, bltScrTex);

            // Set the scene by creating a scene graph
            _scene = CreateNineSliceScene();

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_scene);
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


            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 0, -15, 0, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRot;

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}