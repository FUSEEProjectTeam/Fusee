using System;
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

        #endregion

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

        #endregion

        #region Public Members

        #region Properties

        // ReSharper disable InconsistentNaming

        /// <summary>
        ///     Gets or sets an Fusee.Math.float3 with the x, y and z components of this instance.
        /// </summary>
        public float3 xyz
        {
            get { return _xyz; }
            set { _xyz = value; }
        }

        /// <summary>
        ///     Gets or sets the x component of this instance.
        /// </summary>
        public float x
        {
            get { return _xyz.x; }
            set { _xyz.x = value; }
        }

        /// <summary>
        ///     Gets or sets the y component of this instance.
        /// </summary>
        public float y
        {
            get { return _xyz.y; }
            set { _xyz.y = value; }
        }

        /// <summary>
        ///     Gets or sets the z component of this instance.
        /// </summary>
        public float z
        {
            get { return _xyz.z; }
            set { _xyz.z = value; }
        }

        /// <summary>
        ///     Gets or sets the w component of this instance.
        /// </summary>
        public float w
        {
            get { return _w; }
            set { _w = value; }
        }

        // ReSharper restore InconsistentNaming

        #endregion

        #region Instance

        #region ToAxisAngle

        /// <summary>
        ///     Convert this instance to an axis-angle representation.
        /// </summary>
        /// <returns>A float4 that is the axis-angle representation of this quaternion.</returns>
        public float4 ToAxisAngle()
        {
            return ToAxisAngle(this);
        }

        #endregion

        #region public float Length

        /// <summary>
        ///     Gets the length (magnitude) of the quaternion.
        /// </summary>
        /// <seealso cref="LengthSquared" />
        public float Length
        {
            get { return (float) System.Math.Sqrt(w*w + xyz.LengthSquared); }
        }

        #endregion

        #region public float LengthSquared

        /// <summary>
        ///     Gets the square of the quaternion length (magnitude).
        /// </summary>
        public float LengthSquared
        {
            get { return w*w + xyz.LengthSquared; }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        ///     Scales the Quaternion to unit length.
        /// </summary>
        public Quaternion Normalize()
        {
            return Normalize(this);
        }

        #endregion

        #region public void Conjugate()

        /// <summary>
        ///     Convert this quaternion to its conjugate.
        /// </summary>
        public Quaternion Conjugate()
        {
            return Conjugate(this);
        }

        #endregion

        #region public void Invert()

        /// <summary>
        ///     Convert this quaternion to its inverse.
        /// </summary>
        public Quaternion Invert()
        {
            return Invert(this);
        }

        #endregion

        #endregion

        #region Static

        #region Fields

        /// <summary>
        ///     Defines the identity quaternion.
        /// </summary>
        public static Quaternion Identity = new Quaternion(0, 0, 0, 1);

        #endregion

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

        #endregion

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

        #endregion

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
            return new Quaternion(quaternion.x*scale, quaternion.y*scale, quaternion.z*scale, quaternion.w*scale);
        }

        #endregion

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

        #endregion

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

        #endregion

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

        #endregion

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
            result.xyz = axis*(float) System.Math.Sin(angle);
            result.w = (float) System.Math.Cos(angle);

            return Normalize(result);
        }

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

        #endregion

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
                    // Console.WriteLine("q1 and q2 have zero length");
                    return Identity;
                }
                // Console.WriteLine("q1 has zero length");
                return q2;
            }

            if ((q2.LengthSquared < M.EpsilonFloat))
            {
                // Console.WriteLine("q2 has zero length");
                return q1;
            }

            var cosHalfAngle = q1.w*q2.w + float3.Dot(q1.xyz, q2.xyz);

            // if angle = 0.0f, just return one input.
            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // Console.WriteLine("cosHalfAngle outside [-1, 1]");
                return q1;
            }

            if (cosHalfAngle < 0.0f)
            {
                // Console.WriteLine("cosHalfAngle < 0");
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
                // Console.WriteLine("Proper Slerp for big angle: " + MathHelper.RadiansToDegrees((float)System.Math.Acos(cosHalfAngle))+ "°");
                // do proper slerp for big angles
                var halfAngle = (float) System.Math.Acos(cosHalfAngle);
                var sinHalfAngle = (float) System.Math.Sin(halfAngle);
                var oneOverSinHalfAngle = 1.0f/sinHalfAngle;

                blendA = (float) System.Math.Sin(halfAngle*(1.0f - blend))*oneOverSinHalfAngle;
                blendB = (float) System.Math.Sin(halfAngle*blend)*oneOverSinHalfAngle;
            }
            else
            {
                // Console.WriteLine("Simple lerp for small angle: " + MathHelper.RadiansToDegrees((float)System.Math.Acos(cosHalfAngle)) + "°");
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            var result = new Quaternion(blendA*q1.xyz + blendB*q2.xyz, blendA*q1.w + blendB*q2.w);

            return result.LengthSquared > M.EpsilonFloat ? Normalize(result) : Identity;
        }

        #endregion

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

            // Calculating the Sine and Cosine for each half angle.
            // YAW/HEADING
            var s1 = (float) System.Math.Sin(e.y*0.5f);
            var c1 = (float) System.Math.Cos(e.y*0.5f);

            // PITCH/ATTITUDE
            var s2 = (float) System.Math.Sin(e.x*0.5f);
            var c2 = (float) System.Math.Cos(e.x*0.5f);

            // ROLL/BANK
            var s3 = (float) System.Math.Sin(e.z*0.5f);
            var c3 = (float) System.Math.Cos(e.z*0.5f);

            // Formula to construct a new Quaternion based on Euler Angles.
            var x = c1 * s2 * c3 - s1 * c2 * s3;
            var z = s1 * s2 * c3 + c1 * c2 * s3;
            var y = s1 * c2 * c3 + c1 * s2 * s3;
            var w = c1 * c2 * c3 - s1 * s2 * s3;

            return new Quaternion(x, y, z, w);
        }

        public static float3 FromQuatToEuler(Quaternion q1)
        {
            float sqw = q1.w * q1.w;
            float sqx = q1.x * q1.x;
            float sqy = q1.y * q1.y;
            float sqz = q1.z * q1.z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = q1.x * q1.w - q1.y * q1.z;
            float3 v = new float3();

            if (test > 0.4995f * unit)
            { // singularity at north pole
                v.y = M.RadiansToDegrees((float) (2f * System.Math.Atan2(q1.y, q1.x)));
                v.x = M.RadiansToDegrees((float) (System.Math.PI / 2f));
                v.z = 0;
                return NormalizeAngles(v);
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.y = M.RadiansToDegrees((float) (-2f * System.Math.Atan2(q1.y, q1.x)));
                v.x = M.RadiansToDegrees((float) (-System.Math.PI / 2));
                v.z = 0;
                return NormalizeAngles(v);
            }
            Quaternion q = new Quaternion(q1.w, q1.z, q1.x, q1.y);
            v.y = M.RadiansToDegrees((float)System.Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w)));     // Yaw
            v.x = M.RadiansToDegrees((float)System.Math.Asin(2f * (q.x * q.z - q.w * q.y)));                             // Pitch
            v.z = M.RadiansToDegrees((float)System.Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z)));      // Roll
            return NormalizeAngles(v);
        }

        static float3 NormalizeAngles(float3 angles)
        {
            angles.x = NormalizeAngle(angles.x);
            angles.y = NormalizeAngle(angles.y);
            angles.z = NormalizeAngle(angles.z);
            return angles;
        }

        static float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
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
            q = q.Normalize();

            float test = 2.0f*(q.x*q.y + q.z*q.w);

            float x;
            float y;
            float z;

            if (M.Equals(test, 1.0f))
            {
                y = 2.0f*(float) System.Math.Atan2(q.x, q.w);
                x = 0;
                z = M.Pi/2;
            }
            else if (M.Equals(test, -1.0f))
            {
                y = -2.0f*(float) System.Math.Atan2(q.x, q.w);
                x = 0;
                z = M.Pi/-2;
            }
            else
            {
                var sqX = q.x*q.x;
                var sqY = q.y*q.y;
                var sqZ = q.z*q.z;
                var sqW = q.w*q.w;

                y = (float) System.Math.Atan2(2*(q.y*q.w - q.x*q.z), 1 - 2*sqY - 2*sqZ);
                z = (float) System.Math.Asin(M.Clamp(test, -1.0f, 1.0f));
                x = (float) System.Math.Atan2(2*(q.x*q.w - q.y*q.z), 1 - 2*sqX - 2*sqZ);
            }

            // Clamp angles to ranges arond 0 (e.g. [-PI, PI] for yaw)
            while (y >= M.TwoPi)
                y -= M.TwoPi;
            while (y <= -M.TwoPi)
                y += M.TwoPi;

            if (inDegrees)
            {
                x = M.RadiansToDegrees(x);
                y = M.RadiansToDegrees(y);
                z = M.RadiansToDegrees(z);
            }

            return new float3(z, y, x);
        }

        /// <summary>
        ///     Takes a float4x4 matric and returns quaternions.
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

            float w = (float) System.Math.Sqrt(1.0f + right.x + upDirection.y + lookAt.z)*0.5f;
            float w4Recip = 1.0f/(4.0f*w);
            float x = (upDirection.z - lookAt.y)*w4Recip;
            float y = (lookAt.x - right.z)*w4Recip;
            float z = (right.y - upDirection.x)*w4Recip;
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
            return System.Math.Abs(a)*System.Math.Sign(b);
        }

        #endregion

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

            q.Normalize();

            return q;
        }

        #endregion

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
            t = Multiply(quat,v);
            v = Multiply(t,i);

            return new float4(v.x, v.y, v.z, v.w);
        }

        #endregion

        #endregion

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

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        ///     Returns a System.String that represents the current Quaternion.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return String.Format("V: {0}, w: {1}", xyz, w);
        }

        #endregion

        #region public override bool Equals (object o)

        /// <summary>
        ///     Compares this object instance to another object for equality.
        /// </summary>
        /// <param name="other">The other object to be used in the comparison.</param>
        /// <returns>True if both objects are Quaternions of equal value. Otherwise it returns false.</returns>
        public override bool Equals(object other)
        {
            if (other is Quaternion == false) return false;
            return this == (Quaternion) other;
        }

        #endregion

        #region public override int GetHashCode ()

        /// <summary>
        ///     Provides the hash code for this object.
        /// </summary>
        /// <returns>A hash code formed from the bitwise XOR of this objects members.</returns>
        public override int GetHashCode()
        {
            return xyz.GetHashCode() ^ w.GetHashCode();
        }

        #endregion

        #endregion

        #endregion

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

        #endregion
    }
}
