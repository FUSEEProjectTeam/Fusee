using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fusee.Math.Core
{
    public class OBBTest
    {
        [Fact]
        public void ConstructorSingle_MinMax()
        {
            var actual = new OBBf(new float3(0, 0, 0), new float3(1, 1, 1), float4x4.Identity, float3.Zero, float3.One);

            Assert.Equal(new float3(0, 0, 0), actual.Min);
            Assert.Equal(new float3(1, 1, 1), actual.Max);
        }

        [Fact]
        public void ConstructorDouble_MinMax()
        {
            var actual = new OBBd(new double3(0, 0, 0), new double3(1, 1, 1), double4x4.Identity, double3.Zero, double3.One);

            Assert.Equal(new double3(0, 0, 0), actual.Min);
            Assert.Equal(new double3(1, 1, 1), actual.Max);
        }

        [Fact]
        public void CenterSingle_Is1()
        {
            var obbf = new OBBf(new float3(0, 0, 0), new float3(2, 2, 2), float4x4.Identity, float3.Zero, float3.One);

            var actual = obbf.Center;

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Fact]
        public void SizeSingle_Is1()
        {
            var obbf = new OBBf(new float3(0, 0, 0), new float3(1, 1, 1), float4x4.Identity, float3.Zero, float3.One);

            var actual = obbf.Size;

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Fact]
        public void CenterDouble_Is1()
        {
            var obbd = new OBBd(new double3(0, 0, 0), new double3(2, 2, 2), double4x4.Identity, double3.Zero, double3.One);

            var actual = obbd.Center;

            Assert.Equal(new double3(1, 1, 1), actual);
        }

        [Fact]
        public void SizeDouble_Is1()
        {
            var obbd = new OBBd(new double3(0, 0, 0), new double3(1, 1, 1), double4x4.Identity, double3.Zero, double3.One);

            var actual = obbd.Size;

            Assert.Equal(new double3(1, 1, 1), actual);
        }
    }
}
