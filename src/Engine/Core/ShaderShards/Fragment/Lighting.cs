using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    /// <summary>
    /// Collection of shader code strings, describing the struct for a Light and different methods for light and shadow calculation.
    /// </summary>
    public static class Lighting
    {
        ///The maximal number of lights we can render when using the forward pipeline.
        public const int NumberOfLightsForward = 8;

        /// <summary>
        /// Struct, that describes a Light object in the shader code./>
        /// </summary>
        /// <returns></returns>
        public static string LightStructDeclaration = string.Join("\n", new List<string>() {

        "struct Light",
        "{",
        "   vec3 position;",
        "   vec4 intensities;",
        "   vec3 direction;",
        "   float maxDistance;",
        "   float strength;",
        "   float outerConeAngle;",
        "   float innerConeAngle;",
        "   int lightType;",
        "   int isActive;",
        "   int isCastingShadows;",
        "   float bias;",
        "};\n",
        });

        /// <summary>
        /// Caches "allLight[i]." names (used as uniform parameters).
        /// </summary>
        internal static Dictionary<int, LightParamStrings> LightPararamStringsAllLights = new Dictionary<int, LightParamStrings>();

        /// <summary>
        /// Collects all lighting methods, dependent on what is defined in the given <see cref="EffectProps"/> and the LightingCalculationMethod.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>
        public static string AssembleLightingMethods(EffectProps effectProps)
        {
            var lighting = new List<string>
            {
                AttenuationPointComponent(),
                AttenuationConeComponent()
            };

            if (effectProps.LightingProps.DoDiffuseLighting)
                lighting.Add(DiffuseComponent());

            //Adds methods to the PS that calculate the single light components (ambient, diffuse, specular)
            switch (effectProps.LightingProps.SpecularLighting)
            {
                case SpecularLighting.Std:
                    lighting.Add(SpecularComponent());
                    break;
                case SpecularLighting.Pbr:
                    lighting.Add(PbrSpecularComponent());
                    break;
                case SpecularLighting.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Material Type unknown or incorrect: {effectProps.LightingProps.SpecularLighting}");
            }

            lighting.Add(ApplyLightForward(effectProps.LightingProps));

            return string.Join("\n", lighting);
        }

        /// <summary>
        /// Collects all lighting methods, dependent on what is defined in the given <see cref="EffectProps"/> and the LightingCalculationMethod.
        /// </summary>
        /// <param name="setup">The <see cref="LightingSetup"/> which is used to decide which lighting methods we need.</param>
        public static string AssembleLightingMethods(LightingSetup setup)
        {
            if (setup == LightingSetup.Unlit)
                return string.Empty;

            var lighting = new List<string>();

            //Adds methods to the PS that calculate the single light components (ambient, diffuse, specular)
            switch (setup)
            {
                case LightingSetup.SpecularStd:
                    lighting.Add(AttenuationPointComponent());
                    lighting.Add(AttenuationConeComponent());
                    lighting.Add(DiffuseComponent());
                    lighting.Add(SpecularComponent());
                    break;
                case LightingSetup.SpecularPbr:
                    lighting.Add(AttenuationPointComponent());
                    lighting.Add(AttenuationConeComponent());
                    lighting.Add(DiffuseComponent());
                    lighting.Add(PbrSpecularComponent());
                    break;
                case LightingSetup.Unlit:
                    break;
                case LightingSetup.DiffuseOnly:
                    lighting.Add(AttenuationPointComponent());
                    lighting.Add(AttenuationConeComponent());
                    lighting.Add(DiffuseComponent());
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Lighting setup unknown or incorrect: {setup}");
            }

            lighting.Add(ApplyLightForward(setup));

            return string.Join("\n", lighting);
        }

        /// <summary>
        /// Method for calculation the diffuse lighting component.
        /// </summary>
        public static string DiffuseComponent()
        {
            var methodBody = new List<string>
            {
                "return max(dot(N, L), 0.0);"
            };
            return GLSL.CreateMethod(GLSL.Type.Float, "diffuseLighting",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Vec3, "N"), GLSL.CreateVar(GLSL.Type.Vec3, "L")
                }, methodBody);
        }

        /// <summary>
        /// Method for calculation the specular lighting component.
        /// </summary>
        public static string SpecularComponent()
        {
            var methodBody = new List<string>
            {
                "float specularTerm = 0.0;",
                "if(dot(N, L) > 0.0)",
                "{",
                    "// half vector",
                    "vec3 reflectDir = reflect(-L, N);",
                    $"specularTerm = pow(max(dot(V, reflectDir), 0.0), shininess);",
                "}",
                "return specularTerm;"
            };

            return GLSL.CreateMethod(GLSL.Type.Float, "specularLighting",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Vec3, "N"), GLSL.CreateVar(GLSL.Type.Vec3, "L"), GLSL.CreateVar(GLSL.Type.Vec3, "V"),
                    GLSL.CreateVar(GLSL.Type.Float, "shininess")
                }, methodBody);

        }

        //TODO: At the moment Blender's Principled BSDF material gets translated into a MaterialPBR and internally uses this lighting method for the specular component.
        //This is not the same lighting method as is used in Blender and will therefor only produce approximately visually correct results.
        /// <summary>
        /// Method for calculation the specular lighting component.
        /// Replaces the standard specular calculation with the Cook-Torrance-Shader
        /// </summary>
        public static string PbrSpecularComponent()
        {
            var methodBody = new List<string>
            {
                "float NdotL = max(dot(N, L), 0.0);",
                "float specular = 0.0;",
                "float BlinnSpecular = 0.0;",
                "",
                "if(dot(N, L) > 0.0)",
                "{",
                    "// calculate intermediary values",
                    "vec3 H = normalize(L + V);",
                    "float NdotH = max(dot(N, H), 0.0); ",
                    "float NdotV = max(dot(N, L), 0.0); // note: this is NdotL, which is the same value",
                    "float VdotH = max(dot(V, H), 0.0);",
                    "float mSquared = roughnessValue * roughnessValue;",
                    "",
                    "// -- geometric attenuation",
                    "//[Schlick's approximation of Smith's shadow equation]",
                    "float k= roughnessValue * sqrt(0.5 * 3.14159265);",
                    "float one_minus_k= 1.0 - k;",
                    "float geoAtt = ( NdotL / (NdotL * one_minus_k + k) ) * ( NdotV / (NdotV * one_minus_k + k) );",
                    "",
                    "// -- roughness (or: microfacet distribution function)",
                    "// Trowbridge-Reitz or GGX, GTR2",
                        "float a2 = mSquared * mSquared;",
                    "float d = (NdotH * a2 - NdotH) * NdotH + 1.0;",
                    "float roughness = a2 / (3.14 * d * d);",
                    "",
                    "// -- fresnel",
                    "// [Schlick 1994, An Inexpensive BRDF Model for Physically-Based Rendering]",
                    "float fresnel = pow(1.0 - VdotH, 5.0);",
                    $"fresnel = clamp((50.0 * specularCol.y), 0.0, 1.0) * fresnel + (1.0 - fresnel);",
                    "",
                    $"specular = (fresnel * geoAtt * roughness) / (NdotV * NdotL * 3.14);",
                    "",
                "}",
                "",
                $"return (k + specular * (1.0-k));"
            };

            return GLSL.CreateMethod(GLSL.Type.Float, "specularLighting",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Vec3, "N"), GLSL.CreateVar(GLSL.Type.Vec3, "L"), GLSL.CreateVar(GLSL.Type.Vec3, "V"),
                    GLSL.CreateVar(GLSL.Type.Float, "k"), GLSL.CreateVar(GLSL.Type.Vec4, "specularCol"), GLSL.CreateVar(GLSL.Type.Float, "F0"),
                    GLSL.CreateVar(GLSL.Type.Float, "roughnessValue")
                }, methodBody);
        }

        /// <summary>
        /// Calculates the attenuation of a point light.
        /// </summary>
        /// <returns></returns>
        public static string AttenuationPointComponent()
        {
            var methodBody = new List<string>
            {
                $"float distanceToLight = length(lightPos - fragPos);",
                "float distance = pow(distanceToLight / lightMaxDistance, 2.0);",
                "return (clamp(1.0 - pow(distance, 2.0), 0.0, 1.0)) / (pow(distance, 2.0) + 1.0);",
            };

            return (GLSL.CreateMethod(GLSL.Type.Float, "attenuationPointComponent",
                new[] { GLSL.CreateVar(GLSL.Type.Vec3, "fragPos"), GLSL.CreateVar(GLSL.Type.Vec3, "lightPos"), GLSL.CreateVar(GLSL.Type.Float, "lightMaxDistance") }, methodBody));
        }

        /// <summary>
        /// Calculates the cone component of the attenuation of a spot light.
        /// </summary>
        /// <returns></returns>
        public static string AttenuationConeComponent()
        {
            var methodBody = new List<string>
            {
                "vec3 coneDir = lightDir;",
                "float lightToSurfaceAngleCos = dot(coneDir, -fragToLightDir);",

                "float epsilon = cos(innerConeAngle) - cos(outerConeAngle);",
                "float t = (lightToSurfaceAngleCos - cos(outerConeAngle)) / epsilon;",
                "return clamp(t, 0.0, 1.0);"
            };

            return (GLSL.CreateMethod(GLSL.Type.Float, "attenuationConeComponent",
                new[] { GLSL.CreateVar(GLSL.Type.Vec3, "lightDir"), GLSL.CreateVar(GLSL.Type.Vec3, "fragToLightDir"), GLSL.CreateVar(GLSL.Type.Float, "innerConeAngle"), GLSL.CreateVar(GLSL.Type.Float, "outerConeAngle") }, methodBody));
        }

        private static List<string> ViewAndLightDir()
        {
            return new List<string>
            {
                $"vec3 V = normalize(-surfOut.position.xyz);",
                "vec3 L = vec3(0.0, 0.0, 0.0);",
                "if(light.lightType == 1)",
                "{",
                    "L = -normalize(light.direction);",
                "}",
                "else if(light.lightType == 3)",
                "{",
                    "L = normalize(vec3(0.0,0.0,-1.0));",
                "}",
                "else",
                "{",
                    $"L = normalize(light.position - surfOut.position.xyz);",
                "}",
                "",
                "vec3 Idif = vec3(0);",
                "vec3 Ispe = vec3(0);",

            };
        }

        private static List<string> Attenuation()
        {
            var res = new List<string>();

            var attPtLight = "float att = attenuationPointComponent(surfOut.position.xyz, light.position, light.maxDistance);";
            var attSpotLight = "float att = attenuationPointComponent(surfOut.position.xyz, light.position, light.maxDistance) * attenuationConeComponent(light.direction, L, light.innerConeAngle, light.outerConeAngle);";
            var attParallel = "float att = 1.0; //no attenuation --> parallel or legacy light";

            res.Add(attParallel);

            res.Add("");
            res.Add("if(light.lightType == 0) // PointLight");
            res.Add("{");
            res.Add(attPtLight);
            res.Add("}");
            res.Add("else if(light.lightType == 2) // SpotLight");
            res.Add("{");
            res.Add(attSpotLight);
            res.Add("}");

            return res;
        }

        /// <summary>
        /// Wraps all the lighting methods into a single one.
        /// </summary>
        public static string ApplyLightForward(LightingSetup setup)
        {
            var setupShards = ShaderSurfaceOut.GetLightingSetupShards(setup);
            var methodBody = new List<string>();

            switch (setup)
            {
                case LightingSetup.SpecularStd:
                    methodBody.Add("float lightStrength = (1.0 - ambientCo) * light.strength;");
                    methodBody.AddRange(ViewAndLightDir());
                    methodBody.Add($"vec3 N = normalize(surfOut.normal);");

                    methodBody.Add($"Idif = diffuseLighting(N, L) * light.intensities.xyz;");

                    methodBody.Add($"float specularTerm = specularLighting(N, L, V, surfOut.shininess);");
                    methodBody.Add($"Ispe = surfOut.specularStrength * specularTerm * light.intensities.rgb;");

                    methodBody.AddRange(Attenuation());
                    methodBody.Add("return ((Ispe * att) + ((Idif * att) * surfOut.albedo.rgb)) * lightStrength;");

                    break;
                case LightingSetup.SpecularPbr:
                    methodBody.Add("float lightStrength = (1.0 - ambientCo) * light.strength;");
                    methodBody.AddRange(ViewAndLightDir());
                    methodBody.Add($"vec3 N = normalize(surfOut.normal);");

                    methodBody.Add($"Idif = diffuseLighting(N, L) * light.intensities.xyz;");

                    methodBody.Add($"float k = 1.0 - {UniformNameDeclarations.DiffuseFraction};");
                    methodBody.Add($"float specular = specularLighting(N, L, V, k, surfOut.specularCol, {UniformNameDeclarations.FresnelReflectance}, {UniformNameDeclarations.RoughnessValue});");
                    methodBody.Add($"Ispe = light.intensities.rgb * surfOut.specularCol.rgb * (k + specular * (1.0 - k));");

                    methodBody.AddRange(Attenuation());
                    methodBody.Add("return ((Ispe * att) + ((Idif * att) * surfOut.albedo.rgb)) * lightStrength;");

                    break;
                case LightingSetup.DiffuseOnly:
                    methodBody.Add("float lightStrength = (1.0 - ambientCo) * light.strength;");
                    methodBody.AddRange(ViewAndLightDir());
                    methodBody.Add($"vec3 N = normalize(surfOut.normal);");
                    methodBody.Add($"Idif = diffuseLighting(N, L) * light.intensities.xyz;");
                    methodBody.AddRange(Attenuation());

                    methodBody.Add("return ((Idif * att) * surfOut.albedo.rgb) * lightStrength;");

                    break;
                case LightingSetup.Unlit:
                    methodBody.Add("return surfOut.albedo.rgb;");
                    break;
                default:
                    break;
            }

            return GLSL.CreateMethod(GLSL.Type.Vec3, "ApplyLight",
            new[]
            {
                "Light light",
                $"{setupShards.Name} surfOut",
                GLSL.CreateVar(GLSL.Type.Float, "ambientCo"),
            }, methodBody);
        }

        /// <summary>
        /// Wraps all the lighting methods into a single one.
        /// </summary>
        public static string ApplyLightForward(LightingProps lightingProps)
        {
            var normals = new List<string>();
            if (lightingProps.HasNormalMap)
            {
                normals.Add($"vec3 N = texture({UniformNameDeclarations.NormalMap}, {VaryingNameDeclarations.TextureCoordinates} * {UniformNameDeclarations.NormalTextureTiles}).rgb;");
                normals.Add($"N = N * 2.0 - 1.0;");
                normals.Add($"N.xy *= {UniformNameDeclarations.NormalMapIntensity};");
                normals.Add("N = normalize(TBN * N);");
            }
            else
                normals = new List<string>
                {
                    $"vec3 N = normalize({VaryingNameDeclarations.Normal});"
                };

            var fragToLightDirAndLightInit = new List<string>
            {
                "vec3 L = vec3(0.0, 0.0, 0.0);",
                "if(light.lightType == 1){L = -normalize(light.direction);}",
                "else",
                "{",
                $"L = normalize(light.position - {VaryingNameDeclarations.Position}.xyz);",
                "}",
                $"vec3 V = normalize(-{VaryingNameDeclarations.Position}.xyz);",
                "if(light.lightType == 3)",
                "{",
                "L = normalize(vec3(0.0,0.0,-1.0));",
                "}",
                $"vec2 o_texcoords = {VaryingNameDeclarations.TextureCoordinates};",
                "",
                "vec3 Idif = vec3(0);",
                "vec3 Ispe = vec3(0);",
                ""
            };

            var applyLightParams = new List<string>();
            applyLightParams.AddRange(normals);
            applyLightParams.AddRange(fragToLightDirAndLightInit);

            applyLightParams.Add("float lightStrength = (1.0 - ambientCo) * light.strength;");

            if (lightingProps.DoDiffuseLighting)
                applyLightParams.Add($"Idif = diffuseLighting(N, L) * light.intensities.xyz;");


            if (lightingProps.SpecularLighting == SpecularLighting.Std)
            {
                applyLightParams.Add($"float specularTerm = specularLighting(N, L, V, {UniformNameDeclarations.SpecularShininess});");
                applyLightParams.Add($"Ispe = {UniformNameDeclarations.SpecularStrength} * specularTerm * light.intensities.rgb;");
            }
            else if (lightingProps.SpecularLighting == SpecularLighting.Pbr)
            {
                applyLightParams.Add($"float k = 1.0 - {UniformNameDeclarations.DiffuseFraction};");
                applyLightParams.Add($"float specular = specularLighting(N, L, V, k, {UniformNameDeclarations.SpecularColor}, {UniformNameDeclarations.FresnelReflectance}, {UniformNameDeclarations.RoughnessValue});");
                applyLightParams.Add($"Ispe = light.intensities.rgb * {UniformNameDeclarations.SpecularColor}.rgb * (k + specular * (1.0 - k));");
            }

            var pointLight = new List<string>
            {
                $"float att = attenuationPointComponent({VaryingNameDeclarations.Position}.xyz, light.position, light.maxDistance);",
                "lighting = ((Ispe * att) + ((Idif * att) * objCol)) * lightStrength;"
            };

            //No attenuation!
            var parallelLight = new List<string>
            {
                "lighting = (Ispe + (Idif * objCol)) * lightStrength;"
            };

            var spotLight = new List<string>
            {
                $"float att = attenuationPointComponent({VaryingNameDeclarations.Position}.xyz, light.position, light.maxDistance) * attenuationConeComponent(light.direction, L, light.innerConeAngle, light.outerConeAngle);",
                "lighting = ((Ispe * att) + ((Idif * att) * objCol)) * lightStrength;"
            };

            var methodBody = new List<string>();
            methodBody.AddRange(applyLightParams);
            methodBody.Add("vec3 lighting = vec3(0);");
            methodBody.Add("");
            methodBody.Add("if(light.lightType == 0) // PointLight");
            methodBody.Add("{");
            methodBody.AddRange(pointLight);
            methodBody.Add("}");
            methodBody.Add("else if(light.lightType == 1 || light.lightType == 3) // ParallelLight or LegacyLight");
            methodBody.Add("{");
            methodBody.AddRange(parallelLight);
            methodBody.Add("}");
            methodBody.Add("else if(light.lightType == 2) // SpotLight");
            methodBody.Add("{");
            methodBody.AddRange(spotLight);
            methodBody.Add("}");
            methodBody.Add("");
            //methodBody.AddRange(gammaCorrection); // - Disable GammaCorrection for better colors
            methodBody.Add("");

            methodBody.Add("return lighting;");

            return GLSL.CreateMethod(GLSL.Type.Vec3, "ApplyLight",
                new[]
                {
                    "Light light",
                    GLSL.CreateVar(GLSL.Type.Vec3, "objCol"),
                    GLSL.CreateVar(GLSL.Type.Float, "ambientCo"),
                }, methodBody);
        }

        /// <summary>
        /// Wraps all the lighting methods into a single one. For deferred rendering, this equals the "main" method.
        /// </summary>
        public static string ApplyLightDeferred(Light lc, bool isCascaded = false, int numberOfCascades = 0, bool debugCascades = false)
        {
            var methodBody = new List<string>
            {
                $"vec3 normal = texture({UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_NORMAL]}, {VaryingNameDeclarations.TextureCoordinates}).rgb;"
            };

            //Do not do calculations for the background - is there a smarter way (stencil buffer)?
            //---------------------------------------
            methodBody.AddRange(
            new List<string>()
            {
            "if(normal.x == 0.0 && normal.y == 0.0 && normal.z == 0.0)",
            "{"
            });
            methodBody.Add($"    {FragProperties.OutColorName} = {UniformNameDeclarations.BackgroundColor};");
            methodBody.Add(@"    return;");
            methodBody.Add("}");

            methodBody.Add($"vec4 fragPos = texture({UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_POSITION]}, {VaryingNameDeclarations.TextureCoordinates});");
            methodBody.Add($"vec4 albedo = texture({UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO]}, {VaryingNameDeclarations.TextureCoordinates}).rgba;");

            methodBody.Add($"vec3 specularVars = texture({UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_SPECULAR]}, {VaryingNameDeclarations.TextureCoordinates}).rgb;");

            methodBody.AddRange(
                new List<string>() {
                "vec3 viewDir = normalize(-fragPos.xyz);",
                "float shininess = specularVars.r * 256.0;",
                "float specularStrength = specularVars.g;",
                ""
                }
            );

            //Lighting calculation
            //-------------------------            

            //Ambient / Occlusion
            methodBody.AddRange(new List<string>()
            {
            "// then calculate lighting as usual",
            "vec4 lighting = vec4(0);",
            "",
             "float ambientCo = 0.1;",
             "vec4 ambient = vec4(0,0,0,1);",
             "vec4 diffuse = vec4(0,0,0,1);",
            "vec4 specular = vec4(0,0,0,1);",
            $"if({UniformNameDeclarations.RenderPassNo} == 0)",
            "{",
            "   ambient = vec4(ambientCo, ambientCo, ambientCo, 1.0);",
                "",
            $"   if({UniformNameDeclarations.SsaoOn} == 1)",
             "  {",
            $"      vec4 occlusion = vec4(texture({UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.G_SSAO]}, {VaryingNameDeclarations.TextureCoordinates}).rgb, 1.0);",
            "       ambient *= occlusion;",
            "    }",
            "}",
            ""
            });

            methodBody.AddRange(new List<string>()
            {
            "   //attenuation",
            "   float attenuation = 1.0;"
            });

            //Light variables
            methodBody.AddRange(new List<string>()
            {
            "if(light.isActive == 1)",
            "{",
            "   float shadow = 0.0;",
            "",
            "   vec3 lightColor = light.intensities.xyz;",
            "   vec3 lightPosition = light.position;",
            "   vec3 lightDir = normalize(lightPosition - fragPos.xyz);",
            "" });

            switch (lc.Type)
            {
                case LightType.Point:
                    methodBody.Add(
                    "    attenuation = attenuationPointComponent(fragPos.xyz, lightPosition, light.maxDistance);"
                    );
                    break;
                case LightType.Legacy:
                case LightType.Parallel:
                    methodBody.Add(
                    "    lightDir = -light.direction;"
                    );
                    break;
                case LightType.Spot:
                    methodBody.Add(
                    "    attenuation = attenuationPointComponent(fragPos.xyz, lightPosition, light.maxDistance) * attenuationConeComponent(light.direction, lightDir, light.innerConeAngle, light.outerConeAngle);"
                    );
                    break;
                default:
                    break;
            }

            if (lc.IsCastingShadows)
            {
                methodBody.Add(GetShadow(lc, isCascaded, numberOfCascades));
            }

            // diffuse 
            methodBody.AddRange(
            new List<string>() {
            "    diffuse = vec4(diffuseLighting(normal, lightDir) * lightColor, 1.0);",
            "    diffuse = (1.0 - shadow) * (diffuse * attenuation);"
            });

            // specular
            methodBody.AddRange(
            new List<string>() {

            "    float specularTerm = specularLighting(normal, lightDir, viewDir, shininess);",
            "",
            "    specular = vec4(specularStrength * specularTerm * lightColor, 1.0);",
            "    specular = (1.0 - shadow) * (specular * attenuation);"
            });

            if (isCascaded && debugCascades)
            {
                methodBody.Add(ColorDebugCascades());
            }

            methodBody.Add("}");



            methodBody.Add($"float strength = (1.0 - ambientCo) * light.strength;");
            methodBody.Add($"lighting = (((specular + diffuse) * strength) + ambient) * albedo;");
            methodBody.Add($"{FragProperties.OutColorName} = lighting;");

            return Utility.MainMethod(methodBody);
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
        /// Creates the method for calculating whether a fragment is in shadow or not, by using a shadow map (sampler2D).
        /// </summary>
        /// <returns></returns>
        public static string ShadowCalculationCascaded()
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
                "//float closestDepth = texture(shadowMap, vec3(projCoords.xy, layer)).r;",
                "float currentDepth = projCoords.z;",

                "float thisBias = max(bias * (1.0 - dot(normal, lightDir)), bias / 100.0);",

                "vec2 texelSize = 1.0 / vec2(textureSize(shadowMap, 0));",

                "for (int x = -pcfLoop; x <= pcfLoop; ++x)",
                "{",
                    "for (int y = -pcfLoop; y <= pcfLoop; ++y)",
                    "{",
                        "float pcfDepth = texture(shadowMap, vec3(projCoords.xy + vec2(x, y) * texelSize, layer)).r;",
                        "shadow += (currentDepth - thisBias) > pcfDepth ? 1.0 : 0.0;",
                    "}",
                "}",
                "shadow /= pcfKernelSize;",

                "return shadow;"
            };

            return GLSL.CreateMethod(GLSL.Type.Float, "ShadowCalculation", new[]
            {
                GLSL.CreateVar(GLSL.Type.ArrayTexture, "shadowMap"),
                GLSL.CreateVar(GLSL.Type.Int, "layer"),
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
            "   vec3(pcfKernelHalfSize, pcfKernelHalfSize, pcfKernelHalfSize), vec3(pcfKernelHalfSize, -pcfKernelHalfSize, pcfKernelHalfSize), vec3(-pcfKernelHalfSize, -pcfKernelHalfSize, pcfKernelHalfSize), vec3(-pcfKernelHalfSize, pcfKernelHalfSize, pcfKernelHalfSize),",
            "   vec3(pcfKernelHalfSize, pcfKernelHalfSize, -pcfKernelHalfSize), vec3(pcfKernelHalfSize, -pcfKernelHalfSize, -pcfKernelHalfSize), vec3(-pcfKernelHalfSize, -pcfKernelHalfSize, -pcfKernelHalfSize), vec3(-pcfKernelHalfSize, pcfKernelHalfSize, -pcfKernelHalfSize),",
            "   vec3(pcfKernelHalfSize, pcfKernelHalfSize, 0), vec3(pcfKernelHalfSize, -pcfKernelHalfSize, 0), vec3(-pcfKernelHalfSize, -pcfKernelHalfSize, 0), vec3(-pcfKernelHalfSize, pcfKernelHalfSize, 0),",
            "   vec3(pcfKernelHalfSize, 0, pcfKernelHalfSize), vec3(-pcfKernelHalfSize, 0, pcfKernelHalfSize), vec3(pcfKernelHalfSize, 0, -pcfKernelHalfSize), vec3(-pcfKernelHalfSize, 0, -pcfKernelHalfSize),",
            "   vec3(0, pcfKernelHalfSize, pcfKernelHalfSize), vec3(0, -pcfKernelHalfSize, pcfKernelHalfSize), vec3(0, -pcfKernelHalfSize, -pcfKernelHalfSize), vec3(0, pcfKernelHalfSize, -pcfKernelHalfSize)",
            ");",

            "// get vector between fragment position and light position",
            "vec3 fragToLight = (fragPos - lightPos) * -1.0;",
            "// now get current linear depth as the length between the fragment and light position",
            "float currentDepth = length(fragToLight);",

            "float shadow = 0.0;",
            "float thisBias = max(bias * (1.0 - dot(normal, lightDir)), bias * 0.01);//0.15;",
            "int samples = 20;",
            $"vec3 camPos = {UniformNameDeclarations.IView}[3].xyz;",
            "float viewDistance = length(camPos - fragPos);",

            "float diskRadius = 0.5; //(1.0 + (viewDistance / farPlane)) / pcfKernelSize;",
            "for (int i = 0; i < samples; ++i)",
            "{",
            "   float closestDepth = texture(shadowMap, fragToLight + sampleOffsetDirections[i] * diskRadius).r;",
            "   closestDepth *= farPlane;   // Undo mapping [0;1]",
            "   if (currentDepth - thisBias > closestDepth)",
            "       shadow += 1.0;",
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

        private static string GetShadow(Light lc, bool isCascaded, int numberOfCascades = 0)
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
                ");
                frag.AppendLine($"vec4 posInLightSpace1 = (LightSpaceMatrices[thisFragmentsFirstCascade] * {UniformNameDeclarations.IView}) * fragPos;");
                frag.Append(@"
                    float shadow1 = ShadowCalculation(ShadowMap, thisFragmentsFirstCascade, posInLightSpace1, normal, lightDir,  light.bias, 1.0);                   
                    //blend cascades to avoid hard cuts between them
                    if(thisFragmentsSecondCascade != -1)
                    {  
                        float blendStartPercent = max(85.0 - (5.0 * float(thisFragmentsFirstCascade-1)), 50.0); //the farther away the cascade, the earlier we blend the shadow maps        
                    ");
                frag.AppendLine($"vec4 posInLightSpace2 = (LightSpaceMatrices[thisFragmentsSecondCascade] * {UniformNameDeclarations.IView}) * fragPos;");
                frag.Append(@"
                        float shadow2 = ShadowCalculation(ShadowMap, thisFragmentsSecondCascade, posInLightSpace2, normal, lightDir, light.bias, 1.0);    
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
                    ");
                    frag.AppendLine($"  vec4 posInLightSpace = ({UniformNameDeclarations.LightSpaceMatrix} * {UniformNameDeclarations.IView}) * fragPos;");
                    frag.Append(@"
                        shadow = ShadowCalculation(ShadowMap, posInLightSpace, normal, lightDir, light.bias, 1.0);                    
                    }                
                    ");
                }
                else
                {
                    frag.Append(@"
                    // shadow       
                    if (light.isCastingShadows == 1)
                    {
                    ");
                    frag.AppendLine($"  shadow = ShadowCalculationCubeMap(ShadowCubeMap, ({UniformNameDeclarations.IView} * fragPos).xyz, ({UniformNameDeclarations.IView} * vec4(light.position,1.0)).xyz, light.maxDistance, normal, lightDir, light.bias, 2.0);");
                    frag.AppendLine("}");
                }
            }

            return frag.ToString();
        }

        private static string ColorDebugCascades()
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
    }

    internal struct LightParamStrings
    {
        public string PositionViewSpace;
        public string Intensities;
        public string MaxDistance;
        public string Strength;
        public string OuterAngle;
        public string InnerAngle;
        public string Direction;
        public string LightType;
        public string IsActive;
        public string IsCastingShadows;
        public string Bias;

        public LightParamStrings(int arrayPos)
        {
            PositionViewSpace = $"allLights[{arrayPos}].position";
            Intensities = $"allLights[{arrayPos}].intensities";
            MaxDistance = $"allLights[{arrayPos}].maxDistance";
            Strength = $"allLights[{arrayPos}].strength";
            OuterAngle = $"allLights[{arrayPos}].outerConeAngle";
            InnerAngle = $"allLights[{arrayPos}].innerConeAngle";
            Direction = $"allLights[{arrayPos}].direction";
            LightType = $"allLights[{arrayPos}].lightType";
            IsActive = $"allLights[{arrayPos}].isActive";
            IsCastingShadows = $"allLights[{arrayPos}].isCastingShadows";
            Bias = $"allLights[{arrayPos}].bias";
        }
    }
}