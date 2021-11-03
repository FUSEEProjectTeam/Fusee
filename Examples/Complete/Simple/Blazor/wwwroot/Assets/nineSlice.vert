#version 300 es

in vec3 fuVertex;
in vec3 fuNormal;
in vec2 fuUV;

out vec2 vUV;
out vec3 vMVNormal;
out vec4 fragBorders;

uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_ITMV;
uniform mat4 FUSEE_IMV;
uniform mat4 FUSEE_M;
uniform mat4 FUSEE_V;
uniform mat4 FUSEE_P;
uniform vec4 borders;
uniform vec4 borderThickness;

bool isFloatEqual(float a, float b)
{
	return (a + 0.000001 >= b) && (a - 0.000001 <= b);
}

vec4 calculateTranslationVector(vec2 scale, vec2 borderThicknessXY, float borderX, bool isXnegative, float borderY, bool isYnegative, vec3 coordinateSysVecX, vec3 coordinateSysVecY)
{                
	vec4 translateXVec = vec4(0.0,0.0,0.0,0.0);
	vec4 translateYVec = vec4(0.0,0.0,0.0,0.0);
	float translateX = 0.0;
	float translateY = 0.0;
                
	if( borderX > 0.00001)
	{
		float isX = abs(fuVertex.x * (scale.x));
		float translateToX = (((scale.x/2.0) - (borderThicknessXY.x * borderX)) - isX);
		translateXVec = (isXnegative) ? vec4(normalize(coordinateSysVecX) * -translateToX,0.0) : vec4(coordinateSysVecX * translateToX,0.0);                    
	}
                
	if( borderY  > 0.00001 )
	{
		float isY = abs(fuVertex.y * (scale.y));
		float translateToY = (((scale.y/2.0) - (borderThicknessXY.y * borderY)) - isY);
                    
		translateYVec = (isYnegative) ? vec4(normalize(coordinateSysVecY) * -translateToY,0.0) : vec4(coordinateSysVecY * translateToY,0.0);                    
	} 
	return (translateXVec + translateYVec);
}

vec4 calculateGlPosAccordingToUvs()
{
	vec2 scale =  vec2(length(FUSEE_M[0]),length(FUSEE_M[1]));

	mat4 origPlaneCoord = mat4(1.0);
	origPlaneCoord[2][2] = -1.0;

    mat4 planeCoord =  FUSEE_M * origPlaneCoord;

    vec3 xVec = normalize(planeCoord[0].xyz);
    vec3 yVec = normalize(planeCoord[1].xyz);
               
    float offsetL = borders.x;
    float offsetR = borders.y;
    float offsetT = borders.z;
    float offsetB = borders.w;                

	//left bottom corner
    if(isFloatEqual(vUV.x, 1.0/3.0) && isFloatEqual(vUV.y, 1.0/3.0))	
    {
         //Set Vertex and UV in unit plane, according to given border. 
		 gl_Position = vec4(-(0.5-offsetL), -(0.5-offsetB), 0.0, 1.0);		 
		 vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

		 //Translate the Vertex according to the scaling of the plane.
		 vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.x,borderThickness.w), offsetL, true, offsetB, true, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 1.0/3.0) && isFloatEqual(vUV.y, 0.0))
    {
		gl_Position = vec4(-(0.5-offsetL), -0.5, 0.0, 1.0);
		vUV = vec2(gl_Position.x,gl_Position.y) + vec2(0.5,0.5);

		vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.x,borderThickness.w), offsetL, true, 0.0, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 1.0/3.0) && isFloatEqual(vUV.x, 0.0))
    {
		gl_Position = vec4(-0.5, -(0.5-offsetB), 0.0, 1.0);			
		vUV = vec2(gl_Position.x,gl_Position.y) + vec2(0.5,0.5);

        vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.x,borderThickness.w), 0.0, true, offsetB, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	//left top corner
	if(isFloatEqual(vUV.x, 1.0/3.0) && isFloatEqual(vUV.y, 2.0/3.0))
    {
         gl_Position = vec4(-(0.5-offsetL), (0.5-offsetT), 0.0, 1.0);		 
		 vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);
		 
		 vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.x,borderThickness.z), offsetL, true, offsetT, false, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 1.0/3.0) && isFloatEqual(vUV.y, 1.0))
    {
		gl_Position = vec4(-(0.5-offsetL), 0.5, 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);		

		vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.x,borderThickness.z), offsetL, true, 0.0, true, xVec, yVec); 
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 2.0/3.0) && isFloatEqual(vUV.x, 0.0))
    {
		gl_Position = vec4(-0.5, (0.5-offsetT), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);		

        vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.x,borderThickness.z), 0.0, true, offsetT, false, xVec, yVec); 
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	//right bottom corner
    if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 1.0/3.0))	
    {
		gl_Position = vec4((0.5-offsetR), -(0.5-offsetB), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

         vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.y,borderThickness.w), offsetR, false, offsetB, true, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 0.0))
    {
		gl_Position = vec4((0.5-offsetR), -0.5, 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);
		
		vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.y,borderThickness.w), offsetR, false, 0.0, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 1.0/3.0) && isFloatEqual(vUV.x, 1.0))
    {
		gl_Position = vec4(0.5, -(0.5-offsetB), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

        vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.y,borderThickness.w), 0.0, true, offsetB, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	//right top corner
	if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 2.0/3.0))
    {
		gl_Position = vec4((0.5-offsetR), (0.5-offsetT), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);	
			
         vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.y,borderThickness.z), offsetR, false, offsetT, false, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 1.0))
    {
		gl_Position = vec4((0.5-offsetR),  0.5, 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

		vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.y,borderThickness.z), offsetR, false, 0.0, true, xVec, yVec); 
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 2.0/3.0) && isFloatEqual(vUV.x, 1.0))
    {
		gl_Position = vec4(0.5, (0.5-offsetT), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);
		
        vec4 translateVec = calculateTranslationVector(scale,vec2(borderThickness.y,borderThickness.z), 0.0, false, offsetT, false, xVec, yVec); 
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	//corner vertices
    if((isFloatEqual(vUV.x, 1.0) && isFloatEqual(vUV.y, 0.0)) || (isFloatEqual(vUV.x, 1.0) && isFloatEqual(vUV.y, 1.0)) || (isFloatEqual(vUV.x,0.0) && isFloatEqual(vUV.y, 1.0)) || (isFloatEqual(vUV.x, 0.0) && isFloatEqual(vUV.y, 0.0)))
	{ 
		return(FUSEE_P * FUSEE_V * FUSEE_M * vec4(fuVertex, 1.0));
	}	
}
                    
void main() 
{
	vUV = fuUV;
	fragBorders = borders;
	
	vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
	//gl_Position = FUSEE_P * FUSEE_V * FUSEE_M * vec4(fuVertex, 1.0);
	gl_Position = calculateGlPosAccordingToUvs();
}