#version 330 core

uniform mat4 FUSEE_P;
uniform mat4 FUSEE_V;
uniform mat4 FUSEE_M;

//SSAO
uniform vec3[32] SSAOKernel;
uniform sampler2D NoiseTex;
uniform int CalcSSAO;
uniform float SSAOStrength;

uniform int PointShape;
uniform int ColorMode;
uniform int Lighting;
uniform float SpecularStrength;
uniform float Shininess;
uniform vec4 Color;
uniform vec4 SpecularColor;
uniform vec2 ScreenParams;
uniform vec2 ClipPlaneDist;

uniform float EDLStrength;
uniform int EDLNeighbourPixels;
uniform sampler2D DepthTex;

in vec4 vColor;
in vec3 vNormal;
in vec4 vViewPos;
in vec4 vClipPos;
in vec4 vWorldPos;
in float vWorldSpacePointRad;
in vec4 vIntensity;
out vec4 oColor;

float LinearizeDepth(float depth) 
{
	float near = ClipPlaneDist.x;
	float far = ClipPlaneDist.y;

    float z = depth * 2.0 - 1.0; // back to NDC 
	return  (2.0 * near * far) / (far + near - z * (far - near));
}

//https://www.cg.tuwien.ac.at/research/publications/2016/SCHUETZ-2016-POT/SCHUETZ-2016-POT-thesis.pdf
float EDLResponse(float pixelSize, float linearDepth, vec2 thisUv)
{
	vec2 pxToUv = 1.0/ScreenParams;

	vec2 offsetsToNeighbours[8] = vec2[8]
	(
		pixelSize * vec2( pxToUv.x, -pxToUv.y ), 	// right bottom
		pixelSize * vec2( pxToUv.x, 0 ), 			// right middle
		pixelSize * vec2( pxToUv.x, pxToUv.y ),		// right top
		pixelSize * vec2( 0, -pxToUv.y ),			// middle bottom
		pixelSize * vec2( 0, pxToUv.y ), 			// middle top
		pixelSize * vec2( -pxToUv.x, -pxToUv.y ),	// left bottom
		pixelSize * vec2( -pxToUv.x, 0 ), 			// left middle
		pixelSize * vec2( -pxToUv.x, pxToUv.y ) 	// left top
	);
	
	float response = 0.0;	
	int neighbourCount = 0;

	for(int i = 0; i < 8; i++)
	{
		vec2 neighbourUv = thisUv + offsetsToNeighbours[i];
		float neighbourDepth = texture(DepthTex, neighbourUv).x;
		neighbourDepth = LinearizeDepth(neighbourDepth);

		if(neighbourDepth == 0.0)
			neighbourDepth = 1.0 / 0.0; //infinity!
			
		response += max(0,log2(linearDepth) - log2(neighbourDepth));
		neighbourCount += 1;
	}	

	if(neighbourCount == 0)
		return 1.0;

	return response /= neighbourCount;	
}

float EDLShadingFactor(float edlStrength, int pixelSize, float linearDepth, vec2 thisUv)
{
	float response = EDLResponse(pixelSize, linearDepth, thisUv);

	if(linearDepth == 0.0 && response == 0.0)	
		discard;

	if(response > 1)
		response = 1;

	return exp(-response * 300 * edlStrength);
}

vec4 GetDiffuseReflection(vec3 normalDir, vec3 lightDir, vec3 lightColor, vec3 ambient)
{
	float intensityDiff = max(dot(normalDir, lightDir), 0.0);
	vec3 diffuse = intensityDiff * lightColor;

	if(CalcSSAO == 1.0)
		return vec4((diffuse + ambient) *  oColor.rgb, 1); //Diffuse component
	else
		return vec4(diffuse *  oColor.rgb, 1);
}

vec3 ViewNormalFromDepth(float depth, vec2 texcoords) 
{
  
  const vec2 offset1 = vec2(0.0,0.001);
  const vec2 offset2 = vec2(0.001,0.0);
  
  float depth1 = texture(DepthTex, texcoords + offset1).r;
  float depth2 = texture(DepthTex, texcoords + offset2).r;
  
  vec3 p1 = vec3(offset1, depth1 - depth);
  vec3 p2 = vec3(offset2, depth2 - depth);
  
  vec3 normal = cross(p1, p2);
  normal.z = -normal.z;
  
  return normalize(normal);
}

void main(void)
{	
	vec2 distanceVector = (2 * gl_PointCoord) - 1; //[-1,1]
	vec4 position;
	float weight;

	vec4 col;

	switch (PointShape)
	{
	case 0: // default = square
	default:
		gl_FragDepth = gl_FragCoord.z;
		break;
	case 1: // circle						

		float distanceFromCenter = length(2.0 * gl_PointCoord - 1.0);
			
		if(distanceFromCenter > 1.0)
			discard;
			
		gl_FragDepth = gl_FragCoord.z;

		break;
	case 2: //paraboloid

		weight = 1.0 - (pow(distanceVector.x, 2.0) + pow(distanceVector.y, 2.0)); //paraboloid weight function

		position = vViewPos;
		position.z += weight * vWorldSpacePointRad;
		position = FUSEE_P * position;
		position = position / position.w;
		gl_FragDepth = (position.z + 1.0) / 2.0;

		break;
	case 3: //cone

		//[-1, 1]
		weight = 1.0 - length(distanceVector);

		position = vViewPos;
		position.z += weight * vWorldSpacePointRad;
		position = FUSEE_P * position;
		position = position / position.w;
		gl_FragDepth = (position.z + 1.0) / 2.0;

		break;

	case 4: //sphere

		//prevent sqrt(x < 0) - z values can (and should, in this case) become negative 
		float zwerg = 1.0 - (pow(distanceVector.x, 2.0) + pow(distanceVector.y, 2.0));
		if (zwerg < 0)
			weight = -1;
		else
			weight = sqrt(zwerg);

		position = vViewPos;
		position.z += weight * vWorldSpacePointRad;
		position = FUSEE_P * position;
		position = position / position.w;
		gl_FragDepth = (position.z + 1.0) / 2.0;

		break;
	}

	switch (ColorMode)
	{
		case 0: // default = point cloud rgb
			default:
			oColor = vColor; //vColor = vertex color
		break;
		case 1:
			oColor = Color; //one color for all points (uniform)
		break;
		case 2:
			oColor = vec4(vNormal, 1.0);
		break;
		case 3:
			oColor = vec4(weight, weight, weight, 1);
			break;
		case 4:
			oColor = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);
			break;
		case 5:
			oColor = vIntensity;
		break;
	}

	// SSAO - kind of depth only. 
	// See: 
	// https://learnopengl.com/Advanced-Lighting/SSAO
	// http://john-chapman-graphics.blogspot.com/2013/01/ssao-tutorial.html 
	// http://theorangeduck.com/page/pure-depth-ssao

	float radius = 10;
	float occlusion = 0.0;
	float bias = 0.025;
	
	vec2 uv = vec2(gl_FragCoord.x/ScreenParams.x, gl_FragCoord.y/ScreenParams.y);
	float z = texture2D(DepthTex, uv).x;

	vec3 ambient = vec3(1,1,1);
	vec3 viewNormal = ViewNormalFromDepth(z, uv);

	if(CalcSSAO == 1)
	{
		vec2 tiles = vec2(ScreenParams.x/4.0, ScreenParams.y/4.0);
		//vec3 viewNormal = ViewNormalFromDepth(z, uv);
		
		vec3 rvec = texture(NoiseTex, uv * tiles ).xyz;
		vec3 tangent = normalize(rvec - viewNormal * dot(rvec, viewNormal));
		vec3 bitangent = cross(viewNormal, tangent);
		mat3 tbn = mat3(tangent, bitangent, viewNormal);

		int kernelLength = 32;

		for (int i = 0; i < kernelLength; ++i) 
		{

			// get sample position:
			vec3 sampleVal = tbn * SSAOKernel[i];
			sampleVal = sampleVal * radius + vViewPos.xyz;
  
			// project sample position:
			vec4 offset = vec4(sampleVal, 1.0);
			offset = FUSEE_P * offset;		
			offset.xy /= offset.w;
			offset.xy = offset.xy * 0.5 + 0.5;
  
			// get sample depth:
			float sampleDepth = texture(DepthTex, offset.xy).z;
			sampleDepth = LinearizeDepth(sampleDepth);
  
			// range check & accumulate:
			float rangeCheck = smoothstep(0.0, 1.0, radius / abs(vViewPos.z - sampleDepth));
			occlusion += (sampleDepth >= sampleVal.z + bias ? 1.0 : 0.0) * rangeCheck;
		}

		occlusion = 1-((occlusion / kernelLength) * SSAOStrength);
		ambient = vec3(occlusion, occlusion, occlusion);
	}
	//-------------------------------
	vec3 normalDir = vec3(1,1,1);
	vec3 lightDir = vec3(1,1,1);
	vec3 halfwayDir = vec3(1,1,1);

	vec3 lightColor = vec3(1,1,1);

	if(Lighting == 2 || Lighting == 3)
	{
		normalDir = normalize((inverse(inverse(transpose( FUSEE_M))) * vec4(viewNormal,1.0)).xyz);
	
//		mat4 invMv = inverse(FUSEE_V * FUSEE_M);
//		vec3 worldSpaceCameraPos = invMv[3].xyz;
//		vec3 worldSpaceLightPos = worldSpaceCameraPos;

		vec3 viewDir = normalize( -vViewPos.xyz);
		vec3 lightDir = normalize( vec3(0, 0,-.0));
		vec3 halfwayDir = reflect(-lightDir, normalDir);
	}
	
	vec4 specularReflection = vec4(0, 0, 0, 1);	

	switch (Lighting)
	{
		default:
		case 0: // default = unlit		
		{			
			break;
		}
		case 1:
		{			
			float linearDepth = LinearizeDepth(z);

			if(linearDepth > 0.1)
				oColor.xyz *= EDLShadingFactor(EDLStrength, EDLNeighbourPixels, linearDepth, uv);
			
			if(CalcSSAO == 1)
				oColor.xyz *= ambient;

			break;
		}
		case 2: //diffuse
		{	
			
			oColor = GetDiffuseReflection(viewNormal, lightDir, lightColor, ambient);

			break;
		}

		case 3: //blinn phong
		{
			float specularAmmount = pow(max(dot(normalDir, halfwayDir), 0.0), Shininess);
			specularReflection = SpecularColor * pow(specularAmmount, Shininess);
			vec4 diffuseReflection = GetDiffuseReflection(normalDir, lightDir, lightColor, ambient);
			oColor = diffuseReflection + (SpecularStrength * specularReflection);

			break;
		}		
		case 4: //ambient only
		{	
			if(CalcSSAO == 1)			
				oColor.xyz *= ambient;

			break;
		}	

	}

}

