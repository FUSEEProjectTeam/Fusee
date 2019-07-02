#version 300 es

#ifdef GL_ES
precision highp float;
#endif
         
in vec3 vMVNormal;
in vec2 vUV;

uniform sampler2D DiffuseTexture;
uniform vec4 DiffuseColor;
uniform float DiffuseMix;

out vec4 outColor;

void main(void)
{
	outColor = texture(DiffuseTexture, vUV).rgba * DiffuseMix + DiffuseColor * (1.0 - DiffuseMix);
}