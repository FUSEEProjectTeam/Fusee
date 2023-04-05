using Fusee.Math.Core;
using System.Collections.Generic;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class MTest
    {
        #region Max
        [Fact]
        public void Max_Double()
        {
            double a = 1;
            double b = 2;

            var actual = M.Max(a, b);

            Assert.Equal(2, actual);
        }

        [Fact]
        public void Max_Int16()
        {
            short a = 1;
            short b = 2;

            var actual = M.Max(a, b);

            Assert.Equal(2, actual);
        }

        [Fact]
        public void Max_Int32()
        {
            var a = 1;
            var b = 2;

            var actual = M.Max(a, b);

            Assert.Equal(2, actual);
        }

        [Fact]
        public void Max_Single()
        {
            float a = 1;
            float b = 2;

            var actual = M.Max(a, b);

            Assert.Equal(2, actual);
        }

        [Fact]
        public void Max_UInt16()
        {
            ushort a = 1;
            ushort b = 2;

            var actual = M.Max(a, b);

            Assert.Equal(2, actual);
        }

        [Fact]
        public void Max_UInt32()
        {
            uint a = 1;
            uint b = 2;
            uint expected = 2;

            var actual = M.Max(a, b);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Min
        [Fact]
        public void Min_Double()
        {
            double a = 1;
            double b = 2;

            var actual = M.Min(a, b);

            Assert.Equal(1, actual);
        }

        [Fact]
        public void Min_Int16()
        {
            short a = 1;
            short b = 2;

            var actual = M.Min(a, b);

            Assert.Equal(1, actual);
        }

        [Fact]
        public void Min_Int32()
        {
            var a = 1;
            var b = 2;

            var actual = M.Min(a, b);

            Assert.Equal(1, actual);
        }

        [Fact]
        public void Min_Single()
        {
            float a = 1;
            float b = 2;

            var actual = M.Min(a, b);

            Assert.Equal(1, actual);
        }

        [Fact]
        public void Min_UInt16()
        {
            ushort a = 1;
            ushort b = 2;

            var actual = M.Min(a, b);

            Assert.Equal(1, actual);
        }

        [Fact]
        public void Min_UInt32()
        {
            uint a = 1;
            uint b = 2;
            uint expected = 1;

            var actual = M.Min(a, b);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Fields
        [Fact]
        public void EpsilonFloat_IsEpsilon()
        {
            var expected = 1.192093E-07f;

            Assert.Equal(M.EpsilonFloat, expected);
        }

        [Fact]
        public void EpsilonDouble_IsEpsilon()
        {
            var expected = 1.11022302462516E-16d;

            Assert.Equal(M.EpsilonDouble, expected);
        }

        [Fact]
        public void Pi_IsPi()
        {
            var expected = 3.14159265358979f;

            Assert.Equal(M.Pi, expected);
        }

        [Fact]
        public void PiOver2_IsPiOver2()
        {
            var expected = 3.14159265358979f / 2;

            Assert.Equal(M.PiOver2, expected);
        }

        [Fact]
        public void PiOver3_IsPiOver3()
        {
            var expected = 3.14159265358979f / 3;

            Assert.Equal(M.PiOver3, expected);
        }

        [Fact]
        public void PiOver4_IsPiOver4()
        {
            var expected = 3.14159265358979f / 4;

            Assert.Equal(M.PiOver4, expected);
        }

        [Fact]
        public void PiOver6_IsPiOver6()
        {
            var expected = 3.14159265358979f / 6;

            Assert.Equal(M.PiOver6, expected);
        }

        [Fact]
        public void TwoPi_IsTwoPi()
        {
            var expected = 3.14159265358979f * 2;

            Assert.Equal(M.TwoPi, expected);
        }

        [Fact]
        public void ThreePiOverTwo_IsThreePiOverTwo()
        {
            var expected = 3.14159265358979f * 3 / 2;

            Assert.Equal(M.ThreePiOver2, expected);
        }

        [Fact]
        public void E_IsE()
        {
            var expected = 2.71828182845904523536f;

            Assert.Equal(M.E, expected);
        }

        [Fact]
        public void Log10E_IsLog10E()
        {
            var expected = 0.434294482f;

            Assert.Equal(M.Log10E, expected);
        }

        [Fact]
        public void Log2E_IsLog2E()
        {
            var expected = 1.442695041f;

            Assert.Equal(M.Log2E, expected);
        }
        #endregion

        #region Trigonometry
        [Theory]
        [InlineData(0, 1)]
        [InlineData(M.PiOver2, 0)]
        [InlineData(M.TwoPi / 2, -1)]
        public void Cos_TestValues(float value, float expected)
        {
            var actual = M.Cos(value);

            Assert.Equal(expected, actual, 5f);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(M.PiOver2, 1)]
        [InlineData(M.TwoPi / 2, 0)]
        public void Sin_TestValues(float value, float expected)
        {
            var actual = M.Sin(value);

            Assert.Equal(expected, actual, 5f);
        }

        #endregion

        #region NextPowerOfTwo
        [Fact]
        public void NextPowerOfTwo_Double()
        {
            var value = 2.01d;

            var actual = M.NextPowerOfTwo(value);

            Assert.Equal(4, actual);
        }

        [Fact]
        public void NextPowerOfTwo_Int32()
        {
            var value = 3;

            var actual = M.NextPowerOfTwo(value);

            Assert.Equal(4, actual);
        }

        [Fact]
        public void NextPowerOfTwo_Int64()
        {
            long value = 3;

            var actual = M.NextPowerOfTwo(value);

            Assert.Equal(4, actual);
        }

        [Fact]
        public void NextPowerOfTwo_Single()
        {
            var value = 2.1f;

            var actual = M.NextPowerOfTwo(value);

            Assert.Equal(4, actual);
        }

        [Fact]
        public void IsPowerOfTwo_True()
        {
            var actual = M.IsPowerOfTwo(4);

            Assert.True(actual);
        }

        [Fact]
        public void IsPowerOfTwo_False()
        {
            var actual = M.IsPowerOfTwo(3);

            Assert.False(actual);
        }
        #endregion

        #region Factorial
        [Theory]
        [InlineData(1, 1)]
        [InlineData(3, 6)]
        [InlineData(5, 120)]
        public void Factorial_TestValues(int value, int expected)
        {
            var actual = M.Factorial(value);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region CovarianceMatrix

        [Fact]
        public void CovarianceMatrixSingle_IsValid()
        {
            var val = new List<float3>
            {
                new float3(90, 60, 90), new float3(90, 90, 30),
                new float3(60, 60, 60), new float3(60, 60, 90), new float3(30, 30, 30)
            };

            var centroid = M.CalculateCentroid(val);
            var actual = M.CreateCovarianceMatrix(centroid, val);

            var expected = new float4x4(new float4(504, 360, 180, 0), new float4(360, 360, 0, 0), new float4(180, 0, 720, 0), new float4(0, 0, 0, 1));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CovarianceMatrixDouble_IsValid()
        {
            var val = new List<double3>
            {
                new double3(90, 60, 90), new double3(90, 90, 30),
                new double3(60, 60, 60), new double3(60, 60, 90), new double3(30, 30, 30)
            };

            var centroid = M.CalculateCentroid(val);
            var actual = M.CreateCovarianceMatrix(centroid, val);

            var expected = new double4x4(new double4(504, 360, 180, 0), new double4(360, 360, 0, 0), new double4(180, 0, 720, 0), new double4(0, 0, 0, 1));

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Centroid

        [Fact]
        public void CentroidCalculationSingle_IsValid()
        {
            // Cube
            var data = new float3[]
            {
                new float3(0,0,0),
                new float3(1,0,0),
                new float3(1,0,1),
                new float3(1,1,1),
                new float3(0,1,1),
                new float3(1,1,0),
                new float3(0,1,0),
                new float3(0,0,1),
            };

            Assert.Equal(new float3(0.5f, 0.5f, 0.5f), M.CalculateCentroid(data));

        }

        [Fact]
        public void CentroidCalculationDouble_IsValid()
        {
            // Cube
            var data = new double3[]
            {
                new double3(0,0,0),
                new double3(1,0,0),
                new double3(1,0,1),
                new double3(1,1,1),
                new double3(0,1,1),
                new double3(1,1,0),
                new double3(0,1,0),
                new double3(0,0,1),
            };

            Assert.Equal(new double3(0.5, 0.5, 0.5), M.CalculateCentroid(data));
        }
        #endregion

        #region BinomialCoefficient
        [Fact]
        public void BinominalCoefficient_IsOne()
        {
            var k = 0;
            var n = 1;

            Assert.Equal(1, M.BinomialCoefficient(n, k));
        }

        #endregion

        #region InverseSqrtFast
        [Theory]
        [InlineData(1.0d, 1.0d)]
        [InlineData(4.0d, 0.5d)]
        public void InverseSqrtFast_Double(double value, double expected)
        {
            var actual = M.InverseSqrtFast(value);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1.0f, 1.0f)]
        [InlineData(4.0f, 0.5f)]
        public void InverseSqrtFast_Float(float value, float expected)
        {
            var actual = M.InverseSqrtFast(value);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region DegreesToRadians
        [Theory]
        [InlineData(0, 0)]
        [InlineData(90, M.PiOver2)]
        public void DegreesToRadians_TestValues(float degree, float expected)
        {
            var actual = M.DegreesToRadians(degree);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(M.PiOver2, 90)]
        public void RadiansToDegrees_TestValues(float radians, float expected)
        {
            var actual = M.RadiansToDegrees(radians);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region MinAngle
        [Theory]
        [InlineData(2.0 * System.Math.PI, 0)]
        [InlineData(System.Math.PI, System.Math.PI)]
        [InlineData(0, 0)]
        public void MinAngle_Double(double angle, double expected)
        {
            var actual = M.MinAngle(angle);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(M.TwoPi, 0)]
        [InlineData(M.Pi, M.Pi)]
        [InlineData(0, 0)]
        public void MinAngle_Float(float angle, float expected)
        {
            var actual = M.MinAngle(angle);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Conversion
        [Fact]
        public void Float4ToABGR_TestMax()
        {
            var vec = new float4(1, 1, 1, 1);
            var expected = 4294967295;

            var actual = M.Float4ToABGR(vec);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Swap
        [Fact]
        public void Swap_Double()
        {
            double a = 0;
            double b = 1;

            M.Swap(ref a, ref b);

            Assert.Equal(1, a);
            Assert.Equal(0, b);
        }

        [Fact]
        public void Swap_Single()
        {
            float a = 0;
            float b = 1;

            M.Swap(ref a, ref b);

            Assert.Equal(1, a);
            Assert.Equal(0, b);
        }
        #endregion

        #region Clamp
        [Theory]
        [InlineData(2, 0, 1)]
        [InlineData(0, 1, 2)]
        public void Clamp_IsMinIsMax_float(float x, float min, float max)
        {
            Assert.Equal(1, M.Clamp(x, min, max));
        }

        [Theory]
        [InlineData(2, 0, 1)]
        [InlineData(0, 1, 2)]
        public void Clamp_IsMinIsMax_double(double x, double min, double max)
        {
            Assert.Equal(1, M.Clamp(x, min, max));
        }

        [Theory]
        [InlineData(2.222f, 1.111f, 0.0f)]
        [InlineData(1.111f, 2.222f, 1.0f)]
        public void Step(float edge, float val, float expected)
        {
            Assert.Equal(expected, M.Step(edge, val));
        }

        #endregion

        #region Interpolate/Lerp
        [Theory]
        [InlineData(0.5f, 0.5f)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        public void SineStep_TestValues(float t, float expected)
        {
            var actual = M.SineStep(t);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(0.5f, 0.5f)]
        public void SmootherStep(float t, float expected)
        {
            var actual = M.SmootherStep(t);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(0.5f, 0.5f)]
        public void SmoothStep(float t, float expected)
        {
            var actual = M.SmoothStep(t);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 1, 0.5f, 0.5f)]
        [InlineData(0, 1, 0, 0)]
        [InlineData(0, 1, 1, 1)]
        public void Lerp(float a, float b, float blend, float expected)
        {
            var actual = M.Lerp(a, b, blend);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Equals
        [Fact]
        public void Equals_IsEqual_Float()
        {
            var a = 1.0f;
            var b = 1.0f;

            Assert.True(M.Equals(a, b));
        }

        [Fact]
        public void Equals_IsInequal_Float()
        {
            var a = 1.0f;
            var b = 0.0f;

            Assert.False(M.Equals(a, b));
        }

        [Fact]
        public void Equals_IsEqual_Double()
        {
            var a = 1.0d;
            var b = 1.0d;

            Assert.True(M.Equals(a, b));
        }

        [Fact]
        public void Equals_IsInequal_Double()
        {
            var a = 1.0d;
            var b = 0.0d;

            Assert.False(M.Equals(a, b));
        }

        #endregion

        #region ScreenToWorldPos

        [Theory]
        [MemberData(nameof(ScreenToWorldPosData))]
        public void ScreenToWorldPos(float2 data, float3 expected)
        {
            var height = 1000;
            var width = 1000;

            var p = float4x4.CreatePerspectiveFieldOfView(M.PiOver2, (float)width / height, 1, 1000);

            var actual = M.ScreenPointToWorld(data, 1, p, float4x4.Identity, p.Invert(), float4x4.Identity.Invert(), width, height);

            Assert.Equal(expected.x, actual.x);
            Assert.Equal(expected.y, actual.y);
            Assert.Equal(expected.z, actual.z);
        }


        public static IEnumerable<object[]> ScreenToWorldPosData()
        {
            yield return new object[] { new float2(0, 0), new float3(-1, 1, 1) };
            yield return new object[] { new float2(500, 500), new float3(0, 0, 1) };
            yield return new object[] { new float2(1000, 1000), new float3(1, -1, 1) };
        }

        #endregion
    }
}