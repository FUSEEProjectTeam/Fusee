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
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;


namespace Fusee.Examples.Simple.Core
{
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class Simple : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneRendererForward _sceneRenderer;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private readonly float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;

        private bool _keys;

        private DefaultSurfaceEffect _sufEffect;

        // Init is called on startup.
        public override async Task<bool> Init()
        {
            _gui = CreateGui();

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Load the rocket model
            _rocketScene = AssetStorage.Get<SceneContainer>("monkeys.fus");

            //var albedoTex = new Texture(await AssetStorage.GetAsync<ImageData>("FuseeText.png"));

            var albedoTex = new Texture(await AssetStorage.GetAsync<ImageData>("Bricks_1K_Color.png"), true, TextureFilterMode.LINEAR_MIPMAP_LINEAR);
            var normalTex = new Texture(await AssetStorage.GetAsync<ImageData>("Bricks_1K_Normal.png"), true, TextureFilterMode.LINEAR_MIPMAP_LINEAR);
            var defaultTex = new Texture(new ImageData(new byte[3] { 255, 255, 255 }, 1, 1, new ImagePixelFormat(ColorFormat.RGB)), false, TextureFilterMode.NEAREST);
            

            var gold_brdfFx = new ShaderEffect
            (
                new FxPassDeclaration()
                {
                    VS = AssetStorage.Get<string>("BRDF.vert"),
                    PS = AssetStorage.Get<string>("BRDF.frag"),
                    StateSet = RenderStateSet.Default
                },
                new List<IFxParamDeclaration>() 
                {
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ITView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.IView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ModelViewProjection, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ModelView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ITModelView, Value = float4x4.Identity },

                    new FxParamDeclaration<Texture>() { Name = "AlbedoTexture", Value = albedoTex},
                    new FxParamDeclaration<Texture>() { Name = "NormalTexture", Value = normalTex},
                    new FxParamDeclaration<float2>() { Name = "TexTiles", Value = float2.One},
                    new FxParamDeclaration<float>() { Name = "AlbedoTexMix", Value = 1f },

                    new FxParamDeclaration<float4>() { Name = "BaseColor", Value = new float4(1.0f, 227f/256f, 157f/256, 1.0f) },
                    new FxParamDeclaration<float>() { Name = "Metallic", Value = 1f },
                    new FxParamDeclaration<float>() { Name = "IOR", Value = 0.47f },
                    new FxParamDeclaration<float>() { Name = "Roughness", Value = 0.2f },
                    new FxParamDeclaration<float>() { Name = "Subsurface", Value = 0.0f },
                    new FxParamDeclaration<float>() { Name = "Specular", Value = 0.0f },
                    new FxParamDeclaration<float>() { Name = "Ambient", Value = 0.1f },
                }
            );
            
            var paint_brdfFx = new ShaderEffect
            (
                new FxPassDeclaration()
                {
                    VS = AssetStorage.Get<string>("BRDF.vert"),
                    PS = AssetStorage.Get<string>("BRDF.frag"),
                    StateSet = RenderStateSet.Default
                },
                new List<IFxParamDeclaration>()
                {
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ITView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.IView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ModelViewProjection, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ModelView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ITModelView, Value = float4x4.Identity },

                    new FxParamDeclaration<Texture>() { Name = "AlbedoTexture", Value = albedoTex},
                    new FxParamDeclaration<Texture>() { Name = "NormalTexture", Value = normalTex},
                    new FxParamDeclaration<float2>() { Name = "TexTiles", Value = float2.One},
                    new FxParamDeclaration<float>() { Name = "AlbedoTexMix", Value = 1f },

                    new FxParamDeclaration<float4>() { Name = "BaseColor", Value = new float4(0.0f, 231f/256f, 1f, 1.0f) },
                    new FxParamDeclaration<float>() { Name = "Metallic", Value = 0f },
                    new FxParamDeclaration<float>() { Name = "IOR", Value = 1.46f },
                    new FxParamDeclaration<float>() { Name = "Roughness", Value = 0.05f },
                    new FxParamDeclaration<float>() { Name = "Subsurface", Value = 0.0f },
                    new FxParamDeclaration<float>() { Name = "Specular", Value = 1.0f },
                    new FxParamDeclaration<float>() { Name = "Ambient", Value = 0.1f },
                }
            );

            var rubber_brdfFx = new ShaderEffect
            (
                new FxPassDeclaration()
                {
                    VS = AssetStorage.Get<string>("BRDF.vert"),
                    PS = AssetStorage.Get<string>("BRDF.frag"),
                    StateSet = RenderStateSet.Default
                },
                new List<IFxParamDeclaration>()
                {
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ITView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.IView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ModelViewProjection, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ModelView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ITModelView, Value = float4x4.Identity },

                    new FxParamDeclaration<Texture>() { Name = "AlbedoTexture", Value = albedoTex},
                    new FxParamDeclaration<Texture>() { Name = "NormalTexture", Value = normalTex},
                    new FxParamDeclaration<float2>() { Name = "TexTiles", Value = float2.One},
                    new FxParamDeclaration<float>() { Name = "AlbedoTexMix", Value = 1f },

                    new FxParamDeclaration<float4>() { Name = "BaseColor", Value = new float4(214f/256f, 84f/256f, 68f/256f, 1.0f) },
                    new FxParamDeclaration<float>() { Name = "Metallic", Value = 0f },
                    new FxParamDeclaration<float>() { Name = "IOR", Value = 1.519f },
                    new FxParamDeclaration<float>() { Name = "Roughness", Value = 1.0f },
                    new FxParamDeclaration<float>() { Name = "Subsurface", Value = 0.0f },
                    new FxParamDeclaration<float>() { Name = "Specular", Value = 0.1f },
                    new FxParamDeclaration<float>() { Name = "Ambient", Value = 0.1f },
                }
            );

            var subsurf_brdfFx = new ShaderEffect
            (
                new FxPassDeclaration()
                {
                    VS = AssetStorage.Get<string>("BRDF.vert"),
                    PS = AssetStorage.Get<string>("BRDF.frag"),
                    StateSet = RenderStateSet.Default
                },
                new List<IFxParamDeclaration>()
                {
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ITView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.IView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ModelViewProjection, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ModelView, Value = float4x4.Identity },
                    new FxParamDeclaration<float4x4>() { Name = UniformNameDeclarations.ITModelView, Value = float4x4.Identity },

                    new FxParamDeclaration<Texture>() { Name = "AlbedoTexture", Value = albedoTex},
                    new FxParamDeclaration<Texture>() { Name = "NormalTexture", Value = normalTex},
                    new FxParamDeclaration<float2>() { Name = "TexTiles", Value = float2.One},
                    new FxParamDeclaration<float>() { Name = "AlbedoTexMix", Value = 0.5f },

                    new FxParamDeclaration<float4>() { Name = "BaseColor", Value = new float4(255f/256f, 234f/256f, 215f/256f, 1.0f) },
                    new FxParamDeclaration<float>() { Name = "Metallic", Value = 0f },
                    new FxParamDeclaration<float>() { Name = "IOR", Value = 1.4f },
                    new FxParamDeclaration<float>() { Name = "Roughness", Value = 0.508f },
                    new FxParamDeclaration<float>() { Name = "Subsurface", Value = 1.0f },
                    new FxParamDeclaration<float>() { Name = "Specular", Value = 0.079f },
                    new FxParamDeclaration<float>() { Name = "Ambient", Value = 0.1f },
                }
            );

            foreach (var item in SurfaceEffect.CreateForwardLightingParamDecls(8))
            {
                subsurf_brdfFx.ParamDecl.Add(item.Name, item);
                gold_brdfFx.ParamDecl.Add(item.Name, item);
                paint_brdfFx.ParamDecl.Add(item.Name, item);
                rubber_brdfFx.ParamDecl.Add(item.Name, item);
            }

            _sufEffect = (DefaultSurfaceEffect)MakeEffect.FromDiffuseSpecular(new float4(1f, 0f, 0f, 1f), 22f, 1f);

            _rocketScene.Children[0].Components[1] = subsurf_brdfFx;
            _rocketScene.Children[1].Components[1] = rubber_brdfFx;
            _rocketScene.Children[2].Components[1] = paint_brdfFx;
            _rocketScene.Children[3].Components[1] = gold_brdfFx;


            var monkeyOne = (Mesh)_rocketScene.Children[0].Components[2];
            monkeyOne.Tangents = monkeyOne.CalculateTangents();
            monkeyOne.BiTangents = monkeyOne.CalculateBiTangents();

            var monkeyTwo = (Mesh)_rocketScene.Children[1].Components[2];
            monkeyTwo.Tangents = monkeyOne.Tangents;
            monkeyTwo.BiTangents = monkeyOne.BiTangents;

            var monkeyThree = (Mesh)_rocketScene.Children[2].Components[2];
            monkeyThree.Tangents = monkeyOne.Tangents;
            monkeyThree.BiTangents = monkeyOne.BiTangents;

            var monkeyFour = (Mesh)_rocketScene.Children[2].Components[2];
            monkeyFour.Tangents = monkeyOne.Tangents;
            monkeyFour.BiTangents = monkeyOne.BiTangents;

            //_rocketScene.Children[0].Components[1] = _surfEffect;

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_rocketScene);
            _guiRenderer = new SceneRendererForward(_gui);

            return true;
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            if (Keyboard.IsKeyDown(KeyCodes.W))
            {
                if (_sufEffect.SurfaceInput.Albedo.g <= 1.0f)
                    _sufEffect.SurfaceInput.Albedo += new float4(0, 0.2f, 0, 0);
            }
            if (Keyboard.IsKeyDown(KeyCodes.S))
            {
                if (_sufEffect.SurfaceInput.Albedo.g >= 0.0f)
                    _sufEffect.SurfaceInput.Albedo -= new float4(0, 0.2f, 0, 0);
            }

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
            var mtxCam = float4x4.LookAt(0, 0, -7, 0, 0, 0, 0, 1, 0);

            var view = mtxCam * mtxRot;
            var perspective = float4x4.CreatePerspectiveFieldOfView(_fovy, (float)Width / Height, ZNear, ZFar);
            var orthographic = float4x4.CreateOrthographic(Width, Height, ZNear, ZFar);

            // Render the scene loaded in Init()
            RC.View = view;
            RC.Projection = perspective;
            _sceneRenderer.Render(RC);

            //Constantly check for interactive objects.

            RC.Projection = orthographic;
            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);
            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

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
                UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_LEFT),
                //Define Offset and therefor the size of the element.
                UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f)),
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
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_HORIZONTAL),
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_HORIZONTAL, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery),
                HorizontalTextAlignment.CENTER,
                VerticalTextAlignment.CENTER);

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

        public void BtnLogoEnter(CodeComponent sender)
        {
            var effect = _gui.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, new float4(0.0f, 0.0f, 0.0f, 1f));
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