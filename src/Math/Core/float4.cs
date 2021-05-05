using ProtoBuf;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>Represents a 4D vector using four single-precision floating-point numbers.</summary>
    /// <remarks>
    /// The float4 structure is suitable for interoperation with unmanaged code requiring four consecutive floats.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [ProtoContract]
    public struct float4 : IEquatable<float4>
    {
        #region Fields

        /// <summary>
        /// The x component of the float4.
        /// </summary>
        [ProtoMember(1)]
        public float x;

        /// <summary>
        /// The y component of the float4.
        /// </summary>
        [ProtoMember(2)]
        public float y;

        /// <summary>
        /// The z component of the float4.
        /// </summary>
        [ProtoMember(3)]
        public float z;

        /// <summary>
        /// The w component of the float4.
        /// </summary>
        [ProtoMember(4)]
        public float w;

        /// <summary>
        /// Defines a unit-length float4 that points towards the x-axis.
        /// </summary>
        public static float4 UnitX = new float4(1, 0, 0, 0);

        /// <summary>
        /// Defines a unit-length float4 that points towards the y-axis.
        /// </summary>
        public static float4 UnitY = new float4(0, 1, 0, 0);

        /// <summary>
        /// Defines a unit-length float4 that points towards the z-axis.
        /// </summary>
        public static float4 UnitZ = new float4(0, 0, 1, 0);

        /// <summary>
        /// Defines a unit-length float4 that points towards the w-axis.
        /// </summary>
        public static float4 UnitW = new float4(0, 0, 0, 1);

        /// <summary>
        /// Defines a zero-length float4.
        /// </summary>
        public static float4 Zero = new float4(0, 0, 0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly float4 One = new float4(1, 1, 1, 1);

        // <summary>
        // Defines the size of the float4 struct in bytes.
        // </summary>
        // public static readonly int SizeInBytes = Marshal.SizeOf(new float4());

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new float4.
        /// </summary>
        /// <param name="val">This value will be set for the x, y, z and w component.</param>
        public float4(float val)
        {
            x = val;
            y = val;
            z = val;
            w = val;
        }

        /// <summary>
        /// Constructs a new float4.
        /// </summary>
        /// <param name="x">The x component of the float4.</param>
        /// <param name="y">The y component of the float4.</param>
        /// <param name="z">The z component of the float4.</param>
        /// <param name="w">The w component of the float4.</param>
        public float4(float x, float y, float z, float w) : this()
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// Constructs a new float4 from the given float2.
        /// </summary>
        /// <param name="v">The float2 to copy components from.</param>
        public float4(float2 v)
        {
            x = v.x;
            y = v.y;
            z = 0.0f;
            w = 0.0f;
        }

        /// <summary>
        /// Constructs a new float4 from the given float3.
        /// </summary>
        /// <param name="v">The float3 to copy components from.</param>
        public float4(float3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = 0.0f;
        }

        /// <summary>
        /// Constructs a new float4 from the specified float3 and ww component.
        /// </summary>
        /// <param name="v">The float3 to copy components from.</param>
        /// <param name="ww">The ww component of the new float4.</param>
        public float4(float3 v, float ww)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = ww;
        }

        /// <summary>
        /// Constructs a new float4 from the given float4.
        /// </summary>
        /// <param name="v">The float4 to copy components from.</param>
        public float4(float4 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = v.w;
        }

        /// <summary>
        /// Constructs a new float4 by converting from a double4.
        /// </summary>
        /// <param name="d4">The double4 to copy components from.</param>
        public float4(double4 d4)
        {
            x = (float)d4.x;
            y = (float)d4.y;
            z = (float)d4.z;
            w = (float)d4.w;
        }

        #endregion Constructors

        #region Public Members

        #region this

        /// <summary>
        /// Gets or sets the individual components x, y, z, or w, depending on their index.
        /// </summary>
        /// <param name="idx">The index (between 0 and 3).</param>
        /// <returns>The x or y component of the float4.</returns>
        public float this[int idx]
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

                    case 3:
                        return w;

                    default:
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a float4 type");
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

                    case 3:
                        w = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a float4 type");
                }
            }
        }

        #endregion this

        #region Instance

        #region public float Length

        /// <summary>
        /// Gets the length (magnitude) of the vector.
        /// </summary>
        /// <see cref="LengthSquared"/>
        public float Length => (float)System.Math.Sqrt(LengthSquared);

        #endregion public float Length

        #region public float Length1

        /// <summary>
        /// Gets the length in 1-norm.
        /// </summary>
        /// <see cref="LengthSquared"/>
        public float Length1 => (float)System.Math.Abs(x) + System.Math.Abs(y) + System.Math.Abs(z) + System.Math.Abs(w);

        #endregion public float Length1

        #region public float LengthSquared

        /// <summary>
        /// Gets the square of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property avoids the costly square root operation required by the Length property. This makes it more suitable
        /// for comparisons.
        /// </remarks>
        /// <see cref="Length"/>
        public float LengthSquared => x * x + y * y + z * z + w * w;

        #endregion public float LengthSquared

        #region public Normalize()

        /// <summary>
        /// Scales the float4 to unit length.
        /// </summary>
        public float4 Normalize()
        {
            return Normalize(this);
        }

        #endregion public Normalize()

        #region public Normalize1()

        /// <summary>
        /// Scales the float4 to unit length in 1-norm.
        /// </summary>
        public float4 Normalize1()
        {
            return Normalize1(this);
        }

        #endregion public Normalize1()

        #region public NormalizeFast()

        /// <summary>
        /// Scales the float4 to approximately unit length.
        /// </summary>
        public float4 NormalizeFast()
        {
            return NormalizeFast(this);
        }

        #endregion public NormalizeFast()

        #region public float[] ToArray()

        /// <summary>
        /// XML-Comment
        /// </summary>
        /// <returns>An float array of size 4 that cobtains the x,y,z,w components.</returns>
        public float[] ToArray()
        {
            return new float[] { x, y, z, w };
        }

        #endregion public float[] ToArray()

        #region public Round()

        /// <summary>
        /// Rounds the float4 to 6 digits (max float precision).
        /// </summary>
        public float4 Round()
        {
            return Round(this);
        }

        #endregion public Round()

        /// <summary>
        /// Converts this float4 - which is interpreted as a color - from sRgb space to linear color space.       
        /// </summary>
        /// <returns></returns>
        public float4 LinearColorFromSRgb()
        {
            return LinearColorFromSRgb(this);
        }

        /// <summary>
        /// Converts this float4 - which is interpreted as a color - from linear color space to sRgb space.
        /// </summary>
        /// <returns></returns>
        public float4 SRgbFromLinearColor()
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
        public static float4 Add(float4 a, float4 b)
        {
            var result = new float4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
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
        public static float4 Subtract(float4 a, float4 b)
        {
            var result = new float4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
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
        public static float4 Multiply(float4 vector, float scale)
        {
            var result = new float4(vector.x * scale, vector.y * scale, vector.z * scale, vector.w * scale);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by the components a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static float4 Multiply(float4 vector, float4 scale)
        {
            var result = new float4(vector.x * scale.x, vector.y * scale.y, vector.z * scale.z, vector.w * scale.w);
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
        public static float4 Divide(float4 vector, float scale)
        {
            var result = new float4(vector.x / scale, vector.y / scale, vector.z / scale, vector.w / scale);
            return result;
        }

        /// <summary>
        /// Divides a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static float4 Divide(float4 vector, float4 scale)
        {
            var result = new float4(vector.x / scale.x, vector.y / scale.y, vector.z / scale.z, vector.w / scale.w);
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
        public static float4 Min(float4 a, float4 b)
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
        public static float4 Max(float4 a, float4 b)
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
        public static float4 Clamp(float4 vec, float4 min, float4 max)
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
        public static float4 Normalize(float4 vec)
        {
            float scale = 1.0f / vec.Length;
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        #endregion Normalize

        #region Normalize1

        /// <summary>
        /// Scales the vector to unit length in 1-norm.
        /// </summary>
        /// <param name="vec">The input vector.</param>
        /// <returns>The scaled vector.</returns>
        public static float4 Normalize1(float4 vec)
        {
            float scale = 1.0f / vec.Length1;
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        #endregion Normalize1

        #region NormalizeFast

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>The normalized vector</returns>
        public static float4 NormalizeFast(float4 vec)
        {
            float scale = M.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w);
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
        /// <returns>The dot product of the two inputs</returns>
        public static float Dot(float4 left, float4 right)
        {
            return left.x * right.x + left.y * right.y + left.z * right.z + left.w * right.w;
        }

        #endregion Dot

        /// <summary>
        /// Performs <see cref="M.Step(float, float)"/> for each component of the input vectors.
        /// </summary>
        /// <param name="edge">Specifies the location of the edge of the step function.</param>
        /// <param name="val">Specifies the value to be used to generate the step function.</param>
        public static float4 Step(float4 edge, float4 val)
        {
            return new float4(M.Step(edge.x, val.x), M.Step(edge.y, val.y), M.Step(edge.z, val.z), M.Step(edge.w, val.w));
        }

        /// <summary>
        /// Returns a float4 where all components are raised to the specified power.
        /// </summary>
        /// <param name="val">The float4 to be raised to a power.</param>
        /// <param name="exp">A float that specifies a power.</param>
        /// <returns></returns>
        public static float4 Pow(float4 val, float exp)
        {
            return new float4(MathF.Pow(val.x, exp), MathF.Pow(val.y, exp), MathF.Pow(val.z, exp), MathF.Pow(val.w, exp));
        }

        #region Lerp

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
        public static float4 Lerp(float4 a, float4 b, float blend)
        {
            a.x = blend * (b.x - a.x) + a.x;
            a.y = blend * (b.y - a.y) + a.y;
            a.z = blend * (b.z - a.z) + a.z;
            a.w = blend * (b.w - a.w) + a.w;
            return a;
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// Each component of vector a is blended with its equivalent in vector b.
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>       
        public static float4 Lerp(float4 a, float4 b, float4 blend)
        {
            a.x = blend.x * (b.x - a.x) + a.x;
            a.y = blend.y * (b.y - a.y) + a.y;
            a.z = blend.z * (b.z - a.z) + a.z;
            a.w = blend.w * (b.w - a.w) + a.w;
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
        /// <returns>a when u=1, v=0, b when v=1,u=0, c when u=v=0, and a linear combination of a,b,c otherwise</returns>
        public static float4 BaryCentric(float4 a, float4 b, float4 c, float u, float v)
        {
            return u * a + v * b + (1.0f - u - v) * c;
        }

        #endregion Barycentric

        #region Round

        /// <summary>
        /// Rounds a vector to 6 digits (max float precision).
        /// </summary>
        /// <param name="vec">The input vector.</param>
        /// <returns>The rounded vector.</returns>
        public static float4 Round(float4 vec)
        {
            return new float4((float)System.Math.Round(vec.x, 6),
                              (float)System.Math.Round(vec.y, 6),
                              (float)System.Math.Round(vec.z, 6),
                              (float)System.Math.Round(vec.w, 6));
        }

        #endregion Round

        #region Color Conversion

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="sRGBCol">The linear color value as <see cref="float4"/>.</param>
        public static float4 SRgbFromLinearColor(float4 sRGBCol)
        {
            return new float4(float3.SRgbFromLinearColor(sRGBCol.rgb), sRGBCol.a);
        }

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="r">The red color value in range 0 - 255.</param>
        /// <param name="g">The green color value in range 0 - 255.</param>
        /// <param name="b">The blue color value in range 0 - 255.</param>
        /// <param name="a">The alpha value in range 0 - 255.</param>
        public static float4 SRgbFromLinearColor(int r, int g, int b, int a)
        {
            return new float4(float3.SRgbFromLinearColor(r, g, b), a / 255f);
        }

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="hex">The color value as hex code in form of a "FFFFFFFF" string.</param>
        public static float4 SRgbFromLinearColor(string hex)
        {
            var rgba = Convert.ToUInt32(hex, 16);
            return SRgbFromLinearColor(rgba);
        }

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="col">The color value as uint.</param>
        public static float4 SRgbFromLinearColor(uint col)
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
        /// <param name="sRGBCol">The sRgb color value as <see cref="float4"/>.</param>
        public static float4 LinearColorFromSRgb(float4 sRGBCol)
        {
            return new float4(float3.LinearColorFromSRgb(sRGBCol.rgb), sRGBCol.a);
        }

        /// <summary>
        ///Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="r">The red color value in range 0 - 255.</param>
        /// <param name="g">The green color value in range 0 - 255.</param>
        /// <param name="b">The blue color value in range 0 - 255.</param>
        /// <param name="a">The alpha value in range 0 - 255.</param>
        public static float4 LinearColorFromSRgb(int r, int g, int b, int a)
        {
            return new float4(float3.LinearColorFromSRgb(r, g, b), a / 255f);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="hex">The color value as hex code in form of a "FFFFFFFF" string.</param>
        public static float4 LinearColorFromSRgb(string hex)
        {
            var rgba = Convert.ToUInt32(hex, 16);
            return LinearColorFromSRgb(rgba);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="col">The color value as uint.</param>
        public static float4 LinearColorFromSRgb(uint col)
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
        /// Gets and sets an OpenTK.float2 with the x and y components of this instance.
        /// </summary>
        public float2 xy { get => new float2(x, y); set { x = value.x; y = value.y; } }

        /// <summary>
        /// Gets and sets an OpenTK.float3 with the x, y and z components of this instance.
        /// </summary>
        public float3 xyz { get => new float3(x, y, z); set { x = value.x; y = value.y; z = value.z; } }

        #endregion Swizzle

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static float4 operator +(float4 left, float4 right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static float4 operator -(float4 left, float4 right)
        {
            return Subtract(left, right);
        }

        /// <summary>
        /// Negates an instance.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static float4 operator -(float4 vec)
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
        public static float4 operator *(float4 vec, float scale)
        {
            return Multiply(vec, scale);
        }

        /// <summary>
        /// Multiplies two instances (componentwise).
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static float4 operator *(float4 left, float4 right)
        {
            return Multiply(left, right);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="scale">The scalar.</param>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static float4 operator *(float scale, float4 vec)
        {
            return Multiply(vec, scale);
        }

        /// <summary>
        /// Divides an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>The result of the calculation.</returns>
        public static float4 operator /(float4 vec, float scale)
        {
            return Divide(vec, scale);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(float4 left, float4 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(float4 left, float4 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Explicit cast operator to cast a double4 into a float4 value.
        /// </summary>
        /// <param name="d4">The double4 value to cast.</param>
        /// <returns>A float4 value.</returns>
        public static explicit operator float4(double4 d4)
        {
            return new float4(d4);
        }

        /*
        /// <summary>
        /// Returns a pointer to the first element of the specified instance.
        /// </summary>
        /// <param name="v">The instance.</param>
        /// <returns>A pointer to the first element of v.</returns>
        [CLSCompliant(false)]
        unsafe public static explicit operator float*(float4 v)
        {
            return &v.x;
        }

        /// <summary>
        /// Returns a pointer to the first element of the specified instance.
        /// </summary>
        /// <param name="v">The instance.</param>
        /// <returns>A pointer to the first element of v.</returns>
        public static explicit operator IntPtr(float4 v)
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
        /// Returns a System.String that represents the current float4.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current float4.
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
        public override bool Equals(object obj)
        {
            if (!(obj is float4))
                return false;

            return this.Equals((float4)obj);
        }

        #endregion public override bool Equals(object obj)

        #endregion Overrides

        #region Color Swizzle

        /// <summary>
        /// The red component (same as x)
        /// </summary>
        public float r
        {
            get => x;
            set => x = value;
        }

        /// <summary>
        /// The green component (same as y)
        /// </summary>
        public float g
        {
            get => y;
            set => y = value;
        }

        /// <summary>
        /// The blue component (same as z)
        /// </summary>
        public float b
        {
            get => z;
            set => z = value;
        }

        /// <summary>
        /// The rg component (same as xy)
        /// </summary>
        public float2 rg
        {
            get => xy;
            set => xy = value;
        }

        /// <summary>
        /// The rgb component (same as xyz)
        /// </summary>
        public float3 rgb
        {
            get => xyz;
            set => xyz = value;
        }

        /// <summary>
        /// The alpha component (same as w)
        /// </summary>
        public float a
        {
            get => w;
            set => w = value;
        }

        #endregion Color

        #endregion Public Members

        #region IEquatable<float4> Members

        /// <summary>Indicates whether the current vector is equal to another vector.</summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
        public bool Equals(float4 other)
        {
            return
                System.Math.Abs(x - other.x) < M.EpsilonFloat &&
                System.Math.Abs(y - other.y) < M.EpsilonFloat &&
                System.Math.Abs(z - other.z) < M.EpsilonFloat &&
                System.Math.Abs(w - other.w) < M.EpsilonFloat;
        }

        #endregion IEquatable<float4> Members

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a float4.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, float4> ParseConverter { get; set; } = (x => float4.Parse(x));

        /// <summary>
        /// Parses a string into a float4.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static float4 Parse(string source, IFormatProvider? provider = null)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            char separator = M.GetNumericListSeparator(provider);

            string[] strings = source.Split(new char[] { separator, '(', ')', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length != 4)
                throw new FormatException("String parse for float4 did not result in exactly 4 items.");

            float[] floats = new float[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                try
                {
                    floats[i] = float.Parse(strings[i], provider);
                }
                catch
                {
                    throw new FormatException();
                }
            }

            return new float4(floats[0], floats[1], floats[2], floats[3]);
        }
    }
}