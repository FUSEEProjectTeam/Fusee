/////////////////////////////////////////////////////////////
// CINEMA 4D SDK                                           //
/////////////////////////////////////////////////////////////
// (c) MAXON Computer GmbH, all rights reserved            //
/////////////////////////////////////////////////////////////

#ifndef __C4D_CUSTOMDATATYPE_H
#define __C4D_CUSTOMDATATYPE_H

#include "c4d_basedata.h"
#include "c4d_memory.h"
#include "c4d_customguidata.h"
#include "c4d_graphview_enum.h"

class String;
class HyperFile;
class AliasTrans;
class GeData;
class DescID;
class Description;
struct GV_VALUE_HANDLER;

#define CUSTOMDATATYPE_INFO_HASSUBDESCRIPTION					(1<<0)
#define CUSTOMDATATYPE_INFO_NEEDDATAFORSUBDESC				(1<<1)
#define CUSTOMDATATYPE_INFO_TOGGLEDISPLAY							(1<<2)
#define CUSTOMDATATYPE_INFO_DONTREGISTERGVTYPE				(1<<3)
#define CUSTOMDATATYPE_INFO_SUBDESCRIPTIONDISABLEGUI	(1<<4)
#define CUSTOMDATATYPE_INFO_UNDOSAMECUSTOMGUI					(1<<5)
#define CUSTOMDATATYPE_INFO_HASSUBDESCRIPTION_NOANIM	((1<<0) | (1<<6))
#define CUSTOMDATATYPE_INFO_LOADSAVE									(1<<7)

struct CustomDataType
{
	

};


template <class X> struct iCustomDataType : public CustomDataType
	{
		static X* Alloc() { return gNew X; }
		static void Free(X* &data) { gDelete (data); }
	};

struct GvHelper
{
	CustomDataType **data;
};


class CustomDataTypeClass : public BaseData
{
		Int32 defaultconversiontype;
		GV_VALUE_HANDLER *valuehandler;

	public:

		virtual Int32 GetId() = 0;

		virtual Int32 GetDataID();
		virtual Int32 GetValueID();

		virtual CustomDataType*	AllocData() = 0;
		virtual void FreeData(CustomDataType *data) = 0;

		virtual Bool CopyData(const CustomDataType *src,CustomDataType *dest,AliasTrans *aliastrans) = 0;
		virtual Int32 Compare(const CustomDataType *d1,const CustomDataType *d2) = 0;

		virtual Bool WriteData(const CustomDataType *d,HyperFile *hf) = 0;
		virtual Bool ReadData(CustomDataType *d,HyperFile *hf,Int32 level) = 0;

		virtual const Char *GetResourceSym() = 0;
		virtual CustomProperty *GetProperties();
		virtual void GetDefaultProperties(BaseContainer &data);

		virtual Int32 GetConversionsFrom(Int32 *&table);
		virtual GvError ConvertFromGv(Int32 src_type,const void *const src,Int32 cpu_id,CustomDataType *dst);

		virtual Int32 GetConversionsTo(Int32 *&table);
		virtual GvError ConvertToGv(Int32 dst_type,const CustomDataType *src,void *dst,Int32 cpu_id);
		virtual GvError ConvertToGeData(Int32 dst_type,const CustomDataType *src,GeData &dst);

		virtual GvValueFlags GetCalculationFlags();
		virtual GvError Calculate(Int32 calculation,const CustomDataType *src1, const CustomDataType *src2, CustomDataType *dst, Float parm1);

		virtual GV_VALUE_HANDLER *GetGvValueHandler();

		virtual Bool ConvertGeDataToGv(const GeData &src,void *dst,Int32 cpu_id);
		virtual Bool ConvertGvToGeData(const void *const src,Int32 cpu_id,GeData &dst);

		virtual Bool GetDescription() { return FALSE; } // to get the virtual warning
		virtual Bool _GetDescription(const CustomDataType *data,Description &res,DESCFLAGS_DESC &flags,const BaseContainer &parentdescription,DescID *singledescid);
		virtual Bool GetParameter(const CustomDataType *data,const DescID &id,GeData &t_data,DESCFLAGS_GET &flags);
		virtual Bool SetDParameter(CustomDataType *data,const DescID &id,const GeData &t_data,DESCFLAGS_SET &flags);
		virtual Bool GetEnabling(const CustomDataType *data,const DescID &id,const GeData &t_data,DESCFLAGS_ENABLE &flags,const BaseContainer *itemdesc);
		virtual Bool InterpolateKeys(GeData &res, const GeData &t_data1,const GeData &t_data2,Float mix,Int32 flags);

		virtual void CheckData(const BaseContainer &bc,GeData &data);
};

struct CUSTOMDATATYPEPLUGIN;

class ResourceDataTypeClass : public BaseData
{
		Int32 datatypeid;
		CUSTOMDATATYPEPLUGIN *datatype;

	public:

		ResourceDataTypeClass(Int32 datatypeid,CUSTOMDATATYPEPLUGIN *datatype);

		virtual Int32 GetId() = 0;

		virtual Int32 GetCustomDataType();
		virtual CUSTOMDATATYPEPLUGIN *GetCustomDataTypePlugin();
		virtual void GetDefaultProperties(BaseContainer &data);

		virtual const Char *GetResourceSym();
		virtual CustomProperty *GetProperties();

		virtual Bool GetDescription(const CustomDataType *data,Description &res,DESCFLAGS_DESC &flags,const BaseContainer &parentdescription,DescID *singledescid);
		virtual void CheckData(const BaseContainer &bc,GeData &data);
};

Bool RegisterCustomDataTypePlugin  (const String &str, Int32 info, CustomDataTypeClass   *dat,Int32 disclevel);
Bool RegisterResourceDataTypePlugin(const String &str, Int32 info, ResourceDataTypeClass *dat,Int32 disclevel);

#endif
