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
                    $"float ambientCo = 0.2;",
                    $"vec3 ambient = vec3(ambientCo, ambientCo, ambientCo) * surfOut.albedo.rgb;",
                    $"vec3 result = vec3(0.0);",
                    $"for(int i = 0; i < {Lighting.NumberOfLightsForward}; i++)",
                    "{",
                        "if(allLights[i].isActive == 0) continue;",
                        "result += ApplyLight(allLights[i], surfOut, ambientCo);",
                    "}",
                    $"oFragmentColor = vec4(result.rgb + ambient, surfOut.albedo.a);"
                });
            }
            else
                fragMainBody.Add("oFragmentColor = surfOut.albedo;");

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
                            if (!lightingSetup.HasFlag(LightingSetupFlags.Unlit))
                                fragMainBody.Add($"{texName} = vec4(normalize(surfOut.normal.xyz), 1.0);");
                            else
                                fragMainBody.Add($"{texName} = vec4(1.0, 1.0, 1.0, 1.0);");
                        }
                        break;
                    case (int)RenderTargetTextureTypes.Depth:
                        fragMainBody.Add($"{texName} = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);");
                        break;
                    case (int)RenderTargetTextureTypes.Emission:
                        fragMainBody.Add($"{texName} = surfOut.emission;");
                        break;
                    case (int)RenderTargetTextureTypes.Specular:
                        {
                            if (lightingSetup.HasFlag(LightingSetupFlags.BRDF))
                            {
                                fragMainBody.Add("float encodedShadingModel = float((1 & 0xF) | 0) / float(0xFF);");
                                fragMainBody.Add("float invShadingModel = 1.0 / encodedShadingModel;");
                                fragMainBody.Add($"{texName} = vec4(surfOut.roughness, surfOut.metallic, surfOut.specular, encodedShadingModel);");
                            }
                            else if (lightingSetup.HasFlag(LightingSetupFlags.BlinnPhong))
                            {
                                fragMainBody.Add("float encodedShadingModel = float((2 & 0xF) | 0) / float(0xFF);");
                                fragMainBody.Add("float invShadingModel = 1.0 / encodedShadingModel;");
                                fragMainBody.Add("//reason for multiplying by 'invShadingModel': keep alpha blending enabled and allow premultiplied alpha while not changing the colors in the specular tex.");
                                fragMainBody.Add("//this is only needed if alpha blending is enabled");
                                fragMainBody.Add($"{texName} = vec4(surfOut.specularStrength, surfOut.shininess, 0.0, encodedShadingModel);");
                            }
                            else if (lightingSetup.HasFlag(LightingSetupFlags.DiffuseOnly))
                            {
                                fragMainBody.Add("float encodedShadingModel = float((3 & 0xF) | 0) / float(0xFF);");
                                fragMainBody.Add("//Shading model is 'diffuse only' - store just that.");
                                fragMainBody.Add($"{texName} = vec4(0.0, 0.0, 0.0, encodedShadingModel);");
                            }
                            else if (lightingSetup.HasFlag(LightingSetupFlags.Unlit))
                            {
                                fragMainBody.Add("float encodedShadingModel = float((4 & 0xF) | 0) / float(0xFF);");
                                fragMainBody.Add("//Shading model is 'unlit' - store just that.");
                                fragMainBody.Add($"{texName} = vec4(0.0, 0.0, 0.0, encodedShadingModel);");
                            }
                            break;
                        }
                }
            }
            return GLSL.MainMethod(fragMainBody);
        }
    }
}