using System;

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

        #endregion

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
        /// Defines the value of Pi as a <see cref="System.Single"/>.
        /// </summary>
        public const float Pi = 3.14159265358979f;

        /// <summary>
        /// Defines the value of Pi divided by two as a <see cref="System.Single"/>.
        /// </summary>
        public const float PiOver2 = Pi / 2;

        /// <summary>
        /// Defines the value of Pi divided by three as a <see cref="System.Single"/>.
        /// </summary>
        public const float PiOver3 = Pi / 3;

        /// <summary>
        /// Defines the value of  Pi divided by four as a <see cref="System.Single"/>.
        /// </summary>
        public const float PiOver4 = Pi / 4;

        /// <summary>
        /// Defines the value of Pi divided by six as a <see cref="System.Single"/>.
        /// </summary>
        public const float PiOver6 = Pi / 6;

        /// <summary>
        /// Defines the value of Pi multiplied by two as a <see cref="System.Single"/>.
        /// </summary>
        public const float TwoPi = 2 *Pi;

        /// <summary>
        /// Defines the value of Pi multiplied by 3 and divided by two as a <see cref="System.Single"/>.
        /// </summary>
        public const float ThreePiOver2 = 3 *Pi / 2;

        /// <summary>
        /// Defines the value of E as a <see cref="System.Single"/>.
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

        #endregion

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

        #endregion

        #region NextPowerOfTwo

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static long NextPowerOfTwo(long n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n", "Must be positive.");
            return (long)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
        }

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static int NextPowerOfTwo(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n", "Must be positive.");
            return (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
        }

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static float NextPowerOfTwo(float n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n", "Must be positive.");
            return (float)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
        }

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static double NextPowerOfTwo(double n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n", "Must be positive.");
            return System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
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

        #endregion

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

        #endregion

        #region BinomialCoefficient

        /// <summary>
        /// Calculates the binomial coefficient <paramref name="n"/> above <paramref name="k"/>.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="k">The k.</param>
        /// <returns>n! / (k! * (n - k)!)</returns>
        public static long BinomialCoefficient(int n, int k)
        {
            return Factorial(n) / (Factorial(k) *Factorial(n - k));
        }

        #endregion

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

        #endregion

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
            const double degToRad = System.Math.PI / 180.0f;
            return degrees * degToRad;
        }

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="radians">An angle in radians</param>
        /// <returns>The angle expressed in degrees</returns>
        public static double RadiansToDegreesD(double radians)
        {
            const double radToDeg = 180.0f / System.Math.PI;
            return radians * radToDeg;
        }

        #endregion




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

        #endregion

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

        #endregion

        #region Swap

        /// <summary>
        /// Swaps two double values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        public static void Swap(ref double a, ref double b)
        {
            double temp = a;
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
            float temp = a;
            a = b;
            b = temp;
        }

        #endregion

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

        #endregion

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

        #endregion

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

        #endregion

        #endregion
    }
}