using Fusee.Engine.Core.Effects;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Container for a "surface out struct" declaration and the suitable default constructor, written in shader language.
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
        AlbedoTex = 64,

        /// <summary>
        /// Does this Effect have a normal map?
        /// </summary>
        NormalMap = 32,

        /// <summary>
        /// A Effect uses the standard (= non pbr) lighting calculation.
        /// Includes diffuse calculation.
        /// </summary>
        Lambert = 16,

        /// <summary>
        /// A Effect uses a pbr specular calculation (BRDF - metallic setup).
        /// Includes diffuse calculation.
        /// </summary>
        BRDFMetallic = 8,

        /// <summary>
        /// A Effect uses a pbr specular calculation (BRDF - subsurface setup).
        /// Includes diffuse calculation.
        /// TODO: subsurface shading model is not properly supported yet. 
        /// The differentiation into Metallic in Subsurface is mainly caused be wish to store the BRDF values in only one texture.
        /// </summary>
        BRDFSubsurface = 4,

        /// <summary>
        /// Perform only a simple diffuse calculation.
        /// </summary>
        DiffuseOnly = 2,

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
        internal static readonly string ChangeSurfVert = "ChangeSurfVert";

        #region Variables that can be changed in a Shader Shard
        internal static readonly Tuple<GLSL.Type, string> Pos = new Tuple<GLSL.Type, string>(GLSL.Type.Vec4, "position");
        internal static readonly Tuple<GLSL.Type, string> Normal = new Tuple<GLSL.Type, string>(GLSL.Type.Vec3, "normal");
        internal static readonly Tuple<GLSL.Type, string> Albedo = new Tuple<GLSL.Type, string>(GLSL.Type.Vec4, "albedo");
        internal static readonly Tuple<GLSL.Type, string> Shininess = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "shininess");
        internal static readonly Tuple<GLSL.Type, string> SpecularStrength = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "specularStrength");

        //BRDF only
        internal static readonly Tuple<GLSL.Type, string> Roughness = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "roughness");
        internal static readonly Tuple<GLSL.Type, string> Metallic = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "metallic");
        internal static readonly Tuple<GLSL.Type, string> Specular = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "specular");
        internal static readonly Tuple<GLSL.Type, string> IOR = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "ior");
        internal static readonly Tuple<GLSL.Type, string> Subsurface = new Tuple<GLSL.Type, string>(GLSL.Type.Float, "subsurface");
        #endregion

        private static readonly Dictionary<LightingSetupFlags, LightingSetupShards> _lightingSetupCache = new Dictionary<LightingSetupFlags, LightingSetupShards>();

        private static readonly string DefaultUnlitOut = $"{StructName}(vec4(0), vec4(0))";
        private static readonly string DefaultDiffuseOut = $"{StructName}(vec4(0), vec4(0), vec3(0))";
        private static readonly string DefaultSpecOut = $"{StructName}(vec4(0), vec4(0), vec3(0), 0.0, 0.0)";
        private static readonly string DerfafultSpecBRDFOut = $"{StructName}(vec4(0), vec4(0), vec3(0), 0.0, 0.0, 0.0, 0.0, 0.0)";

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

            if (setup.HasFlag(LightingSetupFlags.Lambert))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultSpecOut
                };
                return _lightingSetupCache[setup];
            }
            else if (setup.HasFlag(LightingSetupFlags.BRDFMetallic))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DerfafultSpecBRDFOut
                };
                return _lightingSetupCache[setup];
            }
            else if (setup.HasFlag(LightingSetupFlags.DiffuseOnly))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultDiffuseOut
                };
                return _lightingSetupCache[setup];
            }
            else if (setup.HasFlag(LightingSetupFlags.Unlit))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultUnlitOut
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

        /// <summary>
        /// Returns the GLSL method that modifies the values of the <see cref="SurfaceEffect.SurfaceOutput"/> struct.
        /// </summary>
        /// <param name="methodBody">User-written shader code for modifying.</param>
        /// <param name="setup">The lighting setup.</param>
        /// <returns></returns>
        public static string GetChangeSurfVertMethod(List<string> methodBody, LightingSetupFlags setup)
        {
            var bodyCompl = new List<string>()
            {
                $"{StructName} OUT = {_lightingSetupCache[setup].DefaultInstance};"
            };
            bodyCompl.AddRange(methodBody);
            bodyCompl.Add("return OUT;");
            return GLSL.CreateMethod(StructName, ChangeSurfVert, new string[] {}, bodyCompl);
        }

        private static string BuildStructDecl(LightingSetupFlags setup)
        {
            var dcl = new List<string>
            {
                $"struct {StructName}",
                "{",
                $"  {GLSL.DecodeType(Pos.Item1)} {Pos.Item2};",
                $"  {GLSL.DecodeType(Albedo.Item1)} {Albedo.Item2};",
            };

            if (!setup.HasFlag(LightingSetupFlags.Unlit))
                dcl.Add($"  {GLSL.DecodeType(Normal.Item1)} {Normal.Item2};");

            if (setup.HasFlag(LightingSetupFlags.Lambert))
            {
                dcl.Add($"  {GLSL.DecodeType(SpecularStrength.Item1)} {SpecularStrength.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Shininess.Item1)} {Shininess.Item2};");
            }
            else if (setup.HasFlag(LightingSetupFlags.BRDFMetallic))
            {
                dcl.Add($"  {GLSL.DecodeType(Roughness.Item1)} {Roughness.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Metallic.Item1)} {Metallic.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Specular.Item1)} {Specular.Item2};");
                dcl.Add($"  {GLSL.DecodeType(IOR.Item1)} {IOR.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Subsurface.Item1)} {Subsurface.Item2};");
            }
            dcl.Add("};\n");
            return string.Join("\n", dcl);
        }
    }
}
