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

public class RenameTextureMessage : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal RenameTextureMessage(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(RenameTextureMessage obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~RenameTextureMessage() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_RenameTextureMessage(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public RenameTextureMessage() : this(C4dApiPINVOKE.new_RenameTextureMessage(), true) {
  }

  public Filename oldname {
    set {
      C4dApiPINVOKE.RenameTextureMessage_oldname_set(swigCPtr, Filename.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = C4dApiPINVOKE.RenameTextureMessage_oldname_get(swigCPtr);
      Filename ret = (cPtr == global::System.IntPtr.Zero) ? null : new Filename(cPtr, false);
      return ret;
    } 
  }

  public Filename newname {
    set {
      C4dApiPINVOKE.RenameTextureMessage_newname_set(swigCPtr, Filename.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = C4dApiPINVOKE.RenameTextureMessage_newname_get(swigCPtr);
      Filename ret = (cPtr == global::System.IntPtr.Zero) ? null : new Filename(cPtr, false);
      return ret;
    } 
  }

  public BaseDocument doc {
    set {
      C4dApiPINVOKE.RenameTextureMessage_doc_set(swigCPtr, BaseDocument.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = C4dApiPINVOKE.RenameTextureMessage_doc_get(swigCPtr);
      BaseDocument ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseDocument(cPtr, false);
      return ret;
    } 
  }

  public int changecnt {
    set {
      C4dApiPINVOKE.RenameTextureMessage_changecnt_set(swigCPtr, value);
    } 
    get {
      int ret = C4dApiPINVOKE.RenameTextureMessage_changecnt_get(swigCPtr);
      return ret;
    } 
  }

  public bool noundo {
    set {
      C4dApiPINVOKE.RenameTextureMessage_noundo_set(swigCPtr, value);
    } 
    get {
      bool ret = C4dApiPINVOKE.RenameTextureMessage_noundo_get(swigCPtr);
      return ret;
    } 
  }

}

}
