#version 440 core

in vec3 fuVertex;
in vec3 fuNormal;
in vec3 fuTangent;
in vec3 fuBitangent;
in vec2 fuUV;

out vec3 oNormal;
out vec3 oViewDir;
out vec2 oUV;
out mat3 TBN;

uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_MV;
uniform mat4 FUSEE_IV;
uniform mat4 FUSEE_ITMV;

void main() 
{
    vec3 viewPos = (FUSEE_MV * vec4(fuVertex, 1.0)).xyz;
    
    oViewDir = normalize(-viewPos);
    oNormal = normalize((FUSEE_ITMV * vec4(fuNormal, 1.0)).xyz);
    oUV = fuUV;

    vec3 T = normalize(vec3(FUSEE_ITMV * vec4(fuTangent.xyz, 0.0)));
    vec3 B = normalize(vec3(FUSEE_ITMV * vec4(fuBitangent.xyz, 0.0)));

    TBN = mat3(T, B, oNormal);

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}
