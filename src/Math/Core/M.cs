using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Converter type replacement for System.Converter. Enables this Assembly to be portable
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    public delegate TOutput Converter<in TInput, out TOutput>(TInput input);

    /// <summary>
    ///     Provides standard mathematical functions and helpers for different types.
    /// </summary>
    public static class M
    {
        #region Min and Max

        /// <summary>
        /// Calculates the minimum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The minimum of a and b.</returns>
        public static float Min(float a, float b)
        {
            return (a < b) ? a : b;
        }

        /// <summary>
        /// Calculates the maximum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The maximum of a and b.</returns>
        public static float Max(float a, float b)
        {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// Calculates the minimum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The minimum of a and b.</returns>
        public static double Min(double a, double b)
        {
            return (a < b) ? a : b;
        }

        /// <summary>
        /// Calculates the maximum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The maximum of a and b.</returns>
        public static double Max(double a, double b)
        {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// Calculates the minimum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The minimum of a and b.</returns>
        public static int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        /// <summary>
        /// Calculates the maximum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The maximum of a and b.</returns>
        public static int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// Calculates the minimum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The minimum of a and b.</returns>
        public static uint Min(uint a, uint b)
        {
            return (a < b) ? a : b;
        }

        /// <summary>
        /// Calculates the maximum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The maximum of a and b.</returns>
        public static uint Max(uint a, uint b)
        {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// Calculates the minimum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The minimum of a and b.</returns>
        public static short Min(short a, short b)
        {
            return (a < b) ? a : b;
        }

        /// <summary>
        /// Calculates the maximum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The maximum of a and b.</returns>
        public static short Max(short a, short b)
        {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// Calculates the minimum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The minimum of a and b.</returns>
        public static ushort Min(ushort a, ushort b)
        {
            return (a < b) ? a : b;
        }

        /// <summary>
        /// Calculates the maximum of a and b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The maximum of a and b.</returns>
        public static ushort Max(ushort a, ushort b)
        {
            return (a > b) ? a : b;
        }

        #endregion Min and Max

        #region Fields

        /// <summary>
        /// Defines the value which represents the machine epsilon for <see cref="float"/> in C#.
        /// </summary>
        public const float EpsilonFloat = 1.192093E-07f;

        /// <summary>
        /// Defines the value which represents the machine epsilon for <see cref="double"/> in C#.
        /// </summary>
        public const double EpsilonDouble = 1.11022302462516E-16d;

        /// <summary>
        /// Defines the value of Pi as a <see cref="float"/>.
        /// </summary>
        public const float Pi = 3.14159265358979f;

        /// <summary>
        /// Defines the value of Pi divided by two as a <see cref="float"/>.
        /// </summary>
        public const float PiOver2 = Pi / 2;

        /// <summary>
        /// Defines the value of Pi divided by three as a <see cref="float"/>.
        /// </summary>
        public const float PiOver3 = Pi / 3;

        /// <summary>
        /// Defines the value of  Pi divided by four as a <see cref="float"/>.
        /// </summary>
        public const float PiOver4 = Pi / 4;

        /// <summary>
        /// Defines the value of Pi divided by six as a <see cref="float"/>.
        /// </summary>
        public const float PiOver6 = Pi / 6;

        /// <summary>
        /// Defines the value of Pi multiplied by two as a <see cref="float"/>.
        /// </summary>
        public const float TwoPi = 2 * Pi;

        /// <summary>
        /// Defines the value of Pi multiplied by 3 and divided by two as a <see cref="float"/>.
        /// </summary>
        public const float ThreePiOver2 = 3 * Pi / 2;

        /// <summary>
        /// Defines the value of E as a <see cref="float"/>.
        /// </summary>
        public const float E = 2.71828182845904523536f;

        /// <summary>
        /// Defines the base-10 logarithm of E.
        /// </summary>
        public const float Log10E = 0.434294482f;

        /// <summary>
        /// Defines the base-2 logarithm of E.
        /// </summary>
        public const float Log2E = 1.442695041f;

        #endregion Fields

        #region Public Members

        #region Trigonometry

        /// <summary>
        /// Returns the Sin of the given value as float.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static float Sin(float val)
        {
            return (float)System.Math.Sin(val);
        }

        /// <summary>
        /// Returns the Cos of the given value as float.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static float Cos(float val)
        {
            return (float)System.Math.Cos(val);
        }

        #endregion Trigonometry

        #region NextPowerOfTwo

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static long NextPowerOfTwo(long n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Must be positive.");
            return (long)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(n, 2)));
        }

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static int NextPowerOfTwo(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Must be positive.");
            return (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(n, 2)));
        }

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static float NextPowerOfTwo(float n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Must be positive.");
            return (float)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(n, 2)));
        }

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static double NextPowerOfTwo(double n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Must be positive.");
            return System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(n, 2)));
        }

        /// <summary>
        /// Determines whether the specified value is a power of two.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(int val)
        {
            return (val & (val - 1)) == 0;
        }

        #endregion NextPowerOfTwo

        #region Factorial

        /// <summary>Calculates the factorial of a given natural number.
        /// </summary>
        /// <param name="n">The number.</param>
        /// <returns>n!</returns>
        public static long Factorial(int n)
        {
            long result = 1;

            for (; n > 1; n--)
                result *= n;

            return result;
        }

        #endregion Factorial

        #region BinomialCoefficient

        /// <summary>
        /// Calculates the binomial coefficient <paramref name="n"/> above <paramref name="k"/>.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="k">The k.</param>
        /// <returns>n! / (k! * (n - k)!)</returns>
        public static long BinomialCoefficient(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        #endregion BinomialCoefficient

        #region InverseSqrtFast

        /// <summary>
        /// Returns an approximation of the inverse square root of left number.
        /// </summary>
        /// <param name="x">A number.</param>
        /// <returns>An approximation of the inverse square root of the specified number, with an upper error bound of 0.001</returns>
        /// <remarks>
        /// This is an improved implementation of the the method known as Carmack's inverse square root
        /// which is found in the Quake III source code. This implementation comes from
        /// http://www.codemaestro.com/reviews/review00000105.html. For the history of this method, see
        /// http://www.beyond3d.com/content/articles/8/
        /// </remarks>
        public static float InverseSqrtFast(float x)
        {
            return (float)(1.0 / System.Math.Sqrt(x));
            /*
            unsafe
            {
                float xhalf = 0.5f * x;
                int i = *(int*)&x;              // Read bits as integer.
                i = 0x5f375a86 - (i >> 1);      // Make an initial guess for Newton-Raphson approximation
                x = *(float*)&i;                // Convert bits back to float
                x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.
                return x;
            }
            */
        }

        /// <summary>
        /// Returns an approximation of the inverse square root of left number.
        /// </summary>
        /// <param name="x">A number.</param>
        /// <returns>An approximation of the inverse square root of the specified number, with an upper error bound of 0.001</returns>
        /// <remarks>
        /// This is an improved implementation of the the method known as Carmack's inverse square root
        /// which is found in the Quake III source code. This implementation comes from
        /// http://www.codemaestro.com/reviews/review00000105.html. For the history of this method, see
        /// http://www.beyond3d.com/content/articles/8/
        /// </remarks>
        public static double InverseSqrtFast(double x)
        {
            return InverseSqrtFast((float)x);
            // TODO: The following code is wrong. Fix it, to improve precision.
#if false
            unsafe
            {
                double xhalf = 0.5f * x;
                int i = *(int*)&x;              // Read bits as integer.
                i = 0x5f375a86 - (i >> 1);      // Make an initial guess for Newton-Raphson approximation
                x = *(float*)&i;                // Convert bits back to float
                x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.
                return x;
            }
#endif
        }

        #endregion InverseSqrtFast

        #region DegreesToRadians

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees">An angle in degrees</param>
        /// <returns>The angle expressed in radians</returns>
        public static float DegreesToRadians(float degrees)
        {
            const float degToRad = (float)System.Math.PI / 180.0f;
            return degrees * degToRad;
        }

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="radians">An angle in radians</param>
        /// <returns>The angle expressed in degrees</returns>
        public static float RadiansToDegrees(float radians)
        {
            const float radToDeg = 180.0f / (float)System.Math.PI;
            return radians * radToDeg;
        }

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees">An angle in degrees</param>
        /// <returns>The angle expressed in radians</returns>
        public static double DegreesToRadiansD(double degrees)
        {
            const double degToRad = System.Math.PI / 180.0;
            return degrees * degToRad;
        }

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="radians">An angle in radians</param>
        /// <returns>The angle expressed in degrees</returns>
        public static double RadiansToDegreesD(double radians)
        {
            const double radToDeg = 180.0 / System.Math.PI;
            return radians * radToDeg;
        }

        #endregion DegreesToRadians

        #region Point cloud

        /// <summary>
        ///     Calculates the centroid point (mean) from given vertices in single precision.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static float3 CalculateCentroid(IEnumerable<float3> vertices)
        {
            var centroid = float3.Zero;

            foreach (var vert in vertices)
                centroid += vert;

            return centroid / vertices.Count();
        }

        /// <summary>
        ///     Calculates the centroid point (mean) from given vertices in double precision.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static double3 CalculateCentroid(IEnumerable<double3> vertices)
        {
            var centroid = double3.Zero;

            foreach (var vert in vertices)
                centroid += vert;

            return centroid / vertices.Count();
        }

        #endregion Point cloud

        #region Covariance

        /// <summary>
        ///     Generates a covariance matrix from given centroid and vertices in single precision.
        /// </summary>
        /// <param name="centroid">The centroid of the current mesh or point cloud.</param>
        /// <param name="vertices">The vertex data.</param>
        /// <returns></returns>
        public static float4x4 CreateCovarianceMatrix(float3 centroid, IEnumerable<float3> vertices)
        {
            var res = float4x4.Zero;
            var numPoints = vertices.Count();
            foreach (var vec in vertices)
            {
                res.M11 += Covariance(vec.x, vec.x, centroid.x, centroid.x, numPoints);
                res.M12 += Covariance(vec.x, vec.y, centroid.x, centroid.y, numPoints);
                res.M13 += Covariance(vec.x, vec.z, centroid.x, centroid.z, numPoints);

                res.M21 += Covariance(vec.y, vec.x, centroid.y, centroid.x, numPoints);
                res.M22 += Covariance(vec.y, vec.y, centroid.y, centroid.y, numPoints);
                res.M23 += Covariance(vec.y, vec.z, centroid.y, centroid.z, numPoints);

                res.M31 += Covariance(vec.x, vec.z, centroid.x, centroid.z, numPoints);
                res.M23 += Covariance(vec.y, vec.z, centroid.y, centroid.z, numPoints);
                res.M33 += Covariance(vec.z, vec.z, centroid.z, centroid.z, numPoints);
            }

            res.M44 = 1;

            return res;
        }

        /// <summary>
        ///     Generates a covariance matrix from given centroid and vertices in double precision.
        /// </summary>
        /// <param name="centroid">The centroid of the current mesh or point cloud.</param>
        /// <param name="vertices">The vertex data.</param>
        /// <returns></returns>
        public static double4x4 CreateCovarianceMatrix(double3 centroid, IEnumerable<double3> vertices)
        {
            var res = double4x4.Zero;
            var numPoints = vertices.Count();
            foreach (var vec in vertices)
            {
                res.M11 += Covariance(vec.x, vec.x, centroid.x, centroid.x, numPoints);
                res.M12 += Covariance(vec.x, vec.y, centroid.x, centroid.y, numPoints);
                res.M13 += Covariance(vec.x, vec.z, centroid.x, centroid.z, numPoints);

                res.M21 += Covariance(vec.y, vec.x, centroid.y, centroid.x, numPoints);
                res.M22 += Covariance(vec.y, vec.y, centroid.y, centroid.y, numPoints);
                res.M23 += Covariance(vec.y, vec.z, centroid.y, centroid.z, numPoints);

                res.M31 += Covariance(vec.x, vec.z, centroid.x, centroid.z, numPoints);
                res.M32 += Covariance(vec.y, vec.z, centroid.y, centroid.z, numPoints);
                res.M33 += Covariance(vec.z, vec.z, centroid.z, centroid.z, numPoints);
            }

            res.M44 = 1;

            return res;
        }

        private static float Covariance(float item1, float item2, float centroid1, float centroid2, int numberOfPoints)
        {
            return (item1 - centroid1) * (item2 - centroid2) / numberOfPoints;
        }

        private static double Covariance(double item1, double item2, double centroid1, double centroid2, int numberOfPoints)
        {
            return (item1 - centroid1) * (item2 - centroid2) / numberOfPoints;
        }

        #endregion Covariance

        #region MinAngle

        /// <summary>
        /// Wrap-around to keep angle in the interval of (-PI , +PI]
        /// </summary>
        /// <param name="angle">The angle to minimize.</param>
        /// <returns>The angle limited to a maximum magnitude of PI.</returns>
        public static float MinAngle(float angle)
        {
            while (angle > M.Pi)
                angle -= M.TwoPi;
            while (angle <= -M.Pi)
                angle += M.TwoPi;
            return angle;
        }

        /// <summary>
        /// Wrap-around to keep angle in the interval of (-PI , +PI]
        /// </summary>
        /// <param name="angle">The angle to minimize.</param>
        /// <returns>The angle limited to a maximum magnitude of PI.</returns>
        public static double MinAngle(double angle)
        {
            while (angle > System.Math.PI)
                angle -= 2 * System.Math.PI;
            while (angle <= -M.Pi)
                angle += 2 * System.Math.PI;
            return angle;
        }

        #endregion MinAngle

        #region Conversion

        /// <summary>
        /// Converts a float4 to an ABGR value (Int64).
        /// </summary>
        /// <param name="value">The float4 to convert.</param>
        /// <returns>The ABGR value.</returns>
        public static uint Float4ToABGR(float4 value)
        {
            var r = (uint)(255 * value.x);
            var g = (uint)(255 * value.y);
            var b = (uint)(255 * value.z);
            var a = (uint)(255 * value.w);

            return (a << 24) + (b << 16) + (g << 8) + r;
        }

        #endregion Conversion

        #region Swap

        /// <summary>
        /// Swaps two double values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        public static void Swap(ref double a, ref double b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Swaps two float values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        public static void Swap(ref float a, ref float b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        #endregion Swap

        #region Clamp

        /// <summary>
        /// Clamp a value to the given minimum and maximum vectors.
        /// </summary>
        /// <param name="val">Input value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>
        /// The clamped value.
        /// </returns>
        public static double Clamp(double val, double min, double max)
        {
            return val < min ? min : val > max ? max : val;
        }

        /// <summary>
        /// Clamp a value to the given minimum and maximum vectors.
        /// </summary>
        /// <param name="val">Input value.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>
        /// The clamped value.
        /// </returns>
        public static float Clamp(float val, float min, float max)
        {
            return val < min ? min : val > max ? max : val;
        }

        #endregion Clamp

        #region Interpolate / Lerp

        /// <summary>
        /// Smoothes out the interpolation t when approaching 0.0 and 1.0, using the smoothstep function.
        /// </summary>
        /// <param name="t">t will be clamped between 0.0 and 1.0.</param>
        /// <returns>Returns a value between 0.0 and 1.0, corresponding to t.</returns>
        public static float SmoothStep(float t)
        {
            t = Clamp(t, 0, 1);
            return t * t * (3f - 2f * t);
        }

        /// <summary>
        /// Smoothes out the interpolation t when approaching 0.0 and 1.0, using the smootherstep function.
        /// </summary>
        /// <param name="t">t will be clamped between 0.0 and 1.0.</param>
        /// <returns>Returns a value between 0.0 and 1.0, corresponding to t.</returns>
        public static float SmootherStep(float t)
        {
            t = Clamp(t, 0, 1);
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        /// <summary>
        /// Smoothes out the interpolation t when approaching 0.0 and 1.0, using the sinestep function.
        /// </summary>
        /// <param name="t">t will be clamped between 0.0 and 1.0.</param>
        /// <returns>Returns a value between 0.0 and 1.0, corresponding to t.</returns>
        public static float SineStep(float t)
        {
            t = Clamp(t, 0, 1);
            return 0.5f + (Sin((2 * t - 1) * (Pi / 2)) / 2);
        }

        /// <summary>
        /// Returns a new float value that is the linear blend of the 2 given floats
        /// </summary>
        /// <param name="a">First input float</param>
        /// <param name="b">Second input float</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>
        /// a when blend=0, b when blend=1, and a linear combination otherwise
        /// </returns>
        public static float Lerp(float a, float b, float blend)
        {
            blend = Clamp(blend, 0, 1);
            return (a * (1f - blend)) + (b * blend);
        }

        #endregion Interpolate / Lerp

        #region Equals

        /// <summary>
        /// Compares two double values for equality.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns>True if the numbers are equal.</returns>
        public static bool Equals(double a, double b)
        {
            return (System.Math.Abs(a - b) < EpsilonDouble);
        }

        /// <summary>
        /// Compares two float values for equality.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns>True if the numbers are equal.</returns>
        public static bool Equals(float a, float b)
        {
            return (System.Math.Abs(a - b) < EpsilonFloat);
        }

        #endregion Equals

        #region Step

        /// <summary>
        /// Generates a step function by comparing "val" to "edge".
        /// 0.0 is returned if "val" is smaller than "edge" and 1.0 is returned otherwise.
        /// </summary>
        /// <param name="edge">Specifies the location of the edge of the step function.</param>
        /// <param name="val">Specifies the value to be used to generate the step function.</param>
        /// <returns></returns>
        public static float Step(float edge, float val)
        {
            return val < edge ? 0.0f : 1.0f;
        }

        /// <summary>
        /// Generates a step function by comparing "val" to "edge".
        /// 0.0 is returned if "val" is smaller than "edge" and 1.0 is returned otherwise.
        /// </summary>
        /// <param name="edge">Specifies the location of the edge of the step function.</param>
        /// <param name="val">Specifies the value to be used to generate the step function.</param>
        /// <returns></returns>
        public static double Step(double edge, double val)
        {
            return val < edge ? 0.0 : 1.0;
        }

        #endregion

        #region ScreenToWorldPoint

        /// <summary>
        /// Calculates a world position from given screen point (e. g. mouse coordinates in window space coordinates)
        /// at any desired z coordinate specified by <paramref name="zPosition"/>
        /// </summary>
        /// <param name="windowCoordIn">Position in window space coordinates</param>
        /// <param name="zPosition">Desired z coordinate in clip space</param>
        /// <param name="Projection">Projection matrix</param>
        /// <param name="View">View matrix</param>
        /// <param name="InvProjection">Inverse Projection matrix</param>
        /// <param name="InvView">Inverse View matrix</param>
        /// <param name="windowWidth">Width of window</param>
        /// <param name="windowHeight">Height of window</param>
        /// <returns></returns>
        public static float3 ScreenPointToWorld(float2 windowCoordIn, float zPosition, float4x4 Projection, float4x4 View, float4x4 InvProjection, float4x4 InvView, int windowWidth, int windowHeight)
        {
            var oneInClipSpace = float4x4.TransformPerspective(Projection * View, new float4(0, 0, zPosition, 1));

            var pickPosClip = (windowCoordIn * new float2(2.0f / windowWidth, -2.0f / windowHeight)) + new float2(-1, 1);

            var vec = new float4(pickPosClip.x, pickPosClip.y, oneInClipSpace.z, 1f);

            var pos = float4x4.TransformPerspective(InvProjection, vec);
            pos = InvView * pos;

            return new float3(pos.x, pos.y, pos.z);
        }

        #endregion

        #endregion Public Members

        #region Internal Members

        internal static char GetNumericListSeparator(IFormatProvider provider)
        {
            char numericSeparator = ',';

            // Get the NumberFormatInfo out of the provider, if possible
            // If the IFormatProvider doesn't not contain a NumberFormatInfo, then
            // this method returns the current culture's NumberFormatInfo.
            NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(provider);

            // Is the decimal separator is the same as the list separator?
            // If so, we use the ";".
            if ((numberFormat.NumberDecimalSeparator.Length > 0) && (numericSeparator == numberFormat.NumberDecimalSeparator[0]))
            {
                numericSeparator = ';';
            }

            return numericSeparator;
        }

        #endregion Internal Members
    }
}