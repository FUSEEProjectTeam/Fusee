#version 100

#ifdef GL_ES
precision highp float;
#endif

varying vec2 vUV;
varying vec3 vMVNormal;

uniform sampler2D DiffuseTexture;
uniform vec4 DiffuseColor;
uniform float DiffuseMix;

void main()
{
	vec3 N = normalize(vMVNormal);
	vec3 L = vec3(0.0,0.0,-1.0);
	vec4 color = vec4(texture2D(DiffuseTexture,vUV) * DiffuseMix);

	if(DiffuseMix == 0.0)
		color = vec4(1.0, 1.0, 1.0, texture2D(DiffuseTexture, vUV).a);	

	gl_FragColor = color * DiffuseColor *  max(dot(N, L), 0.0);
}


            