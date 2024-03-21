using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        public static SurfaceEffect Default() => FromDiffuseSpecular(new float4(0.5f, 0.5f, 0.5f, 1.0f), 0f, 22, 1.0f);

        #region Line

        /// <summary>
        /// Generates a line shader which can be used with a <see cref="Mesh"/> with <see cref="Mesh.MeshType"/> set to <see cref="PrimitiveType.Lines"/>.        /// Loads shader files via <see cref="AssetStorage.Get{T}(string)"/>
        /// For an asynchronous version use <see cref="LineEffectAsync(float, float4, bool)"/>
        /// </summary>
        /// <param name="lineThickness"></param>
        /// <param name="albedoColor"></param>
        /// <param name="enableVertexColors"></param>
        /// <returns></returns>
        public static Effect LineEffect(float lineThickness, float4 albedoColor, bool enableVertexColors = false)
        {
            var vs = AssetStorage.Get<string>("line.vert");
            var gs = AssetStorage.Get<string>("line.geom");
            var ps = AssetStorage.Get<string>("line.frag");
            var uniformParameters = new List<IFxParamDeclaration>
            {
                new FxParamDeclaration<float4x4>
                    { Name = UniformNameDeclarations.ModelViewProjection, Value = float4x4.Identity },
                new FxParamDeclaration<float4x4>
                    { Name = UniformNameDeclarations.ModelView, Value = float4x4.Identity },
                new FxParamDeclaration<float4x4>
                    { Name = UniformNameDeclarations.Projection, Value = float4x4.Identity },
                new FxParamDeclaration<float> { Name = "Thickness", Value = lineThickness },
                new FxParamDeclaration<int2> { Name = UniformNameDeclarations.ViewportPx, Value = int2.Zero },
                new FxParamDeclaration<float4> { Name = "Albedo", Value = albedoColor },
                new FxParamDeclaration<bool> { Name = "EnableVertexColors", Value = enableVertexColors }
            };

            return new ShaderEffect(uniformParameters, RenderStateSet.Default, vs, ps, gs);
        }


        /// <summary>
        /// Generates a line shader which can be used with a <see cref="Mesh"/> with <see cref="Mesh.MeshType"/> set to <see cref="PrimitiveType.Lines"/>.        /// Loads shader files via <see cref="AssetStorage.Get{T}(string)"/>
        /// For an asynchronous version use <see cref="LineEffectAsync(float, float4, bool)"/>
        /// </summary>
        /// <param name="lineThickness"></param>
        /// <param name="albedoColor"></param>
        /// <param name="enableVertexColors"></param>
        /// <returns></returns>
        public static Effect LineEffectAdjacency(float lineThickness, float4 albedoColor, bool enableVertexColors = false)
        {
            var vs = AssetStorage.Get<string>("line.vert");
            var gs = AssetStorage.Get<string>("lineAdjacency.geom");
            var ps = AssetStorage.Get<string>("line.frag");
            var uniformParameters = new List<IFxParamDeclaration>
            {
                new FxParamDeclaration<float4x4>
                    { Name = UniformNameDeclarations.ModelViewProjection, Value = float4x4.Identity },
                new FxParamDeclaration<float4x4>
                    { Name = UniformNameDeclarations.ModelView, Value = float4x4.Identity },
                new FxParamDeclaration<float4x4>
                    { Name = UniformNameDeclarations.Projection, Value = float4x4.Identity },
                new FxParamDeclaration<float> { Name = "Thickness", Value = lineThickness },
                new FxParamDeclaration<int2> { Name = UniformNameDeclarations.ViewportPx, Value = int2.Zero },
                new FxParamDeclaration<float4> { Name = "Albedo", Value = albedoColor },
                new FxParamDeclaration<bool> { Name = "EnableVertexColors", Value = enableVertexColors }
            };

            return new ShaderEffect(uniformParameters, RenderStateSet.Default, vs, ps, gs);
        }

        /// <summary>
        /// Generates a line shader which can be used with a <see cref="Mesh"/> with <see cref="Mesh.MeshType"/> set to <see cref="PrimitiveType.Lines"/>.
        /// Loads shader files via <see cref="AssetStorage.GetAsync{T}(string)"/>
        /// For an non asynchronous version use <see cref="LineEffect(float, float4, bool)"/>
        /// </summary>
        /// <param name="lineThickness"></param>
        /// <param name="albedoColor"></param>
        /// <param name="enableVertexColors"></param>
        /// <returns></returns>
        public static async Task<Effect> LineEffectAsync(float lineThickness, float4 albedoColor, bool enableVertexColors = false)
        {
            var vs = await AssetStorage.GetAsync<string>("line.vert");
            var gs = await AssetStorage.GetAsync<string>("line.geom");
            var ps = await AssetStorage.GetAsync<string>("line.frag");
            var uniformParameters = new List<IFxParamDeclaration>
            {
                new FxParamDeclaration<float4x4>
                    { Name = UniformNameDeclarations.ModelViewProjection, Value = float4x4.Identity },
                new FxParamDeclaration<float4x4>
                    { Name = UniformNameDeclarations.ModelView, Value = float4x4.Identity },
                new FxParamDeclaration<float4x4>
                    { Name = UniformNameDeclarations.Projection, Value = float4x4.Identity },
                new FxParamDeclaration<float> { Name = "Thickness", Value = lineThickness },
                new FxParamDeclaration<int2> { Name = UniformNameDeclarations.ViewportPx, Value = int2.Zero },
                new FxParamDeclaration<float4> { Name = "Albedo", Value = albedoColor },
                new FxParamDeclaration<bool> { Name = "EnableVertexColors", Value = enableVertexColors }
            };

            return new ShaderEffect(uniformParameters, RenderStateSet.Default, vs, ps, gs);
        }

        #endregion

        #region Deferred

        /// <summary>
        /// If rendered with FXAA we'll need an additional (final) pass, that takes the lighted scene, rendered to a texture, as input.
        /// </summary>
        /// <param name="srcTex">RenderTarget, that contains a single texture in the Albedo/Specular channel, that contains the lighted scene.</param>
        /// <param name="screenParams">The width and height of the screen.</param>
        // see: http://developer.download.nvidia.com/assets/gamedev/files/sdk/11/FXAA_WhitePaper.pdf
        // http://blog.simonrodriguez.fr/articles/30-07-2016_implementing_fxaa.html
        public static ShaderEffect FXAARenderTargetEffect(WritableTexture srcTex, int2 screenParams)
        {
            return new ShaderEffect(
            effectParameters: new IFxParamDeclaration[]
            {
                new FxParamDeclaration<WritableTexture> { Name = RenderTargetTextureTypes.Albedo.ToString(), Value = srcTex}
            },
            rendererStates: new RenderStateSet
            {
                AlphaBlendEnable = false,
                ZEnable = true,
            },
            vs: DeferredShaders.DeferredVert,
            ps: DeferredShaders.FXAAFrag);
        }

        /// <summary>
        /// Shader effect for the ssao pass.
        /// </summary>
        /// <param name="geomPassRenderTarget">RenderTarget filled in the previous geometry pass.</param>
        /// <param name="kernelLength">SSAO kernel size.</param>
        /// <param name="screenParams">Width and Height of the screen.</param>
        /// <param name="noiseTexSize">Width and height of the noise texture.</param>
        public static ShaderEffect SSAORenderTargetTextureEffect(IRenderTarget geomPassRenderTarget, int kernelLength, int2 screenParams, int noiseTexSize)
        {
            var ssaoKernel = FuseeSsaoHelper.CreateKernel(kernelLength);
            var ssaoNoiseTex = FuseeSsaoHelper.CreateNoiseTex(noiseTexSize);

            //TODO: is there a smart(er) way to set #define KERNEL_LENGTH in file?
            var ps = DeferredShaders.SSAOFrag;

            if (kernelLength != 64)
            {
                var lines = ps.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                lines[1] = $"#define KERNEL_LENGTH {kernelLength}";
                ps = string.Join("\n", lines);
            }

            return new ShaderEffect(
            new IFxParamDeclaration[]
            {
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Position], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Position]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Normal], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Normal]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Albedo], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.Albedo]},

                new FxParamDeclaration<float3[]> {Name = UniformNameDeclarations.SSAOKernel, Value = ssaoKernel},
                new FxParamDeclaration<Texture> {Name = UniformNameDeclarations.NoiseTex, Value = ssaoNoiseTex}
            },
            new RenderStateSet
            {
                AlphaBlendEnable = false,
                ZEnable = true,
            },
            DeferredShaders.DeferredVert,
            ps);

        }

        /// <summary>
        /// Creates a blurred ssao texture, to hide rectangular artifacts originating from the noise texture;
        /// </summary>
        /// <param name="ssaoRenderTex">The non blurred ssao texture.</param>
        public static ShaderEffect SSAORenderTargetBlurEffect(WritableTexture ssaoRenderTex)
        {
            //TODO: is there a smart(er) way to set #define KERNEL_LENGTH in file?
            var frag = DeferredShaders.BlurFrag;
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
            new IFxParamDeclaration[]
            {
                new FxParamDeclaration<WritableTexture> { Name = "InputTex", Value = ssaoRenderTex},

            },
            new RenderStateSet
            {
                AlphaBlendEnable = false,
                ZEnable = true,
            },
            DeferredShaders.DeferredVert,
            frag);
        }

        /// <summary>
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass.
        /// </summary>
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMap">The shadow map.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(IRenderTarget? srcRenderTarget, Light lc, float4 backgroundColor, IWritableTexture? shadowMap = null)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            if (lc.IsCastingShadows)
            {

                if (lc.Type != LightType.Point)
                {
                    effectParams.Add(new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.LightSpaceMatrix, Value = float4x4.Identity });
                    effectParams.Add(new FxParamDeclaration<WritableTexture> { Name = UniformNameDeclarations.ShadowMap, Value = (WritableTexture?)shadowMap });
                }
                else
                    effectParams.Add(new FxParamDeclaration<WritableCubeMap> { Name = UniformNameDeclarations.ShadowCubeMap, Value = (WritableCubeMap?)shadowMap });
            }

            effectParams.AddRange(DeferredLightParams());

            return new ShaderEffect(
            effectParams.ToArray(),
            new RenderStateSet
            {
                AlphaBlendEnable = true,
                ZEnable = true,
                BlendOperation = BlendOperation.Add,
                SourceBlend = Blend.One,
                DestinationBlend = Blend.One,
                ZFunc = Compare.LessEqual,
            },
            DeferredShaders.DeferredVert,
            CreateDeferredLightingPixelShader(lc));
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
        public static ShaderEffect DeferredLightingPassEffect(IRenderTarget srcRenderTarget, Light lc, WritableArrayTexture? shadowMap, float2[]? clipPlanes, int numberOfCascades, float4 backgroundColor)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            //if (lc.IsCastingShadows)
            //{
            //    effectParams.Add(new FxParamDeclaration<int> { Name = "light.isCastingShadows", Value = 0 });
            //    effectParams.Add(new FxParamDeclaration<float> { Name = "light.bias", Value = 0.0f });
            //}

            effectParams.Add(new FxParamDeclaration<float4x4[]> { Name = $"{UniformNameDeclarations.LightSpaceMatrices}[0]", Value = Array.Empty<float4x4>() });
            effectParams.Add(new FxParamDeclaration<WritableArrayTexture> { Name = $"{UniformNameDeclarations.ShadowMap}", Value = shadowMap });
            effectParams.Add(new FxParamDeclaration<float2[]> { Name = $"{UniformNameDeclarations.LightMatClipPlanes}[0]", Value = clipPlanes });

            effectParams.AddRange(DeferredLightParams());

            return new ShaderEffect(

                effectParams.ToArray(),
                new RenderStateSet
                {
                    AlphaBlendEnable = true,
                    ZEnable = true,
                    BlendOperation = BlendOperation.Add,
                    SourceBlend = Blend.One,
                    DestinationBlend = Blend.One,
                    ZFunc = Compare.LessEqual,
                },
                DeferredShaders.DeferredVert,
                CreateDeferredLightingPixelShader(lc, true, numberOfCascades)
            );
        }

        /// <summary>
        /// ShaderEffect that renders the depth map from a lights point of view - this depth map is used as a shadow map.
        /// </summary>
        /// <returns></returns>
        public static ShaderEffect ShadowCubeMapEffectPointPrimitives(float4x4[] lightSpaceMatrices)
        {
            var effectParamDecls = new List<IFxParamDeclaration>
            {
                new FxParamDeclaration<float2> { Name = $"{UniformNameDeclarations.LightMatClipPlanes}", Value = float2.One },
                new FxParamDeclaration<float3> { Name = $"{UniformNameDeclarations.LightShadowPos}", Value = float3.One },
                new FxParamDeclaration<float4x4[]> { Name = $"{UniformNameDeclarations.LightSpaceMatrices}[0]", Value = lightSpaceMatrices }
            };

            return new ShaderEffect(
            effectParamDecls.ToArray(),
            new RenderStateSet
            {
                AlphaBlendEnable = false,
                ZEnable = true,
                CullMode = Cull.Clockwise,
                ZFunc = Compare.LessEqual,
                FillMode = FillMode.Point,
            },
            DeferredShaders.ShadowCubeMapVert,
            DeferredShaders.ShadowCubeMapFrag,
            DeferredShaders.ShadowCubeMapPointPrimitiveGeom);
        }

        /// <summary>
        /// ShaderEffect that renders the depth map from a lights point of view - this depth map is used as a shadow map.
        /// </summary>
        /// <returns></returns>
        public static ShaderEffect ShadowCubeMapEffect(float4x4[] lightSpaceMatrices)
        {
            var effectParamDecls = new List<IFxParamDeclaration>
            {
                new FxParamDeclaration<float2> { Name = $"{UniformNameDeclarations.LightMatClipPlanes}", Value = float2.One },
                new FxParamDeclaration<float3> { Name = $"{UniformNameDeclarations.LightShadowPos}", Value = float3.One },
                new FxParamDeclaration<float4x4[]> { Name = $"{UniformNameDeclarations.LightSpaceMatrices}[0]", Value = lightSpaceMatrices }
            };

            return new ShaderEffect(
                effectParamDecls.ToArray(),
                new RenderStateSet
                {
                    AlphaBlendEnable = false,
                    ZEnable = true,
                    CullMode = Cull.Clockwise,
                    ZFunc = Compare.LessEqual,
                },
                DeferredShaders.ShadowCubeMapVert,
                DeferredShaders.ShadowCubeMapFrag,
                DeferredShaders.ShadowCubeMapGeom);
        }

        /// <summary>
        /// ShaderEffect that renders the depth map from a lights point of view - this depth map is used as a shadow map.
        /// </summary>
        /// <returns></returns>
        public static ShaderEffect ShadowMapEffect()
        {
            return new ShaderEffect(
            new IFxParamDeclaration[]
            {
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.LightSpaceMatrix, Value = float4x4.Identity},
            },
            new RenderStateSet
            {
                AlphaBlendEnable = false,
                ZEnable = true,
                CullMode = Cull.Clockwise,
                ZFunc = Compare.LessEqual,
            },
            DeferredShaders.ShadowMapVert,
            DeferredShaders.ShadowMapFrag);
        }

        private static List<IFxParamDeclaration> DeferredLightingEffectParams(IRenderTarget? srcRenderTarget, float4 backgroundColor)
        {
            return new List<IFxParamDeclaration>()
            {
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Position], Value = srcRenderTarget?.RenderTextures[(int)RenderTargetTextureTypes.Position]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Normal], Value = srcRenderTarget?.RenderTextures[(int)RenderTargetTextureTypes.Normal]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Albedo], Value = srcRenderTarget?.RenderTextures[(int)RenderTargetTextureTypes.Albedo]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Ssao], Value = srcRenderTarget?.RenderTextures[(int)RenderTargetTextureTypes.Ssao]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Specular], Value = srcRenderTarget?.RenderTextures[(int)RenderTargetTextureTypes.Specular]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Emission], Value = srcRenderTarget?.RenderTextures[(int)RenderTargetTextureTypes.Emission]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Subsurface], Value = srcRenderTarget?.RenderTextures[(int)RenderTargetTextureTypes.Subsurface]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Depth], Value = srcRenderTarget?.RenderTextures[(int)RenderTargetTextureTypes.Depth]},

                new FxParamDeclaration<int> { Name = UniformNameDeclarations.RenderPassNo, Value = 0},
                new FxParamDeclaration<float4> { Name = UniformNameDeclarations.BackgroundColor, Value = backgroundColor},
                new FxParamDeclaration<int> { Name = UniformNameDeclarations.SsaoOn, Value = 1},
            };
        }

        private static List<IFxParamDeclaration> DeferredLightParams()
        {
            return new List<IFxParamDeclaration>()
            {
                new FxParamDeclaration<float3> { Name = $"light.{UniformNameDeclarations.LightWorldPos}", Value = new float3(0, 0, -1.0f) },
                new FxParamDeclaration<float4> { Name = $"light.{UniformNameDeclarations.LightIntensities}", Value = float4.Zero },
                new FxParamDeclaration<float> { Name = $"light.{UniformNameDeclarations.LightMaxDist}", Value = 0.0f },
                new FxParamDeclaration<float> { Name = $"light.{UniformNameDeclarations.LightStrength}", Value = 0.0f },
                new FxParamDeclaration<float> { Name = $"light.{UniformNameDeclarations.LightOuterConeAngle}", Value = 0.0f },
                new FxParamDeclaration<float> { Name = $"light.{UniformNameDeclarations.LightInnerConeAngle}", Value = 0.0f },
                new FxParamDeclaration<float3> { Name = $"light.{UniformNameDeclarations.LightDirection}", Value = float3.Zero },
                new FxParamDeclaration<int> { Name = $"light.{UniformNameDeclarations.LightIsActive}", Value = 1 },
                new FxParamDeclaration<int> { Name = $"light.{UniformNameDeclarations.LightType}", Value = 0 },
                new FxParamDeclaration<int> { Name = $"light.{UniformNameDeclarations.LightIsCastingShadows}", Value = 0 },
                new FxParamDeclaration<float> { Name = $"light.{UniformNameDeclarations.LightBias}", Value = 0.0f }
            };
        }

        #endregion

        #region Make Effect from parameters

        /// <summary>
        /// Creates a simple unlit shader from an albedo color and texture.
        /// </summary>
        /// <param name="albedoColor">The albedo color.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <returns></returns>
        public static SurfaceEffect FromUnlit(float4 albedoColor, Texture? albedoTex = null, float2 texTiles = new float2(), float albedoMix = 0)
        {
            var input = new UnlitInput()
            {
                TextureSetup = albedoTex != null ? TextureSetup.AlbedoTex : TextureSetup.NoTextures,
                Albedo = albedoColor,
                AlbedoTex = albedoTex,
                AlbedoMix = albedoMix,
                TexTiles = texTiles
            };
            return new SurfaceEffect(input);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse lighting component.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">If 0.0 (default value) the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.</param>
        /// <param name="emissionColor">If this color isn't black the material emits it. Note that this will not have any effect on global illumination yet.</param>
        public static SurfaceEffect FromDiffuse(float4 albedoColor, float roughness = 0f, float3 emissionColor = new float3(), Texture? albedoTex = null, float albedoMix = 0f, float2 texTiles = new float2(), Texture? normalTex = null, float normalMapStrength = 0.5f)
        {
            var input = new DiffuseInput()
            {
                Albedo = albedoColor,
                AlbedoMix = albedoMix,
                AlbedoTex = albedoTex,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles,
                Roughness = roughness,
                Emission = emissionColor
            };

            var texSetup = TextureSetup.NoTextures;
            if (albedoTex != null)
                texSetup |= TextureSetup.AlbedoTex;
            if (normalTex != null)
                texSetup |= TextureSetup.NormalMap;
            input.TextureSetup = texSetup;

            return new SurfaceEffect(input);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse lighting component. This will set up the underlying shader for instanced rendering.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">If 0.0 (default value) the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.</param>
        /// <param name="emissionColor">If this color isn't black the material emits it. Note that this will not have any effect on global illumination yet.</param>
        public static SurfaceEffect FromDiffuseInstanced(float4 albedoColor, float roughness = 0f, float3 emissionColor = new float3(), Texture? albedoTex = null, float albedoMix = 0f, float2 texTiles = new float2(), Texture? normalTex = null, float normalMapStrength = 0.5f)
        {
            var input = new DiffuseInput()
            {
                Albedo = albedoColor,
                AlbedoMix = albedoMix,
                AlbedoTex = albedoTex,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles,
                Roughness = roughness,
                Emission = emissionColor
            };

            var texSetup = TextureSetup.NoTextures;
            if (albedoTex != null)
                texSetup |= TextureSetup.AlbedoTex;
            if (normalTex != null)
                texSetup |= TextureSetup.NormalMap;
            input.TextureSetup = texSetup;

            return new SurfaceEffect(input, RenderFlags.Instanced);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular lighting components.
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
        public static SurfaceEffect FromDiffuseSpecular(float4 albedoColor, float roughness = 0f, float shininess = 255, float specularStrength = 0.5f, float3 emissionColor = new float3(), Texture? albedoTex = null, float albedoMix = 0f, float2 texTiles = new float2(), Texture? normalTex = null, float normalMapStrength = 0.5f)
        {
            var input = new SpecularInput()
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

            var texSetup = TextureSetup.NoTextures;
            if (albedoTex != null)
                texSetup |= TextureSetup.AlbedoTex;
            if (normalTex != null)
                texSetup |= TextureSetup.NormalMap;
            input.TextureSetup = texSetup;

            return new SurfaceEffect(input);
        }

        /// <summary>
        /// Builds a simple shader effect for a glossy lighting (full metallic setup).
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">Used to calculate the GGX micro facet distribution.</param>
        public static SurfaceEffect FromGlossy(float4 albedoColor, float roughness = 0f, Texture? albedoTex = null, float albedoMix = 0f, float2 texTiles = new float2(), Texture? normalTex = null, float normalMapStrength = 0.5f)
        {
            var input = new GlossyInput()
            {
                Albedo = albedoColor,
                AlbedoMix = albedoMix,
                AlbedoTex = albedoTex,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles,
                Roughness = roughness
            };

            var texSetup = TextureSetup.NoTextures;
            if (albedoTex != null)
                texSetup |= TextureSetup.AlbedoTex;
            if (normalTex != null)
                texSetup |= TextureSetup.NormalMap;
            input.TextureSetup = texSetup;

            return new SurfaceEffect(input);
        }

        /// <summary>
        /// Builds a simple shader effect for physically-based lighting using a bidirectional reflectance distribution function.
        /// </summary>
        /// <param name="albedoColor">The albedo color of the resulting effect.</param>
        /// <param name="emissionColor">If this color isn't black the material emits it. Note that this will not have any effect on global illumination yet.</param>
        /// <param name="subsurfaceColor">Subsurface scattering base color.</param>
        /// <param name="albedoTex">The albedo texture.</param>
        /// <param name="albedoMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="normalTex">The normal map.</param>
        /// <param name="normalMapStrength">The strength of the normal mapping effect.</param>
        /// <param name="thicknessMap">A texture containing the thickness of an object. Used for subsurface scattering.</param>
        /// <param name="texTiles">The number of times the textures are repeated in x and y direction.</param>
        /// <param name="roughness">The roughness of the specular and diffuse reflection.</param>
        /// <param name="metallic">Value used to blend between the metallic and the dielectric model. </param>
        /// <param name="specular">Amount of dielectric specular reflection. </param>
        /// <param name="ior">The index of refraction. Note that this is set to 0.04 for dielectrics when rendering deferred.</param>
        /// <param name="subsurface">Mix between diffuse and subsurface scattering.</param>
        public static SurfaceEffect FromBRDF(float4 albedoColor, float roughness, float metallic, float specular, float ior, float subsurface = 0f, float3 subsurfaceColor = new float3(), float3 emissionColor = new float3(), Texture? albedoTex = null, float albedoMix = 0f, float2 texTiles = new float2(), Texture? normalTex = null, float normalMapStrength = 0.5f, Texture? thicknessMap = null)
        {
            var input = new BRDFInput()
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
                SubsurfaceColor = subsurfaceColor,
                NormalTex = normalTex,
                NormalMappingStrength = normalMapStrength,
                TexTiles = texTiles,
                ThicknessMap = thicknessMap
            };

            var texSetup = TextureSetup.NoTextures;
            if (albedoTex != null)
                texSetup |= TextureSetup.AlbedoTex;
            if (normalTex != null)
                texSetup |= TextureSetup.NormalMap;
            if (thicknessMap != null)
                texSetup |= TextureSetup.ThicknessMap;
            input.TextureSetup = texSetup;

            return new SurfaceEffect(input);
        }
        #endregion

        #region Create Shaders from code fragments

        private static string CreateDeferredLightingPixelShader(Light lc, bool isCascaded = false, int numberOfCascades = 0, bool debugCascades = false)
        {
            var frag = new StringBuilder();
            if (ModuleExtensionPoint.PlatformId == Common.FuseePlatformId.Desktop)
                frag.AppendLine(Header.Version460Core);
            else if (ModuleExtensionPoint.PlatformId == Common.FuseePlatformId.Mesa)
                frag.AppendLine(Header.Version450Core);
            else
                frag.AppendLine(Header.Version300Es);
            frag.Append(Header.DefinePi);
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
            frag.Append(Lighting.LinearizeDepth());
            frag.Append(Lighting.EDLResponse());
            frag.Append(Lighting.EDLShadingFactor());
            frag.Append(Lighting.SchlickFresnel());
            frag.Append(Lighting.G1());
            frag.Append(Lighting.GetF0());
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