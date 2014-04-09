/////////////////////////////////////////////////////////////
// CINEMA 4D SDK                                           //
/////////////////////////////////////////////////////////////
// (c) MAXON Computer GmbH, all rights reserved            //
/////////////////////////////////////////////////////////////

#ifndef _LIB_DESCRIPTION_H_
#define _LIB_DESCRIPTION_H_

#include "c4d_library.h"
#include "c4d_basecontainer.h"


#define DESCID_ROOT						DescID(DescLevel(1000491,0,0))
#define ID_USERDATA						700
#define DESCID_DYNAMICSUB			DescLevel(ID_USERDATA,DTYPE_SUBCONTAINER,0)

#define BOOL_PAGEMODE					'bpmd'

class BaseList2D;
class SubDialog;
class BCResourceObj;
class DescEntry;
class DescriptionCustomGui;

struct RESOURCEDATATYPEPLUGIN;

// defines for description
enum
{
	DTYPE_NONE					= 0,

	DTYPE_CHILDREN			= 0,
	DTYPE_GROUP					= 1,
	DTYPE_COLOR					= 3,
	DTYPE_SUBCONTAINER  = 5,	// subcontainer data
	DTYPE_MULTIPLEDATA  = 6,	// multiple data entry
	DTYPE_TEXTURE				= 7,	// String: Texturename
	DTYPE_BUTTON				= 8,
	DTYPE_DYNAMIC				= 10, // for graphview "DYNAMIC"
	DTYPE_SEPARATOR			= 11, //
	DTYPE_STATICTEXT		= 12, //
	DTYPE_POPUP         = 13,

	DTYPE_LONG					= 15,
	DTYPE_REAL					= 19,
	DTYPE_TIME					= 22,
	DTYPE_VECTOR				= 23,
	DTYPE_MATRIX				= 25,
	DTYPE_STRING				= 130,
	DTYPE_FILENAME			= 131, // DA_FILENAME
	DTYPE_BASELISTLINK	= 133,
	DTYPE_BOOL					= 400006001,//ID_GV_DATA_TYPE_BOOL
	DTYPE_NORMAL				= 400006005,//ID_GV_DATA_TYPE_NORMAL

	//--------------------
	DESC_NAME						= 1,					// name for parameter standalone use
	DESC_SHORT_NAME			= 2,					// short name (only for attribute dialog)

	DESC_VERSION				= 3,					// Int32: bitmask of the following values DESC_VERSION_xxx
		DESC_VERSION_DEMO		= (1<<0),
		DESC_VERSION_XL			= (1<<1),
		DESC_VERSION_ALL		= DESC_VERSION_DEMO|DESC_VERSION_XL,
	DESC_CHILDREN				= 4,					// BaseContainer
	DESC_MIN						= 5,					// Int32/Float/Vector minimum INcluded
	DESC_MAX						= 6,					// Int32/Float/Vector maximum INcluded
	DESC_MINEX					= 7,					// Bool: true == minimum EXcluded
	DESC_MAXEX					= 8,					// Bool: true == maximum EXcluded
	DESC_STEP						= 9,					// Int32/Float/Vector
	DESC_ANIMATE				= 10,					// Int32
		DESC_ANIMATE_OFF		= 0,
		DESC_ANIMATE_ON			= 1,
		DESC_ANIMATE_MIX		= 2,
	DESC_ASKOBJECT			= 11,					// Bool: true - ask object for this parameter, false - look inside container
	DESC_UNIT						= 12,					// Int32: one of the following values DESC_UNIT_xxx for DTYPE_REAL/DTYPE_VECTOR
		DESC_UNIT_FLOAT			= 'frea',		//FORMAT_FLOAT,
		DESC_UNIT_INT			= 'flng',		//FORMAT_INT,
		DESC_UNIT_PERCENT		= 'fpct',		//FORMAT_PERCENT,
		DESC_UNIT_DEGREE		= 'fdgr',		//FORMAT_DEGREE,
		DESC_UNIT_METER			= 'fmet',		//FORMAT_METER,
		DESC_UNIT_TIME			= 'ffrm',		//FORMAT_FRAMES,
	DESC_PARENTGROUP		= 13,					// Int32/DescID: parent id
	DESC_CYCLE					= 14,					// Container: members of cycle
	DESC_HIDE						= 15,					// Bool: indicates whether the property is hidden or not
	DESC_DEFAULT				= 16,					// default value for Int32/Float/Vector:
	DESC_ACCEPT					= 17,					// ACCEPT: for InstanceOf-Check()
	DESC_SEPARATORLINE	= 18,
	DESC_REFUSE					= 19,					// REFUSE: for InstanceOf-Check()
	DESC_PARENTID				= 20,					// for indent and anim track can append the parent-name
	DESC_CUSTOMGUI			= 21,					// customgui for this property


	DESC_COLUMNS					= 22,					// DTYPE_GROUP: number of columns
	DESC_LAYOUTGROUP			= 23,					// Bool: only for layout in columns, in layout groups are only groups allowed!
	DESC_REMOVEABLE				= 24,					// Bool: true allows to remove this entry
	DESC_GUIOPEN					= 25,					// Bool: default open
	DESC_EDITABLE					= 26,					// Bool: true allows to edit this entry
	DESC_MINSLIDER				= 27,					// Int32/Float/Vector minimum INcluded
	DESC_MAXSLIDER				= 28,					// Int32/Float/Vector maximum INcluded
	DESC_GROUPSCALEV			= 29,					// Bool: allow to scale group height
	DESC_SCALEH						= 30,					// Bool: scale element horizontal
	DESC_LAYOUTVERSION		= 31,					// Int32: layout version
	DESC_ALIGNLEFT				= 32,					// Bool: align element left
	DESC_FITH							= 33,					// Bool: fit element
	DESC_NEWLINE        	= 34,					// Bool: line break
	DESC_TITLEBAR					= 35,					// Bool: main group title bar
	DESC_CYCLEICONS				= 36,					// Container: Int32 icon ids for cycle
	DESC_CYCLESYMBOLS			= 37,					// Container: String identifiers for help symbol export
	DESC_PARENT_COLLAPSE	= 38,					// parent collapse id
	DESC_FORBID_INLINE_FOLDING = 39,		// Bool: instruct AM not to allow expanding inline objects for this property
	DESC_FORBID_SCALING		= 40,					// Bool: prevent auto scaling of the parameter with the scale tool (for DESC_UNIT_METER)
	DESC_ANGULAR_XYZ			= 41,					// Bool: angular representation as XYZ vs. HPB

	// port extension for graphview
	DESC_INPORT					= 50,
	DESC_OUTPORT				= 51,
	DESC_STATICPORT			= 52,
	DESC_NEEDCONNECTION	= 53,
	DESC_MULTIPLE				= 54,
	DESC_PORTONLY				= 55,
	DESC_CREATEPORT			= 56,
	DESC_PORTSMIN				= 57,
	DESC_PORTSMAX				= 58,
	DESC_NOTMOVABLE			= 59,
	DESC_EDITPORT				= 60,
	DESC_ITERATOR				= 61,

	DESC_PARENTMSG			= 62,
	DESC_MATEDNOTEXT		= 63,
	DESC_COLUMNSMATED		= 64,					// DESC_COLUMNSMATED: number of columns in left mated window
	DESC_SHADERLINKFLAG	= 65,					// only if (datatype==DTYPE_LINK) to specify if shader
	DESC_NOGUISWITCH		= 66,

	DESC_TEMPDESCID			= 998,				// used internally to store the preferred descid
	DESC_IDENT					= 999,
	DESC_
};

		#define CUSTOMGUI_REAL						DTYPE_REAL
		#define CUSTOMGUI_REALSLIDER			1000489
		#define CUSTOMGUI_REALSLIDERONLY	200000006
		#define CUSTOMGUI_VECTOR					DTYPE_VECTOR
		#define CUSTOMGUI_STRING					DTYPE_STRING
		#define CUSTOMGUI_STRINGMULTI			200000007
		#define CUSTOMGUI_STATICTEXT			DTYPE_STATICTEXT
		#define CUSTOMGUI_CYCLE						200000180
		#define CUSTOMGUI_CYCLEBUTTON			200000255
		#define CUSTOMGUI_LONG						DTYPE_LONG
		#define CUSTOMGUI_LONGSLIDER			1000490
		#define CUSTOMGUI_BOOL						DTYPE_BOOL
		#define CUSTOMGUI_TIME						DTYPE_TIME
		#define CUSTOMGUI_COLOR						1000492
		#define CUSTOMGUI_MATRIX					DTYPE_MATRIX
		#define CUSTOMGUI_BUTTON					DTYPE_BUTTON
		#define CUSTOMGUI_POPUP						DTYPE_POPUP
		#define CUSTOMGUI_SEPARATOR				DTYPE_SEPARATOR
		#define CUSTOMGUI_SUBDESCRIPTION	0
		#define CUSTOMGUI_PROGRESSBAR			200000265
		
		

#ifndef __API_INTERN__

#include "c4d_customdatatype.h"

#define CUSTOMDATATYPE_DESCID		1000486

enum
{
	VECTOR_X		= 1000,
	VECTOR_Y		= 1001,
	VECTOR_Z		= 1002
};

enum
{
	COLOR_R			= 1000,
	COLOR_G			= 1001,
	COLOR_B			= 1002
};

struct DescLevel
{
	Int32	id;
	Int32	dtype;
	Int32	creator;

	DescLevel(Int32 t_id) : id(t_id), dtype(0), creator(0) { }
	DescLevel(Int32 t_id, Int32 t_datatype, Int32 t_creator) : id(t_id), dtype(t_datatype), creator(t_creator) { }

	Bool operator == (const DescLevel &d) const;
	Bool operator != (const DescLevel &d) const;
};

class DescID : public iCustomDataType<DescID> // iCustomDataType is okay here since the size is the same
{
		Int32 temp1;
		Int32 *temp2;

	public:
		DescID();
		DescID(const DescID &src);
		DescID(Int32 id1);
		DescID(const DescLevel &id1);
		DescID(const DescLevel &id1,const DescLevel &id2);
		DescID(const DescLevel &id1,const DescLevel &id2,const DescLevel &id3);
		~DescID();

		void SetId(const DescLevel &subid);
		void PushId(const DescLevel &subid);
		void PopId();
		const DescLevel &operator[] (Int32 pos) const;
		const DescID& operator = (const DescID &id);
		Bool operator == (const DescID &d) const;
		Bool operator != (const DescID &d) const;
		const DescID operator <<(Int32 shift) const;

		Bool Read(HyperFile *hf);
		Bool Write(HyperFile *hf);

		Int32 GetDepth() const;
		Bool IsPartOf(const DescID &cmp,Int32 *pos) const;

		const DescID & operator += (const DescID &s);
		friend const DescID operator + (const DescID &v1, const DescID &v2);
};

class Description
{
	private:
		Description();
		~Description();

	public:
		static Description *Alloc();
		static void Free(Description *&description);

		Bool LoadDescription(const BCResourceObj *bc,Bool copy);
		Bool LoadDescription(Int32 id);
		Bool LoadDescription(const String &id);
		Bool SortGroups();

		const BCResourceObj* GetDescription();														// returns the complete description
		const BaseContainer* GetParameter(const DescID &id,BaseContainer &temp,AtomArray *ar) const;							// returns the specified property
		BaseContainer* GetParameterI(const DescID &id,AtomArray *ar);										// returns the specified property
		Bool SetParameter(const DescID &id,const BaseContainer &param,const DescID &groupid);		// returns the specified property

		void *BrowseInit();																								// browse property start, dont forget to call xxxFree
		Bool GetNext(void *handle,const BaseContainer **bc,DescID &id,DescID &groupid);					// returns all properties sequently
		void BrowseFree(void *&handle);																		// browse property free

		DescEntry *GetFirst(const AtomArray &op);
		DescEntry *GetNext(DescEntry *de);
		DescEntry *GetDown(DescEntry *de);
		void GetDescEntry(DescEntry *de,const BaseContainer **bc,DescID &descid);

		SubDialog *CreateDialogI();
		void FreeDialog(SubDialog *dlg);

		Bool CreatePopupMenu(BaseContainer &menu);
		Bool GetPopupId(Int32 id,const DescID &descid);

		Bool CheckDescID(const DescID &searchid,const AtomArray &ops,DescID *completeid);
		Bool GetSubDescriptionWithData(const DescID &did,const AtomArray &op,RESOURCEDATATYPEPLUGIN  *resdatatypeplugin,const BaseContainer &bc,DescID *singledescid);

		const DescID *GetSingleDescID();
		void SetSingleDescriptionMode(const DescID &descid);
};

class DynamicDescription
{
		DynamicDescription();
		~DynamicDescription();

	public:

		DescID								Alloc(const BaseContainer &datadescription);
		Bool									Set(const DescID &descid,const BaseContainer &datadescription, BaseList2D *bl);
		const BaseContainer*	Find(const DescID &descid);
		Bool									Remove(const DescID &descid);

		Bool									CopyTo(DynamicDescription *dest);

		void*									BrowseInit(void);
		Bool									BrowseGetNext(void* handle,DescID *id,const BaseContainer **data);
		void									BrowseFree(void* &handle);

		Bool									FillDefaultContainer(BaseContainer &res,Int32 type,const String &name);

		UInt32									GetDirty() const;
};
#else
	#include "res_basecontainer.h"
#endif

#define HandleDescGetVector(tid,vector,t_data,flags) \
	switch (tid[1].id) \
	{ \
		case 0:			t_data = GeData(vector);   flags |= DESCFLAGS_GET_PARAM_GET; break; \
		case 1000:	t_data = GeData(vector.x); flags |= DESCFLAGS_GET_PARAM_GET; break; \
		case 1001:	t_data = GeData(vector.y); flags |= DESCFLAGS_GET_PARAM_GET; break; \
		case 1002:	t_data = GeData(vector.z); flags |= DESCFLAGS_GET_PARAM_GET; break; \
	} \

#define HandleDescSetVector(v,tid,vector,t_data,flags) \
	switch (tid[1].id) \
	{ \
		case 0:			v = t_data.GetVector(); flags |= DESCFLAGS_SET_PARAM_SET; break; \
		case 1000:	v = Vector(t_data.GetFloat(),vector.y,vector.z); flags |= DESCFLAGS_SET_PARAM_SET; break; \
		case 1001:	v = Vector(vector.x,t_data.GetFloat(),vector.z); flags |= DESCFLAGS_SET_PARAM_SET; break; \
		case 1002:	v = Vector(vector.x,vector.y,t_data.GetFloat()); flags |= DESCFLAGS_SET_PARAM_SET; break; \
	} \

struct DescriptionCommand
{
	DescID id;
	const BaseContainer *msg;
	DescriptionCustomGui *descgui;

	DescriptionCommand() { msg = nullptr; descgui = nullptr; }
};

struct DescriptionPopup
{
	DescID id;
	Int32   chosen;
	const BaseContainer *msg;
	BaseContainer popup;

	DescriptionPopup() { msg = nullptr; chosen=0; }
};

struct DescriptionCheckDragAndDrop
{
	DescID			id;
	GeListNode	*element;
	Bool				result;

	DescriptionCheckDragAndDrop() { result = true; }
};

struct DescriptionGetBitmap
{
	DescID					id;
	BaseBitmap			*bmp;
	ICONDATAFLAGS		bmpflags;

	DescriptionGetBitmap() : id(0), bmp(nullptr), bmpflags(ICONDATAFLAGS_0) {}
};

struct DescriptionGetObjects
{
	BaseContainer	bc;
	DescID				descid;
};

// public stuff
Bool Description_Register(Int32 id,const String &idstr,LocalResource *res);


// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF

#define LIBRARY_DESCRIPTIONLIB		1000467

struct DescriptionLib : public C4DLibrary
{
	Bool (*Register )(Int32 id,const String &idstr,LocalResource *res);

	Description*					(*Alloc)();
	void									(*Free)(Description *&description);

	Bool									(*LoadDescriptionBc)(Description *desc,const BCResourceObj *bc,Bool copy);
	Bool									(*LoadDescriptionId)(Description *desc,Int32 id);
	Bool									(*LoadDescriptionStr)(Description *desc,const String *id);
	Bool									(*SortGroups)(Description *desc);

	const BCResourceObj*	(*GetDescription)(Description *desc);
	const BaseContainer*	(*GetParameter)(const Description *desc,const DescID &id,BaseContainer &temp,AtomArray *ar);
	BaseContainer*				(*GetParameterI)(Description *desc,const DescID &id,AtomArray *ar);
	Bool									(*SetParameter)(Description *desc,const DescID &id,const BaseContainer &param,const DescID &groupid);	// returns the specified property

	void *								(*BrowseInit)(Description *desc);
	Bool									(*GetNext)(Description *desc,void *handle,const BaseContainer **bc,DescID &id,DescID &groupid);
	void									(*BrowseFree)(Description *desc,void *&handle);
	void									(*EX_01)(Description *desc);

	SubDialog *						(*CreateDialogI)(Description *desc);
	void									(*FreeDialog)(Description *desc,SubDialog *dlg);

	Bool									(*CreatePopupMenu)(Description *desc,BaseContainer *menu);
	Bool									(*GetPopupId)(Description *desc,Int32 id,const DescID &descid);

	const DescID*					(*GetSingleDescID)(Description *desc);
	void									(*SetSingleDescriptionMode)(Description *desc,const DescID &descid);

	void									(*DescID_Init)(DescID *id);
	void									(*DescID_Free)(DescID *id);
	void									(*DescID_SetId)(DescID *id,const DescLevel &subid);
	void									(*DescID_PushId)(DescID *id,const DescLevel &subid);
	void									(*DescID_PopId)(DescID *id);
	const DescLevel&			(*DescID_operator1)(DescID *id,Int32 pos);
	const DescID					(*DescID_operator2)(DescID *id,Int32 pos);
	void									(*DescID_CopyTo)(DescID *src,DescID *dest);
	Bool									(*DescID_Compare)(DescID *d1,DescID *d2);

	DescID								(DynamicDescription::*DDAlloc					)(const BaseContainer &datadescription);
	Bool									(DynamicDescription::*DDSetObsolete		)(const DescID &descid,const BaseContainer &datadescription);
	const BaseContainer*	(DynamicDescription::*DDFind					)(const DescID &descid);
	Bool									(DynamicDescription::*DDRemove				)(const DescID &descid);
	Bool									(DynamicDescription::*DDCopyTo				)(DynamicDescription *dest) const;
	void*									(DynamicDescription::*DDBrowseInit		)(void) const;
	Bool									(DynamicDescription::*DDBrowseGetNext	)(void* handle,DescID *id,const BaseContainer **data) const;
	void									(DynamicDescription::*DDBrowseFree		)(void* &handle) const;
	Bool									(DynamicDescription::*FillDefaultContainer)(BaseContainer &res,Int32 type,const String &name) const;

	Bool									(*GetSubDescriptionWithData						)(Description *desc,const DescID &did,const AtomArray &op,RESOURCEDATATYPEPLUGIN *resdatatypeplugin,const BaseContainer &bc,DescID *singledescid);
	Bool									(*CheckDescID													)(Description *desc,const DescID &searchid,const AtomArray &ops,DescID *completeid);

	DescEntry*						(*DescEntryGetFirst										)(Description *desc,const AtomArray &op);
	DescEntry*						(*DescEntryGetNext										)(Description *desc,DescEntry *de);
	DescEntry*						(*DescEntryGetDown										)(Description *desc,DescEntry *de);
	void									(*DescEntryGetDescEntry								)(Description *desc,DescEntry *de,const BaseContainer **bc,DescID &descid);
	String								(*DescGenerateTitle										)(AtomArray *arr);

	Bool									(*DescID_Read)(DescID *id, HyperFile *hf);
	Bool									(*DescID_Write)(DescID *id, HyperFile *hf);
	Bool									(DynamicDescription::*DDSet						)(const DescID &descid,const BaseContainer &datadescription, BaseList2D *bl);

	UInt32									(DynamicDescription::*GetDirty				)() const;

	const DescID					(*DescID_AddTo)(DescID *d1,DescID *d2);
	DescID								(*DescID_Add)(DescID *d1,DescID *d2);
};

String DescGenerateTitle(AtomArray *arr);

// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF

#endif
