/////////////////////////////////////////////////////////////
// CINEMA 4D SDK                                           //
/////////////////////////////////////////////////////////////
// (c) MAXON Computer GmbH, all rights reserved            //
/////////////////////////////////////////////////////////////

#ifndef _CUSTOMGUI_IN_EXCLUDE_H_
#define _CUSTOMGUI_IN_EXCLUDE_H_

#include "customgui_base.h"
#include "c4d_basecontainer.h"
#include "c4d_gui.h"
#include "c4d_customdatatype.h"

class BaseDocument;

#define CUSTOMGUI_INEXCLUDE_LIST			    1009290
#define CUSTOMDATATYPE_INEXCLUDE_LIST			1009290


#define IN_EXCLUDE_DATA_SELECTION      10          // Bool - TRUE, if IN_EXCLUDE_FLAG_SEND_SELCHANGE_MSG is set
                                                   // and the object is selected in the list

#define IN_EXCLUDE_FLAG_NUM_FLAGS      2000        // Int32 - the number of different flags
#define IN_EXCLUDE_FLAG_INIT_STATE     2065        // Int32 - the initial state of a new inserted element
#define IN_EXCLUDE_FLAG_SEND_SELCHANGE_MSG    2066 // Bool - TRUE, if the selection state should be stored
                                                   // in the object's data container
#define IN_EXCLUDE_FLAG_IMAGE_01_ON    2001        // String - image of "On" state for the column
#define IN_EXCLUDE_FLAG_IMAGE_01_OFF   2002        // String - image of "Off" state for the column
#define IN_EXCLUDE_FLAG_IMAGE_02_ON    2003
#define IN_EXCLUDE_FLAG_IMAGE_02_OFF   2004
#define IN_EXCLUDE_FLAG_IMAGE_03_ON    2005
#define IN_EXCLUDE_FLAG_IMAGE_03_OFF   2006
#define IN_EXCLUDE_FLAG_IMAGE_04_ON    2007
#define IN_EXCLUDE_FLAG_IMAGE_04_OFF   2008
#define IN_EXCLUDE_FLAG_IMAGE_05_ON    2009
#define IN_EXCLUDE_FLAG_IMAGE_05_OFF   2010
#define IN_EXCLUDE_FLAG_IMAGE_06_ON    2011
#define IN_EXCLUDE_FLAG_IMAGE_06_OFF   2012
#define IN_EXCLUDE_FLAG_IMAGE_07_ON    2013
#define IN_EXCLUDE_FLAG_IMAGE_07_OFF   2014
#define IN_EXCLUDE_FLAG_IMAGE_08_ON    2015
#define IN_EXCLUDE_FLAG_IMAGE_08_OFF   2016
#define IN_EXCLUDE_FLAG_IMAGE_09_ON    2017
#define IN_EXCLUDE_FLAG_IMAGE_09_OFF   2018
#define IN_EXCLUDE_FLAG_IMAGE_10_ON    2019
#define IN_EXCLUDE_FLAG_IMAGE_10_OFF   2020
#define IN_EXCLUDE_FLAG_IMAGE_11_ON    2021
#define IN_EXCLUDE_FLAG_IMAGE_11_OFF   2022
#define IN_EXCLUDE_FLAG_IMAGE_12_ON    2023
#define IN_EXCLUDE_FLAG_IMAGE_12_OFF   2024
#define IN_EXCLUDE_FLAG_IMAGE_13_ON    2025
#define IN_EXCLUDE_FLAG_IMAGE_13_OFF   2026
#define IN_EXCLUDE_FLAG_IMAGE_14_ON    2027
#define IN_EXCLUDE_FLAG_IMAGE_14_OFF   2028
#define IN_EXCLUDE_FLAG_IMAGE_15_ON    2029
#define IN_EXCLUDE_FLAG_IMAGE_15_OFF   2030
#define IN_EXCLUDE_FLAG_IMAGE_16_ON    2031
#define IN_EXCLUDE_FLAG_IMAGE_16_OFF   2032
#define IN_EXCLUDE_FLAG_IMAGE_17_ON    2033
#define IN_EXCLUDE_FLAG_IMAGE_17_OFF   2034
#define IN_EXCLUDE_FLAG_IMAGE_18_ON    2035
#define IN_EXCLUDE_FLAG_IMAGE_18_OFF   2036
#define IN_EXCLUDE_FLAG_IMAGE_19_ON    2037
#define IN_EXCLUDE_FLAG_IMAGE_19_OFF   2038
#define IN_EXCLUDE_FLAG_IMAGE_20_ON    2039
#define IN_EXCLUDE_FLAG_IMAGE_20_OFF   2040
#define IN_EXCLUDE_FLAG_IMAGE_21_ON    2041
#define IN_EXCLUDE_FLAG_IMAGE_21_OFF   2042
#define IN_EXCLUDE_FLAG_IMAGE_22_ON    2043
#define IN_EXCLUDE_FLAG_IMAGE_22_OFF   2044
#define IN_EXCLUDE_FLAG_IMAGE_23_ON    2045
#define IN_EXCLUDE_FLAG_IMAGE_23_OFF   2046
#define IN_EXCLUDE_FLAG_IMAGE_24_ON    2047
#define IN_EXCLUDE_FLAG_IMAGE_24_OFF   2048
#define IN_EXCLUDE_FLAG_IMAGE_25_ON    2049
#define IN_EXCLUDE_FLAG_IMAGE_25_OFF   2050
#define IN_EXCLUDE_FLAG_IMAGE_26_ON    2051
#define IN_EXCLUDE_FLAG_IMAGE_26_OFF   2052
#define IN_EXCLUDE_FLAG_IMAGE_27_ON    2053
#define IN_EXCLUDE_FLAG_IMAGE_27_OFF   2054
#define IN_EXCLUDE_FLAG_IMAGE_28_ON    2055
#define IN_EXCLUDE_FLAG_IMAGE_28_OFF   2056
#define IN_EXCLUDE_FLAG_IMAGE_29_ON    2057
#define IN_EXCLUDE_FLAG_IMAGE_29_OFF   2058
#define IN_EXCLUDE_FLAG_IMAGE_30_ON    2059
#define IN_EXCLUDE_FLAG_IMAGE_30_OFF   2060
#define IN_EXCLUDE_FLAG_IMAGE_31_ON    2061
#define IN_EXCLUDE_FLAG_IMAGE_31_OFF   2062
#define IN_EXCLUDE_FLAG_IMAGE_32_ON    2063
#define IN_EXCLUDE_FLAG_IMAGE_32_OFF   2064


class InclusionTable;

class InExcludeData : public CustomDataType
{
private:
  InExcludeData();
  ~InExcludeData();
public:
  Bool InsertObject(BaseList2D* pObject, Int32 lFlags);
  Int32 GetObjectIndex(BaseDocument *doc, BaseList2D* pObject) const;
  Bool DeleteObject(BaseDocument *doc, BaseList2D* pObject) { return DeleteObject(GetObjectIndex(doc,pObject)); }
  Int32 GetFlags(Int32 lIndex) const;
  void SetFlags(Int32 lIndex, Int32 lFlags);
  Int32 GetFlags(BaseDocument *doc, BaseList2D* pObject) const { return GetFlags(GetObjectIndex(doc,pObject)); }
 BaseContainer* GetData(Int32 lIndex) const;
  BaseContainer* GetData(BaseDocument *doc, BaseList2D* pObject) const { return GetData(GetObjectIndex(doc, pObject)); }
  BaseList2D* ObjectFromIndex(BaseDocument *doc, Int32 lIndex) const;

   //returns a table that contains all included objects. Delete the list by calling FreeInclusionTable(table)
   //hierarchy_bit starts at 0
  InclusionTable *BuildInclusionTable(BaseDocument *doc, Int32 hierarchy_bit = NOTOK);

  Bool DeleteObject(Int32 lIndex);
  Int32 GetObjectCount() const;
};

class InclusionTable
{
private:
  InclusionTable();
  ~InclusionTable();

public:
	Bool Check(BaseList2D *op);
	Bool Check(BaseList2D *op, Int32 &flags);
  Int32 GetObjectCount();
  BaseList2D* GetObject(Int32 lIndex);
};

void FreeInclusionTable(InclusionTable*& pTable);

class InExcludeCustomGui : public BaseCustomGui<CUSTOMGUI_INEXCLUDE_LIST>
{
private:
	InExcludeCustomGui();
	~InExcludeCustomGui();
public:
};





// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF


#ifndef _INTERNAL_DEF_
	class iInExcludeCustomGui : public iBaseCustomGui
	{
		iInExcludeCustomGui(const BaseContainer &settings,CUSTOMGUIPLUGIN *plugin) : iBaseCustomGui(settings,plugin) { }
	};
  class iIncludeExcludeData {};
  class iInclusionTable {};
#else
	class iInExcludeCustomGui;
  class iIncludeExcludeData;
  class iInclusionTable;
#endif

struct CustomGuiInExcludeLib : public BaseCustomGuiLib
{
  Bool            (iIncludeExcludeData::*InsertObject)(BaseList2D* pObject, Int32 lFlags);
  Int32            (iIncludeExcludeData::*GetObjectIndex)(BaseDocument *doc, BaseList2D* pObject);
  Bool            (iIncludeExcludeData::*DeleteObject)(Int32 lIndex);
  Int32            (iIncludeExcludeData::*GetObjectCount)();
  Int32            (iIncludeExcludeData::*GetFlags)(Int32 lIndex);
  BaseList2D*     (iIncludeExcludeData::*ObjectFromIndex)(BaseDocument *doc, Int32 lIndex);
  InclusionTable* (iIncludeExcludeData::*BuildInclusionTable)(BaseDocument *doc, Int32 hierarchy_bit);

  Bool            (iInclusionTable::*Check)(BaseList2D *op);
  Int32            (iInclusionTable::*GetObjectCountT)();
  BaseList2D*     (iInclusionTable::*GetObject)(Int32 lIndex);
  void            (*LIB_FreeInclusionTable)(iInclusionTable *pTable);
  BaseContainer*  (iIncludeExcludeData::*GetData)(Int32 lIndex);
	Bool            (iInclusionTable::*CheckFlags)(BaseList2D *op, Int32 &f);
	void            (iIncludeExcludeData::*SetFlags)(Int32 lIndex, Int32 lFlags);
};

// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF
// INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF -- INTERNAL STUFF

#endif // _CUSTOMGUI_IN_EXCLUDE_H_
