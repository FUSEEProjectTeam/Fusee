#version 300 es

precision highp float;

uniform vec2 ScreenParams;
uniform int PointSizeMode;
uniform int PointSize;

uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_MV;
uniform mat4 FUSEE_P;
uniform float InitCamPosZ;

out vec4 vClipPos;
out vec4 vViewPos;
out vec4 vWorldPos;
out vec3 vColor;
out float vWorldSpacePointRad;

in vec3 fuVertex;
in vec3 fuColor;

void main(void)
{	
	float pointSizeDivisor = 100.0;
	vColor = fuColor;

	vClipPos = FUSEE_MVP * vec4(fuVertex.xyz, 1.0);
	vViewPos = FUSEE_MV * vec4(fuVertex.xyz, 1.0);

	float fov = 2.0 * atan(1.0 / FUSEE_P[1][1]);
	float slope = tan(fov / 2.0);float projFactor = ((1.0 / slope)/ -vViewPos.z)* ScreenParams.y / 2.0;
	vWorldSpacePointRad = float(PointSize) / projFactor;

	float minPtSize = 1.0;
	float ptSize = minPtSize;
	float maxPtSize = 100.0;

	switch(PointSizeMode)
	{
		// Fixed pixel size
		default:
		case 0:
		{		
			ptSize = float(PointSize);
			break;
		}
		//Fixed world size
		case 1:
		{ 
			//In this scenario the PointSize is the given point radius in world space - the point size in pixel will shrink if the camera moves farther away

			//Formula that relates to the given PointSie (in px) and the camera position
			//ptSize = (PointSize / vClipPos.w) * InitCamPosZ;
			
			//Formula as given (without division at the end) in Schuetz' thesis - produces points that are to big without the division!
			ptSize = ((ScreenParams.y / 2.0) * (float(PointSize) / ( slope * vViewPos.z))) / pointSizeDivisor;
			break;
		}
	}

	if(ptSize < minPtSize)
		ptSize = minPtSize;
	else if(ptSize > maxPtSize)
		ptSize = maxPtSize;

	gl_PointSize = ptSize;
	gl_Position = vClipPos;	
}