#version 330 core
out vec4 FragColor;
  
in vec4 oColor; // the input variable from the vertex shader (same name and same type)  
in vec3 oNormal;
in vec4 oPos;

void main()
{
    vec3 norm = normalize(oNormal);
    vec3 lightDir = normalize(-oPos.xyz);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * vec3(1,1,1);
    vec3 result = (0.1 + diffuse) * oColor.xyz;
    FragColor = vec4(result, 1.0);
} 