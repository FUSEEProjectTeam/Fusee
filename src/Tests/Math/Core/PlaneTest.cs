using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fusee.Test.Math.Core
{
    public class PlaneTest
    {
        [Fact]
        public void SignedDistanceFromPoint_IsMinus5()
        {
            var plane = new Plane() { A = 1, B = 0, C = 0, D = 5 };
            var dist = plane.SignedDistanceFromPoint(new float3(0, 1, 0));
            Assert.Equal(-5, dist);
        }

        [Fact]
        public void SignedDistanceFromPoint_IsPlus5()
        {
            var plane = new Plane() { A = -1, B = 0, C = 0, D = -5 };
            var dist = plane.SignedDistanceFromPoint(new float3(0, 1, 0));
            Assert.Equal(5, dist);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(Plane plane, Plane expected)
        {
            var normalizedPlane = plane.Normalize();
            Assert.Equal(normalizedPlane, expected);
        }

        [Fact]
        public void IntersectsABBf_IsTrue()
        {
            var aabb = new AABBf(new float3(0, 0, -1), new float3(6, 1, 1));      
            var plane = new Plane() { A = 1, B = 0, C = 0, D = 5 };
            Assert.True(plane.Intersects(aabb));
        }

        [Theory]
        [MemberData(nameof(GetIntersectsAABBf_IsFalse))]
        public void IntersectsABBf_IsFalse(Plane plane, AABBf aabb)
        {            
            Assert.False(plane.Intersects(aabb));
        }

        //[Fact]
        //public void IntersectsOBBf_IsTrue()
        //{
        //    var obb = new OBBf();
        //    var plane = new Plane() { A = 1, B = 0, C = 0, D = 5 };
        //    Assert.True(plane.Intersects(obb));
        //}


        [Theory]
        [MemberData(nameof(GetInsideOrIntersectingABBf_IsTrue))]
        public void InsideOrIntersectingABBf_IsTrue(Plane plane,  AABBf aabb)
        {
            Assert.True(plane.InsideOrIntersecting(aabb));
        }

        [Fact]
        public void InsideOrIntersectingABBf_IsFalse()
        {
            var aabb = new AABBf(new float3(6, 0, 0), new float3(7, 1, 1));
            var plane = new Plane() { A = 1, B = 0, C = 0, D = 5 };
            Assert.False(plane.InsideOrIntersecting(aabb));
        }

        //[Fact]
        //public void InsideOrIntersectingOBBf_IsTrue()
        //{
        //    var obb = new OBBf();
        //    var plane = new Plane() { A = 1, B = 0, C = 0, D = 5 };
        //    Assert.True(plane.InsideOrIntersecting(obb));
        //}

        public static IEnumerable<object[]> GetIntersectsAABBf_IsFalse()
        {
            var plane = new Plane() { A = 1, B = 0, C = 0, D = 5 };
            yield return new object[] { plane, new AABBf(float3.Zero, new float3(4, 1, 1)) };
            yield return new object[] { plane, new AABBf(new float3(6, 0, 0), new float3(7, 1, 1)) };           
        }

        public static IEnumerable<object[]> GetInsideOrIntersectingABBf_IsTrue()
        {
            var plane = new Plane() { A = 1, B = 0, C = 0, D = 5 };
            yield return new object[] { plane, new AABBf(new float3(0, 0, -1), new float3(6, 1, 1)) }; //Intersects true
            yield return new object[] { plane, new AABBf(new float3(0, 0, -1), new float3(5, 1, 1)) };  //Inside true, Intersects false
        }

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] { new Plane() { A = 3, B = 0, C = 0, D = 6 }, new Plane() { A = 1, B = 0, C = 0, D = 2 } };
            yield return new object[] { new Plane() { A = 0, B = 3, C = 0, D = 6 }, new Plane() { A = 0, B = 1, C = 0, D = 2 } };
            yield return new object[] { new Plane() { A = 0, B = 0, C = 3, D = 6 }, new Plane() { A = 0, B = 0, C = 1, D = 2 } };
            yield return new object[]
            {
                new Plane{ A = 1, B = 1, C = 1, D = 1 },
                new Plane(){A = (float)System.Math.Sqrt(1d / 3d), B=(float)System.Math.Sqrt(1d / 3d), C=(float)System.Math.Sqrt(1d / 3d), D=(float)System.Math.Sqrt(1d / 3d) }
            };
        }        
    }
}
