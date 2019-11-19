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
        /// <param name="effectProps">The <see cref="ShaderEffectProps"/> which function as a basis to build the correct lighting method.</param>
        /// <returns></returns>
        public static string ForwardLighting(ShaderEffectProps effectProps)
        {
            string fragColorAlpha = effectProps.MatProbs.HasDiffuse ? $"{UniformNameDeclarations.DiffuseColor}.w" : "1.0";

            var fragMainBody = new List<string>
            {
                $"vec4 result = ambientLighting(0.2, {UniformNameDeclarations.DiffuseColor});", //ambient component
                $"for(int i = 0; i < {LightingShard.NumberOfLightsForward};i++)",
                "{",
                "if(allLights[i].isActive == 0) continue;",
                "vec3 currentPosition = allLights[i].position;",
                "vec4 currentIntensities = allLights[i].intensities;",
                "vec3 currentConeDirection = allLights[i].direction;",
                "float currentAttenuation = allLights[i].maxDistance;",
                "float currentStrength = allLights[i].strength;",
                "float currentOuterConeAngle = allLights[i].outerConeAngle;",
                "float currentInnerConeAngle = allLights[i].innerConeAngle;",
                "int currentLightType = allLights[i].lightType; ",
                "result += ApplyLight(currentPosition, currentIntensities, currentConeDirection, ",
                "currentAttenuation, currentStrength, currentOuterConeAngle, currentInnerConeAngle, currentLightType);",
                "}",

                 effectProps.MatProbs.HasDiffuseTexture ? $"oFragmentColor = result;" : $"oFragmentColor = vec4(result.rgb, {UniformNameDeclarations.DiffuseColor}.w);",
            };

            return ShaderShardUtil.MainMethod(fragMainBody);
        }

        /// <summary>
        /// The main method for rendering into a G-Buffer object.
        /// </summary>
        /// <param name="effectProps">The ShaderEffectProps.</param>
        /// <returns></returns>
        public static string RenderToGBuffer(ShaderEffectProps effectProps)
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
                            fragMainBody.Add($"{texName} = vec4(mix({UniformNameDeclarations.DiffuseColor}.xyz, texture(DiffuseTexture, {VaryingNameDeclarations.TextureCoordinates}).xyz, {UniformNameDeclarations.DiffuseMix}), 1.0);");
                        else
                            fragMainBody.Add($"{texName} = vec4({UniformNameDeclarations.DiffuseColor}.xyz, 1.0);");
                        break;
                    case (int)RenderTargetTextureTypes.G_NORMAL:
                        fragMainBody.Add($"{texName} = vec4(normalize({VaryingNameDeclarations.Normal}.xyz), 1.0);");
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
                                fragMainBody.Add($"{texName} = vec4({UniformNameDeclarations.SpecularStrength}, {UniformNameDeclarations.SpecularShininessName}/256.0, 1.0, 1.0);");
                            }
                            break;
                        }
                }
            }

            return ShaderShardUtil.MainMethod(fragMainBody);
        }
    }
}
