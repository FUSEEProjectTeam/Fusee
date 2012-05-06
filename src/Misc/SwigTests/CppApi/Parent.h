#pragma once
#include "CppApi.h"

enum SIZE
{
	MEDIUM,
	TALL,
	XTALL,
};


class CPPAPI_API Parent
{
public:
	Parent(void);
	virtual ~Parent(void);

	virtual int WhatAmI();

	int I;
	int DoSomething(int a, int b);
	virtual int DoSomethingElse(int a, int b);

	/*
	void MethodWithRefInt(int &a);
	virtual void MethodWithRefSize(SIZE &s);
	*/
};

