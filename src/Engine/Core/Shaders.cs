using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.Engine
{
    public static class Shaders
    {

        public static ShaderProgram GetShader(string name, RenderContext rc)
        {
            if (name == "animation")
            {
                ShaderProgram spSimple = rc.CreateShader(VsAnimation, PsColor);
                return spSimple;
            }
            if (name == "weavy")
            {
                ShaderProgram spSimple = rc.CreateShader(VsWeavy, PsColor);
                return spSimple;
            }
            if (name == "flat")
            {
                ShaderProgram spSimple = rc.CreateShader(VsFlatten, PsColor);
                return spSimple;
            }
            if (name == "simple")
            {

                ShaderProgram spSimple = rc.CreateShader(Vs, PsSimple);
                return spSimple;
            }
            if (name == "color")
            {

                ShaderProgram spColor = rc.CreateShader(Vs, PsColor);
                return spColor;
            }

            if (name == "singleLight")
            {
                ShaderProgram spLight = rc.CreateShader(VsSingleLight, PsSingleLight);
                return spLight;
            }

            if (name == "multiLight")
            {
                ShaderProgram spLight = rc.CreateShader(VsMultiLight, PsMultiLight);
                return spLight;
            }
            ShaderProgram spOriginal = rc.CreateShader(Vs, PsSimple);
            return spOriginal;
        }

        private const string VsMultiLight2 = @"
# version 120
/* Copies incoming vertex color without change.
    * Applies the transformation matrix to vertex position.
    */
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;



uniform float FUSEE_L0_ACTIVE;
uniform vec4 FUSEE_L0_AMBIENT;
uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L0_SPECULAR;
uniform vec3 FUSEE_L0_POSITION;
uniform vec3 FUSEE_L0_DIRECTION;

uniform float FUSEE_L1_ACTIVE;
uniform vec4 FUSEE_L1_AMBIENT;
uniform vec4 FUSEE_L1_DIFFUSE;
uniform vec4 FUSEE_L1_SPECULAR;
uniform vec3 FUSEE_L1_POSITION;
uniform vec3 FUSEE_L1_DIRECTION;

uniform float FUSEE_L2_ACTIVE;
uniform vec4 FUSEE_L2_AMBIENT;
uniform vec4 FUSEE_L2_DIFFUSE;
uniform vec4 FUSEE_L2_SPECULAR;
uniform vec3 FUSEE_L2_POSITION;
uniform vec3 FUSEE_L2_DIRECTION;

uniform float FUSEE_L3_ACTIVE;
uniform vec4 FUSEE_L3_AMBIENT;
uniform vec4 FUSEE_L3_DIFFUSE;
uniform vec4 FUSEE_L3_SPECULAR;
uniform vec3 FUSEE_L3_POSITION;
uniform vec3 FUSEE_L3_DIRECTION;

uniform float FUSEE_L4_ACTIVE;
uniform vec4 FUSEE_L4_AMBIENT;
uniform vec4 FUSEE_L4_DIFFUSE;
uniform vec4 FUSEE_L4_SPECULAR;
uniform vec3 FUSEE_L4_POSITION;
uniform vec3 FUSEE_L4_DIRECTION;

uniform float FUSEE_L5_ACTIVE;
uniform vec4 FUSEE_L5_AMBIENT;
uniform vec4 FUSEE_L5_DIFFUSE;
uniform vec4 FUSEE_L5_SPECULAR;
uniform vec3 FUSEE_L5_POSITION;
uniform vec3 FUSEE_L5_DIRECTION;

uniform float FUSEE_L6_ACTIVE;
uniform vec4 FUSEE_L6_AMBIENT;
uniform vec4 FUSEE_L6_DIFFUSE;
uniform vec4 FUSEE_L6_SPECULAR;
uniform vec3 FUSEE_L6_POSITION;
uniform vec3 FUSEE_L6_DIRECTION;

uniform float FUSEE_L7_ACTIVE;
uniform vec4 FUSEE_L7_AMBIENT;
uniform vec4 FUSEE_L7_DIFFUSE;
uniform vec4 FUSEE_L7_SPECULAR;
uniform vec3 FUSEE_L7_POSITION;
uniform vec3 FUSEE_L7_DIRECTION;

uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

varying vec3 LC; 
varying float SpecIntensity;

vec3 normal;
vec3 pos;

void calcDiffuse(in vec3 lightPos, in vec3 lightColor, inout vec3 endColor)
{
  vec3 toLight = lightPos - pos;
  endColor  += max(dot(normalize(toLight), normal), 0.0) * lightColor;
}

void main()
{

  gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

    normal = mat3(FUSEE_ITMV) * fuNormal;
    pos = vec3(FUSEE_MVP * vec4(fuVertex,1));
  
    vec3 diffuseColor = vec3(0,0,0);
    if(FUSEE_L0_ACTIVE == 1)
      calcDiffuse(vec3(FUSEE_L0_POSITION), vec3(FUSEE_L0_DIFFUSE), diffuseColor);
    if(FUSEE_L1_ACTIVE == 1)
      calcDiffuse(vec3(FUSEE_L1_POSITION), vec3(FUSEE_L1_DIFFUSE), diffuseColor);
    if(FUSEE_L2_ACTIVE == 1)
      calcDiffuse(vec3(FUSEE_L2_POSITION), vec3(FUSEE_L2_DIFFUSE), diffuseColor);
    if(FUSEE_L3_ACTIVE == 1)
      calcDiffuse(vec3(FUSEE_L3_POSITION), vec3(FUSEE_L3_DIFFUSE), diffuseColor);
    if(FUSEE_L4_ACTIVE == 1)
      calcDiffuse(vec3(FUSEE_L4_POSITION), vec3(FUSEE_L4_DIFFUSE), diffuseColor);
    if(FUSEE_L5_ACTIVE == 1)
      calcDiffuse(vec3(FUSEE_L5_POSITION), vec3(FUSEE_L5_DIFFUSE), diffuseColor);
    if(FUSEE_L6_ACTIVE == 1)
      calcDiffuse(vec3(FUSEE_L6_POSITION), vec3(FUSEE_L6_DIFFUSE), diffuseColor);
    if(FUSEE_L7_ACTIVE == 1)
      calcDiffuse(vec3(FUSEE_L7_POSITION), vec3(FUSEE_L7_DIFFUSE), diffuseColor);
    LC = diffuseColor;

         vec3 reflectVec =  vec3(1,1,1); // reflect(-(FUSEE_L0_POSITION-pos), normal);
         vec3 viewVec    = normalize(-pos);
         SpecIntensity   = max(dot(reflectVec, viewVec), 0.0);


}";


        private const string PsMultiLight2 = @"
#ifdef GL_ES
precision highp float;
#endif

varying vec3 LC;
varying float SpecIntensity;

void main()
{
    
    gl_FragColor = vec4(LC ,1) ;
}";

        private const string VsMultiLight = @"
//  # version 120 // not working with GL_ES
/* Copies incoming vertex color without change.
    * Applies the transformation matrix to vertex position.
    */
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
varying vec3 vNormal;

varying vec3 fuL0HalfVector;
varying vec3 fuL1HalfVector;

uniform float FUSEE_MAT_SHININESS;
        
uniform float FUSEE_L0_ACTIVE;
uniform vec4 FUSEE_L0_AMBIENT;
uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L0_SPECULAR;
uniform vec3 FUSEE_L0_POSITION;
uniform vec3 FUSEE_L0_DIRECTION;

uniform float FUSEE_L1_ACTIVE;
uniform vec4 FUSEE_L1_AMBIENT;
uniform vec4 FUSEE_L1_DIFFUSE;
uniform vec4 FUSEE_L1_SPECULAR;
uniform vec3 FUSEE_L1_POSITION;
uniform vec3 FUSEE_L1_DIRECTION;

uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

void main()
{
    vec3 eyeVector = -fuVertex;
    vec3 L0 = vec3(vec4(fuVertex,1) - vec4(FUSEE_L0_POSITION, 1));
    fuL0HalfVector = eyeVector - L0;
    vec3 L1 = vec3(vec4(fuVertex,1) - vec4(FUSEE_L1_POSITION, 1));
    fuL1HalfVector = eyeVector - L1;
    vNormal = mat3(FUSEE_ITMV) * fuNormal;
    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";

        private const string PsMultiLight = @"
#ifdef GL_ES
precision highp float;
#endif
varying vec3 halfVector;
varying vec3 vNormal;
varying vec3 fuL0HalfVector;
varying vec3 fuL1HalfVector;

uniform vec4 FUSEE_L0_AMBIENT;
uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L0_SPECULAR;
uniform vec3 FUSEE_L0_POSITION;
uniform vec3 FUSEE_L0_DIRECTION;

uniform vec4 FUSEE_L1_AMBIENT;
uniform vec4 FUSEE_L1_DIFFUSE;
uniform vec4 FUSEE_L1_SPECULAR;
uniform vec3 FUSEE_L1_POSITION;
uniform vec3 FUSEE_L1_DIRECTION;

uniform float FUSEE_MAT_SHININESS;

void main()
{
    vec3 L0halfV = normalize(fuL0HalfVector);
    vec3 L1halfV = normalize(fuL1HalfVector);
    float frontMaterialAmbient = 0.2;
    float L0NdotHV = max(dot(normalize(vNormal), L0halfV), 0.0);
    float L1NdotHV = max(dot(normalize(vNormal), L1halfV), 0.0);
    vec4 L0colorAmbient = vec4(1,1,1,1);
    vec4 L0colorDiffuse = vec4(0,0,0,1);
    vec4 L0colorSpecular = vec4(0,0,0,1);
    vec4 L1colorAmbient = vec4(1,1,1,1);
    vec4 L1colorDiffuse = vec4(0,0,0,1);
    vec4 L1colorSpecular = vec4(0,0,0,1);
    vec4 color;

    L0colorAmbient = FUSEE_L0_AMBIENT * 0.4;
    float L0NdotL = max(dot(normalize(vNormal), normalize(FUSEE_L0_DIRECTION)), 0.0);
    if(L0NdotL > 0.0)
    {
        L0colorDiffuse += FUSEE_L0_DIFFUSE * L0NdotL;
        L0colorSpecular += FUSEE_L0_SPECULAR * pow(L0NdotHV, FUSEE_MAT_SHININESS);
    }
    L1colorAmbient *= FUSEE_L1_AMBIENT;
    float L1NdotL = max(dot(normalize(vNormal), normalize(FUSEE_L1_DIRECTION)), 0.0);
    if(L1NdotL > 0.0)
    {
        L1colorDiffuse += FUSEE_L1_DIFFUSE * L1NdotL;
        L1colorSpecular += FUSEE_L1_SPECULAR * pow(L1NdotHV, FUSEE_MAT_SHININESS);
    }
    vec4 colorAmbient = L0colorAmbient + L1colorAmbient * 0.4;
    vec4 colorDiffuse = L0colorDiffuse + L1colorDiffuse;
    vec4 colorSpecular= L0colorSpecular + L1colorSpecular;
    color = colorAmbient + colorAmbient + colorSpecular;
    color = colorAmbient * 0.4 + colorDiffuse * 0.6 + colorSpecular;
    gl_FragColor = color;
}";

        private const string VsSingleLight = @"
# version 120
/* Copies incoming vertex color without change.
    * Applies the transformation matrix to vertex position.
    */
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
        
//varying vec4 vColor;
varying vec3 vNormal;
varying vec3 halfVector;
        

uniform vec4 FUSEE_L0_AMBIENT;
uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L0_SPECULAR;
uniform vec3 FUSEE_L0_POSITION;
uniform vec3 FUSEE_L0_DIRECTION;


uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

void main()
{
    vec3 eyeVector = -fuVertex;
    vec3 L = vec3(vec4(fuVertex,1) - vec4(FUSEE_L0_POSITION, 1));
    halfVector = eyeVector - L;

    vNormal = mat3(FUSEE_ITMV) * fuNormal;
    

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

}";


        private const string PsSingleLight = @"

varying vec3 halfVector;
varying vec3 vNormal;
//varying vec4 ambient, diffuse;

uniform vec4 FUSEE_L0_AMBIENT;
uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L0_SPECULAR;
uniform vec3 FUSEE_L0_POSITION;
uniform vec3 FUSEE_L0_DIRECTION;


void main()
{

    vec3 halfV = normalize(halfVector);
    float frontMaterialAmbient = 0.2;
    float NdotHV = max(dot(normalize(vNormal), halfV), 0.0);

    float NdotL = max(dot(normalize(vNormal), normalize(FUSEE_L0_DIRECTION)), 0.0);
    vec4 color = FUSEE_L0_AMBIENT * 0.4;
    
    if(NdotL > 0.0)
    {
        color += FUSEE_L0_DIFFUSE * NdotL;
        color += FUSEE_L0_SPECULAR * pow(NdotHV, 64);
    }
    gl_FragColor = color;
}
    
";

        private const string Vs = @"
# version 120
/* Copies incoming vertex color without change.
    * Applies the transformation matrix to vertex position.
    */
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
        
varying vec4 vColor;
varying vec3 vNormal;
        

uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

void main()
{

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
    vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
    // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
    // vNormal = norm4.xyz;
    vNormal = mat3(FUSEE_ITMV) * fuNormal;
}";

        private const string PsSimple = @"
/* Copies incoming fragment color without change. */
#ifdef GL_ES
precision highp float;
#endif
        
//uniform vec4 vColor;
varying vec3 vNormal;
uniform sampler2D texture1;

void main()
{
    
    gl_FragColor = texture2D(texture1, vec2(vNormal)) * dot(vNormal, vec3(0, 0, 1)) ;
}";

        private const string PsColor = @"

//varying vec4 vColor;


void main()
{
    gl_FragColor = fuColor;
}
    
";

        private const string PsObjColor = @"

void main()
{
    gl_FragColor = fuColor;
}
    
";
        private const string VsFlatten = @"
attribute vec3 fuVertex;
uniform mat4 FUSEE_MVP;
void main()
{
    vec4 v = vec4(fuVertex,1.0);
	v.z = 0.0;
    gl_Position = FUSEE_MVP * v;
}
    
";
        private const string VsWeavy = @"
attribute vec3 fuVertex;
uniform mat4 FUSEE_MVP;
void main()
{
    vec4 v = vec4(fuVertex,1.0);
	v.z = sin(5.0*v.x )*0.25;
    gl_Position = FUSEE_MVP * v;
}
    
";
        private const string VsAnimation = @"
attribute vec3 fuVertex;
varying float vTimer;
varying vec4 vColor;
uniform mat4 FUSEE_MVP;
void main()
{
    vec4 v = vec4(fuVertex,1.0);
	v.z = sin(5.0*v.x + vTimer*0.01)*0.25;
    gl_Position = FUSEE_MVP * v;
}
    
";

        

        private const string PsPosLight = @"
/* Copies incoming fragment color without change. */
#ifdef GL_ES
precision highp float;
#endif
        
uniform vec4 FUSEE_L0_AMBIENT;
uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L0_SPECULAR;
uniform vec4 FUSEE_L0_POSITION;
uniform vec3 FUSEE_L0_DIRECTION;
varying vec4 vColor;
varying vec3 vNormal;


void main()
{
    gl_FragColor = FUSEE_L0_DIFFUSE * dot(normalize(vNormal), FUSEE_L0_DIRECTION);

}";

}
    
}
