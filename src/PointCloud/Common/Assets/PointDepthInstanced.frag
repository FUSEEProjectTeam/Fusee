#version 330 core

uniform mat4 FUSEE_P;

out vec4 FragColor;

in vec2 vPointCoord;
in vec4 vViewPos;


void main()
{
	float weight = 1.0 - (pow(vPointCoord.x, 2.0) + pow(vPointCoord.y, 2.0)); //paraboloid weight function
	vec4 position = vViewPos;
	position.z -= weight;
	position = FUSEE_P * position;
	position = position / position.w;
	gl_FragDepth = position.z;

	FragColor = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);
}