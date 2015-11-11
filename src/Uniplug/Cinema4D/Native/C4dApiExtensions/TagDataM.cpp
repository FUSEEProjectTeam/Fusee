#include "c4d_basetag.h"
#include "TagDataM.h"

TagDataM::TagDataM(void)
{
}


TagDataM::~TagDataM(void)
{
}

Bool TagDataM::GetDDescription(GeListNode *node, Description *description, DESCFLAGS_DESC &flags)
{
	TagData::GetDDescription(node, description, flags);
	
	DDescriptionParams parms;
	parms.Desc = description;
	parms.Flags = flags;
	Bool ret = GetDDescription(node, &parms);
	flags = parms.Flags;
	return ret;
}

Bool TagDataM::GetDDescription(GeListNode *node, DDescriptionParams *descparams)
{
	// Do nothing. To be overridden (in C#)
	return true;
}

BaseContainer *TagDataM::GetDataInstance(BaseTag *op)
{
	return op->GetDataInstance();
}

BaseContainer *TagDataM::GetDataInstance(GeListNode *node)
{
	BaseTag 		*op   = (BaseTag*)node;
	BaseContainer *data = op->GetDataInstance();
	return data;
}