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
    /// Compiler for ShaderCode. Takes a MaterialComponent, evaluates input parameters and creates pixel and vertex shader
    /// </summary>
    public static class ShaderCodeBuilder
    {
        #region Deferred

        /// <summary>
        /// ShaderEffect for rendering into the textures given in a RenderTarget (Geometry Pass).
        /// </summary>       
        /// <param name="diffuseMix">Constant for mixing a single albedo color with a color read from a texture.</param>
        /// <param name="diffuseTex">The texture, containing diffuse colors.</param>
        /// <returns></returns>
        [Obsolete("Not needed due to proto pixel shader!")]
        public static ShaderEffect GBufferTextureEffect(float diffuseMix, Texture diffuseTex = null)
        {
            //------------ vertex shader ------------------//
            var vert = new StringBuilder();

            vert.Append(HeaderShard.Version300Es());
            vert.Append(HeaderShard.EsPrecisionHighpFloat());

            vert.Append(@"
                uniform mat4 FUSEE_M;
                uniform mat4 FUSEE_MV;
                uniform mat4 FUSEE_MVP;
                uniform mat4 FUSEE_ITM;
                uniform mat4 FUSEE_ITMV;               

                in vec3 fuVertex;
                in vec3 fuNormal;
                in vec4 fuColor;
                in vec2 fuUV;
                
                out vec4 vPos;
                out vec3 vNormal;
                out vec4 vColor;    
                out vec2 vUv;

                ");

            vert.Append(@"
                void main() 
                {
                    vPos = FUSEE_MV * vec4(fuVertex.xyz, 1.0);
                    vNormal = (FUSEE_ITMV * vec4(fuNormal, 0.0)).xyz;                   
                    vUv = fuUV;
                    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

                }");

            //--------- Fragment shader ----------- //
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version300Es());
            frag.Append(HeaderShard.EsPrecisionHighpFloat());

            frag.Append(FragPropertiesShard.GBufferOut());

            frag.Append(@"
                in vec4 vPos;
                in vec3 vNormal;
                in vec4 vColor;
                in vec2 vUv;"
            );

            if (diffuseTex != null)
            {
                frag.Append(@"
                uniform vec4 DiffuseColor;
                uniform sampler2D DiffuseTexture;
                uniform float DiffuseMix;"
                );
            }

            frag.AppendLine("void main() {");

            for (int i = 0; i < UniformNameDeclarations.DeferredRenderTextures.Count; i++)
            {
                var tex = UniformNameDeclarations.DeferredRenderTextures[i];
                if (tex == null) continue;

                switch (i)
                {
                    case 0: //POSITION
                        frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(vPos.xyz, vPos.w);");
                        break;
                    case 1: //ALBEDO_SPECULAR
                        if (diffuseTex != null)
                            frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(mix(DiffuseColor.xyz, texture(DiffuseTexture, vUv).xyz, DiffuseMix), DiffuseColor.a);");
                        else
                            frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = DiffuseColor;");
                        break;
                    case 2: //NORMAL
                        frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(normalize(vNormal.xyz), 1.0);");
                        break;
                    case 3: //DEPTH
                        frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);");
                        break;
                }
            }
            frag.Append("}");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_ITM", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "DiffuseColor", Value = new float4(1.0f, 0, 1.0f, 1.0f)},
                new EffectParameterDeclaration {Name = "DiffuseTexture", Value = diffuseTex},
                new EffectParameterDeclaration {Name = "DiffuseMix", Value = diffuseMix},
            });

        }

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
            
            //--------- Fragment shader ----------- //
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version300Es());
            frag.Append(HeaderShard.EsPrecisionHighpFloat());
            frag.Append($"#define SSAO_INPUT_TEX {Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)}\n");
            frag.Append($"#define KERNEL_SIZE {blurKernelSize.ToString("0.0", CultureInfo.InvariantCulture)}\n");
            frag.Append($"#define KERNEL_SIZE_HALF {blurKernelSize * 0.5}\n");

            frag.Append($"in vec2 vTexCoords;\n");

            frag.Append($"uniform sampler2D SSAO_INPUT_TEX;\n");


            frag.Append($"layout (location = 0) out vec4 o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)};\n");

            frag.Append("void main() {");

            frag.Append(@"
            vec2 texelSize = 1.0 / vec2(textureSize(SSAO_INPUT_TEX, 0));
            float result = 0.0;
            for (int x = -KERNEL_SIZE_HALF; x < KERNEL_SIZE_HALF; ++x) 
            {
                for (int y = -KERNEL_SIZE_HALF; y < KERNEL_SIZE_HALF; ++y) 
                {
                    vec2 offset = vec2(float(x), float(y)) * texelSize;
                    result += texture(SSAO_INPUT_TEX, vTexCoords + offset).r;
                }
            }

            result = result / (KERNEL_SIZE * KERNEL_SIZE);
            
            ");

            frag.Append($"o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)} = vec4(result, result, result, 1.0);");

            frag.Append("}");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = AssetStorage.Get<string>("Deferred.vert"),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_SSAO.ToString(), Value = ssaoRenderTex},

            });

        }       
        
        private static string DeferredLightingFS(LightComponent lc)
        {
            // Fragment shader ------------------------------
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version300Es());
            frag.Append("#extension GL_ARB_explicit_uniform_location : enable\n");
            frag.Append(HeaderShard.EsPrecisionHighpFloat());

            frag.Append(FragPropertiesShard.DeferredUniforms());

            frag.Append(LightingShard.LightStructDeclaration());
            frag.Append("uniform Light light;");

            frag.Append("uniform mat4 FUSEE_IV;\n");
            frag.Append("uniform mat4 FUSEE_V;\n");
            frag.Append("uniform mat4 FUSEE_MV;\n");
            frag.Append("uniform mat4 FUSEE_ITV;\n");

            if (lc.IsCastingShadows)
            {
                if (lc.Type != LightType.Point)
                    frag.Append($"uniform sampler2D ShadowMap;\n");
                else
                    frag.Append("uniform samplerCube ShadowCubeMap;\n");
            }

            frag.Append("uniform mat4x4 LightSpaceMatrix;\n");
            frag.Append("uniform int PassNo;\n");
            frag.Append("uniform int SsaoOn;\n");

            frag.Append("uniform vec4 BackgroundColor;\n");

            frag.Append($"in vec2 vTexCoords;\n");
            frag.Append($"layout (location = {0}) out vec4 o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO)};\n");

            //Shadow calculation
            //-------------------------------------- 
            if (lc.Type != LightType.Point)
                frag.Append(LightingShard.ShadowCalculation());
            else
                frag.Append(LightingShard.ShadowCalculationCubeMap());

            frag.Append(@"void main()
            {
            ");

            frag.AppendLine($"vec3 Normal = texture({RenderTargetTextureTypes.G_NORMAL.ToString()}, vTexCoords).rgb;");
            //Do not do calculations for the background - is there a smarter way (stencil buffer)?
            //---------------------------------------
            frag.Append(@"
            if(Normal.x == 0.0 && Normal.y == 0.0 && Normal.z == 0.0)      
            {
            ");

            frag.AppendLine($"  o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO)} = BackgroundColor;");
            frag.AppendLine(@"  return;
            }
            ");

            frag.AppendLine($"vec4 FragPos = texture({RenderTargetTextureTypes.G_POSITION.ToString()}, vTexCoords);");
            frag.AppendLine($"vec3 DiffuseColor = texture({RenderTargetTextureTypes.G_ALBEDO.ToString()}, vTexCoords).rgb;");
            frag.AppendLine($"float SpecularStrength = texture({RenderTargetTextureTypes.G_ALBEDO.ToString()}, vTexCoords).a;");
            frag.AppendLine($"vec3 Occlusion = texture({RenderTargetTextureTypes.G_SSAO.ToString()}, vTexCoords).rgb;");

            //Lighting calculation
            //-------------------------
            frag.Append(@"
            // then calculate lighting as usual
            vec3 lighting = vec3(0,0,0);

            if(PassNo == 0)
            {
                vec3 ambient = vec3(0.2 * DiffuseColor);

                if(SsaoOn == 1)
                    ambient *= Occlusion;

                lighting += ambient;
            }

            vec3 camPos = FUSEE_IV[3].xyz;
            vec3 viewDir = normalize(-FragPos.xyz);

           
            if(light.isActive == 1)
            {
                float shadow = 0.0;

                vec3 lightColor = light.intensities.xyz;
                vec3 lightPosition = light.position;
                vec3 lightDir = normalize(lightPosition - FragPos.xyz);                

                //attenuation
                float attenuation = 1.0;
                switch(light.lightType)
                {
                    //Point
                    case 0:
                    {
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
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
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
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
                if (lc.Type != LightType.Point)
                {
                    frag.Append(@"
                    // shadow                
                    if (light.isCastingShadows == 1)
                    {
                        vec4 posInLightSpace = (LightSpaceMatrix * FUSEE_IV) * FragPos;
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
                        shadow = ShadowCalculationCubeMap(ShadowCubeMap, (FUSEE_IV * FragPos).xyz, light.positionWorldSpace, light.maxDistance, Normal, lightDir, light.bias, 2.0);
                    }
                    ");
                }
            }

            frag.Append(@"
                // diffuse 
                vec3 diffuse = max(dot(Normal, lightDir), 0.0) * DiffuseColor * lightColor;
                lighting += (1.0 - shadow) * (diffuse * attenuation * light.strength);
            
                // specular
                vec3 reflectDir = reflect(-lightDir, Normal);  
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), 100.0);
                vec3 specular = SpecularStrength * spec * lightColor;
                lighting += (1.0 - shadow) * (specular * attenuation * light.strength);
            }              
            ");

            frag.AppendLine($"o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO)} = vec4(lighting, 1.0);");

            frag.Append("}");

            return frag.ToString();
        }

        private static string DeferredLightingFSCascaded(LightComponent lc, int numberOfCascades)
        {
            // Fragment shader ------------------------------
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version300Es());
            frag.Append("#extension GL_ARB_explicit_uniform_location : enable\n");
            frag.Append(HeaderShard.EsPrecisionHighpFloat());

            for (int i = 0; i < Enum.GetNames(typeof(RenderTargetTextureTypes)).Length; i++)
            {
                frag.Append($"uniform sampler2D {Enum.GetName(typeof(RenderTargetTextureTypes), i)};\n");
            }

            frag.Append(LightingShard.LightStructDeclaration());
            frag.Append("uniform Light light;");

            frag.Append("uniform mat4 FUSEE_IV;\n");
            frag.Append("uniform mat4 FUSEE_V;\n");
            frag.Append("uniform mat4 FUSEE_MV;\n");
            frag.Append("uniform mat4 FUSEE_ITV;\n");
           
            frag.Append($"uniform sampler2D[{numberOfCascades}] ShadowMaps;\n");
            frag.Append($"uniform vec2[{numberOfCascades}] ClipPlanes;\n");            

            frag.Append($"uniform mat4x4[{numberOfCascades}] LightSpaceMatrices;\n");
            frag.Append("uniform int PassNo;\n");
            frag.Append("uniform int SsaoOn;\n");

            frag.Append("uniform vec4 BackgroundColor;\n");

            frag.Append($"in vec2 vTexCoords;\n");
            frag.Append($"layout (location = {0}) out vec4 o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO)};\n");

            //Shadow calculation
            //-------------------------------------- 
            if (lc.Type != LightType.Point)
                frag.Append(LightingShard.ShadowCalculation());
            else
                frag.Append(LightingShard.ShadowCalculationCubeMap());

            frag.Append(@"void main()
            {
            ");

            frag.AppendLine($"vec3 Normal = texture({RenderTargetTextureTypes.G_NORMAL.ToString()}, vTexCoords).rgb;");
            //Do not do calculations for the background - is there a smarter way (stencil buffer)?
            //---------------------------------------
            frag.Append(@"
            if(Normal.x == 0.0 && Normal.y == 0.0 && Normal.z == 0.0)      
            {
            ");

            frag.AppendLine($"  o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO)} = BackgroundColor;");
            frag.AppendLine(@"  return;
            }
            ");

            frag.AppendLine($"vec4 FragPos = texture({RenderTargetTextureTypes.G_POSITION.ToString()}, vTexCoords);");
            frag.AppendLine($"vec3 DiffuseColor = texture({RenderTargetTextureTypes.G_ALBEDO.ToString()}, vTexCoords).rgb;");
            frag.AppendLine($"float SpecularStrength = texture({RenderTargetTextureTypes.G_ALBEDO.ToString()}, vTexCoords).a;");
            frag.AppendLine($"vec3 Occlusion = texture({RenderTargetTextureTypes.G_SSAO.ToString()}, vTexCoords).rgb;");

            //Lighting calculation
            //-------------------------
            frag.Append(@"
            // then calculate lighting as usual
            vec3 lighting = vec3(0,0,0);

            if(PassNo == 0)
            {
                vec3 ambient = vec3(0.2 * DiffuseColor);

                if(SsaoOn == 1)
                    ambient *= Occlusion;

                lighting += ambient;
            }

            vec3 camPos = FUSEE_IV[3].xyz;
            vec3 viewDir = normalize(-FragPos.xyz);

           
            if(light.isActive == 1)
            {
                float shadow = 0.0;

                vec3 lightColor = light.intensities.xyz;
                vec3 lightPosition = light.position;
                vec3 lightDir = normalize(lightPosition - FragPos.xyz);                

                //attenuation
                float attenuation = 1.0;
                switch(light.lightType)
                {
                    //Point
                    case 0:
                    {
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
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
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
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
                //TODO: iterate clip planes and choose shadow map for this frag. Use this shadow map in ShadowCalculation()  

                frag.Append(@"
                int thisFragmentsFirstCascade = -1;
                int thisFragmentsSecondCascade = -1;
                float fragDepth = FragPos.z;
                
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
                    vec4 posInLightSpace1 = (LightSpaceMatrices[thisFragmentsFirstCascade] * FUSEE_IV) * FragPos;
                    float shadow1 = ShadowCalculation(ShadowMaps[thisFragmentsFirstCascade], posInLightSpace1, Normal, lightDir,  light.bias, 1.0);                   

                    //blend cascades to avoid hard cuts between them
                    if(thisFragmentsSecondCascade != -1)
                    {  
                        float blendStartPercent = max(85.0 - (5.0 * float(thisFragmentsFirstCascade-1)), 50.0); //the farther away the cascade, the earlier we blend the shadow maps        

                        vec4 posInLightSpace2 = (LightSpaceMatrices[thisFragmentsSecondCascade] * FUSEE_IV) * FragPos;
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

            frag.Append(@"
                // diffuse 
                vec3 diffuse = max(dot(Normal, lightDir), 0.0) * DiffuseColor * lightColor;
                lighting += (1.0 - shadow) * (diffuse * attenuation * light.strength);
            
                // specular
                vec3 reflectDir = reflect(-lightDir, Normal);  
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), 100.0);
                vec3 specular = SpecularStrength * spec * lightColor;
                lighting += (1.0 - shadow) * (specular * attenuation * light.strength);


                //vec3 cascadeColor1 = vec3(0.0,0.0,0.0);
                //vec3 cascadeColor2 = vec3(0.0,0.0,0.0);
                //vec3 cascadeColor = vec3(1.0,1.0,1.0);
                
                //if(thisFragmentsFirstCascade == 0)
                //    cascadeColor1 = vec3(1,0.3f,0.3f);
                //else if(thisFragmentsFirstCascade == 1)
                //     cascadeColor1 = vec3(0.3f,1,0.3f);
                //else if(thisFragmentsFirstCascade == 2)
                //    cascadeColor1 = vec3(0.3f,0.3f,1);
                //else if(thisFragmentsFirstCascade == 3)
                //    cascadeColor1 = vec3(1,1,0.3f);
                //else if(thisFragmentsFirstCascade == 4)
                //    cascadeColor1 = vec3(1,0.3,1);
                //else if(thisFragmentsFirstCascade == 5)
                //    cascadeColor1 = vec3(1,0.3f,1);                
                
                //if(thisFragmentsSecondCascade == 0)
                //    cascadeColor2 = vec3(1,0.3f,0.3f);
                //else if(thisFragmentsSecondCascade == 1)
                //     cascadeColor2 = vec3(0.3f,1,0.3f);
                //else if(thisFragmentsSecondCascade == 2)
                //    cascadeColor2 = vec3(0.3f,0.3f,1);
                //else if(thisFragmentsSecondCascade == 3)
                //    cascadeColor2 = vec3(1,1,0.3f);
                //else if(thisFragmentsSecondCascade == 4)
                //    cascadeColor2 = vec3(1,0.3,1);
                //else if(thisFragmentsSecondCascade == 5)
                //    cascadeColor2 = vec3(1,0.3f,1);

                //if(thisFragmentsSecondCascade != -1)
                //{
                //    float blendStartPercent = max(85.0 - (5.0 * float(thisFragmentsFirstCascade -1)), 50.0); //the farther away the cascade, the earlier we blend the shadow maps   
                //    float z = ClipPlanes[thisFragmentsFirstCascade].y;
                //    float percent = (100.0/z * fragDepth);
                //    float percentNormalized = (percent - blendStartPercent) / (100.0 - blendStartPercent);
                //    if(percent >= blendStartPercent)
                //        cascadeColor = mix(cascadeColor1, cascadeColor2, percentNormalized);
                //    else
                //        cascadeColor = cascadeColor1;
                //}
                //else
                //{
                //    cascadeColor = cascadeColor1;
                //}
                //lighting *= cascadeColor;
            }              
            ");

            frag.AppendLine($"o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO)} = vec4(lighting, 1.0);");

            frag.Append("}");

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

        /// <summary>
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMap">The shadow map.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>            
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, WritableTexture shadowMap, float4 backgroundColor)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            effectParams.Add(new EffectParameterDeclaration { Name = "LightSpaceMatrix", Value = new float4x4[] { } });
            effectParams.Add(new EffectParameterDeclaration { Name = "ShadowMap", Value = shadowMap });

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
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass. Shadow is calculated with cascaded shadow maps.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMaps">The cascaded shadow maps.</param>
        /// <param name="clipPlanes">The clip planes of the frustums. Each frustum is associated with one shadow map.</param>
        /// <param name="numberOfCascades">The number of sub-frustums, used for cascaded shadow mapping.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>            
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, WritableTexture[] shadowMaps, float2[] clipPlanes, int numberOfCascades,float4 backgroundColor)
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
                    PS = DeferredLightingFSCascaded(lc, numberOfCascades),
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
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMap">The shadow map.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>       
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, WritableCubeMap shadowMap, float4 backgroundColor)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            effectParams.Add(new EffectParameterDeclaration { Name = "ShadowCubeMap", Value = shadowMap });

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
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>  
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>       
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, float4 backgroundColor)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

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
            string vs = "";
            string ps = "";

            var effectProps = ShaderShardUtil.CollectEffectProps(null, mc, wc);
            
            if (mc.GetType() == typeof(MaterialPBRComponent))
            {
                if (mc is MaterialPBRComponent) 
                {
                    vs = CreateVertexShader(wc, effectProps);
                    ps = CreatePixelShader(effectProps);
                }
            }
            else
            {
                vs = CreateVertexShader(wc, effectProps);
                ps = CreatePixelShader(effectProps);
            }

            var effectParameters = AssembleEffectParamers(mc);

            if (vs == string.Empty || ps == string.Empty) throw new Exception("Material could not be evaluated or be built!");

            var ret = new ShaderEffect(new[]
                {
                    new EffectPassDeclaration
                    {
                        VS = vs, 
                        //VS = VsBones, 
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
            string vs = "";
            string ps = "";

            var effectProps = ShaderShardUtil.CollectEffectProps(null, mc, wc);
            
            if (mc.GetType() == typeof(MaterialPBRComponent))
            {
                if (mc is MaterialPBRComponent)
                {
                    vs = CreateVertexShader(wc, effectProps);
                    ps = CreateProtoPixelShader(effectProps);
                }
            }
            else
            {
                vs = CreateVertexShader(wc, effectProps);
                ps = CreateProtoPixelShader(effectProps);
            }

            var effectParameters = AssembleEffectParamers(mc);

            if (vs == string.Empty || ps == string.Empty) throw new Exception("Material could not be evaluated or be built!");

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
                if (mc.GetType() == typeof(MaterialComponent))
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.SpecularColorName,
                        Value = mc.Specular.Color
                    });
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
                else if(mc.GetType() == typeof(MaterialPBRComponent))
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
                FragPropertiesShard.FuseeUniforms(effectProps),
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
                FragPropertiesShard.FuseeUniforms(effectProps),
                FragPropertiesShard.MatPropsUniforms(effectProps),                
            };
            
            return string.Join("\n", protoPixelShader);
        }

        #endregion

    }
}