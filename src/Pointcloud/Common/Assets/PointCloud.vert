#version 330 core 

uniform vec2 ScreenParams;
uniform int PointMode;
uniform int PointSize;
uniform int PointShape;
uniform mat4 FUSEE_ITMV;
uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_M;
uniform mat4 FUSEE_P;
uniform mat4 FUSEE_V;
uniform mat4 FUSEE_IV;
uniform vec2 ClipPlaneDist;
uniform float InitCamPosZ;

uniform sampler2D OctreeTex;
uniform int OctreeTexWidth;

uniform int OctantLevel;

out vec3 vNormal;
out vec4 vClipPos;
out vec4 vViewPos;
out vec4 vWorldPos;
out float vWorldSpacePointRad;
out vec3 vColor;

out float vSpacing;
out int vOctantLvl;

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

	//float pSize = round(PointSize / vClipPos.w);	 

	//clamp(pSize, 1, PointSize);

	//if(PointShape == 0 || PointShape == 1)
	//gl_PointSize = pSize;
	//	else

	
	float minPtSize = 1;
	float ptSize = minPtSize;
	float maxPtSize = 100;


	vec3 camPos = vec3(FUSEE_IV[3]);
	vec3 cameraToPoint = vec3(vWorldPos) - camPos;

	float viewDepth = dot(cameraToPoint, camPos);
	
	switch(PointMode)
	{
		default:
		case 0:
		{		
			ptSize = PointSize;			
			break;
		}
		case 1:
		{ 
			//In this scenario the PointSize is the given point radius in world space - the point size in pixel will shrink if the camera moves farther away

			//Formula that relates to the given PointSie (in px) and the camera position
			//ptSize = (PointSize / vClipPos.w) * InitCamPosZ;
			
			//Formula as given (without division at the end) in Schuetz' thesis - produces points that are to big without the division!
			float worldPtSize = PointSize;
			ptSize = ((ScreenParams.y / 2.0) * (worldPtSize / ( slope * vClipPos.w))) / InitCamPosZ;
			break;
		}
		case 2:
		{	
			vOctantLvl = OctantLevel;

			float spacing = pow(0.5, OctantLevel);			
			vSpacing = spacing;
			
			float worldPtSize = PointSize * spacing;
			ptSize = ((ScreenParams.y / 2.0) * (worldPtSize / ( slope * vClipPos.w))) / InitCamPosZ;
			break;
		}
		case 3:
		{	
			ptSize = PointSize;			
			break;
		}	
	}

	if(ptSize < minPtSize)
		ptSize = minPtSize;
	else if(ptSize > maxPtSize)
		ptSize = maxPtSize;


//	ptSize = max(minPtSize, ptSize);
//	ptSize = min(maxPtSize, ptSize);	

	gl_PointSize = ptSize;
	gl_Position = vClipPos;	
}