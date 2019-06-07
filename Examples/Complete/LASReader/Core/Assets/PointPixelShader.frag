#version 330 core

in vec3 vNormal;

out vec4 oColor;

void main(void)
{	
	oColor = vec4(vNormal, 1.0);
}