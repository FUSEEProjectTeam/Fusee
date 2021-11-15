using Fusee.Engine.Common;
using System;
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
        public static string ForwardLighting(ShadingModel shadingModel, string inStructName, string outStructType)
        {
            var fragMainBody = new List<string>
            {
                $"{outStructType} surfOut = {SurfaceOut.ChangeSurfFrag}({inStructName});"
            };

            if (shadingModel != ShadingModel.Unlit)
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
                    $"oFragmentColor = vec4(EncodeSRGB(result.rgb + ambient), surfOut.albedo.a);"
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
        public static string RenderToGBuffer(ShadingModel shadingModel, string inStructName, string outStructType)
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
                var shadingModelInt = shadingModel switch
                {
                    ShadingModel.Unlit => 4,
                    ShadingModel.DiffuseSpecular => 2,
                    ShadingModel.DiffuseOnly => 3,
                    ShadingModel.Glossy => 5,
                    ShadingModel.BRDF => 1,
                    ShadingModel.Edl => 6,
                    _ => throw new InvalidOperationException("Invalid ShadingModel!"),
                };
                switch (i)
                {
                    case (int)RenderTargetTextureTypes.Position:
                        {
                            fragMainBody.Add($"float encodedShadingModel = float(({shadingModelInt} & 0xF) | 0) / float(0xFF);");
                            fragMainBody.Add($"{texName} = vec4(surfOut.position, encodedShadingModel);");
                            break;
                        }
                    case (int)RenderTargetTextureTypes.Albedo:
                        fragMainBody.Add($"{texName} = surfOut.albedo;");

                        break;
                    case (int)RenderTargetTextureTypes.Normal:
                        {
                            if (shadingModel != (ShadingModel.Unlit) && shadingModel != (ShadingModel.Edl))
                                fragMainBody.Add($"{texName} = vec4(normalize(surfOut.{SurfaceOut.Normal.Item2}.xyz), 1.0);");
                            else
                                fragMainBody.Add($"{texName} = vec4(1.0, 1.0, 1.0, 1.0);");
                        }
                        break;
                    case (int)RenderTargetTextureTypes.Depth:
                        fragMainBody.Add($"{texName} = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);");
                        break;
                    case (int)RenderTargetTextureTypes.Emission:                        
                            if (shadingModel == ShadingModel.BRDF)
                                fragMainBody.Add($"{texName} = vec4(surfOut.{SurfaceOut.Emission.Item2}, surfOut.{SurfaceOut.Thickness.Item2});");
                            else
                                fragMainBody.Add($"{texName} = vec4(surfOut.{SurfaceOut.Emission.Item2}, 1.0);");
                        break;
                    case (int)RenderTargetTextureTypes.Subsurface:
                        if (shadingModel == ShadingModel.BRDF)
                        {
                            fragMainBody.Add($"{texName} = vec4(surfOut.{SurfaceOut.SubsurfaceColor.Item2}.rgb, surfOut.{SurfaceOut.Subsurface.Item2});");
                        }
                        break;
                    case (int)RenderTargetTextureTypes.Specular:
                        {
                            switch (shadingModel)
                            {
                                case ShadingModel.BRDF:
                                    fragMainBody.Add($"{texName} = vec4(surfOut.{SurfaceOut.Roughness.Item2}, surfOut.{SurfaceOut.Metallic.Item2}, surfOut.{SurfaceOut.Specular.Item2}, surfOut.{SurfaceOut.IOR.Item2});");
                                    break;
                                case ShadingModel.DiffuseSpecular:
                                    fragMainBody.Add($"{texName} = vec4(surfOut.{SurfaceOut.SpecularStrength.Item2}, surfOut.{SurfaceOut.Shininess.Item2}, surfOut.{SurfaceOut.Roughness.Item2}, 0.0);");
                                    break;
                                case ShadingModel.Glossy:
                                case ShadingModel.DiffuseOnly:
                                    fragMainBody.Add("//Shading model is 'diffuse only' or 'glossy' - store just roughness.");
                                    fragMainBody.Add($"{texName} = vec4(0.0, 0.0, surfOut.{SurfaceOut.Roughness.Item2}, 0.0);");
                                    break;
                                case ShadingModel.Unlit:
                                    fragMainBody.Add("//Shading model is 'unlit' - store just that.");
                                    fragMainBody.Add($"{texName} = vec4(0.0, 0.0, 0.0, 0.0);");
                                    break;
                                case ShadingModel.Edl:
                                    fragMainBody.Add("float encodedNeighbourPx = float((EDLNeighbourPixels & 0xF) | 0) / float(0xFF);");
                                    fragMainBody.Add("//Shading model is 'edl' - store EDLStrength and EDLNeighbourPixels.");
                                    fragMainBody.Add($"{texName} = vec4(EDLStrength, encodedNeighbourPx, 0.0, 0.0);");
                                    break;
                                default:
                                    throw new InvalidOperationException("Invalid ShadingModel.");
                            }
                            break;
                        }
                }
            }
            return GLSL.MainMethod(fragMainBody);
        }
    }
}