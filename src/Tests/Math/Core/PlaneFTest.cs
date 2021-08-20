using Fusee.Math.Core;
using System.Collections.Generic;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class PlaneFTest
    {
        [Fact]
        public void SignedDistanceFromPoint_IsMinus5()
        {
            var plane = new PlaneF() { A = 1, B = 0, C = 0, D = 5 };
            var dist = plane.SignedDistanceFromPoint(new float3(0, 1, 0));
            Assert.Equal(-5, dist);
        }

        [Fact]
        public void AngleBetween_Is90Deg()
        {
            var planeOne = new PlaneD() { A = -0.5f, B = 0, C = 0.5f, D = 0 };
            var planeTwo = new PlaneD() { A = 0.5f, B = 0, C = 0.5f, D = 0 };

            Assert.Equal(M.PiOver2, planeOne.AngleBetween(planeTwo), 5);
        }

        [Fact]
        public void SignedDistanceFromPoint_IsPlus5()
        {
            var plane = new PlaneF() { A = -1, B = 0, C = 0, D = -5 };
            var dist = plane.SignedDistanceFromPoint(new float3(0, 1, 0));
            Assert.Equal(5, dist);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(PlaneF plane, PlaneF expected)
        {
            var normalizedPlane = plane.Normalize();
            Assert.Equal(normalizedPlane, expected);
        }

        [Fact]
        public void IntersectsAABBf_IsTrue()
        {
            var aabb = new AABBf(new float3(0, 0, -1), new float3(6, 1, 1));
            var plane = new PlaneF() { A = 1, B = 0, C = 0, D = 5 };
            Assert.True(plane.Intersects(aabb));
        }

        [Theory]
        [MemberData(nameof(GetIntersectsAABBf_IsFalse))]
        public void IntersectsAABBf_IsFalse(PlaneF plane, AABBf aabb)
        {
            Assert.False(plane.Intersects(aabb));
        }

        [Fact]
        public void IntersectsOBBf_IsTrue()
        {
            var obb = new OBBf(new float3[3] { new float3(4, 0, 0), new float3(6, 0, 0), new float3(5, 5, 5) });
            var plane = new PlaneF() { A = 1, B = 0, C = 0, D = 5 };
            Assert.True(plane.Intersects(obb));
        }

        [Fact]
        public void IntersectsOBBf_IsFalse()
        {
            var obb = new OBBf(new float3[3] { new float3(0, 0, 0), new float3(4, 0, 0), new float3(2, 5, 5) });
            var plane = new PlaneF() { A = 1, B = 0, C = 0, D = 5 };
            Assert.False(plane.Intersects(obb));
        }

        [Theory]
        [MemberData(nameof(GetInsideOrIntersectingAABBf_IsTrue))]
        public void InsideOrIntersectingAABBf_IsTrue(PlaneF plane, AABBf aabb)
        {
            Assert.True(plane.InsideOrIntersecting(aabb));
        }

        [Fact]
        public void InsideOrIntersectingAABBf_IsFalse()
        {
            var aabb = new AABBf(new float3(6, 0, 0), new float3(7, 1, 1));
            var plane = new PlaneF() { A = 1, B = 0, C = 0, D = 5 };
            Assert.False(plane.InsideOrIntersecting(aabb));
        }

        [Theory]
        [MemberData(nameof(GetInsideOrIntersectingOBBf_IsTrue))]
        public void InsideOrIntersectingOBBf_IsTrue(PlaneF plane, OBBf obb)
        {
            Assert.True(plane.InsideOrIntersecting(obb));
        }

        [Fact]
        public void InsideOrIntersectingOBBf_IsFalse()
        {
            var aabb = new AABBf(new float3(6, 0, 0), new float3(7, 1, 1));
            var plane = new PlaneF() { A = 1, B = 0, C = 0, D = 5 };
            Assert.False(plane.InsideOrIntersecting(aabb));
        }

        public static IEnumerable<object[]> GetIntersectsAABBf_IsFalse()
        {
            var plane = new PlaneF() { A = 1, B = 0, C = 0, D = 5 };
            yield return new object[] { plane, new AABBf(float3.Zero, new float3(4, 1, 1)) }; //Intersects false
            yield return new object[] { plane, new AABBf(new float3(6, 0, 0), new float3(7, 1, 1)) }; //Outside true, Intersects false
        }

        public static IEnumerable<object[]> GetInsideOrIntersectingAABBf_IsTrue()
        {
            var plane = new PlaneF() { A = 1, B = 0, C = 0, D = 5 };
            yield return new object[] { plane, new AABBf(new float3(0, 0, -1), new float3(6, 1, 1)) }; //Intersects true
            yield return new object[] { plane, new AABBf(new float3(0, 0, -1), new float3(4, 1, 1)) }; //Inside true, Intersects false
        }

        public static IEnumerable<object[]> GetInsideOrIntersectingOBBf_IsTrue()
        {
            var plane = new PlaneF() { A = 1, B = 0, C = 0, D = 5 };
            yield return new object[] { plane, new OBBf(new float3[3] { new float3(4, 0, 0), new float3(6, 0, 0), new float3(5, 5, 5) }) }; //Intersects true
            yield return new object[] { plane, new OBBf(new float3[3] { new float3(0, 0, -1), new float3(4, 0, 0), new float3(2, 5, 5) }) }; //Inside true, Intersects false
        }

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new PlaneF() { A = 3, B = 0, C = 0, D = 6 }, new PlaneF() { A = 1, B = 0, C = 0, D = 2 } };
            yield return new object[] { new PlaneF() { A = 0, B = 3, C = 0, D = 6 }, new PlaneF() { A = 0, B = 1, C = 0, D = 2 } };
            yield return new object[] { new PlaneF() { A = 0, B = 0, C = 3, D = 6 }, new PlaneF() { A = 0, B = 0, C = 1, D = 2 } };
            yield return new object[]
            {
                new PlaneF{ A = 1, B = 1, C = 1, D = 1 },
                new PlaneF(){A = (float)System.Math.Sqrt(1d / 3d), B=(float)System.Math.Sqrt(1d / 3d), C=(float)System.Math.Sqrt(1d / 3d), D=(float)System.Math.Sqrt(1d / 3d) }
            };
        }
    }
}