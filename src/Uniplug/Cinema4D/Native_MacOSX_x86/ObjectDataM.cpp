#include "ObjectDataM.h"


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
	return GetDDescription(node, &parms);
}

Bool ObjectDataM::GetDDescription(GeListNode *node, DDescriptionParams *descparams)
{
	// Do nothing - to be overridden (in C#)
}