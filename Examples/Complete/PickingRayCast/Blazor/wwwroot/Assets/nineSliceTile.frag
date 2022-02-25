#version 300 es

#ifdef GL_ES
precision highp float;
#endif
         
in vec3 vMVNormal;
in vec2 vUv;
in vec4 fragBorders;

uniform sampler2D AlbedoTexture;
uniform vec4 Albedo;
uniform float AlbedoMix;
uniform vec2 Tile;

out vec4 outColor;

//Scales value from range [fromMin, fromMax] to range [toMin, toMax]
float bringInRange(float fromMin, float fromMax, float toMin, float toMax, float value)
{
	float nominator = (toMax - toMin) * (value - fromMin);
	float denominator = fromMax - fromMin;
	return (nominator/denominator) + toMin;
}

float calculateUvY(float currentTileY, vec2 p1, vec2 p4, vec2 Tile, float height)
{
	float uvY = vUv.y;

	//First tile
	if(currentTileY == 0.0)
		return bringInRange(p1.y , p1.y + height / Tile.y, p1.y, p4.y , vUv.y);
	//Last tile
	else if(currentTileY == Tile.y)
		return bringInRange(p1.y + (height * (currentTileY / Tile.y)), p1.y + height, p1.y, p4.y , vUv.y);
	//Every tile inbetween
	else
		return bringInRange( p1.y + (height * (currentTileY / Tile.y)), p1.y + (height * ((currentTileY + 1.0) / Tile.y)), p1.y, p4.y , vUv.y);

}

void main()
{
	vec3 N = normalize(vMVNormal);
	vec3 L = vec3(0.0,0.0,-1.0);

	//LRTB
	vec2 p1 = vec2(fragBorders.x, fragBorders.w); //lower left
	vec2 p2 = vec2(fragBorders.x, 1.0-fragBorders.z); //upper left
	vec2 p3 = vec2(1.0-fragBorders.y, 1.0-fragBorders.w); //lower right
	vec2 p4 = vec2(1.0-fragBorders.y, 1.0-fragBorders.z); //upper right		
	
	if((vUv.x >= p1.x && vUv.y >= p1.y) && (vUv.x <= p4.x && vUv.y <= p4.y))
	{		
		float currentTileX = floor(Tile.x * bringInRange(p1.x, p4.x, 0.0 ,1.0 , vUv.x));
		float currentTileY = floor(Tile.y * bringInRange(p1.y, p4.y, 0.0 ,1.0 , vUv.y));

		float width = p4.x - p1.x;
		float height = p4.y - p1.y;

		//Fist tile
		if(currentTileX == 0.0)
		{
			float uvX = bringInRange(p1.x , p1.x + width / Tile.x, p1.x, p4.x , vUv.x);
			float uvY = calculateUvY(currentTileY,p1, p4, Tile, height);

            vec4 baseColor = texture(AlbedoTexture, vec2(uvX, uvY)) * AlbedoMix * Albedo;
            vec3 diff = baseColor.rgb *  max(dot(N, L), 0.0);        
            outColor = vec4(diff, baseColor.a);
		}
		//Last tile
		else if(currentTileX == Tile.x) 
		{
			float uvX = bringInRange(p1.x + (width * (currentTileX / Tile.x)), p1.x + width, p1.x, p4.x , vUv.x);
			float uvY = calculateUvY(currentTileY,p1, p4, Tile, height);
			
            vec4 baseColor = texture(AlbedoTexture, vec2(uvX, uvY)) * AlbedoMix * Albedo;
            vec3 diff = baseColor.rgb *  max(dot(N, L), 0.0);        
            outColor = vec4(diff, baseColor.a);
		}
		//Every tile inbetween
		else 
		{
			float uvX = bringInRange( p1.x + (width * (currentTileX / Tile.x)), p1.x + (width * ((currentTileX + 1.0) / Tile.x)), p1.x, p4.x , vUv.x);
			float uvY = calculateUvY(currentTileY,p1, p4, Tile, height);

            vec4 baseColor = texture(AlbedoTexture, vec2(uvX , uvY)) * AlbedoMix * Albedo;
            vec3 diff = baseColor.rgb *  max(dot(N, L), 0.0);        
            outColor = vec4(diff, baseColor.a);
		}	
	}
	else{
        vec4 baseColor = texture(AlbedoTexture, vUv) * AlbedoMix* Albedo;
        vec3 diff = baseColor.rgb *  max(dot(N, L), 0.0);        
		outColor = vec4(diff, baseColor.a);
    }
}