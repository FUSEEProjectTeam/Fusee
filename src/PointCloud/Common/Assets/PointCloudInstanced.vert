#version 300 es

precision highp float;

uniform ivec2 FUSEE_ViewportPx;
uniform int PointSizeMode;
uniform int PointSize;

uniform mat4 FUSEE_V;
uniform mat4 FUSEE_P;
uniform mat4 FUSEE_M;

out vec4 vViewPos;
//out vec3 vColor;
out vec2 vPointCoord; //equivalent to gl_pointCoord

in vec3 fuVertex;
in vec3 fuColor;
in mat4 fuInstanceModelMat;
//in vec3 fuInstanceColor;

#define PI 3.14159265358979323846f

void main(void)
{	
	mat4 mv = FUSEE_V * fuInstanceModelMat;

    //vColor = fuInstanceColor;

    //assumption: position x and y are in range [-0.5, 0.5].
    vPointCoord = vec2(0.5)/fuVertex.xy;
    float billboardHeight = 1.0;

    float z = mv[3][2]; //distance from rect to cam
    float fovY = 2.0 * atan(1.0/FUSEE_P[1][1]) * 180.0 / PI;
    float sizeInPx =  (billboardHeight/ (2.0 * tan(fovY / 2.0) * z)) * float(FUSEE_ViewportPx);
    float scaleFactor = float(PointSize) / sizeInPx;
    
    vViewPos = mv * vec4(0.0, 0.0, 0.0, 1.0)
              + vec4(fuVertex.x, fuVertex.y, 0.0, 0.0)
              * vec4(scaleFactor, scaleFactor, 1.0, 1.0);

    gl_Position =  FUSEE_P * FUSEE_M * vViewPos;
}