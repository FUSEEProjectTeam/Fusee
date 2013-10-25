using System;
using System.Runtime.InteropServices;

namespace Fusee.Math
{
    /// <summary>
    /// Represents a QuaternionD (double precision).
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct QuaternionD : IEquatable<QuaternionD>
    {
        #region Fields

        double3 _xyz;
        double _w;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new QuaternionD from vector and w components
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public QuaternionD(double3 v, double w)
        {
            _xyz = v;
            _w = w;
        }

        /// <summary>
        /// Construct a new QuaternionD
        /// </summary>
        /// <param name="xx">The xx component</param>
        /// <param name="yy">The yy component</param>
        /// <param name="zz">The zz component</param>
        /// <param name="w">The w component</param>
        public QuaternionD(double xx, double yy, double zz, double w)
            : this(new double3(xx, yy, zz), w)
        { }

        #endregion

        #region Public Members

        #region Properties
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets an Fusee.Math.double3 with the x, y and z components of this instance.
        /// </summary>
        public double3 xyz { get { return _xyz; } set { _xyz = value; } }

        /// <summary>
        /// Gets or sets the x component of this instance.
        /// </summary>
        public double x { get { return _xyz.x; } set { _xyz.x = value; } }

        /// <summary>
        /// Gets or sets the y component of this instance.
        /// </summary>
         public double y { get { return _xyz.y; } set { _xyz.y = value; } }

        /// <summary>
        /// Gets or sets the z component of this instance.
        /// </summary>
        public double z { get { return _xyz.z; } set { _xyz.z = value; } }

        /// <summary>
        /// Gets or sets the w component of this instance.
        /// </summary>
        public double w { get { return _w; } set { _w = value; } }

        // ReSharper restore InconsistentNaming
        #endregion

        #region Instance

        #region ToAxisAngle

        /// <summary>
        /// Convert the current QuaternionD to axis angle representation
        /// </summary>
        /// <param name="axis">The resultant axis</param>
        /// <param name="angle">The resultant angle</param>
        public void ToAxisAngle(out double3 axis, out double angle)
        {
            double4 result = ToAxisAngle();
            axis = result.xyz;
            angle = result.w;
        }

        /// <summary>
        /// Convert this instance to an axis-angle representation.
        /// </summary>
        /// <returns>A double4 that is the axis-angle representation of this QuaternionD.</returns>
        public double4 ToAxisAngle()
        {
            QuaternionD q = this;

            if (q.w > 1.0f)
                q.Normalize();

            var result = new double4 {w = 2.0f*System.Math.Acos(q.w)};

            // angle
            var den = System.Math.Sqrt(1.0 - q.w*q.w);

            if (den > MathHelper.EpsilonDouble)
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

        #endregion

        #region public double Length

        /// <summary>
        /// Gets the length (magnitude) of the QuaternionD.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        public double Length
        {
            get
            {
                return System.Math.Sqrt(w * w + xyz.LengthSquared);
            }
        }

        #endregion

        #region public double LengthSquared

        /// <summary>
        /// Gets the square of the QuaternionD length (magnitude).
        /// </summary>
        public double LengthSquared
        {
            get
            {
                return w * w + xyz.LengthSquared;
            }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the QuaternionD to unit length.
        /// </summary>
        public void Normalize()
        {
            if (!(Length > MathHelper.EpsilonDouble)) return;
            var scale = 1.0f / Length;
            xyz *= scale;
            w *= scale;
        }

        #endregion

        #region public void Conjugate()

        /// <summary>
        /// Convert this QuaternionD to its conjugate
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
        /// Defines the identity QuaternionD.
        /// </summary>
        public static QuaternionD Identity = new QuaternionD(0, 0, 0, 1);

        #endregion

        #region Add

        /// <summary>
        /// Add two QuaternionDs
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

        /// <summary>
        /// Add two QuaternionDs
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <param name="result">The result of the addition</param>
        public static void Add(ref QuaternionD left, ref QuaternionD right, out QuaternionD result)
        {
            result = new QuaternionD(
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
        public static QuaternionD Sub(QuaternionD left, QuaternionD right)
        {
            return  new QuaternionD(
                left.xyz - right.xyz,
                left.w - right.w);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Sub(ref QuaternionD left, ref QuaternionD right, out QuaternionD result)
        {
            result = new QuaternionD(
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
        public static QuaternionD Mult(QuaternionD left, QuaternionD right)
        {
            return new QuaternionD(
                right.w * left.xyz + left.w * right.xyz + double3.Cross(left.xyz, right.xyz),
                left.w * right.w - double3.Dot(left.xyz, right.xyz));
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        [Obsolete("Use Multiply instead.")]
        public static void Mult(ref QuaternionD left, ref QuaternionD right, out QuaternionD result)
        {
            result = new QuaternionD(
                right.w * left.xyz + left.w * right.xyz + double3.Cross(left.xyz, right.xyz),
                left.w * right.w - double3.Dot(left.xyz, right.xyz));
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static QuaternionD Multiply(QuaternionD left, QuaternionD right)
        {
            QuaternionD result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref QuaternionD left, ref QuaternionD right, out QuaternionD result)
        {
            result = new QuaternionD(
                left.w * right.x + left.x * right.w + left.y * right.z - left.z * right.y,
                left.w * right.y + left.y * right.w + left.z * right.x - left.x * right.z,
                left.w * right.z + left.z * right.w + left.x * right.y - left.y * right.x,
                left.w * right.w - left.x * right.x - left.y * right.y - left.z * right.z);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternionD">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref QuaternionD quaternionD, double scale, out QuaternionD result)
        {
            result = new QuaternionD(quaternionD.x * scale, quaternionD.y * scale, quaternionD.z * scale, quaternionD.w * scale);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternionD">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static QuaternionD Multiply(QuaternionD quaternionD, double scale)
        {
            return new QuaternionD(quaternionD.x * scale, quaternionD.y * scale, quaternionD.z * scale, quaternionD.w * scale);
        }

        #endregion

        #region Conjugate

        /// <summary>
        /// Get the conjugate of the given QuaternionD
        /// </summary>
        /// <param name="q">The QuaternionD</param>
        /// <returns>The conjugate of the given QuaternionD</returns>
        public static QuaternionD Conjugate(QuaternionD q)
        {
            return new QuaternionD(-q.xyz, q.w);
        }

        /// <summary>
        /// Get the conjugate of the given QuaternionD
        /// </summary>
        /// <param name="q">The QuaternionD</param>
        /// <param name="result">The conjugate of the given QuaternionD</param>
        public static void Conjugate(ref QuaternionD q, out QuaternionD result)
        {
            result = new QuaternionD(-q.xyz, q.w);
        }

        #endregion

        #region Invert

        /// <summary>
        /// Get the inverse of the given QuaternionD
        /// </summary>
        /// <param name="q">The QuaternionD to invert</param>
        /// <returns>The inverse of the given QuaternionD</returns>
        public static QuaternionD Invert(QuaternionD q)
        {
            QuaternionD result;
            Invert(ref q, out result);
            return result;
        }

        /// <summary>
        /// Get the inverse of the given QuaternionD
        /// </summary>
        /// <param name="q">The QuaternionD to invert</param>
        /// <param name="result">The inverse of the given QuaternionD</param>
        public static void Invert(ref QuaternionD q, out QuaternionD result)
        {
            double lengthSq = q.LengthSquared;

            if (lengthSq > MathHelper.EpsilonDouble)
            {
                var i = 1.0f / lengthSq;
                result = new QuaternionD(q.xyz * -i, q.w * i);
            }
            else
            {
                result = q;
            }
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Scale the given QuaternionD to unit length
        /// </summary>
        /// <param name="q">The QuaternionD to normalize</param>
        /// <returns>The normalized QuaternionD</returns>
        public static QuaternionD Normalize(QuaternionD q)
        {
            QuaternionD result;
            Normalize(ref q, out result);
            return result;
        }

        /// <summary>
        /// Scale the given QuaternionD to unit length
        /// </summary>
        /// <param name="q">The QuaternionD to normalize</param>
        /// <param name="result">The normalized QuaternionD</param>
        public static void Normalize(ref QuaternionD q, out QuaternionD result)
        {
            double scale;

            if (!(q.Length > MathHelper.EpsilonFloat))
                scale = 0;
            else
                scale = 1.0 / q.Length;

            result = new QuaternionD(q.xyz * scale, q.w * scale);
        }

        #endregion

        #region FromAxisAngle

        /// <summary>
        /// Build a QuaternionD from the given axis and angle
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns>A QuaternionD that represents the orientation.</returns>
        public static QuaternionD FromAxisAngle(double3 axis, double angle)
        {
            if (axis.LengthSquared > MathHelper.EpsilonDouble)
                return Identity;

            var result = Identity;

            angle *= 0.5f;
            axis.Normalize();
            result.xyz = axis * System.Math.Sin(angle);
            result.w = System.Math.Cos(angle);

            return Normalize(result);
        }

        #endregion

        #region Slerp

        /// <summary>
        /// Do Spherical linear interpolation between two QuaternionDs 
        /// </summary>
        /// <param name="q1">The first QuaternionD</param>
        /// <param name="q2">The second QuaternionD</param>
        /// <param name="blend">The blend factor</param>
        /// <returns>A smooth blend between the given QuaternionDs</returns>
        public static QuaternionD Slerp(QuaternionD q1, QuaternionD q2, double blend)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared < MathHelper.EpsilonDouble)
                return (!(q2.LengthSquared > MathHelper.EpsilonFloat)) ? Identity : q2;

            if ((q2.LengthSquared < MathHelper.EpsilonDouble))
                return q1;

            var cosHalfAngle = q1.w * q2.w + double3.Dot(q1.xyz, q2.xyz);

            // if angle = 0.0f, just return one input.
            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
                return q1;

            if (cosHalfAngle < 0.0f)
            {
                q2.xyz = -q2.xyz;
                q2.w = -q2.w;
                cosHalfAngle = -cosHalfAngle;
            }

            double blendA;
            double blendB;

            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                var halfAngle = System.Math.Acos(cosHalfAngle);
                var sinHalfAngle = System.Math.Sin(halfAngle);
                var oneOverSinHalfAngle = 1.0f / sinHalfAngle;

                blendA = System.Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = System.Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            var result = new QuaternionD(blendA * q1.xyz + blendB * q2.xyz, blendA * q1.w + blendB * q2.w);

            return result.LengthSquared > MathHelper.EpsilonDouble ? Normalize(result) : Identity;
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
        public static QuaternionD operator +(QuaternionD left, QuaternionD right)
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
        public static QuaternionD operator -(QuaternionD left, QuaternionD right)
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
        public static QuaternionD operator *(QuaternionD left, QuaternionD right)
        {
            Multiply(ref left, ref right, out left);
            return left;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternionD">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static QuaternionD operator *(QuaternionD quaternionD, double scale)
        {
            Multiply(ref quaternionD, scale, out quaternionD);
            return quaternionD;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternionD">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static QuaternionD operator *(double scale, QuaternionD quaternionD)
        {
            return new QuaternionD(quaternionD.x * scale, quaternionD.y * scale, quaternionD.z * scale, quaternionD.w * scale);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(QuaternionD left, QuaternionD right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(QuaternionD left, QuaternionD right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current QuaternionD.
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
        /// <returns>True if both objects are QuaternionDs of equal value. Otherwise it returns false.</returns>
        public override bool Equals(object other)
        {
            if (other is QuaternionD == false) return false;
               return this == (QuaternionD)other;
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

        #region IEquatable<QuaternionD> Members

        /// <summary>
        /// Compares this QuaternionD instance to another QuaternionD for equality. 
        /// </summary>
        /// <param name="other">The other QuaternionD to be used in the comparison.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        public bool Equals(QuaternionD other)
        {
            return xyz == other.xyz && (System.Math.Abs(w - other.w) < MathHelper.EpsilonDouble);
        }

        #endregion
    }
}
