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
            string fragColorAlpha = effectProps.MatProbs.HasDiffuse ? $"{UniformNameDeclarations.DiffuseColorName}.w" : "1.0";

            var fragMainBody = new List<string>
            {
                "vec4 result = ambientLighting(0.2);", //ambient component
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

                 effectProps.MatProbs.HasDiffuseTexture ? $"oFragmentColor = result;" : $"oFragmentColor = vec4(result.rgb, {UniformNameDeclarations.DiffuseColorName}.w);",
            };

            return ShaderShardUtil.MainMethod(fragMainBody);
        }

        /// <summary>
        /// The main method for rendering into a G-Buffer object.
        /// </summary>
        /// <param name="hasDiffuseTex">Basis for calculating the albedo color.</param>
        /// <returns></returns>
        public static string RenderToGBuffer(bool hasDiffuseTex)
        {
            var fragMainBody = new List<string>();            

            for (int i = 0; i < UniformNameDeclarations.DeferredRenderTextures.Length - 1; i++)
            {
                var texName = UniformNameDeclarations.DeferredRenderTextures[i];

                switch (i)
                {
                    case 0: //POSITION
                        fragMainBody.Add($"{texName} = vec4(vPos.xyz, vPos.w);");
                        break;
                    case 1: //ALBEDO_SPECULAR
                        if (hasDiffuseTex)
                            fragMainBody.Add($"{texName} = vec4(mix({UniformNameDeclarations.DiffuseColorName}.xyz, texture(DiffuseTexture, vUv).xyz, DiffuseMix), {UniformNameDeclarations.SpecularIntensityName});");
                        else
                            fragMainBody.Add($"{texName} = vec4({UniformNameDeclarations.DiffuseColorName}.xyz, {UniformNameDeclarations.SpecularIntensityName});");
                        break;
                    case 2: //NORMAL
                        fragMainBody.Add($"{texName} = vec4(normalize(vNormal.xyz), 1.0);");
                        break;
                    case 3: //DEPTH
                        fragMainBody.Add($"{texName} = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);");
                        break;
                }
            }

            return ShaderShardUtil.MainMethod(fragMainBody);
        }


    }
}
