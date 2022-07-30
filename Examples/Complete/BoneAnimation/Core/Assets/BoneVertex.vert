#version 330 core

in vec3 fuVertex;
in vec3 fuNormal;
in vec2 tex;
in ivec4 fuBoneIndex; 
in vec4 fuBoneWeight;

uniform vec3 bPos;
uniform mat4 FUSEE_MVP;
	
const int MAX_BONES = 100;
const int MAX_BONE_INFLUENCE = 4;
//const ivec4 fuBoneIndex = ivec4(0,0,0,0); 
//const vec4 fuBoneWeight= vec4(0,1,0,0);
uniform mat4 finalBonesMatrices[MAX_BONES];
	
out vec4 TexCoords;
	
void main()
{
    vec4 totalPosition = vec4(0.0f);
    for(int i = 0 ; i < MAX_BONE_INFLUENCE ; i++)
    {
        if(fuBoneIndex[i] == -1) 
            continue;
        if(fuBoneIndex[i] >=MAX_BONES) 
        {
            totalPosition = vec4(fuVertex,1.0f);
            break;
        }
        vec4 localPosition = finalBonesMatrices[fuBoneIndex[i]] * vec4(fuVertex,1.0f);
        totalPosition += localPosition * fuBoneWeight[i];
        vec3 localNormal = mat3(finalBonesMatrices[fuBoneIndex[i]]) * fuNormal;
    }
    gl_Position =  FUSEE_MVP * totalPosition;
    TexCoords = vec4(fuBoneIndex.xyz, 1);
}
//in vec3 aPos; // the position variable has attribute position 0
//  
//out vec4 vertexColor; // specify a color output to the fragment shader
//
//uniform vec3 bPos;
//uniform mat4 FUSEE_MVP;
//void main()
//{
//    gl_Position = FUSEE_MVP * vec4(aPos, 1.0); // see how we directly give a vec3 to vec4's constructor
//    vertexColor = vec4(bPos, 1.0); // set the output variable to a dark-red color
//}