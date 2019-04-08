using Xunit;
using System.Collections.Generic;

namespace Fusee.Math.Core
{
    public class Float3Test
    {
        #region IEnumerables
        public static IEnumerable<object[]> GetFloat3()
        {
            var zero = new float3(0, 0, 0);
            var one = new float3(1, 1, 1);

            yield return new object[] {zero, one};
            yield return new object[] {one, zero};
        }

        public static IEnumerable<object[]> GetUVExpected()
        {
            yield return new object[] { 0, 0, new float3(0, 0, 1) };
            yield return new object[] { 1, 0, new float3(1, 0, 0) };
            yield return new object[] { 0, 1, new float3(0, 1, 0) };
        }

        public static IEnumerable<object[]> GetFloat3ExpectedAngle()
        {
            var a = new float3(1, 0, 0);
            var b = new float3(0, 1, 0);
            yield return new object[] { a, b, 1.5708f};
            yield return new object[] { a, a, 0};
        }

        public static IEnumerable<object[]> GetVecMinMaxExpected()
        {
            var a = new float3(0, 0, 0);
            var b = new float3(1, 1, 1);
            var c = new float3(2, 2, 2);

            yield return new object[] { a, b, c, b };
            yield return new object[] { c, a, b, b };
            yield return new object[] { b, a, c, b };
        }

        public static IEnumerable<object[]> GetVectorCross()
        {
            var a = new float3(1, 0, 0);
            var b = new float3(0, 1, 0);
            var c = new float3(0, 0, 1);

            yield return new object[] { a, b, c };
            yield return new object[] { c, a, b };
            yield return new object[] { b, c, a };
            yield return new object[] { a, a, new float3(0, 0, 0) };
            yield return new object[] { b, a, -c };
        }

        public static IEnumerable<object[]> GetBlendExpected()
        {
            var a = new float3(0, 0, 0);
            var b = new float3(1, 1, 1);


            yield return new object[] { a, b, 0, a };
            yield return new object[] { a, b, 1, b };
            yield return new object[] { a, b, 0.5f, new float3(0.5f, 0.5f, 0.5f) };
        }

        public static IEnumerable<object[]> GetVectorNorm()
        {
            yield return new object[] { new float3(5, 0, 0), new float3(1, 0, 0) };
            yield return new object[] { new float3(0, 5, 0), new float3(0, 1, 0) };
            yield return new object[] { new float3(0, 0, 5), new float3(0, 0, 1) };
            yield return new object[] { new float3(5, 5, 5), new float3(0.57735f, 0.57735f, 0.57735f) };
        }

        public static IEnumerable<object[]> GetRotationMatrix()
        {
            var a = new float3(1, 0, 0);
            var b = new float3(0, 1, 0);
            var c = new float3(0, 0, 1);

            var xRot = new float4x4(new float4(1, 0, 0, 0), new float4(0, 0, -1, 0), new float4(0, 1, 0, 0), new float4(0, 0, 0, 1));
            var yRot = new float4x4(new float4(0, 0, 1, 0), new float4(0, 1, 0, 0), new float4(-1, 0, 0, 0), new float4(0, 0, 0, 1));
            var zRot = new float4x4(new float4(0, -1, 0, 0), new float4(1, 0, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            yield return new object[] { b, xRot, -c };
            yield return new object[] { a, yRot, c };
            yield return new object[] { b, zRot, a };
        }

        public static IEnumerable<object[]> GetQuaternion()
        {
            var a = new float3(1, 0, 0);
            var b = new float3(0, 1, 0);
            var c = new float3(0, 0, 1);

            var xRot = new Quaternion(new float3(0.707f, 0, 0), 0.707f);
            var yRot = new Quaternion(new float3(0, 0.707f, 0), 0.707f);
            var zRot = new Quaternion(new float3(0, 0, 0.707f), 0.707f);

            yield return new object[] { b, xRot, -c };
            yield return new object[] { a, yRot, c };
            yield return new object[] { b, zRot, a };
        }

        public static IEnumerable<object[]> GetVecLength()
        {
            yield return new object[] { new float3(1, 0, 0), 1 };
            yield return new object[] { new float3(0, 1, 0), 1 };
            yield return new object[] { new float3(0, 0, 1), 1 };
            yield return new object[] { new float3(0, 0, 0), 0 };
            yield return new object[] { new float3(1, 1, 1), 1.73205f };
        }
        #endregion

        #region Fields
        [Fact]
        public void One_IsOne()
        {
            Assert.Equal(new float3(1, 1, 1), float3.One);
        }

        [Fact]
        public void UnitX_EqualsX()
        {
            Assert.Equal(new float3(1, 0, 0), float3.UnitX);
        }

        [Fact]
        public void UnitY_EqualsY()
        {
            Assert.Equal(new float3(0, 1, 0), float3.UnitY);
        }

        [Fact]
        public void UnitZ_EqualZ()
        {
            Assert.Equal(new float3(0, 0, 1), float3.UnitZ);
        }

        [Fact]
        public void Zero_IsZero()
        {
            Assert.Equal(new float3(0, 0, 0), float3.Zero);
        }
        #endregion

        #region Operators
        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void Operator_Add_TwoVectors_ReturnOneVector(float3 x, float3 y)
        {
            var actual = x + y;

            Assert.Equal(new float3(1, 1, 1), actual);

        }

        [Fact]
        public void Operator_Division_VectorAndScalar_ReturnsVector()
        {
            var vec = new float3(2, 2, 2);
            var scalar = 2.0f;

            var actual = vec / scalar;

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Fact]
        public void Operator_Equality_IsEqual()
        {
            var actual = new float3(1, 1, 1);

            Assert.True(new float3(1, 1, 1) == actual);
        }

        [Fact]
        public void Explicit_DoubleToFloat()
        {
            var d = new double3(1, 1, 1);

            var actual = (float3)d;

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Fact]
        public void Operator_Equality_IsNotEqual()
        {
            var actual = new float3(1, 1, 1);

            Assert.False(new float3(0, 0, 0) == actual);
        }

        [Fact]
        public void Operator_Inequality_IsEqual()
        {
            var actual = new float3(1, 1, 1);

            Assert.False(new float3(1, 1, 1) != actual);
        }

        [Fact]
        public void Operator_Inequality_IsNotEqual()
        {
            var actual = new float3(1, 1, 1);

            Assert.True(new float3(0, 0, 0) != actual);
        }

        [Fact]
        public void Operator_Multiply_VectorAndScalar_ReturnVector()
        {
            var vec = new float3(1, 1, 1);
            var scalar = 2.0f;

            var actual = vec * scalar;

            Assert.Equal(new float3(2, 2, 2), actual);
        }

        [Fact]
        public void Operator_Multiply_ScalarAndVector_ReturnVector()
        {
            var vec = new float3(1, 1, 1);
            var scalar = 2.0f;

            var actual = scalar * vec;

            Assert.Equal(new float3(2, 2, 2), actual);
        }

        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void Operator_Multiply_TwoVectors_ReturnVector(float3 x, float3 y)
        {
            var actual = x * y;

            Assert.Equal(new float3(0, 0, 0), actual);
        }

        [Fact]
        public void Operator_Subtract_TwoVectors_ReturnVector()
        {
            var x = new float3(1, 1, 1);
            var y = new float3(1, 1, 1);

            var actual = x - y;

            Assert.Equal(new float3(0, 0, 0), actual);
        }

        [Fact]
        public void Operator_UnaryNegation_NegatePositiveVector()
        {
            var x = new float3(1, 1, 1);

            var actual = -x;

            Assert.Equal(new float3(-1, -1, -1), actual);
        }

        [Fact]
        public void Operator_UnaryNegation_NegateNegativeVector()
        {
            var x = new float3(-1, -1, -1);

            var actual = -x;

            Assert.Equal(new float3(1, 1, 1), actual);
        }
        #endregion

        #region Methods

        #region Add
        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void Add_TwoVectors_ReturnsVector(float3 x, float3 y)
        {
            var actual = float3.Add(x, y);

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void Add_TwoVectors_ToVector(float3 x, float3 y)
        {
            float3 actual;

            float3.Add(ref x, ref y, out actual);

            Assert.Equal(new float3(1, 1, 1), actual);
        }
        #endregion

        #region Barycentric
        [Theory]
        [MemberData(nameof(GetUVExpected))]
        public void Barycentric_VaryUV_ReturnResult(float u, float v, float3 expected)
        {
            var a = new float3(1, 0, 0);
            var b = new float3(0, 1, 0);
            var c = new float3(0, 0, 1);

            var actual = float3.Barycentric(a, b, c, u, v);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetUVExpected))]
        public void GetBarycentric_CheckUV(float expectedU, float expectedV, float3 point)
        {
            var a = new float3(1, 0, 0);
            var b = new float3(0, 1, 0);
            var c = new float3(0, 0, 1);
            float u;
            float v;

            float3.GetBarycentric(a, b, c, point, out u, out v);

            Assert.Equal(expectedU, u);
            Assert.Equal(expectedV, v);
        }
        #endregion

        #region CalculateAngle
        [Theory]
        [MemberData(nameof(GetFloat3ExpectedAngle))]
        public void CalculateAngle_TwoVectors_ReturnAngle(float3 a, float3 b, float expected)
        {
            float actual;

            actual = float3.CalculateAngle(a, b);

            Assert.Equal(expected, actual, 5);
        }

        [Theory]
        [MemberData(nameof(GetFloat3ExpectedAngle))]
        public void CalculateAngle_TwoVectors_SaveToAngle(float3 a, float3 b, float expected)
        {
            float actual;

            float3.CalculateAngle(ref a, ref b, out actual);

            Assert.Equal(expected, actual, 5);
        }
        #endregion

        #region Clamp
        [Theory]
        [MemberData(nameof(GetVecMinMaxExpected))]
        public void Clamp_VectorToMinToMax(float3 vec, float3 min, float3 max, float3 expected)
        {
            var actual = float3.Clamp(vec, min, max);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetVecMinMaxExpected))]
        public void Clamp_VectorToMinToMax_ToVector(float3 vec, float3 min, float3 max, float3 expected)
        {
            float3 actual;

            float3.Clamp(ref vec, ref min, ref max, out actual);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Components MinMax
        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void ComponentMax_TwoVectors_ReturnOne(float3 a, float3 b)
        {
            var actual = float3.ComponentMax(a, b);

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void ComponentMax_TwoVectors_ToVector(float3 a, float3 b)
        {
            float3 actual;

            float3.ComponentMax(ref a, ref b, out actual);

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void ComponentMin_TwoVectors_ReturnOne(float3 a, float3 b)
        {
            var actual = float3.ComponentMin(a, b);

            Assert.Equal(new float3(0, 0, 0), actual);
        }

        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void ComponentMin_TwoVectors_ToVector(float3 a, float3 b)
        {
            float3 actual;

            float3.ComponentMin(ref a, ref b, out actual);

            Assert.Equal(new float3(0, 0, 0), actual);
        }
        #endregion

        #region Cross
        [Theory]
        [MemberData(nameof(GetVectorCross))]
        public void Cross_TwoVectors_ReturnOne(float3 a, float3 b, float3 expected)
        {
            float3 actual;

            actual = float3.Cross(a, b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetVectorCross))]
        public void Cross_TwoVectors_ToVector(float3 a, float3 b, float3 expected)
        {
            float3 actual;

            float3.Cross(ref a, ref b, out actual);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Divide
        [Fact]
        public void Divide_TwoVectors_ReturnOne()
        {
            float3 a = new float3(2, 2, 2);
            float3 b = new float3(2, 2, 2);

            var actual = float3.Divide(a, b);

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Fact]
        public void Divide_TwoVectors_ToVector()
        {
            float3 a = new float3(2, 2, 2);
            float3 b = new float3(2, 2, 2);
            float3 actual;

            float3.Divide(ref a, ref b, out actual);

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Fact]
        public void Divide_VectorScalar_ReturnVector()
        {
            float3 vec = new float3(2, 2, 2);
            float scalar = 2;

            var actual = float3.Divide(vec, scalar);

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Fact]
        public void Divide_VectorScalar_ToVector()
        {
            float3 vec = new float3(2, 2, 2);
            float scalar = 2;
            float3 actual;

            float3.Divide(ref vec, scalar, out actual);

            Assert.Equal(new float3(1, 1, 1), actual);
        }
        #endregion

        #region Dot
        [Fact]
        public void Dot_TwoVectos_ReturnScalar()
        {
            float3 a = new float3(1, 1, 1);
            float3 b = new float3(1, 2, 3);

            var actual = float3.Dot(a, b);

            Assert.Equal(6, actual);
        }

        [Fact]
        public void Dot_TwoVectos_ToScalar()
        {
            float3 a = new float3(1, 1, 1);
            float3 b = new float3(1, 2, 3);
            float actual;

            float3.Dot(ref a, ref b, out actual);

            Assert.Equal(6, actual);
        }

        #endregion

        #region Lerp
        [Theory]
        [MemberData(nameof(GetBlendExpected))]
        public void Lerp_BlendTwoVectors_ReturnVector(float3 a, float3 b, float blend, float3 expected)
        {
            float3 actual;

            actual = float3.Lerp(a, b, blend);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetBlendExpected))]
        public void Lerp_BlendTwoVectors_ToVector(float3 a, float3 b, float blend, float3 expected)
        {
            float3 actual;

            float3.Lerp(ref a, ref b, blend, out actual);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region MinMax
        [Fact]
        public void Max_TwoVectors_ReturnVector()
        {
            var a = new float3(0, 0, 0);
            var b = new float3(1, 1, 1);

            var actual = float3.Max(a, b);
            var actual2 = float3.Max(b, a);

            Assert.Equal(b, actual);
            Assert.Equal(b, actual2);
        }

        [Fact]
        public void Min_TwoVectors_ReturnVector()
        {
            var a = new float3(0, 0, 0);
            var b = new float3(1, 1, 1);

            var actual = float3.Min(a, b);
            var actual2 = float3.Min(b, a);

            Assert.Equal(a, actual);
            Assert.Equal(a, actual2);
        }
        #endregion

        #region Multiply
        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void Multiply_TwoVectors_ReturnVector(float3 a, float3 b)
        {
            var actual = float3.Multiply(a, b);

            Assert.Equal(new float3(0, 0, 0), actual);
        }

        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void Multiply_TwoVectors_ToVector(float3 a, float3 b)
        {
            float3 actual;

            float3.Multiply(ref a, ref b, out actual);

            Assert.Equal(new float3(0, 0, 0), actual);
        }

        [Fact]
        public void Multiply_VectorScalar_ReturnVector()
        {
            var vec = new float3(1, 1, 1);
            var scalar = 0;

            var actual = float3.Multiply(vec, scalar);

            Assert.Equal(new float3(0, 0, 0), actual);
        }

        [Fact]
        public void Multiply_VectorScalar_ToVector()
        {
            var vec = new float3(1, 1, 1);
            var scalar = 0;
            float3 actual;

            float3.Multiply(ref vec, scalar, out actual);

            Assert.Equal(new float3(0, 0, 0), actual);
        }
        #endregion

        #region Normalize
        [Theory]
        [MemberData(nameof(GetVectorNorm))]
        public void Normalize_SameVector(float3 vec, float3 expected)
        {
            vec.Normalize();

            Assert.Equal(expected.x, vec.x, 6);
            Assert.Equal(expected.y, vec.y, 6);
            Assert.Equal(expected.z, vec.z, 6);
        }

        [Theory]
        [MemberData(nameof(GetVectorNorm))]
        public void Normalize_ReturnVecotr(float3 vec, float3 expected)
        {
            float3 actual;

            actual = float3.Normalize(vec);

            Assert.Equal(expected.x, actual.x, 6);
            Assert.Equal(expected.y, actual.y, 6);
            Assert.Equal(expected.z, actual.z, 6);
        }

        [Theory]
        [MemberData(nameof(GetVectorNorm))]
        public void Normalize_ToVector(float3 vec, float3 expected)
        {
            float3 actual;

            float3.Normalize(ref vec, out actual);

            Assert.Equal(expected.x, actual.x, 6);
            Assert.Equal(expected.y, actual.y, 6);
            Assert.Equal(expected.z, actual.z, 6);
        }

        [Theory]
        [MemberData(nameof(GetVectorNorm))]
        public void NormalizeFast_SameVector(float3 vec, float3 expected)
        {
            vec.Normalize();

            Assert.Equal(expected.x, vec.x, 6);
            Assert.Equal(expected.y, vec.y, 6);
            Assert.Equal(expected.z, vec.z, 6);
        }

        [Theory]
        [MemberData(nameof(GetVectorNorm))]
        public void NormalizeFast_ReturnVecotr(float3 vec, float3 expected)
        {
            float3 actual;

            actual = float3.Normalize(vec);

            Assert.Equal(expected.x, actual.x, 6);
            Assert.Equal(expected.y, actual.y, 6);
            Assert.Equal(expected.z, actual.z, 6);
        }

        [Theory]
        [MemberData(nameof(GetVectorNorm))]
        public void NormalizeFast_ToVector(float3 vec, float3 expected)
        {
            float3 actual;

            float3.Normalize(ref vec, out actual);

            Assert.Equal(expected.x, actual.x, 6);
            Assert.Equal(expected.y, actual.y, 6);
            Assert.Equal(expected.z, actual.z, 6);
        }

        [Fact]
        public void OrthoNormalize_NormTangent_ReturnArray()
        {
            var normal = new float3(5, 0, 0);
            var tangent = new float3(4, 4, 4);

            var actual = float3.OrthoNormalize(normal, tangent);

            Assert.Equal(new float3(1,0,0), actual[0]);
            Assert.Equal(0, actual[1].x, 3);
            Assert.Equal(0.707f, actual[1].y, 3);
            Assert.Equal(0.707f, actual[1].z, 3);
        }
        #endregion

        #region Subtract
        [Fact]
        public void Subtract_TwoVectors_ReturnVector()
        {
            var a = new float3(1, 1, 1);

            var actual = float3.Subtract(a, a);

            Assert.Equal(new float3(0, 0, 0), actual);
        }

        [Fact]
        public void Subtract_TwoVectors_ToVector()
        {
            var a = new float3(1, 1, 1);
            float3 actual;

            float3.Subtract(ref a, ref a, out actual);

            Assert.Equal(new float3(0, 0, 0), actual);
        }
        #endregion

        #region Transform
        [Theory]
        [MemberData(nameof(GetRotationMatrix))]
        public void Transform_VectorAndMatrix_ReturnVector(float3 vec, float4x4 matrix, float3 expected)
        {
            float3 actual;

            actual = float3.Transform(vec, matrix);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetQuaternion))]
        public void Transform_VectorAndQuaternion_ReturnVector(float3 vec, Quaternion quat, float3 expected)
        {
            float3 actual;

            actual = float3.Transform(vec, quat);

            Assert.Equal(expected.x, actual.x, 3);
            Assert.Equal(expected.y, actual.y, 3);
            Assert.Equal(expected.z, actual.z, 3);
        }

        [Theory]
        [MemberData(nameof(GetQuaternion))]
        public void Transform_VectorAndQuaternion_ToVector(float3 vec, Quaternion quat, float3 expected)
        {
            float3 actual;

            float3.Transform(ref vec, ref quat, out actual);

            Assert.Equal(expected.x, actual.x, 3);
            Assert.Equal(expected.y, actual.y, 3);
            Assert.Equal(expected.z, actual.z, 3);
        }

        [Theory]
        [MemberData(nameof(GetRotationMatrix))]
        public void TransformNormal_VectorAndMatrix_ReturnVector(float3 vec, float4x4 matrix, float3 expected)
        {
            float3 actual;

            actual = float3.TransformNormal(vec, matrix);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetRotationMatrix))]
        public void TransformNormal_VectorAndMatrix_ToVector(float3 vec, float4x4 matrix, float3 expected)
        {
            float3 actual;

            float3.TransformNormal(ref vec, ref matrix, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetRotationMatrix))]
        public void TransformNormalInverse_VectorAndMatrix_ReturnVector(float3 vec, float4x4 matrix, float3 expected)
        {
            float3 actual;

            actual = float3.TransformNormalInverse(vec, matrix);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetRotationMatrix))]
        public void TransformNormalInverse_VectorAndMatrix_ToVector(float3 vec, float4x4 matrix, float3 expected)
        {
            float3 actual;

            float3.TransformNormalInverse(ref vec, ref matrix, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetRotationMatrix))]
        public void TransformPerspective_VectorAndMatrix_ToVector(float3 vec, float4x4 matrix, float3 expected)
        {
            vec.TransformPerspective(matrix);

            Assert.Equal(expected, vec);
        }

        [Theory]
        [MemberData(nameof(GetRotationMatrix))]
        public void TransfromPosition_VectorAndMatrix_ReturnVector(float3 vec, float4x4 matrix, float3 expected)
        {
            float3 actual;

            actual = float3.TransformPosition(vec, matrix);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetRotationMatrix))]
        public void TransfromPosition_VectorAndMatrix_ToVector(float3 vec, float4x4 matrix, float3 expected)
        {
            float3 actual;

            float3.TransformPosition(ref vec, ref matrix, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetRotationMatrix))]
        public void TransformVector_VectorAndMatrix_ReturnVector(float3 vec, float4x4 matrix, float3 expected)
        {
            float3 actual;

            actual = float3.TransformVector(vec, matrix);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetRotationMatrix))]
        public void TransformVector_VectorAndMatrix_ToVector(float3 vec, float4x4 matrix, float3 expected)
        {
            float3 actual;

            float3.TransformVector(ref vec, ref matrix, out actual);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Other
        [Fact]
        public void Equal_IsEqual()
        {
            var a = new float3(1, 1, 1);
            var b = new float3(1, 1, 1);

            Assert.True(float3.Equals(a, b));
        }

        [Fact]
        public void Equal_IsInequal()
        {
            var a = new float3(1, 1, 1);
            var b = new float3(0, 0, 0);

            Assert.False(float3.Equals(a, b));
        }

        //TODO: HashCode & Type
        [Fact]
        public void ToArray_VectorToFloat()
        {
            var vec = new float3(1, 2, 3);

            var actual = vec.ToArray();

            Assert.Equal(new float[] { 1, 2, 3 }, actual);
        }

        [Fact]
        public void ToString_VectorToString()
        {
            var vec = new float3(1, 2, 3);

            var actual = vec.ToString();

            Assert.Equal("(1, 2, 3)", actual);
        }
        #endregion

        #endregion

        #region Constructors
        [Fact]
        public void Float3_fromDouble3()
        {
            var actual = new double3(1, 1, 1);

            Assert.Equal(new float3(1, 1, 1), new float3(actual));
        }

        [Fact]
        public void Float3_fromFloat2()
        {
            var actual = new float2(1, 1);

            Assert.Equal(new float3(1, 1, 0), new float3(actual));
        }

        [Fact]
        public void Float3_fromFloat3()
        {
            var actual = new float3(1, 1, 1);

            Assert.Equal(new float3(1, 1, 1), new float3(actual));
        }

        [Fact]
        public void Float3_fromFloat4()
        {
            var actual = new float4(1, 1, 1, 1);

            Assert.Equal(new float3(1, 1, 1), new float3(actual));
        }
        #endregion

        #region Properties
        [Theory]
        [MemberData(nameof(GetVecLength))]
        public void Length_TestLength(float3 vec, float expected)
        {
            var actual = vec.Length;

            Assert.Equal(expected, actual, 5);
        }

        [Theory]
        [MemberData(nameof(GetVecLength))]
        public void LengthFast_TestLength(float3 vec, float expected)
        {
            var actual = vec.Length;

            Assert.Equal(expected, actual, 5);
        }

        [Theory]
        [MemberData(nameof(GetVecLength))]
        public void Lengthsquared_TestLength(float3 vec, float expected)
        {
            var actual = vec.LengthSquared;
            expected *= expected;

            Assert.Equal(expected, actual, 5);
        }
        #endregion
    }
}
