#pragma warning disable 1591

using System;
using System.Runtime.InteropServices;
using ProtoBuf;

namespace Fusee.Math.Core
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Represents a 2D vector using two single-precision floating-point numbers.
    /// </summary>
    /// <remarks>
    /// The float2 structure is suitable for interoperation with unmanaged code requiring two consecutive floats.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [ProtoContract]
    public struct float2 : IEquatable<float2>
    {
        #region Fields

        /// <summary>
        /// The x component of the float2.
        /// </summary>
        [ProtoMember(1)]
        public float x;

        /// <summary>
        /// The y component of the float2.
        /// </summary>
        [ProtoMember(2)]
        public float y;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new float2.
        /// </summary>
        /// <param name="x">The x coordinate of the net float2.</param>
        /// <param name="y">The y coordinate of the net float2.</param>
        public float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        #endregion

        #region Public Members

        #region Instance

        #region public float Length

        /// <summary>
        /// Gets the length (magnitude) of the vector.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        /// <see cref="LengthSquared" />
        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(LengthSquared);
            }
        }

        #endregion

        #region public float LengthSquared

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
        public float LengthSquared
        {
            get
            {
                return x * x + y * y;
            }
        }

        #endregion

        #region public float2 PerpendicularRight

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        /// <value>
        /// The perpendicular right.
        /// </value>
        public float2 PerpendicularRight
        {
            get
            {
                return new float2(y, -x);
            }
        }

        #endregion

        #region public float2 PerpendicularLeft

        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        /// <value>
        /// The perpendicular left.
        /// </value>
        public float2 PerpendicularLeft
        {
            get
            {
                return new float2(-y, x);
            }
        }

        #endregion

        #region public Normalize()

        /// <summary>
        /// Scales the float2 to unit length.
        /// </summary>
        public float2 Normalize()
        {
            return Normalize(this);
        }

        #endregion

        #region public NormalizeFast()

        /// <summary>
        /// Scales the float2 to approximately unit length.
        /// </summary>
        public float2 NormalizeFast()
        {
            return NormalizeFast(this);
        }

        #endregion

        /// <summary>
        /// Returns an array of floats with the two components of the vector.
        /// </summary>
        /// <returns>Returns an array of floats with the two components of the vector.</returns>
        public float[] ToArray()
        {
            return new float[] { x, y};
        }

        #endregion

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length float2 that points towards the x-axis.
        /// </summary>
        public static readonly float2 UnitX = new float2(1, 0);

        /// <summary>
        /// Defines a unit-length float2 that points towards the y-axis.
        /// </summary>
        public static readonly float2 UnitY = new float2(0, 1);

        /// <summary>
        /// Defines a zero-length float2.
        /// </summary>
        public static readonly float2 Zero = new float2(0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly float2 One = new float2(1, 1);

        // <summary>
        // Defines the size of the float2 struct in bytes.
        // </summary>
        // public static readonly int SizeInBytes = Marshal.SizeOf(new float2());

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
        public static float2 Add(float2 a, float2 b)
        {
            var result = new float2(a.x + b.x, a.y + b.y);

            return result;
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
        public static float2 Subtract(float2 a, float2 b)
        {
            var result = new float2(a.x - b.x, a.y - b.y);

            return result;
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
        public static float2 Multiply(float2 vector, float scale)
        {
            var result = new float2(vector.x * scale, vector.y * scale);
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
        public static float2 Multiply(float2 vector, float2 scale)
        {
            var result = new float2(vector.x * scale.x, vector.y * scale.y);
            return result;
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
        public static float2 Divide(float2 vector, float scale)
        {
            var result = new float2(vector.x / scale, vector.y / scale);

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
        public static float2 Divide(float2 vector, float2 scale)
        {
            var result = new float2(vector.x / scale.x, vector.y / scale.y);

            return result;
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
        public static float2 ComponentMin(float2 a, float2 b)
        {
            a.x = a.x < b.x ? a.x : b.x;
            a.y = a.y < b.y ? a.y : b.y;
            return a;
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
        public static float2 ComponentMax(float2 a, float2 b)
        {
            a.x = a.x > b.x ? a.x : b.x;
            a.y = a.y > b.y ? a.y : b.y;
            return a;
        }

        #endregion

        #region Min

        /// <summary>
        /// Returns the float3 with the minimum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>
        /// The minimum float3
        /// </returns>
        public static float2 Min(float2 left, float2 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        #endregion

        #region Max

        /// <summary>
        /// Returns the float3 with the minimum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>
        /// The minimum float3
        /// </returns>
        public static float2 Max(float2 left, float2 right)
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
        public static float2 Clamp(float2 vec, float2 min, float2 max)
        {
            vec.x = vec.x < min.x ? min.x : vec.x > max.x ? max.x : vec.x;
            vec.y = vec.y < min.y ? min.y : vec.y > max.y ? max.y : vec.y;
            return vec;
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
        public static float2 Normalize(float2 vec)
        {
            float scale = 1.0f / vec.Length;
            vec.x *= scale;
            vec.y *= scale;
            return vec;
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
        public static float2 NormalizeFast(float2 vec)
        {
            float scale = M.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y);
            vec.x *= scale;
            vec.y *= scale;
            return vec;
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
        public static float Dot(float2 left, float2 right)
        {
            return left.x * right.x + left.y * right.y;
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
        public static float2 Lerp(float2 a, float2 b, float blend)
        {
            a.x = blend * (b.x - a.x) + a.x;
            a.y = blend * (b.y - a.y) + a.y;
            return a;
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
        /// a when u=1,v=9, b when u=0,v=1, c when u=v=1, and a linear combination of a,b,c otherwise
        /// </returns>
        public static float2 Barycentric(float2 a, float2 b, float2 c, float u, float v)
        {
            return u*a + v*b + (1.0f-u-v)*c;
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
        public static void GetBarycentric(float2 a, float2 b, float2 c, float2 point, out float u, out float v)
        {
            float2 cb = b-c;
            float2 cp = point - c;
            float2 ca = a-c;
            float denom = (cb.y * ca.x - cb.x * ca.y);
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
        //public static void GetBarycentric(float2 a, float2 b, float2 c, float2 point, out float u, out float v)
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
        public static bool PointInTriangle(float2 a, float2 b, float2 c, float2 point, out float u, out float v)
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
        public static bool IsTriangleCW(float2 a, float2 b, float2 c)
        {
            float2 cb = b - c;
            float2 ca = a - c;
            // Calculate z component of cross product
            float z = ca.x * cb.y - ca.y * cb.x;
            return z < 0;
        }

        #endregion

        #endregion

        #region Swizzle

        /// <summary>
        /// Gets or sets an OpenTK.float2 with the x and y components of this instance.
        /// </summary>
        public float2 xy { get { return new float2(x, y); } set { x = value.x; y = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.float2 with the y and x components of this instance.
        /// </summary>
        public float2 yx { get { return new float2(y, x); } set { y = value.x; x = value.y; } }

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
        public static float2 operator +(float2 left, float2 right)
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
        public static float2 operator +(float2 left, float scalar)
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
        public static float2 operator -(float2 left, float2 right)
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
        public static float2 operator -(float2 left, float scalar)
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
        public static float2 operator -(float2 vec)
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
        public static float2 operator *(float2 vec, float scale)
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
        public static float2 operator *(float scale, float2 vec)
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
        public static float2 operator *(float2 vec1, float2 vec2)
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
        public static float2 operator /(float2 vec, float scale)
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
        public static bool operator ==(float2 left, float2 right)
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
        public static bool operator !=(float2 left, float2 right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current float2.
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
            if (!(obj is float2))
                return false;

            return this.Equals((float2)obj);
        }

        #endregion

        #endregion

        #region Color

        /// <summary>
        /// The red component (same as x)
        /// </summary>
        public float r
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// The green component (same as y)
        /// </summary>
        public float g
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// The rg component (same as xy)
        /// </summary>
        public float2 rg
        {
            get { return xy; }
            set { xy = value; }
        }

        #endregion

        #endregion

        #region IEquatable<float2> Members

        /// <summary>
        /// Indicates whether the current vector is equal to another vector.
        /// </summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>
        /// true if the current vector is equal to the vector parameter; otherwise, false.
        /// </returns>
        public bool Equals(float2 other)
        {
            return
                x == other.x &&
                y == other.y;
        }

        #endregion

        /// <summary>
        /// Gets or sets the Converter object. Has the ability to convert a string to a float2.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, float2> Parse { get; set; }
    }

    // ReSharper restore InconsistentNaming
}

#pragma warning restore 1591
