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

public class HNSubdiv : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal HNSubdiv(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(HNSubdiv obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~HNSubdiv() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_HNSubdiv(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public PolygonObject op {
    set {
      C4dApiPINVOKE.HNSubdiv_op_set(swigCPtr, PolygonObject.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = C4dApiPINVOKE.HNSubdiv_op_get(swigCPtr);
      PolygonObject ret = (cPtr == global::System.IntPtr.Zero) ? null : new PolygonObject(cPtr, false);
      return ret;
    } 
  }

  public double lod {
    set {
      C4dApiPINVOKE.HNSubdiv_lod_set(swigCPtr, value);
    } 
    get {
      double ret = C4dApiPINVOKE.HNSubdiv_lod_get(swigCPtr);
      return ret;
    } 
  }

  public int subdiv {
    set {
      C4dApiPINVOKE.HNSubdiv_subdiv_set(swigCPtr, value);
    } 
    get {
      int ret = C4dApiPINVOKE.HNSubdiv_subdiv_get(swigCPtr);
      return ret;
    } 
  }

  public HNSubdiv() : this(C4dApiPINVOKE.new_HNSubdiv(), true) {
  }

}

}
