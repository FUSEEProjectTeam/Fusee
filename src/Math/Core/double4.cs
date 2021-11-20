using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a 4D vector using four double-precision floating-point numbers.
    /// </summary>
    /// <remarks>
    /// The double4 structure is suitable for interoperation with unmanaged code requiring four consecutive doubles.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct double4 : IEquatable<double4>
    {
        #region Fields

        /// <summary>
        /// The x component of the double4.
        /// </summary>
        public double x;

        /// <summary>
        /// The y component of the double4.
        /// </summary>
        public double y;

        /// <summary>
        /// The z component of the double4.
        /// </summary>
        public double z;

        /// <summary>
        /// The w component of the double4.
        /// </summary>
        public double w;

        /// <summary>
        /// Defines a unit-length double4 that points towards the x-axis.
        /// </summary>
        public static readonly double4 UnitX = new(1, 0, 0, 0);

        /// <summary>
        /// Defines a unit-length double4 that points towards the y-axis.
        /// </summary>
        public static readonly double4 UnitY = new(0, 1, 0, 0);

        /// <summary>
        /// Defines a unit-length double4 that points towards the z-axis.
        /// </summary>
        public static readonly double4 UnitZ = new(0, 0, 1, 0);

        /// <summary>
        /// Defines a unit-length double4 that points towards the w-axis.
        /// </summary>
        public static readonly double4 UnitW = new(0, 0, 0, 1);

        /// <summary>
        /// Defines a zero-length double4.
        /// </summary>
        public static readonly double4 Zero = new(0, 0, 0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly double4 One = new(1, 1, 1, 1);

        // <summary>
        // Defines the size of the double4 struct in bytes.
        // </summary>
        // public static readonly int SizeInBytes = Marshal.SizeOf(new double4());

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new double4.
        /// </summary>
        /// <param name="x">The x component of the double4.</param>
        /// <param name="y">The y component of the double4.</param>
        /// <param name="z">The z component of the double4.</param>
        /// <param name="w">The w component of the double4.</param>
        public double4(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// Constructs a new double4 from the given double2.
        /// </summary>
        /// <param name="v">The double2 to copy components from.</param>
        public double4(double2 v)
        {
            x = v.x;
            y = v.y;
            z = 0.0f;
            w = 0.0f;
        }

        /// <summary>
        /// Constructs a new double4 from the given double3.
        /// </summary>
        /// <param name="v">The double3 to copy components from.</param>
        public double4(double3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = 0.0f;
        }

        /// <summary>
        /// Constructs a new double4 from the specified double3 and ww component.
        /// </summary>
        /// <param name="v">The double3 to copy components from.</param>
        /// <param name="ww">The ww component of the new double4.</param>
        public double4(double3 v, double ww)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = ww;
        }

        /// <summary>
        /// Constructs a new double4 from the given double4.
        /// </summary>
        /// <param name="v">The double4 to copy components from.</param>
        public double4(double4 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = v.w;
        }

        #endregion Constructors

        #region Public Members

        #region this

        /// <summary>
        /// Gets or sets the individual components x, y, z, or w depending on their index.
        /// </summary>
        /// <param name="idx">The index (between 0 and 3).</param>
        /// <returns>The x or y component of the double4.</returns>
        public double this[int idx]
        {
            get
            {
                return idx switch
                {
                    0 => x,
                    1 => y,
                    2 => z,
                    3 => w,
                    _ => throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a double4 type"),
                };
            }
            set
            {
                switch (idx)
                {
                    case 0:
                        x = value;
                        break;

                    case 1:
                        y = value;
                        break;

                    case 2:
                        z = value;
                        break;

                    case 3:
                        w = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a double4 type");
                }
            }
        }

        #endregion this

        #region Instance

        #region public double Length

        /// <summary>
        /// Gets the length (magnitude) of the vector.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        /// <seealso cref="LengthSquared" />
        public double Length => (double)System.Math.Sqrt(x * x + y * y + z * z + w * w);

        #endregion public double Length

        #region public double LengthSquared

        /// <summary>
        /// Gets the square of the vector length (magnitude).
        /// </summary>
        /// <value>
        /// The length squared.
        /// </value>
        /// <see cref="Length" />
        /// <remarks>
        /// This property avoids the costly square root operation required by the Length property. This makes it more suitable
        /// for comparisons.
        /// </remarks>
        public double LengthSquared => x * x + y * y + z * z + w * w;

        #endregion public double LengthSquared

        #region public Normalize()

        /// <summary>
        /// Scales the double4 to unit length.
        /// </summary>
        public double4 Normalize()
        {
            return Normalize(this);
        }

        #endregion public Normalize()

        #region public NormalizeFast()

        /// <summary>
        /// Scales the double4 to approximately unit length.
        /// </summary>
        public double4 NormalizeFast()
        {
            return NormalizeFast(this);
        }

        #endregion public NormalizeFast()

        /// <summary>
        /// Returns an array of doubles with the four components of the vector.
        /// </summary>
        /// <returns>Returns an array of doubles with the four components of the vector.</returns>
        public double[] ToArray()
        {
            return new double[] { x, y, z, w };
        }

        #endregion Instance

        #region Static

        #region Add

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <returns>
        /// Result of operation.
        /// </returns>
        public static double4 Add(double4 a, double4 b)
        {
            return new double4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        #endregion Add

        #region Subtract

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>
        /// Result of subtraction
        /// </returns>
        public static double4 Subtract(double4 a, double4 b)
        {
            return new double4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        #endregion Subtract

        #region Multiply

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public static double4 Multiply(double4 vector, double scale)
        {
            return new double4(vector.x * scale, vector.y * scale, vector.z * scale, vector.w * scale);
        }

        /// <summary>
        /// Multiplies a vector by the components a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public static double4 Multiply(double4 vector, double4 scale)
        {
            return new double4(vector.x * scale.x, vector.y * scale.y, vector.z * scale.z, vector.w * scale.w);
        }

        #endregion Multiply

        #region Divide

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public static double4 Divide(double4 vector, double scale)
        {
            return Multiply(vector, 1 / scale);
        }

        /// <summary>
        /// Divides a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public static double4 Divide(double4 vector, double4 scale)
        {
            return new double4(vector.x / scale.x, vector.y / scale.y, vector.z / scale.z, vector.w / scale.w);
        }

        #endregion Divide

        #region Min

        /// <summary>
        /// Calculate the component-wise minimum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>
        /// The component-wise minimum
        /// </returns>
        public static double4 Min(double4 a, double4 b)
        {
            a.x = a.x < b.x ? a.x : b.x;
            a.y = a.y < b.y ? a.y : b.y;
            a.z = a.z < b.z ? a.z : b.z;
            a.w = a.w < b.w ? a.w : b.w;
            return a;
        }

        #endregion Min

        #region Max

        /// <summary>
        /// Calculate the component-wise maximum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>
        /// The component-wise maximum
        /// </returns>
        public static double4 Max(double4 a, double4 b)
        {
            a.x = a.x > b.x ? a.x : b.x;
            a.y = a.y > b.y ? a.y : b.y;
            a.z = a.z > b.z ? a.z : b.z;
            a.w = a.w > b.w ? a.w : b.w;
            return a;
        }

        #endregion Max

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
        public static double4 Clamp(double4 vec, double4 min, double4 max)
        {
            vec.x = vec.x < min.x ? min.x : vec.x > max.x ? max.x : vec.x;
            vec.y = vec.y < min.y ? min.y : vec.y > max.y ? max.y : vec.y;
            vec.z = vec.z < min.z ? min.z : vec.z > max.z ? max.z : vec.z;
            vec.w = vec.w < min.w ? min.w : vec.w > max.w ? max.w : vec.w;
            return vec;
        }

        #endregion Clamp

        #region Normalize

        /// <summary>
        /// Scale a vector to unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>
        /// The normalized vector
        /// </returns>
        public static double4 Normalize(double4 vec)
        {
            double scale = 1.0f / vec.Length;
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        #endregion Normalize

        #region NormalizeFast

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>
        /// The normalized vector
        /// </returns>
        public static double4 NormalizeFast(double4 vec)
        {
            double scale = M.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w);
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        #endregion NormalizeFast

        #region Dot

        /// <summary>
        /// Calculate the dot product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>
        /// The dot product of the two inputs
        /// </returns>
        public static double Dot(double4 left, double4 right)
        {
            return left.x * right.x + left.y * right.y + left.z * right.z + left.w * right.w;
        }

        #endregion Dot

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
        public static double4 Lerp(double4 a, double4 b, double blend)
        {
            a.x = blend * (b.x - a.x) + a.x;
            a.y = blend * (b.y - a.y) + a.y;
            a.z = blend * (b.z - a.z) + a.z;
            a.w = blend * (b.w - a.w) + a.w;
            return a;
        }

        #endregion Lerp

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
        public static double4 BaryCentric(double4 a, double4 b, double4 c, double u, double v)
        {
            return u * a + v * b + (1.0 - u - v) * c;
        }

        #endregion Barycentric

        #endregion Static

        #region Swizzle

        /// <summary>
        /// Gets and sets an OpenTK.double2 with the x and y components of this instance.
        /// </summary>
        /// <value>
        /// The xy.
        /// </value>
        public double2 xy { get => new(x, y); set { x = value.x; y = value.y; } }

        /// <summary>
        /// Gets and sets an OpenTK.double3 with the x, y and z components of this instance.
        /// </summary>
        /// <value>
        /// The xyz.
        /// </value>
        public double3 xyz { get => new(x, y, z); set { x = value.x; y = value.y; z = value.z; } }

        #endregion Swizzle

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static double4 operator +(double4 left, double4 right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static double4 operator -(double4 left, double4 right)
        {
            return Subtract(left, right);
        }

        /// <summary>
        /// Negates an instance.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static double4 operator -(double4 vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            vec.z = -vec.z;
            vec.w = -vec.w;
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
        public static double4 operator *(double4 vec, double scale)
        {
            return Multiply(vec, scale);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="scale">The scalar.</param>
        /// <param name="vec">The instance.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static double4 operator *(double scale, double4 vec)
        {
            return Multiply(vec, scale);
        }

        /// <summary>
        /// Multiplies a vector by the components a vector (scale).
        /// </summary>
        /// <param name="vec">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public static double4 operator *(double4 vec, double4 scale)
        {
            return Multiply(vec, scale);
        }

        /// <summary>
        /// Divides an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static double4 operator /(double4 vec, double scale)
        {
            return Divide(vec, scale);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>
        /// True, if left equals right; false otherwise.
        /// </returns>
        public static bool operator ==(double4 left, double4 right)
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
        public static bool operator !=(double4 left, double4 right)
        {
            return !left.Equals(right);
        }

        /*
        /// <summary>
        /// Returns a pointer to the first element of the specified instance.
        /// </summary>
        /// <param name="v">The instance.</param>
        /// <returns>A pointer to the first element of v.</returns>
        [CLSCompliant(false)]
        unsafe public static explicit operator double*(double4 v)
        {
            return &v.x;
        }

        /// <summary>
        /// Returns a pointer to the first element of the specified instance.
        /// </summary>
        /// <param name="v">The instance.</param>
        /// <returns>A pointer to the first element of v.</returns>
        public static explicit operator IntPtr(double4 v)
        {
            unsafe
            {
                return (IntPtr)(&v.x);
            }
        }
        */

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current double4.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current double4.
        /// </summary>
        /// <param name="provider">Provides information about a specific culture.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {
            return ConvertToString(provider);
        }

        internal string ConvertToString(IFormatProvider? provider)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            char separator = M.GetNumericListSeparator(provider);

            return string.Format(provider, "({1}{0} {2}{0} {3}{0} {4})", separator, x, y, z, w);
        }

        #endregion public override string ToString()

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>
        /// A System.Int32 containing the unique hashcode for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        #endregion public override int GetHashCode()

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>
        /// True if the instances are equal; false otherwise.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (!(obj is double4))
                return false;

            return this.Equals((double4)obj);
        }

        #endregion public override bool Equals(object obj)

        #endregion Overrides

        #region Color

        /// <summary>
        /// The red component (same as x)
        /// </summary>
        public double r
        {
            get => x;
            set => x = value;
        }

        /// <summary>
        /// The green component (same as y)
        /// </summary>
        public double g
        {
            get => y;
            set => y = value;
        }

        /// <summary>
        /// The blue component (same as z)
        /// </summary>
        public double b
        {
            get => z;
            set => z = value;
        }

        /// <summary>
        /// The rgb component (same as xyz)
        /// </summary>
        public double3 rgb
        {
            get => xyz;
            set => xyz = value;
        }

        /// <summary>
        /// The alpha component (same as w)
        /// </summary>
        public double a
        {
            get => w;
            set => w = value;
        }

        #endregion Color

        #endregion Public Members

        #region IEquatable<double4> Members

        /// <summary>
        /// Indicates whether the current vector is equal to another vector.
        /// </summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>
        /// true if the current vector is equal to the vector parameter; otherwise, false.
        /// </returns>
        public bool Equals(double4 other)
        {
            return
                x == other.x &&
                y == other.y &&
                z == other.z &&
                w == other.w;
        }

        #endregion IEquatable<double4> Members

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a double4.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, double4> ParseConverter { get; set; } = (x => double4.Parse(x));

        /// <summary>
        /// Parses a string into a double4.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static double4 Parse(string source, IFormatProvider? provider = null)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            char separator = M.GetNumericListSeparator(provider);

            string[] strings = source.Split(new char[] { separator, '(', ')', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length != 4)
                throw new FormatException("String parse for double4 did not result in exactly 4 items.");

            double[] doubles = new double[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                try
                {
                    doubles[i] = double.Parse(strings[i], provider);
                }
                catch
                {
                    throw new FormatException();
                }
            }

            return new double4(doubles[0], doubles[1], doubles[2], doubles[3]);
        }
    }
}