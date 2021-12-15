using ProtoBuf;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a 2D vector using two single-precision inting-point numbers.
    /// </summary>
    /// <remarks>
    /// The int2 structure is suitable for interoperation with unmanaged code requiring two consecutive ints.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [ProtoContract]
    public struct int2 : IEquatable<int2>
    {
        #region Fields

        /// <summary>
        /// The x component of the int2.
        /// </summary>
        [ProtoMember(1)]
        public int x;

        /// <summary>
        /// The y component of the int2.
        /// </summary>
        [ProtoMember(2)]
        public int y;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new int2.
        /// </summary>
        /// <param name="val">This value will be set for the x and y component.</param>
        public int2(int val)
        {
            x = val;
            y = val;
        }

        /// <summary>
        /// Constructs a new int2.
        /// </summary>
        /// <param name="x">The x coordinate of the net int2.</param>
        /// <param name="y">The y coordinate of the net int2.</param>
        public int2(int x, int y)
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
        /// <returns>The x or y component of the int2.</returns>
        public int this[int idx]
        {
            get
            {
                return idx switch
                {
                    0 => x,
                    1 => y,
                    _ => throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a int2 type"),
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
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a int2 type");
                }
            }
        }

        #endregion this

        #region Instance

        #region public int Length

        /// <summary>
        /// Gets the length (magnitude) of the vector.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        /// <see cref="LengthSquared" />
        public float Length => (float)System.Math.Sqrt(LengthSquared);

        #endregion public int Length

        #region public int LengthSquared

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
        public int LengthSquared => x * x + y * y;

        #endregion public int LengthSquared

        #region public int2 PerpendicularRight

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        /// <value>
        /// The perpendicular right.
        /// </value>
        public int2 PerpendicularRight => new(y, -x);

        #endregion public int2 PerpendicularRight

        #region public int2 PerpendicularLeft

        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        /// <value>
        /// The perpendicular left.
        /// </value>
        public int2 PerpendicularLeft => new(-y, x);

        #endregion public int2 PerpendicularLeft

        #region public Normalize()

        /// <summary>
        /// Scales the int2 to unit length.
        /// </summary>
        public float2 Normalize()
        {
            return Normalize(this);
        }

        #endregion public Normalize()

        #region public NormalizeFast()

        /// <summary>
        /// Scales the int2 to approximately unit length.
        /// </summary>
        public float2 NormalizeFast()
        {
            return NormalizeFast(this);
        }

        #endregion public NormalizeFast()

        /// <summary>
        /// Returns an array of ints with the two components of the vector.
        /// </summary>
        /// <returns>Returns an array of ints with the two components of the vector.</returns>
        public int[] ToArray()
        {
            return new int[] { x, y };
        }

        #endregion Instance

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length int2 that points towards the x-axis.
        /// </summary>
        public static readonly int2 UnitX = new(1, 0);

        /// <summary>
        /// Defines a unit-length int2 that points towards the y-axis.
        /// </summary>
        public static readonly int2 UnitY = new(0, 1);

        /// <summary>
        /// Defines a zero-length int2.
        /// </summary>
        public static readonly int2 Zero = new(0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly int2 One = new(1, 1);

        // <summary>
        // Defines the size of the int2 struct in bytes.
        // </summary>
        // public static readonly int SizeInBytes = Marshal.SizeOf(new int2());

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
        public static int2 Add(int2 a, int2 b)
        {
            var result = new int2(a.x + b.x, a.y + b.y);

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
        public static int2 Subtract(int2 a, int2 b)
        {
            var result = new int2(a.x - b.x, a.y - b.y);

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
        public static int2 Multiply(int2 vector, int scale)
        {
            var result = new int2(vector.x * scale, vector.y * scale);
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
        public static int2 Multiply(int2 vector, int2 scale)
        {
            var result = new int2(vector.x * scale.x, vector.y * scale.y);
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
        public static int2 Divide(int2 vector, int scale)
        {
            var result = new int2(vector.x / scale, vector.y / scale);

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
        public static int2 Divide(int2 vector, int2 scale)
        {
            var result = new int2(vector.x / scale.x, vector.y / scale.y);

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
        public static int2 ComponentMin(int2 a, int2 b)
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
        public static int2 ComponentMax(int2 a, int2 b)
        {
            a.x = a.x > b.x ? a.x : b.x;
            a.y = a.y > b.y ? a.y : b.y;
            return a;
        }

        #endregion ComponentMax

        #region Min

        /// <summary>
        /// Returns the int3 with the minimum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>
        /// The minimum int3
        /// </returns>
        public static int2 Min(int2 left, int2 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        #endregion Min

        #region Max

        /// <summary>
        /// Returns the int3 with the minimum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>
        /// The minimum int3
        /// </returns>
        public static int2 Max(int2 left, int2 right)
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
        public static int2 Clamp(int2 vec, int2 min, int2 max)
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
        /// <returns>The normalized vector</returns>
        public static float2 Normalize(int2 vec)
        {
            float scale = 1.0f / vec.Length;

            return new float2()
            {
                x = vec.x * scale,
                y = vec.y * scale
            };
        }
        #endregion Normalize

        #region NormalizeFast

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>The normalized vector</returns>
        public static float2 NormalizeFast(int2 vec)
        {
            float scale = M.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y);
            return new float2()
            {
                x = vec.x * scale,
                y = vec.y * scale
            };
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
        public static int Dot(int2 left, int2 right)
        {
            return left.x * right.x + left.y * right.y;
        }

        #endregion Dot

        /// <summary>
        /// Performs <see cref="M.Step(float, float)"/> for each component of the input vectors.
        /// </summary>
        /// <param name="edge">Specifies the location of the edge of the step function.</param>
        /// <param name="val">Specifies the value to be used to generate the step function.</param>
        public static int2 Step(int2 edge, int2 val)
        {
            return new int2((int)M.Step(edge.x, val.x), (int)M.Step(edge.y, val.y));
        }

        /// <summary>
        /// Returns a int2 where all components are raised to the specified power.
        /// </summary>
        /// <param name="val">The int3 to be raised to a power.</param>
        /// <param name="exp">A int that specifies a power.</param>
        /// <returns></returns>
        public static int2 Pow(int2 val, int exp)
        {
            return new int2((int)MathF.Pow(val.r, exp), (int)MathF.Pow(val.g, exp));
        }

        #region Lerp

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// Each component of vector a is blended with its equivalent in vector b.
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>       
        public static float2 Lerp(int2 a, int2 b, float blend)
        {
            return new float2()
            {
                x = blend * (b.x - a.x) + a.x,
                y = blend * (b.y - a.y) + a.y
            };
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// Each component of vector a is blended with its equivalent in vector b.
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>       
        public static float2 Lerp(int2 a, int2 b, float2 blend)
        {
            return new float2()
            {
                x = blend.x * (b.x - a.x) + a.x,
                y = blend.y * (b.y - a.y) + a.y
            };
        }
        #endregion Lerp

        #endregion Static

        #region Swizzle

        /// <summary>
        /// Gets and sets an OpenTK.int2 with the x and y components of this instance.
        /// </summary>
        public int2 xy { get => new(x, y); set { x = value.x; y = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.int2 with the y and x components of this instance.
        /// </summary>
        public int2 yx { get => new(y, x); set { y = value.x; x = value.y; } }

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
        public static int2 operator +(int2 left, int2 right)
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
        public static int2 operator +(int2 left, int scalar)
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
        public static int2 operator -(int2 left, int2 right)
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
        public static int2 operator -(int2 left, int scalar)
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
        public static int2 operator -(int2 vec)
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
        public static int2 operator *(int2 vec, int scale)
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
        public static int2 operator *(int scale, int2 vec)
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
        public static int2 operator *(int2 vec1, int2 vec2)
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
        public static int2 operator /(int2 vec, int scale)
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
        public static bool operator ==(int2 left, int2 right)
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
        public static bool operator !=(int2 left, int2 right)
        {
            return !left.Equals(right);
        }

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current int2.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current int2.
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
            if (!(obj is int2))
                return false;

            return this.Equals((int2)obj);
        }

        #endregion public override bool Equals(object obj)

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
        /// The rg component (same as xy)
        /// </summary>
        public int2 rg
        {
            get => xy;
            set => xy = value;
        }

        #endregion Color

        #endregion Public Members

        #region IEquatable<int2> Members

        /// <summary>
        /// Indicates whether the current vector is equal to another vector.
        /// </summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>
        /// true if the current vector is equal to the vector parameter; otherwise, false.
        /// </returns>
        public bool Equals(int2 other)
        {
            return
                x == other.x &&
                y == other.y;
        }

        #endregion IEquatable<int2> Members

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a int2.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, int2> ParseConverter { get; set; } = (x => int2.Parse(x));

        /// <summary>
        /// Parses a string into a int2.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static int2 Parse(string source, IFormatProvider? provider = null)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            char separator = M.GetNumericListSeparator(provider);

            string[] strings = source.Split(new char[] { separator, '(', ')', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length != 2)
                throw new FormatException("String parse for int2 did not result in exactly 2 items.");

            int[] ints = new int[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                try
                {
                    ints[i] = int.Parse(strings[i], provider);
                }
                catch
                {
                    throw new FormatException();
                }
            }

            return new int2(ints[0], ints[1]);
        }
    }
}