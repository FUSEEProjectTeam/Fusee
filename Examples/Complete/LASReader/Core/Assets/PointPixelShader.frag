#version 330 core

uniform mat4 FUSEE_P;

uniform int PointShape;
uniform int ColorMode;
uniform int Lighting;
uniform float SpecularStrength;
uniform float Shininess;
uniform vec4 Color;
uniform vec4 SecularColor;

in vec3 vNormal;
in vec4 vClipPos;
in vec4 vViewPos;
in vec4 vWorldPos;
in float vWorldSpacePointRad;
in vec4 vColor;

out vec4 oColor;

float EDLResponse(int pixelSize)
{
	vec2 offsetsToNeighbours[8] = vec2[8]
	(
		pixelSize * vec2( 1, -1 ), 	// right bottom
		pixelSize * vec2( 1, 0 ), 	// right middle
		pixelSize * vec2( 1, 1 ),	// right top
		pixelSize * vec2( 0, -1 ),	// middle bottom
		pixelSize * vec2( 0, 1 ), 	// middle top
		pixelSize * vec2( -1, -1 ),	// left bottom
		pixelSize * vec2( -1, 0 ), 	// left middle
		pixelSize * vec2( -1, 1 ) 	// left top
	);
	
	float response = 0.0;
	float thisDepth = 0.0; //this fragments depth
	int neighbourCount = 0;

	for(int i = 0; i < 8; i++)
	{
		vec2 neighbourUv = vec2(0,0);//vUV + offsetsToNeighbours[i]
		float neighbourDepth = 0;//texture(vDepth, neighbourUv).x;

		if(neighbourDepth == 0.0)
			neighbourDepth = 100000000;
			
		//neighborDepth = getHyperbolicDepth(neighborDepth);
		
		if(neighbourDepth > thisDepth) // only count neighbors which in are in front of pixel
			continue;

		response += max(0,log2(thisDepth) - log2(neighbourDepth));
		neighbourCount += 1;
	}

	response /= neighbourCount;

	if(neighbourCount == 0)
		response = 1.0;

	return response;
	
}

float EDLShadingFactor(int edlStrength, int pixelSize)
{
	float response = EDLResponse(pixelSize);
	return exp(-response * 300 * edlStrength);
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
			oColor = vec4(gl_FragDepth, gl_FragDepth, gl_FragDepth, 1.0);		
		
	}

	vec3 normalDir = normalize(vNormal);
	
	//vec3 worldSpaceCameraPos = invView translation column

	vec3 viewDir = normalize(-vViewPos.xyz);
	vec3 lightDir = normalize(vWorldPos.xyz - vViewPos.xyz);
	vec3 halfwayDir = normalize(lightDir + viewDir);

	vec4 diffuseReflection = vec4((1.0,1.0,1.0) * oColor.rgb * max(0.0, dot(normalDir, lightDir)),1); //Diffuse component

	float intensityDiff = max(0.0, dot(normalDir, lightDir));

	vec4 specularReflection = vec4(0, 0, 0, 1);

	switch (Lighting)
	{
		case 0: // default = unlit
		default:
		{
			break;
		}
		case 1:
		{	
			oColor *= EDLShadingFactor(1,1);
			break;
		}
		case 2: //diffuse
		{
			oColor = diffuseReflection;		
			break;
		}

		case 3: //blinn phong
		{
			float specularAmmount = pow(max(dot(normalDir, halfwayDir), 0.0), Shininess);
			specularReflection = SecularColor * pow(specularAmmount, Shininess);

			oColor = diffuseReflection + (SpecularStrength * specularReflection);						
			break;
		}		
	}

}

