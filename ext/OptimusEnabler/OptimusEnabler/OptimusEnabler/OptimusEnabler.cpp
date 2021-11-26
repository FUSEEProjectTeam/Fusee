#include "OptimusEnabler.h"

int GetNvidiaOptimus()
{
	return NvOptimusEnablement;
}
int GetAmdOptimus()
{
	return AmdPowerXpressRequestHighPerformance;
}