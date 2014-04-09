/////////////////////////////////////////////////////////////
// CINEMA 4D SDK                                           //
/////////////////////////////////////////////////////////////
// (c) MAXON Computer GmbH, all rights reserved            //
/////////////////////////////////////////////////////////////

#ifndef _C4D_GEDATA_H_
#define _C4D_GEDATA_H_

#ifdef __API_INTERN__
abc def xyz
#endif

#include "ge_math.h"
#include "operatingsystem.h"
#include "c4d_string.h"

class C4DAtomGoal;
class BaseContainer;
class BaseTime;
class String;
class Filename;
class AliasTrans;
class BaseLink;
class BaseList2D;
class BaseDocument;
struct CustomDataType;

enum
{
	DA_NIL						= 0,
	DA_VOID						= 14,
	DA_LONG						= 15,
	DA_REAL						= 19,
	DA_TIME						= 22,
	DA_VECTOR					= 23,
	DA_MATRIX					= 25,
	DA_LLONG					= 26,
	DA_BYTEARRAY			= 128,	// mainly for quicktime
	DA_STRING					= 130,
	DA_FILENAME				= 131,
	DA_CONTAINER			= 132,			// BaseContainer
	DA_ALIASLINK			= 133,			// for alias links -> new in 7300
	DA_MARKER					= 256,
	DA_MISSINGPLUG		= 257,			// missing datatype plugin

	DA_CUSTOMDATATYPE = 1000000,	// DataTypes > 1000000 are custom

	DA_END
};

enum DEFAULTVALUETYPE
{
	DEFAULTVALUE
};

enum VOIDVALUETYPE
{
	VOIDVALUE
};

enum LLONGVALUETYPE
{
	LLONGVALUE
};

class GeData
{
private:
	Int32	Type;
	Int32	dummy;	// necessary for Linux alignment of structure

	union
	{
		Int32	DInteger;
		Float	DReal;
		void*	Ddata;
		Int64	DLLong;
	};

public:
	// constructors
	GeData(void)
	{
		Type = DA_NIL;
		DInteger = 0;
	}

	GeData(double n)
	{
		Type	= DA_REAL;
		DReal = (Float)n;
	}

	GeData(const GeData& n)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->CopyData(this, &n, nullptr);
	}

	GeData(Int32 n)
	{
		Type = DA_LONG;
		DInteger = n;
	}

	GeData(Float32 n)
	{
		Type	= DA_REAL;
		DReal = n;
	}

	GeData(void* v, VOIDVALUETYPE dummy)
	{
		Type	= DA_VOID;
		Ddata = v;
	}

	GeData(Int64 v, LLONGVALUETYPE dummy)
	{
		Type = DA_LLONG;
		DLLong = v;
	}

	GeData(const Vector& n)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetVector(this, n);
	}

	GeData(const C4DUuid& n)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetUuid(this, &n);
	}

	// @TODO - fix this
	/* 
	GeData(const Matrix& n)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetMatrix(this, n);
	}
	*/

	GeData(const Char* n)
	{
		String str(n);
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetString(this, &str);
	}

	GeData(const String& n)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetString(this, &n);
	}

	GeData(void* mem, Int count)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetMemory(this, mem, count);
	}

	GeData(const Filename& n)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetFilename(this, &n);
	}

	GeData(const BaseTime& n)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetBaseTime(this, n);
	}

	GeData(const BaseContainer& n)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetBaseContainer(this, &n);
	}

	GeData(const BaseLink* n)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetLink(this, *n);
	}

	GeData(BaseList2D* bl)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetBlLink(this, bl);
	}

	GeData(Int32 type, const CustomDataType& data)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->SetCustomData(this, type, data);
	}

	GeData(Int32 type, DEFAULTVALUETYPE v)
	{
		Type	= DA_NIL;
		Ddata = nullptr;
		C4DOS.Gd->InitCustomData(this, type);
	}

	Bool SetDefault(Int32 type)
	{
		return C4DOS.Gd->InitCustomData(this, type);
	}

	// destructor

	void Free(void)
	{
		C4DOS.Gd->Free(this);
	}

	~GeData(void)
	{
		C4DOS.Gd->Free(this);
	}

	// operators
	const GeData& operator = (const GeData& n)
	{
		n.CopyData(this, nullptr);
		return *this;
	}

	Bool operator == (const GeData& d) const
	{
		return C4DOS.Gd->IsEqual(this, &d);
	}

	Bool operator != (const GeData& d) const
	{
		return !C4DOS.Gd->IsEqual(this, &d);
	}

	Int32 GetType(void) const
	{
		return C4DOS.Gd->GetType(this);
	}

	// take care: the results are only valid as long as the GeData value exists
	Bool GetBool     (void) const	{ return C4DOS.Gd->GetInt32(this) != 0; }
	Int32	GetInt32     (void) const	{ return C4DOS.Gd->GetInt32(this); }
	Int64	GetInt64    (void) const { return C4DOS.Gd->GetInt64(this); }
	Float	GetFloat     (void) const	{	return C4DOS.Gd->GetFloat(this); }
	void*								GetVoid			(void) const { return C4DOS.Gd->GetCustomData(this, DA_VOID);	}
	const Vector&		GetVector   (void) const { return C4DOS.Gd->GetVector(this); }
	const Matrix&		GetMatrix   (void) const { return C4DOS.Gd->GetMatrix(this); }
	const String&		GetString   (void) const { return C4DOS.Gd->GetString(this); }
	const C4DUuid&	GetUuid     (void) const { return C4DOS.Gd->GetUuid(this); }
	const Filename&	GetFilename (void) const { return C4DOS.Gd->GetFilename(this); }
	const BaseTime&	GetTime     (void) const { return C4DOS.Gd->GetTime(this); }
	BaseContainer*			GetContainer(void) const { return C4DOS.Gd->GetContainer(this); }
	BaseLink*						GetBaseLink (void) const { return C4DOS.Gd->GetLink(this); }
	CustomDataType*			GetCustomDataType	(Int32 datatype) const { return C4DOS.Gd->GetCustomData(this, datatype); }
	BaseList2D*					GetLink(const BaseDocument* doc, Int32 instanceof = 0) const;
	C4DAtomGoal*				GetLinkAtom(const BaseDocument* doc, Int32 instanceof = 0) const;
	void*								GetMemoryAndRelease	(Int& count)	{	return C4DOS.Gd->GetMemoryAndRelease(this, count); }
	void*								GetMemory		(Int& count)	{	return C4DOS.Gd->GetMemory(this, count); }

	void CopyData(GeData* dest, AliasTrans* aliastrans) const
	{
		C4DOS.Gd->CopyData(dest, this, aliastrans);
	}

	void SetFloat			 (Float v) { C4DOS.Gd->SetFloat(this, v);	}
	void SetInt32			 (Int32 v) { C4DOS.Gd->SetInt32(this, v);	}
	void SetInt64		 (const Int64& v)	 { C4DOS.Gd->SetInt64(this, v);	}
	void SetVoid		 (void* v) { C4DOS.Gd->SetVoid(this, v); }
	void SetMemory	 (void* data, Int count) { C4DOS.Gd->SetMemory(this, data, count); }
	void SetVector	 (const Vector& v) { C4DOS.Gd->SetVector(this, v); }
	void SetMatrix	 (const Matrix& v) { C4DOS.Gd->SetMatrix(this, v); }
	void SetString	 (const String& v) { C4DOS.Gd->SetString(this, &v);	}
	void SetUuid		 (const C4DUuid& v)	 { C4DOS.Gd->SetUuid(this, &v);	}
	void SetFilename (const Filename& v) { C4DOS.Gd->SetFilename(this, &v);	}
	void SetBaseTime (const BaseTime& v) { C4DOS.Gd->SetBaseTime(this, v); }
	void SetContainer(const BaseContainer& v)	 { C4DOS.Gd->SetBaseContainer(this, &v); }
	void SetBaseLink (const BaseLink& v) { C4DOS.Gd->SetLink(this, v); }
	void SetBaseList2D (BaseList2D* bl)	 { C4DOS.Gd->SetBlLink(this, bl);	}
	void SetCustomDataType(Int32 datatype, const CustomDataType& v)	 { C4DOS.Gd->SetCustomData(this, datatype, v); }
};

class BrowseContainer
{
private:
	BaseContainer* t_bc;
	void*					 handle;

public:
	BrowseContainer(const BaseContainer* bc);
	void Reset(void);
	Bool GetNext(Int32* id, GeData** data);
};

#endif
