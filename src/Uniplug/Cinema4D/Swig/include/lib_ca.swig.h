/////////////////////////////////////////////////////////////
// CINEMA 4D SDK                                           //
/////////////////////////////////////////////////////////////
// (c) MAXON Computer GmbH, all rights reserved            //
/////////////////////////////////////////////////////////////

#ifndef __LIB_CA_H__
#define __LIB_CA_H__

#include "c4d_library.h"
#include "ge_math.h"
#include "lib_description.h"

#ifdef __API_INTERN__

#include "pluginobject.h"
#include "plugintag.h"

#else

#include "c4d_baseobject.h"
#include "c4d_basetag.h"
#include "c4d_tooldata.h"
#include "c4d_descriptiondialog.h"

#endif

class MirrorTransformData
{
public:
	Vector m_Mp,m_Mn;
	Matrix m_Mg,m_Mi;
	const BaseContainer *m_pData;
};

#define MSG_MIRROR_TRANSFORM 1025416
#define MIRROR_TRANSFORM_CONTAINER 1025418

#define MIRROR_TRANSFORM_CONTAINER_MATRIX		1000	// origin mg
#define MIRROR_TRANSFORM_CONTAINER_SOURCE		1001	// origin object link
#define MIRROR_TRANSFORM_CONTAINER_LMATRIXN	1002	// origin rel mln
#define MIRROR_TRANSFORM_CONTAINER_LMATRIX	1003	// origin rel ml
#define MIRROR_TRANSFORM_CONTAINER_FMATRIX	1004	// origin frozen ml
#define MIRROR_TRANSFORM_CONTAINER_POS			1005	// origin rel pos
#define MIRROR_TRANSFORM_CONTAINER_ROT			1006	// origin rel rot
#define MIRROR_TRANSFORM_CONTAINER_SCL			1007	// origin rel scale
#define MIRROR_TRANSFORM_CONTAINER_FPOS			1008	// origin frozen pos
#define MIRROR_TRANSFORM_CONTAINER_FROT			1009	// origin frozen rot
#define MIRROR_TRANSFORM_CONTAINER_FSCL			1010	// origin frozen scale

class Neighbor;

struct JointRestState
{
	JointRestState() { m_Len=0.0; }
	Matrix m_bMg,m_bMi;	// bone rest state
	Matrix m_oMg,m_oMi;	// object rest state
	Real m_Len;			// bone rest length
};

class CAWeightTag;

class CAJointObject : public BaseObject
{
#ifndef __API_INTERN__
private:
	CAJointObject();
	~CAJointObject();
public:
	static CAJointObject *Alloc() { return (CAJointObject*)BaseObject::Alloc(Ojoint); }
	static void Free(CAJointObject *&pObject) { BaseObject *op=pObject; BaseObject::Free(op); pObject=NULL; }
#else
public:
#endif
	void GetBone(Matrix &m, Real &len);
	CAWeightTag *GetWeightTag(LONG &index);
};

class CAWeightTag : public BaseTag
{
#ifndef __API_INTERN__
private:
	CAWeightTag();
	~CAWeightTag();
public:
	static CAWeightTag *Alloc() { return (CAWeightTag*)BaseTag::Alloc(Tweights); }
	static void Free(CAWeightTag *&pTag) { BaseTag *tag=pTag; BaseTag::Free(tag); pTag=NULL; }
#else
public:
#endif

	BaseObject *GetJoint(LONG index, BaseDocument *doc);	// get joint object at 'index', doc can be NULL
	LONG GetJointCount();									// get total joints
	LONG FindJoint(BaseObject *op, BaseDocument *doc);		// return the index of this object or NOTOK if not found, doc can be NULL

	JointRestState GetJointRestState(LONG index);	// get the rest state for the joint at 'index'
	void SetJointRestState(LONG index, const JointRestState &state);	// set the rest state for the joint

	void GetWeightMap(LONG index, SReal *map, LONG cnt);	// fill in the weights to 'map' that must be allocated with 'cnt' (this should be the point count)
	Bool SetWeightMap(LONG index, SReal *map, LONG cnt); // set the entire weight map using 'map'

	LONG GetWeightCount(LONG index);	// get total stored weights, zero weights are not stored
	void GetIndexWeight(LONG index, LONG windex, LONG &pntindex, Real &weight);	// get the windex weight and which point index it is for plus the weight

	Real GetWeight(LONG index, LONG pntindex);	// return the weight for the point pntindex
	Bool SetWeight(LONG index, LONG pntindex, Real weight); // set the weight for pntindex

	ULONG GetWeightDirty();	// get the dirty state of the weights
	void WeightDirty();		// make the weights dirty

	Matrix GetGeomMg();		// get the global matrix for the bind geometry (use this with the global matrices of the joints to get the local transforms)
	void SetGeomMg(const Matrix &mg);	// set the global matrix for the bind geom

	LONG AddJoint(BaseObject *op);	// add joint binding
	void RemoveJoint(BaseObject *op); // remove joint from binding

	void CalculateBoneStates(LONG index);	// calculate JointRestState bone state (m_bMg, m_bMi, m_Len) from m_oMg, use index as NOTOK to do all binds

	Bool TransferWeightMap(BaseDocument *doc, CAWeightTag *dst, LONG sindex, LONG dindex, LONG offset, LONG cnt, AliasTrans *trans); // transfer map sindex to dindex (or all if NOTOK) using point offset and count (or NOTOK for all indexes)
};

//////////////////////////////////////////////////////////////////////////

class CAPoseMorphTag;

enum CAMORPH_COPY_FLAGS
{
	CAMORPH_COPY_FLAGS_0=0
} ENUM_END_FLAGS(CAMORPH_COPY_FLAGS);

enum CAMORPH_MODE_FLAGS
{
	CAMORPH_MODE_FLAGS_COLLAPSE=2048,
	CAMORPH_MODE_FLAGS_EXPAND=4096,
	CAMORPH_MODE_FLAGS_ALL=1007,
	//////////////////////////////////////////////////////////////////////////
	CAMORPH_MODE_FLAGS_0=0
} ENUM_END_FLAGS(CAMORPH_MODE_FLAGS);

enum CAMORPH_MODE
{
	CAMORPH_MODE_ABS = 0,
	CAMORPH_MODE_REL,
	CAMORPH_MODE_ROT,
	CAMORPH_MODE_CORRECTIONAL,
	CAMORPH_MODE_CORRECTIONAL_AREA,
	CAMORPH_MODE_AUTO=-1,
	//////////////////////////////////////////////////////////////////////////
	CAMORPH_MODE_0=0
} ENUM_END_LIST(CAMORPH_MODE);

enum CAMORPH_DATA_FLAGS
{
	CAMORPH_DATA_FLAGS_P=(1<<0),
	CAMORPH_DATA_FLAGS_S=(1<<1),
	CAMORPH_DATA_FLAGS_R=(1<<2),
	CAMORPH_DATA_FLAGS_POINTS=(1<<3),
	CAMORPH_DATA_FLAGS_TANGETS=(1<<4),
	CAMORPH_DATA_FLAGS_VERTEXMAPS=(1<<5),
	CAMORPH_DATA_FLAGS_WEIGHTMAPS=(1<<6),
	CAMORPH_DATA_FLAGS_PARAMS=(1<<7),
	CAMORPH_DATA_FLAGS_USERDATA=(1<<8),
	CAMORPH_DATA_FLAGS_UV=(1<<9),
	//////////////////////////////////////////////////////////////////////////
	CAMORPH_DATA_FLAGS_ASTAG=(1<<15),
	CAMORPH_DATA_FLAGS_ALL=((1<<0)|(1<<1)|(1<<2)|(1<<3)|(1<<4)|(1<<5)|(1<<6)|(1<<7)|(1<<8)|(1<<9)),
	//////////////////////////////////////////////////////////////////////////
	CAMORPH_DATA_FLAGS_0=0
} ENUM_END_FLAGS(CAMORPH_DATA_FLAGS);

class CAMorph;

class CAMorphNode
{
#ifndef __API_INTERN__
private:
	CAMorphNode();
	~CAMorphNode();
#endif
public:
	CAMorphNode *GetNext();
	CAMorphNode *GetPrev();
	CAMorphNode *GetUp();
	CAMorphNode *GetDown();
	BaseList2D *GetLink(CAPoseMorphTag *tag, CAMorph *morph, BaseDocument *doc);

	CAMORPH_DATA_FLAGS GetInfo();

	Vector GetP();
	Vector GetS();
	Vector GetR();

	void SetP(const Vector &p);
	void SetS(const Vector &s);
	void SetR(const Vector &r);

	LONG GetPointCount();
	Bool SetPointCount(LONG cnt);
	Vector GetPoint(LONG index);
	void SetPoint(LONG index, const Vector &pnt);

	LONG GetTangentCount();
	Bool SetTangentCount(LONG cnt);
	Vector GetTangent(LONG index);
	void SetTangent(LONG index, const Vector &v);

	LONG GetVertexMapTagCount();
	LONG GetVertexMapCount(LONG tindex);
	Bool SetVertexMapCount(LONG tindex, LONG cnt);
	Real GetVertexMap(LONG tindex, LONG index);
	void SetVertexMap(LONG tindex, LONG index, Real v);

	LONG GetParamCount();
	Bool SetParamCount(LONG cnt);
	Bool GetParam(LONG index, GeData &data, DescID &id);
	void SetParam(LONG index, const GeData &data, const DescID &id);

	LONG GetUVTagCount();
	LONG GetUVCount(LONG tindex);
	Bool SetUVCount(LONG tindex, LONG cnt);
	void GetUV(LONG tindex, LONG index, UVWStruct &uv);
	void SetUV(LONG tindex, LONG index, const UVWStruct &uv);

	LONG GetWeightMapTagCount();
	LONG GetWeightMapJointCount(LONG tindex);
	LONG GetWeightMapCount(LONG tindex, LONG jindex);
	Bool SetWeightMapCount(LONG tindex, LONG jindex, LONG cnt);
	Real GetWeightMap(LONG tindex, LONG jindex, LONG index);
	void SetWeightMap(LONG tindex, LONG jindex, LONG index, Real v);
};

class CAMorph
{
#ifndef __API_INTERN__
private:
	CAMorph();
	~CAMorph();
#endif
public:
	String GetName();
	void SetName(const String &name);
	LONG GetID();
	Bool CopyFrom(CAMorph *src, AliasTrans *trn, CAMORPH_COPY_FLAGS flags);
	CAMorphNode *Find(CAPoseMorphTag *tag, BaseList2D *bl);
	LONG GetNodeIndex(CAMorphNode *node);
	LONG FindIndex(CAPoseMorphTag *tag, BaseList2D *bl);
	CAMorphNode *FindFromIndex(CAPoseMorphTag *tag, LONG index);
	CAMorphNode *GetFirst();
	Bool SetMode(BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_MODE_FLAGS flags, CAMORPH_MODE mode);
	Bool Store(BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_DATA_FLAGS flags);
	Bool Apply(BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_DATA_FLAGS flags);
};

class CAPoseMorphTag : public BaseTag
{
#ifndef __API_INTERN__
private:
	CAPoseMorphTag();
	~CAPoseMorphTag();
public:
	static CAPoseMorphTag *Alloc() { return (CAPoseMorphTag*)BaseTag::Alloc(Tposemorph); }
	static void Free(CAPoseMorphTag *&pTag) { BaseTag *tag=pTag; BaseTag::Free(tag); pTag=NULL; }
#else
public:
#endif

	LONG GetMorphCount();
	CAMorph *GetMorph(LONG index);
	DescID GetMorphID(LONG index);
	LONG GetActiveMorphIndex();
	LONG GetMode();
	CAMorph *GetActiveMorph() { return GetMorph(GetActiveMorphIndex()); }
	CAMorph *GetMorphBase() { return GetMorph(0); }
	CAMorph *AddMorph();
	void RemoveMorph(LONG index);
	void InitMorphs();
	void UpdateMorphs();
	LONG GetMorphIndex(CAMorph *morph);
	Bool ExitEdit(BaseDocument *doc, Bool apply);
};

//////////////////////////////////////////////////////////////////////////

#define BRUSHBASE_MOUSE_FLAG_ADDUNDO				(1<<0)
#define BRUSHBASE_MOUSE_FLAG_ADDUNDO_FULL			(1<<1)
#define BRUSHBASE_MOUSE_FLAG_SORTED_DIST			(1<<2)
#define BRUSHBASE_MOUSE_FLAG_SORTED_OBJECT			(1<<3)
#define BRUSHBASE_MOUSE_FLAG_SORTED_ORIGINOBJECT	(1<<4)

#define BRUSHBASE_FALLOFF_STRENGTH			(1<<0)
#define BRUSHBASE_FALLOFF_ABSSTRENGTH		(1<<1)

class _BrushToolBase;
class iBrushBase;
class BrushObjectData;
class BrushToolData;

class BrushObjectInfo
{
public:
	BrushObjectInfo()
	{
		m_pObject=NULL;
		m_pOriginObject=NULL;
		m_pDeformObject=NULL;
		m_pNeighbor=NULL;
		m_pPoints=NULL;
		m_pGlobalPoints=NULL;
		m_pNormals=NULL;
		m_pPolys=NULL;
		m_PointCount=0;
		m_PolyCount=0;
	}

	BaseObject *m_pObject;
	BaseObject *m_pOriginObject;
	BaseObject *m_pDeformObject;

	Neighbor *m_pNeighbor;
	const Vector *m_pPoints;
	Vector *m_pGlobalPoints,*m_pNormals;
	const CPolygon *m_pPolys;
	LONG m_PointCount,m_PolyCount;
};

class BrushVertexData
{
public:
	Real m_Dist;
	LONG m_Index;
	BrushObjectData *m_pObject;
};

class BrushPixelData
{
public:
	BrushObjectData *m_pObject;
	LONG m_Index;
	Real m_Z;
	BrushPixelData *m_pNext;
};

class BrushBase
{
private:

	BrushBase();
	~BrushBase();

	_BrushToolBase *m_pBase;

public:

	static BrushBase *Alloc();
	static void Free(BrushBase *&p);

	Bool InitTool(BaseDocument* doc, BaseContainer& data, BaseThread* bt, BrushToolData *tool);
	void FreeTool(BaseDocument* doc, BaseContainer& data);
	void InitDefaultSettings(BaseDocument *doc, BaseContainer &data);
	Bool GetDEnabling(BaseDocument* doc, BaseContainer& data, const DescID& id, const GeData& t_data, DESCFLAGS_ENABLE flags, const BaseContainer* itemdesc);
	Bool SetDParameter(BaseDocument* doc, BaseContainer& data, const DescID& id, const GeData& t_data, DESCFLAGS_SET& flags);
	Bool GetDDescription(BaseDocument* doc, BaseContainer& data, Description* description, DESCFLAGS_DESC& flags);
	Bool Message(BaseDocument *doc, BaseContainer &data, LONG type, void *t_data);
	Bool GetCursorInfo(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, Real x, Real y, BaseContainer& bc);
	Bool MouseInput(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg);
	BrushVertexData *GetSelected(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, LONG &vcnt, LONG x, LONG y, Real rad, BaseObject *op);	// must call ValidateObjects before any calls to this
	Real GetCursor(LONG &x, LONG &y);
	void GetObjectInfo(BrushObjectData *data, BrushObjectInfo &info);
	Bool GetObjectInfo(BaseObject *op, BrushObjectInfo &info);
	Bool ValidateObjects(BaseDocument *doc, BaseContainer& data);
	BrushPixelData *GetObjectAt(LONG x, LONG y);	// must call ValidateObjects before any calls to this
	Real GetFalloff(Real dst, LONG flags);	// only valid within mouseinput
	Bool GetObjects(BaseDocument *doc, AtomArray *objects);
	Bool UpdateCache(BaseDocument *doc, BaseContainer& data, BaseDraw *bd, Bool force);
	Real *CalcSurfaceDistances(PolygonObject *pObject, BaseSelect *selected, Neighbor *pNeighbor=NULL, Vector *pNormals=NULL, Vector *pGlobalPoints=NULL, Real *pDistance=NULL);
	Real *CalcSurfaceDistancesFromPoint(PolygonObject *pObject, LONG pindex, Neighbor *pNeighbor=NULL, Vector *pNormals=NULL, Vector *pGlobalPoints=NULL, Real *pDistance=NULL);
};

#ifndef __API_INTERN__

class BrushToolData : public DescriptionToolData
{
	// INSTANCEOF(BrushToolData,DescriptionToolData)

public:

	// BrushToolData() { m_pBrushBase=NULL; }
	// ~BrushToolData() { BrushBase::Free(m_pBrushBase); }

	BrushBase *m_pBrushBase;

	//////////////////////////////////////////////////////////////////////////

	virtual Bool InitTool(BaseDocument* doc, BaseContainer& data, BaseThread* bt);
	virtual void FreeTool(BaseDocument* doc, BaseContainer& data);
	virtual void InitDefaultSettings(BaseDocument *doc, BaseContainer &data);
	virtual Bool GetDEnabling(BaseDocument* doc, BaseContainer& data, const DescID& id, const GeData& t_data, DESCFLAGS_ENABLE flags, const BaseContainer* itemdesc);
	virtual Bool SetDParameter(BaseDocument* doc, BaseContainer& data, const DescID& id, const GeData& t_data, DESCFLAGS_SET& flags);
	virtual Bool GetDDescription(BaseDocument* doc, BaseContainer& data, Description* description, DESCFLAGS_DESC& flags);
	virtual Bool Message(BaseDocument *doc, BaseContainer &data, LONG type, void *t_data);
	virtual Bool GetCursorInfo(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, Real x, Real y, BaseContainer& bc);
	virtual Bool MouseInput(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg);

	//////////////////////////////////////////////////////////////////////////

	virtual Bool MouseInputStart(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg, LONG &flags) { return TRUE; }
	virtual Bool MouseInputDrag(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg, BrushVertexData* vdata, LONG vcnt, Real x, Real y, LONG &flags) { return TRUE; }
	virtual Bool MouseInputEnd(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg) { return TRUE; }

	// Implement these methods declared in DescriptionToolData. Otherwise BrushToolData stays an abstract class and the Swig-generated instantiations result in c++ compiler errors
	// virtual LONG			GetToolPluginId()  { return 666; } // The number of the beast
	// virtual const String	GetResourceSymbol() { return "THIS SHOULD NEVER HAPPEN"; }

};

#endif

//////////////////////////////////////////////////////////////////////////

// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF

////////////////////////////////////////////////////////////////////////////
//#define LIBRARY_CA		1019742
//#define LIBRARY_BRUSH	1019809
////////////////////////////////////////////////////////////////////////////
//
//struct CALibrary : public C4DLibrary
//{
//	//////////////////////////////////////////////////////////////////////////
//	// Weight Tag
//
//	BaseObject *(*weightGetJoint)(CAWeightTag *tag, LONG index, BaseDocument *doc);
//	LONG (*weightGetJointCount)(CAWeightTag *tag);
//	LONG (*weightFindJoint)(CAWeightTag *tag, BaseObject *op, BaseDocument *doc);
//	JointRestState (*weightGetJointRestState)(CAWeightTag *tag, LONG index);
//	void (*weightSetJointRestState)(CAWeightTag *tag, LONG index, const JointRestState &state);
//	void (*weightGetWeightMap)(CAWeightTag *tag, LONG index, SReal *map, LONG cnt);
//	Bool (*weightSetWeightMap)(CAWeightTag *tag, LONG index, SReal *map, LONG cnt);
//	LONG (*weightGetWeightCount)(CAWeightTag *tag, LONG index);
//	void (*weightGetIndexWeight)(CAWeightTag *tag, LONG index, LONG windex, LONG &pntindex, Real &weight);
//	Real (*weightGetWeight)(CAWeightTag *tag, LONG index, LONG pntindex);
//	Bool (*weightSetWeight)(CAWeightTag *tag, LONG index, LONG pntindex, Real weight);
//	ULONG (*weightGetDirty)(CAWeightTag *tag);
//	void (*weightDirty)(CAWeightTag *tag);
//
//	//////////////////////////////////////////////////////////////////////////
//	// Joint Object
//
//	void (*jointGetBone)(CAJointObject *op, Matrix &m, Real &len);
//	CAWeightTag *(*jointGetWeightTag)(CAJointObject *op, LONG &index);
//
//	//////////////////////////////////////////////////////////////////////////
//	// Weight Tag
//
//	Matrix (*weightGetGeomMg)(CAWeightTag *tag);
//	void (*weightSetGeomMg)(CAWeightTag *tag, const Matrix &mg);
//	LONG (*weightAddJoint)(CAWeightTag *tag, BaseObject *op);
//	void (*weightRemoveJoint)(CAWeightTag *tag, BaseObject *op);
//	void (*weightCalculateBoneStates)(CAWeightTag *tag, LONG index);
//	Bool (*weightTransferWeightMap)(CAWeightTag *tag, BaseDocument *doc, CAWeightTag *dst, LONG sindex, LONG dindex, LONG offset, LONG cnt, AliasTrans *trans);
//
//	//////////////////////////////////////////////////////////////////////////
//	
//	void (*docGetOrderedActiveObjects)(const BaseDocument *doc, AtomArray *objs);
//
//	//////////////////////////////////////////////////////////////////////////
//	// Pose Morph Tag
//
//	LONG (*pmorphGetCount)(CAPoseMorphTag *tag);
//	CAMorph *(*pmorphGetMorph)(CAPoseMorphTag *tag, LONG index);
//	DescID (*pmorphGetDescID)(CAPoseMorphTag *tag, LONG index);
//	LONG (*pmorphGetMode)(CAPoseMorphTag *tag);
//	LONG (*pmorphGetActive)(CAPoseMorphTag *tag);
//	CAMorph *(*pmorphAdd)(CAPoseMorphTag *tag);
//	void (*pmorphRemove)(CAPoseMorphTag *tag, LONG index);
//	void (*pmorphInitMorphs)(CAPoseMorphTag *tag);
//	void (*pmorphUpdateMorphs)(CAPoseMorphTag *tag);
//	LONG (*pmorphGetMorphIndex)(CAPoseMorphTag *tag, CAMorph *morph);
//	Bool (*pmorphExitEdit)(CAPoseMorphTag *tag, BaseDocument *doc, Bool apply);
//	//////////////////////////////////////////////////////////////////////////
//	String (*pmorphGetName)(CAMorph *morph);
//	LONG (*pmorphGetID)(CAMorph *morph);
//	void (*pmorphSetName)(CAMorph *morph, const String &name);
//	Bool (*pmorphCopyFrom)(CAMorph *morph, CAMorph *src, AliasTrans *trn, CAMORPH_COPY_FLAGS flags);
//	CAMorphNode *(*pmorphFind)(CAMorph *morph, CAPoseMorphTag *tag, BaseList2D *bl);
//	CAMorphNode *(*pmorphGetFirst)(CAMorph *morph);
//	Bool (*pmorphSetMode)(CAMorph *morph, BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_MODE_FLAGS flags, CAMORPH_MODE mode);
//	LONG (*pmorphFindIndex)(CAMorph *morph, CAPoseMorphTag *tag, BaseList2D *bl);
//	CAMorphNode *(*pmorphFindFromIndex)(CAMorph *morph, CAPoseMorphTag *tag, LONG index);
//	Bool (*pmorphStore)(CAMorph *morph, BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_DATA_FLAGS flags);
//	Bool (*pmorphApply)(CAMorph *morph, BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_DATA_FLAGS flags);
//	LONG (*pmorphGetNodeIndex)(CAMorph *morph, CAMorphNode *node);
//	//////////////////////////////////////////////////////////////////////////
//	CAMorphNode *(*pmorphnodeGetNext)(CAMorphNode *node);
//	CAMorphNode *(*pmorphnodeGetPrev)(CAMorphNode *node);
//	CAMorphNode *(*pmorphnodeGetUp)(CAMorphNode *node);
//	CAMorphNode *(*pmorphnodeGetDown)(CAMorphNode *node);
//	BaseList2D *(*pmorphnodeGetLink)(CAMorphNode *node, CAPoseMorphTag *tag, CAMorph *morph, BaseDocument *doc);
//	LONG (*pmorphnodeGetPointCount)(CAMorphNode *node);
//	Vector (*pmorphnodeGetPoint)(CAMorphNode *node, LONG index);
//	void (*pmorphnodeSetPoint)(CAMorphNode *node, LONG index, const Vector &pnt);
//	LONG (*pmorphGetTangentCount)(CAMorphNode *node);
//	Vector (*pmorphGetTangent)(CAMorphNode *node, LONG index);
//	void (*pmorphSetTangent)(CAMorphNode *node, LONG index, const Vector &v);
//	LONG (*pmorphGetVertexMapTagCount)(CAMorphNode *node);
//	LONG (*pmorphGetVertexMapCount)(CAMorphNode *node, LONG tindex);
//	Real (*pmorphGetVertexMap)(CAMorphNode *node, LONG tindex, LONG index);
//	void (*pmorphSetVertexMap)(CAMorphNode *node, LONG tindex, LONG index, Real v);
//	Bool (*pmorphSetVertexMapCount)(CAMorphNode *node, LONG tindex, LONG cnt);
//	Bool (*pmorphSetPointCount)(CAMorphNode *node, LONG cnt);
//	Bool (*pmorphSetTangentCount)(CAMorphNode *node, LONG cnt);
//	CAMORPH_DATA_FLAGS (*pmorphGetInfo)(CAMorphNode *node);
//	Vector (*pmorphGetP)(CAMorphNode *node);
//	Vector (*pmorphGetS)(CAMorphNode *node);
//	Vector (*pmorphGetR)(CAMorphNode *node);
//	void (*pmorphSetP)(CAMorphNode *node, const Vector &p);
//	void (*pmorphSetS)(CAMorphNode *node, const Vector &s);
//	void (*pmorphSetR)(CAMorphNode *node, const Vector &r);
//	LONG (*pmorphGetParamCount)(CAMorphNode *node);
//	Bool (*pmorphSetParamCount)(CAMorphNode *node, LONG cnt);
//	Bool (*pmorphGetParam)(CAMorphNode *node, LONG index, GeData &data, DescID &id);
//	void (*pmorphSetParam)(CAMorphNode *node, LONG index, const GeData &data, const DescID &id);
//	LONG (*pmorphGetUVTagCount)(CAMorphNode *node);
//	LONG (*pmorphGetUVCount)(CAMorphNode *node, LONG tindex);
//	Bool (*pmorphSetUVCount)(CAMorphNode *node, LONG tindex, LONG cnt);
//	void (*pmorphGetUV)(CAMorphNode *node, LONG tindex, LONG index, UVWStruct &uv);
//	void (*pmorphSetUV)(CAMorphNode *node, LONG tindex, LONG index, const UVWStruct &uv);
//	LONG (*pmorphGetWeightMapTagCount)(CAMorphNode *node);
//	LONG (*pmorphGetWeightMapJointCount)(CAMorphNode *node, LONG tindex);
//	LONG (*pmorphGetWeightMapCount)(CAMorphNode *node, LONG tindex, LONG jindex);
//	Bool (*pmorphSetWeightMapCount)(CAMorphNode *node, LONG tindex, LONG jindex, LONG cnt);
//	Real (*pmorphGetWeightMap)(CAMorphNode *node, LONG tindex, LONG jindex, LONG index);
//	void (*pmorphSetWeightMap)(CAMorphNode *node, LONG tindex, LONG jindex, LONG index, Real v);
//};
//
//struct BrushBaseLibrary : public C4DLibrary
//{
//	iBrushBase *(*Alloc)();
//	void (*Free)(iBrushBase *&p);
//
//	Bool (iBrushBase::*InitTool)(BaseDocument* doc, BaseContainer& data, BaseThread* bt, BrushToolData *tool);
//	void (iBrushBase::*FreeTool)(BaseDocument* doc, BaseContainer& data);
//	void (iBrushBase::*InitDefaultSettings)(BaseDocument *doc, BaseContainer &data);
//	Bool (iBrushBase::*GetDEnabling)(BaseDocument* doc, BaseContainer& data, const DescID& id, const GeData& t_data, DESCFLAGS_ENABLE flags, const BaseContainer* itemdesc);
//	Bool (iBrushBase::*SetDParameter)(BaseDocument* doc, BaseContainer& data, const DescID& id, const GeData& t_data, DESCFLAGS_SET& flags);
//	Bool (iBrushBase::*GetDDescription)(BaseDocument* doc, BaseContainer& data, Description* description, DESCFLAGS_DESC& flags);
//	Bool (iBrushBase::*Message)(BaseDocument *doc, BaseContainer &data, LONG type, void *t_data);
//	Bool (iBrushBase::*GetCursorInfo)(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, Real x, Real y, BaseContainer& bc);
//	Bool (iBrushBase::*MouseInput)(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg);
//	BrushVertexData *(iBrushBase::*GetSelected)(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, LONG &vcnt, LONG x, LONG y, Real rad, BaseObject *op);
//	Real (iBrushBase::*GetCursor)(LONG &x, LONG &y);
//	void (iBrushBase::*GetObjectInfo)(BrushObjectData *data, BrushObjectInfo &info);
//	Bool (iBrushBase::*GetObjectInfoOp)(BaseObject *op, BrushObjectInfo &info);
//	Bool (iBrushBase::*ValidateObjects)(BaseDocument *doc, BaseContainer& data);
//	BrushPixelData *(iBrushBase::*GetObjectAt)(LONG x, LONG y);
//	Real (iBrushBase::*GetFalloff)(Real dst, LONG flags);
//	Bool (iBrushBase::*GetObjects)(BaseDocument *doc, AtomArray *objects);
//	Bool (iBrushBase::*UpdateCache)(BaseDocument *doc, BaseContainer& data, BaseDraw *bd, Bool force);
//	Real *(iBrushBase::*CalcSurfaceDistances)(PolygonObject *pObject, BaseSelect *selected, Neighbor *pNeighbor, Vector *pNormals, Vector *pGlobalPoints, Real *pDistance);
//	Real *(iBrushBase::*CalcSurfaceDistancesFromPoint)(PolygonObject *pObject, LONG pindex, Neighbor *pNeighbor, Vector *pNormals, Vector *pGlobalPoints, Real *pDistance);
//};

// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF

#endif	// __LIB_CA_H__
