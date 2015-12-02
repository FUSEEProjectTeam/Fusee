#ifndef _CPPINTEROPDEC_H_
#define _CPPINTEROPDEC_H_

// This file declares a set of sophisticated macros which can be used
// to declare wrapper C++ classes, interfaces and function pointers
// wrapping c# classes, interfaces and delegate types.

#define DECLARE_CLASSBYREF(_CLASS_NAME_)			\
private:											\
struct _CLASS_NAME_##Internal;						\
	_CLASS_NAME_##Internal* _pImpl;					\
public:												\
	_CLASS_NAME_();									\
protected:
	

#endif
