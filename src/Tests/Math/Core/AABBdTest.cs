using Fusee.Math.Core;
using System.Collections.Generic;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class AABBdTest
    {
        [Fact]
        public void Constructor_MinMax()
        {
            var actual = new AABBd(new double3(0, 0, 0), new double3(1, 1, 1));

            Assert.Equal(new double3(0, 0, 0), actual.min);
            Assert.Equal(new double3(1, 1, 1), actual.max);
        }

        [Fact]
        public void Center_Is1()
        {
            var aabbf = new AABBd(new double3(0, 0, 0), new double3(2, 2, 2));

            var actual = aabbf.Center;

            Assert.Equal(new double3(1, 1, 1), actual);
        }

        [Fact]
        public void Size_Is1()
        {
            var aabbf = new AABBd(new double3(0, 0, 0), new double3(1, 1, 1));
            var actual = aabbf.Size;

            Assert.Equal(new double3(1, 1, 1), actual);
        }

        [Theory]
        [MemberData(nameof(GetUnion))]
        public void Union_IsUnion(AABBd a, AABBd b, AABBd expected)
        {
            var actual = AABBd.Union(a, b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransform))]
        public void Transform_IsTransform(double4x4 m, AABBd box, AABBd expected)
        {
            var actual = m * box;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IntersectRay_Simple()
        {
            RayD ray = new(new double3(0, 0, 0), new double3(1, 0, 0));
            AABBd box = new(new double3(2, -1, -1), new double3(4, 1, 1));

            Assert.True(box.IntersectRay(ray));
        }

        [Fact]
        public void IntersectRay_AlongEdge()
        {
            RayD ray = new(new double3(0, 0, 0), new double3(1, 0, 0));
            AABBd box = new(new double3(2, 0, 0), new double3(4, 1, 1));

            Assert.True(box.IntersectRay(ray));
        }

        [Fact]
        public void IntersectRay_Outside()
        {
            RayD ray = new(new double3(0, -1, -1), new double3(1, 0, 0));
            AABBd box = new(new double3(2, 0, 0), new double3(4, 1, 1));

            Assert.False(box.IntersectRay(ray));
        }

        #region IEnumerables

        public static IEnumerable<object[]> GetUnion()
        {
            var a = new AABBd(new double3(0, 0, 0), new double3(1, 1, 1));
            var b = new AABBd(new double3(1, 1, 1), new double3(2, 2, 2));

            yield return new object[] { a, b, new AABBd(new double3(0, 0, 0), new double3(2, 2, 2)) };
            yield return new object[] { b, a, new AABBd(new double3(0, 0, 0), new double3(2, 2, 2)) };
        }

        public static IEnumerable<object[]> GetTransform()
        {
            var a = new AABBd(new double3(0, 0, 0), new double3(1, 1, 1));

            var xRot = new double4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1);
            var yRot = new double4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1);
            var zRot = new double4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            yield return new object[] { xRot, a, new AABBd(new double3(0, -1, 0), new double3(1, 0, 1)) };
            yield return new object[] { yRot, a, new AABBd(new double3(0, 0, -1), new double3(1, 1, 0)) };
            yield return new object[] { zRot, a, new AABBd(new double3(-1, 0, 0), new double3(0, 1, 1)) };
        }

        #endregion
    }
}