#version 460 core

layout (lines_adjacency) in;// enables access to four vertices (line segment vertices, predecessor, successor)
layout (triangle_strip, max_vertices = 256) out;

in vec4 vColor0[];
out vec4 gColor;

uniform float Thickness = 4;// just a test default
uniform ivec2 FUSEE_ViewportPx;
uniform mat4 FUSEE_MVP;
uniform bool EnableVertexColors = false;

void drawSegment(float u_width, float u_height, float u_aspect_ratio, vec4 pos0, vec4 pos1)
{

    vec2 ndc_a = pos0.xy / pos0.w;
    vec2 ndc_b = pos1.xy / pos1.w;

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
    gl_Position = vec4((ndc_a + normal_a) * pos0.w, pos0.zw);
    EmitVertex();

    if(EnableVertexColors)
        gColor = vColor0[0];
    gl_Position = vec4((ndc_a - normal_a) * pos0.w, pos0.zw);
    EmitVertex();

    if(EnableVertexColors)
        gColor = vColor0[1];
    gl_Position = vec4((ndc_b + normal_b) * pos1.w, pos1.zw);
    EmitVertex();

    if(EnableVertexColors)
        gColor = vColor0[1];
    gl_Position = vec4((ndc_b - normal_b) * pos1.w, pos1.zw);
    EmitVertex();
}

void main()
{
    float u_width = float(FUSEE_ViewportPx.x);
    float u_height = float(FUSEE_ViewportPx.y);
    float u_aspect_ratio = u_height / u_width;

    drawSegment(u_width, u_height, u_aspect_ratio, gl_in[0].gl_Position, gl_in[1].gl_Position);
    drawSegment(u_width, u_height, u_aspect_ratio, gl_in[1].gl_Position, gl_in[2].gl_Position);
    drawSegment(u_width, u_height, u_aspect_ratio, gl_in[2].gl_Position, gl_in[3].gl_Position);

    EndPrimitive();
}