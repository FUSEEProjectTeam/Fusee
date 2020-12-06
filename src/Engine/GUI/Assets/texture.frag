#version 300 es

#ifdef GL_ES
precision highp float;
#endif

in vec2 vUV;
in vec3 vMVNormal;

uniform sampler2D AlbedoTexture;
uniform vec2 AlbedoTextureTiles;
uniform vec4 Albedo;
uniform float AlbedoMix;

out vec4 outColor;

vec3 EncodeSRgb(vec3 linearRGB)
{
    vec3 a = 12.92 * linearRGB;
    vec3 b = 1.055 * pow(linearRGB, vec3(1.0 / 2.4)) - 0.055;
    vec3 c = step(vec3(0.0031308), linearRGB);
    return mix(a, b, c);
}

vec3 DecodeSRgb(vec3 sRgb)
{
    vec3 a = sRgb / 12.92;
    vec3 b = pow((sRgb + 0.055) / 1.055, vec3(2.4));
    vec3 c = step(vec3(0.04045), sRgb);
    return mix(a, b, c);
}

void main()
{
	vec3 N = normalize(vMVNormal);
	vec3 L = vec3(0.0, 0.0, -1.0);

    vec3 lightColor = vec3(1.0, 1.0, 1.0);
    vec4 texCol = texture(AlbedoTexture, vUV * AlbedoTextureTiles);
    
    texCol = vec4(DecodeSRgb(texCol.rgb), texCol.a);
    float linearLuminance = (0.2126 * texCol.r) + (0.7152 * texCol.g) + (0.0722 * texCol.b);

    vec3 Idif = vec3(max(dot(N, L), 0.0) * lightColor);
    vec3 linearOutCol = Idif * mix(Albedo.rgb * linearLuminance, texCol.rgb, AlbedoMix);
	outColor = vec4(EncodeSRgb(linearOutCol.rgb), texCol.a);
}