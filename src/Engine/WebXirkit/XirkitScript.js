/*
This file contains the js implementation of Xirkit of methods that cant be translatet from the JSIL Cross Compiler.
This file is written manually. 
*/

var $fuseeMath = JSIL.GetAssembly("Fusee.Math.Core");
var $WebXirkitImp = JSIL.DeclareAssembly("Fusee.Engine.WebXirkit");
var $fuseeXirkit = JSIL.GetAssembly("Fusee.Xirkit");
var $customMsCore = JSIL.GetAssembly("mscorlib");

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Xirkit");

var PinFactory_ConvMap = null;

var PinFactory_GetConvMap = function ()
{
    if (PinFactory_ConvMap === null)
    {
        PinFactory_ConvMap = {};

        // From int
        PinFactory_ConvMap["System.Int32"] = {};
        PinFactory_ConvMap["System.Int32"]["System.Int32"] = "_v_";
        PinFactory_ConvMap["System.Int32"]["System.Single"] = "_v_";
        PinFactory_ConvMap["System.Int32"]["System.Double"] = "_v_";
        PinFactory_ConvMap["System.Int32"]["System.Boolean"] = '(_v_ !== 0) ? true : false';
        PinFactory_ConvMap["System.Int32"]["System.String"] = "_v_.toString()";
        PinFactory_ConvMap["System.Int32"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(_v_, 0)";
        PinFactory_ConvMap["System.Int32"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3(_v_, 0, 0)";
        PinFactory_ConvMap["System.Int32"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4(_v_, 0, 0, 0)";
        PinFactory_ConvMap["System.Int32"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4(_v_, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        PinFactory_ConvMap["System.Int32"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(_v_, 0)";
        PinFactory_ConvMap["System.Int32"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3(_v_, 0, 0)";
        PinFactory_ConvMap["System.Int32"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4(_v_, 0, 0, 0)";
        PinFactory_ConvMap["System.Int32"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4(_v_, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        
        // From float
        PinFactory_ConvMap["System.Single"] = {};
        PinFactory_ConvMap["System.Single"]["System.Int32"] = "parseInt(_v_)";
        PinFactory_ConvMap["System.Single"]["System.Single"] = "_v_";
        PinFactory_ConvMap["System.Single"]["System.Double"] = "_v_";
        PinFactory_ConvMap["System.Single"]["System.Boolean"] = '(_v_ !== 0.0) ? true : false';
        PinFactory_ConvMap["System.Single"]["System.String"] = "_v_.toString()";
        PinFactory_ConvMap["System.Single"]["Fusee.Math.double2"] = "new $fuseeMath.Fusee.Math.double2(_v_, 0)";
        PinFactory_ConvMap["System.Single"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3(_v_, 0, 0)";
        PinFactory_ConvMap["System.Single"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4(_v_, 0, 0, 0)";
        PinFactory_ConvMap["System.Single"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4(_v_, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        PinFactory_ConvMap["System.Single"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(_v_, 0)";
        PinFactory_ConvMap["System.Single"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3(_v_, 0, 0)";
        PinFactory_ConvMap["System.Single"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4(_v_, 0, 0, 0)";
        PinFactory_ConvMap["System.Single"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4(_v_, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        
        // From double
        PinFactory_ConvMap["System.Double"] = {};
        PinFactory_ConvMap["System.Double"]["System.Int32"] = "parseInt(_v_)";
        PinFactory_ConvMap["System.Double"]["System.Single"] = "_v_";
        PinFactory_ConvMap["System.Double"]["System.Double"] = "_v_";
        PinFactory_ConvMap["System.Double"]["System.Boolean"] = '(_v_ !== 0.0) ? true : false';
        PinFactory_ConvMap["System.Double"]["System.String"] = "_v_.toString()";
        PinFactory_ConvMap["System.Double"]["Fusee.Math.double2"] = "new $fuseeMath.Fusee.Math.double2(_v_, 0)";
        PinFactory_ConvMap["System.Double"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3(_v_, 0, 0)";
        PinFactory_ConvMap["System.Double"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4(_v_, 0, 0, 0)";
        PinFactory_ConvMap["System.Double"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4(_v_, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        PinFactory_ConvMap["System.Double"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(_v_, 0)";
        PinFactory_ConvMap["System.Double"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3(_v_, 0, 0)";
        PinFactory_ConvMap["System.Double"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4(_v_, 0, 0, 0)";
        PinFactory_ConvMap["System.Double"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4(_v_, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";

        // From string
        PinFactory_ConvMap["System.String"] = {};
        PinFactory_ConvMap["System.String"]["System.Int32"] = "parseInt(_v_)";
        PinFactory_ConvMap["System.String"]["System.Single"] = "parseFloat(_v_)";
        PinFactory_ConvMap["System.String"]["System.Double"] = "parseFloat(_v_)";
        PinFactory_ConvMap["System.String"]["System.Boolean"] = '(_v_ !== "") ? true : false';
        PinFactory_ConvMap["System.String"]["System.String"] = "_v_";
        PinFactory_ConvMap["System.String"]["Fusee.Math.double2"] = "new $fuseeMath.Fusee.Math.double2(parseFloat(_v_), 0)";
        PinFactory_ConvMap["System.String"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3(parseFloat(_v_), 0, 0)";
        PinFactory_ConvMap["System.String"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4(parseFloat(_v_), 0, 0, 0)";
        PinFactory_ConvMap["System.String"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4(parseFloat(_v_), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        PinFactory_ConvMap["System.String"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(parseFloat(_v_), 0)";
        PinFactory_ConvMap["System.String"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3(parseFloat(_v_), 0, 0)";
        PinFactory_ConvMap["System.String"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4(parseFloat(_v_), 0, 0, 0)";
        PinFactory_ConvMap["System.String"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4(parseFloat(_v_), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";

        // From Boolean
        PinFactory_ConvMap["System.Boolean"] = {};
        PinFactory_ConvMap["System.Boolean"]["System.Int32"] = "(_v_ === true)? 1 : 0";
        PinFactory_ConvMap["System.Boolean"]["System.Single"] = "(_v_ === true)? 1.0 : 0.0";
        PinFactory_ConvMap["System.Boolean"]["System.Double"] = "(_v_ === true)? 1.0 : 0.0";
        PinFactory_ConvMap["System.Boolean"]["System.Boolean"] = "_v_";
        PinFactory_ConvMap["System.Boolean"]["System.String"] = "(_v_ === true)? true:false";
        PinFactory_ConvMap["System.Boolean"]["Fusee.Math.double2"] = "new $fuseeMath.Fusee.Math.double2((_v_ === true)? 1.0 : 0.0, 0)";
        PinFactory_ConvMap["System.Boolean"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3((_v_ === true)? 1.0 : 0.0, 0, 0)";
        PinFactory_ConvMap["System.Boolean"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4((_v_ === true)? 1.0 : 0.0, 0, 0, 1)";
        PinFactory_ConvMap["System.Boolean"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4((_v_ === true)? 1.0 : 0.0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        PinFactory_ConvMap["System.Boolean"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2((_v_ === true)? 1.0 : 0.0, 0)";
        PinFactory_ConvMap["System.Boolean"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3((_v_ === true)? 1.0 : 0.0, 0, 0)";
        PinFactory_ConvMap["System.Boolean"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4((_v_ === true)? 1.0 : 0.0, 0, 0, 1)";
        PinFactory_ConvMap["System.Boolean"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4((_v_ === true)? 1.0 : 0.0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";

        // from float2
        PinFactory_ConvMap["Fusee.Math.float2"] = {};
        PinFactory_ConvMap["Fusee.Math.float2"]["System.Int32"] = "parseInt(_v_.x)";
        PinFactory_ConvMap["Fusee.Math.float2"]["System.Single"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.float2"]["System.Double"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.float2"]["System.Boolean"] = "((_v_.x + _v_.y) > 0)? true : false";
        PinFactory_ConvMap["Fusee.Math.float2"]["System.String"] = "(_v_.x + _v_.y).toString()";
        PinFactory_ConvMap["Fusee.Math.float2"]["Fusee.Math.double2"] = "new $fuseeMath.Fusee.Math.double2(_v_.x, _v_.y)";
        PinFactory_ConvMap["Fusee.Math.float2"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3(_v_.x, _v_.y, 0)";
        PinFactory_ConvMap["Fusee.Math.float2"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4(_v_.x, _v_.y, 0, 0)";
        PinFactory_ConvMap["Fusee.Math.float2"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4(_v_.x, _v_.y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        PinFactory_ConvMap["Fusee.Math.float2"]["Fusee.Math.float2"] = "_v_";
        PinFactory_ConvMap["Fusee.Math.float2"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3(_v_.x, _v_.y, 0)";
        PinFactory_ConvMap["Fusee.Math.float2"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4(_v_.x, _v_.y, 0, 0)";
        PinFactory_ConvMap["Fusee.Math.float2"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4(_v_.x, _v_.y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        
        // from float3
        PinFactory_ConvMap["Fusee.Math.float3"] = {};
        PinFactory_ConvMap["Fusee.Math.float3"]["System.Int32"] = "parseInt(_v_)";
        PinFactory_ConvMap["Fusee.Math.float3"]["System.Single"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.float3"]["System.Double"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.float3"]["System.Boolean"] = "((_v_.x + _v_.y + _v_.z) > 0)? true : false";
        PinFactory_ConvMap["Fusee.Math.float3"]["System.String"] = "(_v_.x + _v_.y + _v_.z).toString()";
        PinFactory_ConvMap["Fusee.Math.float3"]["Fusee.Math.double2"] = "new $fuseeMath.Fusee.Math.double2(_v_.x, _v_.y)";
        PinFactory_ConvMap["Fusee.Math.float3"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3(_v_.x, _v_.y, _v_.z)";
        PinFactory_ConvMap["Fusee.Math.float3"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4(_v_.x, _v_.y, _v_.z, 0)";
        PinFactory_ConvMap["Fusee.Math.float3"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4(_v_.x, _v_.y, _v_.z, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        PinFactory_ConvMap["Fusee.Math.float3"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(_v_.x, _v_.y)";
        PinFactory_ConvMap["Fusee.Math.float3"]["Fusee.Math.float3"] = "_v_"; 
        PinFactory_ConvMap["Fusee.Math.float3"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4(_v_.x, _v_.y, _v_.z, 0)";
        PinFactory_ConvMap["Fusee.Math.float3"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4(_v_.x, _v_.y, _v_.z, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        
        // from float4
        PinFactory_ConvMap["Fusee.Math.float4"] = {};
        PinFactory_ConvMap["Fusee.Math.float4"]["System.Int32"] = "parseInt(_v_)";
        PinFactory_ConvMap["Fusee.Math.float4"]["System.Single"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.float4"]["System.Double"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.float4"]["System.Boolean"] = "((_v_.x + _v_.y + _v_.z + _v_.w) > 0)? true : false";
        PinFactory_ConvMap["Fusee.Math.float4"]["System.String"] = "(_v_.x + _v_.y + _v_.z + _v_.w).toString()";
        PinFactory_ConvMap["Fusee.Math.float4"]["Fusee.Math.double2"] = "new $fuseeMath.Fusee.Math.double2(_v_.x, _v_.y)";
        PinFactory_ConvMap["Fusee.Math.float4"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3(_v_.x, _v_.y, _v_.z)";
        PinFactory_ConvMap["Fusee.Math.float4"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4(_v_.x, _v_.y, _v_.z, _v_.w)";
        PinFactory_ConvMap["Fusee.Math.float4"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4(_v_.x, _v_.y, _v_.z, _v_.w, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        PinFactory_ConvMap["Fusee.Math.float4"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(_v_.x, _v_.y)";
        PinFactory_ConvMap["Fusee.Math.float4"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3(_v_.x, _v_.y, _v_.z)";
        PinFactory_ConvMap["Fusee.Math.float4"]["Fusee.Math.float4"] = "_v_";
        PinFactory_ConvMap["Fusee.Math.float4"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4(_v_.x, _v_.y, _v_.z, _v_.w, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";

        // from float4x4
        PinFactory_ConvMap["Fusee.Math.float4x4"] = {};
        PinFactory_ConvMap["Fusee.Math.float4x4"]["System.Int32"] = "parseInt(_v_.M11)";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["System.Single"] = "_v_.M11";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["System.Double"] = "_v_.M11";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["System.Boolean"] = "((_v_.M11 + _v_.M12 + _v_.M13 + _v_.M14 + _v_.M21 + _v_.M22 + _v_.M23 + _v_.M24 + _v_.M31 + _v_.M32 + _v_.M33 + _v_.M34 + _v_.M41 + _v_.M42 + _v_.M43 + _v_.M44) > 0)? true : false";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["System.String"] = "(_v_.M11 + _v_.M12 + _v_.M13 + _v_.M14 + _v_.M21 + _v_.M22 + _v_.M23 + _v_.M24 + _v_.M31 + _v_.M32 + _v_.M33 + _v_.M34 + _v_.M41 + _v_.M42 + _v_.M43 + _v_.M44).toString()";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["Fusee.Math.double2"] = "new $fuseeMath.Fusee.Math.double2(_v_.M11, _v_.M12)";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3(_v_.M11, _v_.M12, _v_.M13)";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4(_v_.M11, _v_.M12, _v_.M13, _v_.M14)";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4(_v_.M11, _v_.M12, _v_.M13, _v_.M14, _v_.M21, _v_.M22, _v_.M23, _v_.M24, _v_.M31, _v_.M32, _v_.M33, _v_.M34, _v_.M41, _v_.M42, _v_.M43, _v_.M44)";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(_v_.M11, _v_.M12)";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3(_v_.M11, _v_.M12, _v_.M13)";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4(_v_.M11, _v_.M12, _v_.M13, _v_.M14)";
        PinFactory_ConvMap["Fusee.Math.float4x4"]["Fusee.Math.float4x4"] = "_v_";

        // from double2
        PinFactory_ConvMap["Fusee.Math.double2"] = {};
        PinFactory_ConvMap["Fusee.Math.double2"]["System.Int32"] = "parseInt(_v_.x)";
        PinFactory_ConvMap["Fusee.Math.double2"]["System.Single"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.double2"]["System.Double"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.double2"]["System.Boolean"] = "((_v_.x + _v_.y) > 0)? true : false";
        PinFactory_ConvMap["Fusee.Math.double2"]["System.String"] = "(_v_.x + _v_.y).toString()";
        PinFactory_ConvMap["Fusee.Math.double2"]["Fusee.Math.double2"] = "_v_";
        PinFactory_ConvMap["Fusee.Math.double2"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3(_v_.x, _v_.y, 0)";
        PinFactory_ConvMap["Fusee.Math.double2"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4(_v_.x, _v_.y, 0, 0)";
        PinFactory_ConvMap["Fusee.Math.double2"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4(_v_.x, _v_.y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        PinFactory_ConvMap["Fusee.Math.double2"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(_v_.x, _v_.y)";
        PinFactory_ConvMap["Fusee.Math.double2"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3(_v_.x, _v_.y, 0)";
        PinFactory_ConvMap["Fusee.Math.double2"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4(_v_.x, _v_.y, 0, 0)";
        PinFactory_ConvMap["Fusee.Math.double2"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4(_v_.x, _v_.y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";

        // from double3
        PinFactory_ConvMap["Fusee.Math.double3"] = {};
        PinFactory_ConvMap["Fusee.Math.double3"]["System.Int32"] = "parseInt(_v_)";
        PinFactory_ConvMap["Fusee.Math.double3"]["System.Single"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.double3"]["System.Double"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.double3"]["System.Boolean"] = "((_v_.x + _v_.y + _v_.z) > 0)? true : false";
        PinFactory_ConvMap["Fusee.Math.double3"]["System.String"] = "(_v_.x + _v_.y + _v_.z).toString()";
        PinFactory_ConvMap["Fusee.Math.double3"]["Fusee.Math.double2"] = "new $fuseeMath.Fusee.Math.double2(_v_.x, _v_.y)";
        PinFactory_ConvMap["Fusee.Math.double3"]["Fusee.Math.double3"] = "_v_"; 
        PinFactory_ConvMap["Fusee.Math.double3"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4(_v_.x, _v_.y, _v_.z, 0)";
        PinFactory_ConvMap["Fusee.Math.double3"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4(_v_.x, _v_.y, _v_.z, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        PinFactory_ConvMap["Fusee.Math.double3"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(_v_.x, _v_.y)";
        PinFactory_ConvMap["Fusee.Math.double3"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3(_v_.x, _v_.y, _v_.z)";
        PinFactory_ConvMap["Fusee.Math.double3"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4(_v_.x, _v_.y, _v_.z, 0)";
        PinFactory_ConvMap["Fusee.Math.double3"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4(_v_.x, _v_.y, _v_.z, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";

        // from double4
        PinFactory_ConvMap["Fusee.Math.double4"] = {};
        PinFactory_ConvMap["Fusee.Math.double4"]["System.Int32"] = "parseInt(_v_)";
        PinFactory_ConvMap["Fusee.Math.double4"]["System.Single"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.double4"]["System.Double"] = "_v_.x";
        PinFactory_ConvMap["Fusee.Math.double4"]["System.Boolean"] = "((_v_.x + _v_.y + _v_.z + _v_.w) > 0)? true : false";
        PinFactory_ConvMap["Fusee.Math.double4"]["System.String"] = "(_v_.x + _v_.y + _v_.z + _v_.w).toString()";
        PinFactory_ConvMap["Fusee.Math.double4"]["Fusee.Math.double2"] = "new $fuseeMath.Fusee.Math.double2(_v_.x, _v_.y)";
        PinFactory_ConvMap["Fusee.Math.double4"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3(_v_.x, _v_.y, _v_.z)";
        PinFactory_ConvMap["Fusee.Math.double4"]["Fusee.Math.double4"] = "_v_"; 
        PinFactory_ConvMap["Fusee.Math.double4"]["Fusee.Math.double4x4"] = "new $fuseeMath.Fusee.Math.double4x4(_v_.x, _v_.y, _v_.z, _v_.w, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
        PinFactory_ConvMap["Fusee.Math.double4"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(_v_.x, _v_.y)";
        PinFactory_ConvMap["Fusee.Math.double4"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3(_v_.x, _v_.y, _v_.z)";
        PinFactory_ConvMap["Fusee.Math.double4"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4(_v_.x, _v_.y, _v_.z, _v_.w)";
        PinFactory_ConvMap["Fusee.Math.double4"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4(_v_.x, _v_.y, _v_.z, _v_.w, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";

        // from double4x4
        PinFactory_ConvMap["Fusee.Math.double4x4"] = {};
        PinFactory_ConvMap["Fusee.Math.double4x4"]["System.Int32"] = "parseInt(_v_.M11)";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["System.Single"] = "_v_.M11";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["System.Double"] = "_v_.M11";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["System.Boolean"] = "((_v_.M11 + _v_.M12 + _v_.M13 + _v_.M14 + _v_.M21 + _v_.M22 + _v_.M23 + _v_.M24 + _v_.M31 + _v_.M32 + _v_.M33 + _v_.M34 + _v_.M41 + _v_.M42 + _v_.M43 + _v_.M44) > 0)? true : false";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["System.String"] = "(_v_.M11 + _v_.M12 + _v_.M13 + _v_.M14 + _v_.M21 + _v_.M22 + _v_.M23 + _v_.M24 + _v_.M31 + _v_.M32 + _v_.M33 + _v_.M34 + _v_.M41 + _v_.M42 + _v_.M43 + _v_.M44).toString()";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["Fusee.Math.double2"] = "new $fuseeMath.Fusee.Math.double2(_v_.M11, _v_.M12)";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["Fusee.Math.double3"] = "new $fuseeMath.Fusee.Math.double3(_v_.M11, _v_.M12, _v_.M13)";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["Fusee.Math.double4"] = "new $fuseeMath.Fusee.Math.double4(_v_.M11, _v_.M12, _v_.M13, _v_.M14)";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["Fusee.Math.double4x4"] = "_v_";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["Fusee.Math.float2"] = "new $fuseeMath.Fusee.Math.float2(_v_.M11, _v_.M12)";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["Fusee.Math.float3"] = "new $fuseeMath.Fusee.Math.float3(_v_.M11, _v_.M12, _v_.M13)";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["Fusee.Math.float4"] = "new $fuseeMath.Fusee.Math.float4(_v_.M11, _v_.M12, _v_.M13, _v_.M14)";
        PinFactory_ConvMap["Fusee.Math.double4x4"]["Fusee.Math.float4x4"] = "new $fuseeMath.Fusee.Math.float4x4(_v_.M11, _v_.M12, _v_.M13, _v_.M14, _v_.M21, _v_.M22, _v_.M23, _v_.M24, _v_.M31, _v_.M32, _v_.M33, _v_.M34, _v_.M41, _v_.M42, _v_.M43, _v_.M44)";

    }
    PinFactory_GetConvMap = function ()
    {
        return PinFactory_ConvMap;
    };
    return PinFactory_ConvMap;
};


//Interfaces
JSIL.MakeInterface(
  "Fusee.Xirkit.IJsMemberAccessor", true, [], function ($)
  {
      $.Method({}, "Set", new JSIL.MethodSignature(null, [$.Object, $.Object]));
      $.Method({}, "Get", new JSIL.MethodSignature($.Object, [$.Object], []));
  }, []);


JSIL.MakeInterface(
  "Fusee.Xirkit.JSIInPin", true, [], function ($)
  {
      $.Method({}, "get_Member", new JSIL.MethodSignature($.String, [], []));
      $.Method({}, "add_ReceivedValue", new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.ReceivedValueHandler")], []));
      $.Method({}, "remove_ReceivedValue", new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.ReceivedValueHandler")], []));
      $.Method({}, "GetPinType", new JSIL.MethodSignature($customMsCore.TypeRef("System.Type"), [], []));
      $.Property({}, "Member");
  }, []);


//JsAccessor Implementation
JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Xirkit.JsAccesssor", true, [], function ($interfaceBuilder)
{
    $ = $interfaceBuilder;

    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, [/*$customMsCore*/$customMsCore.TypeRef("System.Reflection.MemberInfo")], []),
        function ctor(memberInfo)
        {
            this._memberInfo = memberInfo;
            
        }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "Get",
        new JSIL.MethodSignature($.Object, [$.Object], []),
        function Get(o)
        {
            return this.GetImp(o);
        }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "Set",
        new JSIL.MethodSignature(null, [$.Object, ], []),
        function Set(o, val)
        {
            this.SetImp(o, val);
        }
    );

    $.Field({ Static: false, Public: false, ReadOnly: true }, "_storage", $customMSCore.TypeRef("System.Object"));
    $.Field({ Static: false, Public: false, ReadOnly: true }, "_memberInfo", $customMSCore.TypeRef("System.Reflection.MemberInfo"));
    $.ImplementInterfaces( /* 0 */ $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor"));

    return function (newThisType) { $thisType = newThisType; };
});


// implementation of public PinFactory Methods
JSIL.ImplementExternals("Fusee.Xirkit.PinFactory", function ($)
{

    $.Method({ Static: true, Public: true }, "CreateOutPin",
       new JSIL.MethodSignature($fuseeXirkit.TypeRef("Fusee.Xirkit.IOutPin"), [$fuseeXirkit.TypeRef("Fusee.Xirkit.Node"), $.String], []),
         function PinFactory_CreateOutPin(n, member)
         {
             var typeAndAccessor = PinFactory_GetMemberTypeAndAccessor(n, member, null);
             var outPin = new $WebXirkitImp.Fusee.Xirkit.JsOutPin(n, member, typeAndAccessor.elementType, typeAndAccessor.elementAccessor);
             return outPin;
         }
     );

    $.Method({ Static: true, Public: true }, "CreateInPin",
          new JSIL.MethodSignature($asm06.TypeRef("Fusee.Xirkit.IInPin"), [
              $fuseeXirkit.TypeRef("Fusee.Xirkit.Node"), $.String, $customMsCore.TypeRef("System.Type")], []),
              function PinFactory_CreateInPin(n, member, targetType)
              {
                  var typeAndAccessor = PinFactory_GetMemberTypeAndAccessor(n, member, targetType);

                  //if (typeAndAccessor.elementType != targetType) // TODO: && !CanConvert(targetType, memberType))
                  //    throw new Exception("No suitable converter to create converting InPin from " + targetType.Name + " to " + typeAndAccessor.elementType.Name);

                  var inPin = new $WebXirkitImp.Fusee.Xirkit.JsInPin(n, member, typeAndAccessor.elementAccessor);

                  return inPin;
              }
    );

    function PinFactory_GetMemberTypeAndAccessor(n, member, pinType)
    {
        var t = JSIL.GetType(n.get_O());
        var elementAccessor = null;
        var result = null;
        if ((member.indexOf(".") != -1))
        {

            var memberName = member.split(".");
            var miList = new Array(memberName.length);
            var isMemberField = new Array(memberName.length);
            var currentType = JSIL.GetType(n.get_O());
            for (var i = 0; i < memberName.length; i = ((i + 1) | 0))
            {

                var propertyInfo = currentType.GetProperty(memberName[i]);
                if (propertyInfo === null)
                {
                    // It's a field
                    var fieldInfo = currentType.GetField(memberName[i]);
                    if (fieldInfo === null)
                    {
                        throw new Exception("Neither a field nor a property named " + memberName[i] + " exists");
                    }
                    miList[i] = fieldInfo;
                    currentType = fieldInfo.get_FieldType();
                    isMemberField[i] = true;
                }else
                {
                    // It's a property
                    if (propertyInfo.GetGetMethod() === null)
                    {
                        throw new Exception(("A property named " + memberName[i] + " exists but we cannot read from it"));
                    }
                    miList[i] = propertyInfo;
                    currentType = propertyInfo.get_PropertyType();
                    isMemberField[i] = false;
                }
                
            }
            
            var memberType = currentType;
            if (pinType === null)
            {
                pinType = memberType;
            }
                    
            elementAccessor = PinFactory_InstantiateChainedMemberAccessor(miList, pinType, memberType, isMemberField);

            result = {
                "elementAccessor": elementAccessor,
                "elementType": memberType,             
            };
        } else
        {
            // Simple member access (no chain)
            var propertyInfo = t.GetProperty(member);
            if (propertyInfo === null)
            {
                // It's a field
                var fieldInfo = t.GetField(member);
                if (fieldInfo === null)
                {
                    throw new Exception("Neither a field nor a property named " + member + " exists");
                }
                memberType = fieldInfo.get_FieldType();

                if (pinType === null || memberType === pinType)
                {
                    // No conversion
                    elementAccessor = PinFactory_InstantiateFieldAccessor(fieldInfo, memberType);
                } else
                {
                    // Conversion
                    elementAccessor = PinFactory_InstantiateConvertingFieldAccessor(fieldInfo, pinType, memberType);
                }
            } else
            {
                // It's a property
                if (propertyInfo.GetGetMethod() === null)
                {
                    throw new Exception(("A property named " + member + " exists but we cannot read from it"));
                }
                memberType = propertyInfo.get_PropertyType();

                if (pinType === null || memberType === pinType)
                {
                    // No conversion
                    elementAccessor = PinFactory_InstantiatePropertyAccessor(propertyInfo, memberType);
                } else
                {
                    // Conversion
                    elementAccessor = PinFactory_InstantiateConvertingPropertyAccessor(propertyInfo, pinType, memberType);
                }        
            }
            result =
            {
                "elementType": memberType,
                "elementAccessor": elementAccessor
            };
        }
        return result;
    }

    function PinFactory_InstantiateFieldAccessor(memberInfo, memberType)
    {
        
        var ret = new Fusee.Xirkit.JsAccesssor(memberInfo);
        
        ret.SetImp = new Function("o", "val", "o." + memberInfo.Name + " = val;");
        ret.GetImp = new Function("o", "return o." + memberInfo.Name + ";");
        
        return ret;
    }
    
    function PinFactory_InstantiateConvertingFieldAccessor(memberInfo, pinType, memberType)
    {
        
        var ret = new Fusee.Xirkit.JsAccesssor(memberInfo);

        var setExpression = PinFactory_GetConvMap()[pinType.FullName][memberType.FullName];
        var getExpression = PinFactory_GetConvMap()[memberType.FullName][pinType.FullName];

        var setCommand = "o." + memberInfo.Name + " = " + setExpression.replace(/_v_/g, "val") + ";";
        var getCommand = "return " + getExpression.replace(/_v_/g, "o." + memberInfo.Name) + ";";

        ret.SetImp = new Function("o", "val", setCommand);
        ret.GetImp = new Function("o", getCommand);

        return ret;
    }

    function PinFactory_InstantiatePropertyAccessor(memberInfo, memberType)
    {

        var ret = new Fusee.Xirkit.JsAccesssor(memberInfo);
        ret.SetImp = new Function("o", "val", "o.set_" + memberInfo.Name + "(val);");
        ret.GetImp = new Function("o", "return o.get_" + memberInfo.Name + "();");

        return ret;

    }

    function PinFactory_InstantiateConvertingPropertyAccessor(memberInfo, pinType, memberType)
    {
        
        var ret = new Fusee.Xirkit.JsAccesssor(memberInfo);

        var setExpression = PinFactory_GetConvMap()[pinType.FullName][memberType.FullName];
        var getExpression = PinFactory_GetConvMap()[memberType.FullName][pinType.FullName];

        var setCommand = "o.set_" + memberInfo.Name + "(" + setExpression.replace(/_v_/g, "val") + ");";
        var getCommand = "return " + getExpression.replace(/_v_/g, "o.get_" + memberInfo.Name + "()") + ";";

        ret.SetImp = new Function("o", "val", setCommand);
        ret.GetImp = new Function("o", getCommand);

        return ret;
    }

    function PinFactory_InstantiateChainedMemberAccessor(miList, pinType, memberType, isMemberField)
    {
        var ret = new Fusee.Xirkit.JsAccesssor(null);

        var setCommand = "";
        var getCommand = "";
        var getMemberExpression = "";
        var setMemberExpression = "";
        var preSet = "";
        var midSet = "";
        var postSet = "";
 
        ret._storage = new Array(miList.length + 1);
        preSet = "this._storage[" + 0 + "] = o;";
        // preSet = "var storage = new Array(" + (miList.length + 1) + "); storage[" + 0 + "] = o;";
        for (var i = 0; i < miList.length; i++) {
            if (isMemberField[i])
            {
                preSet += "this._storage[" + (i + 1) + "] = this._storage[" + i + "]." + miList[i].Name + ";";
                
                if (i == miList.length - 1)
                {
                    midSet = "this._storage["+ (i + 1) +"] = val;";
                }

                postSet = "this._storage[" + i + "]." + miList[i].Name + " = this._storage[" + (i + 1) + "];"  + postSet;

                getMemberExpression += "." + miList[i].Name;          
            }
            else
            {

                preSet += "this._storage[" + (i + 1) + "] =  this._storage[" + i + "].get_" + miList[i].Name + "();";

                if (i == miList.length - 1)
                {
                    midSet = "this._storage[" + (i + 1) + "] = val;";
                }

                postSet = "this._storage[" + i + "].set_" + miList[i].Name +"(this._storage["+ (i + 1) +"]);" + postSet;

                getMemberExpression += ".get_" + miList[i].Name + "()";
            }
        }
        postSet += "o = this._storage[0];";
        setMemberExpression = preSet + midSet + postSet;
        
        if (pinType !== memberType)
        {
            
            var setExpression = PinFactory_GetConvMap()[pinType.FullName][memberType.FullName];
            var getExpression = PinFactory_GetConvMap()[memberType.FullName][pinType.FullName];

            setCommand = setMemberExpression.replace(/val/g, setExpression).replace(/_v_/g, "val");
            getCommand = "return " + getExpression.replace(/_v_/g, "o" + getMemberExpression) + ";";
        } else
        {
            setCommand = setMemberExpression;
            getCommand = "return o" + getMemberExpression + ";";
        }

        ret.SetImp = new Function("o", "val", setCommand);
        ret.GetImp = new Function("o", getCommand);

        return ret;

    };

    return function (newThisType) { $thisType = newThisType; };
    
});


// Implemantation of OutPin
JSIL.MakeClass($fuseeXirkit.TypeRef("Fusee.Xirkit.Pin"), "Fusee.Xirkit.JsOutPin", true, [], function ($interfaceBuilder)
{
    var $JsPin = function () {
        return ($JsPin = JSIL.Memoize($fuseeXirkit.Fusee.Xirkit.Pin))();
    };

    $ = $interfaceBuilder;

    $.Method({ Static: false, Public: true }, ".ctor",
           new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.Node"), $.String, $customMsCore.TypeRef("System.Type"), $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor")], []),

           function ctor(n, member, pinType, memberAccessor)
           {
                var $InPinList = new JSIL.ConstructorSignature($customMsCore.TypeRef("System.Collections.Generic.List`1", [$WebXirkitImp.TypeRef("Fusee.Xirkit.JsOutPin")]), []);
                $JsPin().prototype._ctor.call(this, n, member);
                this._links = $InPinList.Construct();
                this._pinType = pinType;
                this._memberAccessor = memberAccessor;
           }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "Attach",
      new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.IInPin")], []),
       function Attach(other)
       {
           this._links.Add(other); // cast other to IInPin?
       }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "Detach",
      new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.IInPin")], []),
       function Detach(other)
       {
           this._links.Remove(other); // cast other to IInPin?
       }
    );

    $.Method({ Static: false, Public: true }, "get_MemberAccessor",
      new JSIL.MethodSignature($WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor"), [], []),
      function get_MemberAccessor()
      {
          return this._memberAccessor;
      }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "GetPinType",
      new JSIL.MethodSignature($customMsCore.TypeRef("System.Type"), [], []),
       function GetPinType()
       {
           return this._pinType;
       }
    );

    $.Method({ Static: false, Public: true }, "GetValue",
      new JSIL.MethodSignature($fuseeXirkit.TypeRef("Fusee.Xirkit.JsOutPin"), [], []),
      function GetValue()
      {
          return this._memberAccessor.Get(this.get_N().get_O());
      }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "Propagate",
      new JSIL.MethodSignature(null, [], []),
      function Propagate()
      {
          var $temp00;

          for (var a$0 = this._links._items, i$0 = 0, l$0 = this._links._size; i$0 < l$0; ($temp00 = i$0,
              i$0 = ((i$0 + 1) | 0),
              $temp00))
          {
              var inPin = a$0[i$0];
              inPin.SetValue(this.GetValue());
          }
      }
    );

    $.Method({ Static: false, Public: true }, "set_MemberAccessor",
      new JSIL.MethodSignature(null, [$WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor", [$fuseeXirkit.TypeRef("Fusee.Xirkit.JsOutPin")])], []),
      function set_MemberAccessor(value)
      {
          this._memberAccessor = value;
      }
    );

    $.Field({ Static: false, Public: false }, "_memberAccessor", $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor"));
    $.Property({ Static: false, Public: true }, "MemberAccessor", $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor"));
    $.Field({ Static: false, Public: false }, "_pinType", $customMsCore.TypeRef("System.Type"));

    $.ImplementInterfaces(/* 0 */ $fuseeXirkit.TypeRef("Fusee.Xirkit.IOutPin"));

    return function (newThisType) { $thisType = newThisType; };
});


//InPin implementation
JSIL.MakeClass($fuseeXirkit.TypeRef("Fusee.Xirkit.Pin"), "Fusee.Xirkit.JsInPin", true, [], function ($interfaceBuilder)
{
    $ = $interfaceBuilder;

    var $JsDelagate = function ()
    {
        return ($JsDelagate = JSIL.Memoize($customMsCore.System.Delegate))();
    };
    var $JsInterlocked = function ()
    {
        return ($JsInterlocked = JSIL.Memoize($customMsCore.System.Threading.Interlocked))();
    };
    var $JsReceivedValueHandler = function ()
    {
        return ($JsReceivedValueHandler = JSIL.Memoize($fuseeXirkit.Fusee.Xirkit.ReceivedValueHandler))();
    };
    
    var $JsPin = function ()
    {
        return ($JsPin = JSIL.Memoize($fuseeXirkit.Fusee.Xirkit.Pin))();
    };

    $.Method({ Static: false, Public: true }, ".ctor",
          new JSIL.MethodSignature(null,
              [$fuseeXirkit.TypeRef("Fusee.Xirkit.Node"),
                  $.String,
                  $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor")],
               []),

          function ctor(n, member, memberAccessor)
          {
          $JsPin().prototype._ctor.call(this, n, member);
          this._memberAccessor = memberAccessor;
          }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "add_ReceivedValue",
        new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.ReceivedValueHandler")], []), function add_ReceivedValue(value)
          {
              var receivedValueHandler = this.ReceivedValue;
          
              do
              {
                  var receivedValueHandler2 = receivedValueHandler;
                  var value2 = $JsDelagate().Combine(receivedValueHandler2, value);
                  receivedValueHandler = $JsInterlocked().CompareExchange$b1($JsReceivedValueHandler())(/* ref */new JSIL.MemberReference(this, "ReceivedValue"), value2, receivedValueHandler2);
              } while (receivedValueHandler !== receivedValueHandler2);
          }
    );

    $.Method({ Static: false, Public: true }, "get_MemberAccessor",
        new JSIL.MethodSignature($WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor", [$WebXirkitImp.TypeRef("Fusee.Xirkit.JsInPin")]), [], []),
          function get_MemberAccessor()
          {
          return this._memberAccessor;
          }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "GetPinType",
        new JSIL.MethodSignature($customMsCore.TypeRef("System.Type"), [], []), 
          function GetPinType()
          {
              return $thisType.T.get(this);
          }
    );

    $.Method({ Static: false, Public: true, Virtual: true }, "remove_ReceivedValue",
        new JSIL.MethodSignature(null, [$fuseeXirkit.TypeRef("Fusee.Xirkit.ReceivedValueHandler")], []), function remove_ReceivedValue(value)
        {
           var receivedValueHandler = this.ReceivedValue;
           
           do
           {
               var receivedValueHandler2 = receivedValueHandler;
               var value2 = $JsDelagate().Remove(receivedValueHandler2, value);
               receivedValueHandler = $JsInterlocked().CompareExchange$b1($JsReceivedValueHandler())(/* ref */ new JSIL.MemberReference(this, "ReceivedValue"), value2, receivedValueHandler2);
           } while (receivedValueHandler !== receivedValueHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "set_MemberAccessor",
        new JSIL.MethodSignature(null, [$WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor", [$WebXirkitImp.TypeRef("Fusee.Xirkit.JsInPin")])], []),
        
          function set_MemberAccessor(value)
          {
              this._memberAccessor = value;
          }
    );

    $.Method({ Static: false, Public: true }, "SetValue",
        new JSIL.MethodSignature(null, [ $WebXirkitImp.TypeRef("Fusee.Xirkit.JsInPin")], []), 
    
        function SetValue(value)
        {
            this._memberAccessor.Set(this.get_N().get_O(), value);
            //var $im00 = $asm06.Fusee.Xirkit.IMemberAccessor$b1.Of($thisType.T.get(this)).Set;
            //$im00.Call(this._memberAccessor, null, this.get_N().get_O(), value);
            if (this.ReceivedValue !== null) {
                this.ReceivedValue(this, null);
            }
        }
    );

    $.Field(
        {
            Static: false,
            Public: false
        }, "_memberAccessor", $WebXirkitImp.TypeRef("Fusee.Xirkit.IJsMemberAccessor"));
    

    $.Field(
        {
            Static: false,
            Public: false
        }, "ReceivedValue", $fuseeXirkit.TypeRef("Fusee.Xirkit.ReceivedValueHandler"));
    

    $.Property(
        {
            Static: false,
            Public: true
        }, "MemberAccessor", $fuseeXirkit.TypeRef("Fusee.Xirkit.IJsMemberAccessor", [$WebXirkitImp.TypeRef("Fusee.Xirkit.JsInPin")]));

    $.ImplementInterfaces(
    /* 0 */
    $fuseeXirkit.TypeRef("Fusee.Xirkit.IInPin"));

    return function (newThisType) {
        $thisType = newThisType;
    };
});

