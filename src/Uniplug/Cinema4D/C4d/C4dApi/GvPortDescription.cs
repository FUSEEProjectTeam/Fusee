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

public class GvPortDescription : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal GvPortDescription(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(GvPortDescription obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~GvPortDescription() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_GvPortDescription(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public GvPortDescription() : this(C4dApiPINVOKE.new_GvPortDescription(), true) {
  }

  public string /* constString&_cstype */ name {
    set {
      C4dApiPINVOKE.GvPortDescription_name_set(swigCPtr, value);
      if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = C4dApiPINVOKE.GvPortDescription_name_get(swigCPtr);
      if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public GvPortDescFlags flags {
    set {
      C4dApiPINVOKE.GvPortDescription_flags_set(swigCPtr, (int)value);
    } 
    get {
      GvPortDescFlags ret = (GvPortDescFlags)C4dApiPINVOKE.GvPortDescription_flags_get(swigCPtr);
      return ret;
    } 
  }

  public int data_id {
    set {
      C4dApiPINVOKE.GvPortDescription_data_id_set(swigCPtr, value);
    } 
    get {
      int ret = C4dApiPINVOKE.GvPortDescription_data_id_get(swigCPtr);
      return ret;
    } 
  }

  public int ports_min {
    set {
      C4dApiPINVOKE.GvPortDescription_ports_min_set(swigCPtr, value);
    } 
    get {
      int ret = C4dApiPINVOKE.GvPortDescription_ports_min_get(swigCPtr);
      return ret;
    } 
  }

  public int ports_max {
    set {
      C4dApiPINVOKE.GvPortDescription_ports_max_set(swigCPtr, value);
    } 
    get {
      int ret = C4dApiPINVOKE.GvPortDescription_ports_max_get(swigCPtr);
      return ret;
    } 
  }

  public int parent_id {
    set {
      C4dApiPINVOKE.GvPortDescription_parent_id_set(swigCPtr, value);
    } 
    get {
      int ret = C4dApiPINVOKE.GvPortDescription_parent_id_get(swigCPtr);
      return ret;
    } 
  }

}

}
