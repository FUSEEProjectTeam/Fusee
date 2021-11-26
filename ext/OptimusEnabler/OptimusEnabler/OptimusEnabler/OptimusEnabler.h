#pragma once

#ifdef _WIN32

#include <Windows.h>
extern "C"
{
	__declspec(dllexport) DWORD NvOptimusEnablement = 0x00000001;
	__declspec(dllexport) int AmdPowerXpressRequestHighPerformance = 1;

	__declspec(dllexport) int GetNvidiaOptimus();
	__declspec(dllexport) int GetAmdOptimus();
}

#endif // _WIN32


