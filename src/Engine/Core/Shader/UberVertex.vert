#version 300 es

in vec3 fuVertex;
in vec3 fuNormal;
in vec3 fuUV;
in vec3 fuTangent;
in vec3 fuBiTangent;

uniform mat4 FUSEE_M;
uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_IMV;
uniform mat4 FUSEE_ITMV;
 
out vec3 vNormal;
out vec3 vUV;
out vec3 vViewDir;
out vec4 vLDir;
out vec3 vTangent;
out vec3 vBiTangent;


void main()
{

	vec3 viewPos = FUSEE_IMV[3].xyz;


	// pass vars to fragment shader
	vNormal = normalize(fuNormal);
	vUV = fuUV;
	vViewDir = normalize(viewPos - fuVertex);
	vLDir = FUSEE_IMV *  vec4(0, 0, -1, 0);
	vTangent = fuTangent;
	vBiTangent = fuBiTangent;


	gl_Position = FUSEE_MVP * vec4(fuVertex,1.0);

}