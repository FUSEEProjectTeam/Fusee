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
        public void Constructor_TestConstructors()
        {
            var actual = new Quaternion(new float3(1, 2, 3), 4);
            var expected = new Quaternion(1, 2, 3, 4);

            Assert.Equal(expected, actual);
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



        #endregion

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

            yield return new object[] {one, zero, one};
            yield return new object[] {zero, one, one};
            yield return new object[] {zero, zero, zero};
            yield return new object[] {one, one, new Quaternion(2, 2, 2, 2)};
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new Quaternion(0, 0, 0, 0);
            var one = new Quaternion(1, 1, 1, 1);

            yield return new object[] {one, zero, one};
            yield return new object[] {zero, zero, zero};
            yield return new object[] {one, one, zero};
        }

        public static IEnumerable<object[]> GetQuaternionMultiplication()
        {
            var zero = new Quaternion(0, 0, 0, 0);
            var one = new Quaternion(1, 1, 1, 1);
            var id = new Quaternion(0, 0, 0, 1);

            yield return new object[] {one, id, one};
            yield return new object[] {id, one, one};
            yield return new object[] {one, zero, zero};
            yield return new object[] {zero, one, zero};
            yield return new object[] {one, one, new Quaternion(2, 2, 2, -2)};
        }

        public static IEnumerable<object[]> GetScalarMultiplication()
        {
            var one = new Quaternion(1, 1, 1, 1);

            yield return new object[] {one, 1, one};
            yield return new object[] {one, 2, new Quaternion(2, 2, 2, 2)};
            yield return new object[] {one, 0, new Quaternion(0, 0, 0, 0)};
        }

        #endregion

    }
}