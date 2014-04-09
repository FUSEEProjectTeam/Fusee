using System;
using System.Runtime.InteropServices;
namespace Fusee.Math
{
    /// <summary>
    /// Represents a 3D vector using three double-precision floating-point numbers.
    /// </summary>
    /// <remarks>
    /// The double3 structure is suitable for interoperation with unmanaged code requiring three consecutive doubles.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct double3 : IEquatable<double3>
    {
        #region Fields

        /// <summary>
        /// The x component of the double3.
        /// </summary>
        public double x;

        /// <summary>
        /// The y component of the double3.
        /// </summary>
        public double y;

        /// <summary>
        /// The z component of the double3.
        /// </summary>
        public double z;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new double3.
        /// </summary>
        /// <param name="x">The x component of the double3.</param>
        /// <param name="y">The y component of the double3.</param>
        /// <param name="z">The z component of the double3.</param>
        public double3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Constructs a new double3 from the given double2.
        /// </summary>
        /// <param name="v">The double2 to copy components from.</param>
        public double3(double2 v)
        {
            x = v.x;
            y = v.y;
            z = 0.0f;
        }

        /// <summary>
        /// Constructs a new double3 from the given double3.
        /// </summary>
        /// <param name="v">The double3 to copy components from.</param>
        public double3(double3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        /// <summary>
        /// Constructs a new double3 from the given double4.
        /// </summary>
        /// <param name="v">The double4 to copy components from.</param>
        public double3(double4 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
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
        public void Add(double3 right)
        {
            this.x += right.x;
            this.y += right.y;
            this.z += right.z;
        }

        /// <summary>
        /// Add the Vector passed as parameter to this instance.
        /// </summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        [Obsolete("Use static Add() method instead.")]
        public void Add(ref double3 right)
        {
            this.x += right.x;
            this.y += right.y;
            this.z += right.z;
        }

        #endregion public void Add()

        #region public void Sub()

        /// <summary>
        /// Subtract the Vector passed as parameter from this instance.
        /// </summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        [Obsolete("Use static Subtract() method instead.")]
        public void Sub(double3 right)
        {
            this.x -= right.x;
            this.y -= right.y;
            this.z -= right.z;
        }

        /// <summary>
        /// Subtract the Vector passed as parameter from this instance.
        /// </summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        [Obsolete("Use static Subtract() method instead.")]
        public void Sub(ref double3 right)
        {
            this.x -= right.x;
            this.y -= right.y;
            this.z -= right.z;
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
            this.z *= f;
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
            this.z *= mult;
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
                return (double)System.Math.Sqrt(x * x + y * y + z * z);
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
                return 1.0f / MathHelper.InverseSqrtFast(x * x + y * y + z * z);
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
                return x * x + y * y + z * z;
            }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the double3 to unit length.
        /// </summary>
        public void Normalize()
        {
            double scale = 1.0f / this.Length;
            x *= scale;
            y *= scale;
            z *= scale;
        }

        #endregion

        #region public void NormalizeFast()

        /// <summary>
        /// Scales the double3 to approximately unit length.
        /// </summary>
        public void NormalizeFast()
        {
            double scale = MathHelper.InverseSqrtFast(x * x + y * y + z * z);
            x *= scale;
            y *= scale;
            z *= scale;
        }

        #endregion

        #region public void Scale()

        /// <summary>
        /// Scales the current double3 by the given amounts.
        /// </summary>
        /// <param name="sx">The scale of the x component.</param>
        /// <param name="sy">The scale of the y component.</param>
        /// <param name="sz">The scale of the z component.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Scale(double sx, double sy, double sz)
        {
            this.x = x * sx;
            this.y = y * sy;
            this.z = z * sz;
        }

        /// <summary>
        /// Scales this instance by the given parameter.
        /// </summary>
        /// <param name="scale">The scaling of the individual components.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Scale(double3 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        /// <summary>
        /// Scales this instance by the given parameter.
        /// </summary>
        /// <param name="scale">The scaling of the individual components.</param>
         [Obsolete("Use static Multiply() method instead.")]
        public void Scale(ref double3 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        #endregion public void Scale()

         /// <summary>
         /// Returns an array of doubles with the three components of the vector.
         /// </summary>
         /// <returns>Returns an array of doubles with the three components of the vector.</returns>
         public double[] ToArray()
         {
             return new double[] { x, y, z };
         }


        #endregion

        #region Static

        #region Fields

         /// <summary>
         /// Defines a unit-length double3 that points towards the x-axis.
         /// </summary>
        public static readonly double3 UnitX = new double3(1, 0, 0);

        /// <summary>
        /// Defines a unit-length double3 that points towards the y-axis.
        /// </summary>
        public static readonly double3 UnitY = new double3(0, 1, 0);

        /// <summary>
        /// Defines a unit-length double3 that points towards the z-axis.
        /// </summary>
        public static readonly double3 UnitZ = new double3(0, 0, 1);

        /// <summary>
        /// Defines a zero-length double3.
        /// </summary>
        public static readonly double3 Zero = new double3(0, 0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly double3 One = new double3(1, 1, 1);

        // <summary>
        // Defines the size of the double3 struct in bytes.
        // </summary>
        // public static readonly int SizeInBytes = Marshal.SizeOf(new double3());

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
        public static double3 Sub(double3 a, double3 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            a.z -= b.z;
            return a;
        }

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">Result of subtraction</param>
        [Obsolete("Use static Subtract() method instead.")]
        public static void Sub(ref double3 a, ref double3 b, out double3 result)
        {
            result.x = a.x - b.x;
            result.y = a.y - b.y;
            result.z = a.z - b.z;
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
        public static double3 Mult(double3 a, double f)
        {
            a.x *= f;
            a.y *= f;
            a.z *= f;
            return a;
        }

        /// <summary>
        /// Multiply a vector and a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <param name="result">Result of the multiplication</param>
        [Obsolete("Use static Multiply() method instead.")]
        public static void Mult(ref double3 a, double f, out double3 result)
        {
            result.x = a.x * f;
            result.y = a.y * f;
            result.z = a.z * f;
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
        public static double3 Div(double3 a, double f)
        {
            double mult = 1.0f / f;
            a.x *= mult;
            a.y *= mult;
            a.z *= mult;
            return a;
        }

        /// <summary>
        /// Divide a vector by a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <param name="result">Result of the division</param>
        [Obsolete("Use static Divide() method instead.")]
        public static void Div(ref double3 a, double f, out double3 result)
        {
            double mult = 1.0f / f;
            result.x = a.x * mult;
            result.y = a.y * mult;
            result.z = a.z * mult;
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
        public static double3 Add(double3 a, double3 b)
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
        public static void Add(ref double3 a, ref double3 b, out double3 result)
        {
            result = new double3(a.x + b.x, a.y + b.y, a.z + b.z);
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
        public static double3 Subtract(double3 a, double3 b)
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
        public static void Subtract(ref double3 a, ref double3 b, out double3 result)
        {
            result = new double3(a.x - b.x, a.y - b.y, a.z - b.z);
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
        public static double3 Multiply(double3 vector, double scale)
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
        public static void Multiply(ref double3 vector, double scale, out double3 result)
        {
            result = new double3(vector.x * scale, vector.y * scale, vector.z * scale);
        }

        /// <summary>
        /// Multiplies a vector by the components a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public static double3 Multiply(double3 vector, double3 scale)
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
        public static void Multiply(ref double3 vector, ref double3 scale, out double3 result)
        {
            result = new double3(vector.x * scale.x, vector.y * scale.y, vector.z * scale.z);
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
        public static double3 Divide(double3 vector, double scale)
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
        public static void Divide(ref double3 vector, double scale, out double3 result)
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
        public static double3 Divide(double3 vector, double3 scale)
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
        public static void Divide(ref double3 vector, ref double3 scale, out double3 result)
        {
            result = new double3(vector.x / scale.x, vector.y / scale.y, vector.z / scale.z);
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
        public static double3 ComponentMin(double3 a, double3 b)
        {
            a.x = a.x < b.x ? a.x : b.x;
            a.y = a.y < b.y ? a.y : b.y;
            a.z = a.z < b.z ? a.z : b.z;
            return a;
        }

        /// <summary>
        /// Calculate the component-wise minimum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">The component-wise minimum</param>
        public static void ComponentMin(ref double3 a, ref double3 b, out double3 result)
        {
            result.x = a.x < b.x ? a.x : b.x;
            result.y = a.y < b.y ? a.y : b.y;
            result.z = a.z < b.z ? a.z : b.z;
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
        public static double3 ComponentMax(double3 a, double3 b)
        {
            a.x = a.x > b.x ? a.x : b.x;
            a.y = a.y > b.y ? a.y : b.y;
            a.z = a.z > b.z ? a.z : b.z;
            return a;
        }

        /// <summary>
        /// Calculate the component-wise maximum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">The component-wise maximum</param>
        public static void ComponentMax(ref double3 a, ref double3 b, out double3 result)
        {
            result.x = a.x > b.x ? a.x : b.x;
            result.y = a.y > b.y ? a.y : b.y;
            result.z = a.z > b.z ? a.z : b.z;
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
        public static double3 Min(double3 left, double3 right)
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
        public static double3 Max(double3 left, double3 right)
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
        public static double3 Clamp(double3 vec, double3 min, double3 max)
        {
            vec.x = vec.x < min.x ? min.x : vec.x > max.x ? max.x : vec.x;
            vec.y = vec.y < min.y ? min.y : vec.y > max.y ? max.y : vec.y;
            vec.z = vec.z < min.z ? min.z : vec.z > max.z ? max.z : vec.z;
            return vec;
        }

        /// <summary>
        /// Clamp a vector to the given minimum and maximum vectors
        /// </summary>
        /// <param name="vec">Input vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <param name="result">The clamped vector</param>
        public static void Clamp(ref double3 vec, ref double3 min, ref double3 max, out double3 result)
        {
            result.x = vec.x < min.x ? min.x : vec.x > max.x ? max.x : vec.x;
            result.y = vec.y < min.y ? min.y : vec.y > max.y ? max.y : vec.y;
            result.z = vec.z < min.z ? min.z : vec.z > max.z ? max.z : vec.z;
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
        public static double3 Normalize(double3 vec)
        {
            double scale = 1.0f / vec.Length;
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            return vec;
        }

        /// <summary>
        /// Scale a vector to unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <param name="result">The normalized vector</param>
        public static void Normalize(ref double3 vec, out double3 result)
        {
            double scale = 1.0f / vec.Length;
            result.x = vec.x * scale;
            result.y = vec.y * scale;
            result.z = vec.z * scale;
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
        public static double3 NormalizeFast(double3 vec)
        {
            double scale = MathHelper.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            return vec;
        }

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <param name="result">The normalized vector</param>
        public static void NormalizeFast(ref double3 vec, out double3 result)
        {
            double scale = MathHelper.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
            result.x = vec.x * scale;
            result.y = vec.y * scale;
            result.z = vec.z * scale;
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
        public static double Dot(double3 left, double3 right)
        {
            return left.x * right.x + left.y * right.y + left.z * right.z;
        }

        /// <summary>
        /// Calculate the dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <param name="result">The dot product of the two inputs</param>
        public static void Dot(ref double3 left, ref double3 right, out double result)
        {
            result = left.x * right.x + left.y * right.y + left.z * right.z;
        }

        #endregion

        #region Cross

        /// <summary>
        /// Caclulate the cross (vector) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>
        /// The cross product of the two inputs
        /// </returns>
        public static double3 Cross(double3 left, double3 right)
        {
            double3 result;
            Cross(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Caclulate the cross (vector) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <param name="result">The cross product of the two inputs</param>
        public static void Cross(ref double3 left, ref double3 right, out double3 result)
        {
            result = new double3(left.y * right.z - left.z * right.y,
                left.z * right.x - left.x * right.z,
                left.x * right.y - left.y * right.x);
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
        public static double3 Lerp(double3 a, double3 b, double blend)
        {
            a.x = blend * (b.x - a.x) + a.x;
            a.y = blend * (b.y - a.y) + a.y;
            a.z = blend * (b.z - a.z) + a.z;
            return a;
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
        public static void Lerp(ref double3 a, ref double3 b, double blend, out double3 result)
        {
            result.x = blend * (b.x - a.x) + a.x;
            result.y = blend * (b.y - a.y) + a.y;
            result.z = blend * (b.z - a.z) + a.z;
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
        public static double3 BaryCentric(double3 a, double3 b, double3 c, double u, double v)
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
        public static void BaryCentric(ref double3 a, ref double3 b, ref double3 c, double u, double v, out double3 result)
        {
            result = a; // copy

            double3 temp = b; // copy
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
        /// Transform a direction vector by the given Matrix
        /// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>
        /// The transformed vector
        /// </returns>
        public static double3 TransformVector(double3 vec, double4x4 mat)
        {
            double3 v;
            v.x = double3.Dot(vec, new double3(mat.Column0));
            v.y = double3.Dot(vec, new double3(mat.Column1));
            v.z = double3.Dot(vec, new double3(mat.Column2));
            return v;
        }

        /// <summary>
        /// Transform a direction vector by the given Matrix
        /// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static void TransformVector(ref double3 vec, ref double4x4 mat, out double3 result)
        {
            result.x = vec.x * mat.Row0.x +
                       vec.y * mat.Row1.x +
                       vec.z * mat.Row2.x;

            result.y = vec.x * mat.Row0.y +
                       vec.y * mat.Row1.y +
                       vec.z * mat.Row2.y;

            result.z = vec.x * mat.Row0.z +
                       vec.y * mat.Row1.z +
                       vec.z * mat.Row2.z;
        }

        /// <summary>
        /// Transform a Normal by the given Matrix
        /// </summary>
        /// <param name="norm">The normal to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>
        /// The transformed normal
        /// </returns>
        /// <remarks>
        /// This calculates the inverse of the given matrix, use TransformNormalInverse if you
        /// already have the inverse to avoid this extra calculation
        /// </remarks>
        public static double3 TransformNormal(double3 norm, double4x4 mat)
        {
            mat.Invert();
            return TransformNormalInverse(norm, mat);
        }

        /// <summary>
        /// Transform a Normal by the given Matrix
        /// </summary>
        /// <param name="norm">The normal to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed normal</param>
        /// <remarks>
        /// This calculates the inverse of the given matrix, use TransformNormalInverse if you
        /// already have the inverse to avoid this extra calculation
        /// </remarks>
        public static void TransformNormal(ref double3 norm, ref double4x4 mat, out double3 result)
        {
            double4x4 Inverse = double4x4.Invert(mat);
            double3.TransformNormalInverse(ref norm, ref Inverse, out result);
        }

        /// <summary>
        /// Transform a Normal by the (transpose of the) given Matrix
        /// </summary>
        /// <param name="norm">The normal to transform</param>
        /// <param name="invMat">The inverse of the desired transformation</param>
        /// <returns>
        /// The transformed normal
        /// </returns>
        /// <remarks>
        /// This version doesn't calculate the inverse matrix.
        /// Use this version if you already have the inverse of the desired transform to hand
        /// </remarks>
        public static double3 TransformNormalInverse(double3 norm, double4x4 invMat)
        {
            double3 n;
            n.x = double3.Dot(norm, new double3(invMat.Row0));
            n.y = double3.Dot(norm, new double3(invMat.Row1));
            n.z = double3.Dot(norm, new double3(invMat.Row2));
            return n;
        }

        /// <summary>
        /// Transform a Normal by the (transpose of the) given Matrix
        /// </summary>
        /// <param name="norm">The normal to transform</param>
        /// <param name="invMat">The inverse of the desired transformation</param>
        /// <param name="result">The transformed normal</param>
        /// <remarks>
        /// This version doesn't calculate the inverse matrix.
        /// Use this version if you already have the inverse of the desired transform to hand
        /// </remarks>
        public static void TransformNormalInverse(ref double3 norm, ref double4x4 invMat, out double3 result)
        {
            result.x = norm.x * invMat.Row0.x +
                       norm.y * invMat.Row0.y +
                       norm.z * invMat.Row0.z;

            result.y = norm.x * invMat.Row1.x +
                       norm.y * invMat.Row1.y +
                       norm.z * invMat.Row1.z;

            result.z = norm.x * invMat.Row2.x +
                       norm.y * invMat.Row2.y +
                       norm.z * invMat.Row2.z;
        }

        /// <summary>
        /// Transform a Position by the given Matrix
        /// </summary>
        /// <param name="pos">The position to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>
        /// The transformed position
        /// </returns>
        public static double3 TransformPosition(double3 pos, double4x4 mat)
        {
            double3 p;
            p.x = double3.Dot(pos, new double3(mat.Column0)) + mat.Row3.x;
            p.y = double3.Dot(pos, new double3(mat.Column1)) + mat.Row3.y;
            p.z = double3.Dot(pos, new double3(mat.Column2)) + mat.Row3.z;
            return p;
        }

        /// <summary>
        /// Transform a Position by the given Matrix
        /// </summary>
        /// <param name="pos">The position to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed position</param>
        public static void TransformPosition(ref double3 pos, ref double4x4 mat, out double3 result)
        {
            result.x = pos.x * mat.Row0.x +
                       pos.y * mat.Row1.x +
                       pos.z * mat.Row2.x +
                       mat.Row3.x;

            result.y = pos.x * mat.Row0.y +
                       pos.y * mat.Row1.y +
                       pos.z * mat.Row2.y +
                       mat.Row3.y;

            result.z = pos.x * mat.Row0.z +
                       pos.y * mat.Row1.z +
                       pos.z * mat.Row2.z +
                       mat.Row3.z;
        }

        /// <summary>
        /// Transform a Vector by the given Matrix
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>
        /// The transformed vector
        /// </returns>
        public static double3 Transform(double3 vec, double4x4 mat)
        {
            double3 result;
            Transform(ref vec, ref mat, out result);
            return result;
        }

        /// <summary>
        /// Transform a Vector by the given Matrix
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static void Transform(ref double3 vec, ref double4x4 mat, out double3 result)
        {
            double4 v4 = new double4(vec.x, vec.y, vec.z, 1.0f);
            double4.Transform(ref v4, ref mat, out v4);
            result = v4.xyz;
        }

        /// <summary>
        /// Transforms a vector by a QuaternionD rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The QuaternionD to rotate the vector by.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static double3 Transform(double3 vec, QuaternionD quat)
        {
            double3 result;
            Transform(ref vec, ref quat, out result);
            return result;
        }

        /// <summary>
        /// Transforms a vector by a QuaternionD rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The QuaternionD to rotate the vector by.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Transform(ref double3 vec, ref QuaternionD quat, out double3 result)
        {
            // Since vec.w == 0, we can optimize quat * vec * quat^-1 as follows:
            // vec + 2.0 * cross(quat.xyz, cross(quat.xyz, vec) + quat.ww * vec)
            double3 xyz = quat.xyz, temp, temp2;
            double3.Cross(ref xyz, ref vec, out temp);
            double3.Multiply(ref vec, quat.w, out temp2);
            double3.Add(ref temp, ref temp2, out temp);
            double3.Cross(ref xyz, ref temp, out temp);
            double3.Multiply(ref temp, 2, out temp);
            double3.Add(ref vec, ref temp, out result);
        }

        /// <summary>
        /// Transform a double3 by the given Matrix, and project the resulting double4 back to a double3
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>
        /// The transformed vector
        /// </returns>
        public static double3 TransformPerspective(double3 vec, double4x4 mat)
        {
            double3 result;
            TransformPerspective(ref vec, ref mat, out result);
            return result;
        }

        /// <summary>
        /// Transform a double3 by the given Matrix, and project the resulting double4 back to a double3
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static void TransformPerspective(ref double3 vec, ref double4x4 mat, out double3 result)
        {
            double4 v = new double4(vec);
            double4.Transform(ref v, ref mat, out v);
            result.x = v.x / v.w;
            result.y = v.y / v.w;
            result.z = v.z / v.w;
        }

        #endregion

        #region CalculateAngle

        /// <summary>
        /// Calculates the angle (in radians) between two vectors.
        /// </summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <returns>
        /// Angle (in radians) between the vectors.
        /// </returns>
        /// <remarks>
        /// Note that the returned angle is never bigger than the constant Pi.
        /// </remarks>
        public static double CalculateAngle(double3 first, double3 second)
        {
            return (double)System.Math.Acos((double3.Dot(first, second)) / (first.Length * second.Length));
        }

        /// <summary>
        /// Calculates the angle (in radians) between two vectors.
        /// </summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <param name="result">Angle (in radians) between the vectors.</param>
        /// <remarks>
        /// Note that the returned angle is never bigger than the constant Pi.
        /// </remarks>
        public static void CalculateAngle(ref double3 first, ref double3 second, out double result)
        {
            double temp;
            double3.Dot(ref first, ref second, out temp);
            result = (double)System.Math.Acos(temp / (first.Length * second.Length));
        }

        #endregion

        #endregion

        #region Swizzle

        /// <summary>
        /// Gets or sets an OpenTK.double2 with the x and y components of this instance.
        /// </summary>
        /// <value>
        /// The xy.
        /// </value>
        public double2 xy { get { return new double2(x, y); } set { x = value.x; y = value.y; } }

        #endregion

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static double3 operator +(double3 left, double3 right)
        {
            left.x += right.x;
            left.y += right.y;
            left.z += right.z;
            return left;
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static double3 operator -(double3 left, double3 right)
        {
            left.x -= right.x;
            left.y -= right.y;
            left.z -= right.z;
            return left;
        }

        /// <summary>
        /// Negates an instance.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static double3 operator -(double3 vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            vec.z = -vec.z;
            return vec;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static double3 operator *(double3 vec, double scale)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            return vec;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="scale">The scalar.</param>
        /// <param name="vec">The instance.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static double3 operator *(double scale, double3 vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            return vec;
        }

        /// <summary>
        /// Divides an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static double3 operator /(double3 vec, double scale)
        {
            double mult = 1.0f / scale;
            vec.x *= mult;
            vec.y *= mult;
            vec.z *= mult;
            return vec;
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>
        /// True, if left equals right; false otherwise.
        /// </returns>
        public static bool operator ==(double3 left, double3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>
        /// True, if left does not equa lright; false otherwise.
        /// </returns>
        public static bool operator !=(double3 left, double3 right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current double3.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", x, y, z);
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
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
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
            if (!(obj is double3))
                return false;

            return this.Equals((double3)obj);
        }

        #endregion

        #endregion

        #region Color
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// The red component (same as x)
        /// </summary>
        public double r
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// The green component (same as y)
        /// </summary>
        public double g
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// The blue component (same as z)
        /// </summary>
        public double b
        {
            get { return z; }
            set { z = value; }
        }
        // ReSharper restore InconsistentNaming
        #endregion

        #endregion

        #region IEquatable<double3> Members

        /// <summary>
        /// Indicates whether the current vector is equal to another vector.
        /// </summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>
        /// true if the current vector is equal to the vector parameter; otherwise, false.
        /// </returns>
        public bool Equals(double3 other)
        {
            return
                x == other.x &&
                y == other.y &&
                z == other.z;
        }

        #endregion

        public static Converter<string, double3> Parse { get; set; }
    }
}
