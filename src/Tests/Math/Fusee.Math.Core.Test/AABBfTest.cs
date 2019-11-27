using System.Collections.Generic;
using Xunit;

namespace Fusee.Math.Core.Test
{
    public class AABBfTest
    {
        [Fact]
        public void Constructor_MinMax()
        {
            var actual = new AABBf(new float3(0, 0, 0), new float3(1, 1, 1));

            Assert.Equal(new float3(0, 0, 0), actual.min);
            Assert.Equal(new float3(1, 1, 1), actual.max);
        }

        [Fact]
        public void Center_Is1()
        {
            var aabbf = new AABBf(new float3(0, 0, 0), new float3(2, 2, 2));

            var actual = aabbf.Center;

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Fact]
        public void Size_Is1()
        {
            var aabbf = new AABBf(new float3(0, 0, 0), new float3(1, 1, 1));

            var actual = aabbf.Size;

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        [Theory]
        [MemberData(nameof(GetUnion))]
        public void Union_IsUnion(AABBf a, AABBf b, AABBf expected)
        {
            var actual = AABBf.Union(a, b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransform))]
        public void Transform_IsTransform(float4x4 m, AABBf box, AABBf expected)
        {
            var actual = m * box;

            Assert.Equal(expected, actual);
        }

        #region IEnumerables

        public static IEnumerable<object[]> GetUnion()
        {
            var a = new AABBf(new float3(0, 0, 0), new float3(1, 1, 1));
            var b = new AABBf(new float3(1, 1, 1), new float3(2, 2, 2));

            yield return new object[] {a, b, new AABBf(new float3(0, 0, 0), new float3(2, 2, 2))};
            yield return new object[] {b, a, new AABBf(new float3(0, 0, 0), new float3(2, 2, 2))};
        }

        public static IEnumerable<object[]> GetTransform()
        {
            var a = new AABBf(new float3(0, 0, 0), new float3(1, 1, 1));

            var xRot = new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1);
            var yRot = new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1);
            var zRot = new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            yield return new object[] {xRot, a, new AABBf(new float3(0, -1, 0), new float3(1, 0, 1))};
            yield return new object[] {yRot, a, new AABBf(new float3(0, 0, -1), new float3(1, 1, 0))};
            yield return new object[] {zRot, a, new AABBf(new float3(-1, 0, 0), new float3(0, 1, 1))};
        }

        #endregion
    }
}
