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
        /// <param name="effectProps">The <see cref="EffectProps"/> which function as a basis to build the correct lighting method.</param>
        /// <returns></returns>
        public static string ForwardLighting(EffectProps effectProps)
        {
            var fragMainBody = new List<string>();

            if (effectProps.LightingProps.HasDiffuseTexture)
            {
                fragMainBody.Add($"vec4 texCol = texture({UniformNameDeclarations.AlbedoTexture}, {VaryingNameDeclarations.TextureCoordinates} * {UniformNameDeclarations.DiffuseTextureTiles});");
                //applyLightParams.Add($"texCol = vec4(pow(texCol.r, 1.0/2.2), pow(texCol.g, 1.0/2.2), pow(texCol.b, 1.0/2.2), texCol.a);");
                fragMainBody.Add($"vec3 mix = mix({UniformNameDeclarations.Albedo}.xyz, texCol.xyz, {UniformNameDeclarations.AlbedoMix});");
                fragMainBody.Add("float luma = pow((0.2126 * texCol.r) + (0.7152 * texCol.g) + (0.0722 * texCol.b), 1.0/2.2);");
                fragMainBody.Add($"vec4 objCol = vec4(mix * luma, texCol.a);");
            }
            else
            {
                fragMainBody.Add($"vec4 objCol = {UniformNameDeclarations.Albedo};");
            }

            fragMainBody.AddRange(
            new List<string>()
            {
                $"float ambientCo = 0.1;",
                $"vec3 ambient = vec3(ambientCo, ambientCo, ambientCo) * objCol.rgb;",
                $"vec3 result = vec3(0.0);",
                $"for(int i = 0; i < {Lighting.NumberOfLightsForward}; i++)",
                "{",
                "   if(allLights[i].isActive == 0) continue;",
                "   result += ApplyLight(allLights[i], objCol.rgb, ambientCo);",
                "}",
                $"oFragmentColor = vec4(result.rgb + ambient, objCol.a);"
            });

            return Utility.MainMethod(fragMainBody);
        }

        //TODO: Merge methods
        /// <summary>
        /// Lighting Main method for forward rendering.
        /// </summary>
        public static string ForwardLighting(string inStructName, string outStructType)
        {
            var fragMainBody = new List<string>
            {
                $"{outStructType} surfOut = {FragShards.ChangeSurfFrag}({inStructName});"
            };

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
                $"oFragmentColor = vec4(result.rgb + ambient, surfOut.albedo.a);"

            });

            return Utility.MainMethod(fragMainBody);
        }

        /// <summary>
        /// The main method for rendering into a G-Buffer object.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>
        /// <returns></returns>
        public static string RenderToGBuffer(EffectProps effectProps)
        {
            var fragMainBody = new List<string>();

            var ssaoString = RenderTargetTextureTypes.G_SSAO.ToString();

            for (int i = 0; i < UniformNameDeclarations.DeferredRenderTextures.Count; i++)
            {
                var texName = UniformNameDeclarations.DeferredRenderTextures[i];
                if (texName == ssaoString) continue;
                switch (i)
                {
                    case (int)RenderTargetTextureTypes.G_POSITION:
                        fragMainBody.Add($"{texName} = vec4({VaryingNameDeclarations.Position});");
                        break;
                    case (int)RenderTargetTextureTypes.G_ALBEDO:
                        if (effectProps.LightingProps.HasDiffuseTexture)
                        {
                            fragMainBody.Add($"vec4 texCol = texture({UniformNameDeclarations.AlbedoTexture}, {VaryingNameDeclarations.TextureCoordinates} * {UniformNameDeclarations.DiffuseTextureTiles});");
                            fragMainBody.Add($"{texName} = vec4(mix({UniformNameDeclarations.Albedo}.xyz, texCol.xyz, {UniformNameDeclarations.AlbedoMix}), texCol.a);");
                        }
                        else
                            fragMainBody.Add($"{texName} = {UniformNameDeclarations.Albedo};");

                        break;
                    case (int)RenderTargetTextureTypes.G_NORMAL:
                        {
                            if (!effectProps.LightingProps.HasNormalMap)
                                fragMainBody.Add($"{texName} = vec4(normalize({VaryingNameDeclarations.Normal}.xyz), 1.0);");
                            else
                            {
                                fragMainBody.Add($"vec3 N = texture({UniformNameDeclarations.NormalMap}, {VaryingNameDeclarations.TextureCoordinates} * {UniformNameDeclarations.NormalTextureTiles}).rgb;");
                                fragMainBody.Add($"N = N * 2.0 - 1.0;");
                                fragMainBody.Add($"N.xy *= {UniformNameDeclarations.NormalMapIntensity};");
                                fragMainBody.Add($"{texName} = vec4(normalize(TBN * N), 1.0);");
                            }
                        }

                        break;
                    case (int)RenderTargetTextureTypes.G_DEPTH:
                        fragMainBody.Add($"{texName} = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);");
                        break;
                    case (int)RenderTargetTextureTypes.G_SPECULAR:
                        {
                            if (effectProps.LightingProps.SpecularLighting == SpecularLighting.Pbr)
                            {
                                fragMainBody.Add($"{texName} = vec4({UniformNameDeclarations.RoughnessValue}, {UniformNameDeclarations.FresnelReflectance}, {UniformNameDeclarations.DiffuseFraction}, 1.0);");
                            }
                            else if (effectProps.LightingProps.SpecularLighting == SpecularLighting.Std)
                            {
                                fragMainBody.Add($"{texName} = vec4({UniformNameDeclarations.SpecularStrength}, {UniformNameDeclarations.SpecularShininess}, 1.0, 1.0);");
                            }
                            break;
                        }
                }
            }

            return Utility.MainMethod(fragMainBody);
        }

        /// <summary>
        /// The main method for rendering into a G-Buffer object.
        /// </summary>       
        /// <returns></returns>
        public static string RenderToGBuffer(LightingSetup lightingSetup, string inStructName, string outStructType)
        {
            var fragMainBody = new List<string>
            {
                $"{outStructType} surfOut = {FragShards.ChangeSurfFrag}({inStructName});"
            };

            var ssaoString = RenderTargetTextureTypes.G_SSAO.ToString();

            for (int i = 0; i < UniformNameDeclarations.DeferredRenderTextures.Count; i++)
            {
                var texName = UniformNameDeclarations.DeferredRenderTextures[i];
                if (texName == ssaoString) continue;
                switch (i)
                {
                    case (int)RenderTargetTextureTypes.G_POSITION:
                        fragMainBody.Add($"{texName} = vec4(surfOut.position);");
                        break;
                    case (int)RenderTargetTextureTypes.G_ALBEDO:
                        //if (effectProps.LightingProps.HasDiffuseTexture)
                        //{
                        //    fragMainBody.Add($"vec4 texCol = texture({UniformNameDeclarations.AlbedoTexture}, {VaryingNameDeclarations.TextureCoordinates} * {UniformNameDeclarations.DiffuseTextureTiles});");
                        //    fragMainBody.Add($"{texName} = vec4(mix({UniformNameDeclarations.Albedo}.xyz, texCol.xyz, {UniformNameDeclarations.AlbedoMix}), texCol.a);");
                        //}
                        //else
                            fragMainBody.Add($"{texName} = surfOut.albedo;");

                        break;
                    case (int)RenderTargetTextureTypes.G_NORMAL:
                        {
                            //if (!effectProps.LightingProps.HasNormalMap)
                                fragMainBody.Add($"{texName} = vec4(normalize(surfOut.normal.xyz), 1.0);");
                            //else
                            //{
                            //    fragMainBody.Add($"vec3 N = texture({UniformNameDeclarations.NormalMap}, {VaryingNameDeclarations.TextureCoordinates} * {UniformNameDeclarations.NormalTextureTiles}).rgb;");
                            //    fragMainBody.Add($"N = N * 2.0 - 1.0;");
                            //    fragMainBody.Add($"N.xy *= {UniformNameDeclarations.NormalMapIntensity};");
                            //    fragMainBody.Add($"{texName} = vec4(normalize(TBN * N), 1.0);");
                            //}
                        }

                        break;
                    case (int)RenderTargetTextureTypes.G_DEPTH:
                        fragMainBody.Add($"{texName} = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);");
                        break;
                    case (int)RenderTargetTextureTypes.G_SPECULAR:
                        {
                            if (lightingSetup == LightingSetup.SpecularPbr)
                            {
                                fragMainBody.Add($"{texName} = vec4(surfOut.roughness, surfOut.fresnelReflect, surfOut.diffuseFract, 1.0);");
                            }
                            else if (lightingSetup == LightingSetup.SpecularStd)
                            {
                                fragMainBody.Add("//reason for multiplying by 0.5: keep alpha blending enabled and allow premultiplied alpha while not changing the colors in the specular tex.");
                                fragMainBody.Add($"{texName} = vec4(surfOut.specularStrength * 0.5, surfOut.shininess * 0.5, 0.0, 2.0);");
                            }
                            break;
                        }
                }
            }

            return Utility.MainMethod(fragMainBody);
        }
    }
}
