using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.SimpleDeferred.Core
{
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class SimpleDeferred : RenderCanvas
    {
        // angle variables
        private static float _angleHorz,  _angleVert , _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneRenderer _textureRenderer;
        private SceneRenderer _sceneRenderer;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private readonly float _fovy = M.PiOver4;

        private SceneRenderer _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private readonly CanvasRenderMode _canvasRenderMode = CanvasRenderMode.SCREEN;

        private RenderTarget _renderTarget;      
        
        private bool _keys;

        private EventHandler<ResizeEventArgs> _resizeDelLightingPass;

        private float4 _texClearColor = new float4(1, 1, 1, 1);
        private float4 _backgroundColor = new float4(0, 0, 0, 1);

        // Init is called on startup. 
        public override void Init()
        {
            _gui = CreateGui();
            
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = _backgroundColor;

            // Load the rocket model
            _rocketScene = AssetStorage.Get<SceneContainer>("FUSEERocket.fus");

            _renderTarget = new RenderTarget(TexRes.MID_RES);
            _renderTarget.CreatePositionTex();
            _renderTarget.CreateAlbedoSpecularTex();
            _renderTarget.CreateNormalTex();
            _renderTarget.CreateDepthTex();
            _renderTarget.CreateSSAOTex();
            //_renderTarget.CreateStencilTex();

            //Add resize delegate
            var perspectiveProjComp = _rocketScene.Children[0].GetComponent<ProjectionComponent>();

            _resizeDelLightingPass = delegate 
            {
                perspectiveProjComp.Resize(Width, Height);
                RC.Viewport(0, 0, Width, Height);
            };

            AddResizeDelegate(_resizeDelLightingPass);           

            foreach (var child in _rocketScene.Children)
            {
                var renderTargetMat = new ShaderEffectComponent()
                {
                    Effect = ShaderCodeBuilder.RenderTargetTextureEffect(_renderTarget),
                };

                var oldEffect = child.GetComponent<ShaderEffectComponent>().Effect;

                var col = (float4)oldEffect.GetEffectParam("DiffuseColor");
                var specStrength = (float)oldEffect.GetEffectParam("SpecularIntensity");
                
                col.a = specStrength; 

                child.RemoveComponent<ShaderEffectComponent>();

                child.Components.Insert(1, renderTargetMat);               
                renderTargetMat.Effect.SetEffectParam("DiffuseColor", col);
            }

            var yellowLight = new LightComponent() { Type = LightType.Point, Color = new float4(1, 1, 0, 1), Attenuation = 0.7f, Active = true};
            var redLight = new LightComponent() { Type = LightType.Point, Color = new float4(1, 0, 0, 1), Attenuation = 0.7f, Active = true };
            var blueLight = new LightComponent() { Type = LightType.Point, Color = new float4(0, 0, 1, 1), Attenuation = 0.7f, Active = true };
            var greenLight = new LightComponent() { Type = LightType.Point, Color = new float4(0, 1, 0, 1), Attenuation = 0.7f, Active = true };

            // Wrap a SceneRenderer around the model.
            _textureRenderer = new SceneRenderer(_rocketScene);

            var plane = new Plane();
            var lightMultiplier = 1;
            var planeScene = new SceneContainer()
            {
                Children = new List<SceneNodeContainer>()
                {
                    new SceneNodeContainer()
                    {
                        Components = new List<SceneComponentContainer>()
                        {
                            perspectiveProjComp,
                            new TransformComponent()
                            {
                                Scale = new float3(1,1,1)
                            },
                            new ShaderEffectComponent()
                            {
                                Effect = ShaderCodeBuilder.DeferredLightingPassEffect(_renderTarget, lightMultiplier*4)
                            },
                            plane

                        }
                    }
                }
            };
            var aLotOfLights = new ChildList();

            var rnd = new Random();
            

            for (int i = 0; i < lightMultiplier; i++)
            {
                var rndVal = (float)rnd.NextDouble() * 10;

                aLotOfLights.Add(new SceneNodeContainer()
                {
                    Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent(){ Translation = new float3(-rndVal,0,0) },
                            new LightComponent() { Type = LightType.Point, Color = new float4(1, 1, 0, 1), Attenuation = 0.7f, Active = true}
            }
                });
                aLotOfLights.Add(new SceneNodeContainer()
                {
                    Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent(){ Translation = new float3(rndVal,rndVal,0) },
                            new LightComponent() { Type = LightType.Point, Color = new float4(1, 0, 0, 1), Attenuation = 0.7f, Active = true }
        }
                
                });
                aLotOfLights.Add(new SceneNodeContainer()
                {
                    Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent(){ Translation = new float3(0,rndVal,0) },
                            new LightComponent() { Type = LightType.Point, Color = new float4(0, 0, 1, 1), Attenuation = 0.7f, Active = true }
    }
                });
                aLotOfLights.Add(new SceneNodeContainer()
                {
                    Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent(){ Translation = new float3(0,-rndVal,rndVal) },
                            new LightComponent() { Type = LightType.Point, Color = new float4(0, 1, 0, 1), Attenuation = 0.7f, Active = true }
}
                });
            }

            planeScene.Children.Add(new SceneNodeContainer()
            {
                Name = "LightContainer",
                Children = aLotOfLights
            });

            _sceneRenderer = new SceneRenderer(planeScene);
            _guiRenderer = new SceneRenderer(_gui);            
        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
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

            //Set the view matrix for the interaction handler.
            _sih.View = RC.View;

            // Constantly check for interactive objects.
            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(Mouse.Position, Width, Height);

            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            RC.ClearColor = _texClearColor;
            RC.Viewport(0, 0, (int)_renderTarget.TextureResolution, (int)_renderTarget.TextureResolution);
            // Render the scene loaded in Init()
            _textureRenderer.Render(RC, _renderTarget);

            RC.ClearColor = _backgroundColor;
            RC.Viewport(0, 0, Width, Height);
            RC.View = mtxCam;
            _sceneRenderer.Render(RC);

            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
       
        public override void Resize(ResizeEventArgs e)
        {
                           
        }

        private SceneContainer CreateGui()
        {
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");

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
            var fuseeLogo = new TextureNodeContainer(
                "fuseeLogo",
                vsTex,
                psTex,
                //Set the diffuse texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TOP_TOP_LEFT),
                //Define Offset and therefor the size of the element.                
                UIElementPosition.CalcOffsets(AnchorPos.TOP_TOP_LEFT, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f))
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 18);

            var text = new TextNodeContainer(
                "FUSEE Simple Example",
                "ButtonText",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.STRETCH_HORIZONTAL),
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_HORIZONTAL, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery), 250f);


            var canvas = new CanvasNodeContainer(
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

            var canvasProjComp = new ProjectionComponent(ProjectionMethod.ORTHOGRAPHIC, ZNear, ZFar, _fovy);
            canvas.Components.Insert(0, canvasProjComp);
            AddResizeDelegate(delegate
            {
                canvasProjComp.Resize(Width, Height);
                RC.Viewport(0, 0, Width, Height);
            });

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