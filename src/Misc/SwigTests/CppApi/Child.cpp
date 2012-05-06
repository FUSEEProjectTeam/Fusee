#include "Child.h"


Child::Child(void)
{
	J = 41;
}


Child::~Child(void)
{
}

int Child::WhatAmI()
{
	return 1;
}


int Child::DoEvenMore()
{
	return 42;
}
	
int Child::DoSomethingElse(int a, int b)
{
	return a - b;
}