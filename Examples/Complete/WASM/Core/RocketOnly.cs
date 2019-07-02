using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.RocketOnly.Core
{

    

    [FuseeApplication(Name = "FUSEE Rocket Only Example", Description = "An even simpler example.")]
    public class RocketOnly : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;

        private TransformComponent _cubeTransform;
        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private float _fovy = M.PiOver4;

        private SceneRenderer _guiRenderer;

        private bool _keys;

        public string VertexShader = "";
        public string PixelShader = "";
        public ImageData CurrentTex;

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            /*
            RC.ClearColor = new float4(0.5f, 1, 0.25f, 1);

            _scene = new SceneContainer();
            _scene.Children = new List<SceneNodeContainer>();

            SceneNodeContainer cubeNode = new SceneNodeContainer();
            _scene.Children.Add(cubeNode);

            _cubeTransform = new TransformComponent
            {
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0),
                Rotation = new float3(0, 0, 0)
            };

            var cubeShader = new ShaderEffectComponent
            {
                Effect = SimpleMeshes.MakeShaderEffect(new float3(1, 0, 0), new float3(1, 1, 1), 4)
            };
            var cubeMesh = SimpleMeshes.CreateCuboid(new float3(10, 10, 10));

            cubeNode.Components = new List<SceneComponentContainer>();
            cubeNode.Components.Add(_cubeTransform);
            cubeNode.Components.Add(cubeShader);
            cubeNode.Components.Add(cubeMesh);

            RC.View = float4x4.Invert(float4x4.CreateRotationY(0.4f) *  float4x4.CreateRotationX(0.3f) * float4x4.CreateTranslation(0, 0, -25));

            _sceneRenderer = new SceneRenderer(_scene);
            */

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Load the rocket model
            // if (_rocketScene == null)
            //   _rocketScene = AssetStorage.Get<SceneContainer>("FUSEERocket.fus");
        }

        public SceneContainer RocketScene
        {
            get => _rocketScene;
            set
            {
                _rocketScene = value;


                var mySFX = new ShaderEffect(new[]
                    {
                        new EffectPassDeclaration
                        {
                            VS = VertexShader,// AssetStorage.Get<string>("VertexShader.vert"),
                            PS = PixelShader, //AssetStorage.Get<string>("PixelShader.frag"),
                            StateSet = new RenderStateSet
                            {
                                ZEnable = true,
                                AlphaBlendEnable = true
                            }
                        }
                    },
                    new List<EffectParameterDeclaration>
                    {
                        new EffectParameterDeclaration
                        {
                            Name = "FUSEE_MVP",
                            Value = float4x4.Identity
                        },
                        new EffectParameterDeclaration
                        {
                            Name = "FUSEE_ITMV",
                            Value = float4x4.Identity
                        },
                        new EffectParameterDeclaration
                        {

                            Name = "DiffuseTexture",
                            Value = new Texture(CurrentTex)
                        },
                        new EffectParameterDeclaration
                        {
                            Name = "DiffuseColor",
                            Value = new float4(.6f, .4f,.5f,1)
                        },
                        new EffectParameterDeclaration
                        {
                            Name = "DiffuseMix",
                            Value = 0.75f
                        }
                    }) ;
            

                _rocketScene = new SceneContainer
                {
                    Children = new List<SceneNodeContainer>
                    {
                        new SceneNodeContainer
                        {
                            Components = new List<SceneComponentContainer>
                            {
                                new ProjectionComponent(ProjectionMethod.PERSPECTIVE, 0.1f, 1000f, M.PiOver4),
                                new TransformComponent
                                {
                                    Translation = float3.UnitY * 2,
                                    Scale = float3.One * 2
                                },
                                new ShaderEffectComponent
                                {
                                    Effect = mySFX
                                },
                                new Cube()
                            }
                        }
                    }
                };

                //Add resize delegate
                var projComp = _rocketScene.Children[0].GetComponent<ProjectionComponent>();
                AddResizeDelegate(delegate { projComp.Resize(Width, Height); });
                _sceneRenderer = new SceneRenderer(_rocketScene);
                Diagnostics.Log("Rocket Set");
                projComp.Resize(Width, Height);
            }
        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            /*
            // Diagnostics.Log(Time.DeltaTime);
            // Diagnostics.Log($"Windowsize: {Width} x {Height}");
            Diagnostics.Log($"Active: {Touch.ActiveTouchpoints}. Pos[0]: {Touch.GetPosition(TouchPoints.Touchpoint_0)}");
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            _sceneRenderer.Render(RC);

            Present();
            */

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * DeltaTime;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, +2, -10, 0, +2, 0, 0, 1, 0);
            RC.View = mtxCam * mtxRot;

            // Render the scene 
            if (_sceneRenderer != null)
            {
                _sceneRenderer.Render(RC);
            }

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs rea)
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 0.01 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 200 (Anything further away from the camera than 200 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 0.01f, 200.0f);
            RC.Projection = projection;
        }
    }
}