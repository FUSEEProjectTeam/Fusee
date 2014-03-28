/////////////////////////////////////////////////////////////
// CINEMA 4D SDK                                           //
/////////////////////////////////////////////////////////////
// (c) MAXON Computer GmbH, all rights reserved            //
/////////////////////////////////////////////////////////////

#ifndef __C4D_GRAPHVIEW_H
#define __C4D_GRAPHVIEW_H

////////////////////////////////

#include "c4d_general.h"
#include "c4d_graphview_def.h"
#include "c4d_basecontainer.h"
#include "c4d_memory.h"
#include "c4d_resource.h"
#include "c4d_gui.h"
#include "c4d_nodeplugin.h"
#include "c4d_baselist.h"
#include "c4d_basebitmap.h"
#include "c4d_customdatatypeplugin.h"

#ifdef USE_API_MAXON
#include "sortedarray.h"
#endif

////////////////////////////////

class GeDialog;
class GeUserArea;
class BaseBitmap;
class GvNodeMaster;
class GvOperatorData;
class	GvNode;
class	GvPort;
class GvNodeGUI;
class GvShape;
class GvRun;
class GvCalc;
class GvQuery;
class GvInit;
class GvValue;
class GvWorld;
class GvCalcTable;
class GvCalcTime;
class GvPortList;
struct GvPortListEntry;

struct GvCopyBuffer;
struct OPERATORPLUGIN;

class BaseDrawHelp;

////////////////////////////////
// GvPortList's

struct GvPortListEntry
{
	Int32 id;

	GvPortListEntry(Int32 t_id) : id(t_id) {}
};

class GvPortList
{
	private:
		GvPortList();
		~GvPortList();

	public:
		Int32 GetCount(void) const									{ return (((iGvPortList*)this)->*C4DOS.Gv->portlist->GetCount)(); }
		Bool Append(GvPortListEntry *e)						{ return (((iGvPortList*)this)->*C4DOS.Gv->portlist->Append)(e); }
		Bool Remove(GvPortListEntry *e)						{ return (((iGvPortList*)this)->*C4DOS.Gv->portlist->Remove)(e); }
		GvPortListEntry* GetIndex(Int32 i) const		{ return (((iGvPortList*)this)->*C4DOS.Gv->portlist->GetIndex)(i); }
		void FlushAll(void)												{ (((iGvPortList*)this)->*C4DOS.Gv->portlist->FlushAll)(); }

		static GvPortList* Alloc()								{ return (GvPortList*)C4DOS.Gv->portlist->Alloc(); }
		static void Free(GvPortList* &list)				{ C4DOS.Gv->portlist->Free((iGvPortList*&)list); }
};

////////////////////////////////
// GvCalc class

class GvCalcTime
{
	private:
		GvCalcTime();
		~GvCalcTime();

	public:
		Bool						init;
		Bool						init_time;
		Bool						time_changed;
		Bool						loop_changed;
		Bool						length_changed;
		BaseTime				time;
		BaseTime				delta;
		BaseTime				start;
		BaseTime				end;
		BaseTime				loop_start;
		BaseTime				loop_end;
		BaseTime				previous;
		Int32						fps;
};

class GvCalc
{
	private:
		GvCalc();
		~GvCalc();

	public:
		GvCalcTime			time;
		Int32						cpu_count;
		UInt32						flags;
		BaseDocument*		document;
		GvNodeMaster*		master;
		UInt32						counter;
		BaseThread*			thread;
};

////////////////////////////////
// GvInit class

class GvInit
{
	private:
		GvInit();
		~GvInit();

	public:
		Int32						cpu_count;
		GvCalcFlags			flags;

	public:
		void						SetError(GvCalcError error) { C4DOS.Gv->init->SetError(this,error); }
		GvCalcError			GetError(void) { return C4DOS.Gv->init->GetError(this); }
};

////////////////////////////////
// GvValue class

class GvValue
{
	private:
		GvValue();
		~GvValue();

	public:
		Bool						Calculate(GvNode *node, GvPortIO io, GvRun *r, GvCalc *c, Int32 index=0, BaseTime *time = nullptr) { return C4DOS.Gv->value->Calculate(this,node,io,r,c,index,time); }
		Bool						IsConnected(Int32 index) { return C4DOS.Gv->value->IsConnected(this,index); }
		Bool						IsPort(void) { return C4DOS.Gv->value->IsPort(this); }
		Int32						NrOfPorts(void) { return C4DOS.Gv->value->NrOfPorts(this); }
		GvValueID				GetValueID(void) { return C4DOS.Gv->value->GetValueID(this); }
		Int32						GetMainID() { return C4DOS.Gv->value->GetMainID(this); }
		GvPort*					GetPort(Int32 index = 0) { return C4DOS.Gv->value->GetPort(this,index); }
};

////////////////////////////////
// GvQuery class

class GvQuery
{
	private:
		GvQuery();
		~GvQuery();

	public:
		GvCalcFlags			GetFlags(void) { return C4DOS.Gv->query->GetFlags(this); }
		void						SetFlags(GvCalcFlags flags) { C4DOS.Gv->query->SetFlags(this,flags); }
		void						SetError(GvCalcError error) { C4DOS.Gv->query->SetError(this,error); }
		GvCalcError			GetError(void) { return C4DOS.Gv->query->GetError(this); }
};

////////////////////////////////
// GvRun class

class GvRun
{
	private:
		GvRun();
		~GvRun();

	public:
		void						SetCalculationTable(GvCalcTable *t) { C4DOS.Gv->run->SetCalculationTable(this,t); }
		Bool						AddNodeToCalculationTable(GvNode *node, Bool force_add = false) { return C4DOS.Gv->run->AddNodeToCalculationTable(this,node,force_add); }
		Bool						CalculateTable(GvCalc *calc) { return C4DOS.Gv->run->CalculateTable(this,calc); }
		Int32						GetCpuID(void) { return C4DOS.Gv->run->GetCpuID(this); }
		void						IncrementID(void) { C4DOS.Gv->run->IncrementID(this); }
		void						SetError(GvCalcError error) { C4DOS.Gv->run->SetError(this,error); }
		GvCalcError			GetError(void) { return C4DOS.Gv->run->GetError(this); }
		void						SetState(GvCalcState state) { C4DOS.Gv->run->SetState(this,state); }
		GvCalcState			GetState(void) { return C4DOS.Gv->run->GetState(this); }
		Bool						IsIterationPath(void) { return C4DOS.Gv->run->IsIterationPath(this); }
};

////////////////////////////////
// GvCalcTable class

class GvCalcTable
{
	private:
		GvCalcTable();
		~GvCalcTable();

	public:
		Bool						AddNodeToTable(GvRun *run, GvNode *node) { return C4DOS.Gv->table->AddNodeToTable(this,run,node); }
		void						ResetTable(GvRun *run) { C4DOS.Gv->table->ResetTable(this,run); }
		Int32						GetTableCount(GvRun *run) { return C4DOS.Gv->table->GetTableCount(this,run); }
		GvNode*					GetTableNode(GvRun *run, Int32 id) { return C4DOS.Gv->table->GetTableNode(this,run,id); }
};

////////////////////////////////
// GvNode class

class GvNode : public BaseList2D
{
	private:
		GvNode();
		~GvNode();

	public:
		GvNode*					GetNext(void) { return (GvNode*)AtCall(GetNext)(); }
		GvNode*					GetPred(void) { return (GvNode*)AtCall(GetPred)(); }
		GvNode*					GetUp  (void) { return (GvNode*)AtCall(GetUp)(); }
		GvNode*					GetDown(void) { return (GvNode*)AtCall(GetDown)(); }

		void						Redraw(void) { C4DOS.Gv->node->Redraw(this); }

		const String		GetTitle() { return C4DOS.Gv->node->GetTitle(this,0); }
		void						SetTitle(const String &title)  { C4DOS.Gv->node->SetTitle(this,title,0); }

		Bool						CreateOperator() { return C4DOS.Gv->node->CreateOperator(this,0); }

		GvNodeMaster*		GetNodeMaster(void) { return C4DOS.Gv->node->GetNodeMaster(this); }
		GvOperatorData*	GetOperatorData(void) { return C4DOS.Gv->node->GetOperatorData(this); }
		Int32						GetOperatorID(void) { return C4DOS.Gv->node->GetOperatorID(this); }
		Int32						GetOwnerID(void) { return C4DOS.Gv->node->GetOwnerID(this); }
		Bool						IsGroupNode(void) { return C4DOS.Gv->node->IsGroupNode(this); }

		GvPort*					AddPort(GvPortIO io, Int32 id, GvPortFlags flags = GV_PORT_FLAG_IS_VISIBLE, Bool message = false) { return C4DOS.Gv->node->AddPort(this,io,id,flags,message); }
		Bool						AddPortIsOK(GvPortIO io, Int32 id)  { return C4DOS.Gv->node->AddPortIsOK(this,io,id); }
		Bool						SetPortType(GvPort *port, Int32 id) { return C4DOS.Gv->node->SetPortType(this,port,id); }
		Bool						ResetPortType(Int32 id) { return C4DOS.Gv->node->ResetPortType(this,id); }
		void						RemovePort(GvPort *port, Bool message = true) { C4DOS.Gv->node->RemovePort(this,port,message); }
		Bool						RemovePortIsOK(GvPort *port) { return C4DOS.Gv->node->RemovePortIsOK(this,port); }
		void						RemoveUnusedPorts(Bool message = true) { C4DOS.Gv->node->RemoveUnusedPorts(this,message); }

		GvPort*					AddConnection(GvNode *source_node, GvPort *source_port, GvNode *dest_node, GvPort *dest_port) { return C4DOS.Gv->node->AddConnection(this,source_node,source_port,dest_node,dest_port); }
		void						RemoveConnections(void) {	C4DOS.Gv->node->RemoveNodeConnections(this); }

		void						GetPortList(GvPortIO port, GvPortList &portlist) { C4DOS.Gv->node->GetPortList(this,port,portlist,0); }
		Bool						GetPortDescription(GvPortIO port, Int32 id, GvPortDescription *pd) { return C4DOS.Gv->node->GetPortDescription(this,port,id,pd,0); }

		Int32						GetInPortCount(void) { return C4DOS.Gv->node->GetInPortCount(this); }
		Int32						GetOutPortCount(void) { return C4DOS.Gv->node->GetOutPortCount(this); }

		GvPort*					GetInPort(Int32 index) { return C4DOS.Gv->node->GetInPort(this,index); }
		GvPort*					GetOutPort(Int32 index) { return C4DOS.Gv->node->GetOutPort(this,index); }
		GvPort*					GetInPortFirstMainID(Int32 id) { return C4DOS.Gv->node->GetInPortFirstMainID(this,id); }
		GvPort*					GetOutPortFirstMainID(Int32 id) { return C4DOS.Gv->node->GetOutPortFirstMainID(this,id); }
		GvPort*					GetInPortMainID(Int32 id, Int32 &start) { return C4DOS.Gv->node->GetInPortMainID(this,id,start); }
		GvPort*					GetOutPortMainID(Int32 id, Int32 &start) { return C4DOS.Gv->node->GetOutPortMainID(this,id,start); }
		GvPort*					GetInPortSubID(Int32 id) { return C4DOS.Gv->node->GetInPortSubID(this,id); }
		GvPort*					GetOutPortSubID(Int32 id) { return C4DOS.Gv->node->GetOutPortSubID(this,id); }

		GvPort*					GetPort(Int32 sub_id) { return C4DOS.Gv->node->GetPort(this,sub_id); }
		Int32						GetPortIndex(Int32 sub_id) { return C4DOS.Gv->node->GetPortIndex(this,sub_id); }

		GvPort*					CalculateInPortIndex(Int32 port_index, GvRun *run, GvCalc *calc) { return C4DOS.Gv->node->NodeCalculateInPortIndex(this,port_index,run,calc); }
		GvPort*					CalculateOutPortIndex(Int32 port_index, GvRun *run, GvCalc *calc) { return C4DOS.Gv->node->NodeCalculateOutPortIndex(this,port_index,run,calc); }
		GvPort*					CalculateInPort(GvPort *port, GvRun *run, GvCalc *calc) { return C4DOS.Gv->node->NodeCalculateInPort(this,port,run,calc); }
		GvPort*					CalculateOutPort(GvPort *port, GvRun *run, GvCalc *calc) { return C4DOS.Gv->node->NodeCalculateOutPort(this,port,run,calc); }
		Bool						SetRecalculate(GvRun *r, Bool force_set = false) { return C4DOS.Gv->node->SetRecalculate(this,r,force_set); }

		BaseContainer*	GetOpContainerInstance(void) { return C4DOS.Gv->node->GetOpContainerInstance(this); }
		BaseContainer		GetOperatorContainer(void) { return C4DOS.Gv->node->GetOperatorContainer(this); }
		void						SetOperatorContainer(const BaseContainer &bc) { C4DOS.Gv->node->SetOperatorContainer(this,bc); }

		const String		OperatorGetDetailedText(void) { return C4DOS.Gv->node->OperatorGetDetailedText(this); }
		const String		OperatorGetErrorString(Int32 error) { return C4DOS.Gv->node->OperatorGetErrorString(this,error); }
		Bool						OperatorSetData(GvDataType type, void *data, GvOpSetDataMode mode) { return C4DOS.Gv->node->OperatorSetData(this,type,data,mode); }
		Bool						OperatorIsSetDataAllowed(GvDataType type, void *data, GvOpSetDataMode mode) { return C4DOS.Gv->node->OperatorIsSetDataAllowed(this,type,data,mode); }
		void						OperatorGetIcon(IconData &dat) { C4DOS.Gv->node->OperatorGetIcon(this,dat); }

		Bool						GetSelectState(void) { return C4DOS.Gv->node->GetSelectState(this); }
		Bool						GetFailureState(void) { return C4DOS.Gv->node->GetFailureState(this); }
		Bool						GetDisabledState(void) { return C4DOS.Gv->node->GetDisabledState(this); }
		Bool						GetOpenState(void) { return C4DOS.Gv->node->GetOpenState(this); }
		void						SetOpenState(Bool state) { C4DOS.Gv->node->SetOpenState(this,state); }
		Bool						GetLockState(void)  { return C4DOS.Gv->node->GetLockState(this); }
		void						SetLockState(Bool state) { C4DOS.Gv->node->SetLockState(this,state); }
		Bool						GetShowPortNamesState(void)  { return C4DOS.Gv->node->GetShowPortNamesState(this); }
		void						SetShowPortNamesState(Bool state) { C4DOS.Gv->node->SetShowPortNamesState(this,state); }

		GvValue*				AllocCalculationHandler(Int32 main_id, GvCalc *calc, GvRun *run, Int32 singleport) { return C4DOS.Gv->node->AllocCalculationHandler(this,main_id,calc,run,singleport); }
		void						FreeCalculationHandler(GvValue *&value) { C4DOS.Gv->node->FreeCalculationHandler(this,value); }

		Bool						CalculateRawData(GvValueID value_id, const void* const data1, const void* const data2, void *dest, GvValueFlags calculation, Float parm1) { return C4DOS.Gv->node->CalculateRawData(this,value_id,data1,data2,dest,calculation,parm1); }
};

////////////////////////////////
// GvPort class

class GvPort
{
	private:
		GvPort();
		~GvPort();

	public:
		void						RemoveConnection(void) { C4DOS.Gv->port->RemoveConnection(this); }
		Int32						GetNrOfConnections(void) { return C4DOS.Gv->port->GetNrOfConnections(this); }
		void						RemovePortConnections(void) { C4DOS.Gv->port->RemovePortConnections(this); }
		Bool						IsIncomingConnected(void) { return C4DOS.Gv->port->IsIncomingConnected(this); }
		Bool						GetIncomingDestination(GvNode *&node, GvPort *&port) { return C4DOS.Gv->port->GetIncomingDestination(this,node,port); }
		Bool						GetIncomingSource(GvNode *&node, GvPort *&port) { return C4DOS.Gv->port->GetIncomingSource(this,node,port); }

		GvPortIO				GetIO(void) { return C4DOS.Gv->port->GetIO(this); }
		void						SetMainID(Int32 id) { C4DOS.Gv->port->SetMainID(this,id); }
		Int32						GetMainID(void) { return C4DOS.Gv->port->GetMainID(this); }
		void						SetUserID(Int32 id) { C4DOS.Gv->port->SetUserID(this,id); }
		Int32						GetUserID(void) { return C4DOS.Gv->port->GetUserID(this); }
		Int32						GetSubID(void) { return C4DOS.Gv->port->GetSubID(this); }
		GvValueID				GetValueType(void) { return C4DOS.Gv->port->GetValueType(this); }
		void						SetVisible(Bool v) { C4DOS.Gv->port->SetVisible(this,v); }
		Bool						GetVisible(void) { return C4DOS.Gv->port->GetVisible(this); }
		void						SetValid(GvRun *r, Bool v) { C4DOS.Gv->port->SetValid(this,r,v); }
		Bool						GetValid(GvRun *r) { return C4DOS.Gv->port->GetValid(this,r); }
		void						SetCalculated(GvRun *r) { C4DOS.Gv->port->SetCalculated(this,r); }
		GvPort*					Calculate(GvNode *bn, GvRun *r, GvCalc *c) { return C4DOS.Gv->port->Calculate(this,bn,r,c); }
		Bool						SetRecalculate(GvRun *r, Bool force_set = false) { return C4DOS.Gv->port->SetRecalculate(this,r,force_set); }

		Bool						GetBool(Bool *b, GvRun *r) { return C4DOS.Gv->port->GetBool(this,b,r); }
		Bool						GetInteger(Int32 *i, GvRun *r) { return C4DOS.Gv->port->GetInteger(this,i,r); }
		Bool						GetFloat(Float *f, GvRun *r) { return C4DOS.Gv->port->GetFloat(this,f,r); }
		Bool						GetVector(Vector *v, GvRun *r) { return C4DOS.Gv->port->GetVector(this,v,r); }
		Bool						GetNormal(Vector *n, GvRun *r) { return C4DOS.Gv->port->GetNormal(this,n,r); }
		Bool						GetMatrix(Matrix *m, GvRun *r) { return C4DOS.Gv->port->GetMatrix(this,m,r); }
		Bool						GetTime(BaseTime *t, GvRun *r) { return C4DOS.Gv->port->GetTime(this,t,r); }
		Bool						GetString(String *s, GvRun *r) { return C4DOS.Gv->port->GetString(this,s,r); }
		Bool						GetObject(BaseList2D *&o, GvRun *r) { return C4DOS.Gv->port->GetObject(this,o,r); }
		Bool						GetData(void *d, GvValueID type, GvRun *r) { return C4DOS.Gv->port->GetData(this,d,type,r); }
		Bool						GetDataInstance(const void *&d, GvValueID type, GvRun *r) { return C4DOS.Gv->port->GetDataInstance(this,d,type,r); }

		Bool						SetBool(Bool b, GvRun *r) { return C4DOS.Gv->port->SetBool(this,b,r); }
		Bool						SetInteger(Int32 i, GvRun *r) { return C4DOS.Gv->port->SetInteger(this,i,r); }
		Bool						SetFloat(Float f, GvRun *r) { return C4DOS.Gv->port->SetFloat(this,f,r); }
		Bool						SetVector(const Vector &v, GvRun *r) { return C4DOS.Gv->port->SetVector(this,v,r); }
		Bool						SetNormal(const Vector &n, GvRun *r) { return C4DOS.Gv->port->SetNormal(this,n,r); }
		Bool						SetMatrix(const Matrix &m, GvRun *r) { return C4DOS.Gv->port->SetMatrix(this,m,r); }
		Bool						SetTime(const BaseTime &t, GvRun *r) { return C4DOS.Gv->port->SetTime(this,t,r); }
		Bool						SetString(const String &s, GvRun *r) { return C4DOS.Gv->port->SetString(this,s,r); }
		Bool						SetObject(const BaseList2D* o, GvRun *r) { return C4DOS.Gv->port->SetObject(this,o,r); }
		Bool						SetData(const void* d, GvValueID type, GvRun *r) { return C4DOS.Gv->port->SetData(this,d,type,r); }

		const String		GetName(GvNode *node) { return C4DOS.Gv->port->GetName(this,node); }
		void						SetName(const String &name) { C4DOS.Gv->port->SetName(this,name); }

		Bool						CopyPortData(GvPort *source, GvRun *r) { return C4DOS.Gv->port->CopyPortData(this,source,r); }
		Bool						CopyRawData(void *source, GvRun *r) { return C4DOS.Gv->port->CopyRawData(this,source,r); }
		Bool						CalculateRawData(void *data, void *dest, GvRun *r, GvValueFlags calculation, Float parm1 = (Float)0.0) { return C4DOS.Gv->port->CalculateRawData(this,data,dest,r,calculation,parm1); }
		Bool						CalculateRawRawData(void *data1, void *data2,void *dest, GvRun *r, GvValueFlags calculation, Float parm1 = (Float)0.0) { return C4DOS.Gv->port->CalculateRawRawData(this,data1,data2,dest,r,calculation,parm1); }
		Bool						CalculatePortData(GvPort *data, void *dest, GvRun *r, GvValueFlags calculation, Float parm1 = (Float)0.0) { return C4DOS.Gv->port->CalculatePortData(this,data,dest,r,calculation,parm1); }
		Bool						CalculateRawDataRev(void *data, void *dest, GvRun *r, GvValueFlags calculation, Float parm1 = (Float)0.0) { return C4DOS.Gv->port->CalculateRawDataRev(this,data,dest,r,calculation,parm1); }

		Bool						GetTag(BaseList2D *&t, GvRun *r, Int32 *index = nullptr) { return C4DOS.Gv->port->GetTag(this,t,r,index); }
		Bool						SetTag(const BaseList2D* const t, GvRun *r, Int32 index = 0) { return C4DOS.Gv->port->SetTag(this,t,r,index); }
		Bool						GetMaterial(BaseList2D *&m, GvRun *r, Int32 *index = nullptr) { return C4DOS.Gv->port->GetMaterial(this,m,r,index); }
		Bool						SetMaterial(const BaseList2D* const m, GvRun *r, Int32 index = 0) { return C4DOS.Gv->port->SetMaterial(this,m,r,index); }
		Bool						GetInstance(BaseList2D *&i, GvRun *r, Int32 *index = nullptr) { return C4DOS.Gv->port->GetInstance(this,i,r,index); }
		Bool						SetInstance(const BaseList2D* const i, GvRun *r, Int32 index = 0) { return C4DOS.Gv->port->SetInstance(this,i,r,index); }
		Bool						GetObjectWithIndex(BaseList2D *&o, GvRun *r, Int32 *index = nullptr) { return C4DOS.Gv->port->GetObjectWithIndex(this,o,r,index); }
		Bool						SetObjectWithIndex(const BaseList2D* const o, GvRun *r, Int32 index = 0) { return C4DOS.Gv->port->SetObjectWithIndex(this,o,r,index); }

		GvDestination*	GetOutgoing(Int32 index) { return C4DOS.Gv->port->GetOutgoing(this,index); }
};

////////////////////////////////
// GvNodeMaster class

class GvNodeMaster : public BaseList2D
{
	private:
		GvNodeMaster();
		~GvNodeMaster();

	public:
		GvNode*					AllocNode(Int32 id) { return C4DOS.Gv->master->AllocNode(this,id); }
		void						FreeNode(GvNode *&node) { C4DOS.Gv->master->FreeNode(this,node); }
		GvNode*					CreateNode(GvNode *parent, Int32 id, GvNode *insert = nullptr, Int32 x = -1, Int32 y = -1) { return C4DOS.Gv->master->CreateNode(this,parent,id,insert,x,y); }

		GvNode*					GetRoot(void) { return C4DOS.Gv->master->GetRoot(this); }
		BaseList2D*			GetOwner(void) { return C4DOS.Gv->master->GetOwner(this); }

		Bool						IsConnectionValid(GvNode *source_node, GvPort *source_port, GvNode *dest_node, GvPort *dest_port, GvNode *&source_node_out, GvPort *&source_port_out, GvNode *&dest_node_out, GvPort *&dest_port_out) { return C4DOS.Gv->master->IsConnectionValid(this,source_node,source_port,dest_node,dest_port,source_node_out,source_port_out,dest_node_out,dest_port_out); }

		Bool						InsertFirst(GvNode* parent, GvNode* node) { return C4DOS.Gv->master->InsertFirst(this,parent,node); }
		Bool						InsertLast(GvNode* parent, GvNode* node) { return C4DOS.Gv->master->InsertLast(this,parent,node); }
		Bool						SetHierarchy(GvNode *insert, GvNode *node, GvInsertMode mode = GV_INSERT_AFTER) { return C4DOS.Gv->master->SetHierarchy(this,insert,node,mode); }

		GvCalcError			QueryCalculation(GvQuery *query,BaseThread *thread) { return C4DOS.Gv->master->QueryCalculation(this,query,thread); }
		GvCalcError			InitCalculation(GvInit *init, BaseThread *thread) { return C4DOS.Gv->master->InitCalculation(this,init,thread); }
		GvCalcError			Calculate(Int32 cpu_id) { return C4DOS.Gv->master->Calculate(this,cpu_id); }
		GvCalcError			Recalculate(GvNodeMaster *master, GvNode *node, Int32 cpu_id)  { return C4DOS.Gv->master->Recalculate(this,node,cpu_id); }
		void						FreeCalculation(void) { C4DOS.Gv->master->FreeCalculation(this); }
		GvCalcError			Execute(BaseThread *thread) { return C4DOS.Gv->master->Execute(this,thread); }
		Int32						GetBranchInfo(BranchInfo *info, Int32 max, GETBRANCHINFO flags) { return C4DOS.Gv->master->GetBranchInfo(this,info,max); }

		GvRun*					GetRun(void)  { return C4DOS.Gv->master->GetRun(this); }
		GvCalc*					GetCalc(void)  { return C4DOS.Gv->master->GetCalc(this); }
		GvQuery*				AllocQuery(void) { return C4DOS.Gv->master->AllocQuery(this); }
		void						FreeQuery(GvQuery *&query) { C4DOS.Gv->master->FreeQuery(this,query); }
		GvInit*					AllocInit(void) { return C4DOS.Gv->master->AllocInit(this); }
		void						FreeInit(GvInit *&init) { C4DOS.Gv->master->FreeInit(this,init); }

		GvUserDataID		RegisterUserData(void *data = nullptr) { return C4DOS.Gv->master->RegisterUserData(this,data); }
		void						SetUserData(GvUserDataID id, void *data) { C4DOS.Gv->master->SetUserData(this,id,data); }
		void*						GetUserData(GvUserDataID id) { return C4DOS.Gv->master->GetUserData(this,id); }

		GvCalcTable*		AllocCalculationTable(Int32 cpu_count, Bool sort = true, Int32 nr_of_preallocated_entries = 16, Bool iteration = false) { return C4DOS.Gv->master->AllocCalculationTable(this,cpu_count,sort,nr_of_preallocated_entries,iteration); }
		void						FreeCalculationTable(GvCalcTable *&table) { C4DOS.Gv->master->FreeCalculationTable(this,table); }
		Bool						AddToDrawList(GvNode *bn, void **data = nullptr, Int32 data_size = 0) { return C4DOS.Gv->master->AddToDrawList(this,bn,data,data_size); }

		GvCopyBuffer*		GetCopyBuffer(GvNode *first = nullptr, Bool copy_selected = true)  { return C4DOS.Gv->master->GetCopyBuffer(this,first,copy_selected); }
		void						FreeCopyBuffer(GvCopyBuffer *&buffer) { C4DOS.Gv->master->FreeCopyBuffer(this,buffer); }
		Bool						PasteFromBuffer(GvCopyBuffer &buffer, GvInsertMode mode = GV_INSERT_UNDER, GvNode *dest = nullptr, Int32 x = GV_INVALID_POS_VALUE, Int32 y = GV_INVALID_POS_VALUE, Bool center = false, void *info = nullptr) { return C4DOS.Gv->master->PasteFromBuffer(this,buffer,mode,dest,x,y,center,info); }
		Bool						IsEnabled(void) { return C4DOS.Gv->master->IsEnabled(this); }

		void						SetPrefs(const BaseContainer &bc) { C4DOS.Gv->master->MasterSetPrefs(this,bc); }
		void						GetPrefs(BaseContainer &bc) { C4DOS.Gv->master->MasterGetPrefs(this,bc); }
		Bool						AddUndo(void) { return C4DOS.Gv->master->AddUndo(this); }

		GvCalcError			Execute2(BaseThread *thread, GvCalcFlags flags) { return C4DOS.Gv->master->Execute2(this,thread,flags); }
};

////////////////////////////////
// GvNodeGUI class

class GvNodeGUI
{
	private:
		GvNodeGUI();
		~GvNodeGUI();

	public:
		Bool						Attach(GeDialog *dialog, GvNodeMaster *master) { return C4DOS.Gv->gui->GuiAttach(this,dialog,master); }
		void						Detach(void) { C4DOS.Gv->gui->GuiDetach(this); }
		Bool						InitShapes(void) { return C4DOS.Gv->gui->GuiInitShapes(this); }

	public:
		void						Draw(void) { C4DOS.Gv->gui->GuiDraw(this); }
		void						MouseDown(Int32 x, Int32 y, Int32 chn, Int32 qa, const BaseContainer &msg) { C4DOS.Gv->gui->GuiMouseDown(this,x,y,chn,qa,msg); }
		Int32						Message(const BaseContainer &msg, BaseContainer &result) { return C4DOS.Gv->gui->GuiMessage(this,msg,result); }
		Int32						Command(Int32 id) { return C4DOS.Gv->gui->GuiCommand(this,id); }
		void						Redraw(void) { C4DOS.Gv->gui->GuiRedraw(this); }
		GeUserArea*			GetUserArea(void) { return C4DOS.Gv->gui->GuiGetUserArea(this); }
		GvNodeMaster*		GetMaster(void) { return C4DOS.Gv->gui->GuiGetMaster(this); }
		GeDialog*				GetDialog(void) { return C4DOS.Gv->gui->GuiGetDialog(this); }

	public:
		void						SelectAllNodes(GvNode *node, Bool select_state, Bool add_to_selection = false) { C4DOS.Gv->gui->GuiSelectAllNodes(this,node,select_state,add_to_selection); }
		void						RemoveAllSelectedNodes(GvNode *node) { C4DOS.Gv->gui->GuiRemoveAllSelectedNodes(this,node); }
		void						SelectNode(GvNode *node, Bool select_state, Bool add_to_selection = false, Bool send_message = true) { C4DOS.Gv->gui->GuiSelectNode(this,node,select_state,add_to_selection,send_message); }
		void						DisableSelected(GvNode *node, Bool disable_state) { C4DOS.Gv->gui->GuiDisableSelected(this,node,disable_state); }
		void						SetFocus(GvNode *node, Bool activate) { C4DOS.Gv->gui->GuiSetFocus(this,node,activate); }

	public:
		GvNode*					GetNodeGlobal(Int32 x, Int32 y) { return C4DOS.Gv->gui->GuiGetNodeGlobal(this,x,y); }
		GvNode*					GetNodeLocal(GvNode *node, Int32 x, Int32 y) { return C4DOS.Gv->gui->GuiGetNodeLocal(this,node,x,y); }
		Bool						IsInNodeBody(GvNode *node, Int32 x, Int32 y) { return C4DOS.Gv->gui->GuiIsInNodeBody(this,node,x,y); }
		Bool						IsInNodeHead(GvNode *node, Int32 x, Int32 y) { return C4DOS.Gv->gui->GuiIsInNodeHead(this,node,x,y); }
		Bool						NodeContextMenu(GvNode *node) { return C4DOS.Gv->gui->GuiNodeContextMenu(this,node); }

	public:
		void						SetPrefs(const BaseContainer &bc)  { C4DOS.Gv->gui->GuiSetPrefs(this,bc); }
		void						GetPrefs(BaseContainer &bc) { C4DOS.Gv->gui->GuiGetPrefs(this,bc); }

		void						SetNodePosGlobal(GvNode *node, Int32 x, Int32 y, Bool center = false)  { C4DOS.Gv->gui->GuiSetNodePosGlobal(this,node,x,y,center); }
		void						SetNodePos(GvNode *node, Int32 x, Int32 y)  { C4DOS.Gv->gui->GuiSetNodePos(this,node,x,y); }
		void						SetNodeSize(GvNode *node, Int32 width, Int32 height) { C4DOS.Gv->gui->GuiSetNodeSize(this,node,width,height); }
		void						OptimizeNode(GvNode *node) { C4DOS.Gv->gui->GuiOptimizeNode(this,node); }
		void						ShowAllNodes(GvNode *node) { C4DOS.Gv->gui->GuiShowAllNodes(this,node); }
		void						CenterNodes(GvNode *node) { C4DOS.Gv->gui->GuiCenterNodes(this,node); }
		void						AlignNodesToUpperLeft(GvNode *node) { C4DOS.Gv->gui->GuiAlignNodesToUpperLeft(this,node); }
		void						GetZoom(GvNode *node, Float &zoom) { C4DOS.Gv->gui->GuiGetZoom(this,node,zoom); }
};

////////////////////////////////
// GvWorld class

class GvWorld
{
	private:
		GvWorld();
		~GvWorld();

	public:
		GvNodeMaster*		AllocNodeMaster(BaseList2D *object, Bool add_to_list = true, Bool send_messages = true) { return C4DOS.Gv->world->AllocNodeMaster(this,object,add_to_list,send_messages); }
		void						FreeNodeMaster(GvNodeMaster *&master) { C4DOS.Gv->world->FreeNodeMaster(this,master); }

		GvNodeGUI*			AllocNodeGUI(GvShape *shape, GvShape *group, Int32 user_area_id) { return C4DOS.Gv->world->AllocNodeGUI(this,shape,group,user_area_id); }
		void						FreeNodeGUI(GvNodeGUI *&gui) { C4DOS.Gv->world->FreeNodeGUI(this,gui); }

		GvShape*				AllocShape(void) { return C4DOS.Gv->world->AllocShape(this); }
		GvShape*				AllocGroupShape(void) { return C4DOS.Gv->world->AllocGroupShape(this); }
		void						FreeShape(GvShape *&shape) { C4DOS.Gv->world->FreeShape(this,shape); }

		Bool						RegisterHook(const GvHook &hook, void *user) { return C4DOS.Gv->world->RegisterHook(this,hook,user); }
		Bool						AttachHook(Int32 hook_id, GvHookCallback callback) { return C4DOS.Gv->world->AttachHook(this,hook_id,callback); }
		void						DetachHook(Int32 hook_id) { C4DOS.Gv->world->DetachHook(this,hook_id); }
		BaseList2D*			GetHookInstance(BaseDocument *doc, Int32 hook_id) { return C4DOS.Gv->world->GetHookInstance(this,doc,hook_id); }

		Bool						SendHookMessage(BaseDocument *doc, GvNodeMaster *master, GvMessHook &data, Int32 owner_id) { return C4DOS.Gv->world->SendHookMessage(this,doc,master,data,owner_id); }
		Bool						SendOperatorMessage(BaseDocument *doc, Int32 message_id, void *data) { return C4DOS.Gv->world->SendOperatorMessage(this,doc,message_id,data); }

		Bool						OpenDialog(Int32 id, GvNodeMaster *master) { return C4DOS.Gv->world->OpenDialog(this,id,master); }
		void						CloseDialog(Int32 id) { C4DOS.Gv->world->CloseDialog(this,id); }

		void						RedrawAll(void) { C4DOS.Gv->world->RedrawAll(this); }
		void						RedrawMaster(GvNodeMaster *master) { C4DOS.Gv->world->RedrawMaster(this,master); }
		Bool						AttachNode(GvNodeMaster *master, GvNode *node, Int32 x, Int32 y)  { return C4DOS.Gv->world->AttachNode(this,master,node,x,y); }

	public:
		const String		GetString(const String &title, const String &default_value) { return C4DOS.Gv->world->GetString(this,title,default_value); }
		Float						GetFloat(const String &title, Float default_value) { return C4DOS.Gv->world->GetFloat(this,title,default_value); }
		Int32						GetInteger(const String &title, Int32 default_value) { return C4DOS.Gv->world->GetInteger(this,title,default_value); }

		Int32						GetDataTypesMenu(BaseContainer &bc, BaseContainer &index, Int32 first_menu_id, Int32 first_sub_id, Bool show_undefined_type = false, GvValueFlags flags = GV_CALC_NOP) { return C4DOS.Gv->world->GetDataTypesMenu(this,bc,index,first_menu_id,first_sub_id,show_undefined_type,flags); }
		Int32						GetDataTypes(BaseContainer &bc, GvDataOptions options = GV_DATA_OPTIONS_NONE, GvValueFlags flags = GV_CALC_NOP) { return C4DOS.Gv->world->GetDataTypes(this,bc,options,flags); }
		Bool						GetDataTypesTable(GvDataInfo *&info, Int32 &count) { return C4DOS.Gv->world->GetDataTypesTable(this,info,count); }
		Int32						GetDataTypeIndex(GvDataID id) { return C4DOS.Gv->world->GetDataTypeIndex(this,id); }
		GvDataInfo*			GetDataTypeInfo(GvDataID id) { return C4DOS.Gv->world->GetDataTypeInfo(this,id); }

		GvNodeGUI*			GetMasterGUI(GvNodeMaster *master, UInt32 nr = 0) { return C4DOS.Gv->world->GetMasterGUI(this,master,nr); }
		UInt32						GetUniqueID(void) { return C4DOS.Gv->world->GetUniqueID(this); }

		BaseBitmap*			GetDefaultOperatorIcon(GvOperatorType type) { return C4DOS.Gv->world->GetDefaultOperatorIcon(this,type); }

	public:
		void						SetPrefs(const BaseContainer &bc)  { C4DOS.Gv->world->WorldSetPrefs(this,bc); }
		void						GetPrefs(BaseContainer &bc)  { C4DOS.Gv->world->WorldGetPrefs(this,bc); }

	public:
		Int32						GetDataTypeNames(BaseContainer &bc, GvDataID *ids) { return C4DOS.Gv->world->GetDataTypeNames(this,bc,ids); }
		GvNodeMaster*		GetMaster(Int32 id) { return C4DOS.Gv->world->GetMaster(this,id); }

};

////////////////////////////////
// global functions

GvWorld*						GvGetWorld(void);
const String				GvGetEmptyString(void);
const String				GvGetErrorString(const String &command, Int32 err);
void								GvGetPortList(GvPortsDescInfo *info, GvPortIO port, GvPortList &portlist);
Bool								GvGetPortDescription(GvPortsDescInfo *info, GvPortIO port, Int32 id, GvPortDescription *pd);
Bool								GvGetAllDataTypes(GvPortDescInfo *info, UInt32 default_flag, Int32 first_id);
void								GvFreePortDescInfo(GvPortDescInfo *info);
Bool								GvRegisterOpClassType(GV_OPCLASS_HANDLER *data, Int32 struct_size);
Bool								GvRegisterOpGroupType(GV_OPGROUP_HANDLER *data, Int32 struct_size);
Bool								GvRegisterValueType(GV_VALUE_HANDLER *data, Int32 struct_size);
Bool								GvRegisterDataType(GV_DATA_HANDLER *data, Int32 struct_size, const char *symbol = nullptr);
Bool								GvRegisterValGroupType(GV_VALGROUP_HANDLER *data, Int32 struct_size);
GV_OPCLASS_HANDLER* GvFindRegisteredOpClass(GvOpClassID id);
GV_OPGROUP_HANDLER* GvFindRegisteredOpGroup(GvOpGroupID id);

////////////////////////////////

const String				GvGetOperatorDetailedText(GvOperatorData *op, GvNode *bn);
const String				GvGetOperatorTitle(GvNode *bn, Int32 string_id);

////////////////////////////////

void								GvFreeValuesTable(GvNode *bn, GvValue **&ports, Int32 &nr_of_ports);
void								GvFreeValuesTable(GvNode *bn, GvValuesInfo &info);

Bool								GvBuildInValuesTable(GvNode *bn, GvValue **&ports, Int32 &nr_of_ports, GvCalc *c, GvRun *r, Int32 *ids);
Bool								GvBuildOutPortsTable(GvNode *bn, GvPort **&ports, Int32 &nr_of_ports);
Bool								GvBuildValuesTable(GvNode *bn, GvValue **&in_ports, Int32 &nr_of_in_ports, GvPort **&out_ports, Int32 &nr_of_out_ports);

Bool								GvCalculateInValuesTable(GvNode *bn, GvRun *run, GvCalc *calc, GvValuesInfo &info, Int32 singleport = GV_MULTIPLE_PORTS, BaseTime *time = nullptr);

////////////////////////////////

inline Bool GvBuildInValuesTable(GvNode *bn, GvValuesInfo &info, GvCalc *c, GvRun *r, GvIdTablePtr ids)
{
	return (GvBuildInValuesTable(bn,info.in_values,info.nr_of_in_values,c,r,ids));
}

inline Bool GvBuildOutValuesTable(GvNode *bn, GvValuesInfo &info)
{
	return (GvBuildOutPortsTable(bn,info.out_ports,info.nr_of_out_ports));
}

inline Bool GvBuildValuesTable(GvNode *bn, GvValuesInfo &info, GvCalc *c, GvRun *r, GvIdTablePtr input_ids)
{
	if (!GvBuildInValuesTable(bn,info,c,r,input_ids)) return false;
	if (!GvBuildOutValuesTable(bn,info)) return false;
	return true;
}

////////////////////////////////

inline GvDataInfo* GvGetDataInfo(GvNode *bn, Int32 id)
{
	BaseContainer *bc = bn->GetOpContainerInstance(); if (!bc) return nullptr;
	return GvGetWorld()->GetDataTypeInfo(bc->GetInt32(id));
}

inline Bool GvCheckDataInfo(GvNode *bn, Int32 id)
{
	BaseContainer *bc = bn->GetOpContainerInstance(); if (!bc) return false;
	return GvGetWorld()->GetDataTypeInfo(bc->GetInt32(id)) != nullptr;
}

inline Bool GvAllocDynamicData(GvNode *bn, GvDynamicData &data, GvCalc *c, Int32 id)
{
	data.cpu_count = c->cpu_count;
	data.info = GvGetDataInfo(bn,id);
	return (data.info && data.cpu_count) ? data.info->value_handler->Alloc(data.info->value_handler->userdata,data.data,data.cpu_count) : false;
}

inline void GvFreeDynamicData(GvDynamicData &data)
{
	if (data.data)
	{
		data.info->value_handler->Free(data.info->value_handler->userdata,data.data,data.cpu_count);
		data.data = nullptr;
	}
	data.info = nullptr;
	data.cpu_count = 0;
}

inline void GvClearDynamicData(GvDynamicData &data, GvRun *r)
{
	data.info->value_handler->Calculate(data.info->value_handler->userdata,data.info->value_handler->value_id,nullptr,nullptr,data.data,r->GetCpuID(),GV_CALC_CLR,0.0);
}

inline void GvClearDynamicData(GvDynamicData &data)
{
	Int32 i;
	for (i = 0; i < data.cpu_count; ++i)
	{
		data.info->value_handler->Calculate(data.info->value_handler->userdata,data.info->value_handler->value_id,nullptr,nullptr,data.data,i,GV_CALC_CLR,0.0);
	}
}

inline Bool GvAllocDynamicDataClear(GvNode *bn, GvDynamicData &data, GvCalc *c, Int32 id)
{
	if (! GvAllocDynamicData(bn,data,c,id)) return false;
	GvClearDynamicData(data);
	return true;
}

////////////////////////////////

inline Bool GvAllocDynamicData(GvNode *bn, GvDynamicData &data, GvDataInfo *info)
{
	data.cpu_count = 1;
	data.info = info;
	return (data.info) ? data.info->value_handler->Alloc(data.info->value_handler->userdata,data.data,data.cpu_count) : false;
}

////////////////////////////////

inline Bool GvSetDataInContainer(const void* const data, GvValueID value_id, BaseContainer &bc, Int32 container_id, Int32 cpu_id = 0)
{
	CUSTOMDATATYPEPLUGIN* pl = FindCustomDataTypePlugin(value_id); if (!pl) return false;
	GeData ge_data;	CallCustomDataType(pl,ConvertGvToGeData)(data,cpu_id,ge_data);
	bc.SetData(container_id,ge_data);
	return true;
}

////////////////////////////////
// convinience typecast

inline GvNode* GetNode(GeListNode *bn) { DebugAssert(bn->GetType() == ID_GV_NODEDATA || bn->GetType() == ID_GV_GROUPDATA); return (GvNode*)bn; }

////////////////////////////////

enum DESCIDINFOFLAGS
{
	DESCIDINFOFLAGS_NONE								= 0,
	DESCIDINFOFLAGS_INPORT							= 1<<0,
	DESCIDINFOFLAGS_INPORT_CONNECTED		= 1<<1,
	DESCIDINFOFLAGS_OUTPORT							= 1<<2,
	DESCIDINFOFLAGS_OUTPORT_CONNECTED		= 1<<3
} ENUM_END_FLAGS(DESCIDINFOFLAGS);

/*
class OperatorArray : public c4d_misc::SortedArray<OperatorArray, c4d_misc::BaseArray<C4DAtomGoal*, 4> >
{
public:
	OperatorArray()
	{
	}

	OperatorArray(OperatorArray&& src) : maxon::SortedArray<OperatorArray,maxon::BaseArray<C4DAtomGoal*,4> >(std::move(src))
	{
	}
	MOVE_ASSIGNMENT_OPERATOR(OperatorArray)

	static Bool LessThan(C4DAtomGoal* a, C4DAtomGoal* b)
	{
		return a < b;
	}

	static Bool IsEqual(C4DAtomGoal* a, C4DAtomGoal* b)
	{
		return a == b;
	}

	static Bool InitInsertData(C4DAtomGoal* &initme, C4DAtomGoal* const &src)
	{
		initme = src;
		return true;
	}
};

struct DescIdInfo
{
	DescID						_id;
	OperatorArray			_operators;
	DESCIDINFOFLAGS		_info;
	String						_name;

	DescIdInfo() : _info(DESCIDINFOFLAGS_NONE)
	{

	}
	DescIdInfo(DescIdInfo&& src) : _id(std::move(src._id)), _operators(std::move(src._operators)), _info(src._info), _name(std::move(src._name))
	{
	}

	MOVE_ASSIGNMENT_OPERATOR(DescIdInfo)

	static Bool LessThan(const DescID &a, const DescIdInfo &b)
	{
		Int32 idx=0;
		for (; idx<a.GetDepth() && idx<b._id.GetDepth() && a[idx].id==b._id[idx].id; idx++) {}
		return a[idx].id < b._id[idx].id;
	}

	static Bool LessThan(const DescIdInfo &a, const DescIdInfo &b)
	{
		return LessThan(a._id,b);
	}

	static Bool IsEqual(const DescIdInfo &a, const DescIdInfo &b)
	{
		return a._id == b._id;
	}

	static Bool IsEqual(const DescID &a, const DescIdInfo &b)
	{
		return a == b._id;
	}

	static Bool InitInsertData(DescIdInfo &initme, const DescID &src)
	{
		initme._id = src;
		return true;
	}
};
*/

class DescIdInfoArray : public maxon::SortedArray<DescIdInfo,maxon::BaseArray<DescIdInfo> >
{
public:
	DescIdInfo* Find(const DescID& key, DescIdInfo& tmp) const
	{
		Int32 cnt = 0;
		for (Int32 i=0; i<GetCount(); i++)
		{
			if ((*this)[i]._id.IsPartOf(key,nullptr) || key.IsPartOf((*this)[i]._id,nullptr))
			{
				tmp._id = key;
				if (tmp._name.Content()) tmp._name += ", ";
				tmp._name += (*this)[i]._name;
				for (Int32 j=0; j<(*this)[i]._operators.GetCount(); j++)
				{
					if (!tmp._operators.Find((*this)[i]._operators[j]))
						tmp._operators.Append((*this)[i]._operators[j]);
				}
				tmp._info |= (*this)[i]._info;
				cnt++;
			}
		}
		return cnt ? &tmp : nullptr;
	}
};

#endif //__C4D_GRAPHVIEW_H

////////////////////////////////
