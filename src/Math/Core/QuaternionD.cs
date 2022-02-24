using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    ///     Represents a QuaternionD (single precision).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct QuaternionD : IEquatable<QuaternionD>
    {
        #region Fields

        private double3 _xyz;
        private double _w;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///     Construct a new QuaternionD from vector and w components
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public QuaternionD(double3 v, double w)
        {
            _xyz = v;
            _w = w;
        }

        /// <summary>
        ///     Construct a new QuaternionD
        /// </summary>
        /// <param name="xx">The xx component</param>
        /// <param name="yy">The yy component</param>
        /// <param name="zz">The zz component</param>
        /// <param name="w">The w component</param>
        public QuaternionD(double xx, double yy, double zz, double w)
            : this(new double3(xx, yy, zz), w)
        {
        }

        #endregion Constructors

        #region Public Members

        #region Properties

        /// <summary>
        ///     Gets and sets an Fusee.Math.double3 with the x, y and z components of this instance.
        /// </summary>
        public double3 xyz
        {
            get => _xyz;
            set => _xyz = value;
        }

        /// <summary>
        ///     Gets and sets the x component of this instance.
        /// </summary>
        public double x
        {
            get => _xyz.x;
            set => _xyz.x = value;
        }

        /// <summary>
        ///     Gets and sets the y component of this instance.
        /// </summary>
        public double y
        {
            get => _xyz.y;
            set => _xyz.y = value;
        }

        /// <summary>
        ///     Gets and sets the z component of this instance.
        /// </summary>
        public double z
        {
            get => _xyz.z;
            set => _xyz.z = value;
        }

        /// <summary>
        ///     Gets and sets the w component of this instance.
        /// </summary>
        public double w
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
        public double this[int idx]
        {
            get
            {
                return idx switch
                {
                    0 => x,
                    1 => y,
                    2 => z,
                    3 => w,
                    _ => throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a QuaternionD type"),
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
                        throw new ArgumentOutOfRangeException($"Index {idx} not eligible for a QuaternionD type");
                }
            }
        }

        #endregion this

        #region ToAxisAngle

        /// <summary>
        ///     Convert this instance to an axis-angle representation.
        /// </summary>
        /// <returns>A double4 that is the axis-angle representation of this quaternion.</returns>
        public double4 ToAxisAngle()
        {
            return ToAxisAngle(this);
        }

        /// <summary>
        /// Converts the quaternion into a rotation matrix.
        /// </summary>
        public double4x4 ToRotationMatrixFast()
        {
            return ToRotationMatrixFast(this);
        }

        /// <summary>
        /// Converts the quaternion into a rotation matrix.
        /// </summary>
        public double4x4 ToRotationMatrix()
        {
            return ToRotationMatrix(this);
        }

        #endregion ToAxisAngle

        #region public double Length

        /// <summary>
        ///     Gets the length (magnitude) of the quaternion.
        /// </summary>
        /// <seealso cref="LengthSquared" />
        public double Length => System.Math.Sqrt(w * w + xyz.LengthSquared);

        #endregion public double Length

        #region public double LengthSquared

        /// <summary>
        ///     Gets the square of the quaternion length (magnitude).
        /// </summary>
        public double LengthSquared => w * w + xyz.LengthSquared;

        #endregion public double LengthSquared

        #region public Normalize()

        /// <summary>
        ///     Scales the QuaternionD to unit length.
        /// </summary>
        public QuaternionD Normalize()
        {
            return Normalize(this);
        }

        #endregion public Normalize()

        #region public Conjugate()

        /// <summary>
        ///     Convert this quaternion to its conjugate.
        /// </summary>
        public QuaternionD Conjugate()
        {
            return Conjugate(this);
        }

        #endregion public Conjugate()

        #region public Invert()

        /// <summary>
        ///     Convert this quaternion to its inverse.
        /// </summary>
        public QuaternionD Invert()
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
        public static readonly QuaternionD Identity = new(0, 0, 0, 1);

        #endregion Fields

        #region Add

        /// <summary>
        ///     Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <returns>The result of the addition</returns>
        public static QuaternionD Add(QuaternionD left, QuaternionD right)
        {
            return new QuaternionD(
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
        public static QuaternionD Sub(QuaternionD left, QuaternionD right)
        {
            return new QuaternionD(
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
        public static QuaternionD Multiply(QuaternionD left, QuaternionD right)
        {
            QuaternionD result = new(
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
        public static QuaternionD Multiply(QuaternionD quaternion, double scale)
        {
            return new QuaternionD(quaternion.x * scale, quaternion.y * scale, quaternion.z * scale, quaternion.w * scale);
        }

        #endregion Mult

        #region Conjugate

        /// <summary>
        ///     Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <returns>The conjugate of the given quaternion</returns>
        public static QuaternionD Conjugate(QuaternionD q)
        {
            return new QuaternionD(-q.xyz, q.w);
        }

        #endregion Conjugate

        #region Invert

        /// <summary>
        ///     Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <returns>The inverse of the given quaternion</returns>
        public static QuaternionD Invert(QuaternionD q)
        {
            QuaternionD result;

            double lengthSq = q.LengthSquared;

            if (lengthSq > M.EpsilonDouble)
            {
                var i = 1.0 / lengthSq;
                result = new QuaternionD(q.xyz * -i, q.w * i);
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
        public static QuaternionD Normalize(QuaternionD q)
        {
            QuaternionD result;

            double scale;

            if (!(q.Length > M.EpsilonDouble))
                scale = 0;
            else
                scale = 1.0 / q.Length;

            result = new QuaternionD(q.xyz * scale, q.w * scale);

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
        public static QuaternionD FromAxisAngle(double3 axis, double angle)
        {
            if (axis.LengthSquared < M.EpsilonDouble)
                return Identity;

            if (axis.LengthSquared > 1)
                return Identity;

            var result = Identity;

            angle *= 0.5;
            axis = axis.Normalize();
            result.xyz = axis * System.Math.Sin(angle);
            result.w = System.Math.Cos(angle);

            return Normalize(result);
        }

        /// <summary>
        /// Angle axis representation of the given quaternion.
        /// </summary>
        /// <param name="quat">The quaternion to transform.</param>
        /// <returns></returns>
        public static double4 ToAxisAngle(QuaternionD quat)
        {
            QuaternionD q = quat;

            if (q.w > 1.0)
                q = q.Normalize();

            var result = new double4 { w = 2.0 * System.Math.Acos(q.w) };

            // angle
            var den = System.Math.Sqrt(1.0 - q.w * q.w);

            if (den > M.EpsilonDouble)
            {
                result.xyz = q.xyz / den;
            }
            else
            {
                // This occurs when the angle is zero.
                // Not a problem: just set an arbitrary normalized axis.
                result.xyz = double3.UnitX;
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
        public static QuaternionD Slerp(QuaternionD q1, QuaternionD q2, double blend)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared < M.EpsilonDouble)
            {
                if (q2.LengthSquared < M.EpsilonDouble)
                {
                    return Identity;
                }
                return q2;
            }

            if (q2.LengthSquared < M.EpsilonDouble)
            {
                return q1;
            }

            var cosHalfAngle = q1.w * q2.w + double3.Dot(q1.xyz, q2.xyz);

            // if angle = 0.0f, just return one input.
            if (cosHalfAngle >= 1.0 || cosHalfAngle <= -1.0)
            {
                return q1;
            }

            if (cosHalfAngle < 0.0)
            {
                q2.xyz = -q2.xyz;
                q2.w = -q2.w;
                cosHalfAngle = -cosHalfAngle;
            }

            double blendA;
            double blendB;

            // A proper slerp requires a division by the sine of the halfAngle.
            // If halfAngle is small, its sine possibly gets close towards zero
            // thus causing calculation errors with single precision double.
            // The following requires halfAngle to be at least 0.5 degrees.
            if (cosHalfAngle < 0.99995)
            {
                // do proper slerp for big angles
                var halfAngle = System.Math.Acos(cosHalfAngle);
                var sinHalfAngle = System.Math.Sin(halfAngle);
                var oneOverSinHalfAngle = 1.0 / sinHalfAngle;

                blendA = System.Math.Sin(halfAngle * (1.0 - blend)) * oneOverSinHalfAngle;
                blendB = System.Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0 - blend;
                blendB = blend;
            }

            var result = new QuaternionD(blendA * q1.xyz + blendB * q2.xyz, blendA * q1.w + blendB * q2.w);

            return result.LengthSquared > M.EpsilonDouble ? Normalize(result) : Identity;
        }

        #endregion Slerp

        #region Conversion

        /// <summary>
        /// Convert Euler angle to Quaternion rotation.
        /// </summary>
        /// <param name="x">Angle around x.</param>
        /// <param name="y">Angle around x.</param>
        /// <param name="z">Angle around x.</param>
        /// <param name="inDegrees">Whether the angles are in degrees or radians.</param>
        /// <returns>A Quaternion representing the euler angle passed to this method.</returns>
        /// <remarks>The euler angle is assumed to be in common aviation order where the y axis is up. Thus x is pitch/attitude,
        /// y is yaw/heading, and z is roll/bank. In practice x is never out of [-PI/2, PI/2] while y and z may well be in
        /// the range of [-PI, PI].
        ///
        /// See also <a href="http://www.euclideanspace.com/maths/geometry/rotations/conversions/eulerToQuaternion/index.htm">the euclideanspace website</a>.
        /// </remarks>
        public static QuaternionD FromEuler(double x, double y, double z, bool inDegrees = false)
        {
            if (inDegrees)
            {
                // Converts all degrees angles to radians.
                x = M.DegreesToRadiansD(x);
                y = M.DegreesToRadiansD(y);
                z = M.DegreesToRadiansD(z);
            }

            var q = new QuaternionD();

            double3 s, c;

            s.x = System.Math.Sin(x * 0.5);
            c.x = System.Math.Cos(x * 0.5);

            s.y = System.Math.Sin(y * 0.5);
            c.y = System.Math.Cos(y * 0.5);

            s.z = System.Math.Sin(z * 0.5);
            c.z = System.Math.Cos(z * 0.5);

            q.x = s.x * c.y * c.z + s.y * s.z * c.x;
            q.y = s.y * c.x * c.z - s.x * s.z * c.y;
            q.z = s.z * c.x * c.y - s.x * s.y * c.z;
            q.w = c.x * c.y * c.z + s.y * s.z * s.x;

            return q;
        }


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
        public static QuaternionD FromEuler(double3 e, bool inDegrees = false)
        {
            return FromEuler(e.x, e.y, e.z, inDegrees);
        }

        /// <summary>
        ///     Convert QuaternionD rotation to Euler Angles.
        /// </summary>
        /// <param name="q">QuaternionD rotation to convert.</param>
        /// <param name="inDegrees">Whether the angles shall be in degrees or radians.</param>
        /// <remarks>The euler angle is assumed to be in common aviation order where the y axis is up. Thus x is pitch/attitude,
        /// y is yaw/heading, and z is roll/bank. In practice x is never out of [-PI/2, PI/2] while y and z may well be in
        /// the range of [-PI, PI].
        ///
        /// See also <a href="http://www.euclideanspace.com/maths/geometry/rotations/conversions/eulerToQuaternionD/index.htm">the euclidean space website</a>.
        /// </remarks>
        public static double3 ToEuler(QuaternionD q, bool inDegrees = false)
        {
            // Ref: 3D Math Primer for Graphics and Game Development SE, Page 290 - Listing 8.6, ISBN 978-1-56881-723-1

            double3 euler;
            q = q.Normalize();

            var sp = -2 * (q.y * q.z - q.w * q.x);

            if (System.Math.Abs(sp) > (1.0 - M.EpsilonDouble))
            {
                euler.x = M.PiOver2 * sp;

                euler.y = System.Math.Atan2(-q.x * q.z + q.w * q.y, 0.5 - q.y * q.y - q.z * q.z);
                euler.z = 0;
            }
            else
            {
                euler.x = System.Math.Asin(sp);
                euler.y = System.Math.Atan2(q.x * q.z + q.w * q.y, 0.5 - q.x * q.x - q.y * q.y);
                euler.z = System.Math.Atan2(q.x * q.y + q.w * q.z, 0.5 - q.x * q.x - q.z * q.z);
            }

            if (inDegrees)
            {
                euler.x = M.RadiansToDegreesD(euler.x);
                euler.y = M.RadiansToDegreesD(euler.y);
                euler.z = M.RadiansToDegreesD(euler.z);
            }

            return euler;
        }

        /// <summary>
        /// Build a quaternion from a rotation matrix
        /// </summary>
        /// <param name="mtx"></param>
        /// <returns></returns>
        public static QuaternionD FromRotationMatrix(double4x4 mtx)
        {
            mtx = mtx.RotationComponent();
            var t = mtx.Trace - 1;
            var q = new QuaternionD();

            if (t > 0)
            {
                var s = System.Math.Sqrt(t + 1) * 2;
                var invS = 1 / s;

                q.w = s * 0.25;
                q.x = (mtx.Row3.y - mtx.Row2.z) * invS;
                q.y = (mtx.Row1.z - mtx.Row3.x) * invS;
                q.z = (mtx.Row2.x - mtx.Row1.y) * invS;
            }
            else
            {
                if (mtx.Row1.x > mtx.Row2.y && mtx.Row1.x > mtx.Row3.z)
                {
                    var s = System.Math.Sqrt(1 + mtx.Row1.x - mtx.Row2.y - mtx.Row3.z) * 2;
                    var invS = 1 / s;

                    q.w = (mtx.Row3.y - mtx.Row2.z) * invS;
                    q.x = s * 0.25;
                    q.y = (mtx.Row1.y + mtx.Row2.x) * invS;
                    q.z = (mtx.Row1.z + mtx.Row3.x) * invS;
                }
                else if (mtx.Row2.y > mtx.Row3.z)
                {
                    var s = System.Math.Sqrt(1 + mtx.Row2.y - mtx.Row1.x - mtx.Row3.z) * 2;
                    var invS = 1 / s;

                    q.w = (mtx.Row1.z - mtx.Row3.x) * invS;
                    q.x = (mtx.Row1.y + mtx.Row2.x) * invS;
                    q.y = s * 0.25;
                    q.z = (mtx.Row2.z + mtx.Row3.y) * invS;
                }
                else
                {
                    var s = System.Math.Sqrt(1 + mtx.Row3.z - mtx.Row1.x - mtx.Row2.y) * 2;
                    var invS = 1 / s;

                    q.w = (mtx.Row2.x - mtx.Row1.y) * invS;
                    q.x = (mtx.Row1.z + mtx.Row3.x) * invS;
                    q.y = (mtx.Row2.z + mtx.Row3.y) * invS;
                    q.z = s * 0.25;
                }
            }

            return q;
        }

        /// <summary>
        ///     Takes a lookAt and upDirection vector and returns a quaternion rotation.
        /// </summary>
        /// <param name="lookAt">The look at.</param>
        /// <param name="upDirection">Up direction.</param>
        /// <returns>A QuaternionD.</returns>
        public static QuaternionD LookRotation(double3 lookAt, double3 upDirection)
        {
            double3[] result = double3.OrthoNormalize(lookAt, upDirection);
            upDirection = result[1];
            lookAt = result[0];

            double3 right = double3.Cross(upDirection, lookAt);

            double w = System.Math.Sqrt(1.0 + right.x + upDirection.y + lookAt.z) * 0.5;
            double w4Recip = 1.0 / (4.0 * w);
            double x = (upDirection.z - lookAt.y) * w4Recip;
            double y = (lookAt.x - right.z) * w4Recip;
            double z = (right.y - upDirection.x) * w4Recip;
            var ret = new QuaternionD(x, y, z, w);
            return ret;
        }

        /// <summary>
        ///     Constructs a rotation matrix from a given quaternion
        ///     This uses some geometric algebra magic https://en.wikipedia.org/wiki/Geometric_algebra
        ///     From: https://sourceforge.net/p/mjbworld/discussion/122133/thread/c59339da/#62ce
        /// </summary>
        /// <param name="quat">Input quaternion</param>
        /// <returns></returns>
        public static double4x4 ToRotationMatrixFast(QuaternionD quat)
        {
            var m1 = new double4x4(
            quat.w, quat.z, -quat.y, quat.x,
            -quat.z, quat.w, quat.x, quat.y,
            quat.y, -quat.x, quat.w, quat.z,
            -quat.x, -quat.y, -quat.z, quat.w).Transpose();

            var m2 = new double4x4(
            quat.w, quat.z, -quat.y, -quat.x,
            -quat.z, quat.w, quat.x, -quat.y,
            quat.y, -quat.x, quat.w, -quat.z,
            quat.x, quat.y, quat.z, quat.w).Transpose();

            return m1 * m2;
        }

        /// <summary>
        ///     Convert QuaternionD to rotation matrix
        /// </summary>
        /// <param name="q">QuaternionD to convert.</param>
        /// <returns>A matrix of type double4x4 from the passed QuaternionD.</returns>
        public static double4x4 ToRotationMatrix(QuaternionD q)
        {
            q = q.Normalize();

            return new double4x4
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
        public static double CopySign(double a, double b)
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
        public static QuaternionD FromToRotation(double3 from, double3 to)
        {
            QuaternionD q = new();

            double3 a = double3.Cross(from, to);

            q.xyz = a;
            q.w = (System.Math.Sqrt(System.Math.Pow(from.Length, 2) * System.Math.Pow(to.Length, 2)) + double3.Dot(from, to));

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
        public static double4 Transform(double4 vec, QuaternionD quat)
        {
            QuaternionD v = new(vec.x, vec.y, vec.z, vec.w), i, t;
            i = Invert(quat);
            t = Multiply(v, quat);
            v = Multiply(i, t);

            return new double4(v.x, v.y, v.z, v.w);
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
        public static QuaternionD operator +(QuaternionD left, QuaternionD right)
        {
            return Add(left, right);
        }

        /// <summary>
        ///     Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static QuaternionD operator -(QuaternionD left, QuaternionD right)
        {
            return Sub(left, right);
        }

        /// <summary>
        ///     Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static QuaternionD operator *(QuaternionD left, QuaternionD right)
        {
            return Multiply(left, right);
        }

        /// <summary>
        ///     Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static QuaternionD operator *(QuaternionD quaternion, double scale)
        {
            return Multiply(quaternion, scale);
        }

        /// <summary>
        ///     Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static QuaternionD operator *(double scale, QuaternionD quaternion)
        {
            return Multiply(quaternion, scale);
        }

        /// <summary>
        ///     Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(QuaternionD left, QuaternionD right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(QuaternionD left, QuaternionD right)
        {
            return !left.Equals(right);
        }

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        ///     Returns a System.String that represents the current QuaternionD.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current QuaternionD.
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
        /// <returns>True if both objects are QuaternionDs of equal value. Otherwise it returns false.</returns>
        public override bool Equals(object? other)
        {
            if (other is QuaternionD == false) return false;
            return this == (QuaternionD)other;
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

        #region IEquatable<QuaternionD> Members

        /// <summary>
        ///     Compares this QuaternionD instance to another QuaternionD for equality.
        /// </summary>
        /// <param name="other">The other QuaternionD to be used in the comparison.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        public bool Equals(QuaternionD other)
        {
            return xyz == other.xyz && (System.Math.Abs(w - other.w) < M.EpsilonDouble);
        }

        #endregion IEquatable<QuaternionD> Members
    }
}