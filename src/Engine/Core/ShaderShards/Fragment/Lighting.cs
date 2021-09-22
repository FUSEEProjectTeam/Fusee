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
        internal static Dictionary<int, LightParamStrings> LightPararamStringsAllLights = new();

        /// <summary>
        /// Contains all methods for color management (gamma and from and to sRGB).
        /// </summary>
        /// <returns></returns>
        public static string ColorManagementMethods()
        {
            var lighting = new List<string>
            {
                GammaCorrection(),
                EncodeSRGB(),
                DecodeSRGB()
            };

            return string.Join("\n", lighting);
        }

        /// <summary>
        /// Collects all lighting methods, dependent on what is defined in the given <see cref="LightingSetupFlags"/> and the LightingCalculationMethod.
        /// </summary>
        /// <param name="setup">The <see cref="LightingSetupFlags"/> which is used to decide which lighting methods we need.</param>
        public static string AssembleLightingMethods(LightingSetupFlags setup)
        {
            var lighting = new List<string>
            {
                ColorManagementMethods()
            };

            //Adds methods to the PS that calculate the single light components (diffuse, specular)
            if (setup.HasFlag(LightingSetupFlags.DiffuseSpecular))
            {
                lighting.Add(AttenuationPointComponent());
                lighting.Add(AttenuationConeComponent());
                lighting.Add(LambertDiffuseComponent());
                lighting.Add(OrenNayarDiffuseComponent());
                lighting.Add(SpecularComponent());
            }
            else if (setup.HasFlag(LightingSetupFlags.BRDF))
            {
                lighting.Add(AttenuationPointComponent());
                lighting.Add(AttenuationConeComponent());
                lighting.Add(SchlickFresnel());
                lighting.Add(G1());
                lighting.Add(GetF0());
                lighting.Add(DisneyDiffuseComponent());
                lighting.Add(BRDFSpecularComponent());
            }
            else if (setup.HasFlag(LightingSetupFlags.DiffuseOnly))
            {
                lighting.Add(AttenuationPointComponent());
                lighting.Add(AttenuationConeComponent());
                lighting.Add(LambertDiffuseComponent());
                lighting.Add(OrenNayarDiffuseComponent());
            }
            else if (setup.HasFlag(LightingSetupFlags.Glossy))
            {
                lighting.Add(AttenuationPointComponent());
                lighting.Add(AttenuationConeComponent());
                lighting.Add(SchlickFresnel());
                lighting.Add(G1());
                lighting.Add(GetF0());
                lighting.Add(BRDFSpecularComponent());
            }
            else if (setup.HasFlag(LightingSetupFlags.Edl))
            {
                lighting.Add(LinearizeDepth());
                lighting.Add(EDLResponse());
                lighting.Add(EDLShadingFactor());
            }
            else if (!setup.HasFlag(LightingSetupFlags.Unlit))
            {
                throw new ArgumentOutOfRangeException($"Lighting setup unknown or incorrect: {setup}");
            }

            lighting.Add(ApplyLightForward(setup));

            return string.Join("\n", lighting);
        }

        /// <summary>
        /// Method for linerizing a depth value using the clipping planes of the current camera.
        /// </summary>
        /// <returns></returns>
        public static string LinearizeDepth()
        {
            var methodBody = new List<string>
            {
                "float near = ClippingPlanes.x;",
                "float far = ClippingPlanes.y;",

                "float z = depth * 2.0 - 1.0; // back to NDC",
                "return (2.0 * near * far) / (far + near - z * (far - near));"
        };
            return GLSL.CreateMethod(GLSL.Type.Float, "LinearizeDepth",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Float, "depth")
                }, methodBody);
        }

        /// <summary>
        /// Method for calculating the shading factor for eye dome lighting.
        /// </summary>
        public static string EDLResponse()
        {
            var methodBody = new List<string>
            {
                "vec2 pxToUv = 1.0/screenParams;",

                "vec2 offsetsToNeighbours[8] = vec2[8]",
                "(",
                 "   pixelSize * vec2(pxToUv.x, -pxToUv.y),  // right bottom",
                 "   pixelSize * vec2(pxToUv.x, 0),          // right middle",
                 "   pixelSize * vec2(pxToUv.x, pxToUv.y),   // right top",
                 "   pixelSize * vec2(0, -pxToUv.y),         // middle bottom",
                 "   pixelSize * vec2(0, pxToUv.y),          // middle top",
                 "   pixelSize * vec2(-pxToUv.x, -pxToUv.y), // left bottom",
                 "   pixelSize * vec2(-pxToUv.x, 0),         // left middle",
                 "   pixelSize * vec2(-pxToUv.x, pxToUv.y)   // left top",
                ");",

                "float response = 0.0;",
                "int neighbourCount = 0;",

                "for (int i = 0; i < 8; i++)",
                "{",
                "    vec2 neighbourUv = thisUv + offsetsToNeighbours[i];",
                "    float neighbourDepth = texture(depthTex, neighbourUv).x;",
                "    neighbourDepth = LinearizeDepth(neighbourDepth);",

                "    if (neighbourDepth == 0.0)",
                "       neighbourDepth = 1.0 / 0.0; //infinity!",

                "    response += max(0.0, log2(linearDepth) - log2(neighbourDepth));",
                "    neighbourCount += 1;",
                "}",

                "if (neighbourCount == 0)",
                "    return 1.0;",

                "return response = response / float(neighbourCount);"
            };
            return GLSL.CreateMethod(GLSL.Type.Float, "EDLResponse",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Float, "pixelSize"),
                    GLSL.CreateVar(GLSL.Type.Float, "linearDepth"),
                    GLSL.CreateVar(GLSL.Type.Vec2, "thisUv"),
                    GLSL.CreateVar(GLSL.Type.Vec2, "screenParams"),

                    GLSL.CreateVar(GLSL.Type.Sampler2D, "depthTex"),
                }, methodBody);
        }

        /// <summary>
        /// Method for calculating the eye dome lighting response.
        /// </summary>
        public static string EDLShadingFactor()
        {
            var methodBody = new List<string>
            {
                "float response = EDLResponse(float(pixelSize), linearDepth, thisUv, screenParams, depthTex);",
                "if (linearDepth == 0.0 && response == 0.0)",
                "    discard;",
                "if (response > 1.0)",
                "    response = 1.0;",
                "return exp(-response * 300.0 * edlStrength);"
            };
            return GLSL.CreateMethod(GLSL.Type.Float, "EDLShadingFactor",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Float, "edlStrength"),
                    GLSL.CreateVar(GLSL.Type.Int, "pixelSize"),
                    GLSL.CreateVar(GLSL.Type.Float, "linearDepth"),
                    GLSL.CreateVar(GLSL.Type.Vec2, "thisUv"),
                    GLSL.CreateVar(GLSL.Type.Vec2, "screenParams"),
                    GLSL.CreateVar(GLSL.Type.Sampler2D, "depthTex"),
                }, methodBody);
        }

        /// <summary>
        /// Method for calculation the diffuse lighting component.
        /// </summary>
        public static string LambertDiffuseComponent()
        {
            var methodBody = new List<string>
            {
                "return max(dot(N, L), 0.0);"
            };
            return GLSL.CreateMethod(GLSL.Type.Float, "LambertDiffuseLighting",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Vec3, "N"), GLSL.CreateVar(GLSL.Type.Vec3, "L")
                }, methodBody);
        }

        /// <summary>
        /// Method for calculation the diffuse lighting component.
        /// See: http://glslsandbox.com/e#54592.0
        /// </summary>
        /// <returns></returns>
        public static string OrenNayarDiffuseComponent()
        {
            var methodBody = new List<string>
            {
                "//OrenNayarBrdf",
                "float LdotH = clamp(dot(L, normalize(L + V)), 0.0, 1.0);",
                "float sigma2 = roughness * roughness;",
                "float s = LdotH - NdotL * NdotV;",
                "float A = 1. - .5 * (sigma2 / (((sigma2 + .33) + .000001)));",
                "float B = .45 * sigma2 / ((sigma2 + .09) + .00001);",
                "float ga = dot(V - N * NdotV, N - N * NdotL);",
                "return albedo * max(0., NdotL) * (A + B * max(0., ga) * sqrt(max((1.0 - NdotV * NdotV) * (1.0 - NdotL * NdotL), 0.)) / max(NdotL, NdotV));"
            };

            return GLSL.CreateMethod(GLSL.Type.Vec3, "OrenNayarDiffuseLighting",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Vec3, "albedo"),
                    GLSL.CreateVar(GLSL.Type.Float, "NdotL"),
                    GLSL.CreateVar(GLSL.Type.Float, "NdotV"),
                    GLSL.CreateVar(GLSL.Type.Vec3, "N"),
                    GLSL.CreateVar(GLSL.Type.Vec3, "L"),
                    GLSL.CreateVar(GLSL.Type.Vec3, "V"),
                    GLSL.CreateVar(GLSL.Type.Float, "roughness"),
                }, methodBody);
        }

        /// <summary>
        /// Method for calculation the diffuse lighting component.
        /// Replaces the standard diffuse calculation with the one introduced in [Burley 2012, "Physically-Based Shading at Disney"].
        /// </summary>
        public static string DisneyDiffuseComponent()
        {
            var methodBody = new List<string>
            {
                "// [Burley 2012, Physically-Based Shading at Disney]",
                "float FD90 = 0.5 + 2.0 * LdotH * LdotH * roughness;",
                "float FV = 1.0 + (FD90 - 1.0) * pow(1.0 - NdotV, 5.0);",
                "float FL = 1.0 + (FD90 - 1.0) * pow(1.0 - NdotL, 5.0);",
                "float Fd = FV * FL;",

                "// Based on Hanrahan-Krueger brdf approximation of isotropic bssrdf",
                "// 1.25 scale is used to (roughly) preserve albedo",
                "// Fss90 used to 'flatten' retroreflection based on roughness",
                "float Fss90 = LdotH * LdotH * roughness;",
                "float Fss = mix(1.0, Fss90, FL) * mix(1.0, Fss90, FV);",
                "float ss = 1.25 * (Fss * (1.0 / max((NdotL + NdotV), 0.001) - 0.5) + 0.5);",
                "return mix((albedo) * Fd * NdotL, (subsurfaceColor) * ss, subsurface);"
            };
            return GLSL.CreateMethod(GLSL.Type.Vec3, "DisneyDiffuseLighting",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Vec3, "albedo"),
                    GLSL.CreateVar(GLSL.Type.Float, "NdotL"),
                    GLSL.CreateVar(GLSL.Type.Float, "NdotV"),
                    GLSL.CreateVar(GLSL.Type.Float, "LdotH"),
                    GLSL.CreateVar(GLSL.Type.Float, "roughness"),
                    GLSL.CreateVar(GLSL.Type.Float, "subsurface"),
                    GLSL.CreateVar(GLSL.Type.Vec3, "subsurfaceColor"),
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

        /// <summary>
        /// Method for calculation the specular lighting component.
        /// Replaces the standard specular calculation with the Cook-Torrance calculation.
        /// </summary>
        public static string BRDFSpecularComponent()
        {
            var methodBody = new List<string>
            {
                "float alpha = roughness * roughness;",
                "float D, G;",
                "//vec3 F;",
                "",
                "// D (stribution GGX)",
                "float alphaSqr = alpha * alpha;",
                "float denom = (NdotH * NdotH) * (alphaSqr - 1.0) + 1.0f;",
                "D = alphaSqr / (PI * (denom * denom));",
                "",
                "// G (ometry - Schlicks Approximation of Smith)",
                "float r = roughness + 1.0;",
                "float k = (r * r) / 8.0;",
                "",
                "float ggx1 = G1(k, NdotV);",
                "float ggx2 = G1(k, NdotL);",
                "G = ggx1 * ggx2;",
                "",
                "// F (resnel)",
                "//float LdotH5 = SchlickFresnel(NdotV);",
                "//F = F0 + (1.0 - F0) * LdotH5;",
                "",
                "// GGX BRDF specular",
                "float resDenom = 4.0 * NdotV * NdotL;",
                "vec3 specular = (D * F * G) / max(resDenom, 0.1);",
                "return specular;"

            };
            return GLSL.CreateMethod(GLSL.Type.Vec3, "specularLighting",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Float, "NdotL"),
                    GLSL.CreateVar(GLSL.Type.Float, "NdotV"),
                    GLSL.CreateVar(GLSL.Type.Float, "LdotH"),
                    GLSL.CreateVar(GLSL.Type.Float, "NdotH"),
                    GLSL.CreateVar(GLSL.Type.Float, "roughness"),
                    GLSL.CreateVar(GLSL.Type.Vec3, "F")
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

        internal static string GetF0()
        {
            var methodBody = new List<string>
            {
                "float F0 = abs((1.0 - ior) / (1.0 + ior));",
                "F0 = F0 * F0;",
                $"return mix(vec3(F0, F0, F0), albedo.rgb, metallic);",
            };
            return GLSL.CreateMethod(GLSL.Type.Vec3, "GetF0",
                 new[]
                 {
                     GLSL.CreateVar(GLSL.Type.Vec3, "albedo"),
                     GLSL.CreateVar(GLSL.Type.Float, "ior"),
                     GLSL.CreateVar(GLSL.Type.Float, "metallic")
                 }, methodBody);
        }

        internal static string SchlickFresnel()
        {
            var methodBody = new List<string>
            {
                "float m = clamp(1.0 - value, 0.0, 1.0);",
                "return pow(m, 5.0);",
            };
            return GLSL.CreateMethod(GLSL.Type.Float, "SchlickFresnel",
                 new[]
                 {
                     GLSL.CreateVar(GLSL.Type.Float, "value")
                 }, methodBody);
        }

        internal static string G1()
        {
            var methodBody = new List<string>
            {
                "return x / (x * (1.0 - k) + k);"
            };
            return GLSL.CreateMethod(GLSL.Type.Float, "G1",
                 new[]
                 {
                     GLSL.CreateVar(GLSL.Type.Float, "k"),
                     GLSL.CreateVar(GLSL.Type.Float, "x")
                 }, methodBody);
        }

        internal static List<string> ViewAndLightDir()
        {
            return new List<string>
            {
                $"vec3 V = normalize(-surfOut.{SurfaceOut.Pos.Item2}.xyz);",
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
                    $"L = normalize(light.position - surfOut.{SurfaceOut.Pos.Item2}.xyz);",
                "}",
                "",
                "vec3 Idif = vec3(0);",
                "vec3 Ispe = vec3(0);",
                "vec3 res = vec3(0);"
            };
        }

        internal static List<string> Attenuation()
        {
            var res = new List<string>();

            var attPtLight = $"float att = attenuationPointComponent(surfOut.{SurfaceOut.Pos.Item2}.xyz, light.position, light.maxDistance);";
            var attSpotLight = $"float att = attenuationPointComponent(surfOut.{SurfaceOut.Pos.Item2}.xyz, light.position, light.maxDistance) * attenuationConeComponent(light.direction, L, light.innerConeAngle, light.outerConeAngle);";
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
        public static string ApplyLightForward(LightingSetupFlags setup)
        {
            var methodBody = new List<string>();

            if (!setup.HasFlag(LightingSetupFlags.Edl))
            {
                if (!setup.HasFlag(LightingSetupFlags.Unlit))
                {
                    methodBody.Add("float lightStrength = (1.0 - ambientCo) * light.strength;");
                    methodBody.AddRange(ViewAndLightDir());
                    methodBody.Add($"vec3 N = normalize(surfOut.{SurfaceOut.Normal.Item2});");
                }

                if (setup.HasFlag(LightingSetupFlags.DiffuseSpecular))
                {
                    methodBody.Add($"float NdotL = clamp(dot(N, L), 0.0, 1.0);");
                    methodBody.Add($"float NdotV = clamp(dot(N, V), 0.0, 1.0);");
                    methodBody.Add($"Idif = surfOut.{SurfaceOut.Roughness.Item2} > 0.0 ? OrenNayarDiffuseLighting(surfOut.{SurfaceOut.Albedo.Item2}.rgb, NdotL, NdotV, N, L, V, surfOut.{SurfaceOut.Roughness.Item2}) : LambertDiffuseLighting(N, L) * surfOut.{SurfaceOut.Albedo.Item2}.rgb;");

                    //methodBody.Add($"Idif = LambertDiffuseLighting(N, L) * surfOut.{SurfaceOut.Albedo.Item2}.rgb;");

                    methodBody.Add($"float specularTerm = specularLighting(N, L, V, surfOut.{SurfaceOut.Shininess.Item2});");
                    methodBody.Add($"Ispe = vec3(specularTerm) * surfOut.specularStrength;");

                    methodBody.AddRange(Attenuation());
                    methodBody.Add($"return  (Idif + Ispe + surfOut.{SurfaceOut.Emission.Item2}.rgb) * att * lightStrength * light.intensities.rgb;");
                }
                else if (setup.HasFlag(LightingSetupFlags.BRDF))
                {
                    methodBody.Add($"vec3 halfV = normalize(L + V);");
                    methodBody.Add($"float NdotL = clamp(dot(N, L), 0.0, 1.0);");
                    methodBody.Add($"float NdotH = clamp(dot(N, halfV), 0.0, 1.0);");
                    methodBody.Add($"float NdotV = clamp(dot(N, V), 0.0, 1.0);");
                    methodBody.Add($"float VdotH = clamp(dot(V, halfV), 0.0, 1.0);");
                    methodBody.Add($"float LdotH = clamp(dot(L, halfV), 0.0, 1.0);");

                    methodBody.Add($"vec3 F0 = GetF0(surfOut.{SurfaceOut.Albedo.Item2}.rgb, surfOut.{SurfaceOut.IOR.Item2}, surfOut.{SurfaceOut.Metallic.Item2});");
                    methodBody.Add($"float LdotH5 = SchlickFresnel(NdotV);");
                    methodBody.Add($"vec3 F = F0 + (1.0 - F0) * LdotH5;");

                    methodBody.Add($"Idif = DisneyDiffuseLighting(surfOut.albedo.rgb, NdotL, NdotV, LdotH, surfOut.{SurfaceOut.Roughness.Item2}, surfOut.{SurfaceOut.Subsurface.Item2}, surfOut.{SurfaceOut.SubsurfaceColor.Item2}.rgb);");
                    methodBody.Add($"Ispe = specularLighting(NdotL, NdotV, LdotH, NdotH, surfOut.{SurfaceOut.Roughness.Item2}, F);");

                    methodBody.Add($"//Diffuse color, taking the metallic value into account - metals do not have a diffuse component.");
                    methodBody.Add($"vec3 diffLayer = (1.0 - surfOut.{SurfaceOut.Metallic.Item2}) /** (1-_Transmission)*/ * Idif;");

                    methodBody.Add($"//Specular color, combining metallic and dielectric specular reflection.");
                    methodBody.Add($"//Metallic specular is affected by alebdo color, dielectric isn't!");
                    methodBody.Add($"vec3 specLayerDielectric = surfOut.{SurfaceOut.Specular.Item2} * Ispe;");
                    methodBody.Add($"vec3 specLayerMetallic = surfOut.{SurfaceOut.Metallic.Item2} * Ispe * surfOut.{SurfaceOut.Albedo.Item2}.rgb;");
                    methodBody.Add($"vec3 specLayer = clamp(specLayerDielectric + specLayerMetallic, 0.0, 1.0);");

                    methodBody.Add($"//Combining the layers...");
                    methodBody.Add($"res += (1.0 - F) * diffLayer;      // diffuse layer, affected by reflectivity");
                    methodBody.Add($"res += specLayer;                  // direct specular, not affected by reflectivity");
                    methodBody.Add($"res += surfOut.{SurfaceOut.Emission.Item2}.rgb;");

                    methodBody.AddRange(Attenuation());
                    methodBody.Add("return res * att * lightStrength * light.intensities.rgb;");
                }
                else if (setup.HasFlag(LightingSetupFlags.DiffuseOnly))
                {
                    methodBody.Add($"float NdotV = clamp(dot(N, V), 0.0, 1.0);");
                    methodBody.Add($"float NdotL = clamp(dot(N, L), 0.0, 1.0);");
                    methodBody.Add($"Idif = surfOut.{SurfaceOut.Roughness.Item2} > 0.0 ? OrenNayarDiffuseLighting(surfOut.{SurfaceOut.Albedo.Item2}.rgb, NdotL, NdotV, N, L, V, surfOut.{SurfaceOut.Roughness.Item2}) : LambertDiffuseLighting(N, L) * surfOut.{SurfaceOut.Albedo.Item2}.rgb;");

                    methodBody.AddRange(Attenuation());

                    methodBody.Add($"return Idif * att * lightStrength * light.intensities.rgb;");
                }
                else if (setup.HasFlag(LightingSetupFlags.Glossy))
                {
                    methodBody.Add($"vec3 halfV = normalize(L + V);");
                    methodBody.Add($"float NdotL = clamp(dot(N, L), 0.0, 1.0);");
                    methodBody.Add($"float NdotH = clamp(dot(N, halfV), 0.0, 1.0);");
                    methodBody.Add($"float NdotV = clamp(dot(N, V), 0.0, 1.0);");
                    methodBody.Add($"float VdotH = clamp(dot(V, halfV), 0.0, 1.0);");
                    methodBody.Add($"float LdotH = clamp(dot(L, halfV), 0.0, 1.0);");

                    //Glossy is a full metallic material with no diffuse component and a default IOR value
                    methodBody.Add($"vec3 F0 = GetF0(surfOut.{SurfaceOut.Albedo.Item2}.rgb, 1.45, 1.0);");
                    methodBody.Add($"float LdotH5 = SchlickFresnel(NdotV);");
                    methodBody.Add($"vec3 F = F0 + (1.0 - F0) * LdotH5;");

                    methodBody.Add($"Ispe = specularLighting(NdotL, NdotV, LdotH, NdotH, surfOut.{SurfaceOut.Roughness.Item2}, F);");

                    methodBody.AddRange(Attenuation());
                    methodBody.Add($"return Ispe * surfOut.{SurfaceOut.Albedo.Item2}.rgb * att * lightStrength * light.intensities.rgb;");
                }
                else if (setup.HasFlag(LightingSetupFlags.Unlit))
                    methodBody.Add("return surfOut.albedo.rgb;");
                else
                    throw new ArgumentOutOfRangeException($"Lighting setup unknown or incorrect: {setup}");
            }
            else
            {
                methodBody.Add("if(DoEyeDomeLighting == true)");
                methodBody.Add("{");
                methodBody.Add("    vec2 uv = vec2(gl_FragCoord.x / ScreenParams.x, gl_FragCoord.y / ScreenParams.y);");
                methodBody.Add("    float linearDepth = LinearizeDepth(texture(DepthTex, uv).x);");
                methodBody.Add("    if (linearDepth > 0.1)");
                methodBody.Add("        surfOut.albedo.rgb *= EDLShadingFactor(EDLStrength, EDLNeighbourPixels, linearDepth, uv, ScreenParams, DepthTex);");
                methodBody.Add("}");
                methodBody.Add("return surfOut.albedo.rgb;");
            }

            return GLSL.CreateMethod(GLSL.Type.Vec3, "ApplyLight",
            new[]
            {
                "Light light",
                $"{SurfaceOut.StructName} surfOut",
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
                $"vec3 normal = texture({UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Normal]}, {VaryingNameDeclarations.TextureCoordinates}).rgb;"
            };

            //Do not do calculations for the background - is there a smarter way (stencil buffer)?
            //---------------------------------------
            methodBody.AddRange(
            new List<string>()
            {
            "if(normal.x == 0.0 && normal.y == 0.0 && normal.z == 0.0)",
            "{"
            });
            methodBody.Add($"{FragProperties.OutColorName} = {UniformNameDeclarations.BackgroundColor};");
            methodBody.Add("return;");
            methodBody.Add("}");

            methodBody.Add($"vec4 fragPos = texture({UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Position]}, {VaryingNameDeclarations.TextureCoordinates});");
            methodBody.Add($"vec4 albedo = texture({UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Albedo]}, {VaryingNameDeclarations.TextureCoordinates}).rgba;");
            methodBody.Add($"vec4 emission = texture({UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Emission]}, {VaryingNameDeclarations.TextureCoordinates}).rgba;");
            methodBody.Add($"vec4 specularVars = texture({UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Specular]}, {VaryingNameDeclarations.TextureCoordinates});");


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
            "vec4 lightColor = vec4(0,0,0,1);",
            $"if({UniformNameDeclarations.RenderPassNo} == 0)",
            "{",
                "ambient = vec4(albedo.rgb * ambientCo, albedo.a);",

                $"if({UniformNameDeclarations.SsaoOn} == 1)",
                "{",
                    $"vec4 occlusion = vec4(texture({UniformNameDeclarations.DeferredRenderTextures[(int)RenderTargetTextureTypes.Ssao]}, {VaryingNameDeclarations.TextureCoordinates}).rgb, 1.0);",
                    "ambient.rgb *= vec3(occlusion);",
                "}",
            "}",
            ""
            });

            methodBody.AddRange(new List<string>()
            {
                "//attenuation",
                "float attenuation = 1.0;"
            });

            //Light variables
            methodBody.AddRange(new List<string>()
            {
            $"vec4 res = vec4(0,0,0,1.0);",
            "float shadow = 0.0;",
            "if(light.isActive == 1)",
            "{",
                "lightColor = light.intensities;",
                "vec3 lightPosition = light.position;",
                "vec3 lightDir = normalize(lightPosition - fragPos.xyz);",
            });

            switch (lc.Type)
            {
                case LightType.Point:
                    methodBody.Add(
                    "attenuation = attenuationPointComponent(fragPos.xyz, lightPosition, light.maxDistance);"
                    );
                    break;
                case LightType.Legacy:
                case LightType.Parallel:
                    methodBody.Add(
                    "lightDir = -light.direction;"
                    );
                    break;
                case LightType.Spot:
                    methodBody.Add(
                    "attenuation = attenuationPointComponent(fragPos.xyz, lightPosition, light.maxDistance) * attenuationConeComponent(light.direction, lightDir, light.innerConeAngle, light.outerConeAngle);"
                    );
                    break;
                default:
                    break;
            }

            if (lc.IsCastingShadows)
            {
                methodBody.Add(GetShadow(lc, isCascaded, numberOfCascades));
            }

            methodBody.AddRange(
            new List<string>() {
            "vec3 viewDir = normalize(-fragPos.xyz);",
            "uint decodedShadingModel = uint(round(specularVars.a * float(0xFF))) & uint(0xF);",

            "if(decodedShadingModel == uint(2))",
            "{",
                // diffuse
                "if(specularVars.b > 0.0){",
                    $"float NdotL = clamp(dot(normal, lightDir), 0.0, 1.0);",
                    $"float NdotV = clamp(dot(normal, viewDir), 0.0, 1.0);",
                    $"diffuse = vec4(OrenNayarDiffuseLighting(albedo.rgb, NdotL, NdotV, normal, lightDir, viewDir, specularVars.b), albedo.a);",
                "}",
                "else{",
                    "diffuse = vec4(vec3(LambertDiffuseLighting(normal, lightDir)), 1.0);",
                    "diffuse = diffuse * albedo;",
                "}",
                // specular
                "float shininess = specularVars.g;",
                "float specularStrength = specularVars.r;",
                "",
                "float specularTerm = specularLighting(normal, lightDir, viewDir, shininess);",
                "",
                "specular = vec4(specularTerm, specularTerm, specularTerm, 1.0);",
                "res = diffuse + specular;",

            "}",
            "else if(decodedShadingModel == uint(1))",
            "{",

                "float roughness = specularVars.r;",
                "float metallic = specularVars.g;",
                "float specular = specularVars.b;",

                "//placeholder for future subsurface implementation.",
                "float subsurface = 0.0;",
                "vec3 subsurfaceColor = vec3(1.0);",

                $"vec3 halfV = normalize(lightDir + viewDir);",
                $"float NdotL = clamp(dot(normal, lightDir), 0.0, 1.0);",
                $"float NdotH = clamp(dot(normal, halfV), 0.0, 1.0);",
                $"float NdotV = clamp(dot(normal, viewDir), 0.0, 1.0);",
                $"float VdotH = clamp(dot(viewDir, halfV), 0.0, 1.0);",
                $"float LdotH = clamp(dot(lightDir, halfV), 0.0, 1.0);",

                $"vec3 F0 = mix(vec3(0.04, 0.04, 0.04), albedo.rgb, metallic);",
                $"float LdotH5 = SchlickFresnel(NdotV);",
                $"vec3 F = F0 + (1.0 - F0) * LdotH5;",

                "vec3 diff = DisneyDiffuseLighting(albedo.rgb, NdotL, NdotV, LdotH, roughness, subsurface, subsurfaceColor);",
                "vec3 spec = specularLighting(NdotL, NdotV, LdotH, NdotH, roughness, F);",

                $"//Diffuse color, taking the metallic value into account - metals do not have a diffuse component.",
                $"vec3 diffLayer = (1.0 - metallic) /** (1-_Transmission)*/ * diff;",

                $"//Specular color, combining metallic and dielectric specular reflection.",
                $"//Metallic specular is affected by alebdo color, dielectric isn't!",
                $"vec3 specLayerDielectric = specular * spec;",
                $"vec3 specLayerMetallic = metallic * spec * albedo.rgb;",
                $"vec3 specLayer = clamp(specLayerDielectric + specLayerMetallic, 0.0, 1.0);",

                $"//Combining the layers...",
                $"res.rgb += (1.0 - F) * diffLayer;      // diffuse layer, affected by reflectivity",
                $"res.rgb += specLayer;                  // direct specular, not affected by reflectivity",
            "}",
            "else if(decodedShadingModel == uint(3))",
            "{",
                // diffuse 
                "diffuse = vec4(vec3(LambertDiffuseLighting(normal, lightDir)), 1.0);",
                "res = diffuse * albedo;",
            "}",
            "else if(decodedShadingModel == uint(4))",
            "{",
                // unlit
                "res = albedo;",
            "}",
            "else if(decodedShadingModel == uint(5))",
            "{",
                // glossy
                "float roughness = specularVars.b;",
                $"vec3 halfV = normalize(lightDir + viewDir);",
                $"float NdotL = clamp(dot(normal, lightDir), 0.0, 1.0);",
                $"float NdotH = clamp(dot(normal, halfV), 0.0, 1.0);",
                $"float NdotV = clamp(dot(normal, viewDir), 0.0, 1.0);",
                $"float VdotH = clamp(dot(viewDir, halfV), 0.0, 1.0);",
                $"float LdotH = clamp(dot(lightDir, halfV), 0.0, 1.0);",

                //Glossy is a full metallic material with no diffuse component and a default IOR value
                $"vec3 F0 = mix(vec3(0.04, 0.04, 0.04), albedo.rgb, 1.0);",
                $"float LdotH5 = SchlickFresnel(NdotV);",
                $"vec3 F = F0 + (1.0 - F0) * LdotH5;",

                "vec3 spec = specularLighting(NdotL, NdotV, LdotH, NdotH, roughness, F);",
                "res = vec4(spec * albedo.rgb, 1.0);",
            "}",
            "else if(decodedShadingModel == uint(6))",
            "{",
                // EDL
                //TODO: add edl methods
                "res = albedo;",
            "}",

            });

            if (isCascaded && debugCascades)
            {
                methodBody.Add(ColorDebugCascades());
            }

            methodBody.Add("}");
            methodBody.AddRange(
            new List<string>()
            {
                "if(specularVars.a != 4.0) //4.0 == unlit",
                "{",
                    $"float strength = (1.0 - ambientCo) * light.strength;",
                    $"lighting = emission + ambient + ((1.0 - shadow) * res * attenuation * strength * lightColor);",
                "}",
                "else",
                "{",
                    "lighting = albedo;",
                "}",
            });

            //methodBody.Add($"{FragProperties.OutColorName} = vec4(GammaCorrection(lighting.rgb, 1.0/1.9), lighting.a);");
            methodBody.Add($"{FragProperties.OutColorName} = vec4(EncodeSRGB(lighting.rgb), lighting.a);");

            return GLSL.MainMethod(methodBody);
        }

        /// <summary>
        /// Converts a color from linear space to sRGB.
        /// </summary>
        public static string EncodeSRGB()
        {
            var methodBody = new List<string>
            {
                "vec3 a = 12.92 * linearRGB;",
                "vec3 b = 1.055 * pow(linearRGB, vec3(1.0 / 2.4)) - 0.055;",
                "vec3 c = step(vec3(0.0031308), linearRGB);",
                "return mix(a, b, c);"
            };

            return GLSL.CreateMethod(GLSL.Type.Vec3, "EncodeSRGB",
               new[]
               {
                    GLSL.CreateVar(GLSL.Type.Vec3, "linearRGB")
               }, methodBody);
        }

        /// <summary>
        /// Converts a color from sRGB to linear space.
        /// </summary>
        public static string DecodeSRGB()
        {
            var methodBody = new List<string>
            {
                "vec3 a = screenRGB / 12.92;",
                "vec3 b = pow((screenRGB + 0.055) / 1.055, vec3(2.4));",
                "vec3 c = step(vec3(0.04045), screenRGB);",
                "return mix(a, b, c);"
            };

            return GLSL.CreateMethod(GLSL.Type.Vec3, "DecodeSRGB",
              new[]
              {
                    GLSL.CreateVar(GLSL.Type.Vec3, "screenRGB")
              }, methodBody);
        }

        /// <summary>
        /// Method for gamma correction.
        /// </summary>
        public static string GammaCorrection()
        {
            var methodBody = new List<string> { "return pow(color, vec3(g));" };

            return GLSL.CreateMethod(GLSL.Type.Vec3, "GammaCorrection",
                new[]
                {
                    GLSL.CreateVar(GLSL.Type.Vec3, "color"),
                    GLSL.CreateVar(GLSL.Type.Float, "g")
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
                        "shadow -= texture(shadowMap, vec3(projCoords.xy + vec2(x, y) * texelSize, projCoords.z), thisBias).r;",
                    "}",
                "}",
                "shadow /= pcfKernelSize;",

                "return shadow;"
            };

            return GLSL.CreateMethod(GLSL.Type.Float, "ShadowCalculation", new[]
            {
                GLSL.CreateVar(GLSL.Type.Sampler2DShadow, "shadowMap"),
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
                        "float pcfDepth = texture(shadowMap, vec4(vec3(projCoords.xy + vec2(x, y) * texelSize, layer), projCoords.z)).r;",
                        "shadow += (currentDepth - thisBias) > pcfDepth ? 1.0 : 0.0;",
                    "}",
                "}",
                "shadow /= pcfKernelSize;",

                "return shadow;"
            };

            return GLSL.CreateMethod(GLSL.Type.Float, "ShadowCalculation", new[]
            {
                GLSL.CreateVar(GLSL.Type.ArrayTextureShadow, "shadowMap"),
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
                    frag.AppendLine($"  shadow = ShadowCalculationCubeMap({UniformNameDeclarations.ShadowCubeMap}, ({UniformNameDeclarations.IView} * fragPos).xyz, ({UniformNameDeclarations.IView} * vec4(light.position,1.0)).xyz, light.maxDistance, normal, lightDir, light.bias, 1.0);");
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