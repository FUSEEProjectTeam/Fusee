using Fusee.Math.Core;
using System.Globalization;
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
        public void ToString_NoCulture()
        {
            var rect = new MinMaxRect() { Min = new float2(1, 1), Max = new float2(2.2f, 3.3f) };

            Assert.NotNull(rect.ToString());
        }
        [Fact]
        public void ToString_InvariantCulture()
        {
            var rect = new MinMaxRect() { Min = new float2(1, 1), Max = new float2(2.2f, 3.3f) };

            var actual = rect.ToString(CultureInfo.InvariantCulture);

            Assert.Equal("Min: (1, 1) Max: (2.2, 3.3)", actual);
        }
        [Fact]
        public void ToString_CultureDE()
        {
            var rect = new MinMaxRect() { Min = new float2(1, 1), Max = new float2(2.2f, 3.3f) };

            var actual = rect.ToString(new CultureInfo("de-DE"));

            Assert.Equal("Min: (1; 1) Max: (2,2; 3,3)", actual);
        }
    }
}
