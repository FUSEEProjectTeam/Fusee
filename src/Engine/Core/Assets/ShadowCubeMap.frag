#version 300 es
precision highp float; 
in vec4 FragPos;
uniform vec2 LightMatClipPlanes;
uniform vec3 LightPos;

void main()
{
    // get distance between fragment and light source
    float lightDistance = length(FragPos.xyz - LightPos);

    // map to [0;1] range by dividing by far_plane
    lightDistance = lightDistance / LightMatClipPlanes.y;

    // write this as modified depth                
    gl_FragDepth = lightDistance;
}