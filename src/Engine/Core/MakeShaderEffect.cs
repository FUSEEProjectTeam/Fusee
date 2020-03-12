using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderEffects;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Provides helper methods for creating <see cref="ShaderEffect"/>s.
    /// </summary>
    public static class MakeShaderEffect
    {
        /// <summary>
        /// The default ShaderEffect, that is used if a <see cref="SceneNodeContainer"/> has a mesh but no <see cref="ShaderEffect"/>.
        /// </summary>
        public static ShaderEffectDefault Default { get; } = new ShaderEffectDefault();

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
            //TODO: #define constants to uniforms
            return new ShaderEffect(new[]
            {
                new FxPassDeclaration
                {
                    VS = AssetStorage.Get<string>("Deferred.vert"),
                    PS = AssetStorage.Get<string>("FXAA.frag"),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new IFxParamDeclaration[]
            {
                new FxParamDeclaration<WritableTexture> { Name = RenderTargetTextureTypes.G_ALBEDO.ToString(), Value = srcTex},
                new FxParamDeclaration<float2> { Name = UniformNameDeclarations.ScreenParams, Value = screenParams},
            });

        }

        /// <summary>
        /// Shader effect for the ssao pass.
        /// </summary>        
        /// <param name="geomPassRenderTarget">RenderTarget filled in the previous geometry pass.</param>
        /// <param name="kernelLength">SSAO kernel size.</param>
        /// <param name="screenParams">Width and Height of the screen.</param>        
        public static ShaderEffect SSAORenderTargetTextureEffect(RenderTarget geomPassRenderTarget, int kernelLength, float2 screenParams)
        {
            var ssaoKernel = SSAOHelper.CreateKernel(kernelLength);
            var ssaoNoiseTex = SSAOHelper.CreateNoiseTex(16);

            //TODO: is there a smart(er) way to set #define KERNEL_LENGTH in file?
            var ps = AssetStorage.Get<string>("SSAO.frag");

            if (kernelLength != 64)
            {
                var lines = ps.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                lines[1] = $"#define KERNEL_LENGTH {kernelLength}";
                ps = string.Join("\n", lines);
            }

            return new ShaderEffect(new[]
            {
                new FxPassDeclaration
                {
                    VS = AssetStorage.Get<string>("Deferred.vert"),
                    PS = ps,
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new IFxParamDeclaration[]
            {
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_POSITION], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_POSITION]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_NORMAL], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_NORMAL]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO]},

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
            float blurKernelSize;
            switch (ssaoRenderTex.Width)
            {
                case (int)TexRes.LOW_RES:
                    blurKernelSize = 2.0f;
                    break;
                default:
                case (int)TexRes.MID_RES:
                    blurKernelSize = 4.0f;
                    break;
                case (int)TexRes.HIGH_RES:
                    blurKernelSize = 8.0f;
                    break;
            }

            if (blurKernelSize != 4.0f)
            {
                var lines = frag.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                lines[2] = $"#define KERNEL_SIZE_HALF {blurKernelSize * 0.5}";
                frag = string.Join("\n", lines);
            }

            return new ShaderEffect(new[]
            {
                new FxPassDeclaration
                {
                    VS = AssetStorage.Get<string>("Deferred.vert"),
                    PS = frag,
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
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
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, float4 backgroundColor, IWritableTexture shadowMap = null)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            if (lc.IsCastingShadows)
            {
                if (lc.Type != LightType.Point)
                {
                    effectParams.Add(new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.LightSpaceMatrix, Value = float4x4.Identity });
                    effectParams.Add(new FxParamDeclaration<WritableTexture> { Name = UniformNameDeclarations.ShadowMap, Value = (WritableTexture)shadowMap });
                }
                else
                    effectParams.Add(new FxParamDeclaration<WritableCubeMap> { Name = UniformNameDeclarations.ShadowCubeMap, Value = (WritableCubeMap)shadowMap });
            }

            return new ShaderEffect(new[]
            {
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
                }
            },
            effectParams.ToArray());
        }

        /// <summary>
        /// [Parallel light only] ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass. Shadow is calculated with cascaded shadow maps.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMaps">The cascaded shadow maps.</param>
        /// <param name="clipPlanes">The clip planes of the frustums. Each frustum is associated with one shadow map.</param>
        /// <param name="numberOfCascades">The number of sub-frustums, used for cascaded shadow mapping.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>            
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, WritableTexture[] shadowMaps, float2[] clipPlanes, int numberOfCascades, float4 backgroundColor)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            effectParams.Add(new FxParamDeclaration<float4x4[]> { Name = "LightSpaceMatrices[0]", Value = Array.Empty<float4x4>() });
            effectParams.Add(new FxParamDeclaration<WritableTexture[]> { Name = "ShadowMaps[0]", Value = shadowMaps });
            effectParams.Add(new FxParamDeclaration<float2[]> { Name = "ClipPlanes[0]", Value = clipPlanes });

            return new ShaderEffect(new[]
            {
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

            return new ShaderEffect(new[]
            {
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
            return new ShaderEffect(new[]
            {
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
                }
            },
            new IFxParamDeclaration[]
            {
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.Model, Value = float4x4.Identity},                
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.LightSpaceMatrix, Value = float4x4.Identity},                
            });
        }

        private static List<IFxParamDeclaration> DeferredLightingEffectParams(RenderTarget srcRenderTarget, float4 backgroundColor)
        {
            return new List<IFxParamDeclaration>()
            {
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_POSITION], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_POSITION]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_NORMAL], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_NORMAL]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_SSAO], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_SSAO]},
                new FxParamDeclaration<IWritableTexture> { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_SPECULAR], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_SPECULAR]},
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.IView, Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.View, Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> { Name = UniformNameDeclarations.ITView, Value = float4x4.Identity},
                new FxParamDeclaration<float3> { Name = "light.position", Value = new float3(0, 0, -1.0f)},                
                new FxParamDeclaration<float4> { Name = "light.intensities", Value = float4.Zero},
                new FxParamDeclaration<float> { Name = "light.maxDistance", Value = 0.0f},
                new FxParamDeclaration<float> { Name = "light.strength", Value = 0.0f},
                new FxParamDeclaration<float> { Name = "light.outerConeAngle", Value = 0.0f},
                new FxParamDeclaration<float> { Name = "light.innerConeAngle", Value = 0.0f},
                new FxParamDeclaration<float3> { Name = "light.direction", Value = float3.Zero},
                new FxParamDeclaration<int> { Name = "light.lightType", Value = 1},
                new FxParamDeclaration<int> { Name = "light.isActive", Value = 1},
                new FxParamDeclaration<int> { Name = "light.isCastingShadows", Value = 0},
                new FxParamDeclaration<float> { Name = "light.bias", Value = 0.0f},
                new FxParamDeclaration<int> { Name = UniformNameDeclarations.RenderPassNo, Value = 0},
                new FxParamDeclaration<float4> { Name = UniformNameDeclarations.BackgroundColor, Value = backgroundColor},
                new FxParamDeclaration<int> { Name = UniformNameDeclarations.SsaoOn, Value = 1},
            };
        }

        #endregion

        #region Make ShaderEffect from parameters

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffect FromDiffuseSpecular(float4 diffuseColor, float4 specularColor, float shininess, float specularIntensity = 0.5f)
        {
            MaterialComponent temp = new MaterialComponent
            {
                Diffuse = new MatChannelContainer
                {
                    Color = diffuseColor
                },
                Specular = new SpecularChannelContainer
                {
                    Color = specularColor,
                    Shininess = shininess,
                    Intensity = specularIntensity,
                }
            };
            return FromMatComp(temp);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffectProtoPixel ProtoFromDiffuseSpecular(float4 diffuseColor, float4 specularColor, float shininess, float specularIntensity = 0.5f)
        {
            MaterialComponent temp = new MaterialComponent
            {
                Diffuse = new MatChannelContainer
                {
                    Color = diffuseColor
                },
                Specular = new SpecularChannelContainer
                {
                    Color = specularColor,
                    Shininess = shininess,
                    Intensity = specularIntensity,
                }
            };
            return ProtoFromMatComp(temp);
        }

        /// <summary>
        /// Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="texName">Name of the texture you want to use.</param>
        /// <param name="diffuseMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffect FromDiffuseSpecularTexture(float4 diffuseColor, float4 specularColor, float shininess, string texName, float diffuseMix, float specularIntensity = 0.5f)
        {
            MaterialComponent temp = new MaterialComponent
            {
                Diffuse = new MatChannelContainer
                {
                    Color = diffuseColor,
                    Texture = texName,
                    Mix = diffuseMix
                },
                Specular = new SpecularChannelContainer
                {
                    Color = specularColor,
                    Shininess = shininess,
                    Intensity = specularIntensity,
                }
            };
            return FromMatComp(temp);
        }

        /// <summary>
        ///     Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="texName">Name of the texture you want to use.</param>
        /// <param name="diffuseMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffectProtoPixel ProtoFromDiffuseSpecularTexture(float4 diffuseColor, float4 specularColor, float shininess, string texName, float diffuseMix, float specularIntensity = 0.5f)
        {
            MaterialComponent temp = new MaterialComponent
            {
                Diffuse = new MatChannelContainer
                {
                    Color = diffuseColor,
                    Texture = texName,
                    Mix = diffuseMix
                },
                Specular = new SpecularChannelContainer
                {
                    Color = specularColor,
                    Shininess = shininess,
                    Intensity = specularIntensity,
                }
            };

            return ProtoFromMatComp(temp);
        }

        /// <summary> 
        /// Creates a ShaderEffectComponent from a MaterialComponent 
        /// </summary> 
        /// <param name="mc">The MaterialComponent</param> 
        /// <param name="wc">Only pass over a WeightComponent if you use bone animations in the current node (usage: pass currentNode.GetWeights())</param>        
        /// <returns></returns> 
        /// <exception cref="Exception"></exception> 
        public static ShaderEffect FromMatComp(MaterialComponent mc, WeightComponent wc = null)
        {
            var effectProps = ShaderShardUtil.CollectEffectProps(null, mc, wc);
            var vs = CreateVertexShader(wc, effectProps);
            var ps = CreatePixelShader(effectProps);
            var effectParameters = AssembleEffectParamers(mc);

            if (string.IsNullOrEmpty(vs) || string.IsNullOrEmpty(ps)) throw new Exception("Material could not be evaluated or be built!");

            var ret = new ShaderEffect(new[]
                {
                    new FxPassDeclaration
                    {
                        VS = vs,
                        PS = ps,
                        StateSet = new RenderStateSet
                        {
                            ZEnable = true,
                            AlphaBlendEnable = true,
                            SourceBlend = Blend.SourceAlpha,
                            DestinationBlend = Blend.InverseSourceAlpha,
                            BlendOperation = BlendOperation.Add,
                        }
                    }
                },
                effectParameters
            );

            return ret;
        }

        /// <summary> 
        /// Creates a ShaderEffectComponent from a MaterialComponent 
        /// </summary> 
        /// <param name="mc">The MaterialComponent</param> 
        /// <param name="wc">Only pass over a WeightComponent if you use bone animations in the current node (usage: pass currentNode.GetWeights())</param>        
        /// <returns></returns> 
        /// <exception cref="Exception"></exception> 
        public static ShaderEffectProtoPixel ProtoFromMatComp(MaterialComponent mc, WeightComponent wc = null)
        {
            var effectProps = ShaderShardUtil.CollectEffectProps(null, mc, wc);
            string vs = CreateVertexShader(wc, effectProps);
            string ps = CreateProtoPixelShader(effectProps);
            var effectParameters = AssembleEffectParamers(mc);

            if (string.IsNullOrEmpty(vs) || string.IsNullOrEmpty(ps)) throw new Exception("Material could not be evaluated or be built!");

            var ret = new ShaderEffectProtoPixel(new[]
                {
                    new FxPassDeclarationProto
                    {
                        VS = vs, 
                        //VS = VsBones, 
                        ProtoPS = ps,
                        StateSet = new RenderStateSet
                        {
                            ZEnable = true,
                            AlphaBlendEnable = true,
                            SourceBlend = Blend.SourceAlpha,
                            DestinationBlend = Blend.InverseSourceAlpha,
                            BlendOperation = BlendOperation.Add,
                        }
                    }
                },
                effectParameters
            )
            {
                EffectProps = effectProps
            };
            return ret;
        }

        #endregion

        #region Create Shaders from Shards

        private static string CreateVertexShader(WeightComponent wc, ShaderEffectProps effectProps)
        {
            var vertexShader = new List<string>
            {
                HeaderShard.Version300Es,
                HeaderShard.DefineBones(effectProps, wc),
                VertPropertiesShard.FuseeMatUniforms(effectProps),
                VertPropertiesShard.InAndOutParams(effectProps),
            };

            // Main            
            vertexShader.Add(VertMainShard.VertexMain(effectProps));

            return string.Join("\n", vertexShader);
        }

        private static string CreatePixelShader(ShaderEffectProps effectProps)
        {
            var pixelShader = new List<string>
            {
                HeaderShard.Version300Es,
                HeaderShard.EsPrecisionHighpFloat,

                LightingShard.LightStructDeclaration,

                FragPropertiesShard.InParams(effectProps),
                FragPropertiesShard.FuseeMatrixUniforms(),
                FragPropertiesShard.MaterialPropsUniforms(effectProps),
                FragPropertiesShard.FixedNumberLightArray,
                FragPropertiesShard.ColorOut(),
                LightingShard.AssembleLightingMethods(effectProps)
            };

            //Calculates the lighting for all lights by using the above method
            pixelShader.Add(FragMainShard.ForwardLighting(effectProps));

            return string.Join("\n", pixelShader);
        }

        private static string CreateProtoPixelShader(ShaderEffectProps effectProps)
        {
            var protoPixelShader = new List<string>
            {
                HeaderShard.Version300Es,
                HeaderShard.EsPrecisionHighpFloat,

                FragPropertiesShard.InParams(effectProps),
                FragPropertiesShard.FuseeMatrixUniforms(),
                FragPropertiesShard.MaterialPropsUniforms(effectProps),
            };

            return string.Join("\n", protoPixelShader);
        }

        private static string CreateDeferredLightingPixelShader(LightComponent lc, bool isCascaded = false, int numberOfCascades = 0, bool debugCascades = false)
        {
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version300Es);
            frag.Append("#extension GL_ARB_explicit_uniform_location : enable\n");
            frag.Append("#extension GL_ARB_gpu_shader5 : enable\n");
            frag.Append(HeaderShard.EsPrecisionHighpFloat);

            frag.Append(FragPropertiesShard.DeferredTextureUniforms());
            frag.Append(FragPropertiesShard.FuseeMatrixUniforms());

            frag.Append(LightingShard.LightStructDeclaration);
            frag.Append(FragPropertiesShard.DeferredLightAndShadowUniforms(lc, isCascaded, numberOfCascades));

            frag.Append(GLSL.CreateIn(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates));

            frag.Append(FragPropertiesShard.ColorOut());

            //Shadow calculation methods
            //-------------------------------------- 
            if (lc.Type != LightType.Point)
                frag.Append(LightingShard.ShadowCalculation());
            else
                frag.Append(LightingShard.ShadowCalculationCubeMap());

            //Lighting methods
            //------------------------------------------
            frag.Append(LightingShard.AmbientComponent());
            frag.Append(LightingShard.SpecularComponent());
            frag.Append(LightingShard.DiffuseComponent());
            frag.Append(LightingShard.AttenuationPointComponent());
            frag.Append(LightingShard.AttenuationConeComponent());

            frag.Append(LightingShard.ApplyLightDeferred(lc, isCascaded, numberOfCascades, debugCascades));

            return frag.ToString();
        }

        #endregion

        private static IEnumerable<IFxParamDeclaration> AssembleEffectParamers(MaterialComponent mc)
        {
            var effectParameters = new List<IFxParamDeclaration>();

            if (mc.HasDiffuse)
            {
                effectParameters.Add(new FxParamDeclaration<float4>
                {
                    Name = UniformNameDeclarations.DiffuseColor,
                    Value = mc.Diffuse.Color
                });
                if (mc.Diffuse.Texture != null)
                {
                    effectParameters.Add(new FxParamDeclaration<float>
                    {
                        Name = UniformNameDeclarations.DiffuseMix,
                        Value = mc.Diffuse.Mix
                    });
                    effectParameters.Add(new FxParamDeclaration<Texture>
                    {
                        Name = UniformNameDeclarations.DiffuseTexture,
                        Value = LoadTexture(mc.Diffuse.Texture)
                    });
                }
            }

            if (mc.HasSpecular)
            {
                effectParameters.Add(new FxParamDeclaration<float4>
                {
                    Name = UniformNameDeclarations.SpecularColor,
                    Value = mc.Specular.Color
                });

                if (mc.GetType() == typeof(MaterialComponent))
                {
                    effectParameters.Add(new FxParamDeclaration<float>
                    {
                        Name = UniformNameDeclarations.SpecularShininessName,
                        Value = mc.Specular.Shininess
                    });
                    effectParameters.Add(new FxParamDeclaration<float>
                    {
                        Name = UniformNameDeclarations.SpecularStrength,
                        Value = mc.Specular.Intensity
                    });
                    if (mc.Specular.Texture != null)
                    {
                        effectParameters.Add(new FxParamDeclaration<float>
                        {
                            Name = UniformNameDeclarations.SpecularMixName,
                            Value = mc.Specular.Mix
                        });
                        effectParameters.Add(new FxParamDeclaration<Texture>
                        {
                            Name = UniformNameDeclarations.SpecularTextureName,
                            Value = LoadTexture(mc.Specular.Texture)
                        });
                    }
                }
                else if (mc.GetType() == typeof(MaterialPBRComponent))
                {
                    var mcPbr = (MaterialPBRComponent)mc;

                    var delta = 0.0000001f;
                    var diffuseFractionDelta = 0.99999f; //The value of the diffuse fraction is (incorrectly) the "Metallic" value of the Principled BSDF Material. If it is zero the result here will be by far to bright.

                    var roughness = mcPbr.RoughnessValue + delta; // always float, never int!
                    var fresnel = mcPbr.FresnelReflectance + delta;
                    var df = mcPbr.DiffuseFraction == 0 ? diffuseFractionDelta : mcPbr.DiffuseFraction + delta;

                    effectParameters.Add(new FxParamDeclaration<float>
                    {
                        Name = UniformNameDeclarations.RoughnessValue,
                        Value = roughness
                    });
                    effectParameters.Add(new FxParamDeclaration<float>
                    {
                        Name = UniformNameDeclarations.FresnelReflectance,
                        Value = fresnel
                    });
                    effectParameters.Add(new FxParamDeclaration<float>
                    {
                        Name = UniformNameDeclarations.DiffuseFraction,
                        Value = df
                    });

                }
            }

            if (mc.HasEmissive)
            {
                effectParameters.Add(new FxParamDeclaration<float4>
                {
                    Name = UniformNameDeclarations.EmissiveColorName,
                    Value = mc.Emissive.Color
                });
                if (mc.Emissive.Texture != null)
                {
                    effectParameters.Add(new FxParamDeclaration<float>
                    {
                        Name = UniformNameDeclarations.EmissiveMixName,
                        Value = mc.Emissive.Mix
                    });
                    effectParameters.Add(new FxParamDeclaration<Texture>
                    {
                        Name = UniformNameDeclarations.EmissiveTextureName,
                        Value = LoadTexture(mc.Emissive.Texture)
                    });
                }
            }

            if (mc.HasBump)
            {
                effectParameters.Add(new FxParamDeclaration<float>
                {
                    Name = UniformNameDeclarations.BumpIntensityName,
                    Value = mc.Bump.Intensity
                });
                effectParameters.Add(new FxParamDeclaration<Texture>
                {
                    Name = UniformNameDeclarations.BumpTextureName,
                    Value = LoadTexture(mc.Bump.Texture)
                });
            }

            for (int i = 0; i < LightingShard.NumberOfLightsForward; i++)
            {
                if (!LightingShard.LightPararamStringsAllLights.ContainsKey(i))
                {
                    LightingShard.LightPararamStringsAllLights.Add(i, new LightParamStrings(i));
                }

                effectParameters.Add(new FxParamDeclaration<float3>
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].PositionViewSpace,
                    Value = new float3(0, 0, -1.0f)
                });
                effectParameters.Add(new FxParamDeclaration<float4>
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].Intensities,
                    Value = float4.Zero
                });
                effectParameters.Add(new FxParamDeclaration<float>
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].MaxDistance,
                    Value = 0.0f
                });
                effectParameters.Add(new FxParamDeclaration<float>
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].Strength,
                    Value = 0.0f
                });
                effectParameters.Add(new FxParamDeclaration<float>
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].OuterAngle,
                    Value = 0.0f
                });
                effectParameters.Add(new FxParamDeclaration<float>
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].InnerAngle,
                    Value = 0.0f
                });
                effectParameters.Add(new FxParamDeclaration<float3>
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].Direction,
                    Value = float3.Zero
                });
                effectParameters.Add(new FxParamDeclaration<int>
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].LightType,
                    Value = 1
                });
                effectParameters.Add(new FxParamDeclaration<int>
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].IsActive,
                    Value = 1
                });
                effectParameters.Add(new FxParamDeclaration<int>
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].IsCastingShadows,
                    Value = 0
                });
                effectParameters.Add(new FxParamDeclaration<float>
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].Bias,
                    Value = 0f
                });
            }

            // FUSEE_ PARAMS
            // TODO: Just add the necessary ones!
            effectParameters.Add(new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.Model,
                Value = float4x4.Identity
            });
            effectParameters.Add(new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.ModelView,
                Value = float4x4.Identity
            });
            effectParameters.Add(new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.ModelViewProjection,
                Value = float4x4.Identity
            });
            effectParameters.Add(new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.ITModelView,
                Value = float4x4.Identity
            });

            effectParameters.Add(new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.IModelView,
                Value = float4x4.Identity
            });
            effectParameters.Add(new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.ITView,
                Value = float4x4.Identity
            });
            effectParameters.Add(new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.View,
                Value = float4x4.Identity
            });
            effectParameters.Add(new FxParamDeclaration<float4x4>
            {
                Name = UniformNameDeclarations.Projection,
                Value = float4x4.Identity
            });
            effectParameters.Add(new FxParamDeclaration<float4x4[]>
            {
                Name = UniformNameDeclarations.BonesArray,
                Value = new[] { float4x4.Identity }
            });

            return effectParameters;
        }

        private static Texture LoadTexture(string path)
        {
            var image = AssetStorage.Get<ImageData>(path);
            if (image != null)
                return new Texture(image);

            image = AssetStorage.Get<ImageData>("DefaultTexture.png");
            if (image != null)
                return new Texture(image);

            return new Texture(new ImageData());
        }

        private static ShaderEffect CreateDefaultEffect()
        {
            var defaultMat = new MaterialComponent
            {
                Diffuse = new MatChannelContainer
                {
                    Color = new float4(0.5f, 0.5f, 0.5f, 1.0f)
                },
                Specular = new SpecularChannelContainer
                {
                    Color = new float4(1, 1, 1, 1),
                    Intensity = 0.5f,
                    Shininess = 22
                }
            };

            return FromMatComp(defaultMat);
        }       

    }
}