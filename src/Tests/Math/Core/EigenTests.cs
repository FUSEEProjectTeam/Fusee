using Fusee.Math.Core;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class EigenTests
    {

        #region Eigen

        /// <summary>      
        /// 
        /// Note: Test values generated via Eigen lib:
        /// 
        ///     Eigen::Matrix3f mat;
        ///     mat << 504, 360, 180, 360, 360, 0, 180, 0, 720;
        ///     Eigen::EigenSolver<Eigen::Matrix3f> solver;
        ///     solver.compute(mat, true);
        ///     
        /// </summary>
        [Fact]
        public void EigenFromCovarianceFloatMat_IsValid()
        {
            var val = new float3[]
            {
                new float3(90, 60, 90), new float3(90, 90, 30),
                new float3(60, 60, 60), new float3(60, 60, 90),
                new float3(30, 30, 30)
            };

            var actual = new Eigen(val);

            var expectedVals = new float[] { 910.07f, 44.8197f, 629.11f };

            var expectedVectors = new float3[]
            {
                new float3(0.655802f, -0.64879f, -0.385999f),
                new float3(0.429198f, 0.74105f, -0.516366f),
                new float3(0.621058f, 0.172964f, 0.764441f),
            };

            for (int i = 0; i < 3; i++)
            {
                const int precision = 3;
                Assert.Equal(expectedVals[i], actual.Values[i], precision);
                Assert.Equal(expectedVectors[i].x, actual.Vectors[i].x, precision);
                Assert.Equal(expectedVectors[i].y, actual.Vectors[i].y, precision);
                Assert.Equal(expectedVectors[i].z, actual.Vectors[i].z, precision);
            }
        }

        /// <summary>       
        /// 
        /// Note: Test values generated via Eigen lib:
        /// 
        ///     Eigen::Matrix3d mat;
        ///     mat << 504, 360, 180, 360, 360, 0, 180, 0, 720;
        ///     Eigen::EigenSolver<Eigen::Matrix3d> solver;
        ///     solver.compute(mat, true);
        ///     
        /// </summary>
        [Fact]
        public void EigenFromCovarianceDoubleMat_IsValid()
        {
            var val = new double3[]
            {
                new double3(90, 60, 90), new double3(90, 90, 30),
                new double3(60, 60, 60), new double3(60, 60, 90),
                new double3(30, 30, 30)
            };

            var actual = new Eigen(val);

            var expectedVals = new double[] { 910.0700, 44.8197, 629.1104 };

            var expectedVectors = new double3[]
            {
                new double3(0.655802, -0.64879, -0.385999),
                new double3(0.429198, 0.74105, -0.516366),
                new double3(0.621058, 0.172964, 0.764441),
            };

            for (int i = 0; i < 3; i++)
            {
                const int precision = 4;
                Assert.Equal(expectedVals[i], actual.Values[i], precision);
                Assert.Equal(expectedVectors[i].x, actual.Vectors[i].x, precision);
                Assert.Equal(expectedVectors[i].y, actual.Vectors[i].y, precision);
                Assert.Equal(expectedVectors[i].z, actual.Vectors[i].z, precision);
            }
        }


        #endregion
    }
}