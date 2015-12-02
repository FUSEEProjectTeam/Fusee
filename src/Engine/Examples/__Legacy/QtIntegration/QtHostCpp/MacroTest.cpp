#include "stdafx.h"


#define CASTER(_TYPE_, _VALUE_) (_TYPE_)_VALUE_

#define ARG_int(_PARAM_)	\
int _PARAM_;

#define DECLARER(_TYPE_, _VALUE_)	\
	ARG_##_TYPE_(_VALUE_);		 \



void func3()
{
	DECLARER(int, wert);

	wert = 3;

	volatile int ii = 4;
	ii += 2;
}

void func()
{
	int i;
	double d = 3.1415;

	i = CASTER(int, d);

	return (void) func3();
}



/*
#define DEFINER  \
#define a


#define MACRO1(_PARAM_)				\
#define _CURRENT_PARAM_		_PARAM_	\
;


#define MACRO2()					\
printf(#_CURRENT_PARAM_)

void func()
{
	DEFINER

	MACRO2();

}
*/