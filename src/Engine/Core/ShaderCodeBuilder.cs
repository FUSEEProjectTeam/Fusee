using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    internal struct MeshProbs
    {
        public bool HasVertices;
        public bool HasNormals;
        public bool HasUVs;
        public bool HasColors;
        public bool HasWeightMap;
        public bool HasTangents;
        public bool HasBiTangents;
    }

    internal struct MaterialProbs
    {
        public bool HasDiffuse;
        public bool HasDiffuseTexture;
        public bool HasSpecular;
        public bool HasSpecularTexture;
        public bool HasEmissive;
        public bool HasEmissiveTexture;
        public bool HasBump;
        public bool HasApplyLightString;
    }

    internal enum MaterialType
    {
        Material,
        MaterialLightComponent,
        MaterialPbrComponent
    }

    internal enum Type
    {
        Mat3,
        Mat4,
        Vec2,
        Vec3,
        Vec4,
        Boolean,
        Float,
        Int,
        Sampler2D,
        Void
    }

    // ReSharper disable once InconsistentNaming
    internal class GLSL
    {
        public static string CreateUniform(Type type, string varName)
        {
            return $"uniform {DecodeType(type)} {varName};";
        }

        public static string CreateOut(Type type, string varName)
        {
            return $"out {DecodeType(type)} {varName};";
        }

        public static string CreateIn(Type type, string varName)
        {
            return $"in  {DecodeType(type)} {varName};";
        }

        public static string CreateVar(Type type, string varName)
        {
            return $"{DecodeType(type)} {varName}";
        }

        /// <summary>
        /// Creates a GLSL method
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="methodName"></param>
        /// <param name="methodParams"></param>
        /// <param name="method">method body goes here</param>
        /// <returns></returns>
        public static string CreateMethod(Type returnType, string methodName, string[] methodParams,
            IList<string> method)
        {
            method = method.Select(x => "   " + x).ToList(); // One Tab indent

            var tmpList = new List<string>
            {
                $"{DecodeType(returnType)} {methodName}({string.Join(", ", methodParams)})",
                "{"
            };
            tmpList.AddRange(method);
            tmpList.Add("}");

            return string.Join("\n", tmpList);
        }

        private static string DecodeType(Type type)
        {
            switch (type)
            {
                case Type.Mat3:
                    return "mat3";
                case Type.Mat4:
                    return "mat4";
                case Type.Vec2:
                    return "vec2";
                case Type.Vec3:
                    return "vec3";
                case Type.Vec4:
                    return "vec4";
                case Type.Boolean:
                    return "bool";
                case Type.Float:
                    return "float";
                case Type.Int:
                    return "int";
                case Type.Sampler2D:
                    return "sampler2D";
                case Type.Void:
                    return "void";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    /// <summary>
    /// Compiler for ShaderCode. Takes a MaterialComponent, evaluates input parameters and creates pixel and vertex shader
    /// </summary>
    public class ShaderCodeBuilder
    {
        private readonly LightingCalculationMethod _lightingCalculationMethod;

        private MaterialProbs _materialProbs;
        private MeshProbs _meshProbs;
        private MaterialType _materialType = MaterialType.Material;
        private List<string> _vertexShader;
        private List<string> _pixelShader;
        private readonly bool _renderWithShadows;

        //The maximal number of lights we can render when using the forward pipeline.
        private const int _numberOfLightsForward = 8;

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// The complete VertexShader
        /// </summary>
        public string VS { get; }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// The complete pixel shader
        /// </summary>
        public string PS { get; }

        /// <summary>
        /// LEGACY CONSTRUCTOR
        /// Creates vertex and pixel shader for given material, mesh, weight; light calculation is simple per default
        /// </summary>
        /// <param name="mc">The MaterialCpmponent</param>
        /// <param name="mesh">The Mesh</param>
        /// <param name="wc">Teh WeightComponent</param>
        /// <param name="renderWithShadows">Should the resulting shader include shadow calculation</param>        
        public ShaderCodeBuilder(MaterialComponent mc, Mesh mesh, WeightComponent wc = null,
            bool renderWithShadows = false)
            : this(mc, mesh, LightingCalculationMethod.SIMPLE, wc, renderWithShadows)
        {
        }

        public static Dictionary<string, string> GetFieldValues(object obj)
        {
            return obj.GetType()
                      .GetProperties(BindingFlags.Public | BindingFlags.Static)
                      .Where(f => f.PropertyType == typeof(string))
                      .ToDictionary(f => f.Name,
                                    f => (string)f.GetValue(null));
        }

        private string ParseIncludes(string rawShaderString)
        {
            var fields = GetFieldValues(this);

            var refinedShader = new List<string>();

            var allLines = rawShaderString.Split(new[] { '\r', '\n' });
            foreach (var line in allLines)
            {
                // if we have one or more includes we need to replace them                
                if (line.Contains("#include"))
                {
                    var includeLine = line.Replace(@"#include ", string.Empty);
                    includeLine = includeLine.Replace("\"", string.Empty);
                    var fileFromInclude = includeLine;
                    var foundFile = "";
                    try
                    {
                        foundFile = AssetStorage.Get<string>("Assets/Shader/" + fileFromInclude);
                        if (foundFile == null)
                            throw new FileNotFoundException(foundFile);
                    }
                    catch (FileNotFoundException e)
                    {
                        Diagnostics.Log($"[ShaderCodeBuilder.cs] Error file #include {e.FileName} not found!");
                    }
                    refinedShader.Add(foundFile);
                }
                else
                {
                    refinedShader.Add(line);
                }
            }


            // replace all names
            foreach (var field in fields)
            {
                for (var i = 0; i < refinedShader.Count; i++)
                {
                    if (refinedShader[i].Contains(field.Key))
                    {
                        refinedShader[i] = refinedShader[i].Replace(field.Key, field.Value);
                    }
                }
            }

            return String.Join("\n", refinedShader);
        }

        /// <summary>
        /// Creates vertex and pixel shader for given material, mesh, weight; light calculation is simple per default
        /// </summary>
        /// <param name="mc">The MaterialCpmponent</param>
        /// <param name="mesh">The Mesh</param>
        /// <param name="wc">The WeightComponent</param>
        /// <param name="lightingCalculation">Method of light calculation; simple BLINN PHONG or advanced physically based</param>
        /// <param name="renderWithShadows">Should the resulting shader include shadow calculation</param>        
        public ShaderCodeBuilder(MaterialComponent mc, Mesh mesh,
            LightingCalculationMethod lightingCalculation = LightingCalculationMethod.SIMPLE,
            WeightComponent wc = null, bool renderWithShadows = false)
        {
            // Set Lighting calculation & shadow
            _lightingCalculationMethod = lightingCalculation;
            _renderWithShadows = renderWithShadows;

            _vertexShader = new List<string>();
            _pixelShader = new List<string>();
            /* 
            // here we read and parse out vert and pixel shader
            var pixelShaderRaw = AssetStorage.Get<string>("Assets/Shader/PixelShader.frag"); 
            var vertexShaderRaw = AssetStorage.Get<string>("Assets/Shader/VertexShader.vert");

            VS = ParseIncludes(vertexShaderRaw);
            PS = ParseIncludes(pixelShaderRaw);
            */

            AnalyzeMaterialType(mc);
            AnalyzeMesh(mesh, wc);
            AnalzyeMaterialParams(mc);
            CreateVertexShader(wc);
            VS = string.Join("\n", _vertexShader);
            CreatePixelShader_new(mc);
            PS = string.Join("\n", _pixelShader);

            // Uber Shader - test purposes!
            //VS = AssetStorage.Get<string>("Shader/UberVertex.vert");
            //PS = AssetStorage.Get<string>("Shader/UberFragment.frag");

            //Diagnostics.Log(PS);
            //Diagnostics.Log(VS);
        }

        private static void AddTabsToMethods(ref List<string> list)
        {
            var indent = false;
            for (var i = 0; i < list.Count; i++)
            {
                var s = list[i];
                if (list[i].Contains("}"))
                    break;

                if (indent)
                    list[i] = "   " + s;

                if (list[i].Contains("{"))
                    indent = true;
            }
        }

        #region AnalyzeMaterialParams


        private void AnalzyeMaterialParams(MaterialComponent mc)
        {
            _materialProbs = new MaterialProbs
            {
                HasDiffuse = mc.HasDiffuse,
                HasDiffuseTexture = mc.HasDiffuse && mc.Diffuse.Texture != null,
                HasSpecular = mc.HasSpecular,
                HasSpecularTexture = mc.HasSpecular && mc.Specular.Texture != null,
                HasEmissive = mc.HasEmissive,
                HasEmissiveTexture = mc.HasEmissive && mc.Emissive.Texture != null,
                HasBump = mc.HasBump,
                HasApplyLightString = _materialType == MaterialType.MaterialLightComponent &&
                                      (string.IsNullOrEmpty((mc as MaterialLightComponent)?.ApplyLightString))

            };
        }

        private void AnalyzeMaterialType(MaterialComponent mc)
        {
            if (mc.GetType() == typeof(MaterialPBRComponent))
                _materialType = MaterialType.MaterialPbrComponent;

            if (mc.GetType() == typeof(MaterialLightComponent))
                _materialType = MaterialType.MaterialLightComponent;
        }


        private void AnalyzeMesh(Mesh mesh, WeightComponent wc = null)
        {
            _meshProbs = new MeshProbs
            {
                HasVertices = mesh == null || mesh.Vertices != null && mesh.Vertices.Length > 0, // if no mesh => true
                HasNormals = mesh == null || mesh.Normals != null && mesh.Normals.Length > 0,
                HasUVs = mesh == null || mesh.UVs != null && mesh.UVs.Length > 0,
                HasColors = false,
                HasWeightMap = wc != null,
                HasTangents = mesh == null || (mesh.Tangents != null && mesh.Tangents.Length > 1),
                HasBiTangents = mesh == null || (mesh.BiTangents != null && mesh.BiTangents.Length > 1)
            };
        }

        #endregion

        #region CreateVertexShader

        private void CreateVertexShader(WeightComponent wc)
        {
            // Version
            _vertexShader.Add(Version());

            // Head
            AddVertexAttributes(wc);
            AddVertexUniforms(wc);

            // Main
            AddVertexMain();

            AddTabsToMethods(ref _vertexShader);
        }


        private void AddVertexAttributes(WeightComponent wc)
        {
            if (_meshProbs.HasWeightMap)
                _vertexShader.Add($"#define BONES {wc.Joints.Count}");

            if (_meshProbs.HasVertices)
                _vertexShader.Add(GLSL.CreateIn(Type.Vec3, "fuVertex"));

            if (_meshProbs.HasTangents && _meshProbs.HasBiTangents)
            {
                _vertexShader.Add(GLSL.CreateIn(Type.Vec4, ShaderCodeBuilderHelper.TangentAttribName));
                _vertexShader.Add(GLSL.CreateIn(Type.Vec3, ShaderCodeBuilderHelper.BitangentAttribName));

                _vertexShader.Add(GLSL.CreateOut(Type.Vec4, "vT"));
                _vertexShader.Add(GLSL.CreateOut(Type.Vec3, "vB"));
            }


            if (_materialProbs.HasSpecular)
                _vertexShader.Add(GLSL.CreateOut(Type.Vec3, "vViewDir"));

            if (_meshProbs.HasWeightMap)
            {
                _vertexShader.Add(GLSL.CreateIn(Type.Vec4, "fuBoneIndex"));
                _vertexShader.Add(GLSL.CreateIn(Type.Vec4, "fuBoneWeight"));
            }

            if (_meshProbs.HasNormals)
            {
                _vertexShader.Add(GLSL.CreateIn(Type.Vec3, "fuNormal"));
                _vertexShader.Add(GLSL.CreateOut(Type.Vec3, "vNormal"));
            }

            if (_meshProbs.HasUVs)
            {
                _vertexShader.Add(GLSL.CreateIn(Type.Vec2, "fuUV"));
                _vertexShader.Add(GLSL.CreateOut(Type.Vec2, "vUV"));
            }

            if (_meshProbs.HasColors)
            {
                _vertexShader.Add(GLSL.CreateIn(Type.Vec4, "fuColor"));
                _vertexShader.Add(GLSL.CreateOut(Type.Vec4, "vColors"));
            }

            _vertexShader.Add(GLSL.CreateOut(Type.Vec3, "camPos"));
            _vertexShader.Add(GLSL.CreateOut(Type.Vec3, "vMVNormal"));
            _vertexShader.Add(GLSL.CreateOut(Type.Vec3, "vViewPos"));

            if (_renderWithShadows)
                _vertexShader.Add(GLSL.CreateOut(Type.Vec4, "shadowLight"));

        }

        private void AddVertexUniforms(WeightComponent wc)
        {
            _vertexShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_MVP"));

            if (_meshProbs.HasNormals)
                _vertexShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_ITMV"));

            if (_materialProbs.HasSpecular && !_meshProbs.HasWeightMap)
                _vertexShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_IMV"));

            if (_meshProbs.HasWeightMap)
            {
                _vertexShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_V"));
                _vertexShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_P"));
                _vertexShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_IMV"));
                _vertexShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_BONES[BONES]"));
            }

            _vertexShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_MV"));

            if (_renderWithShadows)
                _vertexShader.Add(GLSL.CreateUniform(Type.Mat4, "shadowMVP"));
        }

        private void AddVertexMain()
        {
            // Main
            _vertexShader.Add("void main() {");

            _vertexShader.Add("gl_PointSize = 10.0;");

            if (_meshProbs.HasNormals && _meshProbs.HasWeightMap)
            {
                _vertexShader.Add("vec4 newVertex;");
                _vertexShader.Add("vec4 newNormal;");
                _vertexShader.Add(
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuVertex, 1.0) ) * fuBoneWeight.x ;");
                _vertexShader.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuNormal, 0.0)) * fuBoneWeight.x;");
                _vertexShader.Add(
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuVertex, 1.0)) * fuBoneWeight.y + newVertex;");
                _vertexShader.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuNormal, 0.0)) * fuBoneWeight.y + newNormal;");
                _vertexShader.Add(
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuVertex, 1.0)) * fuBoneWeight.z + newVertex;");

                _vertexShader.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuNormal, 0.0)) * fuBoneWeight.z + newNormal;");
                _vertexShader.Add(
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuVertex, 1.0)) * fuBoneWeight.w + newVertex;");
                _vertexShader.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuNormal, 0.0)) * fuBoneWeight.w + newNormal;");

                // At this point the normal is in World space - transform back to model space                
                _vertexShader.Add("vMVNormal = mat3(FUSEE_ITMV) * newNormal.xyz;");
            }

            if (_materialProbs.HasSpecular)
            {
                _vertexShader.Add("vec3 camPos = FUSEE_IMV[3].xyz;");

                _vertexShader.Add(_meshProbs.HasWeightMap
                    ? "vViewDir = normalize(camPos - vec3(newVertex));"
                    : "vViewDir = normalize(camPos - fuVertex);");
            }

            if (_meshProbs.HasUVs)
                _vertexShader.Add("vUV = fuUV;");

            if (_meshProbs.HasNormals && !_meshProbs.HasWeightMap)
                _vertexShader.Add("vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);");

            _vertexShader.Add("vViewPos = (FUSEE_MV * vec4(fuVertex, 1.0)).xyz;");

            if (_renderWithShadows)
                _vertexShader.Add("shadowLight = shadowMVP * vViewPos;");

            if (_meshProbs.HasTangents && _meshProbs.HasBiTangents)
            {
                _vertexShader.Add($"vT = {ShaderCodeBuilderHelper.TangentAttribName};");
                _vertexShader.Add($"vB = {ShaderCodeBuilderHelper.BitangentAttribName};");
            }

            _vertexShader.Add(_meshProbs.HasWeightMap
            ? "gl_Position = FUSEE_MVP * vec4(vec3(newVertex), 1.0);"
            : "gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);");

            // End of main
            _vertexShader.Add("}");
        }

        #endregion

        #region CreatePixelShader

        private void CreatePixelShader_new(MaterialComponent mc)
        {
            _pixelShader.Add(Version());

            AddPixelAttributes();
            AddPixelUniforms();
            AddTextureChannels();

            switch (_materialType)
            {
                case MaterialType.Material:
                case MaterialType.MaterialLightComponent:
                    AddAmbientLightMethod();
                    if (_materialProbs.HasDiffuse)
                        AddDiffuseLightMethod();
                    if (_materialProbs.HasSpecular)
                        AddSpecularLightMethod();
                    break;
                case MaterialType.MaterialPbrComponent:
                    if (_lightingCalculationMethod != LightingCalculationMethod.ADVANCED)
                    {
                        AddAmbientLightMethod();
                        if (_materialProbs.HasDiffuse)
                            AddDiffuseLightMethod();
                        if (_materialProbs.HasSpecular)
                            AddSpecularLightMethod();
                    }
                    else
                    {
                        AddAmbientLightMethod();
                        if (_materialProbs.HasDiffuse)
                            AddDiffuseLightMethod();
                        if (_materialProbs.HasSpecular)
                            AddPbrSpecularLightMethod(mc as MaterialPBRComponent);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Material Type unknown or incorrect: {_materialType}");
            }

            if (_renderWithShadows)
                AddShadowMethod();

            AddApplyLightMethod(mc);
            AddPixelBody();

            AddTabsToMethods(ref _pixelShader);

            //Diagnostics.Log(string.Join("\n", _pixelShader));

        }

        private void AddPixelAttributes()
        {
            _pixelShader.Add(EsPrecision());

            // legacy code, should be larger one by default!            
            _pixelShader.Add(LightStructDeclaration());

            _pixelShader.Add(GLSL.CreateIn(Type.Vec3, "vViewDir"));
            _pixelShader.Add(GLSL.CreateIn(Type.Vec3, "vViewPos"));

            if (_meshProbs.HasNormals)
            {
                _pixelShader.Add(GLSL.CreateIn(Type.Vec3, "vMVNormal"));
                _pixelShader.Add(GLSL.CreateIn(Type.Vec3, "vNormal"));
            }
            if (_meshProbs.HasTangents && _meshProbs.HasBiTangents)
            {
                _pixelShader.Add(GLSL.CreateIn(Type.Vec4, "vT"));
                _pixelShader.Add(GLSL.CreateIn(Type.Vec3, "vB"));

            }


            if (_meshProbs.HasUVs)
                _pixelShader.Add(GLSL.CreateIn(Type.Vec2, "vUV"));

            _pixelShader.Add(GLSL.CreateIn(Type.Vec3, "camPos"));

            if (_renderWithShadows)
                _pixelShader.Add(GLSL.CreateIn(Type.Vec4, "shadowLight"));

            _pixelShader.Add(GLSL.CreateOut(Type.Vec4, "fragmentColor"));

        }

        private void AddPixelUniforms()
        {
            _pixelShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_MV"));
            _pixelShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_IMV"));
            _pixelShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_IV"));
            _pixelShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_V"));

            if (_materialProbs.HasBump)
                _pixelShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_ITMV"));

            // Multipass
            _pixelShader.Add(GLSL.CreateUniform(Type.Sampler2D, "firstPassTex"));

            // Multipass-Env
            // returnString += "uniform samplerCube envMap;\n";
        }

        private void AddTextureChannels()
        {
            if (_materialProbs.HasSpecular)
            {
                _pixelShader.Add(GLSL.CreateUniform(Type.Float, SpecularShininessName));
                _pixelShader.Add(GLSL.CreateUniform(Type.Float, SpecularIntensityName));
                _pixelShader.Add(GLSL.CreateUniform(Type.Vec4, SpecularColorName));
            }

            if (_materialProbs.HasBump)
            {
                _pixelShader.Add(GLSL.CreateUniform(Type.Sampler2D, BumpTextureName));
                _pixelShader.Add(GLSL.CreateUniform(Type.Float, BumpIntensityName));
            }

            if (_materialProbs.HasDiffuse)
                _pixelShader.Add(GLSL.CreateUniform(Type.Vec4, DiffuseColorName));

            if (_materialProbs.HasDiffuseTexture)
            {
                _pixelShader.Add(GLSL.CreateUniform(Type.Sampler2D, DiffuseTextureName));
                _pixelShader.Add(GLSL.CreateUniform(Type.Float, DiffuseMixName));
            }

            if (_materialProbs.HasEmissive)
                _pixelShader.Add(GLSL.CreateUniform(Type.Vec4, EmissiveColorName));

            if (_materialProbs.HasEmissiveTexture)
            {
                _pixelShader.Add(GLSL.CreateUniform(Type.Sampler2D, EmissiveTextureName));
                _pixelShader.Add(GLSL.CreateUniform(Type.Float, EmissiveMixName));
            }
        }

        private void AddAmbientLightMethod()
        {
            var methodBody = new List<string>
            {

                "return vec4(DiffuseColor.xyz * ambientCoefficient, 1.0);"
            };

            _pixelShader.Add(GLSL.CreateMethod(Type.Vec4, "ambientLighting",
                new[] { GLSL.CreateVar(Type.Float, "ambientCoefficient") }, methodBody));
        }

        private void AddDiffuseLightMethod()
        {
            var methodBody = new List<string>
            {
                "float diffuseTerm = dot(N, L);"
            };

            //TODO: Test alpha blending between diffuse and texture
            if (_materialProbs.HasDiffuseTexture)
                methodBody.Add(
                    $"vec4 blendedCol = mix({DiffuseColorName}, texture({DiffuseTextureName}, vUV), {DiffuseMixName});" +
                    $"return blendedCol * max(diffuseTerm, 0.0) * intensities;");
            else
                methodBody.Add($"return vec4({DiffuseColorName}.rgb * intensities.rgb * max(diffuseTerm, 0.0), 1.0);");

            _pixelShader.Add(GLSL.CreateMethod(Type.Vec4, "diffuseLighting",
                new[]
                {
                    GLSL.CreateVar(Type.Vec3, "N"), GLSL.CreateVar(Type.Vec3, "L"),
                    GLSL.CreateVar(Type.Vec4, "intensities")
                }, methodBody));

        }

        private void AddSpecularLightMethod()
        {

            var methodBody = new List<string>
            {
                "float specularTerm = 0.0;",
                "if(dot(N, L) > 0.0)",
                "{",
                "   // half vector",
                "   vec3 H = normalize(V + L);",
                $"  specularTerm = pow(max(0.0, dot(H, N)), {SpecularShininessName});",
                "}",
                $"return vec4(({SpecularColorName}.rgb * {SpecularIntensityName} * intensities.rgb) * specularTerm, 1.0);"
            };

            _pixelShader.Add(GLSL.CreateMethod(Type.Vec4, "specularLighting",
                new[]
                {
                    GLSL.CreateVar(Type.Vec3, "N"), GLSL.CreateVar(Type.Vec3, "L"), GLSL.CreateVar(Type.Vec3, "V"),
                    GLSL.CreateVar(Type.Vec4, "intensities")
                }, methodBody));

        }

        private void AddShadowMethod()
        {
            var methodBody = new List<string>
            {
                "// perform perspective divide for ortographic!",
                "vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;",
                "projCoords = projCoords * 0.5 + 0.5; // map to [0,1]",
                "float currentDepth = projCoords.z;",
                "float pcfDepth = texture(firstPassTex, projCoords.xy).r;",
                "float shadow = 0.0;",
                "shadow = currentDepth - 0.01 > pcfDepth ? 1.0 : 0.0;",
                "if (projCoords.z > 1.0)",
                "   shadow = 0.0;",
                "",
                "return shadow;"
            };

            _pixelShader.Add(GLSL.CreateMethod(Type.Float, "CalcShadowFactor",
                new[] { GLSL.CreateVar(Type.Vec4, "fragPosLightSpace") }, methodBody));
        }

        private void AddApplyLightMethod(MaterialComponent mc)
        {
            if (_materialProbs.HasApplyLightString)
                _pixelShader.Add((mc as MaterialLightComponent)?.ApplyLightString);


            /*  var bumpNormals = new List<string>
              {
                  "///////////////// BUMP MAPPING, object space ///////////////////",
                  $"vec3 bumpNormalsDecoded = normalize(texture(BumpTexture, vUV).rgb * 2.0 - 1.0) * (1.0-{BumpIntensityName});",
                  "vec3 N = normalize(vec3(bumpNormalsDecoded.x, bumpNormalsDecoded.y, -bumpNormalsDecoded.z));"
              }; */

            var bumpNormals = new List<string>
            {
                "///////////////// BUMP MAPPING, tangent space ///////////////////",
                $"vec3 N = ((texture(BumpTexture, vUV).rgb * 2.0) - 1.0f) * vec3({BumpIntensityName}, {BumpIntensityName}, 1.0);",
                "N = (N.x * vec3(vT)) + (N.y * vB) + (N.z * vMVNormal);",
                "N = normalize(N);"
            };

            var normals = new List<string>
            {
                "vec3 N = normalize(vMVNormal);"
            };

            var applyLightParamsWithoutNormals = new List<string>
            {
                //"vec3 N = normalize(vMVNormal);",
                "vec3 L = vec3(0.0, 0.0, 0.0);",
                "if(lightType == 1){L = -normalize(direction);}",
                "else{ L = normalize(position - vViewPos);}",
                "vec3 V = normalize(-vViewPos.xyz);",
                "if(lightType == 3) {",
                "   L = normalize(vec3(0.0,0.0,-1.0));",
                "}",
                "vec2 o_texcoords = vUV;",
                "",
                _renderWithShadows ? "float shadowFactor = CalcShadowFactor(shadowLight);" : "",
                "",
                "vec4 Idif = vec4(0);",
                "vec4 Ispe = vec4(0);",
                ""
            };

            var applyLightParams = new List<string>();
            applyLightParams.AddRange(_materialProbs.HasBump ? bumpNormals : normals);

            applyLightParams.AddRange(applyLightParamsWithoutNormals);


            if (_materialProbs.HasDiffuse)
                applyLightParams.Add("Idif = diffuseLighting(N, L, intensities);");


            if (_materialProbs.HasSpecular)
                applyLightParams.Add("Ispe = specularLighting(N, L, V, intensities);");


            var attenuation = new List<string>
            {
                "float distanceToLight = length(position - vViewPos.xyz);",
                "float distance = pow(distanceToLight / maxDistance, 2.0);",
                "float att = (clamp(1.0 - pow(distance, 2.0), 0.0, 1.0)) / (pow(distance, 2.0) + 1.0);",
            };

            var pointLight = new List<string>
            {
                _renderWithShadows
                    ? "lighting = (1.0-shadowFactor) * (Idif * att) + (Ispe * att) ;"
                    : "lighting = (Idif * att) + (Ispe * att);",
                "lighting *= strength;"
            };

            //No attenuation!
            var parallelLight = new List<string>
            {
                _renderWithShadows
                    ? "lighting = (1.0-shadowFactor) * Idif + Ispe;"
                    : "lighting = Idif + Ispe;",
                "lighting *= strength;"
            };

            var spotLight = new List<string>
            { 
            //cone component 
            "float lightToSurfaceAngleCos = dot(direction, -L);",

            "float epsilon = cos(innerConeAngle) - cos(outerConeAngle);",
            "float t = (lightToSurfaceAngleCos - cos(outerConeAngle)) / epsilon;",

            "att *= clamp(t, 0.0, 1.0);",
            "",
                _renderWithShadows
                    ? "lighting = (1.0-shadowFactor) * (Idif * att) + (Ispe * att) ;"
                    : "lighting = (Idif * att) + (Ispe * att);",
                "lighting *= strength;"
            };

            // - Disable GammaCorrection for better colors
            /*var gammaCorrection = new List<string>() 
            {
                "vec3 gamma = vec3(1.0/2.2);",
                "result = pow(result, gamma);"
            };*/

            var methodBody = new List<string>();
            methodBody.AddRange(applyLightParams);
            methodBody.Add("vec4 lighting = vec4(0);");
            methodBody.Add("");
            methodBody.AddRange(attenuation);
            methodBody.Add("if(lightType == 0) // PointLight");
            methodBody.Add("{");
            methodBody.AddRange(pointLight);
            methodBody.Add("}");
            methodBody.Add("else if(lightType == 1 || lightType == 3) // ParallelLight or LegacyLight");
            methodBody.Add("{");
            methodBody.AddRange(parallelLight);
            methodBody.Add("}");
            methodBody.Add("else if(lightType == 2) // SpotLight");
            methodBody.Add("{");
            methodBody.AddRange(spotLight);
            methodBody.Add("}");
            methodBody.Add("");
            //methodBody.AddRange(gammaCorrection); // - Disable GammaCorrection for better colors
            methodBody.Add("");

            methodBody.Add("return lighting;");

            _pixelShader.Add(GLSL.CreateMethod(Type.Vec4, "ApplyLight",
                new[]
                {
                    GLSL.CreateVar(Type.Vec3, "position"), GLSL.CreateVar(Type.Vec4, "intensities"),
                    GLSL.CreateVar(Type.Vec3, "direction"), GLSL.CreateVar(Type.Float, "maxDistance"),
                    GLSL.CreateVar(Type.Float, "strength"), GLSL.CreateVar(Type.Float, "outerConeAngle"),
                    GLSL.CreateVar(Type.Float, "innerConeAngle"), GLSL.CreateVar(Type.Int, "lightType"),
                }, methodBody));
        }

        /// <summary>
        /// Replaces Specular Calculation with Cook-Torrance-Shader
        /// </summary>
        private void AddPbrSpecularLightMethod(MaterialPBRComponent mc)
        {
            var nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };

            var delta = 0.0000001;

            var roughness = mc.RoughnessValue + delta; // always float, never int!
            var fresnel = mc.FresnelReflectance + delta;
            var k = mc.DiffuseFraction + delta;

            var methodBody = new List<string>
            {
                $"float roughnessValue = {roughness.ToString(nfi)}; // 0 : smooth, 1: rough", // roughness 
                $"float F0 = {fresnel.ToString(nfi)}; // fresnel reflectance at normal incidence", // fresnel => Specular from Blender
                $"float k = 1.0-{k.ToString(nfi)}; // metaliness", // metaliness from Blender
                "float NdotL = max(dot(N, L), 0.0);",
                "float specular = 0.0;",
                "float BlinnSpecular = 0.0;",
                "",
                "if(dot(N, L) > 0.0)",
                "{",
                "     // calculate intermediary values",
                "     vec3 H = normalize(L + V);",
                "     float NdotH = max(dot(N, H), 0.0); ",
                "     float NdotV = max(dot(N, L), 0.0); // note: this is NdotL, which is the same value",
                "     float VdotH = max(dot(V, H), 0.0);",
                "     float mSquared = roughnessValue * roughnessValue;",
                "",
                "",
                "",
                "",
                "     // -- geometric attenuation",
                "     //[Schlick's approximation of Smith's shadow equation]",
                "     float k= roughnessValue * sqrt(0.5 * 3.14159265);",
                "     float one_minus_k= 1.0 - k;",
                "     float geoAtt = ( NdotL / (NdotL * one_minus_k + k) ) * ( NdotV / (NdotV * one_minus_k + k) );",
                "",
                "     // -- roughness (or: microfacet distribution function)",
                "     // Trowbridge-Reitz or GGX, GTR2",
                "     float a2 = mSquared * mSquared;",
                "     float d = (NdotH * a2 - NdotH) * NdotH + 1.0;",
                "     float roughness = a2 / (3.14 * d * d);",
                "",
                "     // -- fresnel",
                "     // [Schlick 1994, An Inexpensive BRDF Model for Physically-Based Rendering]",
                "     float fresnel = pow(1.0 - VdotH, 5.0);",
                $"    fresnel = clamp((50.0 * {SpecularColorName}.y), 0.0, 1.0) * fresnel + (1.0 - fresnel);",
                "",
                $"     specular = (fresnel * geoAtt * roughness) / (NdotV * NdotL * 3.14);",
                "     ",
                "}",
                "",
                $"return intensities * {SpecularColorName} * (k + specular * (1.0-k));"
            };

            _pixelShader.Add(GLSL.CreateMethod(Type.Vec4, "specularLighting",
                new[]
                {
                    GLSL.CreateVar(Type.Vec3, "N"), GLSL.CreateVar(Type.Vec3, "L"), GLSL.CreateVar(Type.Vec3, "V"),
                    GLSL.CreateVar(Type.Vec4, "intensities")
                }, methodBody));
        }

        private void AddPixelBody()
        {
            string fragColorAlpha = _materialProbs.HasDiffuse ? $"{DiffuseColorName}.w" : "1.0";

            var methodBody = new List<string>
            {
                "vec4 result = ambientLighting(0.2);", //ambient component
                $"for(int i = 0; i < {_numberOfLightsForward};i++)",
                "{",
                "if(allLights[i].isActive == 0) continue;",
                "vec3 currentPosition = allLights[i].position;",
                "vec4 currentIntensities = allLights[i].intensities;",
                "vec3 currentConeDirection = allLights[i].direction;",
                "float currentAttenuation = allLights[i].maxDistance;",
                "float currentStrength = allLights[i].strength;",
                "float currentOuterConeAngle = allLights[i].outerConeAngle;",
                "float currentInnerConeAngle = allLights[i].innerConeAngle;",
                "int currentLightType = allLights[i].lightType; ",
                "result += ApplyLight(currentPosition, currentIntensities, currentConeDirection, ",
                "currentAttenuation, currentStrength, currentOuterConeAngle, currentInnerConeAngle, currentLightType);",
                "}",

                 _materialProbs.HasDiffuseTexture ? $"fragmentColor = result;" : $"fragmentColor = vec4(result.rgb, {DiffuseColorName}.w);",

            };

            _pixelShader.Add(GLSL.CreateMethod(Type.Void, "main",
                new[] { "" }, methodBody));
        }

        private static string EsPrecision()
        {
            /*return "#ifdef GL_ES\n" +
                   "    precision highp float;\n" +
                   "#endif\n\n";*/
            return "precision highp float; \n";
        }

        private static string LightStructDeclaration()
        {
            var lightStruct = @"
            struct Light 
            {
                vec3 position;
                vec3 positionWorldSpace;
                vec4 intensities;
                vec3 direction;
                vec3 directionWorldSpace;
                float maxDistance;
                float strength;
                float outerConeAngle;
                float innerConeAngle;
                int lightType;
                int isActive;
                int isCastingShadows;
                float bias;
            };
            ";
            return lightStruct + $"uniform Light allLights[{_numberOfLightsForward}];";
            
        }

        private static string Version()
        {
            return "#version 300 es\n";
        }

        #endregion

        #region Deferred

        /// <summary>
        /// FXAA shader relies on the luminosity of the pixels read from the texture.
        /// It is a weighted sum of the red, green and blue components that takes into account the sensibility of our eyes to each wavelength range.
        /// </summary>
        /// <returns></returns>
        private static string RGBLuma()
        {
            return @"
            float rgb2luma(vec3 rgb)
            {
                return rgb.y * (0.587/0.299) + rgb.x; //sqrt(dot(rgb, vec3(0.299, 0.587, 0.114)));
            }
            ";
        }

        private static string Quality()
        {
            return @"
            float QUALITY(int i)
            {
                switch(i)
                {
                    case 8:
                        return 1.5;
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        return 2.0;
                    case 13:
                        return 4.0;
                    case 14:
                        return 8.0;
                }
            }
            ";
        }

        /// <summary>
        /// If rendered with FXAA we'll need an additional (final) pass, that takes the lighted scene, rendered to a texture, as input.
        /// </summary>
        /// <param name="srcRenderTarget">RenderTarget, that contains a single texture in the Albedo/Specular channel, that contains the lighted scene.</param>
        /// <param name="screenParams">The width and height of the screen.</param>       
        // see: http://developer.download.nvidia.com/assets/gamedev/files/sdk/11/FXAA_WhitePaper.pdf
        // http://blog.simonrodriguez.fr/articles/30-07-2016_implementing_fxaa.html
        public static ShaderEffect FXAARenderTargetEffect(RenderTarget srcRenderTarget, float2 screenParams)
        {
            //------------ vertex shader ------------------//
            var vert = new StringBuilder();
            vert.Append(Version());
            vert.Append(EsPrecision());

            vert.Append(@"

            in vec3 fuVertex;
            out vec2 vTexCoords;

            ");

            vert.Append(@"
            void main() 
            {
                vTexCoords = fuVertex.xy * 2.0 * 0.5 + 0.5;
                gl_Position = vec4(fuVertex.xy * 2.0, 0.0 ,1.0);

            }");

            //--------- Fragment shader ----------- //
            var frag = new StringBuilder();
            frag.Append(Version());
            frag.Append(EsPrecision());
            frag.Append($"#define LIGHTED_SCENE_TEX {Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)}\n");
            frag.Append($"#define EDGE_THRESHOLD_MIN 0.0625\n");
            frag.Append($"#define EDGE_THRESHOLD_MAX 0.125\n");
            frag.Append($"#define ITERATIONS 14\n");
            frag.Append($"#define SUBPIXEL_QUALITY 0.125\n");

            frag.Append($"in vec2 vTexCoords;\n");

            frag.Append($"uniform sampler2D LIGHTED_SCENE_TEX;\n");
            frag.Append($"uniform vec2 ScreenParams;\n");

            frag.Append($"out vec4 oColor;\n");

            frag.Append(RGBLuma());

            frag.Append(Quality());

            frag.Append("void main() {");

            frag.Append(@"
                        
            // ------ FXAA calculation ------ //

            // ---- 0. Detecting where to apply FXAA

            vec2 inverseScreenSize = vec2(1.0/ScreenParams.x, 1.0/ScreenParams.y);
            vec3 colorCenter = texture(LIGHTED_SCENE_TEX, vTexCoords).rgb;

            // Luma at the current fragment
            float lumaCenter = rgb2luma(colorCenter);

            // Luma at the four direct neighbours of the current fragment.
            float lumaDown = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(0,-1)).rgb);
            float lumaUp = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(0,1)).rgb);
            float lumaLeft = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(-1,0)).rgb);
            float lumaRight = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(1,0)).rgb);

            // Find the maximum and minimum luma around the current fragment.
            float lumaMin = min(lumaCenter,min(min(lumaDown,lumaUp),min(lumaLeft,lumaRight)));
            float lumaMax = max(lumaCenter,max(max(lumaDown,lumaUp),max(lumaLeft,lumaRight)));

            // Compute the delta.
            float lumaRange = lumaMax - lumaMin;

            // If the luma variation is lower that a threshold (or if we are in a really dark area), we are not on an edge, don't perform any AA.
            if(lumaRange < max(EDGE_THRESHOLD_MIN, lumaMax * EDGE_THRESHOLD_MAX)) 
            {
                oColor = vec4(colorCenter, 1.0);
                return;
            }
            
            // ---- 1. Choosing Edge direction (vertical or horizontal)

            // Query the 4 remaining corners lumas.
            float lumaDownLeft = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(-1,-1)).rgb);
            float lumaUpRight = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(1,1)).rgb);
            float lumaUpLeft = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(-1,1)).rgb);
            float lumaDownRight = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(1,-1)).rgb);

            // Combine the four edges lumas (using intermediary variables for future computations with the same values).
            float lumaDownUp = lumaDown + lumaUp;
            float lumaLeftRight = lumaLeft + lumaRight;

            // Same for corners
            float lumaLeftCorners = lumaDownLeft + lumaUpLeft;
            float lumaDownCorners = lumaDownLeft + lumaDownRight;
            float lumaRightCorners = lumaDownRight + lumaUpRight;
            float lumaUpCorners = lumaUpRight + lumaUpLeft;

            // Compute an estimation of the gradient along the horizontal and vertical axis.
            float edgeHorizontal =  abs(-2.0 * lumaLeft + lumaLeftCorners)  + abs(-2.0 * lumaCenter + lumaDownUp ) * 2.0    + abs(-2.0 * lumaRight + lumaRightCorners);
            float edgeVertical =    abs(-2.0 * lumaUp + lumaUpCorners)      + abs(-2.0 * lumaCenter + lumaLeftRight) * 2.0  + abs(-2.0 * lumaDown + lumaDownCorners);

            // Is the local edge horizontal or vertical ?
            bool isHorizontal = (edgeHorizontal >= edgeVertical);
            
            // ---- 2. Estimating gradient and choosing edge direction (current pixel is not necessarily exactly on the edge).

            // Select the two neighboring texels lumas in the opposite direction to the local edge.
            float luma1 = isHorizontal ? lumaDown : lumaLeft;
            float luma2 = isHorizontal ? lumaUp : lumaRight;
            // Compute gradients in this direction.
            float gradient1 = luma1 - lumaCenter;
            float gradient2 = luma2 - lumaCenter;

            // Which direction is the steepest ?
            bool is1Steepest = abs(gradient1) >= abs(gradient2);

            // Gradient in the corresponding direction, normalized.
            float gradientScaled = 0.25*max(abs(gradient1),abs(gradient2));

            // Choose the step size (one pixel) according to the edge direction.
            float stepLength = isHorizontal ? inverseScreenSize.y : inverseScreenSize.x;

            // Average luma in the correct direction.
            float lumaLocalAverage = 0.0;

            if(is1Steepest)
            {
                // Switch the direction
                stepLength = - stepLength;
                lumaLocalAverage = 0.5*(luma1 + lumaCenter);
            } 
            else 
            {
                lumaLocalAverage = 0.5*(luma2 + lumaCenter);
            }

            // Shift UV in the correct direction by half a pixel.
            vec2 currentUv = vTexCoords;
            if(isHorizontal)
            {
                currentUv.y += stepLength * 0.5;
            } 
            else 
            {
                currentUv.x += stepLength * 0.5;
            }

            // ---- 3. Exploration along the main axis of the edge.

            // Compute offset (for each iteration step) in the right direction.
            vec2 offset = isHorizontal ? vec2(inverseScreenSize.x,0.0) : vec2(0.0,inverseScreenSize.y);
            // Compute UVs to explore on each side of the edge, orthogonally. The QUALITY allows us to step faster.
            vec2 uv1 = currentUv - offset;
            vec2 uv2 = currentUv + offset;

            // Read the lumas at both current extremities of the exploration segment, and compute the delta wrt to the local average luma.
            float lumaEnd1 = rgb2luma(texture(LIGHTED_SCENE_TEX,uv1).rgb);
            float lumaEnd2 = rgb2luma(texture(LIGHTED_SCENE_TEX,uv2).rgb);
            lumaEnd1 -= lumaLocalAverage;
            lumaEnd2 -= lumaLocalAverage;

            // If the luma deltas at the current extremities are larger than the local gradient, we have reached the side of the edge.
            bool reached1 = abs(lumaEnd1) >= gradientScaled;
            bool reached2 = abs(lumaEnd2) >= gradientScaled;
            bool reachedBoth = reached1 && reached2;

            // If the side is not reached, we continue to explore in this direction.
            if(!reached1){
                uv1 -= offset;
            }
            if(!reached2){
                uv2 += offset;
            }   

            // ---- 4. Iterating - keep iterating until both extremities of the edge are reached, or until the maximum number of iterations (12) is reached.
            // If both sides have not been reached, continue to explore.
            if(!reachedBoth){

                for(int i = 2; i < ITERATIONS; i++){
                    // If needed, read luma in 1st direction, compute delta.
                    if(!reached1){
                        lumaEnd1 = rgb2luma(texture(LIGHTED_SCENE_TEX, uv1).rgb);
                        lumaEnd1 = lumaEnd1 - lumaLocalAverage;
                    }
                    // If needed, read luma in opposite direction, compute delta.
                    if(!reached2){
                        lumaEnd2 = rgb2luma(texture(LIGHTED_SCENE_TEX, uv2).rgb);
                        lumaEnd2 = lumaEnd2 - lumaLocalAverage;
                    }
                    // If the luma deltas at the current extremities is larger than the local gradient, we have reached the side of the edge.
                    reached1 = abs(lumaEnd1) >= gradientScaled;
                    reached2 = abs(lumaEnd2) >= gradientScaled;
                    reachedBoth = reached1 && reached2;

                    // If the side is not reached, we continue to explore in this direction, with a variable quality.
                    if(!reached1){
                        uv1 -= offset * QUALITY(i);
                    }
                    if(!reached2){
                        uv2 += offset * QUALITY(i);
                    }

                    // If both sides have been reached, stop the exploration.
                    if(reachedBoth){ break;}
                }
            }

            // ---- 5. Estimating offset.

            // Compute the distances to each extremity of the edge.
            float distance1 = isHorizontal ? (vTexCoords.x - uv1.x) : (vTexCoords.y - uv1.y);
            float distance2 = isHorizontal ? (uv2.x - vTexCoords.x) : (uv2.y - vTexCoords.y);

            // In which direction is the extremity of the edge closer ?
            bool isDirection1 = distance1 < distance2;
            float distanceFinal = min(distance1, distance2);

            // Length of the edge.
            float edgeThickness = (distance1 + distance2);

            // UV offset: read in the direction of the closest side of the edge.
            float pixelOffset = - distanceFinal / edgeThickness + 0.5;
            
            // Is the luma at center smaller than the local average ?
            bool isLumaCenterSmaller = lumaCenter < lumaLocalAverage;

            // If the luma at center is smaller than at its neighbour, the delta luma at each end should be positive (same variation).
            // (in the direction of the closer side of the edge.)
            bool correctVariation = ((isDirection1 ? lumaEnd1 : lumaEnd2) < 0.0) != isLumaCenterSmaller;

            // If the luma variation is incorrect, do not offset.
            float finalOffset = correctVariation ? pixelOffset : 0.0;

            // ---- 5. Subpixel antialiasing

            // Sub-pixel shifting
            // Full weighted average of the luma over the 3x3 neighborhood.
            float lumaAverage = (1.0/12.0) * (2.0 * (lumaDownUp + lumaLeftRight) + lumaLeftCorners + lumaRightCorners);
            // Ratio of the delta between the global average and the center luma, over the luma range in the 3x3 neighborhood.
            float subPixelOffset1 = clamp(abs(lumaAverage - lumaCenter)/lumaRange,0.0,1.0);
            float subPixelOffset2 = (-2.0 * subPixelOffset1 + 3.0) * subPixelOffset1 * subPixelOffset1;
            // Compute a sub-pixel offset based on this delta.
            float subPixelOffsetFinal = subPixelOffset2 * subPixelOffset2 * SUBPIXEL_QUALITY;

            // Pick the biggest of the two offsets.
            finalOffset = max(finalOffset,subPixelOffsetFinal);

            // ---- 6. Final read
            // Compute the final UV coordinates.
            vec2 finalUv = vTexCoords;
            if(isHorizontal)
            {
                finalUv.y += finalOffset * stepLength;
            } 
            else 
            {
                finalUv.x += finalOffset * stepLength;
            }

            // Read the color at the new UV coordinates, and use it.
            vec4 finalColor = texture(LIGHTED_SCENE_TEX, finalUv);
            oColor = finalColor;
            
            ");

            frag.Append("}");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO_SPECULAR]},
                new EffectParameterDeclaration { Name = "ScreenParams", Value = screenParams},
            });

        }

        /// <summary>
        /// Creates a blurred ssao texture, to hide rectangular artifacts originating from the noise texture;
        /// </summary>
        /// <param name="ssaoRenderTex">The non blurred ssao texture.</param>        
        public static ShaderEffect SSAORenderTargetBlurEffect(WritableTexture ssaoRenderTex)
        {
            float blurKernelSize;
            switch (ssaoRenderTex.Width)
            {
                case (int)TexRes.LOW_RES:
                    blurKernelSize = 2.0f;
                    break;
                default:
                case (int)TexRes.MID_RES:
                    blurKernelSize = 4.0f;
                    break;
                case (int)TexRes.HIGH_RES:
                    blurKernelSize = 8.0f;
                    break;
            }

            //------------ vertex shader ------------------//
            var vert = new StringBuilder();
            vert.Append(Version());
            vert.Append(EsPrecision());

            vert.Append(@"

            in vec3 fuVertex;
            out vec2 vTexCoords;

            ");

            vert.Append(@"
            void main() 
            {
                vTexCoords = fuVertex.xy * 2.0 * 0.5 + 0.5;
                gl_Position = vec4(fuVertex.xy * 2.0, 0.0 ,1.0);

            }");

            //--------- Fragment shader ----------- //
            var frag = new StringBuilder();
            frag.Append(Version());
            frag.Append(EsPrecision());
            frag.Append($"#define SSAO_INPUT_TEX {Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)}\n");
            frag.Append($"#define KERNEL_SIZE {blurKernelSize.ToString("0.0", CultureInfo.InvariantCulture)}\n");
            frag.Append($"#define KERNEL_SIZE_HALF {(blurKernelSize * 0.5)}\n");

            frag.Append($"in vec2 vTexCoords;\n");

            frag.Append($"uniform sampler2D SSAO_INPUT_TEX;\n");


            frag.Append($"out vec4 o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)};\n");

            frag.Append("void main() {");

            frag.Append(@"
            vec2 texelSize = 1.0 / vec2(textureSize(SSAO_INPUT_TEX, 0));
            float result = 0.0;
            for (int x = -KERNEL_SIZE_HALF; x < KERNEL_SIZE_HALF; ++x) 
            {
                for (int y = -KERNEL_SIZE_HALF; y < KERNEL_SIZE_HALF; ++y) 
                {
                    vec2 offset = vec2(float(x), float(y)) * texelSize;
                    result += texture(SSAO_INPUT_TEX, vTexCoords + offset).r;
                }
            }

            result = result / (KERNEL_SIZE * KERNEL_SIZE);
            
            ");

            frag.Append($"o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)} = vec4(result, result, result, 1.0);");

            frag.Append("}");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_SSAO.ToString(), Value = ssaoRenderTex},

            });

        }

        /// <summary>
        /// Shader effect for the ssao pass.
        /// </summary>        
        /// <param name="geomPassRenderTarget">RenderTarget filled in the previous geometry pass.</param>
        /// <param name="kernelLength">SSAO kernel size.</param>
        /// <param name="screenParams">Width and Height of the screen.</param>        
        public static ShaderEffect SSAORenderTargetTextureEffect(RenderTarget geomPassRenderTarget, int kernelLength, float2 screenParams)
        {
            var ssaoKernel = SSAOHelper.CreateKernel(kernelLength);
            var ssaoNoiseTex = SSAOHelper.CreateNoiseTex(16);

            //------------ vertex shader ------------------//
            var vert = new StringBuilder();
            vert.Append(Version());
            vert.Append(EsPrecision());

            vert.Append(@"

            in vec3 fuVertex;
            out vec2 vTexCoords;

            ");

            vert.Append(@"
            void main() 
            {
                vTexCoords = fuVertex.xy * 2.0 * 0.5 + 0.5;
                gl_Position = vec4(fuVertex.xy * 2.0, 0.0 ,1.0);

            }");

            //--------- Fragment shader ----------- //
            var frag = new StringBuilder();
            frag.Append(Version());
            frag.Append(EsPrecision());
            frag.Append($"#define KERNEL_LENGTH {kernelLength}\n");

            frag.Append($"in vec2 vTexCoords;\n");

            frag.Append($"uniform vec2 ScreenParams;\n");
            frag.Append($"uniform vec3[KERNEL_LENGTH] SSAOKernel;\n");
            frag.Append($"uniform sampler2D {Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_POSITION)};\n");
            frag.Append($"uniform sampler2D {Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_NORMAL)};\n");
            frag.Append($"uniform sampler2D NoiseTex;\n");
            frag.Append($"uniform mat4 FUSEE_P;\n");

            frag.Append($"out vec4 {Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)};\n");

            frag.Append("void main() {");
            frag.AppendLine($"vec3 Normal = texture({RenderTargetTextureTypes.G_NORMAL.ToString()}, vTexCoords).rgb;");

            frag.Append(@"
            if(Normal.x == 0.0 && Normal.y == 0.0 && Normal.z == 0.0)
                discard;
            ");

            frag.AppendLine($"vec3 FragPos = texture({RenderTargetTextureTypes.G_POSITION.ToString()}, vTexCoords).xyz;");

            //SSAO
            //-------------------------------------- -
            frag.Append(@"
            float radius = 5.0;
            float occlusion = 0.0;
            float bias = 0.005;
            ");

            frag.AppendLine($"vec2 noiseScale = vec2(ScreenParams.x * 0.25, ScreenParams.y * 0.25);");
            frag.AppendLine($"vec3 randomVec = texture(NoiseTex, vTexCoords * noiseScale).xyz;");

            frag.AppendLine($"vec3 tangent = normalize(randomVec - Normal * dot(randomVec, Normal));");
            frag.AppendLine($"vec3 bitangent = cross(Normal, tangent);");
            frag.AppendLine($"mat3 tbn = mat3(tangent, bitangent, Normal);");

            frag.Append(@"

            for (int i = 0; i < KERNEL_LENGTH; ++i) 
            {
             // get sample position:
             vec3 sampleVal = tbn * SSAOKernel[i];
             sampleVal = sampleVal * radius + FragPos.xyz;

             // project sample position:
             vec4 offset = vec4(sampleVal, 1.0);
             offset = FUSEE_P * offset;		
             offset.xy /= offset.w;
             offset.xy = offset.xy * 0.5 + 0.5;

             // get sample depth:
             // ----- EXPENSIVE TEXTURE LOOKUP - graphics card workload goes up and frame rate goes down the nearer the camera is to the model.
             // keyword: dependent texture look up, see also: https://stackoverflow.com/questions/31682173/strange-performance-behaviour-with-ssao-algorithm-using-opengl-and-glsl
            ");

            frag.AppendLine($"float sampleDepth = texture({RenderTargetTextureTypes.G_POSITION.ToString()}, offset.xy).z;");
            frag.Append(@"           

             // range check & accumulate:
             float rangeCheck = smoothstep(0.0, 1.0, radius / abs(FragPos.z - sampleDepth));
             occlusion += (sampleDepth <= sampleVal.z + bias ? 1.0 : 0.0) * rangeCheck;
            }

            occlusion = clamp(1.0 - (occlusion / float(KERNEL_LENGTH)), 0.0, 1.0);           

            ");

            frag.Append($"{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)} = vec4(occlusion, occlusion, occlusion, 1.0);");

            frag.Append("}");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_POSITION.ToString(), Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_POSITION]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_NORMAL.ToString(), Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_NORMAL]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString(), Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO_SPECULAR]},

                new EffectParameterDeclaration { Name = "ScreenParams", Value = screenParams},
                new EffectParameterDeclaration {Name = "SSAOKernel[0]", Value = ssaoKernel},
                new EffectParameterDeclaration {Name = "NoiseTex", Value = ssaoNoiseTex},
                new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity},
            });

        }

        /// <summary>
        /// ShaderEffect for rendering into the textures given in a RenderTarget (Geometry Pass).
        /// </summary>
        /// <param name="rt">The RenderTarget</param>
        /// <param name="diffuseMix">Constant for mixing a single albedo color with a color read from a texture.</param>
        /// <param name="diffuseTex">The texture, containing diffuse colors.</param>
        /// <returns></returns>
        public static ShaderEffect GBufferTextureEffect(RenderTarget rt, float diffuseMix, Texture diffuseTex = null)
        {
            var textures = rt.RenderTextures;

            //------------ vertex shader ------------------//
            var vert = new StringBuilder();

            vert.Append(Version());
            vert.Append(EsPrecision());

            vert.Append(@"
                uniform mat4 FUSEE_M;
                uniform mat4 FUSEE_MV;
                uniform mat4 FUSEE_MVP;
                uniform mat4 FUSEE_ITM;
                uniform mat4 FUSEE_ITMV;
                uniform vec4 DiffuseColor;

                in vec3 fuVertex;
                in vec3 fuNormal;
                in vec4 fuColor;
                in vec2 fuUV;
                
                out vec4 vPos;
                out vec3 vNormal;
                out vec4 vColor;    
                out vec2 vUv;

                ");

            vert.Append(@"
                void main() 
                {
                    vPos = FUSEE_MV * vec4(fuVertex.xyz, 1.0);
                    vNormal = (FUSEE_ITMV * vec4(fuNormal, 0.0)).xyz;
                    vColor = DiffuseColor;
                    vUv = fuUV;

                    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

                }");

            //--------- Fragment shader ----------- //
            var frag = new StringBuilder();
            frag.Append(Version());
            frag.Append(EsPrecision());

            var texCount = 0;

            for (int i = 0; i < textures.Length; i++)
            {
                var tex = textures[i];
                if (tex == null) continue;

                frag.Append($"layout (location = {texCount}) out vec4 {Enum.GetName(typeof(RenderTargetTextureTypes), i)};\n");
                texCount++;
            }

            frag.Append(@"
                in vec4 vPos;
                in vec3 vNormal;
                in vec4 vColor;
                in vec2 vUv;"
            );

            if (diffuseTex != null)
            {
                frag.Append(@"
                uniform sampler2D DiffuseTexture;
                uniform float DiffuseMix;"
                );
            }

            frag.AppendLine("void main() {");

            for (int i = 0; i < textures.Length; i++)
            {
                var tex = textures[i];
                if (tex == null) continue;

                switch (i)
                {
                    case 0: //POSITION
                        frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(vPos.xyz, vPos.w);");
                        break;
                    case 1: //ALBEDO_SPECULAR
                        if (diffuseTex != null)
                            frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(mix(vColor.xyz, texture(DiffuseTexture, vUv).xyz, DiffuseMix), vColor.a);");
                        else
                            frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vColor;");
                        break;
                    case 2: //NORMAL
                        frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(normalize(vNormal.xyz), 1.0);");
                        break;
                    case 3: //DEPTH
                        frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);");
                        break;
                }
            }
            frag.Append("}");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_ITM", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "DiffuseColor", Value = new float4(1.0f, 0, 1.0f, 1.0f)},
                new EffectParameterDeclaration {Name = "DiffuseTexture", Value = diffuseTex},
                new EffectParameterDeclaration {Name = "DiffuseMix", Value = diffuseMix},
            });

        }

        private static string DeferredLightningVS()
        {
            var vert = new StringBuilder();
            vert.Append(Version());
            vert.Append(EsPrecision());

            vert.Append(@"
                
            uniform mat4 FUSEE_MVP;                       
            in vec3 fuVertex;
            out vec2 vTexCoords;           

            ");

            vert.Append(@"
            void main() 
            {                
                vTexCoords = fuVertex.xy * 2.0 * 0.5 + 0.5;
                gl_Position = vec4(fuVertex.xy * 2.0, 0.0 ,1.0);

            }");

            return vert.ToString();
        }        

        private static string DeferredLightningFS(LightComponent lc)
        {
            // Fragment shader ------------------------------
            var frag = new StringBuilder();
            frag.Append(Version());
            frag.Append("#extension GL_ARB_explicit_uniform_location : enable\n");
            frag.Append(EsPrecision());

            for (int i = 0; i < Enum.GetNames(typeof(RenderTargetTextureTypes)).Length; i++)
            {
                frag.Append($"uniform sampler2D {Enum.GetName(typeof(RenderTargetTextureTypes), i)};\n");
            }

            frag.Append(@"struct Light 
            {
                vec3 position;
                vec3 positionWorldSpace;
                vec4 intensities;
                vec3 direction;
                vec3 directionWorldSpace;
                float maxDistance;
                float strength;
                float outerConeAngle;
                float innerConeAngle;
                int lightType;
                int isActive;
                int isCastingShadows;
                float bias;
            };
            uniform Light light;
            ");

            frag.Append("uniform mat4 FUSEE_IV;\n");
            frag.Append("uniform mat4 FUSEE_V;\n");
            frag.Append("uniform mat4 FUSEE_MV;\n");
            frag.Append("uniform mat4 FUSEE_ITV;\n");

            if (lc.IsCastingShadows)
            {
                if (lc.Type != LightType.Point)
                    frag.Append($"uniform sampler2D ShadowMap;\n");
                else
                    frag.Append("uniform samplerCube ShadowCubeMap;\n");
            }

            frag.Append("uniform mat4x4 LightSpaceMatrix;\n");
            frag.Append("uniform int PassNo;\n");
            frag.Append("uniform int SsaoOn;\n");

            frag.Append("uniform vec4 BackgroundColor;\n");

            frag.Append($"in vec2 vTexCoords;\n");
            frag.Append($"layout (location = {0}) out vec4 o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)};\n");

            //Shadow calculation
            //-------------------------------------- 
            if (lc.Type != LightType.Point)
                frag.Append(ShadowCalculation());
            else
                frag.Append(ShadowCalculationCubeMap());

            frag.Append(@"void main()
            {
            ");

            frag.AppendLine($"vec3 Normal = texture({RenderTargetTextureTypes.G_NORMAL.ToString()}, vTexCoords).rgb;");
            //Do not do calculations for the background - is there a smarter way (stencil buffer)?
            //---------------------------------------
            frag.Append(@"
            if(Normal.x == 0.0 && Normal.y == 0.0 && Normal.z == 0.0)      
            {
            ");

            frag.AppendLine($"  o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)} = BackgroundColor;");
            frag.AppendLine(@"  return;
            }
            ");

            frag.AppendLine($"vec4 FragPos = texture({RenderTargetTextureTypes.G_POSITION.ToString()}, vTexCoords);");
            frag.AppendLine($"vec3 DiffuseColor = texture({RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString()}, vTexCoords).rgb;");
            frag.AppendLine($"float SpecularStrength = texture({RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString()}, vTexCoords).a;");
            frag.AppendLine($"vec3 Occlusion = texture({RenderTargetTextureTypes.G_SSAO.ToString()}, vTexCoords).rgb;");

            //Lighting calculation
            //-------------------------
            frag.Append(@"
            // then calculate lighting as usual
            vec3 lighting = vec3(0,0,0);

            if(PassNo == 0)
            {
                vec3 ambient = vec3(0.2 * DiffuseColor);

                if(SsaoOn == 1)
                    ambient *= Occlusion;

                lighting += ambient;
            }

            vec3 camPos = FUSEE_IV[3].xyz;
            vec3 viewDir = normalize(-FragPos.xyz);

           
            if(light.isActive == 1)
            {
                float shadow = 0.0;

                vec3 lightColor = light.intensities.xyz;
                vec3 lightPosition = light.position;
                vec3 lightDir = normalize(lightPosition - FragPos.xyz);                

                //attenuation
                float attenuation = 1.0;
                switch(light.lightType)
                {
                    //Point
                    case 0:
                    {
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
                        float distance = pow(distanceToLight/light.maxDistance, 2.0);
                        attenuation = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);                        

                        break;
                    }
                    //Parallel
                    case 1:
                        lightDir = -light.direction;
                        break;
                    //Spot
                    case 2:
                    {                           
                        //point component
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
                        float distance = pow(distanceToLight/light.maxDistance, 2.0);
                        attenuation = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);
                        
                        //cone component
                        vec3 coneDir = light.direction;
                        float lightToSurfaceAngleCos = dot(coneDir, -lightDir); 
                        
                        float epsilon = cos(light.innerConeAngle) - cos(light.outerConeAngle);
                        float t = (lightToSurfaceAngleCos - cos(light.outerConeAngle)) / epsilon;
                        attenuation *= clamp(t, 0.0, 1.0);
                        break;
                    }
                    case 3:
                        break;
                }
                ");

            if (lc.IsCastingShadows)
            {
                if (lc.Type != LightType.Point)
                {
                    frag.Append(@"
                    // shadow                
                    if (light.isCastingShadows == 1)
                    {
                        vec4 posInLightSpace = (LightSpaceMatrix * FUSEE_IV) * FragPos;
                        shadow = ShadowCalculation(ShadowMap, posInLightSpace, Normal, lightDir,  light.bias);                    
                    }                
                    ");
                }
                else
                {
                    frag.Append(@"
                    // shadow       
                    if (light.isCastingShadows == 1)
                    {
                        shadow = ShadowCalculationCubeMap(ShadowCubeMap, (FUSEE_IV * FragPos).xyz, light.positionWorldSpace, light.maxDistance, Normal, lightDir, light.bias);
                    }
                    ");
                }
            }

            frag.Append(@"
                // diffuse 
                vec3 diffuse = max(dot(Normal, lightDir), 0.0) * DiffuseColor * lightColor;
                lighting += (1.0 - shadow) * (diffuse * attenuation * light.strength);
            
                // specular
                vec3 reflectDir = reflect(-lightDir, Normal);  
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), 100.0);
                vec3 specular = SpecularStrength * spec * lightColor;
                lighting += (1.0 - shadow) * (specular * attenuation * light.strength);
            }              
            ");

            frag.AppendLine($"o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)} = vec4(lighting, 1.0);");

            frag.Append("}");

            return frag.ToString();
        }

        private static string DeferredLightningFSCascaded(LightComponent lc, int numberOfCascades)
        {
            // Fragment shader ------------------------------
            var frag = new StringBuilder();
            frag.Append(Version());
            frag.Append("#extension GL_ARB_explicit_uniform_location : enable\n");
            frag.Append(EsPrecision());

            for (int i = 0; i < Enum.GetNames(typeof(RenderTargetTextureTypes)).Length; i++)
            {
                frag.Append($"uniform sampler2D {Enum.GetName(typeof(RenderTargetTextureTypes), i)};\n");
            }

            frag.Append(@"struct Light 
            {
                vec3 position;
                vec3 positionWorldSpace;
                vec4 intensities;
                vec3 direction;
                vec3 directionWorldSpace;
                float maxDistance;
                float strength;
                float outerConeAngle;
                float innerConeAngle;
                int lightType;
                int isActive;
                int isCastingShadows;
                float bias;
            };
            uniform Light light;
            ");

            frag.Append("uniform mat4 FUSEE_IV;\n");
            frag.Append("uniform mat4 FUSEE_V;\n");
            frag.Append("uniform mat4 FUSEE_MV;\n");
            frag.Append("uniform mat4 FUSEE_ITV;\n");
           
            frag.Append($"uniform sampler2D[{numberOfCascades}] ShadowMaps;\n");
            frag.Append($"uniform vec2[{numberOfCascades}] ClipPlanes;\n");            

            frag.Append($"uniform mat4x4[{numberOfCascades}] LightSpaceMatrices;\n");
            frag.Append("uniform int PassNo;\n");
            frag.Append("uniform int SsaoOn;\n");

            frag.Append("uniform vec4 BackgroundColor;\n");

            frag.Append($"in vec2 vTexCoords;\n");
            frag.Append($"layout (location = {0}) out vec4 o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)};\n");

            //Shadow calculation
            //-------------------------------------- 
            if (lc.Type != LightType.Point)
                frag.Append(ShadowCalculation());
            else
                frag.Append(ShadowCalculationCubeMap());

            frag.Append(@"void main()
            {
            ");

            frag.AppendLine($"vec3 Normal = texture({RenderTargetTextureTypes.G_NORMAL.ToString()}, vTexCoords).rgb;");
            //Do not do calculations for the background - is there a smarter way (stencil buffer)?
            //---------------------------------------
            frag.Append(@"
            if(Normal.x == 0.0 && Normal.y == 0.0 && Normal.z == 0.0)      
            {
            ");

            frag.AppendLine($"  o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)} = BackgroundColor;");
            frag.AppendLine(@"  return;
            }
            ");

            frag.AppendLine($"vec4 FragPos = texture({RenderTargetTextureTypes.G_POSITION.ToString()}, vTexCoords);");
            frag.AppendLine($"vec3 DiffuseColor = texture({RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString()}, vTexCoords).rgb;");
            frag.AppendLine($"float SpecularStrength = texture({RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString()}, vTexCoords).a;");
            frag.AppendLine($"vec3 Occlusion = texture({RenderTargetTextureTypes.G_SSAO.ToString()}, vTexCoords).rgb;");

            //Lighting calculation
            //-------------------------
            frag.Append(@"
            // then calculate lighting as usual
            vec3 lighting = vec3(0,0,0);

            if(PassNo == 0)
            {
                vec3 ambient = vec3(0.2 * DiffuseColor);

                if(SsaoOn == 1)
                    ambient *= Occlusion;

                lighting += ambient;
            }

            vec3 camPos = FUSEE_IV[3].xyz;
            vec3 viewDir = normalize(-FragPos.xyz);

           
            if(light.isActive == 1)
            {
                float shadow = 0.0;

                vec3 lightColor = light.intensities.xyz;
                vec3 lightPosition = light.position;
                vec3 lightDir = normalize(lightPosition - FragPos.xyz);                

                //attenuation
                float attenuation = 1.0;
                switch(light.lightType)
                {
                    //Point
                    case 0:
                    {
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
                        float distance = pow(distanceToLight/light.maxDistance, 2.0);
                        attenuation = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);                        

                        break;
                    }
                    //Parallel
                    case 1:
                        lightDir = -light.direction;
                        break;
                    //Spot
                    case 2:
                    {                           
                        //point component
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
                        float distance = pow(distanceToLight/light.maxDistance, 2.0);
                        attenuation = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);
                        
                        //cone component
                        vec3 coneDir = light.direction;
                        float lightToSurfaceAngleCos = dot(coneDir, -lightDir); 
                        
                        float epsilon = cos(light.innerConeAngle) - cos(light.outerConeAngle);
                        float t = (lightToSurfaceAngleCos - cos(light.outerConeAngle)) / epsilon;
                        attenuation *= clamp(t, 0.0, 1.0);
                        break;
                    }
                    case 3:
                        break;
                }
                ");

            if (lc.IsCastingShadows)
            {
                //TODO: iterate clip planes and choose shadow map for this frag. Use this shadow map in ShadowCalculation()  

                frag.Append(@"
                int thisFragmentsCascade = 0;
                float fragDepth = FragPos.z;
                
                ");

                
                frag.Append($"for (int i = 0; i < {numberOfCascades}; i++)\n");
                frag.Append(
                @"{
                    vec2 thisCascadesClipPlanes = ClipPlanes[i];
                    if(fragDepth < thisCascadesClipPlanes.y)
                    {                        
                        thisFragmentsCascade = i;
                        break;
                    }
                }
                ");

                frag.Append(@"
                // shadow                
                if (light.isCastingShadows == 1)
                {
                    vec4 posInLightSpace = (LightSpaceMatrices[thisFragmentsCascade] * FUSEE_IV) * FragPos;
                    shadow = ShadowCalculation(ShadowMaps[thisFragmentsCascade], posInLightSpace, Normal, lightDir,  light.bias);                    
                }                
                ");               
            }

            frag.Append(@"
                // diffuse 
                vec3 diffuse = max(dot(Normal, lightDir), 0.0) * DiffuseColor * lightColor;
                lighting += (1.0 - shadow) * (diffuse * attenuation * light.strength);
            
                // specular
                vec3 reflectDir = reflect(-lightDir, Normal);  
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), 100.0);
                vec3 specular = SpecularStrength * spec * lightColor;
                lighting += (1.0 - shadow) * (specular * attenuation * light.strength);

                if(thisFragmentsCascade == 0)
                    lighting *= vec3(1,0.3f,0.3f);
                else if(thisFragmentsCascade == 1)
                    lighting *= vec3(0.3f,1,0.3f);
                else if(thisFragmentsCascade == 2)
                    lighting *= vec3(0.3f,0.3f,1);
                else if(thisFragmentsCascade == 3)
                    lighting *= vec3(1,1,0.3f);
                else if(thisFragmentsCascade == 4)
                    lighting *= vec3(1,0.3,1);
                else if(thisFragmentsCascade == 5)
                    lighting *= vec3(1,0.3f,1);
            }              
            ");

            frag.AppendLine($"o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)} = vec4(lighting, 1.0);");

            frag.Append("}");

            return frag.ToString();
        }

        private static List<EffectParameterDeclaration> DefferedLightingEffectParams(RenderTarget srcRenderTarget, float4 backgroundColor)
        {
            return new List<EffectParameterDeclaration>()
            {
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_POSITION.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_POSITION]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_NORMAL.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_NORMAL]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO_SPECULAR]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_SSAO.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_SSAO]},
                new EffectParameterDeclaration { Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_MV", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_IV", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_V", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_ITV", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_P", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "light.position", Value = new float3(0, 0, -1.0f)},
                new EffectParameterDeclaration { Name = "light.positionWorldSpace", Value = new float3(0, 0, -1.0f)},
                new EffectParameterDeclaration { Name = "light.intensities", Value = float4.Zero},
                new EffectParameterDeclaration { Name = "light.maxDistance", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.strength", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.outerConeAngle", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.innerConeAngle", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.direction", Value = float3.Zero},
                new EffectParameterDeclaration { Name = "light.lightType", Value = 1},
                new EffectParameterDeclaration { Name = "light.isActive", Value = 1},
                new EffectParameterDeclaration { Name = "light.isCastingShadows", Value = 0},
                new EffectParameterDeclaration { Name = "light.bias", Value = 0.0f},
                new EffectParameterDeclaration { Name = "PassNo", Value = 0},
                new EffectParameterDeclaration { Name = "BackgroundColor", Value = backgroundColor},
                new EffectParameterDeclaration { Name = "SsaoOn", Value = 1},
            };
        }

        /// <summary>
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMap">The shadow map.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>            
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, WritableTexture shadowMap, float4 backgroundColor)
        {
            var effectParams = DefferedLightingEffectParams(srcRenderTarget, backgroundColor);

            effectParams.Add(new EffectParameterDeclaration { Name = "LightSpaceMatrix", Value = new float4x4[] { } });
            effectParams.Add(new EffectParameterDeclaration { Name = "ShadowMap", Value = shadowMap });

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = DeferredLightningVS(),
                    PS = DeferredLightningFS(lc),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                        BlendOperation = BlendOperation.Add,
                        SourceBlend = Blend.One,
                        DestinationBlend = Blend.One,
                        ZFunc = Compare.LessEqual,
                    }
                }
            },
            effectParams.ToArray());
        }

        /// <summary>
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass. Shadow is calculated with cascaded shadow maps.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMaps">The cascaded shadow maps.</param>
        /// <param name="clipPlanes">The clip planes of the frustums. Each frustum is associated with one shadow map.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>            
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, WritableTexture[] shadowMaps, float2[] clipPlanes, int numberOfCascades,float4 backgroundColor)
        {
            var effectParams = DefferedLightingEffectParams(srcRenderTarget, backgroundColor);

            effectParams.Add(new EffectParameterDeclaration { Name = "LightSpaceMatrix", Value = new float4x4[] { } });
            effectParams.Add(new EffectParameterDeclaration { Name = "ShadowMaps[0]", Value = shadowMaps });
            effectParams.Add(new EffectParameterDeclaration { Name = "ClipPlanes[0]", Value = clipPlanes });

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = DeferredLightningVS(),
                    PS = DeferredLightningFSCascaded(lc, numberOfCascades),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                        BlendOperation = BlendOperation.Add,
                        SourceBlend = Blend.One,
                        DestinationBlend = Blend.One,
                        ZFunc = Compare.LessEqual,
                    }
                }
            },
            effectParams.ToArray());
        }

        /// <summary>
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMap">The shadow map.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>       
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, WritableCubeMap shadowMap, float4 backgroundColor)
        {
            var effectParams = DefferedLightingEffectParams(srcRenderTarget, backgroundColor);

            effectParams.Add(new EffectParameterDeclaration { Name = "ShadowCubeMap", Value = shadowMap });

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = DeferredLightningVS(),
                    PS = DeferredLightningFS(lc),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                        BlendOperation = BlendOperation.Add,
                        SourceBlend = Blend.One,
                        DestinationBlend = Blend.One,
                        ZFunc = Compare.LessEqual,
                    }
                }
            },
            effectParams.ToArray());
        }

        /// <summary>
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>  
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>       
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, float4 backgroundColor)
        {
            var effectParams = DefferedLightingEffectParams(srcRenderTarget, backgroundColor);

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = DeferredLightningVS(),
                    PS = DeferredLightningFS(lc),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                        BlendOperation = BlendOperation.Add,
                        SourceBlend = Blend.One,
                        DestinationBlend = Blend.One,
                        ZFunc = Compare.LessEqual,


                    }
                }
            },
            effectParams.ToArray());
        }

        /// <summary>
        /// ShaderEffect that renders the depth map from a lights point of view - this depth map is used as a shadow map.
        /// </summary>
        /// <returns></returns>
        public static ShaderEffect ShadowCubeMapEffect(float4x4[] lightSpaceMatrices)
        {
            // Vertex shader ------------------------------
            var vert = new StringBuilder();
            vert.AppendLine("#version 330 core");

            vert.Append(@"                
            
            uniform mat4 FUSEE_M;              
            in vec3 fuVertex; 
            ");

            vert.Append(@"
            void main() 
            {                
                gl_Position = FUSEE_M * vec4(fuVertex, 1.0);               

            }");

            //Geometry shader ------------------------------
            var geom = new StringBuilder();
            geom.AppendLine("#version 330 core");
            geom.Append(@"
                layout (triangles) in;
                layout (triangle_strip, max_vertices=18) out;

                uniform mat4 LightSpaceMatrices[6];

                out vec4 FragPos;

                void main()
                {
                    for(int face = 0; face < 6; face++)
                    {
                        gl_Layer = face; // built-in variable that specifies to which face we render.
                        for(int i = 0; i < 3; ++i) // for each triangle's vertices
                        {
                            FragPos = gl_in[i].gl_Position;
                            gl_Position = LightSpaceMatrices[face] * FragPos;
                            EmitVertex();
                        }    
                        EndPrimitive();
                    }
                }  

            ");

            // Fragment shader ------------------------------
            var frag = new StringBuilder();
            frag.Append("#version 330 core\n");

            frag.Append("in vec4 FragPos;\n");
            frag.Append("uniform vec2 LightMatClipPlanes;\n");
            frag.Append("uniform vec3 LightPos;\n");

            frag.Append(@"
            void main()
            {
                // get distance between fragment and light source
                float lightDistance = length(FragPos.xyz - LightPos);
    
                // map to [0;1] range by dividing by far_plane
                lightDistance = lightDistance / LightMatClipPlanes.y;
    
                // write this as modified depth                
                gl_FragDepth = lightDistance;
            }  
                
            ");

            var effectParamDecls = new List<EffectParameterDeclaration>
            {
                new EffectParameterDeclaration { Name = "FUSEE_M", Value = float4x4.Identity },
                new EffectParameterDeclaration { Name = "FUSEE_V", Value = float4x4.Identity },
                new EffectParameterDeclaration { Name = "LightMatClipPlanes", Value = float2.One },
                new EffectParameterDeclaration { Name = "LightPos", Value = float3.One },
                new EffectParameterDeclaration { Name = $"LightSpaceMatrices[0]", Value = lightSpaceMatrices }
            };

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    GS = geom.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                        CullMode = Cull.Clockwise,
                        ZFunc = Compare.LessEqual,
                    }
                }
            },
            effectParamDecls.ToArray());
        }

        /// <summary>
        /// ShaderEffect that renders the depth map from a lights point of view - this depth map is used as a shadow map.
        /// </summary>
        /// <returns></returns>
        public static ShaderEffect ShadowMapEffect()
        {
            // Vertex shader ------------------------------
            var vert = new StringBuilder();
            vert.Append(Version());
            vert.Append(EsPrecision());

            vert.Append(@"
                
            uniform mat4 LightSpaceMatrix;
            uniform mat4 FUSEE_M;              
            in vec3 fuVertex; 
            ");

            vert.Append(@"
            void main() 
            {                
                gl_Position = LightSpaceMatrix* FUSEE_M * vec4(fuVertex, 1.0);               

            }");

            // Fragment shader ------------------------------
            var frag = new StringBuilder();
            frag.Append(Version());
            frag.Append("#extension GL_ARB_explicit_uniform_location : enable\n");
            frag.Append(EsPrecision());

            frag.Append($"layout (location = {0}) out vec4 {Enum.GetName(typeof(RenderTargetTextureTypes), (int)RenderTargetTextureTypes.G_DEPTH)};\n");            
            frag.Append("uniform int LightType;\n");

            frag.Append(@"void main()
            {  
                float d = gl_FragCoord.z;
                
            ");
            frag.AppendLine($" {Enum.GetName(typeof(RenderTargetTextureTypes), (int)RenderTargetTextureTypes.G_DEPTH)} = vec4(d, d, d, 1.0);\n");
            frag.Append(@"}
            ");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                        CullMode = Cull.Clockwise,
                        ZFunc = Compare.LessEqual,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration { Name = "FUSEE_M", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "LightSpaceMatrix", Value = float4x4.Identity},                
                new EffectParameterDeclaration { Name = "LightType", Value = 0},
            });
        }

        private static string ShadowCalculation()
        {
            return @"
                
            float ShadowCalculation(sampler2D shadowMap, vec4 fragPosLightSpace, vec3 normal, vec3 lightDir, float bias)
            {
                float shadow = 0.0;

                // perform perspective divide
                vec4 projCoords = fragPosLightSpace / fragPosLightSpace.w;
                projCoords = projCoords * 0.5 + 0.5; 
                //float closestDepth = texture(shadowMap, projCoords.xy).r;
                float currentDepth = projCoords.z;  

                float thisBias = max(bias * (1.0 - dot(normal, lightDir)), bias/100.0);
            
                vec2 texelSize = 1.0 / vec2(textureSize(shadowMap, 0));
                
                //use this for using sampler2DShadow (automatic PCF) instead of sampler2D
                //float depth = texture(shadowMap, projCoords.xyz).r; 
                //shadow += (currentDepth - thisBias) > depth ? 1.0 : 0.0;
                
                for(int x = -1; x <= 1; ++x)
                {
                    for(int y = -1; y <= 1; ++y)
                    {
                        float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r; 
                        shadow += (currentDepth - thisBias) > pcfDepth ? 1.0 : 0.0;        
                    }    
                }
                shadow /= 9.0;

                return shadow;
            }

            ";
        }

        private static string ShadowCalculationCubeMap()
        {
            return @"
                
            float ShadowCalculationCubeMap(samplerCube shadowMap, vec3 fragPos, vec3 lightPos, float farPlane, vec3 normal, vec3 lightDir, float bias)
            {
                vec3 sampleOffsetDirections[20] = vec3[]
                (
                   vec3( 1,  1,  1), vec3( 1, -1,  1), vec3(-1, -1,  1), vec3(-1,  1,  1), 
                   vec3( 1,  1, -1), vec3( 1, -1, -1), vec3(-1, -1, -1), vec3(-1,  1, -1),
                   vec3( 1,  1,  0), vec3( 1, -1,  0), vec3(-1, -1,  0), vec3(-1,  1,  0),
                   vec3( 1,  0,  1), vec3(-1,  0,  1), vec3( 1,  0, -1), vec3(-1,  0, -1),
                   vec3( 0,  1,  1), vec3( 0, -1,  1), vec3( 0, -1, -1), vec3( 0,  1, -1)
                );

                // get vector between fragment position and light position
                vec3 fragToLight = (fragPos - lightPos) * -1.0;                
                // now get current linear depth as the length between the fragment and light position
                float currentDepth = length(fragToLight);

                float shadow = 0.0;
                float thisBias   = max(bias * (1.0 - dot(normal, lightDir)), bias * 0.01);//0.15;
                int samples  = 20;
                vec3 camPos = FUSEE_IV[3].xyz;
                float viewDistance = length(camPos - fragPos);

                float diskRadius = (1.0 + (viewDistance / farPlane)) / 25.0;
                for(int i = 0; i < samples; ++i)
                {
                    float closestDepth = texture(shadowMap, fragToLight + sampleOffsetDirections[i] * diskRadius).r;
                    closestDepth *= farPlane;   // Undo mapping [0;1]
                    if(currentDepth - thisBias > closestDepth)
                        shadow += 1.0;
                }
                shadow /= float(samples);
                return shadow;
            }

            ";
        }

        #endregion

        #region Make ShaderEffect
        /// <summary>
        ///     Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffect MakeShaderEffect(float4 diffuseColor, float4 specularColor, float shininess, float specularIntensity = 0.5f)
        {
            MaterialComponent temp = new MaterialComponent
            {
                Diffuse = new MatChannelContainer
                {
                    Color = diffuseColor
                },
                Specular = new SpecularChannelContainer
                {
                    Color = specularColor,
                    Shininess = shininess,
                    Intensity = specularIntensity,
                }
            };

            return MakeShaderEffectFromMatComp(temp);
        }

        /// <summary>
        ///     Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="texName">Name of the texture you want to use.</param>
        /// <param name="diffuseMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffect MakeShaderEffect(float4 diffuseColor, float4 specularColor, float shininess, string texName, float diffuseMix, float specularIntensity = 0.5f)
        {
            MaterialComponent temp = new MaterialComponent
            {
                Diffuse = new MatChannelContainer
                {
                    Color = diffuseColor,
                    Texture = texName,
                    Mix = diffuseMix
                },
                Specular = new SpecularChannelContainer
                {
                    Color = specularColor,
                    Shininess = shininess,
                    Intensity = specularIntensity,
                }
            };

            return MakeShaderEffectFromMatComp(temp);
        }

        /// <summary> 
        /// Creates a ShaderEffectComponent from a MaterialComponent 
        /// </summary> 
        /// <param name="mc">The MaterialComponent</param> 
        /// <param name="wc">Only pass over a WeightComponent if you use bone animations in the current node (usage: pass currentNode.GetWeights())</param>        
        /// <returns></returns> 
        /// <exception cref="Exception"></exception> 
        public static ShaderEffect MakeShaderEffectFromMatComp(MaterialComponent mc, WeightComponent wc = null)
        {
            ShaderCodeBuilder scb = null;

            //TODO: LightingCalculationMethod does not seem to have an effect right now.. see ShaderCodeBuilder constructor.
            if (mc.GetType() == typeof(MaterialLightComponent))
            {
                if (mc is MaterialLightComponent lightMat) scb = new ShaderCodeBuilder(lightMat, null, wc);
            }
            else if (mc.GetType() == typeof(MaterialPBRComponent))
            {
                if (mc is MaterialPBRComponent pbrMaterial) scb = new ShaderCodeBuilder(pbrMaterial, null, LightingCalculationMethod.SIMPLE, wc);
            }
            else
            {
                scb = new ShaderCodeBuilder(mc, null, wc); // TODO, CurrentNode.GetWeights() != null); 
            }

            var effectParameters = AssembleEffectParamers(mc);

            if (scb == null) throw new Exception("Material could not be evaluated or be built!");
            var ret = new ShaderEffect(new[]
                {
                    new EffectPassDeclaration
                    {
                        VS = scb.VS, 
                        //VS = VsBones, 
                        PS = scb.PS,
                        StateSet = new RenderStateSet
                        {
                            ZEnable = true,
                            AlphaBlendEnable = true,
                            SourceBlend = Blend.SourceAlpha,
                            DestinationBlend = Blend.InverseSourceAlpha,
                            BlendOperation = BlendOperation.Add,
                        }
                    }
                },
                effectParameters
            );
            return ret;
        }

        private static IEnumerable<EffectParameterDeclaration> AssembleEffectParamers(MaterialComponent mc)
        {
            var effectParameters = new List<EffectParameterDeclaration>();

            if (mc.HasDiffuse)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = DiffuseColorName,
                    Value = mc.Diffuse.Color
                });
                if (mc.Diffuse.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = DiffuseMixName,
                        Value = mc.Diffuse.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = DiffuseTextureName,
                        Value = LoadTexture(mc.Diffuse.Texture)
                    });
                }
            }

            if (mc.HasSpecular)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = SpecularColorName,
                    Value = mc.Specular.Color
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = SpecularShininessName,
                    Value = mc.Specular.Shininess
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = SpecularIntensityName,
                    Value = mc.Specular.Intensity
                });
                if (mc.Specular.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = SpecularMixName,
                        Value = mc.Specular.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = SpecularTextureName,
                        Value = LoadTexture(mc.Specular.Texture)
                    });
                }
            }

            if (mc.HasEmissive)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = EmissiveColorName,
                    Value = mc.Emissive.Color
                });
                if (mc.Emissive.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = EmissiveMixName,
                        Value = mc.Emissive.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = EmissiveTextureName,
                        Value = LoadTexture(mc.Emissive.Texture)
                    });
                }
            }

            if (mc.HasBump)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = BumpIntensityName,
                    Value = mc.Bump.Intensity
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = BumpTextureName,
                    Value = LoadTexture(mc.Bump.Texture)
                });
            }

            for (int i = 0; i < _numberOfLightsForward; i++)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].position",
                    Value = new float3(0, 0, -1.0f)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].intensities",
                    Value = float4.Zero
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].maxDistance",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].strength",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].outerConeAngle",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].innerConeAngle",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].direction",
                    Value = float3.Zero
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].lightType",
                    Value = 1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].isActive",
                    Value = 1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].isCastingShadows",
                    Value = 0
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].bias",
                    Value = 0f
                });
            }

            // FUSEE_ PARAMS
            // TODO: Just add the necessary ones!
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_M",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_MV",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_MVP",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_ITMV",
                Value = float4x4.Identity
            });

            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_IMV",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_V",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_P",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_BONES",
                Value = new[] { float4x4.Identity }
            });

            return effectParameters;
        }

        private static Texture LoadTexture(string path)
        {
            var image = AssetStorage.Get<ImageData>(path);
            if (image != null)
                return new Texture(image);

            image = AssetStorage.Get<ImageData>("DefaultTexture.png");
            if (image != null)
                return new Texture(image);

            return new Texture(new ImageData());
        }

        #endregion

        #region StaticUniformVariablesNames

        /// <summary>
        /// The var name for the uniform DiffuseColor variable within the pixelshaders
        /// </summary>
        public static string AmbientStrengthName { get; } = "AmbientStrength";

        /// <summary>
        /// The var name for the uniform DiffuseColor variable within the pixelshaders
        /// </summary>
        public static string DiffuseColorName { get; } = "DiffuseColor";

        /// <summary>
        /// The var name for the uniform SpecularColor variable within the pixelshaders
        /// </summary>
        public static string SpecularColorName { get; } = "SpecularColor";

        /// <summary>
        /// The var name for the uniform EmissiveColor variable within the pixelshaders
        /// </summary>
        public static string EmissiveColorName { get; } = "EmissiveColor";


        /// <summary>
        /// The var name for the uniform DiffuseTexture variable within the pixelshaders
        /// </summary>
        public static string DiffuseTextureName { get; } = "DiffuseTexture";

        /// <summary>
        /// The var name for the uniform SpecularTexture variable within the pixelshaders
        /// </summary>
        public static string SpecularTextureName { get; } = "SpecularTexture";

        /// <summary>
        /// The var name for the uniform EmissiveTexture variable within the pixelshaders
        /// </summary>
        public static string EmissiveTextureName { get; } = "EmissiveTexture";

        /// <summary>
        /// The var name for the uniform BumpTexture variable within the pixelshaders
        /// </summary>
        public static string BumpTextureName { get; } = "BumpTexture";

        /// <summary>
        /// The var name for the uniform DiffuseMix variable within the pixelshaders
        /// </summary>
        public static string DiffuseMixName { get; } = "DiffuseMix";

        /// <summary>
        /// The var name for the uniform SpecularMix variable within the pixelshaders
        /// </summary>
        public static string SpecularMixName { get; } = "SpecularMix";

        /// <summary>
        /// The var name for the uniform EmissiveMix variable within the pixelshaders
        /// </summary>
        public static string EmissiveMixName { get; } = "EmissiveMix";

        /// <summary>
        /// The var name for the uniform SpecularShininess variable within the pixelshaders
        /// </summary>
        public static string SpecularShininessName { get; } = "SpecularShininess";

        /// <summary>
        /// The var name for the uniform SpecularIntensity variable within the pixelshaders
        /// </summary>
        public static string SpecularIntensityName { get; } = "SpecularIntensity";

        /// <summary>
        /// The var name for the uniform BumpIntensity variable within the pixelshaders
        /// </summary>
        public static string BumpIntensityName { get; } = "BumpIntensity";

        /// <summary>
        /// The var name for the uniform LightDirection variable within the pixelshaders
        /// </summary>
        [Obsolete("LightDirection is no longer in use, adress: uniform Light allLights[MAX_LIGHTS]")]
        public static string LightDirectionName { get; } = "LightDirection";

        /// <summary>
        /// The var name for the uniform LightColor variable within the pixelshaders
        /// </summary>
        [Obsolete("LightColor is no longer in use, adress: uniform Light allLights[MAX_LIGHTS]")]
        public static string LightColorName { get; } = "LightColor";

        /// <summary>
        /// The var name for the uniform LightIntensity variable within the pixelshaders
        /// </summary>
        [Obsolete("LightIntensity is no longer in use, adress: uniform Light allLights[MAX_LIGHTS]")]
        public static string LightIntensityName { get; } = "LightIntensity";

        #endregion

    }
}