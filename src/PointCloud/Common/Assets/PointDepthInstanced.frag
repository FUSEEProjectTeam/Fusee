#version 460 core

uniform mat4 FUSEE_P;
uniform int PointShape;

out vec4 FragColor;

in vec2 vPointCoord;
in vec4 vViewPos;
in float vWorldSpacePointRad;


void main()
{
	switch (PointShape)
    {
            case 0: // default = square
            default:
                gl_FragDepth = gl_FragCoord.z;
                break;
            case 1: // circle	
        
                float distanceFromCenter = length(2.0 * vPointCoord - 1.0);
        
                if(distanceFromCenter > 1.0)
                    discard;
        
                gl_FragDepth = gl_FragCoord.z;
        
                break;
            case 2: //paraboloid
        
                float weight = 1.0 - (pow(vPointCoord.x, 2.0) + pow(vPointCoord.y, 2.0)); //paraboloid weight function
        
                vec4 position = vViewPos;
                position.z += weight * vWorldSpacePointRad;
                position = FUSEE_P * position;
                position /= position.w;
                gl_FragDepth = (position.z + 1.0) / 2.0;
        
                break;
    }

	FragColor = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);
}