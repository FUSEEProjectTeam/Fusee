#include "ObjectDataM.h"
#include "c4d_baseobject.h"

ObjectDataM::ObjectDataM(void)
{
}


ObjectDataM::~ObjectDataM(void)
{
}

Bool ObjectDataM::GetDDescription(GeListNode *node, Description *description, DESCFLAGS_DESC &flags)
{
	ObjectData::GetDDescription(node, description, flags);
	
	DDescriptionParams parms;
	parms.Desc = description;
	parms.Flags = flags;
	Bool ret = GetDDescription(node, &parms);
	flags = parms.Flags;
	return ret;
}

Bool ObjectDataM::GetDDescription(GeListNode *node, DDescriptionParams *descparams)
{
	// Do nothing. To be overridden (in C#)
	return true;
}

BaseContainer *ObjectDataM::GetDataInstance(BaseObject *op)
{
	return op->GetDataInstance();
}

BaseContainer *ObjectDataM::GetDataInstance(GeListNode *node)
{
	BaseObject		*op   = (BaseObject*)node;
	BaseContainer *data = op->GetDataInstance();
	return data;
}