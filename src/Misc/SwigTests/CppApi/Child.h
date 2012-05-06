#pragma once
#include "Parent.h"
#include "CppApi.h"


class CPPAPI_API Child : public Parent
{
public:
	Child(void);
	virtual ~Child(void);

	virtual int WhatAmI();

	int J;
	int DoEvenMore();
	virtual int DoSomethingElse(int a, int b);
};

