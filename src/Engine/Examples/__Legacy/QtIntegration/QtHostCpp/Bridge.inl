//

lxDEC_STRUCT(MyParamStruct)
	lxS_DATA(int, x)
	lxS_DATA(int, y)
	lxS_DATA(int, z)
	lxS_DATA(String, str)
lxEND_STRUCT()


lxDEC_INTERFACE(MyInterface)
	lxI_METHOD_1(int, InterfaceMethod, double, d)
lxEND_INTERFACE()


lxDEC_CLASS(MyClass)
	lxC_IMPLEMENTS(MyInterface)
	lxC_METHOD_1(int, SimpleMethod, double, d)
	lxC_METHOD_1(int, StringMethod, String, str)
	lxC_METHOD_1(int, StructMethod, MyParamStruct, s)
	lxC_METHOD_1(MyParamStruct, ReturnStruct, int, i)
	lxC_METHOD_1(int, InterfaceMethod, double, d)
	lxC_METHOD_1(void, SetDelegate, MyDelegateType, delegateMeth)
	lxC_METHOD_1(int, CallDelegate, double, d)
	lxC_FIELD(int, I)
lxEND_CLASS()


lxDEC_DELEGATE_1(int, MyDelegateType, double, d)