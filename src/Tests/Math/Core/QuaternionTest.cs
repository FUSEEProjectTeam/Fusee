using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class QuaternionTest
    {
        #region Fields

        [Fact]
        public void Identity_IsIdentity()
        {
            var expected = new QuaternionF(0, 0, 0, 1);

            Assert.Equal(expected, QuaternionF.Identity);
        }

        #endregion

        #region Constructors

        [Fact]
        public void Constructor_TestConstructors()
        {
            var actual = new QuaternionF(new float3(1, 2, 3), 4);
            var expected = new QuaternionF(1, 2, 3, 4);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region this

        [Fact]
        public void This_GetWithIdx_IsValid()
        {
            var actual = new QuaternionF(3, 7, 8, 1);

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);

            Assert.Equal(8, actual[2]);

            Assert.Equal(1, actual[3]);
        }

        [Fact]
        public void This_SetWithIdx_IsValid()
        {
            var actual = new QuaternionF(0, 0, 0, 0);
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
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => new QuaternionF(0, 0, 0, 0)[idx]);

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_SetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var q4 = new QuaternionF(0, 0, 0, 0); q4[idx] = 10; });

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
            var actual = new QuaternionF(1, 2, 3, 4);

            Assert.Equal(new float3(1, 2, 3), actual.xyz);
            Assert.Equal(1, actual.x);
            Assert.Equal(2, actual.y);
            Assert.Equal(3, actual.z);
            Assert.Equal(4, actual.w);
        }

        [Fact]
        public void Setter_Test()
        {
            var actual = new QuaternionF
            {
                x = 1,
                y = 2,
                z = 3,
                w = 4
            };
            Assert.Equal(new QuaternionF(1, 2, 3, 4), actual);

            actual.xyz = new float3(3, 2, 1);
            Assert.Equal(new QuaternionF(3, 2, 1, 4), actual);
        }

        #endregion

        #region Instance

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void ToAxisAngle_Instance(QuaternionF quat, float4 expected)
        {
            var actual = quat.ToAxisAngle();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Length_Is2()
        {
            var quat = new QuaternionF(1, 1, 1, 1);

            var actual = quat.Length;

            Assert.Equal(2, actual);
        }

        [Fact]
        public void LengthSquared_Is4()
        {
            var quat = new QuaternionF(1, 1, 1, 1);

            var actual = quat.LengthSquared;

            Assert.Equal(4, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(QuaternionF quat, QuaternionF expected)
        {
            var actual = quat.Normalize();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Conjugate_Instance()
        {
            var quat = new QuaternionF(1, 2, 3, 4);

            var actual = quat.Conjugate();

            Assert.Equal(new QuaternionF(-1, -2, -3, 4), actual);
        }

        [Fact]
        public void Invert_Instance()
        {
            var quat = new QuaternionF(1, 1, 1, 1);

            var actual = quat.Invert();

            Assert.Equal(new QuaternionF(-0.25f, -0.25f, -0.25f, 0.25f), actual);
        }

        #endregion

        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Static(QuaternionF left, QuaternionF right, QuaternionF expected)
        {
            var actual = QuaternionF.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Sub_Static(QuaternionF left, QuaternionF right, QuaternionF expected)
        {
            var actual = QuaternionF.Sub(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Mult

        [Theory]
        [MemberData(nameof(GetQuaternionMultiplication))]
        public void Multiply_TwoQuaternions_Static(QuaternionF left, QuaternionF right, QuaternionF expected)
        {
            var actual = QuaternionF.Multiply(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScalarMultiplication))]
        public void Multiply_QuaternionScalar_Staic(QuaternionF quat, float scale, QuaternionF expected)
        {
            var actual = QuaternionF.Multiply(quat, scale);

            Assert.Equal(expected, actual);

        }

        #endregion

        #region Conjugate

        [Fact]
        public void Conjugate_Static()
        {
            var quat = new QuaternionF(1, 2, 3, 4);

            var actual = QuaternionF.Conjugate(quat);

            Assert.Equal(new QuaternionF(-1, -2, -3, 4), actual);
        }

        #endregion

        #region Invert

        [Fact]
        public void Invert_Static()
        {
            var quat = new QuaternionF(1, 1, 1, 1);

            var actual = QuaternionF.Invert(quat);

            Assert.Equal(new QuaternionF(-0.25f, -0.25f, -0.25f, 0.25f), actual);
        }

        #endregion

        #region Normalize

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Static(QuaternionF quat, QuaternionF expected)
        {
            var actual = QuaternionF.Normalize(quat);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region AxisAngle

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void FromAxisAngle_MainAxes(QuaternionF expected, float4 axisAngle)
        {
            var axis = new float3(axisAngle.xyz);
            var angle = axisAngle.w;

            var actual = QuaternionF.FromAxisAngle(axis, angle);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void ToAxisAngle_Static(QuaternionF quat, float4 expected)
        {
            var actual = QuaternionF.ToAxisAngle(quat);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void ToRotMat_Static(QuaternionF quat, float4 expected)
        {
            var actual = QuaternionF.ToRotationMatrixFast(quat);

            var expectedAxis = new float3(expected.xyz);
            var expectedAngle = expected.w;

            var expectedRotMat = float4x4.CreateFromAxisAngle(expectedAxis, expectedAngle);

            Assert.Equal(expectedRotMat, actual);
        }

        #endregion

        #region Slerp

        [Theory]
        [MemberData(nameof(GetSlerp))]
        public void Slerp_TestSlerp(QuaternionF q1, QuaternionF q2, float blend, QuaternionF expected)
        {
            var actual = QuaternionF.Slerp(q1, q2, blend);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Conversion

        [Theory]
        [MemberData(nameof(GetEuler))]
        public void EulerToQuaternion_MainRotations(float3 euler, QuaternionF expected)
        {
            var actual = QuaternionF.FromEuler(euler, true);

            Assert.Equal(expected.x, actual.x, 4);
            Assert.Equal(expected.y, actual.y, 4);
            Assert.Equal(expected.z, actual.z, 4);
            Assert.Equal(expected.w, actual.w, 4);
        }

        [Theory]
        [MemberData(nameof(GetEuler))]
        public void QuaternionToEuler_MainRotations(float3 expected, QuaternionF quat)
        {
            var actual = QuaternionF.ToEuler(quat, true);

            Assert.Equal(expected.x, actual.x, 4);
            Assert.Equal(expected.y, actual.y, 4);
            Assert.Equal(expected.z, actual.z, 4);
        }

        [Fact]
        public void LookRotation_TestRotation()
        {
            var lookAt = new float3(1, 0, 0);
            var upDirection = new float3(0, 1, 0);

            var actual = QuaternionF.LookRotation(lookAt, upDirection);

            Assert.Equal(new QuaternionF(0, (float)System.Math.Sqrt(0.5f), 0, (float)System.Math.Sqrt(0.5f)), actual);
        }

        [Theory]
        [MemberData(nameof(GetMatrix))]
        public void QuaternionToMatrix_MainRotations(QuaternionF quat, float4x4 expected)
        {
            var actual = QuaternionF.ToRotationMatrix(quat);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CopySign_TestNegative()
        {
            var a = 1;
            var b = -2;

            var actual = QuaternionF.CopySign(a, b);

            Assert.Equal(-1, actual);
        }

        #endregion

        #region FromToRotation

        [Theory]
        [MemberData(nameof(GetFromToRotation))]
        public void FromToRotation_TestRotation(float3 from, float3 to, QuaternionF expected)
        {
            var actual = QuaternionF.FromToRotation(from, to);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Transform

        [Theory]
        [MemberData(nameof(GetTransform))]
        public void Transform_Float4(float4 vec, QuaternionF quat, float4 expected)
        {
            var actual = QuaternionF.Transform(vec, quat);

            Assert.Equal(expected, actual);
        }

        #endregion

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Operator(QuaternionF left, QuaternionF right, QuaternionF expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Sub_Operator(QuaternionF left, QuaternionF right, QuaternionF expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetQuaternionMultiplication))]
        public void Multiply_TwoQuaternions_Operator(QuaternionF left, QuaternionF right, QuaternionF expected)
        {
            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScalarMultiplication))]
        public void Multiply_QuaternionScalar_Operator(QuaternionF quat, float scale, QuaternionF expected)
        {
            var actual = quat * scale;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScalarMultiplication))]
        public void Multiply_ScalarQuaternion_Operator(QuaternionF quat, float scale, QuaternionF expected)
        {
            var actual = scale * quat;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new QuaternionF(1, 1, 1, 1);
            var b = new QuaternionF(1, 1, 1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new QuaternionF(1, 1, 1, 1);
            var b = new QuaternionF(0, 0, 0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new QuaternionF(1, 1, 1, 1);
            var b = new QuaternionF(1, 1, 1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new QuaternionF(1, 1, 1, 1);
            var b = new QuaternionF(0, 0, 0, 0);

            Assert.True(a != b);
        }

        #endregion

        #region Overrides

        [Fact]
        public void ToString_NoCulture()
        {
            Assert.NotNull(new QuaternionF().ToString());
        }

        [Fact]
        public void ToString_InvariantCulture()
        {
            string s = "V: (0.5709415, 0.1675188, 0.1675188) w: 0.7860666";
            QuaternionF q = QuaternionF.FromEuler(float3.One);

            Assert.Equal(s, q.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "V: (0,5709415; 0,1675188; 0,1675188) w: 0,7860666";
            QuaternionF q = QuaternionF.FromEuler(float3.One);

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
                new QuaternionF((float) System.Math.Sqrt(0.5f), 0, 0, (float) System.Math.Sqrt(0.5f)),
                new float4(1, 0, 0, M.DegreesToRadians(90))
            };
            yield return new object[]
            {
                new QuaternionF(0, (float) System.Math.Sqrt(0.5f), 0, (float) System.Math.Sqrt(0.5f)),
                new float4(0, 1, 0, M.DegreesToRadians(90))
            };
            yield return new object[]
            {
                new QuaternionF(0, 0, (float) System.Math.Sqrt(0.5f), (float) System.Math.Sqrt(0.5f)),
                new float4(0, 0, 1, M.DegreesToRadians(90))
            };
        }

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[]
            {
                new QuaternionF(1, 0, 0, 1),
                new QuaternionF((float) System.Math.Sqrt(0.5f), 0, 0, (float) System.Math.Sqrt(0.5f)),
            };
            yield return new object[]
            {
                new QuaternionF(0, 1, 0, 1),
                new QuaternionF(0, (float) System.Math.Sqrt(0.5f), 0, (float) System.Math.Sqrt(0.5f)),
            };
            yield return new object[]
            {
                new QuaternionF(0, 0, 1, 1),
                new QuaternionF(0, 0, (float) System.Math.Sqrt(0.5f), (float) System.Math.Sqrt(0.5f)),
            };
        }

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new QuaternionF(0, 0, 0, 0);
            var one = new QuaternionF(1, 1, 1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { zero, zero, zero };
            yield return new object[] { one, one, new QuaternionF(2, 2, 2, 2) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new QuaternionF(0, 0, 0, 0);
            var one = new QuaternionF(1, 1, 1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, zero, zero };
            yield return new object[] { one, one, zero };
        }

        public static IEnumerable<object[]> GetQuaternionMultiplication()
        {
            var zero = new QuaternionF(0, 0, 0, 0);
            var one = new QuaternionF(1, 1, 1, 1);
            var id = new QuaternionF(0, 0, 0, 1);

            yield return new object[] { one, id, one };
            yield return new object[] { id, one, one };
            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
            yield return new object[] { one, one, new QuaternionF(2, 2, 2, -2) };
        }

        public static IEnumerable<object[]> GetScalarMultiplication()
        {
            var one = new QuaternionF(1, 1, 1, 1);

            yield return new object[] { one, 1, one };
            yield return new object[] { one, 2, new QuaternionF(2, 2, 2, 2) };
            yield return new object[] { one, 0, new QuaternionF(0, 0, 0, 0) };
        }

        public static IEnumerable<object[]> GetSlerp()
        {
            var zero = new QuaternionF(0, 0, 0, 0);
            var id = new QuaternionF(0, 0, 0, 1);
            var x = new QuaternionF(0.5f, 0, 0, 0.5f);
            var y = new QuaternionF(0, 0.5f, 0, 0.5f);

            yield return new object[] { zero, zero, 0.5f, id };
            yield return new object[] { x, zero, 0.5f, x };
            yield return new object[] { zero, x, 0.5f, x };
            yield return new object[] { x, y, 0.5f, new QuaternionF(0.4082483f, 0.4082483f, 0, 0.8164967f) };
        }

        public static IEnumerable<object[]> GetEuler()
        {
            yield return new object[]
            {
                new float3(90, 0, 0),
                new QuaternionF((float) System.Math.Sqrt(0.5f), 0, 0, (float) System.Math.Sqrt(0.5f))
            };
            yield return new object[]
            {
                new float3(0, 90, 0),
                new QuaternionF(0, (float) System.Math.Sqrt(0.5f), 0, (float) System.Math.Sqrt(0.5f))
            };
            yield return new object[]
            {
                new float3(0, 0, 90),
                new QuaternionF(0, 0, (float) System.Math.Sqrt(0.5f), (float) System.Math.Sqrt(0.5f))
            };
        }

        public static IEnumerable<object[]> GetMatrix()
        {
            yield return new object[]
                {new QuaternionF(1, 0, 0, 1), new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1)};
            yield return new object[]
                {new QuaternionF(0, 1, 0, 1), new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1)};
            yield return new object[]
                {new QuaternionF(0, 0, 1, 1), new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)};
        }

        public static IEnumerable<object[]> GetFromToRotation()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            yield return new object[]
                {y, z, new QuaternionF((float) System.Math.Sqrt(0.5f), 0, 0, (float) System.Math.Sqrt(0.5f))};
            yield return new object[]
                {z, x, new QuaternionF(0, (float) System.Math.Sqrt(0.5f), 0, (float) System.Math.Sqrt(0.5f))};
            yield return new object[]
                {x, y, new QuaternionF(0, 0, (float) System.Math.Sqrt(0.5f), (float) System.Math.Sqrt(0.5f))};
        }

        public static IEnumerable<object[]> GetTransform()
        {
            var x = new float4(1, 0, 0, 0);
            var y = new float4(0, 1, 0, 0);
            var z = new float4(0, 0, 1, 0);

            yield return new object[]
                {y, new QuaternionF((float) System.Math.Sqrt(0.5f), 0, 0, (float) System.Math.Sqrt(0.5f)), z};
            yield return new object[]
                {z, new QuaternionF(0, (float) System.Math.Sqrt(0.5f), 0, (float) System.Math.Sqrt(0.5f)), x};
            yield return new object[]
                {x, new QuaternionF(0, 0, (float) System.Math.Sqrt(0.5f), (float) System.Math.Sqrt(0.5f)), y};
        }

        #endregion

    }
}