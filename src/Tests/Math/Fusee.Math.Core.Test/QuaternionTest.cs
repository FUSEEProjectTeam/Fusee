using Xunit;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using Xunit.Sdk;
using Assert = Xunit.Assert;

namespace Fusee.Math.Core
{
    public class QuaternionTest
    {
        #region Fields

        [Fact]
        public void Identity_IsIdentity()
        {
            var expected = new Quaternion(0, 0, 0, 1);

            Assert.Equal(expected, Quaternion.Identity);
        }

        #endregion

        #region Constructors

        [Fact]
        public void Constructor_Float3Single()
        {
            var actual = new Quaternion(new float3(1, 0, 0), 1);

            Assert.Equal(new Quaternion(1, 0, 0, 1), actual);
        }

        [Fact]
        public void Constructor_Singles()
        {
            var actual = new Quaternion(1,0,0, 1);

            Assert.Equal(new Quaternion(new float3(1,0,0), 1), actual);
        }

        #endregion

        #region Properties

        [Fact]
        public void Get_XYZ()
        {
            var quat = new Quaternion(1, 2, 3, 4);

            Assert.Equal(new float3(1,2,3), quat.xyz);
        }

        [Fact]
        public void Get_X()
        {
            var quat = new Quaternion(1, 2, 3, 4);

            Assert.Equal(1, quat.x);
        }

        [Fact]
        public void Get_Y()
        {
            var quat = new Quaternion(1, 2, 3, 4);

            Assert.Equal(2, quat.y);
        }

        [Fact]
        public void Get_Z()
        {
            var quat = new Quaternion(1, 2, 3, 4);

            Assert.Equal(3, quat.z);
        }

        [Fact]
        public void Get_W()
        {
            var quat = new Quaternion(1, 2, 3, 4);

            Assert.Equal(4, quat.w);
        }

        [Fact]
        public void Length_Is2()
        {
            var quat = new Quaternion(1, 1, 1, 1);

            var actual = quat.Length;

            Assert.Equal(2, actual);
        }

        [Fact]
        public void LengthSquared_Is4()
        {
            var quat = new Quaternion(1, 1, 1, 1);

            var actual = quat.LengthSquared;

            Assert.Equal(4, actual);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Operator_Addition(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Operator_Subtraction(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiplicationQuaterions))]
        public void Operator_Multiplication_Quaternions(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiplicationScalar))]
        public void Operator_Multiplication_Scalar(Quaternion quat, float scale, Quaternion expected)
        {
            var actual = quat * scale;

            Assert.Equal(expected, actual);

            actual = scale * quat;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new Quaternion(1, 1, 1, 1);
            var b = new Quaternion(1, 1, 1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new Quaternion(1, 1, 1, 1);
            var b = new Quaternion(0, 0, 0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new Quaternion(1, 1, 1, 1);
            var b = new Quaternion(1, 1, 1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new Quaternion(1, 1, 1, 1);
            var b = new Quaternion(0, 0, 0, 0);

            Assert.True(a != b);
        }

        #endregion

        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_TwoQuaternions(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = Quaternion.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Sub_TwoQuaternions(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = Quaternion.Sub(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiplicationQuaterions))]
        public void Multiply_TwoQuaternions(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = Quaternion.Multiply(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiplicationScalar))]
        public void Multiply_QuaternionScalar(Quaternion quat, float scale, Quaternion expected)
        {
            var actual = Quaternion.Multiply(quat, scale);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Conjugate

        [Fact]
        public void Conjugate_Static()
        {
            var quat = new Quaternion(1, 1, 1, 2);

            var actual = Quaternion.Conjugate(quat);

            Assert.Equal(new Quaternion(-1, -1, -1, 2), actual);
        }

        [Fact]
        public void Conjugate_Instance()
        {
            var quat = new Quaternion(1, 1, 1, 2);

            var actual = quat.Conjugate();

            Assert.Equal(new Quaternion(-1, -1, -1, 2), actual);
        }

        #endregion

        #region Invert

        [Fact]
        public void Invert_Static()
        {
            var quat = new Quaternion(1, 1, 1, 1);

            var actual = Quaternion.Invert(quat);

            Assert.Equal(new Quaternion(-0.25f, -0.25f, -0.25f, 0.25f), actual);
        }

        [Fact]
        public void Invert_Instance()
        {
            var quat = new Quaternion(1, 1, 1, 1);

            var actual = quat.Invert();

            Assert.Equal(new Quaternion(-0.25f, -0.25f, -0.25f, 0.25f), actual);
        }

        #endregion

        #region Normalize

        [Fact]
        public void Normalize_Static()
        {
            var quat = new Quaternion(1, 1, 1, 1);

            var actual = Quaternion.Normalize(quat);

            Assert.Equal(new Quaternion(0.5f, 0.5f, 0.5f, 0.5f), actual);
        }

        [Fact]
        public void Normalize_Instance()
        {
            var quat = new Quaternion(1, 1, 1, 1);

            var actual = quat.Normalize();

            Assert.Equal(new Quaternion(0.5f, 0.5f, 0.5f, 0.5f), actual);
        }

        #endregion

        #region FromAxisAngle

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void FromAxisAngle_MainAxes(float3 axis, float angle, Quaternion expected)
        {
            var actual = Quaternion.FromAxisAngle(axis, angle);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Slerp

        [Theory]
        [MemberData(nameof(GetSlerp))]
        public void Slerp_IsHalf(Quaternion a, Quaternion b, float blend, Quaternion expected)
        {
            var actual = Quaternion.Slerp(a, b, blend);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Conversion

        [Theory]
        [MemberData(nameof(GetEulerQuaternion))]
        public void EulerToQuaternion_MainAxes(float3 euler, bool inDegrees, Quaternion expected)
        {
            var actual = Quaternion.EulerToQuaternion(euler, inDegrees);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetEulerQuaternion))]
        public void QuaternionToEuler_MainAxes(float3 expected, bool inDegrees, Quaternion quat)
        {
            var actual = Quaternion.QuaternionToEuler(quat, inDegrees);

            Assert.Equal(expected.x, actual.x, 4);
            Assert.Equal(expected.y, actual.y, 4);
            Assert.Equal(expected.z, actual.z, 4);
        }

        [Theory]
        [MemberData(nameof(GetMatrixQuaternion))]
        public void QuaternionToMatrix_MainRotations(Quaternion quat, float4x4 expected)
        {
            var actual = Quaternion.QuaternionToMatrix(quat);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LookRotation_IsRotation()
        {
            var lookAt = new float3(1, 0, 0);
            var upDirection = new float3(0, 1, 0);

            var actual = Quaternion.LookRotation(lookAt, upDirection);

            Assert.Equal(new Quaternion(0,0.7071068f,0,0.7071068f), actual);
        }

        [Fact]
        public void CopySign_IsSign()
        {
            var a = 1;
            var b = -2;

            var actual = Quaternion.CopySign(a, b);

            Assert.Equal(-1, actual);
        }

        #endregion

        #region FromToRotation

        [Fact]
        public void FromToRotation_TestShortest()
        {
            var from = new float3(0, 0, 0);
            var to = new float3(90, 0, 0);

            var actual = Quaternion.FromToRotation(from, to);

            Assert.Equal(new Quaternion(0,0,0,0), actual);
        }

        #endregion

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetAddition()
        {
            var one = new Quaternion(1, 1, 1, 1);
            var zero = new Quaternion(0, 0, 0, 0);

            yield return new object[] {one, zero, one};
            yield return new object[] {zero, one, one};
            yield return new object[] {zero, zero, zero};
            yield return new object[] {one, one, new Quaternion(2, 2, 2, 2)};
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var one = new Quaternion(1, 1, 1, 1);
            var zero = new Quaternion(0, 0, 0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] {zero, one, new Quaternion(-1, -1, -1, -1)};
            yield return new object[] { zero, zero, zero };
            yield return new object[] {one, one, zero};
        }

        public static IEnumerable<object[]> GetMultiplicationQuaterions()
        {
            var one = new Quaternion(1, 1, 1, 1);
            var zero = new Quaternion(0, 0, 0, 0);

            yield return new object[] {one, one, new Quaternion(2, 2, 2, -2)};
            yield return new object[] {one, zero, zero};
            yield return new object[] {zero, one, zero};
        }

        public static IEnumerable<object[]> GetMultiplicationScalar()
        {
            yield return new object[] {new Quaternion(1, 1, 1, 1), 2, new Quaternion(2, 2, 2, 2)};
            yield return new object[] {new Quaternion(0, 0, 0, 0), 4, new Quaternion(0, 0, 0, 0)};
        }

        public static IEnumerable<object[]> GetAxisAngle()
        {
            yield return new object[] {new float3(1, 0, 0), M.DegreesToRadians(90), new Quaternion(0.7071068f, 0, 0, 0.7071068f)};
            yield return new object[] {new float3(0, 1, 0), M.DegreesToRadians(90), new Quaternion(0, 0.7071068f, 0, 0.7071068f)};
            yield return new object[] {new float3(0, 0, 1), M.DegreesToRadians(90), new Quaternion(0, 0, 0.7071068f, 0.7071068f)};
        }

        public static IEnumerable<object[]> GetSlerp()
        {
            var zero = new Quaternion(0, 0, 0, 0);
            var one = new Quaternion(1, 1, 1, 1);

            yield return new object[] {one, zero, 0.5f, one};
            yield return new object[] {zero, one, 0.5f, one};
            yield return new object[] {zero, zero, 0.5f, new Quaternion(0, 0, 0, 1)};
            yield return new object[] {new Quaternion(0.7071068f, 0, 0, 0.7071068f), new Quaternion(0, 0.7071068f, 0, 0.7071068f), 0.5f, new Quaternion(0.4082483f, 0.4082483f, 0, 0.8164967f)};
        }

        public static IEnumerable<object[]> GetEulerQuaternion()
        {
            yield return new object[] {new float3(90, 0, 0), true, new Quaternion(0.7071068f, 0, 0, 0.7071068f)};
            yield return new object[] {new float3(0, 90, 0), true, new Quaternion(0, 0.7071068f, 0, 0.7071068f)};
            yield return new object[] {new float3(0, 0, 90), true, new Quaternion(0, 0, 0.7071068f, 0.7071068f)};
        }

        public static IEnumerable<object[]> GetMatrixQuaternion()
        {
            var xRot = new float4x4(new float4(1, 0, 0, 0), new float4(0, 0, -1, 0), new float4(0, 1, 0, 0), new float4(0, 0, 0, 1));
            var yRot = new float4x4(new float4(0, 0, 1, 0), new float4(0, 1, 0, 0), new float4(-1, 0, 0, 0), new float4(0, 0, 0, 1));
            var zRot = new float4x4(new float4(0, -1, 0, 0), new float4(1, 0, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            yield return new object[] {new Quaternion(0.7071068f, 0, 0, 0.7071068f), xRot};
            yield return new object[] {new Quaternion(0, 0.7071068f, 0, 0.7071068f), yRot};
            yield return new object[] {new Quaternion(0, 0, 0.7071068f, 0.7071068f), zRot};
        }

        #endregion
    }
}