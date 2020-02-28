using Fusee.Math.Core;
using System.Collections.Generic;
using Xunit;

namespace Fusee.Test.Math.Core
{
    public class PlaneDTest
    {
        [Fact]
        public void SignedDistanceFromPoint_IsMinus5()
        {
            var plane = new PlaneD() { A = 1, B = 0, C = 0, D = 5 };
            var dist = plane.SignedDistanceFromPoint(new double3(0, 1, 0));
            Assert.Equal(-5, dist);
        }

        [Fact]
        public void SignedDistanceFromPoint_IsPlus5()
        {
            var plane = new PlaneD() { A = -1, B = 0, C = 0, D = -5 };
            var dist = plane.SignedDistanceFromPoint(new double3(0, 1, 0));
            Assert.Equal(5, dist);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(PlaneD plane, PlaneD expected)
        {
            var normalizedPlane = plane.Normalize();
            Assert.Equal(normalizedPlane, expected);
        }

        [Fact]
        public void IntersectsAABBd_IsTrue()
        {
            var aabb = new AABBd(new double3(0, 0, -1), new double3(6, 1, 1));
            var plane = new PlaneD() { A = 1, B = 0, C = 0, D = 5 };
            Assert.True(plane.Intersects(aabb));
        }

        [Theory]
        [MemberData(nameof(GetIntersectsAABBd_IsFalse))]
        public void IntersectsAABBd_IsFalse(PlaneD plane, AABBd aabb)
        {
            Assert.False(plane.Intersects(aabb));
        }

        [Fact]
        public void IntersectsOBBd_IsTrue()
        {
            var obb = new OBBd(new double3[3] { new double3(4, 0, 0), new double3(6, 0, 0), new double3(5, 5, 5) });
            var plane = new PlaneD() { A = 1, B = 0, C = 0, D = 5 };
            Assert.True(plane.Intersects(obb));
        }

        [Fact]
        public void IntersectsOBBd_IsFalse()
        {
            var obb = new OBBd(new double3[3] { new double3(0, 0, 0), new double3(4, 0, 0), new double3(2, 5, 5) });
            var plane = new PlaneD() { A = 1, B = 0, C = 0, D = 5 };
            Assert.False(plane.Intersects(obb));
        }

        [Theory]
        [MemberData(nameof(GetInsideOrIntersectingAABBd_IsTrue))]
        public void InsideOrIntersectingAABBd_IsTrue(PlaneD plane, AABBd aabb)
        {
            Assert.True(plane.InsideOrIntersecting(aabb));
        }

        [Fact]
        public void InsideOrIntersectingAABBd_IsFalse()
        {
            var aabb = new AABBd(new double3(6, 0, 0), new double3(7, 1, 1));
            var plane = new PlaneD() { A = 1, B = 0, C = 0, D = 5 };
            Assert.False(plane.InsideOrIntersecting(aabb));
        }

        [Theory]
        [MemberData(nameof(GetInsideOrIntersectingOBBd_IsTrue))]
        public void InsideOrIntersectingOBBd_IsTrue(PlaneD plane, OBBd obb)
        {
            Assert.True(plane.InsideOrIntersecting(obb));
        }

        [Fact]
        public void InsideOrIntersectingOBBd_IsFalse()
        {
            var aabb = new AABBd(new double3(6, 0, 0), new double3(7, 1, 1));
            var plane = new PlaneD() { A = 1, B = 0, C = 0, D = 5 };
            Assert.False(plane.InsideOrIntersecting(aabb));
        }

        public static IEnumerable<object[]> GetIntersectsAABBd_IsFalse()
        {
            var plane = new PlaneD() { A = 1, B = 0, C = 0, D = 5 };
            yield return new object[] { plane, new AABBd(double3.Zero, new double3(4, 1, 1)) }; //Intersects false
            yield return new object[] { plane, new AABBd(new double3(6, 0, 0), new double3(7, 1, 1)) }; //Outside true, Intersects false
        }

        public static IEnumerable<object[]> GetInsideOrIntersectingAABBd_IsTrue()
        {
            var plane = new PlaneD() { A = 1, B = 0, C = 0, D = 5 };
            yield return new object[] { plane, new AABBd(new double3(0, 0, -1), new double3(6, 1, 1)) }; //Intersects true
            yield return new object[] { plane, new AABBd(new double3(0, 0, -1), new double3(4, 1, 1)) }; //Inside true, Intersects false
        }

        public static IEnumerable<object[]> GetInsideOrIntersectingOBBd_IsTrue()
        {
            var plane = new PlaneD() { A = 1, B = 0, C = 0, D = 5 };
            yield return new object[] { plane, new OBBd(new double3[3] { new double3(4, 0, 0), new double3(6, 0, 0), new double3(5, 5, 5) }) }; //Intersects true
            yield return new object[] { plane, new OBBd(new double3[3] { new double3(0, 0, -1), new double3(4, 0, 0), new double3(2, 5, 5) }) }; //Inside true, Intersects false
        }

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new PlaneD() { A = 3, B = 0, C = 0, D = 6 }, new PlaneD() { A = 1, B = 0, C = 0, D = 2 } };
            yield return new object[] { new PlaneD() { A = 0, B = 3, C = 0, D = 6 }, new PlaneD() { A = 0, B = 1, C = 0, D = 2 } };
            yield return new object[] { new PlaneD() { A = 0, B = 0, C = 3, D = 6 }, new PlaneD() { A = 0, B = 0, C = 1, D = 2 } };
            yield return new object[]
            {
                new PlaneD{ A = 1, B = 1, C = 1, D = 1 },
                new PlaneD(){A = System.Math.Sqrt(1d / 3d), B=System.Math.Sqrt(1d / 3d), C=System.Math.Sqrt(1d / 3d), D=System.Math.Sqrt(1d / 3d) }
            };
        }
    }
}
