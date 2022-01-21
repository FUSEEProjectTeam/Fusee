using System.Linq;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Eigen data structure with values and vectors in double precision.
    /// </summary>
    public struct Eigen
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
        public double4x4 RotationMatrix => new(new double4(Vectors[0]), new double4(Vectors[1]), new double4(Vectors[2]), new double4(0, 0, 0, 1));

        /// <summary>
        /// Checks if two EigenD values are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => obj is Eigen e && e.Values.Equals(Values) && e.Vectors.Equals(Vectors);

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
        public static bool operator ==(Eigen left, Eigen right) => left.Equals(right);

        /// <summary>
        /// Checks if two EigenD values aren't the same
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Eigen left, Eigen right) => !(left == right);

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="vals"></param>
        public Eigen(double3[] vals)
        {
            var centeroid = M.CalculateCentroid(vals);
            var covarianceMatrix = M.CreateCovarianceMatrix(centeroid, vals);

            this = M.Diagonalizer(covarianceMatrix);
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="vals"></param>
        public Eigen(float3[] vals)
        {
            var valsD = vals.Select(val => (double3)val).ToArray();
            var centeroid = M.CalculateCentroid(valsD);
            var covarianceMatrix = M.CreateCovarianceMatrix(centeroid, valsD);

            this = M.Diagonalizer(covarianceMatrix);
        }
    }
}