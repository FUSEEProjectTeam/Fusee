%module (directors="1") CppApi
%{
/* Includes the header in the wrapper code */
#include "Parent.h"
#include "Child.h"
#include "Factory.h"
#include "VectorConsumer.h"
#include "RefRefTest.h"
%}

#define CPPAPI_API

%include "typemaps.i"


%pragma(csharp) imclasscode=%{
  public static Parent InstantiateConcreteObject(IntPtr cPtr, bool owner)
  {
    Parent ret = null;
    if (cPtr == IntPtr.Zero) 
	{
      return ret;
    }
    int type = $modulePINVOKE.Parent_WhatAmI(new HandleRef(null, cPtr));
    switch (type) 
	{
       case 0:
         ret = new Parent(cPtr, owner);
         break;
       case 1:
         ret = new Child(cPtr, owner);
         break;
      // Repeat for every other concrete type.
      default:
        System.Diagnostics.Debug.Assert(false,
        String.Format("Encountered type '{0}' that is not any known concrete class",
            type.ToString()));
        break;
    }
    return ret;
  }
%}

%typemap(csout, excode=SWIGEXCODE)
  Parent *
    /* Insert here every other abstract type returned in the C++ API */   {
    IntPtr cPtr = $imcall;
    $csclassname ret = ($csclassname) $modulePINVOKE.InstantiateConcreteObject(cPtr, $owner);$excode
    return ret;
}

%feature("director") Parent;
%include "Parent.h";
%include "Child.h";
%include "Factory.h";


//////////////////////////
//  "VectorConsumer.h"
%feature("director") VectorConsumer;

// Map CVector3* and &   TO   ref Fusee.Math.Core.Vector3D
%typemap(cstype, out="$csclassname") CVector3 *, CVector3 & "ref Fusee.Math.Core.Vector3D /* cstype */"
%typemap(csin) CVector3 *, CVector3 & "ref $csinput /* csin */"
%typemap(imtype, out="IntPtr") CVector3 *, CVector3 & "ref Fusee.Math.Core.Vector3D /* imtype */"
%typemap(in) CVector3 *, CVector3 & "$1 = ($1_ltype)$input; /* in */"
%typemap(csdirectorin, 
   pre="    Fusee.Math.Core.Vector3D vec_$iminput;\n"
       "    unsafe {vec_$iminput = Fusee.Math.ArrayConversion.Convert.ArrayDoubleToVector3D((double *)$iminput);}\n"
       "    /* csdirectorin_pre */", 
   post="        unsafe {Fusee.Math.ArrayConversion.Convert.Vector3DToArrayDouble(vec_$iminput, (double *)$iminput);}\n"
        "        /* csdirectorin_post */"
  ) CVector3 *, CVector3 &
  "ref vec_$iminput /* csdirectorin */"
%typemap(csdirectorout) CVector3 *, CVector3 & "$cscall /* csdirectorout */"


// Map CVector3   TO   Fusee.Math.Core.Vector3D 
// Working for CVector3 Parameters with Directors
// NOT Working for CVector3 return values with directors
%typemap(cstype, out="$csclassname") CVector3 "Fusee.Math.Core.Vector3D /* cstype */"
%typemap(csin) CVector3 "ref $csinput /* csin */"
%typemap(imtype, out="IntPtr") CVector3 "ref Fusee.Math.Core.Vector3D /* imtype */"
// %typemap(in) CVector3 "$1 = ($1_ltype)$input; /* in */"
%typemap(csdirectorin, 
   pre="    Fusee.Math.Core.Vector3D vec_$iminput;\n"
       "    unsafe {vec_$iminput = Fusee.Math.ArrayConversion.Convert.ArrayDoubleToVector3D((double *)$iminput);}\n"
       "    /* csdirectorin_pre */"
  ) CVector3
  "vec_$iminput /* csdirectorin */"
%typemap(csdirectorout) CVector3 "$cscall /* csdirectorout */"




// Map CVector3   TO   Fusee.Math.Core.Vector3D
// As currently used in ManagedPlugin. Compiles for return types but crashes when run
// %typemap(ctype) CVector3 "CVector3 /* CVector3_ctype */"
// %typemap(out) CVector3 
// %{ /* <CVector3_out> */ 
//    $result = *((CVector3 *)(&$1)); 
//    /* </CVector3_out> */%}
%typemap(cstype) CVector3 "Fusee.Math.Core.Vector3D /* CVector3_cstype */"
%typemap(csin) CVector3 "ref $csinput /* CVector3_csin */"
%typemap(csout, excode=SWIGEXCODE) CVector3 
%{ {  /* <CVector3_csout> */
      Fusee.Math.Core.Vector3D ret = $imcall;$excode
      return ret;
   } /* <CVector3_csout> */ %}
%typemap(imtype, out="Fusee.Math.Core.Vector3D /* CVector3_imtype_out */") CVector3 "ref Fusee.Math.Core.Vector3D /* CVector3_imtype */"
%typemap(csvarin) CVector3 %{
    /* <CVector3_csvarin> */
    set 
	{
      $imcall;$excode
    }  /* </CVector3_csvarin> */  %}
%typemap(csvarout) CVector3 %{ 
   /* <CVector3_csvarout> */
   get
   {  
      Fusee.Math.Core.Vector3D ret = $imcall;$excode
      return ret;
   } /* <CVector3_csvarout> */ %}
// %typemap(in) CVector3 "$1 = ($1_ltype)$input; /* CVector3_in */"
// %typemap(directorin) CVector3 
// %{
// 	/* <CVector3_directorin> */
// 	$input = $1;
// 	/* </CVector3_directorin> */
// % }
%typemap(csdirectorin, 
   pre="/* NOP CVector3_csdirectorin_pre */"
  ) CVector3
  "$iminput /* CVector3_csdirectorin */"
%typemap(csdirectorout) CVector3 "$cscall /* CVector3_csdirectorout */"










// Map CVector4* and &   TO   ref Fusee.Math.Core.Vector4D
%typemap(cstype, out="$csclassname") CVector4 *, CVector4 & "ref Fusee.Math.Core.Vector4D /* cstype */"
%typemap(csin) CVector4 *, CVector4 & "ref $csinput /* csin */"
%typemap(imtype, out="IntPtr") CVector4 *, CVector4 & "ref Fusee.Math.Core.Vector4D /* imtype */"
%typemap(in) CVector4 *, CVector4 & "$1 = ($1_ltype)$input; /* in */"
%typemap(csdirectorin, 
   pre="    Fusee.Math.Core.Vector4D vec_$iminput;\n"
       "    unsafe {vec_$iminput = Fusee.Math.ArrayConversion.Convert.ArrayDoubleToVector4D((double *)$iminput);}\n"
       "    /* csdirectorin_pre */", 
   post="        unsafe {Fusee.Math.ArrayConversion.Convert.Vector4DToArrayDouble(vec_$iminput, (double *)$iminput);}\n"
        "        /* csdirectorin_post */"
  ) CVector4 *, CVector4 &
  "ref vec_$iminput /* csdirectorin */"
%typemap(csdirectorout) CVector4 *, CVector4 & "$cscall /* csdirectorout */"

// Map CVector4   TO   Fusee.Math.Core.Vector4D
%typemap(cstype, out="$csclassname") CVector4 "Fusee.Math.Core.Vector4D /* cstype */"
%typemap(csin) CVector4 "ref $csinput /* csin */"
%typemap(imtype, out="IntPtr") CVector4 "ref Fusee.Math.Core.Vector4D /* imtype */"
// %typemap(in) CVector4 "$1 = ($1_ltype)$input; /* in */"
%typemap(csdirectorin, 
   pre="    Fusee.Math.Core.Vector4D vec_$iminput;\n"
       "    unsafe {vec_$iminput = Fusee.Math.ArrayConversion.Convert.ArrayDoubleToVector4D((double *)$iminput);}\n"
       "    /* csdirectorin_pre */"
  ) CVector4
  "vec_$iminput /* csdirectorin */"
%typemap(csdirectorout) CVector4 "$cscall /* csdirectorout */"

// Map CMatrix34* and &   TO   ref Fusee.Math.Core.Matrix4D
%typemap(cstype, out="$csclassname") CMatrix34 *, CMatrix34 & "ref Fusee.Math.Core.Matrix4D /* cstype */"
%typemap(csin, 
   pre="    double[] adbl_$csinput;\n"
       "    unsafe {adbl_$csinput = Fusee.Math.ArrayConversion.Convert.Matrix4DToArrayDoubleC4DLayout($csinput);"
       "    fixed (double *pdbl_$csinput = adbl_$csinput) {\n"
       "    /* csin_pre */", 
   post="        $csinput = Fusee.Math.ArrayConversion.Convert.ArrayDoubleC4DLayoutToMatrix4D(pdbl_$csinput);\n"
        "        /* csin_post */",
   terminator="} } /* csin_terminator */"
  ) CMatrix34 *, CMatrix34 &
  "(IntPtr) pdbl_$csinput /* csin */"
%typemap(imtype, out="IntPtr") CMatrix34 *, CMatrix34 & "IntPtr /* imtype */"
%typemap(in) CMatrix34 *, CMatrix34 & "$1 = ($1_ltype)$input; /* in */"
%typemap(csdirectorin, 
   pre="    Fusee.Math.Core.Matrix4D mtx_$iminput;\n"
       "    unsafe {mtx_$iminput = Fusee.Math.ArrayConversion.Convert.ArrayDoubleC4DLayoutToMatrix4D((double *)$iminput);}\n"
       "    /* csdirectorin_pre */", 
   post="        unsafe {Fusee.Math.ArrayConversion.Convert.Matrix4DToArrayDoubleC4DLayout(mtx_$iminput, (double *)$iminput);}\n"
        "        /* csdirectorin_post */"
  ) CMatrix34 *, CMatrix34 &
  "ref mtx_$iminput /* csdirectorin */"
%typemap(csdirectorout) CMatrix34 *, CMatrix34 & "$cscall /* csdirectorout */"

// Map CMatrix34   TO   Fusee.Math.Core.Matrix4D
%typemap(ctype, out="CMatrix34 /* CMatrix34_ctype_out */") CMatrix34 "void * /* CMatrix34_ctype */"
%typemap(out) CMatrix34 
%{ /* <CMatrix34_out> */ 
   $result = $1; 
   /* </CMatrix34_out> */%}
%typemap(csout, excode=SWIGEXCODE) CMatrix34 
%{ {  /* <CMatrix34_csout> */
      C34M ret_c34m = $imcall;$excode
	  Fusee.Math.Core.Matrix4D ret;
	  unsafe {ret = Fusee.Math.ArrayConversion.Convert.ArrayDoubleC4DLayoutToMatrix4D(ret_c34m.m);}
      return ret;
   } /* </CMatrix34_csout> */ %}
// %typemap(cstype, out="$csclassname") CMatrix34 "Fusee.Math.Core.Matrix4D /* CMatrix34_cstype */"
%typemap(cstype) CMatrix34 "Fusee.Math.Core.Matrix4D /* CMatrix34_cstype */"
%typemap(csin, 
   pre="    double[] adbl_$csinput;\n"
       "    unsafe {adbl_$csinput = Fusee.Math.ArrayConversion.Convert.Matrix4DToArrayDoubleC4DLayout($csinput);"
       "    fixed (double *pdbl_$csinput = adbl_$csinput) {\n"
       "    /* csin_pre */", 
   terminator="} } /* csin_terminator */"
  ) CMatrix34
  "(IntPtr) pdbl_$csinput /* csin */"
// %typemap(imtype, out="IntPtr") CMatrix34 "IntPtr /* imtype */"
%typemap(imtype, out="C34M /*CMatrix34_imtype_out */") CMatrix34 "IntPtr /* CMatrix34_imtype */"
// %typemap(in) CMatrix34 "$1 = ($1_ltype)$input; /* in */"
%typemap(csdirectorin, 
   pre="    Fusee.Math.Core.Matrix4D mtx_$iminput;\n"
       "    unsafe {mtx_$iminput = Fusee.Math.ArrayConversion.Convert.ArrayDoubleC4DLayoutToMatrix4D((double *)$iminput);}\n"
       "    /* csdirectorin_pre */"
  ) CMatrix34
  "mtx_$iminput /* csdirectorin */"
%typemap(csdirectorout) CMatrix34 "$cscall /* csdirectorout */"



%include "VectorConsumer.h";




//////////////////////////
//  "RefRefTest.h"
%typemap(cstype) AParamType *& "ref AParamType /* cstype */"
%typemap(csin, 
   pre="    IntPtr p_$csinput;\n"
       "    unsafe { void *pp_$csinput = &p_$csinput;"
       "    /* csin_pre */", 
   post="        $csinput = new AParamType(p_$csinput, false);\n"
        "        /* csin_post */",
   terminator="} /* csin_terminator */"
  ) AParamType *&
  "(IntPtr) pp_$csinput /* csin */"
%typemap(imtype) AParamType *& "IntPtr /* imtype */"

%include "RefRefTest.h";

