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

public class GeData : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal GeData(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(GeData obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~GeData() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_GeData(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public GeData() : this(C4dApiPINVOKE.new_GeData__SWIG_0(), true) {
  }

  public GeData(double n) : this(C4dApiPINVOKE.new_GeData__SWIG_1(n), true) {
  }

  public GeData(GeData n) : this(C4dApiPINVOKE.new_GeData__SWIG_2(GeData.getCPtr(n)), true) {
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public GeData(int n) : this(C4dApiPINVOKE.new_GeData__SWIG_3(n), true) {
  }

  public GeData(float n) : this(C4dApiPINVOKE.new_GeData__SWIG_4(n), true) {
  }

  public GeData(SWIGTYPE_p_void v, VOIDVALUETYPE dummy) : this(C4dApiPINVOKE.new_GeData__SWIG_5(SWIGTYPE_p_void.getCPtr(v), (int)dummy), true) {
  }

  public GeData(long v, LLONGVALUETYPE dummy) : this(C4dApiPINVOKE.new_GeData__SWIG_6(v, (int)dummy), true) {
  }

  public GeData(Fusee.Math.Core.double3 /* constVector&_cstype */ n) : this(C4dApiPINVOKE.new_GeData__SWIG_7(ref n /* constVector&_csin */), true) {
  }

  public GeData(SWIGTYPE_p_C4DUuid n) : this(C4dApiPINVOKE.new_GeData__SWIG_8(SWIGTYPE_p_C4DUuid.getCPtr(n)), true) {
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public GeData(SWIGTYPE_p_Char n) : this(C4dApiPINVOKE.new_GeData__SWIG_9(SWIGTYPE_p_Char.getCPtr(n)), true) {
  }

  public GeData(string /* constString&_cstype */ n) : this(C4dApiPINVOKE.new_GeData__SWIG_10(n), true) {
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public GeData(SWIGTYPE_p_void mem, int count) : this(C4dApiPINVOKE.new_GeData__SWIG_11(SWIGTYPE_p_void.getCPtr(mem), count), true) {
  }

  public GeData(Filename n) : this(C4dApiPINVOKE.new_GeData__SWIG_12(Filename.getCPtr(n)), true) {
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public GeData(BaseTime n) : this(C4dApiPINVOKE.new_GeData__SWIG_13(BaseTime.getCPtr(n)), true) {
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public GeData(BaseContainer n) : this(C4dApiPINVOKE.new_GeData__SWIG_14(BaseContainer.getCPtr(n)), true) {
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public GeData(BaseLink n) : this(C4dApiPINVOKE.new_GeData__SWIG_15(BaseLink.getCPtr(n)), true) {
  }

  public GeData(BaseList2D bl) : this(C4dApiPINVOKE.new_GeData__SWIG_16(BaseList2D.getCPtr(bl)), true) {
  }

  public GeData(int type, CustomDataType data) : this(C4dApiPINVOKE.new_GeData__SWIG_17(type, CustomDataType.getCPtr(data)), true) {
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public GeData(int type, DEFAULTVALUETYPE v) : this(C4dApiPINVOKE.new_GeData__SWIG_18(type, (int)v), true) {
  }

  public bool SetDefault(int type) {
    bool ret = C4dApiPINVOKE.GeData_SetDefault(swigCPtr, type);
    return ret;
  }

  public void Free() {
    C4dApiPINVOKE.GeData_Free(swigCPtr);
  }

  public int GetTypeC4D() {
    int ret = C4dApiPINVOKE.GeData_GetTypeC4D(swigCPtr);
    return ret;
  }

  public bool GetBool() {
    bool ret = C4dApiPINVOKE.GeData_GetBool(swigCPtr);
    return ret;
  }

  public int GetInt32() {
    int ret = C4dApiPINVOKE.GeData_GetInt32(swigCPtr);
    return ret;
  }

  public long GetInt64() {
    long ret = C4dApiPINVOKE.GeData_GetInt64(swigCPtr);
    return ret;
  }

  public double GetFloat() {
    double ret = C4dApiPINVOKE.GeData_GetFloat(swigCPtr);
    return ret;
  }

  public SWIGTYPE_p_void GetVoid() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.GeData_GetVoid(swigCPtr);
    SWIGTYPE_p_void ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_void(cPtr, false);
    return ret;
  }

  public Fusee.Math.Core.double3 /* constVector&_cstype_out */ GetVector()  {  /* <constVector&_csout> */
      global::System.IntPtr p_ret = C4dApiPINVOKE.GeData_GetVector(swigCPtr);
      Fusee.Math.Core.double3 ret;
      unsafe {ret = Fusee.Math.ArrayConvert.ArrayDoubleTodouble3((double *)p_ret);}
      return ret;
   } /* </constVector&_csout> */ 

  public Fusee.Math.Core.double4x4 /* constMatrix&_cstype_out */ GetMatrix()  {  /* <constMatrix&_csout> */
      global::System.IntPtr p_ret = C4dApiPINVOKE.GeData_GetMatrix(swigCPtr);
      Fusee.Math.Core.double4x4 ret;
      unsafe {ret = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4((double *)p_ret);}
      return ret;
   } /* </constMatrix&_csout> */ 

  public string /* constString&_cstype */ GetString() {
    string ret = C4dApiPINVOKE.GeData_GetString(swigCPtr);
    return ret;
  }

  public SWIGTYPE_p_C4DUuid GetUuid() {
    SWIGTYPE_p_C4DUuid ret = new SWIGTYPE_p_C4DUuid(C4dApiPINVOKE.GeData_GetUuid(swigCPtr), false);
    return ret;
  }

  public Filename GetFilename() {
    Filename ret = new Filename(C4dApiPINVOKE.GeData_GetFilename(swigCPtr), false);
    return ret;
  }

  public BaseTime GetTime() {
    BaseTime ret = new BaseTime(C4dApiPINVOKE.GeData_GetTime(swigCPtr), false);
    return ret;
  }

  public BaseContainer GetContainer() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.GeData_GetContainer(swigCPtr);
    BaseContainer ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseContainer(cPtr, false);
    return ret;
  }

  public BaseLink GetBaseLink() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.GeData_GetBaseLink(swigCPtr);
    BaseLink ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseLink(cPtr, false);
    return ret;
  }

  public CustomDataType GetCustomDataType(int datatype) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.GeData_GetCustomDataType(swigCPtr, datatype);
    CustomDataType ret = (cPtr == global::System.IntPtr.Zero) ? null : new CustomDataType(cPtr, false);
    return ret;
  }

  public BaseList2D GetLink(BaseDocument doc, int instanceof) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.GeData_GetLink__SWIG_0(swigCPtr, BaseDocument.getCPtr(doc), instanceof);
    BaseList2D ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseList2D(cPtr, false);
    return ret;
  }

  public BaseList2D GetLink(BaseDocument doc) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.GeData_GetLink__SWIG_1(swigCPtr, BaseDocument.getCPtr(doc));
    BaseList2D ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseList2D(cPtr, false);
    return ret;
  }

  public C4DAtomGoal GetLinkAtom(BaseDocument doc, int instanceof) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.GeData_GetLinkAtom__SWIG_0(swigCPtr, BaseDocument.getCPtr(doc), instanceof);
    C4DAtomGoal ret = (cPtr == global::System.IntPtr.Zero) ? null : new C4DAtomGoal(cPtr, false);
    return ret;
  }

  public C4DAtomGoal GetLinkAtom(BaseDocument doc) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.GeData_GetLinkAtom__SWIG_1(swigCPtr, BaseDocument.getCPtr(doc));
    C4DAtomGoal ret = (cPtr == global::System.IntPtr.Zero) ? null : new C4DAtomGoal(cPtr, false);
    return ret;
  }

  public SWIGTYPE_p_void GetMemoryAndRelease(SWIGTYPE_p_Int count) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.GeData_GetMemoryAndRelease(swigCPtr, SWIGTYPE_p_Int.getCPtr(count));
    SWIGTYPE_p_void ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_void(cPtr, false);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public SWIGTYPE_p_void GetMemory(SWIGTYPE_p_Int count) {
    global::System.IntPtr cPtr = C4dApiPINVOKE.GeData_GetMemory(swigCPtr, SWIGTYPE_p_Int.getCPtr(count));
    SWIGTYPE_p_void ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_void(cPtr, false);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void CopyData(GeData dest, AliasTrans aliastrans) {
    C4dApiPINVOKE.GeData_CopyData(swigCPtr, GeData.getCPtr(dest), AliasTrans.getCPtr(aliastrans));
  }

  public void SetFloat(double v) {
    C4dApiPINVOKE.GeData_SetFloat(swigCPtr, v);
  }

  public void SetInt32(int v) {
    C4dApiPINVOKE.GeData_SetInt32(swigCPtr, v);
  }

  public void SetInt64(SWIGTYPE_p_Int64 v) {
    C4dApiPINVOKE.GeData_SetInt64(swigCPtr, SWIGTYPE_p_Int64.getCPtr(v));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetVoid(SWIGTYPE_p_void v) {
    C4dApiPINVOKE.GeData_SetVoid(swigCPtr, SWIGTYPE_p_void.getCPtr(v));
  }

  public void SetMemory(SWIGTYPE_p_void data, int count) {
    C4dApiPINVOKE.GeData_SetMemory(swigCPtr, SWIGTYPE_p_void.getCPtr(data), count);
  }

  public void SetVector(Fusee.Math.Core.double3 /* constVector&_cstype */ v) {
    C4dApiPINVOKE.GeData_SetVector(swigCPtr, ref v /* constVector&_csin */);
  }

  public void SetMatrix(ref Fusee.Math.Core.double4x4 /* constMatrix&_cstype */ v) {
    double[] adbl_v;
    unsafe {adbl_v = Fusee.Math.ArrayConvert.double4x4ToArrayDoubleC4DLayout(v);    fixed (double *pdbl_v = adbl_v) {
    /* constMatrix&_csin_pre */
    try {
      C4dApiPINVOKE.GeData_SetMatrix(swigCPtr, (global::System.IntPtr) pdbl_v /* constMatrix&_csin */);
    } finally {
        // NOP v = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4(pdbl_v);
        /* constMatrix&_csin_post */
    }
} } /* constMatrix&_csin_terminator */
  }

  public void SetString(string /* constString&_cstype */ v) {
    C4dApiPINVOKE.GeData_SetString(swigCPtr, v);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetUuid(SWIGTYPE_p_C4DUuid v) {
    C4dApiPINVOKE.GeData_SetUuid(swigCPtr, SWIGTYPE_p_C4DUuid.getCPtr(v));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetFilename(Filename v) {
    C4dApiPINVOKE.GeData_SetFilename(swigCPtr, Filename.getCPtr(v));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetBaseTime(BaseTime v) {
    C4dApiPINVOKE.GeData_SetBaseTime(swigCPtr, BaseTime.getCPtr(v));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetContainer(BaseContainer v) {
    C4dApiPINVOKE.GeData_SetContainer(swigCPtr, BaseContainer.getCPtr(v));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetBaseLink(BaseLink v) {
    C4dApiPINVOKE.GeData_SetBaseLink(swigCPtr, BaseLink.getCPtr(v));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetBaseList2D(BaseList2D bl) {
    C4dApiPINVOKE.GeData_SetBaseList2D(swigCPtr, BaseList2D.getCPtr(bl));
  }

  public void SetCustomDataType(int datatype, CustomDataType v) {
    C4dApiPINVOKE.GeData_SetCustomDataType(swigCPtr, datatype, CustomDataType.getCPtr(v));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

}

}
