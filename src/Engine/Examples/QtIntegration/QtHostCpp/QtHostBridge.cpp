// VC8LibWrapper.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "QtHostBridge.h"
// #include "CPPInteropImp.h"
#include <vcclr.h>
using namespace System::Runtime::InteropServices;
using namespace System::Reflection;

using namespace System;
using namespace FuseeQtHostBridge;



public ref class QtHostBridgeDelegateContainer
{
public:
	QtHostBridge *_pBridge;
	QtHostBridgeDelegateContainer(QtHostBridge *pBridge)
	{
		_pBridge = pBridge;
	}

	// GetMousePos
	QtHostBridge::MousePosQueryFunc _pMousePosFunc;
	int GetMousePos()
	{
		if (_pMousePosFunc != NULL)
		{
			return _pMousePosFunc();
		}
		return 0;
	}
};


struct QtHostBridge::QtHostBridgeInternal
{
	QtHostBridgeInternal(Fusee::Engine::QtHost^ mh)
	{
		_mh = mh;
	}
	gcroot<Fusee::Engine::QtHost^> _mh;
	gcroot<QtHostBridgeDelegateContainer^> _hbc;
};


QtHostBridge::QtHostBridge(void *hwnd)
{
	_pImpl = new QtHostBridge::QtHostBridgeInternal(gcnew Fusee::Engine::QtHost((IntPtr) hwnd));
}

void QtHostBridge::SetMousePosQuery(MousePosQueryFunc pFunc)
{
	_pImpl->_hbc = gcnew QtHostBridgeDelegateContainer(this);
	_pImpl->_hbc->_pMousePosFunc = pFunc;
	_pImpl->_mh->SetMousePosQuery(gcnew Fusee::Engine::QtHost::MousePosDelegate(_pImpl->_hbc, &QtHostBridgeDelegateContainer::GetMousePos));
}

void QtHostBridge::TriggerMouseDown(int button, int x, int y)
{
	_pImpl->_mh->TriggerMouseDown(button, x, y);
}

void QtHostBridge::TriggerMouseUp(int button, int x, int y)
{
	_pImpl->_mh->TriggerMouseUp(button, x, y);
}

void QtHostBridge::TriggerKeyDown(int vKey)
{
	_pImpl->_mh->TriggerKeyDown(vKey);
}

void QtHostBridge::TriggerKeyUp(int vKey)
{
	_pImpl->_mh->TriggerKeyUp(vKey);
}

void QtHostBridge::TriggerSizeChanged(int w, int h)
{
	_pImpl->_mh->TriggerSizeChanged(w, h);
}

void QtHostBridge::TriggerIdle()
{
	_pImpl->_mh->TriggerIdle();
}

void QtHostBridge::SetTeapotColor(int color)
{
	_pImpl->_mh->SetTeapotColor(color);	
}



// IMyInterface wrapper implementation
public ref class MyInterfaceBridge : public Fusee::Engine::IMyInterface
{
protected:
	FuseeQtHostBridge::IMyInterface *_pIUnManaged;
public:
	MyInterfaceBridge(FuseeQtHostBridge::IMyInterface *pUnManaged)
	{
		_pIUnManaged = pUnManaged;
	}

	virtual int InterfaceMethod(double d)
	{
		return _pIUnManaged->InterfaceMethod(d);
	};
};


struct AParamClass::AParamClassInternal
{
	gcroot<Fusee::Engine::AParamClass^> _mh;

};

AParamClass::AParamClass()
{
	_pImpl = new AParamClass::AParamClassInternal;
	_pImpl->_mh = gcnew Fusee::Engine::AParamClass();
}

void AParamClass::DoSomething()
{
	_pImpl->_mh->DoSomething();
}

void AParamClass::DoSomethingElse()
{
	_pImpl->_mh->DoSomethingElse();
}

int AParamClass::GetI()
{
	return _pImpl->_mh->GetI();
}



// Implementation classes for the interfaces defined in LibWrapper.h
struct MyClass::MyClassInternal
{
	MyClassInternal(Fusee::Engine::MyClass^ mh)
	{
		_mh = mh;
	}
	gcroot<Fusee::Engine::MyClass^> _mh;
};

MyClass::MyClass()
{
	_pImpl = new MyClass::MyClassInternal(gcnew Fusee::Engine::MyClass());
}

MyClass::MyClass(MyClassInternal *pImpl)
{
	_pImpl = pImpl;
}

MyClass::~MyClass()
{
	delete _pImpl;
	_pImpl = NULL;
}


// Various Methods
int MyClass::SimpleMethod(double d)
{
	return _pImpl->_mh->SimpleMethod(d);
}

int MyClass::InterfaceMethod(double d)
{
	return _pImpl->_mh->InterfaceMethod(d);
}

int MyClass::StringMethod(const char *str)
{
	// The reverse function Marshal.StringToHGlobalAuto()
	String^ strN = Marshal::PtrToStringAnsi((IntPtr)const_cast<char *>(str));
	return _pImpl->_mh->StringMethod(strN);
}

double MyClass::BlittableMethod(MyBlittableStruct s)
{
	/* Won't work because GCHandle::FromIntPtr requires a pointer to
	   something allocated using the GCHandle::Alloc
	IntPtr pi = (IntPtr)(&s);
	GCHandle gch = GCHandle::FromIntPtr(pi);
	Fusee::Engine::MyBlittableStruct mbs = (Fusee::Engine::MyBlittableStruct)gch.Target;
	return _pImpl->_mh->BlittableMethod(mbs);*/
	return 0;
}

double MyClass::StructMethod(MyParamStruct s)
{
	/*
	IntPtr pi = (IntPtr)(&s);
	Object^ o = Marshal::PtrToStructure(pi, Fusee::Engine::MyParamStruct::typeid);
	Fusee::Engine::MyParamStruct mps = (Fusee::Engine::MyParamStruct)o;
	*/
	Fusee::Engine::MyParamStruct mps = (Fusee::Engine::MyParamStruct)Marshal::PtrToStructure((IntPtr)(&s), Fusee::Engine::MyParamStruct::typeid);
	return _pImpl->_mh->StructMethod(mps);
}


int MyClass::ClassMethod(AParamClass *c)
{
	return _pImpl->_mh->ClassMethod(c->_pImpl->_mh);
}


// Delegate
void MyClass::SetDelegate(FuseeQtHostBridge::MyDelegateType pfnMyDelegateType)
{
	_pImpl->_mh->SetDelegate((Fusee::Engine::MyDelegateType ^)
		Marshal::GetDelegateForFunctionPointer(
		(IntPtr)(pfnMyDelegateType), 
		Fusee::Engine::MyDelegateType::typeid));
}

int MyClass::CallDelegate(double d)
{
	return _pImpl->_mh->CallDelegate(d);
}

// Field
void MyClass::set_I(int i)
{
	_pImpl->_mh->I = i;
}
int MyClass::get_I()
{
	return _pImpl->_mh->I;
}






struct MyCaller::MyCallerInternal
{
	gcroot<Fusee::Engine::MyCaller^> _mh;
};

MyCaller::MyCaller()
{
	_pImpl = new MyCaller::MyCallerInternal;
	_pImpl->_mh = gcnew Fusee::Engine::MyCaller();
}

MyCaller::~MyCaller()
{
	delete _pImpl;
	_pImpl = NULL;
}

int MyCaller::InvokeCall(IMyInterface *pMI, double d)
{
	// _assert(_pImpl);
	return _pImpl->_mh->InvokeCall(gcnew MyInterfaceBridge(pMI), d);
}


void CheckVersion()
{
	Assembly^ asmbly = Assembly::LoadFrom("CSharpLib.dll");
	array<Object^> ^aOb = asmbly->GetCustomAttributes(AssemblyFileVersionAttribute::typeid, true);
	String ^strAssemblyVersion = aOb[0]->ToString();
	if (strAssemblyVersion != "1.0.0.0")
	{
		String ^str = "Version Mismatch: " + asmbly->GetName() + ") <> CSharpLibWrapper (1.0.0.0)";
		/*
		array<wchar_t> ^msg = str->ToCharArray();
		::MessageBoxW(NULL, msg->, NULL, MB_OK);
		exit(-1);
		*/
	}
}


#ifdef _MANAGED
#pragma managed(push, off)
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
					  DWORD  ul_reason_for_call,
					  LPVOID lpReserved
					  )
{
	return TRUE;
}

#ifdef _MANAGED
#pragma managed(pop)
#endif















/*
// MyClass wrapper implementation
IMPLEMENT_CLASS(MyClass, Fusee::Engine::MyClass)

int MyClass::SimpleMethod(double d)
{IMPLEMENT_METHODCALL_R(int, SimpleMethod, ARG_IMPLICIT(double, d));}

int MyClass::InterfaceMethod(double d)
{IMPLEMENT_METHODCALL_R(int, InterfaceMethod, ARG_IMPLICIT(double, d));}

void MyClass::SetDelegate(MyDelegateType pfnMyDelegateType)
{IMPLEMENT_METHODCALL_V(SetDelegate, ARG_DELEGATE(Fusee::Engine::MyDelegateType, pfnMyDelegateType));}

int MyClass::CallDelegate(double d)
{IMPLEMENT_METHODCALL_R(int, CallDelegate, ARG_IMPLICIT(double, d));}

void MyClass::set_I(int i)
{IMPLEMENT_SET_PROPERTY(I, ARG_IMPLICIT(int, i));}

int MyClass::get_I()
{IMPLEMENT_GET_PROPERTY(int, I);}
*/

