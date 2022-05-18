#version 300 es

precision highp float;

uniform ivec2 FUSEE_ViewportPx;
uniform int PointSizeMode;
uniform int PointSize;

uniform mat4 FUSEE_V;
uniform mat4 FUSEE_P;

out vec3 vViewPos;
//out vec3 vColor;
out vec2 vPointCoord; //equivalent to gl_pointCoord

in vec3 fuVertex;
in vec3 fuColor;
in mat4 fuInstanceModelMat;
//in vec3 fuInstanceColor;

void main(void)
{	
	mat4 mv = FUSEE_V * fuInstanceModelMat;

    //vColor = fuInstanceColor;

    //assumption: position x and y are in range [-0.5, 0.5].
    vPointCoord = vec2(0.5)/fuVertex.xy;

    float scaledPtSize = float(PointSize) * 0.01;
    vViewPos = (mv * vec4(0.0, 0.0, 0.0, 1.0)
              + vec4(fuVertex.x, fuVertex.y, 0.0, 0.0)
              * vec4(scaledPtSize, scaledPtSize, 1.0, 1.0)).xyz;
    gl_Position = FUSEE_P * vec4(vViewPos, 1.0);
}