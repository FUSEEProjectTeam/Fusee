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

public class ObjectTransformMsgData : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal ObjectTransformMsgData(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ObjectTransformMsgData obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~ObjectTransformMsgData() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_ObjectTransformMsgData(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public BaseObject undo {
    set {
      C4dApiPINVOKE.ObjectTransformMsgData_undo_set(swigCPtr, BaseObject.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = C4dApiPINVOKE.ObjectTransformMsgData_undo_get(swigCPtr);
      BaseObject ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseObject(cPtr, false);
      return ret;
    } 
  }

  public Fusee.Math.Core.double4x4 /* Matrix_cstype */ tm {
    /* <Matrix_csvarin> */
    set 
	{
       double[] adbl_value;
       unsafe 
	   {
		   adbl_value = Fusee.Math.ArrayConvert.double4x4ToArrayDoubleC4DLayout(value);
           fixed (double *pdbl_value = adbl_value) 
		   {
              C4dApiPINVOKE.ObjectTransformMsgData_tm_set(swigCPtr, (global::System.IntPtr) pdbl_value /*  Matrix_csin */);
		   }
	   }
    }  /* </Matrix_csvarin> */   
   /* <Matrix_csvarout> */
   get
   {  
      C34M ret_c34m = C4dApiPINVOKE.ObjectTransformMsgData_tm_get(swigCPtr);
	  Fusee.Math.Core.double4x4 ret;
	  unsafe {ret = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4(ret_c34m.m);}
      return ret;   
   } /* <Matrix_csvarout> */ 
  }

  public bool sdown {
    set {
      C4dApiPINVOKE.ObjectTransformMsgData_sdown_set(swigCPtr, value);
    } 
    get {
      bool ret = C4dApiPINVOKE.ObjectTransformMsgData_sdown_get(swigCPtr);
      return ret;
    } 
  }

  public bool useselection {
    set {
      C4dApiPINVOKE.ObjectTransformMsgData_useselection_set(swigCPtr, value);
    } 
    get {
      bool ret = C4dApiPINVOKE.ObjectTransformMsgData_useselection_get(swigCPtr);
      return ret;
    } 
  }

  public int mode {
    set {
      C4dApiPINVOKE.ObjectTransformMsgData_mode_set(swigCPtr, value);
    } 
    get {
      int ret = C4dApiPINVOKE.ObjectTransformMsgData_mode_get(swigCPtr);
      return ret;
    } 
  }

  public BaseDraw bd {
    set {
      C4dApiPINVOKE.ObjectTransformMsgData_bd_set(swigCPtr, BaseDraw.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = C4dApiPINVOKE.ObjectTransformMsgData_bd_get(swigCPtr);
      BaseDraw ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseDraw(cPtr, false);
      return ret;
    } 
  }

  public ObjectTransformMsgData() : this(C4dApiPINVOKE.new_ObjectTransformMsgData(), true) {
  }

}

}
