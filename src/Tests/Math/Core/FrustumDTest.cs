using Fusee.Math.Core;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class FrustumDTest
    {
        [Theory]
        [MemberData(nameof(GetFrustumPlanes))]
        public void CalculateFrustumPlanes_IsFrustumPlanes(PlaneD actual, PlaneD expected)
        {
            Assert.Equal(expected.A, actual.A, 7);
            Assert.Equal(expected.B, actual.B, 7);
            Assert.Equal(expected.C, actual.C, 7);
            Assert.Equal(expected.D, actual.D, 7);
        }

        [Fact]
        public void CalculateFrustumCorners_IsFrustumCorners()
        {
            var projection = double4x4.CreatePerspectiveFieldOfView(M.PiOver2, 1, 2, 10);
            var actualCorners = FrustumD.CalculateFrustumCorners(projection).ToList();

            var expectedCorners = new List<double3>()
            {
                new double3(-2, -2, 2),
                new double3(2, -2, 2),
                new double3(-2, 2, 2),
                new double3(2, 2, 2),

                new double3(-10, -10, 10),
                new double3(10, -10, 10),
                new double3(-10, 10, 10),
                new double3(10, 10, 10),
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
            var projection = double4x4.CreatePerspectiveFieldOfView(M.PiOver2, 1, 2, 10);
            var frustum = new FrustumD();
            frustum.CalculateFrustumPlanes(projection);

            var left = new PlaneD() { A = -0.5, B = 0, C = -0.5, D = 0 };
            var right = new PlaneD() { A = 0.5, B = 0, C = -0.5, D = 0 };

            var near = new PlaneD() { A = 0, B = 0, C = -1, D = -2 };
            var far = new PlaneD() { A = 0, B = 0, C = 1, D = 10 };

            var top = new PlaneD() { A = 0, B = 0.5, C = -0.5, D = 0 };
            var bottom = new PlaneD() { A = 0, B = -0.5, C = -0.5, D = 0 };

            yield return new object[] { near.Normalize(), frustum.Near.Normalize() };
            yield return new object[] { far.Normalize(), frustum.Far.Normalize() };

            yield return new object[] { left.Normalize(), frustum.Left.Normalize() };
            yield return new object[] { right.Normalize(), frustum.Right.Normalize() };

            yield return new object[] { top.Normalize(), frustum.Top.Normalize() };
            yield return new object[] { bottom.Normalize(), frustum.Bottom.Normalize() };
        }
    }
}