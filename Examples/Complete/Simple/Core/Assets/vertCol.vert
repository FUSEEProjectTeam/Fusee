#version 300 es

precision highp float;

uniform mat4 FUSEE_MVP;

in vec3 fuVertex;
in vec3 fuNormal;
in vec3 fuColor;

out vec3 vNormal;
out vec3 vColor;

void main(void)
{	
	vColor = fuColor;
	gl_PointSize = 10.0;
	gl_Position = FUSEE_MVP * vec4(fuVertex.xyz, 1.0);
}