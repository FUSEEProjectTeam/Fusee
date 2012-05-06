#pragma once
#include "CppApi.h"

class CPPAPI_API AParamType
{
public:
	int i;
};

class CPPAPI_API RefRefTest
{
public:
	static bool ParameterTaker(AParamType *& param);

};

