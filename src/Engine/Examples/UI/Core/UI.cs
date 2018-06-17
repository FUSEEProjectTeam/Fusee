using System.Collections.Generic;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Examples.UI.Core
{
    public class UI : RenderCanvas
    {
        protected readonly string GUIVS = @"
            
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
            varying vec2 vUV;
            varying vec3 vMVNormal;
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;
        
            void main() {
               
               vUV = fuUV;
               vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
               gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
            }";

        protected readonly string TEXTUREPS = 
        @"
            #version 120

            #ifdef GL_ES
                precision highp float;
            #endif

            varying vec3 vMVNormal;            
            varying vec2 vUV;            
            uniform mat4 FUSEE_MV;
            uniform sampler2D DiffuseTexture;
            uniform vec4 DiffuseColor;
            uniform float DiffuseMix;

            void main()
            {
                vec3 N = normalize(vMVNormal);
                vec3 L = normalize(vec3(0.0,0.0,-1.0));
                gl_FragColor = vec4(texture2D(DiffuseTexture, vUV) * DiffuseMix) * DiffuseColor *  max(dot(N, L), 0.0) ;
            }";

        protected readonly string NINESLICEVS = AssetStorage.Get<string>("nineSlice.vert");

        protected readonly string NINESLICETILEPS = AssetStorage.Get<string>("nineSliceTile.frag");
        
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;

        private bool _keys;

        private SceneContainer CreateAnchorTestScene()
        {
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = "Null_Transform",
                        Components = new List<SceneComponentContainer>
                        {
                            new TransformComponent
                            {
                                Scale = new float3(1,1,1),
                                Translation = new float3(0,5,0),
                                Rotation = new float3(0,45,0)
                            }
                        },
                        Children = new List<SceneNodeContainer>
                        {
                                new SceneNodeContainer
                                {
                                Name = "Canvas",
                                Components = new List<SceneComponentContainer>
                                {
                                    new CanvasTransformComponent
                                    {
                                        Name = "Canvas_CanvasTransform",
                                        CanvasRenderMode = CanvasRenderMode.WORLD,
                                        Size = new MinMaxRect
                                        {
                                            Min = new float2(-5,0),
                                            Max = new float2(5,3)
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
                                                    Diffuse = new MatChannelContainer {Color = new float3(0,1,0)},
                                                })
                                            },
                                            new Cube()
                                        }
                                    },

                                    new SceneNodeContainer
                                    {
                                        Name = "Child",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new RectTransformComponent
                                            {
                                                Name = "Child_RectTransform",
                                                Anchors = new MinMaxRect
                                                {
                                                    Min = new float2(0,0),
                                                    Max = new float2(1,0)
                                                },
                                                Offsets = new MinMaxRect
                                                {
                                                    Min = new float2(1,0),
                                                    Max = new float2(-1,2)
                                                }

                                             }
                                        },
                                        Children =  new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Child_XForm",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new XFormComponent
                                                    {
                                                        Name = "Child_XForm"
                                                    },
                                                    new ShaderEffectComponent
                                                    {
                                                        Effect = ShaderCodeBuilder.MakeShaderEffectFromMatComp(new MaterialComponent
                                                        {
                                                            Diffuse = new MatChannelContainer {Color = new float3(1,0,0)},
                                                        })
                                                    },
                                                    new Cube()
                                                },
                                                Children = new List<SceneNodeContainer>
                                                {
                                                    new SceneNodeContainer
                                                    {
                                                        Name = "GrandChild_RectTransform",
                                                        Components = new List<SceneComponentContainer>
                                                        {
                                                            new RectTransformComponent
                                                            {
                                                                Name = "GrandChild_RectTransform",
                                                                Anchors = new MinMaxRect
                                                                {
                                                                    Min = new float2(1,1),
                                                                    Max = new float2(1,1)
                                                                },
                                                                Offsets = new MinMaxRect
                                                                {
                                                                    Min = new float2(-1,-1),
                                                                    Max = new float2(0,0)
                                                                }
                                                            }
                                                        },
                                                       Children = new List<SceneNodeContainer>
                                                       {
                                                           new SceneNodeContainer
                                                           {
                                                               Name = "GrandChild_XForm",
                                                               Components = new List<SceneComponentContainer>
                                                               {
                                                                   new XFormComponent
                                                                   {
                                                                       Name = "GrandChild_XForm"
                                                                   },
                                                                   new ShaderEffectComponent
                                                                   {
                                                                       Effect = ShaderCodeBuilder.MakeShaderEffectFromMatComp(new MaterialComponent
                                                                       {
                                                                           Diffuse = new MatChannelContainer {Color = ColorUint.Yellow.Tofloat3()},
                                                                       })
                                                                   },
                                                                   new Cube()
                                                               }
                                                           }
                                                       }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
        private SceneContainer CreateImageTestScene()
        {
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = "Null_Transform",
                        Children = new List<SceneNodeContainer>
                        {
                            new SceneNodeContainer
                            {
                                Name = "Canvas",
                                Components = new List<SceneComponentContainer>
                                {
                                    new CanvasTransformComponent
                                    {
                                        Name = "Canvas_CanvasTransform",
                                        CanvasRenderMode = CanvasRenderMode.WORLD,
                                        Size = new MinMaxRect
                                        {
                                            Min = new float2(-5,0),
                                            Max = new float2(5,3)
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
                                    new SceneNodeContainer
                                    {
                                        Name = "Child",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new RectTransformComponent
                                            {
                                                Name = "Child_RectTransform",
                                                Anchors = new MinMaxRect
                                                {
                                                    Min = new float2(0,0),
                                                    Max = new float2(1,0)
                                                },
                                                Offsets = new MinMaxRect
                                                {
                                                    Min = new float2(1,0),
                                                    Max = new float2(-1,2)
                                                }

                                             }
                                        },
                                        Children =  new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Child_XForm",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new XFormComponent
                                                    {
                                                        Name = "Child_XForm"
                                                    },
                                                    new ShaderEffectComponent{Effect = new ShaderEffect(new[]
                                                        {
                                                            new EffectPassDeclaration
                                                            {
                                                                VS = GUIVS,
                                                                PS = TEXTUREPS,
                                                                StateSet = new RenderStateSet
                                                                {
                                                                    AlphaBlendEnable = true,
                                                                    SourceBlend = Blend.SourceAlpha,
                                                                    DestinationBlend = Blend.InverseSourceAlpha,
                                                                    BlendOperation = BlendOperation.Add,
                                                                    ZEnable = false
                                                                }
                                                            }
                                                        },
                                                        new[]
                                                        {
                                                            new EffectParameterDeclaration {Name = "DiffuseTexture", Value = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"))},
                                                            new EffectParameterDeclaration {Name = "DiffuseColor", Value = float4.One},
                                                            new EffectParameterDeclaration {Name = "DiffuseMix", Value = 1f}
                                                        })},
                                                    new Plane()
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private SceneContainer CreateNineSliceTestScene()
        {
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = "Null_Transform",
                        Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent()
                            {
                                Translation = new float3(0,0,0),
                                Rotation = new float3(0,45,0),
                                Scale = new float3(1,1,1)
                            }
                        },
                        Children = new List<SceneNodeContainer>
                        {
                            new SceneNodeContainer
                            {
                                Name = "Canvas",
                                Components = new List<SceneComponentContainer>
                                {
                                    new CanvasTransformComponent
                                    {
                                        Name = "Canvas_CanvasTransform",
                                        CanvasRenderMode = CanvasRenderMode.WORLD,
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
                                    new SceneNodeContainer
                                    {
                                        Name = "Child1",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new RectTransformComponent
                                            {
                                                Name = "Child1_RectTransform",
                                                Anchors = new MinMaxRect
                                                {
                                                    Min = new float2(0,0),
                                                    Max = new float2(1,0)
                                                },
                                                Offsets = new MinMaxRect
                                                {
                                                    Min = new float2(7.5f,0),
                                                    Max = new float2(-7.5f,5)
                                                }

                                             }
                                        },
                                        Children =  new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Child1_XForm",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new XFormComponent()
                                                    {
                                                        Name = "Child1_XForm",
                                                    },
                                                    new ShaderEffectComponent{Effect = new ShaderEffect(new[]
                                                        {
                                                            new EffectPassDeclaration
                                                            {
                                                                VS = NINESLICEVS,
                                                                PS = NINESLICETILEPS,
                                                                StateSet = new RenderStateSet
                                                                {
                                                                    AlphaBlendEnable = true,
                                                                    SourceBlend = Blend.SourceAlpha,
                                                                    DestinationBlend = Blend.InverseSourceAlpha,
                                                                    BlendOperation = BlendOperation.Add,
                                                                    ZEnable = false
                                                                }
                                                            }
                                                        },
                                                        new[]
                                                        {
                                                            new EffectParameterDeclaration {Name = "DiffuseTexture", Value = new Texture(AssetStorage.Get<ImageData>("Kitti.jpg"))},
                                                            new EffectParameterDeclaration {Name = "DiffuseColor", Value = float4.One},
                                                            new EffectParameterDeclaration {Name = "DiffuseMix", Value = 1f},
                                                            new EffectParameterDeclaration {Name = "Tile", Value = new float2(5,5)},
                                                            new EffectParameterDeclaration {Name = "borders", Value = new float4(0.11f,0.11f,0.06f,0.17f)},
                                                            new EffectParameterDeclaration {Name = "borderThickness", Value = 5f},
                                                            new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                                                            new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                                                            new EffectParameterDeclaration {Name = "FUSEE_V", Value = float4x4.Identity},
                                                            new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity}
                                                        })},
                                                    new NineSlicePlane()
                                                }
                                            }
                                        }
                                    },
                                    new SceneNodeContainer
                                    {
                                        Name = "Child2",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new RectTransformComponent
                                            {
                                                Name = "Child2_RectTransform",
                                                Anchors = new MinMaxRect
                                                {
                                                    Min = new float2(1,1),
                                                    Max = new float2(1,1)
                                                },
                                                Offsets = new MinMaxRect
                                                {
                                                    Min = new float2(-8,-4),
                                                    Max = new float2(0,0)
                                                }

                                             }
                                        },
                                        Children =  new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Child2_XForm",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new XFormComponent()
                                                    {
                                                        Name = "Child2_XForm",
                                                    },
                                                    new ShaderEffectComponent{Effect = new ShaderEffect(new[]
                                                        {
                                                            new EffectPassDeclaration
                                                            {
                                                                VS = NINESLICEVS,
                                                                PS = NINESLICETILEPS,
                                                                StateSet = new RenderStateSet
                                                                {
                                                                    AlphaBlendEnable = true,
                                                                    SourceBlend = Blend.SourceAlpha,
                                                                    DestinationBlend = Blend.InverseSourceAlpha,
                                                                    BlendOperation = BlendOperation.Add,
                                                                    ZEnable = false
                                                                }
                                                            }
                                                        },
                                                        new[]
                                                        {
                                                            new EffectParameterDeclaration {Name = "DiffuseTexture", Value = new Texture(AssetStorage.Get<ImageData>("9SliceSprites-4.png"))},
                                                            new EffectParameterDeclaration {Name = "DiffuseColor", Value = float4.One},
                                                            new EffectParameterDeclaration {Name = "Tile", Value = new float2(2,3)},
                                                            new EffectParameterDeclaration {Name = "DiffuseMix", Value = 1f},
                                                            new EffectParameterDeclaration {Name = "borders", Value = new float4(0.1f,0.1f,0.1f,0.1f)},
                                                            new EffectParameterDeclaration {Name = "borderThickness", Value = 1f},
                                                            new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                                                            new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                                                            new EffectParameterDeclaration {Name = "FUSEE_V", Value = float4x4.Identity},
                                                            new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity}
                                                        })},
                                                    new NineSlicePlane()
                                                }
                                            }
                                        }
                                    },
                                    new SceneNodeContainer
                                    {
                                        Name = "Child2",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new RectTransformComponent
                                            {
                                                Name = "Child2_RectTransform",
                                                Anchors = new MinMaxRect
                                                {
                                                    Min = new float2(0,1),
                                                    Max = new float2(0,1)
                                                },
                                                Offsets = new MinMaxRect
                                                {
                                                    Min = new float2(0,-1),
                                                    Max = new float2(6,0)
                                                }

                                             }
                                        },
                                        Children =  new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Child2_XForm",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new XFormComponent()
                                                    {
                                                        Name = "Child2_XForm",
                                                    },
                                                    new ShaderEffectComponent{Effect = new ShaderEffect(new[]
                                                        {
                                                            new EffectPassDeclaration
                                                            {
                                                                VS = NINESLICEVS,
                                                                PS = NINESLICETILEPS,
                                                                StateSet = new RenderStateSet
                                                                {
                                                                    AlphaBlendEnable = true,
                                                                    SourceBlend = Blend.SourceAlpha,
                                                                    DestinationBlend = Blend.InverseSourceAlpha,
                                                                    BlendOperation = BlendOperation.Add,
                                                                    ZEnable = false
                                                                }
                                                            }
                                                        },
                                                        new[]
                                                        {
                                                            new EffectParameterDeclaration {Name = "DiffuseTexture", Value = new Texture(AssetStorage.Get<ImageData>("testTex.jpg"))},
                                                            new EffectParameterDeclaration {Name = "DiffuseColor", Value = float4.One},
                                                            new EffectParameterDeclaration {Name = "Tile", Value = new float2(5,1)},
                                                            new EffectParameterDeclaration {Name = "DiffuseMix", Value = 1f},
                                                            new EffectParameterDeclaration {Name = "borders", Value = new float4(0.1f,0.1f,0.1f,0.09f)},
                                                            new EffectParameterDeclaration {Name = "borderThickness", Value = 2f},
                                                            new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                                                            new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                                                            new EffectParameterDeclaration {Name = "FUSEE_V", Value = float4x4.Identity},
                                                            new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity}
                                                        })},
                                                    new NineSlicePlane()
                                                }
                                            }
                                        }
                                    }
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

            // Load the rocket model
            //_scene = CreateAnchorTestScene();
            _scene = CreateNineSliceTestScene();

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


            //Viewport transformation
            // Create the camera matrix and set it as the current ModelView transformation 

            //Worldspace


            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 0, -15, 0, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRot;

            //View Space

            // Pick it !
            /*
            if (Mouse.LeftButton)
            {
                PickAtPosition(_scene.Children,
                    Mouse.Position * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1),
                    RC.Projection * RC.ModelView);
            }
            */

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