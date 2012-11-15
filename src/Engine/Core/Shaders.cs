using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.Engine
{
    public static class Shaders

    {

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
        
uniform vec4 vColor;
varying vec3 vNormal;

void main()
{
    gl_FragColor = vColor * dot(vNormal, vec3(0, 0, 1));
}";

        private const string PsColor = @"

uniform vec4 vColor;

void main()
{
    gl_FragColor = vColor;
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

     

    public static ShaderProgram GetShader(string name,RenderContext rc)
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
        if(name == "simple")
        {

            ShaderProgram spSimple = rc.CreateShader(Vs, PsSimple);
            return spSimple;
        }
        if (name == "color")
        {

            ShaderProgram spColor = rc.CreateShader(Vs, PsColor);
            return spColor;
        }
        ShaderProgram spOriginal = rc.CreateShader(Vs, PsSimple);
        return spOriginal;
    }
}
    
}
