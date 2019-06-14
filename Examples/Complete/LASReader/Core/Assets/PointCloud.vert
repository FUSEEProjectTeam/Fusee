#version 330 core 

uniform vec2 ScreenParams;
uniform int PointSize;
uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_M;
uniform mat4 FUSEE_P;
uniform mat4 FUSEE_V;
uniform vec2 ClipPlaneDist;

out vec3 vNormal;
out vec4 vClipPos;
out vec4 vViewPos;
out vec4 vWorldPos;
out float vWorldSpacePointRad;
out vec3 vColor;

in vec3 fuVertex;
in vec3 fuNormal;
in vec3 fuColor;

void main(void)
{	
	vColor = fuColor;

	vClipPos = FUSEE_MVP * vec4(fuVertex.xyz, 1.0);		
	vViewPos = FUSEE_V * FUSEE_M * vec4(fuVertex.xyz, 1.0);
	vWorldPos = FUSEE_M * vec4(fuVertex.xyz, 1.0);

	float fov = 2.0 * atan(1.0 / FUSEE_P[1][1]);
	float projFactor = ((1.0 / tan(fov / 2.0))/ -vViewPos.z)* ScreenParams.y / 2.0;
	vWorldSpacePointRad = PointSize / projFactor;	

	vNormal = fuNormal;

	float pSize = round(PointSize / vClipPos.w);

	if(pSize < 1)
		pSize = 1;
	if(pSize > PointSize)
		pSize = PointSize;

	gl_PointSize = pSize; //OpenGL only
	gl_Position = vClipPos;	
}