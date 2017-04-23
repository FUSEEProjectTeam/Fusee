//#define DEBUG

using System;
using System.Globalization;
using System.Text;
using Fusee.Base.Core;
using Fusee.Serialization;


namespace Fusee.Engine.Core
{
    public class ShaderCodeBuilder
    {
        // Material Analyze
        private bool _hasVertices, _hasNormals, _hasUVs, _hasColors;
        private bool _hasDiffuse, _hasSpecular, _hasEmissive, _hasBump;
        private bool _hasDiffuseTexture, _hasSpecularTexture, _hasEmissiveTexture;
        private bool _isMaterialLightComponent, _isMaterialPBRComponent;
        private bool _hasApplyLightString;
        private bool _hasFragmentString;
        private readonly bool _hasWeightMap;
        private readonly int _nBones;
        private string _applyLightString;
        private string _applyFragmentString;
        private float _pbrRoughness, _pbrDiffuse, _pbrFresnel;
        private LightingCalculationMethod _lightingCalculationMethod;

        // ReSharper disable once InconsistentNaming
        public string VS { get; private set; }
        // ReSharper disable once InconsistentNaming
        public string PS { get; private set; }

        /// <summary>
        /// LEGACY CONSTRUCTOR
        /// Creates vertex and pixel shader for given material, mesh, weight; lightning calculation is simple per default
        /// </summary>
        /// <param name="mc">The MaterialCpmponent</param>
        /// <param name="mesh">The MeshComponent</param>
        /// <param name="wc">Teh WeightComponent</param>
        public ShaderCodeBuilder(MaterialComponent mc, MeshComponent mesh, WeightComponent wc = null)
            : this(mc, mesh, LightingCalculationMethod.SIMPLE, wc)
        {
           
        }

        /// <summary>
        /// Creates vertex and pixel shader for given material, mesh, weight; lightning calculation is simple per default
        /// </summary>
        /// <param name="mc">The MaterialCpmponent</param>
        /// <param name="mesh">The MeshComponent</param>
        /// <param name="wc">The WeightComponent</param>
        /// <param name="lightingCalculation">Method of lightning calculation; simple BLINN PHONG or advanced physically based</param>
        public ShaderCodeBuilder(MaterialComponent mc, MeshComponent mesh,
            LightingCalculationMethod lightingCalculation = LightingCalculationMethod.SIMPLE,
            WeightComponent wc = null)
        {
            // Set Lightnincalculation
            _lightingCalculationMethod = lightingCalculation;

            // first step, analyze Material
            AnalyzeMaterial(mc);

            // check for animation
            if (wc != null)
            {
                _hasWeightMap = true;
                _nBones = wc.Joints.Count;
            }
            else
            {
                _nBones = 0;
            }

            // Check for mesh
            if (mesh != null)
                AnalyzeMesh(mesh);
            else
            {
                _hasVertices = _hasNormals = _hasUVs = true;
            }

            // Create VS
            CreateVertexShader();
            // Create PS
            if (lightingCalculation == LightingCalculationMethod.ADVANCED && _isMaterialPBRComponent)
                CreatePbrPixelShader();
            else if (lightingCalculation == LightingCalculationMethod.ADVANCEDwENVMAP && _isMaterialPBRComponent)
                CreatePbrEnvMapPixelShader();
            else
                CreatePixelShader();


#if DEBUG
            //Diagnostics.Log(VS);
            //Diagnostics.Log(PS);
#endif
        }

        public void AnalyzeMaterial(MaterialComponent mc)
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

            // Reflection for materialComponent
            var t = mc.GetType();
            if (typeof(MaterialPBRComponent).IsAssignableFrom(t))
            {
                // TODO: Write correct physically based shaders
                 var mcPBR = mc as MaterialPBRComponent;
                _isMaterialPBRComponent = true;
                _pbrRoughness = mcPBR.RoughnessValue;
                _pbrDiffuse = mcPBR.DiffuseFraction;
                _pbrFresnel = mcPBR.FresnelReflectance;
            }
            if (typeof(MaterialLightComponent).IsAssignableFrom(t))
            {
                var mlc = mc as MaterialLightComponent;
                _isMaterialLightComponent = true;

                // check for ApplyLightString
                if (!string.IsNullOrEmpty(mlc?.ApplyLightString))
                {
                    _hasApplyLightString = true;
                    _applyLightString = mlc.ApplyLightString;
                }

            }
        }

        private void AnalyzeMesh(MeshComponent mesh)
        {
            _hasVertices = (mesh.Vertices != null && mesh.Vertices.Length > 0);
            _hasNormals = (mesh.Normals != null && mesh.Normals.Length > 0);
            _hasUVs = (mesh.UVs != null && mesh.UVs.Length > 0);
            _hasColors = false;
        }

        private void CreateVertexShader()
        {
            var vertexStringBuilder = new StringBuilder();
            // Version
            vertexStringBuilder.Append(Version());
            // VertexInputs
            vertexStringBuilder.Append(VertexInputDeclarations());
            // VertexMatrix
            vertexStringBuilder.Append(VertexMatrixDeclarations());
            // VertexBody
            vertexStringBuilder.Append(VertexBody());
            // Return VS
            VS = vertexStringBuilder.ToString();
        }

        private void CreatePixelShader()
        {
            var pixelStringBuilder = new StringBuilder();
            // Version
            pixelStringBuilder.Append(Version());
            // PixelInputs
            pixelStringBuilder.Append(PixelInputDeclarations());
            // PixelShaderMethods (specular, light, etc.)
            pixelStringBuilder.Append(PixelShaderMethods());
            // PixelBody
            pixelStringBuilder.Append(PixelBody());
            // Return PS
            PS = pixelStringBuilder.ToString();
        }

        private void CreatePbrPixelShader()
        {
            var pixelStringBuilder = new StringBuilder();
            // Version
            pixelStringBuilder.Append(Version());
            // PixelInputs
            pixelStringBuilder.Append(PixelInputDeclarations());
            // PixelShaderMethods (specular, light, etc.)
            pixelStringBuilder.Append(PixelBPRShaderMethods());
            // PixelBody
            pixelStringBuilder.Append(PixelBody());
            // Return PS
            PS = pixelStringBuilder.ToString();
        }

        private void CreatePbrEnvMapPixelShader()
        {
            var pixelStringBuilder = new StringBuilder();
            // Version
            pixelStringBuilder.Append(Version());
            // PixelInputs
            pixelStringBuilder.Append(PixelInputDeclarations());
            // PixelShaderMethods (specular, light, etc.)
            pixelStringBuilder.Append(PixelBPRShaderMethods());
            // PixelBody
            pixelStringBuilder.Append(PixelBody());
            // Return PS
            PS = pixelStringBuilder.ToString();
          
        }

        private string PixelBPRShaderMethods()
        {
            var returnString = "";

            // Ambient Light
            returnString += AmbientLightMethod();

            // Diffuse Light
            if (_hasDiffuse)
                returnString += DiffuseLightMethod();

            // Specular Light
            if (_hasSpecular && _lightingCalculationMethod == LightingCalculationMethod.ADVANCED)
                returnString += NDFLightMethod();
            
            // Specular Light
            else if (_hasSpecular && _lightingCalculationMethod == LightingCalculationMethod.ADVANCEDwENVMAP)
                returnString += NDFEnvMapLightMethod();

            // Shadows
            returnString += ShadowFactorMethod();

            return returnString;
        }

        private string NDFEnvMapLightMethod()
        {
            // needed for . instead of , in european culture
            var culture = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;

            var returnString = "";
            returnString += "// returns intensity of diffuse reflection with Cook-Torrance NDF \n";
            returnString += "vec3 specularLighting(vec3 N, vec3 L, vec3 V, vec3 intensities) { \n";
            returnString += $"float roughnessValue = {string.Format(culture, "{0:0.#####}", _pbrRoughness)}; // 0 : smooth, 1: rough \n";
            returnString += $"float F0 = {string.Format(culture, "{0:0.#####}", _pbrFresnel)}; // fresnel reflectance at normal incidence \n";
            returnString += $"float k = {string.Format(culture, "{0:0.#####}", _pbrDiffuse)}; // fraction of diffuse reflection (specular reflection = 1 - k)\n";
            returnString += @"
              
                // do the lighting calculation for each fragment.
                float NdotL = max(dot(N, L), 0.0);
    
                float specular = 0.0;
                if(NdotL > 0.0)
                {
                    // calculate intermediary values
                    vec3 H = normalize(L + V);
                    float NdotH = max(dot(N, H), 0.0); 
                    float NdotV = max(dot(N, V), 0.0); // note: this could also be NdotL, which is the same value
                    float VdotH = max(dot(V, H), 0.0);
                    float mSquared = roughnessValue * roughnessValue;
        
                    // geometric attenuation
                    float NH2 = 2.0 * NdotH;
                    float g1 = (NH2 * NdotV) / VdotH;
                    float g2 = (NH2 * NdotL) / VdotH;
                    float geoAtt = min(1.0, min(g1, g2));
     
                    // roughness (or: microfacet distribution function)
                    // beckmann distribution function
                    /* float r1 = 1.0 / ( 4.0 * mSquared * pow(NdotH, 4.0));
                    float r2 = (NdotH * NdotH - 1.0) / (mSquared * NdotH * NdotH);
                    float roughness = r1 * exp(r2); */
                    
                    // roughness (or: microfacet distribution function)
                    // Trowbridge-Reitz or GGX, GTR2
                    float NdotHSquared = dot(N, H) * dot(N, H);
                    float r1 = (pow(NdotHSquared, 2.0) * (mSquared - 1.0) + 1.0);
                    float r2 = 3.14 * pow(r1, 2.0);
                    float roughness = mSquared / r2;

                    // fresnel
                    // Schlick approximation
                    float fresnel = pow(1.0 - VdotH, 5.0);
                    fresnel *= (1.0 - F0);
                    fresnel += F0;
        
                    specular = (fresnel * geoAtt * roughness) / (NdotV * NdotL * 3.14);                    
                } 
                    return intensities * (k + specular * (1.0 - k));            }
            ";

            return returnString;
        }

        private string NDFLightMethod()
        {
            // needed for . instead of , in european culture
            var culture = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;

            var returnString = "";
            returnString += "// returns intensity of diffuse reflection with Cook-Torrance NDF \n";
            returnString += "vec3 specularLighting(vec3 N, vec3 L, vec3 V, vec3 intensities) { \n";
            returnString += $"float roughnessValue = {string.Format(culture, "{0:0.#####}",_pbrRoughness)}; // 0 : smooth, 1: rough \n";
            returnString += $"float F0 = {string.Format(culture, "{0:0.#####}", _pbrFresnel)}; // fresnel reflectance at normal incidence \n";
            returnString += $"float k = {string.Format(culture, "{0:0.#####}", _pbrDiffuse)}; // fraction of diffuse reflection (specular reflection = 1 - k)\n";
            returnString += @"
              
                // do the lighting calculation for each fragment.
                float NdotL = max(dot(N, L), 0.0);
    
                float specular = 0.0;
                if(NdotL > 0.0)
                {
                    // calculate intermediary values
                    vec3 H = normalize(L + V);
                    float NdotH = max(dot(N, H), 0.0); 
                    float NdotV = max(dot(N, V), 0.0); // note: this could also be NdotL, which is the same value
                    float VdotH = max(dot(V, H), 0.0);
                    float mSquared = roughnessValue * roughnessValue;
        
                    // geometric attenuation
                    float NH2 = 2.0 * NdotH;
                    float g1 = (NH2 * NdotV) / VdotH;
                    float g2 = (NH2 * NdotL) / VdotH;
                    float geoAtt = min(1.0, min(g1, g2));
     
                    // roughness (or: microfacet distribution function)
                    // beckmann distribution function
                    /* float r1 = 1.0 / ( 4.0 * mSquared * pow(NdotH, 4.0));
                    float r2 = (NdotH * NdotH - 1.0) / (mSquared * NdotH * NdotH);
                    float roughness = r1 * exp(r2); */
                    
                    // roughness (or: microfacet distribution function)
                    // Trowbridge-Reitz or GGX, GTR2
                    float NdotHSquared = dot(N, H) * dot(N, H);
                    float r1 = (pow(NdotHSquared, 2.0) * (mSquared - 1.0) + 1.0);
                    float r2 = 3.14 * pow(r1, 2.0);
                    float roughness = mSquared / r2;

                    // fresnel
                    // Schlick approximation
                    float fresnel = pow(1.0 - VdotH, 5.0);
                    fresnel *= (1.0 - F0);
                    fresnel += F0;
        
                    specular = (fresnel * geoAtt * roughness) / (NdotV * NdotL * 3.14);                    
                } 
                    return intensities * (k + specular * (1.0 - k));
            }
            ";

            return returnString;
        }

        /// <summary>
        /// Returns the GLSL Version
        /// </summary>
        /// <returns></returns>
        private static string Version()
        {
            return "#version 100\n";
        }

        /// <summary>
        /// Returns the vertex input declarations
        /// </summary>

        private string VertexInputDeclarations()
        {
            var returnString = "";

            if (_hasVertices)
                returnString += "attribute vec3 fuVertex;\n";

            if (_hasSpecular)
                returnString += "varying vec3 vViewDir;\n";

            if (_hasWeightMap)
            {
                returnString += "attribute vec4 fuBoneIndex;\n";
                returnString += "attribute vec4 fuBoneWeight;\n";
            }

            if (_hasNormals)
                returnString += "attribute vec3 fuNormal;\n " +
                                "varying vec3 vNormal;\n";

            if (_hasUVs)
                returnString += "attribute vec2 fuUV;\n" +
                                "varying vec2 vUV;\n";

            if (_hasColors)
                returnString += "attribute vec4 fuColor;" +
                                "\n varying vec4 vColors;\n";

            return returnString;
        }

        /// <summary>
        /// Returns the vertex shader matrix declarations
        /// </summary>
        /// <returns></returns>
        private string VertexMatrixDeclarations()
        {
            var returnString = "";

            // FUSEE_MVP
            returnString += "uniform mat4 FUSEE_MVP;\n";

            if (_hasNormals)
                returnString += "uniform mat4 FUSEE_ITMV;\n";

            if (_hasSpecular)
                returnString += "uniform mat4 FUSEE_IMV;\n";

            if (_hasWeightMap)
            {
                returnString += "uniform mat4 FUSEE_P;\n";
                //returnString += "uniform mat4 FUSEE_V;\n"; legacy code, there is no sperate view anymore!
                returnString += "uniform mat4 FUSEE_IMV;\n";
                returnString += $"uniform mat4 FUSEE_BONES[{_nBones}];\n";
            }

            // lightning calculation
            returnString += "varying vec4 surfacePos;\n";
            returnString += "varying vec3 vMVNormal;\n";
            returnString += "uniform mat4 FUSEE_MV;\n";

            // shadow calculation
            returnString += "varying vec4 shadowLight;\n";
            returnString += "uniform mat4 shadowMVP;\n";

            return returnString;
        }

        /// <summary>
        /// Returns the vertex shader main method
        /// </summary>
        /// <returns></returns>
        private string VertexBody()
        {
            var returnString = "";

            // Start of main
            returnString += "void main() {\n";

            if (_hasNormals && _hasWeightMap)
            {
                returnString += "vec4 newVertex;\n";
                returnString += "vec4 newNormal;\n";

                returnString +=
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuVertex, 1.0) ) * fuBoneWeight.x ;\n";
                returnString +=
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuNormal, 0.0)) * fuBoneWeight.x;\n";

                returnString +=
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuVertex, 1.0)) * fuBoneWeight.y + newVertex;\n";
                returnString +=
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuNormal, 0.0)) * fuBoneWeight.y + newNormal;\n";

                returnString +=
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuVertex, 1.0)) * fuBoneWeight.z + newVertex;\n";
                returnString +=
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuNormal, 0.0)) * fuBoneWeight.z + newNormal;\n";

                returnString +=
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuVertex, 1.0)) * fuBoneWeight.w + newVertex;\n";
                returnString +=
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuNormal, 0.0)) * fuBoneWeight.w + newNormal;\n";

                // At this point the normal is in World space - transform back to model space
                // TODO: Is it a hack to invert Model AND View? Should we rather only invert MODEL (and NOT VIEW)??
                returnString += "vNormal = mat3(FUSEE_IMV) * newNormal.xyz;\n";
            }

            if (_hasSpecular)
            {
                returnString += "vec3 viewPos = FUSEE_IMV[3].xyz;\n";

                returnString += _hasWeightMap
                    ? "vViewDir = normalize(viewPos - vec3(newVertex));\n"
                    : "vViewDir = normalize(viewPos - fuVertex);\n";
            }

            if (_hasUVs)
                returnString += "vUV = fuUV;\n";

            // Add ModelViewSpace normals for lightning calculation
            returnString += "vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);\n";

            // lighting calculation
            returnString += "surfacePos =  FUSEE_MV * vec4(fuVertex, 1.0); \n";
            returnString += "shadowLight = shadowMVP * surfacePos; \n";

            // gl_Position
            returnString += _hasWeightMap
                ? "gl_Position = FUSEE_P * FUSEE_V * vec4(vec3(newVertex), 1.0);\n"
                : "gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);\n";


            // End of main
            returnString += "}\n";

            return returnString;
        }

        /// <summary>
        /// Returns the GL_ES Precision precompiler
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        private static string ESPrecision()
        {
            return "#ifdef GL_ES\n" +
                   "  precision highp float;\n" +
                   "#endif\n\n";
        }

        private string PixelInputDeclarations()
        {
            var returnString = "";
            // #ifdef GL_ES
            returnString += ESPrecision();

            // Define number of lights
            var numberOfLights = SceneRenderer.AllLightResults.Count > 0 ? SceneRenderer.AllLightResults.Count : 1;
                // legacy code, should be larger one by default!
            returnString += $"#define MAX_LIGHTS {numberOfLights}\n";

            // LightStructDeclaration
            returnString += LightStructDeclaration();

            // UniformInputDelcaration (uniform Texture, etc.)
            returnString += ChannelInputDeclaration(_hasDiffuse, _hasDiffuseTexture, "Diffuse");
            returnString += ChannelInputDeclaration(_hasEmissive, _hasEmissiveTexture, "Emissive");
            returnString += SpecularInputDeclaration();
            returnString += BumbInputDeclaration(_hasBump);

            returnString += "varying vec3 vViewDir;\n";

            if (_hasNormals)
            {
                returnString += "varying vec3 vMVNormal;\n";
                returnString += "varying vec3 vNormal;\n";
            }

            if (_hasUVs)
                returnString += "varying vec2 vUV;\n";


            returnString += "varying vec4 surfacePos;\n";
            returnString += "uniform mat4 FUSEE_MV;\n";

            // Multipass
            returnString += "uniform sampler2D firstPassTex;\n";
            returnString += "uniform samplerCube envMap;\n";

            returnString += "  uniform mat4 FUSEE_IMV;\n";
            returnString += "  uniform mat4 FUSEE_IV;\n";

            // Shadow
            returnString += "varying vec4 shadowLight;\n";

            return returnString;
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

        private string ChannelInputDeclaration(bool hasChannel, bool hasChannelTexture, string channelName)
        {
            var returnString = "";

            if (!hasChannel)
                return returnString;

            // This will generate e.g. "  uniform vec4 DiffuseColor;"
            returnString += $"uniform vec3 {channelName}Color; \n";

            if (!hasChannelTexture)
                return returnString;

            // This will generate e.g.
            // "  uniform sampler2D DiffuseTexture;"
            // "  uniform float DiffuseMix;"
            returnString += $"uniform sampler2D {channelName}Texture; \n";
            returnString += $"uniform float {channelName}Mix; \n";

            return returnString;
        }

        private string SpecularInputDeclaration()
        {
            var returnString = "";

            if (!_hasSpecular)
                return returnString;

            returnString += ChannelInputDeclaration(_hasSpecular, _hasSpecularTexture, "Specular");

            returnString += "uniform float SpecularShininess;\n";
            returnString += "uniform float SpecularIntensity;\n";

            return returnString;
        }

        private static string BumbInputDeclaration(bool bumb)
        {
            var returnString = "";

            if (!bumb)
                return returnString;

            returnString += "uniform sampler2D BumpTexture;\n";
            returnString += "uniform float BumpIntensity;\n";

            return returnString;
        }

        private string PixelShaderMethods()
        {
            var returnString = "";

            // Ambient Light
            returnString += AmbientLightMethod();

            // Diffuse Light
            if (_hasDiffuse)
                returnString += DiffuseLightMethod();

            // Specular Light
            if (_hasSpecular)
                returnString += SpecularLightMethod();

            // Shadows
            returnString += ShadowFactorMethod();

            return returnString;
        }

        // TODO: Refactor
        private static string ShadowFactorMethod()
        {
            return @"
                    float CalcShadowFactor(vec4 fragPosLightSpace)
                {

                    // perform perspective divide for ortographic!            
                     vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
                    projCoords = projCoords * 0.5 + 0.5; // map to [0,1]
                    //float bias =  max(0.0005 * (1.0 - NoL), 0.001);  // bias to prevent shadow acne

                    float currentDepth = projCoords.z;
                    float pcfDepth = texture2D(firstPassTex, projCoords.xy).r;
                    float shadow = 0.0;

                /*     // Percentage closer filtering[Currently error with webgl - desktop needs ivec, web expects float for textureSize()]
                    // [http://http.developer.nvidia.com/GPUGems/gpugems_ch11.html]
                        const float texelSizeFloat = textureSize(firstPassTex, 0);
                        //vec2 texelSizeFloat = vec2(texelSize);
                        texelSizeFloat = 1.0 / texelSizeFloat;
                    for (int x = -1; x <= 1; ++x)
                    {
                        for (int y = -1; y <= 1; ++y)
                        {
                            float pcfDepth = texture2D(firstPassTex, projCoords.xy + vec2(x, y) * texelSizeFloat).r;
                            shadow += currentDepth > pcfDepth ? 1.0 : 0.0; // without currentDepth-bias because the number has to be so small, TODO: Fix this
                        }
                    }
                    shadow /= 32.0;
                */

                   shadow = currentDepth - 0.01 > pcfDepth ? 1.0 : 0.0;         

                 if (projCoords.z > 1.0)
                        shadow = 0.0;
        
                   return shadow; 
            }";
        }

        private string DiffuseLightMethod()
        {
            var returnString = "";
            returnString += "// returns intensity of diffuse reflection\n";
            returnString += "vec3 diffuseLighting(vec3 N, vec3 L, vec3 intensities)\n";
            returnString += "{\n";
            returnString += "   // calculation as for Lambertian reflection\n";
            returnString += "   float diffuseTerm = clamp(dot(N, L), 0.0, 1.0) ;\n";
            if (_hasDiffuseTexture)
                returnString +=
                    $"return texture2D({DiffuseTextureName}, vUV).rgb * {DiffuseMixName} * diffuseTerm * intensities;\n";
            else
                returnString += $"  return ({DiffuseColorName} * intensities * diffuseTerm);\n";
            returnString += "}\n";

            return returnString;
        }

        private string SpecularLightMethod()
        {
            var returnString = "";

            returnString += "// returns intensity of diffuse reflection\n";
            returnString += "vec3 specularLighting(vec3 N, vec3 L, vec3 V, vec3 intensities)\n";
            returnString += "{\n";
            returnString += "   float specularTerm = 0.0;\n";
            returnString += "   if(dot(N, L) > 0.0)\n";
            returnString += "   {\n";
            returnString += "   // half vector\n";
            returnString += "   vec3 H = normalize(L + V);\n";
            returnString += $"   specularTerm = max(0.0, pow(dot(N, H), {SpecularShininessName}));\n";
            returnString += "   }\n";
            returnString += $"  return ({SpecularColorName} * {SpecularIntensityName} * intensities) * specularTerm;\n";
            returnString += "}\n";

            return returnString;
        }
        
        private string AmbientLightMethod()
        {
            var returnString = "";

            returnString += "// returns intensity of reflected ambient lighting\n";
            returnString += "vec3 ambientLighting(float ambientCoefficient)\n";
            returnString += "{\n";
            if (EmissiveColorName != null)
                returnString += $"   return ({EmissiveColorName} * ambientCoefficient);\n";
            else
                returnString += "   return vec3(ambientCoefficient);\n";
            returnString += "}\n";

            return returnString;
        }

        private string ApplyLightMethod()
        {
            if (_hasApplyLightString)
                return ApplyLightFunction;

            var returnString = "";

            // Start of ApplyLight()
            returnString +=
                "vec3 ApplyLight(vec3 position, vec3 intensities, vec3 coneDirection, float attenuation, float ambientCoefficient, float coneAngle, int lightType) {\n";

            // ApplyLightParams
            returnString += ApplyLightParams();

            // Result
            returnString += "vec3 result = vec3(0);\n";

            // AtteunationFunction
            returnString += AttenuationFunction();

            // LightCalculation
            returnString += "if(lightType == 0) // PointLight\n";
            returnString += "{";
            returnString += $"  {PointLightCalculation()}\n";
            returnString += "}\n";
            returnString += "else if(lightType == 1 || lightType == 3) // ParallelLight or LegacyLight\n";
            returnString += "{";
            returnString += $"  {ParallelLightCalculation()}\n";
            returnString += "}\n";
            returnString += "else if(lightType == 2) // SpotLight\n";
            returnString += "{";
            returnString += $"  {SpotLightCalculation()}\n";
            returnString += "}\n";

            // Gamma Correction
            returnString += GammaCorrection();
            // Return Result
            returnString += "return result;\n";
            // End of ApplyLight()
            returnString += "}";

            return returnString;
        }

        private static string GammaCorrection()
        {
            return "vec3 gamma = vec3(1.0/2.2);\n" +
                   "result = pow(result, gamma);\n";
        }

        private string PixelBody()
        {
            var returnString = "";

            // ApplyLightMethod
            returnString += ApplyLightMethod();

            //returnString += "uniform mat4 FUSEE_IMV;";

            // Begin of main()
            returnString += "void main() {";


            returnString += "vec3 result = vec3(0.0);\n";
            returnString += "for(int i = 0; i < MAX_LIGHTS;i++)\n";
            returnString += "{\n";
            // TODO: Evaluate if this works with intel GLSL:
            returnString += @"
                vec3 currentPosition = allLights[i].position;
                vec3 currentIntensities = allLights[i].intensities;
                vec3 currentConeDirection = allLights[i].coneDirection;
                float currentAttenuation = allLights[i].attenuation;
                float currentAmbientCoefficient = allLights[i].ambientCoefficient;
                float currentConeAngle = allLights[i].coneAngle;
                int currentLightType = allLights[i].lightType; ";
            returnString +=
                "result += ApplyLight(currentPosition, currentIntensities, currentConeDirection, " +
                "currentAttenuation, currentAmbientCoefficient, currentConeAngle, currentLightType);\n";
            returnString += "}\n";

            /*
                        returnString += @"


                            vec3 N = vNormal;
                            vec3 eyeRay = normalize(surfacePos.xyz-FUSEE_IMV[3].xyz);
                            vec3 reflectVec = reflect(vViewDir, N);
                            vec3 R = reflect(vViewDir, normalize(N));


                            vec3 reflection = textureCube(envMap, N).rgb;   
               
            "; */

            returnString += "gl_FragColor = vec4(result, 1.0);\n";


            // End of main()
            returnString += "}";

            return returnString;
        }
        
        private string ApplyLightParams()
        {
            var outputString = "";

            outputString += "vec3 N = vMVNormal;\n";
            outputString += "vec3 L = normalize(position - surfacePos.xyz);\n"; // Position needed for Spot-, Parallel- and PointLight
            // check for LegacyLight and fallback to leagacy
            outputString += "if(lightType == 3) // LegacyLight\n";
            outputString += "   L = normalize(vec3(0.0,0.0,-1.0));\n"; // legacy mode

            outputString += "vec3 V = normalize(-surfacePos.xyz);\n"; // View is always -surfacePos due to light calculation in ModelViewSpace
            outputString += "vec2 o_texcoords = vUV;\n";
            outputString += "\n";
            outputString += "\n";
            outputString += "float shadowFactor = CalcShadowFactor(shadowLight); \n";
            outputString += "\n";

            // Diffuse, specular, color names
            outputString += "vec3 Idif = vec3(0);\n";
            outputString += "vec3 Ispe = vec3(0);\n";
            outputString += "vec3 diffuseColor = vec3(0);\n";

            // Ambient
            outputString += "vec3 Iamb = ambientLighting(ambientCoefficient);\n";

            // Diffuse
            if (_hasDiffuse)
            {
                outputString += "Idif = diffuseLighting(N, L, intensities);\n";
                if (_hasDiffuseTexture)
                    outputString +=
                        $"diffuseColor = texture2D({DiffuseTextureName}, o_texcoords).rgb * {DiffuseMixName};\n";
                else
                    outputString += $"diffuseColor = {DiffuseColorName};\n";
            }

            // Specular
            if (_hasSpecular)
                outputString += "Ispe = specularLighting(N, L, V, intensities);\n";

            outputString += "\n";
            outputString += "\n";

            return outputString;
        }

        private static string ParallelLightCalculation()
        {
            var returnString = "";
            returnString += "result = Iamb + diffuseColor * (1.0-shadowFactor) * (Idif + Ispe);\n";
            return returnString;
        }

        private static string PointLightCalculation()
        {
            var returnString = "\n";
            returnString += "result = Iamb + diffuseColor * (1.0-shadowFactor) * (Idif + Ispe) * att;\n";

            return returnString;
        }

        private static string SpotLightCalculation()
        {
            var returnString = "\n";
            returnString += "float lightToSurfaceAngle = dot(-L, coneDirection);\n";
            // coneDirection comes in normalized and multiplied with modelview
            returnString += "if (lightToSurfaceAngle > coneAngle)\n";
            // coneAngle comes in converted from degrees to radians
            returnString += "{\n";
            returnString += "   att *= (1.0 - (1.0 - lightToSurfaceAngle) * 1.0/(1.0 - coneAngle));\n";
            returnString += "}\n";
            returnString += "else\n";
            returnString += "{\n";
            returnString += "   att = 0.0;\n";
            returnString += "}\n";

            returnString += "\n";
            returnString += "\n";
            returnString += "       result = Iamb + diffuseColor  * (1.0-shadowFactor) * (Idif + Ispe) * att;\n";

            return returnString;
        }

        /// <summary>
        /// Taken from Unreal Engine 4.0:
        /// (saturate(1 − (distance/lightRadius)^4)^2) / (distance^2 + 1)
        /// TODO: Test if distance is plausible for every scene with / 1000.0
        /// </summary>
        /// <returns></returns>
        private static string AttenuationFunction()
        {
            var returnString = "";
            returnString += "float distanceToLight = distance(position, surfacePos.xyz) / 1000.0; \n";
            returnString += "float distance = pow(distanceToLight/attenuation,4.0);\n";
            returnString += "float att = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);\n";
            return returnString;
        }

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


        public static string StaticDiffuseColorName { get { return "DiffuseColor"; } }
        public static string StaticSpecularColorName { get { return "SpecularColor"; } }
        public static string StaticEmissiveColorName { get { return "EmissiveColor"; } }

        public static string StaticDiffuseTextureName { get { return "DiffuseTexture"; } }
        public static string StaticSpecularTextureName { get { return "SpecularTexture"; } }
        public static string StaticEmissiveTextureName { get { return "EmissiveTexture"; } }
        public static string StaticBumpTextureName { get { return "BumpTexture"; } }

        public static string StaticDiffuseMixName { get { return "DiffuseMix"; } }
        public static string StaticSpecularMixName { get { return "SpecularMix"; } }
        public static string StaticEmissiveMixName { get { return "EmissiveMix"; } }

        public static string StaticSpecularShininessName { get { return "SpecularShininess"; } }
        public static string StaticSpecularIntensityName { get { return "SpecularIntensity"; } }
        public static string StaticBumpIntensityName { get { return "BumpIntensity"; } }

        public static string LightDirectionName { get { return "LightDirection"; } }
        public static string LightColorName { get { return "LightColor"; } }
        public static string LightIntensityName { get { return "LightIntensity"; } }
    }
}







































/*
    public class ShaderCodeBuilder
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

        private LightingCalculationMethod _lightingCalculationMethod = LightingCalculationMethod.SIMPLE;

        // ReSharper disable once InconsistentNaming
        public string VS { get; }

        // ReSharper disable once InconsistentNaming
        public string PS { get; }

        public ShaderCodeBuilder(MaterialComponent mc, MeshComponent mesh, WeightComponent wc = null)
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

            //  Diagnostics.Log($"ShaderCodeBuilder, VS \n{VS}");
            //Diagnostics.Log($"ShaderCodeBuilder, PS \n{PS}");

        }


        private void MeshInputDeclarations(StringBuilder vs)
        {
            vs.Append($"{GLSLVersion()}\n");

            if (_hasVertices)
            {
                vs.Append("  attribute vec3 fuVertex;\n");

                //  if (_hasSpecular)
                //  {
                vs.Append("  varying vec3 vViewDir;\n");
                //  }
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

          //  if (_hasSpecular)
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
            vs.Append("  varying vec3 vMVNormal;\n");
            vs.Append("  uniform mat4 FUSEE_MV;\n");

            // Needed for Shadows
            vs.Append(" varying vec4 shadowLight;\n");
            vs.Append(" uniform mat4 shadowMVP;\n");
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
                    // Lighting done in view space... we need to convert the normals
                    if (_normalizeNormals)
                    {
                        // vs.Append("    vNormal = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);\n");
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

            // needed for lightcalculation
            vs.Append("    surfacePos =  FUSEE_MV * vec4(fuVertex, 1.0); \n");
            vs.Append("    shadowLight = shadowMVP * surfacePos; \n");
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

            ps.Append("  varying vec3 vViewDir;\n");


            if (_hasNormals) 
                ps.Append("  varying vec3 vMVNormal;\n");

            if (_hasUVs)
                ps.Append("  varying vec2 vUV;\n");

            ps.Append("  varying vec4 surfacePos;\n");
            ps.Append("  uniform mat4 FUSEE_MV;\n");

            ps.Append("\n uniform sampler2D firstPassTex; \n");


            ps.Append("\n varying vec4 shadowLight; \n");

            ps.Append("\n varying vec4 modelSpace; \n");

            string calcshadow = @"
                    float CalcShadowFactor(vec4 fragPosLightSpace)
                {

            // perform perspective divide for ortographic!            
             vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
            projCoords = projCoords * 0.5 + 0.5; // map to [0,1]
            //float bias =  max(0.0005 * (1.0 - NoL), 0.001);  // bias to prevent shadow acne

            float currentDepth = projCoords.z;
            float pcfDepth = texture2D(firstPassTex, projCoords.xy).r;
            float shadow = 0.0;

       /*     // Percentage closer filtering[Currently error with webgl - desktop needs ivec, web expects float for textureSize()]
            // [http://http.developer.nvidia.com/GPUGems/gpugems_ch11.html]
                const float texelSizeFloat = textureSize(firstPassTex, 0);
                //vec2 texelSizeFloat = vec2(texelSize);
                texelSizeFloat = 1.0 / texelSizeFloat;
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    float pcfDepth = texture2D(firstPassTex, projCoords.xy + vec2(x, y) * texelSizeFloat).r;
                    shadow += currentDepth > pcfDepth ? 1.0 : 0.0; // without currentDepth-bias because the number has to be so small, TODO: Fix this
                }
            }
            shadow /= 32.0;
*/
/*

           

           shadow = currentDepth - 0.0001 > pcfDepth ? 1.0 : 0.0;         

         if (projCoords.z > 1.0)
                shadow = 0.0;
        
           return shadow; 
      }";

            /*



            
       /*      // perform perspective divide for ortographic!
            vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
            projCoords = projCoords * 0.5 + 0.5; // map to [0,1]
            float bias =  max(0.0005 * (1.0 - NoL), 0.0001);  // bias to prevent shadow acne
            float currentDepth = projCoords.z;
            float shadowDepth = texture2D(firstPassTex, projCoords.xy).r;
            float shadow = 0.0;

            // Percentage closer filtering[Currently error with webgl - desktop needs ivec, web expects float for textureSize()]
            // [http://http.developer.nvidia.com/GPUGems/gpugems_ch11.html]
           ivec2 texelSize = textureSize(firstPassTex, 0);
           vec2 texelSizeFloat = vec2(texelSize);
            texelSizeFloat = 1.0 / texelSizeFloat;
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    float pcfDepth = texture2D(firstPassTex, projCoords.xy + vec2(x, y) * texelSizeFloat).r;
                    shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0; // without currentDepth-bias because the number has to be so small, TODO: Fix this
                }
            }
            shadow /= 16.0;

            shadow = currentDepth > shadowDepth ? 1.0 : 0.0;

            if (projCoords.z > 0.99997)
                shadow = 0.0;

            return shadow; 



            */
        /*    ps.Append(calcshadow);

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
            else if (_lightingCalculationMethod == LightingCalculationMethod.SIMPLE)
            {
                vs.Append("\n\n\n");
                vs.Append($"           {AmbientLightningMethod()}\n");
                if (_hasDiffuse)
                    vs.Append($"           {DiffuseLightingMethod()}\n");
                if (_hasSpecular)
                    vs.Append($"           {SpecularLightingMethod()}\n");
                vs.Append("\n\n\n");
                vs.Append("vec3 ApplyLight(vec3 position, vec3 intensities, vec3 coneDirection, float attenuation, float ambientCoefficient, float coneAngle, int lightType)\n");
                vs.Append("{\n");
                vs.Append("     vec3 result = vec3(0.0, 0.0, 0.0);\n");
                vs.Append($"     {BlinnPhongCalculation()}\n");
                vs.Append($"     {AttenuationFunction()}\n");
                vs.Append("     if(lightType == 0) // PointLight\n");
                vs.Append("     {");
                vs.Append($"           {PointLightCalculation()}\n");
                vs.Append("     }\n");
                vs.Append("     else if(lightType == 1) // ParallelLight\n");
                vs.Append("     {");
                vs.Append($"            {ParallelLightCalculation()}\n");
                vs.Append("     }\n");
                vs.Append("     else if(lightType == 2) // SpotLight\n");
                vs.Append("     {");
                vs.Append($"            {SpotLightCalculation()}\n");
                vs.Append("     }\n");
                vs.Append("     return result;\n");
                vs.Append("}\n");

            }
            /*    else
                { // TODO: Try to parse Diffusevars to PBR values
                    // needed Methods
                    vs.Append("\n\n\n");
                    vs.Append($"{GgxMethod()}");
                    vs.Append($"{FresnelMethod()}");
                    vs.Append($"{GeometryMethod()}");
                    vs.Append($"{AmbientLightningMethod()}");

                    // Method
                    vs.Append("/******* ApplyLight Method ****//*\n");
                    vs.Append("vec3 ApplyLight(vec3 position, vec3 intensities, vec3 coneDirection, float attenuation, float ambientCoefficient, float coneAngle, int lightType)\n");
                    vs.Append("{\n");
                    vs.Append($"     {PhysicallyBasedShadingMethod()}\n");
                    vs.Append($"     {AttenuationFunction()}\n");
                    vs.Append("     if(lightType == 0) // PointLight\n");
                    vs.Append("     {");
                    vs.Append($"           {PointLightCalculation()}\n");
                    vs.Append("     }\n");
                    vs.Append("     else if(lightType == 1) // ParallelLight\n");
                    vs.Append("     {");
                    vs.Append($"            {ParallelLightCalculation()}\n");
                    vs.Append("     }\n");
                    vs.Append("     else if(lightType == 2) // SpotLight\n");
                    vs.Append("     {");
                    vs.Append($"            {SpotLightCalculation()}\n");
                    vs.Append("     }\n");
                    vs.Append("     return result;\n");
                    vs.Append("}\n");
                }
        }


        private string AttenuationFunction()
        {
            var outputString = "";
            outputString += "       float distanceToLight = distance(position, surfacePos.xyz);\n";
            outputString += "       float att = clamp(1.0 - distanceToLight*distanceToLight/(attenuation*attenuation), 0.0, 1.0);\n";
            outputString += "       att *= att;\n";
            return outputString;
        }

        private string AmbientLightningMethod()
        {
            var outputString = "\n";
            outputString += "// returns intensity of reflected ambient lighting\n";
            outputString += "vec3 ambientLighting(float ambientCoefficient)\n";
            outputString += "{\n";
            if (EmissiveColorName != null)
                outputString += $"   return ({EmissiveColorName} * ambientCoefficient);\n";
            else
                outputString += "   return vec3(ambientCoefficient);\n";
            outputString += "}\n";

            return outputString;
        }

        private string DiffuseLightingMethod()
        {
            var outputString = "\n";
            outputString += "// returns intensity of diffuse reflection\n";
            outputString += "vec3 diffuseLighting(vec3 N, vec3 L, vec3 intensities)\n";
            outputString += "{\n";
            outputString += "   // calculation as for Lambertian reflection\n";
            outputString += "   float diffuseTerm = clamp(dot(N, L) / (length(L) * length(N)), 0.0, 1.0) ;\n";
            if(_hasDiffuseTexture)
                outputString += $"return texture2D({DiffuseTextureName}, vUV).rgb * {DiffuseMixName} * diffuseTerm * intensities;\n";
            else
                outputString += $"  return ({DiffuseColorName} * intensities * diffuseTerm);\n";
            outputString += "}\n";

            return outputString;
        }

        private string SpecularLightingMethod()
        {
            var outputString = "\n";
            outputString += "// returns intensity of diffuse reflection\n";
            outputString += "vec3 specularLighting(vec3 N, vec3 L, vec3 V, vec3 intensities)\n";
            outputString += "{\n";
            outputString += "   float specularTerm = 0.0;\n";
            outputString += "   if(dot(N, L) > 0.0)\n";
            outputString += "   {\n";
            outputString += "   // half vector\n";
            outputString += "   vec3 H = normalize(L + V);\n";
            if (_hasSpecular)
            {
                outputString += $"   specularTerm = max(0.0, pow(dot(N, H), {SpecularShininessName}));\n";
                outputString += "   }\n";
                outputString += $"  return ({SpecularColorName} * {SpecularIntensityName} * intensities) * specularTerm;\n";
            }
            else
            { 
              outputString += "   }\n";
            outputString += $"  return intensities;\n";
            }
           
            outputString += "}\n";

            return outputString;
        }

        private string BlinnPhongCalculation()
        {
            var outputString = "\n";

            outputString += "vec3 o_normal = vMVNormal;\n";
            outputString += "vec3 o_toLight = normalize(position - surfacePos.xyz);\n";
            outputString += "vec3 o_toCamera = normalize(vViewDir - surfacePos.xyz);\n";
            outputString += "vec2 o_texcoords = vUV;\n";
            outputString += "\n";
            outputString += "\n";
            outputString += "float shadowFactor = CalcShadowFactor(shadowLight); \n";
            outputString += "vec3 L = o_toLight;\n";
            outputString += "vec3 V = o_toCamera;\n";
            outputString += "vec3 N = o_normal;\n";
            outputString += "vec3 Iamb = ambientLighting(ambientCoefficient);\n";
            if (_hasDiffuse)
                outputString += "vec3 Idif = diffuseLighting(N, L, intensities);\n";
            else
                outputString += "vec3 Idif = vec3(0.1,0.1,0.1);\n";
            if (_hasSpecular)
                outputString += "vec3 Ispe = specularLighting(N, L, V, intensities);\n";
            else
                outputString += "vec3 Ispe = vec3(0.1,0.1,0.1);\n";
            outputString += "\n";
            if (DiffuseTextureName != null)
                outputString += $"vec3 diffuseColor = texture2D({DiffuseTextureName}, o_texcoords).rgb * {DiffuseMixName};\n";
            else
                outputString += $"vec3 diffuseColor = {DiffuseColorName};\n";
            outputString += "\n";

            outputString += "\n";

            return outputString;
        }

        /// <summary>
        /// ParallelLight
        /// </summary>
        /// <returns></returns>
        private static string ParallelLightCalculation()
        {
            var outputString = "\n";
            outputString += "       result = Iamb + (1.0-shadowFactor) *  (Idif + Ispe);\n";
            return outputString;
        }

        /// <summary>
        /// PointLight, with specular component and half-vector
        /// </summary>
        /// <returns></returns>
        private static string PointLightCalculation()
        {
            var outputString = "\n";
            outputString += "\n";
            outputString += "\n";
            outputString += "       result = Iamb + diffuseColor  * (1.0-shadowFactor) * (Idif + Ispe) * att;\n";

            return outputString;
        }

        private static string SpotLightCalculation()
        {
            var outputString = "\n";
            outputString += "       float lightToSurfaceAngle = dot(-o_toLight, coneDirection);\n"; // coneDirection comes in normalized and multiplied with modelview
            outputString += "       if (lightToSurfaceAngle > coneAngle)\n"; // coneAngle comes in converted from degrees to radians
            outputString += "       {\n";
            outputString += "           att *= (1.0 - (1.0 - lightToSurfaceAngle) * 1.0/(1.0 - coneAngle));\n";
            outputString += "       }\n";
            outputString += "       else\n";
            outputString += "       {\n";
            outputString += "       att = 0.0;\n";
            outputString += "       }\n";

            outputString += "\n";
            outputString += "\n";
            outputString += "       result = Iamb + diffuseColor  * (1.0-shadowFactor) * (Idif + Ispe) * att;\n";

            return outputString;
        }

/*
        private string DiffuseEnergyRatio()
        {
            return "float diffuseEnergyRatio(float f0, vec3 n, vec3 l){\n return 1.0 - fresnel(f0, n, l);\n }\n";
        }
*/

        // TODO: Cleanup & improve
/*
        private string PhysicallyBasedShadingMethod()
        {

            var outputString = "";


            outputString += "vec3 o_normal = vMVNormal;\n";
            outputString += "vec3 o_toLight = normalize(position - surfacePos.xyz);\n";
            outputString += "vec3 o_toCamera = normalize(vViewDir - surfacePos.xyz);\n";
            outputString += "vec2 o_texcoords = vUV;\n";
            outputString += "\n";
            outputString += "\n";
            outputString += "vec3 L = o_toLight;\n";
            outputString += "vec3 V = o_toCamera;\n";
            outputString += "vec3 N = o_normal;\n";
            outputString += "float NdotV = max(0.0,dot(N,V));\n";
            outputString += "float NdotL = max(0.0,dot(N,L));\n";
            outputString += "vec3 H = normalize(L + V);\n";
            outputString += "vec3 result = vec3(0);\n";



            outputString += $"float fresnelValue={FresnelValue.ToString("0000.0000", CultureInfo.InvariantCulture)};\n";
            outputString += $"float diffuseFractionValue={DiffuseFractionValue.ToString("0000.0000", CultureInfo.InvariantCulture)};\n";
            outputString += $"float roughnessValue={RoughnessValue.ToString("0000.0000", CultureInfo.InvariantCulture)};\n";
            outputString += " \n";
            outputString += " float G = ggx(N, L, V, roughnessValue, fresnelValue);\n";
            outputString += " float F = fresnel(fresnelValue, N, L);\n";
            outputString += " float D = geometry(N, H, V, L, roughnessValue);\n";
            outputString += " float brdf_spec = G * F * D / (NdotL * NdotV * 3.14);\n";

            if (_hasSpecular)
                outputString += $" vec3 Ispe = (brdf_spec * intensities) * {SpecularColorName} * {SpecularIntensityName};\n";
            else
                outputString += $" vec3 Ispe = (brdf_spec * intensities) * vec3(0.3,0.3,0.3) * 0.5;\n";
            outputString += $" vec3 Idif =  NdotL * (diffuseFractionValue + Ispe * (1.0 - diffuseFractionValue));\n";

            outputString += "vec3 Iamb = ambientLighting(ambientCoefficient);\n";
            if (DiffuseTextureName != null)
                outputString += $"vec3 diffuseColor = texture2D({DiffuseTextureName}, o_texcoords).rgb * {DiffuseMixName};\n";
            else
                outputString += $"vec3 diffuseColor = {DiffuseColorName};\n";

            return outputString;
        }
*/

/*
        private static string GgxMethod()
        {
            var outputString = "\n";
            outputString += "// Normal distribution function, GGX/Trowbridge-Reitz\n";
            outputString += "float ggx(vec3 N, vec3 L, vec3 V, float roughness, float F0)\n";
            outputString += "{\n";
            outputString += "   float alpha = roughness*roughness;\n";
            outputString += "   vec3 H = normalize(L + V);\n";
            outputString += "   float dotLH = max(0.0, dot(L,H));\n";
            outputString += "   float dotNH = max(0.0, dot(N,H));\n";
            outputString += "   float dotNL = max(0.0, dot(N,L));\n";
            outputString += "   float alphaSqr = alpha * alpha;\n";
            outputString += "   float denom = dotNH * dotNH * (alphaSqr - 1.0) + 1.0;\n";
            outputString += "   float D = alphaSqr / (3.141592653589793 * denom * denom);\n";
            outputString += "   float F = F0 + (1.0 - F0) * pow(1.0 - dotLH, 5.0);\n";
            outputString += "   float k = 0.5 * alpha;\n";
            outputString += "   float k2 = k * k;\n";
            outputString += "   return dotNL * D * F / (dotLH*dotLH*(1.0-k2)+k2);\n";
            outputString += "}\n";

            return outputString;
        }
*/

//        private static string FresnelMethod()
//        {
//            /*
//             "float fresnel(float f0, vec3 n, vec3 l){\n\
//                                    return f0 + (1.0-f0) * pow(1.0- dot(n, l), 5.0);\n\
//                                }\n";
//
//            */
//
//            var outputString = "\n";
//            outputString += "// Fresnel term, Schlick's approximation\n";
//            outputString += "float fresnel(float f0, vec3 n, vec3 l)\n";
//            outputString += "{\n";
//            outputString += "   return f0 + (1.0-f0) * pow(1.0- dot(n, l), 5.0);\n";
//            outputString += "}\n";
//
//            return outputString;
//        }

//        private static string GeometryMethod()
//        {
//
//            /*
//
//            "float geometry(vec3 n, vec3 h, vec3 v, vec3 l, float roughness){\n\
//                                                                float NdotL_clamped= max(dot(n, l), 0.0);\n\
//                                                                float NdotV_clamped= max(dot(n, v), 0.0);\n\
//                                                                float k= roughness * sqrt(2.0/3.14159265);\n\
//                                                                float one_minus_k= 1.0 -k;\n\
//                                                                return ( NdotL_clamped / (NdotL_clamped * one_minus_k + k) ) * ( NdotV_clamped / (NdotV_clamped * one_minus_k + k) );\n\
//                                                            }\n";
//
//    */
//            var outputString = "\n";
//            outputString += "// Schlick's approximation of Smith's shadow equation\n";
//            outputString += "float geometry(vec3 n, vec3 h, vec3 v, vec3 l, float roughness)\n";
//            outputString += "{\n";
//            outputString += "   float NdotL_clamped= max(dot(n, l), 0.0);\n";
//            outputString += "   float NdotV_clamped= max(dot(n, v), 0.0);\n";
//            outputString += "   float k= roughness * sqrt(2.0/3.14159265);\n";
//            outputString += "   float one_minus_k= 1.0 -k;\n";
//            outputString += "   return ( NdotL_clamped / (NdotL_clamped * one_minus_k + k) ) * ( NdotV_clamped / (NdotV_clamped * one_minus_k + k) );\n";
//            outputString += "}\n";
//
//            return outputString;
//        }

        // ReSharper disable once InconsistentNaming
/*        private void PSBody(StringBuilder vs)
        {
            if (_lightingCalculationMethod == LightingCalculationMethod.SIMPLE)
            {

                vs.Append("\n\n\n");
                vs.Append("void main()\n");
                vs.Append("{\n");
                vs.Append("    vec3 result = vec3(0.0);\n");
                // Annotation: allLights.length() only supported in version 300; WebGL needs version 100.
                // Therefore we need a workaround.
                vs.Append("    for(int i = 0; i < MAX_LIGHTS;i++)\n");
                vs.Append("    {\n");
                vs.Append("         Light currentLight = allLights[0];\n");
                vs.Append("         result += ApplyLight(allLights[0].position, allLights[0].intensities, allLights[0].coneDirection, " +
                          "allLights[0].attenuation, allLights[0].ambientCoefficient, allLights[0].coneAngle, allLights[0].lightType);\n");
                vs.Append("    }\n");
                vs.Append($"    {GammaCorrection()}\n");
                vs.Append("float shadowFactor = CalcShadowFactor(shadowLight); \n");
                vs.Append("    gl_FragColor = vec4(final_light,1.0);\n");
                //vs.Append("    gl_FragColor = vec4(vec3(1.0,0.0,0.0),1.0);\n");
                vs.Append("}\n");
            }

            else
            {
                var advancedShader = @"
                    #ifdef GL_ES
                      precision highp float;
                    #endif

                    #define PI 3.1415926535897932384626433832795

                    uniform float diffuseFractionValue;
                    uniform	float roughnessValue;

                    uniform vec3 position;


                    // Physically based shading model
                    // parameterized with the below options

                    // Microfacet specular = D*G*F / (4*NoL*NoV) = D*Vis*F
                    // Vis = G / (4*NoL*NoV)


                    float Square( float x )
                    {
	                    return x*x;
                    }

                    vec2 Square( vec2 x )
                    {
	                    return x*x;
                    }

                    vec3 Square( vec3 x )
                    {
	                    return x*x;
                    }

                    vec4 Square( vec4 x )
                    {
	                    return x*x;
                    }

                    float Pow5( float x )
                    {
	                    float xx = x*x;
	                    return xx * xx * x;
                    }

                // GGX / Trowbridge-Reitz
                // [Walter et al. 2007, Microfacet models for refraction through rough surfaces]
                float D_GGX(float Roughness, float NoH)
                {
                    float a = Roughness * Roughness;
                    float a2 = a * a;
                    float d = (NoH * a2 - NoH) * NoH + 1.0; // 2 mad
                    return a2 / (PI * d * d);                   // 4 mul, 1 rcp
                }

                vec3 Diffuse_Lambert(vec3 DiffuseColor )
                {
                    return DiffuseColor * (1.0 / PI);
                }

                // Appoximation of joint Smith term for GGX
                // [Heitz 2014, Understanding the Masking-Shadowing Function in Microfacet-Based BRDFs]
                float Vis_SmithJointApprox(float Roughness, float NoV, float NoL)
                {
                    float a = Square(Roughness);
                    float Vis_SmithV = NoL * (NoV * (1.0 - a) + a);
                    float Vis_SmithL = NoV * (NoL * (1.0 - a) + a);
                    return 0.5 * 1.0 / (Vis_SmithV + Vis_SmithL); // rcp to 1/X
}

                // [Schlick 1994, An Inexpensive BRDF Model for Physically-Based Rendering]
                                vec3 F_Schlick(vec3 SpecularColor, float VoH)
                {
                    float Fc = Pow5(1.0 - VoH);                 // 1 sub, 3 mul
                                                                //return Fc + (1 - Fc) * SpecularColor;		// 1 add, 3 mad

                    // Anything less than 2% is physically impossible and is instead considered to be shadowing
                    // SpecularColor.g to SpecularColor.y
                    return clamp((50.0 * SpecularColor.y), 0.0, 1.0) * Fc + (1.0 - Fc) * SpecularColor;
                }


                // [Enviroment BDRF Aprox]
                vec3 EnvBRDFApprox(vec3 SpecularColor, float Roughness, float NoV)
                {
                // [ Lazarov 2013, Getting More Physical in Call of Duty: Black Ops II ]
                // Adaptation to fit our G term.
                const vec4 c0 = vec4(-1.0, -0.0275, -0.572, 0.022);
                                const vec4 c1 = vec4(1.0, 0.0425, 1.04, -0.04);
                                vec4 r = Roughness * c0 + c1;
                                float a004 = min(r.x * r.x, exp2(-9.28 * NoV)) * r.x + r.y;
                                vec2 AB = vec2(-1.04, 1.04) * a004 + r.zw;

                // Anything less than 2% is physically impossible and is instead considered to be shadowing
                // In ES2 this is skipped for performance as the impact can be small
                // Note: this is needed for the 'specular' show flag to work, since it uses a SpecularColor of 0
                // Changed SpecularColor.g to SpecularColor.y
                AB.y *= clamp((50.0 * SpecularColor.y), 0.0, 1.0);


                    return SpecularColor * AB.x + AB.y;
                }


                float EnvBRDFApproxNonmetal(float Roughness, float NoV)
                 {
                    // Same as EnvBRDFApprox( 0.04, Roughness, NoV )
                    const vec2 c0 = vec2(-1.0, -0.0275);
                    const vec2 c1 = vec2(1.0, 0.0425);
                    vec2 r = Roughness * c0 + c1;
                    return min(r.x * r.x, exp2(-9.28 * NoV)) * r.x + r.y;
                }


                void main()
                {
                    vec3 result = vec3(0.0);


                    vec3 o_normal = vMVNormal;
                    vec3 o_toLight = normalize(position - surfacePos.xyz);
                    vec3 o_toCamera = normalize(vViewDir - surfacePos.xyz);
                    vec2 o_texcoords = vUV;


                    vec3 L = o_toLight;
                    vec3 V = o_toCamera;
                    vec3 N = o_normal;

                    vec3 H = normalize(V + L);

                    float NdotV = clamp(abs(dot(N, V)) + 1e-5, 0.0, 1.0);
                    float NdotL = clamp(dot(N, L), 0.0, 1.0);
                    float VdotH = clamp(dot(V, H), 0.0, 1.0);
                    float NdotH = clamp(dot(N, H), 0.0, 1.0);

                    vec3 RayDirection = 2.0 * dot(V, N) * N - V;
                    float RdotV = clamp(dot(RayDirection, L), 0.0, 1.0);

                    // EnvBRDF
                    // TODO: Check for metal / nonmetal
                    // TODO: Multipass with real envTexture
                    //	vec3 SpecularTextureColor = vec3(texture2D(texture, vUV));
                    //	vec3 SpecularEnvColor = EnvBRDFApprox(SpecularTextureColor, roughnessValue, RdotV);

                    // Generalized microfacet specular
                    float D = D_GGX(roughnessValue, NdotH) * diffuseFractionValue;
                    float Vis = Vis_SmithJointApprox(roughnessValue, NdotV, NdotL);";
                vs.Append(advancedShader);

                if (_hasSpecular)
                    vs.Append("vec3 F = F_Schlick(SpecularColor, VdotH); // F0 is in SpecularColor\n");
                else
                    vs.Append("vec3 F = F_Schlick(vec3(1.0,0.0,0.0), VdotH); // F0 is in SpecularColor\n");

                string weiter;
                if (_hasDiffuseTexture)
                    weiter = @"
                    // [Other diffuses have no visible change but much higher compile costs]
                    // see: http://graphicrants.blogspot.de/2013/08/specular-brdf-reference.html
                    vec3 Diffuse = Diffuse_Lambert(DiffuseColor);

                    vec2 randomPoint;
                    vec3 color = vec3(0.0, 0.0, 0.0);

                    result = texture2D(DiffuseTexture, vUV).rgb + (D * Vis) * F;


                    vec3 gamma = vec3(1.0 / 2.2);
                    vec3 final_light = pow(result, gamma);

                    gl_FragColor = vec4(result, 1.0);
                }
                ";
                else
                    weiter = @"
                    // [Other diffuses have no visible change but much higher compile costs]
                    // see: http://graphicrants.blogspot.de/2013/08/specular-brdf-reference.html
                    vec3 Diffuse = Diffuse_Lambert(DiffuseColor);

                    vec2 randomPoint;
                    vec3 color = vec3(0.0, 0.0, 0.0);

                    result = Diffuse + (D * Vis) * F;


                    vec3 gamma = vec3(1.0 / 2.2);
                    vec3 final_light = pow(result, gamma);

                    gl_FragColor = vec4(result, 1.0);
                }
                ";

                vs.Append(weiter);
            }
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
        
        public static string DiffuseColorName { get { return "DiffuseColor"; } }
        public static string SpecularColorName { get { return "SpecularColor"; } }
        public static string EmissiveColorName { get { return "EmissiveColor"; } }
        public static string DiffuseTextureName { get { return "DiffuseTexture"; } }
        public static string SpecularTextureName { get { return "SpecularTexture"; } }
        public static string EmissiveTextureName { get { return "EmissiveTexture"; } }
        public static string BumpTextureName { get { return "BumpTexture"; } }
        public static string DiffuseMixName { get { return  "DiffuseMix"; } }
        public static string SpecularMixName { get { return "SpecularMix"; } }
        public static string EmissiveMixName { get { return  "EmissiveMix"; } }
        public static string SpecularShininessName { get { return  "SpecularShininess"; } }
        public static string SpecularIntensityName { get { return "SpecularIntensity"; } }
        public static string BumpIntensityName { get { return "BumpIntensity"; } }
        
        
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
            else if (mc.GetType() == typeof(MaterialPBRComponent))
            {
                var mpbr = mc as MaterialPBRComponent;
                if (mpbr == null) return;

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
            _lightingCalculationMethod = SceneRenderer.LightingCalculationMethod;
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

        public float FresnelValue => (_hasFresnel) ? _fresnelValue : 0.5f;

        public float DiffuseFractionValue => (_hasDiffuseFraction) ? _diffuseFractionValue : 0.5f;

        public float RoughnessValue => (_hasRoughness) ? _roughnessValue : 0.5f;

        private string _applyLightString;

        private string _applyFragmentString;

        private float _fresnelValue;

        private float _diffuseFractionValue;

        private float _roughnessValue;

        #endregion
    }

}
*/