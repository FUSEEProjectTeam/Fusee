#version 460 core

in vec3 fuVertex;
in vec4 fuColor;

uniform mat4 FUSEE_MV;
uniform mat4 FUSEE_P;

out vec4 vColor;

void main() {
    vec4 viewPos = FUSEE_MV * vec4(fuVertex, 1.0);

    //prevent clipping
    if (viewPos.z <= 0.0)
    {
        viewPos.z = 0.001;
    }

    vColor = fuColor;
    gl_Position = FUSEE_P * viewPos;
}