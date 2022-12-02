﻿using Fusee.Engine.Core.Effects;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    /// <summary>
    /// Contains pre-defined Shader Shards =
    /// content of the Fragment Shader's method that lets us change values of the "out"-struct that is used for the lighting calculation.
    /// </summary>
    public static class FragShards
    {

        private static List<string> ColorModeSwitch() => new()
        {
            // TODO: Refactor with C#11
            "vec4 resColor;",
            $"switch({UniformNameDeclarations.ColorMode})",
            "{\n",
            "case 0:\n",
            "resColor = IN.Albedo;\n",
            "break;\n",
            "case 1:\n",
            $"resColor = {VaryingNameDeclarations.Color0};\n",
            "break;\n",
            "case 2:\n",
            $"resColor = {VaryingNameDeclarations.Color1};\n",
            "break;\n",
            "case 3:\n",
            $"resColor = {VaryingNameDeclarations.Color2};\n",
            "break;\n",
            "}"
        };


        /// <summary>
        /// Returns a default method body for a given lighting calculation.
        /// <param name="surfInput">The surface input class. Needed to receive the shading model and texture setup.</param>
        /// </summary>
        public static List<string> SurfOutBody(SurfaceEffectInput surfInput)
        {
            var res = new List<string>();

            res.AddRange(ColorModeSwitch());

            switch (surfInput.ShadingModel)
            {
                case ShadingModel.Edl:
                case ShadingModel.Unlit:
                    res.Add("OUT.emission = vec3(0);");
                    break;
                case ShadingModel.DiffuseSpecular:
                    res.Add("OUT.specularStrength = IN.SpecularStrength;");
                    res.Add("OUT.shininess = IN.Shininess;");
                    res.Add("OUT.roughness = IN.Roughness;");
                    res.Add("OUT.emission = IN.Emission;");
                    break;
                case ShadingModel.DiffuseOnly:
                    res.Add("OUT.emission = IN.Emission;");
                    break;
                case ShadingModel.Glossy:
                    res.Add("OUT.emission = vec3(0);");
                    res.Add("OUT.roughness = IN.Roughness;");
                    break;
                case ShadingModel.BRDF:
                    res.Add("OUT.roughness = IN.Roughness;");
                    res.Add("OUT.metallic = IN.Metallic;");
                    res.Add("OUT.ior = IN.IOR;");
                    res.Add("OUT.specular = IN.Specular;");
                    res.Add("OUT.subsurface = IN.Subsurface;");
                    res.Add("OUT.subsurfaceColor = IN.SubsurfaceColor;");
                    res.Add("OUT.emission = IN.Emission;");
                    res.Add("OUT.subsurfaceColor = IN.SubsurfaceColor;");
                    break;
                default:
                    throw new ArgumentException("Invalid ShadingModel!");
            }

            if (surfInput.TextureSetup.HasFlag(TextureSetup.AlbedoTex))
            {
                res.Add($"vec4 texCol = texture(IN.AlbedoTex, {VaryingNameDeclarations.TextureCoordinates} * IN.TexTiles);");
                res.Add($"texCol = vec4(DecodeSRGB(texCol.rgb), texCol.a);");
                res.Add("float linearLuminance = (0.2126 * texCol.r) + (0.7152 * texCol.g) + (0.0722 * texCol.b);");
                res.Add($"vec3 mix = mix(resColor.rgb * linearLuminance, texCol.xyz, IN.AlbedoMix);");
                res.Add($"OUT.albedo = vec4(mix, texCol.a);");
            }
            else
            {
                if (surfInput.ShadingModel != ShadingModel.Edl)
                    res.Add("OUT.albedo = resColor;");
                else
                {
                    res.Add("if(ColorMode == 0)");
                    res.Add("{");
                    res.Add($"   OUT.albedo = {VaryingNameDeclarations.Color0};");
                    res.Add("}");
                    res.Add("else");
                    res.Add("{");
                    res.Add("   OUT.albedo = resColor;");
                    res.Add("}");
                }
            }
            if (surfInput.TextureSetup.HasFlag(TextureSetup.NormalMap))
            {
                res.AddRange(new List<string>
                {
                    $"vec3 N = texture(IN.NormalTex, {VaryingNameDeclarations.TextureCoordinates} * IN.TexTiles).rgb;",
                    $"N = N * 2.0 - 1.0;",
                    $"N.xy *= IN.NormalMappingStrength;",
                    "OUT.normal = normalize(TBN * N);"
                });
            }

            if (surfInput.ShadingModel != ShadingModel.BRDF) return res;

            if (surfInput.TextureSetup.HasFlag(TextureSetup.ThicknessMap))
                res.Add($"OUT.{SurfaceOut.Thickness.Item2} = texture(IN.ThicknessMap, {VaryingNameDeclarations.TextureCoordinates}).r;");
            else
                res.Add($"OUT.{SurfaceOut.Thickness.Item2} = 1.0;");
            return res;
        }

        /// <summary>
        /// Returns a default method body for a given lighting calculation.
        /// </summary>
        public static List<string> SurfOutBody(ShadingModel shadingModel, TextureSetup texSetup)
        {
            var res = new List<string>();

            res.AddRange(ColorModeSwitch());

            switch (shadingModel)
            {
                case ShadingModel.Unlit:
                case ShadingModel.Edl:
                    break;
                case ShadingModel.DiffuseSpecular:
                    res.Add("OUT.specularStrength = IN.SpecularStrength;");
                    res.Add("OUT.shininess = IN.Shininess;");
                    res.Add("OUT.roughness = IN.Roughness;");
                    res.Add("OUT.emission = IN.Emission;");
                    break;
                case ShadingModel.DiffuseOnly:
                case ShadingModel.Glossy:
                    res.Add("OUT.roughness = IN.Roughness;");
                    break;
                case ShadingModel.BRDF:
                    res.Add("OUT.roughness = IN.Roughness;");
                    res.Add("OUT.metallic = IN.Metallic;");
                    res.Add("OUT.ior = IN.IOR;");
                    res.Add("OUT.specular = IN.Specular;");
                    res.Add("OUT.subsurface = IN.Subsurface;");
                    res.Add("OUT.subsurfaceColor = IN.SubsurfaceColor;");
                    res.Add("OUT.emission = IN.Emission;");
                    res.Add("OUT.subsurfaceColor = IN.SubsurfaceColor;");
                    break;
                default:
                    throw new ArgumentException("Invalid ShadingModel!");
            }

            if (texSetup.HasFlag(TextureSetup.AlbedoTex))
            {
                res.Add($"vec4 texCol = texture(IN.AlbedoTex, {VaryingNameDeclarations.TextureCoordinates} * IN.TexTiles);");
                res.Add($"texCol = vec4(DecodeSRGB(texCol.rgb), texCol.a);");
                res.Add("float linearLuminance = (0.2126 * texCol.r) + (0.7152 * texCol.g) + (0.0722 * texCol.b);");
                res.Add($"vec3 mix = mix(resColor.rgb * linearLuminance, texCol.xyz, IN.AlbedoMix);");
                res.Add($"OUT.albedo = vec4(mix, texCol.a);");
            }
            else
            {
                res.Add("OUT.albedo = resColor;");
            }

            if (texSetup.HasFlag(TextureSetup.NormalMap))
            {
                res.AddRange(new List<string>
                {
                    $"vec3 N = texture(IN.NormalTex, {VaryingNameDeclarations.TextureCoordinates} * IN.TexTiles).rgb;",
                    $"N = N * 2.0 - 1.0;",
                    $"N.xy *= IN.NormalMappingStrength;",
                    "OUT.normal = normalize(TBN * N);"
                });
            }

            if (shadingModel != ShadingModel.BRDF) return res;

            if (texSetup.HasFlag(TextureSetup.ThicknessMap))
                res.Add($"OUT.{SurfaceOut.Thickness.Item2} = texture(IN.ThicknessMap, {VaryingNameDeclarations.TextureCoordinates}).r;");
            else
                res.Add($"OUT.{SurfaceOut.Thickness.Item2} = 1.0;");
            return res;
        }
    }
}