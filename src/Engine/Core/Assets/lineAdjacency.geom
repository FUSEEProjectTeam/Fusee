#version 460 core

layout (lines_adjacency) in;// enables access to four vertices (line segment vertices, predecessor, successor)
layout (triangle_strip, max_vertices = 256) out;

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
    vec2 Viewport = vec2(u_width, u_height);
    float line_width = max(1.0, Thickness);
    vec4 pos0 = gl_in[0].gl_Position; 
    vec4 pos1 = gl_in[1].gl_Position;
    vec4 pos2 = gl_in[2].gl_Position;
    vec4 pos3 = gl_in[3].gl_Position;

    //ndc
    vec2 ndc0 = gl_in[0].gl_Position.xy / gl_in[0].gl_Position.w; 
    vec2 ndc1 = gl_in[1].gl_Position.xy / gl_in[1].gl_Position.w;
    vec2 ndc2 = gl_in[2].gl_Position.xy / gl_in[2].gl_Position.w;
    vec2 ndc3 = gl_in[3].gl_Position.xy / gl_in[3].gl_Position.w;

    //direction of the three segments (previous, current, next) */
    vec2 line_vector0 = ndc1 - ndc0;
    vec2 line_vector1 = ndc2 - ndc1;
    vec2 line_vector2 = ndc3 - ndc2;
    vec2 dir0 = normalize(vec2(line_vector0.x, line_vector0.y * u_aspect_ratio));
    vec2 dir1 = normalize(vec2(line_vector1.x, line_vector1.y * u_aspect_ratio));
    vec2 dir2 = normalize(vec2(line_vector2.x, line_vector2.y * u_aspect_ratio));

    //normals of the three segments (previous, current, next)
    vec2 n0 = vec2( -dir0.y, dir0.x );
    vec2 n1 = vec2( -dir1.y, dir1.x );
    vec2 n2 = vec2( -dir2.y, dir2.x );

    // determine miter lines by averaging the normals of the 2 segments
    vec2 miter_a = normalize( n0 + n1 );// miter at start of current segment
    vec2 miter_b = normalize( n1 + n2 );// miter at end of current segment

    // determine the length of the miter by projecting it onto normal and then inverse it
    float an1 = dot(miter_a, n1);
    float bn1 = dot(miter_b, n2);
    if (an1==0) an1 = 1;
    if (bn1==0) bn1 = 1;

    float length_a = line_width / an1;
    if( dot(dir0, dir1 ) < -0.1/*MiterLimit*/) 
    {
        miter_a = n1;
        length_a = Thickness;
    }
    
    float length_b = line_width / bn1;
    if( dot(dir1, dir2) < -0.1/*MiterLimit*/) {
        miter_b = n1;
        length_b = Thickness;
    }

    n0 = vec2(line_width/u_width, line_width/u_height) * n0;
    n1 = vec2(line_width/u_width, line_width/u_height) * n1;
    n2 = vec2(line_width/u_width, line_width/u_height) * n2;
    miter_a = vec2(length_a/u_width, length_a/u_height) * miter_a;
    miter_b = vec2(length_b/u_width, length_b/u_height) * miter_b;

    if(EnableVertexColors)
        gColor = vColor0[0];
    gl_Position = vec4((ndc1 + miter_a) * pos1.w, pos1.zw);
    EmitVertex();

    if(EnableVertexColors)
        gColor = vColor0[0];
    gl_Position = vec4((ndc1 - miter_a) * pos1.w, pos1.zw);
    EmitVertex();

    if(EnableVertexColors)
        gColor = vColor0[1];
    gl_Position = vec4((ndc2 + miter_b) * pos2.w, pos2.zw);
    EmitVertex();

    if(EnableVertexColors)
        gColor = vColor0[1];
    gl_Position = vec4((ndc2 - miter_b) * pos2.w, pos2.zw);
    EmitVertex();

    EndPrimitive();
}