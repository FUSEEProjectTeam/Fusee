#version 300 es
#define KERNEL_LENGTH 64

precision highp float;
in vec2 vUV;
uniform vec2 ScreenParams;
uniform vec3[KERNEL_LENGTH] SSAOKernel;
uniform sampler2D Position;
uniform sampler2D Normal;
uniform sampler2D NoiseTex;
uniform mat4 FUSEE_P;
out vec4 Ssao;

void main() 
{
	vec3 Normal = texture(Normal, vUV).rgb;

    if(Normal.x == 0.0 && Normal.y == 0.0 && Normal.z == 0.0)
        discard;

    vec3 FragPos = texture(Position, vUV).xyz;

    float radius = 5.0;
    float occlusion = 0.0;
    float bias = 0.005;
    vec2 noiseScale = vec2(ScreenParams.x * 0.25, ScreenParams.y * 0.25);
	vec3 randomVec = texture(NoiseTex, vUV * noiseScale).xyz;
	vec3 tangent = normalize(randomVec - Normal * dot(randomVec, Normal));
	vec3 bitangent = cross(Normal, tangent);
	mat3 tbn = mat3(tangent, bitangent, Normal);

    for (int i = 0; i < KERNEL_LENGTH; ++i) 
    {
        // get sample position:
        vec3 sampleVal = tbn * SSAOKernel[i];
        sampleVal = sampleVal * radius + FragPos.xyz;

        // project sample position:
        vec4 offset = vec4(sampleVal, 1.0);
        offset = FUSEE_P * offset;		
        offset.xy /= offset.w;
        offset.xy = offset.xy * 0.5 + 0.5;

        // get sample depth:
        // ----- EXPENSIVE TEXTURE LOOKUP - graphics card workload goes up and frame rate goes down the nearer the camera is to the model.
        // keyword: dependent texture look up, see also: https://stackoverflow.com/questions/31682173/strange-performance-behaviour-with-ssao-algorithm-using-opengl-and-glsl
		float sampleDepth = texture(Position, offset.xy).z;           

        // range check & accumulate:
        float rangeCheck = smoothstep(0.0, 1.0, radius / abs(FragPos.z - sampleDepth));
        occlusion += (sampleDepth <= sampleVal.z + bias ? 1.0 : 0.0) * rangeCheck;
    }

    occlusion = clamp(1.0 - (occlusion / float(KERNEL_LENGTH)), 0.0, 1.0);           

    Ssao = vec4(occlusion, occlusion, occlusion, 1.0);
}