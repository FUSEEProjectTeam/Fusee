using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class Double2Test
    {
        #region Fields

        [Fact]
        public void UnitX_IsUnit()
        {
            var expected = new double2(1, 0);

            Assert.Equal(expected, double2.UnitX);
        }

        [Fact]
        public void UnitY_IsUnit()
        {
            var expected = new double2(0, 1);

            Assert.Equal(expected, double2.UnitY);
        }

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new double2(0, 0);

            Assert.Equal(expected, double2.Zero);
        }

        [Fact]
        public void One_IsOne()
        {
            var expected = new double2(1, 1);

            Assert.Equal(expected, double2.One);
        }

        #endregion

        #region Instance

        [Fact]
        public void Length_IsOne()
        {
            var vec = new double2(1, 0);

            var actual = vec.Length;

            Assert.Equal(1, actual);
        }

        [Fact]
        public void LengthSquared_IsTwo()
        {
            var vec = new double2(1, 1);

            var actual = vec.LengthSquared;

            Assert.Equal(2, actual);
        }

        [Fact]
        public void PerpendicularRight_IsNegativeY()
        {
            var vec = new double2(1, 0);

            var actual = vec.PerpendicularRight;

            Assert.Equal(new double2(0, -1), actual);
        }

        [Fact]
        public void PerpendicularLeft_IsNegativeX()
        {
            var vec = new double2(0, 1);

            var actual = vec.PerpendicularLeft;

            Assert.Equal(new double2(-1, 0), actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(double2 vec, double2 expected)
        {
            var actual = vec.Normalize();

            Assert.Equal(expected.x, actual.x, 14);
            Assert.Equal(expected.y, actual.y, 14);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Instance(double2 vec, double2 expected)
        {
            var actual = vec.NormalizeFast();

            Assert.Equal(expected.x, actual.x, 7);
            Assert.Equal(expected.y, actual.y, 7);
        }

        [Fact]
        public void ToArray_IsArray()
        {
            var vec = new double2(1, 2);

            var actual = vec.ToArray();

            Assert.Equal(new double[] { 1, 2 }, actual);
        }

        #endregion

        #region this

        [Fact]
        public void This_GetWithIdx_IsValid()
        {
            var actual = new double2(3, 7);

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);
        }

        [Fact]
        public void This_SetWithIdx_IsValid()
        {
            var actual = new double2(0, 0);
            actual[0] = 3;
            actual[1] = 7;

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_GetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => new double2(10, 0)[idx]);

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_SetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var d2 = new double2(10, 0); d2[idx] = 10; });

            Assert.Equal(expected, actual.ParamName);
        }

        public static IEnumerable<object[]> ThisException()
        {
            yield return new object[] { 7, "Index 7 not eligible for a double2 type" };
            yield return new object[] { 6, "Index 6 not eligible for a double2 type" };
        }

        #endregion

        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]

        public void Add_Static(double2 left, double2 right, double2 expected)
        {
            var actual = double2.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Static(double2 left, double2 right, double2 expected)
        {
            var actual = double2.Subtract(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Static(double2 vec, double scale, double2 expected)
        {
            var actual = double2.Multiply(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_Static(double2 left, double scale, double2 expected)
        {
            var right = new double2(scale, scale);

            var actual = double2.Multiply(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_VectorScalar_Static(double2 vec, double scale, double2 expected)
        {
            var actual = double2.Divide(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_TwoVectors_Static(double2 left, double scale, double2 expected)
        {
            var right = new double2(scale, scale);

            var actual = double2.Divide(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region MinMax

        [Theory]
        [MemberData(nameof(GetComponentMin))]
        public void ComponentMin_IsZero(double2 left, double2 right, double2 expected)
        {
            var actual = double2.ComponentMin(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetComponentMax))]
        public void ComponentMax_IsOne(double2 left, double2 right, double2 expected)
        {
            var actual = double2.ComponentMax(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMin))]
        public void Min_IsZero(double2 left, double2 right, double2 expected)
        {
            var actual = double2.Min(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMax))]
        public void Max_IsOne(double2 left, double2 right, double2 expected)
        {
            var actual = double2.Max(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Clamp

        [Theory]
        [MemberData(nameof(GetClamp))]
        public void Clamp_TestClamp(double2 vec, double2 min, double2 max, double2 expected)
        {
            var actual = double2.Clamp(vec, min, max);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Normalize

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Static(double2 vec, double2 expected)
        {
            var actual = double2.Normalize(vec);

            Assert.Equal(expected.x, actual.x, 14);
            Assert.Equal(expected.y, actual.y, 14);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Static(double2 vec, double2 expected)
        {
            var actual = double2.NormalizeFast(vec);

            Assert.Equal(expected.x, actual.x, 7);
            Assert.Equal(expected.y, actual.y, 7);
        }

        #endregion

        #region Dot

        [Fact]
        public void Dot_Is2()
        {
            var vec = new double2(1, 1);

            var actual = double2.Dot(vec, vec);

            Assert.Equal(2, actual);
        }

        #endregion

        #region Lerp

        [Theory]
        [MemberData(nameof(GetLerp))]
        public void Lerp_TestLerp(double2 left, double2 right, double blend, double2 expected)
        {
            var actual = double2.Lerp(left, right, blend);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Barycentric

        [Theory]
        [MemberData(nameof(GetBarycentric))]
        public void Barycentric_Edges(double2 a, double2 b, double2 c, double u, double v, double2 expected)
        {
            var actual = double2.Barycentric(a, b, c, u, v);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetBarycentric))]
        public void GetBarycentric_Edges(double2 a, double2 b, double2 c, double uExpected, double vExpected, double2 point)
        {
            double uActual;
            double vActual;

            double2.GetBarycentric(a, b, c, point, out uActual, out vActual);

            Assert.Equal(uExpected, uActual);
            Assert.Equal(vExpected, vActual);
        }

        [Fact]
        public void IsTriangleCW_IsCW()
        {
            var a = new double2(0, 0);
            var b = new double2(0, 1);
            var c = new double2(1, 1);

            Assert.True(double2.IsTriangleCW(a, b, c));
        }

        [Fact]
        public void IsTriangleCW_IsCCW()
        {
            var a = new double2(0, 0);
            var b = new double2(1, 0);
            var c = new double2(1, 1);

            Assert.False(double2.IsTriangleCW(a, b, c));
        }

        #endregion

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Operator(double2 left, double2 right, double2 expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Operator(double2 left, double2 right, double2 expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UnaryNegation_IsNegative()
        {
            var vec = new double2(1, 1);

            var actual = -vec;

            Assert.Equal(new double2(-1, -1), actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Operator(double2 vec, double scale, double2 expected)
        {
            var actual = vec * scale;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_ScalarVector_Operator(double2 vec, double scale, double2 expected)
        {
            var actual = scale * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_Operator(double2 left, double scale, double2 expected)
        {
            var right = new double2(scale, scale);

            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_VectorScalar_Operator(double2 vec, double scale, double2 expected)
        {
            var actual = vec / scale;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new double2(1, 1);
            var b = new double2(1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new double2(1, 1);
            var b = new double2(0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inquality_IsEqual()
        {
            var a = new double2(1, 1);
            var b = new double2(1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new double2(1, 1);
            var b = new double2(0, 0);

            Assert.True(a != b);
        }

        #endregion

        #region Overrides

        //TODO: GetHashCode
        //TODO: Equals(obj)

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new double2(4, 0), new double2(1, 0) };
            yield return new object[] { new double2(0, 4), new double2(0, 1) };
            yield return new object[] { new double2(1, 1), new double2(System.Math.Sqrt(0.5), System.Math.Sqrt(0.5)) };
        }

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new double2(0, 0);
            var one = new double2(1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { one, one, new double2(2, 2) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new double2(0, 0);
            var one = new double2(1, 1);

            yield return new object[] { one, one, zero };
            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, -one };
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new double2(1, 1);

            yield return new object[] { one, 1, one };
            yield return new object[] { one, 2, new double2(2, 2) };
            yield return new object[] { one, 0, new double2(0, 0) };
        }

        public static IEnumerable<object[]> GetDivide()
        {
            var one = new double2(1, 1);

            yield return new object[] { new double2(2, 2), 2, one };
            yield return new object[] { one, 1, one };
        }

        public static IEnumerable<object[]> GetComponentMin()
        {
            var one = new double2(1, 1);
            var zero = new double2(0, 0);

            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
            yield return new object[] { new double2(1, 0), new double2(0, 1), zero };
        }

        public static IEnumerable<object[]> GetComponentMax()
        {
            var one = new double2(1, 1);
            var zero = new double2(0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { new double2(1, 0), new double2(0, 1), one };
        }

        public static IEnumerable<object[]> GetMin()
        {
            var one = new double2(1, 1);
            var zero = new double2(0, 0);

            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
        }

        public static IEnumerable<object[]> GetMax()
        {
            var one = new double2(1, 1);
            var zero = new double2(0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
        }

        public static IEnumerable<object[]> GetClamp()
        {
            var one = new double2(1, 1);
            var zero = new double2(0, 0);

            yield return new object[] { new double2(2, 2), zero, one, one };
            yield return new object[] { new double2(-1, -1), zero, one, zero };
            yield return new object[] { new double2(0.5f, 0.5f), zero, one, new double2(0.5f, 0.5f) };
        }

        public static IEnumerable<object[]> GetLerp()
        {
            var one = new double2(1, 1);
            var zero = new double2(0, 0);

            yield return new object[] { zero, one, 0.5f, new double2(0.5f, 0.5f) };
            yield return new object[] { zero, one, 0, zero };
            yield return new object[] { zero, one, 1, one };
        }

        public static IEnumerable<object[]> GetBarycentric()
        {
            var zero = new double2(0, 0);
            var x = new double2(1, 0);
            var y = new double2(0, 1);

            yield return new object[] { zero, x, y, 0, 0, y };
            yield return new object[] { zero, x, y, 1, 0, zero };
            yield return new object[] { zero, x, y, 0, 1, x };
        }

        #endregion

        #region ToString/Parse

        [Fact]
        public void ToString_NoCulture()
        {
            Assert.NotNull(new double2().ToString());
        }

        [Fact]
        public void ToString_InvariantCulture()
        {
            string s = "(1.5, 1.5)";
            double2 d = double2.One * 1.5f;

            Assert.Equal(s, d.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "(1,5; 1,5)";
            double2 d = double2.One * 1.5d;

            Assert.Equal(s, d.ToString(new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_InvariantCulture()
        {
            string s = "(1.5, 1.5)";
            double2 d = double2.One * 1.5d;

            Assert.Equal(d, double2.Parse(s, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_CultureDE()
        {
            string s = "(1,5; 1,5)";
            double2 d = double2.One * 1.5d;

            Assert.Equal(d, double2.Parse(s, new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_ToString_NoCulture()
        {
            double2 d = double2.One * 1.5d;

            Assert.Equal(d, double2.Parse(d.ToString()));
        }

        [Fact]
        public void Parse_ToString_InvariantCulture()
        {
            double2 d = double2.One * 1.5d;

            Assert.Equal(d, double2.Parse(d.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_ToString_CultureDE()
        {
            double2 d = double2.One * 1.5d;

            Assert.Equal(d, double2.Parse(d.ToString(new CultureInfo("de-DE")), new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_Exception()
        {
            string s = "Fusee";

            Assert.Throws<FormatException>(() => double2.Parse(s));
        }

        #endregion ToString/Parse
    }
}