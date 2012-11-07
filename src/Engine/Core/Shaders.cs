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
uniform mat4 FUSEE_ITMV; //inver transvormierte model view matrix

void main()
{
    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
    // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
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

     

    public static ShaderProgram GetShader(string name,RenderContext RC)
    {
        if (name == "")
        {
            ShaderProgram sp2 = RC.CreateShader(Vs, PsSimple);
            return sp2;
        }
        ShaderProgram sp = RC.CreateShader(Vs, PsSimple);
        return sp;
    }
}
    
}
