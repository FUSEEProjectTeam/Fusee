#version 330            

attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;

varying vec2 vUV;
varying vec3 vMVNormal;

uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_ITMV;
uniform mat4 FUSEE_IMV;
uniform mat4 FUSEE_M;
uniform mat4 FUSEE_V;
uniform mat4 FUSEE_P;
uniform vec4 borders;
uniform float borderThickness;

bool isFloatEqual(float a, float b)
{
	return (a + 0.000001 >= b) && (a - 0.000001 <= b);
}

vec4 calculateTranslationVector(vec2 scale, float borderX, bool isXnegative, float borderY, bool isYnegative, vec3 coordinateSysVecX, vec3 coordinateSysVecY)
{                
	vec4 translateXVec = vec4(0.0,0.0,0.0,0.0);
	vec4 translateYVec = vec4(0.0,0.0,0.0,0.0);
	float translateX = 0.0;
	float translateY = 0.0;
                
	if( borderX > 0.00001)
	{
		float isX = abs(fuVertex.x * (scale.x));
		float translateToX = (((scale.x/2.0) - (borderThickness * borderX)) - isX);
		translateXVec = (isXnegative) ? vec4(normalize(coordinateSysVecX) * -translateToX,0.0) : vec4(coordinateSysVecX * translateToX,0.0);                    
	}
                
	if( borderY  > 0.00001 )
	{
		float isY = abs(fuVertex.y * (scale.y));
		float translateToY = (((scale.y/2.0) - (borderThickness * borderY)) - isY);
                    
		translateYVec = (isYnegative) ? vec4(normalize(coordinateSysVecY) * -translateToY,0.0) : vec4(coordinateSysVecY * translateToY,0.0);                    
	} 
	return (translateXVec + translateYVec);
}

vec4 calculateGlPosAccordingToUvs()
{
	vec2 scale =  vec2(length(FUSEE_M[0]),length(FUSEE_M[1]));

	mat4 origPlaneCoord = mat4(1.0);
	origPlaneCoord[2][2] = -1;

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
		 gl_Position = vec4(-(0.5f-offsetL), -(0.5f-offsetB), 0.0, 1.0);		 
		 vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

		 //Translate the Vertex according to the scaling of the plane.
		 vec4 translateVec = calculateTranslationVector(scale, offsetL, true, offsetB, true, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 1.0/3.0) && isFloatEqual(vUV.y, 0.0))
    {
		gl_Position = vec4(-(0.5f-offsetL), -0.5f, 0.0, 1.0);
		vUV = vec2(gl_Position.x,gl_Position.y) + vec2(0.5,0.5);

		vec4 translateVec = calculateTranslationVector(scale, offsetL, true, 0.0, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 1.0/3.0) && isFloatEqual(vUV.x, 0.0))
    {
		gl_Position = vec4(-0.5f, -(0.5f-offsetB), 0.0, 1.0);			
		vUV = vec2(gl_Position.x,gl_Position.y) + vec2(0.5,0.5);

        vec4 translateVec = calculateTranslationVector(scale, 0.0, true, offsetB, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	//left top corner
	if(isFloatEqual(vUV.x, 1.0/3.0) && isFloatEqual(vUV.y, 2.0/3.0))
    {
         gl_Position = vec4(-(0.5f-offsetL), (0.5f-offsetT), 0.0, 1.0);		 
		 vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);
		 
		 vec4 translateVec = calculateTranslationVector(scale, offsetL, true, offsetT, false, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 1.0/3.0) && isFloatEqual(vUV.y, 1.0))
    {
		gl_Position = vec4(-(0.5f-offsetL), 0.5f, 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);		

		vec4 translateVec = calculateTranslationVector(scale, offsetL, true, 0.0, true, xVec, yVec); 
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 2.0/3.0) && isFloatEqual(vUV.x, 0.0))
    {
		gl_Position = vec4(-0.5f, (0.5f-offsetT), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);		

        vec4 translateVec = calculateTranslationVector(scale, 0.0, true, offsetT, false, xVec, yVec); 
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	//right bottom corner
    if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 1.0/3.0))	
    {
		gl_Position = vec4((0.5f-offsetR), -(0.5f-offsetB), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

         vec4 translateVec = calculateTranslationVector(scale, offsetR, false, offsetB, true, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 0.0))
    {
		gl_Position = vec4((0.5f-offsetR), -0.5f, 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);
		
		vec4 translateVec = calculateTranslationVector(scale, offsetR, false, 0.0, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 1.0/3.0) && isFloatEqual(vUV.x, 1.0))
    {
		gl_Position = vec4(0.5f, -(0.5f-offsetB), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

        vec4 translateVec = calculateTranslationVector(scale, 0.0, true, offsetB, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	//right top corner
	if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 2.0/3.0))
    {
		gl_Position = vec4((0.5f-offsetR), (0.5f-offsetT), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);	
			
         vec4 translateVec = calculateTranslationVector(scale, offsetR, false, offsetT, false, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 1.0))
    {
		gl_Position = vec4((0.5f-offsetR),  0.5f, 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

		vec4 translateVec = calculateTranslationVector(scale, offsetR, false, 0.0, true, xVec, yVec); 
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 2.0/3.0) && isFloatEqual(vUV.x, 1.0))
    {
		gl_Position = vec4(0.5f, (0.5f-offsetT), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);
		
        vec4 translateVec = calculateTranslationVector(scale, 0.0, false, offsetT, false, xVec, yVec); 
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

	vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
	//gl_Position = FUSEE_P * FUSEE_V * FUSEE_M * vec4(fuVertex, 1.0);
	gl_Position = calculateGlPosAccordingToUvs();
}