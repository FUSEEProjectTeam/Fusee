#version 330 core

in vec3 fuVertex;
in vec3 fuNormal;

uniform mat4 FUSEE_MVP;

out vec3 vNormal;

void main(void)
{
	gl_PointSize = 20.0;
	vNormal = fuNormal;
	gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}