using Xunit;
using System;
using System.Collections.Generic;

namespace Fusee.Math.Core
{
    public class Float2Test
    {
        #region Fields

        [Fact]
        public void UnitX_IsXVector()
        {
            var expected = new float2(1, 0);

            Assert.Equal(expected, float2.UnitX);
        }

        [Fact]
        public void UnitY_IsYVector()
        {
            var expected = new float2(0, 1);

            Assert.Equal(expected, float2.UnitY);
        }

        [Fact]
        public void One_IsOne()
        {
            var expected = new float2(1, 1);

            Assert.Equal(expected, float2.One);
        }

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new float2(0, 0);

            Assert.Equal(expected, float2.Zero);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Addition_IsOne(float2 a, float2 b, float2 expected)
        {
            var actual = a + b;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivision))]
        public void Division_IsOne(float2 vec, float scalar, float2 expected)
        {
            var actual = vec / scalar;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new float2(1, 1);
            var b = new float2(1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new float2(1, 1);
            var b = new float2(0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new float2(1, 1);
            var b = new float2(1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new float2(1, 1);
            var b = new float2(0, 0);

            Assert.True(a != b);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors(float x, float2 vec1, float2 expected)
        {
            var vec2 = new float2(x, x);

            var actual = vec1 * vec2;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_ScalarVector(float x, float2 vec, float2 expected)
        {
            var actual = x * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar(float x, float2 vec, float2 expected)
        {
            var actual = vec * x;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtraction_IsZero(float2 a, float2 b, float2 expected)
        {
            var actual = a - b;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UnaryNegation_IsNegative()
        {
            var vec = new float2(1, 1);

            var actual = -vec;

            Assert.Equal(new float2(-1, -1), actual);
        }

        #endregion

        #region Properties

        [Fact]
        public void Length_IsOne()
        {
            var vec = new float2(1, 0);

            var actual = vec.Length;

            Assert.Equal(1, actual);
        }

        [Fact]
        public void LengthSquared_IsTwo()
        {
            var vec = new float2(1, 1);

            var actual = vec.LengthSquared;

            Assert.Equal(2, actual);
        }

        [Fact]
        public void PerpendicularLeft_XToY()
        {
            var vec = new float2(1, 0);

            var actual = vec.PerpendicularLeft;

            Assert.Equal(new float2(0, 1), actual);
        }

        [Fact]
        public void PerpendicularRight_XToY()
        {
            var vec = new float2(1, 0);

            var actual = vec.PerpendicularRight;

            Assert.Equal(new float2(0, -1), actual);
        }

        #endregion

        #region Methods

        #region Normalize

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(float2 vec, float2 expected)
        {
            var actual = vec.Normalize();

            Assert.Equal(expected, actual);
        }

        #endregion

        #endregion

        #region IEnumberables

        public static IEnumerable<object[]> GetAddition()
        {
            float2 one = new float2(1, 1);
            float2 zero = new float2(0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
        }


        public static IEnumerable<object[]> GetDivision()
        {
            yield return new object[] { new float2(2, 2), 2, new float2(1, 1) };
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new float2(1, 1);

            yield return new object[] { 1, one, one };
            yield return new object[] { 2, one, new float2(2, 2) };
            yield return new object[] { 0, one, new float2(0, 0) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            yield return new object[] { new float2(1, 1), new float2(1, 1), new float2(0, 0) };
        }
        #endregion

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] {new float2(2, 0), new float2(1, 0)};
            yield return new object[] {new float2(0, 2), new float2(0, 1)};
        }
    }
}
