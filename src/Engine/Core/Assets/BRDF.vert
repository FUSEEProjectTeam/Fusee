#version 300 es

in vec3 fuVertex;
in vec3 fuNormal;

out vec3 oNormal;
out vec3 oViewDir;

uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_MV;
uniform mat4 FUSEE_IV;
uniform mat4 FUSEE_ITMV;

void main() 
{
    vec3 viewPos = (FUSEE_MV * vec4(fuVertex, 1.0)).xyz;
    
    oViewDir = normalize(-viewPos);
    oNormal = normalize((FUSEE_ITMV * vec4(fuNormal, 1.0)).xyz);

    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}
