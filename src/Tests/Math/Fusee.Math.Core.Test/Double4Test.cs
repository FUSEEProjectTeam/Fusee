using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fusee.Math.Core
{
    public class Double4Test
    {
        #region Fields

        [Fact]
        public void UnitX_IsUnit()
        {
            var expected = new double4(1, 0, 0, 0);

            Assert.Equal(expected, double4.UnitX);
        }

        [Fact]
        public void UnitY_IsUnit()
        {
            var expected = new double4(0, 1, 0, 0);

            Assert.Equal(expected, double4.UnitY);
        }

        [Fact]
        public void UnitZ_IsUnit()
        {
            var expected = new double4(0, 0, 1, 0);

            Assert.Equal(expected, double4.UnitZ);
        }

        [Fact]
        public void UnitW_IsUnit()
        {
            var expected = new double4(0, 0, 0, 1);

            Assert.Equal(expected, double4.UnitW);
        }

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new double4(0, 0, 0, 0);

            Assert.Equal(expected, double4.Zero);
        }

        [Fact]
        public void One_IsOne()
        {
            var expected = new double4(1, 1, 1, 1);

            Assert.Equal(expected, double4.One);
        }

        #endregion

        #region Constructors

        [Fact]
        public void Constructor_FromDouble2()
        {
            var vec = new double2(1, 2);

            var actual = new double4(vec);

            Assert.Equal(new double4(1, 2, 0, 0), actual);
        }

        [Fact]
        public void Constructor_FromDouble3()
        {
            var vec = new double3(1, 2, 3);

            var actual = new double4(vec);

            Assert.Equal(new double4(1, 2, 3, 0), actual);
        }

        [Fact]
        public void Constructor_FromDouble3Scale()
        {
            var vec = new double3(1, 2, 3);

            var actual = new double4(vec, 4);

            Assert.Equal(new double4(1, 2, 3, 4), actual);
        }

        [Fact]
        public void Constructor_FromDouble4()
        {
            var vec = new double4(1, 2, 3, 4);

            var actual = new double4(vec);

            Assert.Equal(new double4(1, 2, 3, 4), actual);
        }

        #endregion

        #region Instance

        [Fact]
        public void Length_Is2()
        {
            var vec = new double4(1, 1, 1, 1);

            var actual = vec.Length;

            Assert.Equal(2, actual);
        }

        [Fact]
        public void LengthSquared_Is4()
        {
            var vec = new double4(1, 1, 1, 1);

            var actual = vec.LengthSquared;

            Assert.Equal(4, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(double4 vec, double4 expected)
        {
            var actual = vec.Normalize();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Instance(double4 vec, double4 expected)
        {
            var actual = vec.NormalizeFast();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToArray_IsArray()
        {
            var vec = new double4(1, 2, 3, 4);

            var actual = vec.ToArray();

            Assert.Equal(new double[] { 1, 2, 3, 4 }, actual);
        }

        #endregion

        #region this

        [Fact]
        public void This_GetWithIdx_IsValid()
        {
            var actual = new double4(3, 7, 8, 1);

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);

            Assert.Equal(8, actual[2]);

            Assert.Equal(1, actual[3]);
        }

        [Fact]
        public void This_SetWithIdx_IsValid()
        {
            var actual = new double4(0, 0, 0, 0);
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
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => new double4(0, 0, 0, 0)[idx]);

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_SetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var d4 = new double4(0, 0, 0, 0); d4[idx] = 10; });

            Assert.Equal(expected, actual.ParamName);
        }

        public static IEnumerable<object[]> ThisException()
        {
            yield return new object[] { 7, "Index 7 not eligible for a double4 type" };
            yield return new object[] { 6, "Index 6 not eligible for a double4 type" };
        }

        #endregion
        
        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Static(double4 left, double4 right, double4 expected)
        {
            var actual = double4.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Static(double4 left, double4 right, double4 expected)
        {
            var actual = double4.Subtract(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Static(double4 vec, double scale, double4 expected)
        {
            var actual = double4.Multiply(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_Static(double4 left, double scale, double4 expected)
        {
            var right = new double4(scale, scale, scale, scale);

            var actual = double4.Multiply(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_Vector_Scalar_Static(double4 vec, double scale, double4 expected)
        {
            var actual = double4.Divide(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_TwoVectors_Static(double4 left, double scale, double4 expected)
        {
            var right = new double4(scale, scale, scale, scale);

            var actual = double4.Divide(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region MinMax

        [Theory]
        [MemberData(nameof(GetMin))]
        public void Min_IsZero(double4 left, double4 right, double4 expected)
        {
            var actual = double4.Min(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMax))]
        public void Max_IsOne(double4 left, double4 right, double4 expected)
        {
            var actual = double4.Max(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Clamp

        [Theory]
        [MemberData(nameof(GetClamp))]
        public void Clamp_TestClamp(double4 vec, double4 min, double4 max, double4 expected)
        {
            var actual = double4.Clamp(vec, min, max);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Normalize

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Static(double4 vec, double4 expected)
        {
            var actual = double4.Normalize(vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Static(double4 vec, double4 expected)
        {
            var actual = double4.NormalizeFast(vec);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Dot

        [Fact]
        public void Dot_Is20()
        {
            var a = new double4(1, 2, 3, 4);
            var b = new double4(4, 3, 2, 1);

            var actual = double4.Dot(a, b);

            Assert.Equal(20, actual);
        }

        #endregion

        #region Lerp

        [Theory]
        [MemberData(nameof(GetLerp))]
        public void Lerp_TestLerp(double4 left, double4 right, double blend, double4 expected)
        {
            var actual = double4.Lerp(left, right, blend);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Barycentric

        [Theory]
        [MemberData(nameof(GetBarycentric))]
        public void Barycentric_Edges(double4 a, double4 b, double4 c, double u, double v, double4 expected)
        {
            var actual = double4.BaryCentric(a, b, c, u, v);

            Assert.Equal(expected, actual);
        }

        #endregion

        #endregion

        #region Swizzle

        [Fact]
        public void Swizzle_Get()
        {
            var vec = new double4(1, 2, 3, 4);

            Assert.Equal(new double2(1, 2), vec.xy);
            Assert.Equal(new double3(1, 2, 3), vec.xyz);
        }

        [Fact]
        public void Swizzle_Set()
        {
            var actual = new double4(0, 0, 0, 0);

            actual.xy = new double2(1, 2);
            Assert.Equal(new double4(1, 2, 0, 0), actual);

            actual.xyz = new double3(3, 3, 3);
            Assert.Equal(new double4(3, 3, 3, 0), actual);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Operator(double4 left, double4 right, double4 expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Operator(double4 left, double4 right, double4 expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UnaryNegation_isNegative()
        {
            var vec = new double4(1, 1, 1, 1);

            var actual = -vec;

            Assert.Equal(new double4(-1, -1, -1, -1), actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Operator(double4 vec, double scale, double4 expected)
        {
            var actual = vec * scale;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_ScalarVector_Operator(double4 vec, double scale, double4 expected)
        {
            var actual = scale * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_Operator(double4 left, double scale, double4 expected)
        {
            var right = new double4(scale, scale, scale, scale);

            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_Operator(double4 vec, double scale, double4 expected)
        {
            var actual = vec / scale;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new double4(1, 1, 1, 1);
            var b = new double4(1, 1, 1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new double4(1, 1, 1, 1);
            var b = new double4(0, 0, 0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new double4(1, 1, 1, 1);
            var b = new double4(1, 1, 1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new double4(1, 1, 1, 1);
            var b = new double4(0, 0, 0, 0);

            Assert.True(a != b);
        }

        [Fact]
        public void Cast_FromDouble4()
        {
            var vec = new double4(1, 1, 1, 1);

            var actual = (double4)vec;

            Assert.Equal(new double4(1, 1, 1, 1), actual);
        }

        #endregion

        #region Overrides

        [Fact]
        public void ToString_IsString()
        {
            var vec = new double4(1, 2, 3, 4);

            var actual = vec.ToString();

            Assert.Equal("(1, 2, 3, 4)", actual);
        }

        //TODO: GetHashCode
        //TODO: Equals(obj)

        #endregion

        #region Color

        [Fact]
        public void Color_Get()
        {
            var actual = new double4(1, 2, 3, 4);

            Assert.Equal(1, actual.r);
            Assert.Equal(2, actual.g);
            Assert.Equal(3, actual.b);
            Assert.Equal(4, actual.a);
            Assert.Equal(new double3(1, 2, 3), actual.rgb);
        }

        [Fact]
        public void Color_Set()
        {
            var actual = new double4();

            actual.r = 1;
            actual.g = 2;
            actual.b = 3;
            actual.a = 4;
            Assert.Equal(new double4(1, 2, 3, 4), actual);

            actual.rgb = new double3(3, 2, 1);
            Assert.Equal(new double4(3, 2, 1, 4), actual);
        }

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new double4(4, 0, 0, 0), new double4(1, 0, 0, 0) };
            yield return new object[] { new double4(0, 4, 0, 0), new double4(0, 1, 0, 0) };
            yield return new object[] { new double4(0, 0, 4, 0), new double4(0, 0, 1, 0) };
            yield return new object[] { new double4(0, 0, 0, 4), new double4(0, 0, 0, 1) };
            yield return new object[] { new double4(1, 1, 1, 1), new double4(0.5f, 0.5f, 0.5f, 0.5f) };
        }

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new double4(0, 0, 0, 0);
            var one = new double4(1, 1, 1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { one, one, new double4(2, 2, 2, 2) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new double4(0, 0, 0, 0);
            var one = new double4(1, 1, 1, 1);

            yield return new object[] { one, one, zero };
            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, -one };
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new double4(1, 1, 1, 1);

            yield return new object[] { one, 1, one };
            yield return new object[] { one, 2, new double4(2, 2, 2, 2) };
            yield return new object[] { one, 0, new double4(0, 0, 0, 0) };
        }

        public static IEnumerable<object[]> GetDivide()
        {
            var one = new double4(1, 1, 1, 1);

            yield return new object[] { new double4(2, 2, 2, 2), 2, one };
            yield return new object[] { one, 1, one };
        }

        public static IEnumerable<object[]> GetMin()
        {
            var one = new double4(1, 1, 1, 1);
            var zero = new double4(0, 0, 0, 0);

            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
        }

        public static IEnumerable<object[]> GetMax()
        {
            var one = new double4(1, 1, 1, 1);
            var zero = new double4(0, 0, 0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
        }

        public static IEnumerable<object[]> GetClamp()
        {
            var one = new double4(1, 1, 1, 1);
            var zero = new double4(0, 0, 0, 0);

            yield return new object[] { new double4(2, 2, 2, 2), zero, one, one };
            yield return new object[] { new double4(-1, -1, -1, -1), zero, one, zero };
            yield return new object[] { new double4(0.5f, 0.5f, 0.5f, 0.5f), zero, one, new double4(0.5f, 0.5f, 0.5f, 0.5f) };
        }

        public static IEnumerable<object[]> GetLerp()
        {
            var one = new double4(1, 1, 1, 1);
            var zero = new double4(0, 0, 0, 0);

            yield return new object[] { zero, one, 0.5f, new double4(0.5f, 0.5f, 0.5f, 0.5f) };
            yield return new object[] { zero, one, 0, zero };
            yield return new object[] { zero, one, 1, one };
        }

        public static IEnumerable<object[]> GetBarycentric()
        {
            var x = new double4(1, 0, 0, 0);
            var y = new double4(0, 1, 0, 0);
            var z = new double4(0, 0, 1, 0);

            yield return new object[] { x, y, z, 0, 0, z };
            yield return new object[] { x, y, z, 1, 0, x };
            yield return new object[] { x, y, z, 0, 1, y };
        }

        #endregion
    }
}
