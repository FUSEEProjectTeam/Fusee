#pragma warning disable 1591

using System;
using System.Runtime.InteropServices;
using ProtoBuf;

namespace Fusee.Math.Core
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Represents a 3D vector using three single-precision floating-point numbers.
    /// </summary>
    /// <remarks>
    /// The float3 structure is suitable for interoperation with unmanaged code requiring three consecutive floats.
    /// </remarks>
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct float3 : IEquatable<float3>
    {
        #region Fields

        /// <summary>
        /// The x component of the float3.
        /// </summary>
        [ProtoMember(1)]
        public float x;

        /// <summary>
        /// The y component of the float3.
        /// </summary>
        [ProtoMember(2)]
        public float y;

        /// <summary>
        /// The z component of the float3.
        /// </summary>
        [ProtoMember(3)]
        public float z;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new float3.
        /// </summary>
        /// <param name="x">The x component of the float3.</param>
        /// <param name="y">The y component of the float3.</param>
        /// <param name="z">The z component of the float3.</param>
        public float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Constructs a new float3 from the given float2.
        /// </summary>
        /// <param name="v">The float2 to copy components from.</param>
        public float3(float2 v)
        {
            x = v.x;
            y = v.y;
            z = 0.0f;
        }

        /// <summary>
        /// Constructs a new float3 from the given float3.
        /// </summary>
        /// <param name="v">The float3 to copy components from.</param>
        public float3(float3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        /// <summary>
        /// Constructs a new float3 from the given float4.
        /// </summary>
        /// <param name="v">The float4 to copy components from.</param>
        public float3(float4 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        /// <summary>
        /// Constructs a new float3 by converting from a double3.
        /// </summary>
        /// <param name="d3">The double3 to copy components from.</param>
        public float3(double3 d3)
        {
            x = (float)d3.x;
            y = (float)d3.y;
            z = (float)d3.z;
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
            get { return (float)System.Math.Sqrt(LengthSquared); }
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
            get { return x * x + y * y + z * z; }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the float3 to unit length.
        /// </summary>
        public float3 Normalize()
        {
            return Normalize(this);
        }

        #endregion

        #region public void NormalizeFast()

        /// <summary>
        /// Scales the float3 to approximately unit length.
        /// </summary>
        public float3 NormalizeFast()
        {
            return NormalizeFast(this);
        }

        #endregion

        /// <summary>
        /// Returns an array of floats with the three components of the vector.
        /// </summary>
        /// <returns>Returns an array of floats with the three components of the vector.</returns>
        public float[] ToArray()
        {
            return new[] { x, y, z };
        }

        #endregion

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
        public static float3 Add(float3 a, float3 b)
        {
            var result = new float3(a.x + b.x, a.y + b.y, a.z + b.z);
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
        public static float3 Subtract(float3 a, float3 b)
        {
            var result = new float3(a.x - b.x, a.y - b.y, a.z - b.z);
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
        public static float3 Multiply(float3 vector, float scale)
        {
            var result = new float3(vector.x * scale, vector.y * scale, vector.z * scale);
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
        public static float3 Multiply(float3 vector, float3 scale)
        {
            var result = new float3(vector.x * scale.x, vector.y * scale.y, vector.z * scale.z);
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
        public static float3 Divide(float3 vector, float scale)
        {
            var result = new float3(vector.x / scale, vector.y / scale, vector.z / scale);
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
        public static float3 Divide(float3 vector, float3 scale)
        {
            var result = new float3(vector.x / scale.x, vector.y / scale.y, vector.z / scale.z);
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
        public static float3 ComponentMin(float3 a, float3 b)
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
        public static float3 ComponentMax(float3 a, float3 b)
        {
            a.x = a.x > b.x ? a.x : b.x;
            a.y = a.y > b.y ? a.y : b.y;
            a.z = a.z > b.z ? a.z : b.z;
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
        public static float3 Min(float3 left, float3 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        #endregion

        #region Max

        /// <summary>
        /// Returns the float3 with the maximum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>
        /// The maximum float3
        /// </returns>
        public static float3 Max(float3 left, float3 right)
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
        public static float3 Clamp(float3 vec, float3 min, float3 max)
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
        public static float3 Normalize(float3 vec)
        {
            if (vec.Length > M.EpsilonFloat)
            {
                var scale = 1.0f / vec.Length;

                vec.x *= scale;
                vec.y *= scale;
                vec.z *= scale;
            }

            return vec;
        }

        /// <summary>
        /// Orthoes the normalize.
        /// </summary>
        /// <param name="normal">The normal.</param>
        /// <param name="tangent">The tangent.</param>
        /// <returns>An float3 Array of size 2 with orthonormalized normal and tangent. </returns>
        public static float3[] OrthoNormalize(float3 normal, float3 tangent)
        {
            var ret = new float3[2];

            normal = Normalize(normal);
            var proj = normal * Dot(tangent, normal);

            tangent -= proj;
            tangent = Normalize(tangent);

            ret[0] = normal;
            ret[1] = tangent;

            return ret;
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
        public static float3 NormalizeFast(float3 vec)
        {
            var scale = M.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
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
        public static float Dot(float3 left, float3 right)
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
        public static float3 Cross(float3 left, float3 right)
        {
            float3 result = new float3(left.y * right.z - left.z * right.y,
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
        public static float3 Lerp(float3 a, float3 b, float blend)
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
        /// a when u=1, v=0, b when v=1,u=0, c when u=v=0, and a linear combination of a,b,c otherwise
        /// </returns>
        public static float3 Barycentric(float3 a, float3 b, float3 c, float u, float v)
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
        /// <param name="u">The resulting u coordinate.</param>
        /// <param name="v">The resulting v coordinate.</param>
        public static void GetBarycentric(float3 a, float3 b, float3 c, float3 point, out float u, out float v)
        {
            // Original taken from http://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates
            // which is a transcript from http://realtimecollisiondetection.net/. Re-arranged to directly calculate u and v (and have w be 1-u-v)
            float3 v0 = b - c, v1 = a - c, v2 = point - c;
            float d00 = Dot(v0, v0);
            float d01 = Dot(v0, v1);
            float d11 = Dot(v1, v1);
            float d20 = Dot(v2, v0);
            float d21 = Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;
            u = (d00 * d21 - d01 * d20) / denom;
            v = (d11 * d20 - d01 * d21) / denom;
        }
        #endregion

        #region Transform

        //TODO: Migrate to float4x4
        /// <summary>
        /// Transform this instance by the given Matrix, and project the resulting float4 back to a float3
        /// </summary>
        /// <param name="mat">The desired transformation</param>
        /// <returns>
        /// The transformed vector
        /// </returns>
        public float3 TransformPerspective(float4x4 mat)
        {
            var v = new float4(this, 1.0f);
            v = mat * v;
            float3 result = new float3();

            if (v.w > M.EpsilonFloat)
            {
                result.x = v.x / v.w;
                result.y = v.y / v.w;
                result.z = v.z / v.w;
            }
            else
                result = Zero;
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
        public static float CalculateAngle(float3 first, float3 second)
        {
            if ((first.Length > M.EpsilonFloat) && (second.Length > M.EpsilonFloat))
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

        #endregion

        #region Rotate

        /// <summary>
        /// Rotates a vector by the given euler-angles in the following order: yaw (y-axis), pitch (x-axis), roll (z-axis).
        /// </summary>
        /// <param name="euler">The angles used for the rotation.</param>
        /// <param name="vec">The vector to rotate.</param>
        /// <param name="inDegrees">Optional: Whether the angles are given in degrees (true) or radians (false). Defautl is radians.</param>
        /// <returns>The rotated vector.</returns>
        public static float3 Rotate(float3 euler, float3 vec, bool inDegrees = false)
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

            vec = float4x4.Transform(yRot, vec);
            vec = float4x4.Transform(xRot, vec);
            vec = float4x4.Transform(zRot, vec);

            float3 result = vec;

            return result;
        }

        /// <summary>
        /// Rotates a vector by the given quaternion.
        /// </summary>
        /// <param name="quat">The quaternion used for the rotation.</param>
        /// <param name="vec">The vector to rotate.</param>
        /// <returns>The rotated vector.</returns>
        public static float3 Rotate(Quaternion quat, float3 vec)
        {
            float3 temp, result;

            temp = Cross(quat.xyz, vec) + quat.w * vec;
            temp = Cross(2 * quat.xyz, temp);
            result = vec + temp;

            return result;
        }

        #endregion

        #endregion

        #region Swizzle

        /// <summary>
        /// Gets or sets an OpenTK.float2 with the x and y components of this instance.
        /// </summary>
        public float2 xy { get { return new float2(x, y); } set { x = value.x; y = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.float2 with the x and z components of this instance.
        /// </summary>
        public float2 xz { get { return new float2(x, z); } set { x = value.x; z = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.float2 with the y and x components of this instance.
        /// </summary>
        public float2 yx { get { return new float2(y, x); } set { y = value.x; x = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.float2 with the y and z components of this instance.
        /// </summary>
        public float2 yz { get { return new float2(y, z); } set { y = value.x; z = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.float2 with the z and x components of this instance.
        /// </summary>
        public float2 zx { get { return new float2(z, x); } set { z = value.x; x = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.float2 with the z and y components of this instance.
        /// </summary>
        public float2 zy { get { return new float2(z, y); } set { z = value.x; y = value.y; } }



        /// <summary>
        /// Gets or sets an OpenTK.float3 with the x, y and z components of this instance.
        /// </summary>
        public float3 xyz { get { return new float3(x, y, z); } set { x = value.x; y = value.y; z = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the x, z and y components of this instance.
        /// </summary>
        public float3 xzy { get { return new float3(x, z, y); } set { x = value.x; z = value.y; y = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the y, z and x components of this instance.
        /// </summary>
        public float3 yzx { get { return new float3(y, z, x); } set { y = value.x; z = value.y; x = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the y, x and z components of this instance.
        /// </summary>
        public float3 yxz { get { return new float3(y, x, z); } set { y = value.x; x = value.y; z = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the z, x and y components of this instance.
        /// </summary>
        public float3 zxy { get { return new float3(z, x, y); } set { z = value.x; x = value.y; y = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.float3 with the z, y and x components of this instance.
        /// </summary>
        public float3 zyx { get { return new float3(z, y, x); } set { z = value.x; y = value.y; x = value.z; } }

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
        public static float3 operator +(float3 left, float3 right)
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
        public static float3 operator -(float3 left, float3 right)
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
        public static float3 operator -(float3 vec)
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
        public static float3 operator *(float3 vec, float scale)
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
        public static float3 operator *(float scale, float3 vec)
        {
            return Multiply(vec, scale);
        }

        /// <summary>
        /// Multiplies two instances (componentwise).
        /// </summary>
        /// <param name="vec1">The first instance.</param>
        /// <param name="vec2">The second instance.</param>
        /// <returns>The result of the multiplication.</returns>
        public static float3 operator *(float3 vec1, float3 vec2)
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
        public static float3 operator /(float3 vec, float scale)
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
        public static bool operator ==(float3 left, float3 right)
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
        public static bool operator !=(float3 left, float3 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Explicit cast operator to cast a double3 into a float3 value.
        /// </summary>
        /// <param name="d3">The double3 value to cast.</param>
        /// <returns>A float3 value.</returns>
        public static explicit operator float3(double3 d3)
        {
            return new float3(d3);
        }
        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current float3.
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
            // ReSharper disable NonReadonlyFieldInGetHashCode
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
            // ReSharper restore NonReadonlyFieldInGetHashCode
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
            if (!(obj is float3))
                return false;

            return Equals((float3)obj);
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
        /// The blue component (same as z)
        /// </summary>
        public float b
        {
            get { return z; }
            set { z = value; }
        }

        #endregion

        #endregion

        #region IEquatable<float3> Members

        /// <summary>
        /// Indicates whether the current vector is equal to another vector.
        /// </summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>
        /// true if the current vector is equal to the vector parameter; otherwise, false.
        /// </returns>
        public bool Equals(float3 other)
        {
            return
                System.Math.Abs(x - other.x) < M.EpsilonFloat &&
                System.Math.Abs(y - other.y) < M.EpsilonFloat &&
                System.Math.Abs(z - other.z) < M.EpsilonFloat;
        }

        #endregion

        /// <summary>
        /// Gets or sets the Converter object. Has the ability to convert a string to a float3.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, float3> Parse { get; set; }
    }

    // ReSharper restore InconsistentNaming
}

#pragma warning restore 1591
