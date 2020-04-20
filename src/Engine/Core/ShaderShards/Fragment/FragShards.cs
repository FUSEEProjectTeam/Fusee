using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    public static class FragShards
    {
        public static readonly string ChangeSurfFrag = "ChangeSurfFrag";

        public static string GetChangeSurfFragMethod(LightingSetup setup, List<string> methodBody, Type inputType)
        {
            return GLSL.CreateMethod(ShaderSurfaceOut.GetLightingSetupShards(setup).Name, ChangeSurfFrag, new string[] { $"{inputType.Name} IN" }, methodBody);
        }

        public static readonly List<string> SurfOutBody_SpecularStd = new List<string>()
        {
            "OUT.albedo = IN.Albedo;",
            "OUT.specularStrength = IN.SpecularStrength;",
            "OUT.shininess = IN.Shininess;",
            $"OUT.normal = {VaryingNameDeclarations.Normal};",
            $"OUT.position = {VaryingNameDeclarations.Position};",
        };

        public static readonly List<string> SurfOutBody_GBuffer = new List<string>()
        {
            $"OUT.position = {VaryingNameDeclarations.Position}",
            "OUT.albedo = IN.Albedo;",
            $"OUT.normal = vec4(normalize({VaryingNameDeclarations.Normal}.xyz), 1.0);",
            "OUT.depth = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);",
            $"OUT.specular =  vec4({UniformNameDeclarations.SpecularStrength}, {UniformNameDeclarations.SpecularShininess}/256.0, 1.0, 1.0);"
        };

        public static readonly List<string> SurfOutBody_SpecularPbr = new List<string>()
        {
            $"OUT.normal = {VaryingNameDeclarations.Normal};",
            $"OUT.position = {VaryingNameDeclarations.Position};",
            "OUT.albedo = IN.Albedo;",
            "OUT.roughness = IN.Roughness;",
            "OUT.fresnelReflect = IN.FresnelReflectance;",
            "OUT.diffuseFract = IN.DiffuseFraction;"
        };

        public static List<string> SurfOutBody_Textures(bool hasAlebedoTex, bool hasNormalTex)
        {
            var res = new List<string>()
            {
                $"OUT.position = {VaryingNameDeclarations.Position};",
                "OUT.specularStrength = IN.SpecularStrength;",
                "OUT.shininess = IN.Shininess;",
            };

            if (hasAlebedoTex)
            {

                res.Add($"vec4 texCol = texture(IN.Albedo, {VaryingNameDeclarations.TextureCoordinates} * IN.TexTiles);");
                res.Add($"vec3 mix = mix(IN.AlbedoTex.rgb, texCol.xyz, IN.AlbedoMix);");
                res.Add("float luma = pow((0.2126 * texCol.r) + (0.7152 * texCol.g) + (0.0722 * texCol.b), 1.0/2.2);");
                res.Add($"OUT.albedo = vec4(mix * luma, texCol.a);");

            }
            else
                res.Add("OUT.albedo = IN.Albedo;");

            if (hasNormalTex)
            {
                //TODO: tbn varying needs to come form the vertex shader surface shard
                res.AddRange(new List<string>
                {
                    $"vec3 N = texture(IN.NormalTex, {VaryingNameDeclarations.TextureCoordinates} * IN.TexTiles).rgb;",
                    $"N = N * 2.0 - 1.0;",
                    $"N.xy *= IN.Normal;",
                    "OUT.normal = normalize(TBN * N);"
                });
            }
            else
                res.Add($"OUT.normal = {VaryingNameDeclarations.Normal};");

            return res;
        }
    }
}
