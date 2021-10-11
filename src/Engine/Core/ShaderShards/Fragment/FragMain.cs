using Fusee.Engine.Common;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    /// <summary>
    /// Collection of shader code strings, describing the Main method of a fragment shader.
    /// </summary>
    public static class FragMain
    {
        /// <summary>
        /// Lighting Main method for forward rendering.
        /// </summary>
        public static string ForwardLighting(LightingSetupFlags setup, string inStructName, string outStructType)
        {
            var fragMainBody = new List<string>
            {
                $"{outStructType} surfOut = {SurfaceOut.ChangeSurfFrag}({inStructName});"
            };

            if (!setup.HasFlag(LightingSetupFlags.Unlit))
            {
                fragMainBody.AddRange(
                new List<string>()
                {
                    $"float ambientCo = 0.1;",
                    $"vec3 ambient = vec3(ambientCo, ambientCo, ambientCo) * surfOut.albedo.rgb;",
                    $"vec3 result = vec3(0.0);",
                    $"for(int i = 0; i < {Lighting.NumberOfLightsForward}; i++)",
                    "{",
                        "if(allLights[i].isActive == 0) continue;",
                        "result += ApplyLight(allLights[i], surfOut, ambientCo);",
                    "}",
                    //$"oFragmentColor = vec4(GammaCorrection(result.rgb, 1.0/2.0)+ ambient, surfOut.albedo.a);"
                    $"oFragmentColor = vec4(EncodeSRGB(result.rgb) + ambient, surfOut.albedo.a);"
                });
            }
            else
            {
                //fragMainBody.Add("oFragmentColor = vec4(GammaCorrection(surfOut.albedo.rgb, 1.0/2.0), surfOut.albedo.a);");
                fragMainBody.Add("oFragmentColor = vec4(EncodeSRGB(surfOut.albedo.rgb), surfOut.albedo.a);");
            }

            return GLSL.MainMethod(fragMainBody);
        }

        /// <summary>
        /// The main method for rendering into a G-Buffer object.
        /// </summary>
        public static string RenderToGBuffer(LightingSetupFlags lightingSetup, string inStructName, string outStructType)
        {
            var fragMainBody = new List<string>
            {
                $"{outStructType} surfOut = {SurfaceOut.ChangeSurfFrag}({inStructName});"
            };

            var ssaoString = RenderTargetTextureTypes.Ssao.ToString();

            for (var i = 0; i < UniformNameDeclarations.DeferredRenderTextures.Count; i++)
            {
                var texName = UniformNameDeclarations.DeferredRenderTextures[i];
                if (texName == ssaoString) continue;
                switch (i)
                {
                    case (int)RenderTargetTextureTypes.Position:
                        fragMainBody.Add($"{texName} = vec4(surfOut.position);");
                        break;
                    case (int)RenderTargetTextureTypes.Albedo:
                        fragMainBody.Add($"{texName} = surfOut.albedo;");

                        break;
                    case (int)RenderTargetTextureTypes.Normal:
                        {
                            if (!lightingSetup.HasFlag(LightingSetupFlags.Unlit) && !lightingSetup.HasFlag(LightingSetupFlags.Edl))
                                fragMainBody.Add($"{texName} = vec4(normalize(surfOut.normal.xyz), 1.0);");
                            else
                                fragMainBody.Add($"{texName} = vec4(1.0, 1.0, 1.0, 1.0);");
                        }
                        break;
                    case (int)RenderTargetTextureTypes.Depth:
                        fragMainBody.Add($"{texName} = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);");
                        break;
                    case (int)RenderTargetTextureTypes.Emission:
                        if (!lightingSetup.HasFlag(LightingSetupFlags.DiffuseOnly) && !lightingSetup.HasFlag(LightingSetupFlags.Glossy) && !lightingSetup.HasFlag(LightingSetupFlags.Unlit) && !lightingSetup.HasFlag(LightingSetupFlags.Edl))
                        {
                            fragMainBody.Add($"{texName} = surfOut.emission;");
                        }
                        break;
                    case (int)RenderTargetTextureTypes.Specular:
                        {
                            if (lightingSetup.HasFlag(LightingSetupFlags.BRDF))
                            {
                                fragMainBody.Add("float encodedShadingModel = float((1 & 0xF) | 0) / float(0xFF);");
                                fragMainBody.Add($"{texName} = vec4(surfOut.roughness, surfOut.metallic, surfOut.specular, encodedShadingModel);");
                            }
                            else if (lightingSetup.HasFlag(LightingSetupFlags.DiffuseSpecular))
                            {
                                fragMainBody.Add("float encodedShadingModel = float((2 & 0xF) | 0) / float(0xFF);");
                                fragMainBody.Add($"{texName} = vec4(surfOut.specularStrength, surfOut.shininess, surfOut.roughness, encodedShadingModel);");
                            }
                            else if (lightingSetup.HasFlag(LightingSetupFlags.DiffuseOnly))
                            {
                                fragMainBody.Add("float encodedShadingModel = float((3 & 0xF) | 0) / float(0xFF);");
                                fragMainBody.Add("//Shading model is 'diffuse only' - store just roughness.");
                                fragMainBody.Add($"{texName} = vec4(0.0, 0.0, surfOut.roughness, encodedShadingModel);");
                            }
                            else if (lightingSetup.HasFlag(LightingSetupFlags.Unlit))
                            {
                                fragMainBody.Add("float encodedShadingModel = float((4 & 0xF) | 0) / float(0xFF);");
                                fragMainBody.Add("//Shading model is 'unlit' - store just that.");
                                fragMainBody.Add($"{texName} = vec4(0.0, 0.0, 0.0, encodedShadingModel);");
                            }
                            else if (lightingSetup.HasFlag(LightingSetupFlags.Glossy))
                            {
                                fragMainBody.Add("float encodedShadingModel = float((5 & 0xF) | 0) / float(0xFF);");
                                fragMainBody.Add("//Shading model is 'glossy' - store just roughness.");
                                fragMainBody.Add($"{texName} = vec4(0.0, 0.0, surfOut.roughness, encodedShadingModel);");
                            }
                            else if (lightingSetup.HasFlag(LightingSetupFlags.Edl))
                            {
                                fragMainBody.Add("float encodedShadingModel = float((6 & 0xF) | 0) / float(0xFF);");
                                fragMainBody.Add("float encodedNeighbourPx = float((EDLNeighbourPixels & 0xF) | 0) / float(0xFF);");
                                fragMainBody.Add("//Shading model is 'edl' - store EDLStrength and EDLNeighbourPixels.");
                                fragMainBody.Add($"{texName} = vec4(EDLStrength, encodedNeighbourPx, 0.0, encodedShadingModel);");
                            }
                            break;
                        }
                }
            }
            return GLSL.MainMethod(fragMainBody);
        }
    }
}