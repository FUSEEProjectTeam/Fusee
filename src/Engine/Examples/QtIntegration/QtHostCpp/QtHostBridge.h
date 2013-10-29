#ifndef _QTHOST_H_
#define _QTHOST_H_

// Unmanaged projects can include this header file to use
// functionality provided by the CSharpLib
// This file contains unmanaged wrappers, boxes and adaptors
// to access the C# Lib functionality.

// This Wrapper file dll-exports all classes and their methods
// and uses the Pimpl/Cheshire Cat pattern to hide the managed
// part of the implementation from the header. Thus the header,
// together with the import-lib created by this dll can be used 
// from unmanaged C++ projects.

#ifdef QTHOST_EXPORTS
#define QTHOST_API __declspec(dllexport)
#else
#define QTHOST_API __declspec(dllimport)
#endif

#include "CPPInteropDec.h"
// #include "qwidget.h"

namespace FuseeQtHostBridge
{
	class QTHOST_API QtHostBridge
	{
	public:
		typedef int (*MousePosQueryFunc)();
	protected:
	    struct QtHostBridgeInternal;
		QtHostBridgeInternal *_pImpl;

	public:
		// Application agnostic
		QtHostBridge(void *hwnd);
		void SetMousePosQuery(MousePosQueryFunc pFunc);
		void TriggerMouseDown(int button, int x, int y);
		void TriggerMouseUp(int button, int x, int y);
		void TriggerKeyDown(int vKey);
		void TriggerKeyUp(int vKey);
		void TriggerSizeChanged(int w, int h);
		void TriggerIdle();

		// Application specific
		void SetTeapotColor(int color);
	};




	// Parameter structure declaration
	struct MyParamStruct
	{
		int x;
		int y;
		int z;
		char *str;
	};

	struct MyBlittableStruct
	{
		int x;
		double y;
		char z;
	};

	// Delegate type declaration
	typedef int (*MyDelegateType)(double d);

	// Interface declaration
	class IMyInterface
	{
	public:
		virtual int InterfaceMethod(double d)	= 0;
	};

	// Class declaration
	class QTHOST_API MyCaller
	{
	protected:
		struct MyCallerInternal;
		MyCallerInternal *_pImpl;
	public:
		MyCaller();
		virtual ~MyCaller();
		virtual int InvokeCall(IMyInterface *pMI, double d);
	};

	class QTHOST_API AParamClass
	{
	public:
		struct AParamClassInternal;
		AParamClassInternal *_pImpl;
		AParamClass();
		void DoSomething();
		void DoSomethingElse();
		int GetI();
	};


	// MyClass declaration
	class QTHOST_API MyClass : public IMyInterface
	{
	public:
		struct MyClassInternal;
		MyClassInternal *_pImpl;
		MyClass(MyClassInternal *pImpl);
		MyClass();
		virtual ~MyClass();
		// Various Methods
		int SimpleMethod(double d);
		virtual int InterfaceMethod(double d);
		int StringMethod(const char *str);
		double StructMethod(MyParamStruct s);
		double BlittableMethod(MyBlittableStruct s);
		int ClassMethod(AParamClass *c);

		// Delegate
		void SetDelegate(MyDelegateType pfnMyDelegateType);
		int CallDelegate(double d);

		// Field
		void set_I(int i);
		int get_I();
	};



}


#endif
