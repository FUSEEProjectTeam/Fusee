#version 440 core

layout (points) in;
layout (points, max_vertices=6) out;

uniform mat4 LightSpaceMatrices[6];

out vec4 FragPos;

void main()
{
    for(int face = 0; face < 6; face++)
    {
        gl_Layer = face; // built-in variable that specifies to which face we render.
        
        FragPos = gl_in[0].gl_Position;
        gl_Position = LightSpaceMatrices[face] * FragPos;
        EmitVertex();
            
        EndPrimitive();
    }
}  