#version 460 core

layout (lines) in;// now we can access 2 vertices
layout (triangle_strip, max_vertices = 4) out;// always (for now) producing 2 triangles (so 4 vertices)

in vec4 vColor0[];
out vec4 gColor;

uniform float Thickness = 4;// just a test default
uniform ivec2 FUSEE_ViewportPx;
uniform mat4 FUSEE_MVP;
uniform bool EnableVertexColors = false;

void main()
{
    float u_width = float(FUSEE_ViewportPx.x);
    float u_height = float(FUSEE_ViewportPx.y);
    float u_aspect_ratio = u_height / u_width;

    vec2 ndc_a = gl_in[0].gl_Position.xy / gl_in[0].gl_Position.w;
    vec2 ndc_b = gl_in[1].gl_Position.xy / gl_in[1].gl_Position.w;

    vec2 line_vector = ndc_b - ndc_a;
    vec2 viewport_line_vector = line_vector * vec2(u_width, u_height);
    vec2 dir = normalize(vec2(line_vector.x, line_vector.y * u_aspect_ratio));

    float line_width = max(1.0, Thickness);
    float line_length = length(viewport_line_vector);

    vec2 normal    = vec2(-dir.y, dir.x);
    vec2 normal_a  = vec2(line_width/u_width, line_width/u_height) * normal;
    vec2 normal_b  = vec2(line_width/u_width, line_width/u_height) * normal;

    if(EnableVertexColors)
        gColor = vColor0[0];
    gl_Position = vec4((ndc_a + normal_a) * gl_in[0].gl_Position.w, gl_in[0].gl_Position.zw);
    EmitVertex();

    if(EnableVertexColors)
        gColor = vColor0[0];
    gl_Position = vec4((ndc_a - normal_a) * gl_in[0].gl_Position.w, gl_in[0].gl_Position.zw);
    EmitVertex();

    if(EnableVertexColors)
        gColor = vColor0[1];
    gl_Position = vec4((ndc_b + normal_b) * gl_in[1].gl_Position.w, gl_in[1].gl_Position.zw);
    EmitVertex();

    if(EnableVertexColors)
        gColor = vColor0[1];
    gl_Position = vec4((ndc_b - normal_b) * gl_in[1].gl_Position.w, gl_in[1].gl_Position.zw);
    EmitVertex();

    EndPrimitive();
}