#version 300 es
precision highp float;
                
uniform mat4 LightSpaceMatrix;
uniform mat4 FUSEE_M;              
in vec3 fuVertex; 

void main() 
{                
    gl_Position = LightSpaceMatrix * FUSEE_M * vec4(fuVertex, 1.0);
}