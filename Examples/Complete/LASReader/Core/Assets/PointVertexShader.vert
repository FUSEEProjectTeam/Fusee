#version 330 core

in vec3 fuVertex;
in vec3 fuNormal;

uniform vec2 ScreenParams;
uniform int PointSize;

uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_MV;
//uniform mat4 FUSEE_ITMV;
uniform mat4 FUSEE_M;
uniform mat4 FUSEE_P;

out vec3 vNormal;
out vec4 vModelPos;
out vec4 vViewPos;
out vec4 vWorldPos;
out vec4 vClipPos;
out float vWorldSpacePointRad;
out vec4 vColor;
//out float vViewNormal;

void main(void)
{
	
	vClipPos = FUSEE_MVP * vec4(fuVertex, 1.0);		
	vViewPos = FUSEE_MV * vec4(fuVertex, 1.0);
	vWorldPos = FUSEE_M * vec4(fuVertex, 1.0);
	vModelPos = vec4(fuVertex, 1.0);

	float fov = 2.0 * atan(1.0 / FUSEE_P[1][1]);
	float projFactor = ((1.0 / tan(fov / 2.0))/ -vViewPos.z)* ScreenParams.y / 2.0;
	vWorldSpacePointRad = PointSize / projFactor;

	//vViewNormal = mul(normalize(v.normal), FUSEE_ITMV);

	vNormal = fuNormal;

	gl_PointSize = PointSize;
	gl_Position = vClipPos;
}