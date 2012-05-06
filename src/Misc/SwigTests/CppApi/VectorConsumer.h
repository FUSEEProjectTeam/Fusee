#pragma once
#include "VectorAndMatrix4.h"

class CPPAPI_API VectorConsumer
{
public:
	VectorConsumer(void);
	~VectorConsumer(void);

	virtual int VectorTakerPtr3(CVector3 *pVec);
	virtual int VectorTakerRef3(CVector3 &rVec);
	virtual int VectorTakerVal3(CVector3 vec);
	virtual CVector3 GimmeSomeVector();

	CVector3 VV;

	virtual int VectorTakerPtr4(CVector4 *pVec);
	virtual int VectorTakerRef4(CVector4 &rVec);
	virtual int VectorTakerVal4(CVector4 vec);

	//virtual int MatrixTakerPtr(CMatrix34 *pMtx);
	//virtual int MatrixTakerRef(CMatrix34 &rMtx);
	//virtual int MatrixTakerVal(CMatrix34 mtx);
	//CMatrix34 GimmeSomeMatrix() const;
};

class CPPAPI_API VectorConsumerCaller
{
public:
	static void CallVectorConsumer(VectorConsumer *pConsumer);
};
