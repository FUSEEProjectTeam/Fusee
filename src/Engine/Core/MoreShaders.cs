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
            if (name == "color")
            {
                ShaderProgram spColor = rc.CreateShader(VsMultiLight2, PsMultiLight2);
                return spColor;
            }

            if (name == "chess")
            {
                ShaderProgram spChess = rc.CreateShader(VSChess, PSChess);
                return spChess;
            }
            if (name == "simple")
            {
                ShaderProgram simple = rc.CreateShader(Vs, Ps);
                return simple;
            }
            ShaderProgram spOriginal = rc.CreateShader(VSColor, PSColor);
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

uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

uniform vec3 FUSEE_L0_POSITION;
varying float LightIntensity;
varying float SpecIntensity;

vec3  Normal;


void main()
{
  gl_Position     = FUSEE_MVP * vec4(fuVertex, 1.0);
  vec3 pos        = vec3(FUSEE_MVP * fuVertex);
  Normal          = normalize(FUSEE_ITMV * fuNormal);
  vec3 posToLight = normalize(LightPos - pos);
  LightIntensity  = max(dot(posToLight, Normal), 0.0);

  //Specular by Randi Rost: www.3dshaders.com
  vec3 reflectVec = reflect(-posToLight, Normal);
  vec3 viewVec    = normalize(-pos);
  SpecIntensity   = max(dot(reflectVec, viewVec), 0.0);

}";


        private const string PsMultiLight2 = @"
#ifdef GL_ES
precision highp float;
#endif

varying float LightIntensity;
varying float SpecIntensity;
uniform vec3 FUSEE_L0_AMBIENT;
uniform float SpecularLevel;
uniform float SpecularSize;

void main()
{
    
    	vec3 thisColor = Color * LightIntensity;
	thisColor += max(0.0, SpecularLevel * pow(SpecIntensity, SpecularSize));
  gl_FragColor = vec4(thisColor, 1.0);
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

uniform vec3 darkColor;
uniform vec3 brightColor;
uniform float chessSize;
uniform float smoothFactor;

varying float LightIntensity;

varying vec3  OBposition;


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



        private const string VSColor = @"
# version 120
/* Copies incoming vertex color without change.
    * Applies the transformation matrix to vertex position.
    */
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

void main()
{
    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";

        private const string PSColor = @"
/* Copies incoming fragment color without change. */
#ifdef GL_ES
precision highp float;
#endif
        
uniform vec4 FUSEE_MAT_AMBIENT = vec4(0,0,1,1);

void main()
{    
    gl_FragColor = FUSEE_MAT_AMBIENT;
}";

                private const string Vs = @"
# version 120
/* Copies incoming vertex color without change.
    * Applies the transformation matrix to vertex position.
    */
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
       
uniform mat4 FUSEE_MVP;  //model view projection matrix
uniform mat4 FUSEE_ITMV; //inverte transformierte model view matrix

uniform vec4 FUSEE_L0_AMBIENT;
uniform vec4 FUSEE_L0_DIFFUSE;
uniform vec4 FUSEE_L0_SPECULAR;
uniform vec3 FUSEE_L0_DIRECTION;
uniform vec4 FUSEE_MAT_AMBIENT;
uniform vec4 FUSEE_MAT_DIFFUSE;
uniform vec4 FUSEE_MAT_SPECULAR;
uniform vec4 FUSEE_MAT_SHININESS;


varying vec4 ambient;
varying vec4 diffuse;
varying vec4 specular;

void main()
{
    ambient = FUSEE_L0_AMBIENT * FUSEE_MAT_AMBIENT;
    diffuse = FUSEE_MAT_DIFFUSE * dot(fuNormal,FUSEE_L0_DIRECTION) * FUSEE_L0_DIFFUSE;
    specular = FUSEE_L0_SPECULAR * FUSEE_MAT_SPECULAR * 0;

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}";

        private const string Ps = @"
/* Copies incoming fragment color without change. */
#ifdef GL_ES
precision highp float;
#endif
        
varying vec4 ambient;
varying vec4 diffuse;
varying vec4 specular;

void main()
{    
    vec4 color = ambient + diffuse + specular;
    gl_FragColor = color;
}";
    }
}