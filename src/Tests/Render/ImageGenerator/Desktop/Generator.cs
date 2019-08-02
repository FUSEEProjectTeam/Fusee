using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Fusee.Engine.GUI;

namespace Fusee.Engine.Examples.ImageGenerator.Desktop
{
    public class Generator : RenderCanvas
    {
        public void DoInit()
        {
            InitCanvas();
        }


        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _model;
        private SceneRenderer _sceneRenderer;

        private SceneRenderer _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;

        private bool _keys;

        // Init is called on startup. 
        public override void Init()
        {
            _gui = CreateGui();
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Load the rocket model
            _model = AssetStorage.Get<SceneContainer>("Model.fus");

            var projComp = _model.Children[0].GetComponent<ProjectionComponent>();
            AddResizeDelegate(delegate { projComp.Resize(Width, Height); });

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_model);
            _guiRenderer = new SceneRenderer(_gui);
        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            /* INPUT
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
            */

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, +2, -10, 0, +2, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRot;

            //Set the view matrix for the interaction handler.
            _sih.View = RC.ModelView;

            /* INPUT
            // Constantly check for interactive objects.
            _sih.CheckForInteractiveObjects(Input.Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }
            */

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");

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

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var latoFontMap = new FontMap(fontLato, 36);
            var text = new TextNodeContainer(
                "FUSEE Simple Example",
                "ButtonText",
                vsTex,
                psTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 0)
                },
                new MinMaxRect
                {
                    Min = new float2(4f, 0f),
                    Max = new float2(-4, 0.5f)
                },
                latoFontMap,
                ColorUint.Tofloat4(ColorUint.Greenery), 0.25f);


            var canvas = new CanvasNodeContainer(
                "Canvas",
                CanvasRenderMode.SCREEN,
                new MinMaxRect
                {
                    Min = new float2(-8, -4.5f),
                    Max = new float2(8, 4.5f)
                }
            )
            {
                
                Children = new ChildList()
                {
                    //Simple Texture Node, contains the fusee logo.
                    fuseeLogo,
                    text
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
