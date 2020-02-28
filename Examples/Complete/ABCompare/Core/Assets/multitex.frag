#version 300 es
precision highp float; 

in vec2 vUV;

uniform sampler2D InputTex1;
uniform sampler2D InputTex2;


uniform vec2 Point1;
uniform vec2 Point2;

uniform vec2 Middlepoint;
uniform float Radius;


layout (location = 0) out vec4 oBlurred;

vec3 result;
vec3 tex1;
vec3 tex2;


void linearEquation()
{
	
	//=>Berechnung der Trennlinie
	
	float Se = (vUV.y - Point1.y)*(Point2.x - Point1.x) - (Point2.y - Point1.y)*(vUV.x - Point1.x);
	
	//=> Werte auf 0 oder 1 setzen
	
	Se = step(0.0,Se);
	
	//=> für eine Interpolation zwischen den Texturen
	
	//Se = smoothstep(-0.01,0.01,Se); 
	
	result = mix(tex2, tex1, Se);
	
}

void circle()
{
	//=> Berechnung ob Wert innerhalb oder außerhalb des Kreises ist
	
	float difference = distance(Middlepoint, vUV) - Radius;
	
	difference = step(0.0,difference); 
	
	result = mix(tex2,tex1,difference);
	

}



//=> Versuche und Fehlschläge!

void inother()
{
	//=> rendert die eine in die andere Textur!
	
	//result = texture(InputTex2, vUV).rgb + texture(InputTex1, vUV).rgb;
}

void overlapping()
{
	//=> rendert beide Texturen über einander!
	
	//result = texture(InputTex2, vUV).rgb * texture(InputTex1, vUV).rgb;
}

void onecolorpoint()
{
	//=>Farbwert einzelner Punkt in Textur
	
	//vec2 texcoord = vec2(0.5, 0.5);  // get a value from the middle of the texture
	//result = texture(InputTex2, texcoord).rgb * texture(InputTex1, texcoord).rgb;
}

void simple()
{
	//=> eine Textur wird gerendert!
	
	//result = texture(InputTex2, vUV).rgb;
	
}

void simplecomparex()
{
	//=> ab Hälfte  der X-Achse wird andere Textur gerendert!
	
	//result = texture(InputTex2,vUV).rgb;
	//if(vUV.x >0.5){
	
	//result = texture(InputTex1, vUV).rgb;
	
	//}
	
}


void simplecomparey()
{
	//=> ab Hälfte der Y-Achse wird andere Textur gerendert!
	
	//if(vUV.y< 0.5){
	
	//result = texture(InputTex2, vUV ).rgb;
	
	//}
	
}


void main() 
{
	result = vec3(0.0, 0.0, 0.0);
	
	tex1 = texture(InputTex1, vUV).rgb;
	tex2 = texture(InputTex2, vUV).rgb;
	
	linearEquation();
	//circle();
	oBlurred = vec4(result, 1.0);
}





