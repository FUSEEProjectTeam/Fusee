#version 300 es

#define PI 3.14159265358979323846f

precision highp float;

struct Light
{
	vec3 position;
	vec4 intensities;
	vec3 direction;
	float maxDistance;
	float strength;
	float outerConeAngle;
	float innerConeAngle;
	int lightType;
	int isActive;
	int isCastingShadows;
	float bias;
};

uniform Light allLights[8];

uniform vec4 BaseColor;
uniform float Metallic;
uniform float IOR;
uniform float Roughness;
uniform float Subsurface;
uniform float Specular;
uniform float Transmission;
uniform float Ambient;

in vec3 oNormal;
in vec3 oViewDir;

out vec4 oColor;

float sqr(float value)
{
	return value * value;
}

float SchlickFresnel(float value)
{
	float m = clamp(1.0 - value, 0.0, 1.0);
	return pow(m, 5.0);
}

float G1 (float k, float x)
{
	return x / (x * (1.0 - k) + k);
}

vec3 CookTorranceSpec(float NdotL, float LdotH, float NdotH, float NdotV, float roughness, vec3 F0)
{
	float alpha = sqr(roughness);
	float D, G;
	vec3 F;

	// D (stribution GGX)
	float alphaSqr = sqr(alpha);
	float denom = sqr(NdotH) * (alphaSqr - 1.0) + 1.0f;
	D = alphaSqr / (PI * sqr(denom));

	// G (ometry - Schlick’s Approximation of Smith)
	float r = Roughness + 1.0;
	float k = sqr(r) / 8.0;

	float ggx1 = G1(k, NdotV);
	float ggx2 = G1(k, NdotL);
	G = ggx1 * ggx2;	

	// F (resnel)
	float LdotH5 = SchlickFresnel(NdotV);
	F = F0 + (1.0 - F0) * LdotH5;

	// GGX BRDF specular
	float resDenom = 4.0 * NdotV * NdotL;
	vec3 specular = (D * F * G) / max(resDenom, 0.1);
	return specular;
}

vec3 DisneyDiff(vec3 albedo, float NdotL, float NdotV, float LdotH, float VdotH, float roughness)
{
	// [Burley 2012, "Physically-Based Shading at Disney"]
	float FD90 = 0.5 + 2.0 * LdotH * LdotH * roughness;
	float FV = 1.0 + (FD90 - 1.0) * pow(1.0 - NdotV, 5.0);
	float FL = 1.0 + (FD90 - 1.0) * pow(1.0 - NdotL, 5.0);
	float Fd = FV * FL;

	// Based on Hanrahan-Krueger brdf approximation of isotropic bssrdf
	// 1.25 scale is used to (roughly) preserve albedo
	// Fss90 used to "flatten" retroreflection based on roughness
	float Fss90 = LdotH * LdotH * roughness;
	float Fss = mix(1.0, Fss90, FL) * mix(1.0, Fss90, FV);
	float ss = 1.25 * (Fss * (1.0 / max((NdotL + NdotV), 0.001) - 0.5) + 0.5);
			
	return (albedo / PI) * mix(Fd * NdotL, ss, Subsurface);
}

vec3 GetF0(vec3 albedo, float ior)
{
	float F0 = abs((1.0 - IOR) / (1.0 + IOR));
	F0 = F0 * F0;
	return mix(vec3(F0,F0,F0), albedo.rgb, Metallic);
}

vec3 LightingCookTorrance()
{
	//Ambient
	vec3 ambientLayer = Ambient * /*((1 - F) * Irradiance(s)) * */ BaseColor.rgb;
	vec3 res = vec3(0,0,0);

	for(int i = 0; i < 8; i++)
	{
		if(allLights[i].isActive == 0) continue;

		//Needed values	
		vec3 viewDir = normalize(oViewDir);
		vec3 lightDir = normalize(-allLights[i].direction);
		vec3 normal = normalize(oNormal);
		vec3 halfV = normalize(lightDir + viewDir);
		float NdotL = clamp(dot(normal, lightDir), 0.0, 1.0);
		float NdotH = clamp(dot(normal, halfV), 0.0, 1.0);
		float NdotV = clamp(dot(normal, viewDir), 0.0, 1.0);
		float VdotH = clamp(dot(viewDir, halfV), 0.0, 1.0);
		float LdotH = clamp(dot(lightDir, halfV), 0.0, 1.0);

		vec3 F0 = GetF0(BaseColor.rgb, IOR);
		float LdotH5 = SchlickFresnel(NdotV);
		vec3 F = F0 + (1.0 - F0) * LdotH5;

		//Direct diffuse
		vec3 diff =  DisneyDiff(BaseColor.rgb, NdotL, NdotV, LdotH, VdotH, Roughness);
			
		//Direct specular
		vec3 spec = CookTorranceSpec(NdotL, LdotH, NdotH, NdotV, Roughness, F0);		

		//Diffuse color, taking the metallic value into account - metals do not have a diffuse component.
		vec3 diffLayer = (1.0 - Metallic) /** (1-_Transmission)*/ * diff;

		//Specular color, combining metallic and dielectric specular reflection.
		//Metallic specular is affected by alebdo color, dielectric isn't!
		vec3 specLayerDielectric = Specular * spec;
		vec3 specLayerMetallic = Metallic * spec * BaseColor.rgb;
		vec3 specLayer = clamp(specLayerDielectric + specLayerMetallic, 0.0, 1.0);

	//	//Indirect specular (IBL Reflection)
	//	vec4 reflectCol = ReflectColor(s, viewDir);
	//	vec3 reflection = F * reflectCol;

	//	// IBL Refraction			
	//	float eta = 1.0 / _IOR; //ratio of the indices of refraction
	//	vec4 refractCol = RefractedColor(s, viewDir, eta);
	//	vec3 refraction = _Transmission * (1-F) * refractCol;
	//
	//	vec3 indirectSpecLyerMetallic = _Metallic *  reflection * albedo.rgb;
	//	vec3 indirectSpecLyerDielectric = _Specular * reflection;
	//	vec3 indirectSpecLyer = saturate(indirectSpecLyerDielectric + indirectSpecLyerMetallic);

		//Combining the layers...
		res += (1.0 - F) * diffLayer; 		// diffuse layer, affected by reflectivity
		res += specLayer;					// direct specular, not affected by reflectivity
	//	res += indirectSpecLyer;
	//	res += refraction;

	}
	res = ambientLayer + (1.0 - Ambient) * res;
	
	return res;
}

void main()
{
	vec3 result = LightingCookTorrance();
	oColor = vec4(result, BaseColor.a);
}
