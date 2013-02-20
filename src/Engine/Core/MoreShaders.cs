using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.Engine
{
    public static class MoreShaders
    {

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
                ShaderProgram spSpecular = rc.CreateShader(VsSpecular, PsSpecular);
                return spSpecular;
            }

            if (name == "bump")
            {
                ShaderProgram spBump = rc.CreateShader(VsBump, PsBump);
                return spBump;
            }
            ShaderProgram spOriginal = rc.CreateShader(Vs, Ps);
            return spOriginal;
        }

private const string VsDiffuse = @"
# version 120
/* Copies incoming vertex color without change.
    * Applies the transformation matrix to vertex position.
    */
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix


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

vec3 vPos;
 
void main()
{
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
    lightDir[0] = -normalize(FUSEE_L0_DIRECTION);
    lightDir[1] = -normalize(FUSEE_L1_DIRECTION);
    lightDir[2] = -normalize(FUSEE_L2_DIRECTION);
    lightDir[3] = -normalize(FUSEE_L3_DIRECTION);
    lightDir[4] = -normalize(FUSEE_L4_DIRECTION);
    lightDir[5] = -normalize(FUSEE_L5_DIRECTION);
    lightDir[6] = -normalize(FUSEE_L6_DIRECTION);
    lightDir[7] = -normalize(FUSEE_L7_DIRECTION); 

    if(FUSEE_L0_ACTIVE == 1) {
        endAmbient += FUSEE_L0_AMBIENT;
    }
    if(FUSEE_L1_ACTIVE == 1) {
        endAmbient += FUSEE_L1_AMBIENT;
    }
    if(FUSEE_L2_ACTIVE == 1) {
        endAmbient += FUSEE_L2_AMBIENT;
    }
    if(FUSEE_L3_ACTIVE == 1) {
        endAmbient += FUSEE_L3_AMBIENT;
    }
    if(FUSEE_L4_ACTIVE == 1) {
        endAmbient += FUSEE_L4_AMBIENT;
    }
    if(FUSEE_L5_ACTIVE == 1) {
        endAmbient += FUSEE_L5_AMBIENT;
    }
    if(FUSEE_L6_ACTIVE == 1) {
        endAmbient += FUSEE_L6_AMBIENT;
    }
    if(FUSEE_L7_ACTIVE == 1) {
        endAmbient += FUSEE_L7_AMBIENT;
    }
    
    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";

        private const string PsDiffuse = @"
/* Copies incoming fragment color without change. */
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

varying vec3 lightDir[8]; 
varying vec3 vNormal;
varying vec2 vUV;
varying vec4 endAmbient;
 
void main()
{
    vec4 endIntensity = vec4(0,0,0,0);
    float intensity[8];    
    for(int i=0; i<intensity.length();i++)
    {
        intensity[i] = max(dot(lightDir[i],normalize(vNormal)),0.0);
    } 

    if(FUSEE_L0_ACTIVE == 1) {
        endIntensity += intensity[0] * FUSEE_L0_DIFFUSE;
    }
    if(FUSEE_L1_ACTIVE == 1) {
        endIntensity += intensity[1] * FUSEE_L1_DIFFUSE;
    }
    if(FUSEE_L2_ACTIVE == 1) {
        endIntensity += intensity[2] * FUSEE_L2_DIFFUSE;
    }
    if(FUSEE_L3_ACTIVE == 1) {
        endIntensity += intensity[3] * FUSEE_L3_DIFFUSE;
    }
    if(FUSEE_L4_ACTIVE == 1) {
        endIntensity += intensity[4] * FUSEE_L4_DIFFUSE;
    }
    if(FUSEE_L5_ACTIVE == 1) {
        endIntensity += intensity[5] * FUSEE_L5_DIFFUSE;
    }
    if(FUSEE_L6_ACTIVE == 1) {
        endIntensity += intensity[6] * FUSEE_L6_DIFFUSE;
    }
    if(FUSEE_L7_ACTIVE == 1) {
        endIntensity += intensity[7] * FUSEE_L7_DIFFUSE;
    }
    endIntensity += endAmbient; 

    gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
}";

private const string VsSpecular = @"
# version 120
/* Copies incoming vertex color without change.
    * Applies the transformation matrix to vertex position.
    */
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

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
varying vec4 vDiffuse[8];
//varying vec4 vSpecular[8];
varying vec4 endAmbient;
//varying float SpecIntensity[8];
varying vec3 vHalfVector[8];

vec3 vPos;
 
void main()
{
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);

    lightDir[0] = -normalize(FUSEE_L0_DIRECTION);
    lightDir[1] = -normalize(FUSEE_L1_DIRECTION);
    lightDir[2] = -normalize(FUSEE_L2_DIRECTION);
    lightDir[3] = -normalize(FUSEE_L3_DIRECTION);
    lightDir[4] = -normalize(FUSEE_L4_DIRECTION);
    lightDir[5] = -normalize(FUSEE_L5_DIRECTION);
    lightDir[6] = -normalize(FUSEE_L6_DIRECTION);
    lightDir[7] = -normalize(FUSEE_L7_DIRECTION); 

    vec3 eyeVector = mat3(FUSEE_MVP) * fuVertex;
    vHalfVector[0] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L0_POSITION));
    vHalfVector[1] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L1_POSITION));
    vHalfVector[2] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L2_POSITION));
    vHalfVector[3] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L3_POSITION));
    vHalfVector[4] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L4_POSITION));
    vHalfVector[5] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L5_POSITION));
    vHalfVector[6] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L6_POSITION));
    vHalfVector[7] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L7_POSITION));
      
    endAmbient = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE == 1) {
        endAmbient += FUSEE_L0_AMBIENT;
    }
    if(FUSEE_L1_ACTIVE == 1) {
        endAmbient += FUSEE_L1_AMBIENT;
    }
    if(FUSEE_L2_ACTIVE == 1) {
        endAmbient += FUSEE_L2_AMBIENT;
    }
    if(FUSEE_L3_ACTIVE == 1) {
        endAmbient += FUSEE_L3_AMBIENT;
    }
    if(FUSEE_L4_ACTIVE == 1) {
        endAmbient += FUSEE_L4_AMBIENT;
    }
    if(FUSEE_L5_ACTIVE == 1) {
        endAmbient += FUSEE_L5_AMBIENT;
    }
    if(FUSEE_L6_ACTIVE == 1) {
        endAmbient += FUSEE_L6_AMBIENT;
    }
    if(FUSEE_L7_ACTIVE == 1) {
        endAmbient += FUSEE_L7_AMBIENT;
    }

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
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

uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L1_DIFFUSE;
uniform vec4 FUSEE_L2_DIFFUSE;
uniform vec4 FUSEE_L3_DIFFUSE;
uniform vec4 FUSEE_L4_DIFFUSE;
uniform vec4 FUSEE_L5_DIFFUSE;
uniform vec4 FUSEE_L6_DIFFUSE;
uniform vec4 FUSEE_L7_DIFFUSE;

varying vec3 lightDir[8]; 
varying vec3 vNormal;
varying vec2 vUV;
varying vec4 endAmbient;
varying vec3 vHalfVector[8];

 
void main()
{
    vec4 endSpecular = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE == 1) {
        float L0NdotHV = max(dot(normalize(vNormal), vHalfVector[0]), 0.0);
        endSpecular += FUSEE_L0_SPECULAR * pow(L0NdotHV, 64) * 16;
    }
    if(FUSEE_L1_ACTIVE == 1) {
        float L1NdotHV = max(dot(normalize(vNormal), vHalfVector[1]), 0.0);
        endSpecular += FUSEE_L1_SPECULAR * pow(L1NdotHV, 64) * 16;
    }
    if(FUSEE_L2_ACTIVE == 1) {
        float L2NdotHV = max(dot(normalize(vNormal), vHalfVector[2]), 0.0);
        endSpecular += FUSEE_L2_SPECULAR * pow(L2NdotHV, 64) * 16;
    }
    if(FUSEE_L3_ACTIVE == 1) {
        float L3NdotHV = max(dot(normalize(vNormal), vHalfVector[3]), 0.0);
        endSpecular += FUSEE_L3_SPECULAR * pow(L3NdotHV, 64) * 16;
    }
    if(FUSEE_L4_ACTIVE == 1) {
        float L4NdotHV = max(dot(normalize(vNormal), vHalfVector[4]), 0.0);
        endSpecular += FUSEE_L4_SPECULAR * pow(L4NdotHV, 64) * 16;
    }
    if(FUSEE_L5_ACTIVE == 1) {
        float L5NdotHV = max(dot(normalize(vNormal), vHalfVector[5]), 0.0);
        endSpecular += FUSEE_L5_SPECULAR * pow(L5NdotHV, 64) * 16;
    }
    if(FUSEE_L6_ACTIVE == 1) {
        float L6NdotHV = max(dot(normalize(vNormal), vHalfVector[6]), 0.0);
        endSpecular += FUSEE_L6_SPECULAR * pow(L6NdotHV, 64) * 16;
    }
    if(FUSEE_L7_ACTIVE == 1) {
        float L7NdotHV = max(dot(normalize(vNormal), vHalfVector[7]), 0.0);
        endSpecular += FUSEE_L7_SPECULAR * pow(L7NdotHV, 64) * 16;
    }
    
    vec4 endIntensity = vec4(0,0,0,0);
    float intensity[8];    
    for(int i=0; i<intensity.length();i++)
    {
        intensity[i] = max(dot(lightDir[i],normalize(vNormal)),0.0);
    }
    
    if(FUSEE_L0_ACTIVE == 1) {
        endIntensity += intensity[0] * FUSEE_L0_DIFFUSE;
    }
    if(FUSEE_L1_ACTIVE == 1) {
        endIntensity += intensity[1] * FUSEE_L1_DIFFUSE;
    }
    if(FUSEE_L2_ACTIVE == 1) {
        endIntensity += intensity[2] * FUSEE_L2_DIFFUSE;
    }
    if(FUSEE_L3_ACTIVE == 1) {
        endIntensity += intensity[3] * FUSEE_L3_DIFFUSE;
    }
    if(FUSEE_L4_ACTIVE == 1) {
        endIntensity += intensity[4] * FUSEE_L4_DIFFUSE;
    }
    if(FUSEE_L5_ACTIVE == 1) {
        endIntensity += intensity[5] * FUSEE_L5_DIFFUSE;
    }
    if(FUSEE_L6_ACTIVE == 1) {
        endIntensity += intensity[6] * FUSEE_L6_DIFFUSE;
    }
    if(FUSEE_L7_ACTIVE == 1) {
        endIntensity += intensity[7] * FUSEE_L7_DIFFUSE;
    }

    endIntensity += endSpecular;
    endIntensity += endAmbient; 
    gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
}";



private const string VsBump = @"
#ifndef GL_ES
    # version 120
#endif

/* Copies incoming vertex color without change.
    * Applies the transformation matrix to vertex position.
    */
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

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
varying vec3 vHalfVector[8];

vec3 vPos;
 
void main()
{
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);

    lightDir[0] = -normalize(FUSEE_L0_DIRECTION);
    lightDir[1] = -normalize(FUSEE_L1_DIRECTION);
    lightDir[2] = -normalize(FUSEE_L2_DIRECTION);
    lightDir[3] = -normalize(FUSEE_L3_DIRECTION);
    lightDir[4] = -normalize(FUSEE_L4_DIRECTION);
    lightDir[5] = -normalize(FUSEE_L5_DIRECTION);
    lightDir[6] = -normalize(FUSEE_L6_DIRECTION);
    lightDir[7] = -normalize(FUSEE_L7_DIRECTION); 

    vec3 eyeVector = mat3(FUSEE_MVP) * fuVertex;
    vHalfVector[0] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L0_POSITION));
    vHalfVector[1] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L1_POSITION));
    vHalfVector[2] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L2_POSITION));
    vHalfVector[3] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L3_POSITION));
    vHalfVector[4] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L4_POSITION));
    vHalfVector[5] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L5_POSITION));
    vHalfVector[6] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L6_POSITION));
    vHalfVector[7] = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L7_POSITION));
      
    endAmbient = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE == 1) {
        endAmbient += FUSEE_L0_AMBIENT;
    }
    if(FUSEE_L1_ACTIVE == 1) {
        endAmbient += FUSEE_L1_AMBIENT;
    }
    if(FUSEE_L2_ACTIVE == 1) {
        endAmbient += FUSEE_L2_AMBIENT;
    }
    if(FUSEE_L3_ACTIVE == 1) {
        endAmbient += FUSEE_L3_AMBIENT;
    }
    if(FUSEE_L4_ACTIVE == 1) {
        endAmbient += FUSEE_L4_AMBIENT;
    }
    if(FUSEE_L5_ACTIVE == 1) {
        endAmbient += FUSEE_L5_AMBIENT;
    }
    if(FUSEE_L6_ACTIVE == 1) {
        endAmbient += FUSEE_L6_AMBIENT;
    }
    if(FUSEE_L7_ACTIVE == 1) {
        endAmbient += FUSEE_L7_AMBIENT;
    }

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";

private const string PsBump = @"
           #ifndef GL_ES
               #version 120
            #endif

            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

uniform sampler2D texture1;
uniform sampler2D normalTex;

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

varying vec3 lightDir[8]; 
varying vec3 vNormal;
varying vec2 vUV;
varying vec4 endAmbient;
varying vec3 vHalfVector[8];
 
void main()
{       
    float maxVariance = 2;
    float minVariance = maxVariance/2;
    vec3 tempNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);
 
    vec4 endSpecular = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE == 1) {
        float L0NdotHV = max(dot(normalize(tempNormal), vHalfVector[0]), 0.0);
        endSpecular += FUSEE_L0_SPECULAR * pow(L0NdotHV, 64) * 16;
    }
    if(FUSEE_L1_ACTIVE == 1) {
        float L1NdotHV = max(dot(normalize(tempNormal), vHalfVector[1]), 0.0);
        endSpecular += FUSEE_L1_SPECULAR * pow(L1NdotHV, 64) * 16;
    }
    if(FUSEE_L2_ACTIVE == 1) {
        float L2NdotHV = max(dot(normalize(tempNormal), vHalfVector[2]), 0.0);
        endSpecular += FUSEE_L2_SPECULAR * pow(L2NdotHV, 64) * 16;
    }
    if(FUSEE_L3_ACTIVE == 1) {
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector[3]), 0.0);
        endSpecular += FUSEE_L3_SPECULAR * pow(L3NdotHV, 64) * 16;
    }
    if(FUSEE_L4_ACTIVE == 1) {
        float L4NdotHV = max(dot(normalize(tempNormal), vHalfVector[4]), 0.0);
        endSpecular += FUSEE_L4_SPECULAR * pow(L4NdotHV, 64) * 16;
    }
    if(FUSEE_L5_ACTIVE == 1) {
        float L5NdotHV = max(dot(normalize(tempNormal), vHalfVector[5]), 0.0);
        endSpecular += FUSEE_L5_SPECULAR * pow(L5NdotHV, 64) * 16;
    }
    if(FUSEE_L6_ACTIVE == 1) {
        float L6NdotHV = max(dot(normalize(tempNormal), vHalfVector[6]), 0.0);
        endSpecular += FUSEE_L6_SPECULAR * pow(L6NdotHV, 64) * 16;
    }
    if(FUSEE_L7_ACTIVE == 1) {
        float L7NdotHV = max(dot(normalize(tempNormal), vHalfVector[7]), 0.0);
        endSpecular += FUSEE_L7_SPECULAR * pow(L7NdotHV, 64) * 16;
    }
    
    vec4 endIntensity = vec4(0,0,0,0);
    float intensity[8];    
    for(int i=0; i<intensity.length();i++)
    {
        intensity[i] = max(dot(lightDir[i],normalize(vNormal)),0.0);
    }
    
    if(FUSEE_L0_ACTIVE == 1) {
        endIntensity += intensity[0] * FUSEE_L0_DIFFUSE;
    }
    if(FUSEE_L1_ACTIVE == 1) {
        endIntensity += intensity[1] * FUSEE_L1_DIFFUSE;
    }
    if(FUSEE_L2_ACTIVE == 1) {
        endIntensity += intensity[2] * FUSEE_L2_DIFFUSE;
    }
    if(FUSEE_L3_ACTIVE == 1) {
        endIntensity += intensity[3] * FUSEE_L3_DIFFUSE;
    }
    if(FUSEE_L4_ACTIVE == 1) {
        endIntensity += intensity[4] * FUSEE_L4_DIFFUSE;
    }
    if(FUSEE_L5_ACTIVE == 1) {
        endIntensity += intensity[5] * FUSEE_L5_DIFFUSE;
    }
    if(FUSEE_L6_ACTIVE == 1) {
        endIntensity += intensity[6] * FUSEE_L6_DIFFUSE;
    }
    if(FUSEE_L7_ACTIVE == 1) {
        endIntensity += intensity[7] * FUSEE_L7_DIFFUSE;
    }

    endIntensity += endSpecular;
    endIntensity += endAmbient; 
    gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
}";



private const string Vs = @"
            #ifndef GL_ES
               #version 120
            #endif

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
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
                vUV = fuUV;
            }";

        private const string Ps = @"
           #ifndef GL_ES
               #version 120
            #endif

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
    }
}