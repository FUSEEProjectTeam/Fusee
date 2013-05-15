using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// Contains all pixel and vertex shaders and a method to create a ShaderProgram in Rendercontext.
    /// </summary>
    public static class MoreShaders
    {

        /// <summary>
        /// Creates the shader in RenderContext and returns a ShaderProgram.
        /// </summary>
        /// <param name="name">ShaderName.</param>
        /// <param name="rc">RenderContext.</param>
        /// <returns></returns>
        public static ShaderProgram GetShader(string name, RenderContext rc)
        {
            if (name == "simple")
            {
                ShaderProgram spSimple = rc.CreateShader(Vs, Ps);
                return spSimple;
            }
            if (name == "diffuse")
            {
                ShaderProgram spDiffuse = rc.CreateShader(VsDiffuse, PsDiffuse);
                return spDiffuse;
            }

            if (name == "specular")
            {
                ShaderProgram spSpecular = rc.CreateShader(VsSpecular2, PsSpecular2);
                return spSpecular;
            }

            if (name == "bump")
            {
                ShaderProgram spBump = rc.CreateShader(VsBump, PsBump);
                return spBump;
            }

            if (name == "oneColor")
            {
                ShaderProgram spOneColor = rc.CreateShader(VsOneColor, PsOneColor);
                return spOneColor;
            }

            ShaderProgram spOriginal = rc.CreateShader(Vs, Ps);
            return spOriginal;
        }


        private const string VsOneColor = @"
#ifdef GL_ES
    precision mediump float;
#endif
attribute vec3 fuVertex;

varying vec4 vColor;

uniform vec4 Col;
uniform mat4 FUSEE_MVP;


void main(){
gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}
";
        private const string PsOneColor = @"
#ifdef GL_ES
    precision mediump float;
#endif

uniform vec4 Col;
varying vec4 vColor;

void main(){
    gl_FragColor = Col;
}
";

private const string VsDiffuse = @"
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix
uniform mat4 FUSEE_M;
uniform mat4 FUSEE_V;
uniform mat4 FUSEE_P;

uniform mat4 FUSEE_MV;

varying vec2 vUV;
varying vec3 vNormal;
varying vec3 vNormalView;
varying vec3 vGlobalPos;
varying vec3 vViewPos;

vec3 vPos;
 
void main()
{
    vec4 vGlobalTemp = FUSEE_M * vec4(fuVertex, 1);
    vGlobalPos = vec3(vGlobalTemp)/vGlobalTemp.w ;

    vec4 vViewTemp = FUSEE_MV * vec4(fuVertex, 1);
    vViewPos = vec3(vViewTemp)/vViewTemp.w;
       
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_M[0].xyz, FUSEE_M[1].xyz, FUSEE_M[2].xyz) * fuNormal);
    vNormalView = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);
    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";


private const string PsDiffuse = @"
#ifdef GL_ES
    precision highp float;
#endif

uniform sampler2D texture1;

uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix
uniform mat4 FUSEE_M;
uniform mat4 FUSEE_V;
uniform mat4 FUSEE_P;

uniform mat4 FUSEE_MV;
uniform mat4 FUSEE_MP;
uniform mat4 FUSEE_ITP;

uniform vec3 FUSEE_L0_POSITION;
uniform vec3 FUSEE_L1_POSITION;
uniform vec3 FUSEE_L2_POSITION;
uniform vec3 FUSEE_L3_POSITION;
uniform vec3 FUSEE_L4_POSITION;
uniform vec3 FUSEE_L5_POSITION;
uniform vec3 FUSEE_L6_POSITION;
uniform vec3 FUSEE_L7_POSITION;

uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L1_DIFFUSE;
uniform vec4 FUSEE_L2_DIFFUSE;
uniform vec4 FUSEE_L3_DIFFUSE;
uniform vec4 FUSEE_L4_DIFFUSE;
uniform vec4 FUSEE_L5_DIFFUSE;
uniform vec4 FUSEE_L6_DIFFUSE;
uniform vec4 FUSEE_L7_DIFFUSE;

uniform vec4 FUSEE_L0_AMBIENT;
uniform vec4 FUSEE_L1_AMBIENT;
uniform vec4 FUSEE_L2_AMBIENT;
uniform vec4 FUSEE_L3_AMBIENT;
uniform vec4 FUSEE_L4_AMBIENT;
uniform vec4 FUSEE_L5_AMBIENT;
uniform vec4 FUSEE_L6_AMBIENT;
uniform vec4 FUSEE_L7_AMBIENT;

uniform float FUSEE_L0_ACTIVE;
uniform float FUSEE_L1_ACTIVE;
uniform float FUSEE_L2_ACTIVE;
uniform float FUSEE_L3_ACTIVE;
uniform float FUSEE_L4_ACTIVE;
uniform float FUSEE_L5_ACTIVE;
uniform float FUSEE_L6_ACTIVE;
uniform float FUSEE_L7_ACTIVE;

uniform vec3 FUSEE_L0_DIRECTION;
uniform vec3 FUSEE_L1_DIRECTION;
uniform vec3 FUSEE_L2_DIRECTION;
uniform vec3 FUSEE_L3_DIRECTION;
uniform vec3 FUSEE_L4_DIRECTION;
uniform vec3 FUSEE_L5_DIRECTION;
uniform vec3 FUSEE_L6_DIRECTION;
uniform vec3 FUSEE_L7_DIRECTION;

varying vec3 vNormal;
varying vec2 vUV;
varying vec3 vNormalView;
varying vec3 vGlobalPos;
varying vec3 vViewPos;

void CalcDirectLight(vec4 difColor, vec4 ambColor, vec3 direction, inout vec4 intensity) {
    intensity += ambColor;
    intensity += max(dot(-normalize(direction),normalize(vNormal)),0.0) * difColor;
}

void CalcPointLight(vec4 difColor, vec4 ambColor, vec3 position, inout vec4 intensity) {
    vec4 tempPos = FUSEE_V * vec4(position,1);
    vec3 tempPos2 = vec3(tempPos)/tempPos.w;
    intensity += ambColor;
    vec3 pos = position - vGlobalPos; /// ADD GLOBAL VERTEX-POSITION HERE
    intensity += max(dot(normalize(pos),normalize(vNormal)),0.0) * difColor;   
}

void CalcSpotLight() {
    // add diffuse light calculation for spotlight here
}
 
void main()
{
    vec4 endIntensity = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE != 0.0){
        if(FUSEE_L0_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_DIRECTION, endIntensity);
        if(FUSEE_L0_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_POSITION, endIntensity);
        if(FUSEE_L0_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L1_ACTIVE != 0.0){
        if(FUSEE_L1_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_DIRECTION, endIntensity);
        if(FUSEE_L1_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_POSITION, endIntensity);
        if(FUSEE_L1_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L2_ACTIVE != 0.0){
        if(FUSEE_L2_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_DIRECTION, endIntensity);
        if(FUSEE_L2_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_POSITION, endIntensity);
        if(FUSEE_L2_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L3_ACTIVE != 0.0){
        if(FUSEE_L3_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_DIRECTION, endIntensity);
        if(FUSEE_L3_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_POSITION, endIntensity);
        if(FUSEE_L3_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L4_ACTIVE != 0.0){
        if(FUSEE_L4_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_DIRECTION, endIntensity);
        if(FUSEE_L4_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_POSITION, endIntensity);
        if(FUSEE_L4_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L5_ACTIVE != 0.0){
        if(FUSEE_L5_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_DIRECTION, endIntensity);
        if(FUSEE_L5_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_POSITION, endIntensity);
        if(FUSEE_L5_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L6_ACTIVE != 0.0){
        if(FUSEE_L6_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_DIRECTION, endIntensity);
        if(FUSEE_L6_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_POSITION, endIntensity);
        if(FUSEE_L6_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L7_ACTIVE != 0.0){
        if(FUSEE_L7_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_DIRECTION, endIntensity);
        if(FUSEE_L7_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_POSITION, endIntensity);
        if(FUSEE_L7_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    endIntensity = clamp(endIntensity, 0.0, 1.0);

    gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
}";


private const string VsSpecular = @"
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix
uniform mat4 FUSEE_M;
uniform mat4 FUSEE_MV;

varying vec2 vUV;
varying vec3 vNormal;
varying vec3 viewVertex;
varying vec3 eyeVector;
varying vec4 pos;
varying vec3 vNormalView;
varying vec3 vGlobalPos;
varying vec3 vViewPos;
varying vec3 halfVector;



//vec3 vPos;
 
void main()
{
    viewVertex = fuVertex;
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_M[0].xyz, FUSEE_M[1].xyz, FUSEE_M[2].xyz) * fuNormal);
    vNormalView = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);
    vec4 vGlobalTemp = FUSEE_M * vec4(fuVertex, 1);
    vGlobalPos = vec3(vGlobalTemp)/vGlobalTemp.w ;

    vec4 vViewTemp = FUSEE_MV * vec4(fuVertex, 1);
    vViewPos = vec3(vViewTemp)/vViewTemp.w;

    eyeVector = mat3(FUSEE_MVP[0].xyz, FUSEE_MVP[1].xyz, FUSEE_MVP[2].xyz) * fuVertex;
      

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);


    pos = FUSEE_MV * vec4(fuVertex, 1.0);



       
    vUV = fuUV;

    

}";


private const string PsSpecular = @"
/* Copies incoming fragment color without change. */
#ifdef GL_ES
    precision highp float;
#endif

uniform sampler2D texture1;

uniform vec4 FUSEE_L0_SPECULAR;
uniform vec4 FUSEE_L1_SPECULAR;
uniform vec4 FUSEE_L2_SPECULAR;
uniform vec4 FUSEE_L3_SPECULAR;
uniform vec4 FUSEE_L4_SPECULAR;
uniform vec4 FUSEE_L5_SPECULAR;
uniform vec4 FUSEE_L6_SPECULAR;
uniform vec4 FUSEE_L7_SPECULAR;

uniform float FUSEE_L0_ACTIVE;
uniform float FUSEE_L1_ACTIVE;
uniform float FUSEE_L2_ACTIVE;
uniform float FUSEE_L3_ACTIVE;
uniform float FUSEE_L4_ACTIVE;
uniform float FUSEE_L5_ACTIVE;
uniform float FUSEE_L6_ACTIVE;
uniform float FUSEE_L7_ACTIVE;

uniform vec3 FUSEE_L0_DIRECTION;
uniform vec3 FUSEE_L1_DIRECTION;
uniform vec3 FUSEE_L2_DIRECTION;
uniform vec3 FUSEE_L3_DIRECTION;
uniform vec3 FUSEE_L4_DIRECTION;
uniform vec3 FUSEE_L5_DIRECTION;
uniform vec3 FUSEE_L6_DIRECTION;
uniform vec3 FUSEE_L7_DIRECTION;

uniform vec4 FUSEE_L0_AMBIENT;
uniform vec4 FUSEE_L1_AMBIENT;
uniform vec4 FUSEE_L2_AMBIENT;
uniform vec4 FUSEE_L3_AMBIENT;
uniform vec4 FUSEE_L4_AMBIENT;
uniform vec4 FUSEE_L5_AMBIENT;
uniform vec4 FUSEE_L6_AMBIENT;
uniform vec4 FUSEE_L7_AMBIENT;

uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L1_DIFFUSE;
uniform vec4 FUSEE_L2_DIFFUSE;
uniform vec4 FUSEE_L3_DIFFUSE;
uniform vec4 FUSEE_L4_DIFFUSE;
uniform vec4 FUSEE_L5_DIFFUSE;
uniform vec4 FUSEE_L6_DIFFUSE;
uniform vec4 FUSEE_L7_DIFFUSE;

uniform vec3 FUSEE_L0_POSITION;
uniform vec3 FUSEE_L1_POSITION;
uniform vec3 FUSEE_L2_POSITION;
uniform vec3 FUSEE_L3_POSITION;
uniform vec3 FUSEE_L4_POSITION;
uniform vec3 FUSEE_L5_POSITION;
uniform vec3 FUSEE_L6_POSITION;
uniform vec3 FUSEE_L7_POSITION;

uniform mat4 FUSEE_V;

varying vec3 vNormal;
varying vec2 vUV;
varying vec3 eyeVector;
varying vec3 viewVertex;
varying vec4 pos;
varying vec3 vNormalView;
varying vec3 vGlobalPos;
varying vec3 vViewPos;
varying vec3 halfVector;

            void CalcDirectLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 direction, inout vec4 intensity) {
                intensity += ambColor;
                intensity += max(dot(-normalize(direction),normalize(vNormal)),0.0) * difColor;
            }

            void CalcPointLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 position, inout vec4 intensity) {
                vec4 tempPos = FUSEE_V * vec4(position,1);
                vec3 tempPos2 = vec3(tempPos)/tempPos.w;
                intensity += ambColor;
                vec3 pos = position - vGlobalPos; 
                intensity += max(dot(normalize(pos),normalize(vNormal)),0.0) * difColor;   
                
                float att;
                vec3 n,halfV,viewV,lightDir;
                float NdotL,NdotHV;
                n = normalize(vNormal);
                lightDir = position - vGlobalPos;
                NdotL = max(dot(n,normalize(lightDir)),0.0);

                if(NdotL > 0.0){
                    att = 1.0;        
                    halfV = normalize(halfVector);
                    NdotHV = max(dot(n,halfV),0.0);
                   vec4 colorSpec = specColor * pow(NdotHV,64.0);
                    intensity += att * colorSpec;
                }

            }

            void CalcSpotLight() {
               // intensity += ambColor;
             //   intensity += max(dot(-normalize(direction),normalize(vNormal)),0.0) * difColor;
            }

void main()
{
    //vec4 endSpecular = vec4(0,0,0,0);
    //float specularLevel = 64.0;

//    if(FUSEE_L0_ACTIVE != 0.0) {
//        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L0_DIRECTION));
//        float L0NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
//        float shine = pow(L0NdotHV, specularLevel) * 8.0;
//        endSpecular += FUSEE_L0_SPECULAR * shine;
//    }
    
    vec4 endIntensity = vec4(0,0,0,0);

    if(FUSEE_L0_ACTIVE != 0.0){
        if(FUSEE_L0_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_DIRECTION, endIntensity);
        if(FUSEE_L0_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_POSITION, endIntensity);
        if(FUSEE_L0_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L1_ACTIVE != 0.0){
        if(FUSEE_L1_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_SPECULAR, FUSEE_L1_DIRECTION, endIntensity);
        if(FUSEE_L1_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_SPECULAR, FUSEE_L1_POSITION, endIntensity);
        if(FUSEE_L1_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L2_ACTIVE != 0.0){
        if(FUSEE_L2_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_SPECULAR, FUSEE_L2_DIRECTION, endIntensity);
        if(FUSEE_L2_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_SPECULAR, FUSEE_L2_POSITION, endIntensity);
        if(FUSEE_L2_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L3_ACTIVE != 0.0){
        if(FUSEE_L3_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_SPECULAR, FUSEE_L3_DIRECTION, endIntensity);
        if(FUSEE_L3_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_SPECULAR, FUSEE_L3_POSITION, endIntensity);
        if(FUSEE_L3_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L4_ACTIVE != 0.0){
        if(FUSEE_L4_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_SPECULAR, FUSEE_L4_DIRECTION, endIntensity);
        if(FUSEE_L4_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_SPECULAR, FUSEE_L4_POSITION, endIntensity);
        if(FUSEE_L4_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L5_ACTIVE != 0.0){
        if(FUSEE_L5_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_SPECULAR, FUSEE_L5_DIRECTION, endIntensity);
        if(FUSEE_L5_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_SPECULAR, FUSEE_L5_POSITION, endIntensity);
        if(FUSEE_L5_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L6_ACTIVE != 0.0){
        if(FUSEE_L6_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_SPECULAR, FUSEE_L6_DIRECTION, endIntensity);
        if(FUSEE_L6_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_SPECULAR, FUSEE_L6_POSITION, endIntensity);
        if(FUSEE_L6_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    if(FUSEE_L7_ACTIVE != 0.0){
        if(FUSEE_L7_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_SPECULAR, FUSEE_L7_DIRECTION, endIntensity);
        if(FUSEE_L7_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_SPECULAR, FUSEE_L7_POSITION, endIntensity);
        if(FUSEE_L7_ACTIVE == 3.0)
            CalcSpotLight();
    }  

    endIntensity = clamp(endIntensity, 0.0, 1.0);
    gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
}";




private const string VsBump = @"
/* Copies incoming vertex color without change.
    * Applies the transformation matrix to vertex position.
    */
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix
uniform mat4 FUSEE_M;

varying vec2 vUV;
varying vec3 lightDir[8];
varying vec3 vNormal;
varying vec4 endAmbient;
varying vec3 eyeVector;
varying vec3 viewVertex;


vec3 vPos;
 
void main()
{
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_M[0].xyz, FUSEE_M[1].xyz, FUSEE_M[2].xyz) * fuNormal);
    viewVertex = fuVertex;
    eyeVector = mat3(FUSEE_MVP[0].xyz, FUSEE_MVP[1].xyz, FUSEE_MVP[2].xyz) * -fuVertex;

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";

private const string PsBump = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

uniform sampler2D texture1;
uniform sampler2D normalTex;
uniform float specularLevel;

uniform vec4 FUSEE_L0_SPECULAR;
uniform vec4 FUSEE_L1_SPECULAR;
uniform vec4 FUSEE_L2_SPECULAR;
uniform vec4 FUSEE_L3_SPECULAR;
uniform vec4 FUSEE_L4_SPECULAR;
uniform vec4 FUSEE_L5_SPECULAR;
uniform vec4 FUSEE_L6_SPECULAR;
uniform vec4 FUSEE_L7_SPECULAR;

uniform float FUSEE_L0_ACTIVE;
uniform float FUSEE_L1_ACTIVE;
uniform float FUSEE_L2_ACTIVE;
uniform float FUSEE_L3_ACTIVE;
uniform float FUSEE_L4_ACTIVE;
uniform float FUSEE_L5_ACTIVE;
uniform float FUSEE_L6_ACTIVE;
uniform float FUSEE_L7_ACTIVE;

uniform vec4 FUSEE_L0_AMBIENT;
uniform vec4 FUSEE_L1_AMBIENT;
uniform vec4 FUSEE_L2_AMBIENT;
uniform vec4 FUSEE_L3_AMBIENT;
uniform vec4 FUSEE_L4_AMBIENT;
uniform vec4 FUSEE_L5_AMBIENT;
uniform vec4 FUSEE_L6_AMBIENT;
uniform vec4 FUSEE_L7_AMBIENT;

uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L1_DIFFUSE;
uniform vec4 FUSEE_L2_DIFFUSE;
uniform vec4 FUSEE_L3_DIFFUSE;
uniform vec4 FUSEE_L4_DIFFUSE;
uniform vec4 FUSEE_L5_DIFFUSE;
uniform vec4 FUSEE_L6_DIFFUSE;
uniform vec4 FUSEE_L7_DIFFUSE;

uniform vec3 FUSEE_L0_POSITION;
uniform vec3 FUSEE_L1_POSITION;
uniform vec3 FUSEE_L2_POSITION;
uniform vec3 FUSEE_L3_POSITION;
uniform vec3 FUSEE_L4_POSITION;
uniform vec3 FUSEE_L5_POSITION;
uniform vec3 FUSEE_L6_POSITION;
uniform vec3 FUSEE_L7_POSITION;

uniform vec3 FUSEE_L0_DIRECTION;
uniform vec3 FUSEE_L1_DIRECTION;
uniform vec3 FUSEE_L2_DIRECTION;
uniform vec3 FUSEE_L3_DIRECTION;
uniform vec3 FUSEE_L4_DIRECTION;
uniform vec3 FUSEE_L5_DIRECTION;
uniform vec3 FUSEE_L6_DIRECTION;
uniform vec3 FUSEE_L7_DIRECTION;

varying vec3 vNormal;
varying vec2 vUV;
varying vec4 endAmbient;
varying vec3 eyeVector;
varying vec3 viewVertex;

void CalcDirectLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 direction, inout vec4 intensity) {
    intensity += ambColor;
    intensity += max(dot(-normalize(direction),normalize(vNormal)),0.0) * difColor;
}
void CalcPointLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 position, inout vec4 intensity) {
    intensity += ambColor;
    intensity += max(dot(-normalize(position),normalize(vNormal)),0.0) * difColor;
}

void CalcSpotLight() {

}

 
void main()
{       
    float maxVariance = 2.0;
    float minVariance = maxVariance/2.0;
    vec3 tempNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);
 


//    vec4 endSpecular = vec4(0,0,0,0);
//    if(FUSEE_L0_ACTIVE != 0.0 ) {
//        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L0_POSITION));
//        float L3NdotHV = max(min(dot(normalize(tempNormal), vHalfVector),1.0), 0.0);
//        float shine = pow(L3NdotHV, specularLevel) * 16.0;
//        endSpecular += FUSEE_L0_SPECULAR * shine;
//    }
////    vec3 lightVec = normalize(FUSEE_L0_DIRECTION - eyeVector);
////    vec3 r = -normalize(reflect(lightVec, vNormal));
////    float s = pow(max(dot(r, normalize(-viewVertex)), 0.0), specularLevel);
////    endSpecular = FUSEE_L0_SPECULAR * s;
//
//    if(FUSEE_L1_ACTIVE != 0.0) {
//        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L1_POSITION));
//        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
//        float shine = pow(L3NdotHV, specularLevel) * 16.0;
//        endSpecular += FUSEE_L1_SPECULAR * shine;
//    }
//    if(FUSEE_L2_ACTIVE != 0.0) {
//        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L2_POSITION));
//        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
//        float shine = pow(L3NdotHV, specularLevel) * 16.0;
//        endSpecular += FUSEE_L2_SPECULAR * shine;
//    }
//    if(FUSEE_L3_ACTIVE != 0.0) {
//        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L3_POSITION));
//        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
//        float shine = pow(L3NdotHV, specularLevel) * 16.0;
//        endSpecular += FUSEE_L3_SPECULAR * shine;
//    }
//    if(FUSEE_L4_ACTIVE != 0.0) {
//        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L4_POSITION));
//        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
//        float shine = pow(L3NdotHV, specularLevel) * 16.0;
//        endSpecular += FUSEE_L4_SPECULAR * shine;
//    }
//    if(FUSEE_L5_ACTIVE != 0.0) {
//        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L5_POSITION));
//        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
//        float shine = pow(L3NdotHV, specularLevel) * 16.0;
//        endSpecular += FUSEE_L5_SPECULAR * shine;
//    }
//    if(FUSEE_L6_ACTIVE != 0.0) {
//        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L6_POSITION));
//        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
//        float shine = pow(L3NdotHV, specularLevel) * 16.0;
//        endSpecular += FUSEE_L6_SPECULAR * shine;
//    }
//    if(FUSEE_L7_ACTIVE != 0.0) {
//        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L7_POSITION));
//        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
//        float shine = pow(L3NdotHV, specularLevel) * 16.0;
//        endSpecular += FUSEE_L7_SPECULAR * shine;
//    }
    
    vec4 endIntensity = vec4(0,0,0,0);

    if(FUSEE_L0_ACTIVE != 0.0){
        if(FUSEE_L0_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_DIRECTION, endIntensity);
        if(FUSEE_L0_ACTIVE == 2.0)
            CalcPointLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_POSITION, endIntensity);
        if(FUSEE_L0_ACTIVE == 3.0)
            CalcSpotLight();
    }  

//    if(FUSEE_L1_ACTIVE != 0.0) {
//        float intensity = max(dot(-normalize(FUSEE_L1_DIRECTION),normalize(tempNormal)),0.0);
//        endIntensity += intensity * FUSEE_L1_DIFFUSE;
//    }

    //endIntensity += endSpecular;
    endIntensity = clamp(endIntensity, 0.0, 1.0);
    gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
}";

private const string Vs = @"
            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
            
        
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
                // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
                // vNormal = norm4.xyz;
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }";


        private const string Ps = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

         
            uniform sampler2D texture1;
            uniform vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {             
                gl_FragColor = texture2D(texture1, vUV)  /* *dot(vNormal, vec3(0, 0, 1))*/;
            }";


        private const string VsSpecular2 = @"

            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
                  
            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec3 vViewPos;
            varying vec3 vNormalView;
    
            uniform mat4 FUSEE_M;
            uniform mat4 FUSEE_MV; 
            uniform mat4 FUSEE_MVP;

            uniform float FUSEE_L0_ACTIVE;
            uniform float FUSEE_L1_ACTIVE;
            uniform float FUSEE_L2_ACTIVE;
            uniform float FUSEE_L3_ACTIVE;
            uniform float FUSEE_L4_ACTIVE;
            uniform float FUSEE_L5_ACTIVE;
            uniform float FUSEE_L6_ACTIVE;
            uniform float FUSEE_L7_ACTIVE;


            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vUV = fuUV;
                vNormal = normalize(mat3(FUSEE_M[0].xyz, FUSEE_M[1].xyz, FUSEE_M[2].xyz) * fuNormal);
                vec4 vViewTemp = FUSEE_MV * vec4(fuVertex, 1);
                vViewPos = vec3(vViewTemp)/vViewTemp.w;
                vNormalView = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);
            }";


        private const string PsSpecular2 = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

            uniform mat4 FUSEE_V;
         
            uniform sampler2D texture1;

            uniform vec3 FUSEE_L0_POSITION;
            uniform vec3 FUSEE_L1_POSITION;
            uniform vec3 FUSEE_L2_POSITION;
            uniform vec3 FUSEE_L3_POSITION;
            uniform vec3 FUSEE_L4_POSITION;
            uniform vec3 FUSEE_L5_POSITION;
            uniform vec3 FUSEE_L6_POSITION;
            uniform vec3 FUSEE_L7_POSITION;

            uniform vec4 FUSEE_L0_DIFFUSE;
            uniform vec4 FUSEE_L1_DIFFUSE;
            uniform vec4 FUSEE_L2_DIFFUSE;
            uniform vec4 FUSEE_L3_DIFFUSE;
            uniform vec4 FUSEE_L4_DIFFUSE;
            uniform vec4 FUSEE_L5_DIFFUSE;
            uniform vec4 FUSEE_L6_DIFFUSE;
            uniform vec4 FUSEE_L7_DIFFUSE;

            uniform vec4 FUSEE_L0_AMBIENT;
            uniform vec4 FUSEE_L1_AMBIENT;
            uniform vec4 FUSEE_L2_AMBIENT;
            uniform vec4 FUSEE_L3_AMBIENT;
            uniform vec4 FUSEE_L4_AMBIENT;
            uniform vec4 FUSEE_L5_AMBIENT;
            uniform vec4 FUSEE_L6_AMBIENT;
            uniform vec4 FUSEE_L7_AMBIENT;

            uniform float FUSEE_L0_ACTIVE;
            uniform float FUSEE_L1_ACTIVE;
            uniform float FUSEE_L2_ACTIVE;
            uniform float FUSEE_L3_ACTIVE;
            uniform float FUSEE_L4_ACTIVE;
            uniform float FUSEE_L5_ACTIVE;
            uniform float FUSEE_L6_ACTIVE;
            uniform float FUSEE_L7_ACTIVE;

            uniform vec3 FUSEE_L0_DIRECTION;
            uniform vec3 FUSEE_L1_DIRECTION;
            uniform vec3 FUSEE_L2_DIRECTION;
            uniform vec3 FUSEE_L3_DIRECTION;
            uniform vec3 FUSEE_L4_DIRECTION;
            uniform vec3 FUSEE_L5_DIRECTION;
            uniform vec3 FUSEE_L6_DIRECTION;
            uniform vec3 FUSEE_L7_DIRECTION;
            
            uniform vec4 FUSEE_L0_SPECULAR;
            uniform vec4 FUSEE_L1_SPECULAR;
            uniform vec4 FUSEE_L2_SPECULAR;
            uniform vec4 FUSEE_L3_SPECULAR;
            uniform vec4 FUSEE_L4_SPECULAR;
            uniform vec4 FUSEE_L5_SPECULAR;
            uniform vec4 FUSEE_L6_SPECULAR;
            uniform vec4 FUSEE_L7_SPECULAR;

            uniform float shininess;
    
            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec3 vViewPos;
            varying vec3 vNormalView;

            void CalcDirectLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 direction, inout vec4 intensity) {
                vec3 tempDir2 = normalize(mat3(FUSEE_V[0].xyz, FUSEE_V[1].xyz, FUSEE_V[2].xyz) * direction); 
                intensity += ambColor;
                intensity += max(dot(-normalize(direction),normalize(vNormal)),0.0) * difColor;
                vec3 lightVector = normalize(tempDir2);
                vec3 r = normalize(reflect(lightVector, normalize(vNormalView)));
                float s = pow(max(dot(r, vec3(0,0,1.0)), 0.0), 16.0);
                intensity += specColor * s;
            }
            void CalcPointLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 position, inout vec4 intensity) {
                vec4 tempPos = FUSEE_V * vec4(position,1);
                vec3 tempPos2 = vec3(tempPos)/tempPos.w;
                intensity += ambColor;
                vec3 pos = tempPos2 - vViewPos; /// ADD GLOBAL VERTEX-POSITION HERE
                intensity += max(dot(normalize(pos),normalize(vNormalView)),0.0) * difColor; 
                vec3 lightVector = normalize(-pos);  
                // Calculate specular term
                vec3 r = normalize(reflect(lightVector, normalize(vNormalView)));
                float s = pow(max(dot(r, vec3(0,0,1.0)), 0.0), 16.0);
                intensity += specColor * s;
            }
            void CalcSpotLight(){

            }

            void main()
            {              
         
                vec4 endIntensity = vec4(0, 0, 0, 0);
                if(FUSEE_L0_ACTIVE != 0.0){
                    if(FUSEE_L0_ACTIVE == 1.0)
                        CalcDirectLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_DIRECTION, endIntensity);
                    if(FUSEE_L0_ACTIVE == 2.0)
                        CalcPointLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_POSITION, endIntensity);
                    if(FUSEE_L0_ACTIVE == 3.0)
                        CalcSpotLight();
                }    
                if(FUSEE_L1_ACTIVE != 0.0){
                    if(FUSEE_L1_ACTIVE == 1.0)
                        CalcDirectLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_SPECULAR, FUSEE_L1_DIRECTION, endIntensity);
                    if(FUSEE_L1_ACTIVE == 2.0)
                        CalcPointLight(FUSEE_L1_DIFFUSE, FUSEE_L1_AMBIENT, FUSEE_L1_SPECULAR, FUSEE_L1_POSITION, endIntensity);
                    if(FUSEE_L1_ACTIVE == 3.0)
                        CalcSpotLight();
                } 
                if(FUSEE_L2_ACTIVE != 0.0){
                    if(FUSEE_L2_ACTIVE == 1.0)
                        CalcDirectLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_SPECULAR, FUSEE_L2_DIRECTION, endIntensity);
                    if(FUSEE_L2_ACTIVE == 2.0)
                        CalcPointLight(FUSEE_L2_DIFFUSE, FUSEE_L2_AMBIENT, FUSEE_L2_SPECULAR, FUSEE_L2_POSITION, endIntensity);
                    if(FUSEE_L2_ACTIVE == 3.0)
                        CalcSpotLight();
                } 
                if(FUSEE_L3_ACTIVE != 0.0){
                    if(FUSEE_L3_ACTIVE == 1.0)
                        CalcDirectLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_SPECULAR, FUSEE_L3_DIRECTION, endIntensity);
                    if(FUSEE_L3_ACTIVE == 2.0)
                        CalcPointLight(FUSEE_L3_DIFFUSE, FUSEE_L3_AMBIENT, FUSEE_L3_SPECULAR, FUSEE_L3_POSITION, endIntensity);
                    if(FUSEE_L3_ACTIVE == 3.0)
                        CalcSpotLight();
                }    
                if(FUSEE_L4_ACTIVE != 0.0){
                    if(FUSEE_L4_ACTIVE == 1.0)
                        CalcDirectLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_SPECULAR, FUSEE_L4_DIRECTION, endIntensity);
                    if(FUSEE_L4_ACTIVE == 2.0)
                        CalcPointLight(FUSEE_L4_DIFFUSE, FUSEE_L4_AMBIENT, FUSEE_L4_SPECULAR, FUSEE_L4_POSITION, endIntensity);
                    if(FUSEE_L4_ACTIVE == 3.0)
                        CalcSpotLight();
                }                     
                if(FUSEE_L5_ACTIVE != 0.0){
                    if(FUSEE_L5_ACTIVE == 1.0)
                        CalcDirectLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_SPECULAR, FUSEE_L5_DIRECTION, endIntensity);
                    if(FUSEE_L5_ACTIVE == 2.0)
                        CalcPointLight(FUSEE_L5_DIFFUSE, FUSEE_L5_AMBIENT, FUSEE_L5_SPECULAR, FUSEE_L5_POSITION, endIntensity);
                    if(FUSEE_L5_ACTIVE == 3.0)
                        CalcSpotLight();
                }    
                if(FUSEE_L6_ACTIVE != 0.0){
                    if(FUSEE_L6_ACTIVE == 1.0)
                        CalcDirectLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_SPECULAR, FUSEE_L6_DIRECTION, endIntensity);
                    if(FUSEE_L6_ACTIVE == 2.0)
                        CalcPointLight(FUSEE_L6_DIFFUSE, FUSEE_L6_AMBIENT, FUSEE_L6_SPECULAR, FUSEE_L6_POSITION, endIntensity);
                    if(FUSEE_L6_ACTIVE == 3.0)
                        CalcSpotLight();
                }  
                if(FUSEE_L7_ACTIVE != 0.0){
                    if(FUSEE_L7_ACTIVE == 1.0)
                        CalcDirectLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_SPECULAR, FUSEE_L7_DIRECTION, endIntensity);
                    if(FUSEE_L7_ACTIVE == 2.0)
                        CalcPointLight(FUSEE_L7_DIFFUSE, FUSEE_L7_AMBIENT, FUSEE_L7_SPECULAR, FUSEE_L7_POSITION, endIntensity);
                    if(FUSEE_L7_ACTIVE == 3.0)
                        CalcSpotLight();
                }      

                //endIntensity = clamp(endIntensity, 0.0, 1.0);
                gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
            }";


    }
}