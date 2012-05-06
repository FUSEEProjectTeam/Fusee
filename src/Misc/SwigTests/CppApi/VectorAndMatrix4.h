#pragma once
#include "CppApi.h"

struct CVector4
{
	double _x;
	double _y;
	double _z;
	double _w;
};

struct CVector3_POD
{
	double _x;
	double _y;
	double _z;
};


struct CVector3
{
	double _x;
	double _y;
	double _z;

	// Uncommenting this makes CVector3 a UDT (unser defined type) leading to P/Invoke problems when objects of this type are returnded
	//  otherwise (without contructors) this type remains a POD (plain old datatype) which seems to work using P/Invoke for functions returning this type
	CVector3()
	{
		_x = _y = _z = 0;
	}
	CVector3(double x, double y, double z)
	{
		_x = x;
		_y = y;
		_z = z;
	}

	// This is needed to work around some crude swig c# initialization error:
	CVector3(int dummy)
	{
		_x = dummy;
	}
};

//
//struct CMatrix34
//{
//	CVector3 off, v1, v2, v3;
//	/* Uncommenting this makes CMatrix34 a UDT (unser defined type) leading to P/Invoke problems when objects of this type are returnded
//	   otherwise (without contructors) this type remains a POD (plain old datatype) which seems to work using P/Invoke for functions returning this type
//	CMatrix34()
//	{
//	}
//
//	CMatrix34(const CVector3 &off_in, const CVector3 &v1_in, const CVector3 &v2_in, const CVector3 &v3_in)
//	{
//		off = off_in;
//		v1  = v1_in;
//		v2  = v2_in;
//		v3  = v3_in;
//	}
//	*/
//};

// Hand made
extern "C" CPPAPI_API int __stdcall HandMadeVectorTaker(CVector4 *pVec);
// extern "C" CPPAPI_API int __stdcall HandMadeVectorTaker(void *pVec);
//extern "C" CPPAPI_API int __stdcall HandMadeMatrixTaker(CMatrix34 *pVec);
extern "C" CPPAPI_API CVector4* __stdcall HandMadeVectorPtrReturner();
extern "C" CPPAPI_API CVector4 __stdcall HandMadeVectorReturner();
//extern "C" CPPAPI_API CMatrix34 __stdcall HandMadeMatrixReturner();