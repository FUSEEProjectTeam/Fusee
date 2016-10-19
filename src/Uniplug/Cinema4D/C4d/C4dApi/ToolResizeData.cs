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

public class ToolResizeData : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal ToolResizeData(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ToolResizeData obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~ToolResizeData() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_ToolResizeData(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public ToolResizeData.RESIZE_PASS pass {
    set {
      C4dApiPINVOKE.ToolResizeData_pass_set(swigCPtr, (int)value);
    } 
    get {
      ToolResizeData.RESIZE_PASS ret = (ToolResizeData.RESIZE_PASS)C4dApiPINVOKE.ToolResizeData_pass_get(swigCPtr);
      return ret;
    } 
  }

  public int delta {
    set {
      C4dApiPINVOKE.ToolResizeData_delta_set(swigCPtr, value);
    } 
    get {
      int ret = C4dApiPINVOKE.ToolResizeData_delta_get(swigCPtr);
      return ret;
    } 
  }

  public int total_delta {
    set {
      C4dApiPINVOKE.ToolResizeData_total_delta_set(swigCPtr, value);
    } 
    get {
      int ret = C4dApiPINVOKE.ToolResizeData_total_delta_get(swigCPtr);
      return ret;
    } 
  }

  public bool horizontal {
    set {
      C4dApiPINVOKE.ToolResizeData_horizontal_set(swigCPtr, value);
    } 
    get {
      bool ret = C4dApiPINVOKE.ToolResizeData_horizontal_get(swigCPtr);
      return ret;
    } 
  }

  public BaseContainer data {
    set {
      C4dApiPINVOKE.ToolResizeData_data_set(swigCPtr, BaseContainer.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = C4dApiPINVOKE.ToolResizeData_data_get(swigCPtr);
      BaseContainer ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseContainer(cPtr, false);
      return ret;
    } 
  }

  public bool cross_type {
    set {
      C4dApiPINVOKE.ToolResizeData_cross_type_set(swigCPtr, value);
    } 
    get {
      bool ret = C4dApiPINVOKE.ToolResizeData_cross_type_get(swigCPtr);
      return ret;
    } 
  }

  public class ToolResizeFalloffData : global::System.IDisposable {
    private global::System.Runtime.InteropServices.HandleRef swigCPtr;
    protected bool swigCMemOwn;
  
    internal ToolResizeFalloffData(global::System.IntPtr cPtr, bool cMemoryOwn) {
      swigCMemOwn = cMemoryOwn;
      swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
    }
  
    internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ToolResizeFalloffData obj) {
      return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
    }
  
    ~ToolResizeFalloffData() {
      Dispose();
    }
  
    public virtual void Dispose() {
      lock(this) {
        if (swigCPtr.Handle != global::System.IntPtr.Zero) {
          if (swigCMemOwn) {
            swigCMemOwn = false;
            C4dApiPINVOKE.delete_ToolResizeData_ToolResizeFalloffData(swigCPtr);
          }
          swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
        }
        global::System.GC.SuppressFinalize(this);
      }
    }
  
    public ToolResizeFalloffData() : this(C4dApiPINVOKE.new_ToolResizeData_ToolResizeFalloffData(), true) {
    }
  
    public bool show {
      set {
        C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_show_set(swigCPtr, value);
      } 
      get {
        bool ret = C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_show_get(swigCPtr);
        return ret;
      } 
    }
  
    public double opacity {
      set {
        C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_opacity_set(swigCPtr, value);
      } 
      get {
        double ret = C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_opacity_get(swigCPtr);
        return ret;
      } 
    }
  
    public double size {
      set {
        C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_size_set(swigCPtr, value);
      } 
      get {
        double ret = C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_size_get(swigCPtr);
        return ret;
      } 
    }
  
    public double hardness {
      set {
        C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_hardness_set(swigCPtr, value);
      } 
      get {
        double ret = C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_hardness_get(swigCPtr);
        return ret;
      } 
    }
  
    public Fusee.Math.Core.double3 /* Vector_cstype_out */ color {
      /* <Vector_csvarin> */
      set 
  	{
        C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_color_set(swigCPtr, value /* Vector_csin */);
      }  /* </Vector_csvarin> */   
     /* <Vector_csvarout> */
     get
     {  
        Fusee.Math.Core.double3 ret = C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_color_get(swigCPtr);
        return ret;
     } /* <Vector_csvarout> */ 
    }
  
    public Fusee.Math.Core.double4x4 /* Matrix_cstype */ position {
      /* <Matrix_csvarin> */
      set 
  	{
         double[] adbl_value;
         unsafe 
  	   {
  		   adbl_value = Fusee.Math.ArrayConvert.double4x4ToArrayDoubleC4DLayout(value);
             fixed (double *pdbl_value = adbl_value) 
  		   {
                C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_position_set(swigCPtr, (global::System.IntPtr) pdbl_value /*  Matrix_csin */);
  		   }
  	   }
      }  /* </Matrix_csvarin> */   
     /* <Matrix_csvarout> */
     get
     {  
        C34M ret_c34m = C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_position_get(swigCPtr);
  	  Fusee.Math.Core.double4x4 ret;
  	  unsafe {ret = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4(ret_c34m.m);}
        return ret;   
     } /* <Matrix_csvarout> */ 
    }
  
    public bool is_3D {
      set {
        C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_is_3D_set(swigCPtr, value);
      } 
      get {
        bool ret = C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_is_3D_get(swigCPtr);
        return ret;
      } 
    }
  
    public bool dirty {
      set {
        C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_dirty_set(swigCPtr, value);
      } 
      get {
        bool ret = C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_dirty_get(swigCPtr);
        return ret;
      } 
    }
  
    public SWIGTYPE_p_SplineData falloff {
      set {
        C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_falloff_set(swigCPtr, SWIGTYPE_p_SplineData.getCPtr(value));
      } 
      get {
        global::System.IntPtr cPtr = C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_falloff_get(swigCPtr);
        SWIGTYPE_p_SplineData ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_SplineData(cPtr, false);
        return ret;
      } 
    }
  
    public BaseBitmap custom_shape {
      set {
        C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_custom_shape_set(swigCPtr, BaseBitmap.getCPtr(value));
      } 
      get {
        global::System.IntPtr cPtr = C4dApiPINVOKE.ToolResizeData_ToolResizeFalloffData_custom_shape_get(swigCPtr);
        BaseBitmap ret = (cPtr == global::System.IntPtr.Zero) ? null : new BaseBitmap(cPtr, false);
        return ret;
      } 
    }
  
  }

  public ToolResizeData.ToolResizeFalloffData falloff {
    set {
      C4dApiPINVOKE.ToolResizeData_falloff_set(swigCPtr, ToolResizeData.ToolResizeFalloffData.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = C4dApiPINVOKE.ToolResizeData_falloff_get(swigCPtr);
      ToolResizeData.ToolResizeFalloffData ret = (cPtr == global::System.IntPtr.Zero) ? null : new ToolResizeData.ToolResizeFalloffData(cPtr, false);
      return ret;
    } 
  }

  public string /* constString&_cstype */ cursor_text {
    set {
      C4dApiPINVOKE.ToolResizeData_cursor_text_set(swigCPtr, value);
      if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = C4dApiPINVOKE.ToolResizeData_cursor_text_get(swigCPtr);
      if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public ToolResizeData() : this(C4dApiPINVOKE.new_ToolResizeData(), true) {
  }

  public enum RESIZE_PASS {
    RESIZE_PASS_INIT = 0,
    RESIZE_PASS_GENERATEFALLOFF = 1,
    RESIZE_PASS_RESIZE = 2,
    RESIZE_PASS_END = 3,
    RESIZE_PASS_RESET = 4
  }

}

}
