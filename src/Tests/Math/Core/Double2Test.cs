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
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var f2 = new double2(10, 0); f2[idx] = 10; });

            Assert.Equal(expected, actual.ParamName);
        }

        public static IEnumerable<object[]> ThisException()
        {
            yield return new object[] { 7, "Index 7 not eligible for a double2 type" };
            yield return new object[] { 6, "Index 6 not eligible for a double2 type" };
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

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Instance(double2 vec, double2 expected)
        {
            var actual = vec.NormalizeFast();

            //See note on precision in "NormalizeFast" implementation.
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

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Static(double2 vec, double2 expected)
        {
            var actual = double2.NormalizeFast(vec);

            //See note on precision in "NormalizeFast" implementation.
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

        [Theory]
        [MemberData(nameof(GetStep))]
        public void Step(double2 edge, double2 val, double2 expected)
        {
            Assert.Equal(expected, double2.Step(edge, val));
        }

        #region Lerp

        [Theory]
        [MemberData(nameof(GetLerp))]
        public void Lerp_TestLerp(double2 left, double2 right, double blend, double2 expected)
        {
            var actual = double2.Lerp(left, right, blend);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetLerp2))]
        public void Lerp_TestLerp2(double2 left, double2 right, double2 blend, double2 expected)
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

            double2.GetBarycentric(a, b, c, point, out double uActual, out double vActual);

            Assert.Equal(uExpected, uActual);
            Assert.Equal(vExpected, vActual);
        }

        [Theory]
        [MemberData(nameof(GetBarycentric))]
        public void PointIntriangle(double2 a, double2 b, double2 c, double _1, double _2, double2 point)
        {
            Assert.True(double2.PointInTriangle(a, b, c, point, out _, out _));
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

        #region Swizzle

        [Fact]
        public void Swizzle_Get()
        {
            var vec = new double2(1, 2);

            Assert.Equal(new double2(1, 2), vec.xy);
            Assert.Equal(new double2(2, 1), vec.yx);
        }

        [Fact]
        public void Swizzle_Set()
        {
            var actual = new double2();
            var vec = new double2(1, 2);

            actual.xy = vec;

            Assert.Equal(new double2(1, 2), actual);

            actual.yx = vec;

            Assert.Equal(new double2(2, 1), actual);
        }

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

        #region Color

        [Fact]
        public void Color_Get()
        {
            var vec = new double2(1, 2);

            Assert.Equal(new double2(1, 2), vec.rg);
            Assert.Equal(1, vec.r);
            Assert.Equal(2, vec.g);
        }

        [Fact]
        public void Color_Set()
        {
            var vec = new double2
            {
                rg = new double2(1, 2)
            };

            Assert.Equal(new double2(1, 2), vec);

            vec.r = 0;
            vec.g = 1;

            Assert.Equal(new double2(0, 1), vec);
        }

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new double2(4, 0), new double2(1, 0) };
            yield return new object[] { new double2(0, 4), new double2(0, 1) };
            yield return new object[] { new double2(1, 1), new double2((double)System.Math.Sqrt(0.5), (double)System.Math.Sqrt(0.5)) };
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
            yield return new object[] { new double2(0.5, 0.5), zero, one, new double2(0.5, 0.5) };
        }

        public static IEnumerable<object[]> GetLerp()
        {
            var one = new double2(1, 1);
            var zero = new double2(0, 0);

            yield return new object[] { zero, one, 0.5, new double2(0.5, 0.5) };
            yield return new object[] { zero, one, 0, zero };
            yield return new object[] { zero, one, 1, one };
        }

        public static IEnumerable<object[]> GetLerp2()
        {
            var one = new double2(1, 1);
            var zero = new double2(0, 0);

            yield return new object[] { zero, one, new double2(0.5, 0.5), new double2(0.5, 0.5) };
            yield return new object[] { zero, one, double2.Zero, zero };
            yield return new object[] { zero, one, double2.One, one };
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

        public static IEnumerable<object[]> GetStep()
        {
            var x = new double2(2.222, 2.222);
            var y = new double2(1.111, 1.111);
            yield return new object[] { x, y, double2.Zero };
            yield return new object[] { y, x, double2.One };
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
            double2 f = double2.One * 1.5f;

            Assert.Equal(s, f.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "(1,5; 1,5)";
            double2 f = double2.One * 1.5;

            Assert.Equal(s, f.ToString(new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_InvariantCulture()
        {
            string s = "(1.5, 1.5)";
            double2 f = double2.One * 1.5;

            Assert.Equal(f, double2.Parse(s, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_CultureDE()
        {
            string s = "(1,5; 1,5)";
            double2 f = double2.One * 1.5;

            Assert.Equal(f, double2.Parse(s, new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_ToString_NoCulture()
        {
            double2 f = double2.One * 1.5;

            Assert.Equal(f, double2.Parse(f.ToString()));
        }

        [Fact]
        public void Parse_ToString_InvariantCulture()
        {
            double2 f = double2.One * 1.5;

            Assert.Equal(f, double2.Parse(f.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_ToString_CultureDE()
        {
            double2 f = double2.One * 1.5;

            Assert.Equal(f, double2.Parse(f.ToString(new CultureInfo("de-DE")), new CultureInfo("de-DE")));
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