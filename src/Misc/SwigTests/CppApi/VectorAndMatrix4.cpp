#include "VectorAndMatrix4.h"


// Hand coded 
extern "C" CPPAPI_API int __stdcall HandMadeVectorTaker(CVector4 *pVec)
// extern "C" CPPAPI_API int __stdcall HandMadeVectorTaker(void *p1)
{ 
	//CVector4 *pVec = (CVector4 *)p1;
	return (int) (pVec->_x + pVec->_y);
}

//extern "C" CPPAPI_API int __stdcall HandMadeMatrixTaker(CMatrix34 *pM)
//{ 
//	//CMatrix34 *pM = (CMatrix34 *)pVec;
//	return (int) (pM->off._x + pM->v1._y);
//}

extern "C" CPPAPI_API CVector4 __stdcall HandMadeVectorReturner()
{
	volatile int i = 42; // Have this to keep the compiler from optimizing us away
	CVector4 ret = {4, 3, 2, 1};
	return ret;
}

extern "C" CPPAPI_API CVector4* __stdcall HandMadeVectorPtrReturner()
{
	volatile int i = 42; // Have this to keep the compiler from optimizing us away
	CVector4 *pRet = new CVector4();
	pRet->_x = 1;
	pRet->_y = 2;
	pRet->_z = 3;
	pRet->_w = 4;

	return pRet;
}


//extern "C" CPPAPI_API CMatrix34 __stdcall HandMadeMatrixReturner()
//{
//	volatile int i = 42; // Have this to keep the compiler from optimizing us away
//	/* // UDT style initialization
//	CMatrix34 ret = CMatrix34(
//		CVector3(41, 42, 43),
//		CVector3(11, 12, 13),
//		CVector3(21, 22, 23),
//		CVector3(31, 32, 33)
//	);*/
//
//	// POD style initialization
//	CMatrix34 ret = 
//	{
//		{41, 42, 43},
//		{11, 12, 13},
//		{21, 22, 23},
//		{31, 32, 33}
//	};
//
//	return ret;
//}