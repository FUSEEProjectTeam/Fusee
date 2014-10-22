#ifndef __C4DCANIMATION_H
#define __C4DCANIMATION_H

#ifdef __API_INTERN__
abc def xyz
#endif

#include "c4d_basetime.h"
#include "c4d_gedata.h"
#include "c4d_baselist.h"

#define CaCall(fnc) (this->*C4DOS.CA->fnc)

class CAnimInfo
{
private:
	CAnimInfo(void);
	~CAnimInfo(void);

public:
	BaseDocument* doc;					// document
	BaseList2D*		op;						// original object
	BaseTime			otime;				// original time
	Float64				xtime;				// remapped time
	Float64				fac;					// factor of time compared to length of sequence
	Float64				rel;					// factor between two keys, only !=0.0 if k1!=nullptr && k2!=nullptr
	CKey*					k1;						// last key <= time ATTENTION can be nullptr
	CKey*					k2;						// next key > time ATTENTION can be nullptr
	Int32					k1idx, k2idx;	// indices for k1 & k2
	Int32					cycle;				// 0 for the range from first key to last key, after last key it is +1,+2... before first key -1,-2,...
};

class CKey : public GeListNode
{
private:
	CKey(void);
	~CKey(void);
	const CKey& operator = (const CKey& key);

public:
	// general
	BaseTime GetTime(void) const { return CaCall(GetTime) (); }
	BaseTime GetTimeLeft(void) const { return CaCall(GetTimeLeft) (); }
	BaseTime GetTimeRight(void) const	{ return CaCall(GetTimeRight) (); }
	Float GetValue(void) const { return CaCall(GetValue) (); }
	Float GetValueLeft(void) const { return CaCall(GetValueLeft) (); }
	Float GetValueRight(void) const	{ return CaCall(GetValueRight) (); }
	CINTERPOLATION GetInterpolation(void) const { return CaCall(GetInterpolation) (); }

	void SetTime(CCurve* seq, const BaseTime& t)							{ CaCall(SetTime) (seq, t); }
	void SetTimeLeft(CCurve* seq, const BaseTime& t)					{ CaCall(SetTimeLeft) (seq, t); }
	void SetTimeRight(CCurve* seq, const BaseTime& t)					{ CaCall(SetTimeRight) (seq, t); }
	void SetValue(CCurve* seq, Float v)												{ CaCall(SetValue) (seq, v); }
	void SetGeData(CCurve* seq, const GeData& d)							{ CaCall(SetGeData) (seq, d); }
	void SetValueLeft(CCurve* seq, Float v)										{ CaCall(SetValueLeft) (seq, v); }
	void SetValueRight(CCurve* seq, Float v)										{ CaCall(SetValueRight) (seq, v); }
	void SetInterpolation(CCurve* seq, CINTERPOLATION inter)	{ CaCall(SetInterpolation) (seq, inter); }

	// functions
	Bool CopyDataTo(CCurve* destseq, CKey* dest, AliasTrans* trans) const { return CaCall(CopyDataTo) (destseq, dest, trans); }
	CTrack* GetTrack(void)																		{ return CaCall(GetTrackCKey) (); }
	CCurve* GetCurve(void)                                    { return CaCall(GetSequenceCKey) (); }
	const GeData& GetGeData(void) const { return CaCall(GetGeData) (); }
	void FlushData(void)																			{ CaCall(FlushData1) (); }

	CKey* GetClone(AliasTrans* trans) const
	{
		CKey* key = CKey::Alloc();
		if (!key)
			return nullptr;
		CopyDataTo(nullptr, key, trans);
		return key;
	}

	static CKey* Alloc() { return C4DOS.CA->CKey_Alloc(); }
	static void Free(CKey*& key) { C4DOS.CA->CKey_Free(key); }
};

class CCurve : public BaseList2D
{
private:
	CCurve(void);
	~CCurve(void);
	const CCurve& operator = (const CCurve& seq);

public:
	Int32 GetKeyCount(void) const	{ return CaCall(GetKeyCount) (); }
	const CKey*  GetKey(Int32 index) const { return CaCall(GetKey2) (index); }
	CKey*        GetKey(Int32 index)																		{ return CaCall(GetKey1) (index); }
	CKey*        FindKey(const BaseTime& time, Int32* idx = nullptr, FINDANIM match = FINDANIM_EXACT) { return CaCall(FindKey1) (time, idx, match); }
	const CKey*  FindKey(const BaseTime& time, Int32* idx = nullptr, FINDANIM match = FINDANIM_EXACT) const { return CaCall(FindKey2) (time, idx, match); }
	CKey*					AddKey(const BaseTime& time, Int32* nidx = nullptr)             { return CaCall(AddKey) (time, nidx); }


	Bool InsertKey(CKey* ckey)                                      { return CaCall(InsertKey) (ckey); }
	Bool DelKey(Int32 index)                                          { return CaCall(DelKey) (index); }
	Int32 MoveKey(const BaseTime& time, Int32 idx, CCurve* dseq = nullptr) { return CaCall(MoveKey) (time, idx, dseq); }
	void FlushKeys(void)																						{ CaCall(FlushKeys) (); }
	void SortKeysByTime(void)																				{ CaCall(SortKeysByTime) (); }
	Float GetValue(const BaseTime& time, Int32 fps) const { return CaCall(GetValue1) (time, fps); }
	void CalcSoftTangents(Int32 kidx, Float* vl, Float* vr, BaseTime* tl, BaseTime* tr) { CaCall(CalcSoftTangents) (kidx, vl, vr, tl, tr); }
	Float64 CalcHermite(Float64 time, Float64 t1, Float64 t2, Float64 val1, Float64 val2, Float64 tan1_val, Float64 tan2_val, Float64 tan1_t, Float64 tan2_t, Bool deriv) const { return CaCall(CalcHermite) (time, t1, t2, val1, val2, tan1_val, tan2_val, tan1_t, tan2_t, deriv); }
	void GetTangents(Int32 kidx, Float64* vl, Float64* vr, Float64* tl, Float64* tr) { CaCall(GetTangents) (kidx, vl, vr, tl, tr); }

	void SetKeyDirty() { SetDirty(DIRTYFLAGS_CHILDREN); }

	CTrack* GetTrack()              { return CaCall(GetTrackCSeq) (); }

	void SetKeyDefault(BaseDocument* doc, Int32 kidx);

	BaseTime GetStartTime(void) const	{ return CaCall(GetStartTime) (); }
	BaseTime GetEndTime(void) const	{ return CaCall(GetEndTime) (); }

	CKey* FindNextUnmuted(Int32 idx, Int32* ret_idx = nullptr)       { return CaCall(FindNextUnmuted1) (idx, ret_idx); }
	const CKey* FindNextUnmuted(Int32 idx, Int32* ret_idx = nullptr) const { return CaCall(FindNextUnmuted2) (idx, ret_idx); }

	CKey* FindPrevUnmuted(Int32 idx, Int32* ret_idx = nullptr)       { return CaCall(FindPrevUnmuted1) (idx, ret_idx); }
	const CKey* FindPrevUnmuted(Int32 idx, Int32* ret_idx = nullptr) const { return CaCall(FindPrevUnmuted2) (idx, ret_idx); }

	CKey*	AddKeyAdaptTangent(const BaseTime& time, Bool bUndo, Int32* nidx = nullptr)             { return CaCall(AddKeyAdaptTangent) (time, bUndo, nidx); }

};

#define CTRACK_CATEGORY_VALUE	 1
#define CTRACK_CATEGORY_DATA	 2
#define CTRACK_CATEGORY_PLUGIN 3

class CTrack : public BaseList2D
{
private:
	CTrack(void);
	~CTrack(void);

public:
	// ******** from BaseList2D *********
	CTrack* GetNext() const { return (CTrack*)AtCall(GetNext) (); }
	CTrack* GetPred() const { return (CTrack*)AtCall(GetPred) (); }

	BaseList2D* GetObject() const { return (BaseList2D*)AtCall(GetMain) (); }

	const DescID& GetDescriptionID() const { return CaCall(GetDescriptionID) (); }
	Bool SetDescriptionID(BaseList2D* object, const DescID& id)  { return CaCall(SetDescriptionID) (object, id); }
	const BaseContainer* GetParameterDescription(BaseContainer& temp) const { return CaCall(GetParameterDescription) (temp); }

	Int32 GetTrackCategory() const { return CaCall(GetTrackCategory) (); }

	static CTrack* Alloc(BaseList2D* bl, const DescID& id);
	static void Free(CTrack*& bl);

	// functions
	CLOOP GetBefore(void) const { return CaCall(GetBefore) (); }
	CLOOP GetAfter(void) const { return CaCall(GetAfter) (); }
	CTrack* GetTimeTrack(BaseDocument* doc)          { return CaCall(GetTimeTrack) (doc); }
	CCurve* GetCurve(CCURVE type = CCURVE_CURVE, Bool bCreate = true)         { return CaCall(GetCurve) (type, bCreate); }
	void SetBefore(CLOOP type)                     { CaCall(SetBefore) (type); }
	void SetAfter(CLOOP type)                     { CaCall(SetAfter) (type); }
	void SetTimeTrack(CTrack* track)                   { CaCall(SetTimeTrack) (track); }
	Bool AnimateTrack(BaseDocument* doc, BaseList2D* op, const BaseTime& tt, const Int32 flags, Bool* chg, void* data = nullptr) { return CaCall(AnimateTrack) (doc, op, tt, flags, chg, data); }
	Bool Animate(const CAnimInfo* info, Bool* chg, void* data = nullptr)  { return CaCall(Animate) (info, chg, data); }
	void FlushData(void)                               { CaCall(FlushData) (); }
	Bool Draw(GeClipMap* map, const BaseTime& clip_left, const BaseTime& clip_right) const { return CaCall(Draw) (map, clip_left, clip_right); }
	Float GetValue(BaseDocument* doc, const BaseTime& time, Int32 fps) { return CaCall(GetValue3) (doc, time, fps); }
	Bool Remap(Float64 time, Float64* ret_time, Int32* ret_cycle) const { return CaCall(Remap) (time, ret_time, ret_cycle); }
	Bool FillKey(BaseDocument* doc, BaseList2D* bl, CKey* key) { return CaCall(FillKey) (doc, bl, key); }

	Int32 GuiMessage(const BaseContainer& msg, BaseContainer& result) { return CaCall(GuiMessage) (msg, result); }
	Int32 GetHeight() { return CaCall(GetHeight) (); }
	Bool TrackInformation(BaseDocument* doc, CKey* key, String* str, Bool set) { return CaCall(TrackInformation) (doc, key, str, set); }
	Int32 GetUnit(Float* step) { return CaCall(GetUnit) (step); }
	Int32 GetTLHeight(Int32 id){ return CaCall(GetTLHeight) (id); }
	void SetTLHeight(Int32 id, Int32 size){ CaCall(SetTLHeight) (id, size); }
};

#endif
