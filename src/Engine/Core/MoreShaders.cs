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

            if (name == "diffuse2")
            {
                ShaderProgram spDiffuse2 = rc.CreateShader(VsDiffuse2, PsDiffuse2);
                return spDiffuse2;
            }

            if (name == "specular")
            {
                ShaderProgram spSpecular = rc.CreateShader(VsSpecular, PsSpecular);
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
uniform mat4 FUSEE_MP;
uniform mat4 FUSEE_ITP;

varying vec2 vUV;
varying vec3 vNormal;
varying vec3 vGlobalPos;

vec3 vPos;
 
void main()
{
    vGlobalPos = mat3(FUSEE_M[0].xyz, FUSEE_M[1].xyz, FUSEE_M[2].xyz) * fuVertex;
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_M[0].xyz, FUSEE_M[1].xyz, FUSEE_M[2].xyz) * fuNormal);
    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";


private const string PsDiffuse = @"
#ifdef GL_ES
    precision highp float;
#endif

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

varying vec3 vNormal;
varying vec2 vUV;
varying vec3 vGlobalPos;

void CalcDirectLight(vec4 difColor, vec4 ambColor, vec3 direction, inout vec4 intensity) {
    intensity += ambColor;
    intensity += max(dot(-normalize(direction),normalize(vNormal)),0.0) * difColor;
}

void CalcPointLight(vec4 difColor, vec4 ambColor, vec3 position, inout vec4 intensity) {
    intensity += ambColor;
    vec3 pos = vGlobalPos - position;
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

private const string VsDiffuse2 = @"
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;
       
uniform mat4 FUSEE_M;
uniform mat4 FUSEE_MV; 
uniform mat4 FUSEE_MVP;  

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


varying vec2 vUV;
varying vec3 vNormal;
varying vec4 endAmbient;
varying vec3 vPos;

void main(void)
{
   vUV = fuUV;
   vPos = normalize(vec3(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuVertex));       
   vNormal = normalize(vec3(mat3(FUSEE_M[0].xyz, FUSEE_M[1].xyz, FUSEE_M[2].xyz) * fuNormal));

    endAmbient=vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE != 0.0) {
        endAmbient += FUSEE_L0_AMBIENT;
    }
    if(FUSEE_L1_ACTIVE != 0.0) {
        endAmbient += FUSEE_L1_AMBIENT;
    }
    if(FUSEE_L2_ACTIVE != 0.0) {
        endAmbient += FUSEE_L2_AMBIENT;
    }
    if(FUSEE_L3_ACTIVE != 0.0) {
        endAmbient += FUSEE_L3_AMBIENT;
    }
    if(FUSEE_L4_ACTIVE != 0.0) {
        endAmbient += FUSEE_L4_AMBIENT;
    }
    if(FUSEE_L5_ACTIVE != 0.0) {
        endAmbient += FUSEE_L5_AMBIENT;
    }
    if(FUSEE_L6_ACTIVE != 0.0) {
        endAmbient += FUSEE_L6_AMBIENT;
    }
    if(FUSEE_L7_ACTIVE != 0.0) {
        endAmbient += FUSEE_L7_AMBIENT;
    }
    //endAmbient=normalize(endAmbient);
    //endAmbient = clamp(endAmbient, 0.0, 1.0); 
   gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

}";

private const string PsDiffuse2 = @"
#ifdef GL_ES
    precision highp float;
#endif

uniform sampler2D texture1;

uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L1_DIFFUSE;
uniform vec4 FUSEE_L2_DIFFUSE;
uniform vec4 FUSEE_L3_DIFFUSE;
uniform vec4 FUSEE_L4_DIFFUSE;
uniform vec4 FUSEE_L5_DIFFUSE;
uniform vec4 FUSEE_L6_DIFFUSE;
uniform vec4 FUSEE_L7_DIFFUSE;

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
varying vec3 vPos;
varying vec2 vUV;
varying vec4 endAmbient;

void main(void)
{

// diffuse
   vec4 Idiff = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE != 0.0){  
        Idiff += FUSEE_L0_DIFFUSE * dot(vNormal,-FUSEE_L0_DIRECTION)*6.0; 
    }

    if(FUSEE_L1_ACTIVE != 0.0){ 
        Idiff += FUSEE_L1_DIFFUSE * dot(vNormal,-FUSEE_L1_DIRECTION); 
    }

    if(FUSEE_L2_ACTIVE != 0.0){ 
        Idiff += FUSEE_L2_DIFFUSE * dot(vNormal,-FUSEE_L2_DIRECTION); 
    }

    if(FUSEE_L3_ACTIVE != 0.0){
        Idiff += FUSEE_L3_DIFFUSE * dot(vNormal,-FUSEE_L3_DIRECTION); 
    }

    if(FUSEE_L4_ACTIVE != 0.0){ 
        Idiff += FUSEE_L4_DIFFUSE * dot(vNormal,-FUSEE_L4_DIRECTION); 
    }

    if(FUSEE_L5_ACTIVE != 0.0){  
        Idiff += FUSEE_L5_DIFFUSE * dot(vNormal,-FUSEE_L5_DIRECTION); 
    }

    if(FUSEE_L6_ACTIVE != 0.0){ 
        Idiff += FUSEE_L6_DIFFUSE * dot(vNormal,-FUSEE_L6_DIRECTION); 
    }

    if(FUSEE_L7_ACTIVE != 0.0){   
        Idiff += FUSEE_L7_DIFFUSE * dot(vNormal,-FUSEE_L7_DIRECTION); 
    }

    Idiff = clamp(Idiff, 0.0, 1.0); 
    gl_FragColor = texture2D(texture1, vUV)*(Idiff*endAmbient);
}

";

private const string VsSpecular = @"
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix
uniform mat4 FUSEE_M;



varying vec2 vUV;
varying vec3 vNormal;
varying vec3 viewVertex;
varying vec3 eyeVector;

vec3 vPos;
 
void main()
{
    viewVertex = fuVertex;
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_M[0].xyz, FUSEE_M[1].xyz, FUSEE_M[2].xyz) * fuNormal);

    eyeVector = mat3(FUSEE_MVP[0].xyz, FUSEE_MVP[1].xyz, FUSEE_MVP[2].xyz) * fuVertex;
      

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";


private const string PsSpecular = @"
/* Copies incoming fragment color without change. */
#ifdef GL_ES
    precision highp float;
#endif

uniform sampler2D texture1;
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

varying vec3 vNormal;
varying vec2 vUV;
varying vec3 eyeVector;
varying vec3 viewVertex;

            void CalcDirectLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 direction, inout vec4 intensity) {
                intensity += ambColor;
                intensity += max(dot(-normalize(direction),normalize(vNormal)),0.0) * difColor;
            }

void main()
{
    vec4 endSpecular = vec4(0,0,0,0);


    if(FUSEE_L0_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L0_DIRECTION));
        float L0NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L0NdotHV, specularLevel) * 8.0;
        endSpecular += FUSEE_L0_SPECULAR * shine;
    }

    if(FUSEE_L1_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L1_POSITION));
        float L1NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L1NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L1_SPECULAR * shine;
    }
    if(FUSEE_L2_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L2_POSITION));
        float L2NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L2NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L2_SPECULAR * shine;
    }
    if(FUSEE_L3_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L3_POSITION));
        float L3NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L3_SPECULAR * shine;
    }
    if(FUSEE_L4_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L4_POSITION));
        float L4NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L4NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L4_SPECULAR * shine;
    }
    if(FUSEE_L5_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L5_POSITION));
        float L5NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L5NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L5_SPECULAR * shine;
    }
    if(FUSEE_L6_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L6_POSITION));
        float L6NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L6NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L6_SPECULAR * shine;
    }
    if(FUSEE_L7_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L7_POSITION));
        float L7NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L7NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L7_SPECULAR * shine;
    }
    
    vec4 endIntensity = vec4(0,0,0,0);

    if(FUSEE_L0_ACTIVE != 0.0){
        if(FUSEE_L0_ACTIVE == 1.0)
            CalcDirectLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_DIRECTION, endIntensity);
    }
  
    if(FUSEE_L1_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L1_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L1_DIFFUSE;
    }
    if(FUSEE_L2_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L2_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L2_DIFFUSE;
    }
    if(FUSEE_L3_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L3_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L3_DIFFUSE;
    }
    if(FUSEE_L4_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L4_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L4_DIFFUSE;
    }
    if(FUSEE_L5_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L5_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L5_DIFFUSE;
    }
    if(FUSEE_L6_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L6_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L6_DIFFUSE;
    }
    if(FUSEE_L7_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L7_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L7_DIFFUSE;
    }

    endIntensity += endSpecular;
    endIntensity = clamp(endIntensity, 0, 1); 
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
      
    endAmbient = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE != 0.0) {
        endAmbient += FUSEE_L0_AMBIENT;
    }
    if(FUSEE_L1_ACTIVE != 0.0) {
        endAmbient += FUSEE_L1_AMBIENT;
    }
    if(FUSEE_L2_ACTIVE != 0.0) {
        endAmbient += FUSEE_L2_AMBIENT;
    }
    if(FUSEE_L3_ACTIVE != 0.0) {
        endAmbient += FUSEE_L3_AMBIENT;
    }
    if(FUSEE_L4_ACTIVE != 0.0) {
        endAmbient += FUSEE_L4_AMBIENT;
    }
    if(FUSEE_L5_ACTIVE != 0.0) {
        endAmbient += FUSEE_L5_AMBIENT;
    }
    if(FUSEE_L6_ACTIVE != 0.0) {
        endAmbient += FUSEE_L6_AMBIENT;
    }
    if(FUSEE_L7_ACTIVE != 0.0) {
        endAmbient += FUSEE_L7_AMBIENT;
    }

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
 
void main()
{       
    float maxVariance = 2.0;
    float minVariance = maxVariance/2.0;
    vec3 tempNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);
 


    vec4 endSpecular = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE != 0.0 ) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L0_POSITION));
        float L3NdotHV = max(min(dot(normalize(tempNormal), vHalfVector),1.0), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L0_SPECULAR * shine;
    }
//    vec3 lightVec = normalize(FUSEE_L0_DIRECTION - eyeVector);
//    vec3 r = -normalize(reflect(lightVec, vNormal));
//    float s = pow(max(dot(r, normalize(-viewVertex)), 0.0), specularLevel);
//    endSpecular = FUSEE_L0_SPECULAR * s;

    if(FUSEE_L1_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L1_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L1_SPECULAR * shine;
    }
    if(FUSEE_L2_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L2_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L2_SPECULAR * shine;
    }
    if(FUSEE_L3_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L3_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L3_SPECULAR * shine;
    }
    if(FUSEE_L4_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L4_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L4_SPECULAR * shine;
    }
    if(FUSEE_L5_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L5_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L5_SPECULAR * shine;
    }
    if(FUSEE_L6_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L6_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L6_SPECULAR * shine;
    }
    if(FUSEE_L7_ACTIVE != 0.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L7_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, specularLevel) * 16.0;
        endSpecular += FUSEE_L7_SPECULAR * shine;
    }
    
    vec4 endIntensity = vec4(0,0,0,0);



    if(FUSEE_L0_ACTIVE != 0.0) {
        endIntensity += max(dot(-normalize(FUSEE_L0_DIRECTION),normalize(tempNormal)),0.0) * FUSEE_L0_DIFFUSE;
    }
    if(FUSEE_L1_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L1_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L1_DIFFUSE;
    }
    if(FUSEE_L2_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L2_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L2_DIFFUSE;
    }
    if(FUSEE_L3_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L3_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L3_DIFFUSE;
    }
    if(FUSEE_L4_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L4_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L4_DIFFUSE;
    }
    if(FUSEE_L5_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L5_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L5_DIFFUSE;
    }
    if(FUSEE_L6_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L6_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L6_DIFFUSE;
    }
    if(FUSEE_L7_ACTIVE != 0.0) {
        float intensity = max(dot(-normalize(FUSEE_L7_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L7_DIFFUSE;
    }

    endIntensity += endSpecular;
    endIntensity += endAmbient; 
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
                  
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec4 vWorldVertex;
            varying vec3 vWorldNormal;
            varying vec3 vViewVec;
    
            uniform mat4 FUSEE_M;
            uniform mat4 FUSEE_MV; 
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;
            uniform mat4 FUSEE_V;
            uniform mat4 FUSEE_P; 

            uniform float FUSEE_L0_ACTIVE;


            void main()
            {
                vWorldVertex = FUSEE_M * vec4(fuVertex, 1.0);
                vec4 viewVertex = FUSEE_V * vWorldVertex;
                gl_Position = FUSEE_P * viewVertex;
                vUV = fuUV;

                vWorldNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vNormal = normalize(mat3(FUSEE_M[0].xyz, FUSEE_M[1].xyz, FUSEE_M[2].xyz) * fuNormal);
                vViewVec = normalize(-viewVertex.xyz);

                endAmbient=vec4(0,0,0,0);
                if(FUSEE_L0_ACTIVE != 0.0) {
                    endAmbient += FUSEE_L0_AMBIENT;
                }

            }";


        private const string PsSpecular2 = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

         
            uniform sampler2D texture1;
            uniform vec3 FUSEE_L0_DIRECTION;
            uniform vec3 FUSEE_L0_POSITION;
            uniform vec4 FUSEE_L0_DIFFUSE;
            uniform vec4 FUSEE_L0_AMBIENT;
            uniform vec4 FUSEE_L0_SPECULAR;
            uniform float FUSEE_L0_ACTIVE;
            uniform float specularLevel;
    
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec4 vWorldVertex;
            varying vec3 vWorldNormal;
            varying vec3 vViewVec;

            void CalcDirectLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 direction, inout vec4 intensity) {
                intensity += ambColor;
                intensity += max(dot(-normalize(direction),normalize(vNormal)),0.0) * difColor;
            }

            void main()
            {             
                vec3 normal = normalize(vWorldNormal);
                vec3 lightVec = normalize(FUSEE_L0_DIRECTION - normalize(vWorldVertex.xyz));
                
                // Calculate specular term
                vec3 r = -normalize(reflect(lightVec, normal));
                float s = pow(max(dot(r, vViewVec), 0.0), specularLevel);
                
                vec4 endIntensity = vec4(0, 0, 0, 0);

                if(FUSEE_L0_ACTIVE != 0.0){
                    if(FUSEE_L0_ACTIVE == 1.0)
                        CalcDirectLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_SPECULAR, FUSEE_L0_DIRECTION, endIntensity);
                    if(FUSEE_L0_ACTIVE == 2.0)
                        CalcPointLight(FUSEE_L0_DIFFUSE, FUSEE_L0_AMBIENT, FUSEE_L0_POSITION, endIntensity);
                    if(FUSEE_L0_ACTIVE == 3.0)
                        CalcSpotLight();
                }  
                
                endIntensity += FUSEE_L0_SPECULAR * s;

                gl_FragColor = texture2D(texture1, vUV) * endIntensity;  /* *dot(vNormal, vec3(0, 0, 1))*/;
            }";

    }
}