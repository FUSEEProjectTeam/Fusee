using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Provides a collection of ShaderEffects.
    /// </summary>
    public static class ShaderCodeBuilder
    {
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
                new EffectParameterDeclaration { Name = "ScreenParams", Value = screenParams},
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
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_POSITION.ToString(), Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_POSITION]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_NORMAL.ToString(), Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_NORMAL]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_ALBEDO.ToString(), Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO]},

                new EffectParameterDeclaration { Name = "ScreenParams", Value = screenParams},
                new EffectParameterDeclaration {Name = "SSAOKernel[0]", Value = ssaoKernel},
                new EffectParameterDeclaration {Name = "NoiseTex", Value = ssaoNoiseTex},
                new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity},
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
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, float4 backgroundColor, IWritableTexture shadowMap = null)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            if (lc.IsCastingShadows)
            {
                if (lc.Type != LightType.Point) 
                {
                    effectParams.Add(new EffectParameterDeclaration { Name = "LightSpaceMatrix", Value = new float4x4[] { } });
                    effectParams.Add(new EffectParameterDeclaration { Name = "ShadowMap", Value = (WritableTexture)shadowMap }); 
                }
                else                
                    effectParams.Add(new EffectParameterDeclaration { Name = "ShadowCubeMap", Value = (WritableCubeMap)shadowMap });                
            }

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = AssetStorage.Get<string>("Deferred.vert"),
                    PS = DeferredLightingFS(lc),
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

            effectParams.Add(new EffectParameterDeclaration { Name = "LightSpaceMatrix", Value = new float4x4[] { } });
            effectParams.Add(new EffectParameterDeclaration { Name = "ShadowMaps[0]", Value = shadowMaps });
            effectParams.Add(new EffectParameterDeclaration { Name = "ClipPlanes[0]", Value = clipPlanes });

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = AssetStorage.Get<string>("Deferred.vert"),
                    PS = DeferredLightingFS(lc, true, numberOfCascades),
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
                new EffectParameterDeclaration { Name = "FUSEE_M", Value = float4x4.Identity },
                new EffectParameterDeclaration { Name = "FUSEE_V", Value = float4x4.Identity },
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
                new EffectParameterDeclaration { Name = "FUSEE_M", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "LightSpaceMatrix", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "LightType", Value = 0},
            });
        }

        private static string DeferredLightingFS(LightComponent lc, bool isCascaded = false, int numberOfCascades = 0, bool debugCascades = false)
        {
            // Fragment shader ------------------------------
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version300Es());
            frag.Append("#extension GL_ARB_explicit_uniform_location : enable\n");
            frag.Append(HeaderShard.EsPrecisionHighpFloat());

            frag.Append(FragPropertiesShard.DeferredUniforms());            
            frag.Append(FragPropertiesShard.FuseeMatrixUniforms());

            frag.Append(LightingShard.LightStructDeclaration());
            frag.Append("uniform Light light;");

            if (!isCascaded)
            {
                if (lc.IsCastingShadows)
                {
                    if (lc.Type != LightType.Point)
                        frag.Append($"uniform sampler2D ShadowMap;\n");
                    else
                        frag.Append("uniform samplerCube ShadowCubeMap;\n");
                }

                frag.Append("uniform mat4x4 LightSpaceMatrix;\n");
            }
            else
            {
                frag.Append($"uniform sampler2D[{numberOfCascades}] ShadowMaps;\n");
                frag.Append($"uniform vec2[{numberOfCascades}] ClipPlanes;\n");

                frag.Append($"uniform mat4x4[{numberOfCascades}] LightSpaceMatrices;\n");
            }

            frag.Append("uniform int PassNo;\n");
            frag.Append("uniform int SsaoOn;\n");

            frag.Append("uniform vec4 BackgroundColor;\n");

            frag.Append($"in vec2 {VaryingNameDeclarations.TextureCoordinates};\n");
            frag.Append(FragPropertiesShard.ColorOut());

            //Shadow calculation
            //-------------------------------------- 
            if (lc.Type != LightType.Point)
                frag.Append(LightingShard.ShadowCalculation());
            else
                frag.Append(LightingShard.ShadowCalculationCubeMap());

            frag.Append(@"void main()
            {
            ");

            frag.AppendLine($"vec3 Normal = texture({RenderTargetTextureTypes.G_NORMAL.ToString()}, {VaryingNameDeclarations.TextureCoordinates}).rgb;");
            //Do not do calculations for the background - is there a smarter way (stencil buffer)?
            //---------------------------------------
            frag.Append(@"
            if(Normal.x == 0.0 && Normal.y == 0.0 && Normal.z == 0.0)      
            {
            ");

            frag.AppendLine($"  {FragPropertiesShard.OutColorName} = BackgroundColor;");
            frag.AppendLine(@"  return;
            }
            ");

            frag.AppendLine($"vec4 fragPos = texture({RenderTargetTextureTypes.G_POSITION.ToString()}, {VaryingNameDeclarations.TextureCoordinates});");
            frag.AppendLine($"vec3 diffuseColor = texture({RenderTargetTextureTypes.G_ALBEDO.ToString()}, {VaryingNameDeclarations.TextureCoordinates}).rgb;");            
            frag.AppendLine($"vec3 occlusion = texture({RenderTargetTextureTypes.G_SSAO.ToString()}, {VaryingNameDeclarations.TextureCoordinates}).rgb;");
            frag.AppendLine($"vec3 specularVars = texture({RenderTargetTextureTypes.G_SPECULAR.ToString()}, {VaryingNameDeclarations.TextureCoordinates}).rgb;");

            //Lighting calculation
            //-------------------------
            frag.Append(@"
            // then calculate lighting as usual
            vec3 lighting = vec3(0,0,0);

            if(PassNo == 0)
            {
                vec3 ambient = vec3(0.2 * diffuseColor);

                if(SsaoOn == 1)
                    ambient *= occlusion;

                lighting += ambient;
            }

            vec3 camPos = FUSEE_IV[3].xyz;
            vec3 viewDir = normalize(-fragPos.xyz);
           
            if(light.isActive == 1)
            {
                float shadow = 0.0;

                vec3 lightColor = light.intensities.xyz;
                vec3 lightPosition = light.position;
                vec3 lightDir = normalize(lightPosition - fragPos.xyz);                

                //attenuation
                float attenuation = 1.0;
                switch(light.lightType)
                {
                    //Point
                    case 0:
                    {
                        float distanceToLight = length(lightPosition - fragPos.xyz); 
                        float distance = pow(distanceToLight/light.maxDistance, 2.0);
                        attenuation = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);                        

                        break;
                    }
                    //Parallel
                    case 1:
                        lightDir = -light.direction;
                        break;
                    //Spot
                    case 2:
                    {                           
                        //point component
                        float distanceToLight = length(lightPosition - fragPos.xyz); 
                        float distance = pow(distanceToLight/light.maxDistance, 2.0);
                        attenuation = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);
                        
                        //cone component
                        vec3 coneDir = light.direction;
                        float lightToSurfaceAngleCos = dot(coneDir, -lightDir); 
                        
                        float epsilon = cos(light.innerConeAngle) - cos(light.outerConeAngle);
                        float t = (lightToSurfaceAngleCos - cos(light.outerConeAngle)) / epsilon;
                        attenuation *= clamp(t, 0.0, 1.0);
                        break;
                    }
                    case 3:
                        lightDir = -light.direction;
                        break;
                }
                ");

            if (lc.IsCastingShadows)
            {
                frag.Append(GetShadow(lc, isCascaded, numberOfCascades));
            }

            // diffuse 
            frag.Append(@"                
                vec3 diffuse = max(dot(Normal, lightDir), 0.0) * diffuseColor * lightColor;
                lighting += (1.0 - shadow) * (diffuse * attenuation * light.strength);
             ");

            // specular
            
            frag.Append(@"
                float shininess = specularVars.r * 256.0;
                float specularStrength = specularVars.g;
                vec3 reflectDir = reflect(-lightDir, Normal);  
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
                vec3 specular = specularStrength * spec * lightColor;
                lighting += (1.0 - shadow) * (specular * attenuation * light.strength);
             ");            

            if (isCascaded && debugCascades)
            {
                frag.Append(DebugCascades());
            }

            frag.AppendLine("}");

            frag.AppendLine($"{FragPropertiesShard.OutColorName} = vec4(lighting, 1.0);");

            frag.Append("}");

            return frag.ToString();
        }

        private static string GetShadow(LightComponent lc, bool isCascaded, int numberOfCascades = 0)
        {
            var frag = new StringBuilder();
            if (isCascaded)
            {
                frag.Append(@"
                int thisFragmentsFirstCascade = -1;
                int thisFragmentsSecondCascade = -1;
                float fragDepth = fragPos.z;
                
                ");

                frag.AppendLine($"int numberOfCascades = {numberOfCascades};");
                frag.Append($"for (int i = 0; i < numberOfCascades; i++)\n");
                frag.Append(
                @"{
                    vec2 cp1 = ClipPlanes[i];
                    if(fragDepth < cp1.y)
                    {                        
                        thisFragmentsFirstCascade = i;  
                        if(i + 1 <= numberOfCascades - 1)
                        {
                            vec2 cp2 = ClipPlanes[i+1];
                            if(fragDepth < cp2.y)                                                 
                                thisFragmentsSecondCascade = i+1;
                        }
                        break;
                    }                    
                }
                ");

                frag.Append(@"
                // shadow                
                if (light.isCastingShadows == 1)
                {
                    vec4 posInLightSpace1 = (LightSpaceMatrices[thisFragmentsFirstCascade] * FUSEE_IV) * fragPos;
                    float shadow1 = ShadowCalculation(ShadowMaps[thisFragmentsFirstCascade], posInLightSpace1, Normal, lightDir,  light.bias, 1.0);                   

                    //blend cascades to avoid hard cuts between them
                    if(thisFragmentsSecondCascade != -1)
                    {  
                        float blendStartPercent = max(85.0 - (5.0 * float(thisFragmentsFirstCascade-1)), 50.0); //the farther away the cascade, the earlier we blend the shadow maps        

                        vec4 posInLightSpace2 = (LightSpaceMatrices[thisFragmentsSecondCascade] * FUSEE_IV) * fragPos;
                        float shadow2 = ShadowCalculation(ShadowMaps[thisFragmentsSecondCascade], posInLightSpace2, Normal, lightDir, light.bias, 1.0);    
                        float z = ClipPlanes[thisFragmentsFirstCascade].y - ClipPlanes[thisFragmentsFirstCascade].x;
                        float percent = (100.0/z * (fragDepth - ClipPlanes[thisFragmentsFirstCascade].x));
                        float percentNormalized = (percent - blendStartPercent) / (100.0 - blendStartPercent);

                        if(percent >= blendStartPercent)
                            shadow = mix(shadow1, shadow2, percentNormalized);
                        else
                            shadow = shadow1;
                    }
                    else                    
                        shadow = shadow1;
                }                              
                ");
            }
            else
            {
                if (lc.Type != LightType.Point)
                {
                    frag.Append(@"
                    // shadow                
                    if (light.isCastingShadows == 1)
                    {
                        vec4 posInLightSpace = (LightSpaceMatrix * FUSEE_IV) * fragPos;
                        shadow = ShadowCalculation(ShadowMap, posInLightSpace, Normal, lightDir, light.bias, 1.0);                    
                    }                
                    ");
                }
                else
                {
                    frag.Append(@"
                    // shadow       
                    if (light.isCastingShadows == 1)
                    {
                        shadow = ShadowCalculationCubeMap(ShadowCubeMap, (FUSEE_IV * fragPos).xyz, light.positionWorldSpace, light.maxDistance, Normal, lightDir, light.bias, 2.0);
                    }
                    ");
                }
            }

            return frag.ToString();
        }

        private static string DebugCascades()
        {
            var frag = new StringBuilder();
            frag.Append(@"                     

            vec3 cascadeColor1 = vec3(0.0,0.0,0.0);
            vec3 cascadeColor2 = vec3(0.0,0.0,0.0);
            vec3 cascadeColor = vec3(1.0,1.0,1.0);

            if(thisFragmentsFirstCascade == 0)
                cascadeColor1 = vec3(1,0.3f,0.3f);
            else if(thisFragmentsFirstCascade == 1)
                    cascadeColor1 = vec3(0.3f,1,0.3f);
            else if(thisFragmentsFirstCascade == 2)
                cascadeColor1 = vec3(0.3f,0.3f,1);
            else if(thisFragmentsFirstCascade == 3)
                cascadeColor1 = vec3(1,1,0.3f);
            else if(thisFragmentsFirstCascade == 4)
                cascadeColor1 = vec3(1,0.3,1);
            else if(thisFragmentsFirstCascade == 5)
                cascadeColor1 = vec3(1,0.3f,1);                

            if(thisFragmentsSecondCascade == 0)
                cascadeColor2 = vec3(1,0.3f,0.3f);
            else if(thisFragmentsSecondCascade == 1)
                    cascadeColor2 = vec3(0.3f,1,0.3f);
            else if(thisFragmentsSecondCascade == 2)
                cascadeColor2 = vec3(0.3f,0.3f,1);
            else if(thisFragmentsSecondCascade == 3)
                cascadeColor2 = vec3(1,1,0.3f);
            else if(thisFragmentsSecondCascade == 4)
                cascadeColor2 = vec3(1,0.3,1);
            else if(thisFragmentsSecondCascade == 5)
                cascadeColor2 = vec3(1,0.3f,1);

            if(thisFragmentsSecondCascade != -1)
            {
                float blendStartPercent = max(85.0 - (5.0 * float(thisFragmentsFirstCascade -1)), 50.0); //the farther away the cascade, the earlier we blend the shadow maps   
                float z = ClipPlanes[thisFragmentsFirstCascade].y;
                float percent = (100.0/z * fragDepth);
                float percentNormalized = (percent - blendStartPercent) / (100.0 - blendStartPercent);
                if(percent >= blendStartPercent)
                    cascadeColor = mix(cascadeColor1, cascadeColor2, percentNormalized);
                else
                    cascadeColor = cascadeColor1;
            }
            else
            {
                cascadeColor = cascadeColor1;
            }
            lighting *= cascadeColor;
                      
                ");
            return frag.ToString();
        }

        private static List<EffectParameterDeclaration> DeferredLightingEffectParams(RenderTarget srcRenderTarget, float4 backgroundColor)
        {
            return new List<EffectParameterDeclaration>()
            {
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_POSITION.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_POSITION]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_NORMAL.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_NORMAL]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_ALBEDO.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_SSAO.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_SSAO]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_SPECULAR.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_SPECULAR]},
                new EffectParameterDeclaration { Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_MV", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_IV", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_V", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_ITV", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_P", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "light.position", Value = new float3(0, 0, -1.0f)},
                new EffectParameterDeclaration { Name = "light.positionWorldSpace", Value = new float3(0, 0, -1.0f)},
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
                new EffectParameterDeclaration { Name = "PassNo", Value = 0},
                new EffectParameterDeclaration { Name = "BackgroundColor", Value = backgroundColor},
                new EffectParameterDeclaration { Name = "SsaoOn", Value = 1},
            };
        }

        #endregion

        #region Make ShaderEffect
        /// <summary>
        ///     Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffect MakeShaderEffect(float4 diffuseColor, float4 specularColor, float shininess, float specularIntensity = 0.5f)
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
            return MakeShaderEffectFromMatComp(temp);
        }

        /// <summary>
        ///     Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffectProtoPixel MakeShaderEffectProto(float4 diffuseColor, float4 specularColor, float shininess, float specularIntensity = 0.5f)
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
            return MakeShaderEffectFromMatCompProto(temp);
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
        public static ShaderEffect MakeShaderEffect(float4 diffuseColor, float4 specularColor, float shininess, string texName, float diffuseMix, float specularIntensity = 0.5f)
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
            return MakeShaderEffectFromMatComp(temp);
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
        public static ShaderEffectProtoPixel MakeShaderEffectProto(float4 diffuseColor, float4 specularColor, float shininess, string texName, float diffuseMix, float specularIntensity = 0.5f)
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

            return MakeShaderEffectFromMatCompProto(temp);
        }

        /// <summary> 
        /// Creates a ShaderEffectComponent from a MaterialComponent 
        /// </summary> 
        /// <param name="mc">The MaterialComponent</param> 
        /// <param name="wc">Only pass over a WeightComponent if you use bone animations in the current node (usage: pass currentNode.GetWeights())</param>        
        /// <returns></returns> 
        /// <exception cref="Exception"></exception> 
        public static ShaderEffect MakeShaderEffectFromMatComp(MaterialComponent mc, WeightComponent wc = null)
        {
            var effectProps = ShaderShardUtil.CollectEffectProps(null, mc, wc);
            var vs = CreateVertexShader(wc, effectProps);
            var ps = CreatePixelShader(effectProps);
            var effectParameters = AssembleEffectParamers(mc);

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
        /// <param name="mc">The MaterialComponent</param> 
        /// <param name="wc">Only pass over a WeightComponent if you use bone animations in the current node (usage: pass currentNode.GetWeights())</param>        
        /// <returns></returns> 
        /// <exception cref="Exception"></exception> 
        public static ShaderEffectProtoPixel MakeShaderEffectFromMatCompProto(MaterialComponent mc, WeightComponent wc = null)
        {
            var effectProps = ShaderShardUtil.CollectEffectProps(null, mc, wc);
            string vs = CreateVertexShader(wc, effectProps);
            string ps = CreateProtoPixelShader(effectProps);
            var effectParameters = AssembleEffectParamers(mc);

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
            );
            ret.EffectProps = effectProps;
            return ret;
        }

        private static IEnumerable<EffectParameterDeclaration> AssembleEffectParamers(MaterialComponent mc)
        {
            var effectParameters = new List<EffectParameterDeclaration>();

            if (mc.HasDiffuse)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.DiffuseColorName,
                    Value = mc.Diffuse.Color
                });
                if (mc.Diffuse.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.DiffuseMixName,
                        Value = mc.Diffuse.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.DiffuseTextureName,
                        Value = LoadTexture(mc.Diffuse.Texture)
                    });
                }
            }

            if (mc.HasSpecular)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.SpecularColorName,
                    Value = mc.Specular.Color
                });

                if (mc.GetType() == typeof(MaterialComponent))
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.SpecularShininessName,
                        Value = mc.Specular.Shininess
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.SpecularIntensityName,
                        Value = mc.Specular.Intensity
                    });
                    if (mc.Specular.Texture != null)
                    {
                        effectParameters.Add(new EffectParameterDeclaration
                        {
                            Name = UniformNameDeclarations.SpecularMixName,
                            Value = mc.Specular.Mix
                        });
                        effectParameters.Add(new EffectParameterDeclaration
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

            if (mc.HasEmissive)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.EmissiveColorName,
                    Value = mc.Emissive.Color
                });
                if (mc.Emissive.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.EmissiveMixName,
                        Value = mc.Emissive.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.EmissiveTextureName,
                        Value = LoadTexture(mc.Emissive.Texture)
                    });
                }
            }

            if (mc.HasBump)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.BumpIntensityName,
                    Value = mc.Bump.Intensity
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.BumpTextureName,
                    Value = LoadTexture(mc.Bump.Texture)
                });
            }

            for (int i = 0; i < LightingShard.NumberOfLightsForward; i++)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].position",
                    Value = new float3(0, 0, -1.0f)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].intensities",
                    Value = float4.Zero
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].maxDistance",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].strength",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].outerConeAngle",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].innerConeAngle",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].direction",
                    Value = float3.Zero
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].lightType",
                    Value = 1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].isActive",
                    Value = 1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].isCastingShadows",
                    Value = 0
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].bias",
                    Value = 0f
                });
            }

            // FUSEE_ PARAMS
            // TODO: Just add the necessary ones!
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_M",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_MV",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_MVP",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_ITMV",
                Value = float4x4.Identity
            });

            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_IMV",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_ITV",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_V",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_P",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_BONES",
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

        private static string CreateVertexShader(WeightComponent wc, ShaderEffectProps effectProps)
        {
            var vertexShader = new List<string>
            {
                HeaderShard.Version300Es(),
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
                HeaderShard.Version300Es(),
                HeaderShard.EsPrecisionHighpFloat(),

                LightingShard.LightStructDeclaration(),

                FragPropertiesShard.InParams(effectProps),
                FragPropertiesShard.FuseeMatrixUniforms(),
                FragPropertiesShard.MatPropsUniforms(effectProps),
                FragPropertiesShard.FixedNumberLightArray(),
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
                HeaderShard.Version300Es(),
                HeaderShard.EsPrecisionHighpFloat(),

                FragPropertiesShard.InParams(effectProps),
                FragPropertiesShard.FuseeMatrixUniforms(),
                FragPropertiesShard.MatPropsUniforms(effectProps),
            };

            return string.Join("\n", protoPixelShader);
        }

        #endregion

    }
}