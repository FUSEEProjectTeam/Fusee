using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Provides a collection of ShaderEffects.
    /// </summary>
    public static class ShaderCodeBuilder
    {
        /// <summary>
        /// The default ShaderEffect, that is used if a <see cref="SceneNodeContainer"/> has a mesh but no <see cref="ShaderEffect"/>.
        /// </summary>
        public static ShaderEffect Default { get; } = CreateDefaultEffect();

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
                new EffectPassDeclaration
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
            new[]
            {
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_ALBEDO.ToString(), Value = srcTex},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.ScreenParams, Value = screenParams},
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
                new EffectPassDeclaration
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
            new[]
            {
                new EffectParameterDeclaration { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_POSITION], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_POSITION]},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_NORMAL], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_NORMAL]},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO], Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO]},

                new EffectParameterDeclaration {Name = UniformNameDeclarations.ScreenParams, Value = screenParams},
                new EffectParameterDeclaration {Name = UniformNameDeclarations.SSAOKernel, Value = ssaoKernel},
                new EffectParameterDeclaration {Name = UniformNameDeclarations.NoiseTex, Value = ssaoNoiseTex},
                new EffectParameterDeclaration {Name = UniformNameDeclarations.Projection, Value = float4x4.Identity},
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
                new EffectPassDeclaration
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
            new[]
            {
                new EffectParameterDeclaration { Name = "InputTex", Value = ssaoRenderTex},

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
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, Light lc, float4 backgroundColor, IWritableTexture shadowMap = null)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            if (lc.IsCastingShadows)
            {
                if (lc.Type != LightType.Point)
                {
                    effectParams.Add(new EffectParameterDeclaration { Name = UniformNameDeclarations.LightSpaceMatrix, Value = float4x4.Identity });
                    effectParams.Add(new EffectParameterDeclaration { Name = UniformNameDeclarations.ShadowMap, Value = (WritableTexture)shadowMap });
                }
                else
                    effectParams.Add(new EffectParameterDeclaration { Name = UniformNameDeclarations.ShadowCubeMap, Value = (WritableCubeMap)shadowMap });
            }

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
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
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, Light lc, WritableTexture[] shadowMaps, float2[] clipPlanes, int numberOfCascades, float4 backgroundColor)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            effectParams.Add(new EffectParameterDeclaration { Name = "LightSpaceMatrices[0]", Value = Array.Empty<float4x4>() });
            effectParams.Add(new EffectParameterDeclaration { Name = "ShadowMaps[0]", Value = shadowMaps });
            effectParams.Add(new EffectParameterDeclaration { Name = "ClipPlanes[0]", Value = clipPlanes });

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
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
            var effectParamDecls = new List<EffectParameterDeclaration>
            {
                new EffectParameterDeclaration { Name = UniformNameDeclarations.Model, Value = float4x4.Identity },
                new EffectParameterDeclaration { Name = UniformNameDeclarations.View, Value = float4x4.Identity },
                new EffectParameterDeclaration { Name = "LightMatClipPlanes", Value = float2.One },
                new EffectParameterDeclaration { Name = "LightPos", Value = float3.One },
                new EffectParameterDeclaration { Name = $"LightSpaceMatrices[0]", Value = lightSpaceMatrices }
            };

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
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
                new EffectPassDeclaration
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
            new[]
            {
                new EffectParameterDeclaration { Name = UniformNameDeclarations.Model, Value = float4x4.Identity},                
                new EffectParameterDeclaration { Name = UniformNameDeclarations.LightSpaceMatrix, Value = float4x4.Identity},                
            });
        }

        private static List<EffectParameterDeclaration> DeferredLightingEffectParams(RenderTarget srcRenderTarget, float4 backgroundColor)
        {
            return new List<EffectParameterDeclaration>()
            {
                new EffectParameterDeclaration { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_POSITION], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_POSITION]},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_NORMAL], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_NORMAL]},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO]},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_SSAO], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_SSAO]},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_SPECULAR], Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_SPECULAR]},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.IView, Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.View, Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.ITView, Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "light.position", Value = new float3(0, 0, -1.0f)},                
                new EffectParameterDeclaration { Name = "light.intensities", Value = float4.Zero},
                new EffectParameterDeclaration { Name = "light.maxDistance", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.strength", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.outerConeAngle", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.innerConeAngle", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.direction", Value = float3.Zero},
                new EffectParameterDeclaration { Name = "light.lightType", Value = 1},
                new EffectParameterDeclaration { Name = "light.isActive", Value = 1},
                new EffectParameterDeclaration { Name = "light.isCastingShadows", Value = 0},
                new EffectParameterDeclaration { Name = "light.bias", Value = 0.0f},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.RenderPassNo, Value = 0},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.BackgroundColor, Value = backgroundColor},
                new EffectParameterDeclaration { Name = UniformNameDeclarations.SsaoOn, Value = 1},
            };
        }

        #endregion

        #region Make ShaderEffect

        //TODO: At the moment the ShaderCodebuilder doesn't get meshes and therefore we always have the default values. Do we need (or want this here)? This would mean we have a relation of the ShaderEffect to the Mesh.....
        private static MeshProps AnalyzeMesh(Mesh mesh, Weight wc = null)
        {
            return new MeshProps
            {
                HasNormals = mesh == null || mesh.Normals != null && mesh.Normals.Length > 0,
                HasUVs = mesh == null || mesh.UVs != null && mesh.UVs.Length > 0,
                HasColors = false,
                HasWeightMap = wc != null,
                HasTangents = mesh == null || (mesh.Tangents != null && mesh.Tangents.Length > 1),
                HasBiTangents = mesh == null || (mesh.BiTangents != null && mesh.BiTangents.Length > 1)
            };
        }

        /// <summary>
        ///     Builds a very simple shader effect with just albedo
        /// </summary>
        /// <param name="albedoColor">The albedo color the resulting effect.</param>
        /// <param name="albedoTexture">The (optional) albedo texture to use.</param>   
        /// <param name="albedoTextureMix">The mix between albedo color and texture.</param>   
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffect MakeShaderEffect(float4 albedoColor, string albedoTexture = "", float albedoTextureMix = 0.5f)
        {
            return MakeShaderEffectFromShaderEffectProps(new ShaderEffectProps
            {
                MatProbs =
                {
                    HasAlbedo = true,
                    HasAlbedoTexture = albedoTexture != string.Empty
                },
                MatType = MaterialType.Standard,
                MatValues =
                {
                    AlbedoColor = albedoColor,
                    AlbedoTexture = albedoTexture != string.Empty ? albedoTexture : null,
                    AlbedoMix = albedoTextureMix
                }
            });
        }

        /// <summary>
        ///     Builds a very simple shader effect with just albedo
        /// </summary>
        /// <param name="albedoColor">The albedo color for the resulting effect.</param>
        /// <param name="albedoTexture">The (optional) albedo texture to use.</param>   
        /// <param name="albedoTextureMix">The mix between albedo color and texture.</param>   
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffectProtoPixel MakeShaderEffectProto(float4 albedoColor, string albedoTexture = "", float albedoTextureMix = 0.5f)
        {
            return MakeShaderEffectFromShaderEffectPropsProto(new ShaderEffectProps
            {
                MatProbs =
                {
                    HasAlbedo = true,
                    HasAlbedoTexture = albedoTexture != string.Empty
                },
                MatType = MaterialType.Standard,
                MatValues =
                {
                    AlbedoColor = albedoColor,
                    AlbedoTexture = albedoTexture != string.Empty ? albedoTexture : null,
                    AlbedoMix = albedoTextureMix
                }
            });
        }

        /// <summary>
        ///     Builds a simple shader effect with albedo and specular color.
        /// </summary>
        /// <param name="albedoColor">The albedo color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffect MakeShaderEffect(float4 albedoColor, float4 specularColor, float shininess, float specularIntensity = 0.5f)
        {
            return MakeShaderEffectFromShaderEffectProps(new ShaderEffectProps
            {
                MatProbs =
                {
                    HasAlbedo = true,
                    HasSpecular = true
                },
                MatType = MaterialType.Standard,
                MatValues =
                {
                    AlbedoColor = albedoColor,
                    SpecularColor = specularColor,
                    SpecularShininess = shininess,
                    SpecularIntensity = specularIntensity
                }
            });
        }

        /// <summary>
        ///     Builds a simple shader effect with albedo and specular color.
        /// </summary>
        /// <param name="albedoColor">The albedo color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffectProtoPixel MakeShaderEffectProto(float4 albedoColor, float4 specularColor, float shininess, float specularIntensity = 0.5f)
        {
            return MakeShaderEffectFromShaderEffectPropsProto(new ShaderEffectProps
            {
                MatProbs =
                {
                    HasAlbedo = true,
                    HasSpecular = true
                },
                MatType = MaterialType.Standard,
                MatValues =
                {
                    AlbedoColor = albedoColor,
                    SpecularColor = specularColor,
                    SpecularShininess = shininess,
                    SpecularIntensity = specularIntensity
                }
            });
        }

        /// <summary>
        ///     Builds a simple shader effect with albedo and specular color.
        /// </summary>
        /// <param name="albedoColor">The albedo color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="albedoTexture">Name of the albedo texture you want to use.</param>
        /// <param name="albedoTextureMix">Determines how much the albedo color and the color from the texture are mixed.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffect MakeShaderEffect(float4 albedoColor, float4 specularColor, float shininess, string albedoTexture, float albedoTextureMix, float specularIntensity = 0.5f)
        {           
            return MakeShaderEffectFromShaderEffectProps(new ShaderEffectProps
            {
                MatProbs =
                {
                    HasAlbedo = true,
                    HasAlbedoTexture = true,
                    HasSpecular = true
                },
                MatType = MaterialType.Standard,
                MatValues =
                {
                    AlbedoColor = albedoColor,
                    AlbedoMix = albedoTextureMix,
                    AlbedoTexture = albedoTexture,
                    SpecularColor = specularColor,
                    SpecularShininess = shininess,
                    SpecularIntensity = specularIntensity
                }
            });
        }

        /// <summary>
        ///     Builds a simple shader effect with albedo and specular color.
        /// </summary>
        /// <param name="albedoColor">The albedo color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="albedoTexture">Name of the albedo texture you want to use.</param>
        /// <param name="albedoTextureMix">Determines how much the albedo color and the color from the texture are mixed.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffectProtoPixel MakeShaderEffectProto(float4 albedoColor, float4 specularColor, float shininess, string albedoTexture, float albedoTextureMix, float specularIntensity = 0.5f)
        {
            return MakeShaderEffectFromShaderEffectPropsProto(new ShaderEffectProps
            {
                MatProbs =
                {
                    HasAlbedo = true,
                    HasAlbedoTexture = true,
                    HasSpecular = true
                },
                MatType = MaterialType.Standard,
                MatValues =
                {
                    AlbedoColor = albedoColor,
                    AlbedoMix = albedoTextureMix,
                    AlbedoTexture = albedoTexture,
                    SpecularColor = specularColor,
                    SpecularShininess = shininess,
                    SpecularIntensity = specularIntensity
                }                
            });
        }

        /// <summary>
        /// Build a complex shader by specifying the <see cref="ShaderEffectProps"/> explicitly.
        /// </summary>
        /// <param name="fxProps">The ShaderEffectProps to use (including values)</param>
        /// <returns>a new ShaderEffectProtoPixel</returns>
        public static ShaderEffectProtoPixel MakeShaderEffectProto(ShaderEffectProps fxProps) => MakeShaderEffectFromShaderEffectPropsProto(fxProps);

        /// <summary>
        /// Build a complex shader by specifying the <see cref="ShaderEffectProps"/> explicitly.
        /// </summary>
        /// <param name="fxProps">The ShaderEffectProps to use (including values)</param>
        /// <returns></returns>
        public static ShaderEffect MakeShaderEffect(ShaderEffectProps fxProps) => MakeShaderEffectFromShaderEffectProps(fxProps);

        /// <summary> 
        /// Creates a ShaderEffectComponent from a MaterialComponent 
        /// </summary> 
        /// <param name="fxProps">The shader effect properties</param> 
        /// <param name="wc">Only pass over a WeightComponent if you use bone animations in the current node (usage: pass currentNode.GetWeights())</param>        
        /// <returns></returns> 
        /// <exception cref="Exception"></exception> 
        public static ShaderEffect MakeShaderEffectFromShaderEffectProps(ShaderEffectProps fxProps, Weight wc = null)
        {
            fxProps.MeshProbs = AnalyzeMesh(null);

            var vs = CreateVertexShader(wc, fxProps);
            var ps = CreatePixelShader(fxProps);
            var effectParameters = AssembleEffectParamers(fxProps);

            if (string.IsNullOrEmpty(vs) || string.IsNullOrEmpty(ps)) throw new Exception("Material could not be evaluated or be built!");

            var ret = new ShaderEffect(new[]
                {
                    new EffectPassDeclaration
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
        /// <param name="fxProps">The shader effect properties</param> 
        /// <param name="wc">Only pass over a WeightComponent if you use bone animations in the current node (usage: pass currentNode.GetWeights())</param>        
        /// <returns></returns> 
        /// <exception cref="Exception"></exception> 
        public static ShaderEffectProtoPixel MakeShaderEffectFromShaderEffectPropsProto(ShaderEffectProps fxProps, Weight wc = null)
        {
            fxProps.MeshProbs = AnalyzeMesh(null);

            string vs = CreateVertexShader(wc, fxProps);
            string ps = CreateProtoPixelShader(fxProps);
            var effectParameters = AssembleEffectParamers(fxProps);

            if (string.IsNullOrEmpty(vs) || string.IsNullOrEmpty(ps)) throw new Exception("Material could not be evaluated or be built!");

            var ret = new ShaderEffectProtoPixel(new[]
                {
                    new EffectPassDeclarationProto
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
                EffectProps = fxProps
            };
            return ret;
        }

        #endregion

        #region Create Shaders from Shards

        private static string CreateVertexShader(Weight wc, ShaderEffectProps effectProps)
        {
            var vertexShader = new List<string>
            {
                HeaderShard.Version300Es,
                HeaderShard.DefineBones(effectProps, wc),
                VertPropertiesShard.FuseeUniforms(effectProps),
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

        private static string CreateDeferredLightingPixelShader(Light lc, bool isCascaded = false, int numberOfCascades = 0, bool debugCascades = false)
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

        private static IEnumerable<EffectParameterDeclaration> AssembleEffectParamers(ShaderEffectProps fxProps)
        {
            var effectParameters = new List<EffectParameterDeclaration>();

            if (fxProps.MatProbs.HasAlbedo)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.AlbedoColor,
                    Value = fxProps.MatValues.AlbedoColor
                });
                if (fxProps.MatValues.AlbedoTexture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.AlbedoMix,
                        Value = fxProps.MatValues.AlbedoMix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.AlbedoTexture,
                        Value = LoadTexture(fxProps.MatValues.AlbedoTexture)
                    });
                }
            }

            if (fxProps.MatProbs.HasSpecular)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.SpecularColor,
                    Value = fxProps.MatValues.SpecularColor
                });

                if (fxProps.MatType == MaterialType.Standard)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.SpecularShininess,
                        Value = fxProps.MatValues.SpecularShininess
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.SpecularIntensity,
                        Value = fxProps.MatValues.SpecularIntensity
                    });
                    if (fxProps.MatValues.SpecularTexture != null)
                    {
                        effectParameters.Add(new EffectParameterDeclaration
                        {
                            Name = UniformNameDeclarations.SpecularMix,
                            Value = fxProps.MatValues.SpecularMix
                        });
                        effectParameters.Add(new EffectParameterDeclaration
                        {
                            Name = UniformNameDeclarations.SpecularTexture,
                            Value = LoadTexture(fxProps.MatValues.SpecularTexture)
                        });
                    }
                }
                else if (fxProps.MatType == MaterialType.MaterialPbr)
                {
                    var mcPbr = fxProps.MatValues;

                    var delta = 0.0000001f;
                    var diffuseFractionDelta = 0.99999f; //The value of the diffuse fraction is (incorrectly) the "Metallic" value of the Principled BSDF Material. If it is zero the result here will be by far to bright.

                    var roughness = mcPbr.RoughnessValue + delta; // always float, never int!
                    var fresnel = mcPbr.FresnelReflectance + delta;
                    var df = mcPbr.DiffuseFraction == 0 ? diffuseFractionDelta : mcPbr.DiffuseFraction + delta;

                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.RoughnessValue,
                        Value = roughness
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.FresnelReflectance,
                        Value = fresnel
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.DiffuseFraction,
                        Value = df
                    });

                }
            }

            if (fxProps.MatProbs.HasEmissive)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.EmissiveColor,
                    Value = fxProps.MatValues.EmissiveColor
                });
                if (fxProps.MatValues.EmissiveTexture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.EmissiveMix,
                        Value = fxProps.MatValues.EmissiveMix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.EmissiveTexture,
                        Value = LoadTexture(fxProps.MatValues.EmissiveTexture)
                    });
                }
            }

            if (fxProps.MatProbs.HasNormalMap)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.NormalMapIntensity,
                    Value = fxProps.MatValues.NormalMapIntensity
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.NormalMap,
                    Value = LoadTexture(fxProps.MatValues.NormalMap)
                });
            }

            for (int i = 0; i < LightingShard.NumberOfLightsForward; i++)
            {
                if (!LightingShard.LightPararamStringsAllLights.ContainsKey(i))
                {
                    LightingShard.LightPararamStringsAllLights.Add(i, new LightParamStrings(i));
                }

                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].PositionViewSpace,
                    Value = new float3(0, 0, -1.0f)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].Intensities,
                    Value = float4.Zero
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].MaxDistance,
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].Strength,
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].OuterAngle,
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].InnerAngle,
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].Direction,
                    Value = float3.Zero
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].LightType,
                    Value = 1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].IsActive,
                    Value = 1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].IsCastingShadows,
                    Value = 0
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = LightingShard.LightPararamStringsAllLights[i].Bias,
                    Value = 0f
                });
            }

            // FUSEE_ PARAMS
            // TODO: Just add the necessary ones!
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = UniformNameDeclarations.Model,
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = UniformNameDeclarations.ModelView,
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = UniformNameDeclarations.ModelViewProjection,
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = UniformNameDeclarations.ITModelView,
                Value = float4x4.Identity
            });

            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = UniformNameDeclarations.IModelView,
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = UniformNameDeclarations.ITView,
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = UniformNameDeclarations.View,
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = UniformNameDeclarations.Projection,
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
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

            Diagnostics.Warn("Texture not found", null, new[] { path });

            return new Texture(new ImageData());
        }

        private static ShaderEffect CreateDefaultEffect()
        {
            return MakeShaderEffectFromShaderEffectProps(new ShaderEffectProps
            {
                MatProbs =
                {
                    HasAlbedo = true,
                    HasSpecular = true
                },
                MatType = MaterialType.Standard,
                MatValues =
                {
                    AlbedoColor = new float4(0.5f, 0.5f, 0.5f, 1.0f),
                    SpecularColor = new float4(1, 1, 1, 1),
                    SpecularShininess = 22,
                    SpecularIntensity = 0.5f
                }
            });
        }       

    }
}