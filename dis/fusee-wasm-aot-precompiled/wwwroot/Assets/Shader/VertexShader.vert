#include "Blinn/Blinn_CalculateVertexPosition.vert"

void main(void)
{
	gl_Position = CalculateVertexPosition();
}