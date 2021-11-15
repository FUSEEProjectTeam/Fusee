using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;
using System.Linq;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.RenderToTexture.Core
{
    [FuseeApplication(Name = "FUSEE Texture Rendering Example", Description = "An example on how to render a camera to a texture.")]
    public class RenderToTexture : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.Screen;

        private readonly Engine.Core.Scene.Camera _renderCam = new Engine.Core.Scene.Camera(ProjectionMethod.Perspective, 5, 100, M.PiOver4);
        private readonly Engine.Core.Scene.Camera _mainCam = new Engine.Core.Scene.Camera(ProjectionMethod.Perspective, 1, 1000, M.PiOver4);
        private Transform _renderCamTransform;
        private Transform _mainCamTransform;

        private WritableTexture _renderTexture = new WritableTexture(RenderTargetTextureTypes.Albedo, new ImagePixelFormat(ColorFormat.RGB), 500, 500);

        // Init is called on startup.
        public override void Init()
        {
            _mainCam.Viewport = new float4(0, 0, 100, 100);
            _mainCam.BackgroundColor = new float4(1f, 1f, 1f, 1);
            _mainCam.Layer = 10;

            _renderCam.Viewport = new float4(0, 0, 10, 10);
            _renderCam.BackgroundColor = new float4(0f, 0f, 0f, 1);
            _renderCam.Layer = 20;
            _renderCam.RenderTexture = _renderTexture;

            _mainCamTransform = new Transform()
            {
                Rotation = new float3(0, 0, 0),
                Translation = new float3(0, 20, 20),
                Scale = new float3(1, 1, 1),
            };
            var rotation = float4x4.LookAt(_mainCamTransform.Translation, new float3(0, -5, 0), float3.UnitY);
            _mainCamTransform.Rotate(rotation.RotationComponent());


            SceneNode mainCam = new SceneNode()
            {
                Name = "MainCam",
                Components = new List<SceneComponent>()
                {
                    _mainCamTransform,
                    _mainCam,
                }
            };

            _renderCamTransform = new Transform()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 5, -20),
                Scale = new float3(1, 1, 1),
            };

            SceneNode renderCam = new SceneNode()
            {
                Name = "RenderCam",
                Components = new List<SceneComponent>()
                {
                    _renderCamTransform,
                    _renderCam,
                    MakeEffect.FromDiffuseSpecular(new float4(1,0,0,1), float4.Zero),
                    new Engine.Core.Primitives.Cube(),

                },
                Children = new ChildList()
                {
                    new SceneNode()
                    {
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Scale = new float3(0.5f, 0.5f, 1f),
                                Translation = new float3(0,0, 1f)
                            },
                            new Engine.Core.Primitives.Cube()
                        }
                    }
                }
            };

            SceneNode plane = new SceneNode()
            {
                Name = "Plane",
                Components = new List<SceneComponent>()
                {
                    new Transform
                    {
                        Rotation = new float3(0, M.Pi, 0),
                        Translation = new float3(0, 5, 20),
                        Scale = float3.One
                    },
                    MakeEffect.FromDiffuseRenderTexture((float4)ColorUint.Red, _renderTexture, new float2(1, 1), .5f),
                    new Engine.Core.Primitives.Plane()
                }
            };

            _gui = CreateGui();
            _scene = CreateScene();

            _scene.Children.Add(mainCam);
            _scene.Children.Add(renderCam);
            _scene.Children.Add(plane);

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _guiRenderer = new SceneRendererForward(_gui);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            // Mouse and keyboard movement

            _mainCamTransform.RotateAround(new float3(0, -50, 0), new float3(0, DeltaTime * 0.2f, 0));

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);
            //_guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");
            var psText = AssetStorage.Get<string>("text.frag");

            var canvasWidth = Width / 100f;
            var canvasHeight = Height / 100f;

            var btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            btnFuseeLogo.OnMouseExit += BtnLogoExit;
            btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            var fuseeLogo = new TextureNode(
                "fuseeLogo",
                vsTex,
                psTex,
                //Set the albedo texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                //Define Offset and therefor the size of the element.
                UIElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f)),
                float2.One
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 24);

            var text = new TextNode(
                "FUSEE Simple Example",
                "ButtonText",
                vsTex,
                psText,
                UIElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                UIElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                (float4)ColorUint.Greenery,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            var canvas = new CanvasNode(
                "Canvas",
                _canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-canvasWidth / 2, -canvasHeight / 2f),
                    Max = new float2(canvasWidth / 2, canvasHeight / 2f)
                })
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
                Children = new List<SceneNode>
                {
                    //Add canvas.
                    canvas
                }
            };
        }

        private SceneContainer CreateScene()
        {
            var scene = new SceneContainer
            {
                Header = new SceneHeader
                {
                    CreationDate = "November 2021",
                    CreatedBy = "Jonas Haller",
                    Generator = "Handcoded with pride :)",
                },
                Children = new List<SceneNode> { },
            };

            var rand = new System.Random();

            for (int i = 0; i < 20; i++)
            {
                int x = rand.Next(-10, 10);
                int y = rand.Next(-10, 10);
                int z = rand.Next(-10, 10);

                var cube = new SceneNode
                {
                    Name = "Cube" + i,
                    Components = new List<SceneComponent>
                    {
                        new Transform {
                            Translation=new float3(x, y, z),
                            Scale = float3.One
                        },
                        MakeEffect.FromDiffuseSpecular((float4)ColorUint.Gray, float4.Zero, 4.0f, 1f),
                        new Engine.Core.Primitives.Cube()
                    }
                };

                scene.Children.Add(cube);
            }

            return scene;
        }

        public void BtnLogoEnter(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, (float4)ColorUint.Black);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 0.8f);
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, float4.One);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 1f);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            OpenLink("http://fusee3d.org");
        }
    }
}