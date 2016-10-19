using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    // ReSharper disable InconsistentNaming
    //TODO: Implement ShaderCode for Light
    //TODO: Standard ApplyLight()
    //TODO: Refactor and restructre
    //TODO: Restructure with legacymode if no lightcomponent is found
    //TODO: Implement PBR
    //TODO: Implement usage of FBO if supported
    //TODO: Implement usage of more than one RenderPass (w&wo FBO)
    class ShaderCodeBuilder
    {
        public static IList<LightResult> _allLights = new List<LightResult>();
        
  
        private bool _hasVertices, _hasNormals, _hasUVs, _hasColors;
        private bool _hasDiffuse, _hasSpecular, _hasEmissive, _hasBump;
        private bool _hasDiffuseTexture, _hasSpecularTexture, _hasEmissiveTexture;
        private bool _hasWeightMap;
        private int _nBones;
        private bool _normalizeNormals;

        // Needed for MaterialLightComponent
        private bool _hasApplyLightString;
        private bool _hasFragmentString;
        private LightningCalculationMethod _lightningCalculationMethod = LightningCalculationMethod.SIMPLE;


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

        /// <summary>
        /// If we have a MaterialPBRComponent this constructor is called.
        /// </summary>
        /// <param name="mlc">The MaterialLightComponent</param>
        /// <param name="pbrMaterialPbrComponent"></param>
        /// <param name="mesh">The Mesh</param>
        /// <param name="wc">WeightCompoennt for animations</param>
        public ShaderCodeBuilder(MaterialPBRComponent pbrMaterialPbrComponent, MeshComponent mesh, WeightComponent wc = null)
        {

            // Check WC
            if (wc != null)
            {
                _hasWeightMap = true;
                _nBones = wc.Joints.Count;
            }
            else
            {
                _nBones = 0;
            }

            //float f1 = 1;
            //var type = f1.GetType();
            _normalizeNormals = true;

            // Check for mesh
            if (mesh != null)
                AnalyzeMesh(mesh);
            else
            {
                _hasVertices = _hasNormals = _hasUVs = true;
            }

            // Analyze the Material
            AnalyzeMaterial(pbrMaterialPbrComponent);
            // VS
            StringBuilder vs = new StringBuilder();
            MeshInputDeclarations(vs);
            MatrixDeclarations(vs);
            VSBody(vs);
            _vs = vs.ToString();

            // PS
            StringBuilder ps = new StringBuilder();
            PixelInputDeclarations(ps);
            PSPBRBody(ps, pbrMaterialPbrComponent);
            _ps = ps.ToString();

        }


        /// <summary>
        /// If we have a MaterialLightComponent this constructor is called.
        /// </summary>
        /// <param name="mlc">The MaterialLightComponent</param>
        /// <param name="mesh">The Mesh</param>
        /// <param name="wc">WeightCompoennt for animations</param>
        public ShaderCodeBuilder(MaterialLightComponent mlc, MeshComponent mesh, WeightComponent wc = null)
        {

            // Check WC
            if (wc != null)
            {
                _hasWeightMap = true;
                _nBones = wc.Joints.Count;
            }
            else
            {
                _nBones = 0;
            }

            //float f1 = 1;
            //var type = f1.GetType();
            _normalizeNormals = true;

            // Check for mesh
            if (mesh != null)
                AnalyzeMesh(mesh);
            else
            {
                _hasVertices = _hasNormals = _hasUVs = true;
            }

            // Analyze the Material
            AnalyzeMaterial(mlc);


            // VS
            StringBuilder vs = new StringBuilder();
            MeshInputDeclarations(vs);
            MatrixDeclarations(vs);
            VSBody(vs);
            _vs = vs.ToString();

            // PS
            StringBuilder ps = new StringBuilder();
            PixelInputDeclarations(ps);
            PSCustomBody(ps, mlc);
            _ps = ps.ToString();
       
        }

        public ShaderCodeBuilder(MaterialComponent mc, MeshComponent mesh, WeightComponent wc = null)
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

            //float f1 = 1;
            //var type = f1.GetType();
            _normalizeNormals = true;
            if (mesh != null)
                AnalyzeMesh(mesh);
            else
            {
                _hasVertices = _hasNormals = _hasUVs = true;
            }
            AnalyzeMaterial(mc);

            //TODO: Use/switch lightning method to build PS & VS

            StringBuilder vs = new StringBuilder();
            MeshInputDeclarations(vs);
            MatrixDeclarations(vs);
            VSBody(vs);
            _vs = vs.ToString();

            StringBuilder ps = new StringBuilder();
            PixelInputDeclarations(ps);
            PSBody(ps);
            _ps = ps.ToString();

        }

        private static void ParseLights(StringBuilder ps)
        {
            // no LightComponent found
            //  if (_allLights.Count == 0)
            //     return;

            // LightComponent found, add Light struct
            ps.Append("\n\n " +
                      "uniform struct Light {\n" +
                      "vec4 position;\n" +
                      "vec3 intensities;\n" +
                      "vec3 coneDirection;\n" +
                      "float attenuation;\n" +
                      "float ambientCoefficient;\n" +
                      "float coneAngle;\n" +
                      "int lightType;\n" +
                      "};\n" +
                      "\n\n");

            ps.Append("\n\n " +
                      "uniform Light allLights[MAX_LIGHTS]; \n" +
                      "\n\n");

        }

        private void AnalyzeMesh(MeshComponent mesh)
        {
            _hasVertices = (mesh.Vertices != null && mesh.Vertices.Length > 0);
            _hasNormals = (mesh.Normals != null && mesh.Normals.Length > 0);
            _hasUVs = (mesh.UVs != null && mesh.UVs.Length > 0);
            _hasColors = false;
            // _hasColors = (mesh.Colors != null && mesh.Colors.Length > 0);
        }

        private void AnalyzeMaterial(MaterialComponent mc)
        {
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
        }

        private void AnalyzeMaterial(MaterialLightComponent mlc)
            {
            _hasDiffuse = mlc.HasDiffuse;
            if (_hasDiffuse)
                _hasDiffuseTexture = (mlc.Diffuse.Texture != null);
            _hasSpecular = mlc.HasSpecular;
            if (_hasSpecular)
                _hasSpecularTexture = (mlc.Specular.Texture != null);
            _hasEmissive = mlc.HasEmissive;
            if (_hasEmissive)
                _hasEmissiveTexture = (mlc.Emissive.Texture != null);
            _hasBump = mlc.HasBump; // always has a texture...

            _hasApplyLightString = (mlc.ApplyLightString != null);
            _hasFragmentString = (mlc.FragmentShaderString != null);

            }

        private void AnalyzeMaterial(MaterialPBRComponent mlc)
        {
            _hasDiffuse = mlc.HasDiffuse;
            if (_hasDiffuse)
                _hasDiffuseTexture = (mlc.Diffuse.Texture != null);
            _hasSpecular = mlc.HasSpecular;
            if (_hasSpecular)
                _hasSpecularTexture = (mlc.Specular.Texture != null);
            _hasEmissive = mlc.HasEmissive;
            if (_hasEmissive)
                _hasEmissiveTexture = (mlc.Emissive.Texture != null);
            _hasBump = mlc.HasBump; // always has a texture...
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
            if(_hasWeightMap){
                vs.Append("uniform mat4 FUSEE_P;\n");
                vs.Append("uniform mat4 FUSEE_V;\n");
            }
            else
            {
                vs.Append("  uniform mat4 FUSEE_MVP;\n");
            }
                
            if(_hasWeightMap)
                vs.Append("  uniform mat4 FUSEE_BONES[" + _nBones + "];\n");

            // needed for lightning calc
            vs.Append("varying vec4 surfacePos;\n");
            vs.Append("uniform mat4 FUSEE_MV;\n");

        }



        private void VSBody(StringBuilder vs)
        {
            // needed for cook torrance:
            vs.Append("\n\n  varying vec3 oNormal; \n  \n"); 

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

            // needed for spotlight
            vs.Append(" surfacePos =  vec4(fuVertex, 1.0); \n");
            vs.Append(" oNormal = fuNormal; \n");

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
            
            ps.Append("#ifdef GL_ES\n");
            ps.Append("  precision highp float;\n");
            ps.Append("#endif\n\n");

            // define max lights
            var numberOfLights = SceneRenderer.AllLightResults.Count > 0 ? SceneRenderer.AllLightResults.Count : 1;

            ps.Append("\n\n #define MAX_LIGHTS " + numberOfLights + "\n\n");


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

        private void ChannelInputDeclaration(StringBuilder ps, bool hasChannel, bool hasChannelTexture, string channelName)
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
            ParseLights(ps);
            CreatePSBodyFromLightningMethod(ps);

        }

        // This is called when no MaterialLightComponent is found and ...
        private void CreatePSBodyFromLightningMethod(StringBuilder ps)
        {
            /////////////// Legacy Mode 
            // ...no LightComponents are in scene
            if (SceneRenderer.AllLightResults.Count == 0)
            {
              
                
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
            /////////////// Create ApplyLight from lightning method 
            // ...LightComponents are in scene
            else
            {
                AddStandardLightningCalculation(ps);
            }
        }

        // This is called when MaterialLightComponent is found
        private void PSCustomBody(StringBuilder ps, MaterialLightComponent mlc)
        {
            ParseLights(ps);
           

            AddApplyLightCalculation(ps, mlc);

            ps.Append("\n\n  void main()\n  {\n");
            ps.Append("    vec3 result = vec3(0, 0, 0);\n\n");

            AddNormalVec(ps);
            AddCameraVec(ps);

            // ApplyLight() is always called
            ps.Append("\n   for(int i = 0; i < 3000; i++) { \n ");
            ps.Append("\n   if(i > MAX_LIGHTS) break; \n ");
            ps.Append("\n   result += ApplyLight(allLights[i]); \n ");
            ps.Append("\n   } \n ");
            ps.Append("\n   vec3 gamma = vec3(1.0/2.2);\n ");
            ps.Append("\n   vec3 final_light = pow(result, gamma); \n ");
            ps.Append("\n   gl_FragColor = vec4(final_light, 1.0);\n");
            ps.Append("  }\n\n");
        }

        private void PSPBRBody(StringBuilder ps, MaterialPBRComponent mpbr)
        {
            ParseLights(ps);
            AddNormalVec(ps);
            AddCameraVec(ps);

            if(!_hasSpecular)
                ps.Append("  varying vec3 vViewDir;\n");


            AddcooktorrancePBRMat(ps, mpbr);

            ps.Append("\n\n  void main()\n  {\n");
            ps.Append("    vec3 result = vec3(0, 0, 0);\n\n");


            // ApplyLight() is always called
            ps.Append("\n   for(int i = 0; i < 3000; i++) { \n ");
            ps.Append("\n   if(i > MAX_LIGHTS) break; \n ");
            ps.Append("\n   result += ApplyLight(allLights[i]); \n ");
            ps.Append("\n   vec3 gamma = vec3(1.0/2.2);\n ");
            ps.Append("\n   vec3 final_light = pow(result, gamma); \n ");
            ps.Append("\n   gl_FragColor = vec4(final_light, 1.0);\n");
            ps.Append("  }\n\n");
        }

        private void AddcooktorrancePBRMat(StringBuilder ps, MaterialPBRComponent mpbr)
        {

            /*

        uniform struct Light {
           vec4 position;
           vec3 intensities;
           float attenuation;
           float ambientCoefficient;
           float coneAngle;
           vec3 coneDirection;
           float lightType;
           } allLights[MAX_LIGHTS];


            SpecularBaseColor * light.intensities * light.ambientCoefficient * SpecularIntensity
            DiffuseBaseColor

       */
            if (_hasEmissive)
            {
                ps.Append("\n\n    //*********** Emissive *********\n");
                AddChannelBaseColorCalculation(ps, _hasEmissiveTexture, "Emissive");
            }

            ps.Append("\n\n    //*********** DIFFUSE *********\n");
            AddChannelBaseColorCalculation(ps, _hasDiffuseTexture, "Diffuse");
            ps.Append("\n\n    //*********** Specular *********\n");
            AddChannelBaseColorCalculation(ps, _hasSpecularTexture, "Specular");

            // needed for spotlight
            ps.Append("\n\n   varying vec4 surfacePos; \n");
            ps.Append("\n\n   varying mat4 FUSEE_ITMV; \n");

            // needed for CookTorrance
            ps.Append("\n\n   varying vec3 oNormal; \n");



            // TODO: Vars from SceneRenderer.
            ps.Append("vec3 ApplyLight(Light light) { \n\n");
            ps.Append(" \n\n" +
                      "float roughnessValue = ");
            // CultureInfo needed for float conversion fullstop, otherwise he formats floats to: #,##
                ps.AppendFormat("{0} ;\n", mpbr.RoughnessValue.ToString(CultureInfo.InvariantCulture));
                ps.AppendFormat("float F0 = {0};\n", mpbr.FresnelReflectance.ToString(CultureInfo.InvariantCulture));
                ps.AppendFormat("float k = {0};\n", mpbr.DiffuseFraction.ToString(CultureInfo.InvariantCulture));
               ps.Append("vec3 lightColor = light.intensities;\n\n" +
                      "vec3 normal = Normal;\n\n" +
                      "// do the lighting calculation for each fragment.\n" +
                      "float NdotL = max(dot(normal, normalize(Camera)), 0.0);\n\n" +
                      " float specular = 0.0;\n\n" +
                      "if(NdotL > 0.0) {\n\n" +
                      "vec3 eyeDir = vViewDir;\n" +
                      "// calculate intermediary values\n" +
                      "vec3 halfVector = normalize(eyeDir + Camera);\n" +
                      "float NdotH = max(dot(normal, halfVector), 0.0); \n" +
                      "float NdotV = max(dot(normal, eyeDir), 0.0); // note: this could also be NdotL, which is the same value\n" +
                      "float VdotH = max(dot(eyeDir, halfVector), 0.0);\n" +
                      "float mSquared = roughnessValue * roughnessValue;\n\n" +
                      "// geometric attenuation\n" +
                      "float NH2 = 2.0 * NdotH;\n" +
                      "float g1 = (NH2 * NdotV) / VdotH;\n" +
                      "float g2 = (NH2 * NdotL) / VdotH;\n" +
                      "float geoAtt = min(1.0, min(g1, g2));\n\n" +
                      " // roughness (or: microfacet distribution function)\n" +
                      " // beckmann distribution function\n" +
                      "float r1 = 1.0 / ( 4.0 * mSquared * pow(NdotH, 4.0));\n" +
                      "float r2 = (NdotH * NdotH - 1.0) / (mSquared * NdotH * NdotH);\n" +
                      "float roughness = r1 * exp(r2);\n\n" +
                      "// fresnel - Schlick approximation\n" +
                      "float fresnel = pow(1.0 - VdotH, 5.0);\n" +
                      "fresnel *= (1.0 - F0);\n" +
                      "fresnel += F0;\n" +
                      "specular = SpecularBaseColor * ( (fresnel * geoAtt * roughness) / (NdotV * NdotL * 3.14));\n\n" +
                      "}\n\n" +
                      "return light.ambientCoefficient * DiffuseBaseColor * lightColor * NdotL * (k + specular * (1.0 - k));\n");
            ps.Append("} \n\n");

        }

        private void AddApplyLightCalculation(StringBuilder ps, MaterialLightComponent mlc)
        {
            if (!_hasApplyLightString)
                return;

            if (!CheckApplyLightStringForErrors(mlc.ApplyLightString))
                throw new ArgumentException($"Error while compiling ApplyLight(). Are you sure this:\n {mlc.ApplyLightString} \nis correct?" + $"\nPerhaps (re)consult the documentation of MaterialLightComponent class.");

            ps.Append("\n\n" + mlc.ApplyLightString);
        }

        private static bool CheckApplyLightStringForErrors(string applyLightMethod)
        {
            const string applyLightString = "vec3 ApplyLight(Light light)";
            return !string.IsNullOrEmpty(applyLightMethod) && applyLightMethod.Contains(applyLightString);
        }

        // This is called when no MaterialLightComponent is found but there are LightComponents in the scene
        private void AddStandardLightningCalculation(StringBuilder ps)
        {
            if (_hasApplyLightString)
                return;

            switch (_lightningCalculationMethod)
            {
                case LightningCalculationMethod.SIMPLE:
                    AddBlinnphongLightning(ps);
                    break;
                case LightningCalculationMethod.ADVANCED:
                    // TODO: Convert material params to cook_torrance
                    AddcooktorranceLightning(ps);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Lightning calculation method: {_lightningCalculationMethod} not found!");
            }

            // TODO: To for loop for all lights
            // ApplyLight() is always called
            ps.Append("\n\n  void main()\n  {\n");
            ps.Append("    vec3 result = vec3(0, 0, 0);\n\n");
            // TODO MAX ITTERATIONS?? - WebGL needs this: 
            ps.Append("\n   for(int i = 0; i < 3000; i++) { \n ");
            ps.Append("\n   if(i > MAX_LIGHTS) break; \n ");
            ps.Append("\n   result += ApplyLight(allLights[i]); \n ");
            ps.Append("\n   } \n ");
            ps.Append("\n   vec3 gamma = vec3(1.0/2.2);\n ");
            ps.Append("\n   vec3 final_light = pow(result, gamma); \n ");
            ps.Append("\n   gl_FragColor = vec4(final_light, 1.0);\n");
            ps.Append("  }\n\n");

        }

        private  void AddBlinnphongLightning(StringBuilder ps)
        {
            // get all necessary vars for calculation
            AddNormalVec(ps);
            AddCameraVec(ps);
            AddLightVec(ps);

            if (_hasEmissive)
            {
                ps.Append("\n\n    //*********** Emissive *********\n");
                AddChannelBaseColorCalculation(ps, _hasEmissiveTexture, "Emissive");
            }

            ps.Append("\n\n    //*********** DIFFUSE *********\n");
            AddChannelBaseColorCalculation(ps, _hasDiffuseTexture, "Diffuse");
            ps.Append("\n\n    //*********** Specular *********\n");
            AddChannelBaseColorCalculation(ps, _hasSpecularTexture, "Specular");

            // needed for spotlight
            ps.Append("\n\n   varying vec4 surfacePos; \n");
            
            // TODO IMPLEMENT Parallel Light
            ps.Append("vec3 ApplyLight(Light light) { \n\n");
            ps.Append("// switch type: 0 = Point; 1 = Parallel; 2 = Spot;\n");
            // POINTLIGHT
            ps.Append("////// POINTLIGHT \n\n");
            ps.Append("if(light.lightType == 0) { \n\n");

            ps.Append("vec3 result = vec3(0, 0, 0); \n\n");
            ps.Append("vec3 DiffuseBaseColor = DiffuseColor; \n\n");
            ps.Append("float diffFactor = dot(LDir, Normal); \n\n");
            ps.Append("  result += DiffuseBaseColor * light.intensities * light.ambientCoefficient * max(diffFactor, 0.0); \n\n");


            ps.Append("  if (diffFactor > 0.0) \n\n");
            ps.Append(" { \n\n");
            ps.Append(" vec3 SpecularBaseColor = SpecularColor; \n\n");
            ps.Append(" vec3 h = normalize(light.coneDirection + Camera); \n\n");
            ps.Append(" result += SpecularBaseColor * light.intensities * light.ambientCoefficient * SpecularIntensity * pow(max(0.0, dot(h, vNormal)), SpecularShininess); \n\n");
            ps.Append(" } \n\n");
            ps.Append(" return result; \n\n");


            ps.Append("     }\n\n");


            // SPOTLIGHT
            ps.Append("////// SPOTLIGHT \n\n");
            ps.Append("if(light.lightType == 2) { \n\n");

            ps.Append(" vec3 surfaceToLight; \n");
            ps.Append("  float attenuation = 1.0; \n\n");
           
            ps.Append(" surfaceToLight = normalize(light.position.xyz - surfacePos.xyz); \n");
            ps.Append(" float distanceToLight = length(light.position.xyz - surfacePos.xyz); \n");
            ps.Append("  attenuation = 1.0 / (1.0 + light.attenuation * pow(distanceToLight, 2.0)); \n\n");

            ps.Append(" //cone restrictions (affects attenuation) \n");
            ps.Append(" float lightToSurfaceAngle = degrees(acos(dot(-surfaceToLight, normalize(light.coneDirection)))); \n\n");
            ps.Append(" if(lightToSurfaceAngle > light.coneAngle) { \n\n");
            ps.Append("  attenuation = 0.0; } \n\n");
         
               
            ps.Append("vec3 result = vec3(0, 0, 0); \n\n");
            ps.Append("vec3 DiffuseBaseColor = DiffuseColor; \n\n");
            ps.Append("float diffFactor = dot(LDir, Normal); \n\n");
            ps.Append("  result += attenuation * DiffuseBaseColor * light.intensities * light.ambientCoefficient * max(diffFactor, 0.0); \n\n");


            ps.Append("  if (diffFactor > 0.0) \n\n");
            ps.Append(" { \n\n");
            ps.Append(" vec3 SpecularBaseColor = SpecularColor; \n\n");
            ps.Append(" vec3 h = normalize(light.coneDirection + Camera); \n\n");
            ps.Append(" result += attenuation * SpecularBaseColor * light.intensities * light.ambientCoefficient * SpecularIntensity * pow(max(0.0, dot(h, Normal)), SpecularShininess); \n\n");
            ps.Append(" } \n\n");
            ps.Append(" return result; \n\n");
            ps.Append("     } \n\n");

     

            // End of Method
            ps.Append("}\n\n");
        }

        private static void AddcooktorranceLightning(StringBuilder ps)
        {
            //TODO: Add shader code
            ps.Append("\n\n" + " vec3 ApplyLight() { return vec3(0.0, 1.0, 1.0); } \n\n");
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

            ps.Append("      result += SpecularBaseColor * LColor * LIntensity * SpecularIntensity * pow(max(0.0, dot(h, Normal)), SpecularShininess);\n");
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


        private string _vs;

        public string VS
        {
            get { return _vs; }
        }

        private string _ps;

        public string PS
        {
            get { return _ps; }
        }

        public string DiffuseColorName
        {
            get { return (_hasDiffuse) ? "DiffuseColor" : null; }
        }

        public string SpecularColorName
        {
            get { return (_hasSpecular) ? "SpecularColor" : null; }
        }

        public string EmissiveColorName
        {
            get { return (_hasEmissive) ? "EmissiveColor" : null; }
        }

        public string DiffuseTextureName
        {
            get { return (_hasDiffuseTexture) ? "DiffuseTexture" : null; }
        }

        public string SpecularTextureName
        {
            get { return (_hasSpecularTexture) ? "SpecularTexture" : null; }
        }

        public string EmissiveTextureName
        {
            get { return (_hasEmissiveTexture) ? "EmissiveTexture" : null; }
        }

        public string BumpTextureName
        {
            get { return (_hasBump) ? "BumpTexture" : null; }
        }

        public string DiffuseMixName
        {
            get { return (_hasDiffuse) ? "DiffuseMix" : null; }
        }

        public string SpecularMixName
        {
            get { return (_hasSpecular) ? "SpecularMix" : null; }
        }

        public string EmissiveMixName
        {
            get { return (_hasEmissive) ? "EmissiveMix" : null; }
        }

        public string SpecularShininessName
        {
            get { return (_hasSpecular) ? "SpecularShininess" : null; }
        }

        public string SpecularIntensityName
        {
            get { return (_hasSpecular) ? "SpecularIntensity" : null; }
        }

        public string BumpIntensityName
        {
            get { return (_hasBump) ? "BumpIntensity" : null; }
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
