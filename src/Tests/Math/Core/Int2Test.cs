using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class Int2Test
    {
        #region Fields

        [Fact]
        public void UnitX_IsUnit()
        {
            var expected = new int2(1, 0);

            Assert.Equal(expected, int2.UnitX);
        }

        [Fact]
        public void UnitY_IsUnit()
        {
            var expected = new int2(0, 1);

            Assert.Equal(expected, int2.UnitY);
        }

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new int2(0, 0);

            Assert.Equal(expected, int2.Zero);
        }

        [Fact]
        public void One_IsOne()
        {
            var expected = new int2(1, 1);

            Assert.Equal(expected, int2.One);
        }

        #endregion

        #region this

        [Fact]
        public void This_GetWithIdx_IsValid()
        {
            var actual = new int2(3, 7);

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);
        }

        [Fact]
        public void This_SetWithIdx_IsValid()
        {
            var actual = new int2(0, 0);
            actual[0] = 3;
            actual[1] = 7;

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_GetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => new int2(10, 0)[idx]);

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_SetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var f2 = new int2(10, 0); f2[idx] = 10; });

            Assert.Equal(expected, actual.ParamName);
        }

        public static IEnumerable<object[]> ThisException()
        {
            yield return new object[] { 7, "Index 7 not eligible for a int2 type" };
            yield return new object[] { 6, "Index 6 not eligible for a int2 type" };
        }

        #endregion

        #region Instance

        [Fact]
        public void Length_IsOne()
        {
            var vec = new int2(1, 0);

            var actual = vec.Length;

            Assert.Equal(1, actual);
        }

        [Fact]
        public void LengthSquared_IsTwo()
        {
            var vec = new int2(1, 1);

            var actual = vec.LengthSquared;

            Assert.Equal(2, actual);
        }

        [Fact]
        public void PerpendicularRight_IsNegativeY()
        {
            var vec = new int2(1, 0);

            var actual = vec.PerpendicularRight;

            Assert.Equal(new int2(0, -1), actual);
        }

        [Fact]
        public void PerpendicularLeft_IsNegativeX()
        {
            var vec = new int2(0, 1);

            var actual = vec.PerpendicularLeft;

            Assert.Equal(new int2(-1, 0), actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(int2 vec, float2 expected)
        {
            var actual = vec.Normalize();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Instance(int2 vec, float2 expected)
        {
            var actual = vec.NormalizeFast();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToArray_IsArray()
        {
            var vec = new int2(1, 2);

            var actual = vec.ToArray();

            Assert.Equal(new int[] { 1, 2 }, actual);
        }

        #endregion

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]

        public void Add_Static(int2 left, int2 right, int2 expected)
        {
            var actual = int2.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Static(int2 left, int2 right, int2 expected)
        {
            var actual = int2.Subtract(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Static(int2 vec, int scale, int2 expected)
        {
            var actual = int2.Multiply(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_Static(int2 left, int scale, int2 expected)
        {
            var right = new int2(scale, scale);

            var actual = int2.Multiply(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_VectorScalar_Static(int2 vec, int scale, int2 expected)
        {
            var actual = int2.Divide(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_TwoVectors_Static(int2 left, int scale, int2 expected)
        {
            var right = new int2(scale, scale);

            var actual = int2.Divide(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region MinMax

        [Theory]
        [MemberData(nameof(GetComponentMin))]
        public void ComponentMin_IsZero(int2 left, int2 right, int2 expected)
        {
            var actual = int2.ComponentMin(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetComponentMax))]
        public void ComponentMax_IsOne(int2 left, int2 right, int2 expected)
        {
            var actual = int2.ComponentMax(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMin))]
        public void Min_IsZero(int2 left, int2 right, int2 expected)
        {
            var actual = int2.Min(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMax))]
        public void Max_IsOne(int2 left, int2 right, int2 expected)
        {
            var actual = int2.Max(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Clamp

        [Theory]
        [MemberData(nameof(GetClamp))]
        public void Clamp_TestClamp(int2 vec, int2 min, int2 max, int2 expected)
        {
            var actual = int2.Clamp(vec, min, max);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Normalize

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Static(int2 vec, float2 expected)
        {
            var actual = int2.Normalize(vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Static(int2 vec, float2 expected)
        {
            var actual = int2.NormalizeFast(vec);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Dot

        [Fact]
        public void Dot_Is2()
        {
            var vec = new int2(1, 1);

            var actual = int2.Dot(vec, vec);

            Assert.Equal(2, actual);
        }

        #endregion

        [Theory]
        [MemberData(nameof(GetStep))]
        public void Step(int2 edge, int2 val, int2 expected)
        {
            Assert.Equal(expected, int2.Step(edge, val));
        }

        #region Lerp

        [Theory]
        [MemberData(nameof(GetLerp))]
        public void Lerp_TestLerp(int2 left, int2 right, float blend, float2 expected)
        {
            var actual = int2.Lerp(left, right, blend);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetLerp2))]
        public void Lerp_TestLerp2(int2 left, int2 right, float2 blend, float2 expected)
        {
            var actual = int2.Lerp(left, right, blend);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Swizzle

        [Fact]
        public void Swizzle_Get()
        {
            var vec = new int2(1, 2);

            Assert.Equal(new int2(1, 2), vec.xy);
            Assert.Equal(new int2(2, 1), vec.yx);
        }

        [Fact]
        public void Swizzle_Set()
        {
            var actual = new int2();
            var vec = new int2(1, 2);

            actual.xy = vec;

            Assert.Equal(new int2(1, 2), actual);

            actual.yx = vec;

            Assert.Equal(new int2(2, 1), actual);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Operator(int2 left, int2 right, int2 expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Operator(int2 left, int2 right, int2 expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UnaryNegation_IsNegative()
        {
            var vec = new int2(1, 1);

            var actual = -vec;

            Assert.Equal(new int2(-1, -1), actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Operator(int2 vec, int scale, int2 expected)
        {
            var actual = vec * scale;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_ScalarVector_Operator(int2 vec, int scale, int2 expected)
        {
            var actual = scale * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_Operator(int2 left, int scale, int2 expected)
        {
            var right = new int2(scale, scale);

            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_VectorScalar_Operator(int2 vec, int scale, int2 expected)
        {
            var actual = vec / scale;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new int2(1, 1);
            var b = new int2(1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new int2(1, 1);
            var b = new int2(0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inquality_IsEqual()
        {
            var a = new int2(1, 1);
            var b = new int2(1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new int2(1, 1);
            var b = new int2(0, 0);

            Assert.True(a != b);
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
            var vec = new int2(1, 2);

            Assert.Equal(new int2(1, 2), vec.rg);
            Assert.Equal(1, vec.r);
            Assert.Equal(2, vec.g);
        }

        [Fact]
        public void Color_Set()
        {
            var vec = new int2
            {
                rg = new int2(1, 2)
            };

            Assert.Equal(new int2(1, 2), vec);

            vec.r = 0;
            vec.g = 1;

            Assert.Equal(new int2(0, 1), vec);
        }

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new int2(4, 0), new float2(1, 0) };
            yield return new object[] { new int2(0, 4), new float2(0, 1) };
            yield return new object[] { new int2(1, 1), new float2((float)System.Math.Sqrt(0.5), (float)System.Math.Sqrt(0.5)) };
        }

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new int2(0, 0);
            var one = new int2(1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { one, one, new int2(2, 2) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new int2(0, 0);
            var one = new int2(1, 1);

            yield return new object[] { one, one, zero };
            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, -one };
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new int2(1, 1);

            yield return new object[] { one, 1, one };
            yield return new object[] { one, 2, new int2(2, 2) };
            yield return new object[] { one, 0, new int2(0, 0) };
        }

        public static IEnumerable<object[]> GetDivide()
        {
            var one = new int2(1, 1);

            yield return new object[] { new int2(2, 2), 2, one };
            yield return new object[] { one, 1, one };
        }

        public static IEnumerable<object[]> GetComponentMin()
        {
            var one = new int2(1, 1);
            var zero = new int2(0, 0);

            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
            yield return new object[] { new int2(1, 0), new int2(0, 1), zero };
        }

        public static IEnumerable<object[]> GetComponentMax()
        {
            var one = new int2(1, 1);
            var zero = new int2(0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { new int2(1, 0), new int2(0, 1), one };
        }

        public static IEnumerable<object[]> GetMin()
        {
            var one = new int2(1, 1);
            var zero = new int2(0, 0);

            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
        }

        public static IEnumerable<object[]> GetMax()
        {
            var one = new int2(1, 1);
            var zero = new int2(0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
        }

        public static IEnumerable<object[]> GetClamp()
        {
            var one = new int2(1, 1);
            var zero = new int2(0, 0);

            yield return new object[] { new int2(2, 2), zero, one, one };
            yield return new object[] { new int2(-1, -1), zero, one, zero };
            yield return new object[] { new int2(5, 5), zero, one, new int2(1, 1) };
        }

        public static IEnumerable<object[]> GetLerp()
        {
            var one = new int2(1, 1);
            var zero = new int2(0, 0);

            yield return new object[] { zero, one, 0.5f, new float2(0.5f, 0.5f) };
            yield return new object[] { zero, one, 0f, float2.Zero };
            yield return new object[] { zero, one, 1f, float2.One };
        }

        public static IEnumerable<object[]> GetLerp2()
        {
            var one = new int2(1, 1);
            var zero = new int2(0, 0);

            yield return new object[] { zero, one, new float2(0.5f, 0.5f), new float2(0.5f, 0.5f) };
            yield return new object[] { zero, one, float2.Zero, float2.Zero };
            yield return new object[] { zero, one, float2.One, float2.One };
        }

        public static IEnumerable<object[]> GetStep()
        {
            var x = new int2(2, 2);
            var y = new int2(1, 1);
            yield return new object[] { x, y, int2.Zero };
            yield return new object[] { y, x, int2.One };
        }

        #endregion

        #region ToString/Parse

        [Fact]
        public void ToString_NoCulture()
        {
            Assert.NotNull(new int2().ToString());
        }

        [Fact]
        public void ToString_InvariantCulture()
        {
            string s = "(2, 2)";
            int2 f = int2.One * 2;

            Assert.Equal(s, f.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "(2; 2)";
            int2 f = int2.One * 2;

            Assert.Equal(s, f.ToString(new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_InvariantCulture()
        {
            string s = "(2, 2)";
            int2 f = int2.One * 2;

            Assert.Equal(f, int2.Parse(s, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_CultureDE()
        {
            string s = "(2; 2)";
            int2 f = int2.One * 2;

            Assert.Equal(f, int2.Parse(s, new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_ToString_NoCulture()
        {
            int2 f = int2.One * 2;

            Assert.Equal(f, int2.Parse(f.ToString()));
        }

        [Fact]
        public void Parse_ToString_InvariantCulture()
        {
            int2 f = int2.One * 2;

            Assert.Equal(f, int2.Parse(f.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_ToString_CultureDE()
        {
            int2 f = int2.One * 2;

            Assert.Equal(f, int2.Parse(f.ToString(new CultureInfo("de-DE")), new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_Exception()
        {
            string s = "Fusee";

            Assert.Throws<FormatException>(() => int2.Parse(s));
        }

        #endregion ToString/Parse
    }
}