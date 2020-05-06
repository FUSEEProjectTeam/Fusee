using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.GUI;
using Fusee.Jometri;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using Transform = Fusee.Engine.Core.Scene.Transform;

namespace Fusee.Examples.Materials.Core
{
    [FuseeApplication(Name = "FUSEE Materials Example", Description = "Showcase of our materials")]
    public class Materials : RenderCanvas
    {
        private SceneRendererForward _renderer;
        private SceneRendererForward _guiDescRenderer;

        private float _alpha, _beta;
        private float _zoom = -25f;
        private float _offsetX, _offsetY = 0;

        private SceneContainer _scene;


        // Init is called on startup.
        public override async Task<bool> Init()
        {
            var fontLato = await AssetStorage.GetAsync<Font>("Lato-Black.ttf");
            var fontLatoMap = new FontMap(fontLato, 32);

            var vsTex = await AssetStorage.GetAsync<string>("texture.vert");
            var psTex = await AssetStorage.GetAsync<string>("texture.frag");

            var albedoTex = new Texture(await AssetStorage.GetAsync<ImageData>("albedoTex.jpg"));
            var emissiveTex = new Texture(await AssetStorage.GetAsync<ImageData>("emissiveTex.jpg"));
            var specularTex = new Texture(await AssetStorage.GetAsync<ImageData>("specularTex.jpg"));
            var normalTex = new Texture(await AssetStorage.GetAsync<ImageData>("normalTex.jpg"));

            var icosphereWithTangents = new Icosphere(5);
            icosphereWithTangents.Tangents = icosphereWithTangents.CalculateTangents();
            icosphereWithTangents.BiTangents = icosphereWithTangents.CalculateBiTangents();

            icosphereWithTangents.BoundingBox = new AABBf(icosphereWithTangents.Vertices);

            var canvasWidth = Width / 100f;
            var canvasHeight = Height / 100f;

            var guiDescriptionScene = new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    new CanvasNode("Canvas", CanvasRenderMode.WORLD, new MinMaxRect
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
                            UIElementPosition.GetAnchors(AnchorPos.DOWN_DOWN_LEFT),
                            UIElementPosition.CalcOffsets(AnchorPos.DOWN_DOWN_LEFT, new float2(-11, -5), canvasHeight, canvasWidth, new float2(12, 1)),
                            fontLatoMap,
                            new float4(1,1,0,1),
                            HorizontalTextAlignment.LEFT,
                            VerticalTextAlignment.CENTER)
                        }
                    },
                    new CanvasNode("Complete", CanvasRenderMode.WORLD, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "TextTransform",
                                        Translation = new float3(-15, 2.5f, 0)
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
                                        new float4(0,0,0,1),
                                        HorizontalTextAlignment.LEFT,
                                        VerticalTextAlignment.CENTER),new TextNode(
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
                                        HorizontalTextAlignment.LEFT,
                                        VerticalTextAlignment.CENTER)
                                }
                            },
                    new CanvasNode("Albedo and specular", CanvasRenderMode.WORLD, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "TextTransform",
                                        Translation = new float3(-10, 2.5f, 0)
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
                                        new float4(0,0,0,1),
                                        HorizontalTextAlignment.LEFT,
                                        VerticalTextAlignment.CENTER)
                                }
                            },
                    new CanvasNode("Albedo, specular and albedo texture", CanvasRenderMode.WORLD, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "TextTransform",
                                        Translation = new float3(-5, 2.5f, 0)
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
                                        new float4(0,0,0,1),
                                        HorizontalTextAlignment.LEFT,
                                        VerticalTextAlignment.CENTER)
                                }
                            },
                    new CanvasNode("Specular texture", CanvasRenderMode.WORLD, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "TextTransform",
                                        Translation = new float3(0, 2.5f, 0)
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
                                        new float4(0,0,0,1),
                                        HorizontalTextAlignment.LEFT,
                                        VerticalTextAlignment.CENTER),
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
                                        new float4(1,0,0,0.75f),
                                        HorizontalTextAlignment.LEFT,
                                        VerticalTextAlignment.CENTER)
                                }
                            },
                    new CanvasNode("Normal map", CanvasRenderMode.WORLD, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                                                {
                                                    Components = new List<SceneComponent>
                                                    {
                                                        new Transform
                                                        {
                                                            Name = "TextTransform",
                                                            Translation = new float3(5, 2.5f, 0)
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
                                                            new float4(0,0,0,1),
                                                            HorizontalTextAlignment.LEFT,
                                                            VerticalTextAlignment.CENTER)
                                                    }
                                                },
                    new CanvasNode("Albedo and emissive", CanvasRenderMode.WORLD, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "TextTransform",
                                        Translation = new float3(10, 2.5f, 0)
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
                                        new float4(0,0,0,1),
                                        HorizontalTextAlignment.LEFT,
                                        VerticalTextAlignment.CENTER),
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
                                        new float4(1,0,0,0.75f),
                                        HorizontalTextAlignment.LEFT,
                                        VerticalTextAlignment.CENTER)
                                }
                            },
                    new CanvasNode("Albedo and emissive with texture", CanvasRenderMode.WORLD, MinMaxRect.FromCenterSize(float2.Zero, float2.One))
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "TextTransform",
                                        Translation = new float3(15, 3, 0)
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
                                        new float4(0,0,0,1),
                                        HorizontalTextAlignment.LEFT,
                                        VerticalTextAlignment.CENTER),
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
                                        new float4(1,0,0,0.75f),
                                        HorizontalTextAlignment.LEFT,
                                        VerticalTextAlignment.CENTER)
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
                                        Translation = new float3(-15, 0, 0)
                                    },
                                       ShaderCodeBuilder.MakeShaderEffectFromShaderEffectPropsProto(new ShaderEffectProps
                                        {
                                            MatProbs =
                                            {
                                                HasAlbedo = true,
                                                HasAlbedoTexture = true,
                                                HasSpecular = true,
                                                HasSpecularTexture = true,
                                                HasEmissive = true,
                                                HasEmissiveTexture = true,
                                                HasNormalMap = true
                                            },
                                            MatType = MaterialType.Standard,
                                            MatValues =
                                            {
                                                AlbedoColor = float4.One * 0.25f,
                                                AlbedoMix = 1f,
                                                AlbedoTexture = albedoTex,
                                                SpecularColor = float4.One,
                                                SpecularIntensity = 2f,
                                                SpecularShininess = 25f,
                                                SpecularMix = 1f,
                                                SpecularTexture = specularTex,
                                                NormalMap = normalTex,
                                                NormalMapIntensity = 1f,
                                                EmissiveColor = new float4(0, 1, 1, 1),
                                                EmissiveMix = 0.5f,
                                                EmissiveTexture = emissiveTex
                                            }
                                        }),
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
                                        Translation = new float3(-10, 0, 0)
                                    },
                                    ShaderCodeBuilder.MakeShaderEffectProto(albedoColor: new float4(0.39f, 0.19f, 0, 1),
                                    specularColor: new float4(.5f, .5f, .5f, 1),
                                    shininess: 25.0f,
                                    specularIntensity: 2.5f),
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
                                        Translation = new float3(-5, 0, 0)
                                    },
                                    ShaderCodeBuilder.MakeShaderEffectProto(albedoColor: new float4(0.39f, 0.19f, 0, 1),
                                    specularColor: new float4(.5f, .5f, .5f, 1),
                                    albedoTexture: albedoTex,
                                    albedoTextureMix: 1f,
                                    shininess: 256.0f,
                                    specularIntensity: 20.0f),
                                    icosphereWithTangents
                                }
                            },
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "specular texture",
                                        Translation = new float3(0, 0, 0)
                                    },
                                    ShaderCodeBuilder.MakeShaderEffectFromShaderEffectPropsProto(new ShaderEffectProps
                                    {
                                        MatProbs =
                                        {
                                            HasAlbedo = true,
                                            HasSpecular = true,
                                            HasSpecularTexture = true
                                        },
                                        MatType = MaterialType.Standard,
                                        MatValues =
                                        {
                                            AlbedoColor = new float4(0.39f, 0.19f, 0, 1),
                                            SpecularColor = float4.One,
                                            SpecularIntensity = 2f,
                                            SpecularShininess = 25f,
                                            SpecularMix = 1f, // TODO: Implement in ShaderShards
                                            SpecularTexture = specularTex // TODO: Implement in ShaderShards
                                        }
                                    }),
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
                                        Translation = new float3(5, 0, 0)
                                    },
                                    ShaderCodeBuilder.MakeShaderEffectFromShaderEffectPropsProto(new ShaderEffectProps
                                    {
                                        MatProbs =
                                        {
                                            HasAlbedo = true,
                                            HasAlbedoTexture = true,
                                            HasNormalMap = true,
                                            HasSpecular = true
                                        },
                                        MatType = MaterialType.Standard,
                                        MatValues =
                                        {
                                            AlbedoColor = float4.One * 0.25f,
                                            AlbedoMix = 1f,
                                            AlbedoTexture = albedoTex,
                                            SpecularColor = float4.One,
                                            SpecularIntensity = 5f,
                                            SpecularShininess = 200f,
                                            NormalMap = normalTex,
                                            NormalMapIntensity = 1f
                                        }
                                    }),
                                    icosphereWithTangents
                                }
                            },
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "albedo, emissive",
                                        Translation = new float3(10, 0, 0)
                                    },
                                    ShaderCodeBuilder.MakeShaderEffectFromShaderEffectPropsProto(new ShaderEffectProps
                                    {
                                        MatProbs =
                                        {
                                            HasAlbedo = true,
                                            HasAlbedoTexture = true,
                                            HasEmissive = true
                                        },
                                        MatType = MaterialType.Standard,
                                        MatValues =
                                        {
                                            AlbedoColor = float4.One * 0.25f,
                                            AlbedoMix = 1f,
                                            AlbedoTexture = albedoTex,
                                            EmissiveColor = new float4(1, 0, 0, 1) // TODO: Implement in ShaderShards
                                        }
                                    }),
                                    icosphereWithTangents
                                }
                            },
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "albedo, emissive, emissive texture",
                                        Translation = new float3(15, 0, 0)
                                    },
                                    ShaderCodeBuilder.MakeShaderEffectFromShaderEffectPropsProto(new ShaderEffectProps
                                    {
                                        MatProbs =
                                        {
                                            HasAlbedo = true,
                                            HasAlbedoTexture = true,
                                            HasEmissive = true,
                                            HasEmissiveTexture = true
                                        },
                                        MatType = MaterialType.Standard,
                                        MatValues =
                                        {
                                            AlbedoColor = float4.One * 0.25f,
                                            AlbedoMix = 1f,
                                            AlbedoTexture = albedoTex,
                                            EmissiveColor = new float4(0, 1, 1, 1), // TODO: Implement in ShaderShards
                                            EmissiveMix = 0.5f, // TODO: Implement in ShaderShards
                                            EmissiveTexture = emissiveTex // TODO: Implement in ShaderShards
                                        }
                                    }),
                                    icosphereWithTangents
                                }
                            }
                        }
                    }
                }
            };

            _guiDescRenderer = new SceneRendererForward(guiDescriptionScene);
            _renderer = new SceneRendererForward(_scene);

            return true;
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            RC.ClearColor = new float4(0.75f, 0.75f, 0.75f, 1);

            // Clear the back buffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);

            var speed = Mouse.Velocity + Touch.GetVelocity(TouchPoints.Touchpoint_0);
            if (Mouse.LeftButton || Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _alpha -= speed.x * 0.00001f;
                _beta -= speed.y * 0.00001f;
            }

            // damping            
            var curDamp = (float)System.Math.Exp(-0.8 * Time.DeltaTime);
            _alpha *= curDamp;
            _beta *= curDamp;

            _zoom += Mouse.WheelVel * 0.05f;

            _scene.Children[0].GetComponentsInChildren<Transform>().ToList().ForEach(t => { if (t.Name != "TextTransform") t.Rotate(new float3(_beta, _alpha, 0)); });

            // Create the camera matrix and set it as the current ModelView transformation
            _offsetX += Keyboard.ADAxis * -0.4f;
            _offsetY += Keyboard.WSAxis * -0.3f;
            var mtxCam = float4x4.LookAt(0, 0, _zoom, 0, 0, 0, 0, 1, 0);
            var offset = float4x4.CreateTranslation(new float3(_offsetX, _offsetY, 0));
            RC.View = mtxCam * offset;

            _renderer.Render(RC);

            _guiDescRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}