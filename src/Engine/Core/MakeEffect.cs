using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Provides helper methods for creating <see cref="ShaderEffect"/>s.
    /// </summary>
    public static class MakeEffect
    {
        /// <summary>
        /// The default <see cref="Effect"/>, that is used if a <see cref="SceneNode"/> has a mesh but no effect.
        /// </summary>
        public static SurfaceEffect Default { get; } = FromDiffuseSpecular(new float4(0.5f, 0.5f, 0.5f, 1.0f), new float4(), 22, 1.0f);

        #region Deferred

        /// <summary>
        /// If rendered with FXAA we'll need an additional (final) pass, that takes the lighted scene, rendered to a texture, as input.
        /// </summary>
        /// <param name="srcTex">RenderTarget, that contains a single texture in the Albedo/Specular channel, that contains the lighted scene.</param>
        /// <param name="screenParams">The width and height of the screen.</param>
        // see: http://developer.download.nvidia.com/assets/gamedev/files/sdk/11/FXAA_WhitePaper.pdf
        // http://blog.simonrodriguez.fr/articles/30-07-2016_implementing_fxaa.html
        public static ShaderEffect FXAARenderTargetEffect(WritableTexture srcTex, float2 screenParams)
        {
            return new ShaderEffect(

            new FxPassDeclaration
            {
                VS = AssetStorage.Get<string>("Deferred.vert"),
                PS = AssetStorage.Get<string>("FXAA.frag"),
                StateSet = new RenderStateSet
                {
                    AlphaBlendEnable = false,
                    ZEnable = true,
                }
            },
            new IFxParamDeclaration[]
            {
                new FxParamDeclaration<WritableTexture> { Name = RenderTargetTextureTypes.Albedo.ToString(), Value = srcTex},
                new FxParamDeclaration<float2> { Name = UniformNameDeclarations.ScreenParams, Value = screenParams},
            });
        }

        /// <summary>
        /// Shader effect for the ssao pass.
        /// </summary>
        /// <param name="geomPassRenderTarget">RenderTarget filled in the previous geometry pass.</param>
        /// <param name="kernelLength">SSAO kernel size.</param>
        /// <param name="screenParams">Width and Height of the screen.</param>
        /// <param name="noiseTexSize">Width and height of the noise texture.</param>
        public static ShaderEffect SSAORenderTargetTextureEffect(IRenderTarget geomPassRenderTarget, int kernelLength, float2 screenParams, int noiseTexSize)
        {
            var ssaoKernel = FuseeSsaoHelper.CreateKernel(kernelLength);
            var ssaoNoiseTex = FuseeSsaoHelper.CreateNoiseTex(noiseTexSize);

            //TODO: is there a smart(er) way to set #define KERNEL_LENGTH in file?
            var ps = AssetStorage.Get<string>("SSAO.frag");

            if (kernelLength != 64)
            {
                var lines = ps.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                lines[1] = $"#define KERNEL_LENGTH {kernelLength}";
                ps = string.Join("\n", lines);
            }

            return new ShaderEffect(

            new FxPassDeclaration
            {
                VS = AssetStorage.Get<string>("Deferred.vert"),
                PS = ps,
                StateSet = new RenderStateSet
                {
                    AlphaBlendEnable = false,
                    ZEnable = true,
                }

            },
            new IFxParamDeclaration[]
            {
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Position], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Position]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Normal], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Normal]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Albedo], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Albedo]},

                new FxParamDeclaration<float2> {Name = UniformNameDeclarations.ScreenParams, Value = screenParams},
                new FxParamDeclaration<float3[]> {Name = UniformNameDeclarations.SSAOKernel, Value = ssaoKernel},
                new FxParamDeclaration<Texture> {Name = UniformNameDeclarations.NoiseTex, Value = ssaoNoiseTex},
                new FxParamDeclaration<float4x4> {Name = UniformNameDeclarations.Projection, Value = float4x4.Identity},
            });

        }

        /// <summary>
        /// Creates a blurred ssao texture, to hide rectangular artifacts originating from the noise texture;
        /// </summary>
        /// <param name="ssaoRenderTex">The non blurred ssao texture.</param>
        public static ShaderEffect SSAORenderTargetBlurEffect(WritableTexture ssaoRenderTex)
        {
            //TODO: is there a smart(er) way to set #define KERNEL_LENGTH in file?
            var frag = AssetStorage.Get<string>("SimpleBlur.frag");
            var blurKernelSize = ssaoRenderTex.Width switch
            {
                (int)TexRes.Low => 2.0f,
                (int)TexRes.High => 8.0f,
                _ => 4.0f,
            };
            if (blurKernelSize != 4.0f)
            {
                var lines = frag.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                lines[2] = $"#define KERNEL_SIZE_HALF {blurKernelSize * 0.5}";
                frag = string.Join("\n", lines);
            }

            return new ShaderEffect(
            new FxPassDeclaration
            {
                VS = AssetStorage.Get<string>("Deferred.vert"),
                PS = frag,
                StateSet = new RenderStateSet
                {
                    AlphaBlendEnable = false,
                    ZEnable = true,
                }

            },
            new IFxParamDeclaration[]
            {
                new FxParamDeclaration<WritableTexture> { Name = "InputTex", Value = ssaoRenderTex},

            });
        }

        /// <summary>
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMap">The shadow map.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>            
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(IRenderTarget srcRenderTarget, Light lc, float4 backgroundColor, IWritableTexture shadowMap = null)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            if (lc.IsCastingShadows)
            {
                effectParams.Add(new FxParamDeclaration<int> { Name = "light.isCastingShadows", Value = 0 });
                effectParams.Add(new FxParamDeclaration<float> { Name = "light.bias", Value = 0.0f });
                if (lc.Type != LightType.Point)
                {
                    effectParams.Add(new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.LightSpaceMatrix, Value = float4x4.Identity });
                    effectParams.Add(new FxParamDeclaration<WritableTexture> { Name = UniformNameDeclarations.ShadowMap, Value = (WritableTexture)shadowMap });
                }
                else
                    effectParams.Add(new FxParamDeclaration<WritableCubeMap> { Name = UniformNameDeclarations.ShadowCubeMap, Value = (WritableCubeMap)shadowMap });
            }

            effectParams.AddRange(DeferredLightParams(lc.Type));

            return new ShaderEffect(
            new FxPassDeclaration
            {
                VS = AssetStorage.Get<string>("Deferred.vert"),
                PS = CreateDeferredLightingPixelShader(lc),
                StateSet = new RenderStateSet
                {
                    AlphaBlendEnable = true,
                    ZEnable = true,
                    BlendOperation = BlendOperation.Add,
                    SourceBlend = Blend.One,
                    DestinationBlend = Blend.One,
                    ZFunc = Compare.LessEqual,
                }
            },
            effectParams.ToArray());
        }

        /// <summary>
        /// [Parallel light only] ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass. Shadow is calculated with cascaded shadow maps.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMap">The cascaded shadow maps.</param>
        /// <param name="clipPlanes">The clip planes of the frustums. Each frustum is associated with one shadow map.</param>
        /// <param name="numberOfCascades">The number of sub-frustums, used for cascaded shadow mapping.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(IRenderTarget srcRenderTarget, Light lc, WritableArrayTexture shadowMap, float2[] clipPlanes, int numberOfCascades, float4 backgroundColor)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            if (lc.IsCastingShadows)
            {
                effectParams.Add(new FxParamDeclaration<int> { Name = "light.isCastingShadows", Value = 0 });
                effectParams.Add(new FxParamDeclaration<float> { Name = "light.bias", Value = 0.0f });
            }

            effectParams.Add(new FxParamDeclaration<float4x4[]> { Name = "LightSpaceMatrices[0]", Value = Array.Empty<float4x4>() });
            effectParams.Add(new FxParamDeclaration<WritableArrayTexture> { Name = "ShadowMap", Value = shadowMap });
            effectParams.Add(new FxParamDeclaration<float2[]> { Name = "ClipPlanes[0]", Value = clipPlanes });

            effectParams.AddRange(DeferredLightParams(lc.Type));

            return new ShaderEffect(
            new FxPassDeclaration
            {
                VS = AssetStorage.Get<string>("Deferred.vert"),
                PS = CreateDeferredLightingPixelShader(lc, true, numberOfCascades),
                StateSet = new RenderStateSet
                {
                    AlphaBlendEnable = true,
                    ZEnable = true,
                    BlendOperation = BlendOperation.Add,
                    SourceBlend = Blend.One,
                    DestinationBlend = Blend.One,
                    ZFunc = Compare.LessEqual,
                }

            },
            effectParams.ToArray());
        }

        /// <summary>
        /// ShaderEffect that renders the depth map from a lights point of view - this depth map is used as a shadow map.
        /// </summary>
        /// <returns></returns>
        public static ShaderEffect ShadowCubeMapEffect(float4x4[] lightSpaceMatrices)
        {
            var effectParamDecls = new List<IFxParamDeclaration>
            {
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.Model, Value = float4x4.Identity },
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.View, Value = float4x4.Identity },
                new FxParamDeclaration<float2> { Name = "LightMatClipPlanes", Value = float2.One },
                new FxParamDeclaration<float3> { Name = "LightPos", Value = float3.One },
                new FxParamDeclaration<float4x4[]> { Name = $"LightSpaceMatrices[0]", Value = lightSpaceMatrices }
            };

            return new ShaderEffect(
            new FxPassDeclaration
            {
                VS = AssetStorage.Get<string>("ShadowCubeMap.vert"),
                GS = AssetStorage.Get<string>("ShadowCubeMap.geom"),
                PS = AssetStorage.Get<string>("ShadowCubeMap.frag"),
                StateSet = new RenderStateSet
                {
                    AlphaBlendEnable = false,
                    ZEnable = true,
                    CullMode = Cull.Clockwise,
                    ZFunc = Compare.LessEqual,
                }

            },
            effectParamDecls.ToArray());
        }

        /// <summary>
        /// ShaderEffect that renders the depth map from a lights point of view - this depth map is used as a shadow map.
        /// </summary>
        /// <returns></returns>
        public static ShaderEffect ShadowMapEffect()
        {
            return new ShaderEffect(
            new FxPassDeclaration
            {
                VS = AssetStorage.Get<string>("ShadowMap.vert"),
                PS = AssetStorage.Get<string>("ShadowMap.frag"),
                StateSet = new RenderStateSet
                {
                    AlphaBlendEnable = false,
                    ZEnable = true,
                    CullMode = Cull.Clockwise,
                    ZFunc = Compare.LessEqual,
                }
            },
            new IFxParamDeclaration[]
            {
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.Model, Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.LightSpaceMatrix, Value = float4x4.Identity},
            });
        }

        private static List<IFxParamDeclaration> DeferredLightingEffectParams(IRenderTarget srcRenderTarget, float4 backgroundColor)
        {
            return new List<IFxParamDeclaration>()
            {
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Position], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Position]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Normal], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Normal]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Albedo], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Albedo]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Ssao], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Ssao]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Specular], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Specular]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Emission], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Emission]},
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.IView, Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.View, Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.ITView, Value = float4x4.Identity},
                new FxParamDeclaration<int> { Name = UniformNameDeclarations.RenderPassNo, Value = 0},
                new FxParamDeclaration<float4> { Name = UniformNameDeclarations.BackgroundColor, Value = backgroundColor},
                new FxParamDeclaration<int> { Name = UniformNameDeclarations.SsaoOn, Value = 1},
            };
        }

        private static List<IFxParamDeclaration> DeferredLightParams(LightType type)
        {
            return type switch
            {
                LightType.Point => new List<IFxParamDeclaration>()
                    {
                        new FxParamDeclaration<float3> { Name = "light.position", Value = new float3(0, 0, -1.0f) },
                        new FxParamDeclaration<float4> { Name = "light.intensities", Value = float4.Zero },
                        new FxParamDeclaration<float> { Name = "light.maxDistance", Value = 0.0f },
                        new FxParamDeclaration<float> { Name = "light.strength", Value = 0.0f },
                        new FxParamDeclaration<int> { Name = "light.isActive", Value = 1 }
                    },
                LightType.Legacy or LightType.Parallel => new List<IFxParamDeclaration>()
                    {
                        new FxParamDeclaration<float4> { Name = "light.intensities", Value = float4.Zero },
                        new FxParamDeclaration<float3> { Name = "light.direction", Value = float3.Zero },
                        new FxParamDeclaration<float> { Name = "light.strength", Value = 0.0f },
                        new FxParamDeclaration<int> { Name = "light.isActive", Value = 1 }
                    },
                _ => new List<IFxParamDeclaration>()
                    {
                        new FxParamDeclaration<float3> { Name = "light.position", Value = new float3(0, 0, -1.0f) },
                        new FxParamDeclaration<float4> { Name = "light.intensities", Value = float4.Zero },
                        new FxParamDeclaration<float> { Name = "light.maxDistance", Value = 0.0f },
                        new FxParamDeclaration<float> { Name = "light.strength", Value = 0.0f },
                        new FxParamDeclaration<float> { Name = "light.outerConeAngle", Value = 0.0f },
                        new FxParamDeclaration<float> { Name = "light.innerConeAngle", Value = 0.0f },
                        new FxParamDeclaration<float3> { Name = "light.direction", Value = float3.Zero },
                        new FxParamDeclaration<int> { Name = "light.isActive", Value = 1 }
                    },
            };
        }

        #endregion

        #region Make Effect from parameters

        #region unlit

        /// <summary>
        /// Creates a simple unlit effect from an color only.
        /// </summary>
        /// <param name="albedoColor">The albedo color.</param>
        /// <returns></returns>
        public static DefaultSurfaceEffect FromUnlit(float4 albedoColor)
        {
            var input = new ColorInput()
            {
                Albedo = albedoColor
            };
            return new DefaultSurfaceEffect(LightingSetupFlags.Unlit, input, FragShards.SurfOutBody_Color, VertShards.SufOutBody_Pos);
        }

        /// <summary>
        /// Creates a simple unlit shader from an albedo color and texture.
        /// </summary>
        /// <param name="albedoColor">The albedo color.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <returns></returns>
        public static DefaultSurfaceEffect FromUnlitAlbedoTexture(float4 albedoColor, Texture albedoTex, float2 texTiles, float albedoMix)
        {
            var lightingSetup = LightingSetupFlags.Unlit | LightingSetupFlags.AlbedoTex;
            var input = new TextureInputColorUnlit()
            {
                Albedo = albedoColor,
                AlbedoTex = albedoTex,
                AlbedoMix = albedoMix,
                TexTiles = texTiles
            };
            return new DefaultSurfaceEffect(lightingSetup, input, FragShards.SurfOutBody_Textures(lightingSetup), VertShards.SufOutBody_Pos);
        }

        #endregion

        #region diffuse

        /// <summary>
        /// Builds a simple shader effect with diffuse component.
        /// </summary>
        /// <param name="albedoColor">The diffuse color the resulting effect.</param>
        /// <param name="roughness">If 0.0 (default value) the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static DefaultSurfaceEffect FromDiffuse(float4 albedoColor, float roughness = 0f)
        {
            var input = new ColorInput()
            {
                Albedo = albedoColor,
                Roughness = roughness
            };
            return new DefaultSurfaceEffect(LightingSetupFlags.DiffuseOnly, input, FragShards.SurfOutBody_Color, VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse component.
        /// </summary>
        /// <param name="albedoColor">The diffuse color the resulting effect.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">If 0.0 (default value) the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static DefaultSurfaceEffect FromDiffuseAlbedoTexture(float4 albedoColor, Texture albedoTex, float2 texTiles, float albedoMix, float roughness = 0f)
        {
            var input = new TextureInputColor()
            {
                Albedo = albedoColor,
                AlbedoTex = albedoTex,
                AlbedoMix = albedoMix,
                TexTiles = texTiles,
                Roughness = roughness
            };

            var lighingSetup = LightingSetupFlags.DiffuseOnly | LightingSetupFlags.AlbedoTex;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse component.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">If 0.0 (default value) the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.</param>
        public static DefaultSurfaceEffect FromDiffuseNormalTexture(float4 albedoColor, Texture normalTex, float normalMapStrength, float2 texTiles, float roughness = 0f)
        {
            var input = new TextureInputColor()
            {
                Albedo = albedoColor,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles,
                Roughness = roughness
            };

            var lighingSetup = LightingSetupFlags.DiffuseOnly | LightingSetupFlags.NormalMap;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse component.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">If 0.0 (default value) the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.</param>
        public static DefaultSurfaceEffect FromDiffuseTexture(float4 albedoColor, Texture albedoTex, Texture normalTex, float albedoMix, float2 texTiles, float normalMapStrength = 0.5f, float roughness = 0f)
        {
            var input = new TextureInputColor()
            {
                Albedo = albedoColor,
                AlbedoMix = albedoMix,
                AlbedoTex = albedoTex,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles,
                Roughness = roughness
            };

            var lighingSetup = LightingSetupFlags.DiffuseOnly | LightingSetupFlags.AlbedoTex | LightingSetupFlags.NormalMap;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }

        #endregion

        #region diffuse specular

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular components.
        /// </summary>
        /// <param name="albedoColor">The diffuse color the resulting effect.</param>
        /// <param name="emissionColor">If this color isn't black the material emits it. Note that this will not have any effect on global illumination yet.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="specularStrength">The resulting effects specular intensity.</param>
        /// <param name="roughness">If 0.0 (default value) the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static DefaultSurfaceEffect FromDiffuseSpecular(float4 albedoColor, float4 emissionColor, float shininess = 255, float specularStrength = 0.0f, float roughness = 0f)
        {
            var input = new SpecularInput()
            {
                Albedo = albedoColor,
                Emission = emissionColor,
                Shininess = shininess,
                SpecularStrength = specularStrength,
                Roughness = roughness
            };
            return new DefaultSurfaceEffect(LightingSetupFlags.DiffuseSpecular, input, FragShards.SurfOutBody_DiffSpecular, VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="emissionColor">If this color isn't black the material emits it. Note that this will not have any effect on global illumination yet.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="specularStrength">The resulting effects specular intensity.</param>
        /// <param name="roughness">If 0.0 (default value) the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.</param>
        public static DefaultSurfaceEffect FromDiffuseSpecularAlbedoTexture(float4 albedoColor, float4 emissionColor, Texture albedoTex, float albedoMix, float2 texTiles, float shininess = 255, float specularStrength = 0.0f, float roughness = 0f)
        {
            var input = new TextureInputSpecular()
            {
                Albedo = albedoColor,
                Emission = emissionColor,
                Shininess = shininess,
                SpecularStrength = specularStrength,
                AlbedoMix = albedoMix,
                AlbedoTex = albedoTex,
                TexTiles = texTiles,
                Roughness = roughness
            };

            var lighingSetup = LightingSetupFlags.DiffuseSpecular | LightingSetupFlags.AlbedoTex;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="emissionColor">If this color isn't black the material emits it. Note that this will not have any effect on global illumination yet.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="specularStrength">The resulting effects specular intensity.</param>
        /// <param name="roughness">If 0.0 (default value) the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.</param>
        public static DefaultSurfaceEffect FromDiffuseSpecularNormalTexture(float4 albedoColor, float4 emissionColor, Texture normalTex, float normalMapStrength, float2 texTiles, float shininess = 255, float specularStrength = 0.0f, float roughness = 0f)
        {
            var input = new TextureInputSpecular()
            {
                Albedo = albedoColor,
                Emission = emissionColor,
                Shininess = shininess,
                SpecularStrength = specularStrength,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles,
                Roughness = roughness
            };

            var lighingSetup = LightingSetupFlags.DiffuseSpecular | LightingSetupFlags.NormalMap;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="emissionColor">If this color isn't black the material emits it. Note that this will not have any effect on global illumination yet.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="specularStrength">The resulting effects specular intensity.</param>
        /// <param name="roughness">If 0.0 (default value) the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.</param>
        public static DefaultSurfaceEffect FromDiffuseSpecularTexture(float4 albedoColor, float4 emissionColor, Texture albedoTex, Texture normalTex, float albedoMix, float2 texTiles, float shininess = 255, float specularStrength = 0.5f, float normalMapStrength = 0.5f, float roughness = 0f)
        {
            var input = new TextureInputSpecular()
            {
                Albedo = albedoColor,
                Emission = emissionColor,
                Shininess = shininess,
                SpecularStrength = specularStrength,
                AlbedoMix = albedoMix,
                AlbedoTex = albedoTex,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles,
                Roughness = roughness
            };

            var lighingSetup = LightingSetupFlags.DiffuseSpecular | LightingSetupFlags.AlbedoTex | LightingSetupFlags.NormalMap;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }
        #endregion

        #region glossy

        /// <summary>
        /// Builds a simple metallic shader effect - used for metals or mirrors.
        /// </summary>
        /// <param name="albedoColor">The albedo color the resulting effect.</param>
        /// <param name="roughness">Used to calculate the GGX microfacet distribution.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static DefaultSurfaceEffect FromGlossy(float4 albedoColor, float roughness = 0f)
        {
            var input = new ColorInput()
            {
                Albedo = albedoColor,
                Roughness = roughness
            };
            return new DefaultSurfaceEffect(LightingSetupFlags.Glossy, input, FragShards.SurfOutBody_Color, VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple metallic shader effect - used for metals or mirrors.
        /// </summary>
        /// <param name="albedoColor">The albedo color the resulting effect.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the albedo color and the color from the texture are mixed.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">Used to calculate the GGX microfacet distribution.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static DefaultSurfaceEffect FromGlossyAlbedoTexture(float4 albedoColor, Texture albedoTex, float2 texTiles, float albedoMix, float roughness = 0f)
        {
            var input = new TextureInputColor()
            {
                Albedo = albedoColor,
                AlbedoTex = albedoTex,
                AlbedoMix = albedoMix,
                TexTiles = texTiles,
                Roughness = roughness
            };

            var lighingSetup = LightingSetupFlags.Glossy | LightingSetupFlags.AlbedoTex;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse component.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">Used to calculate the GGX microfacet distribution.</param>
        public static DefaultSurfaceEffect FromGlossyNormalTexture(float4 albedoColor, Texture normalTex, float normalMapStrength, float2 texTiles, float roughness = 0f)
        {
            var input = new TextureInputColor()
            {
                Albedo = albedoColor,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles,
                Roughness = roughness
            };

            var lighingSetup = LightingSetupFlags.Glossy | LightingSetupFlags.NormalMap;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse component.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">Used to calculate the GGX microfacet distribution.</param>
        public static DefaultSurfaceEffect FromGlossyTexture(float4 albedoColor, Texture albedoTex, Texture normalTex, float albedoMix, float2 texTiles, float normalMapStrength = 0.5f, float roughness = 0f)
        {
            var input = new TextureInputColor()
            {
                Albedo = albedoColor,
                AlbedoMix = albedoMix,
                AlbedoTex = albedoTex,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles,
                Roughness = roughness
            };

            var lighingSetup = LightingSetupFlags.Glossy | LightingSetupFlags.AlbedoTex | LightingSetupFlags.NormalMap;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }

        #endregion

        #region BRDF

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular components.
        /// </summary>
        /// <param name="albedoColor">The diffuse color the resulting effect.</param>
        /// <param name="emissionColor">If this color isn't black the material emits it. Note that this will not have any effect on global illumination yet.</param>
        /// <param name="roughness">The roughness of the specular and diffuse reflection.</param>
        /// <param name="metallic">Value used to blend between the metallic and the dielectric model. </param>
        /// <param name="specular">Amount of dielectric specular reflection. </param>
        /// <param name="ior">The index of refraction. Note that this is set to 0.04 for dielectrics when rendering deferred.</param>
        /// <param name="subsurface">Mix between diffuse and subsurface scattering.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static DefaultSurfaceEffect FromBRDF(float4 albedoColor, float4 emissionColor, float roughness, float metallic, float specular, float ior, float subsurface)
        {
            var input = new BRDFInput()
            {
                Albedo = albedoColor,
                Emission = emissionColor,
                Roughness = roughness,
                Metallic = metallic,
                Specular = specular,
                IOR = ior,
                Subsurface = subsurface
            };
            return new DefaultSurfaceEffect(LightingSetupFlags.BRDF, input, FragShards.SurfOutBody_BRDF, VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="emissionColor">If this color isn't black the material emits it. Note that this will not have any effect on global illumination yet.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">The roughness of the specular and diffuse reflection.</param>
        /// <param name="metallic">Value used to blend between the metallic and the dielectric model. </param>
        /// <param name="specular">Amount of dielectric specular reflection. </param>
        /// <param name="ior">The index of refraction. Note that this is set to 0.04 for dielectrics when rendering deferred.</param>
        /// <param name="subsurface">Mix between diffuse and subsurface scattering.</param>
        public static DefaultSurfaceEffect FromBRDFAlbedoTexture(float4 albedoColor, float4 emissionColor, float roughness, float metallic, float specular, float ior, float subsurface, Texture albedoTex, float albedoMix, float2 texTiles)
        {
            var input = new TextureInputBRDF()
            {
                Albedo = albedoColor,
                Emission = emissionColor,
                AlbedoMix = albedoMix,
                AlbedoTex = albedoTex,
                Roughness = roughness,
                Metallic = metallic,
                Specular = specular,
                IOR = ior,
                Subsurface = subsurface,
                TexTiles = texTiles
            };

            var lighingSetup = LightingSetupFlags.BRDF | LightingSetupFlags.AlbedoTex;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="emissionColor">If this color isn't black the material emits it. Note that this will not have any effect on global illumination yet.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">The roughness of the specular and diffuse reflection.</param>
        /// <param name="metallic">Value used to blend between the metallic and the dielectric model. </param>
        /// <param name="specular">Amount of dielectric specular reflection. </param>
        /// <param name="ior">The index of refraction. Note that this is set to 0.04 for dielectrics when rendering deferred.</param>
        /// <param name="subsurface">Mix between diffuse and subsurface scattering.</param>
        public static DefaultSurfaceEffect FromBRDFNormalTexture(float4 albedoColor, float4 emissionColor, float roughness, float metallic, float specular, float ior, float subsurface, Texture normalTex, float normalMapStrength, float2 texTiles)
        {
            var input = new TextureInputBRDF()
            {
                Albedo = albedoColor,
                Emission = emissionColor,
                Roughness = roughness,
                Metallic = metallic,
                Specular = specular,
                IOR = ior,
                Subsurface = subsurface,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles
            };

            var lighingSetup = LightingSetupFlags.BRDF | LightingSetupFlags.NormalMap;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="emissionColor">If this color isn't black the material emits it. Note that this will not have any effect on global illumination yet.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">The roughness of the specular and diffuse reflection.</param>
        /// <param name="metallic">Value used to blend between the metallic and the dielectric model. </param>
        /// <param name="specular">Amount of dielectric specular reflection. </param>
        /// <param name="ior">The index of refraction. Note that this is set to 0.04 for dielectrics when rendering deferred.</param>
        /// <param name="subsurface">Mix between diffuse and subsurface scattering.</param>
        public static DefaultSurfaceEffect FromBRDFTexture(float4 albedoColor, float4 emissionColor, float roughness, float metallic, float specular, float ior, float subsurface, Texture albedoTex, Texture normalTex, float albedoMix, float2 texTiles, float normalMapStrength = 0.5f)
        {
            var input = new TextureInputBRDF()
            {
                Albedo = albedoColor,
                Emission = emissionColor,
                AlbedoMix = albedoMix,
                AlbedoTex = albedoTex,
                Roughness = roughness,
                Metallic = metallic,
                Specular = specular,
                IOR = ior,
                Subsurface = subsurface,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles
            };

            var lighingSetup = LightingSetupFlags.BRDF | LightingSetupFlags.AlbedoTex | LightingSetupFlags.NormalMap;
            return new DefaultSurfaceEffect(lighingSetup, input, FragShards.SurfOutBody_Textures(lighingSetup), VertShards.SufOutBody_PosNorm);
        }
        #endregion

        #endregion

        #region Create Shaders from code fragments

        private static string CreateDeferredLightingPixelShader(Light lc, bool isCascaded = false, int numberOfCascades = 0, bool debugCascades = false)
        {
            var frag = new StringBuilder();
            frag.Append(Header.Version300Es);
            frag.Append(Header.DefinePi);
            frag.Append("#extension GL_ARB_explicit_uniform_location : enable\n");
            frag.Append("#extension GL_ARB_gpu_shader5 : enable\n");
            frag.Append(Header.EsPrecisionHighpFloat);

            frag.Append(FragProperties.DeferredTextureUniforms());
            frag.Append(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.IView));

            frag.Append(Lighting.LightStructDeclaration);
            frag.Append(FragProperties.DeferredLightAndShadowUniforms(lc, isCascaded, numberOfCascades));

            frag.Append(GLSL.CreateIn(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates));

            frag.Append(FragProperties.ColorOut());

            //Shadow calculation methods
            //-------------------------------------- 
            if (isCascaded)
                frag.Append(Lighting.ShadowCalculationCascaded());
            else if (lc.Type == LightType.Point)
                frag.Append(Lighting.ShadowCalculationCubeMap());

            else
                frag.Append(Lighting.ShadowCalculation());

            //Lighting methods
            //------------------------------------------
            frag.Append(Lighting.SchlickFresnel());
            frag.Append(Lighting.G1());
            //frag.Append(Lighting.GetF0());
            frag.Append(Lighting.LambertDiffuseComponent());
            frag.Append(Lighting.OrenNayarDiffuseComponent());
            frag.Append(Lighting.DisneyDiffuseComponent());
            frag.Append(Lighting.SpecularComponent());
            frag.Append(Lighting.BRDFSpecularComponent());
            frag.Append(Lighting.AttenuationPointComponent());
            frag.Append(Lighting.AttenuationConeComponent());
            frag.Append(Lighting.GammaCorrection());
            frag.Append(Lighting.EncodeSRGB());
            frag.Append(Lighting.DecodeSRGB());

            frag.Append(Lighting.ApplyLightDeferred(lc, isCascaded, numberOfCascades, debugCascades));

            return frag.ToString();
        }

        #endregion
    }
}