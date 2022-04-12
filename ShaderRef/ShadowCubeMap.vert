#version 300 es        
            
uniform mat4 FUSEE_M;              
in vec3 fuVertex; 

void main() 
{                
    gl_Position = FUSEE_M * vec4(fuVertex, 1.0);
}