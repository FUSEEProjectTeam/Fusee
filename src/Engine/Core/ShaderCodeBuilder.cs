using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Fusee.Base.Core;
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

        public static string CreateVarying(Type type, string varName)
        {
            return $"varying {DecodeType(type)} {varName};";
        }

        public static string CreateAttribute(Type type, string varName)
        {
            return $"attribute {DecodeType(type)} {varName};";
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
    /// Compiler for ShaderCode. Takes a MaterialComponent, evaluates input parameters and creates pixel and vertexshader
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

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// The complete VertexShader
        /// </summary>
        public string VS { get; }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// The complete Pixelshader
        /// </summary>
        public string PS { get; }

        /// <summary>
        /// LEGACY CONSTRUCTOR
        /// Creates vertex and pixel shader for given material, mesh, weight; light calculation is simple per default
        /// </summary>
        /// <param name="mc">The MaterialCpmponent</param>
        /// <param name="mesh">The Mesh</param>
        /// <param name="wc">Teh WeightComponent</param>
        /// <param name="renderWithShadows">Should the resulting shader include shadowcalculation</param>
        public ShaderCodeBuilder(MaterialComponent mc, Mesh mesh, WeightComponent wc = null,
            bool renderWithShadows = false)
            : this(mc, mesh, LightingCalculationMethod.SIMPLE, wc, renderWithShadows)
        {
        }

        /// <summary>
        /// Creates vertex and pixel shader for given material, mesh, weight; light calculation is simple per default
        /// </summary>
        /// <param name="mc">The MaterialCpmponent</param>
        /// <param name="mesh">The Mesh</param>
        /// <param name="wc">The WeightComponent</param>
        /// <param name="lightingCalculation">Method of light calculation; simple BLINN PHONG or advanced physically based</param>
        /// <param name="renderWithShadows">Should the resulting shader include shadowcalculation</param>
        public ShaderCodeBuilder(MaterialComponent mc, Mesh mesh,
            LightingCalculationMethod lightingCalculation = LightingCalculationMethod.SIMPLE,
            WeightComponent wc = null, bool renderWithShadows = false)
        {
            // Set Lightingcalculation & shadow
            _lightingCalculationMethod = lightingCalculation;
            _renderWithShadows = renderWithShadows;

            _vertexShader = new List<string>();
            _pixelShader = new List<string>();

            AnalyzeMaterialType(mc);
            AnalyzeMesh(mesh, wc);
            AnalzyeMaterialParams(mc);
            CreateVertexShader(wc);
            VS = string.Join("\n", _vertexShader);
            CreatePixelShader_new(mc);
            PS = string.Join("\n", _pixelShader);
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
                HasWeightMap = wc != null
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
                _vertexShader.Add(GLSL.CreateAttribute(Type.Vec3, "fuVertex"));

            if (_materialProbs.HasSpecular)
                _vertexShader.Add(GLSL.CreateVarying(Type.Vec3, "vViewDir"));

            if (_meshProbs.HasWeightMap)
            {
                _vertexShader.Add(GLSL.CreateAttribute(Type.Vec4, "fuBoneIndex"));
                _vertexShader.Add(GLSL.CreateAttribute(Type.Vec4, "fuBoneWeight"));
            }

            if (_meshProbs.HasNormals)
            {
                _vertexShader.Add(GLSL.CreateAttribute(Type.Vec3, "fuNormal"));
                _vertexShader.Add(GLSL.CreateVarying(Type.Vec3, "vNormal"));
            }

            if (_meshProbs.HasUVs)
            {
                _vertexShader.Add(GLSL.CreateAttribute(Type.Vec2, "fuUV"));
                _vertexShader.Add(GLSL.CreateVarying(Type.Vec2, "vUV"));
            }

            if (_meshProbs.HasColors)
            {
                _vertexShader.Add(GLSL.CreateAttribute(Type.Vec4, "fuColor"));
                _vertexShader.Add(GLSL.CreateVarying(Type.Vec4, "vColors"));
            }

            _vertexShader.Add(GLSL.CreateVarying(Type.Vec3, "viewPos"));
            _vertexShader.Add(GLSL.CreateVarying(Type.Vec3, "vMVNormal"));

            if (_renderWithShadows)
                _vertexShader.Add(GLSL.CreateVarying(Type.Vec4, "shadowLight"));

        }

        private void AddVertexUniforms(WeightComponent wc)
        {
            _vertexShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_MVP"));

            if (_meshProbs.HasNormals)
                _vertexShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_ITMV"));

            if (_materialProbs.HasSpecular)
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
                // TODO: Is it a hack to invert Model AND View? Should we rather only invert MODEL (and NOT VIEW)??
                _vertexShader.Add("vMVNormal = mat3(FUSEE_ITMV) * newNormal.xyz;");
            }

            if (_materialProbs.HasSpecular)
            {
                _vertexShader.Add("vec3 viewPos = FUSEE_IMV[3].xyz;");

                _vertexShader.Add(_meshProbs.HasWeightMap
                    ? "vViewDir = normalize(viewPos - vec3(newVertex));"
                    : "vViewDir = normalize(viewPos - fuVertex);");
            }

            if (_meshProbs.HasUVs)
                _vertexShader.Add("vUV = fuUV;");

            if (_meshProbs.HasNormals && !_meshProbs.HasWeightMap)
                _vertexShader.Add("vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);");

            _vertexShader.Add("viewPos = (FUSEE_MV * vec4(fuVertex, 1.0)).xyz;");

            if (_renderWithShadows)
                _vertexShader.Add("shadowLight = shadowMVP * viewPos;");

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

            // Define number of lights
            var numberOfLights = SceneRenderer.AllLightResults.Count > 0 ? SceneRenderer.AllLightResults.Count : 1;

            // legacy code, should be larger one by default!
            _pixelShader.Add($"#define MAX_LIGHTS {numberOfLights}");
            _pixelShader.Add(LightStructDeclaration());

            _pixelShader.Add(GLSL.CreateVarying(Type.Vec3, "vViewDir"));

            if (_meshProbs.HasNormals)
            {
                _pixelShader.Add(GLSL.CreateVarying(Type.Vec3, "vMVNormal"));
                _pixelShader.Add(GLSL.CreateVarying(Type.Vec3, "vNormal"));
            }

          

            if (_meshProbs.HasUVs)
                _pixelShader.Add(GLSL.CreateVarying(Type.Vec2, "vUV"));

            _pixelShader.Add(GLSL.CreateVarying(Type.Vec3, "viewPos"));

            if (_renderWithShadows)
                _pixelShader.Add(GLSL.CreateVarying(Type.Vec4, "shadowLight"));

        }

        private void AddPixelUniforms()
        {
            _pixelShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_MV"));
            _pixelShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_IMV"));
            _pixelShader.Add(GLSL.CreateUniform(Type.Mat4, "FUSEE_IV"));

            if(_materialProbs.HasBump)
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
                _pixelShader.Add(GLSL.CreateUniform(Type.Vec3, SpecularColorName));
            }

            if (_materialProbs.HasBump)
            {
                _pixelShader.Add(GLSL.CreateUniform(Type.Sampler2D, BumpTextureName));
                _pixelShader.Add(GLSL.CreateUniform(Type.Float, BumpIntensityName));
            }

            if (_materialProbs.HasDiffuse)
                _pixelShader.Add(GLSL.CreateUniform(Type.Vec3, DiffuseColorName));

            //if (_materialProbs.HasDiffuseTexture)
            //{
                _pixelShader.Add(GLSL.CreateUniform(Type.Sampler2D, DiffuseTextureName));
                _pixelShader.Add(GLSL.CreateUniform(Type.Float, DiffuseMixName));
            //}

            if (_materialProbs.HasEmissive)
                _pixelShader.Add(GLSL.CreateUniform(Type.Vec3, EmissiveColorName));

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
                _materialProbs.HasEmissive
                    ? $"return vec3({EmissiveColorName} * ambientCoefficient);"
                    : "return vec3(ambientCoefficient);"
            };

            _pixelShader.Add(GLSL.CreateMethod(Type.Vec3, "ambientLighting",
                new[] { GLSL.CreateVar(Type.Float, "ambientCoefficient") }, methodBody));
        }

        private void AddDiffuseLightMethod()
        {
            var methodBody = new List<string>
            {
                "float diffuseTerm = dot(N, L);"
            };

            if (_materialProbs.HasDiffuseTexture)
                methodBody.Add(
                    $"return texture2D({DiffuseTextureName}, vUV).rgb * {DiffuseMixName} *  max(diffuseTerm, 0.0) * intensities;");
            else
                methodBody.Add($"return ({DiffuseColorName} * intensities * diffuseTerm);");

            _pixelShader.Add(GLSL.CreateMethod(Type.Vec3, "diffuseLighting",
                new[]
                {
                    GLSL.CreateVar(Type.Vec3, "N"), GLSL.CreateVar(Type.Vec3, "L"),
                    GLSL.CreateVar(Type.Vec3, "intensities")
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
                $"return ({SpecularColorName} * {SpecularIntensityName} * intensities) * specularTerm;"
            };

            _pixelShader.Add(GLSL.CreateMethod(Type.Vec3, "specularLighting",
                new[]
                {
                    GLSL.CreateVar(Type.Vec3, "N"), GLSL.CreateVar(Type.Vec3, "L"), GLSL.CreateVar(Type.Vec3, "V"),
                    GLSL.CreateVar(Type.Vec3, "intensities")
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
                "float pcfDepth = texture2D(firstPassTex, projCoords.xy).r;",
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

            var bumpNormals = new List<string>
            {
                "// First implementation ONLY working with object space normals. See",
                "// http://gamedev.stackexchange.com/a/72806/44105",
                "// http://docs.cryengine.com/display/SDKDOC4/Tangent+Space+Normal+Mapping",
                "// http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-13-normal-mapping/",
                "vec3 bv =  normalize(texture2D(BumpTexture, vUV).xyz * 2.0 - 1.0);",
                "bv = vec3(bv.x, bv.y, -bv.z);",
                "vec3 N =  normalize(bv);"
            };

            var normals = new List<string>
            {
                "vec3 N = normalize(vMVNormal);"
            };

            var applyLightParamsWithoutNormals = new List<string>
            {
                //"vec3 N = normalize(vMVNormal);",
                "vec3 L = normalize(position - viewPos.xyz);",
                "vec3 V = normalize(-viewPos.xyz);",
                "if(lightType == 3) {",
                "   L = normalize(vec3(0.0,0.0,-1.0));",
                "   V = vec3(0);",
                "}",
                "vec2 o_texcoords = vUV;",
                "",
                _renderWithShadows ? "float shadowFactor = CalcShadowFactor(shadowLight);" : "",
                "",
                "vec3 Idif = vec3(0);",
                "vec3 Ispe = vec3(0);",
                ""
            };

            var applyLightParams = new List<string>();
            applyLightParams.AddRange(_materialProbs.HasBump ? bumpNormals : normals);

            applyLightParams.AddRange(applyLightParamsWithoutNormals);


            if (_materialProbs.HasDiffuse)
                applyLightParams.Add("Idif = diffuseLighting(N, L, intensities);");


            if (_materialProbs.HasSpecular)
                applyLightParams.Add("Ispe = specularLighting(N, L, V, intensities);");

            applyLightParams.Add("vec3 Iamb = ambientLighting(ambientCoefficient);");


            var attenuation = new List<string>()
            {
                "float distanceToLight = distance(position, viewPos.xyz) / 1000.0;",
                "float distance = pow(distanceToLight/attenuation,4.0);",
                "float att = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);"
            };

            var pointLight = new List<string>()
            {
                _renderWithShadows
                    ? "result = Iamb + (1.0-shadowFactor) * (Idif + Ispe) * att;"
                    : "result = Iamb + (Idif + Ispe) * att;"
            };

            var parallelLight = new List<string>()
            {
                _renderWithShadows
                    ? "result = Iamb + (1.0-shadowFactor) * (Idif + Ispe);"
                    : "result =  Iamb + (Idif + Ispe);"
            };

            var spotLight = new List<string>()
            {
                "float lightToSurfaceAngle = dot(-L, coneDirection);",
                "if (lightToSurfaceAngle > coneAngle)",
                "{",
                "   att *= (1.0 - (1.0 - lightToSurfaceAngle) * 1.0/(1.0 - coneAngle));",
                "}",
                "else",
                "{",
                "   att = 0.0;",
                "}",
                "",
                _renderWithShadows
                    ? "result = Iamb + (1.0-shadowFactor) * (Idif + Ispe) * att;"
                    : "result = Iamb + (Idif + Ispe) * att;"
            };

            // - Disable GammaCorrection for better colors
            /*var gammaCorrection = new List<string>() 
            {
                "vec3 gamma = vec3(1.0/2.2);",
                "result = pow(result, gamma);"
            };*/

            var methodBody = new List<string>();
            methodBody.AddRange(applyLightParams);
            methodBody.Add("vec3 result = vec3(0);");
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
            methodBody.Add("return result;");

            _pixelShader.Add(GLSL.CreateMethod(Type.Vec3, "ApplyLight",
                new[]
                {
                    GLSL.CreateVar(Type.Vec3, "position"), GLSL.CreateVar(Type.Vec3, "intensities"),
                    GLSL.CreateVar(Type.Vec3, "coneDirection"), GLSL.CreateVar(Type.Float, "attenuation"),
                    GLSL.CreateVar(Type.Float, "ambientCoefficient"), GLSL.CreateVar(Type.Float, "coneAngle"),
                    GLSL.CreateVar(Type.Int, "lightType")
                }, methodBody));
        }


        //private void AddPbrDiffuseLightMethod(MaterialPBRComponent mc)
        //{

        //    var nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
        //    var delta = 0.0000001;
        //    var k = mc.DiffuseFraction + delta;

        //    var methodBody = new List<string>
        //    {
        //        "float diffuseTerm = dot(N, L);",
        //        $"float k = {k.ToString(nfi)};"
        //    };

        //    if (_materialProbs.HasDiffuseTexture)
        //        methodBody.Add($"return texture2D({DiffuseTextureName}, vUV).rgb * {DiffuseMixName} *  max(diffuseTerm, 0.0) * (1.0-k) * intensities;");
        //    else
        //        methodBody.Add($"return ({DiffuseColorName} * intensities * max(diffuseTerm, 0.0) * (1.0-k));");

        //    _pixelShader.Add(GLSL.CreateMethod(Type.Vec3, "diffuseLighting",
        //        new[] { GLSL.CreateVar(Type.Vec3, "N"), GLSL.CreateVar(Type.Vec3, "L"), GLSL.CreateVar(Type.Vec3, "intensities") }, methodBody));
        //}

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
                "     float k= roughnessValue * sqrt(2.0/3.14159265);",
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

            _pixelShader.Add(GLSL.CreateMethod(Type.Vec3, "specularLighting",
                new[]
                {
                    GLSL.CreateVar(Type.Vec3, "N"), GLSL.CreateVar(Type.Vec3, "L"), GLSL.CreateVar(Type.Vec3, "V"),
                    GLSL.CreateVar(Type.Vec3, "intensities")
                }, methodBody));
        }

        private void AddPixelBody()
        {
            var methodBody = new List<string>
            {
                "vec3 result = vec3(0.0);",
                "for(int i = 0; i < MAX_LIGHTS;i++)",
                "{",
                "vec3 currentPosition = allLights[i].position;",
                "vec3 currentIntensities = allLights[i].intensities;",
                "vec3 currentConeDirection = allLights[i].coneDirection;",
                "float currentAttenuation = allLights[i].attenuation;",
                "float currentAmbientCoefficient = allLights[i].ambientCoefficient;",
                "float currentConeAngle = allLights[i].coneAngle;",
                "int currentLightType = allLights[i].lightType; ",
                "result += ApplyLight(currentPosition, currentIntensities, currentConeDirection, ",
                "currentAttenuation, currentAmbientCoefficient, currentConeAngle, currentLightType);",
                "}",
                $"gl_FragColor = vec4(result, 1.0);"
            };

            _pixelShader.Add(GLSL.CreateMethod(Type.Void, "main",
                new[] { "" }, methodBody));
        }


        private static string EsPrecision()
        {
            return "#ifdef GL_ES\n" +
                   "    precision highp float;\n" +
                   "#endif\n\n";
        }

        private static string LightStructDeclaration()
        {
            return @"
            struct Light 
            {
                vec3 position;
                vec3 intensities;
                vec3 coneDirection;
                float attenuation;
                float ambientCoefficient;
                float coneAngle;
                int lightType;
            };
            uniform Light allLights[MAX_LIGHTS];
            ";
        }

        private static string Version()
        {
            return "#version 100\n";
        }

        #endregion

        #region Make ShaderEffect 

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

            // If MaterialLightComponent is found call the LegacyShaderCodeBuilder with the MaterialLight 
            // The LegacyShaderCodeBuilder is intelligent enough to handle all the necessary compilations needed for the VS & PS 
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
                            AlphaBlendEnable = false
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

            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].position",
                Value = new float3(0, 0, -1.0f)
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].intensities",
                Value = float3.Zero
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].attenuation",
                Value = 0.0f
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].ambientCoefficient",
                Value = 0.0f
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].coneAngle",
                Value = 0.0f
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].coneDirection",
                Value = float3.Zero
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].lightType",
                Value = 1.0f
            });

            // FUSEE_ PARAMS
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
                    Name = "FUSEE_BONES[BONES]",
                    Value = float4x4.Identity
            });

            return effectParameters;
        }

        private static Texture LoadTexture(string path)
        {
            var image = AssetStorage.Get<ImageData>(path);
            return new Texture(image);
        }

        #endregion

        #region StaticUniformVariablesNames

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

