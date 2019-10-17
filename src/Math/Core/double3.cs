using System;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a 3D vector using three double-precision floating-point numbers.
    /// </summary>
    /// <remarks>
    /// The double3 structure is suitable for interoperation with unmanaged code requiring three consecutive doubles.
    /// </remarks>
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

        #region this
        public double this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a double3 type");
                }
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
                    default:
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a double3 type");
                }
            }
        }
        #endregion

        #region Instance

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

        #region public Normalize()

        /// <summary>
        /// Scales the double3 to unit length.
        /// </summary>
        public double3 Normalize()
        {
            return Normalize(this);
        }

        #endregion

        #region public NormalizeFast()

        /// <summary>
        /// Scales the double3 to approximately unit length.
        /// </summary>
        public double3 NormalizeFast()
        {
            return NormalizeFast(this);
        }

        #endregion

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
            return new double3(a.x + b.x, a.y + b.y, a.z + b.z);
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
            return new double3(a.x - b.x, a.y - b.y, a.z - b.z);
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
            return new double3(vector.x * scale, vector.y * scale, vector.z * scale);
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
            return new double3(vector.x * scale.x, vector.y * scale.y, vector.z * scale.z);
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
        public static double3 Divide(double3 vector, double3 scale)
        {
            return new double3(vector.x / scale.x, vector.y / scale.y, vector.z / scale.z);
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
            double scale = 1.0 / vec.Length;
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
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
        public static double3 NormalizeFast(double3 vec)
        {
            double scale = M.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
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
        public static double Dot(double3 left, double3 right)
        {
            return left.x * right.x + left.y * right.y + left.z * right.z;
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
            double3 result = new double3(left.y * right.z - left.z * right.y,
                left.z * right.x - left.x * right.z,
                left.x * right.y - left.y * right.x);

            return result;
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
        public static double3 Barycentric(double3 a, double3 b, double3 c, double u, double v)
        {
            return u*a + v*b + (1.0-u-v)*c;
        }

        /// <summary>
        /// Calculates the barycentric coordinates for the given point in the given triangle, such that u*a + v*b + (1-u-v)*c = point.
        /// </summary>
        /// <param name="a">The first point of the triangle.</param>
        /// <param name="b">The second point of the triangle.</param>
        /// <param name="c">The third point of the triangle.</param>
        /// <param name="point">The point to calculate the barycentric coordinates for.</param>
        /// <param name="u">The resulting u coordinate.</param>
        /// <param name="v">The resulting v coordinate.</param>
        public static void GetBarycentric(double3 a, double3 b, double3 c, double3 point, out double u, out double v)
        {
            // Original taken from http://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates
            // which is a transcript from http://realtimecollisiondetection.net/. Re-arranged to directly calculate u and v (and have w be 1-u-v)
            double3 v0 = b - c, v1 = a - c, v2 = point - c;
            double d00 = Dot(v0, v0);
            double d01 = Dot(v0, v1);
            double d11 = Dot(v1, v1);
            double d20 = Dot(v2, v0);
            double d21 = Dot(v2, v1);
            double denom = d00 * d11 - d01 * d01;
            u = (d00 * d21 - d01 * d20) / denom;
            v = (d11 * d20 - d01 * d21) / denom;
        }


        #endregion

        #region Rotate

        /// <summary>
        /// Rotates a vector by the given euler-angles in the following order: yaw (y-axis), pitch (x-axis), roll (z-axis).
        /// </summary>
        /// <param name="euler">The angles used for the rotation.</param>
        /// <param name="vec">The vector to rotate.</param>
        /// <param name="inDegrees">Optional: Whether the angles are given in degrees (true) or radians (false). Defautl is radians.</param>
        /// <returns>The rotated vector.</returns>
        public static double3 Rotate(double3 euler, double3 vec, bool inDegrees = false)
        {
            if (inDegrees)
            {
                euler.x = M.DegreesToRadiansD(euler.x);
                euler.y = M.DegreesToRadiansD(euler.y);
                euler.z = M.DegreesToRadiansD(euler.z);
            }

            double4x4 xRot = double4x4.CreateRotationX(euler.x);
            double4x4 yRot = double4x4.CreateRotationY(euler.y);
            double4x4 zRot = double4x4.CreateRotationZ(euler.z);

            vec = double4x4.Transform(yRot, vec);
            vec = double4x4.Transform(xRot, vec);
            vec = double4x4.Transform(zRot, vec);

            double3 result = vec;

            return result;
        }

        /// <summary>
        /// Rotates a vector by the given quaternion.
        /// </summary>
        /// <param name="quat">The quaternion used for the rotation.</param>
        /// <param name="vec">The vector to rotate.</param>
        /// <returns>The rotated vector.</returns>
        public static double3 Rotate(QuaternionD quat, double3 vec)
        {
            double3 temp, result;

            temp = Cross(quat.xyz, vec) + quat.w * vec;
            temp = Cross(2 * quat.xyz, temp);
            result = vec + temp;

            return result;
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

        #endregion

        #endregion

        #region Swizzle

        /// <summary>
        /// Gets and sets an OpenTK.double2 with the x and y components of this instance.
        /// </summary>
        /// <value>
        /// The xy.
        /// </value>
        public double2 xy { get { return new double2(x, y); } set { x = value.x; y = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the x, y and z components of this instance.
        /// </summary>
        public double3 xyz { get { return new double3(x, y, z); } set { x = value.x; y = value.y; z = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the x, z and y components of this instance.
        /// </summary>
        public double3 xzy { get { return new double3(x, z, y); } set { x = value.x; z = value.y; y = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the y, z and x components of this instance.
        /// </summary>
        public double3 yzx { get { return new double3(y, z, x); } set { y = value.x; z = value.y; x = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the y, x and z components of this instance.
        /// </summary>
        public double3 yxz { get { return new double3(y, x, z); } set { y = value.x; x = value.y; z = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the z, x and y components of this instance.
        /// </summary>
        public double3 zxy { get { return new double3(z, x, y); } set { z = value.x; x = value.y; y = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the z, y and x components of this instance.
        /// </summary>
        public double3 zyx { get { return new double3(z, y, x); } set { z = value.x; y = value.y; x = value.z; } }

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
        public static double3 operator -(double3 left, double3 right)
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
        public static double3 operator *(double scale, double3 vec)
        {
            return Multiply(vec, scale);
        }

        /// <summary>
        /// Multiplies a vector by the components a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public static double3 operator *(double3 vec, double3 scale)
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
        public static double3 operator /(double3 vec, double scale)
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

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a double3.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, double3> Parse { get; set; }
    }
}