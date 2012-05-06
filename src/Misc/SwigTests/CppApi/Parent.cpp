#include "Parent.h"


Parent::Parent(void)
{
	I = 21;
}


Parent::~Parent(void)
{
}


int Parent::WhatAmI()
{
	return 0;
}

int Parent::DoSomething(int a, int b)
{
	return a + b;
}

	
int Parent::DoSomethingElse(int a, int b)
{
	return b - a;
}

/*
void Parent::MethodWithRefInt(int &a)
{
   I = a*2;
   a = I+3;
}

void Parent::MethodWithRefSize(SIZE &s)
{
   I = (int)s;
   s = (SIZE)(I+3);
}
*/
