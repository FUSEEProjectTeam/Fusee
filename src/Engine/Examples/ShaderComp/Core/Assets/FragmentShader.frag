#ifdef GL_ES
	precision highp float;
#endif

// normals from VertexShader
varying vec3 vNormal;

// uniform parameter, set as EffectParameter
uniform vec3 uColor;

void main() {
	gl_FragColor = vec4((vNormal * 0.5 + 0.5) * uColor, 1.0);
}