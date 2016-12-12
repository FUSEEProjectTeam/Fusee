#ifdef GL_ES
precision highp float;
#endif

varying vec3 normal;

void main()
{
	gl_FragColor = vec4(normal*0.5+0.5, 1);
}