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

public class CommandData : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal CommandData(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(CommandData obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~CommandData() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_CommandData(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public CommandData(bool memOwn) : this(C4dApiPINVOKE.new_CommandData(), memOwn) {
    SwigDirectorConnect();
  }

  public virtual bool Execute(BaseDocument doc) {
    bool ret = (SwigDerivedClassHasMethod("Execute", swigMethodTypes0) ? C4dApiPINVOKE.CommandData_ExecuteSwigExplicitCommandData(swigCPtr, BaseDocument.getCPtr(doc)) : C4dApiPINVOKE.CommandData_Execute(swigCPtr, BaseDocument.getCPtr(doc)));
    return ret;
  }

  public virtual bool ExecuteSubID(BaseDocument doc, int subid) {
    bool ret = (SwigDerivedClassHasMethod("ExecuteSubID", swigMethodTypes1) ? C4dApiPINVOKE.CommandData_ExecuteSubIDSwigExplicitCommandData(swigCPtr, BaseDocument.getCPtr(doc), subid) : C4dApiPINVOKE.CommandData_ExecuteSubID(swigCPtr, BaseDocument.getCPtr(doc), subid));
    return ret;
  }

  public virtual bool ExecuteOptionID(BaseDocument doc, int plugid, int subid) {
    bool ret = (SwigDerivedClassHasMethod("ExecuteOptionID", swigMethodTypes2) ? C4dApiPINVOKE.CommandData_ExecuteOptionIDSwigExplicitCommandData(swigCPtr, BaseDocument.getCPtr(doc), plugid, subid) : C4dApiPINVOKE.CommandData_ExecuteOptionID(swigCPtr, BaseDocument.getCPtr(doc), plugid, subid));
    return ret;
  }

  public virtual int GetState(BaseDocument doc) {
    int ret = (SwigDerivedClassHasMethod("GetState", swigMethodTypes3) ? C4dApiPINVOKE.CommandData_GetStateSwigExplicitCommandData(swigCPtr, BaseDocument.getCPtr(doc)) : C4dApiPINVOKE.CommandData_GetState(swigCPtr, BaseDocument.getCPtr(doc)));
    return ret;
  }

  public virtual bool GetSubContainer(BaseDocument doc, BaseContainer submenu) {
    bool ret = (SwigDerivedClassHasMethod("GetSubContainer", swigMethodTypes4) ? C4dApiPINVOKE.CommandData_GetSubContainerSwigExplicitCommandData(swigCPtr, BaseDocument.getCPtr(doc), BaseContainer.getCPtr(submenu)) : C4dApiPINVOKE.CommandData_GetSubContainer(swigCPtr, BaseDocument.getCPtr(doc), BaseContainer.getCPtr(submenu)));
    if (C4dApiPINVOKE.SWIGPendingException.Pending) throw C4dApiPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public virtual bool RestoreLayout(SWIGTYPE_p_void secret) {
    bool ret = (SwigDerivedClassHasMethod("RestoreLayout", swigMethodTypes5) ? C4dApiPINVOKE.CommandData_RestoreLayoutSwigExplicitCommandData(swigCPtr, SWIGTYPE_p_void.getCPtr(secret)) : C4dApiPINVOKE.CommandData_RestoreLayout(swigCPtr, SWIGTYPE_p_void.getCPtr(secret)));
    return ret;
  }

  public virtual string /* String_cstype */ GetScriptName()  {  /* <String_csout> */
      string ret = (SwigDerivedClassHasMethod("GetScriptName", swigMethodTypes6) ? C4dApiPINVOKE.CommandData_GetScriptNameSwigExplicitCommandData(swigCPtr) : C4dApiPINVOKE.CommandData_GetScriptName(swigCPtr));
      return ret;
   } /* </String_csout> */ 

  public virtual bool Message(int type, SWIGTYPE_p_void data) {
    bool ret = (SwigDerivedClassHasMethod("Message", swigMethodTypes7) ? C4dApiPINVOKE.CommandData_MessageSwigExplicitCommandData(swigCPtr, type, SWIGTYPE_p_void.getCPtr(data)) : C4dApiPINVOKE.CommandData_Message(swigCPtr, type, SWIGTYPE_p_void.getCPtr(data)));
    return ret;
  }

  private CommandData() : this(C4dApiPINVOKE.new_CommandData(), true) {
    SwigDirectorConnect();
  }

  private void SwigDirectorConnect() {
    if (SwigDerivedClassHasMethod("Execute", swigMethodTypes0))
      swigDelegate0 = new SwigDelegateCommandData_0(SwigDirectorExecute);
    if (SwigDerivedClassHasMethod("ExecuteSubID", swigMethodTypes1))
      swigDelegate1 = new SwigDelegateCommandData_1(SwigDirectorExecuteSubID);
    if (SwigDerivedClassHasMethod("ExecuteOptionID", swigMethodTypes2))
      swigDelegate2 = new SwigDelegateCommandData_2(SwigDirectorExecuteOptionID);
    if (SwigDerivedClassHasMethod("GetState", swigMethodTypes3))
      swigDelegate3 = new SwigDelegateCommandData_3(SwigDirectorGetState);
    if (SwigDerivedClassHasMethod("GetSubContainer", swigMethodTypes4))
      swigDelegate4 = new SwigDelegateCommandData_4(SwigDirectorGetSubContainer);
    if (SwigDerivedClassHasMethod("RestoreLayout", swigMethodTypes5))
      swigDelegate5 = new SwigDelegateCommandData_5(SwigDirectorRestoreLayout);
    if (SwigDerivedClassHasMethod("GetScriptName", swigMethodTypes6))
      swigDelegate6 = new SwigDelegateCommandData_6(SwigDirectorGetScriptName);
    if (SwigDerivedClassHasMethod("Message", swigMethodTypes7))
      swigDelegate7 = new SwigDelegateCommandData_7(SwigDirectorMessage);
    C4dApiPINVOKE.CommandData_director_connect(swigCPtr, swigDelegate0, swigDelegate1, swigDelegate2, swigDelegate3, swigDelegate4, swigDelegate5, swigDelegate6, swigDelegate7);
  }

  private bool SwigDerivedClassHasMethod(string methodName, global::System.Type[] methodTypes) {
    global::System.Reflection.MethodInfo methodInfo = this.GetType().GetMethod(methodName, global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance, null, methodTypes, null);
    bool hasDerivedMethod = methodInfo.DeclaringType.IsSubclassOf(typeof(CommandData));
    return hasDerivedMethod;
  }

  private bool SwigDirectorExecute(global::System.IntPtr doc) {
    return Execute((doc == global::System.IntPtr.Zero) ? null : new BaseDocument(doc, false));
  }

  private bool SwigDirectorExecuteSubID(global::System.IntPtr doc, int subid) {
    return ExecuteSubID((doc == global::System.IntPtr.Zero) ? null : new BaseDocument(doc, false), subid);
  }

  private bool SwigDirectorExecuteOptionID(global::System.IntPtr doc, int plugid, int subid) {
    return ExecuteOptionID((doc == global::System.IntPtr.Zero) ? null : new BaseDocument(doc, false), plugid, subid);
  }

  private int SwigDirectorGetState(global::System.IntPtr doc) {
    return GetState((doc == global::System.IntPtr.Zero) ? null : new BaseDocument(doc, false));
  }

  private bool SwigDirectorGetSubContainer(global::System.IntPtr doc, global::System.IntPtr submenu) {
    return GetSubContainer((doc == global::System.IntPtr.Zero) ? null : new BaseDocument(doc, false), new BaseContainer(submenu, false));
  }

  private bool SwigDirectorRestoreLayout(global::System.IntPtr secret) {
    return RestoreLayout((secret == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_void(secret, false));
  }

  private string /* String_imtype */ SwigDirectorGetScriptName() {
    return GetScriptName() /* String_csdirectorout */;
  }

  private bool SwigDirectorMessage(int type, global::System.IntPtr data) {
    return Message(type, (data == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_void(data, false));
  }

  public delegate bool SwigDelegateCommandData_0(global::System.IntPtr doc);
  public delegate bool SwigDelegateCommandData_1(global::System.IntPtr doc, int subid);
  public delegate bool SwigDelegateCommandData_2(global::System.IntPtr doc, int plugid, int subid);
  public delegate int SwigDelegateCommandData_3(global::System.IntPtr doc);
  public delegate bool SwigDelegateCommandData_4(global::System.IntPtr doc, global::System.IntPtr submenu);
  public delegate bool SwigDelegateCommandData_5(global::System.IntPtr secret);
  public delegate string /* String_imtype */ SwigDelegateCommandData_6();
  public delegate bool SwigDelegateCommandData_7(int type, global::System.IntPtr data);

  private SwigDelegateCommandData_0 swigDelegate0;
  private SwigDelegateCommandData_1 swigDelegate1;
  private SwigDelegateCommandData_2 swigDelegate2;
  private SwigDelegateCommandData_3 swigDelegate3;
  private SwigDelegateCommandData_4 swigDelegate4;
  private SwigDelegateCommandData_5 swigDelegate5;
  private SwigDelegateCommandData_6 swigDelegate6;
  private SwigDelegateCommandData_7 swigDelegate7;

  private static global::System.Type[] swigMethodTypes0 = new global::System.Type[] { typeof(BaseDocument) };
  private static global::System.Type[] swigMethodTypes1 = new global::System.Type[] { typeof(BaseDocument), typeof(int) };
  private static global::System.Type[] swigMethodTypes2 = new global::System.Type[] { typeof(BaseDocument), typeof(int), typeof(int) };
  private static global::System.Type[] swigMethodTypes3 = new global::System.Type[] { typeof(BaseDocument) };
  private static global::System.Type[] swigMethodTypes4 = new global::System.Type[] { typeof(BaseDocument), typeof(BaseContainer) };
  private static global::System.Type[] swigMethodTypes5 = new global::System.Type[] { typeof(SWIGTYPE_p_void) };
  private static global::System.Type[] swigMethodTypes6 = new global::System.Type[] {  };
  private static global::System.Type[] swigMethodTypes7 = new global::System.Type[] { typeof(int), typeof(SWIGTYPE_p_void) };
}

}
