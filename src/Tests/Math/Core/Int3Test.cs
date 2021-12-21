using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class Int3Test
    {
        #region Fields

        [Fact]
        public void UnitX_IsUnit()
        {
            var expected = new int3(1, 0, 0);

            Assert.Equal(expected, int3.UnitX);
        }

        [Fact]
        public void UnitY_IsUnit()
        {
            var expected = new int3(0, 1, 0);

            Assert.Equal(expected, int3.UnitY);
        }

        [Fact]
        public void UnitZ_IsUnit()
        {
            var expected = new int3(0, 0, 1);

            Assert.Equal(expected, int3.UnitZ);
        }

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new int3(0, 0, 0);

            Assert.Equal(expected, int3.Zero);
        }

        [Fact]
        public void One_IsOne()
        {
            var expected = new int3(1, 1, 1);

            Assert.Equal(expected, int3.One);
        }

        #endregion

        #region Constructors

        [Fact]
        public void Constructor_FromInt2()
        {
            var f2 = new int2(1, 2);

            var actual = new int3(f2);

            Assert.Equal(new int3(1, 2, 0), actual);
        }

        [Fact]
        public void Constructor_FromInt3()
        {
            var f3 = new int3(1, 2, 3);

            var actual = new int3(f3);

            Assert.Equal(new int3(1, 2, 3), actual);
        }

        [Fact]
        public void Constructor_Fromint4()
        {
            var f4 = new int4(1, 2, 3, 4);

            var actual = new int3(f4);

            Assert.Equal(new int3(1, 2, 3), actual);
        }

        [Fact]
        public void Constructor_FromDouble3()
        {
            var d3 = new double3(1, 2, 3);

            var actual = new int3(d3);

            Assert.Equal(new int3(1, 2, 3), actual);
        }

        #endregion

        #region Instance

        [Fact]
        public void Length_Is3()
        {
            var vec = new int3(1, 2, 2);

            var actual = vec.Length;

            Assert.Equal(3, actual);
        }

        [Fact]
        public void LengthSquared_Is3()
        {
            var vec = new int3(1, 1, 1);

            var actual = vec.LengthSquared;

            Assert.Equal(3, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(int3 vec, float3 expected)
        {
            var actual = vec.Normalize();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Instance(int3 vec, float3 expected)
        {
            var actual = vec.NormalizeFast();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToArray_IsArray()
        {
            var vec = new int3(1, 2, 3);

            var actual = vec.ToArray();

            Assert.Equal(new int[] { 1, 2, 3 }, actual);
        }

        #endregion

        #region this

        [Fact]
        public void This_GetWithIdx_IsValid()
        {
            var actual = new int3(3, 7, 8);

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);

            Assert.Equal(8, actual[2]);
        }

        [Fact]
        public void This_SetWithIdx_IsValid()
        {
            var actual = new int3(0, 0, 0);
            actual[0] = 3;
            actual[1] = 7;
            actual[2] = 8;

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);

            Assert.Equal(8, actual[2]);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_GetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => new int3(0, 0, 0)[idx]);

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_SetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var f3 = new int3(0, 0, 0); f3[idx] = 10; });

            Assert.Equal(expected, actual.ParamName);
        }

        public static IEnumerable<object[]> ThisException()
        {
            yield return new object[] { 7, "Index 7 not eligible for a int3 type" };
            yield return new object[] { 6, "Index 6 not eligible for a int3 type" };
        }

        #endregion

        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_TwoVectors_Static(int3 left, int3 right, int3 expected)
        {
            var actual = int3.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_VectorScalar_Static(int3 left, int3 right, int3 expected)
        {
            var actual = int3.Add(left, right.x);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_TwoVectors_Static(int3 left, int3 right, int3 expected)
        {
            var actual = int3.Subtract(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_VectorScalar_Static(int3 left, int3 right, int3 expected)
        {
            var actual = int3.Subtract(left, right.x);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Static(int3 vec, int scale, int3 expected)
        {
            var actual = int3.Multiply(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multitply_TwoVectors_Static(int3 left, int scale, int3 expected)
        {
            var right = new int3(scale, scale, scale);

            var actual = int3.Multiply(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_VectorScalar_Static(int3 vec, int scale, int3 expected)
        {
            var actual = int3.Divide(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_TwoVectors_Static(int3 left, int scale, int3 expected)
        {
            var right = new int3(scale, scale, scale);

            var actual = int3.Divide(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region MinMax

        [Theory]
        [MemberData(nameof(GetComponentMin))]
        public void ComponentMin_IsZero(int3 left, int3 right, int3 expected)
        {
            var actual = int3.ComponentMin(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetComponentMax))]
        public void ComponentMax_IsOne(int3 left, int3 right, int3 expected)
        {
            var actual = int3.ComponentMax(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMin))]
        public void Min_IsZero(int3 left, int3 right, int3 expected)
        {
            var actual = int3.Min(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMax))]
        public void Max_IsOne(int3 left, int3 right, int3 expected)
        {
            var actual = int3.Max(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Clamp

        [Theory]
        [MemberData(nameof(GetClamp))]
        public void Clamp_TestClamp(int3 vec, int3 min, int3 max, int3 expected)
        {
            var actual = int3.Clamp(vec, min, max);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Normalize

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Static(int3 vec, float3 expected)
        {
            var actual = int3.Normalize(vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Static(int3 vec, float3 expected)
        {
            var actual = int3.NormalizeFast(vec);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Dot

        [Fact]
        public void Dot_Is10()
        {
            var left = new int3(1, 2, 3);
            var right = new int3(3, 2, 1);

            var actual = int3.Dot(left, right);

            Assert.Equal(10, actual);
        }

        #endregion

        #region Cross

        [Fact]
        public void Cross_IsVector()
        {
            var left = new int3(1, 2, 3);
            var right = new int3(3, 2, 1);

            var actual = int3.Cross(left, right);

            Assert.Equal(new int3(-4, 8, -4), actual);
        }

        #endregion

        #region Lerp

        [Theory]
        [MemberData(nameof(GetLerp))]
        public void Lerp_TestLerp(int3 left, int3 right, float blend, float3 expected)
        {
            var actual = int3.Lerp(left, right, blend);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetLerp3))]
        public void Lerp_TestLerp3(int3 left, int3 right, float3 blend, float3 expected)
        {
            var actual = int3.Lerp(left, right, blend);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region CalculateAngle

        [Fact]
        public void CalculateAngle_90Degrees()
        {
            var x = new int3(1, 0, 0);
            var y = new int3(0, 1, 0);

            var actual = int3.CalculateAngle(x, y);

            Assert.Equal(M.DegreesToRadians(90), actual);
        }

        #endregion

        #region Rotate

        [Theory]
        [MemberData(nameof(GetEuler))]
        public void Rotate_Euler(float3 euler, int3 vec, float3 expected)
        {
            var actual = int3.Rotate(euler, vec, true);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetQuaternion))]
        public void Rotate_Quaternion(Quaternion quat, int3 vec, float3 expected)
        {
            var actual = int3.Rotate(quat, vec);

            Assert.Equal(expected, actual);
        }

        #endregion

        [Theory]
        [MemberData(nameof(GetStep))]
        public void Step(int3 edge, int3 val, int3 expected)
        {
            Assert.Equal(expected, int3.Step(edge, val));
        }

        #endregion

        #region Swizzle

        [Fact]
        public void Swizzle_int2_Get()
        {
            var actual = new int3(1, 2, 3);

            Assert.Equal(new int2(1, 2), actual.xy);
            Assert.Equal(new int2(1, 3), actual.xz);
            Assert.Equal(new int2(2, 1), actual.yx);
            Assert.Equal(new int2(2, 3), actual.yz);
            Assert.Equal(new int2(3, 1), actual.zx);
            Assert.Equal(new int2(3, 2), actual.zy);
        }

        [Fact]
        public void Swizzle_int2_Set()
        {
            var actual = new int3
            {
                xy = new int2(1, 2)
            };
            Assert.Equal(new int3(1, 2, 0), actual);

            actual.xz = new int2(3, 1);
            Assert.Equal(new int3(3, 2, 1), actual);

            actual.yx = new int2(3, 2);
            Assert.Equal(new int3(2, 3, 1), actual);

            actual.yz = new int2(1, 3);
            Assert.Equal(new int3(2, 1, 3), actual);

            actual.zx = new int2(2, 3);
            Assert.Equal(new int3(3, 1, 2), actual);

            actual.zy = new int2(1, 2);
            Assert.Equal(new int3(3, 2, 1), actual);
        }

        [Fact]
        public void Swizzle_int3_Get()
        {
            var actual = new int3(1, 2, 3);

            Assert.Equal(new int3(1, 2, 3), actual.xyz);
            Assert.Equal(new int3(1, 3, 2), actual.xzy);
            Assert.Equal(new int3(2, 3, 1), actual.yzx);
            Assert.Equal(new int3(2, 1, 3), actual.yxz);
            Assert.Equal(new int3(3, 1, 2), actual.zxy);
            Assert.Equal(new int3(3, 2, 1), actual.zyx);
        }

        [Fact]
        public void Swizzle_int3_Set()
        {
            var actual = new int3
            {
                xyz = new int3(1, 2, 3)
            };
            Assert.Equal(new int3(1, 2, 3), actual);

            actual.xzy = new int3(1, 2, 3);
            Assert.Equal(new int3(1, 3, 2), actual);

            actual.yzx = new int3(1, 2, 3);
            Assert.Equal(new int3(3, 1, 2), actual);

            actual.yxz = new int3(1, 2, 3);
            Assert.Equal(new int3(2, 1, 3), actual);

            actual.zxy = new int3(1, 2, 3);
            Assert.Equal(new int3(2, 3, 1), actual);

            actual.zyx = new int3(1, 2, 3);
            Assert.Equal(new int3(3, 2, 1), actual);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_TwoVectors_Operator(int3 left, int3 right, int3 expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_VectorScalar_Operator(int3 left, int3 right, int3 expected)
        {
            var actual = left + right.x;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_TwoVectors_Operator(int3 left, int3 right, int3 expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_VectorScalar_Operator(int3 left, int3 right, int3 expected)
        {
            var actual = left - right.x;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UnaryNegation_IsNegative()
        {
            var vec = new int3(1, 1, 1);

            var actual = -vec;

            Assert.Equal(new int3(-1, -1, -1), actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Operator(int3 vec, int scale, int3 expected)
        {
            var actual = vec * scale;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_ScalarVector_Operator(int3 vec, int scale, int3 expected)
        {
            var actual = scale * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_Operator(int3 left, int scale, int3 expected)
        {
            var right = new int3(scale, scale, scale);

            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_Operator(int3 vec, int scale, int3 expected)
        {
            var actual = vec / scale;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new int3(1, 1, 1);
            var b = new int3(1, 1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new int3(1, 1, 1);
            var b = new int3(0, 0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new int3(1, 1, 1);
            var b = new int3(1, 1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new int3(1, 1, 1);
            var b = new int3(0, 0, 0);

            Assert.True(a != b);
        }

        [Fact]
        public void Cast_FromDouble3()
        {
            var vec = new double3(1, 2, 3);

            var actual = (int3)vec;

            Assert.Equal(new int3(1, 2, 3), actual);
        }

        #endregion

        #region Overrides

        //TODO: GetHashCode
        //TODO: Equals(obj)

        #endregion

        #region Color

        [Fact]
        public void Color_Get()
        {
            var vec = new int3(1, 2, 3);

            Assert.Equal(1, vec.r);
            Assert.Equal(2, vec.g);
            Assert.Equal(3, vec.b);
        }

        [Fact]
        public void Color_Set()
        {
            var actual = new int3
            {
                r = 1
            };
            Assert.Equal(new int3(1, 0, 0), actual);

            actual.g = 2;
            Assert.Equal(new int3(1, 2, 0), actual);

            actual.b = 3;
            Assert.Equal(new int3(1, 2, 3), actual);
        }

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new int3(4, 0, 0), new float3(1, 0, 0) };
            yield return new object[] { new int3(0, 4, 0), new float3(0, 1, 0) };
            yield return new object[] { new int3(0, 0, 4), new float3(0, 0, 1) };
            yield return new object[]
            {
                new int3(1, 1, 1),
                new float3((float)System.Math.Sqrt(1d / 3d), (float)System.Math.Sqrt(1d / 3d), (float)System.Math.Sqrt(1d / 3d))
            };
        }

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new int3(0, 0, 0);
            var one = new int3(1, 1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { one, one, new int3(2, 2, 2) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new int3(0, 0, 0);
            var one = new int3(1, 1, 1);

            yield return new object[] { one, one, zero };
            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, -one };
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new int3(1, 1, 1);

            yield return new object[] { one, 1, one };
            yield return new object[] { one, 2, new int3(2, 2, 2) };
            yield return new object[] { one, 0, new int3(0, 0, 0) };
        }

        public static IEnumerable<object[]> GetDivide()
        {
            var one = new int3(1, 1, 1);

            yield return new object[] { new int3(2, 2, 2), 2, one };
            yield return new object[] { one, 1, one };
        }

        public static IEnumerable<object[]> GetComponentMin()
        {
            var one = new int3(1, 1, 1);
            var zero = new int3(0, 0, 0);

            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
            yield return new object[] { new int3(1, 0, 1), new int3(0, 1, 0), zero };
        }

        public static IEnumerable<object[]> GetComponentMax()
        {
            var one = new int3(1, 1, 1);
            var zero = new int3(0, 0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { new int3(1, 0, 1), new int3(0, 1, 0), one };
        }

        public static IEnumerable<object[]> GetMin()
        {
            var one = new int3(1, 1, 1);
            var zero = new int3(0, 0, 0);

            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
        }

        public static IEnumerable<object[]> GetMax()
        {
            var one = new int3(1, 1, 1);
            var zero = new int3(0, 0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
        }

        public static IEnumerable<object[]> GetClamp()
        {
            var one = new int3(1, 1, 1);
            var zero = new int3(0, 0, 0);

            yield return new object[] { new int3(2, 2, 2), zero, one, one };
            yield return new object[] { new int3(-1, -1, -1), zero, one, zero };
            yield return new object[] { new int3(5, 5, 5), zero, one, new int3(1, 1, 1) };
        }

        public static IEnumerable<object[]> GetLerp()
        {
            var one = new int3(1, 1, 1);
            var zero = new int3(0, 0, 0);

            yield return new object[] { zero, one, 0.5f, new float3(0.5f, 0.5f, 0.5f) };
            yield return new object[] { zero, one, 0f, float3.Zero };
            yield return new object[] { zero, one, 1f, float3.One };
        }

        public static IEnumerable<object[]> GetLerp3()
        {
            var one = new int3(1, 1, 1);
            var zero = new int3(0, 0, 0);

            yield return new object[] { zero, one, new float3(0.5f, 0.5f, 0.5f), new float3(0.5f, 0.5f, 0.5f) };
            yield return new object[] { zero, one, float3.Zero, float3.Zero };
            yield return new object[] { zero, one, float3.One, float3.One };
        }

        public static IEnumerable<object[]> GetEuler()
        {
            var x = new int3(1, 0, 0);
            var y = new int3(0, 1, 0);
            var z = new int3(0, 0, 1);

            var xF = new float3(1, 0, 0);
            var yF = new float3(0, 1, 0);
            var zF = new float3(0, 0, 1);

            yield return new object[] { new float3(90, 0, 0), y, zF };
            yield return new object[] { new float3(0, 90, 0), z, xF };
            yield return new object[] { new float3(0, 0, 90), x, yF };
        }

        public static IEnumerable<object[]> GetQuaternion()
        {
            var x = new int3(1, 0, 0);
            var y = new int3(0, 1, 0);
            var z = new int3(0, 0, 1);

            var xF = new float3(1, 0, 0);
            var yF = new float3(0, 1, 0);
            var zF = new float3(0, 0, 1);

            var xRot = new Quaternion((float)System.Math.Sqrt(0.5), 0, 0, (float)System.Math.Sqrt(0.5));
            var yRot = new Quaternion(0, (float)System.Math.Sqrt(0.5), 0, (float)System.Math.Sqrt(0.5));
            var zRot = new Quaternion(0, 0, (float)System.Math.Sqrt(0.5), (float)System.Math.Sqrt(0.5));

            yield return new object[] { xRot, y, zF };
            yield return new object[] { yRot, z, xF };
            yield return new object[] { zRot, x, yF };
        }

        public static IEnumerable<object[]> GetStep()
        {
            var x = new int3(2, 2, 2);
            var y = new int3(1, 1, 1);
            yield return new object[] { x, y, int3.Zero };
            yield return new object[] { y, x, int3.One };
        }

        #endregion

        #region ToString/Parse

        [Fact]
        public void ToString_NoCulture()
        {
            Assert.NotNull(new int3().ToString());
        }

        [Fact]
        public void ToString_InvariantCulture()
        {
            string s = "(2, 2, 2)";
            int3 f = int3.One * 2;

            Assert.Equal(s, f.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "(2; 2; 2)";
            int3 f = int3.One * 2;

            Assert.Equal(s, f.ToString(new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_InvariantCulture()
        {
            string s = "(2, 2, 2)";
            int3 f = int3.One * 2;

            Assert.Equal(f, int3.Parse(s, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_CultureDE()
        {
            string s = "(2; 2; 2)";
            int3 f = int3.One * 2;

            Assert.Equal(f, int3.Parse(s, new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_ToString_NoCulture()
        {
            int3 f = int3.One * 2;

            Assert.Equal(f, int3.Parse(f.ToString()));
        }

        [Fact]
        public void Parse_ToString_InvariantCulture()
        {
            int3 f = int3.One * 2;

            Assert.Equal(f, int3.Parse(f.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_ToString_CultureDE()
        {
            int3 f = int3.One * 2;

            Assert.Equal(f, int3.Parse(f.ToString(new CultureInfo("de-DE")), new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_Exception()
        {
            string s = "Fusee";

            Assert.Throws<FormatException>(() => int3.Parse(s));
        }

        #endregion ToString/Parse
    }
}