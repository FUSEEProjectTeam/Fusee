////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// This file controls the swig wrapping process creating C# classes and methods from the Cinema4D API classes 
// and functions
//
// Note that everything that should be wrapped must be included in some 
// %include "file.h"; 
// statement. READ IT TWICE: THERE'S A PERCENT SIGN AT THE BEGINNING AND A SEMICOLON AT THE END.
// "Normal" #include statements either lead to including those files into the generated cpp bridge 
// file or are included to be processed by the swig compiler itself (without wrapping its contents).
// Consult the swig documentation to fully understand what's happening here.
//
// Wrapping funcionality from C4D header files is done in a levelled approach. 
// 
// 1. The first attempt is
//    to simply %include; the entire file and have its entire contents wrapped. If this works, everything
//    is fine. Sometimes some additional help must be given to swig. For example it might be useful to
//    %ignore individual classes or it might be necessary to add some information, declare certain macros etc.
// 
// 2. In some cases the original C4D header file cannot be processed by swig at all. Then the file is
//    copied to the Swig project, renamed to <original-filename>.swig.h and the necessary changes are done
//    to the file. Note that this might break whenever the C4D API changes (e.g. with a new C4D version). 
//    Thus it's still recommended to make as few changes as possible to the *.swig.h file. Try to concentrate 
//    on the necessary changes and control as much as possible from this .i file.
//
// 3. Sometimes it's even necessary to add C++ functionality (code) to the API. For simply adding functions this can be 
//    done using the %extend command in this .i file. For adding entire classes it is recommended to create those
//    classes within the "Native" project's C4dApiExtensions folder. With respect to possible changes in the original
//    C4D API, it is also recommended here to keep the dependency on the C4D API as samll as possible.
//
//  For a handfull of types some extra hand coding is done here. This happens whenever an appropriate C# type already
//  exists (and must thus not be created and used by Swig). Examples are the C#-standard string type (mapped from the 
//  Maxon-C++-API-String type), or the Math.La C#- Matrix and Vector classes (mapped from the respective Maxon-C++ classes).
//  Mapping a C++-API type to an existing C# type can be done using so called typemaps - a concept which has some learning 
//  curve, especially with the poor documentation given (at least try to grasp the general typemap syntax from the docs).
//  To keep an overview what typemap code results in which generated code, we're using the convention to add comments to 
//  the generated code containing a string structured like: <C++-Type>_<Typemap-Item>. See for example the 
//  String_ctype comment in the typemap entry for the (Maxon-C++-)String class for the ctype typemap item. You can then 
//  look at the generated code, search for the "String_ctype" comment and track back which typemap entry generated it.
//  Make sure to keep those comments AFTER the code - otherwise Swig might get in trouble in certain situations.
//

////////////////////////////////////////////////////////////////////////////////////
// The module definition. "directors" (= two-way wrapper classes) are allowed and the overall module name is C4dApi
%module (directors="1") C4dApi


////////////////////////////////////////////////////////////////////////////////////
// This code gets verbatim copied into the generated C++ bridge file (C4dApiWrapper.cpp). Header files included here
// with the "normal" #include are not wrapped (yet), although most of them will appear in some later %include; section.
%{
/* Includes the header in the wrapper code */
#include "c4d.h"
#include "lib_ca.h"
#include "lib_description.h"
#include "c4d_file.h"
#include "c4d_graphview.h"
#include "c4d_operatordata.h"
#include "c4d_operatorplugin.h"
#include "c4d_filterdata.h"
#include "c4d_filterplugin.h"
#include "operatingsystem.h"
#include "c4d_basetag.h"
#include "c4d_baseselect.h" //NEU
#include "c4d_basebitmap.h"
#include "c4d_nodedata.h"
#include "gvdynamic.h"
#include "gvobject.h"
#include "gvmath.h"
#include "ObjectDataM.h"
#include "c4d_customdatatype.h"//neu
#include "customgui_inexclude.h"//neu

// POD (plain old datatype = no construcors or methods) version of C4D's Vector.
// We need this type as return values for swig-generated C++ stubs. If we use
// the original UDT (user defined type = constructors and methods) Vector type we'll get 
// P/Invoke errors complaining that P/Invoke cannot find the C++ stub.
// Make sure to keep this type bit-compatible with the original Vector type.
struct Vector_POD
{
	double x, y, z;
};

// POD (plain old datatype = no construcors or methods) version of C4D's Matrix.
// See comment above.
// Make sure to keep this type bit-compatible with the original Matrix type.
struct Matrix_POD
{
	Vector_POD off, v1, v2, v3;
};

struct Vector32_POD
{
	float x, y, z;
};

%}

////////////////////////////////////////////////////////////////////////////////////
// DeleteMemPtr can be called from generated cs code to free 
// pointers allocated by C4D as return arrays. This is used in CreatePhongNormals
//
// <void*-IntPtr mapping>
%typemap(cstype, out="IntPtr /* void*_cstype */") void *memPtr "IntPtr /* void*_cstype */"
%typemap(csin) void *memPtr
  "new HandleRef(null,$csinput) /* void*_csin */"
%inline %{
void DeleteMemPtr(void *memPtr) {
	DeleteMem(memPtr);
}
%}
// </void*-IntPtr mapping>


////////////////////////////////////////////////////////////////////////////////////
// The following code is about the C#-Std string mapping to the Cinema4D String classes
//
// <String-string mapping>
%naturalvar String;

class String;

%typemap(ctype) String "char * /* String_ctype */"
%typemap(imtype) String "string /* String_imtype */"
%typemap(cstype) String "string /* String_cstype */"

%typemap(csdirectorin) String "$iminput /* String_csdirectorin */"
%typemap(csdirectorout) String "$cscall /* String_csdirectorout */"

%typemap(in, canthrow=1) String 
%{ /* <String_in> */
   if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null string", 0);
    return $null;
   }
   $1 = $input; 
   /* </String_in> */ %}
%typemap(out) String 
%{ /* <String_out> */ 
   $result = SWIG_csharp_string_callback($1.GetCStringCopy()); 
   /* </String_out> */%}

%typemap(directorout, canthrow=1) String 
%{ /* <String_directorout> */
   if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null string", 0);
    return $null;
   }
   $result = $input; 
   /* </String_directorout> */ %}

%typemap(directorin) String 
%{ /* <String_directorin> */
   $input = SWIG_csharp_string_callback($1.GetCStringCopy()); 
   /* </String_directorin> */ %}

%typemap(csin) String "$csinput /* String_csin */"
%typemap(csout, excode=SWIGEXCODE) String 
%{ {  /* <String_csout> */
      string ret = $imcall;$excode
      return ret;
   } /* </String_csout> */ %}

%typemap(typecheck) String = char *;

%typemap(throws, canthrow=1) String
%{ /* <String_throws> */
   SWIG_CSharpSetPendingException(SWIG_CSharpApplicationException, $1.GetCStringCopy());
   return $null; 
   /* </String_throws> */ %}

// const String &
%typemap(ctype) const String & "char * /* constString&_ctype */"
%typemap(imtype) const String & "string /* constString&_imtype */"
%typemap(cstype) const String & "string /* constString&_cstype */"

%typemap(csdirectorin) const String & "$iminput /* constString&_csdirectorin */"
%typemap(csdirectorout) const String & "$cscall /* constString&_csdirectorout */"

%typemap(in, canthrow=1) const String &
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null string", 0);
    return $null;
   }
   String $1_str($input);
   $1 = &$1_str; %}
%typemap(out) const String & %{ $result = SWIG_csharp_string_callback($1->GetCStringCopy()); %}

%typemap(csin) const String & "$csinput"
%typemap(csout, excode=SWIGEXCODE) const String & {
    string ret = $imcall;$excode
    return ret;
  }

%typemap(directorout, canthrow=1, warning=SWIGWARN_TYPEMAP_THREAD_UNSAFE_MSG) const String &
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null string", 0);
    return $null;
   }
   /* possible thread/reentrant code problem */
   static String $1_str;
   $1_str = $input;
   $result = &$1_str; %}

%typemap(directorin) const String & %{ $input = SWIG_csharp_string_callback($1.GetCStringCopy()); %}

%typemap(csvarin, excode=SWIGEXCODE2) const String & %{
    set {
      $imcall;$excode
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) const String & %{
    get {
      string ret = $imcall;$excode
      return ret;
    } %}

%typemap(typecheck) const String & = char *;

%typemap(throws, canthrow=1) const String &
%{ SWIG_CSharpSetPendingException(SWIG_CSharpApplicationException, $1.GetCStringCopy());
   return $null; %}
// </String-string mapping>



////////////////////////////////////////////////////////////////////////////////////
// Some simple instructions to handle C4d-defined simple types as builtin C++ types
//%apply int   { Int32 }
//%apply bool   { Bool }
//%apply double { Real }
//%apply double { LReal }
//%apply float { SReal }

%apply bool { Bool }

%apply char { maxon::Char }
%apply unsigned char { maxon::UChar }
%apply short { maxon::Int16 }
%apply unsigned short { maxon::UInt16 }
%apply int { maxon::Int32 }
%apply unsigned int { maxon::UInt32 }
%apply long long { maxon::Int64 }
%apply unsigned long long { maxon::UInt64 }
%apply int { maxon::Int }
%apply unsigned int { maxon::UInt }
%apply float { maxon::Float32 }
%apply double { maxon::Float }
%apply double { maxon::Float64 }

%apply char { Char }
%apply unsigned char { UChar }
%apply short { Int16 }
%apply unsigned short { UInt16 }
%apply int { Int32 }
%apply unsigned int { UInt32 }
%apply long long { Int64 }
%apply unsigned long long { UInt64 }
%apply int { Int }
%apply unsigned int { UInt }
%apply float { Float32 }
%apply double { Float }
%apply double { Float64 }



////////////////////////////////////////////////////////////////////////////////////
// This code is added to allow polymorphic downcast using the wrapped classes.
// We need them if we e.g. traverse the scene graph and only find instances of 
// BaseObject. To really use those objects we need to ask them what they really are
// (e.g. a Cube) and cast them to the respective concrete type. To wrap the concrete
// types with concrete wrapper instances the following code is necessary
//
// <polymorphic-downcasts>

// for BaseTag derivatives tags
%pragma(csharp) imclasscode=%{
  public static BaseTag InstantiateConcreteTag(IntPtr cPtr, bool owner)
  {
    BaseTag ret = null;
    if (cPtr == IntPtr.Zero) 
	{
      return ret;
    }
    int type = $modulePINVOKE.C4DAtom_GetType(new HandleRef(null, cPtr));
    switch (type) 
	{
       case 0:
         ret = new BaseTag(cPtr, owner);
         break;
       case 1001149:
         ret = new XPressoTag(cPtr, owner);
         break;
	   case 5616: // Ttexture
		 ret = new TextureTag(cPtr, owner);
		 break;
	   case 5671: // Tuvw
		 ret = new UVWTag(cPtr, owner);
		 break;
	   case 5673: // Tpolygonselection  WARNING!!! There is is only ONE class (SelectionTag) for three type IDs (Point, Edge, and Poly). C4D programmers, you are real men...
		 ret = new SelectionTag(cPtr, owner);
		 break;
      // Repeat for every other concrete type.
      default:
	  //changed from the debug output to return a BaseTag object
        ret = new BaseTag(cPtr, owner);
        break;
    }
    return ret;
  }
%}

%typemap(csout, excode=SWIGEXCODE)
BaseTag *
/* Insert here every other abstract type returned in the C++ API */
{
    IntPtr cPtr = $imcall;
    $csclassname ret = ($csclassname) $modulePINVOKE.InstantiateConcreteTag(cPtr, $owner);$excode
    return ret;
}

// for BaseObject derivatives
%pragma(csharp) imclasscode=%{
  public static BaseObject InstantiateConcreteObject(IntPtr cPtr, bool owner)
  {
    BaseObject ret = null;
    if (cPtr == IntPtr.Zero) 
	{
      return ret;
    }
    int type = $modulePINVOKE.C4DAtom_GetType(new HandleRef(null, cPtr));
    switch (type) 
	{
       case 0:
         ret = new BaseObject(cPtr, owner);
         break;
	  case 5100: // Opolygon
		 ret = new PolygonObject(cPtr, owner);
		 break;
      // Repeat for every other concrete type.
      default:
	  //changed from the debug output to return a BaseTag object
        ret = new BaseObject(cPtr, owner);
        break;
    }
    return ret;
  }
%}

%typemap(csout, excode=SWIGEXCODE)
BaseObject *
/* Insert here every other abstract type returned in the C++ API */
{
    IntPtr cPtr = $imcall;
    $csclassname ret = ($csclassname) $modulePINVOKE.InstantiateConcreteObject(cPtr, $owner);$excode
    return ret;
}

// for BaseMaterial derivatives
%pragma(csharp) imclasscode=%{
  public static BaseMaterial InstantiateConcreteMaterial(IntPtr cPtr, bool owner)
  {
    BaseMaterial ret = null;
    if (cPtr == IntPtr.Zero) 
	{
      return ret;
    }
    int type = $modulePINVOKE.C4DAtom_GetType(new HandleRef(null, cPtr));
    switch (type) 
	{
       case 0:
         ret = new BaseMaterial(cPtr, owner);
         break;
	  case 5703: // Mmaterial
		 ret = new Material(cPtr, owner);
		 break;
      // Repeat for every other concrete type.
      default:
	  //changed from the debug output to return a BaseTag object
        ret = new BaseMaterial(cPtr, owner);
        break;
    }
    return ret;
  }
%}

%typemap(csout, excode=SWIGEXCODE)
BaseMaterial *
/* Insert here every other abstract type returned in the C++ API */
{
    IntPtr cPtr = $imcall;
    $csclassname ret = ($csclassname) $modulePINVOKE.InstantiateConcreteMaterial(cPtr, $owner);$excode
    return ret;
}


// </polymorphic-downcasts>


//////////////////////////////////////////////////////////////////////////// 
//
// This code is taken from 
// http://old.nabble.com/C--delegates-and-function-pointers-td21456631.html
// it should allow mapping C++ function pointers to C# delegates as callbacks
// (from C++ code to C# code)
//
// <map-functionpointers-to-delegates>
//
// cs_callback is used to marshall callbacks. It allows a C# function to 
// be passed to C++ as a function pointer through P/Invoke, which has the 
// ability to make unmanaged-to-managed thunks. It does NOT allow you to 
// pass C++ function pointers to C#. 
// 
// I would have liked to support FastDelegate<...> as the C++ argument 
// type; this would have involved the cs_callback2 macro... but it turns 
// out not to work under default project settings because .NET functions 
// use the __stdcall calling convention, but FastDelegate uses the default 
// convention which tends to be something else (__fastcall?). So nevermind. 
// 
// Anyway, to use this macro you need to declare the function pointer type 
// TYPE in the appropriate header file (including the calling convention), 
// declare a delegate named after CSTYPE in your C# project, and use this 
// macro in your .i file. Here is an example: 
// 
// in C++ header file (%include this header in your .i file): 
// typedef void (__stdcall *Callback)(PCWSTR); 
// void Foo(Callback c); 
// 
// in C# code: 
// public delegate void CppCallback([MarshalAs(UnmanagedType.LPWStr)] string message); 
// 
// in your .i file: 
// %cs_callback(Callback, CppCallback) 
// 
// Remember to invoke %cs_callback before any code involving Callback. 
%define %cs_callback(TYPE, CSTYPE) 
        %typemap(ctype) TYPE, TYPE& "void*" 
        %typemap(in) TYPE  %{ $1 = (TYPE)$input; %} 
        %typemap(in) TYPE& %{ $1 = (TYPE*)&$input; %} 
        %typemap(imtype, out="IntPtr") TYPE, TYPE& "CSTYPE" 
        %typemap(cstype, out="IntPtr") TYPE, TYPE& "CSTYPE" 
        %typemap(csin) TYPE, TYPE& "$csinput" 
%enddef 
%define %cs_callback2(TYPE, CTYPE, CSTYPE) 
        %typemap(ctype) TYPE "CTYPE" 
        %typemap(in) TYPE %{ $1 = (TYPE)$input; %} 
        %typemap(imtype, out="IntPtr") TYPE "CSTYPE" 
        %typemap(cstype, out="IntPtr") TYPE "CSTYPE" 
        %typemap(csin) TYPE "$csinput" 
%enddef 
// </map-functionpointers-to-delegates>


//////////////////////////////////////////////////////////////////////////// 
//
// This code is taken from 
// http://www.nickdarnell.com/2011/05/swig-and-a-miss/
// and should make SWIG use the C# ref and out keywords accordingly
//
// use 
// %TypeOutParam(MyType) or
// %TypeRefParam(MyType)
// for each C++ type that you want to convert like:
// C++:
// void MyFunc(MyType &param)
// to
// C#:
// void MyFunc([out|ref] MyType param)
//
// <map C++-&-to-C#-ref-or-out>

// </map C++-&-to-C#-ref-or-out>


//////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////
// List of include files to generate wrapper classes from. 
// Make sure to ignore everything not needed. 
// Insert director directives and addtional wrapper
// code right in front of the respective file to keep things
// belonging together near to each others. If a file is unprocessable
// by swig, copy it to the Swig project and rename it to *.swig.h
// typical statments used here: %ignore, %extend, %inline

// Global ignores:
%ignore CalcFaceNormal;
%ignore ENUM_END_FLAGS;
%ignore ENUM_END_LIST;
%ignore INSTANCEOF;

//////////////////////////////////////////////////////////////////
// ge_math.h (and child includes) won't be wrapped - instead we'll 
// directly map vector and matrix type(s) to Fusee.Math structures
// %include "ge_vector.h";
// %include "ge_lvector.h";
// %include "ge_matrix.h";
// %include "ge_lmatrix.h";
// %ignore __NET;
// %ignore __SERVER;
// %ignore __CLIENT;
// %ignore __BODYPAINT;
// %ignore __PARALLEL;
// %ignore __UPDATER;
// %ignore __INSTALLER;
// %ignore __LICENSESERVER;
// %ignore __MEDIZINI;
// %include "ge_math.h";

// Map Vector* and &   TO   ref Fusee.Math.double3
%typemap(cstype, out="$csclassname") Vector *, Vector & "ref Fusee.Math.double3 /* Vector*&_cstype */"
%typemap(csin) Vector *, Vector & "ref $csinput /* Vector*&_csin */"
%typemap(imtype, out="IntPtr") Vector *, Vector & "ref Fusee.Math.double3 /* Vector*&_imtype */"
%typemap(in) Vector *, Vector & "$1 = ($1_ltype)$input; /* Vector*&_in */"
%typemap(csdirectorin, 
   pre="    Fusee.Math.double3 vec_$iminput;\n"
       "    unsafe {vec_$iminput = Fusee.Math.ArrayConvert.ArrayDoubleTodouble3((double *)$iminput);}\n"
       "    /* Vector*&_csdirectorin_pre */", 
   post="        unsafe {Fusee.Math.ArrayConvert.double3ToArrayDouble(vec_$iminput, (double *)$iminput);}\n"
        "        /* Vector*&_csdirectorin_post */"
  ) Vector *, Vector &
  "ref vec_$iminput /* Vector*&_csdirectorin */"
%typemap(csdirectorout) Vector *, Vector & "$cscall /* Vector*&_csdirectorout */"

// Map const Vector &  TO  Fusee.Math.double3
%typemap(cstype, out="Fusee.Math.double3 /* constVector&_cstype_out */") const Vector & "Fusee.Math.double3 /* constVector&_cstype */"
%typemap(csin) const Vector & "ref $csinput /* constVector&_csin */"
%typemap(imtype, out="IntPtr /* constVector&_imtype_out */") const Vector & "ref Fusee.Math.double3 /* constVector&_imtype */"
%typemap(csout, excode=SWIGEXCODE) const Vector &
%{ {  /* <constVector&_csout> */
      IntPtr p_ret = $imcall;$excode
      Fusee.Math.double3 ret;
      unsafe {ret = Fusee.Math.ArrayConvert.ArrayDoubleTodouble3((double *)p_ret);}
      return ret;
   } /* </constVector&_csout> */ %}
%typemap(csdirectorin, 
   pre="    Fusee.Math.double3 vec_$iminput;\n"
       "    unsafe {vec_$iminput = Fusee.Math.ArrayConvert.ArrayDoubleTodouble3((double *)$iminput);}\n"
       "    /* constVector&_csdirectorin_pre */", 
   post="        /* no re-conversion because of const declaration */\n"
        "        /* constVector&_csdirectorin_post */"
  ) const Vector &
  "vec_$iminput /* Vector*&_csdirectorin */"


// Map Vector   TO   Fusee.Math.double3
%typemap(cstype, out="Fusee.Math.double3 /* Vector_cstype_out */") Vector "Fusee.Math.double3 /* Vector_cstype */"
%typemap(csout, excode=SWIGEXCODE) Vector 
%{ {  /* <Vector_csout> */
      Fusee.Math.double3 ret = $imcall;$excode
      return ret;
   } /* <Vector_csout> */ %}
%typemap(imtype, out="Fusee.Math.double3 /* Vector_imtype_out */") Vector "Fusee.Math.double3 /* Vector_imtype */"
%typemap(ctype, out="Vector_POD /* Vector_ctype_out */") Vector "Vector /* Vector_ctype */"
%typemap(directorout) Vector
%{ /* <Vector_directorout> */
   $result = *((Vector *)&($input)); 
   /* </Vector_directorout> */
 %}
%typemap(directorin) Vector 
%{ /* <Vector_directorin> */
   $input = *((Vector_POD *)&($1)); 
   /* </Vector_directorin> */ 
%}
%typemap(out) Vector 
%{
	/* <Vector_out> */
	$result = *((Vector_POD *)&($1));
	/* </Vector_out> */
%}
%typemap(in) Vector 
%{
	/* <Vector_in> */
	$1 = *((Vector *)&($input));
	/* </Vector_in> */
%}
%typemap(csin) Vector "$csinput /* Vector_csin */"
%typemap(csdirectorin, 
   pre="/* NOP Vector_csdirectorin_pre */"
  ) Vector
  "$iminput /* Vector_csdirectorin */"
%typemap(csdirectorout) Vector "$cscall /* Vector_csdirectorout */"
%typemap(csvarin) Vector %{
    /* <Vector_csvarin> */
    set 
	{
      $imcall;$excode
    }  /* </Vector_csvarin> */  %}
%typemap(csvarout) Vector %{ 
   /* <Vector_csvarout> */
   get
   {  
      Fusee.Math.double3 ret = $imcall;$excode
      return ret;
   } /* <Vector_csvarout> */ %}


// Map Matrix* and &   TO   ref Fusee.Math.double4x4
%typemap(cstype, out="$csclassname") Matrix *, Matrix & "ref Fusee.Math.double4x4 /* Matrix*&_cstype */"
%typemap(csin, 
   pre="    double[] adbl_$csinput;\n"
       "    unsafe {adbl_$csinput = Fusee.Math.ArrayConvert.double4x4ToArrayDoubleC4DLayout($csinput);"
       "    fixed (double *pdbl_$csinput = adbl_$csinput) {\n"
       "    /* Matrix*&_csin_pre */", 
   post="        $csinput = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4(pdbl_$csinput);\n"
        "        /* Matrix*&_csin_post */",
   terminator="} } /* Matrix*&_csin_terminator */"
  ) Matrix *, Matrix &
  "(IntPtr) pdbl_$csinput /* Matrix*&_csin */"
%typemap(imtype, out="IntPtr") Matrix *, Matrix & "IntPtr /* Matrix*&_imtype */"
%typemap(in) Matrix *, Matrix & "$1 = ($1_ltype)$input; /* Matrix*&_in */"
%typemap(csdirectorin, 
   pre="    Fusee.Math.double4x4 mtx_$iminput;\n"
       "    unsafe {mtx_$iminput = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4((double *)$iminput);}\n"
       "    /* Matrix*&_csdirectorin_pre */", 
   post="        unsafe {Fusee.Math.ArrayConvert.double4x4ToArrayDoubleC4DLayout(mtx_$iminput, (double *)$iminput);}\n"
        "        /* Matrix*&_csdirectorin_post */"
  ) Matrix *, Matrix &
  "ref mtx_$iminput /* Matrix*&_csdirectorin */"
%typemap(csdirectorout) Matrix *, Matrix & "$cscall /* Matrix*&_csdirectorout */"

// Map Matrix   TO   Fusee.Math.double4x4
%typemap(ctype) Matrix "Matrix_POD /* Matrix_ctype */"
%typemap(out) Matrix 
%{ /* <Matrix_out> */ 
   $result = *((Matrix_POD *)(&$1)); 
   /* </Matrix_out> */%}
%typemap(in) Matrix 
%{ /* <Matrix_in> */ 
   $1 = *((Matrix *)(&$input)); 
   /* </Matrix_in> */%}

%typemap(cstype) Matrix "Fusee.Math.double4x4 /* Matrix_cstype */"
%typemap(csin, 
   pre="    double[] adbl_$csinput;\n"
       "    unsafe {adbl_$csinput = Fusee.Math.ArrayConvert.double4x4ToArrayDoubleC4DLayout($csinput);"
       "    fixed (double *pdbl_$csinput = adbl_$csinput) {\n"
       "    /* Matrix_csin_pre */", 
   terminator="} } /* Matrix_csin_terminator */"
  ) Matrix
  "(IntPtr) pdbl_$csinput /*  Matrix_csin */"
%typemap(csout, excode=SWIGEXCODE) Matrix 
%{ {  /* <Matrix_csout> */
      C34M ret_c34m = $imcall;$excode
	  Fusee.Math.double4x4 ret;
	  unsafe {ret = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4(ret_c34m.m);}
      return ret;
   } /* </Matrix_csout> */ %}
%typemap(imtype, out="C34M /* Matrix_imtype_out */") Matrix "IntPtr /* Matrix_imtype */"
// %typemap(in) Matrix "$1 = ($1_ltype)$input; /* Matrix_in */"
%typemap(csdirectorin, 
   pre="    Fusee.Math.double4x4 mtx_$iminput;\n"
       "    unsafe {mtx_$iminput = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4((double *)$iminput);}\n"
       "    /* Matrix_csdirectorin_pre */"
  ) Matrix
  "mtx_$iminput /* Matrix_csdirectorin */"
%typemap(csdirectorout) Matrix "$cscall /* Matrix_csdirectorout */"
%typemap(csvarin) Matrix %{
    /* <Matrix_csvarin> */
    set 
	{
       double[] adbl_$csinput;
       unsafe 
	   {
		   adbl_$csinput = Fusee.Math.ArrayConvert.double4x4ToArrayDoubleC4DLayout($csinput);
           fixed (double *pdbl_$csinput = adbl_$csinput) 
		   {
              $imcall;$excode
		   }
	   }
    }  /* </Matrix_csvarin> */  %}
%typemap(csvarout) Matrix %{ 
   /* <Matrix_csvarout> */
   get
   {  
      C34M ret_c34m = $imcall;$excode
	  Fusee.Math.double4x4 ret;
	  unsafe {ret = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4(ret_c34m.m);}
      return ret;   
   } /* <Matrix_csvarout> */ %}

// Map  const Matrix&  TO   Fusee.Math. (currently only if occurring as return value)
%typemap(cstype, out="Fusee.Math.double4x4 /* constMatrix&_cstype_out */") const Matrix& "ref Fusee.Math.double4x4 /* constMatrix&_cstype */"
%typemap(csin, 
   pre="    double[] adbl_$csinput;\n"
       "    unsafe {adbl_$csinput = Fusee.Math.ArrayConvert.double4x4ToArrayDoubleC4DLayout($csinput);"
       "    fixed (double *pdbl_$csinput = adbl_$csinput) {\n"
       "    /* constMatrix&_csin_pre */", 
   post="        // NOP $csinput = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4(pdbl_$csinput);\n"
        "        /* constMatrix&_csin_post */",
   terminator="} } /* constMatrix&_csin_terminator */"
  ) const Matrix&
  "(IntPtr) pdbl_$csinput /* constMatrix&_csin */"
%typemap(csout, excode=SWIGEXCODE) const Matrix&
%{ {  /* <constMatrix&_csout> */
      IntPtr p_ret = $imcall;$excode
      Fusee.Math.double4x4 ret;
      unsafe {ret = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4((double *)p_ret);}
      return ret;
   } /* </constMatrix&_csout> */ %}
%typemap(imtype, out="IntPtr /* constMatrix&_imtype_out */") const Matrix& "IntPtr /* constMatrix&_imtype */"
%typemap(in) const Matrix& "$1 = ($1_ltype)$input; /* constMatrix&_in */"
%typemap(csdirectorin, 
   pre="    Fusee.Math.double4x4 mtx_$iminput;\n"
       "    unsafe {mtx_$iminput = Fusee.Math.ArrayConvert.ArrayDoubleC4DLayoutTodouble4x4((double *)$iminput);}\n"
       "    /* constMatrix&_csdirectorin_pre */", 
   post="        unsafe {Fusee.Math.ArrayConvert.double4x4ToArrayDoubleC4DLayout(mtx_$iminput, (double *)$iminput);}\n"
        "        /* constMatrix&_csdirectorin_post */"
  ) const Matrix&
  "ref mtx_$iminput /* constMatrix&_csdirectorin */"
%typemap(csdirectorout) const Matrix& "$cscall /* constMatrix&_csdirectorout */"


// implement this as a function - a #define won't be wrapped
%inline %{
	inline void blDelete_cs(GeListNode *v) { if (v) C4DOS.Bl->Free(v); }
%}


//////////////////////////////////////////////////////////////////
// ge_prepass.h
%rename (SERIALINFO_ENUM) SERIALINFO;
%include "ge_prepass.swig.h";


//////////////////////////////////////////////////////////////////
// c4d_general.h
%include "c4d_general.h";

//////////////////////////////////////////////////////////////////
// c4d_thread.h
%include "c4d_thread.h";

//////////////////////////////////////////////////////////////////
// c4d_file.h
%include "c4d_file.h";

//////////////////////////////////////////////////////////////////
// lib_description.h
%ignore DescriptionLib;
%include "lib_description.swig.h";


//////////////////////////////////////////////////////////////////
//NEW
%extend DescID
{
	const DescLevel &GetAt(Int32 pos) const { return (*self)[pos]; }
}

//////////////////////////////////////////////////////////////////
// c4d_basebitmap.h
%include "c4d_basebitmap.swig.h";
%extend BaseBitmap
{
	static BaseBitmap *AutoBitmap(const String &str)
	{
		BaseBitmap *bmp=BaseBitmap::Alloc();
		if (!bmp) 
			return NULL;
				
		char *debug = str.GetCStringCopy();

		if (bmp->Init(str)!=IMAGERESULT_OK)
		{
			BaseBitmap::Free(bmp);
			return NULL;
		}
		return bmp;
	}

	static BaseBitmap *AutoBitmap(Int32 id)
	{
		BaseBitmap *bmp = InitResourceBitmap(id);
		return bmp;
	}
};

//////////////////////////////////////////////////////////////////
// c4d_gedata.h
%include "c4d_gedata.swig.h"

//////////////////////////////////////////////////////////////////
// c4d_nodedata.h
%ignore AutoBitmap;

// %cs_callback(DataAllocator, NodeDataAllocator)
// %define %cs_callback(TYPE, CSTYPE) 
%typemap(ctype) DataAllocator*, DataAllocator*& "void* /* DataAllocator*_ctype */" 
%typemap(in) DataAllocator*  %{ $1 = (DataAllocator*)$input; /* DataAllocator*_in */%} 
%typemap(in) DataAllocator*& %{ $1 = (DataAllocator**)&$input;  /* DataAllocator*&_in */%} 
%typemap(imtype, out="IntPtr") DataAllocator*, DataAllocator*& "NodeDataAllocator /* DataAllocator*_imtype */" 
%typemap(cstype, out="IntPtr") DataAllocator*, DataAllocator*& "NodeDataAllocator /* DataAllocator*_cstype */" 
%typemap(csin) DataAllocator*, DataAllocator*& "$csinput /* DataAllocator*_csin */" 
// %enddef 

%include "c4d_nodedata.h";

//////////////////////////////////////////////////////////////////
// obase.h
%include "obase.h";


//////////////////////////////////////////////////////////////////
// "c4d_baselist.h"
%include "c4d_baselist.h";
%extend C4DAtom
{
	virtual Int64 RefUID()
	{
		return (Int64)self;
	}
}


//////////////////////////////////////////////////////////////////
// c4d_baseobject.h
%extend PointObject
{
	Vector GetPointAt(Int32 inx)
	{
		return self->GetPointR()[inx];
	}
	void SetPointAt(Int32 inx, Vector v)
	{
		self->GetPointW()[inx] = v;
	}
	
}
%extend PolygonObject
{
	Vector GetPointAt(Int32 inx)
	{
		return self->GetPointR()[inx];
	}
	void SetPointAt(Int32 inx, Vector v)
	{
		self->GetPointW()[inx] = v;
	}
	CPolygon GetPolygonAt(Int32 inx)
	{
		return self->GetPolygonR()[inx];
	}
	void SetPolygonAt(Int32 inx, CPolygon v)
	{
		self->GetPolygonW()[inx] = v;
	}
}
%extend SplineObject
{
	Tangent *GetTangentAt(Int32 inx)
	{
		return (Tangent *)(self->GetTangentR() + inx);
	}
	void SetTangentAt(Int32 inx, Tangent *pT)
	{
		self->GetTangentW()[inx] = *pT;
	}
	Segment *GetSegmentAt(Int32 inx)
	{
		return (Segment *)(self->GetSegmentR() + inx);
	}
	void SetSegmentAt(Int32 inx, Segment *pT)
	{
		self->GetSegmentW()[inx] = *pT;
	}
}
%extend PointObject
{
 static PointObject* GetPointObject(BaseObject* iObj){
  return (PointObject*)iObj;
 }
}
%typemap(cstype, out="Fusee.Math.float3[] /* Vector32*PolygonObject::CreatePhongNormals_cstype */") Vector32 *PolygonObject::CreatePhongNormals "Fusee.Math.float3[] /* Vector32*PolygonObject::CreatePhongNormals_cstype */"
%typemap(out) Vector32 *PolygonObject::CreatePhongNormals
%{ /* <Vector32*PolygonObject::CreatePhongNormals_out> */ 
   $result = *((Vector32_POD **)(&$1)); 
   /* </Vector32*PolygonObject::CreatePhongNormals_out> */%}
%typemap(csout, excode=SWIGEXCODE) Vector32 *PolygonObject::CreatePhongNormals
%{ {  /* <Vector32*PolygonObject::CreatePhongNormals_csout> */
      IntPtr p_ret = $imcall;$excode
	  if (p_ret == IntPtr.Zero)
	      return null;
	  int nNormals = this.GetPolygonCount()*4;
      Fusee.Math.float3[] ret = new Fusee.Math.float3[nNormals];
      unsafe
	  {
	      for (int i = 0; i < nNormals; i++)
		  {
			  ret[i] = Fusee.Math.ArrayConvert.ArrayFloatTofloat3(((float *)(p_ret))+3*i);
	      }
	  }
	  C4dApi.DeleteMemPtr(p_ret);
      return ret;
   } /* </Vector32*PolygonObject::CreatePhongNormals_csout> */ %}


%include "c4d_baseobject.h";

//////////////////////////////////////////////////////////////////
// "c4d_basecontainer.h"
%include "c4d_basecontainer.h";

//////////////////////////////////////////////////////////////////
// "c4d_basedraw.h"
%include "c4d_basedraw.h";

//////////////////////////////////////////////////////////////////
// "c4d_basedocument.h"
%include "c4d_basedocument.h";

//////////////////////////////////////////////////////////////////
// "c4d_basetag.h"
%include "c4d_basetag.h";

///////////////////////////////////////////////
// "c4d_baseselect.h"
%extend BaseSelect
{
	Int32 GetRangeA(Int32 seg)
	{
		Int32 ret, b;
		if (self->GetRange(seg, 2147483647, &ret, &b))
			return ret;
		return -1;
	}
	Int32 GetRangeB(Int32 seg)
	{
		Int32 a, ret;
		if (self->GetRange(seg, 2147483647, &a, &ret))
			return ret;
		return -1;
	}
};

%include "c4d_baseselect.h";

///////////////////////////////////////////////
// "c4d_basematerial.h"
%include "c4d_basematerial.h";


///////////////////////////////////////////////
// "c4d_basechannel.h"
%include "c4d_basechannel.h";

///////////////////////////////////////////////
// "c4d_shader.h"
%ignore Multipass;
%include "c4d_shader.h";

//////////////////////////////////////////////////////////////////
// "c4d_commanddata.h"
%feature("director") CommandData;
%csmethodmodifiers CommandData::CommandData "private";
%typemap(cscode) CommandData %{
  public CommandData(bool memOwn) : this(C4dApiPINVOKE.new_CommandData(), memOwn) {
    SwigDirectorConnect();
  }
%}
%include "c4d_commanddata.h";

//////////////////////////////////////////////////////////////////
// lib_ca.h
%feature("director") BrushToolData;
%nodefaultctor BrushToolData;

%ignore m_pPoints;
%ignore m_pGlobalPoints;
%ignore m_pNormals;
%ignore m_pPolys;
%extend BrushObjectInfo
{
	// Replacement for m_pPoints
	Vector GetPointAt(Int32 inx)
	{
		return self->m_pPoints[inx];
	}
	// No setter because m_pPoints is const Vector*

	// Replacement for m_pGlobalPoints
	Vector GetGlobalPointAt(Int32 inx)
	{
		return self->m_pGlobalPoints[inx];
	}
	void SetGlobalPointAt(Int32 inx, Vector v)
	{
		self->m_pGlobalPoints[inx] = v;
	}

	// Replacement for m_pNormals
	Vector GetNormalAt(Int32 inx)
	{
		return self->m_pNormals[inx];
	}
	void SetNormalAt(Int32 inx, Vector v)
	{
		self->m_pNormals[inx] = v;
	}

	// Replacement for m_pPolys
	CPolygon GetPolyAt(Int32 inx)
	{
		return self->m_pPolys[inx];
	}
	// No Setter because m_pPolys is const CPoly*
}
%feature("director") BaseTag;
%include "lib_ca.swig.h";


//////////////////////////////////////////////////////////////////
// "c4d_objectdata.h" (replaced by own implementation)
%feature("director") ObjectDataM;
%csmethodmodifiers ObjectDataM::ObjectDataM "private";
%typemap(cscode) ObjectDataM %{
  public ObjectDataM(bool memOwn) : this(C4dApiPINVOKE.new_ObjectDataM(), memOwn) {
    SwigDirectorConnect();
  }
%}
%include "c4d_objectdata.h" // for keeping inheritance ObjectDataM -> ObjectData
%include "ObjectDataM.h";


//////////////////////////////////////////////////////////////////
// gvdynamic.h
// This is used for xpresso node calculation data constants etc
%include "gvdynamic.h"

//////////////////////////////////////////////////////////////////
// gvobject.h
// This is used to retrieve a scene object from xpresso nodes etc
%include "gvobject.h"

//////////////////////////////////////////////////////////////////
// gvmath.h
// used to determine which type of math function should be used
%include "gvmath.h"

//////////////////////////////////////////////////////////////////
// "c4d_graphview_def.h"
%ignore C4D_GraphView;
%ignore C4D_GvGlobals;
%ignore C4D_GvInit;
%ignore C4D_GvNode;
%ignore C4D_GvNodeGUI;
%ignore C4D_GvNodeMaster;
%ignore C4D_GvPort;
%ignore C4D_GvPortList;
%ignore C4D_GvQuery;
%ignore C4D_GvRun;
%ignore C4D_GvValue;
%ignore C4D_GvWorld;
%ignore GV_DATA_HANDLER;
%ignore GV_OPCLASS_HANDLER;
%ignore GV_OPGROUP_HANDLER;
%ignore GV_VALGROUP_HANDLER;
%ignore GV_VALUE_HANDLER;
%include "c4d_graphview_def.h";

//////////////////////////////////////////////////////////////////
// "c4d_graphview_enum.h"
%include "c4d_graphview_enum.h";

//////////////////////////////////////////////////////////////////
// "c4d_graphview.h"
%inline %{
	// Maxon forgot to implement this. It's probably never used, but since it is declared, Swig wants to generate a wrapper for it
	inline Bool GvBuildValuesTable(GvNode *bn, GvValue **&in_ports, Int32 &nr_of_in_ports, GvPort **&out_ports, Int32 &nr_of_out_ports)
	{
		return FALSE;	
	}
%}

//////////////////////////////////////////////////////////////////
// "GvPort changes"
%typemap(cstype) GvPort *& "ref GvPort /* GvPort_cstype */"

%typemap(csin, 
   pre="    IntPtr p_$csinput;\n"
       "    unsafe { void *pp_$csinput = &p_$csinput;"
       "    /* GvPort_csin_pre */", 
   post="        $csinput = new GvPort(p_$csinput, false);\n"
        "        /* GvPort_csin_post */",
   terminator="} /* GvPort_csin_terminator */"
  ) GvPort *&
  "(IntPtr) pp_$csinput /* GvPort_csin */"

%typemap(imtype) GvPort *& "IntPtr /* GvPort_imtype */"

//////////////////////////////////////////////////////////////////
// "GvNode changes"
%typemap(cstype) GvNode *& "ref GvNode /* GvNode_cstype */"

%typemap(csin, 
   pre="    IntPtr p_$csinput;\n"
       "    unsafe { void *pp_$csinput = &p_$csinput;"
       "    /* GvNode_csin_pre */", 
   post="        $csinput = new GvNode(p_$csinput, false);\n"
        "        /* GvNode_csin_post */",
   terminator="} /* GvNode_csin_terminator */"
  ) GvNode *&
  "(IntPtr) pp_$csinput /* GvNode_csin */"

%typemap(imtype) GvNode *& "IntPtr /* GvNode_imtype */"

%include "c4d_graphview.swig.h";
// %include "c4d_graphview.h";

//////////////////////////////////////////////////////////////////
// "c4d_operatordata.h"
%feature("director") OperatorData;
%csmethodmodifiers OperatorData::OperatorData "private";
%typemap(cscode) OperatorData %{
  public OperatorData(bool memOwn) : this(C4dApiPINVOKE.new_OperatorData(), memOwn) {
    SwigDirectorConnect();
  }
%}%include "c4d_operatordata.h";

//////////////////////////////////////////////////////////////////
// "c4d_operatorplugin.h"
%include "c4d_operatorplugin.h";

//////////////////////////////////////////////////////////////////
// "c4d_filterdata.h"
%feature("director") SceneLoaderData;
%csmethodmodifiers SceneLoaderData::SceneLoaderData "private";
%typemap(cscode) SceneLoaderData %{
  public SceneLoaderData(bool memOwn) : this(C4dApiPINVOKE.new_SceneLoaderData(), memOwn) {
    SwigDirectorConnect();
  }
%}
%feature("director") SceneSaverData;
%csmethodmodifiers SceneSaverData::SceneSaverData "private";
%typemap(cscode) SceneSaverData %{
  public SceneSaverData(bool memOwn) : this(C4dApiPINVOKE.new_SceneSaverData(), memOwn) {
    SwigDirectorConnect();
  }
%}
%feature("director") BitmapLoaderData;
%csmethodmodifiers BitmapLoaderData::BitmapLoaderData "private";
%typemap(cscode) BitmapLoaderData %{
  public BitmapLoaderData(bool memOwn) : this(C4dApiPINVOKE.new_BitmapLoaderData(), memOwn) {
    SwigDirectorConnect();
  }
%}
%feature("director") BitmapSaverData;
%csmethodmodifiers BitmapSaverData::BitmapSaverData "private";
%typemap(cscode) BitmapSaverData %{
  public BitmapSaverData(bool memOwn) : this(C4dApiPINVOKE.new_BitmapSaverData(), memOwn) {
    SwigDirectorConnect();
  }
%}
%include "c4d_filterdata.h";

//////////////////////////////////////////////////////////////////
// "c4d_filterplugin.h"
%include "c4d_filterplugin.h";


//////////////////////////////////////////////////////////////////
//added 23062011 by DS
//%include "c4d_customdatatype.h";
//%include "ge_prepass.h";

//////////////////////////////////////////////////////////////////
// Res file includes (for constants)
// NOTE: These files are typically found under $(C4D)/resource/res/description.
// These constants are most likely not found by a find-in-files on the c4dsdk.
// Try to guess the dialog name of the res/h file and look into it to make sure
// the data you need is included
%include "osplineprimitive.h";
%include "ospline.h";
%include "ttexture.h";
%include "mmaterial.h";

//////////////////////////////////////////////////////////////////
//operatingsystem.h
%include "operatingsystem.swig.h";
%extend ModelingCommandData {

	 ModelingCommandData(	BaseDocument* doc=NULL,	BaseObject* op=NULL,BaseContainer* bc=NULL,
		MODELINGCOMMANDMODE mode=MODELINGCOMMANDMODE_ALL,
		MODELINGCOMMANDFLAGS flags=MODELINGCOMMANDFLAGS_0,
		AtomArray* result=NULL,
		AtomArray* arr=NULL)
		{

	//~ModelingCommandData();

		return 0;
	
		 }
}

//////////////////////////////////////////////////////////////////
//ddoc.h
%include "ddoc.h";

//////////////////////////////////////////////////////////////////
///NEU
//"custom_CUSTOMDATATYPE"

%include "c4d_customdatatype.swig.h";

%template(iCustomDataType_inex) iCustomDataType<DescID>;

// {
//	  static X* Alloc() { return gNew X; }
//	  static void Free(X* &data) { gDelete (data); }
//  };
//////////////////////////////////////////////////////////////////
///NEU
//"custom_INEXCLUDE"

%include "customgui_inexclude.swig.h";

////////////////////////////
//lib_selectionchanger.h
//%include "lib_selectionchanger.swig.h";

//////////////////////////////////////////////////////////////////
// Hand coded declarations. TODO: replace them by including the appropriate header files

// From c4d_tools.h
#define MAXRANGE	     1.0e20			// maximum value for metric data
#define MAXELEMENTS	   100000000	// maximum number of points
#define MIN_EPSILON    0.001			// epsilon value

// From somewhere
#define Texpresso												1001149
void MessageDialog(const String &str);
Bool QuestionDialog(const String& str);
void GePrint(const String &str);
BaseContainer GetCustomDataTypeDefault(Int32 type);
