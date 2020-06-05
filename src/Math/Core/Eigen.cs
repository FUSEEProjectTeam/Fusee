using System.Numerics;
using System;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Eigen data structure with values and vectors in single precision.
    /// </summary>
    public struct EigenF
    {
        /// <summary>
        /// Eigen values.
        /// </summary>
        public float[] Values;

        /// <summary>
        /// Eigen vectors.
        /// </summary>
        public float3[] Vectors;

        /// <summary>
        /// Generates the rotation matrix from eigen vectors
        /// </summary>
        public float4x4 RotationMatrix => new float4x4(new float4(Vectors[0]), new float4(Vectors[1]), new float4(Vectors[2]), new float4(0, 0, 0, 1));

        /// <summary>
        /// Checks if two EigenF values are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is EigenF e && e.Values.Equals(Values) && e.Vectors.Equals(Vectors);

        /// <summary>
        /// Returns hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Values.GetHashCode() + Vectors.GetHashCode();

        /// <summary>
        /// Checks if two EigenF values are the same
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(EigenF left, EigenF right) => left.Equals(right);

        /// <summary>
        /// Checks if two EigenF values aren't the same
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(EigenF left, EigenF right) => !(left == right);

    }

    /// <summary>
    /// Eigen data structure with values and vectors in double precision.
    /// </summary>
    public struct EigenD
    {
        /// <summary>
        /// Eigen values.
        /// </summary>
        public double[] Values;

        /// <summary>
        /// Eigen vectors.
        /// </summary>
        public double3[] Vectors;

        /// <summary>
        /// Generates the rotation matrix from eigen vectors
        /// </summary>
        public double4x4 RotationMatrix => new double4x4(new double4(Vectors[0]), new double4(Vectors[1]), new double4(Vectors[2]), new double4(0, 0, 0, 1));


        /// <summary>
        /// Checks if two EigenD values are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is EigenD e && e.Values.Equals(Values) && e.Vectors.Equals(Vectors);

        /// <summary>
        /// Returns hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Values.GetHashCode() + Vectors.GetHashCode();

        /// <summary>
        /// Checks if two EigenD values are the same
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(EigenD left, EigenD right) => left.Equals(right);

        /// <summary>
        /// Checks if two EigenD values aren't the same
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(EigenD left, EigenD right) => !(left == right);
    }
}