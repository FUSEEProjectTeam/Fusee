//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.8
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace C4d {

public class MovieSaver : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal MovieSaver(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(MovieSaver obj) {
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

  public IMAGERESULT Open(Filename name, BaseBitmap bm, int fps, int format, BaseContainer data, SAVEBIT savebits, SWIGTYPE_p_BaseSound sound) {
    IMAGERESULT ret = (IMAGERESULT)C4dApiPINVOKE.MovieSaver_Open__SWIG_0(swigCPtr, Filename.getCPtr(name), BaseBitmap.getCPtr(bm), fps, format, BaseContainer.getCPtr(data), (int)savebits, SWIGTYPE_p_BaseSound.getCPtr(sound));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public IMAGERESULT Open(Filename name, BaseBitmap bm, int fps, int format, BaseContainer data, SAVEBIT savebits) {
    IMAGERESULT ret = (IMAGERESULT)C4dApiPINVOKE.MovieSaver_Open__SWIG_1(swigCPtr, Filename.getCPtr(name), BaseBitmap.getCPtr(bm), fps, format, BaseContainer.getCPtr(data), (int)savebits);
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public IMAGERESULT Write(BaseBitmap bm) {
    IMAGERESULT ret = (IMAGERESULT)C4dApiPINVOKE.MovieSaver_Write(swigCPtr, BaseBitmap.getCPtr(bm));
    return ret;
  }

  public void Close() {
    C4dApiPINVOKE.MovieSaver_Close(swigCPtr);
  }

  public bool Choose(int format, BaseContainer bc) {
    bool ret = C4dApiPINVOKE.MovieSaver_Choose(swigCPtr, format, BaseContainer.getCPtr(bc));
    return ret;
  }

  public static MovieSaver Alloc() {
    global::System.IntPtr cPtr = C4dApiPINVOKE.MovieSaver_Alloc();
    MovieSaver ret = (cPtr == global::System.IntPtr.Zero) ? null : new MovieSaver(cPtr, false);
    return ret;
  }

  public static void Free(SWIGTYPE_p_p_MovieSaver bc) {
    C4dApiPINVOKE.MovieSaver_Free(SWIGTYPE_p_p_MovieSaver.getCPtr(bc));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
  }

}

}
