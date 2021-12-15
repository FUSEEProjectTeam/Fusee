using ProtoBuf;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>Represents a 4D vector using four single-precision inting-point numbers.</summary>
    /// <remarks>
    /// The int4 structure is suitable for interoperation with unmanaged code requiring four consecutive ints.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [ProtoContract]
    public struct int4 : IEquatable<int4>
    {
        #region Fields

        /// <summary>
        /// The x component of the int4.
        /// </summary>
        [ProtoMember(1)]
        public int x;

        /// <summary>
        /// The y component of the int4.
        /// </summary>
        [ProtoMember(2)]
        public int y;

        /// <summary>
        /// The z component of the int4.
        /// </summary>
        [ProtoMember(3)]
        public int z;

        /// <summary>
        /// The w component of the int4.
        /// </summary>
        [ProtoMember(4)]
        public int w;

        /// <summary>
        /// Defines a unit-length int4 that points towards the x-axis.
        /// </summary>
        public static readonly int4 UnitX = new(1, 0, 0, 0);

        /// <summary>
        /// Defines a unit-length int4 that points towards the y-axis.
        /// </summary>
        public static readonly int4 UnitY = new(0, 1, 0, 0);

        /// <summary>
        /// Defines a unit-length int4 that points towards the z-axis.
        /// </summary>
        public static readonly int4 UnitZ = new(0, 0, 1, 0);

        /// <summary>
        /// Defines a unit-length int4 that points towards the w-axis.
        /// </summary>
        public static readonly int4 UnitW = new(0, 0, 0, 1);

        /// <summary>
        /// Defines a zero-length int4.
        /// </summary>
        public static readonly int4 Zero = new(0, 0, 0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly int4 One = new(1, 1, 1, 1);

        // <summary>
        // Defines the size of the int4 struct in bytes.
        // </summary>
        // public static readonly int SizeInBytes = Marshal.SizeOf(new int4());

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new int4.
        /// </summary>
        /// <param name="val">This value will be set for the x, y, z and w component.</param>
        public int4(int val)
        {
            x = val;
            y = val;
            z = val;
            w = val;
        }

        /// <summary>
        /// Constructs a new int4.
        /// </summary>
        /// <param name="x">The x component of the int4.</param>
        /// <param name="y">The y component of the int4.</param>
        /// <param name="z">The z component of the int4.</param>
        /// <param name="w">The w component of the int4.</param>
        public int4(int x, int y, int z, int w) : this()
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// Constructs a new int4 from the given int2.
        /// </summary>
        /// <param name="v">The int2 to copy components from.</param>
        public int4(int2 v)
        {
            x = v.x;
            y = v.y;
            z = 0;
            w = 0;
        }

        /// <summary>
        /// Constructs a new int4 from the given int3.
        /// </summary>
        /// <param name="v">The int3 to copy components from.</param>
        public int4(int3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = 0;
        }

        /// <summary>
        /// Constructs a new int4 from the specified int3 and ww component.
        /// </summary>
        /// <param name="v">The int3 to copy components from.</param>
        /// <param name="ww">The ww component of the new int4.</param>
        public int4(int3 v, int ww)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = ww;
        }

        /// <summary>
        /// Constructs a new int4 from the given int4.
        /// </summary>
        /// <param name="v">The int4 to copy components from.</param>
        public int4(int4 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = v.w;
        }

        /// <summary>
        /// Constructs a new int4 by converting from a double4.
        /// </summary>
        /// <param name="d4">The double4 to copy components from.</param>
        public int4(double4 d4)
        {
            x = (int)d4.x;
            y = (int)d4.y;
            z = (int)d4.z;
            w = (int)d4.w;
        }

        #endregion Constructors

        #region Public Members

        #region this

        /// <summary>
        /// Gets or sets the individual components x, y, z, or w, depending on their index.
        /// </summary>
        /// <param name="idx">The index (between 0 and 3).</param>
        /// <returns>The x or y component of the int4.</returns>
        public int this[int idx]
        {
            get
            {
                return idx switch
                {
                    0 => x,
                    1 => y,
                    2 => z,
                    3 => w,
                    _ => throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a int4 type"),
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
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a int4 type");
                }
            }
        }

        #endregion this

        #region Instance

        #region public int Length

        /// <summary>
        /// Gets the length (magnitude) of the vector.
        /// </summary>
        /// <see cref="LengthSquared"/>
        public float Length => (float)System.Math.Sqrt(LengthSquared);

        #endregion public int Length

        #region public int Length1

        /// <summary>
        /// Gets the length in 1-norm.
        /// </summary>
        /// <see cref="LengthSquared"/>
        public int Length1 => System.Math.Abs(x) + System.Math.Abs(y) + System.Math.Abs(z) + System.Math.Abs(w);

        #endregion public int Length1

        #region public int LengthSquared

        /// <summary>
        /// Gets the square of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property avoids the costly square root operation required by the Length property. This makes it more suitable
        /// for comparisons.
        /// </remarks>
        /// <see cref="Length"/>
        public int LengthSquared => x * x + y * y + z * z + w * w;

        #endregion public int LengthSquared

        #region public Normalize()

        /// <summary>
        /// Scales the int4 to unit length.
        /// </summary>
        public float4 Normalize()
        {
            return Normalize(this);
        }

        #endregion public Normalize()

        #region public Normalize1()

        /// <summary>
        /// Scales the int4 to unit length in 1-norm.
        /// </summary>
        public float4 Normalize1()
        {
            return Normalize1(this);
        }

        #endregion public Normalize1()

        #region public NormalizeFast()

        /// <summary>
        /// Scales the int4 to approximately unit length.
        /// </summary>
        public float4 NormalizeFast()
        {
            return NormalizeFast(this);
        }

        #endregion public NormalizeFast()

        #region public int[] ToArray()

        /// <summary>
        /// XML-Comment
        /// </summary>
        /// <returns>An int array of size 4 that cobtains the x,y,z,w components.</returns>
        public int[] ToArray()
        {
            return new int[] { x, y, z, w };
        }

        #endregion public int[] ToArray()

        /// <summary>
        /// Converts this int4 - which is interpreted as a color - from sRgb space to linear color space.       
        /// </summary>
        /// <returns></returns>
        public int4 LinearColorFromSRgb()
        {
            return LinearColorFromSRgb(this);
        }

        /// <summary>
        /// Converts this int4 - which is interpreted as a color - from linear color space to sRgb space.
        /// </summary>
        /// <returns></returns>
        public int4 SRgbFromLinearColor()
        {
            return SRgbFromLinearColor(this);
        }

        #endregion Instance

        #region Static

        #region Add

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <returns>Result of operation.</returns>
        public static int4 Add(int4 a, int4 b)
        {
            var result = new int4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
            return result;
        }

        #endregion Add

        #region Subtract

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result of subtraction</returns>
        public static int4 Subtract(int4 a, int4 b)
        {
            var result = new int4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
            return result;
        }

        #endregion Subtract

        #region Multiply

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static int4 Multiply(int4 vector, int scale)
        {
            var result = new int4(vector.x * scale, vector.y * scale, vector.z * scale, vector.w * scale);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by the components a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static int4 Multiply(int4 vector, int4 scale)
        {
            var result = new int4(vector.x * scale.x, vector.y * scale.y, vector.z * scale.z, vector.w * scale.w);
            return result;
        }

        #endregion Multiply

        #region Divide

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static int4 Divide(int4 vector, int scale)
        {
            var result = new int4(vector.x / scale, vector.y / scale, vector.z / scale, vector.w / scale);
            return result;
        }

        /// <summary>
        /// Divides a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static int4 Divide(int4 vector, int4 scale)
        {
            var result = new int4(vector.x / scale.x, vector.y / scale.y, vector.z / scale.z, vector.w / scale.w);
            return result;
        }

        #endregion Divide

        #region Min

        /// <summary>
        /// Calculate the component-wise minimum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>The component-wise minimum</returns>
        public static int4 Min(int4 a, int4 b)
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
        /// <returns>The component-wise maximum</returns>
        public static int4 Max(int4 a, int4 b)
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
        /// <returns>The clamped vector</returns>
        public static int4 Clamp(int4 vec, int4 min, int4 max)
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
        /// <returns>The normalized vector</returns>
        public static float4 Normalize(int4 vec)
        {
            float scale = 1.0f / vec.Length;

            return new float4()
            {
                x = vec.x * scale,
                y = vec.y * scale,
                z = vec.z * scale,
                w = vec.w * scale
            };
        }

        #endregion Normalize

        #region Normalize1

        /// <summary>
        /// Scales the vector to unit length in 1-norm.
        /// </summary>
        /// <param name="vec">The input vector.</param>
        /// <returns>The scaled vector.</returns>
        public static float4 Normalize1(int4 vec)
        {
            float scale = 1.0f / vec.Length1;

            return new float4()
            {
                x = vec.x * scale,
                y = vec.y * scale,
                z = vec.z * scale,
                w = vec.w * scale
            };
        }

        #endregion Normalize1

        #region NormalizeFast

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>The normalized vector</returns>
        public static float4 NormalizeFast(int4 vec)
        {
            float scale = M.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w);
            return new float4()
            {
                x = vec.x * scale,
                y = vec.y * scale,
                z = vec.z * scale,
                w = vec.w * scale
            };
        }

        #endregion NormalizeFast

        #region Dot

        /// <summary>
        /// Calculate the dot product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>The dot product of the two inputs</returns>
        public static int Dot(int4 left, int4 right)
        {
            return left.x * right.x + left.y * right.y + left.z * right.z + left.w * right.w;
        }

        #endregion Dot

        /// <summary>
        /// Performs <see cref="M.Step(float, float)"/> for each component of the input vectors.
        /// </summary>
        /// <param name="edge">Specifies the location of the edge of the step function.</param>
        /// <param name="val">Specifies the value to be used to generate the step function.</param>
        public static int4 Step(int4 edge, int4 val)
        {
            return new int4((int)M.Step(edge.x, val.x), (int)M.Step(edge.y, val.y), (int)M.Step(edge.z, val.z), (int)M.Step(edge.w, val.w));
        }

        /// <summary>
        /// Returns a int4 where all components are raised to the specified power.
        /// </summary>
        /// <param name="val">The int4 to be raised to a power.</param>
        /// <param name="exp">A int that specifies a power.</param>
        /// <returns></returns>
        public static int4 Pow(int4 val, int exp)
        {
            return new int4((int)MathF.Pow(val.x, exp), (int)MathF.Pow(val.y, exp), (int)MathF.Pow(val.z, exp), (int)MathF.Pow(val.w, exp));
        }

        #region Lerp

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
        public static float4 Lerp(int4 a, int4 b, float blend)
        {
            return new float4()
            {
                x = blend * (b.x - a.x) + a.x,
                y = blend * (b.y - a.y) + a.y,
                z = blend * (b.z - a.z) + a.z,
                w = blend * (b.w - a.w) + a.w
            };
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// Each component of vector a is blended with its equivalent in vector b.
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>       
        public static float4 Lerp(int4 a, int4 b, float4 blend)
        {
            return new float4()
            {
                x = blend.x * (b.x - a.x) + a.x,
                y = blend.y * (b.y - a.y) + a.y,
                z = blend.z * (b.z - a.z) + a.z,
                w = blend.w * (b.w - a.w) + a.w
            };
        }

        #endregion Lerp

        #region Color Conversion

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="sRGBCol">The linear color value as <see cref="int4"/>.</param>
        public static int4 SRgbFromLinearColor(int4 sRGBCol)
        {
            return new int4(int3.SRgbFromLinearColor(sRGBCol.rgb), sRGBCol.a);
        }

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="r">The red color value in range 0 - 255.</param>
        /// <param name="g">The green color value in range 0 - 255.</param>
        /// <param name="b">The blue color value in range 0 - 255.</param>
        /// <param name="a">The alpha value in range 0 - 255.</param>
        public static int4 SRgbFromLinearColor(int r, int g, int b, int a)
        {
            return new int4(int3.SRgbFromLinearColor(r, g, b), a);
        }

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="hex">The color value as hex code in form of a "FFFFFFFF" string.</param>
        public static int4 SRgbFromLinearColor(string hex)
        {
            var rgba = Convert.ToUInt32(hex, 16);
            return SRgbFromLinearColor(rgba);
        }

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="col">The color value as uint.</param>
        public static int4 SRgbFromLinearColor(uint col)
        {
            var a = (byte)(col & byte.MaxValue);
            var b = (byte)(col >> 8 & byte.MaxValue);
            var g = (byte)(col >> 16 & byte.MaxValue);
            var r = (byte)(col >> 24 & byte.MaxValue);
            return SRgbFromLinearColor(r, g, b, a);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="sRGBCol">The sRgb color value as <see cref="int4"/>.</param>
        public static int4 LinearColorFromSRgb(int4 sRGBCol)
        {
            return new int4(int3.LinearColorFromSRgb(sRGBCol.rgb), sRGBCol.a);
        }

        /// <summary>
        ///Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="r">The red color value in range 0 - 255.</param>
        /// <param name="g">The green color value in range 0 - 255.</param>
        /// <param name="b">The blue color value in range 0 - 255.</param>
        /// <param name="a">The alpha value in range 0 - 255.</param>
        public static int4 LinearColorFromSRgb(int r, int g, int b, int a)
        {
            return new int4(int3.LinearColorFromSRgb(r, g, b), a);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="hex">The color value as hex code in form of a "FFFFFFFF" string.</param>
        public static int4 LinearColorFromSRgb(string hex)
        {
            var rgba = Convert.ToUInt32(hex, 16);
            return LinearColorFromSRgb(rgba);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="col">The color value as uint.</param>
        public static int4 LinearColorFromSRgb(uint col)
        {
            var a = (byte)(col & byte.MaxValue);
            var b = (byte)(col >> 8 & byte.MaxValue);
            var g = (byte)(col >> 16 & byte.MaxValue);
            var r = (byte)(col >> 24 & byte.MaxValue);
            return LinearColorFromSRgb(r, g, b, a);
        }

        #endregion

        #endregion Static

        #region Swizzle

        /// <summary>
        /// Gets and sets an OpenTK.int2 with the x and y components of this instance.
        /// </summary>
        public int2 xy { get => new(x, y); set { x = value.x; y = value.y; } }

        /// <summary>
        /// Gets and sets an OpenTK.int3 with the x, y and z components of this instance.
        /// </summary>
        public int3 xyz { get => new(x, y, z); set { x = value.x; y = value.y; z = value.z; } }

        #endregion Swizzle

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static int4 operator +(int4 left, int4 right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static int4 operator -(int4 left, int4 right)
        {
            return Subtract(left, right);
        }

        /// <summary>
        /// Negates an instance.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static int4 operator -(int4 vec)
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
        /// <returns>The result of the calculation.</returns>
        public static int4 operator *(int4 vec, int scale)
        {
            return Multiply(vec, scale);
        }

        /// <summary>
        /// Multiplies two instances (componentwise).
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static int4 operator *(int4 left, int4 right)
        {
            return Multiply(left, right);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="scale">The scalar.</param>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static int4 operator *(int scale, int4 vec)
        {
            return Multiply(vec, scale);
        }

        /// <summary>
        /// Divides an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>The result of the calculation.</returns>
        public static int4 operator /(int4 vec, int scale)
        {
            return Divide(vec, scale);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(int4 left, int4 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(int4 left, int4 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Explicit cast operator to cast a double4 into a int4 value.
        /// </summary>
        /// <param name="d4">The double4 value to cast.</param>
        /// <returns>A int4 value.</returns>
        public static explicit operator int4(double4 d4)
        {
            return new int4(d4);
        }

        /*
        /// <summary>
        /// Returns a pointer to the first element of the specified instance.
        /// </summary>
        /// <param name="v">The instance.</param>
        /// <returns>A pointer to the first element of v.</returns>
        [CLSCompliant(false)]
        unsafe public static explicit operator int*(int4 v)
        {
            return &v.x;
        }

        /// <summary>
        /// Returns a pointer to the first element of the specified instance.
        /// </summary>
        /// <param name="v">The instance.</param>
        /// <returns>A pointer to the first element of v.</returns>
        public static explicit operator IntPtr(int4 v)
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
        /// Returns a System.String that represents the current int4.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current int4.
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
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
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
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object? obj)
        {
            if (!(obj is int4))
                return false;

            return this.Equals((int4)obj);
        }

        #endregion public override bool Equals(object obj)

        #endregion Overrides

        #region Color Swizzle

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

        /// <summary>
        /// The rg component (same as xy)
        /// </summary>
        public int2 rg
        {
            get => xy;
            set => xy = value;
        }

        /// <summary>
        /// The rgb component (same as xyz)
        /// </summary>
        public int3 rgb
        {
            get => xyz;
            set => xyz = value;
        }

        /// <summary>
        /// The alpha component (same as w)
        /// </summary>
        public int a
        {
            get => w;
            set => w = value;
        }

        #endregion Color

        #endregion Public Members

        #region IEquatable<int4> Members

        /// <summary>Indicates whether the current vector is equal to another vector.</summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
        public bool Equals(int4 other)
        {
            return
                x == other.x &&
                y == other.y &&
                z == other.z &&
                w == other.w;
        }

        #endregion IEquatable<int4> Members

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a int4.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, int4> ParseConverter { get; set; } = (x => int4.Parse(x));

        /// <summary>
        /// Parses a string into a int4.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static int4 Parse(string source, IFormatProvider? provider = null)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            char separator = M.GetNumericListSeparator(provider);

            string[] strings = source.Split(new char[] { separator, '(', ')', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length != 4)
                throw new FormatException("String parse for int4 did not result in exactly 4 items.");

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

            return new int4(ints[0], ints[1], ints[2], ints[3]);
        }
    }
}