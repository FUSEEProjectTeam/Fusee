using ProtoBuf;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a 3D vector using integers.
    /// </summary>
    /// <remarks>
    /// The int3 structure is suitable for inter-operation with unmanaged code requiring three consecutive ints.
    /// </remarks>
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct int3 : IEquatable<int3>
    {
        #region Fields

        /// <summary>
        /// The x component of the int3.
        /// </summary>
        [ProtoMember(1)]
        public int x;

        /// <summary>
        /// The y component of the int3.
        /// </summary>
        [ProtoMember(2)]
        public int y;

        /// <summary>
        /// The z component of the int3.
        /// </summary>
        [ProtoMember(3)]
        public int z;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new int3.
        /// </summary>
        /// <param name="val">This value will be set for the x, y and z component.</param>
        public int3(int val)
        {
            x = val;
            y = val;
            z = val;
        }

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
        /// Constructs a new int3 from the given int2.
        /// </summary>
        /// <param name="v">The int2 to copy components from.</param>
        public int3(int2 v)
        {
            x = v.x;
            y = v.y;
            z = 0;
        }

        /// <summary>
        /// Constructs a new int3 from the given int3.
        /// </summary>
        /// <param name="v">The int3 to copy components from.</param>
        public int3(int3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        /// <summary>
        /// Constructs a new int3 from the given int3.
        /// </summary>
        /// <param name="v">The int3 to copy components from.</param>
        public int3(int4 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        /// <summary>
        /// Constructs a new int3 by converting from a double3.
        /// </summary>
        /// <param name="d3">The double3 to copy components from.</param>
        public int3(double3 d3)
        {
            x = (int)d3.x;
            y = (int)d3.y;
            z = (int)d3.z;
        }

        /// <summary>
        /// Constructs a new int3 by converting from a double3.
        /// </summary>
        /// <param name="d3">The double3 to copy components from.</param>
        public int3(float3 d3)
        {
            x = (int)d3.x;
            y = (int)d3.y;
            z = (int)d3.z;
        }

        #endregion Constructors

        #region Public Members

        #region this

        /// <summary>
        /// Gets or sets the individual components x, y, or z, depending on their index.
        /// </summary>
        /// <param name="idx">The index (between 0 and 2).</param>
        /// <returns>The x or y component of the int3.</returns>
        public int this[int idx]
        {
            get
            {
                return idx switch
                {
                    0 => x,
                    1 => y,
                    2 => z,
                    _ => throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a int3 type"),
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

                    default:
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a int3 type");
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
        public int LengthSquared => x * x + y * y + z * z;

        #endregion public int LengthSquared

        #region public Normalize()

        /// <summary>
        /// Scales the int3 to unit length.
        /// </summary>
        public float3 Normalize()
        {
            return Normalize(this);
        }

        #endregion public Normalize()

        #region public NormalizeFast()

        /// <summary>
        /// Scales the int3 to approximately unit length.
        /// </summary>
        public float3 NormalizeFast()
        {
            return NormalizeFast(this);
        }

        #endregion public NormalizeFast()

        #region Color Conversion

        /// <summary>
        /// Converts this int3 - which is interpreted as a color - from linear color space to sRgb space.
        /// </summary>
        /// <returns></returns>
        public int3 LinearColorFromSRgb()
        {
            return LinearColorFromSRgb(this);
        }

        /// <summary>
        /// Converts this int3 - which is interpreted as a color - from sRgb space to linear color space.
        /// </summary>
        /// <returns></returns>
        public int3 SRgbFromLinearColor()
        {
            return SRgbFromLinearColor(this);
        }

        #endregion

        /// <summary>
        /// Returns an array of ints with the three components of the vector.
        /// </summary>
        /// <returns>Returns an array of ints with the three components of the vector.</returns>
        public int[] ToArray()
        {
            return new[] { x, y, z };
        }

        #endregion Instance

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length int3 that points towards the x-axis.
        /// </summary>
        public static readonly int3 UnitX = new(1, 0, 0);

        /// <summary>
        /// Defines a unit-length int3 that points towards the y-axis.
        /// </summary>
        public static readonly int3 UnitY = new(0, 1, 0);

        /// <summary>
        /// Defines a unit-length int3 that points towards the z-axis.
        /// </summary>
        public static readonly int3 UnitZ = new(0, 0, 1);

        /// <summary>
        /// Defines a zero-length int3.
        /// </summary>
        public static readonly int3 Zero = new(0, 0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly int3 One = new(1, 1, 1);

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
        /// <param name="vec">The first instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static int3 Add(int3 vec, int scale)
        {
            var result = new int3(vec.x + scale, vec.y + scale, vec.z + scale);
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
        /// <param name="vec">The first instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>
        /// The result of the calculation.
        /// </returns>
        public static int3 Subtract(int3 vec, int scale)
        {
            var result = new int3(vec.x - scale, vec.y - scale, vec.z - scale);
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

        #region Min

        /// <summary>
        /// Returns the int3 with the minimum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>
        /// The minimum int3
        /// </returns>
        public static int3 Min(int3 left, int3 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        #endregion Min

        #region Max

        /// <summary>
        /// Returns the int3 with the maximum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>
        /// The maximum int3
        /// </returns>
        public static int3 Max(int3 left, int3 right)
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
        public static int3 Clamp(int3 vec, int3 min, int3 max)
        {
            vec.x = vec.x < min.x ? min.x : vec.x > max.x ? max.x : vec.x;
            vec.y = vec.y < min.y ? min.y : vec.y > max.y ? max.y : vec.y;
            vec.z = vec.z < min.z ? min.z : vec.z > max.z ? max.z : vec.z;
            return vec;
        }

        #endregion Clamp

        #region Normalize

        /// <summary>
        /// Scale a vector to unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>The normalized vector</returns>
        public static float3 Normalize(int3 vec)
        {
            float scale = 1.0f / vec.Length;

            return new float3()
            {
                x = vec.x * scale,
                y = vec.y * scale,
                z = vec.z * scale
            };
        }
        #endregion Normalize

        #region NormalizeFast

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>The normalized vector</returns>
        public static float3 NormalizeFast(int3 vec)
        {
            float scale = M.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
            return new float3()
            {
                x = vec.x * scale,
                y = vec.y * scale,
                z = vec.z * scale,
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
        public static int Dot(int3 left, int3 right)
        {
            return left.x * right.x + left.y * right.y + left.z * right.z;
        }

        #endregion Dot

        #region Cross

        /// <summary>
        /// Calculate the cross (vector) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>
        /// The cross product of the two inputs
        /// </returns>
        public static int3 Cross(int3 left, int3 right)
        {
            int3 result = new(left.y * right.z - left.z * right.y,
                                       left.z * right.x - left.x * right.z,
                                       left.x * right.y - left.y * right.x);

            return result;
        }

        #endregion Cross

        /// <summary>
        /// Performs <see cref="M.Step(float, float)"/> for each component of the input vectors.
        /// </summary>
        /// <param name="edge">Specifies the location of the edge of the step function.</param>
        /// <param name="val">Specifies the value to be used to generate the step function.</param>
        public static int3 Step(int3 edge, int3 val)
        {
            return new int3((int)M.Step(edge.x, val.x), (int)M.Step(edge.y, val.y), (int)M.Step(edge.z, val.z));
        }

        /// <summary>
        /// Returns a int3 where all components are raised to the specified power.
        /// </summary>
        /// <param name="val">The int3 to be raised to a power.</param>
        /// <param name="exp">A int that specifies a power.</param>
        /// <returns></returns>
        public static int3 Pow(int3 val, int exp)
        {
            return new int3((int)MathF.Pow(val.r, exp), (int)MathF.Pow(val.g, exp), (int)MathF.Pow(val.b, exp));
        }

        #region Lerp

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// Each component of vector a is blended with its equivalent in vector b.
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>       
        public static float3 Lerp(int3 a, int3 b, float blend)
        {
            return new float3()
            {
                x = blend * (b.x - a.x) + a.x,
                y = blend * (b.y - a.y) + a.y,
                z = blend * (b.z - a.z) + a.z
            };
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// Each component of vector a is blended with its equivalent in vector b.
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>       
        public static float3 Lerp(int3 a, int3 b, float3 blend)
        {
            return new float3()
            {
                x = blend.x * (b.x - a.x) + a.x,
                y = blend.y * (b.y - a.y) + a.y,
                z = blend.z * (b.z - a.z) + a.z
            };
        }

        #endregion Lerp        

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
        public static float CalculateAngle(int3 first, int3 second)
        {
            if ((first.Length > 0) && (second.Length > 0))
            {
                var dotP = Dot(first, second) / (first.Length * second.Length);

                if (dotP < -1)
                    dotP = -1;
                if (dotP > 1)
                    dotP = 1;

                return (float)System.Math.Acos(dotP);
            }

            return 0;
        }

        #endregion CalculateAngle

        #region Rotate

        /// <summary>
        /// Rotates a vector by the given euler-angles in the following order: yaw (y-axis), pitch (x-axis), roll (z-axis).
        /// </summary>
        /// <param name="euler">The angles used for the rotation.</param>
        /// <param name="vec">The vector to rotate.</param>
        /// <param name="inDegrees">Optional: Whether the angles are given in degrees (true) or radians (false). Default is radians.</param>
        /// <returns>The rotated vector.</returns>
        public static float3 Rotate(float3 euler, int3 vec, bool inDegrees = false)
        {
            if (inDegrees)
            {
                euler.x = M.DegreesToRadians(euler.x);
                euler.y = M.DegreesToRadians(euler.y);
                euler.z = M.DegreesToRadians(euler.z);
            }

            float4x4 xRot = float4x4.CreateRotationX(euler.x);
            float4x4 yRot = float4x4.CreateRotationY(euler.y);
            float4x4 zRot = float4x4.CreateRotationZ(euler.z);

            var res = new float3(vec.x, vec.y, vec.z);
            res = float4x4.Transform(yRot, res);
            res = float4x4.Transform(xRot, res);
            res = float4x4.Transform(zRot, res);

            return res;
        }

        /// <summary>
        /// Rotates a vector by the given quaternion.
        /// </summary>
        /// <param name="quat">The quaternion used for the rotation.</param>
        /// <param name="vec">The vector to rotate.</param>
        /// <returns>The rotated vector.</returns>
        public static float3 Rotate(QuaternionF quat, int3 vec)
        {
            float3 temp, result;
            var tempfloat3 = new float3(vec.x, vec.y, vec.z);
            temp = float3.Cross(quat.xyz, tempfloat3) + quat.w * tempfloat3;
            temp = float3.Cross(2 * quat.xyz, temp);
            result = tempfloat3 + temp;

            return result;
        }

        #endregion Rotate

        #region Color Conversion

        /// <summary>
        /// Converts a color value [0, 255] from linear to sRgb space.
        /// </summary>
        /// <param name="linearColor">The linear color value as <see cref="int3"/>.</param>
        public static int3 SRgbFromLinearColor(int3 linearColor)
        {
            return new int3((int)(SRgbToLinear(linearColor.x) * 255f), (int)(SRgbToLinear(linearColor.y) * 255f), (int)(SRgbToLinear(linearColor.z) * 255f));
        }

        private static float SRgbToLinear(int input)
        {
            var floatVal = input / 255f;
            return floatVal <= 0.0031308f ? floatVal * 12.92f : 1.055f * MathF.Pow(floatVal, 1f / 2.4f) - 0.055f;
        }

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="r">The red color value in range 0 - 255.</param>
        /// <param name="g">The green color value in range 0 - 255.</param>
        /// <param name="b">The blue color value in range 0 - 255.</param>
        public static int3 SRgbFromLinearColor(int r, int g, int b)
        {
            var val = new int3(r, g, b);
            return SRgbFromLinearColor(val);
        }

        /// <summary>
        ///Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="hex">The color value as hex code in form of a "FFFFFF" string.</param>
        public static int3 SRgbFromLinearColor(string hex)
        {
            var rgb = Convert.ToUInt32(hex, 16);
            return SRgbFromLinearColor(rgb);
        }

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="col">The color value as uint.</param>
        public static int3 SRgbFromLinearColor(uint col)
        {
            var b = (byte)(col & byte.MaxValue);
            var g = (byte)(col >> 8 & byte.MaxValue);
            var r = (byte)(col >> 16 & byte.MaxValue);

            return SRgbFromLinearColor(r, g, b);
        }

        /// <summary>
        /// Converts a color value [0, 255] from sRgb to linear space.
        /// </summary>
        /// <param name="sRGBCol">The sRgb color value as <see cref="int3"/>.</param>
        public static int3 LinearColorFromSRgb(int3 sRGBCol)
        {
            return new int3((int)LinearToSRgb(sRGBCol.x / 255f) * 255, (int)LinearToSRgb(sRGBCol.y / 255f) * 255, (int)LinearToSRgb(sRGBCol.z / 255f) * 255);
        }

        private static float LinearToSRgb(float input)
        {
            return input <= 0.04045f ? input / 12.92f : MathF.Pow((input + 0.055f) / 1.055f, 2.4f);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="r">The red color value in range 0 - 255.</param>
        /// <param name="g">The green color value in range 0 - 255.</param>
        /// <param name="b">The blue color value in range 0 - 255.</param>
        public static int3 LinearColorFromSRgb(int r, int g, int b)
        {
            var val = new int3(r, g, b);
            return LinearColorFromSRgb(val);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="hex">The color value as hex code in form of a "FFFFFF" string.</param>
        public static int3 LinearColorFromSRgb(string hex)
        {
            var rgb = Convert.ToUInt32(hex, 16);
            return LinearColorFromSRgb(rgb);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="col">The color value as uint.</param>
        public static int3 LinearColorFromSRgb(uint col)
        {
            var b = (byte)(col & byte.MaxValue);
            var g = (byte)(col >> 8 & byte.MaxValue);
            var r = (byte)(col >> 16 & byte.MaxValue);

            return LinearColorFromSRgb(r, g, b);
        }

        #endregion

        #endregion Static

        #region Swizzle

        /// <summary>
        /// Gets or sets an OpenTK.int2 with the x and y components of this instance.
        /// </summary>
        public int2 xy { get => new(x, y); set { x = value.x; y = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.int2 with the x and z components of this instance.
        /// </summary>
        public int2 xz { get => new(x, z); set { x = value.x; z = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.int2 with the y and x components of this instance.
        /// </summary>
        public int2 yx { get => new(y, x); set { y = value.x; x = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.int2 with the y and z components of this instance.
        /// </summary>
        public int2 yz { get => new(y, z); set { y = value.x; z = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.int2 with the z and x components of this instance.
        /// </summary>
        public int2 zx { get => new(z, x); set { z = value.x; x = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.int2 with the z and y components of this instance.
        /// </summary>
        public int2 zy { get => new(z, y); set { z = value.x; y = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.int3 with the x, y and z components of this instance.
        /// </summary>
        public int3 xyz { get => new(x, y, z); set { x = value.x; y = value.y; z = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.int3 with the x, z and y components of this instance.
        /// </summary>
        public int3 xzy { get => new(x, z, y); set { x = value.x; z = value.y; y = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.int3 with the y, z and x components of this instance.
        /// </summary>
        public int3 yzx { get => new(y, z, x); set { y = value.x; z = value.y; x = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.int3 with the y, x and z components of this instance.
        /// </summary>
        public int3 yxz { get => new(y, x, z); set { y = value.x; x = value.y; z = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.int3 with the z, x and y components of this instance.
        /// </summary>
        public int3 zxy { get => new(z, x, y); set { z = value.x; x = value.y; y = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.int3 with the z, y and x components of this instance.
        /// </summary>
        public int3 zyx { get => new(z, y, x); set { z = value.x; y = value.y; x = value.z; } }

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
        /// Multiplies two instances (component-wise).
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
        /// True, if left does not equal right; false otherwise.
        /// </returns>
        public static bool operator !=(int3 left, int3 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Explicit cast operator to cast a double3 into a int3 value.
        /// </summary>
        /// <param name="d3">The double3 value to cast.</param>
        /// <returns>A int3 value.</returns>
        public static explicit operator int3(double3 d3)
        {
            return new int3(d3);
        }

        /// <summary>
        /// Explicit cast operator to cast a double3 into a int3 value.
        /// </summary>
        /// <param name="f3">The double3 value to cast.</param>
        /// <returns>A int3 value.</returns>
        public static explicit operator int3(float3 f3)
        {
            return new int3(f3);
        }

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current int3.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current int3.
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

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A System.Int32 containing the unique hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
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
            if (obj is not int3)
                return false;

            return Equals((int3)obj);
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

        #endregion Color

        #endregion Public Members

        #region IEquatable<int3> Members

        /// <summary>
        /// Indicates whether the current vector is equal to another vector.
        /// </summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>
        /// true if the current vector is equal to the vector parameter; otherwise, false.
        /// </returns>
        public bool Equals(int3 other)
        {
            return
                x == other.x &&
                y == other.y &&
                z == other.z;
        }

        #endregion IEquatable<int3> Members

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a int3.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, int3> ParseConverter { get; set; } = (x => Parse(x));

        /// <summary>
        /// Parses a string into a int3.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static int3 Parse(string source, IFormatProvider? provider = null)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            char separator = M.GetNumericListSeparator(provider);

            string[] strings = source.Split(new char[] { separator, '(', ')', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length != 3)
                throw new FormatException("String parse for int3 did not result in exactly 3 items.");

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

            return new int3(ints[0], ints[1], ints[2]);
        }

    }
}