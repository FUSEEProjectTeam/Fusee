using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using ProtoBuf;

namespace Fusee.Math
{
    /// <summary>Represents a 4D vector using four single-precision floating-point numbers.</summary>
    /// <remarks>
    /// The float4 structure is suitable for interoperation with unmanaged code requiring four consecutive floats.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [ProtoContract]
// ReSharper disable InconsistentNaming
    public struct float4  : IEquatable<float4>
// ReSharper restore InconsistentNaming
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

        #endregion

        #region Constructors



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
            x = (float) d4.x;
            y = (float) d4.y;
            z = (float) d4.z;
            w = (float) d4.w;
        }

        #endregion

        #region Public Members

        #region Instance

        #region public void Add()

        /// <summary>Add the Vector passed as parameter to this instance.</summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        [Obsolete("Use static Add() method instead.")]
        public void Add(float4 right)
        {
            x += right.x;
            y += right.y;
            z += right.z;
            w += right.w;
        }

        /// <summary>Add the Vector passed as parameter to this instance.</summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        [Obsolete("Use static Add() method instead.")]
        public void Add(ref float4 right)
        {
            x += right.x;
            y += right.y;
            z += right.z;
            w += right.w;
        }

        #endregion public void Add()

        #region public void Sub()

        /// <summary>Subtract the Vector passed as parameter from this instance.</summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        [Obsolete("Use static Subtract() method instead.")]
        public void Sub(float4 right)
        {
            this.x -= right.x;
            this.y -= right.y;
            this.z -= right.z;
            this.w -= right.w;
        }

        /// <summary>Subtract the Vector passed as parameter from this instance.</summary>
        /// <param name="right">Right operand. This parameter is only read from.</param>
        [Obsolete("Use static Subtract() method instead.")]
        public void Sub(ref float4 right)
        {
            this.x -= right.x;
            this.y -= right.y;
            this.z -= right.z;
            this.w -= right.w;
        }

        #endregion public void Sub()

        #region public void Mult()

        /// <summary>Multiply this instance by a scalar.</summary>
        /// <param name="f">Scalar operand.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Mult(float f)
        {
            this.x *= f;
            this.y *= f;
            this.z *= f;
            this.w *= f;
        }

        #endregion public void Mult()

        #region public void Div()

        /// <summary>Divide this instance by a scalar.</summary>
        /// <param name="f">Scalar operand.</param>
        [Obsolete("Use static Divide() method instead.")]
        public void Div(float f)
        {
            float mult = 1.0f / f;
            this.x *= mult;
            this.y *= mult;
            this.z *= mult;
            this.w *= mult;
        }

        #endregion public void Div()

        #region public float Length

        /// <summary>
        /// Gets the length (magnitude) of the vector.
        /// </summary>
        /// <see cref="LengthFast"/>
        /// <seealso cref="LengthSquared"/>
        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(x * x + y * y + z * z + w * w);
            }
        }

        #endregion

        #region public float LengthFast

        /// <summary>
        /// Gets an approximation of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property uses an approximation of the square root function to calculate vector magnitude, with
        /// an upper error bound of 0.001.
        /// </remarks>
        /// <see cref="Length"/>
        /// <seealso cref="LengthSquared"/>
        public float LengthFast
        {
            get
            {
                return 1.0f / MathHelper.InverseSqrtFast(x * x + y * y + z * z + w * w);
            }
        }

        #endregion

        #region public float LengthSquared

        /// <summary>
        /// Gets the square of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property avoids the costly square root operation required by the Length property. This makes it more suitable
        /// for comparisons.
        /// </remarks>
        /// <see cref="Length"/>
        /// <seealso cref="LengthFast"/>
        public float LengthSquared
        {
            get
            {
                return x * x + y * y + z * z + w * w;
            }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the float4 to unit length.
        /// </summary>
        public void Normalize()
        {
            float scale = 1.0f / this.Length;
            x *= scale;
            y *= scale;
            z *= scale;
            w *= scale;
        }

        #endregion

        #region public void NormalizeFast()

        /// <summary>
        /// Scales the float4 to approximately unit length.
        /// </summary>
        public void NormalizeFast()
        {
            float scale = MathHelper.InverseSqrtFast(x * x + y * y + z * z + w * w);
            x *= scale;
            y *= scale;
            z *= scale;
            w *= scale;
        }

        #endregion

        #region public void Scale()

        /// <summary>
        /// Scales the current float4 by the given amounts.
        /// </summary>
        /// <param name="sx">The scale of the x component.</param>
        /// <param name="sy">The scale of the y component.</param>
        /// <param name="sz">The scale of the z component.</param>
        /// <param name="sw">The scale of the z component.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Scale(float sx, float sy, float sz, float sw)
        {
            this.x = x * sx;
            this.y = y * sy;
            this.z = z * sz;
            this.w = w * sw;
        }

        /// <summary>Scales this instance by the given parameter.</summary>
        /// <param name="scale">The scaling of the individual components.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Scale(float4 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
            this.w *= scale.w;
        }

        /// <summary>Scales this instance by the given parameter.</summary>
        /// <param name="scale">The scaling of the individual components.</param>
        [Obsolete("Use static Multiply() method instead.")]
        public void Scale(ref float4 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
            this.w *= scale.w;
        }

        #endregion public void Scale()

        #region public float[] ToArray()

        /// <summary>
        /// XML-Comment
        /// </summary>
        /// <returns>An float array of size 4 that cobtains the x,y,z,w components.</returns>
        public float[] ToArray()
        {
            return new float[] { x, y, z, w };
        }

        #endregion

        #region public void Round()

        /// <summary>
        /// Rounds the float4 to 6 digits (max float precision).
        /// </summary>
        public void Round()
        {
            x = (float) System.Math.Round(x, 6);
            y = (float) System.Math.Round(y, 6);
            z = (float) System.Math.Round(z, 6);
            w = (float) System.Math.Round(w, 6);
        }

        #endregion

        #endregion

        #region Static

        #region Obsolete

        #region Sub

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result of subtraction</returns>
        public static float4 Sub(float4 a, float4 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            a.z -= b.z;
            a.w -= b.w;
            return a;
        }

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">Result of subtraction</param>
        public static void Sub(ref float4 a, ref float4 b, out float4 result)
        {
            result.x = a.x - b.x;
            result.y = a.y - b.y;
            result.z = a.z - b.z;
            result.w = a.w - b.w;
        }

        #endregion

        #region Mult

        /// <summary>
        /// Multiply a vector and a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <returns>Result of the multiplication</returns>
        public static float4 Mult(float4 a, float f)
        {
            a.x *= f;
            a.y *= f;
            a.z *= f;
            a.w *= f;
            return a;
        }

        /// <summary>
        /// Multiply a vector and a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <param name="result">Result of the multiplication</param>
        public static void Mult(ref float4 a, float f, out float4 result)
        {
            result.x = a.x * f;
            result.y = a.y * f;
            result.z = a.z * f;
            result.w = a.w * f;
        }

        #endregion

        #region Div

        /// <summary>
        /// Divide a vector by a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <returns>Result of the division</returns>
        public static float4 Div(float4 a, float f)
        {
            float mult = 1.0f / f;
            a.x *= mult;
            a.y *= mult;
            a.z *= mult;
            a.w *= mult;
            return a;
        }

        /// <summary>
        /// Divide a vector by a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <param name="result">Result of the division</param>
        public static void Div(ref float4 a, float f, out float4 result)
        {
            float mult = 1.0f / f;
            result.x = a.x * mult;
            result.y = a.y * mult;
            result.z = a.z * mult;
            result.w = a.w * mult;
        }

        #endregion

        #endregion

        #region Add

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <returns>Result of operation.</returns>
        public static float4 Add(float4 a, float4 b)
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
        public static void Add(ref float4 a, ref float4 b, out float4 result)
        {
            result = new float4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        #endregion

        #region Subtract

        /// <summary>
        /// Subtract one Vector from another
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result of subtraction</returns>
        public static float4 Subtract(float4 a, float4 b)
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
        public static void Subtract(ref float4 a, ref float4 b, out float4 result)
        {
            result = new float4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        #endregion

        #region Multiply

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static float4 Multiply(float4 vector, float scale)
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
        public static void Multiply(ref float4 vector, float scale, out float4 result)
        {
            result = new float4(vector.x * scale, vector.y * scale, vector.z * scale, vector.w * scale);
        }

        /// <summary>
        /// Multiplies a vector by the components a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static float4 Multiply(float4 vector, float4 scale)
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
        public static void Multiply(ref float4 vector, ref float4 scale, out float4 result)
        {
            result = new float4(vector.x * scale.x, vector.y * scale.y, vector.z * scale.z, vector.w * scale.w);
        }

        #endregion

        #region Divide

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static float4 Divide(float4 vector, float scale)
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
        public static void Divide(ref float4 vector, float scale, out float4 result)
        {
            Multiply(ref vector, 1 / scale, out result);
        }

        /// <summary>
        /// Divides a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        public static float4 Divide(float4 vector, float4 scale)
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
        public static void Divide(ref float4 vector, ref float4 scale, out float4 result)
        {
            result = new float4(vector.x / scale.x, vector.y / scale.y, vector.z / scale.z, vector.w / scale.w);
        }

        #endregion

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

        /// <summary>
        /// Calculate the component-wise minimum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">The component-wise minimum</param>
        public static void Min(ref float4 a, ref float4 b, out float4 result)
        {
            result.x = a.x < b.x ? a.x : b.x;
            result.y = a.y < b.y ? a.y : b.y;
            result.z = a.z < b.z ? a.z : b.z;
            result.w = a.w < b.w ? a.w : b.w;
        }

        #endregion

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

        /// <summary>
        /// Calculate the component-wise maximum of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <param name="result">The component-wise maximum</param>
        public static void Max(ref float4 a, ref float4 b, out float4 result)
        {
            result.x = a.x > b.x ? a.x : b.x;
            result.y = a.y > b.y ? a.y : b.y;
            result.z = a.z > b.z ? a.z : b.z;
            result.w = a.w > b.w ? a.w : b.w;
        }

        #endregion

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
            vec.z = vec.x < min.z ? min.z : vec.z > max.z ? max.z : vec.z;
            vec.w = vec.y < min.w ? min.w : vec.w > max.w ? max.w : vec.w;
            return vec;
        }

        /// <summary>
        /// Clamp a vector to the given minimum and maximum vectors
        /// </summary>
        /// <param name="vec">Input vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <param name="result">The clamped vector</param>
        public static void Clamp(ref float4 vec, ref float4 min, ref float4 max, out float4 result)
        {
            result.x = vec.x < min.x ? min.x : vec.x > max.x ? max.x : vec.x;
            result.y = vec.y < min.y ? min.y : vec.y > max.y ? max.y : vec.y;
            result.z = vec.x < min.z ? min.z : vec.z > max.z ? max.z : vec.z;
            result.w = vec.y < min.w ? min.w : vec.w > max.w ? max.w : vec.w;
        }

        #endregion

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

        /// <summary>
        /// Scale a vector to unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <param name="result">The normalized vector</param>
        public static void Normalize(ref float4 vec, out float4 result)
        {
            float scale = 1.0f / vec.Length;
            result.x = vec.x * scale;
            result.y = vec.y * scale;
            result.z = vec.z * scale;
            result.w = vec.w * scale;
        }

        #endregion

        #region NormalizeFast

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <returns>The normalized vector</returns>
        public static float4 NormalizeFast(float4 vec)
        {
            float scale = MathHelper.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w);
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        /// <summary>
        /// Scale a vector to approximately unit length
        /// </summary>
        /// <param name="vec">The input vector</param>
        /// <param name="result">The normalized vector</param>
        public static void NormalizeFast(ref float4 vec, out float4 result)
        {
            float scale = MathHelper.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w);
            result.x = vec.x * scale;
            result.y = vec.y * scale;
            result.z = vec.z * scale;
            result.w = vec.w * scale;
        }

        #endregion

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

        /// <summary>
        /// Calculate the dot product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <param name="result">The dot product of the two inputs</param>
        public static void Dot(ref float4 left, ref float4 right, out float result)
        {
            result = left.x * right.x + left.y * right.y + left.z * right.z + left.w * right.w;
        }

        #endregion

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
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
        public static void Lerp(ref float4 a, ref float4 b, float blend, out float4 result)
        {
            result.x = blend * (b.x - a.x) + a.x;
            result.y = blend * (b.y - a.y) + a.y;
            result.z = blend * (b.z - a.z) + a.z;
            result.w = blend * (b.w - a.w) + a.w;
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
        /// <returns>a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</returns>
        public static float4 BaryCentric(float4 a, float4 b, float4 c, float u, float v)
        {
            return a + u * (b - a) + v * (c - a);
        }

        /// <summary>Interpolate 3 Vectors using Barycentric coordinates</summary>
        /// <param name="a">First input Vector.</param>
        /// <param name="b">Second input Vector.</param>
        /// <param name="c">Third input Vector.</param>
        /// <param name="u">First Barycentric Coordinate.</param>
        /// <param name="v">Second Barycentric Coordinate.</param>
        /// <param name="result">Output Vector. a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</param>
        public static void BaryCentric(ref float4 a, ref float4 b, ref float4 c, float u, float v, out float4 result)
        {
            result = a; // copy

            float4 temp = b; // copy
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

        /// <summary>Transform a Vector by the given Matrix</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed vector</returns>
        public static float4 Transform(float4 vec, float4x4 mat)
        {
            float4 result;
            Transform(ref vec, ref mat, out result);
            return result;
        }

        /// <summary>Transform a Vector by the given Matrix</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static void Transform(ref float4 vec, ref float4x4 mat, out float4 result)
        {
            result = new float4(
                vec.x * mat.Row0.x + vec.y * mat.Row1.x + vec.z * mat.Row2.x + vec.w * mat.Row3.x,
                vec.x * mat.Row0.y + vec.y * mat.Row1.y + vec.z * mat.Row2.y + vec.w * mat.Row3.y,
                vec.x * mat.Row0.z + vec.y * mat.Row1.z + vec.z * mat.Row2.z + vec.w * mat.Row3.z,
                vec.x * mat.Row0.w + vec.y * mat.Row1.w + vec.z * mat.Row2.w + vec.w * mat.Row3.w);
        }

        /// <summary>
        /// Transforms a vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the vector by.</param>
        /// <returns>The result of the operation.</returns>
        public static float4 Transform(float4 vec, Quaternion quat)
        {
            float4 result;
            Transform(ref vec, ref quat, out result);
            return result;
        }

        /// <summary>
        /// Transforms a vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the vector by.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Transform(ref float4 vec, ref Quaternion quat, out float4 result)
        {
            Quaternion v = new Quaternion(vec.x, vec.y, vec.z, vec.w), i, t;
            Quaternion.Invert(ref quat, out i);
            Quaternion.Multiply(ref quat, ref v, out t);
            Quaternion.Multiply(ref t, ref i, out v);

            result = new float4(v.x, v.y, v.z, v.w);
        }

        #endregion

        #region Round

        /// <summary>
        /// Rounds a vector to 6 digits (max float precision).
        /// </summary>
        /// <param name="vec">The input vector.</param>
        /// <returns>The rounded vector.</returns>
        public static float4 Round(float4 vec)
        {
            return new float4((float) System.Math.Round(vec.x, 6),
                              (float) System.Math.Round(vec.y, 6),
                              (float) System.Math.Round(vec.z, 6),
                              (float) System.Math.Round(vec.w, 6));
        }

        #endregion

        #endregion

        #region Swizzle

        /// <summary>
        /// Gets or sets an OpenTK.float2 with the x and y components of this instance.
        /// </summary>
// ReSharper disable InconsistentNaming
        public float2 xy { get { return new float2(x, y); } set { x = value.x; y = value.y; } }
// ReSharper restore InconsistentNaming

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the x, y and z components of this instance.
        /// </summary>
// ReSharper disable InconsistentNaming
        public float3 xyz { get { return new float3(x, y, z); } set { x = value.x; y = value.y; z = value.z; } }
// ReSharper restore InconsistentNaming

        #endregion

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static float4 operator +(float4 left, float4 right)
        {
            left.x += right.x;
            left.y += right.y;
            left.z += right.z;
            left.w += right.w;
            return left;
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static float4 operator -(float4 left, float4 right)
        {
            left.x -= right.x;
            left.y -= right.y;
            left.z -= right.z;
            left.w -= right.w;
            return left;
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
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="scale">The scalar.</param>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static float4 operator *(float scale, float4 vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        /// <summary>
        /// Divides an instance by a scalar.
        /// </summary>
        /// <param name="vec">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>The result of the calculation.</returns>
        public static float4 operator /(float4 vec, float scale)
        {
            float mult = 1.0f / scale;
            vec.x *= mult;
            vec.y *= mult;
            vec.z *= mult;
            vec.w *= mult;
            return vec;
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
        /// <returns>True, if left does not equa lright; false otherwise.</returns>
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
        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current float4.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return String.Format("({0}, {1}, {2}, {3})", x, y, z, w);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
// ReSharper disable NonReadonlyFieldInGetHashCode
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
// ReSharper restore NonReadonlyFieldInGetHashCode
        }

        #endregion

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

        #endregion

        #endregion

        #region Color
        // ReSharper disable InconsistentNaming
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
        /// The blue component (same as z)
        /// </summary>
        public float b
        {
            get { return z; }
            set { z = value; }
        }

        /// <summary>
        /// The rgb component (same as xyz)
        /// </summary>
        public float3 rgb
        {
            get { return xyz; }
            set { xyz = value; }
        }

        /// <summary>
        /// The alpha component (same as w)
        /// </summary>
        public float a
        {
            get { return w; }
            set { w = value; }
        }
        // ReSharper restore InconsistentNaming
        #endregion

        #endregion

        #region IEquatable<float4> Members

        /// <summary>Indicates whether the current vector is equal to another vector.</summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
        public bool Equals(float4 other)
        {
            return
                System.Math.Abs(x - other.x) < MathHelper.EpsilonFloat &&
                System.Math.Abs(y - other.y) < MathHelper.EpsilonFloat &&
                System.Math.Abs(z - other.z) < MathHelper.EpsilonFloat &&
                System.Math.Abs(w - other.w) < MathHelper.EpsilonFloat;
        }

        #endregion

        public static Converter<string, float4> Parse { get; set; }
    }
}
