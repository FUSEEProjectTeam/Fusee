using Fusee.Engine.Core.Effects;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Container for a "surface out struct" declaration and the suitable default constructor, written in shader language.
    /// </summary>
    public struct ShadingModelShards
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
    /// Used to tell if a SurfaceEffect has one or more of the defined textures.
    /// </summary>
    public enum TextureSetup
    {
        /// <summary>
        /// The effect dosn't have any textures
        /// </summary>
        NoTextures = 0,

        /// <summary>
        /// Does this Effect have an albedo texture?
        /// </summary>
        AlbedoTex = 1,

        /// <summary>
        /// Does this Effect have a normal map?
        /// </summary>
        NormalMap = 2,

        /// <summary>
        /// Does this Effect have a thickness map?
        /// </summary>
        ThicknessMap = 4

    }

    /// <summary>
    /// Used to create the correct Surface Effect for a given lighting calculation.
    /// </summary>
    public enum ShadingModel
    {
        /// <summary>
        /// Does this effect perform no lighting calculation at all?
        /// </summary>
        Unlit = 1,

        /// <summary>
        /// A Effect uses the standard (= non pbr) lighting calculation.
        /// </summary>
        DiffuseSpecular = 2,

        /// <summary>
        /// Perform only a simple diffuse calculation.
        /// </summary>
        DiffuseOnly = 4,

        /// <summary>
        /// This effect is fully metallic by default - needs a roughness value.
        /// </summary>
        Glossy = 8,

        /// <summary>
        /// A Effect uses a pbr specular calculation (BRDF).
        /// Includes diffuse calculation.
        /// </summary>
        BRDF = 16,

        /// <summary>
        /// This effect uses eye dome lighting and is used for point cloud rendering.
        /// CAUTION: it will only work with <see cref="SurfaceEffectBase"/>s that have the needed unirom paramters.
        /// See: <see cref="PointCloudSurfaceEffect.EDLStrength"/> and <see cref="PointCloudSurfaceEffect.EDLNeighbourPixels"/>
        /// </summary>
        Edl = 32
    }

    /// <summary>
    /// Contains all needed information to define the <see cref="SurfaceEffectBase.SurfaceOutput"/>.
    /// </summary>
    public static class SurfaceOut
    {
        /// <summary>
        /// The surface effects "out"-struct (<see cref="SurfaceEffectBase.SurfaceOutput"/>) always has this type in the shader code.
        /// </summary>
        public const string StructName = "SurfOut";

        /// <summary>
        /// The surface effects "out"-struct (<see cref="SurfaceEffectBase.SurfaceOutput"/>) always has this variable name in the shader code.
        /// </summary>
        public const string SurfOutVaryingName = "surfOut";

        internal static readonly string ChangeSurfFrag = "ChangeSurfFrag";
        internal static readonly string ChangeSurfVert = "ChangeSurfVert";

        #region Variables that can be changed in a Shader Shard
        internal static readonly Tuple<GLSL.Type, string> Pos = new(GLSL.Type.Vec3, "position");
        internal static readonly Tuple<GLSL.Type, string> Normal = new(GLSL.Type.Vec3, "normal");
        internal static readonly Tuple<GLSL.Type, string> Albedo = new(GLSL.Type.Vec4, "albedo");
        internal static readonly Tuple<GLSL.Type, string> Emission = new(GLSL.Type.Vec3, "emission");
        internal static readonly Tuple<GLSL.Type, string> Shininess = new(GLSL.Type.Float, "shininess");
        internal static readonly Tuple<GLSL.Type, string> SpecularStrength = new(GLSL.Type.Float, "specularStrength");

        //BRDF only
        internal static readonly Tuple<GLSL.Type, string> Roughness = new(GLSL.Type.Float, "roughness");
        internal static readonly Tuple<GLSL.Type, string> Metallic = new(GLSL.Type.Float, "metallic");
        internal static readonly Tuple<GLSL.Type, string> Specular = new(GLSL.Type.Float, "specular");
        internal static readonly Tuple<GLSL.Type, string> IOR = new(GLSL.Type.Float, "ior");
        internal static readonly Tuple<GLSL.Type, string> Subsurface = new(GLSL.Type.Float, "subsurface");
        internal static readonly Tuple<GLSL.Type, string> SubsurfaceColor = new(GLSL.Type.Vec3, "subsurfaceColor");
        internal static readonly Tuple<GLSL.Type, string> Thickness = new(GLSL.Type.Float, "thickness");
        #endregion

        private static readonly Dictionary<ShadingModel, ShadingModelShards> _shadingModelCache = new();

        private static readonly string DefaultUnlitOut = $"{StructName}(vec3(0), vec4(0))";
        private static readonly string DefaultDiffuseOut = $"{StructName}(vec3(0), vec4(0), vec3(0), 0.0)";
        private static readonly string DefaultDiffSpecOut = $"{StructName}(vec3(0), vec4(0), vec3(0), vec3(0), 0.0, 0.0, 0.0)";
        private static readonly string DefaultGlossyOut = $"{StructName}(vec3(0), vec4(0), vec3(0), 0.0)";
        private static readonly string DerfafultBRDFOut = $"{StructName}(vec3(0), vec4(0), vec3(0), vec3(0), 0.0, 0.0, 0.0, 0.0, 0.0, vec3(1), 0.0)";

        /// <summary>
        /// Returns the GLSL default constructor and declaration of the <see cref="SurfaceEffectBase.SurfaceOutput"/> struct.
        /// </summary>
        /// <param name="setup">The <see cref="ShadingModel"/> that decides what the appropriate struct is.</param>
        /// <returns></returns>
        public static ShadingModelShards GetShadingModelShards(ShadingModel setup)
        {
            if (_shadingModelCache.TryGetValue(setup, out var res))
                return res;

            var structDcl = BuildStructDecl(setup);

            if (setup.HasFlag(ShadingModel.DiffuseSpecular))
            {
                _shadingModelCache[setup] = new ShadingModelShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultDiffSpecOut
                };
                return _shadingModelCache[setup];
            }
            else if (setup.HasFlag(ShadingModel.BRDF))
            {
                _shadingModelCache[setup] = new ShadingModelShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DerfafultBRDFOut
                };
                return _shadingModelCache[setup];
            }
            else if (setup.HasFlag(ShadingModel.DiffuseOnly))
            {
                _shadingModelCache[setup] = new ShadingModelShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultDiffuseOut
                };
                return _shadingModelCache[setup];
            }
            else if (setup.HasFlag(ShadingModel.Glossy))
            {
                _shadingModelCache[setup] = new ShadingModelShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultGlossyOut
                };
                return _shadingModelCache[setup];
            }
            else if (setup.HasFlag(ShadingModel.Unlit) || setup.HasFlag(ShadingModel.Edl))
            {
                _shadingModelCache[setup] = new ShadingModelShards()
                {
                    StructDecl = structDcl,
                    DefaultInstance = DefaultUnlitOut
                };
                return _shadingModelCache[setup];
            }
            else
            {
                throw new ArgumentException($"Invalid Lighting flags: {setup}");
            }
        }

        /// <summary>
        /// Returns the GLSL method that modifies the values of the <see cref="SurfaceEffectBase.SurfaceOutput"/> struct.
        /// </summary>
        /// <param name="methodBody">User-written shader code for modifying.</param>
        /// <param name="inputType">The type of the <see cref="SurfaceEffectBase.SurfaceInput"/> struct.</param>
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
        /// Returns the GLSL method that modifies the values of the <see cref="SurfaceEffectBase.SurfaceOutput"/> struct.
        /// </summary>
        /// <param name="methodBody">User-written shader code for modifying.</param>
        /// <param name="setup">The lighting setup.</param>
        /// <returns></returns>
        public static string GetChangeSurfVertMethod(List<string> methodBody, ShadingModel setup)
        {
            var bodyCompl = new List<string>()
            {
                $"{StructName} OUT = {_shadingModelCache[setup].DefaultInstance};"
            };
            bodyCompl.AddRange(methodBody);
            bodyCompl.Add("return OUT;");
            return GLSL.CreateMethod(StructName, ChangeSurfVert, Array.Empty<string>(), bodyCompl);
        }

        private static string BuildStructDecl(ShadingModel setup)
        {
            var dcl = new List<string>
            {
                $"struct {StructName}",
                "{",
                $"  {GLSL.DecodeType(Pos.Item1)} {Pos.Item2};",
                $"  {GLSL.DecodeType(Albedo.Item1)} {Albedo.Item2};"
            };

            if (setup.HasFlag(ShadingModel.DiffuseSpecular) || setup.HasFlag(ShadingModel.BRDF))
                dcl.Add($"  {GLSL.DecodeType(Emission.Item1)} {Emission.Item2};");

            if (!setup.HasFlag(ShadingModel.Unlit) && !setup.HasFlag(ShadingModel.Edl))
                dcl.Add($"  {GLSL.DecodeType(Normal.Item1)} {Normal.Item2};");

            if (setup.HasFlag(ShadingModel.DiffuseOnly) || setup.HasFlag(ShadingModel.DiffuseSpecular) || setup.HasFlag(ShadingModel.Glossy))
                dcl.Add($"  {GLSL.DecodeType(Roughness.Item1)} {Roughness.Item2};");

            if (setup.HasFlag(ShadingModel.DiffuseSpecular))
            {
                dcl.Add($"  {GLSL.DecodeType(SpecularStrength.Item1)} {SpecularStrength.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Shininess.Item1)} {Shininess.Item2};");
            }
            else if (setup.HasFlag(ShadingModel.BRDF))
            {
                dcl.Add($"  {GLSL.DecodeType(Roughness.Item1)} {Roughness.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Metallic.Item1)} {Metallic.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Specular.Item1)} {Specular.Item2};");
                dcl.Add($"  {GLSL.DecodeType(IOR.Item1)} {IOR.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Subsurface.Item1)} {Subsurface.Item2};");
                dcl.Add($"  {GLSL.DecodeType(SubsurfaceColor.Item1)} {SubsurfaceColor.Item2};");
                dcl.Add($"  {GLSL.DecodeType(Thickness.Item1)} {Thickness.Item2};");
            }
            dcl.Add("};\n");
            return string.Join("\n", dcl);
        }
    }
}