#version 300 es
precision highp float; 

in vec2 vUV;
uniform sampler2D InputTex1;

layout (location = 0) out vec4 oBlurred;

void main() 
{
	vec3 result = vec3(0.0, 0.0, 0.0);
	
	//=> Erstellung einer simplen Verzerrung
	
	result = texture(InputTex1, vUV + 0.005*vec2( sin(1024.0*vUV.r),cos(768.0*vUV.g))).rgb;
	        
	oBlurred = vec4(result, 1.0);
}