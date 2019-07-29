#version 330 core

#extension GL_EXT_gpu_shader4 : enable

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

uniform usampler2D OctreeTex;
uniform int OctreeTexWidth;

uniform int OctantLevel;
uniform vec3 OctreeRootCenter;
uniform float OctreeRootLength;

out vec3 vNormal;
out vec4 vClipPos;
out vec4 vViewPos;
out vec4 vWorldPos;
out float vWorldSpacePointRad;
out vec3 vColor;

in vec3 fuVertex;
in vec3 fuNormal;
in vec3 fuColor;


/**
 * number of 1-bits up to exclusive index position
 * number is treated as if it were an integer in the range 0-255
 * https://github.com/potree/potree/blob/develop/src/materials/shaders/pointcloud.vs
 */
int offsetFirstToGivenChild(uint uNumber, int index)
{
	int numOnes = 0;
	int tmp = 128;
	int number = int(uNumber);

	for(int i = 7; i >= 0; i--){
		
		if(number >= tmp){
			number = number - tmp;

			if(i <= index){
				numOnes++;
			}
		}
		
		tmp = tmp / 2;
	}

	return numOnes -1;
}

/**
 * Checks whether a certain bit (given by index) is set inside
 * a byte (given by number).
 * @param number [0, 255] 8 bits representing children
 * @param index [0, 7] index of bit to check
 */
bool isBitSet(uint uNumber, int index)
{    
	int number = int(uNumber);
	float exponentiation = pow(2.0, index);
	
	int times = int(number / exponentiation);
	return mod(times, 2) != 0;
}


/**
 * Determines the center position of a child node.
 * Inverse of getChildIndex().
 */
vec3 getChildNodeCenter(int childIndex, vec3 parentNodeCenter, float parentSideLength)
{
	bool width, height, depth;

	switch(childIndex)
	{
		case 0:
			width = false; height = false; depth = false;
			break;
		case 1:
			width = true; height = false; depth = false;
			break;
		case 2:
			width = false; height = false; depth = true;
			break;
		case 3:
			width = true; height = false; depth = true;
			break;
		case 4:
			width = false; height = true; depth = false;
			break;
		case 5:
			width = true; height = true; depth = false;
			break;
		case 6:
			width = false; height = true; depth = true;
			break;
		case 7:
			width = true; height = true; depth = true;
			break;
	}
	
	vec3 childCenter = parentNodeCenter;
	float summand = parentSideLength / 4.0;
	
	childCenter.x += width ? summand : -summand;
	childCenter.y += height ? summand : -summand;
	childCenter.z += depth ? summand : -summand;	
	
	return childCenter;
}

/**
 * Computes the child index the current point has inside 
 * a certain node. Compare to OctreeNode.cs
 */
int getChildIndex(vec3 nodeCenter)
{
	vec3 diffToCenter = fuVertex - nodeCenter;
	
	bool width = diffToCenter.x >= 0.0;
	bool height = diffToCenter.y >= 0.0;
	bool depth = diffToCenter.z >= 0.0;
	
	int index = 0;

	if (!width && !height && !depth)
		index = 0;
	else if (width && !height && !depth)
		index = 1;
	else if (!width && !height && depth)
		index = 2;
	else if (width && !height && depth)
		index = 3;
	else if (!width && height && !depth)
		index = 4;
	else if (width && height && !depth)
		index = 5;
	else if (!width && height && depth)
		index = 6;
	else if (width && height && depth)
		index = 7;

	return index;
}

int getLevelOfDetail()
{
	int lod = 0;

	float pixelSize = 1.0 / OctreeTexWidth;
	float texPosition = 0;
	int pxPosition = 0;
	vec3 centerPos = OctreeRootCenter;
	float sideLength = OctreeRootLength;

	//0. Get node info
	uvec4 nodeInfo = texture2D(OctreeTex, vec2(texPosition, 0.5));	

	//1. get the child node the point falls into
	int childIndexPointFallsInto = getChildIndex(centerPos);

	//2. check if the child from step 1. is visible == exists in the texture
	bool childExists = isBitSet(nodeInfo.b, childIndexPointFallsInto);  //points needs to fall into one of the children of the root	

	//3. jump to child
	pxPosition += int(nodeInfo.g);
	
	while(childExists)
	{	
		lod++;

		//3. jump to child and get the child info
		pxPosition += int(nodeInfo.g) + offsetFirstToGivenChild(nodeInfo.b, childIndexPointFallsInto);
		texPosition = pxPosition * pixelSize; //jump to first child of the node
				
		//0. Get node info
		nodeInfo = texture2D(OctreeTex, vec2(texPosition, 0.5));		

		centerPos = getChildNodeCenter(childIndexPointFallsInto, centerPos, sideLength);
		sideLength = sideLength/2;

		//1. get the child node the point falls into
		childIndexPointFallsInto = getChildIndex(centerPos);			

		//2. check if the child from step 1. is visible == exists in the texture
		childExists = isBitSet(nodeInfo.b, childIndexPointFallsInto);
	}

	return lod;
}


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

	float depth = length(cameraToPoint);

	float viewDepth = dot(cameraToPoint, camPos);
	
	switch(PointMode)
	{
		// Fixed pixel size
		default:
		case 0:
		{		
			ptSize = PointSize;			
			break;
		}
		//Fixed world size
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
		//Octree level-dependent
		case 2:
		{	
			float spacing = pow(0.5, OctantLevel);
			
			float worldPtSize = PointSize * spacing;
			ptSize = ((ScreenParams.y / 2.0) * (worldPtSize / ( slope * vClipPos.w))) / InitCamPosZ;
			break;
		}
		//level of detail
		case 3:
		{			
			float worldPtSize = PointSize / (pow(2, getLevelOfDetail()));
			ptSize = ((ScreenParams.y / 2.0) * (worldPtSize / ( slope * vClipPos.w))) / InitCamPosZ;
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