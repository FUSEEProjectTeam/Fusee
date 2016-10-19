using System.Text;
using Fusee.Base.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{

    public class ForwardShaderCodeBuilder
    {
        private bool _hasVertices, _hasNormals, _hasUVs, _hasColors;
        private bool _hasDiffuse, _hasSpecular, _hasEmissive, _hasBump;
        private bool _hasDiffuseTexture, _hasSpecularTexture, _hasEmissiveTexture;
        private readonly bool _hasWeightMap;
        private readonly int _nBones;
        private readonly bool _normalizeNormals;

        // Needed for MaterialLightComponent
        private bool _hasApplyLightString;
        private bool _hasFragmentString;

        // Needed for MaterialPBRCompoent
        private bool _hasFresnel;
        private bool _hasDiffuseFraction;
        private bool _hasRoughness;

        private LightningCalculationMethod _lightningCalculationMethod = LightningCalculationMethod.SIMPLE;

        // ReSharper disable once InconsistentNaming
        public string VS { get; }

        // ReSharper disable once InconsistentNaming
        public string PS { get; }

        public ForwardShaderCodeBuilder(MaterialComponent mc, MeshComponent mesh, WeightComponent wc = null)
        {
            GetLightningCalculationMethodFromSceneRender();
            AnalyzeMaterial(mc);

            if (wc != null)
            {
                _hasWeightMap = true;
                _nBones = wc.Joints.Count;
            }
            else
            {
                _nBones = 0;
            }

            _normalizeNormals = true;

            // Check for mesh
            if (mesh != null)
                AnalyzeMesh(mesh);
            else
            {
                _hasVertices = _hasNormals = _hasUVs = true;
            }

            // Analyze the Material
            AnalyzeMaterial(mc);
            // VS
            var vs = new StringBuilder();
            MeshInputDeclarations(vs);
            MatrixDeclarations(vs);
            VSBody(vs);
            VS = vs.ToString();

            // PS
            var ps = new StringBuilder();
            PixelInputDeclarations(ps);
            ApplyLightMethod(ps);
            PSBody(ps);
            PS = ps.ToString();
            
            Diagnostics.Log($"ForwardShaderCodeBuilder, VS \n{VS}");
            Diagnostics.Log($"ForwardShaderCodeBuilder, PS \n{PS}");

        }


        private void MeshInputDeclarations(StringBuilder vs)
        {
            vs.Append($"{GLSLVersion()}\n");

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
            // Lighting done in view space
             if (_hasNormals)
                vs.Append("  uniform mat4 FUSEE_ITMV;\n");

            if (_hasSpecular)
                vs.Append("  uniform mat4 FUSEE_IMV;\n");
            
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

            // needed for lightning calc
            vs.Append("  varying vec4 surfacePos;\n");
            vs.Append("  varying vec4 surfacePosOriginal;\n");
            vs.Append("  uniform mat4 FUSEE_MV;\n");
            vs.Append("  varying vec3 vMVNormal;\n");
            vs.Append("  uniform mat4 FUSEE_M;\n");
            vs.Append("  uniform mat4 FUSEE_IV;\n");
            vs.Append("  uniform mat4 FUSEE_V;\n");
            vs.Append("  uniform mat4 FUSEE_IM;\n");
            vs.Append("  varying vec3 viewpos;\n");
            vs.Append(" varying mat4 ModelMatrix;\n");

        }

        // ReSharper disable once InconsistentNaming
        private void VSBody(StringBuilder vs)
        {
          

            vs.Append("\n\n  void main()\n  {\n");
            if (_hasNormals)
            {
                if (_hasWeightMap)
                {
                    vs.Append("    vec4 newVertex;\n");
                    vs.Append("    vec4 newNormal;\n");

                    vs.Append("    newVertex = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuVertex, 1.0) ) * fuBoneWeight.x ;\n");
                    vs.Append("    newNormal = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuNormal, 0.0)) * fuBoneWeight.x;\n");

                    vs.Append("    newVertex = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuVertex, 1.0)) * fuBoneWeight.y + newVertex;\n");
                    vs.Append("    newNormal = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuNormal, 0.0)) * fuBoneWeight.y + newNormal;\n");

                    vs.Append("    newVertex = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuVertex, 1.0)) * fuBoneWeight.z + newVertex;\n");
                    vs.Append("    newNormal = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuNormal, 0.0)) * fuBoneWeight.z + newNormal;\n");

                    vs.Append("    newVertex = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuVertex, 1.0)) * fuBoneWeight.w + newVertex;\n");
                    vs.Append("    newNormal = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuNormal, 0.0)) * fuBoneWeight.w + newNormal;\n");

                    // At this point the normal is in World space - transform back to model space
                    // TODO: Is it a hack to invert Model AND View? Should we rather only invert MODEL (and NOT VIEW)??
                    vs.Append("    vNormal = mat3(FUSEE_IMV) * newNormal.xyz;\n");

                    if (_normalizeNormals)
                    {
                        vs.Append("    vNormal = normalize(fuNormal);\n");
                        vs.Append("    vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);\n");
                    }
                }
                else
                {
                    // Lighting done in model space... no need to convert normals
                    if (_normalizeNormals) { 
                        // vs.Append("    vNormal = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);\n");
                        vs.Append("    vNormal = normalize(mat3(FUSEE_IMV) * fuNormal); \n");
                    vs.Append("    vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);\n");
                    }
                    else
                        vs.Append("    vNormal = fuNormal;\n");
                }
            }

            if (_hasSpecular)
            {
                vs.Append("    vec3 viewPos = FUSEE_IMV[3].xyz;\n");

                vs.Append(_hasWeightMap
                    ? "    vViewDir = normalize(viewPos - vec3(newVertex));\n"
                    : "    vViewDir = normalize(viewPos - fuVertex);\n");
            }

            vs.Append(_hasWeightMap
                ? "    gl_Position = FUSEE_P * FUSEE_V * vec4(vec3(newVertex), 1.0);\n "
                : "    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);\n");


            if (_hasUVs)
                vs.Append("    vUV = fuUV;\n");

            // needed for spotlight
            vs.Append("    surfacePos =  FUSEE_M * vec4(fuVertex, 1.0); \n");
            vs.Append("    surfacePosOriginal = vec4( fuVertex, 1.0); \n");
            vs.Append("    viewpos = (FUSEE_MV * vec4(fuVertex, 1.0)).xyz;");
            vs.Append("    ModelMatrix = FUSEE_M;");

            
            vs.Append("  }\n\n");
        }


        private void PixelInputDeclarations(StringBuilder ps)
        {
            ps.Append($"{GLSLVersion()}\n");

            ps.Append("#ifdef GL_ES\n");
            ps.Append("  precision highp float;\n");
            ps.Append("#endif\n\n");

            // define max lights
            var numberOfLights = SceneRenderer.AllLightResults.Count > 0 ? SceneRenderer.AllLightResults.Count : 1;
            ps.Append("\n\n #define MAX_LIGHTS " + numberOfLights + "\n\n");

            LightStructDeclaration(ps);

            ChannelInputDeclaration(ps, _hasDiffuse, _hasDiffuseTexture, "Diffuse");
            SpecularInputDeclaration(ps);
            ChannelInputDeclaration(ps, _hasEmissive, _hasEmissiveTexture, "Emissive");
            BumpInputDeclaration(ps);

            if (_hasSpecular)
                ps.Append("  varying vec3 vViewDir;\n");
            

            if (_hasNormals)
                ps.Append("  varying vec3 vNormal;\n");

            if (_hasUVs)
                ps.Append("  varying vec2 vUV;\n");

            ps.Append(" varying vec4 surfacePos;\n");
            ps.Append(" varying vec4 surfacePosOriginal;\n");
            
            ps.Append(" varying vec3 vMVNormal;\n");
            ps.Append(" uniform mat4 FUSEE_MV;\n");
            ps.Append(" varying mat4 ModelMatrix;\n");
            ps.Append("  uniform mat4 FUSEE_IMV;\n");
            ps.Append("  uniform mat4 FUSEE_IM;\n");
            ps.Append("  uniform mat4 FUSEE_IV;\n");
            ps.Append("  uniform mat4 FUSEE_V;\n");
            ps.Append("  varying vec3 viewpos;\n");

        }

        // ReSharper disable once InconsistentNaming
        private static string GLSLVersion()
        {
            return "#version 100";
        }

        private void LightStructDeclaration(StringBuilder ps)
        {
            ps.Append("\n\n");
            ps.Append("struct Light\n");
            ps.Append("{\n");
                ps.Append(" vec3 position;\n");
                ps.Append(" vec3 intensities;\n");
                ps.Append(" vec3 coneDirection;\n");
                ps.Append(" float attenuation;\n");
                ps.Append(" float ambientCoefficient;\n");
                ps.Append(" float coneAngle;\n");
                ps.Append(" int lightType;\n");
            ps.Append("};\n");
            ps.Append("\n\n");
            ps.Append(" uniform Light allLights[MAX_LIGHTS]; \n\n");
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

        private void BumpInputDeclaration(StringBuilder ps)
        {
            if (!_hasBump)
                return;

            ps.Append("  uniform sampler2D BumpTexture;\n");
            ps.Append("  uniform float BumpIntensity;\n\n");
        }

        private void ApplyLightMethod(StringBuilder vs)
        {
            if (_hasApplyLightString)
            {
                vs.Append($"\n\n    {_applyLightString}     \n\n");
            }
            else if (_lightningCalculationMethod == LightningCalculationMethod.SIMPLE)
            {
                vs.Append("\n\n\n");
                vs.Append($"           {AmbientLightningMethod()}\n");
                vs.Append($"           {DiffuseLightingMethod()}\n");
                vs.Append($"           {SpecularLightingMethod()}\n");
                vs.Append("\n\n\n");
                vs.Append("/******* ApplyLight Method ****/\n");
                vs.Append("vec3 ApplyLight(int i)\n");
                vs.Append("{\n");
                vs.Append("     vec3 result = vec3(0.0, 0.0, 0.0);\n");
                vs.Append("     float attenuation = 1.0;\n");
                vs.Append("     if(allLights[i].lightType == 0) // PointLight\n");
                vs.Append("     {");
                vs.Append($"           {PointLightCalculation()}\n");
                vs.Append("     }\n");
                vs.Append("     else if(allLights[i].lightType == 1) // ParallelLight\n");
                vs.Append("     {");
                vs.Append($"            {ParallelLightCalculation()}\n");
                vs.Append("     }\n");
                vs.Append("     else if(allLights[i].lightType == 2) // SpotLight\n");
                vs.Append("     {");
                vs.Append($"            {SpotLightCalculation()}\n");
                vs.Append("     }\n");
                vs.Append("     return result;\n");
                vs.Append("}\n");
               
            }
            else
            { // TODO: Add CookTorranceModel and try to parse Diffusevars to PBR values
                vs.Append("\n\n\n");
                vs.Append("/******* ApplyLight Method ****/\n");
                vs.Append("vec3 ApplyLight(int i\n");
                vs.Append("{\n");
                vs.Append("     vec3 result = vec3(0, 0, 0);\n");
                vs.Append("     if(light.lightType == 0)\n");
                vs.Append("    {\n");
                vs.Append($"           {PointLightCalculation()}\n");
                vs.Append("    }\n");
                vs.Append("     else if(light.lightType == 2)\n");
                vs.Append("     {\n");
                vs.Append($"            {ParallelLightCalculation()}\n");
                vs.Append("     }\n");
                vs.Append("     else if(light.lightType == 1)\n");
                vs.Append("     {\n");
                vs.Append($"            {SpotLightCalculation()}\n");
                vs.Append("     }\n");
                vs.Append("     return result;\n");
                vs.Append("}\n");
            }
        }

        private string AmbientLightningMethod()
        {
            var outputString = "\n";
            outputString += "// returns intensity of reflected ambient lighting\n";
            outputString += "vec3 ambientLighting(int i)\n";
            outputString += "{\n";
                if(EmissiveColorName != null)
                    outputString += $"   return ({EmissiveColorName} * allLights[i].ambientCoefficient);\n";
                else
                    outputString += $"   return vec3(allLights[i].ambientCoefficient);\n";
            outputString += "}\n";

            return outputString;
        }

        private string DiffuseLightingMethod()
        {
            var outputString = "\n";
            outputString += "// returns intensity of diffuse reflection\n";
            outputString += "vec3 diffuseLighting(vec3 N, vec3 L, int i)\n";
            outputString += "{\n";
            outputString += "   // calculation as for Lambertian reflection\n";
            outputString += "   float diffuseTerm = clamp(dot(N, L) / (length(L) * length(N)), 0.0, 1.0) ;\n";
            outputString += $"  return ({DiffuseColorName} * allLights[i].intensities * diffuseTerm);\n";
            outputString += "}\n";

            return outputString;
        }

        private string SpecularLightingMethod()
        {
            var outputString = "\n";
            outputString += "// returns intensity of diffuse reflection\n";
            outputString += "vec3 specularLighting(vec3 N, vec3 L, vec3 V, int i)\n";
            outputString += "{\n";
            outputString += "   float specularTerm = 0.0;\n";
            outputString += "   if(dot(N, L) > 0.0)\n";
            outputString += "   {\n";
            outputString += "   // half vector\n";
            outputString += "   vec3 H = normalize(L + V);\n";
            outputString += $"   specularTerm = max(0.0, pow(dot(N, H), {SpecularShininessName}));\n";
            outputString += "   }\n";
            outputString += $"  return ({SpecularColorName} * {SpecularIntensityName} * allLights[i].intensities) * specularTerm;\n";
            outputString += "}\n";

            return outputString;
        }

        /// <summary>
        /// ParallelLight, no specular component
        /// ConeDirection specifies direction of light.
        /// </summary>
        /// <returns></returns>
        private string ParallelLightCalculation()
        {
            var outputString = "\n";

            outputString += "vec3 o_normal = vNormal;\n";
            outputString += "vec3 o_toLight = normalize(allLights[i].position.xyz - surfacePos.xyz);\n";
            outputString += "vec3 o_toCamera = normalize(vViewDir - surfacePos.xyz);\n";
            outputString += "vec2 o_texcoords = vUV;\n";
            outputString += "\n";
            outputString += "\n";
            outputString += "vec3 L = normalize(o_toLight);\n";
            outputString += "vec3 V = normalize(o_toCamera);\n";
            outputString += "vec3 N = normalize(o_normal);\n";
            outputString += "vec3 Iamb = ambientLighting(i);\n";
            outputString += "vec3 Idif = diffuseLighting(N, L, i);\n";
            outputString += "vec3 Ispe = specularLighting(N, L, V, i);\n";
            outputString += "\n";
            if (DiffuseTextureName != null)
                outputString += $"vec3 diffuseColor = texture2D({DiffuseTextureName}, o_texcoords).rgb * {DiffuseMixName};\n";
            else
                outputString += $"vec3 diffuseColor = {DiffuseColorName};\n";
            outputString += "\n";
            outputString += "\n";
            outputString += "result = diffuseColor * (Iamb + Idif + Ispe);\n";

            return outputString;
        }

        /// <summary>
        /// PointLight, with specular component and half-vector
        /// </summary>
        /// <returns></returns>
        private string PointLightCalculation()
        {

            var outputString = "\n";
         
            outputString += "vec3 o_normal = vNormal;\n";
            outputString += "vec3 o_toLight = normalize( allLights[i].position.xyz - surfacePos.xyz);\n";
            outputString += "vec3 o_toCamera = normalize(vViewDir - surfacePos.xyz);\n";
            outputString += "vec2 o_texcoords = vUV;\n";
            outputString += "\n";
            outputString += "\n";
            outputString += "vec3 L = o_toLight;\n";
            outputString += "vec3 V = o_toCamera;\n";
            outputString += "vec3 N = o_normal;\n";
            outputString += "vec3 Iamb = ambientLighting(i);\n";
            outputString += "vec3 Idif = diffuseLighting(N, L, i);\n";
            outputString += "vec3 Ispe = specularLighting(N, L, V, i);\n";
            outputString += "\n";
            
            outputString += "       float distanceToLight = distance( allLights[i].position.xyz, surfacePos.xyz);\n";
            outputString += "       float att = clamp(1.0 - distanceToLight*distanceToLight/(allLights[i].attenuation*allLights[i].attenuation), 0.0, 1.0);\n";
            outputString += "       att *= att;\n";
            if (DiffuseTextureName != null)
                outputString += $"vec3 diffuseColor = texture2D({DiffuseTextureName}, o_texcoords).rgb * {DiffuseMixName};\n";
            else
                outputString += $"vec3 diffuseColor = {DiffuseColorName};\n";
            outputString += "\n";
            outputString += "\n";
            outputString += "result = diffuseColor * (Iamb + Idif + Ispe) * att;\n";
            //outputString += "result = diffuseColor * allLights[i].intensities;\n";

            return outputString;

        }

        private string SpotLightCalculation()
        {
            var outputString = "\n";

            outputString += "vec3 o_normal = vNormal;\n";
            outputString += "vec3 o_toLight = normalize(allLights[i].position.xyz - surfacePos.xyz);\n";
            outputString += "vec3 o_toCamera = normalize(viewpos - surfacePos.xyz);\n";
            outputString += "vec2 o_texcoords = vUV;\n";
            outputString += "\n";
            outputString += "\n";
            outputString += "vec3 L = o_toLight;\n";
            outputString += "vec3 V = o_toCamera;\n";
            outputString += "vec3 N = o_normal;\n";
            outputString += "vec3 Iamb = ambientLighting(i);\n";
            outputString += "vec3 Idif = diffuseLighting(N, L, i);\n";
            outputString += "vec3 Ispe = specularLighting(N, L, V, i);\n";
            outputString += "\n";
            outputString += "       float distanceToLight = distance(allLights[i].position.xyz, surfacePos.xyz);\n";
            outputString += "       float att = clamp(1.0 - distanceToLight*distanceToLight/(allLights[i].attenuation*allLights[i].attenuation), 0.0, 1.0);";
            outputString += "       att *= att;";
            if (DiffuseTextureName != null)
                outputString += $"vec3 diffuseColor = texture2D({DiffuseTextureName}, o_texcoords).rgb * {DiffuseMixName};\n";
            else
                outputString += $"vec3 diffuseColor = {DiffuseColorName};\n";

            outputString += "       float lightToSurfaceAngle = degrees(acos(dot(-o_toLight, normalize(allLights[i].coneDirection))));\n";
            outputString += "       if (lightToSurfaceAngle > allLights[i].coneAngle)\n";
            outputString += "       {\n";
            outputString += "           att = 0.0;\n";
            outputString += "       }\n";

            outputString += "\n";
            outputString += "\n";
            outputString += "result = diffuseColor * (Iamb + Idif + Ispe) * att;\n";

            return outputString;
        }

        // ReSharper disable once InconsistentNaming
        private void PSBody(StringBuilder vs)
        {
            vs.Append("\n\n\n");
            vs.Append("void main()\n");
            vs.Append("{\n");
                vs.Append("    vec3 result = vec3(0.0);\n");
                // Annotation: allLights.length() only supported in version 300; WebGL needs version 100.
                // Therefore we need a workaround.  
                vs.Append("    for(int i = 0; i < MAX_LIGHTS;i++)\n");
                vs.Append("    {\n");
                    vs.Append("         result += ApplyLight(i);\n");
                vs.Append("    }\n");
                vs.Append($"    {GammaCorrection()}\n");
                vs.Append("    gl_FragColor = vec4(final_light ,1.0);\n");
            vs.Append("}\n");
        }


        private static string GammaCorrection()
        {
            return "    vec3 gamma = vec3(1.0/2.2);\n   vec3 final_light = pow(result, gamma);\n";
        }
      

        private static void ChannelInputDeclaration(StringBuilder ps, bool hasChannel, bool hasChannelTexture, string channelName)
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

        /// <summary>
        /// Analyzes the material.
        /// </summary>
        /// <param name="mc">The MaterialComponent</param>
        private void AnalyzeMaterial(MaterialComponent mc)
        {
            // MaterialComponent analysis:
            _hasDiffuse = mc.HasDiffuse;

            if (_hasDiffuse)
                _hasDiffuseTexture = (mc.Diffuse.Texture != null);
            _hasSpecular = mc.HasSpecular;

            if (_hasSpecular)
                _hasSpecularTexture = (mc.Specular.Texture != null);
            _hasEmissive = mc.HasEmissive;

            if (_hasEmissive)
                _hasEmissiveTexture = (mc.Emissive.Texture != null);
            _hasBump = mc.HasBump; // always has a texture...

            if (mc.GetType() == typeof(MaterialLightComponent))
            {
                var mlc = mc as MaterialLightComponent;
                if (mlc == null) return;
                
                // check for ApplyLightString
                if (!string.IsNullOrEmpty(mlc.ApplyLightString))
                {
                    _hasApplyLightString = true;
                    _applyLightString = mlc.ApplyLightString;
                }

                // check for  FragmentString
                if (!string.IsNullOrEmpty(mlc.FragmentShaderString))
                {
                    _hasFragmentString = true;
                    _applyFragmentString = mlc.FragmentShaderString;
                }
                   

            }
            else if(mc.GetType() == typeof(MaterialPBRComponent))
            {
                var mpbr = mc as MaterialPBRComponent;
                if(mpbr == null) return;
                
                // check for fraction
                if (mpbr.DiffuseFraction > 0f)
                {
                    _hasDiffuseFraction = true;
                    _diffuseFractionValue = mpbr.DiffuseFraction;
                }
                // check for fresnel
                if (mpbr.FresnelReflectance > 0f)
                {
                    _hasFresnel = true;
                    _fresnelValue = mpbr.FresnelReflectance;
                }
                // check for roughness
                if (mpbr.RoughnessValue > 0f)
                {
                    _hasRoughness = true;
                    _roughnessValue = mpbr.RoughnessValue;
                }

            }
        }
        /// <summary>
        /// Analyzes the mesh
        /// </summary>
        /// <param name="mesh"></param>
        private void AnalyzeMesh(MeshComponent mesh)
        {
            _hasVertices = (mesh.Vertices != null && mesh.Vertices.Length > 0);
            _hasNormals = (mesh.Normals != null && mesh.Normals.Length > 0);
            _hasUVs = (mesh.UVs != null && mesh.UVs.Length > 0);
            _hasColors = false;
        }

        private void GetLightningCalculationMethodFromSceneRender()
        {
            _lightningCalculationMethod = SceneRenderer.LightningCalculationMethod;
        }

        #region NamesAndValues

        public string DiffuseColorName => (_hasDiffuse) ? "DiffuseColor" : null;

        public string SpecularColorName => (_hasSpecular) ? "SpecularColor" : null;

        public string EmissiveColorName => (_hasEmissive) ? "EmissiveColor" : null;

        public string DiffuseTextureName => (_hasDiffuseTexture) ? "DiffuseTexture" : null;

        public string SpecularTextureName => (_hasSpecularTexture) ? "SpecularTexture" : null;

        public string EmissiveTextureName => (_hasEmissiveTexture) ? "EmissiveTexture" : null;

        public string BumpTextureName => (_hasBump) ? "BumpTexture" : null;

        public string DiffuseMixName => (_hasDiffuse) ? "DiffuseMix" : null;

        public string SpecularMixName => (_hasSpecular) ? "SpecularMix" : null;

        public string EmissiveMixName => (_hasEmissive) ? "EmissiveMix" : null;

        public string SpecularShininessName => (_hasSpecular) ? "SpecularShininess" : null;

        public string SpecularIntensityName => (_hasSpecular) ? "SpecularIntensity" : null;

        public string BumpIntensityName => (_hasBump) ? "BumpIntensity" : null;

        public string ApplyLightFunction => (_hasApplyLightString) ? _applyLightString : null;

        public string ApplyFragmentFunction => (_hasFragmentString) ? _applyFragmentString : null;

        public float FresnelValue => (_hasFresnel) ? _fresnelValue : 0;

        public float DiffuseFractionValue => (_hasDiffuseFraction) ? _diffuseFractionValue : 0;

        public float RoughnessValue => (_hasRoughness) ? _roughnessValue : 0;

        private string  _applyLightString;

        private string _applyFragmentString;

        private float _fresnelValue;

        private float _diffuseFractionValue;

        private float _roughnessValue;

        #endregion
    }

}
