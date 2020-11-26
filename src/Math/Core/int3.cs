using ProtoBuf;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a 3D vector or an index (eg. for a grid-like structure) using three integers.
    /// </summary>
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct int3 : IEquatable<int3>
    {
        #region Fields

        /// <summary>
        /// The x component of the float3.
        /// </summary>
        [ProtoMember(1)]
        public int x;

        /// <summary>
        /// The y component of the float3.
        /// </summary>
        [ProtoMember(2)]
        public int y;

        /// <summary>
        /// The z component of the float3.
        /// </summary>
        [ProtoMember(3)]
        public int z;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new int3.
        /// </summary>
        /// <param name="x">The x component of the int3.</param>
        /// <param name="y">The y component of the int3.</param>
        /// <param name="z">The z component of the int3.</param>
        public int3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Constructs a new int3.
        /// </summary>
        /// <param name="i">The x, y and z component of the int3.</param>

        public int3(int i)
        {
            x = i;
            y = i;
            z = i;
        }

        #endregion Constructors

        #region Public Members

        #region Instance

        /// <summary>
        /// Returns an array of floats with the three components of the vector.
        /// </summary>
        /// <returns>Returns an array of floats with the three components of the vector.</returns>
        public int[] ToArray()
        {
            return new[] { x, y, z };
        }

        #endregion Instance

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length float3 that points towards the x-axis.
        /// </summary>
        public static readonly float3 UnitX = new float3(1, 0, 0);

        /// <summary>
        /// Defines a unit-length float3 that points towards the y-axis.
        /// </summary>
        public static readonly float3 UnitY = new float3(0, 1, 0);

        /// <summary>
        /// Defines a unit-length float3 that points towards the z-axis.
        /// </summary>
        public static readonly float3 UnitZ = new float3(0, 0, 1);

        /// <summary>
        /// Defines a zero-length float3.
        /// </summary>
        public static readonly float3 Zero = new float3(0, 0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly float3 One = new float3(1, 1, 1);

        // <summary>
        // Defines the size of the float3 struct in bytes.
        // </summary>
        // public static readonly int SizeInBytes = Marshal.SizeOf(new float3());

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
        public static int3 Add(int3 a, int3 b)
        {
            var result = new int3(a.x + b.x, a.y + b.y, a.z + b.z);
            return result;
        }

        /// <summary>
        /// Adds a scalar to a instance.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static int3 Add(int3 left, int scalar)
        {
            var result = new int3(left.x + scalar, left.y + scalar, left.z + scalar);
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
        public static int3 Subtract(int3 a, int3 b)
        {
            var result = new int3(a.x - b.x, a.y - b.y, a.z - b.z);
            return result;
        }

        /// <summary>
        /// Adds a scalar from a instance.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static int3 Subtract(int3 left, int scalar)
        {
            var result = new int3(left.x - scalar, left.y - scalar, left.z - scalar);
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
        public static int3 Multiply(int3 vector, int scale)
        {
            var result = new int3(vector.x * scale, vector.y * scale, vector.z * scale);
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
        public static int3 Multiply(int3 vector, int3 scale)
        {
            var result = new int3(vector.x * scale.x, vector.y * scale.y, vector.z * scale.z);
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
        public static int3 Divide(int3 vector, int scale)
        {
            var result = new int3(vector.x / scale, vector.y / scale, vector.z / scale);
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
        public static int3 Divide(int3 vector, int3 scale)
        {
            var result = new int3(vector.x / scale.x, vector.y / scale.y, vector.z / scale.z);
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
        public static int3 ComponentMin(int3 a, int3 b)
        {
            a.x = a.x < b.x ? a.x : b.x;
            a.y = a.y < b.y ? a.y : b.y;
            a.z = a.z < b.z ? a.z : b.z;
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
        public static int3 ComponentMax(int3 a, int3 b)
        {
            a.x = a.x > b.x ? a.x : b.x;
            a.y = a.y > b.y ? a.y : b.y;
            a.z = a.z > b.z ? a.z : b.z;
            return a;
        }

        #endregion ComponentMax

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
        public static int3 Clamp(int3 vec, int3 min, int3 max)
        {
            vec.x = vec.x < min.x ? min.x : vec.x > max.x ? max.x : vec.x;
            vec.y = vec.y < min.y ? min.y : vec.y > max.y ? max.y : vec.y;
            vec.z = vec.z < min.z ? min.z : vec.z > max.z ? max.z : vec.z;
            return vec;
        }

        #endregion Clamp

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
        public static int3 Lerp(int3 a, int3 b, int blend)
        {
            a.x = blend * (b.x - a.x) + a.x;
            a.y = blend * (b.y - a.y) + a.y;
            a.z = blend * (b.z - a.z) + a.z;
            return a;
        }

        #endregion Lerp

        #endregion Static

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static int3 operator +(int3 left, int3 right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Adds a scalar to a instance.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static int3 operator +(int3 left, int scalar)
        {
            return Add(left, scalar);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static int3 operator -(int3 left, int3 right)
        {
            return Subtract(left, right);
        }

        /// <summary>
        /// Adds a scalar from a instance.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static int3 operator -(int3 left, int scalar)
        {
            return Subtract(left, scalar);
        }

        /// <summary>
        /// Negates an instance.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static int3 operator -(int3 vec)
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
        public static int3 operator *(int3 vec, int scale)
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
        public static int3 operator *(int scale, int3 vec)
        {
            return Multiply(vec, scale);
        }

        /// <summary>
        /// Multiplies two instances (componentwise).
        /// </summary>
        /// <param name="vec1">The first instance.</param>
        /// <param name="vec2">The second instance.</param>
        /// <returns>The result of the multiplication.</returns>
        public static int3 operator *(int3 vec1, int3 vec2)
        {
            return Multiply(vec1, vec2);
        }

        /// <summary>
        /// Divides an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static int3 operator /(int3 vec, int scale)
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
        public static bool operator ==(int3 left, int3 right)
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
        public static bool operator !=(int3 left, int3 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Explicit cast operator to cast a double3 into a float3 value.
        /// </summary>
        /// <param name="i">The double3 value to cast.</param>
        /// <returns>A float3 value.</returns>
        public static explicit operator int3(int i)
        {
            return new int3(i);
        }

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current float3.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current float3.
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

            return string.Format(provider, "({1}{0} {2}{0} {3})", separator, x, y, z);
        }

        #endregion public override string ToString()

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>
        /// True if the instances are equal; false otherwise.
        /// </returns>
        public bool Equals(int3 other)
        {
            if (x == other.x && y == other.y && z == other.z)
                return true;
            else return false;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>
        /// True if the instances are equal; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is int3))
                return false;

            return Equals((int3)obj);
        }

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>
        /// A System.Int32 containing the unique hashcode for this instance.
        /// </returns>
        //https://stackoverflow.com/questions/22494481/how-to-hash-a-3-dimensional-index
        public override int GetHashCode()
        {
            unchecked
            {
                return x + (31 * y) + (31 * 31 * z);
            }
        }

        #endregion Overrides

        #region Color

        /// <summary>
        /// The red component (same as x)
        /// </summary>
        public int r
        {
            get => x;
            set => x = value;
        }

        /// <summary>
        /// The green component (same as y)
        /// </summary>
        public int g
        {
            get => y;
            set => y = value;
        }

        /// <summary>
        /// The blue component (same as z)
        /// </summary>
        public int b
        {
            get => z;
            set => z = value;
        }

        #endregion Color

        #endregion Public Members
    }
}