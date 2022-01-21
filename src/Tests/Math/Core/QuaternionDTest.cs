using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class QuaternionDTest
    {
        #region Fields

        [Fact]
        public void Identity_IsIdentity()
        {
            var expected = new QuaternionD(0, 0, 0, 1);

            Assert.Equal(expected, QuaternionD.Identity);
        }

        #endregion

        #region Constructors

        [Fact]
        public void Constructor_TestConstructors()
        {
            var actual = new QuaternionD(new double3(1, 2, 3), 4);
            var expected = new QuaternionD(1, 2, 3, 4);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region this

        [Fact]
        public void This_GetWithIdx_IsValid()
        {
            var actual = new QuaternionD(3, 7, 8, 1);

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);

            Assert.Equal(8, actual[2]);

            Assert.Equal(1, actual[3]);
        }

        [Fact]
        public void This_SetWithIdx_IsValid()
        {
            var actual = new QuaternionD(0, 0, 0, 0);
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
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => new QuaternionD(0, 0, 0, 0)[idx]);

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_SetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var q4 = new QuaternionD(0, 0, 0, 0); q4[idx] = 10; });

            Assert.Equal(expected, actual.ParamName);
        }

        public static IEnumerable<object[]> ThisException()
        {
            yield return new object[] { 7, "Index 7 not eligible for a QuaternionD type" };
            yield return new object[] { 6, "Index 6 not eligible for a QuaternionD type" };
        }

        #endregion

        #region Properties

        [Fact]
        public void Getter_Test()
        {
            var actual = new QuaternionD(1, 2, 3, 4);

            Assert.Equal(new double3(1, 2, 3), actual.xyz);
            Assert.Equal(1, actual.x);
            Assert.Equal(2, actual.y);
            Assert.Equal(3, actual.z);
            Assert.Equal(4, actual.w);
        }

        [Fact]
        public void Setter_Test()
        {
            var actual = new QuaternionD
            {
                x = 1,
                y = 2,
                z = 3,
                w = 4
            };
            Assert.Equal(new QuaternionD(1, 2, 3, 4), actual);

            actual.xyz = new double3(3, 2, 1);
            Assert.Equal(new QuaternionD(3, 2, 1, 4), actual);
        }

        #endregion

        #region Instance

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void ToAxisAngle_Instance(QuaternionD quat, double4 expected)
        {
            var actual = quat.ToAxisAngle();

            Assert.Equal(expected.x, actual.x, 14);
            Assert.Equal(expected.y, actual.y, 14);
            Assert.Equal(expected.z, actual.z, 14);
            Assert.Equal(expected.w, actual.w, 14);
        }

        [Fact]
        public void Length_Is2()
        {
            var quat = new QuaternionD(1, 1, 1, 1);

            var actual = quat.Length;

            Assert.Equal(2, actual);
        }

        [Fact]
        public void LengthSquared_Is4()
        {
            var quat = new QuaternionD(1, 1, 1, 1);

            var actual = quat.LengthSquared;

            Assert.Equal(4, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(QuaternionD quat, QuaternionD expected)
        {
            var actual = quat.Normalize();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Conjugate_Instance()
        {
            var quat = new QuaternionD(1, 2, 3, 4);

            var actual = quat.Conjugate();

            Assert.Equal(new QuaternionD(-1, -2, -3, 4), actual);
        }

        [Fact]
        public void Invert_Instance()
        {
            var quat = new QuaternionD(1, 1, 1, 1);

            var actual = quat.Invert();

            Assert.Equal(new QuaternionD(-0.25, -0.25, -0.25, 0.25), actual);
        }

        #endregion

        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Static(QuaternionD left, QuaternionD right, QuaternionD expected)
        {
            var actual = QuaternionD.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Sub_Static(QuaternionD left, QuaternionD right, QuaternionD expected)
        {
            var actual = QuaternionD.Sub(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Mult

        [Theory]
        [MemberData(nameof(GetQuaternionDMultiplication))]
        public void Multiply_TwoQuaternionDs_Static(QuaternionD left, QuaternionD right, QuaternionD expected)
        {
            var actual = QuaternionD.Multiply(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScalarMultiplication))]
        public void Multiply_QuaternionDScalar_Staic(QuaternionD quat, double scale, QuaternionD expected)
        {
            var actual = QuaternionD.Multiply(quat, scale);

            Assert.Equal(expected, actual);

        }

        #endregion

        #region Conjugate

        [Fact]
        public void Conjugate_Static()
        {
            var quat = new QuaternionD(1, 2, 3, 4);

            var actual = QuaternionD.Conjugate(quat);

            Assert.Equal(new QuaternionD(-1, -2, -3, 4), actual);
        }

        #endregion

        #region Invert

        [Fact]
        public void Invert_Static()
        {
            var quat = new QuaternionD(1, 1, 1, 1);

            var actual = QuaternionD.Invert(quat);

            Assert.Equal(new QuaternionD(-0.25, -0.25, -0.25, 0.25), actual);
        }

        #endregion

        #region Normalize

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Static(QuaternionD quat, QuaternionD expected)
        {
            var actual = QuaternionD.Normalize(quat);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region AxisAngle

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void FromAxisAngle_MainAxes(QuaternionD expected, double4 axisAngle)
        {
            var axis = new double3(axisAngle.xyz);
            var angle = axisAngle.w;

            var actual = QuaternionD.FromAxisAngle(axis, angle);

            Assert.Equal(expected.x, actual.x, 14);
            Assert.Equal(expected.y, actual.y, 14);
            Assert.Equal(expected.z, actual.z, 14);
            Assert.Equal(expected.w, actual.w, 14);
        }

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void ToAxisAngle_Static(QuaternionD quat, double4 expected)
        {
            var actual = QuaternionD.ToAxisAngle(quat);

            Assert.Equal(expected.x, actual.x, 14);
            Assert.Equal(expected.y, actual.y, 14);
            Assert.Equal(expected.z, actual.z, 14);
            Assert.Equal(expected.w, actual.w, 14);
        }

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void ToRotMat_Static(QuaternionD quat, double4 expected)
        {
            var actual = QuaternionD.ToRotationMatrixFast(quat);

            var expectedAxis = new double3(expected.xyz);
            var expectedAngle = expected.w;

            var expectedRotMat = double4x4.CreateFromAxisAngle(expectedAxis, expectedAngle);

            var expAsArray = expectedRotMat.ToArray();
            var actualAsArray = actual.ToArray();
            for (int i = 0; i < expAsArray.Length; i++)
            {
                Assert.Equal(expAsArray[i], actualAsArray[i], 14);
            }
        }

        #endregion

        #region Slerp

        [Theory]
        [MemberData(nameof(GetSlerp))]
        public void Slerp_TestSlerp(QuaternionD q1, QuaternionD q2, double blend, QuaternionD expected)
        {
            var actual = QuaternionD.Slerp(q1, q2, blend);
            Assert.Equal(expected.x, actual.x, 14);
            Assert.Equal(expected.y, actual.y, 14);
            Assert.Equal(expected.z, actual.z, 14);
            Assert.Equal(expected.w, actual.w, 14);
        }

        #endregion

        #region Conversion

        [Theory]
        [MemberData(nameof(GetEuler))]
        public void EulerToQuaternionD_MainRotations(double3 euler, QuaternionD expected)
        {
            var actual = QuaternionD.FromEuler(euler, true);

            Assert.Equal(expected.x, actual.x, 14);
            Assert.Equal(expected.y, actual.y, 14);
            Assert.Equal(expected.z, actual.z, 14);
            Assert.Equal(expected.w, actual.w, 14);
        }

        [Theory]
        [MemberData(nameof(GetEuler))]
        public void QuaternionDToEuler_MainRotations(double3 expected, QuaternionD quat)
        {
            var actual = QuaternionD.ToEuler(quat, true);

            Assert.Equal(expected.x, actual.x, 5);
            Assert.Equal(expected.y, actual.y, 5);
            Assert.Equal(expected.z, actual.z, 5);
        }

        [Fact]
        public void LookRotation_TestRotation()
        {
            var lookAt = new double3(1, 0, 0);
            var upDirection = new double3(0, 1, 0);

            var actual = QuaternionD.LookRotation(lookAt, upDirection);

            Assert.Equal(new QuaternionD(0, System.Math.Sqrt(0.5), 0, System.Math.Sqrt(0.5)), actual);
        }

        [Theory]
        [MemberData(nameof(GetMatrix))]
        public void QuaternionDToMatrix_MainRotations(QuaternionD quat, double4x4 expected)
        {
            var actual = QuaternionD.ToRotationMatrix(quat);

            var expAsArray = expected.ToArray();
            var actualAsArray = actual.ToArray();
            for (int i = 0; i < expAsArray.Length; i++)
            {
                Assert.Equal(expAsArray[i], actualAsArray[i], 14);
            }
        }

        [Fact]
        public void CopySign_TestNegative()
        {
            var a = 1;
            var b = -2;

            var actual = QuaternionD.CopySign(a, b);

            Assert.Equal(-1, actual);
        }

        #endregion

        #region FromToRotation

        [Theory]
        [MemberData(nameof(GetFromToRotation))]
        public void FromToRotation_TestRotation(double3 from, double3 to, QuaternionD expected)
        {
            var actual = QuaternionD.FromToRotation(from, to);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Transform

        [Theory]
        [MemberData(nameof(GetTransform))]
        public void Transform_Float4(double4 vec, QuaternionD quat, double4 expected)
        {
            var actual = QuaternionD.Transform(vec, quat);

            Assert.Equal(expected, actual);
        }

        #endregion

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Operator(QuaternionD left, QuaternionD right, QuaternionD expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Sub_Operator(QuaternionD left, QuaternionD right, QuaternionD expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetQuaternionDMultiplication))]
        public void Multiply_TwoQuaternionDs_Operator(QuaternionD left, QuaternionD right, QuaternionD expected)
        {
            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScalarMultiplication))]
        public void Multiply_QuaternionDScalar_Operator(QuaternionD quat, double scale, QuaternionD expected)
        {
            var actual = quat * scale;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScalarMultiplication))]
        public void Multiply_ScalarQuaternionD_Operator(QuaternionD quat, double scale, QuaternionD expected)
        {
            var actual = scale * quat;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new QuaternionD(1, 1, 1, 1);
            var b = new QuaternionD(1, 1, 1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new QuaternionD(1, 1, 1, 1);
            var b = new QuaternionD(0, 0, 0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new QuaternionD(1, 1, 1, 1);
            var b = new QuaternionD(1, 1, 1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new QuaternionD(1, 1, 1, 1);
            var b = new QuaternionD(0, 0, 0, 0);

            Assert.True(a != b);
        }

        #endregion

        #region Overrides

        [Fact]
        public void ToString_NoCulture()
        {
            Assert.NotNull(new QuaternionD().ToString());
        }

        [Fact]
        public void ToString_InvariantCulture()
        {
            string s = "V: (0.5709414713577319, 0.16751879124639693, 0.16751879124639693) w: 0.7860666291368439";
            QuaternionD q = QuaternionD.FromEuler(double3.One);

            Assert.Equal(s, q.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "V: (0,5709414713577319; 0,16751879124639693; 0,16751879124639693) w: 0,7860666291368439";
            QuaternionD q = QuaternionD.FromEuler(double3.One);

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
                new QuaternionD(System.Math.Sqrt(0.5), 0, 0,  System.Math.Sqrt(0.5)),
                new double4(1, 0, 0, M.DegreesToRadiansD((double)90))
            };
            yield return new object[]
            {
                new QuaternionD(0, System.Math.Sqrt(0.5), 0,  System.Math.Sqrt(0.5)),
                new double4(0, 1, 0, M.DegreesToRadiansD((double)90))
            };
            yield return new object[]
            {
                new QuaternionD(0, 0, System.Math.Sqrt(0.5),  System.Math.Sqrt(0.5)),
                new double4(0, 0, 1, M.DegreesToRadiansD((double)90))
            };
        }

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[]
            {
                new QuaternionD(1, 0, 0, 1),
                new QuaternionD( System.Math.Sqrt(0.5), 0, 0,  System.Math.Sqrt(0.5)),
            };
            yield return new object[]
            {
                new QuaternionD(0, 1, 0, 1),
                new QuaternionD(0,  System.Math.Sqrt(0.5), 0,  System.Math.Sqrt(0.5)),
            };
            yield return new object[]
            {
                new QuaternionD(0, 0, 1, 1),
                new QuaternionD(0, 0,  System.Math.Sqrt(0.5),  System.Math.Sqrt(0.5)),
            };
        }

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new QuaternionD(0, 0, 0, 0);
            var one = new QuaternionD(1, 1, 1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
            yield return new object[] { zero, zero, zero };
            yield return new object[] { one, one, new QuaternionD(2, 2, 2, 2) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new QuaternionD(0, 0, 0, 0);
            var one = new QuaternionD(1, 1, 1, 1);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, zero, zero };
            yield return new object[] { one, one, zero };
        }

        public static IEnumerable<object[]> GetQuaternionDMultiplication()
        {
            var zero = new QuaternionD(0, 0, 0, 0);
            var one = new QuaternionD(1, 1, 1, 1);
            var id = new QuaternionD(0, 0, 0, 1);

            yield return new object[] { one, id, one };
            yield return new object[] { id, one, one };
            yield return new object[] { one, zero, zero };
            yield return new object[] { zero, one, zero };
            yield return new object[] { one, one, new QuaternionD(2, 2, 2, -2) };
        }

        public static IEnumerable<object[]> GetScalarMultiplication()
        {
            var one = new QuaternionD(1, 1, 1, 1);

            yield return new object[] { one, 1, one };
            yield return new object[] { one, 2, new QuaternionD(2, 2, 2, 2) };
            yield return new object[] { one, 0, new QuaternionD(0, 0, 0, 0) };
        }

        public static IEnumerable<object[]> GetSlerp()
        {
            var zero = new QuaternionD(0, 0, 0, 0);
            var id = new QuaternionD(0, 0, 0, 1);
            var x = new QuaternionD(0.5, 0, 0, 0.5);
            var y = new QuaternionD(0, 0.5, 0, 0.5);

            yield return new object[] { zero, zero, 0.5, id };
            yield return new object[] { x, zero, 0.5, x };
            yield return new object[] { zero, x, 0.5, x };
            yield return new object[] { x, y, 0.5, new QuaternionD(0.40824829046386, 0.40824829046386, 0, 0.81649658092773) };
        }

        public static IEnumerable<object[]> GetEuler()
        {
            yield return new object[]
            {
                new double3(90, 0, 0),
                new QuaternionD(System.Math.Sqrt(0.5), 0, 0,  System.Math.Sqrt(0.5))
            };
            yield return new object[]
            {
                new double3(0, 90, 0),
                new QuaternionD(0, System.Math.Sqrt(0.5), 0,  System.Math.Sqrt(0.5))
            };
            yield return new object[]
            {
                new double3(0, 0, 90),
                new QuaternionD(0, 0, System.Math.Sqrt(0.5f),  System.Math.Sqrt(0.5f))
            };
        }

        public static IEnumerable<object[]> GetMatrix()
        {
            yield return new object[]
                {new QuaternionD(1, 0, 0, 1), new double4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1)};
            yield return new object[]
                {new QuaternionD(0, 1, 0, 1), new double4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1)};
            yield return new object[]
                {new QuaternionD(0, 0, 1, 1), new double4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)};
        }

        public static IEnumerable<object[]> GetFromToRotation()
        {
            var x = new double3(1, 0, 0);
            var y = new double3(0, 1, 0);
            var z = new double3(0, 0, 1);

            yield return new object[]
                {y, z, new QuaternionD(System.Math.Sqrt(0.5), 0, 0, System.Math.Sqrt(0.5))};
            yield return new object[]
                {z, x, new QuaternionD(0, System.Math.Sqrt(0.5), 0, System.Math.Sqrt(0.5))};
            yield return new object[]
                {x, y, new QuaternionD(0, 0, System.Math.Sqrt(0.5), System.Math.Sqrt(0.5))};
        }

        public static IEnumerable<object[]> GetTransform()
        {
            var x = new double4(1, 0, 0, 0);
            var y = new double4(0, 1, 0, 0);
            var z = new double4(0, 0, 1, 0);

            yield return new object[]
                {y, new QuaternionD( System.Math.Sqrt(0.5), 0, 0, System.Math.Sqrt(0.5)), z};
            yield return new object[]
                {z, new QuaternionD(0, System.Math.Sqrt(0.5), 0, System.Math.Sqrt(0.5)), x};
            yield return new object[]
                {x, new QuaternionD(0, 0, System.Math.Sqrt(0.5), System.Math.Sqrt(0.5)), y};
        }

        #endregion

    }
}