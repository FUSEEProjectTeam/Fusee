#version 300 es
#extension GL_ARB_explicit_uniform_location : enable
precision highp float; 
layout (location = 0) out vec4 G_DEPTH;

void main()
{  
    float d = gl_FragCoord.z;                
    G_DEPTH = vec4(d, d, d, 1.0);
}