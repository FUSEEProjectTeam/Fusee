using Xunit;
using System;
using System.Collections.Generic;

namespace Fusee.Math.Core
{
    public class Float4Test
    {
        #region Fields

        [Fact]
        public void UnitX_IsXVector()
        {
            var expected = new float4(1, 0, 0, 0);

            Assert.Equal(expected, float4.UnitX);
        }

        [Fact]
        public void UnitY_IsYVector()
        {
            var expected = new float4(0, 1, 0, 0);

            Assert.Equal(expected, float4.UnitY);
        }

        [Fact]
        public void UnitZ_IsZVector()
        {
            var expected = new float4(0, 0, 1, 0);

            Assert.Equal(expected, float4.UnitZ);
        }

        [Fact]
        public void UnitW_IsWVector()
        {
            var expected = new float4(0, 0, 0, 1);

            Assert.Equal(expected, float4.UnitW);
        }

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new float4(0, 0, 0, 0);

            Assert.Equal(expected, float4.Zero);
        }

        [Fact]
        public void One_IsOne()
        {
            var expected = new float4(1, 1, 1, 1);

            Assert.Equal(expected, float4.One);
        }

        #endregion

        #region Constructors

        [Fact]
        public void Float2_ToFloat4()
        {
            var actual = new float4(new float2(1, 1));

            Assert.Equal(new float4(1, 1, 0, 0), actual);
        }

        [Fact]
        public void Float3_ToFloat4()
        {
            var actual = new float4(new float3(1, 1, 1));

            Assert.Equal(new float4(1, 1, 1, 0), actual);
        }

        [Fact]
        public void Float4AndV_ToFloat4()
        {
            var actual = new float4(new float3(1, 1, 1), 1);

            Assert.Equal(new float4(1, 1, 1, 1), actual);
        }

        [Fact]
        public void Float4_ToFloat4()
        {
            var actual = new float4(new float4(1, 1, 1, 1));

            Assert.Equal(new float4(1, 1, 1, 1), actual);
        }

        [Fact]
        public void Double4_ToFloat4()
        {
            var actual = new float4(new double4(1, 1, 1, 1));

            Assert.Equal(new float4(1, 1, 1, 1), actual);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Addition_TwoVectors(float4 a, float4 b, float4 expected)
        {
            var actual = a + b;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivision))]
        public void Division_IsOne(float4 vec, float scalar, float4 expected)
        {
            var actual = vec / scalar;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new float4(1, 1, 1, 1);
            var b = new float4(1, 1, 1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new float4(1, 1, 1, 1);
            var b = new float4(0, 0, 0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Explicit_Double4_ToFloat4()
        {
            var a = new double4(1, 1, 1, 1);

            var actual = (float4)a;

            Assert.Equal(new float4(1, 1, 1, 1), actual);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new float4(1, 1, 1, 1);
            var b = new float4(1, 1, 1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new float4(1, 1, 1, 1);
            var b = new float4(0, 0, 0, 0);

            Assert.True(a != b);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_ScalarVector(float scalar, float4 vec, float4 expected)
        {
            var actual = scalar * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar(float scalar, float4 vec, float4 expected)
        {
            var actual = vec * scalar;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors(float x, float4 vec1, float4 expected)
        {
            var vec2 = new float4(x, x, x, x);

            var actual = vec1 * vec2;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtraction_IsZero(float4 vec1, float4 vec2, float4 expected)
        {
            var actual = vec1 - vec2;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UnaryNegation_IsNegative()
        {
            var vec = new float4(1, 1, 1, 1);

            var actual = -vec;

            Assert.Equal(new float4(-1, -1, -1, -1), actual);
        }

        #endregion

        #region Properties

        [Fact]
        public void Length_IsOne()
        {
            var vec = new float4(0.5f, 0.5f, 0.5f, 0.5f);

            var actual = vec.Length;

            Assert.Equal(1, actual);
        }

        [Fact]
        public void Length1_IsOne()
        {
            var vec = new float4(0.25f, 0.25f, 0.25f, 0.25f);

            var actual = vec.Length1;

            Assert.Equal(1, actual);
        }

        [Fact]
        public void LengthSquared_IsOne()
        {
            var vec = new float4(0.5f, 0.5f, 0.5f, 0.5f);

            var actual = vec.LengthSquared;

            Assert.Equal(1, actual);
        }

        #endregion

        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_TwoVectors_ReturnFloat4(float4 a, float4 b, float4 expected)
        {
            var actual = float4.Add(a, b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_TwoVectors_ToFloat4(float4 a, float4 b, float4 expected)
        {
            float4 actual;

            float4.Add(ref a, ref b, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivision))]
        public void Divide_TwoVectos_ReturnVector(float4 vec1, float x, float4 expected)
        {
            var vec2 = new float4(x, x, x, x);

            var actual = float4.Divide(vec1, vec2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivision))]
        public void Divide_TwoVectos_ToVector(float4 vec1, float x, float4 expected)
        {
            var vec2 = new float4(x, x, x, x);
            float4 actual;
            
            float4.Divide(ref vec1, ref vec2, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivision))]
        public void Divide_VectorScalar_ReturnVector(float4 vec1, float x, float4 expected)
        {
            var actual = float4.Divide(vec1, x);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivision))]
        public void Divide_VectorScalar_ToVector(float4 vec1, float x, float4 expected)
        {
            float4 actual;
            
            float4.Divide(ref vec1, x, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_ReturnVector(float x, float4 vec1, float4 expected)
        {
            var vec2 = new float4(x, x, x, x);
            float4 actual;

            actual = float4.Multiply(vec1, vec2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_ToVector(float x, float4 vec1, float4 expected)
        {
            var vec2 = new float4(x, x, x, x);
            float4 actual;

            float4.Multiply(ref vec1, ref vec2, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_ReturnVector(float x, float4 vec1, float4 expected)
        {
            float4 actual;

            actual = float4.Multiply(vec1, x);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_ToVector(float x, float4 vec1, float4 expected)
        {
            float4 actual;

            float4.Multiply(ref vec1, x, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_TwoVectors_ReturnVector(float4 vec1, float4 vec2, float4 expected)
        {
            float4 actual;

            actual = float4.Subtract(vec1, vec2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_TwoVectors_ToVector(float4 vec1, float4 vec2, float4 expected)
        {
            float4 actual;

            float4.Subtract(ref vec1, ref vec2, out actual);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Barycentric
        [Theory]
        [MemberData(nameof(GetBarycentric))]
        public void BaryCentric_ReturnVector(float u, float v, float4 expected)
        {
            var a = new float4(1, 0, 0, 1);
            var b = new float4(0, 1, 0, 1);
            var c = new float4(0, 0, 1, 1);

            var actual = float4.BaryCentric(a, b, c, u, v);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Clamp
        [Theory]
        [MemberData(nameof(GetClamp))]
        public void Clamp_ReturnVector(float4 value, float4 min, float4 max, float4 expected)
        {
            float4 actual;

            actual = float4.Clamp(value, min, max);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetClamp))]
        public void Clamp_ToVector(float4 value, float4 min, float4 max, float4 expected)
        {
            float4 actual;

            float4.Clamp(ref value, ref min, ref max, out actual);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Dot
        [Fact]
        public void Dot_TwoVectos_ReturnScalar()
        {
            float4 a = new float4(1, 1, 1, 1);
            float4 b = new float4(1, 2, 3, 4);

            var actual = float4.Dot(a, b);

            Assert.Equal(10, actual);
        }

        [Fact]
        public void Dot_TwoVectos_ToScalar()
        {
            float4 a = new float4(1, 1, 1, 1);
            float4 b = new float4(1, 2, 3, 4);
            float actual;

            float4.Dot(ref a, ref b, out actual);

            Assert.Equal(10, actual);
        }
        #endregion

        #region Lerp
        [Theory]
        [MemberData(nameof(GetLerp))]
        public void Lerp_ReturnVector(float4 a, float4 b, float blend, float4 expected)
        {
            float4 actual;

            actual = float4.Lerp(a, b, blend);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetLerp))]
        public void Lerp_ToVector(float4 a, float4 b, float blend, float4 expected)
        {
            float4 actual;

            float4.Lerp(ref a, ref b, blend, out actual);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region MinMax
        [Theory]
        [MemberData(nameof(GetMinMax))]
        public void Max_ReturnVector(float4 a, float4 b)
        {
            float4 actual;

            actual = float4.Max(a, b);

            Assert.Equal(new float4(1, 1, 1, 1), actual);
        }

        [Theory]
        [MemberData(nameof(GetMinMax))]
        public void Max_ToVector(float4 a, float4 b)
        {
            float4 actual;

            float4.Max(ref a, ref b, out actual);

            Assert.Equal(new float4(1, 1, 1, 1), actual);
        }

        [Theory]
        [MemberData(nameof(GetMinMax))]
        public void Min_ReturnVector(float4 a, float4 b)
        {
            float4 actual;

            actual = float4.Min(a, b);

            Assert.Equal(new float4(0, 0, 0, 0), actual);
        }

        [Theory]
        [MemberData(nameof(GetMinMax))]
        public void Min_ToVector(float4 a, float4 b)
        {
            float4 actual;

            float4.Min(ref a, ref b, out actual);

            Assert.Equal(new float4(0, 0, 0, 0), actual);
        }
        #endregion

        #region Normalize
        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Direct(float4 vec, float4 expected)
        {
            float4 actual = vec;

            actual.Normalize();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_ReturnVector(float4 vec, float4 expected)
        {
            float4 actual;

            actual = float4.Normalize(vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_ToVector(float4 vec, float4 expected)
        {
            float4 actual;

            float4.Normalize(ref vec, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Direct(float4 vec, float4 expected)
        {
            float4 actual = vec;

            actual.NormalizeFast();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_ReturnVector(float4 vec, float4 expected)
        {
            float4 actual;

            actual = float4.NormalizeFast(vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_ToVector(float4 vec, float4 expected)
        {
            float4 actual;

            float4.NormalizeFast(ref vec, out actual);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Normalize1_Direct()
        {
            float4 actual = new float4(1, 1, 1, 1);

            actual.Normalize1();

            Assert.Equal(new float4(0.25f, 0.25f, 0.25f, 0.25f), actual);
        }
        #endregion

        #region Round
        [Fact]
        public void Round_Direct()
        {
            var actual = new float4(1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f);

            actual.Round();

            Assert.Equal(new float4(1.234568f, 1.234568f, 1.234568f, 1.234568f), actual);
        }

        [Fact]
        public void Round_ReturnVector()
        {
            var vec = new float4(1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f);

            var actual = float4.Round(vec);

            Assert.Equal(new float4(1.234568f, 1.234568f, 1.234568f, 1.234568f), actual);
        }
        #endregion

        #region Transform

        [Theory]
        [MemberData(nameof(GetTransformQuaternion))]
        public void Transform_Quaternion(float4 vec, Quaternion quat, float4 expected)
        {
            var actual = float4.Transform(vec, quat);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransfromMatrix))]
        public void Transform_4x4Matrix(float4 vec, float4x4 mat, float4 expected)
        {
            var actual = float4.Transform(mat, vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransfromMatrix))]
        public void TransformPerspective_Direct(float4 vec, float4x4 mat, float4 expected)
        {
            var actual = vec.TransformPerspective(mat);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Other
        //TODO: Equals

        //TODO: GetHashCode

        //TODO: GetType

        //TODO: ToArray

        //TODO: ToString
        #endregion

        #endregion

        #region IEnumberables

        public static IEnumerable<object[]> GetAddition()
        {
            float4 a = new float4(1, 1, 1, 1);
            float4 b = new float4(0, 0, 0, 0);

            yield return new object[] { a, b, a };
            yield return new object[] { b, a, a };
        }

        public static IEnumerable<object[]> GetDivision()
        {
            yield return new object[] { new float4(2, 2, 2, 2), 2, new float4(1, 1, 1, 1) };
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new float4(1, 1, 1, 1);

            yield return new object[] { 1, one, one };
            yield return new object[] { 2, one, new float4(2, 2, 2, 2) };
            yield return new object[] { 0, one, new float4(0, 0, 0, 0) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            yield return new object[] { new float4(1, 1, 1, 1), new float4(1, 1, 1, 1), new float4(0, 0, 0, 0) };
        }

        public static IEnumerable<object[]> GetClamp()
        {
            var zero = new float4(0, 0, 0, 0);
            var one = new float4(1, 1, 1, 1);

            yield return new object[] { new float4(-1, -1, -1, -1), zero, one, zero };
            yield return new object[] { new float4(2, 2, 2, 2), zero, one, one };
            yield return new object[] { new float4(0.5f, 0.5f, 0.5f, 0.5f), zero, one, new float4(0.5f, 0.5f, 0.5f, 0.5f) };
        }

        public static IEnumerable<object[]> GetLerp()
        {
            var zero = new float4(0, 0, 0, 0);
            var one = new float4(1, 1, 1, 1);

            yield return new object[] { zero, one, 0, zero };
            yield return new object[] { zero, one, 1, one };
            yield return new object[] { zero, one, 0.5f, new float4(0.5f, 0.5f, 0.5f, 0.5f) };
        }

        public static IEnumerable<object[]> GetMinMax()
        {
            var one = new float4(1, 1, 1, 1);
            var zero = new float4(0, 0, 0, 0);

            yield return new object[] { one, zero };
            yield return new object[] { zero, one };
        }

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new float4(1, 1, 1, 1), new float4(0.5f, 0.5f, 0.5f, 0.5f) };
        }

        public static IEnumerable<object[]> GetTransformQuaternion()
        {
            var a = new float4(1, 0, 0, 0);
            var b = new float4(0, 1, 0, 0);
            var c = new float4(0, 0, 1, 0);

            yield return new object[] { b, new Quaternion(0.707f, 0, 0, 0.707f), c };
            yield return new object[] { c, new Quaternion(0, 0.707f, 0, 0.707f), a };
            yield return new object[] { a, new Quaternion(0, 0, 0.707f, 0.707f), b };
        }

        public static IEnumerable<object[]> GetTransfromMatrix()
        {
            var a = new float4(1, 0, 0, 1);
            var b = new float4(0, 1, 0, 1);
            var c = new float4(0, 0, 1, 1);

            var xRot = new float4x4(new float4(1, 0, 0, 0), new float4(0, 0, -1, 0), new float4(0, 1, 0, 0), new float4(0, 0, 0, 1));
            var yRot = new float4x4(new float4(0, 0, 1, 0), new float4(0, 1, 0, 0), new float4(-1, 0, 0, 0), new float4(0, 0, 0, 1));
            var zRot = new float4x4(new float4(0, -1, 0, 0), new float4(1, 0, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            yield return new object[] { b, xRot, c };
            yield return new object[] { c, yRot, a };
            yield return new object[] { a, zRot, b };
        }

        public static IEnumerable<object[]> GetBarycentric()
        {
            yield return new object[] { 0, 0, new float4(0, 0, 1, 1) };
            yield return new object[] { 1, 0, new float4(1, 0, 0, 1) };
            yield return new object[] { 0, 1, new float4(0, 1, 0, 1) };
        }
        #endregion
    }
}
