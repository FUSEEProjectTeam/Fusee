using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    public struct LightingSetupShards
    {
        public string Name;
        public string StructDecl;
        public string DefaultInstance;
    }

    public enum LightingSetup
    {
        SpecularStd,
        SpecularPbr,
        DiffuseOnly,
        Unlit
    }

    public static class SurfaceOut
    {
        private static readonly Dictionary<LightingSetup, LightingSetupShards> _lightingSetupCache = new Dictionary<LightingSetup, LightingSetupShards>();

        private const string DiffuseOutName = "DiffuseOut";
        private static readonly string DefaultDiffuseOut = $"{DiffuseOutName}(vec4(0), vec3(0), vec4(0))";
        private static readonly string DiffuseOut = string.Join("\n", new List<string>()
        {
            $"struct {DiffuseOutName}",
            "{",
            "   vec4 position;",
            "   vec3 normal;",
            "   vec4 albedo;",
            "};\n",
        });


        private const string SpecularOutName = "SpecularOut";
        private static readonly string DefaultSpecOut = $"{SpecularOutName}(vec4(0), vec3(0), vec4(0), vec4(0), 0.0, 0.0)";
        private static readonly string SpecularOut = string.Join("\n", new List<string>()
        {
            $"struct {SpecularOutName}",
            "{",
            "   vec4 position;",
            "   vec3 normal;",
            "   vec4 albedo;",
            "   vec4 specularCol;",
            "   float specularStrength;",
            "   float shininess;",
            "};\n",
        });

        private const string SpecularPbrOutName = "SpecularOut";
        private static readonly string DerfafultSpecPbrOut = $"{SpecularOutName}(vec4(0), vec3(0), vec4(0), vec4(0), 0.0);";
        private static readonly string SpecularPbrOut = string.Join("\n", new List<string>()
        {
            $"struct {SpecularPbrOutName}",
            "{",
            "   vec4 position;",
            "   vec3 normal;",
            "   vec4 albedo;",
            "   vec4 specularCol;",
            "   float roughness;",
            "};\n",
        });

        public static LightingSetupShards GetLightingSetupShards(LightingSetup setup)
        {
            if (_lightingSetupCache.TryGetValue(setup, out var res))
                return res;
            switch (setup)
            {
                case LightingSetup.SpecularStd:
                    {
                        _lightingSetupCache[setup] = new LightingSetupShards()
                        {
                            Name = SpecularOutName,
                            StructDecl = SpecularOut,
                            DefaultInstance = DefaultSpecOut
                        };
                        return _lightingSetupCache[setup];

                    }
                case LightingSetup.SpecularPbr:
                    {
                        _lightingSetupCache[setup] = new LightingSetupShards()
                        {
                            Name = SpecularPbrOutName,
                            StructDecl = SpecularPbrOut,
                            DefaultInstance = DerfafultSpecPbrOut
                        };
                        return _lightingSetupCache[setup];

                    }
                case LightingSetup.Unlit:
                case LightingSetup.DiffuseOnly:
                    {
                        _lightingSetupCache[setup] = new LightingSetupShards()
                        {
                            Name = DiffuseOutName,
                            StructDecl = DiffuseOut,
                            DefaultInstance = DefaultDiffuseOut
                        };
                        return _lightingSetupCache[setup];
                    }

                default:
                    throw new ArgumentException($"Invalid argument: {setup}");
            }
        }
    }
}
