using Fusee.Math.Core;
using Xunit;

namespace Fusee.Test.Math.Core
{
    public class MinMaxRectTest
    {
        [Fact]
        public void Min_Is0()
        {
            var rect = MinMaxRect.FromCenterSize(new float2(1,1), new float2(2,2));

            var actual = rect.Min;

            Assert.Equal(new float2(0, 0), actual);
        }

        [Fact]
        public void Max_Is2()
        {
            var rect = MinMaxRect.FromCenterSize(new float2(1, 1), new float2(2, 2));

            var actual = rect.Max;

            Assert.Equal(new float2(2, 2), actual);
        }

        [Fact]
        public void ToString_IsString()
        {
            var rect = MinMaxRect.FromCenterSize(new float2(1, 1), new float2(2, 2));

            var actual = rect.ToString();

            Assert.Equal("Min: (0, 0) Max: (2, 2)", actual);
        }
    }
}
