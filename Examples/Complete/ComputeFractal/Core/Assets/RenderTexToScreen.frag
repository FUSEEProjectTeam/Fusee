#version 460 core
precision highp float; 

in vec2 vUV;
out vec4 oColor;

uniform sampler2D srcTex;

void main()
{
    oColor = texture2D(srcTex, vUV);
}