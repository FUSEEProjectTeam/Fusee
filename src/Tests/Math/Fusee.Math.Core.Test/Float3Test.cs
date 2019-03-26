using Xunit;
using System.Collections.Generic;

namespace Fusee.Math.Core
{
    public class Float3Test
    {
        #region UtilityMethods
        public static IEnumerable<object[]> GetFloat3()
        {
            var zero = new float3(0, 0, 0);
            var one = new float3(1, 1, 1);

            yield return new object[] {zero, one};
            yield return new object[] {one, zero};
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
