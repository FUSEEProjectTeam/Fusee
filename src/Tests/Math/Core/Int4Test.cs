using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class Int4Test
    {
        #region Fields

        [Fact]
        public void UnitX_IsUnit()
        {
            var expected = new int4(1, 0, 0, 0);

            Assert.Equal(expected, int4.UnitX);
        }

        [Fact]
        public void UnitY_IsUnit()
        {
            var expected = new int4(0, 1, 0, 0);

            Assert.Equal(expected, int4.UnitY);
        }

        [Fact]
        public void UnitZ_IsUnit()
        {
            var expected = new int4(0, 0, 1, 0);

            Assert.Equal(expected, int4.UnitZ);
        }

        [Fact]
        public void UnitW_IsUnit()
        {
            var expected = new int4(0, 0, 0, 1);

            Assert.Equal(expected, int4.UnitW);
        }

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new int4(0, 0, 0, 0);

            Assert.Equal(expected, int4.Zero);
        }

        [Fact]
        public void One_IsOne()
        {
            var expected = new int4(1, 1, 1, 1);

            Assert.Equal(expected, int4.One);
        }

        #endregion

        #region Constructors

        [Fact]
        public void Constructor_Fromint2()
        {
            var vec = new int2(1, 2);

            var actual = new int4(vec);

            Assert.Equal(new int4(1, 2, 0, 0), actual);
        }

        [Fact]
        public void Constructor_Fromint3()
        {
            var vec = new int3(1, 2, 3);

            var actual = new int4(vec);

            Assert.Equal(new int4(1, 2, 3, 0), actual);
        }

        [Fact]
        public void Constructor_Fromint3Scale()
        {
            var vec = new int3(1, 2, 3);

            var actual = new int4(vec, 4);

            Assert.Equal(new int4(1, 2, 3, 4), actual);
        }

        [Fact]
        public void Constructor_Fromint4()
        {
            var vec = new int4(1, 2, 3, 4);

            var actual = new int4(vec);

            Assert.Equal(new int4(1, 2, 3, 4), actual);
        }

        [Fact]
        public void Constructor_FromDouble4()
        {
            var vec = new double4(1, 2, 3, 4);

            var actual = new int4(vec);

            Assert.Equal(new int4(1, 2, 3, 4), actual);
        }

        #endregion

        #region Instance

        [Fact]
        public void Length_Is2()
        {
            var vec = new int4(1, 1, 1, 1);

            var actual = vec.Length;

            Assert.Equal(2, actual);
        }

        [Fact]
        public void Length1_Is4()
        {
            var vec = new int4(1, 1, 1, 1);

            var actual = vec.Length1;

            Assert.Equal(4, actual);
        }

        [Fact]
        public void LengthSquared_Is4()
        {
            var vec = new int4(1, 1, 1, 1);

            var actual = vec.LengthSquared;

            Assert.Equal(4, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(int4 vec, float4 expected)
        {
            var actual = vec.Normalize();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Instance(int4 vec, float4 expected)
        {
            var actual = vec.NormalizeFast();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Normalize1_Instance()
        {
            var vec = new int4(1, 1, 1, 1);

            var actual = vec.Normalize1();

            Assert.Equal(new float4(0.25f, 0.25f, 0.25f, 0.25f), actual);
        }

        [Fact]
        public void ToArray_IsArray()
        {
            var vec = new int4(1, 2, 3, 4);

            var actual = vec.ToArray();

            Assert.Equal(new int[] { 1, 2, 3, 4 }, actual);
        }

        #endregion

        #region this

        [Fact]
        public void This_GetWithIdx_IsValid()
        {
            var actual = new int4(3, 7, 8, 1);

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);

            Assert.Equal(8, actual[2]);

            Assert.Equal(1, actual[3]);
        }

        [Fact]
        public void This_SetWithIdx_IsValid()
        {
            var actual = new int4(0, 0, 0, 0);
            actual[0] = 3;
            actual[1] = 7;
            actual[2] = 8;
            actual[3] = 1;

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);

            Assert.Equal(8, actual[2]);

            Assert.Equal(1, actual[3]);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_GetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => new int4(0, 0, 0, 0)[idx]);

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_SetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var f4 = new int4(0, 0, 0, 0); f4[idx] = 10; });

            Assert.Equal(expected, actual.ParamName);
        }

        public static IEnumerable<object[]> ThisException()
        {
            yield return new object[] { 7, "Index 7 not eligible for a int4 type" };
            yield return new object[] { 6, "Index 6 not eligible for a int4 type" };
        }

        #endregion

        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Static(int4 left, int4 right, int4 expected)
        {
            var actual = int4.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Static(int4 left, int4 right, int4 expected)
        {
            var actual = int4.Subtract(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Static(int4 vec, int scale, int4 expected)
        {
            var actual = int4.Multiply(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_Static(int4 left, int scale, int4 expected)
        {
            var right = new int4(scale, scale, scale, scale);

            var actual = int4.Multiply(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_Vector_Scalar_Static(int4 vec, int scale, int4 expected)
        {
            var actual = int4.Divide(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_TwoVectors_Static(int4 left, int scale, int4 expected)
        {
            var right = new int4(scale, scale, scale, scale);

            var actual = int4.Divide(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region MinMax

        [Theory]
        [MemberData(nameof(GetMin))]
        public void Min_IsZero(int4 left, int4 right, int4 expected)
        {
            var actual = int4.Min(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMax))]
        public void Max_IsOne(int4 left, int4 right, int4 expected)
        {
            var actual = int4.Max(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Clamp

        [Theory]
        [MemberData(nameof(GetClamp))]
        public void Clamp_TestClamp(int4 vec, int4 min, int4 max, int4 expected)
        {
            var actual = int4.Clamp(vec, min, max);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Normalize

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Static(int4 vec, float4 expected)
        {
            var actual = int4.Normalize(vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Static(int4 vec, float4 expected)
        {
            var actual = int4.NormalizeFast(vec);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Normalize1_Static()
        {
            var vec = new int4(1, 1, 1, 1);

            var actual = vec.Normalize1();

            Assert.Equal(new float4(0.25f, 0.25f, 0.25f, 0.25f), actual);
        }

        #endregion

        #region Dot

        [Fact]
        public void Dot_Is20()
        {
            var a = new int4(1, 2, 3, 4);
            var b = new int4(4, 3, 2, 1);

            var actual = int4.Dot(a, b);

            Assert.Equal(20, actual);
        }

        #endregion

        #region Lerp

        [Theory]
        [MemberData(nameof(GetLerp))]
        public void Lerp_TestLerp(int4 left, int4 right, float blend, float4 expected)
        {
            float4 actual = int4.Lerp(left, right, blend);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetLerp4))]
        public void Lerp_TestLerp4(int4 left, int4 right, float4 blend, float4 expected)
        {
            float4 actual = int4.Lerp(left, right, blend);

            Assert.Equal(expected, actual);
        }

        #endregion

        #endregion

        #region Swizzle

        [Fact]
        public void Swizzle_Get()
        {
            var vec = new int4(1, 2, 3, 4);

            Assert.Equal(new int2(1, 2), vec.xy);
            Assert.Equal(new int3(1, 2, 3), vec.xyz);
        }

        [Fact]
        public void Swizzle_Set()
        {
            var actual = new int4(0, 0, 0, 0)
            {
                xy = new int2(1, 2)
            };
            Assert.Equal(new int4(1, 2, 0, 0), actual);

            actual.xyz = new int3(3, 3, 3);
            Assert.Equal(new int4(3, 3, 3, 0), actual);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Operator(int4 left, int4 right, int4 expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Operator(int4 left, int4 right, int4 expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UnaryNegation_isNegative()
        {
            var vec = new int4(1, 1, 1, 1);

            var actual = -vec;

            Assert.Equal(new int4(-1, -1, -1, -1), actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Operator(int4 vec, int scale, int4 expected)
        {
            var actual = vec * scale;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_ScalarVector_Operator(int4 vec, int scale, int4 expected)
        {
            var actual = scale * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_Operator(int4 left, int scale, int4 expected)
        {
            var right = new int4(scale, scale, scale, scale);

            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_Operator(int4 vec, int scale, int4 expected)
        {
            var actual = vec / scale;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new int4(1, 1, 1, 1);
            var b = new int4(1, 1, 1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new int4(1, 1, 1, 1);
            var b = new int4(0, 0, 0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new int4(1, 1, 1, 1);
            var b = new int4(1, 1, 1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new int4(1, 1, 1, 1);
            var b = new int4(0, 0, 0, 0);

            Assert.True(a != b);
        }

        [Fact]
        public void Cast_FromDouble4()
        {
            var vec = new double4(1, 1, 1, 1);

            var actual = (int4)vec;

            Assert.Equal(new int4(1, 1, 1, 1), actual);
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
            var actual = new int4(1, 2, 3, 4);

            Assert.Equal(1, actual.r);
            Assert.Equal(2, actual.g);
            Assert.Equal(3, actual.b);
            Assert.Equal(4, actual.a);
            Assert.Equal(new int2(1, 2), actual.rg);
            Assert.Equal(new int3(1, 2, 3), actual.rgb);
        }

        [Fact]
        public void Color_Set()
        {
            var actual = new int4
            {
                r = 1,
                g = 2,
                b = 3,
                a = 4
            };
            Assert.Equal(new int4(1, 2, 3, 4), actual);

            actual.rg = new int2(2, 1);
            Assert.Equal(new int4(2, 1, 3, 4), actual);

            actual.rgb = new int3(3, 2, 1);
            Assert.Equal(new int4(3, 2, 1, 4), actual);
        }

        #endregion

        [Theory]
        [MemberData(nameof(GetStep))]
        public void Step(int4 edge, int4 val, int4 expected)
        {
            Assert.Equal(expected, int4.Step(edge, val));
        }

        #region IEnumerables

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new int4(4, 0, 0, 0), new float4(1, 0, 0, 0) };
            yield return new object[] { new int4(0, 4, 0, 0), new float4(0, 1, 0, 0) };
            yield return new object[] { new int4(0, 0, 4, 0), new float4(0, 0, 1, 0) };
            yield return new object[] { new int4(0, 0, 0, 4), new float4(0, 0, 0, 1) };
            yield return new object[] { new int4(1, 1, 1, 1), new float4(0.5f, 0.5f, 0.5f, 0.5f) };
        }

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new int4(0, 0, 0, 0);
            var one = new int4(1, 1, 1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { one, one, new int4(2, 2, 2, 2) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new int4(0, 0, 0, 0);
            var one = new int4(1, 1, 1, 1);

            yield return new object[] { one, one, zero };
            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, -one };
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new int4(1, 1, 1, 1);

            yield return new object[] { one, 1, one };
            yield return new object[] { one, 2, new int4(2, 2, 2, 2) };
            yield return new object[] { one, 0, new int4(0, 0, 0, 0) };
        }

        public static IEnumerable<object[]> GetDivide()
        {
            var one = new int4(1, 1, 1, 1);

            yield return new object[] { new int4(2, 2, 2, 2), 2, one };
            yield return new object[] { one, 1, one };
        }

        public static IEnumerable<object[]> GetMin()
        {
            var one = new int4(1, 1, 1, 1);
            var zero = new int4(0, 0, 0, 0);

            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
        }

        public static IEnumerable<object[]> GetMax()
        {
            var one = new int4(1, 1, 1, 1);
            var zero = new int4(0, 0, 0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
        }

        public static IEnumerable<object[]> GetClamp()
        {
            var one = new int4(1, 1, 1, 1);
            var zero = new int4(0, 0, 0, 0);

            yield return new object[] { new int4(2, 2, 2, 2), zero, one, one };
            yield return new object[] { new int4(-1, -1, -1, -1), zero, one, zero };
            yield return new object[] { new int4(1, 1, 1, 1), zero, one, new int4(1, 1, 1, 1) };
        }

        public static IEnumerable<object[]> GetLerp()
        {
            var one = new int4(1, 1, 1, 1);
            var zero = new int4(0, 0, 0, 0);

            yield return new object[] { zero, one, 0.5f, new float4(0.5f, 0.5f, 0.5f, 0.5f) };
            yield return new object[] { zero, one, 0f, float4.Zero };
            yield return new object[] { zero, one, 1f, float4.One };
        }

        public static IEnumerable<object[]> GetLerp4()
        {
            var one = new int4(1, 1, 1, 1);
            var zero = new int4(0, 0, 0, 0);

            yield return new object[] { zero, one, new float4(0.5f, 0.5f, 0.5f, 0.5f), new float4(0.5f, 0.5f, 0.5f, 0.5f) };
            yield return new object[] { zero, one, float4.Zero, float4.Zero };
            yield return new object[] { zero, one, float4.One, float4.One };
        }

        public static IEnumerable<object[]> GetStep()
        {
            var x = new int4(2, 2, 2, 2);
            var y = new int4(1, 1, 1, 1);
            yield return new object[] { x, y, int4.Zero };
            yield return new object[] { y, x, int4.One };
        }

        #endregion

        #region ToString/Parse

        [Fact]
        public void ToString_NoCulture()
        {
            Assert.NotNull(new int4().ToString());
        }

        [Fact]
        public void ToString_InvariantCulture()
        {
            string s = "(2, 2, 2, 2)";
            int4 f = int4.One * 2;

            Assert.Equal(s, f.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "(2; 2; 2; 2)";
            int4 f = int4.One * 2;

            Assert.Equal(s, f.ToString(new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_InvariantCulture()
        {
            string s = "(2, 2, 2, 2)";
            int4 f = int4.One * 2;

            Assert.Equal(f, int4.Parse(s, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_CultureDE()
        {
            string s = "(2; 2; 2; 2)";
            int4 f = int4.One * 2;

            Assert.Equal(f, int4.Parse(s, new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_ToString_NoCulture()
        {
            int4 f = int4.One * 2;

            Assert.Equal(f, int4.Parse(f.ToString()));
        }

        [Fact]
        public void Parse_ToString_InvariantCulture()
        {
            int4 f = int4.One * 2;

            Assert.Equal(f, int4.Parse(f.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_ToString_CultureDE()
        {
            int4 f = int4.One * 2;

            Assert.Equal(f, int4.Parse(f.ToString(new CultureInfo("de-DE")), new CultureInfo("de-DE")));
        }

        //[Fact]
        //public void Parse_Exception()
        //{
        //    string s = "Fusee";

        //    Assert.Throws<FormatException>(() => int4.Parse(s));
        //}

        #endregion ToString/Parse
    }
}