using System.Text;
using Fusee.Serialization;

namespace Examples.SceneViewer
{
    class ShaderCodeBuilder
    {
        private bool _hasVertices, _hasNormals, _hasUVs, _hasColors;
        private bool _hasDiffuse, _hasSpecular, _hasEmissive, _hasBump;
        private bool _hasDiffuseTexture, _hasSpecularTexture, _hasEmissiveTexture;
        private bool _normalizeNormals;

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

        public ShaderCodeBuilder(MaterialContainer mc, MeshContainer mesh)
        {
            float f1 = 1;
            f1.GetType();
            _normalizeNormals = true;
            if (mesh != null)
                AnalyzeMesh(mesh);
            else
            {
                _hasVertices = _hasNormals = _hasUVs = true;
            }
            AnalyzMaterial(mc);

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

        private void AnalyzeMesh(MeshContainer mesh)
        {
            _hasVertices = (mesh.Vertices != null && mesh.Vertices.Length > 0);
            _hasNormals = (mesh.Normals != null && mesh.Normals.Length > 0);
            _hasUVs = (mesh.UVs != null && mesh.UVs.Length > 0);
            _hasColors = false;
            // _hasColors = (mesh.Colors != null && mesh.Colors.Length > 0);
        }

        private void AnalyzMaterial(MaterialContainer mc)
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

        private void MeshInputDeclarations(StringBuilder vs)
        {
            if (_hasVertices)
            {
                vs.Append("  attribute vec3 fuVertex;\n");
                if (_hasSpecular)
                    vs.Append("  varying vec3 vViewPos;\n");
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
            if (_hasNormals)
                vs.Append("  uniform mat4 FUSEE_ITMV;\n");

            if (_hasSpecular)
            {
                vs.Append("  uniform mat4 FUSEE_MV;\n");
                vs.Append("  uniform mat4 FUSEE_P;\n");
            }
            else
            {
                vs.Append("  uniform mat4 FUSEE_MV;\n");
                vs.Append("  uniform mat4 FUSEE_MVP;\n");                
            }
        }

        private void VSBody(StringBuilder vs)
        {
            vs.Append("\n\n  void main()\n  {\n");
            if (_hasNormals)
            {
                if (_normalizeNormals)
                    vs.Append("    vNormal = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);\n");
                else
                    vs.Append("    vNormal = mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal;\n");
            }

            if (_hasSpecular)
            {
                vs.Append("    vec4 vViewPosTemp = FUSEE_MV * vec4(fuVertex, 1);\n");
                vs.Append("    vViewPos = vec3(vViewPosTemp)/vViewPosTemp.w;\n");
                vs.Append("    gl_Position = FUSEE_P * vViewPosTemp;\n");
            }
            else
            {
                vs.Append("    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);\n");
            }

            if (_hasUVs)
                vs.Append("    vUV = fuUV;\n");

            vs.Append("  }\n\n");
        }

        private void PixelInputDeclarations(StringBuilder ps)
        {
            
            ps.Append("#ifdef GL_ES\n");
            ps.Append("  precision highp float;\n");
            ps.Append("#endif\n\n");

            ChannelInputDeclaration(ps, _hasDiffuse, _hasDiffuseTexture, "Diffuse");
            SpecularInputDeclaration(ps);
            ChannelInputDeclaration(ps, _hasEmissive, _hasEmissiveTexture, "Emissive");
            BumpInputDeclaration(ps);

            if (_hasSpecular)
            {
                // shouldn't bee needed... ps.Append("  uniform mat4 FUSEE_V;\n");
                ps.Append("  varying vec3 vViewPos;\n");
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
            ps.Append("\n\n  void main()\n  {\n");
            ps.Append("    vec3 result = vec3(0, 0, 0);\n\n");

            AddNormalVec(ps);
            AddCameraVec(ps);
            AddLightVec(ps);

            AddEmissiveChannel(ps);
            AddDiffuseChannel(ps);
            AddSpecularChannel(ps);

            ps.Append("\n    gl_FragColor = vec4(result, 1.0);\n");
            ps.Append("  }\n\n");
        }

        private void AddCameraVec(StringBuilder ps)
        {
            ps.Append("    vec3 Camera = vec3(0, 0, -1.0);\n");
        }

        private void AddLightVec(StringBuilder ps)
        {
            ps.Append("    vec3 Light = vec3(0, 0, -1.0);\n");
            ps.Append("    vec3 LightColor = vec3(1.0, 1.0, 1.0);\n");
        }
        private void AddNormalVec(StringBuilder ps)
        {
            if (_hasBump)
            {
                ps.Append("\n\n    //*********** BUMP *********\n");
                // TODO: Got Work. We'll probably need Tangents and Bitangents?
                // http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-13-normal-mapping/
                ps.Append("    vec3 Normal = vNormal + BumpIntensity * texture2D(BumpTexture, vUV);\n\n");
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
            ps.Append("    float diffFactor = dot(Light, Normal);\n");
            ps.Append("    result += DiffuseBaseColor * LightColor * max(diffFactor, 0.0);\n");
        }

        private void AddSpecularChannel(StringBuilder ps)
        {
            if (!_hasSpecular)
                return;

            ps.Append("\n\n    //*********** SPECULAR *********\n");
            if (!_hasDiffuse)
                ps.Append("    float diffFactor = dot(Light, Normal);\n");

            ps.Append("      if (diffFactor > 0.0) {\n");
            AddChannelBaseColorCalculation(ps, _hasSpecularTexture, "Specular");
            ps.Append("      vec3 h = normalize(Light + Camera);\n");

            ps.Append(
                "      result += SpecularBaseColor * LightColor * SpecularIntensity * pow(max(0.0, dot(h, Normal)), SpecularShininess);\n");
            ps.Append("    }\n");
        }

        private void AddChannelBaseColorCalculation(StringBuilder ps,  bool hasChannelTexture, string channelName)
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

        public string DiffuseColorName { get { return (_hasDiffuse) ? "DiffuseColor" : null; } }
        public string SpecularColorName { get { return (_hasSpecular) ? "SpecularColor" : null; } }
        public string EmissiveColorName { get { return (_hasEmissive) ? "EmissiveColor" : null; } }

        public string DiffuseTextureName { get { return (_hasDiffuseTexture) ? "DiffuseTexture" : null; } }
        public string SpecularTextureName { get { return (_hasSpecularTexture) ? "SpecularTexture" : null; } }
        public string EmissiveTextureName { get { return (_hasEmissiveTexture) ? "EmissiveTexture" : null; } }
        public string BumpTextureName { get { return (_hasBump) ? "BumpTexture" : null; } }

        public string DiffuseMixName { get { return (_hasDiffuse) ? "DiffuseMix" : null; } }
        public string SpecularMixName { get { return (_hasSpecular) ? "SpecularMix" : null; } }
        public string EmissiveMixName { get { return (_hasEmissive) ? "EmissiveMix" : null; } }

        public string SpecularShininessName { get { return (_hasSpecular) ? "SpecularShininess" : null; } }
        public string SpecularIntensityName { get { return (_hasSpecular) ? "SpecularIntensity" : null; } }
        public string BumpIntensityName { get { return (_hasBump) ? "BumpIntensity" : null; } }
    }
}
