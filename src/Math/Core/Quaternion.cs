using System;
using System.Runtime.InteropServices;

namespace Fusee.Math
{
    /// <summary>
    /// Represents a Quaternion (single precision).
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion : IEquatable<Quaternion>
    {
        #region Fields

        float3 _xyz;
        float _w;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new Quaternion from vector and w components
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public Quaternion(float3 v, float w)
        {
            _xyz = v;
            _w = w;
        }

        /// <summary>
        /// Construct a new Quaternion
        /// </summary>
        /// <param name="xx">The xx component</param>
        /// <param name="yy">The yy component</param>
        /// <param name="zz">The zz component</param>
        /// <param name="w">The w component</param>
        public Quaternion(float xx, float yy, float zz, float w)
            : this(new float3(xx, yy, zz), w)
        { }

        #endregion

        #region Public Members

        #region Properties
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets an Fusee.Math.float3 with the x, y and z components of this instance.
        /// </summary>
        public float3 xyz { get { return _xyz; } set { _xyz = value; } }

        /// <summary>
        /// Gets or sets the x component of this instance.
        /// </summary>
        public float x { get { return _xyz.x; } set { _xyz.x = value; } }

        /// <summary>
        /// Gets or sets the y component of this instance.
        /// </summary>
         public float y { get { return _xyz.y; } set { _xyz.y = value; } }

        /// <summary>
        /// Gets or sets the z component of this instance.
        /// </summary>
        public float z { get { return _xyz.z; } set { _xyz.z = value; } }

        /// <summary>
        /// Gets or sets the w component of this instance.
        /// </summary>
        public float w { get { return _w; } set { _w = value; } }

        // ReSharper restore InconsistentNaming
        #endregion

        #region Instance

        #region ToAxisAngle

        /// <summary>
        /// Convert the current quaternion to axis angle representation
        /// </summary>
        /// <param name="axis">The resultant axis</param>
        /// <param name="angle">The resultant angle</param>
        public void ToAxisAngle(out float3 axis, out float angle)
        {
            float4 result = ToAxisAngle();
            axis = result.xyz;
            angle = result.w;
        }

        /// <summary>
        /// Convert this instance to an axis-angle representation.
        /// </summary>
        /// <returns>A float4 that is the axis-angle representation of this quaternion.</returns>
        public float4 ToAxisAngle()
        {
            Quaternion q = this;

            if (q.w > 1.0f)
                q.Normalize();

            var result = new float4 {w = 2.0f*(float) System.Math.Acos(q.w)};

            // angle
            var den = (float)System.Math.Sqrt(1.0 - q.w * q.w);

            if (den > MathHelper.EpsilonFloat)
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

        #region public float Length

        /// <summary>
        /// Gets the length (magnitude) of the quaternion.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(w * w + xyz.LengthSquared);
            }
        }

        #endregion

        #region public float LengthSquared

        /// <summary>
        /// Gets the square of the quaternion length (magnitude).
        /// </summary>
        public float LengthSquared
        {
            get
            {
                return w * w + xyz.LengthSquared;
            }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the Quaternion to unit length.
        /// </summary>
        public void Normalize()
        {
            if (!(Length > MathHelper.EpsilonFloat)) return;
            var scale = 1.0f / Length;
            xyz *= scale;
            w *= scale;
        }

        #endregion

        #region public void Conjugate()

        /// <summary>
        /// Convert this quaternion to its conjugate
        /// </summary>
        public void Conjugate()
        {
            xyz = -xyz;
        }

        #endregion

        #endregion

        #region Static

        #region Fields

        /// <summary>
        /// Defines the identity quaternion.
        /// </summary>
        public static Quaternion Identity = new Quaternion(0, 0, 0, 1);

        #endregion

        #region Add

        /// <summary>
        /// Add two quaternions
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

        /// <summary>
        /// Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <param name="result">The result of the addition</param>
        public static void Add(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result = new Quaternion(
                left.xyz + right.xyz,
                left.w + right.w);
        }

        #endregion

        #region Sub

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>The result of the operation.</returns>
        public static Quaternion Sub(Quaternion left, Quaternion right)
        {
            return  new Quaternion(
                left.xyz - right.xyz,
                left.w - right.w);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Sub(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result = new Quaternion(
                left.xyz - right.xyz,
                left.w - right.w);
        }

        #endregion

        #region Mult

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        [Obsolete("Use Multiply instead.")]
        public static Quaternion Mult(Quaternion left, Quaternion right)
        {
            return new Quaternion(
                right.w * left.xyz + left.w * right.xyz + float3.Cross(left.xyz, right.xyz),
                left.w * right.w - float3.Dot(left.xyz, right.xyz));
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        [Obsolete("Use Multiply instead.")]
        public static void Mult(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result = new Quaternion(
                right.w * left.xyz + left.w * right.xyz + float3.Cross(left.xyz, right.xyz),
                left.w * right.w - float3.Dot(left.xyz, right.xyz));
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
            Quaternion result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result = new Quaternion(
                left.w*right.x + left.x*right.w + left.y*right.z - left.z*right.y,
                left.w*right.y + left.y*right.w + left.z*right.x - left.x*right.z,
                left.w*right.z + left.z*right.w + left.x*right.y - left.y*right.x,
                left.w*right.w - left.x*right.x - left.y*right.y - left.z*right.z);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref Quaternion quaternion, float scale, out Quaternion result)
        {
            result = new Quaternion(quaternion.x * scale, quaternion.y * scale, quaternion.z * scale, quaternion.w * scale);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion Multiply(Quaternion quaternion, float scale)
        {
            return new Quaternion(quaternion.x * scale, quaternion.y * scale, quaternion.z * scale, quaternion.w * scale);
        }

        #endregion

        #region Conjugate

        /// <summary>
        /// Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <returns>The conjugate of the given quaternion</returns>
        public static Quaternion Conjugate(Quaternion q)
        {
            return new Quaternion(-q.xyz, q.w);
        }

        /// <summary>
        /// Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <param name="result">The conjugate of the given quaternion</param>
        public static void Conjugate(ref Quaternion q, out Quaternion result)
        {
            result = new Quaternion(-q.xyz, q.w);
        }

        #endregion

        #region Invert

        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <returns>The inverse of the given quaternion</returns>
        public static Quaternion Invert(Quaternion q)
        {
            Quaternion result;
            Invert(ref q, out result);
            return result;
        }

        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <param name="result">The inverse of the given quaternion</param>
        public static void Invert(ref Quaternion q, out Quaternion result)
        {
            float lengthSq = q.LengthSquared;

            if (lengthSq > MathHelper.EpsilonFloat)
            {
                var i = 1.0f / lengthSq;
                result = new Quaternion(q.xyz * -i, q.w * i);
            }
            else
            {
                result = q;
            }
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>The normalized quaternion</returns>
        public static Quaternion Normalize(Quaternion q)
        {
            Quaternion result;
            Normalize(ref q, out result);
            return result;
        }

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <param name="result">The normalized quaternion</param>
        public static void Normalize(ref Quaternion q, out Quaternion result)
        {
            float scale;

            if (!(q.Length > MathHelper.EpsilonFloat))
                scale = 0;
            else
                scale = 1.0f / q.Length;
           
            result = new Quaternion(q.xyz * scale, q.w * scale);
        }

        #endregion

        #region FromAxisAngle

        /// <summary>
        /// Build a quaternion from the given axis and angle
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns>A normalized quaternion rotation.</returns>
        public static Quaternion FromAxisAngle(float3 axis, float angle)
        {
            if (axis.LengthSquared > MathHelper.EpsilonFloat)
                return Identity;

            var result = Identity;

            angle *= 0.5f;
            axis.Normalize();
            result.xyz = axis * (float)System.Math.Sin(angle);
            result.w = (float)System.Math.Cos(angle);

            return Normalize(result);
        }

        #endregion

        #region Slerp

        /// <summary>
        /// Do Spherical linear interpolation between two quaternions 
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <param name="blend">The blend factor</param>
        /// <returns>A smooth blend between the given quaternions</returns>
        public static Quaternion Slerp(Quaternion q1, Quaternion q2, float blend)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared < MathHelper.EpsilonFloat)
                return (!(q2.LengthSquared > MathHelper.EpsilonFloat)) ? Identity : q2;
            
            if ((q2.LengthSquared < MathHelper.EpsilonFloat))
                return q1;

            var cosHalfAngle = q1.w * q2.w + float3.Dot(q1.xyz, q2.xyz);

            // if angle = 0.0f, just return one input.
            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
                return q1;
            
            if (cosHalfAngle < 0.0f)
            {
                q2.xyz = -q2.xyz;
                q2.w = -q2.w;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;

            if (cosHalfAngle < 0.99f)
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

            return result.LengthSquared > MathHelper.EpsilonFloat ? Normalize(result) : Identity;
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Convert Euler angle to Quaternion rotation.
        /// </summary>
        /// <param name="e">Euler angle to convert.</param>
        /// <param name="inDegrees">Whether the angles are in degrees or radians.</param>
        /// <returns>A Quaternion representing the euler angle passed to this method.</returns>
        public static Quaternion EulerToQuaternion(float3 e, bool inDegrees = false)
        {
            if (inDegrees)
            {
                // Converts all degrees angles to radians.
                var rX = MathHelper.DegreesToRadians(e.x);
                var rY = MathHelper.DegreesToRadians(e.y);
                var rZ = MathHelper.DegreesToRadians(e.z);

                e = new float3(rX, rY, rZ);
            }

            // Calculating the Sine and Cosine for each half angle.
            var sX = (float)System.Math.Sin(e.x * 0.5f);
            var cX = (float)System.Math.Cos(e.x * 0.5f);

            var sY = (float)System.Math.Sin(e.y * 0.5f);
            var cY = (float)System.Math.Cos(e.y * 0.5f);

            var sZ = (float)System.Math.Sin(e.z * 0.5f);
            var cZ = (float)System.Math.Cos(e.z * 0.5f);

            // Formula to construct a new Quaternion based on Euler Angles.
            var x = sX * cY * cZ - cX * sY * sZ;
            var y = cX * sY * cZ + sX * cY * sZ;
            var z = cX * cY * sZ - sX * sY * cZ;
            var w = cX * cY * cZ + sX * sY * sZ;

            return new Quaternion(x, y, z, w);
        }

        /// <summary>
        /// Convert Quaternion rotation to Euler Angles.
        /// </summary>
        /// <param name="q">Quaternion rotation to convert.</param>
        /// <param name="inDegrees">Whether the angles shall be in degrees or radians.</param>
        /// <returns>An Euler angle of type float3 from the passed Quaternion rotation.</returns>
        public static float3 QuaternionToEuler(Quaternion q, bool inDegrees = false)
        {
            q.Normalize();

            var test = 2.0*(q.y*q.w - q.x*q.z);

            float x;
            float y;
            float z;

            if (MathHelper.Equals(test, 1.0f))
            {
                z = -2.0f*(float) System.Math.Atan2(q.x, q.w);
                x = 0;
                y = MathHelper.Pi / 2;
            }
            else if (MathHelper.Equals(test, -1.0f))
            {
                z = 2.0f*(float) System.Math.Atan2(q.x, q.w);
                x = 0;
                y = MathHelper.Pi/-2;
            }
            else
            {
                var sqX = q.x*q.x;
                var sqY = q.y*q.y;
                var sqZ = q.z*q.z;
                var sqW = q.w*q.w;

                x = (float) System.Math.Atan2(2*(q.y*q.z + q.w*q.x), sqW - sqX - sqY + sqZ);
                y = (float) System.Math.Asin(MathHelper.Clamp(test, -1.0f, 1.0f));
                z = (float) System.Math.Atan2(2*(q.x*q.y + q.w*q.z), sqW + sqX - sqY - sqZ);
            }

            if (inDegrees)
            {
                x = MathHelper.RadiansToDegrees(x);
                y = MathHelper.RadiansToDegrees(y);
                z = MathHelper.RadiansToDegrees(z);
            }

            return new float3(x, y, z);
        }

        /// <summary>
        /// Takes a float4x4 matric and returns quaternions.
        /// </summary>
        /// <param name="lookAt">The look at.</param>
        /// <param name="upDirection">Up direction.</param>
        /// <returns>A Quaternion.</returns>
        public static Quaternion LookRotation(float3 lookAt, float3 upDirection)
        {
            float3[] result = float3.OrthoNormalize(lookAt,upDirection);
            upDirection = result[1];
            lookAt = result[0];

            float3 right = float3.Cross(upDirection, lookAt);
            
            float w = (float)System.Math.Sqrt(1.0f + right.x + upDirection.y + lookAt.z)*0.5f;
            float w4Recip = 1.0f/(4.0f*w);
            float x = (upDirection.z - lookAt.y)*w4Recip;
            float y = (lookAt.x-right.z)*w4Recip;
            float z = (right.y - upDirection.x)*w4Recip;
            var ret = new Quaternion(x,y,z,w);
            return ret;
        }

        /// <summary>
        /// Convert Quaternion to rotation matrix
        /// </summary>
        /// <param name="q">Quaternion to convert.</param>
        /// <returns>A matrix of type float4x4 from the passed Quaternion.</returns>
        public static float4x4 QuaternionToMatrix(Quaternion q)
        {
            q.Normalize();

            // be careful here: you might need a transposed matrix!
            return new float4x4
                       {
                           M11 = 1 - 2*(q.y*q.y + q.z*q.z),
                           M12 = 2*(q.x*q.y + q.z*q.w),
                           M13 = 2*(q.x*q.z - q.y*q.w),
                           M14 = 0,
                           M21 = 2*(q.x*q.y - q.z*q.w),
                           M22 = 1 - 2*(q.x*q.x + q.z*q.z),
                           M23 = 2*(q.z*q.y + q.x*q.w),
                           M24 = 0,
                           M31 = 2*(q.x*q.z + q.y*q.w),
                           M32 = 2*(q.y*q.z - q.x*q.w),
                           M33 = 1 - 2*(q.x*q.x + q.y*q.y),
                           M34 = 0,
                           M41 = 0,
                           M42 = 0,
                           M43 = 0,
                           M44 = 1
                       };
        }

        /// <summary>
        /// a with the algebraic sign of b.
        /// </summary>
        /// <remarks>Takes a as an absolute value and multiplies it with: +1 for any positiv number for b, -1 for any negative number for b or 0 for 0 for b.</remarks>
        /// <param name="a">Absolut value</param>
        /// <param name="b">A positiv/negativ number or zero.</param>
        /// <returns>Returns a with the algebraic sign of b.</returns>
        public static float CopySign(float a, float b)
        {
            return System.Math.Abs(a)*System.Math.Sign(b);
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            left.xyz += right.xyz;
            left.w += right.w;
            return left;
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            left.xyz -= right.xyz;
            left.w -= right.w;
            return left;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            Multiply(ref left, ref right, out left);
            return left;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion operator *(Quaternion quaternion, float scale)
        {
            Multiply(ref quaternion, scale, out quaternion);
            return quaternion;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion operator *(float scale, Quaternion quaternion)
        {
            return new Quaternion(quaternion.x * scale, quaternion.y * scale, quaternion.z * scale, quaternion.w * scale);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
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
        /// Returns a System.String that represents the current Quaternion.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return String.Format("V: {0}, w: {1}", xyz, w);
        }

        #endregion

        #region public override bool Equals (object o)

        /// <summary>
        /// Compares this object instance to another object for equality. 
        /// </summary>
        /// <param name="other">The other object to be used in the comparison.</param>
        /// <returns>True if both objects are Quaternions of equal value. Otherwise it returns false.</returns>
        public override bool Equals(object other)
        {
            if (other is Quaternion == false) return false;
               return this == (Quaternion)other;
        }

        #endregion

        #region public override int GetHashCode ()

        /// <summary>
        /// Provides the hash code for this object. 
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
        /// Compares this Quaternion instance to another Quaternion for equality. 
        /// </summary>
        /// <param name="other">The other Quaternion to be used in the comparison.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        public bool Equals(Quaternion other)
        {
            return xyz == other.xyz && (System.Math.Abs(w - other.w) < MathHelper.EpsilonFloat);
        }

        #endregion
    }
}
