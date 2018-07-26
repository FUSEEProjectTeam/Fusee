#version 100        

attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;

varying vec2 vUV;
varying vec3 vMVNormal;


uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_ITMV;
uniform mat4 FUSEE_IMV;

                    
void main() 
{
	vUV = fuUV;
	
	vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
	gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
	
}