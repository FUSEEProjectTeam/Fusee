#version 300 es

#ifdef GL_ES
precision highp float;
#endif

in vec2 vUV;
in vec3 vMVNormal;

uniform sampler2D DiffuseTexture;
uniform vec4 DiffuseColor;
uniform float DiffuseMix;

out vec4 outColor;

void main()
{
	vec3 N = normalize(vMVNormal);
	vec3 L = vec3(0.0,0.0,-1.0);
	vec4 color = vec4(texture(DiffuseTexture,vUV) * DiffuseMix);

	if(DiffuseMix == 0.0)
		color = vec4(1.0, 1.0, 1.0, texture(DiffuseTexture, vUV).a);	

	outColor = color * DiffuseColor *  max(dot(N, L), 0.0);
}


            