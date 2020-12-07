using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    /// <summary>
    /// Contains pre-defined Shader Shards = 
    /// content of the Fragment Shader's method that lets us change values of the "out"-struct that is used for the lighting calculation. 
    /// </summary>
    public static class FragShards
    {
        /// <summary>
        /// Returns a default method body for a diffuse-specular lighting calculation.
        /// </summary>
        public static readonly List<string> SurfOutBody_Color = new List<string>()
        {
            "OUT.albedo = IN.Albedo;"
        };

        /// <summary>
        /// Returns a default method body for a diffuse-specular lighting calculation.
        /// </summary>
        public static readonly List<string> SurfOutBody_Roughness = new List<string>()
        {
            "OUT.albedo = IN.Albedo;",
            "OUT.roughness = IN.Roughness;"
        };

        /// <summary>
        /// Returns a default method body for a diffuse-specular lighting calculation.
        /// </summary>
        public static readonly List<string> SurfOutBody_DiffSpecular = new List<string>()
        {
            "OUT.albedo = IN.Albedo;",
            "OUT.specularStrength = IN.SpecularStrength;",
            "OUT.shininess = IN.Shininess;",
            "OUT.roughness = IN.Roughness;",
            "OUT.emission = IN.Emission;"
        };

        /// <summary>
        /// Returns a default method body for rendering into a G-Buffer.
        /// </summary>
        public static readonly List<string> SurfOutBody_GBuffer = new List<string>()
        {
            "OUT.albedo = IN.Albedo;",
            "OUT.emission = IN.Emission;",
            "OUT.depth = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);",
            $"OUT.specular =  vec4({UniformNameDeclarations.SpecularStrength}, {UniformNameDeclarations.SpecularShininess}/256.0, 1.0, 1.0);"
        };

        /// <summary>
        /// Returns a default method body for a physically based diffuse-specular lighting calculation.
        /// </summary>
        public static readonly List<string> SurfOutBody_BRDF = new List<string>()
        {
            "OUT.albedo = IN.Albedo;",
            "OUT.emission = IN.Emission;",
            "OUT.roughness = IN.Roughness;",
            "OUT.metallic = IN.Metallic;",
            "OUT.ior = IN.IOR;",
            "OUT.specular = IN.Specular;",
            "OUT.subsurface = IN.Subsurface;"
        };

        /// <summary>
        /// Returns a default method body for a diffuse-specular lighting calculation that uses textures (albedo and normal).
        /// <param name="lightingSetup">The lighting setup on which basis the appropriate lighting parameters are chosen.</param>
        /// </summary>
        public static List<string> SurfOutBody_Textures(LightingSetupFlags lightingSetup)
        {
            var res = new List<string>();
            if (lightingSetup.HasFlag(LightingSetupFlags.DiffuseOnly) || lightingSetup.HasFlag(LightingSetupFlags.Glossy))
            {
                res.Add("OUT.roughness = IN.Roughness;");
            }
            else if (lightingSetup.HasFlag(LightingSetupFlags.DiffuseSpecular))
            {
                res.Add("OUT.specularStrength = IN.SpecularStrength;");
                res.Add("OUT.shininess = IN.Shininess;");
                res.Add("OUT.roughness = IN.Roughness;");
                res.Add("OUT.emission = IN.Emission;");
            }
            else if (lightingSetup.HasFlag(LightingSetupFlags.BRDF))
            {
                res.Add("OUT.roughness = IN.Roughness;");
                res.Add("OUT.metallic = IN.Metallic;");
                res.Add("OUT.ior = IN.IOR;");
                res.Add("OUT.specular = IN.Specular;");
                res.Add("OUT.subsurface = IN.Subsurface;");
                res.Add("OUT.subsurfaceColor = IN.SubsurfaceColor;");
                res.Add("OUT.emission = IN.Emission;");
            }

            if (lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex))
            {
                res.Add($"vec4 texCol = texture(IN.AlbedoTex, {VaryingNameDeclarations.TextureCoordinates} * IN.TexTiles);");
                res.Add($"texCol = vec4(DecodeSRGB(texCol.rgb), texCol.a);");
                res.Add("float linearLuminance = (0.2126 * texCol.r) + (0.7152 * texCol.g) + (0.0722 * texCol.b);");
                res.Add($"vec3 mix = mix(IN.Albedo.rgb * linearLuminance, texCol.xyz, IN.AlbedoMix);");
                res.Add($"OUT.albedo = vec4(mix, texCol.a);");
            }
            else
                res.Add("OUT.albedo = IN.Albedo;");

            if (lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
            {
                res.AddRange(new List<string>
                {
                    $"vec3 N = texture(IN.NormalTex, {VaryingNameDeclarations.TextureCoordinates} * IN.TexTiles).rgb;",
                    $"N = N * 2.0 - 1.0;",
                    $"N.xy *= IN.NormalMappingStrength;",
                    "OUT.normal = normalize(TBN * N);"
                });
            }
            return res;
        }
    }
}