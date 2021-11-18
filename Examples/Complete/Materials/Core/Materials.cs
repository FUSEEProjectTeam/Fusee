using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
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
        private float _alpha, _beta;
        private float _zoom = -25f;
        private float _offsetX, _offsetY = 0;

        private SceneContainer _scene;

        // Init is called on startup.
        public override void Init()
        {
            _ = AssetStorage.Get<Font>("Lato-Black.ttf");

            Icosphere icosphereWithTangents = new(5);
            icosphereWithTangents.Tangents = icosphereWithTangents.CalculateTangents();
            icosphereWithTangents.BiTangents = icosphereWithTangents.CalculateBiTangents();

            icosphereWithTangents.BoundingBox = new AABBf(icosphereWithTangents.Vertices);
            _ = Width / 100f;
            _ = Height / 100f;

            var albedoTex = new Texture(AssetStorage.Get<ImageData>("albedoTex.jpg"));
            var normalTex = new Texture(AssetStorage.Get<ImageData>("normalTex.jpg"));

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
                                        Translation = new float3(-10, 0, 0)
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
                                        Translation = new float3(-5, 0, 0)
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
                            new SceneNode
                            {
                                Components = new List<SceneComponent>
                                {
                                    new Transform
                                    {
                                        Name = "specular texture - not impl.",
                                        Translation = new float3(0, 0, 0)
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
                                        Translation = new float3(5, 0, 0)
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
                                        Translation = new float3(10, 0, 0)
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
                                        Translation = new float3(15, 0, 0)
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

            //_guiDescRenderer = new SceneRendererForward(guiDescriptionScene);
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

            //_guiDescRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}