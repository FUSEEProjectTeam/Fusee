using System;
using System.Reflection;
using System.Collections.Generic;
using Fusee.Math;
using JSIL.Meta;

namespace Fusee.Xirkit
{
    /// <summary>
    /// This class' static member function creates Pins depending on the type of the member (int, double, string, ...) and also 
    /// depending on whether the memeber is a propery (with a setter and getter) or a simple field.
    /// </summary>

    static class PinFactory
    {
       [JSExternal]
        public static IOutPin CreateOutPin(Node n, String member)
        {
            // The code below mainly does something like a
            // 
            // if (member is a real property with setters and getters and not just a field)
            //    return new OutPin<t>(this, member, new PropertyAccessor<t>(propertyInfo));
            // else  // it's only a field...
            //    return new OutPin<t>(this, member, new FieldAccessor<t>(fieldInfo));
            // 
            // Unfortunately we cannot write it that way because we don't have t
            // at compile time. So we need to do some Reflection magic.

            object elementAccessor;
            Type memberType = GetMemberTypeAndAccessor(n, member, out elementAccessor);

            // Perform  <code> return new OutPin<t>(n, member, elementAccessor); </code> with t known at runtime, not at compile time
            Type outPinGeneric = typeof(OutPin<>).MakeGenericType(new Type[] { memberType });
            return (IOutPin)Activator.CreateInstance(outPinGeneric, new object[] { n, member, elementAccessor });
        }

       [JSExternal]
        public static IInPin CreateInPin(Node n, string member, Type targetType)
        {
            // The code below mainly does something like a
            // 
            // if (member is a real property with setters and getters and not just a field)
            //    return new InPin<t>(this, member, new PropertyAccessor<t>(propertyInfo));
            // else  // it's only a field...
            //    return new InPin<t>(this, member, new FieldAccessor<t>(fieldInfo));
            // 
            // Unfortunately we cannot write it that way because we don't have t
            // at compile time. So we need to do some Reflection magic.
            object elementAccessor;
            Type memberType = GetMemberTypeAndAccessor(n, member, targetType, out elementAccessor);

            // pre-check if types are compatible or castable
            if (memberType != targetType && !CanConvert(targetType, memberType))
                throw new Exception("No suitable converter to create converting InPin from " + targetType.Name + " to " + memberType.Name);

            // Perform  <code> return new InPin<t>(n, member, elementAccessor); </code> with t known at runtime, not at compile time
            Type inPinGeneric = typeof(InPin<>).MakeGenericType(new Type[] { targetType });
            return (IInPin)Activator.CreateInstance(inPinGeneric, new object[] { n, member, elementAccessor });
        }


       [JSExternal]
        public static void ReAttachInPin(Node n, IInPin ip)
        {
            string member = ip.Member;
            Type targetType = ip.GetPinType();
            object elementAccessor;
            Type memberType = GetMemberTypeAndAccessor(n, member, targetType, out elementAccessor);

            // This does a ip.ElementAccessor = elementAccessor. We need to use reflection because the type is not known at compile time
            Type t = ip.GetType();
            PropertyInfo pi = t.GetProperty("MemberAccessor");
            pi.SetValue(ip, elementAccessor, null);
        }

       [JSExternal]
        public static void ReAttachOutPin(Node n, IOutPin op)
        {
            string member = op.Member;
            object elementAccessor;
            Type memberType = GetMemberTypeAndAccessor(n, member, null, out elementAccessor);

            // This does a ip.ElementAccessor = elementAccessor. We need to use reflection because the type is not known at compile time
            op.GetType().GetProperty("MemberAccessor").SetValue(op, elementAccessor, null);
        }

       [JSIgnore]
        private static Type GetMemberTypeAndAccessor(Node n, string member, out object elementAccessor)
        {
            return GetMemberTypeAndAccessor(n, member, null, out elementAccessor);
        }


       [JSIgnore]
        private static Type GetMemberTypeAndAccessor(Node n, string member, Type pinType, out object elementAccessor)
        {
            Type t = n.O.GetType();

            // First see if it's an entire access chain (like n.O.transform.vec.x)
            if (member.Contains("."))
            {
                Type memberType;
                string[] memberName = member.Split(new char[] { '.' });
                MemberInfo[] miList = new MemberInfo[memberName.Length];
                Type currentType = n.O.GetType();
                MemberInfo[] miFound;
                for (int i = 0; i < memberName.Length; i++)
                {
                    miFound = currentType.GetMember(memberName[i]);
                    if (miFound.Length == 0 || !(miFound[0] is FieldInfo || miFound[0] is PropertyInfo))
                        throw new Exception("Neither a field nor a property named " + memberName[i] + " exists along the member chain " + member);
                    if (miFound.Length > 1)
                        throw new Exception("More than one member named " + memberName[i] + " exists along the member chain " + member);

                    miList[i] = miFound[0];

                    currentType = (miList[i] is FieldInfo) ? ((FieldInfo)miList[i]).FieldType : ((PropertyInfo)miList[i]).PropertyType;
                }
                memberType = currentType;
                if (pinType == null)
                    pinType = memberType;
                elementAccessor = InstantiateChainedMemberAccessor(miList, pinType, memberType);
                return memberType;
            }
            else
            {
                // It's a simple accessor
                PropertyInfo propertyInfo = t.GetProperty(member);
                FieldInfo fieldInfo;
                Type memberType;

                elementAccessor = null;
                if (((object)propertyInfo) == null)
                {
                    fieldInfo = t.GetField(member);
                    if (fieldInfo == null)
                    {
                        //TODO: change Exception to an apropriate exception type
                        throw new Exception(
                            "Neither a field nor a property named " + member + " exists");
                    }
                    else
                    {
                        memberType = fieldInfo.FieldType;
                        if (pinType == null || memberType == pinType)
                            elementAccessor = InstantiateFieldAccessor(fieldInfo, memberType);
                        else
                            elementAccessor = InstantiateConvertingFieldAccessor(fieldInfo, pinType, memberType);
                    }
                }
                else
                {

                    if (!propertyInfo.CanRead)
                    {
                        //TODO: change Exception to an apropriate exception type
                        throw new Exception(
                            "A property named " + member + " exists but we cannot read from it");
                    }
                    else
                    {
                        memberType = propertyInfo.PropertyType;
                        if (pinType == null || memberType == pinType)
                            elementAccessor = InstantiatePropertyAccessor(propertyInfo, memberType);
                        else
                            elementAccessor = InstantiateConvertingPropertyAccessor(propertyInfo, pinType, memberType);
                    }
                }
                return memberType;
            }
        }


        // To toy around with different element accessor implementations, create your own versions of 
        // IMemberAccessor<T> derivatives and change the instantiation code below.

       [JSIgnore]
        private static object InstantiatePropertyAccessor(PropertyInfo propertyInfo, Type memberType)
        {
            // Perform  <code> return new PropertyAccessor<t>(fieldInfo); </code> with a dynamic t (t known at runtime, not at compile time) 
            object elementAccessor;
            Type propertyAccessorGeneric =
                typeof(PropertyAccessor<>).MakeGenericType(new Type[] { memberType });
            elementAccessor = Activator.CreateInstance(propertyAccessorGeneric, new object[] { propertyInfo });
            return elementAccessor;
        }

       [JSIgnore]
        private static object InstantiateFieldAccessor(FieldInfo fieldInfo, Type memberType)
        {
            // Perform  <code> return new FieldAccessor<t>(fieldInfo); </code> with a dynamic t (t known at runtime, not at compile time) 
            object elementAccessor;
            Type fieldAccessorGeneric = typeof(FieldAccesssor<>).MakeGenericType(new Type[] { memberType });
            elementAccessor = Activator.CreateInstance(fieldAccessorGeneric, new object[] { fieldInfo });
            return elementAccessor;
        }


       [JSIgnore]
        private static Dictionary<Type, Dictionary<Type, Delegate>> _convMap = null;

       [JSIgnore]
        private static void InitConvMap()
        {
            // Look at http://msdn.microsoft.com/de-de/library/bb882516.aspx or 
            // google "anonymous functions c#" to see how to define the anonymous converter code.

            _convMap = new Dictionary<Type, Dictionary<Type, Delegate>>();

            // From int
            AddConverter<int, int>(x => x);
            AddConverter<int, float>(x => (float)x);
            AddConverter<int, double>(x => (double)x);
            AddConverter<int, bool>(x => (x == 0) ? false : true);
            AddConverter<int, string>(x => x.ToString());
            AddConverter<int, double2>(x => new double2(x, 0));
            AddConverter<int, double3>(x => new double3(x, 0, 0));
            AddConverter<int, double4>(x => new double4(x, 0, 0, 1));
            AddConverter<int, double4x4>(x => new double4x4(x, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            AddConverter<int, float2>(x => new float2(x, 0));
            AddConverter<int, float3>(x => new float3(x, 0, 0));
            AddConverter<int, float4>(x => new float4(x, 0, 0, 1));
            AddConverter<int, float4x4>(x => new float4x4(x, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

            // From float
            AddConverter<float, int>(x => (int)x);
            AddConverter<float, float>(x => x);
            AddConverter<float, double>(x => (double)x);
            AddConverter<float, bool>(x => (x == 0.0f) ? false : true);
            AddConverter<float, string>(x => x.ToString());
            AddConverter<float, double2>(x => new double2(x, 0));
            AddConverter<float, double3>(x => new double3(x, 0, 0));
            AddConverter<float, double4>(x => new double4(x, 0, 0, 1));
            AddConverter<float, double4x4>(x => new double4x4(x, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            AddConverter<float, float2>(x => new float2(x, 0));
            AddConverter<float, float3>(x => new float3(x, 0, 0));
            AddConverter<float, float4>(x => new float4(x, 0, 0, 1));
            AddConverter<float, float4x4>(x => new float4x4((float)x, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

            // From double
            AddConverter<double, int>(x => (int)x);
            AddConverter<double, float>(x => (float)x);
            AddConverter<double, double>(x => x);
            AddConverter<double, bool>(x => (x == 0.0) ? false : true);
            AddConverter<double, string>(x => x.ToString());
            AddConverter<double, double2>(x => new double2(x, 0));
            AddConverter<double, double3>(x => new double3(x, 0, 0));
            AddConverter<double, double4>(x => new double4(x, 0, 0, 1));
            AddConverter<double, double4x4>(x => new double4x4(x, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            AddConverter<double, float2>(x => new float2((float)x, 0));
            AddConverter<double, float3>(x => new float3((float)x, 0, 0));
            AddConverter<double, float4>(x => new float4((float)x, 0, 0, 1));
            AddConverter<double, float4x4>(x => new float4x4((float)x, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

            // From bool 
            AddConverter<bool, int>(x => (x) ? 0 : 1);
            AddConverter<bool, float>(x => (x) ? 0.0f : 1.0f);
            AddConverter<bool, double>(x => (x) ? 0.0 : 1.0);
            AddConverter<bool, bool>(x => x);
            AddConverter<bool, string>(x => x.ToString());
            AddConverter<bool, double2>(x => new double2((x) ? 0.0 : 1.0, 0));
            AddConverter<bool, double3>(x => new double3((x) ? 0.0 : 1.0, 0, 0));
            AddConverter<bool, double4>(x => new double4((x) ? 0.0 : 1.0, 0, 0, 1));
            AddConverter<bool, double4x4>(x => new double4x4((x) ? 0.0 : 1.0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            AddConverter<bool, float2>(x => new float2((x) ? 0.0f : 1.0f, 0));
            AddConverter<bool, float3>(x => new float3((x) ? 0.0f : 1.0f, 0, 0));
            AddConverter<bool, float4>(x => new float4((x) ? 0.0f : 1.0f, 0, 0, 1));
            AddConverter<bool, float4x4>(x => new float4x4((x) ? 0.0f : 1.0f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

            // From string
            AddConverter<string, int>(x => int.Parse(x));
            AddConverter<string, float>(x => float.Parse(x));
            AddConverter<string, double>(x => double.Parse(x));
            AddConverter<string, bool>(x => bool.Parse(x));
            AddConverter<string, string>(x => x);
            AddConverter<string, double2>(double2.Parse);  // Pass the Parse method directly (it already has the Converter signature)
            AddConverter<string, double3>(double3.Parse);
            AddConverter<string, double4>(double4.Parse);
            AddConverter<string, double4x4>(double4x4.Parse);
            AddConverter<string, float2>(float2.Parse);
            AddConverter<string, float3>(float3.Parse);
            AddConverter<string, float4>(float4.Parse);
            AddConverter<string, float4x4>(float4x4.Parse);

            // From double2
            AddConverter<double2, int>(v => (int)v.x);
            AddConverter<double2, float>(v => (float)v.x);
            AddConverter<double2, double>(v => v.x);
            AddConverter<double2, bool>(v => (v.x != 0.0 || v.y != 0.0));
            AddConverter<double2, string>(v => v.ToString());
            AddConverter<double2, double2>(v => v);
            AddConverter<double2, double3>(v => new double3(v.x, v.y, 0));
            AddConverter<double2, double4>(v => new double4(v.x, v.y, 0, 1));
            AddConverter<double2, double4x4>(v => new double4x4(v.x, v.y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            AddConverter<double2, float2>(v => new float2((float)v.x, (float)v.y));
            AddConverter<double2, float3>(v => new float3((float)v.x, (float)v.y, 0));
            AddConverter<double2, float4>(v => new float4((float)v.x, (float)v.y, 0, 1));
            AddConverter<double2, float4x4>(v => new float4x4((float)v.x, (float)v.y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));


            // From double3
            AddConverter<double3, int>(v => (int)v.x);
            AddConverter<double3, float>(v => (float)v.x);
            AddConverter<double3, double>(v => v.x);
            AddConverter<double3, bool>(v => (v.x != 0.0 || v.y != 0.0 || v.z != 0.0));
            AddConverter<double3, string>(v => v.ToString());
            AddConverter<double3, double2>(v => new double2(v.x, v.y));
            AddConverter<double3, double3>(v => v);
            AddConverter<double3, double4>(v => new double4(v.x, v.y, v.z, 1));
            AddConverter<double3, double4x4>(v => new double4x4(v.x, v.y, v.z, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            AddConverter<double3, float2>(v => new float2((float)v.x, (float)v.y));
            AddConverter<double3, float3>(v => new float3((float)v.x, (float)v.y, (float)v.z));
            AddConverter<double3, float4>(v => new float4((float)v.x, (float)v.y, (float)v.z, 1));
            AddConverter<double3, float4x4>(v => new float4x4((float)v.x, (float)v.y, (float)v.z, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

            // From double4
            AddConverter<double4, int>(v => (int)v.x);
            AddConverter<double4, float>(v => (float)v.x);
            AddConverter<double4, double>(v => v.x);
            AddConverter<double4, bool>(v => (v.x != 0.0 || v.y != 0.0 || v.z != 0.0));
            AddConverter<double4, string>(v => v.ToString());
            AddConverter<double4, double2>(v => new double2(v.x, v.y));
            AddConverter<double4, double3>(v => new double3(v.x, v.y, v.z));
            AddConverter<double4, double4>(v => v);
            AddConverter<double4, float4x4>(v => new float4x4((float)v.x, (float)v.y, (float)v.z, (float)v.w, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            AddConverter<double4, float2>(v => new float2((float)v.x, (float)v.y));
            AddConverter<double4, float3>(v => new float3((float)v.x, (float)v.y, (float)v.z));
            AddConverter<double4, float4>(v => new float4((float)v.x, (float)v.y, (float)v.z, (float)v.w));
            AddConverter<double4, double4x4>(v => new double4x4(v.x, v.y, v.z, v.w, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

            // From float2
            AddConverter<float2, int>(v => (int)v.x);
            AddConverter<float2, float>(v => (float)v.x);
            AddConverter<float2, double>(v => v.x);
            AddConverter<float2, bool>(v => (v.x != 0.0 || v.y != 0.0));
            AddConverter<float2, string>(v => v.ToString());
            AddConverter<float2, double2>(v => new double2(v.x, v.y));
            AddConverter<float2, double3>(v => new double3(v.x, v.y, 0));
            AddConverter<float2, double4>(v => new double4(v.x, v.y, 0, 1));
            AddConverter<float2, double4x4>(v => new double4x4(v.x, v.y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            AddConverter<float2, float2>(v => v);
            AddConverter<float2, float3>(v => new float3(v.x, v.y, 0));
            AddConverter<float2, float4>(v => new float4(v.x, v.y, 0, 1));
            AddConverter<float2, double4x4>(v => new double4x4(v.x, v.y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

            // From float3
            AddConverter<float3, int>(v => (int)v.x);
            AddConverter<float3, float>(v => (float)v.x);
            AddConverter<float3, double>(v => v.x);
            AddConverter<float3, bool>(v => (v.x != 0.0 || v.y != 0.0 || v.z != 0.0));
            AddConverter<float3, string>(v => v.ToString());
            AddConverter<float3, double2>(v => new double2(v.x, v.y));
            AddConverter<float3, double3>(v => new double3(v.x, v.y, v.z));
            AddConverter<float3, double4>(v => new double4(v.x, v.y, v.z, 1));
            AddConverter<float3, double4x4>(v => new double4x4(v.x, v.y, v.z, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            AddConverter<float3, float2>(v => new float2(v.x, v.y));
            AddConverter<float3, float3>(v => v);
            AddConverter<float3, float4>(v => new float4(v.x, v.y, (float)v.z, 1));
            AddConverter<float3, double4x4>(v => new double4x4(v.x, v.y, v.z, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

            // From float4
            AddConverter<float4, int>(v => (int)v.x);
            AddConverter<float4, float>(v => v.x);
            AddConverter<float4, double>(v => v.x);
            AddConverter<float4, bool>(v => (v.x != 0.0 || v.y != 0.0 || v.z != 0.0));
            AddConverter<float4, string>(v => v.ToString());
            AddConverter<float4, double2>(v => new double2(v.x, v.y));
            AddConverter<float4, double3>(v => new double3(v.x, v.y, v.z));
            AddConverter<float4, double4>(v => new double4(v.x, v.y, v.z, v.w));
            AddConverter<float4, double4x4>(v => new double4x4(v.x, v.y, v.z, v.w, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            AddConverter<float4, float2>(v => new float2(v.x, v.y));
            AddConverter<float4, float3>(v => new float3(v.x, v.y, v.z));
            AddConverter<float4, float4>(v => v);
            AddConverter<float4, float4x4>(v => new float4x4(v.x, v.y, v.z, v.w, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));


            // From float4x4
            AddConverter<float4x4, int>(v => (int)v.M11);
            AddConverter<float4x4, float>(v => v.M11);
            AddConverter<float4x4, double>(v => (double)v.M11);
            AddConverter<float4x4, string>(v => v.ToString());
            AddConverter<float4x4, bool>(v => (v.M11 != 0.0 || v.M12 != 0.0 || v.M13 != 0.0 || v.M14 != 0.0 || v.M21 != 0.0 || v.M22 != 0.0 || v.M23 != 0.0 || v.M24 != 0.0 || v.M31 != 0.0 || v.M32 != 0.0 || v.M33 != 0.0 || v.M34 != 0.0 || v.M41 != 0.0 || v.M42 != 0.0 || v.M43 != 0.0 || v.M44 != 0.0));
            AddConverter<float4x4, double2>(v => new double2(v.M11, v.M12));
            AddConverter<float4x4, double3>(v => new double3(v.M11, v.M12, v.M13));
            AddConverter<float4x4, double4>(v => new double4(v.M11, v.M12, v.M13, v.M14));
            AddConverter<float4x4, double4x4>(v => new double4x4(v.M11, v.M12, v.M13, v.M14, v.M21, v.M22, v.M23, v.M24, v.M31, v.M32, v.M33, v.M34, v.M41, v.M42, v.M43, v.M44));
            AddConverter<float4x4, float2>(v => new float2(v.M11, v.M12));
            AddConverter<float4x4, float3>(v => new float3(v.M11, v.M12, v.M13));
            AddConverter<float4x4, float4>(v => new float4(v.M11, v.M12, v.M13, v.M14));
            AddConverter<float4x4, float4x4>(v => v);

            // From double4x4
            AddConverter<double4x4, int>(v => (int)v.M11);
            AddConverter<double4x4, float>(v => (float)v.M11);
            AddConverter<double4x4, double>(v => (double)v.M11);
            AddConverter<double4x4, string>(v => v.ToString());
            AddConverter<double4x4, bool>(v => (v.M11 != 0.0 || v.M12 != 0.0 || v.M13 != 0.0 || v.M14 != 0.0 || v.M21 != 0.0 || v.M22 != 0.0 || v.M23 != 0.0 || v.M24 != 0.0 || v.M31 != 0.0 || v.M32 != 0.0 || v.M33 != 0.0 || v.M34 != 0.0 || v.M41 != 0.0 || v.M42 != 0.0 || v.M43 != 0.0 || v.M44 != 0.0));
            AddConverter<double4x4, double2>(v => new double2(v.M11, v.M12));
            AddConverter<double4x4, double3>(v => new double3(v.M11, v.M12, v.M13));
            AddConverter<double4x4, double4>(v => new double4(v.M11, v.M12, v.M13, v.M14));
            AddConverter<double4x4, double4x4>(v => v);
            AddConverter<double4x4, float2>(v => new float2((float)v.M11, (float)v.M12));
            AddConverter<double4x4, float3>(v => new float3((float)v.M11, (float)v.M12, (float)v.M13));
            AddConverter<double4x4, float4>(v => new float4((float)v.M11, (float)v.M12, (float)v.M13, (float)v.M14));
            AddConverter<double4x4, float4x4>(v => new float4x4((float)v.M11, (float)v.M12, (float)v.M13, (float)v.M14, (float)v.M21, (float)v.M22, (float)v.M23, (float)v.M24, (float)v.M31, (float)v.M32, (float)v.M33, (float)v.M34, (float)v.M41, (float)v.M42, (float)v.M43, (float)v.M44));




        }

       [JSIgnore]
        private static void AddConverter<TParm, TRet>(Converter<TParm, TRet> c)
        {
            Delegate d = (Delegate)c;

            Dictionary<Type, Delegate> val;
            if (!_convMap.TryGetValue(typeof(TParm), out val))
                _convMap[typeof(TParm)] = new Dictionary<Type, Delegate>();

            _convMap[typeof(TParm)][typeof(TRet)] = d;
        }

       [JSIgnore]
        private static Delegate LookupConverter(Type from, Type to)
        {
            if (_convMap == null)
                InitConvMap();

            return _convMap[from][to];
        }

       [JSIgnore]
        private static bool CanConvert(Type from, Type to)
        {
            if (_convMap == null)
                InitConvMap();

            Dictionary<Type, Delegate> dict;
            if (!_convMap.TryGetValue(from, out dict))
                return false;

            Delegate del = null;
            if (!dict.TryGetValue(to, out del))
                return false;

            return del != null;
        }

       [JSIgnore]
        private static object InstantiateConvertingPropertyAccessor(PropertyInfo propertyInfo, Type pinType, Type memberType)
        {
            // Perform  <code> return new ConvertingPropertyAccessor<pinType, memberType>(fieldInfo); </code> with a dynamic t (t known at runtime, not at compile time) 
            object elementAccessor;
            Type propertyAccessorGeneric =
                typeof(ConvertingPropertyAccessor<,>).MakeGenericType(new Type[] { pinType, memberType });
            elementAccessor = Activator.CreateInstance(propertyAccessorGeneric, new object[] { propertyInfo, LookupConverter(pinType, memberType), LookupConverter(memberType, pinType) });
            return elementAccessor;
        }

       [JSIgnore]
        private static object InstantiateConvertingFieldAccessor(FieldInfo fieldInfo, Type pinType, Type memberType)
        {
            // Perform  <code> return new FieldAccessor<pinType, memberType>(fieldInfo); </code> with a dynamic t (t known at runtime, not at compile time) 
            Type fieldAccessorGeneric = typeof(ConvertingFieldAccessor<,>).MakeGenericType(new Type[] { pinType, memberType });
            object elementAccessor = Activator.CreateInstance(fieldAccessorGeneric, new object[] { fieldInfo, LookupConverter(pinType, memberType), LookupConverter(memberType, pinType) });
            return elementAccessor;
        }

       [JSIgnore]
        private static object InstantiateChainedMemberAccessor(MemberInfo[] miList, Type pinType, Type memberType)
        {
            // Perform  <code> return new ChainedMemberAccessor<pinType, memberType>(miList, converter, converter); </code> with a dynamic t (t known at runtime, not at compile time) 
            Type fieldAccessorGeneric = typeof(ChainedMemberAccessor<,>).MakeGenericType(new Type[] { pinType, memberType });
            object elementAccessor = Activator.CreateInstance(fieldAccessorGeneric, new object[] { miList, LookupConverter(pinType, memberType), LookupConverter(memberType, pinType) });
            return elementAccessor;
        }
    }
}
