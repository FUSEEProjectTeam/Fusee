using ProtoBuf;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a 2D vector using two double-precision.
    /// </summary>
    /// <remarks>
    /// The double2 structure is suitable for inter-operation with unmanaged code requiring two consecutive doubles.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [ProtoContract]
    public struct double2 : IEquatable<double2>
    {
        #region Fields

        /// <summary>
        /// The x component of the double2.
        /// </summary>
        [ProtoMember(1)]
        public double x;

        /// <summary>
        /// The y component of the double2.
        /// </summary>
        [ProtoMember(2)]
        public double y;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new double2.
        /// </summary>
        /// <param name="val">This value will be set for the x and y component.</param>
        public double2(double val)
        {
            x = val;
            y = val;
        }

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

        #endregion Constructors

        #region Public Members

        #region this

        /// <summary>
        /// Gets or sets the individual components x and y, depending on their index.
        /// </summary>
        /// <param name="idx">The index (between 0 and 1).</param>
        /// <returns>The x or y component of the double2.</returns>
        public double this[int idx]
        {
            get
            {
                return idx switch
                {
                    0 => x,
                    1 => y,
                    _ => throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a double2 type"),
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

                    default:
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a double2 type");
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
        /// <see cref="LengthSquared" />
        public double Length => (double)System.Math.Sqrt(LengthSquared);

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
        public double LengthSquared => x * x + y * y;

        #endregion public double LengthSquared

        #region public double2 PerpendicularRight

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        /// <value>
        /// The perpendicular right.
        /// </value>
        public double2 PerpendicularRight => new(y, -x);

        #endregion public double2 PerpendicularRight

        #region public double2 PerpendicularLeft

        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        /// <value>
        /// The perpendicular left.
        /// </value>
        public double2 PerpendicularLeft => new(-y, x);

        #endregion public double2 PerpendicularLeft

        #region public Normalize()

        /// <summary>
        /// Scales the double2 to unit length.
        /// </summary>
        public double2 Normalize()
        {
            return Normalize(this);
        }

        #endregion public Normalize()

        #region public NormalizeFast()

        /// <summary>
        /// Scales the double2 to approximately unit length.
        /// </summary>
        public double2 NormalizeFast()
        {
            return NormalizeFast(this);
        }

        #endregion public NormalizeFast()

        /// <summary>
        /// Returns an array of doubles with the two components of the vector.
        /// </summary>
        /// <returns>Returns an array of doubles with the two components of the vector.</returns>
        public double[] ToArray()
        {
            return new double[] { x, y };
        }

        #endregion Instance

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length double2 that points towards the x-axis.
        /// </summary>
        public static readonly double2 UnitX = new(1, 0);

        /// <summary>
        /// Defines a unit-length double2 that points towards the y-axis.
        /// </summary>
        public static readonly double2 UnitY = new(0, 1);

        /// <summary>
        /// Defines a zero-length double2.
        /// </summary>
        public static readonly double2 Zero = new(0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly double2 One = new(1, 1);

        // <summary>
        // Defines the size of the double2 struct in bytes.
        // </summary>
        // public static readonly int SizeInBytes = Marshal.SizeOf(new double2());

        #endregion Fields

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
            var result = new double2(a.x + b.x, a.y + b.y);

            return result;
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
        public static double2 Subtract(double2 a, double2 b)
        {
            var result = new double2(a.x - b.x, a.y - b.y);

            return result;
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
        public static double2 Multiply(double2 vector, double scale)
        {
            var result = new double2(vector.x * scale, vector.y * scale);
            return result;
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
            var result = new double2(vector.x * scale.x, vector.y * scale.y);
            return result;
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
        public static double2 Divide(double2 vector, double scale)
        {
            var result = new double2(vector.x / scale, vector.y / scale);

            return result;
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
            var result = new double2(vector.x / scale.x, vector.y / scale.y);

            return result;
        }

        #endregion Divide

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

        #endregion ComponentMin

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

        #endregion ComponentMax

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

        #endregion Min

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
        public static double2 Clamp(double2 vec, double2 min, double2 max)
        {
            vec.x = vec.x < min.x ? min.x : vec.x > max.x ? max.x : vec.x;
            vec.y = vec.y < min.y ? min.y : vec.y > max.y ? max.y : vec.y;
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
        public static double2 Normalize(double2 vec)
        {
            double scale = 1.0 / vec.Length;
            vec.x *= scale;
            vec.y *= scale;
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
        public static double2 NormalizeFast(double2 vec)
        {
            double scale = M.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y);
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        #endregion NormalizeFast

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

        #endregion Dot

        /// <summary>
        /// Performs <see cref="M.StepD(double, double)"/> for each component of the input vectors.
        /// </summary>
        /// <param name="edge">Specifies the location of the edge of the step function.</param>
        /// <param name="val">Specifies the value to be used to generate the step function.</param>
        public static double2 Step(double2 edge, double2 val)
        {
            return new double2(M.StepD(edge.x, val.x), M.StepD(edge.y, val.y));
        }

        /// <summary>
        /// Returns a double2 where all components are raised to the specified power.
        /// </summary>
        /// <param name="val">The double3 to be raised to a power.</param>
        /// <param name="exp">A double that specifies a power.</param>
        /// <returns></returns>
        public static double2 Pow(double2 val, double exp)
        {
            return new double2(System.Math.Pow(val.r, exp), System.Math.Pow(val.g, exp));
        }

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
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// Each component of vector a is blended with its equivalent in vector b.
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>       
        public static double2 Lerp(double2 a, double2 b, double2 blend)
        {
            a.x = blend.x * (b.x - a.x) + a.x;
            a.y = blend.y * (b.y - a.y) + a.y;
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
        /// a when u=1,v=9, b when u=0,v=1, c when u=v=1, and a linear combination of a,b,c otherwise
        /// </returns>
        public static double2 Barycentric(double2 a, double2 b, double2 c, double u, double v)
        {
            return u * a + v * b + (1.0 - u - v) * c;
        }

        /// <summary>
        /// Calculates the barycentric coordinates for the given point in the given triangle, such that u*a + v*b + (1-u-v)*c = point.
        /// </summary>
        /// <param name="a">The first point of the triangle.</param>
        /// <param name="b">The second point of the triangle.</param>
        /// <param name="c">The third point of the triangle.</param>
        /// <param name="point">The point to calculate the barycentric coordinates for.</param>
        /// <param name="u">The resulting barycentric u coordinate (weight for vertex a).</param>
        /// <param name="v">The resulting barycentric v coordinate (weight for vertex b).</param>
        public static void GetBarycentric(double2 a, double2 b, double2 c, double2 point, out double u, out double v)
        {
            double2 cb = b - c;
            double2 cp = point - c;
            double2 ca = a - c;
            double denom = (cb.y * ca.x - cb.x * ca.y);
            u = (cb.y * cp.x - cb.x * cp.y) / denom;
            v = (ca.x * cp.y - ca.y * cp.x) / denom;
        }

        ///// <summary>
        ///// Calculates the barycentric coordinates for the given point in the given triangle, such that u*a + v*b + (1-u-v)*c = point.
        ///// </summary>
        ///// <param name="a">The first point of the triangle.</param>
        ///// <param name="b">The second point of the triangle.</param>
        ///// <param name="c">The third point of the triangle.</param>
        ///// <param name="point">The point to calculate the barycentric coordinates for.</param>
        ///// <param name="u">The resulting u coordinate.</param>
        ///// <param name="v">The resulting v coordinate.</param>
        //public static void GetBarycentric(double2 a, double2 b, double2 c, double2 point, out double u, out double v)
        //{
        //    u = ((b.y - c.y) * (point.x - c.x) + (c.x - b.x) * (point.y - c.y)) / ((b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y));
        //    v = ((c.y - a.y) * (point.x - c.x) + (a.x - c.x) * (point.y - c.y)) / ((b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y));
        //}

        /// <summary>
        /// Checks if the give point is inside the given triangle (a, b, c). Returns the barycentric coordinates using <see cref="GetBarycentric"/>.
        /// </summary>
        /// <param name="a">The first point of the triangle.</param>
        /// <param name="b">The second point of the triangle.</param>
        /// <param name="c">The third point of the triangle.</param>
        /// <param name="point">The point to calculate the barycentric coordinates for.</param>
        /// <param name="u">The resulting barycentric u coordinate (weight for vertex a).</param>
        /// <param name="v">The resulting barycentric v coordinate (weight for vertex b).</param>
        /// <returns>true, if the point is inside the triangle a, b, c. Otherwise false.</returns>
        public static bool PointInTriangle(double2 a, double2 b, double2 c, double2 point, out double u, out double v)
        {
            GetBarycentric(a, b, c, point, out u, out v);

            //was previously  "u >= 0 && v >= 0 && u + v < 1;", which returned false for u=v=0 and u=1, v=0
            return u >= 0 && v >= 0 && u + v <= 1;
        }

        /// <summary>
        /// Checks if the three given 2D points form a clockwise (CW) triangle
        /// </summary>
        /// <param name="a">The first point of the triangle.</param>
        /// <param name="b">The second point of the triangle.</param>
        /// <param name= "c">The third point of the triangle.</param>
        /// <returns>true if the triangle is clockwise, otherwise false.</returns>
        public static bool IsTriangleCW(double2 a, double2 b, double2 c)
        {
            double2 cb = b - c;
            double2 ca = a - c;
            // Calculate z component of cross product
            double z = ca.x * cb.y - ca.y * cb.x;
            return z < 0;
        }

        #endregion Barycentric

        #endregion Static

        #region Swizzle

        /// <summary>
        /// Gets and sets an OpenTK.double2 with the x and y components of this instance.
        /// </summary>
        public double2 xy { get => new(x, y); set { x = value.x; y = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.double2 with the y and x components of this instance.
        /// </summary>
        public double2 yx { get => new(y, x); set { y = value.x; x = value.y; } }

        #endregion Swizzle

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
            return Add(left, right);
        }

        /// <summary>
        /// Adds a scalar to the specified instance.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        /// Result of addition.
        /// </returns>
        public static double2 operator +(double2 left, double scalar)
        {
            left.x += scalar;
            left.y += scalar;
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
            return Subtract(left, right);
        }

        /// <summary>
        /// Subtracts a scalar from the specified instance.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        /// Result of addition.
        /// </returns>
        public static double2 operator -(double2 left, double scalar)
        {
            left.x -= scalar;
            left.y -= scalar;
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
            return Multiply(vec, scale);
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
            return Multiply(vec, scale);
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="vec1">Left operand.</param>
        /// <param name="vec2">Right operand.</param>
        /// <returns>
        /// Result of multiplication.
        /// </returns>
        public static double2 operator *(double2 vec1, double2 vec2)
        {
            return Multiply(vec1, vec2);
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
            return Divide(vec, scale);
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

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current double2.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current double2.
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

            return string.Format(provider, "({1}{0} {2})", separator, x, y);
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
            return x.GetHashCode() ^ y.GetHashCode();
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
            if (!(obj is double2))
                return false;

            return this.Equals((double2)obj);
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
        /// The rg component (same as xy)
        /// </summary>
        public double2 rg
        {
            get => xy;
            set => xy = value;
        }

        #endregion Color

        #endregion Public Members

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
                System.Math.Abs(x - other.x) < M.EpsilonDouble &&
                System.Math.Abs(y - other.y) < M.EpsilonDouble;
        }

        #endregion IEquatable<double2> Members

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a double2.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, double2> ParseConverter { get; set; } = (x => double2.Parse(x));

        /// <summary>
        /// Parses a string into a double2.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static double2 Parse(string source, IFormatProvider? provider = null)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            char separator = M.GetNumericListSeparator(provider);

            string[] strings = source.Split(new char[] { separator, '(', ')', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length != 2)
                throw new FormatException("String parse for double2 did not result in exactly 2 items.");

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

            return new double2(doubles[0], doubles[1]);
        }
    }
}