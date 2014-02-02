//using Fusee.Math;

//namespace Fusee.Engine
//{
//    /// <summary>
//    /// Contains all pixel and vertex shaders and a method to create a ShaderProgram in Rendercontext.
//    /// Handles the <see cref="ShaderProgram"/> creation.
//    /// </summary>
//    public static class Shaders
//    {

//        /// <summary>
//        /// Creates the shader in RenderContext and returns a ShaderProgram.
//        /// </summary>
//        /// <param name="name">ShaderName.</param>
//        /// <param name="rc">RenderContext.</param>
//        /// <returns></returns>
//        public static ShaderProgram GetShader(string name, RenderContext rc)
//        {
//            if (name == "texture")
//            {
//                ShaderProgram spSimple = rc.CreateShader(VsSimpleTexture, PsSimpleTexture);
//                return spSimple;
//            }
//            if (name == "diffuse")
//            {
//                ShaderProgram spDiffuse = rc.CreateShader(VsDiffuse, PsDiffuse);
//                return spDiffuse;
//            }

//            if (name == "specular")
//            {
//                ShaderProgram spSpecular = rc.CreateShader(VsSpecular, PsSpecular);
//                return spSpecular;
//            }

//            if (name == "bump")
//            {
//                ShaderProgram spBump = rc.CreateShader(VsBump, PsBump);
//                return spBump;
//            }

//            if (name == "color")
//            {
//                ShaderProgram spOneColor = rc.CreateShader(VsSimpleColor, PsSimpleColor);
//                return spOneColor;
//            }

//            ShaderProgram spColor = rc.CreateShader(VsSimpleColor, PsSimpleColor);
//            IShaderParam color = rc.GetShaderParam(spColor, "color");
//            rc.SetShaderParam(color, new float4(1, 1, 1, 1));
//            return spColor;
//        }


//        private const string VsSimpleTexture = @"
//        #ifdef GL_ES
//            precision mediump float;
//        #endif
//        attribute vec3 fuVertex;
//        attribute vec3 fuNormal;
//        attribute vec2 fuUV;
//
//        varying vec3 vNormal;
//        varying vec2 vUV;
//
//        uniform mat4 FUSEE_MVP;
//        uniform mat4 FUSEE_ITMV;
//
//        void main(){
//            vUV = fuUV;
//            gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
//            vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
//        }
//        ";
//        private const string PsSimpleTexture = @"
//        #ifdef GL_ES
//            precision mediump float;
//        #endif
//
//        uniform sampler2D texture1;
//        varying vec3 vNormal;
//        varying vec2 vUV;
//
//        void main(){
//            gl_FragColor = max(dot(vec3(0,0,1),normalize(vNormal)), 0.2) * texture2D(texture1, vUV);
//        }
//        ";

//        private const string VsDiffuse = @"
//attribute vec4 fuColor;
//attribute vec3 fuVertex;
//attribute vec3 fuNormal;
//attribute vec2 fuUV;
//       
//uniform mat4 FUSEE_MVP;
//uniform mat4 FUSEE_MV;
//
//varying vec2 vUV;
//varying vec3 vNormal;
//varying vec3 vViewPos;
//
//vec3 vPos;
// 
//void main()
//{
//    vec4 vViewPosTemp = FUSEE_MV * vec4(fuVertex, 1);
//    vViewPos = vec3(vViewPosTemp)/vViewPosTemp.w;      
//    vUV = fuUV;
//    vNormal = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);
//    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
//}";


//        private const string PsDiffuse = @"
//#ifdef GL_ES
//    precision highp float;
//#endif
//
//uniform sampler2D texture1;
//
//uniform vec3 FUSEE_L0_POSITION;
//uniform vec3 FUSEE_L1_POSITION;
//uniform vec3 FUSEE_L2_POSITION;
//uniform vec3 FUSEE_L3_POSITION;
//uniform vec3 FUSEE_L4_POSITION;
//uniform vec3 FUSEE_L5_POSITION;
//uniform vec3 FUSEE_L6_POSITION;
//uniform vec3 FUSEE_L7_POSITION;
//
//uniform vec4 FUSEE_L0_DIFFUSE;
//uniform vec4 FUSEE_L1_DIFFUSE;
//uniform vec4 FUSEE_L2_DIFFUSE;
//uniform vec4 FUSEE_L3_DIFFUSE;
//uniform vec4 FUSEE_L4_DIFFUSE;
//uniform vec4 FUSEE_L5_DIFFUSE;
//uniform vec4 FUSEE_L6_DIFFUSE;
//uniform vec4 FUSEE_L7_DIFFUSE;
//
//uniform vec4 FUSEE_L0_AMBIENT;
//uniform vec4 FUSEE_L1_AMBIENT;
//uniform vec4 FUSEE_L2_AMBIENT;
//uniform vec4 FUSEE_L3_AMBIENT;
//uniform vec4 FUSEE_L4_AMBIENT;
//uniform vec4 FUSEE_L5_AMBIENT;
//uniform vec4 FUSEE_L6_AMBIENT;
//uniform vec4 FUSEE_L7_AMBIENT;
//
//uniform float FUSEE_L0_ACTIVE;
//uniform float FUSEE_L1_ACTIVE;
//uniform float FUSEE_L2_ACTIVE;
//uniform float FUSEE_L3_ACTIVE;
//uniform float FUSEE_L4_ACTIVE;
//uniform float FUSEE_L5_ACTIVE;
//uniform float FUSEE_L6_ACTIVE;
//uniform float FUSEE_L7_ACTIVE;
//
//uniform vec3 FUSEE_L0_DIRECTION;
//uniform vec3 FUSEE_L1_DIRECTION;
//uniform vec3 FUSEE_L2_DIRECTION;
//uniform vec3 FUSEE_L3_DIRECTION;
//uniform vec3 FUSEE_L4_DIRECTION;
//uniform vec3 FUSEE_L5_DIRECTION;
//uniform vec3 FUSEE_L6_DIRECTION;
//uniform vec3 FUSEE_L7_DIRECTION;
//
//uniform float FUSEE_L0_SPOTANGLE;
//uniform float FUSEE_L1_SPOTANGLE;
//uniform float FUSEE_L2_SPOTANGLE;
//uniform float FUSEE_L3_SPOTANGLE;
//uniform float FUSEE_L4_SPOTANGLE;
//uniform float FUSEE_L5_SPOTANGLE;
//uniform float FUSEE_L6_SPOTANGLE;
//uniform float FUSEE_L7_SPOTANGLE;
//
//varying vec3 vNormal;
//varying vec2 vUV;
//varying vec3 vViewPos;
//
//void CalcDirectLight(vec4 difColor, vec4 ambColor, vec3 direction, inout vec4 intensity) {
//    intensity += ambColor;
//    intensity += max(dot(-normalize(direction),normalize(vNormal)),0.0) * difColor;
//}
//
//void CalcPointLight(vec4 difColor, vec4 ambColor, vec3 position, inout vec4 intensity) {
//    intensity += ambColor;
//    vec3 pos = position - vViewPos;
//    intensity += max(dot(normalize(pos),normalize(vNormal)),0.0) * difColor;   
//}
//
//void CalcSpotLight(vec4 difColor, vec4 ambColor, vec3 position, vec3 direction, float angle, inout vec4 intensity) {
//    intensity += ambColor;
//    vec3 pos = position - vViewPos;
//    float alpha = acos(dot(normalize(pos), normalize(-direction)));
//    if(alpha < angle){
//        intensity += max(dot(normalize(pos),normalize(vNormal)),0.0) * difColor;  
//    }     
//}
// 
//void main()
//{
//    vec4 endIntensity = vec4(0,0,0,0);
//    if(FUSEE_L0_ACTIVE != 0.0){
//        if(FUSEE_L0_ACTIVE == 1.0)
//            CalcDirectLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_DIRECTION, endIntensity);
//        if(FUSEE_L0_ACTIVE == 2.0)
//            CalcPointLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_POSITION, endIntensity);
//        if(FUSEE_L0_ACTIVE == 3.0)
//            CalcSpotLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_POSITION, FUSEE_L0_DIRECTION, FUSEE_L0_SPOTANGLE, endIntensity);
//    }  
//
//    if(FUSEE_L1_ACTIVE != 0.0){
//        if(FUSEE_L1_ACTIVE == 1.0)
//            CalcDirectLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_DIRECTION, endIntensity);
//        if(FUSEE_L1_ACTIVE == 2.0)
//            CalcPointLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_POSITION, endIntensity);
//        if(FUSEE_L1_ACTIVE == 3.0)
//            CalcSpotLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_POSITION, FUSEE_L1_DIRECTION, FUSEE_L1_SPOTANGLE, endIntensity);
//    }  
//
//    if(FUSEE_L2_ACTIVE != 0.0){
//        if(FUSEE_L2_ACTIVE == 1.0)
//            CalcDirectLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_DIRECTION, endIntensity);
//        if(FUSEE_L2_ACTIVE == 2.0)
//            CalcPointLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_POSITION, endIntensity);
//        if(FUSEE_L2_ACTIVE == 3.0)
//            CalcSpotLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_POSITION, FUSEE_L2_DIRECTION, FUSEE_L2_SPOTANGLE, endIntensity);
//    }  
//
//    if(FUSEE_L3_ACTIVE != 0.0){
//        if(FUSEE_L3_ACTIVE == 1.0)
//            CalcDirectLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_DIRECTION, endIntensity);
//        if(FUSEE_L3_ACTIVE == 2.0)
//            CalcPointLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_POSITION, endIntensity);
//        if(FUSEE_L3_ACTIVE == 3.0)
//            CalcSpotLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_POSITION, FUSEE_L3_DIRECTION, FUSEE_L3_SPOTANGLE, endIntensity);
//    }  
//
//    if(FUSEE_L4_ACTIVE != 0.0){
//        if(FUSEE_L4_ACTIVE == 1.0)
//            CalcDirectLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_DIRECTION, endIntensity);
//        if(FUSEE_L4_ACTIVE == 2.0)
//            CalcPointLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_POSITION, endIntensity);
//        if(FUSEE_L4_ACTIVE == 3.0)
//            CalcSpotLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_POSITION, FUSEE_L4_DIRECTION, FUSEE_L4_SPOTANGLE, endIntensity);
//    }  
//
//    if(FUSEE_L5_ACTIVE != 0.0){
//        if(FUSEE_L5_ACTIVE == 1.0)
//            CalcDirectLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_DIRECTION, endIntensity);
//        if(FUSEE_L5_ACTIVE == 2.0)
//            CalcPointLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_POSITION, endIntensity);
//        if(FUSEE_L5_ACTIVE == 3.0)
//            CalcSpotLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_POSITION, FUSEE_L5_DIRECTION, FUSEE_L5_SPOTANGLE, endIntensity);
//    }  
//
//    if(FUSEE_L6_ACTIVE != 0.0){
//        if(FUSEE_L6_ACTIVE == 1.0)
//            CalcDirectLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_DIRECTION, endIntensity);
//        if(FUSEE_L6_ACTIVE == 2.0)
//            CalcPointLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_POSITION, endIntensity);
//        if(FUSEE_L6_ACTIVE == 3.0)
//            CalcSpotLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_POSITION, FUSEE_L6_DIRECTION, FUSEE_L6_SPOTANGLE, endIntensity);
//    }  
//
//    if(FUSEE_L7_ACTIVE != 0.0){
//        if(FUSEE_L7_ACTIVE == 1.0)
//            CalcDirectLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_DIRECTION, endIntensity);
//        if(FUSEE_L7_ACTIVE == 2.0)
//            CalcPointLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_POSITION, endIntensity);
//        if(FUSEE_L7_ACTIVE == 3.0)
//            CalcSpotLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_POSITION, FUSEE_L7_DIRECTION, FUSEE_L7_SPOTANGLE, endIntensity);
//    }  
//
//    gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
//}";



//        private const string VsSpecular = @"
//
//            attribute vec3 fuVertex;
//            attribute vec3 fuNormal;
//            attribute vec2 fuUV;
//                  
//            varying vec3 vNormal;
//            varying vec2 vUV;
//            varying vec3 vViewPos;
//
//            uniform mat4 FUSEE_MV; 
//            uniform mat4 FUSEE_MVP;
//
//            uniform float FUSEE_L0_ACTIVE;
//            uniform float FUSEE_L1_ACTIVE;
//            uniform float FUSEE_L2_ACTIVE;
//            uniform float FUSEE_L3_ACTIVE;
//            uniform float FUSEE_L4_ACTIVE;
//            uniform float FUSEE_L5_ACTIVE;
//            uniform float FUSEE_L6_ACTIVE;
//            uniform float FUSEE_L7_ACTIVE;
//
//            void main()
//            {
//                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
//                vUV = fuUV;
//                vNormal = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);
//                vec4 vViewTemp = FUSEE_MV * vec4(fuVertex, 1);
//                vViewPos = vec3(vViewTemp)/vViewTemp.w;
//            }";


//        private const string PsSpecular = @"
//            /* Copies incoming fragment color without change. */
//            #ifdef GL_ES
//                precision highp float;
//            #endif
//         
//            uniform sampler2D texture1;
//            uniform float specularLevel;
//            uniform float shininess;
//
//            uniform vec3 FUSEE_L0_POSITION;
//            uniform vec3 FUSEE_L1_POSITION;
//            uniform vec3 FUSEE_L2_POSITION;
//            uniform vec3 FUSEE_L3_POSITION;
//            uniform vec3 FUSEE_L4_POSITION;
//            uniform vec3 FUSEE_L5_POSITION;
//            uniform vec3 FUSEE_L6_POSITION;
//            uniform vec3 FUSEE_L7_POSITION;
//
//            uniform vec4 FUSEE_L0_DIFFUSE;
//            uniform vec4 FUSEE_L1_DIFFUSE;
//            uniform vec4 FUSEE_L2_DIFFUSE;
//            uniform vec4 FUSEE_L3_DIFFUSE;
//            uniform vec4 FUSEE_L4_DIFFUSE;
//            uniform vec4 FUSEE_L5_DIFFUSE;
//            uniform vec4 FUSEE_L6_DIFFUSE;
//            uniform vec4 FUSEE_L7_DIFFUSE;
//
//            uniform vec4 FUSEE_L0_AMBIENT;
//            uniform vec4 FUSEE_L1_AMBIENT;
//            uniform vec4 FUSEE_L2_AMBIENT;
//            uniform vec4 FUSEE_L3_AMBIENT;
//            uniform vec4 FUSEE_L4_AMBIENT;
//            uniform vec4 FUSEE_L5_AMBIENT;
//            uniform vec4 FUSEE_L6_AMBIENT;
//            uniform vec4 FUSEE_L7_AMBIENT;
//
//            uniform float FUSEE_L0_ACTIVE;
//            uniform float FUSEE_L1_ACTIVE;
//            uniform float FUSEE_L2_ACTIVE;
//            uniform float FUSEE_L3_ACTIVE;
//            uniform float FUSEE_L4_ACTIVE;
//            uniform float FUSEE_L5_ACTIVE;
//            uniform float FUSEE_L6_ACTIVE;
//            uniform float FUSEE_L7_ACTIVE;
//
//            uniform vec3 FUSEE_L0_DIRECTION;
//            uniform vec3 FUSEE_L1_DIRECTION;
//            uniform vec3 FUSEE_L2_DIRECTION;
//            uniform vec3 FUSEE_L3_DIRECTION;
//            uniform vec3 FUSEE_L4_DIRECTION;
//            uniform vec3 FUSEE_L5_DIRECTION;
//            uniform vec3 FUSEE_L6_DIRECTION;
//            uniform vec3 FUSEE_L7_DIRECTION;
//            
//            uniform vec4 FUSEE_L0_SPECULAR;
//            uniform vec4 FUSEE_L1_SPECULAR;
//            uniform vec4 FUSEE_L2_SPECULAR;
//            uniform vec4 FUSEE_L3_SPECULAR;
//            uniform vec4 FUSEE_L4_SPECULAR;
//            uniform vec4 FUSEE_L5_SPECULAR;
//            uniform vec4 FUSEE_L6_SPECULAR;
//            uniform vec4 FUSEE_L7_SPECULAR;
//
//            uniform float FUSEE_L0_SPOTANGLE;
//            uniform float FUSEE_L1_SPOTANGLE;
//            uniform float FUSEE_L2_SPOTANGLE;
//            uniform float FUSEE_L3_SPOTANGLE;
//            uniform float FUSEE_L4_SPOTANGLE;
//            uniform float FUSEE_L5_SPOTANGLE;
//            uniform float FUSEE_L6_SPOTANGLE;
//            uniform float FUSEE_L7_SPOTANGLE;
//    
//            varying vec3 vNormal;
//            varying vec2 vUV;
//            varying vec3 vViewPos;
//
//            void CalcDirectLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 direction, inout vec4 intensity) {
//                intensity += ambColor;
//                intensity += max(dot(-normalize(direction),normalize(vNormal)),0.0) * difColor;                
//                if(specularLevel != 0.0){
//                    vec3 lightVector = normalize(direction);
//                    vec3 r = normalize(reflect(lightVector, normalize(vNormal)));
//                    float s = pow(max(dot(r, vec3(0,0,1.0)), 0.0), specularLevel) * shininess;
//                    intensity += specColor * s;
//                }
//            }
//            void CalcPointLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 position, inout vec4 intensity) {
//                intensity += ambColor;
//                vec3 pos = position - vViewPos;
//                intensity += max(dot(normalize(pos),normalize(vNormal)),0.0) * difColor;  
//                if(specularLevel != 0.0){
//                    vec3 lightVector = normalize(-pos); 
//                    vec3 r = normalize(reflect(lightVector, normalize(vNormal)));
//                    float s = pow(max(dot(r, vec3(0,0,1.0)), 0.0), specularLevel) * shininess;
//                    intensity += specColor * s;
//                }
//            }
//            void CalcSpotLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 position, vec3 direction, float angle, inout vec4 intensity){
//                intensity += ambColor;
//                vec3 pos = position - vViewPos;
//                float alpha = acos(dot(normalize(pos), normalize(-direction)));
//                if(alpha < angle){
//                    intensity += max(dot(normalize(pos),normalize(vNormal)),0.0) * difColor; 
//                    if(specularLevel != 0.0){
//                        vec3 lightVector = normalize(-pos);  
//                        vec3 r = normalize(reflect(lightVector, normalize(vNormal)));
//                        float s = pow(max(dot(r, vec3(0,0,1.0)), 0.0), specularLevel) * shininess;
//                        intensity += specColor * s; 
//                    }
//                }
//            }
//
//            void main()
//            {              
//                vec4 endIntensity = vec4(0, 0, 0, 0);
//                if(FUSEE_L0_ACTIVE != 0.0){
//                    if(FUSEE_L0_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_DIRECTION, endIntensity);
//                    if(FUSEE_L0_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_POSITION, endIntensity);
//                    if(FUSEE_L0_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_POSITION, FUSEE_L0_DIRECTION, FUSEE_L0_SPOTANGLE, endIntensity);
//                }    
//                if(FUSEE_L1_ACTIVE != 0.0){
//                    if(FUSEE_L1_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_SPECULAR, FUSEE_L1_DIRECTION, endIntensity);
//                    if(FUSEE_L1_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_SPECULAR, FUSEE_L1_POSITION, endIntensity);
//                    if(FUSEE_L1_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_SPECULAR, FUSEE_L1_POSITION, FUSEE_L1_DIRECTION, FUSEE_L1_SPOTANGLE, endIntensity);
//                } 
//                if(FUSEE_L2_ACTIVE != 0.0){
//                    if(FUSEE_L2_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_SPECULAR, FUSEE_L2_DIRECTION, endIntensity);
//                    if(FUSEE_L2_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_SPECULAR, FUSEE_L2_POSITION, endIntensity);
//                    if(FUSEE_L2_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_SPECULAR, FUSEE_L2_POSITION, FUSEE_L2_DIRECTION, FUSEE_L2_SPOTANGLE, endIntensity);
//                } 
//                if(FUSEE_L3_ACTIVE != 0.0){
//                    if(FUSEE_L3_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_SPECULAR, FUSEE_L3_DIRECTION, endIntensity);
//                    if(FUSEE_L3_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_SPECULAR, FUSEE_L3_POSITION, endIntensity);
//                    if(FUSEE_L3_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_SPECULAR, FUSEE_L3_POSITION, FUSEE_L3_DIRECTION, FUSEE_L3_SPOTANGLE, endIntensity);
//                }    
//                if(FUSEE_L4_ACTIVE != 0.0){
//                    if(FUSEE_L4_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_SPECULAR, FUSEE_L4_DIRECTION, endIntensity);
//                    if(FUSEE_L4_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_SPECULAR, FUSEE_L4_POSITION, endIntensity);
//                    if(FUSEE_L4_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_SPECULAR, FUSEE_L4_POSITION, FUSEE_L4_DIRECTION, FUSEE_L4_SPOTANGLE, endIntensity);
//                }                     
//                if(FUSEE_L5_ACTIVE != 0.0){
//                    if(FUSEE_L5_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_SPECULAR, FUSEE_L5_DIRECTION, endIntensity);
//                    if(FUSEE_L5_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_SPECULAR, FUSEE_L5_POSITION, endIntensity);
//                    if(FUSEE_L5_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_SPECULAR, FUSEE_L5_POSITION, FUSEE_L5_DIRECTION, FUSEE_L5_SPOTANGLE, endIntensity);
//                }    
//                if(FUSEE_L6_ACTIVE != 0.0){
//                    if(FUSEE_L6_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_SPECULAR, FUSEE_L6_DIRECTION, endIntensity);
//                    if(FUSEE_L6_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_SPECULAR, FUSEE_L6_POSITION, endIntensity);
//                    if(FUSEE_L6_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_SPECULAR, FUSEE_L6_POSITION, FUSEE_L6_DIRECTION, FUSEE_L6_SPOTANGLE, endIntensity);
//                }  
//                if(FUSEE_L7_ACTIVE != 0.0){
//                    if(FUSEE_L7_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_SPECULAR, FUSEE_L7_DIRECTION, endIntensity);
//                    if(FUSEE_L7_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_SPECULAR, FUSEE_L7_POSITION, endIntensity);
//                    if(FUSEE_L7_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_SPECULAR, FUSEE_L7_POSITION, FUSEE_L7_DIRECTION, FUSEE_L7_SPOTANGLE, endIntensity);
//                }      
//
//                gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
//            }";

//        private const string VsBump = @"
//
//            attribute vec3 fuVertex;
//            attribute vec3 fuNormal;
//            attribute vec2 fuUV;
//                  
//            varying vec3 vNormal;
//            varying vec2 vUV;
//            varying vec3 vViewPos;
//    
//            uniform mat4 FUSEE_MV; 
//            uniform mat4 FUSEE_MVP;
//
//            uniform float FUSEE_L0_ACTIVE;
//            uniform float FUSEE_L1_ACTIVE;
//            uniform float FUSEE_L2_ACTIVE;
//            uniform float FUSEE_L3_ACTIVE;
//            uniform float FUSEE_L4_ACTIVE;
//            uniform float FUSEE_L5_ACTIVE;
//            uniform float FUSEE_L6_ACTIVE;
//            uniform float FUSEE_L7_ACTIVE;
//
//            void main()
//            {
//                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
//                vUV = fuUV;
//                vNormal = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);
//                vec4 vViewTemp = FUSEE_MV * vec4(fuVertex, 1);
//                vViewPos = vec3(vViewTemp)/vViewTemp.w;
//            }";


//        private const string PsBump = @"
//            /* Copies incoming fragment color without change. */
//            #ifdef GL_ES
//                precision highp float;
//            #endif
//
//            uniform mat4 FUSEE_V;
//            uniform mat4 FUSEE_MV;
//         
//            uniform sampler2D texture1;
//            uniform sampler2D normalTex;
//            uniform float shininess;
//            uniform float specularLevel;
//
//            uniform vec3 FUSEE_L0_POSITION;
//            uniform vec3 FUSEE_L1_POSITION;
//            uniform vec3 FUSEE_L2_POSITION;
//            uniform vec3 FUSEE_L3_POSITION;
//            uniform vec3 FUSEE_L4_POSITION;
//            uniform vec3 FUSEE_L5_POSITION;
//            uniform vec3 FUSEE_L6_POSITION;
//            uniform vec3 FUSEE_L7_POSITION;
//
//            uniform vec4 FUSEE_L0_DIFFUSE;
//            uniform vec4 FUSEE_L1_DIFFUSE;
//            uniform vec4 FUSEE_L2_DIFFUSE;
//            uniform vec4 FUSEE_L3_DIFFUSE;
//            uniform vec4 FUSEE_L4_DIFFUSE;
//            uniform vec4 FUSEE_L5_DIFFUSE;
//            uniform vec4 FUSEE_L6_DIFFUSE;
//            uniform vec4 FUSEE_L7_DIFFUSE;
//
//            uniform vec4 FUSEE_L0_AMBIENT;
//            uniform vec4 FUSEE_L1_AMBIENT;
//            uniform vec4 FUSEE_L2_AMBIENT;
//            uniform vec4 FUSEE_L3_AMBIENT;
//            uniform vec4 FUSEE_L4_AMBIENT;
//            uniform vec4 FUSEE_L5_AMBIENT;
//            uniform vec4 FUSEE_L6_AMBIENT;
//            uniform vec4 FUSEE_L7_AMBIENT;
//
//            uniform float FUSEE_L0_ACTIVE;
//            uniform float FUSEE_L1_ACTIVE;
//            uniform float FUSEE_L2_ACTIVE;
//            uniform float FUSEE_L3_ACTIVE;
//            uniform float FUSEE_L4_ACTIVE;
//            uniform float FUSEE_L5_ACTIVE;
//            uniform float FUSEE_L6_ACTIVE;
//            uniform float FUSEE_L7_ACTIVE;
//
//            uniform vec3 FUSEE_L0_DIRECTION;
//            uniform vec3 FUSEE_L1_DIRECTION;
//            uniform vec3 FUSEE_L2_DIRECTION;
//            uniform vec3 FUSEE_L3_DIRECTION;
//            uniform vec3 FUSEE_L4_DIRECTION;
//            uniform vec3 FUSEE_L5_DIRECTION;
//            uniform vec3 FUSEE_L6_DIRECTION;
//            uniform vec3 FUSEE_L7_DIRECTION;
//            
//            uniform vec4 FUSEE_L0_SPECULAR;
//            uniform vec4 FUSEE_L1_SPECULAR;
//            uniform vec4 FUSEE_L2_SPECULAR;
//            uniform vec4 FUSEE_L3_SPECULAR;
//            uniform vec4 FUSEE_L4_SPECULAR;
//            uniform vec4 FUSEE_L5_SPECULAR;
//            uniform vec4 FUSEE_L6_SPECULAR;
//            uniform vec4 FUSEE_L7_SPECULAR;
//
//            uniform float FUSEE_L0_SPOTANGLE;
//            uniform float FUSEE_L1_SPOTANGLE;
//            uniform float FUSEE_L2_SPOTANGLE;
//            uniform float FUSEE_L3_SPOTANGLE;
//            uniform float FUSEE_L4_SPOTANGLE;
//            uniform float FUSEE_L5_SPOTANGLE;
//            uniform float FUSEE_L6_SPOTANGLE;
//            uniform float FUSEE_L7_SPOTANGLE;
//    
//            varying vec3 vNormal;
//            varying vec2 vUV;
//            varying vec3 vViewPos;
//
//            void CalcDirectLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 direction, inout vec4 intensity) {
//                float maxVariance = 100.0; //Parameter for Bump Intensity
//                float minVariance = maxVariance / 2.0;
//                vec3 bumpNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);
//                intensity += ambColor;
//                intensity += max(dot(-normalize(direction),normalize(bumpNormal)),0.0) * difColor;
//                if(specularLevel != 0.0){
//                    vec3 lightVector = normalize(direction);
//                    vec3 r = normalize(reflect(lightVector, normalize(bumpNormal)));
//                    float s = pow(max(dot(r, vec3(0,0,1.0)), 0.0), specularLevel) * shininess;
//                    intensity += specColor * s;
//                }
//            }
//
//            void CalcPointLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 position, inout vec4 intensity) {
//                float maxVariance = 100.0; //Parameter for Bump Intensity
//                float minVariance = maxVariance / 2.0;
//                vec3 bumpNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);
//                intensity += ambColor;
//                vec3 pos = position - vViewPos;
//                intensity += max(dot(normalize(pos),normalize(bumpNormal)),0.0) * difColor; 
//                if(specularLevel != 0.0){
//                    vec3 lightVector = normalize(-pos);  
//                    vec3 r = normalize(reflect(lightVector, normalize(bumpNormal)));
//                    float s = pow(max(dot(r, vec3(0,0,1.0)), 0.0), specularLevel) * shininess;
//                    intensity += specColor * s;
//                }
//            }
//            void CalcSpotLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 position, vec3 direction, float angle, inout vec4 intensity){
//                float maxVariance = 100.0; //Parameter for Bump Intensity
//                float minVariance = maxVariance / 2.0;
//                vec3 bumpNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);
//                intensity += ambColor;
//                vec3 pos = position - vViewPos;
//                float alpha = acos(dot(normalize(pos), normalize(-direction)));
//                if(alpha < angle){
//                    intensity += max(dot(normalize(pos),normalize(bumpNormal)),0.0) * difColor; 
//                    if(specularLevel != 0.0){
//                        vec3 lightVector = normalize(-pos);  
//                        vec3 r = normalize(reflect(lightVector, normalize(bumpNormal)));
//                        float s = pow(max(dot(r, vec3(0,0,1.0)), 0.0), specularLevel) * shininess;
//                        intensity += specColor * s;
//                    }
//                }
//            }
//            
//
//            void main()
//            {              
//                vec4 endIntensity = vec4(0, 0, 0, 0);
//                if(FUSEE_L0_ACTIVE != 0.0){
//                    if(FUSEE_L0_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_DIRECTION, endIntensity);
//                    if(FUSEE_L0_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_POSITION, endIntensity);
//                    if(FUSEE_L0_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_POSITION, FUSEE_L0_DIRECTION, FUSEE_L0_SPOTANGLE, endIntensity);
//                }    
//                if(FUSEE_L1_ACTIVE != 0.0){
//                    if(FUSEE_L1_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_SPECULAR, FUSEE_L1_DIRECTION, endIntensity);
//                    if(FUSEE_L1_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_SPECULAR, FUSEE_L1_POSITION, endIntensity);
//                    if(FUSEE_L1_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_SPECULAR, FUSEE_L1_POSITION, FUSEE_L1_DIRECTION, FUSEE_L1_SPOTANGLE, endIntensity);
//                } 
//                if(FUSEE_L2_ACTIVE != 0.0){
//                    if(FUSEE_L2_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_SPECULAR, FUSEE_L2_DIRECTION, endIntensity);
//                    if(FUSEE_L2_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_SPECULAR, FUSEE_L2_POSITION, endIntensity);
//                    if(FUSEE_L2_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_SPECULAR, FUSEE_L2_POSITION, FUSEE_L2_DIRECTION, FUSEE_L2_SPOTANGLE, endIntensity);
//                } 
//                if(FUSEE_L3_ACTIVE != 0.0){
//                    if(FUSEE_L3_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_SPECULAR, FUSEE_L3_DIRECTION, endIntensity);
//                    if(FUSEE_L3_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_SPECULAR, FUSEE_L3_POSITION, endIntensity);
//                    if(FUSEE_L3_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_SPECULAR, FUSEE_L3_POSITION, FUSEE_L3_DIRECTION, FUSEE_L3_SPOTANGLE, endIntensity);
//                }    
//                if(FUSEE_L4_ACTIVE != 0.0){
//                    if(FUSEE_L4_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_SPECULAR, FUSEE_L4_DIRECTION, endIntensity);
//                    if(FUSEE_L4_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_SPECULAR, FUSEE_L4_POSITION, endIntensity);
//                    if(FUSEE_L4_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_SPECULAR, FUSEE_L4_POSITION, FUSEE_L4_DIRECTION, FUSEE_L4_SPOTANGLE, endIntensity);
//                }                     
//                if(FUSEE_L5_ACTIVE != 0.0){
//                    if(FUSEE_L5_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_SPECULAR, FUSEE_L5_DIRECTION, endIntensity);
//                    if(FUSEE_L5_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_SPECULAR, FUSEE_L5_POSITION, endIntensity);
//                    if(FUSEE_L5_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_SPECULAR, FUSEE_L5_POSITION, FUSEE_L5_DIRECTION, FUSEE_L5_SPOTANGLE, endIntensity);
//                }    
//                if(FUSEE_L6_ACTIVE != 0.0){
//                    if(FUSEE_L6_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_SPECULAR, FUSEE_L6_DIRECTION, endIntensity);
//                    if(FUSEE_L6_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_SPECULAR, FUSEE_L6_POSITION, endIntensity);
//                    if(FUSEE_L6_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_SPECULAR, FUSEE_L6_POSITION, FUSEE_L6_DIRECTION, FUSEE_L6_SPOTANGLE, endIntensity);
//                }  
//                if(FUSEE_L7_ACTIVE != 0.0){
//                    if(FUSEE_L7_ACTIVE == 1.0)
//                        CalcDirectLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_SPECULAR, FUSEE_L7_DIRECTION, endIntensity);
//                    if(FUSEE_L7_ACTIVE == 2.0)
//                        CalcPointLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_SPECULAR, FUSEE_L7_POSITION, endIntensity);
//                    if(FUSEE_L7_ACTIVE == 3.0)
//                        CalcSpotLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_SPECULAR, FUSEE_L7_POSITION, FUSEE_L7_DIRECTION, FUSEE_L7_SPOTANGLE, endIntensity);
//                }      
//
//                gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
//            }";


//        private const string VsSimpleColor = @"
//            attribute vec3 fuVertex;
//            attribute vec3 fuNormal;       
//        
//            varying vec3 vNormal;
//        
//            uniform mat4 FUSEE_MVP;
//            uniform mat4 FUSEE_ITMV;
//
//            void main()
//            {
//                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
//                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
//            }";


//        private const string PsSimpleColor = @"
//            #ifdef GL_ES
//                precision highp float;
//            #endif    
//  
//            uniform vec4 color;
//            varying vec3 vNormal;
//
//            void main()
//            {             
//                gl_FragColor = max(dot(vec3(0,0,1),normalize(vNormal)), 0.1) * color;
//            }";

//    }
//}