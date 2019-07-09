#version 330 core 

uniform vec2 ScreenParams;
uniform int PointMode;
uniform float ScreenProjectedOctantSize;
uniform int PointSize;
uniform int gridCellRes;
uniform int PointShape;
uniform mat4 FUSEE_ITMV;
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
	//vIntensity = intensity;

	vClipPos = FUSEE_MVP * vec4(fuVertex.xyz, 1.0);		
	vViewPos = FUSEE_V * FUSEE_M * vec4(fuVertex.xyz, 1.0);
	vWorldPos = FUSEE_M * vec4(fuVertex.xyz, 1.0);

	float fov = 2.0 * atan(1.0 / FUSEE_P[1][1]);
	float slope = tan(fov / 2.0);
	float projFactor = ((1.0 / slope)/ -vViewPos.z)* ScreenParams.y / 2.0;
	vWorldSpacePointRad = PointSize / projFactor;	

	vNormal = (FUSEE_ITMV * vec4(fuNormal, 0.0)).xyz; //FUSEE_ITMV - normal matrix for transformation into world space;

	float pSize = round(PointSize / vClipPos.w);	 

	clamp(pSize, 1, PointSize);

//	if(PointShape == 0 || PointShape == 1)
//		gl_PointSize = pSize;
//	else

	
	switch (PointMode)
	{
		case 0: // default = fixed, user-given point size in px
		default:
			gl_PointSize = 2;
		case 1: 
			gl_PointSize = 20;
		case 2:
			gl_PointSize = 200;
		
	}

	gl_Position = vClipPos;	
}