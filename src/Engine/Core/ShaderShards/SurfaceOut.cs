using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    public struct LightingSetupShards
    {
        public string StructDecl;
        public string DefaultInstance;
    }

    public enum LightingSetup
    {
        AlbedoTex = 32,
        NormalMap = 16,
        SpecularStd = 8,
        SpecularPbr = 4,
        Diffuse = 2,
        Unlit = 1,
    }

    public static class SurfaceOut
    {
        public const string StructName = "SurfOut";
        public static readonly string SurfOutVaryingName = $"surfOut";

        private static readonly Dictionary<LightingSetup, LightingSetupShards> _lightingSetupCache = new Dictionary<LightingSetup, LightingSetupShards>();

        internal static readonly Tuple<GLSL.Type, string> Pos = new Tuple<GLSL.Type, string>(GLSL.Type.Vec4, "position");
        internal static readonly Tuple<GLSL.Type, string> Normal = new Tuple<GLSL.Type, string>(GLSL.Type.Vec3, "normal");
        internal static readonly Tuple<GLSL.Type, string> Albedo = new Tuple<GLSL.Type, string>(GLSL.Type.Vec4, "albedo");
        internal static readonly Tuple<GLSL.Type, string> Shininess = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "shininess");
        internal static readonly Tuple<GLSL.Type, string> SpecularStrength = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "specularStrength");
        internal static readonly Tuple<GLSL.Type, string> Roughness = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "roughness");
        internal static readonly Tuple<GLSL.Type, string> Fresnel = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "fresnelReflect");
        internal static readonly Tuple<GLSL.Type, string> DiffuseFraction = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "diffuseFract");

        private static readonly string DefaultDiffuseOut = $"{StructName}(vec4(0), vec3(0), vec4(0))";
        private static readonly string DefaultSpecOut = $"{StructName}(vec4(0), vec4(0), vec3(0), 0.0, 0.0)";
        private static readonly string DerfafultSpecPbrOut = $"{StructName}(vec4(0), vec4(0), vec3(0), 1.0, 1.0, 0.0)";

        private static string BuildStructDecl(LightingSetup lightingSetup)
        {
            var dcl = new List<string>
            {
                $"struct {StructName}",
                "{",
                $"   {GLSL.DecodeType(Pos.Item1)} {Pos.Item2};",
                $"   {GLSL.DecodeType(Albedo.Item1)} {Albedo.Item2};",
            };

            if (!lightingSetup.HasFlag(LightingSetup.Unlit))
                dcl.Add($"   {GLSL.DecodeType(Normal.Item1)} {Normal.Item2};");

            if (lightingSetup.HasFlag(LightingSetup.SpecularStd))
            {
                dcl.Add($"   {GLSL.DecodeType(SpecularStrength.Item1)} {SpecularStrength.Item2};");
                dcl.Add($"   {GLSL.DecodeType(Shininess.Item1)} {Shininess.Item2};");
            }
            else if (lightingSetup.HasFlag(LightingSetup.SpecularPbr))
            {
                dcl.Add($"   {GLSL.DecodeType(Roughness.Item1)} {Roughness.Item2};");
                dcl.Add($"   {GLSL.DecodeType(Fresnel.Item1)} {Fresnel.Item2};");
                dcl.Add($"   {GLSL.DecodeType(DiffuseFraction.Item1)} {DiffuseFraction.Item2};");
            }
            dcl.Add("};");
            return string.Join("\n", dcl);
        }

        public static LightingSetupShards GetLightingSetupShards(LightingSetup setup)
        {
            if (_lightingSetupCache.TryGetValue(setup, out var res))
                return res;

            var structDcl = BuildStructDecl(setup);

            if (setup.HasFlag(LightingSetup.SpecularStd))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultSpecOut
                };
                return _lightingSetupCache[setup];
            }
            else if (setup.HasFlag(LightingSetup.SpecularPbr))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DerfafultSpecPbrOut
                };
                return _lightingSetupCache[setup];
            }
            else if (setup.HasFlag(LightingSetup.Unlit) || setup.HasFlag(LightingSetup.Diffuse))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultDiffuseOut
                };
                return _lightingSetupCache[setup];
            }
            else
            {
                throw new ArgumentException($"Invalid Lighting flags: {setup}");
            }
        }
    }
}
