using Fusee.Engine.Core.Effects;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Container for a GLSL "surface out struct" declaration and default constructor.
    /// </summary>
    public struct LightingSetupShards
    {
        /// <summary>
        /// The struct declaration in GLSL.
        /// </summary>
        public string StructDecl;

        /// <summary>
        /// The default constructor in GLSL.
        /// </summary>
        public string DefaultInstance;
    }

    /// <summary>
    /// Used to create the correct Surface Effect for the given lighting parameters.
    /// </summary>
    public enum LightingSetupFlags
    {
        /// <summary>
        /// Does this Effect have an albedo texture?
        /// </summary>
        AlbedoTex = 32,

        /// <summary>
        /// Does this Effect have a normal map?
        /// </summary>
        NormalMap = 16,

        /// <summary>
        /// Does this Effect use the standard (= non pbr) specular calculation?
        /// </summary>
        SpecularStd = 8,

        /// <summary>
        /// Does this Effect use the pbr specular calculation?
        /// </summary>
        SpecularPbr = 4,

        /// <summary>
        /// Does this effect perform diffuse lighting calculation?
        /// </summary>
        Diffuse = 2,

        /// <summary>
        /// Does this effect perform no lighting calculation at all?
        /// </summary>
        Unlit = 1,
    }

    /// <summary>
    /// Contains all needed information to define the <see cref="SurfaceEffect.SurfaceOutput"/>.
    /// </summary>
    public static class SurfaceOut
    {
        /// <summary>
        /// The surface effects "out"-struct (<see cref="SurfaceEffect.SurfaceOutput"/>) always has this type in the shader code.
        /// </summary>
        public const string StructName = "SurfOut";

        /// <summary>
        /// The surface effects "out"-struct (<see cref="SurfaceEffect.SurfaceOutput"/>) always has this variable name in the shader code.
        /// </summary>
        public const string SurfOutVaryingName = "surfOut";

        internal static readonly string ChangeSurfFrag = "ChangeSurfFrag";

        #region Variables that can be changed in a Shader Shard
        internal static readonly Tuple<GLSL.Type, string> Pos = new Tuple<GLSL.Type, string>(GLSL.Type.Vec4, "position");
        internal static readonly Tuple<GLSL.Type, string> Normal = new Tuple<GLSL.Type, string>(GLSL.Type.Vec3, "normal");
        internal static readonly Tuple<GLSL.Type, string> Albedo = new Tuple<GLSL.Type, string>(GLSL.Type.Vec4, "albedo");
        internal static readonly Tuple<GLSL.Type, string> Shininess = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "shininess");
        internal static readonly Tuple<GLSL.Type, string> SpecularStrength = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "specularStrength");
        internal static readonly Tuple<GLSL.Type, string> Roughness = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "roughness");
        internal static readonly Tuple<GLSL.Type, string> Fresnel = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "fresnelReflect");
        internal static readonly Tuple<GLSL.Type, string> DiffuseFraction = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "diffuseFract");
        #endregion

        private static readonly Dictionary<LightingSetupFlags, LightingSetupShards> _lightingSetupCache = new Dictionary<LightingSetupFlags, LightingSetupShards>();

        private static readonly string DefaultDiffuseOut = $"{StructName}(vec4(0), vec3(0), vec4(0))";
        private static readonly string DefaultSpecOut = $"{StructName}(vec4(0), vec4(0), vec3(0), 0.0, 0.0)";
        private static readonly string DerfafultSpecPbrOut = $"{StructName}(vec4(0), vec4(0), vec3(0), 1.0, 1.0, 0.0)";

        /// <summary>
        /// Returns the GLSL default constructor and declaration of the <see cref="SurfaceEffect.SurfaceOutput"/> struct.
        /// </summary>
        /// <param name="setup">The <see cref="LightingSetupFlags"/> that decides what the appropriate struct is.</param>
        /// <returns></returns>
        public static LightingSetupShards GetLightingSetupShards(LightingSetupFlags setup)
        {
            if (_lightingSetupCache.TryGetValue(setup, out var res))
                return res;

            var structDcl = BuildStructDecl(setup);

            if (setup.HasFlag(LightingSetupFlags.SpecularStd))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultSpecOut
                };
                return _lightingSetupCache[setup];
            }
            else if (setup.HasFlag(LightingSetupFlags.SpecularPbr))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DerfafultSpecPbrOut
                };
                return _lightingSetupCache[setup];
            }
            else if (setup.HasFlag(LightingSetupFlags.Unlit) || setup.HasFlag(LightingSetupFlags.Diffuse))
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

        /// <summary>
        /// Returns the GLSL method that modifies the values of the <see cref="SurfaceEffect.SurfaceOutput"/> struct.
        /// </summary>
        /// <param name="methodBody">User-written shader code for modifying.</param>
        /// <param name="inputType">The type of the <see cref="SurfaceEffect.SurfaceInput"/> struct.</param>
        /// <returns></returns>
        public static string GetChangeSurfFragMethod(List<string> methodBody, Type inputType)
        {
            var bodyCompl = new List<string>()
            {
                $"{StructName} OUT = {SurfOutVaryingName};"
            };
            bodyCompl.AddRange(methodBody);
            bodyCompl.Add("return OUT;");
            return GLSL.CreateMethod(StructName, ChangeSurfFrag, new string[] { $"{inputType.Name} IN" }, bodyCompl);
        }

        private static string BuildStructDecl(LightingSetupFlags setup)
        {
            var dcl = new List<string>
            {
                $"struct {StructName}",
                "{",
                $"{GLSL.DecodeType(Pos.Item1)} {Pos.Item2};",
                $"{GLSL.DecodeType(Albedo.Item1)} {Albedo.Item2};",
            };

            if (!setup.HasFlag(LightingSetupFlags.Unlit))
                dcl.Add($"{GLSL.DecodeType(Normal.Item1)} {Normal.Item2};");

            if (setup.HasFlag(LightingSetupFlags.SpecularStd))
            {
                dcl.Add($"{GLSL.DecodeType(SpecularStrength.Item1)} {SpecularStrength.Item2};");
                dcl.Add($"{GLSL.DecodeType(Shininess.Item1)} {Shininess.Item2};");
            }
            else if (setup.HasFlag(LightingSetupFlags.SpecularPbr))
            {
                dcl.Add($"{GLSL.DecodeType(Roughness.Item1)} {Roughness.Item2};");
                dcl.Add($"{GLSL.DecodeType(Fresnel.Item1)} {Fresnel.Item2};");
                dcl.Add($"{GLSL.DecodeType(DiffuseFraction.Item1)} {DiffuseFraction.Item2};");
            }
            dcl.Add("};");
            return string.Join("\n", dcl);
        }
    }
}
