using System;
using System.Collections.Generic;
using System.Globalization;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    /// <summary>
    /// Collection of Shader Shards, describing the struct for a Light and different methods for light and shadow calculation.
    /// </summary>
    public static class LightingShard
    {
        ///The maximal number of lights we can render when using the forward pipeline.
        public const int NumberOfLightsForward = 8;
         
        /// <summary>
        /// Struct, that describes a Light object in the shader code./>
        /// </summary>
        /// <returns></returns>
        public static string LightStructDeclaration()
        {
            var lightStruct = @"
            struct Light 
            {
                vec3 position;
                vec3 positionWorldSpace;
                vec4 intensities;
                vec3 direction;
                vec3 directionWorldSpace;
                float maxDistance;
                float strength;
                float outerConeAngle;
                float innerConeAngle;
                int lightType;
                int isActive;
                int isCastingShadows;
                float bias;
            };           
            ";
            return lightStruct;
        }

        /// <summary>
        /// Collects all lighting methods, dependent on what is defined in the given <see cref="ShaderEffectProps"/> and the LightingCalculationMethod.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>        
        public static string AssembleLightingMethods(ShaderEffectProps effectProps)
        {
            var lighting = new List<string>();

            //Adds methods to the PS that calculate the single light components (ambient, diffuse, specular)
            switch (effectProps.MatType)
            {
                case MaterialType.Standard:                
                    lighting.Add(AmbientLightMethod());
                    if (effectProps.MatProbs.HasDiffuse)
                        lighting.Add(DiffuseLightMethod(effectProps));
                    if (effectProps.MatProbs.HasSpecular)
                        lighting.Add(SpecularLightMethod());
                    break;
                case MaterialType.MaterialPbr:                    
                    lighting.Add(AmbientLightMethod());
                    if (effectProps.MatProbs.HasDiffuse)
                        lighting.Add(DiffuseLightMethod(effectProps));
                    if (effectProps.MatProbs.HasSpecular)
                        lighting.Add(PbrSpecularLightMethod(effectProps));
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Material Type unknown or incorrect: {effectProps.MatType}");
            }

            lighting.Add(ApplyLightMethod(effectProps));


            return string.Join("\n", lighting);
        }

        /// <summary>
        /// Method for calculation the ambient lighting component.
        /// </summary>       
        public static string AmbientLightMethod()
        {
            var methodBody = new List<string>
            {
                "return vec4(DiffuseColor.xyz * ambientCoefficient, 1.0);"
            };

            return (GLSL.CreateMethod(GLSL.Type.Vec4, "ambientLighting",
                new[] { GLSL.CreateVar(GLSL.Type.Float, "ambientCoefficient") }, methodBody));
        }

        /// <summary>
        /// Method for calculation the diffuse lighting component.
        /// </summary>       
        public static string DiffuseLightMethod(ShaderEffectProps effectProps)
        {
            var methodBody = new List<string>
            {
                "float diffuseTerm = dot(N, L);"
            };

            //TODO: Test alpha blending between diffuse and texture
            if (effectProps.MatProbs.HasDiffuseTexture)
                methodBody.Add(
                    $"vec4 blendedCol = mix({UniformNameDeclarations.DiffuseColorName}, texture({UniformNameDeclarations.DiffuseTextureName}, vUV), {UniformNameDeclarations.DiffuseMixName});" +
                    $"return blendedCol * max(diffuseTerm, 0.0) * intensities;");
            else
                methodBody.Add($"return vec4({UniformNameDeclarations.DiffuseColorName}.rgb * intensities.rgb * max(diffuseTerm, 0.0), 1.0);");

            return GLSL.CreateMethod(GLSL.Type.Vec4, "diffuseLighting",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Vec3, "N"), GLSL.CreateVar(GLSL.Type.Vec3, "L"),
                    GLSL.CreateVar(GLSL.Type.Vec4, "intensities")
                }, methodBody);
        }


        /// <summary>
        /// Method for calculation the specular lighting component.
        /// </summary>       
        public static string SpecularLightMethod()
        {
            var methodBody = new List<string>
            {
                "float specularTerm = 0.0;",
                "if(dot(N, L) > 0.0)",
                "{",
                "   // half vector",   
                    "vec3 reflectDir = reflect(-L, N);",
                $"  specularTerm = pow(max(0.0, dot(V, reflectDir)), {UniformNameDeclarations.SpecularShininessName});",
                "}",
                $"return vec4(({UniformNameDeclarations.SpecularColorName}.rgb * {UniformNameDeclarations.SpecularIntensityName} * intensities.rgb) * specularTerm, 1.0);"
            };

            return GLSL.CreateMethod(GLSL.Type.Vec4, "specularLighting",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Vec3, "N"), GLSL.CreateVar(GLSL.Type.Vec3, "L"), GLSL.CreateVar(GLSL.Type.Vec3, "V"),
                    GLSL.CreateVar(GLSL.Type.Vec4, "intensities")
                }, methodBody);

        }

        //TODO: At the moment Blender's Principled BSDF material gets translated into a MaterialPBR and internally uses this lighting method for the specular component.
        //This is not the same lighting method as is used in Blender and will therefor only produce approximately visually correct results.
        /// <summary>
        /// Method for calculation the specular lighting component.
        /// Replaces the standard specular calculation with the Cook-Torrance-Shader
        /// </summary>
        public static string PbrSpecularLightMethod(ShaderEffectProps effectProps)
        {
            var methodBody = new List<string>
            {
                $"float roughnessValue = {UniformNameDeclarations.RoughnessValue}; // 0 : smooth, 1: rough", // roughness 
                $"float F0 = {UniformNameDeclarations.FresnelReflectance}; // fresnel reflectance at normal incidence", // fresnel => Specular from Blender
                $"float k = 1.0-{UniformNameDeclarations.DiffuseFraction};",
                "float NdotL = max(dot(N, L), 0.0);",
                "float specular = 0.0;",
                "float BlinnSpecular = 0.0;",
                "",
                "if(dot(N, L) > 0.0)",
                "{",
                "     // calculate intermediary values",
                "     vec3 H = normalize(L + V);",
                "     float NdotH = max(dot(N, H), 0.0); ",
                "     float NdotV = max(dot(N, L), 0.0); // note: this is NdotL, which is the same value",
                "     float VdotH = max(dot(V, H), 0.0);",
                "     float mSquared = roughnessValue * roughnessValue;",
                "",
                "",
                "",
                "",
                "     // -- geometric attenuation",
                "     //[Schlick's approximation of Smith's shadow equation]",
                "     float k= roughnessValue * sqrt(0.5 * 3.14159265);",
                "     float one_minus_k= 1.0 - k;",
                "     float geoAtt = ( NdotL / (NdotL * one_minus_k + k) ) * ( NdotV / (NdotV * one_minus_k + k) );",
                "",
                "     // -- roughness (or: microfacet distribution function)",
                "     // Trowbridge-Reitz or GGX, GTR2",
                "     float a2 = mSquared * mSquared;",
                "     float d = (NdotH * a2 - NdotH) * NdotH + 1.0;",
                "     float roughness = a2 / (3.14 * d * d);",
                "",
                "     // -- fresnel",
                "     // [Schlick 1994, An Inexpensive BRDF Model for Physically-Based Rendering]",
                "     float fresnel = pow(1.0 - VdotH, 5.0);",
                $"    fresnel = clamp((50.0 * {UniformNameDeclarations.SpecularColorName}.y), 0.0, 1.0) * fresnel + (1.0 - fresnel);",
                "",
                $"     specular = (fresnel * geoAtt * roughness) / (NdotV * NdotL * 3.14);",
                "     ",
                "}",
                "",
                $"return intensities * {UniformNameDeclarations.SpecularColorName} * (k + specular * (1.0-k));"
            };

            return GLSL.CreateMethod(GLSL.Type.Vec4, "specularLighting",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Vec3, "N"), GLSL.CreateVar(GLSL.Type.Vec3, "L"), GLSL.CreateVar(GLSL.Type.Vec3, "V"),
                    GLSL.CreateVar(GLSL.Type.Vec4, "intensities")
                }, methodBody);
        }

        /// <summary>
        /// Wraps all the lighting methods into a single one.
        /// </summary>
        public static string ApplyLightMethod(ShaderEffectProps effectProps)
        {
            /*  var bumpNormals = new List<string>
              {
                  "///////////////// BUMP MAPPING, object space ///////////////////",
                  $"vec3 bumpNormalsDecoded = normalize(texture(BumpTexture, vUV).rgb * 2.0 - 1.0) * (1.0-{BumpIntensityName});",
                  "vec3 N = normalize(vec3(bumpNormalsDecoded.x, bumpNormalsDecoded.y, -bumpNormalsDecoded.z));"
              }; */

            var bumpNormals = new List<string>
            {
                "///////////////// BUMP MAPPING, tangent space ///////////////////",
                $"vec3 N = ((texture(BumpTexture, vUV).rgb * 2.0) - 1.0f) * vec3({UniformNameDeclarations.BumpIntensityName}, {UniformNameDeclarations.BumpIntensityName}, 1.0);",
                "N = (N.x * vec3(vT)) + (N.y * vB) + (N.z * vNormal);",
                "N = normalize(N);"
            };

            var normals = new List<string>
            {
                "vec3 N = normalize(vNormal);"
            };

            var applyLightParamsWithoutNormals = new List<string>
            {
                //"vec3 N = normalize(vNormal);",
                "vec3 L = vec3(0.0, 0.0, 0.0);",
                "if(lightType == 1){L = -normalize(direction);}",
                "else{ L = normalize(position - vPos.xyz);}",
                "vec3 V = normalize(-vPos.xyz);",
                "if(lightType == 3) {",
                "   L = normalize(vec3(0.0,0.0,-1.0));",
                "}",
                "vec2 o_texcoords = vUV;",
                "",
                "vec4 Idif = vec4(0);",
                "vec4 Ispe = vec4(0);",
                ""
            };

            var applyLightParams = new List<string>();
            applyLightParams.AddRange(effectProps.MatProbs.HasBump ? bumpNormals : normals);

            applyLightParams.AddRange(applyLightParamsWithoutNormals);


            if (effectProps.MatProbs.HasDiffuse)
                applyLightParams.Add("Idif = diffuseLighting(N, L, intensities);");


            if (effectProps.MatProbs.HasSpecular)
                applyLightParams.Add("Ispe = specularLighting(N, L, V, intensities);");


            var attenuation = new List<string>
            {
                "float distanceToLight = length(position - vPos.xyz);",
                "float distance = pow(distanceToLight / maxDistance, 2.0);",
                "float att = (clamp(1.0 - pow(distance, 2.0), 0.0, 1.0)) / (pow(distance, 2.0) + 1.0);",
            };

            var pointLight = new List<string>
            {
                "lighting = (Idif * att) + (Ispe * att);",
                "lighting *= strength;"
            };

            //No attenuation!
            var parallelLight = new List<string>
            {
               "lighting = Idif + Ispe;",
                "lighting *= strength;"
            };

            var spotLight = new List<string>
            { 
            //cone component 
            "float lightToSurfaceAngleCos = dot(direction, -L);",

            "float epsilon = cos(innerConeAngle) - cos(outerConeAngle);",
            "float t = (lightToSurfaceAngleCos - cos(outerConeAngle)) / epsilon;",

            "att *= clamp(t, 0.0, 1.0);",
            "",

                "lighting = (Idif * att) + (Ispe * att);",
                "lighting *= strength;"
            };

            // - Disable GammaCorrection for better colors
            /*var gammaCorrection = new List<string>() 
            {
                "vec3 gamma = vec3(1.0/2.2);",
                "result = pow(result, gamma);"
            };*/

            var methodBody = new List<string>();
            methodBody.AddRange(applyLightParams);
            methodBody.Add("vec4 lighting = vec4(0);");
            methodBody.Add("");
            methodBody.AddRange(attenuation);
            methodBody.Add("if(lightType == 0) // PointLight");
            methodBody.Add("{");
            methodBody.AddRange(pointLight);
            methodBody.Add("}");
            methodBody.Add("else if(lightType == 1 || lightType == 3) // ParallelLight or LegacyLight");
            methodBody.Add("{");
            methodBody.AddRange(parallelLight);
            methodBody.Add("}");
            methodBody.Add("else if(lightType == 2) // SpotLight");
            methodBody.Add("{");
            methodBody.AddRange(spotLight);
            methodBody.Add("}");
            methodBody.Add("");
            //methodBody.AddRange(gammaCorrection); // - Disable GammaCorrection for better colors
            methodBody.Add("");

            methodBody.Add("return lighting;");

            return GLSL.CreateMethod(GLSL.Type.Vec4, "ApplyLight",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Vec3, "position"), GLSL.CreateVar(GLSL.Type.Vec4, "intensities"),
                    GLSL.CreateVar(GLSL.Type.Vec3, "direction"), GLSL.CreateVar(GLSL.Type.Float, "maxDistance"),
                    GLSL.CreateVar(GLSL.Type.Float, "strength"), GLSL.CreateVar(GLSL.Type.Float, "outerConeAngle"),
                    GLSL.CreateVar(GLSL.Type.Float, "innerConeAngle"), GLSL.CreateVar(GLSL.Type.Int, "lightType"),
                }, methodBody);
        }

        /// <summary>
        /// Creates the method for calculating whether a fragment is in shadow or not, by using a shadow map (sampler2D).
        /// </summary>
        /// <returns></returns>
        public static string ShadowCalculation()
        {
            var methodBody = new List<string>()
            {
                "float shadow = 0.0;",
                "int pcfLoop = int(pcfKernelHalfSize);",
                "float pcfKernelSize = pcfKernelHalfSize + pcfKernelHalfSize + 1.0;",
                "pcfKernelSize *= pcfKernelSize;",

                "// perform perspective divide",
                "vec4 projCoords = fragPosLightSpace / fragPosLightSpace.w;",
                "projCoords = projCoords * 0.5 + 0.5;",
                "//float closestDepth = texture(shadowMap, projCoords.xy).r;",
                "float currentDepth = projCoords.z;",

                "float thisBias = max(bias * (1.0 - dot(normal, lightDir)), bias / 100.0);",

                "vec2 texelSize = 1.0 / vec2(textureSize(shadowMap, 0));",

                "//use this for using sampler2DShadow (automatic PCF) instead of sampler2D",
                "//float depth = texture(shadowMap, projCoords.xyz).r;",
                "//shadow += (currentDepth - thisBias) > depth ? 1.0 : 0.0;",

                "for (int x = -pcfLoop; x <= pcfLoop; ++x)",
                "{",
                    "for (int y = -pcfLoop; y <= pcfLoop; ++y)",
                    "{",
                        "float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r;",
                        "shadow += (currentDepth - thisBias) > pcfDepth ? 1.0 : 0.0;",
                    "}",
                "}",
                "shadow /= pcfKernelSize;",

                "return shadow;"
            };

            return GLSL.CreateMethod(GLSL.Type.Float, "ShadowCalculation", new[]
            {
                GLSL.CreateVar(GLSL.Type.Sampler2D, "shadowMap"),
                GLSL.CreateVar(GLSL.Type.Vec4, "fragPosLightSpace"),
                GLSL.CreateVar(GLSL.Type.Vec3, "normal"),
                GLSL.CreateVar(GLSL.Type.Vec3, "lightDir"),
                GLSL.CreateVar(GLSL.Type.Float, "bias"),
                GLSL.CreateVar(GLSL.Type.Float, "pcfKernelHalfSize")
            }, methodBody);
        }

        /// <summary>
        /// Creates the method for calculating whether a fragment is in shadow or not, by using a shadow map (sampler2DCube).
        /// The cube map is used when calculating the shadows for a point light.
        /// </summary>
        /// <returns></returns>
        public static string ShadowCalculationCubeMap()
        {
            var methodBody = new List<string>()
            {
                "float pcfKernelSize = pcfKernelHalfSize + pcfKernelHalfSize + 1.0;",
            "pcfKernelSize *= pcfKernelSize;",

            "vec3 sampleOffsetDirections[20] = vec3[]",
            "(",
               "vec3(pcfKernelHalfSize, pcfKernelHalfSize, pcfKernelHalfSize), vec3(pcfKernelHalfSize, -pcfKernelHalfSize, pcfKernelHalfSize), vec3(-pcfKernelHalfSize, -pcfKernelHalfSize, pcfKernelHalfSize), vec3(-pcfKernelHalfSize, pcfKernelHalfSize, pcfKernelHalfSize),",
               "vec3(pcfKernelHalfSize, pcfKernelHalfSize, -pcfKernelHalfSize), vec3(pcfKernelHalfSize, -pcfKernelHalfSize, -pcfKernelHalfSize), vec3(-pcfKernelHalfSize, -pcfKernelHalfSize, -pcfKernelHalfSize), vec3(-pcfKernelHalfSize, pcfKernelHalfSize, -pcfKernelHalfSize),",
               "vec3(pcfKernelHalfSize, pcfKernelHalfSize, 0), vec3(pcfKernelHalfSize, -pcfKernelHalfSize, 0), vec3(-pcfKernelHalfSize, -pcfKernelHalfSize, 0), vec3(-pcfKernelHalfSize, pcfKernelHalfSize, 0),",
               "vec3(pcfKernelHalfSize, 0, pcfKernelHalfSize), vec3(-pcfKernelHalfSize, 0, pcfKernelHalfSize), vec3(pcfKernelHalfSize, 0, -pcfKernelHalfSize), vec3(-pcfKernelHalfSize, 0, -pcfKernelHalfSize),",
               "vec3(0, pcfKernelHalfSize, pcfKernelHalfSize), vec3(0, -pcfKernelHalfSize, pcfKernelHalfSize), vec3(0, -pcfKernelHalfSize, -pcfKernelHalfSize), vec3(0, pcfKernelHalfSize, -pcfKernelHalfSize)",
            ");",

            "// get vector between fragment position and light position",
            "vec3 fragToLight = (fragPos - lightPos) * -1.0;",
            "// now get current linear depth as the length between the fragment and light position",
            "float currentDepth = length(fragToLight);",

            "float shadow = 0.0;",
            "float thisBias = max(bias * (1.0 - dot(normal, lightDir)), bias * 0.01);//0.15;",
            "int samples = 20;",
            "vec3 camPos = FUSEE_IV[3].xyz;",
            "float viewDistance = length(camPos - fragPos);",

            "float diskRadius = 0.5; //(1.0 + (viewDistance / farPlane)) / pcfKernelSize;",
            "for (int i = 0; i < samples; ++i)",
            "{",
                "float closestDepth = texture(shadowMap, fragToLight + sampleOffsetDirections[i] * diskRadius).r;",
                "closestDepth *= farPlane;   // Undo mapping [0;1]",
                "if (currentDepth - thisBias > closestDepth)",
                    "shadow += 1.0;",
            "}",
            "shadow /= float(samples);",
            "return shadow;"
            };
            return GLSL.CreateMethod(GLSL.Type.Float, "ShadowCalculationCubeMap", new[]
            {
                GLSL.CreateVar(GLSL.Type.SamplerCube, "shadowMap"),
                GLSL.CreateVar(GLSL.Type.Vec3, "fragPos"),
                GLSL.CreateVar(GLSL.Type.Vec3, "lightPos"),
                GLSL.CreateVar(GLSL.Type.Float, "farPlane"),
                GLSL.CreateVar(GLSL.Type.Vec3, "normal"),
                GLSL.CreateVar(GLSL.Type.Vec3, "lightDir"),
                GLSL.CreateVar(GLSL.Type.Float, "bias"),
                GLSL.CreateVar(GLSL.Type.Float, "pcfKernelHalfSize")
            }, methodBody);
        }
    }
}
