using System;
using System.Runtime.InteropServices;
namespace Fusee.Math
{
    /// <summary>
    /// Represents a 2D vector using two double-precision floating-point numbers.
    /// </summary>
    /// <remarks>
    /// The double2 structure is suitable for interoperation with unmanaged code requiring two consecutive doubles.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct double2 : IEquatable<double2>
    {
        #region Fields

        /// <summary>
        /// The x component of the double2.
        /// </summary>
        public double x;

        /// <summary>
        /// The y component of the double2.
        /// </summary>
        public double y;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new double2.
        /// </summary>
        /// <param name="x">The x coordinate of the net double2.</param>
        /// <param name="y">The y coordinate of the net double2.</param>
        public double2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Constructs a new double2 from the given double2.
        /// </summary>
        /// <param name="v">The double2 to copy components from.</param>
        [Obsolete]
        public double2(double2 v)
        {
            x = v.x;
            y = v.y;
        }

        /// <summary>
        /// Constructs a new double2 from the given double3.
        /// </summary>
        /// <param name="v">The double3 to copy components from. Z is discarded.</param>
        [Obsolete]
        public double2(double3 v)
        {
            x = v.x;
            y = v.y;
        }

        /// <summary>
        /// Constructs a new double2 from the given double4.
        /// </summary>
        /// <param name="v">The double4 to copy components from. Z and W are discarded.</param>
        [Obsolete]
        public double2(double4 v)
        {
            x = v.x;
            y = v.y;
        }

        #endregion

        #region Public Members

        #region Instance

        #region public void Add()

        /// <summary>
        /// Add the Vector passed as parameter to this instance.
        /// </summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        [Obsolete("Use static Add() method instead.")]
        public void Add(double2 right)
        {
            this.x += right.x;
            this.y += right.y;
        }

        /// <summary>
        /// Add the Vector passed as parameter to this instance.
        /// </summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        [Obsolete("Use static Add() method instead.")]
        public void Add(ref double2 right)
        {
            this.x += right.x;
            this.y += right.y;
        }

        #endregion public void Add()

        #region public void Sub()

        /// <summary>
        /// Subtract the Vector passed as parameter from this instance.
        /// </summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        [Obsolete("Use static Subtract() method instead.")]
        public void Sub(double2 right)
        {
            this.x -= right.x;
            this.y -= right.y;
        }

        /// <summary>
        /// Subtract the Vector passed as parameter from this instance.
        /// </summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        [Obsolete("Use static Subtract() method instead.")]
        public void Sub(ref double2 right)
        {
            this.x -= right.x;
            this.y -= right.y;
        }

        #endregion public void Sub()

        #region public void Mult()

        /// <summary>
        /// Multiply this instance by a scalar.
        /// </summary>
        /// <param name="f">Scalar operand.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Mult(double f)
        {
            this.x *= f;
            this.y *= f;
        }

        #endregion public void Mult()

        #region public void Div()

        /// <summary>
        /// Divide this instance by a scalar.
        /// </summary>
        /// <param name="f">Scalar operand.</param>
        [Obsolete("Use static Divide() method instead.")]
        public void Div(double f)
        {
            double mult = 1.0f / f;
            this.x *= mult;
            this.y *= mult;
        }

        #endregion public void Div()

        #region public double Length

        /// <summary>
        /// Gets the length (magnitude) of the vector.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        /// <see cref="LengthFast" />
        ///   <seealso cref="LengthSquared" />
        public double Length
        {
            get
            {
                return (double)System.Math.Sqrt(x * x + y * y);
            }
        }

        #endregion

        #region public double LengthFast

        /// <summary>
        /// Gets an approximation of the vector length (magnitude).
        /// </summary>
        /// <value>
        /// The length fast.
        /// </value>
        /// <see cref="Length" />
        ///   <seealso cref="LengthSquared" />
        /// <remarks>
        /// This property uses an approximation of the square root function to calculate vector magnitude, with
        /// an upper error bound of 0.001.
        /// </remarks>
        public double LengthFast
        {
            get
            {
                return 1.0f / MathHelper.InverseSqrtFast(x * x + y * y);
            }
        }

        #endregion

        #region public double LengthSquared

        /// <summary>
        /// Gets the square of the vector length (magnitude).
        /// </summary>
        /// <value>
        /// The length squared.
        /// </value>
        /// <see cref="Length" />
        ///   <seealso cref="LengthFast" />
        /// <remarks>
        /// This property avoids the costly square root operation required by the Length property. This makes it more suitable
        /// for comparisons.
        /// </remarks>
        public double LengthSquared
        {
            get
            {
                return x * x + y * y;
            }
        }

        #endregion

        #region public double2 PerpendicularRight

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        /// <value>
        /// The perpendicular right.
        /// </value>
        public double2 PerpendicularRight
        {
            get
            {
                return new double2(y, -x);
            }
        }

        #endregion

        #region public double2 PerpendicularLeft

        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        /// <value>
        /// The perpendicular left.
        /// </value>
        public double2 PerpendicularLeft
        {
            get
            {
                return new double2(-y, x);
            }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the double2 to unit length.
        /// </summary>
        public void Normalize()
        {
            double scale = 1.0f / this.Length;
            x *= scale;
            y *= scale;
        }

        #endregion

        #region public void NormalizeFast()

        /// <summary>
        /// Scales the double2 to approximately unit length.
        /// </summary>
        public void NormalizeFast()
        {
            double scale = MathHelper.InverseSqrtFast(x * x + y * y);
            x *= scale;
            y *= scale;
        }

        #endregion

        #region public void Scale()

        /// <summary>
        /// Scales the current double2 by the given amounts.
        /// </summary>
        /// <param name="sx">The scale of the x component.</param>
        /// <param name="sy">The scale of the y component.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Scale(double sx, double sy)
        {
            this.x = x * sx;
            this.y = y * sy;
        }

        /// <summary>
        /// Scales this instance by the given parameter.
        /// </summary>
        /// <param name="scale">The scaling of the individual components.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Scale(double2 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }

        /// <summary>
        /// Scales this instance by the given parameter.
        /// </summary>
        /// <param name="scale">The scaling of the individual components.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Scale(ref double2 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }

        #endregion public void Scale()

        /// <summary>
        /// Returns an array of doubles with the two components of the vector.
        /// </summary>
        /// <returns>Returns an array of doubles with the two components of the vector.</returns>
        public double[] ToArray()
        {
            return new double[] { x, y };
        }


        #endregion

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length double2 that points towards the x-axis.
        /// </summary>
        public static readonly double2 UnitX = new double2(1, 0);

        /// <summary>
        /// Defines a unit-length double2 that points towards the y-axis.
        /// </summary>
        public static readonly double2 UnitY = new double2(0, 1);

        /// <summary>
        /// Defines a zero-length double2.
        /// </summary>
        public static readonly double2 Zero = new double2(0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly double2 One = new double2(1, 1);

        // <summary>
        // Defines the size of the double2 struct in bytes.
        // </summary>
        // public static readonly int SizeInBytes = Marshal.SizeOf(new double2());

        #endregion

        #region Obsolete

        #region Sub

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>
        /// Result of subtraction
        /// </returns>
        [Obsolete("Use static Subtract() method instead.")]
        public static double2 Sub(double2 a, double2 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            return a;
        }

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">Result of subtraction</param>
        [Obsolete("Use static Subtract() method instead.")]
        public static void Sub(ref double2 a, ref double2 b, out double2 result)
        {
            result.x = a.x - b.x;
            result.y = a.y - b.y;
        }

        #endregion

        #region Mult

        /// <summary>
        /// Multiply a vector and a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <returns>
        /// Result of the multiplication
        /// </returns>
        [Obsolete("Use static Multiply() method instead.")]
        public static double2 Mult(double2 a, double f)
        {
            a.x *= f;
            a.y *= f;
            return a;
        }

        /// <summary>
        /// Multiply a vector and a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <param name="result">Result of the multiplication</param>
        [Obsolete("Use static Multiply() method instead.")]
        public static void Mult(ref double2 a, double f, out double2 result)
        {
            result.x = a.x * f;
            result.y = a.y * f;
        }

        #endregion

        #region Div

        /// <summary>
        /// Divide a vector by a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <returns>
        /// Result of the division
        /// </returns>
        [Obsolete("Use static Divide() method instead.")]
        public static double2 Div(double2 a, double f)
        {
            double mult = 1.0f / f;
            a.x *= mult;
            a.y *= mult;
            return a;
        }

        /// <summary>
        /// Divide a vector by a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <param name="result">Result of the division</param>
        [Obsolete("Use static Divide() method instead.")]
        public static void Div(ref double2 a, double f, out double2 result)
        {
            double mult = 1.0f / f;
            result.x = a.x * mult;
            result.y = a.y * mult;
        }

        #endregion

        #endregion

        #region Add

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <returns>
        /// Result of operation.
        /// </returns>
        public static double2 Add(double2 a, double2 b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <param name="result">Result of operation.</param>
        public static void Add(ref double2 a, ref double2 b, out double2 result)
        {
            result = new double2(a.x + b.x, a.y + b.y);
        }

        #endregion

        #region Subtract

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>
        /// Result of subtraction
        /// </returns>
        public static double2 Subtract(double2 a, double2 b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">Result of subtraction</param>
        public static void Subtract(ref double2 a, ref double2 b, out double2 result)
        {
            result = new double2(a.x - b.x, a.y - b.y);
        }

        #endregion

        #region Multiply

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public static double2 Multiply(double2 vector, double scale)
        {
            Multiply(ref vector, scale, out vector);
            return vector;
        }

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(ref double2 vector, double scale, out double2 result)
        {
            result = new double2(vector.x * scale, vector.y * scale);
        }

        /// <summary>
        /// Multiplies a vector by the components a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public static double2 Multiply(double2 vector, double2 scale)
        {
            Multiply(ref vector, ref scale, out vector);
            return vector;
        }

        /// <summary>
        /// Multiplies a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(ref double2 vector, ref double2 scale, out double2 result)
        {
            result = new double2(vector.x * scale.x, vector.y * scale.y);
        }

        #endregion

        #region Divide

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public static double2 Divide(double2 vector, double scale)
        {
            Divide(ref vector, scale, out vector);
            return vector;
        }

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(ref double2 vector, double scale, out double2 result)
        {
            Multiply(ref vector, 1 / scale, out result);
        }

        /// <summary>
        /// Divides a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public static double2 Divide(double2 vector, double2 scale)
        {
            Divide(ref vector, ref scale, out vector);
            return vector;
        }

        /// <summary>
        /// Divide a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(ref double2 vector, ref double2 scale, out double2 result)
        {
            result = new double2(vector.x / scale.x, vector.y / scale.y);
        }

        #endregion

        #region ComponentMin

        /// <summary>
        /// Calculate the component-wise minimum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>
        /// The component-wise minimum
        /// </returns>
        public static double2 ComponentMin(double2 a, double2 b)
        {
            a.x = a.x < b.x ? a.x : b.x;
            a.y = a.y < b.y ? a.y : b.y;
            return a;
        }

        /// <summary>
        /// Calculate the component-wise minimum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">The component-wise minimum</param>
        public static void ComponentMin(ref double2 a, ref double2 b, out double2 result)
        {
            result.x = a.x < b.x ? a.x : b.x;
            result.y = a.y < b.y ? a.y : b.y;
        }

        #endregion

        #region ComponentMax

        /// <summary>
        /// Calculate the component-wise maximum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>
        /// The component-wise maximum
        /// </returns>
        public static double2 ComponentMax(double2 a, double2 b)
        {
            a.x = a.x > b.x ? a.x : b.x;
            a.y = a.y > b.y ? a.y : b.y;
            return a;
        }

        /// <summary>
        /// Calculate the component-wise maximum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">The component-wise maximum</param>
        public static void ComponentMax(ref double2 a, ref double2 b, out double2 result)
        {
            result.x = a.x > b.x ? a.x : b.x;
            result.y = a.y > b.y ? a.y : b.y;
        }

        #endregion

        #region Min

        /// <summary>
        /// Returns the double3 with the minimum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>
        /// The minimum double3
        /// </returns>
        public static double2 Min(double2 left, double2 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        #endregion

        #region Max

        /// <summary>
        /// Returns the double3 with the minimum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>
        /// The minimum double3
        /// </returns>
        public static double2 Max(double2 left, double2 right)
        {
            return left.LengthSquared >= right.LengthSquared ? left : right;
        }

        #endregion

        #region Clamp

        /// <summary>
        /// Clamp a vector to the given minimum and maximum vectors
        /// </summary>
        /// <param name="vec">Input vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <returns>
        /// The clamped vector
        /// </returns>
        public static double2 Clamp(double2 vec, double2 min, double2 max)
        {
            vec.x = vec.x < min.x ? min.x : vec.x > max.x ? max.x : vec.x;
            vec.y = vec.y < min.y ? min.y : vec.y > max.y ? max.y : vec.y;
            return vec;
        }

        /// <summary>
        /// Clamp a vector to the given minimum and maximum vectors
        /// </summary>
        /// <param name="vec">Input vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <param name="result">The clamped vector</param>
        public static void Clamp(ref double2 vec, ref double2 min, ref double2 max, out double2 result)
        {
            result.x = vec.x < min.x ? min.x : vec.x > max.x ? max.x : vec.x;
            result.y = vec.y < min.y ? min.y : vec.y > max.y ? max.y : vec.y;
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Scale a vector to unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>
        /// The normalized vector
        /// </returns>
        public static double2 Normalize(double2 vec)
        {
            double scale = 1.0f / vec.Length;
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        /// <summary>
        /// Scale a vector to unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <param name="result">The normalized vector</param>
        public static void Normalize(ref double2 vec, out double2 result)
        {
            double scale = 1.0f / vec.Length;
            result.x = vec.x * scale;
            result.y = vec.y * scale;
        }

        #endregion

        #region NormalizeFast

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>
        /// The normalized vector
        /// </returns>
        public static double2 NormalizeFast(double2 vec)
        {
            double scale = MathHelper.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y);
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <param name="result">The normalized vector</param>
        public static void NormalizeFast(ref double2 vec, out double2 result)
        {
            double scale = MathHelper.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y);
            result.x = vec.x * scale;
            result.y = vec.y * scale;
        }

        #endregion

        #region Dot

        /// <summary>
        /// Calculate the dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>
        /// The dot product of the two inputs
        /// </returns>
        public static double Dot(double2 left, double2 right)
        {
            return left.x * right.x + left.y * right.y;
        }

        /// <summary>
        /// Calculate the dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <param name="result">The dot product of the two inputs</param>
        public static void Dot(ref double2 left, ref double2 right, out double result)
        {
            result = left.x * right.x + left.y * right.y;
        }

        #endregion

        #region Lerp

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>
        /// a when blend=0, b when blend=1, and a linear combination otherwise
        /// </returns>
        public static double2 Lerp(double2 a, double2 b, double blend)
        {
            a.x = blend * (b.x - a.x) + a.x;
            a.y = blend * (b.y - a.y) + a.y;
            return a;
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
        public static void Lerp(ref double2 a, ref double2 b, double blend, out double2 result)
        {
            result.x = blend * (b.x - a.x) + a.x;
            result.y = blend * (b.y - a.y) + a.y;
        }

        #endregion

        #region Barycentric

        /// <summary>
        /// Interpolate 3 Vectors using Barycentric coordinates
        /// </summary>
        /// <param name="a">First input Vector</param>
        /// <param name="b">Second input Vector</param>
        /// <param name="c">Third input Vector</param>
        /// <param name="u">First Barycentric Coordinate</param>
        /// <param name="v">Second Barycentric Coordinate</param>
        /// <returns>
        /// a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise
        /// </returns>
        public static double2 BaryCentric(double2 a, double2 b, double2 c, double u, double v)
        {
            return a + u * (b - a) + v * (c - a);
        }

        /// <summary>
        /// Interpolate 3 Vectors using Barycentric coordinates
        /// </summary>
        /// <param name="a">First input Vector.</param>
        /// <param name="b">Second input Vector.</param>
        /// <param name="c">Third input Vector.</param>
        /// <param name="u">First Barycentric Coordinate.</param>
        /// <param name="v">Second Barycentric Coordinate.</param>
        /// <param name="result">Output Vector. a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</param>
        public static void BaryCentric(ref double2 a, ref double2 b, ref double2 c, double u, double v, out double2 result)
        {
            result = a; // copy

            double2 temp = b; // copy
            Subtract(ref temp, ref a, out temp);
            Multiply(ref temp, u, out temp);
            Add(ref result, ref temp, out result);

            temp = c; // copy
            Subtract(ref temp, ref a, out temp);
            Multiply(ref temp, v, out temp);
            Add(ref result, ref temp, out result);
        }

        #endregion

        #region Transform

        /// <summary>
        /// Transforms a vector by a QuaternionD rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The QuaternionD to rotate the vector by.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static double2 Transform(double2 vec, QuaternionD quat)
        {
            double2 result;
            Transform(ref vec, ref quat, out result);
            return result;
        }

        /// <summary>
        /// Transforms a vector by a QuaternionD rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The QuaternionD to rotate the vector by.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Transform(ref double2 vec, ref QuaternionD quat, out double2 result)
        {
            QuaternionD v = new QuaternionD(vec.x, vec.y, 0, 0), i, t;
            QuaternionD.Invert(ref quat, out i);
            QuaternionD.Multiply(ref quat, ref v, out t);
            QuaternionD.Multiply(ref t, ref i, out v);

            result = new double2(v.x, v.y);
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Adds the specified instances.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>
        /// Result of addition.
        /// </returns>
        public static double2 operator +(double2 left, double2 right)
        {
            left.x += right.x;
            left.y += right.y;
            return left;
        }

        /// <summary>
        /// Subtracts the specified instances.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>
        /// Result of subtraction.
        /// </returns>
        public static double2 operator -(double2 left, double2 right)
        {
            left.x -= right.x;
            left.y -= right.y;
            return left;
        }

        /// <summary>
        /// Negates the specified instance.
        /// </summary>
        /// <param name="vec">Operand.</param>
        /// <returns>
        /// Result of negation.
        /// </returns>
        public static double2 operator -(double2 vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            return vec;
        }

        /// <summary>
        /// Multiplies the specified instance by a scalar.
        /// </summary>
        /// <param name="vec">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of multiplication.
        /// </returns>
        public static double2 operator *(double2 vec, double scale)
        {
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        /// <summary>
        /// Multiplies the specified instance by a scalar.
        /// </summary>
        /// <param name="scale">Left operand.</param>
        /// <param name="vec">Right operand.</param>
        /// <returns>
        /// Result of multiplication.
        /// </returns>
        public static double2 operator *(double scale, double2 vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        /// <summary>
        /// Divides the specified instance by a scalar.
        /// </summary>
        /// <param name="vec">Left operand</param>
        /// <param name="scale">Right operand</param>
        /// <returns>
        /// Result of the division.
        /// </returns>
        public static double2 operator /(double2 vec, double scale)
        {
            double mult = 1.0f / scale;
            vec.x *= mult;
            vec.y *= mult;
            return vec;
        }

        /// <summary>
        /// Compares the specified instances for equality.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>
        /// True if both instances are equal; false otherwise.
        /// </returns>
        public static bool operator ==(double2 left, double2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares the specified instances for inequality.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>
        /// True if both instances are not equal; false otherwise.
        /// </returns>
        public static bool operator !=(double2 left, double2 right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current double2.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("({0}, {1})", x, y);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>
        /// A System.Int32 containing the unique hashcode for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>
        /// True if the instances are equal; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is double2))
                return false;

            return this.Equals((double2)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<double2> Members

        /// <summary>
        /// Indicates whether the current vector is equal to another vector.
        /// </summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>
        /// true if the current vector is equal to the vector parameter; otherwise, false.
        /// </returns>
        public bool Equals(double2 other)
        {
            return
                x == other.x &&
                y == other.y;
        }

        #endregion

        public static Converter<string, double2> Parse { get ; set; }
    }
}
