using Xunit;
using System.Collections.Generic;

namespace Fusee.Math.Core
{
    public class Float3Test
    {
        #region Utility
        public static IEnumerable<object[]> GetFloat3()
        {
            var zero = new float3(0, 0, 0);
            var one = new float3(1, 1, 1);

            yield return new object[] {zero, one};
            yield return new object[] {one, zero};
        }

        public static IEnumerable<object[]> GetUVExpected()
        {
            yield return new object[] { 0, 0, new float3(1, 0, 0) };
            yield return new object[] { 1, 0, new float3(0, 1, 0) };
            yield return new object[] { 0, 1, new float3(0, 0, 1) };
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
        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void Add_TwoVectors_ReturnsVector(float3 x, float3 y)
        {
            var actual = float3.Add(x,y);

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Theory]
        [MemberData(nameof(GetFloat3))]
        public void Add_TwoVectors_SaveToThird(float3 x, float3 y)
        {
            float3 actual;

            float3.Add(ref x, ref y, out actual);

            Assert.Equal(new float3(1, 1, 1), actual);
        }

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

        [Fact]
        public void Equal
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
    }
}
