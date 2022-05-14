﻿#version 330 core

in vec3 fuVertex;
in vec3 fuNormal;
in vec2 fuUV;

out vec2 vUv;
out vec3 vMVNormal;

uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_ITMV;
uniform mat4 FUSEE_IMV;

                    
void main() 
{
	vUv = fuUV;
	
	vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
	gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
	
}