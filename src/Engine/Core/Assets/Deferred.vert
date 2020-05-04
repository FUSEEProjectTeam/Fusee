#version 300 es

in vec3 fuVertex;
out vec2 vUV;

void main() 
{
    vUV = fuVertex.xy * 2.0 * 0.5 + 0.5;
    gl_Position = vec4(fuVertex.xy * 2.0, 0.0 ,1.0);
}
