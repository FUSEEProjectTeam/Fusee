attribute vec3 fuVertex;
attribute vec3 fuNormal;

uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_ITMV;


varying vec3 vNormal;

void main()
{
	vNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
	gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
}