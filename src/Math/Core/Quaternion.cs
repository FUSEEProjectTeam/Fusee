using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    ///     Represents a Quaternion (single precision).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion : IEquatable<Quaternion>
    {
        #region Fields

        private float3 _xyz;
        private float _w;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///     Construct a new Quaternion from vector and w components
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public Quaternion(float3 v, float w)
        {
            _xyz = v;
            _w = w;
        }

        /// <summary>
        ///     Construct a new Quaternion
        /// </summary>
        /// <param name="xx">The xx component</param>
        /// <param name="yy">The yy component</param>
        /// <param name="zz">The zz component</param>
        /// <param name="w">The w component</param>
        public Quaternion(float xx, float yy, float zz, float w)
            : this(new float3(xx, yy, zz), w)
        {
        }

        #endregion Constructors

        #region Public Members

        #region Properties

        /// <summary>
        ///     Gets and sets an Fusee.Math.float3 with the x, y and z components of this instance.
        /// </summary>
        public float3 xyz
        {
            get => _xyz;
            set => _xyz = value;
        }

        /// <summary>
        ///     Gets and sets the x component of this instance.
        /// </summary>
        public float x
        {
            get => _xyz.x;
            set => _xyz.x = value;
        }

        /// <summary>
        ///     Gets and sets the y component of this instance.
        /// </summary>
        public float y
        {
            get => _xyz.y;
            set => _xyz.y = value;
        }

        /// <summary>
        ///     Gets and sets the z component of this instance.
        /// </summary>
        public float z
        {
            get => _xyz.z;
            set => _xyz.z = value;
        }

        /// <summary>
        ///     Gets and sets the w component of this instance.
        /// </summary>
        public float w
        {
            get => _w;
            set => _w = value;
        }

        #endregion Properties

        #region Instance

        #region this

        /// <summary>
        ///     Sets/Gets value from given idx
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
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
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a Quaternion type");
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
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a Quaternion type");
                }
            }
        }

        #endregion this

        #region ToAxisAngle

        /// <summary>
        ///     Convert this instance to an axis-angle representation.
        /// </summary>
        /// <returns>A float4 that is the axis-angle representation of this quaternion.</returns>
        public float4 ToAxisAngle()
        {
            return ToAxisAngle(this);
        }

        /// <summary>
        /// Converts the quaternion into a rotation matrix.
        /// </summary>
        public float4x4 ToRotMat()
        {
            return ToRotMat(this);
        }

        #endregion ToAxisAngle

        #region public float Length

        /// <summary>
        ///     Gets the length (magnitude) of the quaternion.
        /// </summary>
        /// <seealso cref="LengthSquared" />
        public float Length => (float)System.Math.Sqrt(w * w + xyz.LengthSquared);

        #endregion public float Length

        #region public float LengthSquared

        /// <summary>
        ///     Gets the square of the quaternion length (magnitude).
        /// </summary>
        public float LengthSquared => w * w + xyz.LengthSquared;

        #endregion public float LengthSquared

        #region public Normalize()

        /// <summary>
        ///     Scales the Quaternion to unit length.
        /// </summary>
        public Quaternion Normalize()
        {
            return Normalize(this);
        }

        #endregion public Normalize()

        #region public Conjugate()

        /// <summary>
        ///     Convert this quaternion to its conjugate.
        /// </summary>
        public Quaternion Conjugate()
        {
            return Conjugate(this);
        }

        #endregion public Conjugate()

        #region public Invert()

        /// <summary>
        ///     Convert this quaternion to its inverse.
        /// </summary>
        public Quaternion Invert()
        {
            return Invert(this);
        }

        #endregion public Invert()

        #endregion Instance

        #region Static

        #region Fields

        /// <summary>
        ///     Defines the identity quaternion.
        /// </summary>
        public static Quaternion Identity = new Quaternion(0, 0, 0, 1);

        #endregion Fields

        #region Add

        /// <summary>
        ///     Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <returns>The result of the addition</returns>
        public static Quaternion Add(Quaternion left, Quaternion right)
        {
            return new Quaternion(
                left.xyz + right.xyz,
                left.w + right.w);
        }

        #endregion Add

        #region Sub

        /// <summary>
        ///     Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>The result of the operation.</returns>
        public static Quaternion Sub(Quaternion left, Quaternion right)
        {
            return new Quaternion(
                left.xyz - right.xyz,
                left.w - right.w);
        }

        #endregion Sub

        #region Mult

        /// <summary>
        ///     Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
            Quaternion result = new Quaternion(
                left.w * right.x + left.x * right.w - left.y * right.z + left.z * right.y,
                left.w * right.y + left.x * right.z + left.y * right.w - left.z * right.x,
                left.w * right.z - left.x * right.y + left.y * right.x + left.z * right.w,
                left.w * right.w - left.x * right.x - left.y * right.y - left.z * right.z);

            return result;
        }

        /// <summary>
        ///     Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion Multiply(Quaternion quaternion, float scale)
        {
            return new Quaternion(quaternion.x * scale, quaternion.y * scale, quaternion.z * scale, quaternion.w * scale);
        }

        #endregion Mult

        #region Conjugate

        /// <summary>
        ///     Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <returns>The conjugate of the given quaternion</returns>
        public static Quaternion Conjugate(Quaternion q)
        {
            return new Quaternion(-q.xyz, q.w);
        }

        #endregion Conjugate

        #region Invert

        /// <summary>
        ///     Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <returns>The inverse of the given quaternion</returns>
        public static Quaternion Invert(Quaternion q)
        {
            Quaternion result;

            float lengthSq = q.LengthSquared;

            if (lengthSq > M.EpsilonFloat)
            {
                var i = 1.0f / lengthSq;
                result = new Quaternion(q.xyz * -i, q.w * i);
            }
            else
            {
                result = q;
            }

            return result;
        }

        #endregion Invert

        #region Normalize

        /// <summary>
        ///     Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>The normalized quaternion</returns>
        public static Quaternion Normalize(Quaternion q)
        {
            Quaternion result;

            float scale;

            if (!(q.Length > M.EpsilonFloat))
                scale = 0;
            else
                scale = 1.0f / q.Length;

            result = new Quaternion(q.xyz * scale, q.w * scale);

            return result;
        }

        #endregion Normalize

        #region AxisAngle

        /// <summary>
        ///     Build a quaternion from the given axis and angle
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns>A normalized quaternion rotation.</returns>
        public static Quaternion FromAxisAngle(float3 axis, float angle)
        {
            if (axis.LengthSquared < M.EpsilonFloat)
                return Identity;

            if (axis.LengthSquared > 1f)
                return Identity;

            var result = Identity;

            angle *= 0.5f;
            axis = axis.Normalize();
            result.xyz = axis * (float)System.Math.Sin(angle);
            result.w = (float)System.Math.Cos(angle);

            return Normalize(result);
        }

        /// <summary>
        ///     Constructs a rotation matrix from a given quaternion
        ///     This uses some geometric algebra magic https://en.wikipedia.org/wiki/Geometric_algebra
        ///     From: https://sourceforge.net/p/mjbworld/discussion/122133/thread/c59339da/#62ce
        /// </summary>
        /// <param name="quat">Input quaternion</param>
        /// <returns></returns>
        public static float4x4 ToRotMat(Quaternion quat)
        {
            var m1 = new float4x4(
            quat.w, quat.z, -quat.y, quat.x,
            -quat.z, quat.w, quat.x, quat.y,
            quat.y, -quat.x, quat.w, quat.z,
            -quat.x, -quat.y, -quat.z, quat.w).Transpose();

            var m2 = new float4x4(
            quat.w, quat.z, -quat.y, -quat.x,
            -quat.z, quat.w, quat.x, -quat.y,
            quat.y, -quat.x, quat.w, -quat.z,
            quat.x, quat.y, quat.z, quat.w).Transpose();

            return m1 * m2;
        }

        /// <summary>
        /// Angle axis representation of the given quaternion.
        /// </summary>
        /// <param name="quat">The quaternion to transform.</param>
        /// <returns></returns>
        public static float4 ToAxisAngle(Quaternion quat)
        {
            Quaternion q = quat;

            if (q.w > 1.0f)
                q = q.Normalize();

            var result = new float4 { w = 2.0f * (float)System.Math.Acos(q.w) };

            // angle
            var den = (float)System.Math.Sqrt(1.0 - q.w * q.w);

            if (den > M.EpsilonFloat)
            {
                result.xyz = q.xyz / den;
            }
            else
            {
                // This occurs when the angle is zero.
                // Not a problem: just set an arbitrary normalized axis.
                result.xyz = float3.UnitX;
            }

            return result;
        }

        #endregion AxisAngle

        #region Slerp

        /// <summary>
        ///     Do Spherical linear interpolation between two quaternions
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <param name="blend">The blend factor</param>
        /// <returns>A smooth blend between the given quaternions</returns>
        public static Quaternion Slerp(Quaternion q1, Quaternion q2, float blend)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared < M.EpsilonFloat)
            {
                if (q2.LengthSquared < M.EpsilonFloat)
                {
                    return Identity;
                }
                return q2;
            }

            if (q2.LengthSquared < M.EpsilonFloat)
            {
                return q1;
            }

            var cosHalfAngle = q1.w * q2.w + float3.Dot(q1.xyz, q2.xyz);

            // if angle = 0.0f, just return one input.
            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                return q1;
            }

            if (cosHalfAngle < 0.0f)
            {
                q2.xyz = -q2.xyz;
                q2.w = -q2.w;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;

            // A proper slerp requires a division by the sine of the halfAngle.
            // If halfAngle is small, its sine possibly gets close towards zero
            // thus causing calculation errors with single precision float.
            // The following requires halfAngle to be at least 0.5 degrees.
            if (cosHalfAngle < 0.99995f)
            {
                // do proper slerp for big angles
                var halfAngle = (float)System.Math.Acos(cosHalfAngle);
                var sinHalfAngle = (float)System.Math.Sin(halfAngle);
                var oneOverSinHalfAngle = 1.0f / sinHalfAngle;

                blendA = (float)System.Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = (float)System.Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            var result = new Quaternion(blendA * q1.xyz + blendB * q2.xyz, blendA * q1.w + blendB * q2.w);

            return result.LengthSquared > M.EpsilonFloat ? Normalize(result) : Identity;
        }

        #endregion Slerp

        #region Conversion

        /// <summary>
        ///     Convert Euler angle to Quaternion rotation.
        /// </summary>
        /// <param name="e">Euler angle to convert.</param>
        /// <param name="inDegrees">Whether the angles are in degrees or radians.</param>
        /// <returns>A Quaternion representing the euler angle passed to this method.</returns>
        /// <remarks>The euler angle is assumed to be in common aviation order where the y axis is up. Thus x is pitch/attitude,
        /// y is yaw/heading, and z is roll/bank. In practice x is never out of [-PI/2, PI/2] while y and z may well be in
        /// the range of [-PI, PI].
        ///
        /// See also <a href="http://www.euclideanspace.com/maths/geometry/rotations/conversions/eulerToQuaternion/index.htm">the euclideanspace website</a>.
        /// </remarks>
        public static Quaternion EulerToQuaternion(float3 e, bool inDegrees = false)
        {
            if (inDegrees)
            {
                // Converts all degrees angles to radians.
                var rX = M.DegreesToRadians(e.x);
                var rY = M.DegreesToRadians(e.y);
                var rZ = M.DegreesToRadians(e.z);

                e = new float3(rX, rY, rZ);
            }

            var q = new Quaternion();

            float3 s, c;

            s.x = MathF.Sin(e.x * 0.5f);
            c.x = MathF.Cos(e.x * 0.5f);
                      
            s.y = MathF.Sin(e.y * 0.5f);
            c.y = MathF.Cos(e.y * 0.5f);
                      
            s.z = MathF.Sin(e.z * 0.5f);
            c.z = MathF.Cos(e.z * 0.5f);

            q.x = s.x * c.y * c.z + s.y * s.z * c.x;
            q.y = s.y * c.x * c.z - s.x * s.z * c.y;
            q.z = s.z * c.x * c.y - s.x * s.y * c.z;
            q.w = c.x * c.y * c.z + s.y * s.z * s.x;

            return q;
        }

        /// <summary>
        ///     Convert Quaternion rotation to Euler Angles.
        /// </summary>
        /// <param name="q">Quaternion rotation to convert.</param>
        /// <param name="inDegrees">Whether the angles shall be in degrees or radians.</param>
        /// <remarks>The euler angle is assumed to be in common aviation order where the y axis is up. Thus x is pitch/attitude,
        /// y is yaw/heading, and z is roll/bank. In practice x is never out of [-PI/2, PI/2] while y and z may well be in
        /// the range of [-PI, PI].
        ///
        /// See also <a href="http://www.euclideanspace.com/maths/geometry/rotations/conversions/eulerToQuaternion/index.htm">the euclidean space website</a>.
        /// </remarks>
        public static float3 QuaternionToEuler(Quaternion q, bool inDegrees = false)
        {
            // Ref: 3D Math Primer for Graphics and Game Development SE, Page 290 - Listing 8.6, ISBN 978-1-56881-723-1

            float3 euler;
            q = q.Normalize();

            var sp = -2 * (q.y * q.z - q.w * q.x);

            if (MathF.Abs(sp) > 0.99999f)
            {
                euler.x = M.PiOver2 * sp;

                euler.y = MathF.Atan2(-q.x * q.z + q.w * q.y, 0.5f - q.y * q.y - q.z * q.z);
                euler.z = 0;
            }
            else
            {
                euler.x = MathF.Asin(sp);
                euler.y = MathF.Atan2(q.x * q.z + q.w * q.y, 0.5f - q.x * q.x - q.y * q.y);
                euler.z = MathF.Atan2(q.x * q.y + q.w * q.z, 0.5f - q.x * q.x - q.z * q.z);
            }

            if (inDegrees)
            {
                euler.x = M.RadiansToDegrees(euler.x);
                euler.y = M.RadiansToDegrees(euler.y);
                euler.z = M.RadiansToDegrees(euler.z);
            }

            return euler;
        }

        /// <summary>
        /// Build a quaternion from a rotation matrix
        /// </summary>
        /// <param name="mtx"></param>
        /// <returns></returns>
        public static Quaternion FromRotationMatrix(float4x4 mtx)
        {
            mtx = mtx.RotationComponent();
            var t = mtx.Trace - 1;
            var q = new Quaternion();

            if (t > 0f)
            {
                var s = MathF.Sqrt(t + 1) * 2;
                var invS = 1f / s;

                q.w = s * 0.25f;
                q.x = (mtx.Row2.y - mtx.Row1.z) * invS;
                q.y = (mtx.Row0.z - mtx.Row2.x) * invS;
                q.z = (mtx.Row1.x - mtx.Row0.y) * invS;
            }
            else
            {
                if (mtx.Row0.x > mtx.Row1.y && mtx.Row0.x > mtx.Row2.z)
                {
                    var s = MathF.Sqrt(1 + mtx.Row0.x - mtx.Row1.y - mtx.Row2.z) * 2;
                    var invS = 1f / s;

                    q.w = (mtx.Row2.y - mtx.Row1.z) * invS;
                    q.x = s * 0.25f;
                    q.y = (mtx.Row0.y + mtx.Row1.x) * invS;
                    q.z = (mtx.Row0.z + mtx.Row2.x) * invS;
                }
                else if (mtx.Row1.y > mtx.Row2.z)
                {
                    var s = MathF.Sqrt(1 + mtx.Row1.y - mtx.Row0.x - mtx.Row2.z) * 2;
                    var invS = 1f / s;

                    q.w = (mtx.Row0.z - mtx.Row2.x) * invS;
                    q.x = (mtx.Row0.y + mtx.Row1.x) * invS;
                    q.y = s * 0.25f;
                    q.z = (mtx.Row1.z + mtx.Row2.y) * invS;
                }
                else
                {
                    var s = MathF.Sqrt(1 + mtx.Row2.z - mtx.Row0.x - mtx.Row1.y) * 2;
                    var invS = 1f / s;

                    q.w = (mtx.Row1.x - mtx.Row0.y) * invS;
                    q.x = (mtx.Row0.z + mtx.Row2.x) * invS;
                    q.y = (mtx.Row1.z + mtx.Row2.y) * invS;
                    q.z = s * 0.25f;
                }
            }

            return q;
        }

        /// <summary>
        ///     Takes a lookAt and upDirection vector and returns a quaternion rotation.
        /// </summary>
        /// <param name="lookAt">The look at.</param>
        /// <param name="upDirection">Up direction.</param>
        /// <returns>A Quaternion.</returns>
        public static Quaternion LookRotation(float3 lookAt, float3 upDirection)
        {
            float3[] result = float3.OrthoNormalize(lookAt, upDirection);
            upDirection = result[1];
            lookAt = result[0];

            float3 right = float3.Cross(upDirection, lookAt);

            float w = (float)System.Math.Sqrt(1.0f + right.x + upDirection.y + lookAt.z) * 0.5f;
            float w4Recip = 1.0f / (4.0f * w);
            float x = (upDirection.z - lookAt.y) * w4Recip;
            float y = (lookAt.x - right.z) * w4Recip;
            float z = (right.y - upDirection.x) * w4Recip;
            var ret = new Quaternion(x, y, z, w);
            return ret;
        }

        /// <summary>
        ///     Convert Quaternion to rotation matrix
        /// </summary>
        /// <param name="q">Quaternion to convert.</param>
        /// <returns>A matrix of type float4x4 from the passed Quaternion.</returns>
        public static float4x4 QuaternionToMatrix(Quaternion q)
        {
            q = q.Normalize();

            return new float4x4
            {
                M11 = 1 - 2 * (q.y * q.y + q.z * q.z),
                M12 = 2 * (q.x * q.y - q.z * q.w),
                M13 = 2 * (q.x * q.z + q.y * q.w),
                M14 = 0,
                M21 = 2 * (q.x * q.y + q.z * q.w),
                M22 = 1 - 2 * (q.x * q.x + q.z * q.z),
                M23 = 2 * (q.y * q.z - q.x * q.w),
                M24 = 0,
                M31 = 2 * (q.x * q.z - q.y * q.w),
                M32 = 2 * (q.y * q.z + q.x * q.w),
                M33 = 1 - 2 * (q.x * q.x + q.y * q.y),
                M34 = 0,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = 1
            };
        }

        /// <summary>
        ///     a with the algebraic sign of b.
        /// </summary>
        /// <remarks>
        ///     Takes a as an absolute value and multiplies it with: +1 for any positiv number for b, -1 for any negative
        ///     number for b or 0 for 0 for b.
        /// </remarks>
        /// <param name="a">Absolut value</param>
        /// <param name="b">A positiv/negativ number or zero.</param>
        /// <returns>Returns a with the algebraic sign of b.</returns>
        public static float CopySign(float a, float b)
        {
            return System.Math.Abs(a) * System.Math.Sign(b);
        }

        #endregion Conversion

        #region FromToRotation

        /// <summary>
        ///     Build a quaternion with the shortest rotation from to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>A normalized quaternion rotation.</returns>
        public static Quaternion FromToRotation(float3 from, float3 to)
        {
            Quaternion q = new Quaternion();

            float3 a = float3.Cross(from, to);

            q.xyz = a;
            q.w = (float)(System.Math.Sqrt(System.Math.Pow(from.Length, 2) * System.Math.Pow(to.Length, 2)) + float3.Dot(from, to));

            q = q.Normalize();

            return q;
        }

        #endregion FromToRotation

        #region Transform

        /// <summary>
        /// Transforms a vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the vector by.</param>
        /// <returns>The result of the operation.</returns>
        public static float4 Transform(float4 vec, Quaternion quat)
        {
            Quaternion v = new Quaternion(vec.x, vec.y, vec.z, vec.w), i, t;
            i = Invert(quat);
            t = Multiply(v, quat);
            v = Multiply(i, t);

            return new float4(v.x, v.y, v.z, v.w);
        }

        #endregion Transform

        #endregion Static

        #region Operators

        /// <summary>
        ///     Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            return Add(left, right);
        }

        /// <summary>
        ///     Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            return Sub(left, right);
        }

        /// <summary>
        ///     Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            return Multiply(left, right);
        }

        /// <summary>
        ///     Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion operator *(Quaternion quaternion, float scale)
        {
            return Multiply(quaternion, scale);
        }

        /// <summary>
        ///     Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion operator *(float scale, Quaternion quaternion)
        {
            return Multiply(quaternion, scale);
        }

        /// <summary>
        ///     Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !left.Equals(right);
        }

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        ///     Returns a System.String that represents the current Quaternion.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current Quaternion.
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

            return string.Format(provider, "V: {0} w: {1}", xyz.ToString(provider), w.ToString(provider));
        }

        #endregion public override string ToString()

        #region public override bool Equals (object o)

        /// <summary>
        ///     Compares this object instance to another object for equality.
        /// </summary>
        /// <param name="other">The other object to be used in the comparison.</param>
        /// <returns>True if both objects are Quaternions of equal value. Otherwise it returns false.</returns>
        public override bool Equals(object other)
        {
            if (other is Quaternion == false) return false;
            return this == (Quaternion)other;
        }

        #endregion public override bool Equals (object o)

        #region public override int GetHashCode ()

        /// <summary>
        ///     Provides the hash code for this object.
        /// </summary>
        /// <returns>A hash code formed from the bitwise XOR of this objects members.</returns>
        public override int GetHashCode()
        {
            return xyz.GetHashCode() ^ w.GetHashCode();
        }

        #endregion public override int GetHashCode ()

        #endregion Overrides

        #endregion Public Members

        #region IEquatable<Quaternion> Members

        /// <summary>
        ///     Compares this Quaternion instance to another Quaternion for equality.
        /// </summary>
        /// <param name="other">The other Quaternion to be used in the comparison.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        public bool Equals(Quaternion other)
        {
            return xyz == other.xyz && (System.Math.Abs(w - other.w) < M.EpsilonFloat);
        }

        #endregion IEquatable<Quaternion> Members
    }
}