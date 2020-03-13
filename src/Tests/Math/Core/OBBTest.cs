using Fusee.Math.Core;
using System.Collections.Generic;
using Xunit;

namespace Fusee.Test.Math.Core
{
    public class OBBTest
    {
        private const int fPrecision = 4;
        private const int dPrecision = 6;

        [Theory]
        [MemberData(nameof(OBBfData))]
        public void ConstructorSingle_MinMax(float3[] vertices, float3 min, float3 max)
        {
            var actual = new OBBf(vertices);

            for (var i = 0; i < 3; i++)
            {
                Assert.Equal(min[i], actual.Min[i], fPrecision);
                Assert.Equal(max[i], actual.Max[i], fPrecision);
            }
        }

        [Theory]
        [MemberData(nameof(OBBdData))]
        public void ConstructorDouble_MinMax(double3[] vertices, double3 min, double3 max)
        {
            var actual = new OBBd(vertices);

            for (var i = 0; i < 3; i++)
            {
                Assert.Equal(min[i], actual.Min[i], dPrecision);
                Assert.Equal(max[i], actual.Max[i], dPrecision);
            }
        }

        public static IEnumerable<object[]> OBBfData()
        {

            yield return new object[]
            {
                new float3[]
                {
                    new float3(float.NaN, float.NaN, float.NaN),
                },
                new float3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity),
                new float3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity),
            };

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
                },
                new float3(0, 0, 0),
                new float3(2, 2, 2)
            };

            yield return new object[]
            {
                new float3[]
                {
                    new float3(-5, -5, -5),
                    new float3(-0.25f, -0.25f, -0.25f),
                    new float3(-0.5f, -0.5f, -0.5f),
                    new float3(0.75f, 0.75f, 0.75f),
                    new float3(1.25f, 1.25f, 1.25f),
                    new float3(1.5f, 1.5f, 1.5f),
                    new float3(1.75f, 1.75f, 1.75f),
                    new float3(15, 15, 15),
                },
                new float3(-5, -5, -5),
                new float3(15, 15, 15)
            };

            yield return new object[]
            {
                new float3[]
                {
                    new float3(0, 0, 0)
                },
                new float3(0, 0, 0),
                new float3(0, 0, 0)
            };

            yield return new object[]
            {
                new float3[]
                {
                    new float3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity),
                    new float3(0.25f, 0.25f, 0.25f),
                    new float3(0.5f, 0.5f, 0.5f),
                    new float3(0.75f, 0.75f, 0.75f),
                    new float3(1.25f, 1.25f, 1.25f),
                    new float3(1.5f, 1.5f, 1.5f),
                    new float3(1.75f, 1.75f, 1.75f),
                    new float3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity),
                },
                new float3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity),
                new float3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity),
            };
        }

        public static IEnumerable<object[]> OBBdData()
        {

            yield return new object[]
            {
                new double3[]
                {
                    new double3(double.NaN, double.NaN, double.NaN),
                },
                new double3(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity),
                new double3(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity),
            };

            yield return new object[]
            {
                new double3[]
                {
                    new double3(0, 0, 0),
                    new double3(0.25f, 0.25f, 0.25f),
                    new double3(0.5f, 0.5f, 0.5f),
                    new double3(0.75f, 0.75f, 0.75f),
                    new double3(1.25f, 1.25f, 1.25f),
                    new double3(1.5f, 1.5f, 1.5f),
                    new double3(1.75f, 1.75f, 1.75f),
                    new double3(2, 2, 2),
                },
                new double3(0, 0, 0),
                new double3(2, 2, 2)
            };

            yield return new object[]
            {
                new double3[]
                {
                    new double3(-5, -5, -5),
                    new double3(-0.25f, -0.25f, -0.25f),
                    new double3(-0.5f, -0.5f, -0.5f),
                    new double3(0.75f, 0.75f, 0.75f),
                    new double3(1.25f, 1.25f, 1.25f),
                    new double3(1.5f, 1.5f, 1.5f),
                    new double3(1.75f, 1.75f, 1.75f),
                    new double3(15, 15, 15),
                },
                new double3(-5, -5, -5),
                new double3(15, 15, 15)
            };

            yield return new object[]
            {
                new double3[]
                {
                    new double3(0, 0, 0)
                },
                new double3(0, 0, 0),
                new double3(0, 0, 0)
            };

            yield return new object[]
            {
                new double3[]
                {
                    new double3(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity),
                    new double3(0.25f, 0.25f, 0.25f),
                    new double3(0.5f, 0.5f, 0.5f),
                    new double3(0.75f, 0.75f, 0.75f),
                    new double3(1.25f, 1.25f, 1.25f),
                    new double3(1.5f, 1.5f, 1.5f),
                    new double3(1.75f, 1.75f, 1.75f),
                    new double3(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity),
                },
                new double3(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity),
                new double3(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity),
            };
        }
    }

}
