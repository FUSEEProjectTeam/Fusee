#version 330 core

uniform mat4 FUSEE_P;

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

in vec3 vColor;
in vec3 vNormal;
in vec4 vViewPos;
in vec4 vClipPos;
in vec4 vWorldPos;
in float vWorldSpacePointRad;
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
			oColor = vec4(vColor,1.0); //vColor = vertex color
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
			vec2 uv = vec2(gl_FragCoord.x/ScreenParams.x, gl_FragCoord.y/ScreenParams.y);
			float z = texture2D(DepthTex, uv).x;
			float linearDepth = LinearizeDepth(z);
			if(linearDepth > 0.1)
				oColor *= EDLShadingFactor(EDLStrength, EDLNeighbourPixels, linearDepth, uv);						
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
			specularReflection = SpecularColor * pow(specularAmmount, Shininess);

			oColor = diffuseReflection + (SpecularStrength * specularReflection);						
			break;
		}
	}

}

