using Fusee.Math.Core;
using Xunit;

namespace Fusee.Test.Math.Core
{
    public class OBBTest
    {
        [Fact]
        public void ConstructorSingle_MinMax()
        {
            var vertices = new float3[2] { new float3(0, 0, 0), new float3(1, 1, 1) };
            var actual = new OBBf(vertices);

            Assert.Equal(new float3(0, 0, 0), actual.Min);
            Assert.Equal(new float3(1, 1, 1), actual.Max);
        }

        [Fact]
        public void ConstructorDouble_MinMax()
        {
            var vertices = new double3[2] { new double3(0, 0, 0), new double3(1, 1, 1) };
            var actual = new OBBd(vertices);

            Assert.Equal(new double3(0, 0, 0), actual.Min);
            Assert.Equal(new double3(1, 1, 1), actual.Max);
        }

        [Fact]
        public void CenterSingle_Is1()
        {
            var vertices = new float3[2] { new float3(0, 0, 0), new float3(2, 2, 2) };
            var obbf = new OBBf(vertices);

            var actual = obbf.Center;

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Fact]
        public void SizeSingle_Is1()
        {
         
            var vertices = new float3[2] { new float3(0, 0, 0), new float3(1, 1, 1) };
            var obbf = new OBBf(vertices);

            var actual = obbf.Size;

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Fact]
        public void CenterDouble_Is1()
        {
            var vertices = new double3[2] { new double3(0, 0, 0), new double3(2, 2, 2) };
            var obbd = new OBBd(vertices);

            var actual = obbd.Center;

            Assert.Equal(new double3(1, 1, 1), actual);
        }

        [Fact]
        public void SizeDouble_Is1()
        {
            var vertices = new double3[2] { new double3(0, 0, 0), new double3(1, 1, 1) };
            var obbd = new OBBd(vertices);

            var actual = obbd.Size;

            Assert.Equal(new double3(1, 1, 1), actual);
        }
    }
}
