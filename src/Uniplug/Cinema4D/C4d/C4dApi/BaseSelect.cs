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

public class BaseSelect : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal BaseSelect(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(BaseSelect obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          throw new global::System.MethodAccessException("C++ destructor does not have public access");
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public static BaseSelect Alloc() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseSelect_Alloc();
    BaseSelect ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseSelect(cPtr, false);
    return ret;
  }

  public static void Free(SWIGTYPE_p_p_BaseSelect bs) {
    C4dApiPINVOKE.BaseSelect_Free(SWIGTYPE_p_p_BaseSelect.getCPtr(bs));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public int GetCount() {
    int ret = C4dApiPINVOKE.BaseSelect_GetCount(swigCPtr);
    return ret;
  }

  public int GetSegments() {
    int ret = C4dApiPINVOKE.BaseSelect_GetSegments(swigCPtr);
    return ret;
  }

  public bool Select(int num) {
    bool ret = C4dApiPINVOKE.BaseSelect_Select(swigCPtr, num);
    return ret;
  }

  public bool SelectAll(int min, int max) {
    bool ret = C4dApiPINVOKE.BaseSelect_SelectAll(swigCPtr, min, max);
    return ret;
  }

  public bool Deselect(int num) {
    bool ret = C4dApiPINVOKE.BaseSelect_Deselect__SWIG_0(swigCPtr, num);
    return ret;
  }

  public bool DeselectAll() {
    bool ret = C4dApiPINVOKE.BaseSelect_DeselectAll(swigCPtr);
    return ret;
  }

  public bool Toggle(int num) {
    bool ret = C4dApiPINVOKE.BaseSelect_Toggle(swigCPtr, num);
    return ret;
  }

  public bool ToggleAll(int min, int max) {
    bool ret = C4dApiPINVOKE.BaseSelect_ToggleAll(swigCPtr, min, max);
    return ret;
  }

  public bool GetRange(int seg, int maxElements, SWIGTYPE_p_Int32 a, SWIGTYPE_p_Int32 b) {
    bool ret = C4dApiPINVOKE.BaseSelect_GetRange(swigCPtr, seg, maxElements, SWIGTYPE_p_Int32.getCPtr(a), SWIGTYPE_p_Int32.getCPtr(b));
    return ret;
  }

  public bool IsSelected(int num) {
    bool ret = C4dApiPINVOKE.BaseSelect_IsSelected(swigCPtr, num);
    return ret;
  }

  public bool CopyTo(BaseSelect dest) {
    bool ret = C4dApiPINVOKE.BaseSelect_CopyTo(swigCPtr, BaseSelect.getCPtr(dest));
    return ret;
  }

  public BaseSelect GetClone() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseSelect_GetClone(swigCPtr);
    BaseSelect ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseSelect(cPtr, false);
    return ret;
  }

  public bool Merge(BaseSelect src) {
    bool ret = C4dApiPINVOKE.BaseSelect_Merge(swigCPtr, BaseSelect.getCPtr(src));
    return ret;
  }

  public bool Deselect(BaseSelect src) {
    bool ret = C4dApiPINVOKE.BaseSelect_Deselect__SWIG_1(swigCPtr, BaseSelect.getCPtr(src));
    return ret;
  }

  public bool Cross(BaseSelect src) {
    bool ret = C4dApiPINVOKE.BaseSelect_Cross(swigCPtr, BaseSelect.getCPtr(src));
    return ret;
  }

  public bool FromArray(SWIGTYPE_p_UChar selection, int count) {
    bool ret = C4dApiPINVOKE.BaseSelect_FromArray(swigCPtr, SWIGTYPE_p_UChar.getCPtr(selection), count);
    return ret;
  }

  public SWIGTYPE_p_UChar ToArray(int count) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseSelect_ToArray(swigCPtr, count);
    SWIGTYPE_p_UChar ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_UChar(cPtr, false);
    return ret;
  }

  public bool Read(HyperFile hf) {
    bool ret = C4dApiPINVOKE.BaseSelect_Read(swigCPtr, HyperFile.getCPtr(hf));
    return ret;
  }

  public void Write(HyperFile hf) {
    C4dApiPINVOKE.BaseSelect_Write(swigCPtr, HyperFile.getCPtr(hf));
  }

  public bool FindSegment(int num, SWIGTYPE_p_Int32 segment) {
    bool ret = C4dApiPINVOKE.BaseSelect_FindSegment(swigCPtr, num, SWIGTYPE_p_Int32.getCPtr(segment));
    return ret;
  }

  public int GetDirty() {
    int ret = C4dApiPINVOKE.BaseSelect_GetDirty(swigCPtr);
    return ret;
  }

  public int GetLastElement() {
    int ret = C4dApiPINVOKE.BaseSelect_GetLastElement(swigCPtr);
    return ret;
  }

  public BaseSelectData GetData() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.BaseSelect_GetData(swigCPtr);
    BaseSelectData ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseSelectData(cPtr, false);
    return ret;
  }

  public bool CopyFrom(BaseSelectData ndata, int ncnt) {
    bool ret = C4dApiPINVOKE.BaseSelect_CopyFrom(swigCPtr, BaseSelectData.getCPtr(ndata), ncnt);
    return ret;
  }

  public int GetRangeA(int seg) {
    int ret = C4dApiPINVOKE.BaseSelect_GetRangeA(swigCPtr, seg);
    return ret;
  }

  public int GetRangeB(int seg) {
    int ret = C4dApiPINVOKE.BaseSelect_GetRangeB(swigCPtr, seg);
    return ret;
  }

}

}
