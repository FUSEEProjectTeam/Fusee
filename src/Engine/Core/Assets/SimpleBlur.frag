#version 300 es
precision highp float; 
#define KERNEL_SIZE_HALF 2

in vec2 vUV;
uniform sampler2D InputTex;
layout (location = 0) out vec4 oBlurred;

void main() 
{
	vec2 texelSize = 1.0 / vec2(textureSize(InputTex, 0));
	float result = 0.0;

	for (int x = -KERNEL_SIZE_HALF; x < KERNEL_SIZE_HALF; ++x) 
	{
		for (int y = -KERNEL_SIZE_HALF; y < KERNEL_SIZE_HALF; ++y) 
		{
			vec2 offset = vec2(float(x), float(y)) * texelSize;
			result += texture(InputTex, vUV + offset).r;
		}
	}
            
	float kernelSize = float(KERNEL_SIZE_HALF) * 2.0;
	result = result / (kernelSize * kernelSize);
            
	oBlurred = vec4(result, result, result, 1.0);
}