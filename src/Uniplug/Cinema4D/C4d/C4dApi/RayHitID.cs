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

public class RayHitID : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal RayHitID(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(RayHitID obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~RayHitID() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_RayHitID(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public RayHitID() : this(C4dApiPINVOKE.new_RayHitID__SWIG_0(), true) {
  }

  public RayHitID(SWIGTYPE_p__DONTCONSTRUCT DC) : this(C4dApiPINVOKE.new_RayHitID__SWIG_1(SWIGTYPE_p__DONTCONSTRUCT.getCPtr(DC)), true) {
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public RayHitID(RayHitID other) : this(C4dApiPINVOKE.new_RayHitID__SWIG_2(RayHitID.getCPtr(other)), true) {
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public RayHitID(SWIGTYPE_p_RayObject t_rayobject, int t_polygon, bool second) : this(C4dApiPINVOKE.new_RayHitID__SWIG_3(SWIGTYPE_p_RayObject.getCPtr(t_rayobject), t_polygon, second), true) {
  }

  public bool IsEqual(RayHitID snd) {
    bool ret = C4dApiPINVOKE.RayHitID_IsEqual(swigCPtr, RayHitID.getCPtr(snd));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public bool Content() {
    bool ret = C4dApiPINVOKE.RayHitID_Content(swigCPtr);
    return ret;
  }

  public void Clear() {
    C4dApiPINVOKE.RayHitID_Clear(swigCPtr);
  }

  public void Set(SWIGTYPE_p_RayObject t_rayobject, int t_polygon, bool second) {
    C4dApiPINVOKE.RayHitID_Set(swigCPtr, SWIGTYPE_p_RayObject.getCPtr(t_rayobject), t_polygon, second);
  }

  public SWIGTYPE_p_RayObject GetObject(SWIGTYPE_p_VolumeData vd) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.RayHitID_GetObject(swigCPtr, SWIGTYPE_p_VolumeData.getCPtr(vd));
    SWIGTYPE_p_RayObject ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_RayObject(cPtr, false);
    return ret;
  }

  public int GetPolygon() {
    int ret = C4dApiPINVOKE.RayHitID_GetPolygon(swigCPtr);
    return ret;
  }

  public bool GetSecond() {
    bool ret = C4dApiPINVOKE.RayHitID_GetSecond(swigCPtr);
    return ret;
  }

  public void ClearSecond() {
    C4dApiPINVOKE.RayHitID_ClearSecond(swigCPtr);
  }

  public void SetSecond() {
    C4dApiPINVOKE.RayHitID_SetSecond(swigCPtr);
  }

  public void SetPrivateData(int t_rayobject, int t_polygon) {
    C4dApiPINVOKE.RayHitID_SetPrivateData(swigCPtr, t_rayobject, t_polygon);
  }

  public void GetPrivateData(SWIGTYPE_p_Int32 t_rayobject, SWIGTYPE_p_Int32 t_polygon) {
    C4dApiPINVOKE.RayHitID_GetPrivateData(swigCPtr, SWIGTYPE_p_Int32.getCPtr(t_rayobject), SWIGTYPE_p_Int32.getCPtr(t_polygon));
  }

}

}
