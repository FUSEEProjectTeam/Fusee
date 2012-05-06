#pragma once

#include "Parent.h"
#include "Child.h"

class CPPAPI_API Factory
{
public:
	static Parent *GimmeAParent();
	static Child *GimmeAChild();
	static Parent *GimmeAChildAsAParent();

};

