using Fusee.Math.Core;
using System.Collections.Generic;
using Xunit;

namespace Fusee.Test.Math.Core
{
    public class OBBTest
    {
        public static IEnumerable<object[]> OBBfData()
        {
            yield return new object[]
            {
                new float3[]
                {
                    new float3(0, 0, 0),
                    new float3(0.25f, 0.25f, 0.25f),
                    new float3(0.5f, 0.5f, 0.5f),
                    new float3(0.75f, 0.75f, 0.75f),
                    new float3(1.25f, 1.25f, 1.25f),
                    new float3(1.5f, 1.5f, 1.5f),
                    new float3(1.75f, 1.75f, 1.75f),
                    new float3(2, 2, 2),
                }
            };
        }

        public static IEnumerable<object[]> OBBdData()
        {
            yield return new object[]
            {
                new double3[]
                {
                    new double3(0, 0, 0),
                    new double3(0.25, 0.25, 0.25),
                    new double3(0.5, 0.5, 0.5),
                    new double3(0.75, 0.75, 0.75),
                    new double3(1.25, 1.25, 1.25),
                    new double3(1.5, 1.5, 1.5),
                    new double3(1.75, 1.75, 1.75),
                    new double3(2, 2, 2),
                }
            };
        }

        private const int fPrecision = 4;
        private const int dPrecision = 6;

        [Theory]
        [MemberData(nameof(OBBfData))]
        public void ConstructorSingle_MinMax(float3[] vertices)
        {
            var actual = new OBBf(vertices);

            for (var i = 0; i < 3; i++)
            {
                Assert.Equal(0f, actual.Min[i], fPrecision);
                Assert.Equal(2f, actual.Max[i], fPrecision);
            }
        }

        [Theory]
        [MemberData(nameof(OBBdData))]
        public void ConstructorDouble_MinMax(double3[] vertices)
        {
            var actual = new OBBd(vertices);

            for (var i = 0; i < 3; i++)
            {
                Assert.Equal(0, actual.Min[i], dPrecision);
                Assert.Equal(2, actual.Max[i], dPrecision);
            }
        }

        [Theory]
        [MemberData(nameof(OBBfData))]
        public void CenterSingle_Is1(float3[] vertices)
        {
            var obbf = new OBBf(vertices);

            var actual = obbf.Center;

            Assert.Equal(1, actual.x, fPrecision);
            Assert.Equal(1, actual.y, fPrecision);
            Assert.Equal(1, actual.z, fPrecision);
        }

        [Theory]
        [MemberData(nameof(OBBdData))]
        public void CenterDouble_Is1(double3[] vertices)
        {
            var obbd = new OBBd(vertices);

            var actual = obbd.Center;

            Assert.Equal(1, actual.x, dPrecision);
            Assert.Equal(1, actual.y, dPrecision);
            Assert.Equal(1, actual.z, dPrecision);
        }

        [Theory]
        [MemberData(nameof(OBBfData))]
        public void SizeSingle_Is1(float3[] vertices)
        {
            var obbf = new OBBf(vertices);

            var size = obbf.Size;

            Assert.Equal(2, size.x, fPrecision);
            Assert.Equal(2, size.y, fPrecision);
            Assert.Equal(2, size.z, fPrecision);
        }

        [Theory]
        [MemberData(nameof(OBBdData))]
        public void SizeDouble_Is1(double3[] vertices)
        {
            var obbd = new OBBd(vertices);

            var size = obbd.Size;

            Assert.Equal(2, size.x, dPrecision);
            Assert.Equal(2, size.y, dPrecision);
            Assert.Equal(2, size.z, dPrecision);
        }
    }

}
