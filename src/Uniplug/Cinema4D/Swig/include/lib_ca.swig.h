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

void RemoveXRefData(BaseDocument *doc, BaseList2D *bl); // private
Bool HasDocumentXRefs(BaseDocument *doc); // private
Int32 GetDocumentXRefState(); // private
UInt64 GetXRefID(BaseList2D *bl); // private
Bool XRefHasParam(BaseDocument *doc, BaseList2D *bl, const DescID &id); // private
Bool XRefGetParam(BaseDocument *doc, BaseList2D *bl, const DescID &id, GeData &dat); // private
Bool XRefRemoveParam(BaseDocument *doc, BaseList2D *bl, const DescID &id); // private

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
	Float m_Len;			// bone rest length
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
	static void Free(CAJointObject *&pObject) { BaseObject *op=pObject; BaseObject::Free(op); pObject=nullptr; }
#else
public:
#endif
	void GetBone(Matrix &m, Float &len);
	CAWeightTag *GetWeightTag(Int32 &index);
};

class CAWeightTag : public BaseTag
{
#ifndef __API_INTERN__
private:
	CAWeightTag();
	~CAWeightTag();
public:
	static CAWeightTag *Alloc() { return (CAWeightTag*)BaseTag::Alloc(Tweights); }
	static void Free(CAWeightTag *&pTag) { BaseTag *tag=pTag; BaseTag::Free(tag); pTag=nullptr; }
#else
public:
#endif

	BaseObject *GetJoint(Int32 index, BaseDocument *doc);	// get joint object at 'index', doc can be nullptr
	Int32 GetJointCount();									// get total joints
	Int32 FindJoint(BaseObject *op, BaseDocument *doc);		// return the index of this object or NOTOK if not found, doc can be nullptr

	JointRestState GetJointRestState(Int32 index);	// get the rest state for the joint at 'index'
	void SetJointRestState(Int32 index, const JointRestState &state);	// set the rest state for the joint

	void GetWeightMap(Int32 index, Float32 *map, Int32 cnt);	// fill in the weights to 'map' that must be allocated with 'cnt' (this should be the point count)
	Bool SetWeightMap(Int32 index, Float32 *map, Int32 cnt); // set the entire weight map using 'map'

	Int32 GetWeightCount(Int32 index);	// get total stored weights, zero weights are not stored
	void GetIndexWeight(Int32 index, Int32 windex, Int32 &pntindex, Float &weight);	// get the windex weight and which point index it is for plus the weight

	Float GetWeight(Int32 index, Int32 pntindex);	// return the weight for the point pntindex
	Bool SetWeight(Int32 index, Int32 pntindex, Float weight); // set the weight for pntindex

	UInt32 GetWeightDirty();	// get the dirty state of the weights
	void WeightDirty();		// make the weights dirty

	Matrix GetGeomMg();		// get the global matrix for the bind geometry (use this with the global matrices of the joints to get the local transforms)
	void SetGeomMg(const Matrix &mg);	// set the global matrix for the bind geom

	Int32 AddJoint(BaseObject *op);	// add joint binding
	void RemoveJoint(BaseObject *op); // remove joint from binding

	void CalculateBoneStates(Int32 index);	// calculate JointRestState bone state (m_bMg, m_bMi, m_Len) from m_oMg, use index as NOTOK to do all binds

	Bool TransferWeightMap(BaseDocument *doc, CAWeightTag *dst, Int32 sindex, Int32 dindex, Int32 offset, Int32 cnt, AliasTrans *trans); // transfer map sindex to dindex (or all if NOTOK) using point offset and count (or NOTOK for all indexes)
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

	Int32 GetPointCount();
	Bool SetPointCount(Int32 cnt);
	Vector GetPoint(Int32 index);
	void SetPoint(Int32 index, const Vector &pnt);

	Int32 GetTangentCount();
	Bool SetTangentCount(Int32 cnt);
	Vector GetTangent(Int32 index);
	void SetTangent(Int32 index, const Vector &v);

	Int32 GetVertexMapTagCount();
	Int32 GetVertexMapCount(Int32 tindex);
	Bool SetVertexMapCount(Int32 tindex, Int32 cnt);
	Float GetVertexMap(Int32 tindex, Int32 index);
	void SetVertexMap(Int32 tindex, Int32 index, Float v);

	Int32 GetParamCount();
	Bool SetParamCount(Int32 cnt);
	Bool GetParam(Int32 index, GeData &data, DescID &id);
	void SetParam(Int32 index, const GeData &data, const DescID &id);

	Int32 GetUVTagCount();
	Int32 GetUVCount(Int32 tindex);
	Bool SetUVCount(Int32 tindex, Int32 cnt);
	void GetUV(Int32 tindex, Int32 index, UVWStruct &uv);
	void SetUV(Int32 tindex, Int32 index, const UVWStruct &uv);

	Int32 GetWeightMapTagCount();
	Int32 GetWeightMapJointCount(Int32 tindex);
	Int32 GetWeightMapCount(Int32 tindex, Int32 jindex);
	Bool SetWeightMapCount(Int32 tindex, Int32 jindex, Int32 cnt);
	Float GetWeightMap(Int32 tindex, Int32 jindex, Int32 index);
	void SetWeightMap(Int32 tindex, Int32 jindex, Int32 index, Float v);
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
	Int32 GetID();
	Bool CopyFrom(CAMorph *src, AliasTrans *trn, CAMORPH_COPY_FLAGS flags);
	CAMorphNode *Find(CAPoseMorphTag *tag, BaseList2D *bl);
	Int32 GetNodeIndex(CAMorphNode *node);
	Int32 FindIndex(CAPoseMorphTag *tag, BaseList2D *bl);
	CAMorphNode *FindFromIndex(CAPoseMorphTag *tag, Int32 index);
	CAMorphNode *GetFirst();
	Bool SetMode(BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_MODE_FLAGS flags, CAMORPH_MODE mode);
	Bool Store(BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_DATA_FLAGS flags);
	Bool Apply(BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_DATA_FLAGS flags);
	BaseList2D *GetTarget(BaseDocument *doc);
	void SetTarget(CAPoseMorphTag *tag, BaseDocument *doc, BaseList2D *bl);
	void SetStrength(Float strength);
	Float GetStrength();
};

class CAPoseMorphTag : public BaseTag
{
#ifndef __API_INTERN__
private:
	CAPoseMorphTag();
	~CAPoseMorphTag();
public:
	static CAPoseMorphTag *Alloc() { return (CAPoseMorphTag*)BaseTag::Alloc(Tposemorph); }
	static void Free(CAPoseMorphTag *&pTag) { BaseTag *tag=pTag; BaseTag::Free(tag); pTag=nullptr; }
#else
public:
#endif

	Int32 GetMorphCount();
	CAMorph *GetMorph(Int32 index);
	DescID GetMorphID(Int32 index);
	Int32 GetActiveMorphIndex();
	Int32 GetMode();
	CAMorph *GetActiveMorph() { return GetMorph(GetActiveMorphIndex()); }
	CAMorph *GetMorphBase() { return GetMorph(0); }
	CAMorph *AddMorph();
	void RemoveMorph(Int32 index);
	void InitMorphs();
	void UpdateMorphs(BaseDocument *doc = nullptr);
	Int32 GetMorphIndex(CAMorph *morph);
	Bool ExitEdit(BaseDocument *doc, Bool apply);
	void SetActiveMorphIndex(Int32 index);
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
		m_pObject=nullptr;
		m_pOriginObject=nullptr;
		m_pDeformObject=nullptr;
		m_pNeighbor=nullptr;
		m_pPoints=nullptr;
		m_pGlobalPoints=nullptr;
		m_pNormals=nullptr;
		m_pPolys=nullptr;
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
	Int32 m_PointCount,m_PolyCount;
};

class BrushVertexData
{
public:
	Float m_Dist;
	Int32 m_Index;
	BrushObjectData *m_pObject;
};

class BrushPixelData
{
public:
	BrushObjectData *m_pObject;
	Int32 m_Index;
	Float m_Z;
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
	Bool Message(BaseDocument *doc, BaseContainer &data, Int32 type, void *t_data);
	Bool GetCursorInfo(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, Float x, Float y, BaseContainer& bc);
	Bool MouseInput(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg);
	BrushVertexData *GetSelected(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, Int32 &vcnt, Int32 x, Int32 y, Float rad, BaseObject *op);	// must call ValidateObjects before any calls to this
	Float GetCursor(Int32 &x, Int32 &y);
	void GetObjectInfo(BrushObjectData *data, BrushObjectInfo &info);
	Bool GetObjectInfo(BaseObject *op, BrushObjectInfo &info);
	Bool ValidateObjects(BaseDocument *doc, BaseContainer& data);
	BrushPixelData *GetObjectAt(Int32 x, Int32 y);	// must call ValidateObjects before any calls to this
	Float GetFalloff(Float dst, Int32 flags);	// only valid within mouseinput
	Bool GetObjects(BaseDocument *doc, AtomArray *objects);
	Bool UpdateCache(BaseDocument *doc, BaseContainer& data, BaseDraw *bd, Bool force);
	Float *CalcSurfaceDistances(PolygonObject *pObject, BaseSelect *selected, Neighbor *pNeighbor=nullptr, Vector *pNormals=nullptr, Vector *pGlobalPoints=nullptr, Float *pDistance=nullptr);
	Float *CalcSurfaceDistancesFromPoint(PolygonObject *pObject, Int32 pindex, Neighbor *pNeighbor=nullptr, Vector *pNormals=nullptr, Vector *pGlobalPoints=nullptr, Float *pDistance=nullptr);
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
	virtual Bool Message(BaseDocument *doc, BaseContainer &data, Int32 type, void *t_data);
	virtual Bool GetCursorInfo(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, Float x, Float y, BaseContainer& bc);
	virtual Bool MouseInput(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg);

	//////////////////////////////////////////////////////////////////////////

	virtual Bool MouseInputStart(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg, Int32 &flags) { return true; }
	virtual Bool MouseInputDrag(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg, BrushVertexData* vdata, Int32 vcnt, Float x, Float y, Int32 &flags) { return true; }
	virtual Bool MouseInputEnd(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg) { return true; }

	// Implement these methods declared in DescriptionToolData. Otherwise BrushToolData stays an abstract class and the Swig-generated instantiations result in c++ compiler errors
	// virtual Int32			GetToolPluginId()  { return 666; } // The number of the beast
	// virtual const String	GetResourceSymbol() { return "THIS SHOULD NEVER HAPPEN"; }
};

#endif

//////////////////////////////////////////////////////////////////////////

// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF

//////////////////////////////////////////////////////////////////////////
#define LIBRARY_CA		1019742
#define LIBRARY_BRUSH	1019809
//////////////////////////////////////////////////////////////////////////

//struct CALibrary : public C4DLibrary
//{
//	//////////////////////////////////////////////////////////////////////////
//	// Weight Tag
//
//	BaseObject *(*weightGetJoint)(CAWeightTag *tag, Int32 index, BaseDocument *doc);
//	Int32 (*weightGetJointCount)(CAWeightTag *tag);
//	Int32 (*weightFindJoint)(CAWeightTag *tag, BaseObject *op, BaseDocument *doc);
//	JointRestState (*weightGetJointRestState)(CAWeightTag *tag, Int32 index);
//	void (*weightSetJointRestState)(CAWeightTag *tag, Int32 index, const JointRestState &state);
//	void (*weightGetWeightMap)(CAWeightTag *tag, Int32 index, Float32 *map, Int32 cnt);
//	Bool (*weightSetWeightMap)(CAWeightTag *tag, Int32 index, Float32 *map, Int32 cnt);
//	Int32 (*weightGetWeightCount)(CAWeightTag *tag, Int32 index);
//	void (*weightGetIndexWeight)(CAWeightTag *tag, Int32 index, Int32 windex, Int32 &pntindex, Float &weight);
//	Float (*weightGetWeight)(CAWeightTag *tag, Int32 index, Int32 pntindex);
//	Bool (*weightSetWeight)(CAWeightTag *tag, Int32 index, Int32 pntindex, Float weight);
//	UInt32 (*weightGetDirty)(CAWeightTag *tag);
//	void (*weightDirty)(CAWeightTag *tag);
//
//	//////////////////////////////////////////////////////////////////////////
//	// Joint Object
//
//	void (*jointGetBone)(CAJointObject *op, Matrix &m, Float &len);
//	CAWeightTag *(*jointGetWeightTag)(CAJointObject *op, Int32 &index);
//
//	//////////////////////////////////////////////////////////////////////////
//	// Weight Tag
//
//	Matrix (*weightGetGeomMg)(CAWeightTag *tag);
//	void (*weightSetGeomMg)(CAWeightTag *tag, const Matrix &mg);
//	Int32 (*weightAddJoint)(CAWeightTag *tag, BaseObject *op);
//	void (*weightRemoveJoint)(CAWeightTag *tag, BaseObject *op);
//	void (*weightCalculateBoneStates)(CAWeightTag *tag, Int32 index);
//	Bool (*weightTransferWeightMap)(CAWeightTag *tag, BaseDocument *doc, CAWeightTag *dst, Int32 sindex, Int32 dindex, Int32 offset, Int32 cnt, AliasTrans *trans);
//
//	//////////////////////////////////////////////////////////////////////////
//	// Pose Morph Tag
//
//	Int32 (*pmorphGetCount)(CAPoseMorphTag *tag);
//	CAMorph *(*pmorphGetMorph)(CAPoseMorphTag *tag, Int32 index);
//	DescID (*pmorphGetDescID)(CAPoseMorphTag *tag, Int32 index);
//	Int32 (*pmorphGetMode)(CAPoseMorphTag *tag);
//	Int32 (*pmorphGetActive)(CAPoseMorphTag *tag);
//	CAMorph *(*pmorphAdd)(CAPoseMorphTag *tag);
//	void (*pmorphRemove)(CAPoseMorphTag *tag, Int32 index);
//	void (*pmorphInitMorphs)(CAPoseMorphTag *tag);
//	void (*pmorphUpdateMorphsEx)(CAPoseMorphTag *tag);
//	Int32 (*pmorphGetMorphIndex)(CAPoseMorphTag *tag, CAMorph *morph);
//	Bool (*pmorphExitEdit)(CAPoseMorphTag *tag, BaseDocument *doc, Bool apply);
//	//////////////////////////////////////////////////////////////////////////
//	String (*pmorphGetName)(CAMorph *morph);
//	Int32 (*pmorphGetID)(CAMorph *morph);
//	void (*pmorphSetName)(CAMorph *morph, const String &name);
//	Bool (*pmorphCopyFrom)(CAMorph *morph, CAMorph *src, AliasTrans *trn, CAMORPH_COPY_FLAGS flags);
//	CAMorphNode *(*pmorphFind)(CAMorph *morph, CAPoseMorphTag *tag, BaseList2D *bl);
//	CAMorphNode *(*pmorphGetFirst)(CAMorph *morph);
//	Bool (*pmorphSetMode)(CAMorph *morph, BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_MODE_FLAGS flags, CAMORPH_MODE mode);
//	Int32 (*pmorphFindIndex)(CAMorph *morph, CAPoseMorphTag *tag, BaseList2D *bl);
//	CAMorphNode *(*pmorphFindFromIndex)(CAMorph *morph, CAPoseMorphTag *tag, Int32 index);
//	Bool (*pmorphStore)(CAMorph *morph, BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_DATA_FLAGS flags);
//	Bool (*pmorphApply)(CAMorph *morph, BaseDocument *doc, CAPoseMorphTag *tag, CAMORPH_DATA_FLAGS flags);
//	Int32 (*pmorphGetNodeIndex)(CAMorph *morph, CAMorphNode *node);
//	//////////////////////////////////////////////////////////////////////////
//	CAMorphNode *(*pmorphnodeGetNext)(CAMorphNode *node);
//	CAMorphNode *(*pmorphnodeGetPrev)(CAMorphNode *node);
//	CAMorphNode *(*pmorphnodeGetUp)(CAMorphNode *node);
//	CAMorphNode *(*pmorphnodeGetDown)(CAMorphNode *node);
//	BaseList2D *(*pmorphnodeGetLink)(CAMorphNode *node, CAPoseMorphTag *tag, CAMorph *morph, BaseDocument *doc);
//	Int32 (*pmorphnodeGetPointCount)(CAMorphNode *node);
//	Vector (*pmorphnodeGetPoint)(CAMorphNode *node, Int32 index);
//	void (*pmorphnodeSetPoint)(CAMorphNode *node, Int32 index, const Vector &pnt);
//	Int32 (*pmorphGetTangentCount)(CAMorphNode *node);
//	Vector (*pmorphGetTangent)(CAMorphNode *node, Int32 index);
//	void (*pmorphSetTangent)(CAMorphNode *node, Int32 index, const Vector &v);
//	Int32 (*pmorphGetVertexMapTagCount)(CAMorphNode *node);
//	Int32 (*pmorphGetVertexMapCount)(CAMorphNode *node, Int32 tindex);
//	Float (*pmorphGetVertexMap)(CAMorphNode *node, Int32 tindex, Int32 index);
//	void (*pmorphSetVertexMap)(CAMorphNode *node, Int32 tindex, Int32 index, Float v);
//	Bool (*pmorphSetVertexMapCount)(CAMorphNode *node, Int32 tindex, Int32 cnt);
//	Bool (*pmorphSetPointCount)(CAMorphNode *node, Int32 cnt);
//	Bool (*pmorphSetTangentCount)(CAMorphNode *node, Int32 cnt);
//	CAMORPH_DATA_FLAGS (*pmorphGetInfo)(CAMorphNode *node);
//	Vector (*pmorphGetP)(CAMorphNode *node);
//	Vector (*pmorphGetS)(CAMorphNode *node);
//	Vector (*pmorphGetR)(CAMorphNode *node);
//	void (*pmorphSetP)(CAMorphNode *node, const Vector &p);
//	void (*pmorphSetS)(CAMorphNode *node, const Vector &s);
//	void (*pmorphSetR)(CAMorphNode *node, const Vector &r);
//	Int32 (*pmorphGetParamCount)(CAMorphNode *node);
//	Bool (*pmorphSetParamCount)(CAMorphNode *node, Int32 cnt);
//	Bool (*pmorphGetParam)(CAMorphNode *node, Int32 index, GeData &data, DescID &id);
//	void (*pmorphSetParam)(CAMorphNode *node, Int32 index, const GeData &data, const DescID &id);
//	Int32 (*pmorphGetUVTagCount)(CAMorphNode *node);
//	Int32 (*pmorphGetUVCount)(CAMorphNode *node, Int32 tindex);
//	Bool (*pmorphSetUVCount)(CAMorphNode *node, Int32 tindex, Int32 cnt);
//	void (*pmorphGetUV)(CAMorphNode *node, Int32 tindex, Int32 index, UVWStruct &uv);
//	void (*pmorphSetUV)(CAMorphNode *node, Int32 tindex, Int32 index, const UVWStruct &uv);
//	Int32 (*pmorphGetWeightMapTagCount)(CAMorphNode *node);
//	Int32 (*pmorphGetWeightMapJointCount)(CAMorphNode *node, Int32 tindex);
//	Int32 (*pmorphGetWeightMapCount)(CAMorphNode *node, Int32 tindex, Int32 jindex);
//	Bool (*pmorphSetWeightMapCount)(CAMorphNode *node, Int32 tindex, Int32 jindex, Int32 cnt);
//	Float (*pmorphGetWeightMap)(CAMorphNode *node, Int32 tindex, Int32 jindex, Int32 index);
//	void (*pmorphSetWeightMap)(CAMorphNode *node, Int32 tindex, Int32 jindex, Int32 index, Float v);
//	//////////////////////////////////////////////////////////////////////////
//	void (*xrefStripRefData)(BaseDocument *doc, BaseList2D *bl); // private
//	Bool (*xrefHasRefs)(BaseDocument *doc); // private
//	Int32 (*xrefGetState)(); // private
//	UInt64 (*xrefGetID)(BaseList2D *bl); // private
//	Bool (*xrefHasParam)(BaseDocument *doc, BaseList2D *bl, const DescID &id); // private
//	Bool (*xrefGetParam)(BaseDocument *doc, BaseList2D *bl, const DescID &id, GeData &dat); // private
//	Bool (*xrefRemoveParam)(BaseDocument *doc, BaseList2D *bl, const DescID &id); // private
//
//	void (*pmorphSetActiveMorphIndex)(CAPoseMorphTag *tag, Int32 index);
//	BaseList2D *(*pmorphGetTarget)(CAMorph *morph, BaseDocument *doc);
//	void (*pmorphSetTarget)(CAMorph *morph, CAPoseMorphTag *tag, BaseDocument *doc, BaseList2D *bl);
//	void (*pmorphSetStrength)(CAMorph *morph, Float strength);
//	Float (*pmorphGetStrength)(CAMorph *morph);
//	void (*pmorphUpdateMorphs)(CAPoseMorphTag *tag, BaseDocument *doc);
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
//	Bool (iBrushBase::*Message)(BaseDocument *doc, BaseContainer &data, Int32 type, void *t_data);
//	Bool (iBrushBase::*GetCursorInfo)(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, Float x, Float y, BaseContainer& bc);
//	Bool (iBrushBase::*MouseInput)(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, EditorWindow* win, const BaseContainer& msg);
//	BrushVertexData *(iBrushBase::*GetSelected)(BaseDocument* doc, BaseContainer& data, BaseDraw* bd, Int32 &vcnt, Int32 x, Int32 y, Float rad, BaseObject *op);
//	Float (iBrushBase::*GetCursor)(Int32 &x, Int32 &y);
//	void (iBrushBase::*GetObjectInfo)(BrushObjectData *data, BrushObjectInfo &info);
//	Bool (iBrushBase::*GetObjectInfoOp)(BaseObject *op, BrushObjectInfo &info);
//	Bool (iBrushBase::*ValidateObjects)(BaseDocument *doc, BaseContainer& data);
//	BrushPixelData *(iBrushBase::*GetObjectAt)(Int32 x, Int32 y);
//	Float (iBrushBase::*GetFalloff)(Float dst, Int32 flags);
//	Bool (iBrushBase::*GetObjects)(BaseDocument *doc, AtomArray *objects);
//	Bool (iBrushBase::*UpdateCache)(BaseDocument *doc, BaseContainer& data, BaseDraw *bd, Bool force);
//	Float *(iBrushBase::*CalcSurfaceDistances)(PolygonObject *pObject, BaseSelect *selected, Neighbor *pNeighbor, Vector *pNormals, Vector *pGlobalPoints, Float *pDistance);
//	Float *(iBrushBase::*CalcSurfaceDistancesFromPoint)(PolygonObject *pObject, Int32 pindex, Neighbor *pNeighbor, Vector *pNormals, Vector *pGlobalPoints, Float *pDistance);
//};

// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF

#endif	// __LIB_CA_H__
