using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    public static class FragShards
    {
        public static readonly string ChangeSurfFrag = "ChangeSurfFrag";

        public static string GetChangeSurfFragMethod(List<string> methodBody, Type inputType)
        {
            return GLSL.CreateMethod(SurfaceOut.StructName, ChangeSurfFrag, new string[] { $"{inputType.Name} IN" }, methodBody);
        }

        public static readonly List<string> SurfOutBody_SpecularStd = new List<string>()
        {
            "OUT.albedo = IN.Albedo;",
            "OUT.specularStrength = IN.SpecularStrength;",
            "OUT.shininess = IN.Shininess;",
            "return OUT;"
        };

        public static readonly List<string> SurfOutBody_GBuffer = new List<string>()
        {
            "OUT.albedo = IN.Albedo;",
            "OUT.depth = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);",
            $"OUT.specular =  vec4({UniformNameDeclarations.SpecularStrength}, {UniformNameDeclarations.SpecularShininess}/256.0, 1.0, 1.0);",
            "return OUT;"
        };

        public static readonly List<string> SurfOutBody_SpecularPbr = new List<string>()
        {
            "OUT.albedo = IN.Albedo;",
            "OUT.roughness = IN.Roughness;",
            "OUT.fresnelReflect = IN.FresnelReflectance;",
            "OUT.diffuseFract = IN.DiffuseFraction;",
            "return OUT;"
        };

        public static List<string> SurfOutBody_Textures(LightingSetup lightingSetup)
        {
            var res = new List<string>();
            if (lightingSetup.HasFlag(LightingSetup.SpecularStd))
            {
                res.Add("OUT.specularStrength = IN.SpecularStrength;");
                res.Add("OUT.shininess = IN.Shininess;");
            }
            else if (lightingSetup.HasFlag(LightingSetup.SpecularPbr))
            {
                res.Add("OUT.roughness = IN.Roughness;");
                res.Add("OUT.fresnelReflect = IN.FresnelReflectance;");
                res.Add("OUT.diffuseFract = IN.DiffuseFraction;");
            }

            if (lightingSetup.HasFlag(LightingSetup.AlbedoTex))
            {
                res.Add($"vec4 texCol = texture(IN.AlbedoTex, {VaryingNameDeclarations.TextureCoordinates} * IN.TexTiles);");
                res.Add($"vec3 mix = mix(IN.Albedo.rgb, texCol.xyz, IN.AlbedoMix);");
                res.Add("float luma = pow((0.2126 * texCol.r) + (0.7152 * texCol.g) + (0.0722 * texCol.b), 1.0/2.2);");
                res.Add($"OUT.albedo = vec4(mix * luma, texCol.a);");
            }
            else
                res.Add("OUT.albedo = IN.Albedo;");

            if (lightingSetup.HasFlag(LightingSetup.NormalMap))
            {
                res.AddRange(new List<string>
                {
                    $"vec3 N = texture(IN.NormalTex, {VaryingNameDeclarations.TextureCoordinates} * IN.TexTiles).rgb;",
                    $"N = N * 2.0 - 1.0;",
                    $"N.xy *= IN.NormalMappingStrength;",
                    "OUT.normal = normalize(TBN * N);"
                });
            }

            res.Add("return OUT;");

            return res;
        }
    }
}
