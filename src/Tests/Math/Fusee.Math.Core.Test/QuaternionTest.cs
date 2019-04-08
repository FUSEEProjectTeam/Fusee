using Xunit;
using System;
using System.Collections.Generic;

namespace Fusee.Math.Core
{
    public class QuaternionTest
    {
        #region Fields
        [Fact]
        public void Identity_IsIdentity()
        {
            var actual = Quaternion.Identity;

            Assert.Equal(new Quaternion(0, 0, 0, 1), actual);
        }
        #endregion

        #region Properties
        [Fact]
        public void Length_IsOne()
        {
            var quat = new Quaternion(0, 0, 0, 1);

            var actual = quat.Length;

            Assert.Equal(1, actual);
        }

        [Fact]
        public void LengthSquared_IsTwo()
        {
            var quat = new Quaternion(1, 0, 0, 1);

            var actual = quat.LengthSquared;

            Assert.Equal(2, actual);
        }

        [Fact]
        public void XYZ_IsVector()
        {
            var quat = new Quaternion(1, 2, 3, 1);

            var actual = quat.xyz;

            Assert.Equal(new float3(1, 2, 3), actual);
        }

        [Fact]
        public void W_IsOne()
        {
            var quat = new Quaternion(0, 0, 0, 1);

            var actual = quat.w;

            Assert.Equal(1, actual);
        }
        #endregion

        #region Operators
        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Addition_TwoQuaternions(Quaternion a, Quaternion b, Quaternion expected)
        {
            var actual = a + b;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new Quaternion(0, 0, 0, 1);
            var b = new Quaternion(0, 0, 0, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new Quaternion(0, 0, 0, 1);
            var b = new Quaternion(1, 0, 0, 1);

            Assert.False(a == b);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new Quaternion(0, 0, 0, 1);
            var b = new Quaternion(0, 0, 0, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new Quaternion(0, 0, 0, 1);
            var b = new Quaternion(1, 0, 0, 1);

            Assert.True(a != b);
        }

        [Fact]
        public void Multiply_QuaternionScalar()
        {
            var quat = new Quaternion(1, 0, 0, 1);

            var actual = quat * 2;

            Assert.Equal(new Quaternion(2, 0, 0, 2), actual);
        }

        [Fact]
        public void Multiply_ScalarQuaternion()
        {
            var quat = new Quaternion(1, 0, 0, 1);

            var actual = 2 * quat;

            Assert.Equal(new Quaternion(2, 0, 0, 2), actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiplication))]
        public void Multiply_TwoQuaternions(Quaternion a, Quaternion b, Quaternion expected)
        {
            var actual = a * b;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Subtraction_IsZero()
        {
            var quat = new Quaternion(1, 1, 1, 1);

            var actual = quat - quat;

            Assert.Equal(new Quaternion(0, 0, 0, 0), actual);
        }
        #endregion

        #region Methods

        #region Arithmetic Functions
        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_ReturnMatrix(Quaternion a, Quaternion b, Quaternion expected)
        {
            Quaternion actual;

            actual = Quaternion.Add(a, b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_ToMatrix(Quaternion a, Quaternion b, Quaternion expected)
        {
            Quaternion actual;

            Quaternion.Add(ref a, ref b, out actual);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Sub_IsZero_ReturnQuaternion()
        {
            var quat = new Quaternion(1, 1, 1, 1);

            var actual = Quaternion.Sub(quat, quat);

            Assert.Equal(new Quaternion(0, 0, 0, 0), actual);
        }

        [Fact]
        public void Sub_IsZero_ToQuaternion()
        {
            var quat = new Quaternion(1, 1, 1, 1);
            Quaternion actual;

            Quaternion.Sub(ref quat, ref quat, out actual);

            Assert.Equal(new Quaternion(0, 0, 0, 0), actual);
        }
        #endregion

        #region Conjugate
        [Fact]
        public void Conjugate_Direct()
        {
            var actual = new Quaternion(1, 0, 0, 1);

            actual.Conjugate();

            Assert.Equal(new Quaternion(-1, 0, 0, 1), actual);
        }

        [Fact]
        public void Conjugate_ReturnMatrix()
        {
            var quat = new Quaternion(1, 0, 0, 1);

            var actual = Quaternion.Conjugate(quat);

            Assert.Equal(new Quaternion(-1, 0, 0, 1), actual);
        }

        [Fact]
        public void Conjugate_ToMatrix()
        {
            var quat = new Quaternion(1, 0, 0, 1);
            Quaternion actual;

            Quaternion.Conjugate(ref quat, out actual);

            Assert.Equal(new Quaternion(-1, 0, 0, 1), actual);
        }
        #endregion

        #region Conversion
        [Theory]
        [MemberData(nameof(GetEuler))]
        public void EulerToQuaternion_ReturnQuaternion(float3 angle, bool inDegrees, Quaternion expected)
        {
            Quaternion actual;

            actual = Quaternion.EulerToQuaternion(angle, inDegrees);

            Assert.Equal(expected.x, actual.x, 3);
            Assert.Equal(expected.y, actual.y, 3);
            Assert.Equal(expected.z, actual.z, 3);
            Assert.Equal(expected.w, actual.w, 3);
        }

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void FromAxisAngle_ReturnQuaternion(float3 axis, float angle, Quaternion expected)
        {
            Quaternion actual;

            actual = Quaternion.FromAxisAngle(axis, angle);

            Assert.Equal(expected.x, actual.x, 3);
            Assert.Equal(expected.y, actual.y, 3);
            Assert.Equal(expected.z, actual.z, 3);
            Assert.Equal(expected.w, actual.w, 3);
        }

        //TODO: LookRotation
        [Theory]
        [MemberData(nameof(GetLookRotation))]
        public void LookRotation_ReturnQuaternion(float3 lookAt, float3 upDirection, Quaternion expected)
        {
            var actual = Quaternion.LookRotation(lookAt, upDirection);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetEuler))]
        public void QuaternionToEuler_ReturnEuler(float3 expected, bool inDegrees, Quaternion quat)
        {
            float3 actual;

            actual = Quaternion.QuaternionToEuler(quat, inDegrees);

            Assert.Equal(expected.x, actual.x, 4);
            Assert.Equal(expected.y, actual.y, 4);
            Assert.Equal(expected.z, actual.z, 4);
        }

        [Theory]
        [MemberData(nameof(GetMatrix))]
        public void QuaternionToMatrix_ReturnMatrix(Quaternion quat, float4x4 expected)
        {
            float4x4 actual;

            actual = Quaternion.QuaternionToMatrix(quat);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void ToAxisAngle_ReturnAxisAngle(float3 expectedVec, float expectedAngle, Quaternion quat)
        {
            float3 actualVec;
            float actualAngle;

            float4 actual = quat.ToAxisAngle();
            actualVec = actual.xyz;
            actualAngle = actual.w;

            Assert.Equal(expectedVec.x, actualVec.x, 3);
            Assert.Equal(expectedVec.y, actualVec.y, 3);
            Assert.Equal(expectedVec.z, actualVec.z, 3);
            Assert.Equal(expectedAngle, actualAngle, 3);
        }

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void ToAxisAngle_ToAxisAngle(float3 expectedVec, float expectedAngle, Quaternion quat)
        {
            float3 actualVec;
            float actualAngle;

            quat.ToAxisAngle(out actualVec, out actualAngle);

            Assert.Equal(expectedVec.x, actualVec.x, 3);
            Assert.Equal(expectedVec.y, actualVec.y, 3);
            Assert.Equal(expectedVec.z, actualVec.z, 3);
            Assert.Equal(expectedAngle, actualAngle, 3);
        }
        #endregion

        #region Invert
        [Fact]
        public void Invert_ReturnQuaternion()
        {
            var quat = new Quaternion(1, 0, 0, 1);

            var actual = Quaternion.Invert(quat);

            Assert.Equal(new Quaternion(-0.5f, 0, 0, 0.5f), actual);
        }

        [Fact]
        public void Invert_ToQuaternion()
        {
            var quat = new Quaternion(1, 0, 0, 1);
            Quaternion actual;

            Quaternion.Invert(ref quat, out actual);

            Assert.Equal(new Quaternion(-0.5f, 0, 0, 0.5f), actual);
        }
        #endregion

        #region Multiply
        [Theory]
        [MemberData(nameof(GetMultiplication))]
        public void Multiply_TwoQuaternions_ReturnQuaternion(Quaternion a, Quaternion b, Quaternion expected)
        {
            Quaternion actual;
            
            actual = Quaternion.Multiply(a, b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiplication))]
        public void Multiply_TwoQuaternions_ToQuaternion(Quaternion a, Quaternion b, Quaternion expected)
        {
            Quaternion actual;

            Quaternion.Multiply(ref a, ref b, out actual);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Multiply_QuaternionScalar_ReturnQuaternion()
        {
            var quat = new Quaternion(1, 0, 0, 1);
            Quaternion actual;

            actual = Quaternion.Multiply(quat, 2);

            Assert.Equal(new Quaternion(2, 0, 0, 2), actual);
        }

        [Fact]
        public void Multiply_QuaternionScalar_ToQuaternion()
        {
            var quat = new Quaternion(1, 0, 0, 1);
            Quaternion actual;

            Quaternion.Multiply(ref quat, 2, out actual);

            Assert.Equal(new Quaternion(2, 0, 0, 2), actual);
        }
        #endregion

        #region Normalize
        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Direct(Quaternion quat, Quaternion expected)
        {
            quat.Normalize();

            var actual = quat;

            Assert.Equal(expected.x, actual.x, 3);
            Assert.Equal(expected.y, actual.y, 3);
            Assert.Equal(expected.z, actual.z, 3);
            Assert.Equal(expected.w, actual.w, 3);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_ReturnMatrix(Quaternion quat, Quaternion expected)
        {
            Quaternion actual;

            actual = Quaternion.Normalize(quat);

            Assert.Equal(expected.x, actual.x, 3);
            Assert.Equal(expected.y, actual.y, 3);
            Assert.Equal(expected.z, actual.z, 3);
            Assert.Equal(expected.w, actual.w, 3);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_ToMatrix(Quaternion quat, Quaternion expected)
        {
            Quaternion actual;

            Quaternion.Normalize(ref quat, out actual);

            Assert.Equal(expected.x, actual.x, 3);
            Assert.Equal(expected.y, actual.y, 3);
            Assert.Equal(expected.z, actual.z, 3);
            Assert.Equal(expected.w, actual.w, 3);
        }
        #endregion

        #region Slerp
        [Theory]
        [MemberData(nameof(GetSlerp))]
        public void Slerp_ReturnQuaternion(Quaternion quat1, Quaternion quat2, float blend, Quaternion expected)
        {
            quat1.Normalize();
            quat2.Normalize();

            var actual = Quaternion.Slerp(quat1, quat2, blend);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Other
        [Fact]
        public void CopySign_IsMinusOne()
        {
            var a = 1;
            var b = -2;

            var actual = Quaternion.CopySign(a, b);

            Assert.Equal(-a, actual);
        }

        [Fact]
        public void Equals_IsEqual()
        {
            var a = new Quaternion(1, 0, 0, 1);
            var b = new Quaternion(1, 0, 0, 1);

            Assert.True(a.Equals(b));
        }

        [Fact]
        public void Equals_IsInequal()
        {
            var a = new Quaternion(1, 0, 0, 1);
            var b = new Quaternion(0, 0, 0, 1);

            Assert.False(a.Equals(b));
        }

        //TODO: GetHashCodes

        //TODO: GetType

        [Fact]
        public void ToString_IsString()
        {
            var quat = new Quaternion(1, 0, 0, 1);

            var actual = quat.ToString();

            Assert.Equal("V: (1, 0, 0), w: 1", actual);
        }
        #endregion

        #endregion

        #region IEnumberables
        public static IEnumerable<object[]> GetAddition()
        {
            var quat1 = new Quaternion(1, 1, 1, 0);
            var quat2 = new Quaternion(1, 1, 1, 1);

            yield return new object[] { quat1, Quaternion.Identity, quat2 };
            yield return new object[] { Quaternion.Identity, quat1, quat2 };
        }

        public static IEnumerable<object[]> GetMultiplication()
        {
            var a = new Quaternion(1, 1, 1, 1);
            var b = new Quaternion(1, 0, 0, 1);

            yield return new object[] { a, Quaternion.Identity, a };
            yield return new object[] { Quaternion.Identity, a, a };
        }

        public static IEnumerable<object[]> GetEuler()
        {
            yield return new object[] { new float3(0, 0, 0), false, new Quaternion(0, 0, 0, 1) };
            yield return new object[] { new float3(90, 0, 0), true, new Quaternion(0.707f, 0, 0, 0.707f) };
            yield return new object[] { new float3(0, 90, 0), true, new Quaternion(0, 0.707f, 0, 0.707f) };
            yield return new object[] { new float3(0, 0, 90), true, new Quaternion(0, 0, 0.707f, 0.707f) };
        }

        public static IEnumerable<object[]> GetAxisAngle()
        {
            yield return new object[] { new float3(1, 0, 0), 0, new Quaternion(0, 0, 0, 1) };
            yield return new object[] { new float3(1, 0, 0), 1.5708f, new Quaternion(0.707f, 0, 0, 0.707f) };
            yield return new object[] { new float3(0, 1, 0), 1.5708f, new Quaternion(0, 0.707f, 0, 0.707f) };
            yield return new object[] { new float3(0, 0, 1), 1.5708f, new Quaternion(0, 0, 0.707f, 0.707f) };
        }

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new Quaternion(1, 0, 0, 1), new Quaternion(0.707f, 0, 0, 0.707f) };
            yield return new object[] { new Quaternion(0, 0, 0, 1), new Quaternion(0, 0, 0, 1) };
            yield return new object[] { new Quaternion(1, 1, 1, 1), new Quaternion(0.5f, 0.5f, 0.5f, 0.5f) };
        }

        public static IEnumerable<object[]> GetMatrix()
        {
            yield return new object[] { new Quaternion(1, 0, 0, 1), new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1) };
            yield return new object[] { new Quaternion(0, 1, 0, 1), new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1) };
            yield return new object[] { new Quaternion(0, 0, 1, 1), new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1) };
        }

        public static IEnumerable<object[]> GetSlerp()
        {
            yield return new object[] { new Quaternion(0, 0, 0, 1), new Quaternion(1, 0, 0, 1), 0.5f, new Quaternion(0.3826835f, 0, 0, 0.9238796f) };
            yield return new object[] { new Quaternion(1, 0, 0, 1), new Quaternion(1, 0, 0, 1), 0.5f, new Quaternion(0.7071068f, 0, 0, 0.7071068f) };
        }

        public static IEnumerable<object[]> GetLookRotation()
        {
            yield return new object[] { new float3(0, 0, 0), new float3(0, 1, 0), new Quaternion(0, 0, 0, 0.7071068f) };
            yield return new object[] { new float3(1, 0, 0), new float3(0, 1, 0), new Quaternion(0, 0.7071068f, 0, 0.7071068f) };
        }
        #endregion
    }
}
