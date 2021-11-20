#version 300 es

in vec3 fuVertex;
in vec3 fuNormal;

in vec2 fuUV;

in vec4 fuColor;

uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_ITMV;
uniform mat4 FUSEE_IMV;
uniform mat4 FUSEE_MV;

uniform float uPointSize;

out vec2 vUV;
out vec4 vColor;

out vec3 vNormal;
out vec3 vViewPos;

vec4 CalculateVertexPosition()
{
	// convert normals
    vNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);

    // set viewPos  
    vViewPos = (FUSEE_MV * vec4(fuVertex, 1.0)).xyz;  

	// set color and uv
	vUV = fuUV;
	vColor = fuColor;

	// return position
    return FUSEE_MVP * vec4(fuVertex, 1.0);
}