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
        /// This effect is fully metallic by default - needs a roughness value.
        /// </summary>
        Glossy = 64,

        /// <summary>
        /// Does this Effect have an albedo texture?
        /// </summary>
        AlbedoTex = 32,

        /// <summary>
        /// Does this Effect have a normal map?
        /// </summary>
        NormalMap = 16,

        /// <summary>
        /// A Effect uses the standard (= non pbr) lighting calculation.
        /// </summary>
        DiffuseSpecular = 8,

        /// <summary>
        /// A Effect uses a pbr specular calculation (BRDF).
        /// Includes diffuse calculation.
        /// </summary>
        BRDF = 4,

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
    public sealed class SurfaceOut
    {
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static SurfaceOut() { }

        private SurfaceOut()
        {
            DefaultUnlitOut = $"{StructName}(vec4(0), vec4(0))";
            DefaultDiffuseOut = $"{StructName}(vec4(0), vec4(0), vec3(0), 0.0)";
            DefaultDiffSpecOut = $"{StructName}(vec4(0), vec4(0), vec4(0), vec3(0), 0.0, 0.0, 0.0)";
            DefaultGlossyOut = $"{StructName}(vec4(0), vec4(0), vec3(0), 0.0)";
            DerfafultBRDFOut = $"{StructName}(vec4(0), vec4(0), vec4(0), vec3(0), 0.0, 0.0, 0.0, 0.0, 0.0, vec3(1))";
        }

        public static SurfaceOut Instance => _instance;
        private static readonly SurfaceOut _instance = new();

        /// <summary>
        /// The surface effects "out"-struct (<see cref="SurfaceEffect.SurfaceOutput"/>) always has this type in the shader code.
        /// </summary>
        public readonly string StructName = "SurfOut";

        /// <summary>
        /// The surface effects "out"-struct (<see cref="SurfaceEffect.SurfaceOutput"/>) always has this variable name in the shader code.
        /// </summary>
        public readonly string SurfOutVaryingName = "surfOut";

        internal readonly string ChangeSurfFrag = "ChangeSurfFrag";
        internal readonly string ChangeSurfVert = "ChangeSurfVert";

        #region Variables that can be changed in a Shader Shard
        internal readonly Tuple<GLSL.Type, string> Pos = new(GLSL.Type.Vec4, "position");
        internal readonly Tuple<GLSL.Type, string> Normal = new(GLSL.Type.Vec3, "normal");
        internal readonly Tuple<GLSL.Type, string> Albedo = new(GLSL.Type.Vec4, "albedo");
        internal readonly Tuple<GLSL.Type, string> Emission = new(GLSL.Type.Vec4, "emission");
        internal readonly Tuple<GLSL.Type, string> Shininess = new(GLSL.Type.Float, "shininess");
        internal readonly Tuple<GLSL.Type, string> SpecularStrength = new(GLSL.Type.Float, "specularStrength");

        //BRDF only
        internal readonly Tuple<GLSL.Type, string> Roughness = new(GLSL.Type.Float, "roughness");
        internal readonly Tuple<GLSL.Type, string> Metallic = new(GLSL.Type.Float, "metallic");
        internal readonly Tuple<GLSL.Type, string> Specular = new(GLSL.Type.Float, "specular");
        internal readonly Tuple<GLSL.Type, string> IOR = new(GLSL.Type.Float, "ior");
        internal readonly Tuple<GLSL.Type, string> Subsurface = new(GLSL.Type.Float, "subsurface");
        internal readonly Tuple<GLSL.Type, string> SubsurfaceColor = new(GLSL.Type.Vec3, "subsurfaceColor");
        #endregion

        private readonly Dictionary<LightingSetupFlags, LightingSetupShards> _lightingSetupCache = new();

        private readonly string DefaultUnlitOut;
        private readonly string DefaultDiffuseOut;
        private readonly string DefaultDiffSpecOut;
        private readonly string DefaultGlossyOut;
        private readonly string DerfafultBRDFOut;

        /// <summary>
        /// Returns the GLSL default constructor and declaration of the <see cref="SurfaceEffect.SurfaceOutput"/> struct.
        /// </summary>
        /// <param name="setup">The <see cref="LightingSetupFlags"/> that decides what the appropriate struct is.</param>
        /// <returns></returns>
        public LightingSetupShards GetLightingSetupShards(LightingSetupFlags setup)
        {
            if (_lightingSetupCache.TryGetValue(setup, out var res))
                return res;

            var structDcl = BuildStructDecl(setup);

            if (setup.HasFlag(LightingSetupFlags.DiffuseSpecular))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultDiffSpecOut
                };
                return _lightingSetupCache[setup];
            }
            else if (setup.HasFlag(LightingSetupFlags.BRDF))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DerfafultBRDFOut
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
            else if (setup.HasFlag(LightingSetupFlags.Glossy))
            {
                _lightingSetupCache[setup] = new LightingSetupShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultGlossyOut
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
        public string GetChangeSurfFragMethod(List<string> methodBody, Type inputType)
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
        public string GetChangeSurfVertMethod(List<string> methodBody, LightingSetupFlags setup)
        {
            var bodyCompl = new List<string>()
            {
                $"{StructName} OUT = {_lightingSetupCache[setup].DefaultInstance};"
            };
            bodyCompl.AddRange(methodBody);
            bodyCompl.Add("return OUT;");
            return GLSL.CreateMethod(StructName, ChangeSurfVert, Array.Empty<string>(), bodyCompl);
        }

        private string BuildStructDecl(LightingSetupFlags setup)
        {
            var dcl = new List<string>
            {
                $"struct {StructName}",
                "{",
                $"  {GLSL.DecodeType(Pos.Item1)} {Pos.Item2};",
                $"  {GLSL.DecodeType(Albedo.Item1)} {Albedo.Item2};"
            };

            if (setup.HasFlag(LightingSetupFlags.DiffuseSpecular) || setup.HasFlag(LightingSetupFlags.BRDF))
                dcl.Add($"  {GLSL.DecodeType(Emission.Item1)} {Emission.Item2};");

            if (!setup.HasFlag(LightingSetupFlags.Unlit))
                dcl.Add($"  {GLSL.DecodeType(Normal.Item1)} {Normal.Item2};");

            if (setup.HasFlag(LightingSetupFlags.DiffuseOnly) || setup.HasFlag(LightingSetupFlags.DiffuseSpecular) || setup.HasFlag(LightingSetupFlags.Glossy))
                dcl.Add($"  {GLSL.DecodeType(Roughness.Item1)} {Roughness.Item2};");

            if (setup.HasFlag(LightingSetupFlags.DiffuseSpecular))
            {
                dcl.Add($"  {GLSL.DecodeType(SpecularStrength.Item1)} {SpecularStrength.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Shininess.Item1)} {Shininess.Item2};");
            }
            else if (setup.HasFlag(LightingSetupFlags.BRDF))
            {
                dcl.Add($"  {GLSL.DecodeType(Roughness.Item1)} {Roughness.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Metallic.Item1)} {Metallic.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Specular.Item1)} {Specular.Item2};");
                dcl.Add($"  {GLSL.DecodeType(IOR.Item1)} {IOR.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Subsurface.Item1)} {Subsurface.Item2};");
                dcl.Add($"  {GLSL.DecodeType(SubsurfaceColor.Item1)} {SubsurfaceColor.Item2};");
            }
            dcl.Add("};\n");
            return string.Join("\n", dcl);
        }
    }
}