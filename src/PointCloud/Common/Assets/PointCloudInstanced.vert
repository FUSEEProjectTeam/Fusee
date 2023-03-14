#version 460 core

precision highp float;

uniform ivec2 FUSEE_ViewportPx;
uniform int PointSizeMode;
uniform int PointSize;

uniform mat4 FUSEE_V;
uniform mat4 FUSEE_P;
uniform mat4 FUSEE_M;

out vec4 vViewPos;
out float vWorldSpacePointRad;
//out vec3 vColor;
out vec2 vPointCoord; //equivalent to gl_pointCoord

in vec3 fuVertex;
in mat4 fuInstanceModelMat;
//in vec3 fuInstanceColor;

#define PI 3.14159265358979323846f

void main(void)
{	
	mat4 mv = FUSEE_V * FUSEE_M * fuInstanceModelMat;

    //vColor = fuInstanceColor;

    //assumption: position x and y are in range [-0.5, 0.5].
    vPointCoord = vec2(0.5)/fuVertex.xy;

    float z = mv[3][2]; //distance from rect to cam
    float fov = 2.0 * atan(1.0/FUSEE_P[1][1]) * 180.0 / PI;

    float slope = tan(fov / 2.0);
    float projFactor = ((1.0 / slope) / - z) * float(FUSEE_ViewportPx.y) / 2.0;
    vWorldSpacePointRad = float (PointSize) / projFactor;
    
    float sizeInPx = 1.0;
    float billboardHeight = 1.0;
    switch(PointSizeMode)
    {
        // Fixed pixel size"
        case 0:
        {
            sizeInPx = (billboardHeight / (2.0 * slope * z)) * float(FUSEE_ViewportPx);
            break;
        }
        //Fixed world size"
        case 1:
        {
            //In this scenario the PointSize is the given point radius in world space - the point size in pixel will shrink if the camera moves farther away"                    "",
            sizeInPx = (billboardHeight / (2.0 * slope)) * float(FUSEE_ViewportPx);
            break;
        }
    }
    float scaleFactor = float(PointSize) / sizeInPx;
    
    vViewPos = mv * vec4(0.0, 0.0, 0.0, 1.0)
              + vec4(fuVertex.x, fuVertex.y, 0.0, 0.0)
              * vec4(scaleFactor, scaleFactor, 1.0, 1.0);

    gl_Position =  FUSEE_P * vViewPos;
}