using ProtoBuf;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a 3D vector using three double-precision numbers.
    /// </summary>
    /// <remarks>
    /// The double3 structure is suitable for inter-operation with unmanaged code requiring three consecutive doubles.
    /// </remarks>
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct double3 : IEquatable<double3>
    {
        #region Fields

        /// <summary>
        /// The x component of the double3.
        /// </summary>
        [ProtoMember(1)]
        public double x;

        /// <summary>
        /// The y component of the double3.
        /// </summary>
        [ProtoMember(2)]
        public double y;

        /// <summary>
        /// The z component of the double3.
        /// </summary>
        [ProtoMember(3)]
        public double z;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new double3.
        /// </summary>
        /// <param name="val">This value will be set for the x, y and z component.</param>
        public double3(double val)
        {
            x = val;
            y = val;
            z = val;
        }

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
            z = 0.0;
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

        /// <summary>
        /// Constructs a new double3 by converting from a double3.
        /// </summary>
        /// <param name="d3">The double3 to copy components from.</param>
        public double3(float3 d3)
        {
            x = d3.x;
            y = d3.y;
            z = d3.z;
        }

        #endregion Constructors

        #region Public Members

        #region this

        /// <summary>
        /// Gets or sets the individual components x, y, or z, depending on their index.
        /// </summary>
        /// <param name="idx">The index (between 0 and 2).</param>
        /// <returns>The x or y component of the double3.</returns>
        public double this[int idx]
        {
            get
            {
                return idx switch
                {
                    0 => x,
                    1 => y,
                    2 => z,
                    _ => throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a double3 type"),
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
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a double3 type");
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
        public double Length => System.Math.Sqrt(LengthSquared);

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
        public double LengthSquared => x * x + y * y + z * z;

        #endregion public double LengthSquared

        #region public Normalize()

        /// <summary>
        /// Scales the double3 to unit length.
        /// </summary>
        public double3 Normalize()
        {
            return Normalize(this);
        }

        #endregion public Normalize()

        #region public NormalizeFast()

        /// <summary>
        /// Scales the double3 to approximately unit length.
        /// </summary>
        public double3 NormalizeFast()
        {
            return NormalizeFast(this);
        }

        #endregion public NormalizeFast()

        #region Color Conversion

        /// <summary>
        /// Converts this double3 - which is interpreted as a color - from linear color space to sRgb space.
        /// </summary>
        /// <returns></returns>
        public double3 LinearColorFromSRgb()
        {
            return LinearColorFromSRgb(this);
        }

        /// <summary>
        /// Converts this double3 - which is interpreted as a color - from sRgb space to linear color space.
        /// </summary>
        /// <returns></returns>
        public double3 SRgbFromLinearColor()
        {
            return SRgbFromLinearColor(this);
        }

        #endregion

        /// <summary>
        /// Returns an array of doubles with the three components of the vector.
        /// </summary>
        /// <returns>Returns an array of doubles with the three components of the vector.</returns>
        public double[] ToArray()
        {
            return new[] { x, y, z };
        }

        /// <summary>
        /// Returns a bool which determines whether this double3 isNaN
        /// </summary>
        public bool IsNaN => double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z);

        /// <summary>
        /// Returns a bool which determines whether this double3 contains a infinity value
        /// </summary>
        public bool IsInfinity => double.IsInfinity(x) || double.IsInfinity(y) || double.IsInfinity(z);

        #endregion Instance

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length double3 that points towards the x-axis.
        /// </summary>
        public static readonly double3 UnitX = new(1, 0, 0);

        /// <summary>
        /// Defines a unit-length double3 that points towards the y-axis.
        /// </summary>
        public static readonly double3 UnitY = new(0, 1, 0);

        /// <summary>
        /// Defines a unit-length double3 that points towards the z-axis.
        /// </summary>
        public static readonly double3 UnitZ = new(0, 0, 1);

        /// <summary>
        /// Defines a zero-length double3.
        /// </summary>
        public static readonly double3 Zero = new(0, 0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly double3 One = new(1, 1, 1);

        #region Infinity

        /// <summary>
        /// Returns a double3 which contains positive infinity values
        /// </summary>
        public static double3 PositiveInfinity => One * double.PositiveInfinity;

        /// <summary>
        /// Returns a double3 which contains negative infinity values
        /// </summary>
        public static double3 NegativeInfinity => One * double.NegativeInfinity;


        #endregion


        // <summary>
        // Defines the size of the double3 struct in bytes.
        // </summary>
        // public static readonly int SizeInBytes = Marshal.SizeOf(new double3());

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
        public static double3 Add(double3 a, double3 b)
        {
            var result = new double3(a.x + b.x, a.y + b.y, a.z + b.z);
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
        public static double3 Add(double3 vec, double scale)
        {
            var result = new double3(vec.x + scale, vec.y + scale, vec.z + scale);
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
        public static double3 Subtract(double3 a, double3 b)
        {
            var result = new double3(a.x - b.x, a.y - b.y, a.z - b.z);
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
        public static double3 Subtract(double3 vec, double scale)
        {
            var result = new double3(vec.x - scale, vec.y - scale, vec.z - scale);
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
        public static double3 Multiply(double3 vector, double scale)
        {
            var result = new double3(vector.x * scale, vector.y * scale, vector.z * scale);
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
        public static double3 Multiply(double3 vector, double3 scale)
        {
            var result = new double3(vector.x * scale.x, vector.y * scale.y, vector.z * scale.z);
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
        public static double3 Divide(double3 vector, double scale)
        {
            var result = new double3(vector.x / scale, vector.y / scale, vector.z / scale);
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
        public static double3 Divide(double3 vector, double3 scale)
        {
            var result = new double3(vector.x / scale.x, vector.y / scale.y, vector.z / scale.z);
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
        public static double3 ComponentMin(double3 a, double3 b)
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
        public static double3 ComponentMax(double3 a, double3 b)
        {
            a.x = a.x > b.x ? a.x : b.x;
            a.y = a.y > b.y ? a.y : b.y;
            a.z = a.z > b.z ? a.z : b.z;
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
        public static double3 Min(double3 left, double3 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        #endregion Min

        #region Max

        /// <summary>
        /// Returns the double3 with the maximum magnitude
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>
        /// The maximum double3
        /// </returns>
        public static double3 Max(double3 left, double3 right)
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
        public static double3 Clamp(double3 vec, double3 min, double3 max)
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
        /// <returns>
        /// The normalized vector
        /// </returns>
        public static double3 Normalize(double3 vec)
        {
            if (vec.Length > M.EpsilonDouble)
            {
                var scale = 1.0 / vec.Length;

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
        /// <returns>An double3 Array of size 2 with orthonormalized normal and tangent. </returns>
        public static double3[] OrthoNormalize(double3 normal, double3 tangent)
        {
            var ret = new double3[2];

            normal = Normalize(normal);
            var proj = normal * Dot(tangent, normal);

            tangent -= proj;
            tangent = Normalize(tangent);

            ret[0] = normal;
            ret[1] = tangent;

            return ret;
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
        public static double3 NormalizeFast(double3 vec)
        {
            var scale = M.InverseSqrtFast(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
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
        public static double Dot(double3 left, double3 right)
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
        public static double3 Cross(double3 left, double3 right)
        {
            double3 result = new(left.y * right.z - left.z * right.y,
                                       left.z * right.x - left.x * right.z,
                                       left.x * right.y - left.y * right.x);

            return result;
        }

        #endregion Cross

        /// <summary>
        /// Performs <see cref="M.Step(double, double)"/> for each component of the input vectors.
        /// </summary>
        /// <param name="edge">Specifies the location of the edge of the step function.</param>
        /// <param name="val">Specifies the value to be used to generate the step function.</param>
        public static double3 Step(double3 edge, double3 val)
        {
            return new double3(M.Step(edge.x, val.x), M.Step(edge.y, val.y), M.Step(edge.z, val.z));
        }

        /// <summary>
        /// Returns a double3 where all components are raised to the specified power.
        /// </summary>
        /// <param name="val">The double3 to be raised to a power.</param>
        /// <param name="exp">A double that specifies a power.</param>
        /// <returns></returns>
        public static double3 Pow(double3 val, double exp)
        {
            return new double3(System.Math.Pow(val.r, exp), System.Math.Pow(val.g, exp), System.Math.Pow(val.b, exp));
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
        public static double3 Lerp(double3 a, double3 b, double blend)
        {
            a.x = blend * (b.x - a.x) + a.x;
            a.y = blend * (b.y - a.y) + a.y;
            a.z = blend * (b.z - a.z) + a.z;
            return a;
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// Each component of vector a is blended with its equivalent in vector b.
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>       
        public static double3 Lerp(double3 a, double3 b, double3 blend)
        {
            a.x = blend.x * (b.x - a.x) + a.x;
            a.y = blend.y * (b.y - a.y) + a.y;
            a.z = blend.z * (b.z - a.z) + a.z;
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
        /// a when u=1, v=0, b when v=1,u=0, c when u=v=0, and a linear combination of a,b,c otherwise
        /// </returns>
        public static double3 Barycentric(double3 a, double3 b, double3 c, double u, double v)
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

        /// <summary>
        /// Checks if the given point is inside the given triangle (a, b, c). Returns the barycentric coordinates using <see cref="GetBarycentric"/>.
        /// </summary>
        /// <param name="a">The first point of the triangle.</param>
        /// <param name="b">The second point of the triangle.</param>
        /// <param name="c">The third point of the triangle.</param>
        /// <param name="point">the point to test.</param>
        /// <param name="u">The resulting barycentric u coordinate (weight for vertex a).</param>
        /// <param name="v">The resulting barycentric v coordinate (weight for vertex b).</param>
        /// <returns>True if the point is inside the triangle a, b, c. Otherwise returns false.</returns>
        public static bool PointInTriangle(double3 a, double3 b, double3 c, double3 point, out double u, out double v)
        {
            GetBarycentric(a, b, c, point, out u, out v);

            return u >= 0 && v >= 0 && u + v <= 1;
        }

        #endregion Barycentric

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
            if ((first.Length > M.EpsilonDouble) && (second.Length > M.EpsilonDouble))
            {
                var dotP = Dot(first, second) / (first.Length * second.Length);

                if (dotP < -1)
                    dotP = -1;
                if (dotP > 1)
                    dotP = 1;

                return System.Math.Acos(dotP);
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

        #endregion Rotate

        #region Color Conversion

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="linearColor">The linear color value as <see cref="double3"/>.</param>
        public static double3 SRgbFromLinearColor(double3 linearColor)
        {
            return new double3(SRgbToLinear(linearColor.x), SRgbToLinear(linearColor.y), SRgbToLinear(linearColor.z));
        }

        private static double SRgbToLinear(double input)
        {
            return input <= 0.0031308f ? input * 12.92f : 1.055f * System.Math.Pow(input, 1f / 2.4f) - 0.055f;
        }

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="r">The red color value in range 0 - 255.</param>
        /// <param name="g">The green color value in range 0 - 255.</param>
        /// <param name="b">The blue color value in range 0 - 255.</param>
        public static double3 SRgbFromLinearColor(int r, int g, int b)
        {
            var val = new double3(r / 255f, g / 255f, b / 255f);
            return SRgbFromLinearColor(val);
        }

        /// <summary>
        ///Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="hex">The color value as hex code in form of a "FFFFFF" string.</param>
        public static double3 SRgbFromLinearColor(string hex)
        {
            var rgb = Convert.ToUInt32(hex, 16);
            return SRgbFromLinearColor(rgb);
        }

        /// <summary>
        /// Converts a color value from linear to sRgb space.
        /// </summary>
        /// <param name="col">The color value as uint.</param>
        public static double3 SRgbFromLinearColor(uint col)
        {
            var b = (byte)(col & byte.MaxValue);
            var g = (byte)(col >> 8 & byte.MaxValue);
            var r = (byte)(col >> 16 & byte.MaxValue);

            return SRgbFromLinearColor(r, g, b);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="sRGBCol">The sRgb color value as <see cref="double3"/>.</param>
        public static double3 LinearColorFromSRgb(double3 sRGBCol)
        {
            return new double3(LinearToSRgb(sRGBCol.x), LinearToSRgb(sRGBCol.y), LinearToSRgb(sRGBCol.z));
        }

        private static double LinearToSRgb(double input)
        {
            return input <= 0.04045f ? input / 12.92f : System.Math.Pow((input + 0.055f) / 1.055f, 2.4f);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="r">The red color value in range 0 - 255.</param>
        /// <param name="g">The green color value in range 0 - 255.</param>
        /// <param name="b">The blue color value in range 0 - 255.</param>
        public static double3 LinearColorFromSRgb(int r, int g, int b)
        {
            var val = new double3(r / 255f, g / 255f, b / 255f);
            return LinearColorFromSRgb(val);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="hex">The color value as hex code in form of a "FFFFFF" string.</param>
        public static double3 LinearColorFromSRgb(string hex)
        {
            var rgb = Convert.ToUInt32(hex, 16);
            return LinearColorFromSRgb(rgb);
        }

        /// <summary>
        /// Converts a color value from sRgb to linear space.
        /// </summary>
        /// <param name="col">The color value as uint.</param>
        public static double3 LinearColorFromSRgb(uint col)
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
        /// Gets or sets an OpenTK.double2 with the x and y components of this instance.
        /// </summary>
        public double2 xy { get => new(x, y); set { x = value.x; y = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.double2 with the x and z components of this instance.
        /// </summary>
        public double2 xz { get => new(x, z); set { x = value.x; z = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.double2 with the y and x components of this instance.
        /// </summary>
        public double2 yx { get => new(y, x); set { y = value.x; x = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.double2 with the y and z components of this instance.
        /// </summary>
        public double2 yz { get => new(y, z); set { y = value.x; z = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.double2 with the z and x components of this instance.
        /// </summary>
        public double2 zx { get => new(z, x); set { z = value.x; x = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.double2 with the z and y components of this instance.
        /// </summary>
        public double2 zy { get => new(z, y); set { z = value.x; y = value.y; } }

        /// <summary>
        /// Gets or sets an OpenTK.double3 with the x, y and z components of this instance.
        /// </summary>
        public double3 xyz { get => new(x, y, z); set { x = value.x; y = value.y; z = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.double3 with the x, z and y components of this instance.
        /// </summary>
        public double3 xzy { get => new(x, z, y); set { x = value.x; z = value.y; y = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.double3 with the y, z and x components of this instance.
        /// </summary>
        public double3 yzx { get => new(y, z, x); set { y = value.x; z = value.y; x = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.double3 with the y, x and z components of this instance.
        /// </summary>
        public double3 yxz { get => new(y, x, z); set { y = value.x; x = value.y; z = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.double3 with the z, x and y components of this instance.
        /// </summary>
        public double3 zxy { get => new(z, x, y); set { z = value.x; x = value.y; y = value.z; } }

        /// <summary>
        /// Gets or sets an OpenTK.double3 with the z, y and x components of this instance.
        /// </summary>
        public double3 zyx { get => new(z, y, x); set { z = value.x; y = value.y; x = value.z; } }

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
        public static double3 operator +(double3 left, double3 right)
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
        public static double3 operator +(double3 left, double scalar)
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
        public static double3 operator -(double3 left, double3 right)
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
        public static double3 operator -(double3 left, double scalar)
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
        /// Multiplies two instances (component-wise).
        /// </summary>
        /// <param name="vec1">The first instance.</param>
        /// <param name="vec2">The second instance.</param>
        /// <returns>The result of the multiplication.</returns>
        public static double3 operator *(double3 vec1, double3 vec2)
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
        /// True, if left does not equal right; false otherwise.
        /// </returns>
        public static bool operator !=(double3 left, double3 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Explicit cast operator to cast a double3 into a double3 value.
        /// </summary>
        /// <param name="d3">The double3 value to cast.</param>
        /// <returns>A double3 value.</returns>
        public static explicit operator double3(float3 d3)
        {
            return new double3(d3);
        }

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current double3.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current double3.
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
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>
        /// A System.Int32 containing the unique hashcode for this instance.
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
            if (!(obj is double3))
                return false;

            return Equals((double3)obj);
        }

        #endregion public override bool Equals(object obj)

        #endregion Overrides

        #region Color Swizzle

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
        /// The blue component (same as z)
        /// </summary>
        public double b
        {
            get => z;
            set => z = value;
        }

        #endregion Color

        #endregion Public Members

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
                System.Math.Abs(x - other.x) < M.EpsilonDouble &&
                System.Math.Abs(y - other.y) < M.EpsilonDouble &&
                System.Math.Abs(z - other.z) < M.EpsilonDouble;
        }

        #endregion IEquatable<double3> Members

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a double3.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, double3> ParseConverter { get; set; } = (x => Parse(x));

        /// <summary>
        /// Parses a string into a double3.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static double3 Parse(string source, IFormatProvider? provider = null)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            char separator = M.GetNumericListSeparator(provider);

            string[] strings = source.Split(new char[] { separator, '(', ')', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length != 3)
                throw new FormatException("String parse for double3 did not result in exactly 3 items.");

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

            return new double3(doubles[0], doubles[1], doubles[2]);
        }

    }
}