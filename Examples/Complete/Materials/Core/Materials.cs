using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using static Fusee.Engine.Core.Input;
using Transform = Fusee.Engine.Core.Scene.Transform;

namespace Fusee.Examples.Materials.Core
{
    [FuseeApplication(Name = "FUSEE Materials Example", Description = "Showcase of our materials")]
    public class Materials : RenderCanvas
    {
        private SceneRendererDeferred _renderer;
        private SceneRendererForward _guiDescRenderer;

        private float _alpha, _beta;
        private float _zoom = -25f;
        private float _offsetX, _offsetY = 0;

        private SceneContainer _scene;

        // Init is called on startup.
        public override void Init()
        {
            Font fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            FontMap fontLatoMap = new FontMap(fontLato, 32);

            string vsTex = AssetStorage.Get<string>("texture.vert");
            string psTex = AssetStorage.Get<string>("texture.frag");

            Icosphere icosphereWithTangents = new Icosphere(5);
            icosphereWithTangents.Tangents = icosphereWithTangents.CalculateTangents();
            icosphereWithTangents.BiTangents = icosphereWithTangents.CalculateBiTangents();

            icosphereWithTangents.BoundingBox = new AABBf(icosphereWithTangents.Vertices);

            float canvasWidth = Width / 100f;
            float canvasHeight = Height / 100f;

            var albedoTex = new Texture(AssetStorage.Get<ImageData>("albedoTex.jpg"));
            var normalTex = new Texture(AssetStorage.Get<ImageData>("normalTex.jpg"));

            SceneContainer guiDescriptionScene = new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    new CanvasNode("Canvas", CanvasRenderMode.World, new MinMaxRect
                    {
                        Min = new float2(-canvasWidth / 2, -canvasHeight / 2f),
                        Max = new float2(canvasWidth / 2, canvasHeight / 2f)
                    })
                    {
                        Children = new ChildList
                        {
                            new TextNode(
                            "How-To:\n############################\n- Move with WASD\n- Left mouse button rotates spheres\n- Mouse wheel zooms",
                            "howTo",
                            vsTex,
                            psTex,
                            UIElementPosition.GetAnchors(AnchorPos.DownDownLeft),
                            UIElementPosition.CalcOffsets(AnchorPos.DownDownLeft, new float2(-11, -5), canvasHeight, canvasWidth, new float2(12, 1)),
                            fontLatoMap,
                            new float4(1, 1, 0, 1).LinearColorFromSRgb(),
                            HorizontalTextAlignment.Left,
                            VerticalTextAlignment.Center)
                        }
                    },
                    new CanvasNode("Complete", CanvasRenderMode.World, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                    {
                        Components = new List<SceneComponent>
                        {
                            new Transform
                            {
                                Name = "TextTransform",
                                TranslationVector = new float3(-15, 2.5f, 0)
                            }
                        },
                        Children = new ChildList
                        {
                                new TextNode(
                                "Complete",
                                "desc",
                                vsTex,
                                psTex,
                                MinMaxRect.FromCenterSize(float2.Zero, float2.One),
                                new MinMaxRect(),
                                fontLatoMap,
                                (float4)ColorUint.Black,
                                HorizontalTextAlignment.Left,
                                VerticalTextAlignment.Center),new TextNode(
                                "NOT YET IMPLEMENTED",
                                "desc",
                                vsTex,
                                psTex,
                                MinMaxRect.FromCenterSize(float2.Zero, float2.One),
                                new MinMaxRect
                                {
                                    Max = new float2(0, 0),
                                    Min = new float2(0, -1.25f)
                                },
                                fontLatoMap,
                                new float4(1,0,0,0.5f),
                                HorizontalTextAlignment.Left,
                                VerticalTextAlignment.Center)
                        }
                    },
                    new CanvasNode("Albedo and specular", CanvasRenderMode.World, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                    {
                        Components = new List<SceneComponent>
                        {
                            new Transform
                            {
                                Name = "TextTransform",
                                TranslationVector = new float3(-10, 2.5f, 0)
                            }
                        },
                        Children = new ChildList
                        {
                                new TextNode(
                                "Albedo and Specular",
                                "desc",
                                vsTex,
                                psTex,
                                MinMaxRect.FromCenterSize(float2.Zero, float2.One),
                                new MinMaxRect(),
                                fontLatoMap,
                                (float4)ColorUint.Black,
                                HorizontalTextAlignment.Left,
                                VerticalTextAlignment.Center)
                        }
                    },
                    new CanvasNode("Albedo, specular and albedo texture", CanvasRenderMode.World, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                    {
                        Components = new List<SceneComponent>
                        {
                            new Transform
                            {
                                Name = "TextTransform",
                                TranslationVector = new float3(-5, 2.5f, 0)
                            }
                        },
                        Children = new ChildList
                        {
                                new TextNode(
                                "Albedo, specular and\nalbedo texture",
                                "desc",
                                vsTex,
                                psTex,
                                MinMaxRect.FromCenterSize(float2.Zero, float2.One),
                                new MinMaxRect(),
                                fontLatoMap,
                                (float4)ColorUint.Black,
                                HorizontalTextAlignment.Left,
                                VerticalTextAlignment.Center)
                        }
                    },
                    new CanvasNode("Specular texture", CanvasRenderMode.World, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                    {
                        Components = new List<SceneComponent>
                        {
                            new Transform
                            {
                                Name = "TextTransform",
                                TranslationVector = new float3(0, 2.5f, 0)
                            }
                        },
                        Children = new ChildList
                        {
                                new TextNode(
                                "Specular texture",
                                "desc",
                                vsTex,
                                psTex,
                                MinMaxRect.FromCenterSize(float2.Zero, float2.One),
                                new MinMaxRect(),
                                fontLatoMap,
                                (float4)ColorUint.Black,
                                HorizontalTextAlignment.Left,
                                VerticalTextAlignment.Center),
                                new TextNode(
                                "NOT YET IMPLEMENTED",
                                "desc",
                                vsTex,
                                psTex,
                                MinMaxRect.FromCenterSize(float2.Zero, float2.One),
                                new MinMaxRect
                                {
                                    Max = new float2(0, 0),
                                    Min = new float2(0, -1.25f)
                                },
                                fontLatoMap,
                                new float4(1,0,0,0.75f).LinearColorFromSRgb(),
                                HorizontalTextAlignment.Left,
                                VerticalTextAlignment.Center)
                        }
                    },
                    new CanvasNode("Normal map", CanvasRenderMode.World, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                    {
                        Components = new List<SceneComponent>
                        {
                            new Transform
                            {
                                Name = "TextTransform",
                                TranslationVector = new float3(5, 2.5f, 0)
                            }
                        },
                        Children = new ChildList
                        {
                                new TextNode(
                                "Normal map",
                                "desc",
                                vsTex,
                                psTex,
                                MinMaxRect.FromCenterSize(float2.Zero, float2.One),
                                new MinMaxRect(),
                                fontLatoMap,
                                (float4)ColorUint.Black,
                                HorizontalTextAlignment.Left,
                                VerticalTextAlignment.Center)
                        }
                    },
                    new CanvasNode("Albedo and emissive", CanvasRenderMode.World, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "TextTransform",
                                        TranslationVector = new float3(10, 2.5f, 0)
                                    }
                                },
                                Children = new ChildList
                                {
                                     new TextNode(
                                        "Albedo and emissive",
                                        "desc",
                                        vsTex,
                                        psTex,
                                        MinMaxRect.FromCenterSize(float2.Zero, float2.One),
                                        new MinMaxRect(),
                                        fontLatoMap,
                                        (float4)ColorUint.Black,
                                        HorizontalTextAlignment.Left,
                                        VerticalTextAlignment.Center),
                                      new TextNode(
                                        "NOT YET IMPLEMENTED",
                                        "desc",
                                        vsTex,
                                        psTex,
                                        MinMaxRect.FromCenterSize(float2.Zero, float2.One),
                                        new MinMaxRect
                                        {
                                            Max = new float2(0, 0),
                                            Min = new float2(0, -1.25f)
                                        },
                                        fontLatoMap,
                                        new float4(1,0,0,0.75f).LinearColorFromSRgb(),
                                        HorizontalTextAlignment.Left,
                                        VerticalTextAlignment.Center)
                                }
                            },
                    new CanvasNode("Albedo and emissive with texture", CanvasRenderMode.World, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "TextTransform",
                                        TranslationVector = new float3(15, 3, 0)
                                    }
                                },
                                Children = new ChildList
                                {
                                     new TextNode(
                                        "Albedo, emissive and\nemissive texture",
                                        "desc",
                                        vsTex,
                                        psTex,
                                        MinMaxRect.FromCenterSize(float2.Zero, float2.One),
                                        new MinMaxRect(),
                                        fontLatoMap,
                                        (float4)ColorUint.Black,
                                        HorizontalTextAlignment.Left,
                                        VerticalTextAlignment.Center),
                                     new TextNode(
                                        "NOT YET IMPLEMENTED",
                                        "desc",
                                        vsTex,
                                        psTex,
                                        MinMaxRect.FromCenterSize(float2.Zero, float2.One),
                                        new MinMaxRect
                                        {
                                            Max = new float2(0, 0),
                                            Min = new float2(0, -1.75f)
                                        },
                                        fontLatoMap,
                                        new float4(1,0,0,0.75f).LinearColorFromSRgb(),
                                        HorizontalTextAlignment.Left,
                                        VerticalTextAlignment.Center)
                                }
                            }
                }
            };

            _scene = new SceneContainer
            {
                Header = new SceneHeader
                {
                    CreatedBy = "MR",
                    CreationDate = DateTime.Now.ToString(),
                    Generator = "by hand"
                },
                Children = new List<SceneNode>
                {
                    new SceneNode
                    {
                        Children = new ChildList
                        {
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "complete",
                                        TranslationVector = new float3(-15, 0, 0)
                                    },
                                    MakeEffect.FromDiffuseSpecularTexture(
                                        albedoColor : (float4.One * 0.25f).LinearColorFromSRgb(),
                                        emissionColor: float4.Zero,
                                        shininess : 25f,
                                        albedoTex : albedoTex,
                                        normalTex : normalTex,
                                        albedoMix : 1f,
                                        texTiles: float2.One,
                                        specularStrength : 1f,
                                        normalMapStrength : 1f

                                        //SpecularMix = 1f,
                                        //SpecularTexture = "specularTex.jpg",
                                        
                                        //EmissiveColor = new float4(0, 1, 1, 1),
                                        //EmissiveMix = 0.5f,
                                        //EmissiveTexture = "emissiveTex.jpg"
                                    ),
                                    icosphereWithTangents,
                                }
                            },
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "albedo and specular",
                                        TranslationVector = new float3(-10, 0, 0)
                                    },
                                    MakeEffect.FromDiffuseSpecular(
                                    albedoColor: new float4(0.39f, 0.19f, 0, 1).LinearColorFromSRgb(),
                                    emissionColor: float4.Zero,
                                    shininess: 25.0f,
                                    specularStrength: 1f),
                                    icosphereWithTangents
                                }
                            },
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "albedo, specular, albedo texture",
                                        TranslationVector = new float3(-5, 0, 0)
                                    },
                                    MakeEffect.FromDiffuseSpecularAlbedoTexture
                                    (
                                        albedoColor: new float4(0.39f, 0.19f, 0, 1).LinearColorFromSRgb(),
                                        emissionColor: float4.Zero,
                                        albedoTex: albedoTex,
                                        albedoMix: 1f,
                                        texTiles : float2.One,
                                        shininess: 256.0f,
                                        specularStrength: 1.0f
                                    ),
                                    icosphereWithTangents
                                }
                            },
                            // ---- Specular Textures are not implemented yet. There is no fitting shader! ---- //
                            //new SceneNode
                            //{
                            //    Components = new List<SceneComponent>
                            //    {
                            //        new Transform
                            //        {
                            //            Name = "specular texture",
                            //            Translation = new float3(0, 0, 0)
                            //        },
                            //        ShaderCodeBuilder.MakeShaderEffectFromShaderEffectPropsProto(new ShaderEffectProps
                            //        {
                            //            MatProbs =
                            //            {
                            //                HasAlbedo = true,
                            //                HasAlbedoTexture = true,
                            //                HasSpecular = true,
                            //                HasSpecularTexture = true
                            //            },
                            //            MatType = MaterialType.Standard,
                            //            MatValues =
                            //            {
                            //                AlbedoColor = new float4(0.39f, 0.19f, 0, 1),
                            //                SpecularColor = float4.One,
                            //                SpecularIntensity = 2f,
                            //                SpecularShininess = 25f,
                            //                SpecularMix = 1f, // TODO: Implement in ShaderShards
                            //                SpecularTexture = "specularTex.jpg" // TODO: Implement in ShaderShards
                            //            }
                            //        }),
                            //        icosphereWithTangents
                            //    }
                            //},
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "specular texture - not impl.",
                                        TranslationVector = new float3(0, 0, 0)
                                    },
                                    MakeEffect.FromDiffuseSpecular(float4.One, float4.Zero, 85, 0.5f),
                                    icosphereWithTangents
                                }
                            },
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "normal map",
                                        TranslationVector = new float3(5, 0, 0)
                                    },
                                    MakeEffect.FromDiffuseSpecularTexture(
                                            albedoColor: (float4.One * 0.25f).LinearColorFromSRgb(),
                                            emissionColor: float4.Zero,
                                            shininess: 200f,
                                            albedoTex : albedoTex,
                                            normalTex : normalTex,
                                            albedoMix: 1f,
                                            texTiles : float2.One,
                                            specularStrength : 1f,
                                            normalMapStrength : 1f
                                    ),
                                    icosphereWithTangents
                                }
                            },
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "albedo, emissive - not impl.",
                                        TranslationVector = new float3(10, 0, 0)
                                    },
                                    MakeEffect.FromDiffuseSpecularAlbedoTexture(
                                    albedoColor: new float4(0.39f, 0.19f, 0, 1).LinearColorFromSRgb(),
                                    emissionColor: float4.Zero,
                                    albedoTex: albedoTex,
                                    albedoMix: 1f,
                                    texTiles:float2.One,
                                    shininess: 256.0f,
                                    specularStrength: 1f),
                                    icosphereWithTangents
                                }
                            },
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "albedo, emissive, emissive texture - not impl.",
                                        TranslationVector = new float3(15, 0, 0)
                                    },
                                    MakeEffect.FromDiffuseSpecularAlbedoTexture(
                                    albedoColor: new float4(0.39f, 0.19f, 0, 1).LinearColorFromSRgb(),
                                    emissionColor: float4.Zero,
                                    albedoTex: albedoTex,
                                    albedoMix: 1f,
                                    texTiles: float2.One,
                                    shininess: 256.0f,
                                    specularStrength: 1.0f),
                                    icosphereWithTangents
                                }
                            }
                        }
                    }
                }
            };

            _guiDescRenderer = new SceneRendererForward(guiDescriptionScene);
            _renderer = new SceneRendererDeferred(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            RC.ClearColor = new float4(0.75f, 0.75f, 0.75f, 1);

            // Clear the back buffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            float2 speed = Mouse.Velocity + Touch.GetVelocity(TouchPoints.Touchpoint_0);
            if (Mouse.LeftButton || Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _alpha -= speed.x * 0.00001f;
                _beta -= speed.y * 0.00001f;
            }

            // damping            
            float curDamp = (float)System.Math.Exp(-0.8 * Time.DeltaTime);
            _alpha *= curDamp;
            _beta *= curDamp;

            _zoom += Mouse.WheelVel * 0.05f;

            _scene.Children[0].GetComponentsInChildren<Transform>().ToList().ForEach(t => { if (t.Name != "TextTransform") { t.Rotate(new float3(_beta, _alpha, 0)); } });

            // Create the camera matrix and set it as the current ModelView transformation
            _offsetX += Keyboard.ADAxis * -0.4f;
            _offsetY += Keyboard.WSAxis * -0.3f;
            float4x4 mtxCam = float4x4.LookAt(0, 0, _zoom, 0, 0, 0, 0, 1, 0);
            float4x4 offset = float4x4.CreateTranslation(new float3(_offsetX, _offsetY, 0));
            RC.View = mtxCam * offset;

            _renderer.Render(RC);

            _guiDescRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}