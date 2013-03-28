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
#version 120

attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix


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

vec3 vPos;
 
void main()
{
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);

    if(FUSEE_L0_ACTIVE == 1.0) {
        endAmbient += FUSEE_L0_AMBIENT;
    }
    if(FUSEE_L1_ACTIVE == 1.0) {
        endAmbient += FUSEE_L1_AMBIENT;
    }
    if(FUSEE_L2_ACTIVE == 1.0) {
        endAmbient += FUSEE_L2_AMBIENT;
    }
    if(FUSEE_L3_ACTIVE == 1.0) {
        endAmbient += FUSEE_L3_AMBIENT;
    }
    if(FUSEE_L4_ACTIVE == 1.0) {
        endAmbient += FUSEE_L4_AMBIENT;
    }
    if(FUSEE_L5_ACTIVE == 1.0) {
        endAmbient += FUSEE_L5_AMBIENT;
    }
    if(FUSEE_L6_ACTIVE == 1.0) {
        endAmbient += FUSEE_L6_AMBIENT;
    }
    if(FUSEE_L7_ACTIVE == 1.0) {
        endAmbient += FUSEE_L7_AMBIENT;
    }
    
    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";

private const string PsDiffuse = @"
#version 120

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
 
void main()
{
    vec4 endIntensity = vec4(0,0,0,0);

    if(FUSEE_L0_ACTIVE == 1.0) {
        endIntensity += max(dot(-normalize(FUSEE_L0_DIRECTION),normalize(vNormal)),0.0) * FUSEE_L0_DIFFUSE;
    }
    if(FUSEE_L1_ACTIVE == 1.0) {
        endIntensity += max(dot(-normalize(FUSEE_L1_DIRECTION),normalize(vNormal)),0.0) * FUSEE_L1_DIFFUSE;
    }
    if(FUSEE_L2_ACTIVE == 1.0) {
        endIntensity += max(dot(-normalize(FUSEE_L2_DIRECTION),normalize(vNormal)),0.0) * FUSEE_L2_DIFFUSE;
    }
    if(FUSEE_L3_ACTIVE == 1.0) {
        endIntensity += max(dot(-normalize(FUSEE_L3_DIRECTION),normalize(vNormal)),0.0) * FUSEE_L3_DIFFUSE;
    }
    if(FUSEE_L4_ACTIVE == 1.0) {
        endIntensity += max(dot(-normalize(FUSEE_L4_DIRECTION),normalize(vNormal)),0.0) * FUSEE_L4_DIFFUSE;
    }
    if(FUSEE_L5_ACTIVE == 1.0) {
        endIntensity += max(dot(-normalize(FUSEE_L5_DIRECTION),normalize(vNormal)),0.0) * FUSEE_L5_DIFFUSE;
    }
    if(FUSEE_L6_ACTIVE == 1.0) {
        endIntensity += max(dot(-normalize(FUSEE_L6_DIRECTION),normalize(vNormal)),0.0) * FUSEE_L6_DIFFUSE;
    }
    if(FUSEE_L7_ACTIVE == 1.0) {
        endIntensity += max(dot(-normalize(FUSEE_L7_DIRECTION),normalize(vNormal)),0.0) * FUSEE_L7_DIFFUSE;
    }
    endIntensity += endAmbient; 

    gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
}";

private const string VsSpecular = @"
#ifndef GL_ES
    #version 120
#endif

attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

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
varying vec3 eyeVector;

vec3 vPos;
 
void main()
{
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);

    eyeVector = mat3(FUSEE_MVP) * fuVertex;
      
    endAmbient = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE == 1.0) {
        endAmbient += FUSEE_L0_AMBIENT;
    }
    if(FUSEE_L1_ACTIVE == 1.0) {
        endAmbient += FUSEE_L1_AMBIENT;
    }
    if(FUSEE_L2_ACTIVE == 1.0) {
        endAmbient += FUSEE_L2_AMBIENT;
    }
    if(FUSEE_L3_ACTIVE == 1.0) {
        endAmbient += FUSEE_L3_AMBIENT;
    }
    if(FUSEE_L4_ACTIVE == 1.0) {
        endAmbient += FUSEE_L4_AMBIENT;
    }
    if(FUSEE_L5_ACTIVE == 1.0) {
        endAmbient += FUSEE_L5_AMBIENT;
    }
    if(FUSEE_L6_ACTIVE == 1.0) {
        endAmbient += FUSEE_L6_AMBIENT;
    }
    if(FUSEE_L7_ACTIVE == 1.0) {
        endAmbient += FUSEE_L7_AMBIENT;
    }

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";

private const string PsSpecular = @"
#ifndef GL_ES
    #version 120
#endif

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
varying vec4 endAmbient;
varying vec3 eyeVector;

void main()
{
    vec4 endSpecular = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L0_POSITION));
        float L0NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L0NdotHV, 64.0) * 8.0;
        endSpecular += FUSEE_L0_SPECULAR * shine;
    }
    if(FUSEE_L1_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L1_POSITION));
        float L1NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L1NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L1_SPECULAR * shine;
    }
    if(FUSEE_L2_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L2_POSITION));
        float L2NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L2NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L2_SPECULAR * shine;
    }
    if(FUSEE_L3_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L3_POSITION));
        float L3NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L3_SPECULAR * shine;
    }
    if(FUSEE_L4_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L4_POSITION));
        float L4NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L4NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L4_SPECULAR * shine;
    }
    if(FUSEE_L5_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L5_POSITION));
        float L5NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L5NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L5_SPECULAR * shine;
    }
    if(FUSEE_L6_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L6_POSITION));
        float L6NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L6NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L6_SPECULAR * shine;
    }
    if(FUSEE_L7_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L7_POSITION));
        float L7NdotHV = max(dot(normalize(vNormal), vHalfVector), 0.0);
        float shine = pow(L7NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L7_SPECULAR * shine;
    }
    
    vec4 endIntensity = vec4(0,0,0,0);

    if(FUSEE_L0_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L0_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L0_DIFFUSE;
    }
    if(FUSEE_L1_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L1_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L1_DIFFUSE;
    }
    if(FUSEE_L2_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L2_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L2_DIFFUSE;
    }
    if(FUSEE_L3_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L3_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L3_DIFFUSE;
    }
    if(FUSEE_L4_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L4_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L4_DIFFUSE;
    }
    if(FUSEE_L5_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L5_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L5_DIFFUSE;
    }
    if(FUSEE_L6_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L6_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L6_DIFFUSE;
    }
    if(FUSEE_L7_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L7_DIRECTION),normalize(vNormal)),0.0);
        endIntensity += intensity * FUSEE_L7_DIFFUSE;
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

vec3 vPos;
 
void main()
{
    vUV = fuUV;
    vNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);

    eyeVector = mat3(FUSEE_MVP) * fuVertex;
      
    endAmbient = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE == 1.0) {
        endAmbient += FUSEE_L0_AMBIENT;
    }
    if(FUSEE_L1_ACTIVE == 1.0) {
        endAmbient += FUSEE_L1_AMBIENT;
    }
    if(FUSEE_L2_ACTIVE == 1.0) {
        endAmbient += FUSEE_L2_AMBIENT;
    }
    if(FUSEE_L3_ACTIVE == 1.0) {
        endAmbient += FUSEE_L3_AMBIENT;
    }
    if(FUSEE_L4_ACTIVE == 1.0) {
        endAmbient += FUSEE_L4_AMBIENT;
    }
    if(FUSEE_L5_ACTIVE == 1.0) {
        endAmbient += FUSEE_L5_AMBIENT;
    }
    if(FUSEE_L6_ACTIVE == 1.0) {
        endAmbient += FUSEE_L6_AMBIENT;
    }
    if(FUSEE_L7_ACTIVE == 1.0) {
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
 
void main()
{       
    float maxVariance = 2.0;
    float minVariance = maxVariance/2.0;
    vec3 tempNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);
 
    vec4 endSpecular = vec4(0,0,0,0);
    if(FUSEE_L0_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L0_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L0_SPECULAR * shine;
    }
    if(FUSEE_L1_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L1_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L1_SPECULAR * shine;
    }
    if(FUSEE_L2_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L2_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L2_SPECULAR * shine;
    }
    if(FUSEE_L3_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L3_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L3_SPECULAR * shine;
    }
    if(FUSEE_L4_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L4_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L4_SPECULAR * shine;
    }
    if(FUSEE_L5_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L5_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L5_SPECULAR * shine;
    }
    if(FUSEE_L6_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L6_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L6_SPECULAR * shine;
    }
    if(FUSEE_L7_ACTIVE == 1.0) {
        vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L7_POSITION));
        float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
        float shine = pow(L3NdotHV, 64.0) * 16.0;
        endSpecular += FUSEE_L7_SPECULAR * shine;
    }
    
    vec4 endIntensity = vec4(0,0,0,0);

    if(FUSEE_L0_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L0_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L0_DIFFUSE;
    }
    if(FUSEE_L1_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L1_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L1_DIFFUSE;
    }
    if(FUSEE_L2_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L2_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L2_DIFFUSE;
    }
    if(FUSEE_L3_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L3_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L3_DIFFUSE;
    }
    if(FUSEE_L4_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L4_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L4_DIFFUSE;
    }
    if(FUSEE_L5_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L5_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L5_DIFFUSE;
    }
    if(FUSEE_L6_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L6_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L6_DIFFUSE;
    }
    if(FUSEE_L7_ACTIVE == 1.0) {
        float intensity = max(dot(-normalize(FUSEE_L7_DIRECTION),normalize(tempNormal)),0.0);
        endIntensity += intensity * FUSEE_L7_DIFFUSE;
    }

    endIntensity += endSpecular;
    endIntensity += endAmbient; 
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
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
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
    }
}