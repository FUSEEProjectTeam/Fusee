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
        public float4x4 Vectors;
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
        public double4x4 Vectors;
    }
}