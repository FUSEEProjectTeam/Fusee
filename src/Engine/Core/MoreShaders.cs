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

                ShaderProgram spSimple = rc.CreateShader(Vs, PsSimple);
                return spSimple;
            }

            if (name == "chess")
            {
                ShaderProgram spLight = rc.CreateShader(VSChess, PSChess);
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

        private const string VSChess = @"
//  # version 120 // not working with GL_ES

attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;


uniform vec3 darkColor;
uniform vec3 brightColor;
uniform float chessSize;
uniform float smoothFactor;
uniform vec3 FUSEE_L0_POSITION;
vec3  Normal;
varying float LightIntensity;
varying vec3  OBposition;

uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

void main(void) {
  Normal         = mat3(FUSEE_ITMV) * fuNormal;
  vec4 pos       = FUSEE_MVP * vec4(fuVertex,1);
  LightIntensity = max(dot(normalize(FUSEE_L0_POSITION - pos.xyz), Normal), 0.0);
  //end light

  OBposition     = fuVertex.xyz;
  gl_Position    = FUSEE_MVP * vec4(fuVertex, 1.0);

}";

        private const string PSChess = @"
#ifdef GL_ES
precision highp float;
#endif

varying float LightIntensity;

varying vec3  OBposition;
uniform vec3 darkColor;
uniform vec3 brightColor;
uniform float chessSize;
uniform float smoothFactor;

void main(){
  float halfChess = chessSize / 2.0;
  vec3 position = mod(OBposition,chessSize);
  position = abs(position-halfChess)*2.0;
  vec3 useColor = smoothstep(halfChess-smoothFactor,halfChess+smoothFactor,position);
  useColor -= smoothstep(halfChess-smoothFactor,halfChess+smoothFactor,chessSize-position);
  
  //float mixVal = clamp(useColor.x * useColor.y * useColor.z,0.0,1.0);
  float mixVal = max(useColor.x * useColor.y * useColor.z,0.0);
  vec3 color   = mix(darkColor, brightColor, mixVal);

  gl_FragColor = vec4(color * LightIntensity, 1.0);
}";



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


    }
}