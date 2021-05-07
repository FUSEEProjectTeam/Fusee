using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Fusee.Test.Math.Core
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
        public void Constructor_TestConstructors()
        {
            var actual = new Quaternion(new float3(1, 2, 3), 4);
            var expected = new Quaternion(1, 2, 3, 4);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region this

        [Fact]
        public void This_GetWithIdx_IsValid()
        {
            var actual = new Quaternion(3, 7, 8, 1);

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);

            Assert.Equal(8, actual[2]);

            Assert.Equal(1, actual[3]);
        }

        [Fact]
        public void This_SetWithIdx_IsValid()
        {
            var actual = new Quaternion(0, 0, 0, 0);
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
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => new Quaternion(0, 0, 0, 0)[idx]);

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_SetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var q4 = new Quaternion(0, 0, 0, 0); q4[idx] = 10; });

            Assert.Equal(expected, actual.ParamName);
        }

        public static IEnumerable<object[]> ThisException()
        {
            yield return new object[] { 7, "Index 7 not eligible for a Quaternion type" };
            yield return new object[] { 6, "Index 6 not eligible for a Quaternion type" };
        }

        #endregion

        #region Properties

        [Fact]
        public void Getter_Test()
        {
            var actual = new Quaternion(1, 2, 3, 4);

            Assert.Equal(new float3(1, 2, 3), actual.xyz);
            Assert.Equal(1, actual.x);
            Assert.Equal(2, actual.y);
            Assert.Equal(3, actual.z);
            Assert.Equal(4, actual.w);
        }

        [Fact]
        public void Setter_Test()
        {
            var actual = new Quaternion();

            actual.x = 1;
            actual.y = 2;
            actual.z = 3;
            actual.w = 4;
            Assert.Equal(new Quaternion(1, 2, 3, 4), actual);

            actual.xyz = new float3(3, 2, 1);
            Assert.Equal(new Quaternion(3, 2, 1, 4), actual);
        }

        #endregion

        #region Instance

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void ToAxisAngle_Instance(Quaternion quat, float4 expected)
        {
            var actual = quat.ToAxisAngle();

            Assert.Equal(expected, actual);
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

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(Quaternion quat, Quaternion expected)
        {
            var actual = quat.Normalize();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Conjugate_Instance()
        {
            var quat = new Quaternion(1, 2, 3, 4);

            var actual = quat.Conjugate();

            Assert.Equal(new Quaternion(-1, -2, -3, 4), actual);
        }

        [Fact]
        public void Invert_Instance()
        {
            var quat = new Quaternion(1, 1, 1, 1);

            var actual = quat.Invert();

            Assert.Equal(new Quaternion(-0.25f, -0.25f, -0.25f, 0.25f), actual);
        }

        #endregion

        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Static(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = Quaternion.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Sub_Static(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = Quaternion.Sub(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Mult

        [Theory]
        [MemberData(nameof(GetQuaternionMultiplication))]
        public void Multiply_TwoQuaternions_Static(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = Quaternion.Multiply(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScalarMultiplication))]
        public void Multiply_QuaternionScalar_Staic(Quaternion quat, float scale, Quaternion expected)
        {
            var actual = Quaternion.Multiply(quat, scale);

            Assert.Equal(expected, actual);

        }

        #endregion

        #region Conjugate

        [Fact]
        public void Conjugate_Static()
        {
            var quat = new Quaternion(1, 2, 3, 4);

            var actual = Quaternion.Conjugate(quat);

            Assert.Equal(new Quaternion(-1, -2, -3, 4), actual);
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

        #endregion

        #region Normalize

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Static(Quaternion quat, Quaternion expected)
        {
            var actual = Quaternion.Normalize(quat);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region AxisAngle

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void FromAxisAngle_MainAxes(Quaternion expected, float4 axisAngle)
        {
            var axis = new float3(axisAngle.xyz);
            var angle = axisAngle.w;

            var actual = Quaternion.FromAxisAngle(axis, angle);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void ToAxisAngle_Static(Quaternion quat, float4 expected)
        {
            var actual = Quaternion.ToAxisAngle(quat);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void ToRotMat_Static(Quaternion quat, float4 expected)
        {
            var actual = Quaternion.ToRotMat(quat);

            var expectedAxis = new float3(expected.xyz);
            var expectedAngle = expected.w;

            var expectedRotMat = float4x4.CreateFromAxisAngle(expectedAxis, expectedAngle);

            Assert.Equal(expectedRotMat, actual);
        }

        #endregion

        #region Slerp

        [Theory]
        [MemberData(nameof(GetSlerp))]
        public void Slerp_TestSlerp(Quaternion q1, Quaternion q2, float blend, Quaternion expected)
        {
            var actual = Quaternion.Slerp(q1, q2, blend);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Conversion

        [Theory]
        [MemberData(nameof(GetEuler))]
        public void EulerToQuaternion_MainRotations(float3 euler, Quaternion expected)
        {
            var actual = Quaternion.EulerToQuaternion(euler, true);

            Assert.Equal(expected.x, actual.x, 4);
            Assert.Equal(expected.y, actual.y, 4);
            Assert.Equal(expected.z, actual.z, 4);
            Assert.Equal(expected.w, actual.w, 4);
        }

        [Theory]
        [MemberData(nameof(GetEuler))]
        public void QuaternionToEuler_MainRotations(float3 expected, Quaternion quat)
        {
            var actual = Quaternion.QuaternionToEuler(quat, true);

            Assert.Equal(expected.x, actual.x, 4);
            Assert.Equal(expected.y, actual.y, 4);
            Assert.Equal(expected.z, actual.z, 4);
        }

        [Fact]
        public void LookRotation_TestRotation()
        {
            var lookAt = new float3(1, 0, 0);
            var upDirection = new float3(0, 1, 0);

            var actual = Quaternion.LookRotation(lookAt, upDirection);

            Assert.Equal(new Quaternion(0, (float)System.Math.Sqrt(0.5f), 0, (float)System.Math.Sqrt(0.5f)), actual);
        }

        [Theory]
        [MemberData(nameof(GetMatrix))]
        public void QuaternionToMatrix_MainRotations(Quaternion quat, float4x4 expected)
        {
            var actual = Quaternion.QuaternionToMatrix(quat);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CopySign_TestNegative()
        {
            var a = 1;
            var b = -2;

            var actual = Quaternion.CopySign(a, b);

            Assert.Equal(-1, actual);
        }

        #endregion

        #region FromToRotation

        [Theory]
        [MemberData(nameof(GetFromToRotation))]
        public void FromToRotation_TestRotation(float3 from, float3 to, Quaternion expected)
        {
            var actual = Quaternion.FromToRotation(from, to);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Transform

        [Theory]
        [MemberData(nameof(GetTransform))]
        public void Transform_Float4(float4 vec, Quaternion quat, float4 expected)
        {
            var actual = Quaternion.Transform(vec, quat);

            Assert.Equal(expected, actual);
        }

        #endregion

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Operator(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Sub_Operator(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetQuaternionMultiplication))]
        public void Multiply_TwoQuaternions_Operator(Quaternion left, Quaternion right, Quaternion expected)
        {
            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScalarMultiplication))]
        public void Multiply_QuaternionScalar_Operator(Quaternion quat, float scale, Quaternion expected)
        {
            var actual = quat * scale;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScalarMultiplication))]
        public void Multiply_ScalarQuaternion_Operator(Quaternion quat, float scale, Quaternion expected)
        {
            var actual = scale * quat;

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

        #region Overrides

        [Fact]
        public void ToString_NoCulture()
        {
            Assert.NotNull(new Quaternion().ToString());
        }

        [Fact]
        public void ToString_InvariantCulture()
        {
            string s = "V: (0.5709415, 0.1675188, 0.16751876) w: 0.7860666";
            Quaternion q = Quaternion.EulerToQuaternion(float3.One);

            Assert.Equal(s, q.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "V: (0,5709415; 0,1675188; 0,16751876) w: 0,7860666";
            Quaternion q = Quaternion.EulerToQuaternion(float3.One);

            Assert.Equal(s, q.ToString(new CultureInfo("de-DE")));
        }

        //TODO: Equals(obj)
        //TODO: GetHashCode

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetAxisAngle()
        {
            yield return new object[]
            {
                new Quaternion((float) System.Math.Sqrt(0.5f), 0, 0, (float) System.Math.Sqrt(0.5f)),
                new float4(1, 0, 0, M.DegreesToRadians(90))
            };
            yield return new object[]
            {
                new Quaternion(0, (float) System.Math.Sqrt(0.5f), 0, (float) System.Math.Sqrt(0.5f)),
                new float4(0, 1, 0, M.DegreesToRadians(90))
            };
            yield return new object[]
            {
                new Quaternion(0, 0, (float) System.Math.Sqrt(0.5f), (float) System.Math.Sqrt(0.5f)),
                new float4(0, 0, 1, M.DegreesToRadians(90))
            };
        }

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[]
            {
                new Quaternion(1, 0, 0, 1),
                new Quaternion((float) System.Math.Sqrt(0.5f), 0, 0, (float) System.Math.Sqrt(0.5f)),
            };
            yield return new object[]
            {
                new Quaternion(0, 1, 0, 1),
                new Quaternion(0, (float) System.Math.Sqrt(0.5f), 0, (float) System.Math.Sqrt(0.5f)),
            };
            yield return new object[]
            {
                new Quaternion(0, 0, 1, 1),
                new Quaternion(0, 0, (float) System.Math.Sqrt(0.5f), (float) System.Math.Sqrt(0.5f)),
            };
        }

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new Quaternion(0, 0, 0, 0);
            var one = new Quaternion(1, 1, 1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { zero, zero, zero };
            yield return new object[] { one, one, new Quaternion(2, 2, 2, 2) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new Quaternion(0, 0, 0, 0);
            var one = new Quaternion(1, 1, 1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, zero, zero };
            yield return new object[] { one, one, zero };
        }

        public static IEnumerable<object[]> GetQuaternionMultiplication()
        {
            var zero = new Quaternion(0, 0, 0, 0);
            var one = new Quaternion(1, 1, 1, 1);
            var id = new Quaternion(0, 0, 0, 1);

            yield return new object[] { one, id, one };
            yield return new object[] { id, one, one };
            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
            yield return new object[] { one, one, new Quaternion(2, 2, 2, -2) };
        }

        public static IEnumerable<object[]> GetScalarMultiplication()
        {
            var one = new Quaternion(1, 1, 1, 1);

            yield return new object[] { one, 1, one };
            yield return new object[] { one, 2, new Quaternion(2, 2, 2, 2) };
            yield return new object[] { one, 0, new Quaternion(0, 0, 0, 0) };
        }

        public static IEnumerable<object[]> GetSlerp()
        {
            var zero = new Quaternion(0, 0, 0, 0);
            var id = new Quaternion(0, 0, 0, 1);
            var x = new Quaternion(0.5f, 0, 0, 0.5f);
            var y = new Quaternion(0, 0.5f, 0, 0.5f);

            yield return new object[] { zero, zero, 0.5f, id };
            yield return new object[] { x, zero, 0.5f, x };
            yield return new object[] { zero, x, 0.5f, x };
            yield return new object[] { x, y, 0.5f, new Quaternion(0.4082483f, 0.4082483f, 0, 0.8164967f) };
        }

        public static IEnumerable<object[]> GetEuler()
        {
            yield return new object[]
            {
                new float3(90, 0, 0),
                new Quaternion((float) System.Math.Sqrt(0.5f), 0, 0, (float) System.Math.Sqrt(0.5f))
            };
            yield return new object[]
            {
                new float3(0, 90, 0),
                new Quaternion(0, (float) System.Math.Sqrt(0.5f), 0, (float) System.Math.Sqrt(0.5f))
            };
            yield return new object[]
            {
                new float3(0, 0, 90),
                new Quaternion(0, 0, (float) System.Math.Sqrt(0.5f), (float) System.Math.Sqrt(0.5f))
            };
        }

        public static IEnumerable<object[]> GetMatrix()
        {
            yield return new object[]
                {new Quaternion(1, 0, 0, 1), new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1)};
            yield return new object[]
                {new Quaternion(0, 1, 0, 1), new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1)};
            yield return new object[]
                {new Quaternion(0, 0, 1, 1), new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)};
        }

        public static IEnumerable<object[]> GetFromToRotation()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            yield return new object[]
                {y, z, new Quaternion((float) System.Math.Sqrt(0.5f), 0, 0, (float) System.Math.Sqrt(0.5f))};
            yield return new object[]
                {z, x, new Quaternion(0, (float) System.Math.Sqrt(0.5f), 0, (float) System.Math.Sqrt(0.5f))};
            yield return new object[]
                {x, y, new Quaternion(0, 0, (float) System.Math.Sqrt(0.5f), (float) System.Math.Sqrt(0.5f))};
        }

        public static IEnumerable<object[]> GetTransform()
        {
            var x = new float4(1, 0, 0, 0);
            var y = new float4(0, 1, 0, 0);
            var z = new float4(0, 0, 1, 0);

            yield return new object[]
                {y, new Quaternion((float) System.Math.Sqrt(0.5f), 0, 0, (float) System.Math.Sqrt(0.5f)), z};
            yield return new object[]
                {z, new Quaternion(0, (float) System.Math.Sqrt(0.5f), 0, (float) System.Math.Sqrt(0.5f)), x};
            yield return new object[]
                {x, new Quaternion(0, 0, (float) System.Math.Sqrt(0.5f), (float) System.Math.Sqrt(0.5f)), y};
        }

        #endregion

    }
}