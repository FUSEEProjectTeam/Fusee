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

public class DescriptionCheckUpdate : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal DescriptionCheckUpdate(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(DescriptionCheckUpdate obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~DescriptionCheckUpdate() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_DescriptionCheckUpdate(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public DescriptionCheckUpdate() : this(C4dApiPINVOKE.new_DescriptionCheckUpdate(), true) {
  }

  public BaseDocument doc {
    set {
      C4dApiPINVOKE.DescriptionCheckUpdate_doc_set(swigCPtr, BaseDocument.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = C4dApiPINVOKE.DescriptionCheckUpdate_doc_get(swigCPtr);
      BaseDocument ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseDocument(cPtr, false);
      return ret;
    } 
  }

  public int drawflags {
    set {
      C4dApiPINVOKE.DescriptionCheckUpdate_drawflags_set(swigCPtr, value);
    } 
    get {
      int ret = C4dApiPINVOKE.DescriptionCheckUpdate_drawflags_get(swigCPtr);
      return ret;
    } 
  }

  public DescID descid {
    set {
      C4dApiPINVOKE.DescriptionCheckUpdate_descid_set(swigCPtr, DescID.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = C4dApiPINVOKE.DescriptionCheckUpdate_descid_get(swigCPtr);
      DescID ret = (cPtr == global::System.IntPtr.Zero) ? null : new DescID(cPtr, false);
      return ret;
    } 
  }

}

}
