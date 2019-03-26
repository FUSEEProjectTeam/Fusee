using Xunit;

namespace Fusee.Math.Core
{
    public class MTest
    {
        [Fact]
        public void BinominalCoefficient_IsOne()
        {
            var k = 0;
            var n = 1;

            Assert.Equal(1, M.BinomialCoefficient(n, k));
        }

        [Theory]
        [InlineData(2, 0, 1)]
        [InlineData(0, 1, 2)]
        public void Clamp_IsMinIsMax_float(float x, float min, float max)
        {
            Assert.Equal(1, M.Clamp(x, min, max));
        }

        [Theory]
        [InlineData(2, 0, 1)]
        [InlineData(0, 1, 2)]
        public void Clamp_IsMinIsMax_double(double x, double min, double max)
        {
            Assert.Equal(1, M.Clamp(x, min, max));
        }
    }
}
