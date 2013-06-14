#ifndef _CPPINTEROPIMP_H_
#define _CPPINTEROPIMP_H_
// This file is to be used by developers implementing a wrapper layer
// enabling interoperability between components implemented in pure 
// .Net (e.g. C#) and unmanaged C++ (abbreviated by uC++ in the following)
//
// Why do we neet this? With IJW ("It just works"), /CLR, .Net-Interop, 
// COM-Visibility etc. aren't there lots of ways to accomplish right this?
//
// Using this file and the method descibed here doesn't invent another method to 
// make C# and uC++ work together.
// In fact it hooks on existing interoperability functionality. It just tries
// to encapsulate the interoperability part. It is meant as a "best practice" way
// and comes with a couple of C++-preprocessor macros to save hand coding.
// It is especially useful if one or more of the following constraints are given:
// o C# components should be used from uC++ applications. Not the other way round.
// o C# developers shouldn't worry too much about how their components will be used
//   from uC++. The design of these components cannot or should not be altered to 
//   expose functionality to uC++. 
// o .Net Assemblies without source code need be accessible from uC++
// o uC++ developers shouldn't bother with any of the Interop mechanisms 
//   mentioned above in order to access the functionality exposed by C#
//   components. Accessing C# functionality should look like native unmanaged
//   C++ code.
// o uC++ code should stay free from managed extensions (e.g. for maintaining 
//   platform / compiler independance)
// o uC++ code must not or cannot be compiled using the /clr flag.
// o Using COM is not wanted. Neither on the C# side nor on the uC++ side.
//
// In order to keep things simple, only restricted use of classes, interfaces 
// and delegates are supported by interop layers implemented using this method. 
// The following describes what's possible:
//
// *Classes*
// Classes can only be used in the following way:
// o Implemented as .Net classes.
// o NOT exposing public data members. (Data should be exposed by 
//   Properties/getters/setters
// o uC++ code should be able to 
//		- create (new) and delete instances
//		- Call methods on the instances (trigger execution 
//        of managed code).
// o in uC++, instances of derived types will not be passed
//   back to .Net.
// 
// *Interfaces*
// Interfaces can only be used in the following way
// o Declared as .Net interfaces (or classes with virtual methods)
// o uC++ code should be able to implement / override methods
// o uC++ code should be able to pass inherited / implementing instances
//   of an interface to .Net
//
// *Delegates*
// Delegates can only be used in the following way
// o Used as parameters to Class methods.
// o Triggered by .Net code.
// o Implemented as unbound (static or global) functions in uC++
//
// *Don't expect too much!*
// WARNING: The method and header files described here represent some kind of 
// "art of the possible". It just uses macros to reduce repeating and 
// error prone hand-coding. Still, there are things to do by hand introducing
// the danger of inconsitencies. Especially if something changes in the C#
// library without the wrapper library being changed can lead to unexpected
// behavior.
//
// 
// *How to write a wrapper dll*
// o Identify all classes, interfaces and delegate types within
//   the C# component / .Net assembly to be used from uC++ code. 
//   Make sure they meet the requirements on how they should be 
//   used mentioned above.
// o Create a new VC8 C++ Win32 DLL project. (NO MFC, NO ATL!)
// o In the project's properties, on the General tab enable
//   "Common language runtime support /clr".
// o In the project's properties, on the "Common Properties->References"
//   tab add the C# project / .Net assembly you want to wrap to
//   the wrapper dll's references.
// o Add a header file that will contain declarations of 
//   uC++ classes, interfaces and delegate types (called the wrapper 
//   dll header). This file will be included in uC++ projects and 
//   thus MUST NOT contain any managed extensions (like ref, 
//   gcnew, ^, etc.).
// o #include CPPInteropDec.h in the header file
// o Declare a wrapper type for all classes, interfaces and delegates
//   existing in the C# component that need to be exposed to uC++.
//   See the paragraphs below how to do that for the different types.
// o Make sure that the project's cpp file will be compiled with the
//   /clr flag. The cpp file will contain managed code fragments. 
//   #include the wrapper dll header and CPPInteropImp.h to the cpp file.
// o   

#ifndef _MANAGED
#error "This file must be compiled with the /clr flag set"
#endif

#include <vcclr.h>
using namespace System::Runtime::InteropServices;

// Class definition
#define IMPL_CLASS(_CLASS_NAME_, _CS_CLASS_NAME_)			\
struct _CLASS_NAME_::_CLASS_NAME_##Internal					\
{															\
	gcroot<_CS_CLASS_NAME_^> _mref;							\
};															\
_CLASS_NAME_::_CLASS_NAME_()								\
{															\
	_pImpl = new _CLASS_NAME_##Internal();					\
}


// Argument marshalling macros
#define ARG_int(_IMPLICITTYPE_, _IMPLICITTVAL_)		\
	_IMPLICITTVAL_

#define ARG_INTERFACE(_INTERFACETYPE_, _INTERFACEIMP_)		\
	if (_INTERFACETYPE_->_INTERFACEIMP_())

#define ARG_DELEGATE(_DELG_TYPE_, _DELG_FUNC_)					\
	(_DELG_TYPE_ ^)Marshal::GetDelegateForFunctionPointer((IntPtr)(pfnMyDelegateType), _DELG_TYPE_::typeid)


#define IMPL_MARSHALER(_CPP_TYPE_, _CS_TYPE_)					\
static void _CS_TYPE_ Marshal##_CPP_TYPE_(_CPP_TYPE_ arg)		\
{

	
}



// Class Method Implementation
#define IMPL_CLASS_METHODCALL0(_RETTYPE_, _CLASS_NAME_, _METHOD_)	\
_RETTYPE_ _CLASS_NAME_::_METHOD_()									\
{																	\
	return (_RETTYPE_)_pImpl->_mref->_METHOD_();					\
}

// Class Method Implementation
#define IMPL_CLASS_METHODCALL1(_RETTYPE_, _CLASS_NAME_, _ARGTYPE0_, _VALUE0_)	\
_RETTYPE_ _CLASS_NAME_::_METHOD_(_ARGTYPE0_ _VALUE0_)							\
{																				\
	return (_RETTYPE_)_pImpl->_mref->_METHOD_(Marshal##_ARGTYPE0_(_VALUE0_));	\
}


#define IMPL_CLASS_GET_PROPERTY(_RETTYPE_, _PROPERTY_)		\
return _pImpl->_mref->_PROPERTY_;

#define IMPL_CLASS_SET_PROPERTY(_PROPERTY_, _ARGUMENT_)		\
_pImpl->_mref->_PROPERTY_ = _ARGUMENT_;


// Interface Implementation
#define BEGIN_IMPL_INTERFACE(_INTERFACE_NAME_, _CS_INTERFACE_NAME_)	\
public ref class _INTERFACE_NAME_##Bridge : public _CS_INTERFACE_NAME_		\
{																			\
private:																	\
	_INTERFACE_NAME_ *_pMI;													\
public:

#define IMPL_INTF_METHOD(_RETVAL_, _METHOD_NAME_, ...)
virtual _RETVAL_ _METHOD_NAME_(__VA_ARGS__)
{
	_pMI->_METHOD_NAME_
}

	
#define END_IMPLEMENT_INTERFACE		\
};


#endif
