#include "VectorConsumer.h"


VectorConsumer::VectorConsumer(void)
{
	VV = CVector3(12, 13, 14);
}


VectorConsumer::~VectorConsumer(void)
{
}

int VectorConsumer::VectorTakerPtr3(CVector3 *pVec)
{ 
	pVec->_y = 4711;
	return (int) (pVec->_y + pVec->_z);
}

int VectorConsumer::VectorTakerRef3(CVector3 &rVec)
{ 
	rVec._x = 42;
	return (int) (rVec._y + rVec._z);
}

int VectorConsumer::VectorTakerVal3(CVector3 vec)
{ 
	return (int) (vec._y + vec._z);
}

int VectorConsumer::VectorTakerPtr4(CVector4 *pVec)
{ 
	pVec->_w = 4711;
	return (int) (pVec->_y + pVec->_z);
}

int VectorConsumer::VectorTakerRef4(CVector4 &rVec)
{ 
	rVec._x = 42;
	return (int) (rVec._y + rVec._z);
}

int VectorConsumer::VectorTakerVal4(CVector4 vec)
{ 
	return (int) (vec._y + vec._z);
}


//int VectorConsumer::MatrixTakerPtr(CMatrix34 *pMtx)
//{ 
//	pMtx->v1._x = 111;
//	return (int) (pMtx->v1._x + pMtx->v2._z);
//}
//
//int VectorConsumer::MatrixTakerRef(CMatrix34 &rMtx)
//{ 
//	rMtx.v2._y = 222;
//	return (int) (rMtx.v1._x + rMtx.v2._z);
//}

/*
int VectorConsumer::MatrixTakerVal(CMatrix34 mtx)
{ 
	return (int) (mtx.v1._x + mtx.v2._z);
}
*/

//CMatrix34 VectorConsumer::GimmeSomeMatrix() const
//{
//	/* // UDT style initialization
//	CMatrix34 m = CMatrix34(
//		CVector3(41, 42, 43),
//		CVector3(11, 12, 13),
//		CVector3(21, 22, 23),
//		CVector3(31, 32, 33)
//	); */
//
//	// POD style initialization
//	CMatrix34 m = 
//	{
//		{41, 42, 43},
//		{11, 12, 13},
//		{21, 22, 23},
//		{31, 32, 33}
//	};
//
//	return m;
//}

CVector3 VectorConsumer::GimmeSomeVector()
{
	// UDT style initialization
	CVector3 v = CVector3(11, 12, 13);

	// POD style initialization
	// CVector3 v = {11, 12, 13};

	return v;
}


void VectorConsumerCaller::CallVectorConsumer(VectorConsumer *pConsumer)
{
	/* // UDT style initialization
	CVector3 v3 = CVector3(11, 22, 33);
	CVector4 v4 = {1, 2, 3, 4};
	CMatrix34 m = CMatrix34(
		CVector3(41, 42, 43),
		CVector3(11, 12, 13),
		CVector3(21, 22, 23),
		CVector3(31, 32, 33)
	);
	*/

	// POD style initialization
	CVector3 v3 = CVector3(11, 22, 33);
	CVector4 v4 = {1, 2, 3, 4};
	//CMatrix34 m = 
	//{
	//	{41, 42, 43},
	//	{11, 12, 13},
	//	{21, 22, 23},
	//	{31, 32, 33}
	//};

	int res;
	res = pConsumer->VectorTakerPtr3(&v3);
	res = pConsumer->VectorTakerRef3(v3);
	res = pConsumer->VectorTakerVal3(v3);
	CVector3 vRet = pConsumer->GimmeSomeVector();

	res = pConsumer->VectorTakerPtr4(&v4);
	res = pConsumer->VectorTakerRef4(v4);
	res = pConsumer->VectorTakerVal4(v4);

	//res = pConsumer->MatrixTakerPtr(&m);
	//res = pConsumer->MatrixTakerRef(m);
	// res = pConsumer->MatrixTakerVal(m);
}

