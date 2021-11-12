#version 460 core

in vec3 fuVertex;
out vec2 vUv;

void main() 
{
    vUv = fuVertex.xy * 2.0 * 0.5 + 0.5;
    gl_Position = vec4(fuVertex.xy * 2.0, 0.0 ,1.0);
}
