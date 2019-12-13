using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fusee.Test.Math.Core
{
    public class Double3Test
    {
        #region Fields

        [Fact]
        public void UnitX_IsUnit()
        {
            var expected = new double3(1, 0, 0);

            Assert.Equal(expected, double3.UnitX);
        }

        [Fact]
        public void UnitY_IsUnit()
        {
            var expected = new double3(0, 1, 0);

            Assert.Equal(expected, double3.UnitY);
        }

        [Fact]
        public void UnitZ_IsUnit()
        {
            var expected = new double3(0, 0, 1);

            Assert.Equal(expected, double3.UnitZ);
        }

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new double3(0, 0, 0);

            Assert.Equal(expected, double3.Zero);
        }

        [Fact]
        public void One_IsOne()
        {
            var expected = new double3(1, 1, 1);

            Assert.Equal(expected, double3.One);
        }

        #endregion

        #region Constructors

        [Fact]
        public void Constructor_FromDouble2()
        {
            var f2 = new double2(1, 2);

            var actual = new double3(f2);

            Assert.Equal(new double3(1, 2, 0), actual);
        }

        [Fact]
        public void Constructor_FromDouble3()
        {
            var f3 = new double3(1, 2, 3);

            var actual = new double3(f3);

            Assert.Equal(new double3(1, 2, 3), actual);
        }

        [Fact]
        public void Constructor_FromDouble4()
        {
            var f4 = new double4(1, 2, 3, 4);

            var actual = new double3(f4);

            Assert.Equal(new double3(1, 2, 3), actual);
        }

        #endregion

        #region this

        [Fact]
        public void This_GetWithIdx_IsValid()
        {
            var actual = new double3(3, 7, 8);

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);

            Assert.Equal(8, actual[2]);
        }

        [Fact]
        public void This_SetWithIdx_IsValid()
        {
            var actual = new double3(0, 0, 0);
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
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => new double3(0, 0, 0)[idx]);

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_SetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var d3 = new double3(0, 0, 0); d3[idx] = 10; });

            Assert.Equal(expected, actual.ParamName);
        }

        public static IEnumerable<object[]> ThisException()
        {
            yield return new object[] { 7, "Index 7 not eligible for a double3 type" };
            yield return new object[] { 6, "Index 6 not eligible for a double3 type" };
        }

        #endregion

        #region Instance

        [Fact]
        public void Length_Is3()
        {
            var vec = new double3(1, 2, 2);

            var actual = vec.Length;

            Assert.Equal(3, actual);
        }

        [Fact]
        public void LengthSquared_Is3()
        {
            var vec = new double3(1, 1, 1);

            var actual = vec.LengthSquared;

            Assert.Equal(3, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(double3 vec, double3 expected)
        {
            var actual = vec.Normalize();

            Assert.Equal(expected.x, actual.x, 14);
            Assert.Equal(expected.y, actual.y, 14);
            Assert.Equal(expected.z, actual.z, 14);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Instance(double3 vec, double3 expected)
        {
            var actual = vec.NormalizeFast();

            Assert.Equal(expected.x, actual.x, 7);
            Assert.Equal(expected.y, actual.y, 7);
            Assert.Equal(expected.z, actual.z, 7);
        }

        [Fact]
        public void ToArray_IsArray()
        {
            var vec = new double3(1, 2, 3);

            var actual = vec.ToArray();

            Assert.Equal(new double[] { 1, 2, 3 }, actual);
        }

        #endregion

        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_TwoVectors_Static(double3 left, double3 right, double3 expected)
        {
            var actual = double3.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_TwoVectors_Static(double3 left, double3 right, double3 expected)
        {
            var actual = double3.Subtract(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Static(double3 vec, double scale, double3 expected)
        {
            var actual = double3.Multiply(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multitply_TwoVectors_Static(double3 left, double scale, double3 expected)
        {
            var right = new double3(scale, scale, scale);

            var actual = double3.Multiply(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_VectorScalar_Static(double3 vec, double scale, double3 expected)
        {
            var actual = double3.Divide(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_TwoVectors_Static(double3 left, double scale, double3 expected)
        {
            var right = new double3(scale, scale, scale);

            var actual = double3.Divide(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region MinMax

        [Theory]
        [MemberData(nameof(GetComponentMin))]
        public void ComponentMin_IsZero(double3 left, double3 right, double3 expected)
        {
            var actual = double3.ComponentMin(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetComponentMax))]
        public void ComponentMax_IsOne(double3 left, double3 right, double3 expected)
        {
            var actual = double3.ComponentMax(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMin))]
        public void Min_IsZero(double3 left, double3 right, double3 expected)
        {
            var actual = double3.Min(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMax))]
        public void Max_IsOne(double3 left, double3 right, double3 expected)
        {
            var actual = double3.Max(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Clamp

        [Theory]
        [MemberData(nameof(GetClamp))]
        public void Clamp_TestClamp(double3 vec, double3 min, double3 max, double3 expected)
        {
            var actual = double3.Clamp(vec, min, max);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Normalize

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Static(double3 vec, double3 expected)
        {
            var actual = double3.Normalize(vec);

            Assert.Equal(expected.x, actual.x, 14);
            Assert.Equal(expected.y, actual.y, 14);
            Assert.Equal(expected.z, actual.z, 14);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Static(double3 vec, double3 expected)
        {
            var actual = double3.NormalizeFast(vec);

            Assert.Equal(expected.x, actual.x, 7);
            Assert.Equal(expected.y, actual.y, 7);
            Assert.Equal(expected.z, actual.z, 7);
        }

        #endregion

        #region Dot

        [Fact]
        public void Dot_Is10()
        {
            var left = new double3(1, 2, 3);
            var right = new double3(3, 2, 1);

            var actual = double3.Dot(left, right);

            Assert.Equal(10, actual);
        }

        #endregion

        #region Cross

        [Fact]
        public void Cross_IsVector()
        {
            var left = new double3(1, 2, 3);
            var right = new double3(3, 2, 1);

            var actual = double3.Cross(left, right);

            Assert.Equal(new double3(-4, 8, -4), actual);
        }

        #endregion

        #region Lerp

        [Theory]
        [MemberData(nameof(GetLerp))]
        public void Lerp_TestLerp(double3 left, double3 right, double blend, double3 expected)
        {
            var actual = double3.Lerp(left, right, blend);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Barycentric

        [Theory]
        [MemberData(nameof(GetBarycentric))]
        public void Barycentric_Edges(double3 a, double3 b, double3 c, double u, double v, double3 expected)
        {
            var actual = double3.Barycentric(a, b, c, u, v);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetBarycentric))]
        public void GetBarycentric_Edges(double3 a, double3 b, double3 c, double uExpected, double vExpected, double3 point)
        {
            double uActual;
            double vActual;

            double3.GetBarycentric(a, b, c, point, out uActual, out vActual);

            Assert.Equal(uExpected, uActual);
            Assert.Equal(vExpected, vActual);
        }

        #endregion

        #region CalculateAngle

        [Fact]
        public void CalculateAngle_90Degrees()
        {
            var x = new double3(1, 0, 0);
            var y = new double3(0, 1, 0);

            var actual = double3.CalculateAngle(x, y);

            Assert.Equal(M.DegreesToRadians(90), actual, 6);
        }

        #endregion

        #region Rotate

        [Theory]
        [MemberData(nameof(GetEuler))]
        public void Rotate_Euler(double3 euler, double3 vec, double3 expected)
        {
            var actual = double3.Rotate(euler, vec, true);

            Assert.Equal(expected.x, actual.x, 14);
            Assert.Equal(expected.y, actual.y, 14);
            Assert.Equal(expected.z, actual.z, 14);
        }

        [Theory]
        [MemberData(nameof(GetQuaternion))]
        public void Rotate_Quaternion(QuaternionD quat, double3 vec, double3 expected)
        {
            var actual = double3.Rotate(quat, vec);

            Assert.Equal(expected.x, actual.x, 14);
            Assert.Equal(expected.y, actual.y, 14);
            Assert.Equal(expected.z, actual.z, 14);
        }

        #endregion

        #endregion

        #region Swizzle

        [Fact]
        public void Swizzle_double2_Get()
        {
            var actual = new double3(1, 2, 3);

            Assert.Equal(new double2(1, 2), actual.xy);
        }

        [Fact]
        public void Swizzle_double2_Set()
        {
            var actual = new double3();

            actual.xy = new double2(1, 2);
            Assert.Equal(new double3(1, 2, 0), actual);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_TwoVectors_Operator(double3 left, double3 right, double3 expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_TwoVectors_Operator(double3 left, double3 right, double3 expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UnaryNegation_IsNegative()
        {
            var vec = new double3(1, 1, 1);

            var actual = -vec;

            Assert.Equal(new double3(-1, -1, -1), actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Operator(double3 vec, double scale, double3 expected)
        {
            var actual = vec * scale;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_ScalarVector_Operator(double3 vec, double scale, double3 expected)
        {
            var actual = scale * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_Operator(double3 left, double scale, double3 expected)
        {
            var right = new double3(scale, scale, scale);

            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_Operator(double3 vec, double scale, double3 expected)
        {
            var actual = vec / scale;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new double3(1, 1, 1);
            var b = new double3(1, 1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new double3(1, 1, 1);
            var b = new double3(0, 0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new double3(1, 1, 1);
            var b = new double3(1, 1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new double3(1, 1, 1);
            var b = new double3(0, 0, 0);

            Assert.True(a != b);
        }

        #endregion

        #region Overrides

        [Fact]
        public void ToString_IsString()
        {
            var vec = new double3(1, 2, 3);

            var actual = vec.ToString();

            Assert.Equal("(1, 2, 3)", actual);
        }

        //TODO: GetHashCode
        //TODO: Equals(obj)

        #endregion

        #region Color

        [Fact]
        public void Color_Get()
        {
            var vec = new double3(1, 2, 3);

            Assert.Equal(1, vec.r);
            Assert.Equal(2, vec.g);
            Assert.Equal(3, vec.b);
        }

        [Fact]
        public void Color_Set()
        {
            var actual = new double3();

            actual.r = 1;
            Assert.Equal(new double3(1, 0, 0), actual);

            actual.g = 2;
            Assert.Equal(new double3(1, 2, 0), actual);

            actual.b = 3;
            Assert.Equal(new double3(1, 2, 3), actual);
        }

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new double3(4, 0, 0), new double3(1, 0, 0) };
            yield return new object[] { new double3(0, 4, 0), new double3(0, 1, 0) };
            yield return new object[] { new double3(0, 0, 4), new double3(0, 0, 1) };
            yield return new object[]
            {
                new double3(1, 1, 1),
                new double3((double)System.Math.Sqrt(1d / 3d), (double)System.Math.Sqrt(1d / 3d), (double)System.Math.Sqrt(1d / 3d))
            };
        }

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new double3(0, 0, 0);
            var one = new double3(1, 1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { one, one, new double3(2, 2, 2) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new double3(0, 0, 0);
            var one = new double3(1, 1, 1);

            yield return new object[] { one, one, zero };
            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, -one };
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new double3(1, 1, 1);

            yield return new object[] { one, 1, one };
            yield return new object[] { one, 2, new double3(2, 2, 2) };
            yield return new object[] { one, 0, new double3(0, 0, 0) };
        }

        public static IEnumerable<object[]> GetDivide()
        {
            var one = new double3(1, 1, 1);

            yield return new object[] { new double3(2, 2, 2), 2, one };
            yield return new object[] { one, 1, one };
        }

        public static IEnumerable<object[]> GetComponentMin()
        {
            var one = new double3(1, 1, 1);
            var zero = new double3(0, 0, 0);

            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
            yield return new object[] { new double3(1, 0, 1), new double3(0, 1, 0), zero };
        }

        public static IEnumerable<object[]> GetComponentMax()
        {
            var one = new double3(1, 1, 1);
            var zero = new double3(0, 0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { new double3(1, 0, 1), new double3(0, 1, 0), one };
        }

        public static IEnumerable<object[]> GetMin()
        {
            var one = new double3(1, 1, 1);
            var zero = new double3(0, 0, 0);

            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
        }

        public static IEnumerable<object[]> GetMax()
        {
            var one = new double3(1, 1, 1);
            var zero = new double3(0, 0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
        }

        public static IEnumerable<object[]> GetClamp()
        {
            var one = new double3(1, 1, 1);
            var zero = new double3(0, 0, 0);

            yield return new object[] { new double3(2, 2, 2), zero, one, one };
            yield return new object[] { new double3(-1, -1, -1), zero, one, zero };
            yield return new object[] { new double3(0.5f, 0.5f, 0.5f), zero, one, new double3(0.5f, 0.5f, 0.5f) };
        }

        public static IEnumerable<object[]> GetLerp()
        {
            var one = new double3(1, 1, 1);
            var zero = new double3(0, 0, 0);

            yield return new object[] { zero, one, 0.5f, new double3(0.5f, 0.5f, 0.5f) };
            yield return new object[] { zero, one, 0, zero };
            yield return new object[] { zero, one, 1, one };
        }

        public static IEnumerable<object[]> GetBarycentric()
        {
            var x = new double3(1, 0, 0);
            var y = new double3(0, 1, 0);
            var z = new double3(0, 0, 1);

            yield return new object[] { x, y, z, 0, 0, z };
            yield return new object[] { x, y, z, 1, 0, x };
            yield return new object[] { x, y, z, 0, 1, y };
        }

        public static IEnumerable<object[]> GetEuler()
        {
            var x = new double3(1, 0, 0);
            var y = new double3(0, 1, 0);
            var z = new double3(0, 0, 1);

            yield return new object[] { new double3(90, 0, 0), y, z };
            yield return new object[] { new double3(0, 90, 0), z, x };
            yield return new object[] { new double3(0, 0, 90), x, y };
        }

        public static IEnumerable<object[]> GetQuaternion()
        {
            var x = new double3(1, 0, 0);
            var y = new double3(0, 1, 0);
            var z = new double3(0, 0, 1);

            var xRot = new QuaternionD(System.Math.Sqrt(0.5), 0, 0, System.Math.Sqrt(0.5));
            var yRot = new QuaternionD(0, System.Math.Sqrt(0.5), 0, System.Math.Sqrt(0.5));
            var zRot = new QuaternionD(0, 0, System.Math.Sqrt(0.5), System.Math.Sqrt(0.5));

            yield return new object[] { xRot, y, z };
            yield return new object[] { yRot, z, x };
            yield return new object[] { zRot, x, y };
        }

        #endregion
    }
}
