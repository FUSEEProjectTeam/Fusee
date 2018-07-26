#version 300 es
#define PI 3.14159265359


precision highp float;

in vec3 vUV;
in vec3 vNormal;
in vec3 vViewDir;
in vec4 vFragPos;
in vec4 vLDir;
in vec3 vTangent;
in vec3 vBiTangent;


uniform samplerCube cubeMapEnvironment;
uniform vec3 DiffuseColor;

out vec4 fragColor;


// SPECULAR
uniform vec3 SpecularColor;
uniform float SpecularShininess;
uniform float SpecularIntensity;

vec3 CalulateDiffuse(vec3 N, vec3 L, vec3 color)
{
	// normalize N & L just to be sure
	N = normalize(N);
	L = normalize(L);
	
	float NdotL = dot(N, L);
	
	return max(NdotL, 0.0) * color;
}

vec3 CalulateSpecular(vec3 N, vec3 L, vec3 V, vec3 color, float intensity, float shininess) {

	vec3 result = vec3(0);

	if(dot(L, N) > 0.0) {
		vec3 H = normalize(L + V);
		float specularTerm = pow(max(0.0, dot(H, N)), shininess);
		result = SpecularColor * intensity * specularTerm;
	}

	return result;
}

//////////// BRDF Cook-Torrance ////

float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a2     = roughness*roughness;
    float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;
	
    float nom    = a2;
    float denom  = (NdotH2 * (a2 - 1.0) + 1.0);
    denom        = PI * denom * denom;
	
    return nom / denom;
}


float GeometrySchlickGGX(float NdotV, float k)
{
    float nom   = NdotV;
    float denom = NdotV * (1.0 - k) + k;
	
    return nom / denom;
}
  
float GeometrySmith(vec3 N, vec3 V, vec3 L, float k)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx1 = GeometrySchlickGGX(NdotV, k);
    float ggx2 = GeometrySchlickGGX(NdotL, k);
	
    return ggx1 * ggx2;
}

vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

vec3 CalulateBDRF(vec3 N, vec3 V, vec3 L, float roughness, vec3 albedo, float metallic, vec3 radiance) {
	
	vec3 H = normalize(L + V);

	vec3 F0 = vec3(0.04); 
	F0	= mix(F0, albedo, metallic);
	vec3 F  = fresnelSchlick(max(dot(H, V), 0.0), F0);

	float NDF = DistributionGGX(N, H, roughness);       
	float G   = GeometrySmith(N, V, L, roughness);       

	vec3 numerator = NDF * G * F;
	float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
	vec3 specular = numerator / max(denominator, 0.001);
	
	vec3 kS = F;
	vec3 kD = vec3(1.0) - kS;
  
	vec3 irradiance = texture(cubeMapEnvironment, N).rgb;
	vec3 diffuse    = irradiance * albedo;
	vec3 ambient    = (kD * albedo); 

	//kD *= 1.0 - metallic;	
	float NdotL = max(dot(N, L), 0.0);

	return (kD * albedo / PI + specular) * radiance * NdotL;
}

vec3 CalculateIrradianceFromCubeMapTexture(vec3 N, samplerCube cubeTexture) {

	vec3 irradiance = vec3(0.0);  

	vec3 up    = vec3(0.0, 1.0, 0.0);
	vec3 right = cross(up, N);
	up         = cross(N, right);

	float sampleDelta = 0.025;
	float nrSamples = 0.0; 
	for(float phi = 0.0; phi < 2.0 * PI; phi += sampleDelta)
	{
		for(float theta = 0.0; theta < 0.5 * PI; theta += sampleDelta)
		{
			// spherical to cartesian (in tangent space)
			vec3 tangentSample = vec3(sin(theta) * cos(phi),  sin(theta) * sin(phi), cos(theta));
			// tangent space to world
			vec3 sampleVec = tangentSample.x * right + tangentSample.y * up + tangentSample.z * N; 

			irradiance += texture(cubeTexture, sampleVec).rgb * cos(theta) * sin(theta);
			nrSamples++;
		}
	}
	irradiance = PI * irradiance * (1.0 / float(nrSamples));

	return irradiance;
}


void main() {

	vec3 N = vNormal;
	vec3 V = vViewDir;
	vec3 L = normalize(vLDir.xyz);

	vec3 diffuse = CalulateDiffuse(N, L, DiffuseColor);
	vec3 specular = CalulateSpecular(N, L, V, SpecularColor, SpecularIntensity, SpecularShininess);

	vec3 irradiance = CalculateIrradianceFromCubeMapTexture(N, cubeMapEnvironment);

	vec3 ambient = vec3(0.03) * DiffuseColor;
    vec3 color = diffuse + specular + ambient;

	fragColor = vec4(color, 1.0);

}

