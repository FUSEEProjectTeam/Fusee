#version 300 es   

in vec3 fuVertex;
in vec3 fuNormal;
in vec2 fuUV;

uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_ITMV;

out vec2 vUV;
out vec3 vMVNormals;
	
void main(void) 
{
	vUV = fuUV;	
	vMVNormals = normalize(mat3(FUSEE_ITMV) * fuNormal);
	
	gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}