#version 300 es
precision highp float; 

in vec2 vUV;

uniform sampler2D InputTex1;
uniform sampler2D InputTex2;


uniform vec2 Point1;
uniform vec2 Point2;

uniform vec2 Middlepoint;
uniform float Radius;

//float x;
//float y;

layout (location = 0) out vec4 oBlurred;


void main() 
{
	vec3 result = vec3(0.0, 0.0, 0.0);
	
	vec3 tex1 = texture(InputTex1, vUV).rgb;
	vec3 tex2 = texture(InputTex2, vUV).rgb;
	
	//=> rendert die eine in die andere Textur!
	
	//result = texture(InputTex2, vUV).rgb + texture(InputTex1, vUV).rgb;
	
	//=> rendert beide Texturen über einander!
	
	//result = texture(InputTex2, vUV).rgb * texture(InputTex1, vUV).rgb;
	
	//=>Farbwert einzelner Punkt in Textur
	
	//vec2 texcoord = vec2(0.5, 0.5);  // get a value from the middle of the texture
	//result = texture(InputTex2, texcoord).rgb * texture(InputTex1, texcoord).rgb;
	
	//=> eine Textur wird gerendert!
	
	//result = texture(InputTex2, vUV).rgb;
	
	//=> Parametrisierung für diagonal!
		
		//x = vUV.x;
		 //y = vUV.y;
		
		
		
		//=> letzter Wert= t; Muss berechnet werden
		//result = mix(tex1, tex2, 0.5);
		
	
	
	//=> ab Hälfte  der X-Achse wird andere Textur gerendert!
	
	//result = texture(InputTex2,vUV).rgb;
	//if(vUV.x >0.5){
	
	//result = texture(InputTex1, vUV).rgb;
	
	//}
	
	//=> ab Hälfte der Y-Achse wird andere Textur gerendert!
	
	//if(vUV.y< 0.5){
	
	//result = texture(InputTex2, vUV ).rgb;
	
	//}
	
	//=> GLSL Test without if
	
	//float x = fract(vUV.x * 2.0);
    //float f = smoothstep(0.7, 0.7, vUV.x); //=> vergleicht mit Grenze und gibt 0 oder 1 aus oder interpoliert
    //result = mix(tex2, tex1, f);  //=> Blend between x * (1-f) and y * f
	
	
	//=> Linear Equation
	
	float Se = (vUV.y - Point1.y)*(Point2.x - Point1.x) - (Point2.y - Point1.y)*(vUV.x - Point1.x);
		
	Se = step(0.0,Se);
	
	//=> for interpolation
	
	//Se = smoothstep(-0.01,0.01,Se);
	
	result = mix(tex1, tex2, Se);
	
	
	//=> Circle
	
	//float difference = distance(Middlepoint, vUV) - Radius;
	
	//difference = step(0.0,difference); 
	
	//result = mix(tex2,tex1,difference);
	
	oBlurred = vec4(result, 1.0);
}

