#include "Blinn/Blinn_CalculatePixelColor.frag"

out vec4 oColor;

void main(void)
{
	oColor = CalculatePixelColor();
}