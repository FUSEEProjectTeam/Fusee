#version 300 es
precision highp float;

in vec2 vUV;
in vec4 vColor;

in vec3 vNormal;
in vec3 vViewPos;
in vec3 vFragPos;

uniform float AmbientStrengthName;
uniform float SpecularShininessName;
uniform float SpecularIntensityName;
uniform vec3 DiffuseColorName;
uniform vec3 SpecularColorName;


vec4 CalculatePixelColor()
{
	vec3 N = normalize(vNormal);
	vec3 V = -vViewPos;
	vec3 LDir = vec3(0.0, 0.0, -1.0);
	
	// Ambient lighting
	vec3 ambient = AmbientStrengthName * SpecularColorName;  

	// Diffuse   
    float intensityDiff = max(0.0, dot(N, LDir));
	vec3 diffuse = intensityDiff * SpecularColorName;
	
    // Specular
	vec3 specular = vec3(0.0);
	if(intensityDiff > 0.0)
	{	
		vec3 H = normalize(V + LDir);
		float intensitySpec = pow(abs(dot(H, N)), SpecularShininessName);
		specular = SpecularIntensityName * intensitySpec * SpecularColorName;
	}

	vec3 result = (ambient + diffuse + specular) * DiffuseColorName;
	    
    return vec4(result, 1.0);
}