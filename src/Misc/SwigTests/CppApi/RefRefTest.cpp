#include "RefRefTest.h"




bool RefRefTest::ParameterTaker(AParamType *& param)
{
	param = new AParamType;
	param->i = 42;
	return true;
}

