#version 450 core
precision highp float; 

in vec2 vUv;
out vec4 oColor;

uniform sampler2D srcTex;

void main()
{
    oColor = texture(srcTex, vUv);
}