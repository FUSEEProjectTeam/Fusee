#version 460 core

uniform vec4 Albedo = vec4(1, 0, 1, 1);
uniform bool EnableVertexColors = false;

in vec4 gColor;
out vec4 fragColor;

void main() {
    fragColor = EnableVertexColors ? gColor : Albedo;
}