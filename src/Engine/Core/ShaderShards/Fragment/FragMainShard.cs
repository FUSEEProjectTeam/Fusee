using Fusee.Engine.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    /// <summary>
    /// Collection of Shader Shards, describing the Main method of a fragment shader.
    /// </summary>
    public static class FragMainShard
    {
        /// <summary>
        /// Lighting Main method for forward rendering.
        /// </summary>
        /// <param name="effectProps">The <see cref="EffectProps"/> which function as a basis to build the correct lighting method.</param>
        /// <returns></returns>
        public static string ForwardLighting(EffectProps effectProps)
        {
            var fragMainBody = new List<string>
            {
                $"vec4 objCol = vec4(0);"
            };
            if (effectProps.MatProbs.HasDiffuseTexture)
            {
                fragMainBody.Add($"vec4 texCol = texture({UniformNameDeclarations.DiffuseTexture}, {VaryingNameDeclarations.TextureCoordinates} * {UniformNameDeclarations.DiffuseTextureTiles});");
                //applyLightParams.Add($"texCol = vec4(pow(texCol.r, 1.0/2.2), pow(texCol.g, 1.0/2.2), pow(texCol.b, 1.0/2.2), texCol.a);");
                fragMainBody.Add($"vec3 mix = mix({UniformNameDeclarations.Albedo}.xyz, texCol.xyz, {UniformNameDeclarations.DiffuseMix});");
                fragMainBody.Add("float luma = pow((0.2126 * texCol.r) + (0.7152 * texCol.g) + (0.0722 * texCol.b), 1.0/2.2);");
                fragMainBody.Add($"objCol = vec4(mix * luma, texCol.a);");
               
            }
            else
            {
                fragMainBody.Add($"objCol = {UniformNameDeclarations.Albedo};");
            }

            fragMainBody.AddRange(
            new List<string>()
            {
                $"float ambientCo = 0.1;",
                $"vec3 ambient = vec3(ambientCo, ambientCo, ambientCo) * objCol.rgb;",
                $"vec3 result = vec3(0.0);",
                $"for(int i = 0; i < {LightingShard.NumberOfLightsForward}; i++)",
                "{",
                "   if(allLights[i].isActive == 0) continue;",
                "   vec3 currentPosition = allLights[i].position;",
                "   vec4 currentIntensities = allLights[i].intensities;",
                "   vec3 currentConeDirection = allLights[i].direction;",
                "   float currentAttenuation = allLights[i].maxDistance;",
                "   float currentStrength = (1.0 - ambientCo) * allLights[i].strength;",
                "   float currentOuterConeAngle = allLights[i].outerConeAngle;",
                "   float currentInnerConeAngle = allLights[i].innerConeAngle;",
                "   int currentLightType = allLights[i].lightType; ",
                "   result += ApplyLight(currentPosition, currentIntensities, currentConeDirection, ",
                "   currentAttenuation, currentStrength, currentOuterConeAngle, currentInnerConeAngle, currentLightType, objCol.rgb);",
                "}",
                $"oFragmentColor = vec4(result.rgb + ambient, objCol.a);"
                 //effectProps.MatProbs.HasDiffuseTexture ? $"oFragmentColor = result;" : $"oFragmentColor = vec4(result.rgb, {UniformNameDeclarations.Albedo}.w);",
            });

            return ShaderShardUtil.MainMethod(fragMainBody);
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
                        if (effectProps.MatProbs.HasDiffuseTexture)
                            fragMainBody.Add($"{texName} = vec4(mix({UniformNameDeclarations.Albedo}.xyz, texture({UniformNameDeclarations.DiffuseTexture}, {VaryingNameDeclarations.TextureCoordinates} * {UniformNameDeclarations.DiffuseTextureTiles}).xyz, {UniformNameDeclarations.DiffuseMix}), 1.0);");
                        else
                            fragMainBody.Add($"{texName} = vec4({UniformNameDeclarations.Albedo}.xyz, 1.0);");
                        break;
                    case (int)RenderTargetTextureTypes.G_NORMAL:
                        {
                            if(!effectProps.MatProbs.HasBump)
                                fragMainBody.Add($"{texName} = vec4(normalize({VaryingNameDeclarations.Normal}.xyz), 1.0);");
                            else
                            {
                                fragMainBody.Add($"vec3 N = texture({UniformNameDeclarations.BumpTexture}, {VaryingNameDeclarations.TextureCoordinates} * {UniformNameDeclarations.BumpTextureTiles}).rgb;");
                                fragMainBody.Add($"N = N * 2.0 - 1.0;");
                                fragMainBody.Add($"N.xy *= {UniformNameDeclarations.BumpIntensity};");
                                fragMainBody.Add($"{texName} = vec4(normalize(TBN * N), 1.0);");
                            }
                        }
                        
                        break;
                    case (int)RenderTargetTextureTypes.G_DEPTH:
                        fragMainBody.Add($"{texName} = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);");
                        break;
                    case (int)RenderTargetTextureTypes.G_SPECULAR:
                        {
                            if (effectProps.MatType == MaterialType.MaterialPbr)
                            {
                                fragMainBody.Add($"{texName} = vec4({UniformNameDeclarations.RoughnessValue}, {UniformNameDeclarations.FresnelReflectance}, {UniformNameDeclarations.DiffuseFraction}, 1.0);");
                            }
                            else if (effectProps.MatType == MaterialType.Standard)
                            {
                                fragMainBody.Add($"{texName} = vec4({UniformNameDeclarations.SpecularStrength}, {UniformNameDeclarations.SpecularShininess}/256.0, 1.0, 1.0);");
                            }
                            break;
                        }
                }
            }

            return ShaderShardUtil.MainMethod(fragMainBody);
        }
    }
}
