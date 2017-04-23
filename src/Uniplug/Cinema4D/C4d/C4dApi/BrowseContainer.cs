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

public class BrowseContainer : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal BrowseContainer(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(BrowseContainer obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~BrowseContainer() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_BrowseContainer(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public BrowseContainer(BaseContainer bc) : this(C4dApiPINVOKE.new_BrowseContainer(BaseContainer.getCPtr(bc)), true) {
  }

  public void Reset() {
    C4dApiPINVOKE.BrowseContainer_Reset(swigCPtr);
  }

  public bool GetNext(SWIGTYPE_p_Int32 id, SWIGTYPE_p_p_GeData data) {
    bool ret = C4dApiPINVOKE.BrowseContainer_GetNext(swigCPtr, SWIGTYPE_p_Int32.getCPtr(id), SWIGTYPE_p_p_GeData.getCPtr(data));
    return ret;
  }

}

}
