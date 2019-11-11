using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    public static class FragMainShard
    {
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
    }
}
