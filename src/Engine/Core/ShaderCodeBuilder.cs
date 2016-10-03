using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum LightningMethod
    {
        BLINN_PHONG,
        COOK_TORRANCE
    }

    internal class ShaderCodeBuilder
    {
        public LightningMethod LightningCalulationMethod;

        private bool _hasVertices, _hasNormals, _hasUVs, _hasColors;
        private bool _hasDiffuse, _hasSpecular, _hasEmissive, _hasBump;
        private bool _hasDiffuseTexture, _hasSpecularTexture, _hasEmissiveTexture;
        private readonly bool _hasWeightMap;
        private readonly int _nBones;
        private readonly bool _normalizeNormals;

        /*
        struct SurfaceOutput {
            half3 Albedo;
            half3 Normal;
            half3 Emission;
            half Specular;
            half Gloss;
            half Alpha;
        };
        */

        public ShaderCodeBuilder(MaterialComponent mc, MeshComponent mesh, string applyLightString,
            WeightComponent wc = null)
        {
            if (wc != null)
            {
                _hasWeightMap = true;
                _nBones = wc.Joints.Count;
            }
            else
            {
                _nBones = 0;
            }

            float f1 = 1;
            f1.GetType();
            _normalizeNormals = true;
            if (mesh != null)
                AnalyzeMesh(mesh);
            else
            {
                _hasVertices = _hasNormals = _hasUVs = true;
            }
            AnalyzeMaterial(mc);

            var vs = new StringBuilder();
            MeshInputDeclarations(vs);
            MatrixDeclarations(vs);
            VSBody(vs);
            VS = vs.ToString();

            var ps = new StringBuilder();
            PixelInputDeclarations(ps);
            PSBodyWithLight(ps, applyLightString);
            PS = ps.ToString();
        }


        public ShaderCodeBuilder(MaterialComponent mc, MeshComponent mesh, LightningMethod lightningCalulationMethod, WeightComponent wc = null)
        {
            LightningCalulationMethod = lightningCalulationMethod;

            if (wc != null)
            {
                _hasWeightMap = true;
                _nBones = wc.Joints.Count;
            }
            else
            {
                _nBones = 0;
            }

            float f1 = 1;
            f1.GetType();
            _normalizeNormals = true;
            if (mesh != null)
                AnalyzeMesh(mesh);
            else
            {
                _hasVertices = _hasNormals = _hasUVs = true;
            }
            AnalyzeMaterial(mc);

            var vs = new StringBuilder();
            MeshInputDeclarations(vs);
            MatrixDeclarations(vs);
            VSBody(vs);
            VS = vs.ToString();

            var ps = new StringBuilder();
            PixelInputDeclarations(ps);
            PSBody(ps);
            PS = ps.ToString();
        }

        private void AnalyzeMesh(MeshComponent mesh)
        {
            _hasVertices = mesh.Vertices != null && mesh.Vertices.Length > 0;
            _hasNormals = mesh.Normals != null && mesh.Normals.Length > 0;
            _hasUVs = mesh.UVs != null && mesh.UVs.Length > 0;
            _hasColors = false;
            // _hasColors = (mesh.Colors != null && mesh.Colors.Length > 0);
        }

        private void AnalyzeMaterial(MaterialComponent mc)
        {
            _hasDiffuse = mc.HasDiffuse;
            if (_hasDiffuse)
                _hasDiffuseTexture = mc.Diffuse.Texture != null;
            _hasSpecular = mc.HasSpecular;
            if (_hasSpecular)
                _hasSpecularTexture = mc.Specular.Texture != null;
            _hasEmissive = mc.HasEmissive;
            if (_hasEmissive)
                _hasEmissiveTexture = mc.Emissive.Texture != null;
            _hasBump = mc.HasBump; // always has a texture...
        }

        private void MeshInputDeclarations(StringBuilder vs)
        {
            if (_hasVertices)
            {
                vs.Append("  attribute vec3 fuVertex;\n");

                if (_hasSpecular)
                {
                    vs.Append("  varying vec3 vViewDir;\n");
                }
            }

            if (_hasWeightMap)
            {
                vs.Append("  attribute vec4 fuBoneIndex;\n");
                vs.Append("  attribute vec4 fuBoneWeight;\n");
            }

            if (_hasNormals)
                vs.Append("  attribute vec3 fuNormal;\n  varying vec3 vNormal;\n");

            if (_hasUVs)
                vs.Append("  attribute vec2 fuUV;\n  varying vec2 vUV;\n");

            if (_hasColors)
                vs.Append("  attribute vec4 fuColor;\n  varying vec4 vColors;\n");
        }

        private void MatrixDeclarations(StringBuilder vs)
        {
            // Lighting done in model space... no need to convert normals
            // if (_hasNormals)
            //    vs.Append("  uniform mat4 FUSEE_ITMV;\n");

            if (_hasSpecular)
            {
                vs.Append("  uniform mat4 FUSEE_IMV;\n");
            }
            // vs.Append("  uniform mat4 FUSEE_MV;\n");
            if (_hasWeightMap)
            {
                vs.Append("uniform mat4 FUSEE_P;\n");
                vs.Append("uniform mat4 FUSEE_V;\n");
            }
            else
            {
                vs.Append("  uniform mat4 FUSEE_MVP;\n");
            }

            if (_hasWeightMap)
                vs.Append("  uniform mat4 FUSEE_BONES[" + _nBones + "];\n");
        }


        private void VSBody(StringBuilder vs)
        {
            vs.Append("\n\n  void main()\n  {\n");
            if (_hasNormals)
            {
                if (_hasWeightMap)
                {
                    vs.Append("    vec4 newVertex;\n");
                    vs.Append("    vec4 newNormal;\n");

                    vs.Append(
                        "    newVertex = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuVertex, 1.0) ) * fuBoneWeight.x ;\n");
                    vs.Append(
                        "    newNormal = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuNormal, 0.0)) * fuBoneWeight.x;\n");

                    vs.Append(
                        "    newVertex = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuVertex, 1.0)) * fuBoneWeight.y + newVertex;\n");
                    vs.Append(
                        "    newNormal = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuNormal, 0.0)) * fuBoneWeight.y + newNormal;\n");

                    vs.Append(
                        "    newVertex = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuVertex, 1.0)) * fuBoneWeight.z + newVertex;\n");
                    vs.Append(
                        "    newNormal = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuNormal, 0.0)) * fuBoneWeight.z + newNormal;\n");

                    vs.Append(
                        "    newVertex = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuVertex, 1.0)) * fuBoneWeight.w + newVertex;\n");
                    vs.Append(
                        "    newNormal = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuNormal, 0.0)) * fuBoneWeight.w + newNormal;\n");

                    // At this point the normal is in World space - transform back to model space
                    // TODO: Is it a hack to invert Model AND View? Should we rather only invert MODEL (and NOT VIEW)??
                    vs.Append("    vNormal = mat3(FUSEE_IMV) * newNormal.xyz;\n");

                    if (_normalizeNormals)
                        vs.Append("    vNormal = normalize(vNormal);\n");
                }
                else
                {
                    // Lighting done in model space... no need to convert normals
                    if (_normalizeNormals)
                        // vs.Append("    vNormal = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);\n");
                        vs.Append("    vNormal = normalize(fuNormal);\n");
                    else
                        vs.Append("    vNormal = fuNormal;\n");
                }
            }

            if (_hasSpecular)
            {
                // vs.Append("    vec4 viewDirTmp = FUSEE_IMV * vec4(0, 0, 0, 1);\n");
                // vs.Append("    vViewDir = viewDirTmp.xyz * 1/viewDirTmp.w;\n");
                vs.Append("    vec3 viewPos = FUSEE_IMV[3].xyz;\n");

                if (_hasWeightMap)
                    vs.Append("    vViewDir = normalize(viewPos - vec3(newVertex));\n");
                else
                    vs.Append("    vViewDir = normalize(viewPos - fuVertex);\n");
                // vs.Append("    vViewDir = vec3(0, -1, 0);\n");
            }

            if (_hasWeightMap)
                vs.Append("    gl_Position = FUSEE_P * FUSEE_V * vec4(vec3(newVertex), 1.0);\n ");
            else
                vs.Append("    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);\n");


            if (_hasUVs)
                vs.Append("    vUV = fuUV;\n");

            vs.Append("  }\n\n");
        }


        //private void VSBody(StringBuilder vs)
        //{
        //    vs.Append("\n\n  void main()\n  {\n");
        //    if (_hasNormals)
        //    {
        //        if (_hasWeightMap){
        //            vs.Append("    vec4 newVertex;\n");
        //            vs.Append("    vec4 newNormal;\n");

        //            vs.Append("    newVertex = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuVertex, 1.0) ) * fuBoneWeight.x ;\n");
        //            vs.Append("    newNormal = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuNormal, 0.0)) * fuBoneWeight.x;\n");

        //            vs.Append("    newVertex = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuVertex, 1.0)) * fuBoneWeight.y + newVertex;\n");
        //            vs.Append("    newNormal = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuNormal, 0.0)) * fuBoneWeight.y + newNormal;\n");

        //            vs.Append("    newVertex = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuVertex, 1.0)) * fuBoneWeight.z + newVertex;\n");
        //            vs.Append("    newNormal = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuNormal, 0.0)) * fuBoneWeight.z + newNormal;\n");

        //            vs.Append("    newVertex = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuVertex, 1.0)) * fuBoneWeight.w + newVertex;\n");
        //            vs.Append("    newNormal = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuNormal, 0.0)) * fuBoneWeight.w + newNormal;\n");

        //            // At this point the normal is in World space - transform back to model space
        //            // TODO: Is it a hack to invert Model AND View? SHould we rather only invert MODEL (and NOT VIEW)??
        //            vs.Append("    vNormal = vec3(FUSEE_IMV * newNormal);\n");

        //            if (_normalizeNormals)
        //                vs.Append("    vNormal = normalize(vNormal);\n");
        //        }                   
        //        else
        //        {
        //            // Lighting done in model space... no need to convert normals
        //            if (_normalizeNormals)
        //                // vs.Append("    vNormal = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);\n");
        //                vs.Append("    vNormal = normalize(fuNormal);\n");
        //            else
        //                vs.Append("    vNormal = fuNormal;\n");
        //        }
        //    }

        //    if (_hasSpecular)
        //    {
        //        // vs.Append("    vec4 viewDirTmp = FUSEE_IMV * vec4(0, 0, 0, 1);\n");
        //        // vs.Append("    vViewDir = viewDirTmp.xyz * 1/viewDirTmp.w;\n");
        //        vs.Append("    vec3 viewPos = FUSEE_IMV[3].xyz;\n");

        //        if (_hasWeightMap)
        //            vs.Append("    vViewDir = normalize(viewPos - vec3(newVertex));\n");
        //        else
        //            vs.Append("    vViewDir = normalize(viewPos - fuVertex);\n");
        //        // vs.Append("    vViewDir = vec3(0, -1, 0);\n");
        //    }

        //    if (_hasWeightMap)
        //        vs.Append("    gl_Position = FUSEE_P * FUSEE_V * vec4(vec3(newVertex), 1.0);\n ");
        //    else
        //        vs.Append("    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);\n");


        //    if (_hasUVs)
        //        vs.Append("    vUV = fuUV;\n");

        //    vs.Append("  }\n\n");
        //}

        private void PixelInputDeclarations(StringBuilder ps)
        {
            if (LightningCalulationMethod == LightningMethod.BLINN_PHONG)
            {
                ps.Append("#ifdef GL_ES\n");
                ps.Append("  precision highp float;\n");
                ps.Append("#endif\n\n");

                ChannelInputDeclaration(ps, _hasDiffuse, _hasDiffuseTexture, "Diffuse");
                SpecularInputDeclaration(ps);
                ChannelInputDeclaration(ps, _hasEmissive, _hasEmissiveTexture, "Emissive");
                BumpInputDeclaration(ps);

                if (_hasSpecular || _hasDiffuse)
                {
                    ps.Append("  uniform vec3 ");
                    ps.Append(LightColorName);
                    ps.Append(";\n");
                    ps.Append("  uniform float ");
                    ps.Append(LightIntensityName);
                    ps.Append(";\n");
                    ps.Append("  uniform vec3 ");
                    ps.Append(LightDirectionName);
                    ps.Append(";\n");
                }

                if (_hasSpecular)
                {
                    ps.Append("  varying vec3 vViewDir;\n");
                }

                if (_hasNormals)
                    ps.Append("  varying vec3 vNormal;\n");

                if (_hasUVs)
                    ps.Append("  varying vec2 vUV;\n");
            }

            if (LightningCalulationMethod == LightningMethod.COOK_TORRANCE)
                AppendCookTorranceLightningCalculation(ps);
        }

        private void AppendCookTorranceLightningCalculation(StringBuilder ps)
        {
            AppendLightFunctions(ps);
           // throw new NotImplementedException();
        }

        // TODO: Make material value factory methods
        private void AppendLightFunctions(StringBuilder ps)
        {
            ps.Append("#ifdef GL_ES\n");
            ps.Append("  precision highp float;\n");
            ps.Append("#endif\n\n");
            ps.Append("  uniform vec3 ");
            ps.Append(LightDirectionName);
            ps.Append(";\n");
            ps.Append("varying vec3 vNormal;\n" +
                      "varying vec3 vViewDir;\n\n" +
                      "void main() \n" +
                      "{\n" +
                      "// set important material values \n" +
                      "float roughnessValue = 0.1; // 0 : smooth, 1: rough \n" +
                      "float F0 = 0.8; // fresnel reflectance at normal incidence \n" +
                      "float k = 0.2; // fraction of diffuse reflection (specular reflection = 1 - k) \n" +
                      "vec3 lightColor = vec3(0.1, 0.9, 0.1);\n\n" +
                      "// interpolating normals will change the length of the normal, so renormalize the normal. \n" +
                      "vec3 normal = normalize(vNormal);\n\n" +
                      "// do the lighting calculation for each fragment.\n" +
                      "float NdotL = max(dot(normal, LightDirection), 0.0);\n" +
                      "float specular = 0.0;\n\n" +
                      "if (NdotL > 0.0)\n" +
                      "{\n\n" +
                      "vec3 eyeDir = normalize(vViewDir);\n\n" +
                      "// calculate intermediary values\n" +
                      "vec3 halfVector = normalize(LightDirection + eyeDir);\n" +
                      "float NdotH = max(dot(normal, halfVector), 0.0);\n" +
                      "float NdotV = max(dot(normal, eyeDir), 0.0); // note: this could also be NdotL, which is the same value\n" +
                      "float VdotH = max(dot(eyeDir, halfVector), 0.0);\n" +
                      "float mSquared = roughnessValue * roughnessValue;\n\n" +
                      "// geometric attenuation\n" +
                      "float NH2 = 2.0 * NdotH;\n" +
                      "float g1 = (NH2 * NdotV) / VdotH;\n" +
                      "float g2 = (NH2 * NdotL) / VdotH;\n" +
                      "float geoAtt = min(1.0, min(g1, g2));\n\n" +
                      "// roughness (or: microfacet distribution function)\n" +
                      "// beckmann distribution function\n" +
                      "float r1 = 1.0 / (4.0 * mSquared * pow(NdotH, 4.0));\n" +
                      "float r2 = (NdotH * NdotH - 1.0) / (mSquared * NdotH * NdotH);\n" +
                      "float roughness = r1 * exp(r2);\n\n" +
                      "// fresnel\n" +
                      "// Schlick approximation\n" +
                      "float fresnel = pow(1.0 - VdotH, 5.0);\n" +
                      "fresnel *= (1.0 - F0);\n" +
                      "fresnel += F0;\n\n" +
                      "specular = (fresnel * geoAtt * roughness) / (NdotV * NdotL * 3.14);\n" +
                      "}\n\n" +
                      "vec3 finalValue = lightColor * NdotL * (k + specular * (1.0 - k));\n" +
                      "gl_FragColor = vec4(finalValue, 1.0);\n" +
                      "}\n" +
                      "");
        }

        private void BumpInputDeclaration(StringBuilder ps)
        {
            if (!_hasBump)
                return;

            ps.Append("  uniform sampler2D BumpTexture;\n");
            ps.Append("  uniform float BumpIntensity;\n\n");
        }

        private void SpecularInputDeclaration(StringBuilder ps)
        {
            if (!_hasSpecular)
                return;

            ChannelInputDeclaration(ps, _hasSpecular, _hasSpecularTexture, "Specular");
            // This will generate e.g. "  uniform vec4 DiffuseColor;"
            ps.Append("  uniform float SpecularShininess;\n");
            ps.Append("  uniform float SpecularIntensity;\n\n");
        }

        private void ChannelInputDeclaration(StringBuilder ps, bool hasChannel, bool hasChannelTexture,
            string channelName)
        {
            if (!hasChannel)
                return;

            // This will generate e.g. "  uniform vec4 DiffuseColor;"
            ps.Append("  uniform vec3 ");
            ps.Append(channelName);
            ps.Append("Color;\n");

            if (!hasChannelTexture)
                return;

            // This will generate e.g. 
            // "  uniform sampler2D DiffuseTexture;"
            // "  uniform float DiffuseMix;"
            ps.Append("  uniform sampler2D ");
            ps.Append(channelName);
            ps.Append("Texture;\n");

            ps.Append("  uniform float ");
            ps.Append(channelName);
            ps.Append("Mix;\n\n");
        }

        private void PSBody(StringBuilder ps)
        {
            if (LightningCalulationMethod != LightningMethod.BLINN_PHONG) return;

            ps.Append("\n\n  void main()\n  {\n");
            ps.Append("    vec3 result = vec3(0, 0, 0);\n\n");

            AddNormalVec(ps);
            AddCameraVec(ps);
            AddLightVec(ps);

            AddEmissiveChannel(ps);
            AddDiffuseChannel(ps);
            AddSpecularChannel(ps);

            ps.Append("\n    gl_FragColor = vec4(result, 1.0);\n");
            // ps.Append("\n    gl_FragColor = vec4((Normal + 1.0) * 0.5, 1.0);\n");
            ps.Append("  }\n\n");
        }

        private void PSBodyWithLight(StringBuilder ps, string applyLightMethod)
        {
            ps.Append(applyLightMethod);
            ps.Append("\n\n  void main()\n  {\n");
            ps.Append("    vec3 result = vec3(0, 0, 0);\n\n");

            AddNormalVec(ps);
            AddCameraVec(ps);
            ps.Append("\n    vec3 linearColor = vec3(0);\n");
            ps.Append("\n    linearColor += ApplyLight();\n");
            ps.Append("\n    gl_FragColor = vec4(linearColor, 1.0);\n");
            // ps.Append("\n    gl_FragColor = vec4((Normal + 1.0) * 0.5, 1.0);\n");
            ps.Append("  }\n\n");
        }

        private void AddCameraVec(StringBuilder ps)
        {
            if (!_hasSpecular)
                return;
            ps.Append("    vec3 Camera = vViewDir;\n");
        }

        private void AddLightVec(StringBuilder ps)
        {
            if (_hasDiffuse || _hasSpecular)
            {
                ps.Append("    vec3 LDir = ");
                ps.Append(LightDirectionName);
                ps.Append(";\n");
                ps.Append("    vec3 LColor = ");
                ps.Append(LightColorName);
                ps.Append(";\n");
                ps.Append("    float LIntensity = ");
                ps.Append(LightIntensityName);
                ps.Append(";\n");
            }
        }

        private void AddNormalVec(StringBuilder ps)
        {
            if (_hasBump)
            {
                ps.Append("\n\n    //*********** BUMP *********\n");
                // First implementation ONLY working with object space normals. See
                // http://gamedev.stackexchange.com/a/72806/44105
                // http://docs.cryengine.com/display/SDKDOC4/Tangent+Space+Normal+Mapping
                // http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-13-normal-mapping/
                ps.Append("    vec3 bv =  normalize(texture2D(BumpTexture, vUV).xyz * 2.0 - 1.0);\n");
                ps.Append("    bv = vec3(bv.x, bv.y, -bv.z);\n");
                ps.Append("    vec3 Normal =  normalize(bv);\n\n");
                // ps.Append("    vec3 Normal =  normalize(vNormal);\n\n");
                // ps.Append("    vec3 Normal =  normalize(/*vNormal +*/  BumpIntensity * texture2D(BumpTexture, vUV).xyz);\n\n");
            }
            else
            {
                ps.Append("    vec3 Normal = normalize(vNormal);\n");
            }
        }

        private void AddEmissiveChannel(StringBuilder ps)
        {
            if (!_hasEmissive)
                return;

            ps.Append("\n\n    //*********** EMISSIVE *********\n");
            AddChannelBaseColorCalculation(ps, _hasEmissiveTexture, "Emissive");
            ps.Append("    result += EmissiveBaseColor;\n");
        }

        private void AddDiffuseChannel(StringBuilder ps)
        {
            if (!_hasDiffuse)
                return;

            ps.Append("\n\n    //*********** DIFFUSE *********\n");
            AddChannelBaseColorCalculation(ps, _hasDiffuseTexture, "Diffuse");
            ps.Append("    float diffFactor = dot(LDir, Normal);\n");
            ps.Append("    result += DiffuseBaseColor * LColor * LIntensity * max(diffFactor, 0.0);\n");
        }

        private void AddSpecularChannel(StringBuilder ps)
        {
            if (!_hasSpecular)
                return;

            ps.Append("\n\n    //*********** SPECULAR *********\n");
            if (!_hasDiffuse)
                ps.Append("    float diffFactor = dot(LDir, Normal);\n");

            ps.Append("    if (diffFactor > 0.0) {\n  ");
            AddChannelBaseColorCalculation(ps, _hasSpecularTexture, "Specular");
            ps.Append("      vec3 h = normalize(LDir + Camera);\n");

            ps.Append(
                "      result += SpecularBaseColor * LColor * LIntensity * SpecularIntensity * pow(max(0.0, dot(h, Normal)), SpecularShininess);\n");
            ps.Append("    }\n");
        }

        private void AddChannelBaseColorCalculation(StringBuilder ps, bool hasChannelTexture, string channelName)
        {
            if (!(hasChannelTexture && _hasUVs))
            {
                ps.Append("    vec3 ");
                ps.Append(channelName);
                ps.Append("BaseColor = ");
                ps.Append(channelName);
                ps.Append("Color;\n");
            }
            else
            {
                ps.Append("    vec3 ");
                ps.Append(channelName);
                ps.Append("BaseColor = ");
                ps.Append(channelName);
                ps.Append("Color * (1.0 - ");
                ps.Append(channelName);
                ps.Append("Mix) + texture2D(");
                ps.Append(channelName);
                ps.Append("Texture, vUV).rgb * ");
                ps.Append(channelName);
                ps.Append("Mix;\n");
            }
        }


        public string VS { get; }

        public string PS { get; }

        public string DiffuseColorName
        {
            get { return _hasDiffuse ? "DiffuseColor" : null; }
        }

        public string SpecularColorName
        {
            get { return _hasSpecular ? "SpecularColor" : null; }
        }

        public string EmissiveColorName
        {
            get { return _hasEmissive ? "EmissiveColor" : null; }
        }

        public string DiffuseTextureName
        {
            get { return _hasDiffuseTexture ? "DiffuseTexture" : null; }
        }

        public string SpecularTextureName
        {
            get { return _hasSpecularTexture ? "SpecularTexture" : null; }
        }

        public string EmissiveTextureName
        {
            get { return _hasEmissiveTexture ? "EmissiveTexture" : null; }
        }

        public string BumpTextureName
        {
            get { return _hasBump ? "BumpTexture" : null; }
        }

        public string DiffuseMixName
        {
            get { return _hasDiffuse ? "DiffuseMix" : null; }
        }

        public string SpecularMixName
        {
            get { return _hasSpecular ? "SpecularMix" : null; }
        }

        public string EmissiveMixName
        {
            get { return _hasEmissive ? "EmissiveMix" : null; }
        }

        public string SpecularShininessName
        {
            get { return _hasSpecular ? "SpecularShininess" : null; }
        }

        public string SpecularIntensityName
        {
            get { return _hasSpecular ? "SpecularIntensity" : null; }
        }

        public string BumpIntensityName
        {
            get { return _hasBump ? "BumpIntensity" : null; }
        }

        public static string LightDirectionName
        {
            get { return "LightDirection"; }
        }

        public static string LightColorName
        {
            get { return "LightColor"; }
        }

        public static string LightIntensityName
        {
            get { return "LightIntensity"; }
        }
    }
}
