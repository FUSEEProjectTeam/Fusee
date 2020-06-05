using Fusee.Math.Core;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fusee.Test.Math.Core
{
    public class FrustumTest
    {
        [Theory]
        [MemberData(nameof(GetFrustumPlanes))]
        public void CalculateFrustumPlanes_IsFrustumPlanes(PlaneF actual, PlaneF expected)
        {
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CalculateFrustumCorners_IsFrustumCorners()
        {
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver2, 1, 2, 10);
            var actualCorners = Frustum.CalculateFrustumCorners(projection).ToList();

            var expectedCorners = new List<float3>()
            {
                new float3(-2, -2, 2),
                new float3(2, -2, 2),
                new float3(-2, 2, 2),
                new float3(2, 2, 2),

                new float3(-10, -10, 10),
                new float3(10, -10, 10),
                new float3(-10, 10, 10),
                new float3(10, 10, 10),
            };

            Assert.Equal(actualCorners.Count, expectedCorners.Count);

            for (int i = 0; i < actualCorners.Count; i++)
            {
                Assert.Equal(expectedCorners[i].x, actualCorners[i].x, 4);
                Assert.Equal(expectedCorners[i].y, actualCorners[i].y, 4);
                Assert.Equal(expectedCorners[i].z, actualCorners[i].z, 4);
            }
        }

        public static IEnumerable<object[]> GetFrustumPlanes()
        {
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver2, 1, 2, 10);
            var frustum = new Frustum();
            frustum.CalculateFrustumPlanes(projection);

            var left = new PlaneF() { A = -0.5f, B = 0, C = -0.5f, D = 0 };
            var right = new PlaneF() { A = 0.5f, B = 0, C = -0.5f, D = 0 };

            var near = new PlaneF() { A = 0, B = 0, C = -1, D = -2 };
            var far = new PlaneF() { A = 0, B = 0, C = 1, D = 10 };

            var top = new PlaneF() { A = 0, B = 0.5f, C = -0.5f, D = 0 };
            var bottom = new PlaneF() { A = 0, B = -0.5f, C = -0.5f, D = 0 };

            yield return new object[] { near.Normalize(), frustum.Near.Normalize() };
            yield return new object[] { far.Normalize(), frustum.Far.Normalize() };

            yield return new object[] { left.Normalize(), frustum.Left.Normalize() };
            yield return new object[] { right.Normalize(), frustum.Right.Normalize() };

            yield return new object[] { top.Normalize(), frustum.Top.Normalize() };
            yield return new object[] { bottom.Normalize(), frustum.Bottom.Normalize() };
        }
    }
}