//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.10
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace C4d {

public class BaseDocument : BaseList2D {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal BaseDocument(global::System.IntPtr cPtr, bool cMemoryOwn) : base(C4dApiPINVOKE.BaseDocument_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(BaseDocument obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  public override void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          throw new global::System.MethodAccessException("C++ destructor does not have public access");
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
      base.Dispose();
    }
  }

  public static BaseDocument Alloc() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_Alloc();
    BaseDocument ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseDocument(cPtr, false);
    return ret;
  }

  public static void Free(SWIGTYPE_p_p_BaseDocument bl) {
    C4dApiPINVOKE.BaseDocument_Free(SWIGTYPE_p_p_BaseDocument.getCPtr(bl));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void Flush() {
    C4dApiPINVOKE.BaseDocument_Flush(swigCPtr);
  }

  public new BaseDocument GetNext() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetNext(swigCPtr);
    BaseDocument ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseDocument(cPtr, false);
    return ret;
  }

  public new BaseDocument GetPred() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetPred(swigCPtr);
    BaseDocument ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseDocument(cPtr, false);
    return ret;
  }

  public BaseContainer GetData(DOCUMENTSETTINGS type) {
    BaseContainer ret = new BaseContainer(C4dApiPINVOKE.BaseDocument_GetData(swigCPtr, (int)type), true);
    return ret;
  }

  public void SetData(DOCUMENTSETTINGS type, BaseContainer bc) {
    C4dApiPINVOKE.BaseDocument_SetData(swigCPtr, (int)type, BaseContainer.getCPtr(bc));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public BaseContainer GetSettingsInstance(int type) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetSettingsInstance(swigCPtr, type);
    BaseContainer ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseContainer(cPtr, false);
    return ret;
  }

  public SWIGTYPE_p_NetRenderDocumentContext GetNetRenderDocumentContext() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetNetRenderDocumentContext(swigCPtr);
    SWIGTYPE_p_NetRenderDocumentContext ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_NetRenderDocumentContext(cPtr, false);
    return ret;
  }

  public void SetNetRenderDocumentContext(SWIGTYPE_p_NetRenderDocumentContext context) {
    C4dApiPINVOKE.BaseDocument_SetNetRenderDocumentContext(swigCPtr, SWIGTYPE_p_NetRenderDocumentContext.getCPtr(context));
  }

  public BaseObject GetFirstObject() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetFirstObject(swigCPtr);
    BaseObject ret = (BaseObject) C4dApiPINVOKE.InstantiateConcreteObject(cPtr, false);
    return ret;
}

  public BaseMaterial GetFirstMaterial() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetFirstMaterial(swigCPtr);
    BaseMaterial ret = (BaseMaterial) C4dApiPINVOKE.InstantiateConcreteMaterial(cPtr, false);
    return ret;
}

  public RenderData GetFirstRenderData() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetFirstRenderData(swigCPtr);
    RenderData ret = (cPtr == global::System.IntPtr.Zero) ? null : new RenderData(cPtr, false);
    return ret;
  }

  public void InsertObject(BaseObject op, BaseObject parent, BaseObject pred, bool checknames) {
    C4dApiPINVOKE.BaseDocument_InsertObject__SWIG_0(swigCPtr, BaseObject.getCPtr(op), BaseObject.getCPtr(parent), BaseObject.getCPtr(pred), checknames);
  }

  public void InsertObject(BaseObject op, BaseObject parent, BaseObject pred) {
    C4dApiPINVOKE.BaseDocument_InsertObject__SWIG_1(swigCPtr, BaseObject.getCPtr(op), BaseObject.getCPtr(parent), BaseObject.getCPtr(pred));
  }

  public void InsertMaterial(BaseMaterial mat, BaseMaterial pred, bool checknames) {
    C4dApiPINVOKE.BaseDocument_InsertMaterial__SWIG_0(swigCPtr, BaseMaterial.getCPtr(mat), BaseMaterial.getCPtr(pred), checknames);
  }

  public void InsertMaterial(BaseMaterial mat, BaseMaterial pred) {
    C4dApiPINVOKE.BaseDocument_InsertMaterial__SWIG_1(swigCPtr, BaseMaterial.getCPtr(mat), BaseMaterial.getCPtr(pred));
  }

  public void InsertMaterial(BaseMaterial mat) {
    C4dApiPINVOKE.BaseDocument_InsertMaterial__SWIG_2(swigCPtr, BaseMaterial.getCPtr(mat));
  }

  public void InsertRenderData(RenderData rd, RenderData parent, RenderData pred) {
    C4dApiPINVOKE.BaseDocument_InsertRenderData(swigCPtr, RenderData.getCPtr(rd), RenderData.getCPtr(parent), RenderData.getCPtr(pred));
  }

  public void InsertRenderDataLast(RenderData rd) {
    C4dApiPINVOKE.BaseDocument_InsertRenderDataLast(swigCPtr, RenderData.getCPtr(rd));
  }

  public BaseObject GetActiveObject() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetActiveObject(swigCPtr);
    BaseObject ret = (BaseObject) C4dApiPINVOKE.InstantiateConcreteObject(cPtr, false);
    return ret;
}

  public BaseMaterial GetActiveMaterial() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetActiveMaterial(swigCPtr);
    BaseMaterial ret = (BaseMaterial) C4dApiPINVOKE.InstantiateConcreteMaterial(cPtr, false);
    return ret;
}

  public BaseTag GetActiveTag() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetActiveTag(swigCPtr);
    BaseTag ret = (BaseTag) C4dApiPINVOKE.InstantiateConcreteTag(cPtr, false);
    return ret;
}

  public RenderData GetActiveRenderData() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetActiveRenderData(swigCPtr);
    RenderData ret = (cPtr == global::System.IntPtr.Zero) ? null : new RenderData(cPtr, false);
    return ret;
  }

  public BaseObject GetRealActiveObject(AtomArray help, SWIGTYPE_p_Bool multi) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetRealActiveObject(swigCPtr, AtomArray.getCPtr(help), SWIGTYPE_p_Bool.getCPtr(multi));
    BaseObject ret = (BaseObject) C4dApiPINVOKE.InstantiateConcreteObject(cPtr, false);
    return ret;
}

  public void GetActiveObjects(AtomArray selection, GETACTIVEOBJECTFLAGS flags) {
    C4dApiPINVOKE.BaseDocument_GetActiveObjects(swigCPtr, AtomArray.getCPtr(selection), (int)flags);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void GetActivePolygonObjects(AtomArray selection, bool children) {
    C4dApiPINVOKE.BaseDocument_GetActivePolygonObjects(swigCPtr, AtomArray.getCPtr(selection), children);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void GetActiveObjectsFilter(AtomArray selection, bool children, int type, int instanceof) {
    C4dApiPINVOKE.BaseDocument_GetActiveObjectsFilter(swigCPtr, AtomArray.getCPtr(selection), children, type, instanceof);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void GetActiveMaterials(AtomArray selection) {
    C4dApiPINVOKE.BaseDocument_GetActiveMaterials(swigCPtr, AtomArray.getCPtr(selection));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void GetActiveTags(AtomArray selection) {
    C4dApiPINVOKE.BaseDocument_GetActiveTags(swigCPtr, AtomArray.getCPtr(selection));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void GetSelection(AtomArray selection) {
    C4dApiPINVOKE.BaseDocument_GetSelection(swigCPtr, AtomArray.getCPtr(selection));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetActiveObject(BaseObject op, int mode) {
    C4dApiPINVOKE.BaseDocument_SetActiveObject__SWIG_0(swigCPtr, BaseObject.getCPtr(op), mode);
  }

  public void SetActiveObject(BaseObject op) {
    C4dApiPINVOKE.BaseDocument_SetActiveObject__SWIG_1(swigCPtr, BaseObject.getCPtr(op));
  }

  public void SetActiveMaterial(BaseMaterial mat, int mode) {
    C4dApiPINVOKE.BaseDocument_SetActiveMaterial__SWIG_0(swigCPtr, BaseMaterial.getCPtr(mat), mode);
  }

  public void SetActiveMaterial(BaseMaterial mat) {
    C4dApiPINVOKE.BaseDocument_SetActiveMaterial__SWIG_1(swigCPtr, BaseMaterial.getCPtr(mat));
  }

  public void SetActiveTag(BaseTag tag, int mode) {
    C4dApiPINVOKE.BaseDocument_SetActiveTag__SWIG_0(swigCPtr, BaseTag.getCPtr(tag), mode);
  }

  public void SetActiveTag(BaseTag tag) {
    C4dApiPINVOKE.BaseDocument_SetActiveTag__SWIG_1(swigCPtr, BaseTag.getCPtr(tag));
  }

  public void SetActiveRenderData(RenderData rd) {
    C4dApiPINVOKE.BaseDocument_SetActiveRenderData(swigCPtr, RenderData.getCPtr(rd));
  }

  public void SetSelection(BaseList2D bl, int mode) {
    C4dApiPINVOKE.BaseDocument_SetSelection__SWIG_0(swigCPtr, BaseList2D.getCPtr(bl), mode);
  }

  public void SetSelection(BaseList2D bl) {
    C4dApiPINVOKE.BaseDocument_SetSelection__SWIG_1(swigCPtr, BaseList2D.getCPtr(bl));
  }

  public BaseObject SearchObject(string /* constString&_cstype */ str) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_SearchObject(swigCPtr, str);
    BaseObject ret = (BaseObject) C4dApiPINVOKE.InstantiateConcreteObject(cPtr, false);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
}

  public BaseObject SearchObjectInc(string /* constString&_cstype */ str) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_SearchObjectInc(swigCPtr, str);
    BaseObject ret = (BaseObject) C4dApiPINVOKE.InstantiateConcreteObject(cPtr, false);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
}

  public BaseMaterial SearchMaterial(string /* constString&_cstype */ str) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_SearchMaterial(swigCPtr, str);
    BaseMaterial ret = (BaseMaterial) C4dApiPINVOKE.InstantiateConcreteMaterial(cPtr, false);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
}

  public BaseMaterial SearchMaterialInc(string /* constString&_cstype */ str) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_SearchMaterialInc(swigCPtr, str);
    BaseMaterial ret = (BaseMaterial) C4dApiPINVOKE.InstantiateConcreteMaterial(cPtr, false);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
}

  public bool GetChanged() {
    bool ret = C4dApiPINVOKE.BaseDocument_GetChanged(swigCPtr);
    return ret;
  }

  public void SetChanged() {
    C4dApiPINVOKE.BaseDocument_SetChanged(swigCPtr);
  }

  public Filename GetDocumentName() {
    Filename ret = new Filename(C4dApiPINVOKE.BaseDocument_GetDocumentName(swigCPtr), true);
    return ret;
  }

  public Filename GetDocumentPath() {
    Filename ret = new Filename(C4dApiPINVOKE.BaseDocument_GetDocumentPath(swigCPtr), true);
    return ret;
  }

  public void SetDocumentName(Filename fn) {
    C4dApiPINVOKE.BaseDocument_SetDocumentName(swigCPtr, Filename.getCPtr(fn));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetDocumentPath(Filename path) {
    C4dApiPINVOKE.BaseDocument_SetDocumentPath(swigCPtr, Filename.getCPtr(path));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public double GetLOD() {
    double ret = C4dApiPINVOKE.BaseDocument_GetLOD(swigCPtr);
    return ret;
  }

  public void SetLOD(double lod) {
    C4dApiPINVOKE.BaseDocument_SetLOD(swigCPtr, lod);
  }

  public bool GetRenderLod() {
    bool ret = C4dApiPINVOKE.BaseDocument_GetRenderLod(swigCPtr);
    return ret;
  }

  public void SetRenderLod(bool lod) {
    C4dApiPINVOKE.BaseDocument_SetRenderLod(swigCPtr, lod);
  }

  public BaseTime GetTime() {
    BaseTime ret = new BaseTime(C4dApiPINVOKE.BaseDocument_GetTime(swigCPtr), true);
    return ret;
  }

  public void SetTime(BaseTime t) {
    C4dApiPINVOKE.BaseDocument_SetTime(swigCPtr, BaseTime.getCPtr(t));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public int GetFps() {
    int ret = C4dApiPINVOKE.BaseDocument_GetFps(swigCPtr);
    return ret;
  }

  public void SetFps(int fps) {
    C4dApiPINVOKE.BaseDocument_SetFps(swigCPtr, fps);
  }

  public BaseTime GetMinTime() {
    BaseTime ret = new BaseTime(C4dApiPINVOKE.BaseDocument_GetMinTime(swigCPtr), true);
    return ret;
  }

  public void SetMinTime(BaseTime t) {
    C4dApiPINVOKE.BaseDocument_SetMinTime(swigCPtr, BaseTime.getCPtr(t));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public BaseTime GetMaxTime() {
    BaseTime ret = new BaseTime(C4dApiPINVOKE.BaseDocument_GetMaxTime(swigCPtr), true);
    return ret;
  }

  public void SetMaxTime(BaseTime t) {
    C4dApiPINVOKE.BaseDocument_SetMaxTime(swigCPtr, BaseTime.getCPtr(t));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public BaseTime GetUsedMinTime(BaseList2D check) {
    BaseTime ret = new BaseTime(C4dApiPINVOKE.BaseDocument_GetUsedMinTime(swigCPtr, BaseList2D.getCPtr(check)), true);
    return ret;
  }

  public BaseTime GetUsedMaxTime(BaseList2D check) {
    BaseTime ret = new BaseTime(C4dApiPINVOKE.BaseDocument_GetUsedMaxTime(swigCPtr, BaseList2D.getCPtr(check)), true);
    return ret;
  }

  public BaseTime GetLoopMinTime() {
    BaseTime ret = new BaseTime(C4dApiPINVOKE.BaseDocument_GetLoopMinTime(swigCPtr), true);
    return ret;
  }

  public void SetLoopMinTime(BaseTime t) {
    C4dApiPINVOKE.BaseDocument_SetLoopMinTime(swigCPtr, BaseTime.getCPtr(t));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public BaseTime GetLoopMaxTime() {
    BaseTime ret = new BaseTime(C4dApiPINVOKE.BaseDocument_GetLoopMaxTime(swigCPtr), true);
    return ret;
  }

  public void SetLoopMaxTime(BaseTime t) {
    C4dApiPINVOKE.BaseDocument_SetLoopMaxTime(swigCPtr, BaseTime.getCPtr(t));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public int GetMode() {
    int ret = C4dApiPINVOKE.BaseDocument_GetMode(swigCPtr);
    return ret;
  }

  public void SetMode(int m) {
    C4dApiPINVOKE.BaseDocument_SetMode(swigCPtr, m);
  }

  public bool IsEditMode() {
    bool ret = C4dApiPINVOKE.BaseDocument_IsEditMode(swigCPtr);
    return ret;
  }

  public int GetAction() {
    int ret = C4dApiPINVOKE.BaseDocument_GetAction(swigCPtr);
    return ret;
  }

  public void SetAction(int a) {
    C4dApiPINVOKE.BaseDocument_SetAction(swigCPtr, a);
  }

  public bool StartUndo() {
    bool ret = C4dApiPINVOKE.BaseDocument_StartUndo(swigCPtr);
    return ret;
  }

  public bool EndUndo() {
    bool ret = C4dApiPINVOKE.BaseDocument_EndUndo(swigCPtr);
    return ret;
  }

  public bool AddUndo(UNDOTYPE type, SWIGTYPE_p_void data, bool allowFromThread) {
    bool ret = C4dApiPINVOKE.BaseDocument_AddUndo__SWIG_0(swigCPtr, (int)type, SWIGTYPE_p_void.getCPtr(data), allowFromThread);
    return ret;
  }

  public bool AddUndo(UNDOTYPE type, SWIGTYPE_p_void data) {
    bool ret = C4dApiPINVOKE.BaseDocument_AddUndo__SWIG_1(swigCPtr, (int)type, SWIGTYPE_p_void.getCPtr(data));
    return ret;
  }

  public bool AddUndo(BaseDraw bd) {
    bool ret = C4dApiPINVOKE.BaseDocument_AddUndo__SWIG_2(swigCPtr, BaseDraw.getCPtr(bd));
    return ret;
  }

  public bool DoUndo(bool multiple) {
    bool ret = C4dApiPINVOKE.BaseDocument_DoUndo__SWIG_0(swigCPtr, multiple);
    return ret;
  }

  public bool DoUndo() {
    bool ret = C4dApiPINVOKE.BaseDocument_DoUndo__SWIG_1(swigCPtr);
    return ret;
  }

  public bool DoRedo() {
    bool ret = C4dApiPINVOKE.BaseDocument_DoRedo(swigCPtr);
    return ret;
  }

  public void FlushUndoBuffer() {
    C4dApiPINVOKE.BaseDocument_FlushUndoBuffer(swigCPtr);
  }

  public BaseList2D GetUndoPtr() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetUndoPtr(swigCPtr);
    BaseList2D ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseList2D(cPtr, false);
    return ret;
  }

  public BaseList2D FindUndoPtr(BaseList2D bl, UNDOTYPE type) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_FindUndoPtr(swigCPtr, BaseList2D.getCPtr(bl), (int)type);
    BaseList2D ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseList2D(cPtr, false);
    return ret;
  }

  public void AutoKey(BaseList2D undo, BaseList2D op, bool recursive, bool pos, bool scale, bool rot, bool param, bool pla) {
    C4dApiPINVOKE.BaseDocument_AutoKey(swigCPtr, BaseList2D.getCPtr(undo), BaseList2D.getCPtr(op), recursive, pos, scale, rot, param, pla);
  }

  public bool RecordKey(BaseList2D op, BaseTime time, DescID id, BaseList2D undo, bool eval_attribmanager, bool autokeying, bool allow_linking) {
    bool ret = C4dApiPINVOKE.BaseDocument_RecordKey(swigCPtr, BaseList2D.getCPtr(op), BaseTime.getCPtr(time), DescID.getCPtr(id), BaseList2D.getCPtr(undo), eval_attribmanager, autokeying, allow_linking);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Record() {
    C4dApiPINVOKE.BaseDocument_Record(swigCPtr);
  }

  public BaseDraw GetActiveBaseDraw() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetActiveBaseDraw(swigCPtr);
    BaseDraw ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseDraw(cPtr, false);
    return ret;
  }

  public BaseDraw GetRenderBaseDraw() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetRenderBaseDraw(swigCPtr);
    BaseDraw ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseDraw(cPtr, false);
    return ret;
  }

  public BaseDraw GetBaseDraw(int num) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetBaseDraw(swigCPtr, num);
    BaseDraw ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseDraw(cPtr, false);
    return ret;
  }

  public int GetBaseDrawCount() {
    int ret = C4dApiPINVOKE.BaseDocument_GetBaseDrawCount(swigCPtr);
    return ret;
  }

  public void ForceCreateBaseDraw() {
    C4dApiPINVOKE.BaseDocument_ForceCreateBaseDraw(swigCPtr);
  }

  public int GetDrawTime() {
    int ret = C4dApiPINVOKE.BaseDocument_GetDrawTime(swigCPtr);
    return ret;
  }

  public PickSessionDataStruct GetPickSession() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetPickSession(swigCPtr);
    PickSessionDataStruct ret = (cPtr == global::System.IntPtr.Zero) ? null : new PickSessionDataStruct(cPtr, false);
    return ret;
  }

  public void StartPickSession(PickSessionDataStruct psd) {
    C4dApiPINVOKE.BaseDocument_StartPickSession(swigCPtr, PickSessionDataStruct.getCPtr(psd));
  }

  public void StopPickSession(bool cancel) {
    C4dApiPINVOKE.BaseDocument_StopPickSession(swigCPtr, cancel);
  }

  public void AnimateObject(BaseList2D op, BaseTime time, ANIMATEFLAGS flags) {
    C4dApiPINVOKE.BaseDocument_AnimateObject(swigCPtr, BaseList2D.getCPtr(op), BaseTime.getCPtr(time), (int)flags);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public bool ExecutePasses(BaseThread bt, bool animation, bool expressions, bool caches, BUILDFLAGS flags) {
    bool ret = C4dApiPINVOKE.BaseDocument_ExecutePasses(swigCPtr, BaseThread.getCPtr(bt), animation, expressions, caches, (int)flags);
    return ret;
  }

  public BaseDocument Polygonize(bool keepanimation) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_Polygonize__SWIG_0(swigCPtr, keepanimation);
    BaseDocument ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseDocument(cPtr, false);
    return ret;
  }

  public BaseDocument Polygonize() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_Polygonize__SWIG_1(swigCPtr);
    BaseDocument ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseDocument(cPtr, false);
    return ret;
  }

  public bool CollectSounds(SWIGTYPE_p_BaseSound snd, BaseTime from, BaseTime to) {
    bool ret = C4dApiPINVOKE.BaseDocument_CollectSounds(swigCPtr, SWIGTYPE_p_BaseSound.getCPtr(snd), BaseTime.getCPtr(from), BaseTime.getCPtr(to));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public BaseSceneHook FindSceneHook(int id) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_FindSceneHook(swigCPtr, id);
    BaseSceneHook ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseSceneHook(cPtr, false);
    return ret;
  }

  public int GetSplinePlane() {
    int ret = C4dApiPINVOKE.BaseDocument_GetSplinePlane(swigCPtr);
    return ret;
  }

  public GeListHead GetLayerObjectRoot() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetLayerObjectRoot(swigCPtr);
    GeListHead ret = (cPtr == global::System.IntPtr.Zero) ? null : new GeListHead(cPtr, false);
    return ret;
  }

  public BaseContainer GetAllTextures(AtomArray ar) {
    BaseContainer ret = new BaseContainer(C4dApiPINVOKE.BaseDocument_GetAllTextures__SWIG_0(swigCPtr, AtomArray.getCPtr(ar)), true);
    return ret;
  }

  public BaseContainer GetAllTextures(bool isNet, AtomArray ar) {
    BaseContainer ret = new BaseContainer(C4dApiPINVOKE.BaseDocument_GetAllTextures__SWIG_1(swigCPtr, isNet, AtomArray.getCPtr(ar)), true);
    return ret;
  }

  public BaseObject GetHighest(int type, bool editor) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetHighest(swigCPtr, type, editor);
    BaseObject ret = (BaseObject) C4dApiPINVOKE.InstantiateConcreteObject(cPtr, false);
    return ret;
}

  public BaseBitmap GetDocPreviewBitmap() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetDocPreviewBitmap(swigCPtr);
    BaseBitmap ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseBitmap(cPtr, false);
    return ret;
  }

  public void GetHighlightedObjects(AtomArray selection) {
    C4dApiPINVOKE.BaseDocument_GetHighlightedObjects(swigCPtr, AtomArray.getCPtr(selection));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetHighlightedObject(BaseObject op, int mode) {
    C4dApiPINVOKE.BaseDocument_SetHighlightedObject__SWIG_0(swigCPtr, BaseObject.getCPtr(op), mode);
  }

  public void SetHighlightedObject(BaseObject op) {
    C4dApiPINVOKE.BaseDocument_SetHighlightedObject__SWIG_1(swigCPtr, BaseObject.getCPtr(op));
  }

  public void SetHighlightedObjects(AtomArray selection, int mode) {
    C4dApiPINVOKE.BaseDocument_SetHighlightedObjects__SWIG_0(swigCPtr, AtomArray.getCPtr(selection), mode);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetHighlightedObjects(AtomArray selection) {
    C4dApiPINVOKE.BaseDocument_SetHighlightedObjects__SWIG_1(swigCPtr, AtomArray.getCPtr(selection));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public bool GetDefaultKey(CKey pKey, SWIGTYPE_p_Bool bOverdub) {
    bool ret = C4dApiPINVOKE.BaseDocument_GetDefaultKey(swigCPtr, CKey.getCPtr(pKey), SWIGTYPE_p_Bool.getCPtr(bOverdub));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void SetDefaultKey(CKey pKey, bool bOverdub) {
    C4dApiPINVOKE.BaseDocument_SetDefaultKey(swigCPtr, CKey.getCPtr(pKey), bOverdub);
  }

  public bool IsCacheBuilt(bool force) {
    bool ret = C4dApiPINVOKE.BaseDocument_IsCacheBuilt__SWIG_0(swigCPtr, force);
    return ret;
  }

  public bool IsCacheBuilt() {
    bool ret = C4dApiPINVOKE.BaseDocument_IsCacheBuilt__SWIG_1(swigCPtr);
    return ret;
  }

  public bool IsAxisEnabled() {
    bool ret = C4dApiPINVOKE.BaseDocument_IsAxisEnabled(swigCPtr);
    return ret;
  }

  public BaseObject GetHelperAxis() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetHelperAxis(swigCPtr);
    BaseObject ret = (BaseObject) C4dApiPINVOKE.InstantiateConcreteObject(cPtr, false);
    return ret;
}

  public bool HandleSelectedTextureFilename(BaseChannel bc, Filename fn, Filename resfilename, bool undo, SWIGTYPE_p_GEMB_R already_answered) {
    bool ret = C4dApiPINVOKE.BaseDocument_HandleSelectedTextureFilename(swigCPtr, BaseChannel.getCPtr(bc), Filename.getCPtr(fn), Filename.getCPtr(resfilename), undo, SWIGTYPE_p_GEMB_R.getCPtr(already_answered));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public bool ReceiveMaterials(BaseObject op, AtomArray mat, bool clearfirst) {
    bool ret = C4dApiPINVOKE.BaseDocument_ReceiveMaterials(swigCPtr, BaseObject.getCPtr(op), AtomArray.getCPtr(mat), clearfirst);
    return ret;
  }

  public bool ReceiveNewTexture(BaseObject op, Filename filename, bool sdown, SWIGTYPE_p_GEMB_R already_answered) {
    bool ret = C4dApiPINVOKE.BaseDocument_ReceiveNewTexture(swigCPtr, BaseObject.getCPtr(op), Filename.getCPtr(filename), sdown, SWIGTYPE_p_GEMB_R.getCPtr(already_answered));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void SendInfo(int type, int format, Filename fn, BaseList2D bl, bool hooks_only) {
    C4dApiPINVOKE.BaseDocument_SendInfo(swigCPtr, type, format, Filename.getCPtr(fn), BaseList2D.getCPtr(bl), hooks_only);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void RecordZero() {
    C4dApiPINVOKE.BaseDocument_RecordZero(swigCPtr);
  }

  public void RecordNoEvent() {
    C4dApiPINVOKE.BaseDocument_RecordNoEvent(swigCPtr);
  }

  public void SetRewind(int flags) {
    C4dApiPINVOKE.BaseDocument_SetRewind__SWIG_0(swigCPtr, flags);
  }

  public void SetRewind() {
    C4dApiPINVOKE.BaseDocument_SetRewind__SWIG_1(swigCPtr);
  }

  public SWIGTYPE_p_TakeData GetTakeData() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetTakeData(swigCPtr);
    SWIGTYPE_p_TakeData ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_TakeData(cPtr, false);
    return ret;
  }

  public BaseObject GetTargetObject() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseDocument_GetTargetObject(swigCPtr);
    BaseObject ret = (BaseObject) C4dApiPINVOKE.InstantiateConcreteObject(cPtr, false);
    return ret;
}

  public void SetTargetObject(BaseObject op) {
    C4dApiPINVOKE.BaseDocument_SetTargetObject(swigCPtr, BaseObject.getCPtr(op));
  }

}

}
